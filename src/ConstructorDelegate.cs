using System;
using System.Reflection.Emit;

namespace xnaMugen
{
	internal static class ConstructorDelegate
	{
		public static Constructor FastConstruct(Type type, params Type[] argtypes)
		{
			var cinfo = type.GetConstructor(argtypes);

			var method = new DynamicMethod("Create", typeof(object), new[] { typeof(object[]) }, typeof(ConstructorDelegate));
			var generator = method.GetILGenerator();

			for (var i = 0; i != argtypes.Length; ++i)
			{
				generator.Emit(OpCodes.Ldarg, 0);
				generator.Emit(OpCodes.Ldc_I4, i);
				generator.Emit(OpCodes.Ldelem_Ref);

				if (argtypes[i].IsValueType) generator.Emit(OpCodes.Unbox_Any, argtypes[i]);
			}

			generator.Emit(OpCodes.Newobj, cinfo);
			generator.Emit(OpCodes.Ret);

			return (Constructor)method.CreateDelegate(typeof(Constructor));
		}
	}
}