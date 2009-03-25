using System;
using System.Diagnostics;

namespace xnaMugen
{
	/// <summary>
	/// A random number generator.
	/// </summary>
	class Random : SubSystem
	{
		/// <summary>
		/// Initializes a new instance of this class with a time dependant seed.
		/// </summary>
		public Random(SubSystems subsystems)
			: base(subsystems)
		{
			m_random = new System.Random();
		}

		/// <summary>
		/// Re-initializes random number generation with a specified seed.
		/// </summary>
		/// <param name="seed">Number used to start generation of random numbers.</param>
		public void Seed(Int32 seed)
		{
			m_random = new System.Random(seed);
		}

		/// <summary>
		/// Returns a randomly generated number within a specified range.
		/// </summary>
		/// <param name="min">The lowest number that can be generated, inclusive.</param>
		/// <param name="max">The highest number that can be generated, exclusive.</param>
		/// <returns>An Int32 that is greater than or equal to min but less than max.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">min is greater than max.</exception> 
		public Int32 NewInt(Int32 min, Int32 max)
		{
			return m_random.Next(min, max);
		}

		/// <summary>
		/// Return a randomly generated number.
		/// </summary>
		/// <returns>A Single that is greater than or equal to 0.0f and less than 1.0f.</returns>
		public Single NewSingle()
		{
			return (Single)m_random.NextDouble();
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		System.Random m_random;

		#endregion
	}
}
