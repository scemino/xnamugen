using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading;
using System.Reflection;
using xnaMugen.Evaluation.Tokenizing;

namespace xnaMugen.Evaluation
{
	class TreeCompiler
	{
		public TreeCompiler()
		{
			AssemblyBuilder newAssembly = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.Run);
			m_module = newAssembly.DefineDynamicModule("Module");

			m_numbercallbacks = new Dictionary<Number, EvaluationCallback>();
			m_types = new Dictionary<String, Constructor>();
		}

		public EvaluationCallback Create(Node node)
		{
			foreach (Node n in EnumerateNodes(node))
			{
				if (n.Token.AsOperator == Operator.Assignment) return NestedFunctionBuild(node);

				if (n.Token.Data is Tokenizing.CustomFunctionData) return NestedFunctionBuild(node);
			}

			return SingleFunctionBuild(node);
		}

		EvaluationCallback GetNumberCallback(Number number)
		{
			EvaluationCallback callback;
			if (m_numbercallbacks.TryGetValue(number, out callback) == true) return callback;

			DynamicMethod method = new DynamicMethod(String.Empty, typeof(Number), new Type[] { typeof(Object) }, typeof(TreeCompiler));
			ILGenerator generator = method.GetILGenerator();

			switch (number.NumberType)
			{
				case NumberType.Int:
					generator.Emit(OpCodes.Ldc_I4, number.IntValue);
					generator.Emit(OpCodes.Newobj, typeof(Number).GetConstructor(new Type[] { typeof(Int32) }));
					break;

				case NumberType.Float:
					generator.Emit(OpCodes.Ldc_R4, number.FloatValue);
					generator.Emit(OpCodes.Newobj, typeof(Number).GetConstructor(new Type[] { typeof(Single) }));
					break;

				default:
					generator.Emit(OpCodes.Ldloc, generator.DeclareLocal(typeof(Number)).LocalIndex);
					break;
			}

			generator.Emit(OpCodes.Ret);

			callback = (EvaluationCallback)method.CreateDelegate(typeof(EvaluationCallback));
			m_numbercallbacks[number] = callback;

			return callback;
		}

		Constructor GetOperatorTypeConstructor(OperatorData data)
		{
			Constructor constructorfunc;
			if (m_types.TryGetValue(data.Name, out constructorfunc) == true) return constructorfunc;

			Int32 argcount = (data is BinaryOperatorData) ? 2 : 1;

			Type[] paramtypes = new Type[argcount];
			for (Int32 i = 0; i != argcount; ++i) paramtypes[i] = typeof(EvaluationCallback);

			TypeBuilder typebuilder = m_module.DefineType(data.Name, TypeAttributes.Public);
			typebuilder.AddInterfaceImplementation(typeof(IFunction));

			FieldBuilder[] fields = new FieldBuilder[argcount];
			for (Int32 i = 0; i != argcount; ++i) fields[i] = typebuilder.DefineField("m_arg" + i, typeof(EvaluationCallback), FieldAttributes.Private);

			ConstructorBuilder constructor = typebuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, paramtypes);
			ILGenerator constructorgenerator = constructor.GetILGenerator();

			constructorgenerator.Emit(OpCodes.Ldarg, 0);
			constructorgenerator.Emit(OpCodes.Call, typeof(Object).GetConstructor(Type.EmptyTypes));

			for (Int32 i = 0; i != argcount; ++i)
			{
				constructorgenerator.Emit(OpCodes.Ldarg, 0);
				constructorgenerator.Emit(OpCodes.Ldarg, i + 1);
				constructorgenerator.Emit(OpCodes.Stfld, fields[i]);
			}

			constructorgenerator.Emit(OpCodes.Ret);

			MethodBuilder method = typebuilder.DefineMethod("Evaluate", MethodAttributes.Public | MethodAttributes.Virtual, typeof(Number), new Type[] { typeof(Object) });
			ILGenerator methodgenerator = method.GetILGenerator();

