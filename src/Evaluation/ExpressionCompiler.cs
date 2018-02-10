using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Exp = System.Linq.Expressions.Expression;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace xnaMugen.Evaluation
{
    internal class ExpressionCompiler : ICompiler
    {
        private class CompilerState
        {
            public ILGenerator Generator { get; set; }
            public ParameterExpression FunctionState { get; set; }
            public LabelTarget ErrorLabel { get; set; }
            public Exp ErrorVariable { get; set; }
        }

        public ExpressionCompiler()
        {
            m_constfunctionmethods = new Dictionary<string, MethodInfo>(StringComparer.InvariantCultureIgnoreCase);
            m_gethitvarmethods = new Dictionary<string, MethodInfo>(StringComparer.InvariantCultureIgnoreCase);

            BuildFunctionMap();
        }

        public EvaluationCallback Create(Node node)
        {
            var compilerstate = new CompilerState
            {
                FunctionState = Exp.Parameter(typeof(object)),
                ErrorVariable = Exp.Constant(false),
                ErrorLabel = Exp.Label()
            };
            var result = Make(compilerstate, node);
            if (result.Type == typeof(bool))
            {
                result = ToInteger(result);
            }
            if (result.Type == typeof(int) || result.Type == typeof(float))
            {
                // if result ok
                var constructor = typeof(Number).GetConstructor(new[] { result.Type });
                result = Exp.New(typeof(Number).GetConstructor(new[] { result.Type }), new[] { result });
                var resultFailed = Exp.New(typeof(Number).GetConstructor(new[] { typeof(int) }), new[] { Exp.Constant(200) });

                result = Exp.TryCatch(result, Exp.Catch(typeof(Exception), resultFailed));

                var func = Exp.Lambda<Func<object, Number>>(result, compilerstate.FunctionState).Compile();
                return new EvaluationCallback(func);
            }
            throw new Exception();
        }

        private void BuildFunctionMap()
        {
            foreach (var method in typeof(Triggers.Const).GetMethods())
            {
                var tag = (TagAttribute)Attribute.GetCustomAttribute(method, typeof(TagAttribute));
                if (tag == null) continue;

                m_constfunctionmethods.Add(tag.Value, method);
            }

            foreach (var method in typeof(Triggers.GetHitVar).GetMethods())
            {
                var tag = (TagAttribute)Attribute.GetCustomAttribute(method, typeof(TagAttribute));
                if (tag == null) continue;

                m_gethitvarmethods.Add(tag.Value, method);
            }
        }

        private Exp Make(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            if (node.Token.Data is Tokenizing.NumberData)
            {
                return MakeNumber(state, node);
            }

            if (node.Token.Data is Tokenizing.OperatorData)
            {
                return MakeOperator(state, node);
            }

            if (node.Token.Data is Tokenizing.StateRedirectionData)
            {
                return MakeStateRedirection(state, node);
            }

            if (node.Token.Data is Tokenizing.CustomFunctionData)
            {
                return MakeFunction(state, node);
            }

            if (node.Token.Data is Tokenizing.RangeData)
            {
                return MakeRange(state, node);
            }

            throw new Exception();
        }

        private Exp MakeMethod(CompilerState state, MethodInfo method, List<Exp> args)
        {
            if (state == null) throw new Exception();
            if (method == null) throw new Exception();
            if (args == null) throw new Exception();

            var parameters = method.GetParameters();

            for (var i = 0; i != args.Count; ++i)
            {
                if (parameters[i].ParameterType == typeof(float) && args[i].Type != typeof(float))
                {
                    args[i] = ToFloat(args[i]);
                }

                if (parameters[i].ParameterType == typeof(double) && args[i].Type != typeof(double))
                {
                    args[i] = ToDouble(args[i]);
                }
            }

            var result = Exp.Call(null, method, args);
            if (result.Type == typeof(bool))
            {
                return ToInteger(result);
            }
            if (result.Type == typeof(double))
            {
                return ToFloat(result);
            }
            return result;
        }

        private Exp MakeRange(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var data = node.Token.Data as Tokenizing.RangeData;
            if (data == null) throw new Exception();

            if (node.Children.Count != 3) throw new Exception();
            if (node.Arguments.Count != 3) throw new Exception();

            var args = MakeDescendants(state, node);

            var argtypes = new Type[args.Count];
            for (var i = 0; i != args.Count; ++i) argtypes[i] = args[i].Type;

            var method = typeof(SpecialFunctions).GetMethod("Range", argtypes);
            var parameters = method.GetParameters();
            for (var i = 0; i != args.Count; ++i)
            {
                if (parameters[i].ParameterType != args[i].Type)
                {
                    args[i] = Exp.Convert(args[i], parameters[i].ParameterType);
                }
            }
            return Exp.Call(null, method, args);
        }

        private Exp MakeStateRedirection(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var data = node.Token.Data as Tokenizing.StateRedirectionData;
            if (data == null) throw new Exception();

            if (node.Children.Count < 1) throw new Exception();

            var functionargs = MakeRedirectionDescendants(state, node);
            var method = FindCorrectRedirectionMethod(data.Type, functionargs);

            functionargs.Insert(0, state.FunctionState);
            functionargs.Insert(1, state.ErrorVariable);
            var callExp = Exp.Call(null, method, functionargs);
            var oldState = state.FunctionState;
            state.FunctionState = Exp.Parameter(typeof(object));
            var lambdaExp = Exp.Lambda(Make(state, node.Children[node.Children.Count - 1]), state.FunctionState);
            state.FunctionState = oldState;
            return Exp.Invoke(lambdaExp, callExp);
        }

        private Exp MakeFunction(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var data = node.Token.Data as Tokenizing.CustomFunctionData;
            if (data == null) throw new Exception();

            if (data.Type == typeof(Triggers.Const) || data.Type == typeof(Triggers.GetHitVar))
            {
                return MakeSpecialFunction(state, data.Type, (string)node.Arguments[0]);
            }

            var functionargs = MakeDescendants(state, node);
            var method = FindCorrectMethod(data.Type, functionargs);
            var parameters = method.GetParameters();

            functionargs.Insert(0, state.FunctionState);
            functionargs.Insert(1, state.ErrorVariable);

            var methodParamTypes = method.GetParameters().Select(o => o.ParameterType).ToList();

            for (int i = 2; i < methodParamTypes.Count; i++)
            {
                var paramType = methodParamTypes[i];
                var argType = functionargs[i].Type;

                if (paramType == typeof(int) && argType == typeof(bool))
                {
                    functionargs[i] = Exp.Condition(functionargs[i], Exp.Constant(1), Exp.Constant(0));
                }
                else if (paramType != argType)
                {
                    functionargs[i] = Exp.Convert(functionargs[i], paramType);
                }
            }

            var result = Exp.Call(null, method, functionargs);
            return result;
        }

        private Exp MakeSpecialFunction(CompilerState state, Type type, string constant)
        {
            if (state == null) throw new Exception();
            if (type == null) throw new Exception();
            if (constant == null) throw new Exception();

            Dictionary<string, MethodInfo> methodmap;
            if (type == typeof(Triggers.Const)) methodmap = m_constfunctionmethods;
            else if (type == typeof(Triggers.GetHitVar)) methodmap = m_gethitvarmethods;
            else throw new Exception();

            MethodInfo method;
            if (methodmap.TryGetValue(constant, out method) == false) throw new Exception();

            var args = new List<Exp> { state.FunctionState, state.ErrorVariable };
            return Exp.Call(null, method, args);
        }

        private Exp MakeOperator(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var data = node.Token.Data as Tokenizing.OperatorData;
            if (data == null) throw new Exception();

            if (node.Children.Count == 1) return MakeUnary(state, node);

            if (node.Children.Count != 2) throw new Exception();

            if (data.Operator == Operator.Assignment) return MakeAssignment(state, node);

            var lhs = Make(state, node.Children[0]);
            var rhs = Make(state, node.Children[1]);

            switch (data.Operator)
            {
                case Operator.Plus:
                    return MakeBinary(state, ExpressionType.Add, lhs, rhs);

                case Operator.Minus:
                    return MakeBinary(state, ExpressionType.Subtract, lhs, rhs);

                case Operator.Divide:
                    return MakeBinary(state, ExpressionType.Divide, lhs, rhs);

                case Operator.Multiply:
                    return MakeBinary(state, ExpressionType.Multiply, lhs, rhs);

                case Operator.Modulus:
                    return MakeBinary(state, ExpressionType.Modulo, lhs, rhs);

                case Operator.Equals:
                    return MakeBinary(state, ExpressionType.Equal, lhs, rhs);
                case Operator.NotEquals:
                    return MakeBinary(state, ExpressionType.NotEqual, lhs, rhs);
                case Operator.Greater:
                    return MakeBinary(state, ExpressionType.GreaterThan, lhs, rhs);
                case Operator.GreaterEquals:
                    return MakeBinary(state, ExpressionType.GreaterThanOrEqual, lhs, rhs);
                case Operator.Lesser:
                    return MakeBinary(state, ExpressionType.LessThan, lhs, rhs);
                case Operator.LesserEquals:
                    return MakeBinary(state, ExpressionType.LessThanOrEqual, lhs, rhs);

                case Operator.LogicalAnd:
                    return MakeBinary(state, ExpressionType.And, lhs, rhs);
                case Operator.LogicalOr:
                    return MakeBinary(state, ExpressionType.Or, lhs, rhs);
                case Operator.LogicalXor:
                    return MakeBinary(state, ExpressionType.ExclusiveOr, lhs, rhs);

                case Operator.Exponent:
                    return MakeMethod(state, typeof(Math).GetMethod("Pow"), new List<Exp> { lhs, rhs });

                default:
                    throw new Exception();
            }
        }

        private Exp MakeAssignment(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var data = node.Token.Data as Tokenizing.OperatorData;
            if (data == null || data.Operator != Operator.Assignment) throw new Exception();

            if (node.Children.Count != 2) throw new Exception();

            var vardata = node.Children[0].Token.Data as Tokenizing.CustomFunctionData;
            if (vardata == null) throw new Exception();

            if (node.Children[0].Children.Count != 1) throw new Exception();

            var index = Make(state, node.Children[0].Children[0]);
            var value = Make(state, node.Children[1]);

            var args = new List<Exp> { Exp.Constant(state.FunctionState), index, value };

            if (vardata.Type == typeof(Triggers.Var))
            {
                return MakeMethod(state, typeof(SpecialFunctions).GetMethod("Assignment_Var"), args);
            }

            if (vardata.Type == typeof(Triggers.FVar))
            {
                return MakeMethod(state, typeof(SpecialFunctions).GetMethod("Assignment_FVar"), args);
            }

            if (vardata.Type == typeof(Triggers.SysVar))
            {
                return MakeMethod(state, typeof(SpecialFunctions).GetMethod("Assignment_SysVar"), args);
            }

            if (vardata.Type == typeof(Triggers.SysFVar))
            {
                return MakeMethod(state, typeof(SpecialFunctions).GetMethod("Assignment_SysFVar"), args);
            }

            throw new Exception();
        }

        private Exp MakeBinary(CompilerState state, ExpressionType op, Exp lhs, Exp rhs)
        {
            if (state == null) throw new Exception();
            if (lhs == null) throw new Exception();
            if (rhs == null) throw new Exception();

            Transform(ref lhs, ref rhs);

            if (op == ExpressionType.Divide) CheckIfZero(state, rhs);

            switch (op)
            {
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.ExclusiveOr:
                    lhs = ToBoolean(lhs);
                    rhs = ToBoolean(rhs);
                    break;
            }

            return Exp.MakeBinary(op, lhs, rhs);
        }

        private static void Transform(ref Exp lhs, ref Exp rhs)
        {
            if (lhs.Type == typeof(bool))
            {
                lhs = Exp.Condition(lhs, Exp.Constant(1), Exp.Constant(0));
            }
            if (rhs.Type == typeof(bool))
            {
                rhs = Exp.Condition(rhs, Exp.Constant(1), Exp.Constant(0));
            }
            if (lhs.Type == typeof(float) && rhs.Type == typeof(int))
            {
                rhs = Exp.Convert(rhs, typeof(float));
            }
            if (lhs.Type == typeof(int) && rhs.Type == typeof(float))
            {
                lhs = Exp.Convert(lhs, typeof(float));
            }
        }

        private Exp MakeUnary(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var data = node.Token.Data as Tokenizing.OperatorData;
            if (data == null) throw new Exception();

            if (node.Children.Count != 1) throw new Exception();

            if (data.Operator == Operator.Minus)
            {
                var value = Make(state, node.Children[0]);
                return Exp.MakeUnary(ExpressionType.Negate, value, null);
            }

            if (data.Operator == Operator.LogicalNot)
            {
                var value = Make(state, node.Children[0]);
                return Exp.Not(ToBoolean(value));
            }

            throw new Exception();
        }

        private Exp MakeNumber(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var data = node.Token.Data as Tokenizing.NumberData;
            if (data == null) throw new Exception();

            var number = data.GetNumber(node.Token.ToString());

            if (number.NumberType == NumberType.Int)
            {
                return Exp.Constant(number.IntValue);
            }

            if (number.NumberType == NumberType.Float)
            {
                return Exp.Constant(number.FloatValue);
            }

            throw new Exception();
        }

        private Exp ToBoolean(Exp expression)
        {
            if (expression.Type == typeof(bool)) return expression;
            if (expression.Type == typeof(int))
            {
                return Exp.MakeBinary(ExpressionType.Equal, expression, Exp.Constant(1));
            }
            if (expression.Type == typeof(float))
            {
                return Exp.MakeBinary(ExpressionType.NotEqual, expression, Exp.Constant(0.0f));
            }
            throw new Exception("Cannot convert expression to boolean");
        }

        private Exp ToInteger(Exp expression)
        {
            if (expression.Type == typeof(int)) return expression;

            if (expression.Type == typeof(bool))
            {
                expression = Exp.Condition(expression, Exp.Constant(1), Exp.Constant(0));
            }
            return Exp.Convert(expression, typeof(int));
        }

        private Exp ToFloat(Exp expression)
        {
            if (expression.Type == typeof(float)) return expression;

            if (expression.Type == typeof(bool))
            {
                expression = Exp.Condition(expression, Exp.Constant(1.0f), Exp.Constant(0.0f));
            }
            return Exp.Convert(expression, typeof(float));
        }

        private Exp ToDouble(Exp expression)
        {
            if (expression.Type == typeof(double)) return expression;

            if (expression.Type == typeof(bool))
            {
                expression = Exp.Condition(expression, Exp.Constant(1.0), Exp.Constant(0.0));
            }
            return Exp.Convert(expression, typeof(double));
        }

        private MethodInfo FindCorrectMethod(Type functiontype, List<Exp> args)
        {
            if (functiontype == null) throw new Exception();
            if (args == null) throw new Exception();

            var argtypes = new Type[args.Count + 2];
            argtypes[0] = typeof(object);
            argtypes[1] = typeof(bool).MakeByRefType();
            for (var i = 0; i != args.Count; ++i) argtypes[i + 2] = args[i].Type;
            var info = functiontype.GetMethod("Evaluate", argtypes);
            if (info != null) return info;
            for (int i = 2; i < argtypes.Length; i++)
            {
                if (argtypes[i] == typeof(bool))
                {
                    argtypes[i] = typeof(int);
                }
            }
            info = functiontype.GetMethod("Evaluate", argtypes);
            if (info != null) return info;

            throw new Exception();
        }

        private MethodInfo FindCorrectRedirectionMethod(Type functiontype, List<Exp> args)
        {
            if (functiontype == null) throw new Exception();
            if (args == null) throw new Exception();

            var argtypes = new Type[args.Count + 2];
            argtypes[0] = typeof(object);
            argtypes[1] = typeof(bool).MakeByRefType();
            for (var i = 0; i != args.Count; ++i) argtypes[i + 2] = args[i].Type;

            var info = functiontype.GetMethod("RedirectState", argtypes);
            if (info != null) return info;

            throw new Exception();
        }

        private List<Exp> MakeDescendants(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var output = new List<Exp>();

            foreach (var child in node.Children) output.Add(Make(state, child));

            foreach (var arg in node.Arguments)
            {
                if (arg is string)
                {
                    output.Add(Exp.Constant(arg));
                }
                else if (arg.GetType().IsEnum)
                {
                    output.Add(Exp.Constant(arg));
                }
                else if (arg is Combat.HitType[])
                {
                    output.Add(Exp.Constant(arg));
                }
                else
                {
                    throw new Exception();
                }
            }

            return output;
        }

        private List<Exp> MakeRedirectionDescendants(CompilerState state, Node node)
        {
            if (state == null) throw new Exception();
            if (node == null) throw new Exception();

            var output = new List<Exp>();

            for (var i = 0; i < node.Children.Count - 1; ++i) output.Add(Make(state, node.Children[i]));

            foreach (var arg in node.Arguments)
            {
                if (arg is string)
                {
                    output.Add(Exp.Constant(arg));
                }
                else if (arg.GetType().IsEnum)
                {
                    output.Add(Exp.Constant(arg));
                }
                else
                {
                    throw new Exception();
                }
            }

            return output;
        }

        private Exp CheckIfZero(CompilerState state, Exp expression)
        {
            if (state == null) throw new Exception();
            if (expression == null) throw new Exception();

            if (expression.Type == typeof(int))
            {
                return Exp.Lambda(Exp.Block(
                    Exp.IfThen(Exp.MakeBinary(ExpressionType.Equal, expression, Exp.Constant(0)), Exp.Goto(state.ErrorLabel)),
                    expression));
            }

            if (expression.Type == typeof(float))
            {
                return Exp.Lambda(Exp.Block(
                    Exp.IfThen(Exp.MakeBinary(ExpressionType.Equal, expression, Exp.Constant(0.0f)), Exp.Goto(state.ErrorLabel)),
                    expression));
            }
            return expression;
        }

        private readonly Dictionary<string, MethodInfo> m_constfunctionmethods;

        private readonly Dictionary<string, MethodInfo> m_gethitvarmethods;
    }
}