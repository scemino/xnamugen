using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Combat
{
	class CharacterVariables
	{
		public CharacterVariables()
		{
			m_int = new List<Int32>(60);
			m_sysint = new List<Int32>(4);
			m_float = new List<Single>(40);
			m_sysfloat = new List<Single>(4);

			for (Int32 i = 0; i != m_int.Capacity; ++i) m_int.Add(0);
			for (Int32 i = 0; i != m_sysint.Capacity; ++i) m_sysint.Add(0);
			for (Int32 i = 0; i != m_float.Capacity; ++i) m_float.Add(0);
			for (Int32 i = 0; i != m_sysfloat.Capacity; ++i) m_sysfloat.Add(0);
		}

		public void Reset()
		{
			for (Int32 i = 0; i != m_int.Count; ++i) m_int[i] = 0;
			for (Int32 i = 0; i != m_sysint.Count; ++i) m_sysint[i] = 0;
			for (Int32 i = 0; i != m_float.Count; ++i) m_float[i] = 0;
			for (Int32 i = 0; i != m_sysfloat.Count; ++i) m_sysfloat[i] = 0;
		}

		public Boolean GetInteger(Int32 index, Boolean system, out Int32 value)
		{
			var variables = system ? m_sysint : m_int;

			if (index < 0 || index > variables.Count)
			{
				value = Int32.MinValue;
				return false;
			}
			else
			{
				value = variables[index];
				return true;
			}
		}

		public Boolean SetInteger(Int32 index, Boolean system, Int32 value)
		{
			var variables = system ? m_sysint : m_int;

			if (index < 0 || index > variables.Count)
			{
				return false;
			}
			else
			{
				variables[index] = value;
				return true;
			}
		}

		public Boolean AddInteger(Int32 index, Boolean system, Int32 value)
		{
			var variables = system ? m_sysint : m_int;

			if (index < 0 || index > variables.Count)
			{
				return false;
			}
			else
			{
				variables[index] += value;
				return true;
			}
		}

		public Boolean GetFloat(Int32 index, Boolean system, out Single value)
		{
			var variables = system ? m_sysfloat : m_float;

			if (index < 0 || index > variables.Count)
			{
				value = Single.MinValue;
				return false;
			}
			else
			{
				value = variables[index];
				return true;
			}
		}

		public Boolean SetFloat(Int32 index, Boolean system, Single value)
		{
			var variables = system ? m_sysfloat : m_float;

			if (index < 0 || index > variables.Count)
			{
				return false;
			}
			else
			{
				variables[index] = value;
				return true;
			}
		}

		public Boolean AddFloat(Int32 index, Boolean system, Single value)
		{
			var variables = system ? m_sysfloat : m_float;

			if (index < 0 || index > variables.Count)
			{
				return false;
			}
			else
			{
				variables[index] += value;
				return true;
			}
		}

		public Collections.ListIterator<Int32> IntegerVariables
		{
			get { return new Collections.ListIterator<Int32>(m_int); }
		}

		public Collections.ListIterator<Int32> SystemIntegerVariables
		{
			get { return new Collections.ListIterator<Int32>(m_sysint); }
		}

		public Collections.ListIterator<Single> FloatVariables
		{
			get { return new Collections.ListIterator<Single>(m_float); }
		}

		public Collections.ListIterator<Single> SystemFloatVariables
		{
			get { return new Collections.ListIterator<Single>(m_sysfloat); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Int32> m_int;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Int32> m_sysint;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Single> m_float;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Single> m_sysfloat;

		#endregion
	}
}