			for (Int32 i = 0; i != argcount; ++i)
			{
				methodgenerator.Emit(OpCodes.Ldarg, 0);
				methodgenerator.Emit(OpCodes.Ldfld, fields[i]);
				methodgenerator.Emit(OpCodes.Ldarg, 1);
				methodgenerator.Emit(OpCodes.Callvirt, typeof(EvaluationCallback).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance));
			}

			methodgenerator.Emit(OpCodes.Call, typeof(Number).GetMethod(data.Name, BindingFlags.Public | BindingFlags.Static));
			methodgenerator.Emit(OpCodes.Ret);

			constructorfunc = FastConstruct(typebuilder.CreateType(), paramtypes);
			m_types[data.Name] = constructorfunc;

			return constructorfunc;
		}

		Constructor GetRangeConstructor()
		{
			String name = "__Range__";
			Constructor constructorfunc;

			if (m_types.TryGetValue(name, out constructorfunc) == true) return constructorfunc;

			TypeBuilder typebuilder = m_module.DefineType(name, TypeAttributes.Public);
			typebuilder.AddInterfaceImplementation(typeof(IFunction));

			MethodInfo evalmethod = typeof(Number).GetMethod("Range", BindingFlags.Public | BindingFlags.Static);
			ParameterInfo[] parameters = evalmethod.GetParameters();
			Type[] paramtypes = new Type[parameters.Length];
			FieldBuilder[] fields = new FieldBuilder[parameters.Length];

			for (Int32 i = 0; i != parameters.Length; ++i)
			{
				Type type = parameters[i].ParameterType;
				if (type == typeof(Number)) type = typeof(EvaluationCallback);

				paramtypes[i] = type;

				fields[i] = typebuilder.DefineField("m_arg" + i, paramtypes[i], FieldAttributes.Private);
			}

			ConstructorBuilder constructor = typebuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, paramtypes);
			ILGenerator constructorgenerator = constructor.GetILGenerator();

			constructorgenerator.Emit(OpCodes.Ldarg, 0);
			constructorgenerator.Emit(OpCodes.Call, typeof(Object).GetConstructor(Type.EmptyTypes));

			for (Int32 i = 0; i != fields.Length; ++i)
			{
				constructorgenerator.Emit(OpCodes.Ldarg, 0);
				constructorgenerator.Emit(OpCodes.Ldarg, i + 1);
				constructorgenerator.Emit(OpCodes.Stfld, fields[i]);
			}

			constructorgenerator.Emit(OpCodes.Ret);

			MethodBuilder method = typebuilder.DefineMethod("Evaluate", MethodAttributes.Public | MethodAttributes.Virtual, typeof(Number), new Type[] { typeof(Object) });
			ILGenerator methodgenerator = method.GetILGenerator();

			for (Int32 i = 0; i != fields.Length; ++i)
			{
				methodgenerator.Emit(OpCodes.Ldarg, 0);
				methodgenerator.Emit(OpCodes.Ldfld, fields[i]);

				if (fields[i].FieldType == typeof(EvaluationCallback))
				{
					methodgenerator.Emit(OpCodes.Ldarg, 1);
					methodgenerator.Emit(OpCodes.Callvirt, typeof(EvaluationCallback).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance));
				}
			}

			methodgenerator.Emit(OpCodes.Call, evalmethod);
			methodgenerator.Emit(OpCodes.Ret);

			constructorfunc = FastConstruct(typebuilder.CreateType(), paramtypes);
			m_types[name] = constructorfunc;

			return constructorfunc;
		}

		MethodInfo GetCorrectMethod(Type type, Object[] args)
		{
			Type[] parameters = new Type[args.Length + 1];
			parameters[0] = typeof(Object);

			for (Int32 i = 0; i != args.Length; ++i)
			{
				parameters[i + 1] = args[i].GetType();
			}

			MethodInfo method = type.GetMethod("RedirectState", BindingFlags.Static | BindingFlags.Public, null, parameters, null);
			if (method != null) return method;

			for (Int32 i = 0; i != args.Length; ++i)
			{
				if (parameters[i + 1] == typeof(EvaluationCallback)) parameters[i + 1] = typeof(Number);
			}

			method = type.GetMethod("Evaluate", BindingFlags.Static | BindingFlags.Public, null, parameters, null);

			return method;
		}

		Constructor GetCustomFunctionConstructor(String name, Type functiontype, Object[] args)
		{
			Constructor constructorfunc;
			if (m_types.TryGetValue(name, out constructorfunc) == true) return constructorfunc;

			TypeBuilder typebuilder = m_module.DefineType(name, TypeAttributes.Public);
			typebuilder.AddInterfaceImplementation(typeof(IFunction));

			MethodInfo evalmethod = functiontype.GetMethod("RedirectState", BindingFlags.Static | BindingFlags.Public) ?? functiontype.GetMethod("Evaluate", BindingFlags.Static | BindingFlags.Public);
			ParameterInfo[] parameters = evalmethod.GetParameters();
			Type[] paramtypes = new Type[parameters.Length - 1];
			FieldBuilder[] fields = new FieldBuilder[parameters.Length - 1];

			for (Int32 i = 1; i != parameters.Length; ++i)
			{
				Type type = parameters[i].ParameterType;
				if (type == typeof(Number)) type = typeof(EvaluationCallback);

				paramtypes[i - 1] = type;

				fields[i - 1] = typebuilder.DefineField("m_arg" + i, paramtypes[i - 1], FieldAttributes.Private);
			}

			ConstructorBuilder constructor = typebuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, paramtypes);
			ILGenerator constructorgenerator = constructor.GetILGenerator();

			constructorgenerator.Emit(OpCodes.Ldarg, 0);
			constructorgenerator.Emit(OpCodes.Call, typeof(Object).GetConstructor(Type.EmptyTypes));

			for (Int32 i = 0; i != fields.Length; ++i)
			{
				constructorgenerator.Emit(OpCodes.Ldarg, 0);
				constructorgenerator.Emit(OpCodes.Ldarg, i + 1);
				constructorgenerator.Emit(OpCodes.Stfld, fields[i]);
			}

			constructorgenerator.Emit(OpCodes.Ret);

			MethodBuilder method = typebuilder.DefineMethod("Evaluate", MethodAttributes.Public | MethodAttributes.Virtual, typeof(Number), new Type[] { typeof(Object) });
			ILGenerator methodgenerator = method.GetILGenerator();

			methodgenerator.Emit(OpCodes.Ldarg, 1);

			for (Int32 i = 0; i != fields.Length; ++i)
			{
				methodgenerator.Emit(OpCodes.Ldarg, 0);
				methodgenerator.Emit(OpCodes.Ldfld, fields[i]);

				if (evalmethod.Name == "Evaluate" && fields[i].FieldType == typeof(EvaluationCallback))
				{
					methodgenerator.Emit(OpCodes.Ldarg, 1);
					methodgenerator.Emit(OpCodes.Callvirt, typeof(EvaluationCallback).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance));
				}
			}

			methodgenerator.Emit(OpCodes.Call, evalmethod);
			methodgenerator.Emit(OpCodes.Ret);

			constructorfunc = FastConstruct(typebuilder.CreateType(), paramtypes);

			m_types[name] = constructorfunc;
			return constructorfunc;
		}

		Constructor FastConstruct(Type type, params Type[] argtypes)
		{
			ConstructorInfo cinfo = type.GetConstructor(argtypes);

			DynamicMethod method = new DynamicMethod("Create", typeof(Object), new Type[] { typeof(Object[]) }, m_module);
			ILGenerator generator = method.GetILGenerator();

			for (Int32 i = 0; i != argtypes.Length; ++i)
			{
				generator.Emit(OpCodes.Ldarg, 0);
				generator.Emit(OpCodes.Ldc_I4, i);
				generator.Emit(OpCodes.Ldelem_Ref);

				if (argtypes[i].IsValueType == true) generator.Emit(OpCodes.Unbox_Any, argtypes[i]);
			}

			generator.Emit(OpCodes.Newobj, cinfo);
			generator.Emit(OpCodes.Ret);

			return (Constructor)method.CreateDelegate(typeof(Constructor));
		}

		EvaluationCallback NestedFunctionBuild(Node node)
		{
			if (node.Token.Data is Evaluation.Tokenizing.NumberData)
			{
				Number number = (node.Token.Data as Evaluation.Tokenizing.NumberData).GetNumber(node.Token.ToString());
				return GetNumberCallback(number);
			}

			if (node.Token.Data is Evaluation.Tokenizing.RangeData)
			{
				Object[] args = new Object[node.Children.Count + node.Arguments.Count];

				for (Int32 i = 0; i != node.Children.Count; ++i) args[i] = NestedFunctionBuild(node.Children[i]);
				for (Int32 i = 0; i != node.Arguments.Count; ++i) args[i + node.Children.Count] = node.Arguments[i];

				Constructor constructor = GetRangeConstructor();
				IFunction function = (IFunction)constructor(args);

				return function.Evaluate;
			}

			if (node.Token.AsOperator == Operator.Assignment)
			{
				Object[] args = new Object[node.Children.Count + node.Arguments.Count];

				for (Int32 i = 0; i != node.Children.Count; ++i) args[i] = NestedFunctionBuild(node.Children[i]);
				for (Int32 i = 0; i != node.Arguments.Count; ++i) args[i + node.Children.Count] = node.Arguments[i];

				Constructor constructor = GetCustomFunctionConstructor("_Assignment", typeof(Evaluation.Triggers._Assignment), args);
				IFunction function = (IFunction)constructor(args);

				return function.Evaluate;
			}

			if (node.Token.AsOperator != Operator.None)
			{
				Constructor constructor = GetOperatorTypeConstructor(node.Token.Data as OperatorData);
				EvaluationCallback[] childcallbacks;

				if (node.Token.AsOperator == Operator.Minus && node.Children.Count == 1)
				{
					childcallbacks = new EvaluationCallback[2] { GetNumberCallback(new Number(0)), NestedFunctionBuild(node.Children[0]) };
				}
				else
				{
					childcallbacks = new EvaluationCallback[node.Children.Count];
					for (Int32 i = 0; i != node.Children.Count; ++i) childcallbacks[i] = NestedFunctionBuild(node.Children[i]);
				}

				IFunction function = (IFunction)constructor(childcallbacks);
				return function.Evaluate;
			}

			if (node.Token.Data is Evaluation.Tokenizing.CustomFunctionData)
			{
				Object[] args = new Object[node.Children.Count + node.Arguments.Count];

				for (Int32 i = 0; i != node.Children.Count; ++i) args[i] = NestedFunctionBuild(node.Children[i]);
				for (Int32 i = 0; i != node.Arguments.Count; ++i) args[i + node.Children.Count] = node.Arguments[i];

				CustomFunctionData data = node.Token.Data as CustomFunctionData;

				Constructor constructor = GetCustomFunctionConstructor(data.Name, data.Type, args);
				IFunction function = (IFunction)constructor(args);

				return function.Evaluate;
			}

			return null;
		}

		EvaluationCallback SingleFunctionBuild(Node node)
		{
			DynamicMethod method = new DynamicMethod(String.Empty, typeof(Number), new Type[] { typeof(Object) }, typeof(TreeCompiler));
			ILGenerator generator = method.GetILGenerator();

			foreach (Node n in EnumerateNodes(node))
			{
				if (n.Token.Data is Evaluation.Tokenizing.CustomFunctionData) generator.Emit(OpCodes.Ldarg, 0);
			}

			SingleFunctionBuild(generator, node);

			generator.Emit(OpCodes.Ret);

			EvaluationCallback callback = (EvaluationCallback)method.CreateDelegate(typeof(EvaluationCallback));

			return callback;
		}

		void SingleFunctionBuild(ILGenerator generator, Node node)
		{
			if (node.Token.Data is Evaluation.Tokenizing.NumberData)
			{
				Number number = (node.Token.Data as Evaluation.Tokenizing.NumberData).GetNumber(node.Token.ToString());

				switch (number.NumberType)
				{
					case NumberType.Int:
						generator.Emit(OpCodes.Ldc_I4, number.IntValue);
						generator.Emit(OpCodes.Newobj, typeof(Number).GetConstructor(new Type[] { typeof(Int32) }));
						break;

					case NumberType.Float:
						generator.Emit(OpCodes.Ldc_R4, number.FloatValue);
						generator.Emit(OpCodes.Newobj, typeof(Number).GetConstructor(new Type[] { typeof(Single) }));
						break;

					default:
						generator.Emit(OpCodes.Ldloc, generator.DeclareLocal(typeof(Number)).LocalIndex);
						break;
				}
			}
			else if (node.Token.Data is Evaluation.Tokenizing.OperatorData)
			{
				foreach (Node childnode in node.Children) SingleFunctionBuild(generator, childnode);

				if (node.Children.Count == 1 && node.Token.AsOperator == Operator.Minus)
				{
					generator.Emit(OpCodes.Call, typeof(Number).GetMethod("Negate", BindingFlags.Static | BindingFlags.Public));
				}
				else
				{
					String methodname = (node.Token.Data as Evaluation.Tokenizing.OperatorData).Name;
					generator.Emit(OpCodes.Call, typeof(Number).GetMethod(methodname, BindingFlags.Static | BindingFlags.Public));
				}
			}
			else if (node.Token.Data is Evaluation.Tokenizing.RangeData)
			{
				SingleFunctionSubBuild(generator, node);

				generator.Emit(OpCodes.Call, typeof(Number).GetMethod("Range", BindingFlags.Static | BindingFlags.Public));
			}
			else if (node.Token.Data is Evaluation.Tokenizing.CustomFunctionData)
			{
				SingleFunctionSubBuild(generator, node);

				generator.Emit(OpCodes.Call, (node.Token.Data as Evaluation.Tokenizing.CustomFunctionData).Type.GetMethod("Evaluate", BindingFlags.Static | BindingFlags.Public));
			}
			else
			{
			}
		}

		void SingleFunctionSubBuild(ILGenerator generator, Node node)
		{
			foreach (Node childnode in node.Children)
			{
				SingleFunctionBuild(generator, childnode);
			}

			foreach (Object arg in node.Arguments)
			{
				if (arg is String)
				{
					generator.Emit(OpCodes.Ldstr, (String)arg);
				}
				else if (arg.GetType().IsEnum == true)
				{
					generator.Emit(OpCodes.Ldc_I4, (Int32)arg);
				}
				else
				{
				}
			}
		}

		static IEnumerable<Node> EnumerateNodes(Node basenode)
		{
			foreach (Node node in basenode.Children)
			{
				foreach (Node child in EnumerateNodes(node)) yield return child;
			}

			yield return basenode;
		}

		#region Fields

		readonly ModuleBuilder m_module;

		readonly Dictionary<Number, EvaluationCallback> m_numbercallbacks;

		readonly Dictionary<String, Constructor> m_types;

		#endregion
	}
}