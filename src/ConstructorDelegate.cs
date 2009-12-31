using System;
using System.Reflection;
using System.Reflection.Emit;

namespace xnaMugen
{
	static class ConstructorDelegate
	{
		public static Constructor FastConstruct(Type type, params Type[] argtypes)
		{
			ConstructorInfo cinfo = type.GetConstructor(argtypes);

			DynamicMethod method = new DynamicMethod("Create", typeof(Object), new Type[] { typeof(Object[]) }, typeof(ConstructorDelegate));
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
	}
}