using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	class Compiler
	{
		class CompilerState
		{
			public ILGenerator Generator { get; set; }
			public LocalVariableInfo FunctionState { get; set; }
			public Label ErrorLabel { get; set; }
			public LocalVariableInfo ErrorVariable { get; set; }
		}

		public Compiler()
		{
			AssemblyBuilder newAssembly = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.Run);
			m_module = newAssembly.DefineDynamicModule("Module");

			m_constfunctionmethods = new Dictionary<String, MethodInfo>(StringComparer.InvariantCultureIgnoreCase);
			m_gethitvarmethods = new Dictionary<String, MethodInfo>(StringComparer.InvariantCultureIgnoreCase);

			BuildFunctionMap();
		}

		void BuildFunctionMap()
		{
			m_constfunctionmethods.Clear();
			m_gethitvarmethods.Clear();

			foreach (MethodInfo method in typeof(Triggers.Const).GetMethods())
			{
				TagAttribute tag = (TagAttribute)Attribute.GetCustomAttribute(method, typeof(TagAttribute));
				if (tag == null) continue;

				m_constfunctionmethods.Add(tag.Value, method);
			}

			foreach (MethodInfo method in typeof(Triggers.GetHitVar).GetMethods())
			{
				TagAttribute tag = (TagAttribute)Attribute.GetCustomAttribute(method, typeof(TagAttribute));
				if (tag == null) continue;

				m_gethitvarmethods.Add(tag.Value, method);
			}
		}

		public EvaluationCallback Create(Node node)
		{
			DynamicMethod method = new DynamicMethod(String.Empty, typeof(Number), new Type[] { typeof(Object) }, typeof(Compiler), true);

			CompilerState compilerstate = new CompilerState();
			compilerstate.Generator = method.GetILGenerator();
			compilerstate.FunctionState = compilerstate.Generator.DeclareLocal(typeof(Object));
			compilerstate.ErrorLabel = compilerstate.Generator.DefineLabel();
			compilerstate.ErrorVariable = compilerstate.Generator.DeclareLocal(typeof(Boolean));

			compilerstate.Generator.Emit(OpCodes.Ldarg, 0);
			StoreLocalVariable(compilerstate, compilerstate.FunctionState);

			LocalVariableInfo result = Emit(compilerstate, node);

			if (result.LocalType == typeof(Int32))
			{
				LoadLocalVariable(compilerstate, result);
				compilerstate.Generator.Emit(OpCodes.Newobj, typeof(Number).GetConstructor(new Type[] { typeof(Int32) }));
			}
			else if (result.LocalType == typeof(Single))
			{
				LoadLocalVariable(compilerstate, result);
				compilerstate.Generator.Emit(OpCodes.Newobj, typeof(Number).GetConstructor(new Type[] { typeof(Single) }));
			}
			else
			{
				throw new Exception();
			}

			compilerstate.Generator.Emit(OpCodes.Ret);

			compilerstate.Generator.MarkLabel(compilerstate.ErrorLabel);
			LoadLocalVariable(compilerstate, compilerstate.Generator.DeclareLocal(typeof(Number)));
			compilerstate.Generator.Emit(OpCodes.Ret);

			EvaluationCallback callback = (EvaluationCallback)method.CreateDelegate(typeof(EvaluationCallback));
			return callback;
		}

		LocalVariableInfo Emit(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			if (node.Token.Data is Evaluation.Tokenizing.NumberData)
			{
				return EmitNumber(state, node);
			}

			if (node.Token.Data is Evaluation.Tokenizing.OperatorData)
			{
				return EmitOperator(state, node);
			}

			if (node.Token.Data is Evaluation.Tokenizing.StateRedirectionData)
			{
				return EmitStateRedirection(state, node);
			}

			if (node.Token.Data is Evaluation.Tokenizing.CustomFunctionData)
			{
				return EmitFunction(state, node);
			}

			if (node.Token.Data is Evaluation.Tokenizing.RangeData)
			{
				return EmitRange(state, node);
			}

			throw new Exception();
		}

		LocalVariableInfo EmitMethod(CompilerState state, MethodInfo method, List<LocalVariableInfo> args)
		{
			if (state == null) throw new Exception();
			if (method == null) throw new Exception();
			if (args == null) throw new Exception();

			ParameterInfo[] parameters = method.GetParameters();

			for (Int32 i = 0; i != args.Count; ++i)
			{
				LoadLocalVariable(state, args[i]);

				if (parameters[i].ParameterType == typeof(Single) && args[i].LocalType != typeof(Single))
				{
					state.Generator.Emit(OpCodes.Conv_R4);
				}

				if (parameters[i].ParameterType == typeof(Double) && args[i].LocalType != typeof(Double))
				{
					state.Generator.Emit(OpCodes.Conv_R8);
				}
			}

			state.Generator.Emit(OpCodes.Call, method);

			Type returntype = method.ReturnType;
			if (returntype == typeof(Boolean))
			{
				returntype = typeof(Int32);
			}
			else if (returntype == typeof(Double))
			{
				state.Generator.Emit(OpCodes.Conv_R4);
				returntype = typeof(Single);
			}

			LocalVariableInfo result = state.Generator.DeclareLocal(returntype);
			StoreLocalVariable(state, result);

			LoadLocalVariable(state, state.ErrorVariable);
			state.Generator.Emit(OpCodes.Brtrue, state.ErrorLabel);

			return result;
		}

		LocalVariableInfo EmitRange(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			Evaluation.Tokenizing.RangeData data = node.Token.Data as Evaluation.Tokenizing.RangeData;
			if (data == null) throw new Exception();

			if (node.Children.Count != 3) throw new Exception();
			if (node.Arguments.Count != 3) throw new Exception();

			List<LocalVariableInfo> args = EmitDescendants(state, node);

			Type[] argtypes = new Type[args.Count];
			for (Int32 i = 0; i != args.Count; ++i) argtypes[i] = args[i].LocalType;

			MethodInfo method = typeof(SpecialFunctions).GetMethod("Range", argtypes);

			return EmitMethod(state, method, args);
		}

		LocalVariableInfo EmitStateRedirection(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			Evaluation.Tokenizing.StateRedirectionData data = node.Token.Data as Evaluation.Tokenizing.StateRedirectionData;
			if (data == null) throw new Exception();

			if (node.Children.Count < 1) throw new Exception();

			List<LocalVariableInfo> functionargs = EmitRedirectionDescendants(state, node);
			MethodInfo method = FindCorrectRedirectionMethod(data.Type, functionargs);
			ParameterInfo[] parameters = method.GetParameters();

			LoadLocalVariable(state, state.FunctionState);
			state.Generator.Emit(OpCodes.Ldloca, state.ErrorVariable.LocalIndex);

			for (Int32 i = 0; i != functionargs.Count; ++i)
			{
				LocalVariableInfo arg = functionargs[i];
				ParameterInfo parameter = parameters[i + 2];

				LoadLocalVariable(state, arg);

				//if (parameter.ParameterType == typeof(Int32) && arg.LocalType == typeof(Single)) state.Generator.Emit(OpCodes.Conv_I4);
				//if (parameter.ParameterType == typeof(Single) && arg.LocalType == typeof(Int32)) state.Generator.Emit(OpCodes.Conv_R4);
			}

			state.Generator.Emit(OpCodes.Call, method);

			LocalVariableInfo oldstate = state.FunctionState;

			state.FunctionState = state.Generator.DeclareLocal(typeof(Object));
			StoreLocalVariable(state, state.FunctionState);

			LoadLocalVariable(state, state.ErrorVariable);
			state.Generator.Emit(OpCodes.Brtrue, state.ErrorLabel);

			LocalVariableInfo returnvalue = Emit(state, node.Children[node.Children.Count - 1]);

			state.FunctionState = oldstate;
			return returnvalue;
		}

		LocalVariableInfo EmitFunction(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			Evaluation.Tokenizing.CustomFunctionData data = node.Token.Data as Evaluation.Tokenizing.CustomFunctionData;
			if (data == null) throw new Exception();

			if (data.Type == typeof(Triggers.Const) || data.Type == typeof(Triggers.GetHitVar))
			{
				return EmitSpecialFunction(state, data.Type, (String)node.Arguments[0]);
			}

			List<LocalVariableInfo> functionargs = EmitDescendants(state, node);
			MethodInfo method = FindCorrectMethod(data.Type, functionargs);
			ParameterInfo[] parameters = method.GetParameters();

			LoadLocalVariable(state, state.FunctionState);
			state.Generator.Emit(OpCodes.Ldloca, state.ErrorVariable.LocalIndex);

			for (Int32 i = 0; i != functionargs.Count; ++i)
			{
				LocalVariableInfo arg = functionargs[i];
				ParameterInfo parameter = parameters[i + 2];

				LoadLocalVariable(state, arg);

				//if (parameter.ParameterType == typeof(Int32) && arg.LocalType == typeof(Single)) state.Generator.Emit(OpCodes.Conv_I4);
				//if (parameter.ParameterType == typeof(Single) && arg.LocalType == typeof(Int32)) state.Generator.Emit(OpCodes.Conv_R4);
			}

			state.Generator.Emit(OpCodes.Call, method);

			Type returntype = method.ReturnType;
			if (returntype == typeof(Boolean)) returntype = typeof(Int32);

			LocalVariableInfo result = state.Generator.DeclareLocal(returntype);
			StoreLocalVariable(state, result);

			LoadLocalVariable(state, state.ErrorVariable);
			state.Generator.Emit(OpCodes.Brtrue, state.ErrorLabel);

			return result;
		}

		LocalVariableInfo EmitSpecialFunction(CompilerState state, Type type, String constant)
		{
			if (state == null) throw new Exception();
			if (type == null) throw new Exception();
			if (constant == null) throw new Exception();

			Dictionary<String, MethodInfo> methodmap;
			if (type == typeof(Triggers.Const)) methodmap = m_constfunctionmethods;
			else if (type == typeof(Triggers.GetHitVar)) methodmap = m_gethitvarmethods;
			else throw new Exception();

			MethodInfo method;
			if (methodmap.TryGetValue(constant, out method) == false) throw new Exception();

			LoadLocalVariable(state, state.FunctionState);
			state.Generator.Emit(OpCodes.Ldloca, state.ErrorVariable.LocalIndex);

			state.Generator.Emit(OpCodes.Call, method);

			Type returntype = method.ReturnType;
			if (returntype == typeof(Boolean)) returntype = typeof(Int32);

			LocalVariableInfo result = state.Generator.DeclareLocal(returntype);
			StoreLocalVariable(state, result);

			LoadLocalVariable(state, state.ErrorVariable);
			state.Generator.Emit(OpCodes.Brtrue, state.ErrorLabel);

			return result;
		}

		LocalVariableInfo EmitOperator(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			Evaluation.Tokenizing.OperatorData data = node.Token.Data as Evaluation.Tokenizing.OperatorData;
			if (data == null) throw new Exception();

			if (node.Children.Count == 1) return EmitUnaryOperator(state, node);

			if (node.Children.Count != 2) throw new Exception();

			if (data.Operator == Operator.Assignment) return EmitAssignmentOperator(state, node);

			LocalVariableInfo lhs = Emit(state, node.Children[0]);
			LocalVariableInfo rhs = Emit(state, node.Children[1]);

			switch (data.Operator)
			{
				case Operator.Plus:
					return EmitArithmeticOperator(state, OpCodes.Add, lhs, rhs);

				case Operator.Minus:
					return EmitArithmeticOperator(state, OpCodes.Sub, lhs, rhs);

				case Operator.Divide:
					return EmitArithmeticOperator(state, OpCodes.Div, lhs, rhs);

				case Operator.Multiply:
					return EmitArithmeticOperator(state, OpCodes.Mul, lhs, rhs);

				case Operator.Modulus:
					return EmitArithmeticOperator(state, OpCodes.Rem, lhs, rhs);

				case Operator.Equals:
				case Operator.NotEquals:
				case Operator.Greater:
				case Operator.GreaterEquals:
				case Operator.Lesser:
				case Operator.LesserEquals:
					return EmitComparsionOperator(state, data.Operator, lhs, rhs);

				case Operator.LogicalAnd:
				case Operator.LogicalOr:
				case Operator.LogicalXor:
					return EmitLogicalOperator(state, data.Operator, lhs, rhs);

				case Operator.Exponent:
					return EmitMethod(state, typeof(Math).GetMethod("Pow"), new List<LocalVariableInfo>() { lhs, rhs });

				default:
					throw new Exception();
			}
		}

		LocalVariableInfo EmitAssignmentOperator(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			Evaluation.Tokenizing.OperatorData data = node.Token.Data as Evaluation.Tokenizing.OperatorData;
			if (data == null || data.Operator != Operator.Assignment) throw new Exception();

			if (node.Children.Count != 2) throw new Exception();

			Evaluation.Tokenizing.CustomFunctionData vardata = node.Children[0].Token.Data as Evaluation.Tokenizing.CustomFunctionData;
			if (vardata == null) throw new Exception();

			if (node.Children[0].Children.Count != 1) throw new Exception();

			LocalVariableInfo index = Emit(state, node.Children[0].Children[0]);
			LocalVariableInfo value = Emit(state, node.Children[1]);

			List<LocalVariableInfo> args = new List<LocalVariableInfo>() { state.FunctionState, index, value };

			if (vardata.Type == typeof(Triggers.Var))
			{
				return EmitMethod(state, typeof(SpecialFunctions).GetMethod("Assignment_Var"), args);
			}

			if (vardata.Type == typeof(Triggers.FVar))
			{
				return EmitMethod(state, typeof(SpecialFunctions).GetMethod("Assignment_FVar"), args);
			}

			if (vardata.Type == typeof(Triggers.SysVar))
			{
				return EmitMethod(state, typeof(SpecialFunctions).GetMethod("Assignment_SysVar"), args);
			}

			if (vardata.Type == typeof(Triggers.SysFVar))
			{
				return EmitMethod(state, typeof(SpecialFunctions).GetMethod("Assignment_SysFVar"), args);
			}

			throw new Exception();
		}

		LocalVariableInfo EmitLogicalOperator(CompilerState state, Operator @operator, LocalVariableInfo lhs, LocalVariableInfo rhs)
		{
			if (state == null) throw new Exception();
			if (lhs == null) throw new Exception();
			if (rhs == null) throw new Exception();

			LocalVariableInfo result = state.Generator.DeclareLocal(typeof(Int32));

			PushAsBoolean(state, lhs);
			PushAsBoolean(state, rhs);

			switch (@operator)
			{
				case Operator.LogicalAnd:
					state.Generator.Emit(OpCodes.And);
					StoreLocalVariable(state, result);
					break;

				case Operator.LogicalOr:
					state.Generator.Emit(OpCodes.Or);
					StoreLocalVariable(state, result);
					break;

				case Operator.LogicalXor:
					state.Generator.Emit(OpCodes.Xor);
					StoreLocalVariable(state, result);
					break;

				default:
					throw new Exception();

			}

			return result;
		}

		LocalVariableInfo EmitComparsionOperator(CompilerState state, Operator @operator, LocalVariableInfo lhs, LocalVariableInfo rhs)
		{
			if (state == null) throw new Exception();
			if (lhs == null) throw new Exception();
			if (rhs == null) throw new Exception();

			LocalVariableInfo result = state.Generator.DeclareLocal(typeof(Int32));

			if (lhs.LocalType == typeof(Int32) && rhs.LocalType == typeof(Int32))
			{
				LoadLocalVariable(state, lhs);
				LoadLocalVariable(state, rhs);
			}
			else
			{
				LoadLocalVariable(state, lhs);
				state.Generator.Emit(OpCodes.Conv_R4);

				LoadLocalVariable(state, rhs);
				state.Generator.Emit(OpCodes.Conv_R4);
			}

			switch (@operator)
			{
				case Operator.Equals:
					state.Generator.Emit(OpCodes.Ceq);
					StoreLocalVariable(state, result);
					break;

				case Operator.NotEquals:
					state.Generator.Emit(OpCodes.Ceq);
					state.Generator.Emit(OpCodes.Ldc_I4_0);
					state.Generator.Emit(OpCodes.Ceq);
					StoreLocalVariable(state, result);
					break;

				case Operator.Lesser:
					state.Generator.Emit(OpCodes.Clt);
					StoreLocalVariable(state, result);
					break;

				case Operator.LesserEquals:
					state.Generator.Emit(OpCodes.Cgt);
					state.Generator.Emit(OpCodes.Ldc_I4_0);
					state.Generator.Emit(OpCodes.Ceq);
					StoreLocalVariable(state, result);
					break;

				case Operator.Greater:
					state.Generator.Emit(OpCodes.Cgt);
					StoreLocalVariable(state, result);
					break;

				case Operator.GreaterEquals:
					state.Generator.Emit(OpCodes.Clt);
					state.Generator.Emit(OpCodes.Ldc_I4_0);
					state.Generator.Emit(OpCodes.Ceq);
					StoreLocalVariable(state, result);
					break;

				default:
					throw new Exception();
			}

			return result;
		}

		LocalVariableInfo EmitArithmeticOperator(CompilerState state, OpCode msilcode, LocalVariableInfo lhs, LocalVariableInfo rhs)
		{
			if (state == null) throw new Exception();
			if (lhs == null) throw new Exception();
			if (rhs == null) throw new Exception();

			if (msilcode == OpCodes.Div) CheckIfZero(state, rhs);

			LocalVariableInfo result;
			if (lhs.LocalType == typeof(Int32) && rhs.LocalType == typeof(Int32))
			{
				result = state.Generator.DeclareLocal(typeof(Int32));

				LoadLocalVariable(state, lhs);
				LoadLocalVariable(state, rhs);
			}
			else
			{
				result = state.Generator.DeclareLocal(typeof(Single));

				LoadLocalVariable(state, lhs);
				state.Generator.Emit(OpCodes.Conv_R4);

				LoadLocalVariable(state, rhs);
				state.Generator.Emit(OpCodes.Conv_R4);
			}

			state.Generator.Emit(msilcode);
			StoreLocalVariable(state, result);

			return result;
		}

		LocalVariableInfo EmitUnaryOperator(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			Evaluation.Tokenizing.OperatorData data = node.Token.Data as Evaluation.Tokenizing.OperatorData;
			if (data == null) throw new Exception();

			if (node.Children.Count != 1) throw new Exception();

			if (data.Operator == Operator.Minus)
			{
				LocalVariableInfo value = Emit(state, node.Children[0]);
				LocalVariableInfo result = state.Generator.DeclareLocal(value.LocalType);

				LoadLocalVariable(state, value);
				state.Generator.Emit(OpCodes.Neg);
				StoreLocalVariable(state, result);

				return result;
			}

			if (data.Operator == Operator.LogicalNot)
			{
				LocalVariableInfo value = Emit(state, node.Children[0]);
				LoadLocalVariable(state, value);

				LocalVariableInfo result = state.Generator.DeclareLocal(typeof(Int32));
				Label l1 = state.Generator.DefineLabel();
				Label l2 = state.Generator.DefineLabel();

				state.Generator.Emit(OpCodes.Brtrue, l1);
				state.Generator.Emit(OpCodes.Ldc_I4_1);
				state.Generator.Emit(OpCodes.Br, l2);

				state.Generator.MarkLabel(l1);
				state.Generator.Emit(OpCodes.Ldc_I4_0);

				state.Generator.MarkLabel(l2);
				StoreLocalVariable(state, result);

				return result;
			}

			throw new Exception();
		}

		LocalVariableInfo EmitNumber(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			Evaluation.Tokenizing.NumberData data = node.Token.Data as Evaluation.Tokenizing.NumberData;
			if (data == null) throw new Exception();

			Number number = data.GetNumber(node.Token.ToString());

			if (number.NumberType == NumberType.Int)
			{
				LocalVariableInfo local = state.Generator.DeclareLocal(typeof(Int32));

				state.Generator.Emit(OpCodes.Ldc_I4, number.IntValue);
				StoreLocalVariable(state, local);
				return local;
			}

			if (number.NumberType == NumberType.Float)
			{
				LocalVariableInfo local = state.Generator.DeclareLocal(typeof(Single));

				state.Generator.Emit(OpCodes.Ldc_R4, number.FloatValue);
				StoreLocalVariable(state, local);
				return local;
			}

			throw new Exception();
		}

		MethodInfo FindCorrectMethod(Type functiontype, List<LocalVariableInfo> args)
		{
			if (functiontype == null) throw new Exception();
			if (args == null) throw new Exception();

			Type[] argtypes = new Type[args.Count + 2];
			argtypes[0] = typeof(Object);
			argtypes[1] = typeof(Boolean).MakeByRefType();
			for (Int32 i = 0; i != args.Count; ++i) argtypes[i + 2] = args[i].LocalType;

			MethodInfo info = functiontype.GetMethod("Evaluate", argtypes);
			if (info != null) return info;

			throw new Exception();
		}

		MethodInfo FindCorrectRedirectionMethod(Type functiontype, List<LocalVariableInfo> args)
		{
			if (functiontype == null) throw new Exception();
			if (args == null) throw new Exception();

			Type[] argtypes = new Type[args.Count + 2];
			argtypes[0] = typeof(Object);
			argtypes[1] = typeof(Boolean).MakeByRefType();
			for (Int32 i = 0; i != args.Count; ++i) argtypes[i + 2] = args[i].LocalType;

			MethodInfo info = functiontype.GetMethod("RedirectState", argtypes);
			if (info != null) return info;

			throw new Exception();
		}

		List<LocalVariableInfo> EmitDescendants(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			List<LocalVariableInfo> output = new List<LocalVariableInfo>();

			foreach (Node child in node.Children) output.Add(Emit(state, child));

			foreach (Object arg in node.Arguments)
			{
				if (arg is String)
				{
					LocalVariableInfo local = state.Generator.DeclareLocal(typeof(String));

					state.Generator.Emit(OpCodes.Ldstr, (String)arg);
					StoreLocalVariable(state, local);

					output.Add(local);
				}
				else if (arg.GetType().IsEnum == true)
				{
					LocalVariableInfo local = state.Generator.DeclareLocal(arg.GetType());

					state.Generator.Emit(OpCodes.Ldc_I4, (Int32)arg);
					StoreLocalVariable(state, local);

					output.Add(local);
				}
				else if (arg is Combat.HitType[])
				{
					LocalVariableInfo local = state.Generator.DeclareLocal(arg.GetType());
					Combat.HitType[] hitypes = (Combat.HitType[])arg;

					state.Generator.Emit(OpCodes.Ldc_I4, hitypes.Length);
					state.Generator.Emit(OpCodes.Newarr, typeof(Combat.HitType));

					StoreLocalVariable(state, local);

					for (Int32 i = 0; i != hitypes.Length; ++i)
					{
						LoadLocalVariable(state, local);

						state.Generator.Emit(OpCodes.Ldc_I4, i);

						state.Generator.Emit(OpCodes.Ldc_I4, (Int32)hitypes[i].Class);
						state.Generator.Emit(OpCodes.Ldc_I4, (Int32)hitypes[i].Power);
						state.Generator.Emit(OpCodes.Newobj, typeof(Combat.HitType).GetConstructor(new Type[] { typeof(AttackClass), typeof(AttackPower) }));

						state.Generator.Emit(OpCodes.Stelem, typeof(Combat.HitType));
					}

					output.Add(local);
				}
				else
				{
					throw new Exception();
				}
			}

			return output;
		}

		List<LocalVariableInfo> EmitRedirectionDescendants(CompilerState state, Node node)
		{
			if (state == null) throw new Exception();
			if (node == null) throw new Exception();

			List<LocalVariableInfo> output = new List<LocalVariableInfo>();

			for (Int32 i = 0; i < node.Children.Count - 1; ++i) output.Add(Emit(state, node.Children[i]));

			foreach (Object arg in node.Arguments)
			{
				if (arg is String)
				{
					LocalVariableInfo local = state.Generator.DeclareLocal(typeof(String));

					state.Generator.Emit(OpCodes.Ldstr, (String)arg);
					StoreLocalVariable(state, local);

					output.Add(local);
				}
				else if (arg.GetType().IsEnum == true)
				{
					LocalVariableInfo local = state.Generator.DeclareLocal(arg.GetType());

					state.Generator.Emit(OpCodes.Ldc_I4, (Int32)arg);
					StoreLocalVariable(state, local);

					output.Add(local);
				}
				else
				{
					throw new Exception();
				}
			}

			return output;
		}

		void CheckIfZero(CompilerState state, LocalVariableInfo local)
		{
			if (state == null) throw new Exception();
			if (local == null) throw new Exception();

			if (local.LocalType == typeof(Int32))
			{
				LoadLocalVariable(state, local);
				state.Generator.Emit(OpCodes.Ldc_I4_0);
				state.Generator.Emit(OpCodes.Beq, state.ErrorLabel);
			}

			if (local.LocalType == typeof(Single))
			{
				LoadLocalVariable(state, local);
				state.Generator.Emit(OpCodes.Ldc_R4, 0.0f);
				state.Generator.Emit(OpCodes.Beq, state.ErrorLabel);
			}
		}

		void LoadLocalVariable(CompilerState state, LocalVariableInfo local)
		{
			if (state == null) throw new Exception();
			if (local == null) throw new Exception();

			switch (local.LocalIndex)
			{
				case 0:
					state.Generator.Emit(OpCodes.Ldloc_0);
					break;

				case 1:
					state.Generator.Emit(OpCodes.Ldloc_1);
					break;

				case 2:
					state.Generator.Emit(OpCodes.Ldloc_2);
					break;

				case 3:
					state.Generator.Emit(OpCodes.Ldloc_3);
					break;

				default:
					state.Generator.Emit(OpCodes.Ldloc, local.LocalIndex);
					break;
			}
		}

		void StoreLocalVariable(CompilerState state, LocalVariableInfo local)
		{
			if (state == null) throw new Exception();
			if (local == null) throw new Exception();

			switch (local.LocalIndex)
			{
				case 0:
					state.Generator.Emit(OpCodes.Stloc_0);
					break;

				case 1:
					state.Generator.Emit(OpCodes.Stloc_1);
					break;

				case 2:
					state.Generator.Emit(OpCodes.Stloc_2);
					break;

				case 3:
					state.Generator.Emit(OpCodes.Stloc_3);
					break;

				default:
					state.Generator.Emit(OpCodes.Stloc, local.LocalIndex);
					break;
			}
		}

		void PushAsBoolean(CompilerState state, LocalVariableInfo local)
		{
			if (state == null) throw new Exception();
			if (local == null) throw new Exception();

			Label l1 = state.Generator.DefineLabel();
			Label l2 = state.Generator.DefineLabel();

			LoadLocalVariable(state, local);

			if (local.LocalType == typeof(Int32))
			{
				state.Generator.Emit(OpCodes.Brfalse, l1);
			}
			else if (local.LocalType == typeof(Single))
			{
				state.Generator.Emit(OpCodes.Ldc_R4, 0);
				state.Generator.Emit(OpCodes.Beq, l1);
			}
			else
			{
				throw new Exception();
			}

			state.Generator.Emit(OpCodes.Ldc_I4_1);
			state.Generator.Emit(OpCodes.Br, l2);

			state.Generator.MarkLabel(l1);
			state.Generator.Emit(OpCodes.Ldc_I4_0);

			state.Generator.MarkLabel(l2);
		}

		readonly ModuleBuilder m_module;

		readonly Dictionary<String, MethodInfo> m_constfunctionmethods;

		readonly Dictionary<String, MethodInfo> m_gethitvarmethods;
	}
}