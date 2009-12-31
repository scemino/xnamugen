using System;
using System.Collections.Generic;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Animations
{
	/// <summary>
	/// Represents a sequence of sprites to draw.
	/// </summary>
	class Animation
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="number">The identifing number of this Animation.</param>
		/// <param name="loopstart">The element index where the sequence of this Animation starts on when looping.</param>
		/// <param name="elements">The collection of elements making up this Animation.</param>
		public Animation(Int32 number, Int32 loopstart, List<AnimationElement> elements)
		{
			if (number < 0) throw new ArgumentOutOfRangeException("number");
			if (loopstart < 0) throw new ArgumentOutOfRangeException("loopstart");
			if (elements == null) throw new ArgumentNullException("elements");
			if (elements.Count == 0) throw new ArgumentException("elements");
			if (loopstart >= elements.Count) throw new ArgumentOutOfRangeException("loopstart");

			m_number = number;
			m_loopstart = loopstart;
			m_elements = elements;
			m_totaltime = CalculateTotalTime();
		}

		/// <summary>
		/// Calculates the total amount of time, in gameticks, it takes for this animation to start looping.
		/// </summary>
		/// <returns>The sum of the time of all the elements of this Animation. If the last element has a length of -1, this returns -1.</returns>
		Int32 CalculateTotalTime()
		{
			Int32 time = 0;

			foreach (AnimationElement element in this)
			{
				if (element.Gameticks == -1) return -1;
				time += element.Gameticks;
			}

			return time;
		}

		public Int32 GetElementStartTime(Int32 elementnumber)
		{
			if (elementnumber < 0 || elementnumber >= Elements.Count) throw new ArgumentOutOfRangeException("elementnumber");

			return Elements[elementnumber].StartTick;
		}

		/// <summary>
		/// Returns the next element in the animation sequence from a given element index.
		/// </summary>
		/// <param name="elementnumber">The element index that precedes the returned element.</param>
		/// <returns>The next element in the animation sequence.</returns>
		public AnimationElement GetNextElement(Int32 elementnumber)
		{
			if (elementnumber < 0 || elementnumber >= Elements.Count) throw new ArgumentOutOfRangeException("elementnumber");

			++elementnumber;
			return (elementnumber < Elements.Count) ? Elements[elementnumber] : Elements[Loopstart];
		}

		/// <summary>
		/// Return a element from this Animation that would be drawn when this Animation has been drawn for the given time, in gameticks.
		/// </summary>
		/// <param name="time">The time, in gameticks.</param>
		/// <returns>The element that would be drawn at the given time.</returns>
		public AnimationElement GetElementFromTime(Int32 time)
		{
			if (time < 0) throw new ArgumentOutOfRangeException("time");

			for (AnimationElement element = Elements[0]; element != null; element = GetNextElement(element.Id))
			{
				if (element.Gameticks == -1) return element;

				time -= element.Gameticks;
				if (time < 0) return element;
			}

			throw new InvalidOperationException();
		}

		/// <summary>
		/// Return an enumerator used for iterating through the elements of this Animation.
		/// </summary>
		/// <returns>An enumerator used for iterating through the elements of this Animation.</returns>
		public List<AnimationElement>.Enumerator GetEnumerator()
		{
			return Elements.GetEnumerator();
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override String ToString()
		{
			return "Animation #" + Number.ToString();
		}

		/// <summary>
		/// Retusn the identifing number of this Animation.
		/// </summary>
		/// <returns>The identifing number of this Animation.</returns>
		public Int32 Number
		{
			get { return m_number; }
		}

		/// <summary>
		/// Returns the index of the element where the sequence of this Animation starts on when looping.
		/// </summary>
		/// <returns>The index of the element where the sequence of this Animation starts on when looping.</returns>
		public Int32 Loopstart
		{
			get { return m_loopstart; }
		}

		/// <summary>
		/// Returns an iterator for iterating through the elements of this Animation.
		/// </summary>
		/// <returns>An iterator for iterating through the elements of this Animation.</returns>
		public ListIterator<AnimationElement> Elements
		{
			get { return new ListIterator<AnimationElement>(m_elements); }
		}

		/// <summary>
		/// Returns the total amount of time, in gameticks, required for this Animation to start looping.
		/// </summary>
		/// <returns>The total amount of time, in gameticks, required for this Animation to start looping.</returns>
		public Int32 TotalTime
		{
			get { return m_totaltime; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_number;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_loopstart;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_totaltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<AnimationElement> m_elements;

		#endregion
	}
}