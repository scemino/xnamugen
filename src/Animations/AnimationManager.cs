using System;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Animations
{
	/// <summary>
	/// Controls the playing of Animations.
	/// </summary>
	internal class AnimationManager
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="filepath">Path to the AIR file that the Animations were parsed out of.</param>
		/// <param name="animations">Collection of Animations that were parsed of the AIR file.</param>
		public AnimationManager(string filepath, KeyedCollection<int, Animation> animations)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));
			if (animations == null) throw new ArgumentNullException(nameof(animations));

			m_filepath = filepath;
			m_animations = animations;
			m_foreignanimation = false;
			m_currentanimation = null;
			m_finishedanimation = false;
			m_animationinloop = false;
			m_animationtime = 0;
			m_elementswitchtime = 0;
		}

		/// <summary>
		/// Creates a new AnimationManager contains the same Animations.
		/// </summary>
		/// <returns></returns>
		public AnimationManager Clone()
		{
			return new AnimationManager(Filepath, m_animations);
		}

		/// <summary>
		/// Determines whether or not an Animation is part of the collection.
		/// </summary>
		/// <param name="number">The Animation number that is looked for.</param>
		/// <returns>true if the requested Animation exists; false otherwise.</returns>
		public bool HasAnimation(int number)
		{
			return m_animations.Contains(number);
		}

		/// <summary>
		/// Changes the active Animation.
		/// </summary>
		/// <param name="animationnumber">The number of the new Animation.</param>
		/// <param name="elementnumber">The index of the starting element of the new Animation.</param>
		/// <returns>true if the requested Animation is set; false otherwise.</returns>
		public bool SetLocalAnimation(int animationnumber, int elementnumber)
		{
			if (HasAnimation(animationnumber) == false) return false;

			var animation = m_animations[animationnumber];
			if (elementnumber < 0 || elementnumber >= animation.Elements.Count) return false;

			m_foreignanimation = false;
			SetAnimation(animation, animation.Elements[elementnumber]);
			return true;
		}

		/// <summary>
		/// Changes the active Animation to one in a different AnimationManager.
		/// </summary>
		/// <param name="animations">The AnimationManager to get the new Animation out of.</param>
		/// <param name="animationnumber">The number of the new Animation.</param>
		/// <param name="elementnumber">The index of the starting element of the new Animation.</param>
		/// <returns>true if the requested Animation is set; false otherwise.</returns>
		public bool SetForeignAnimation(AnimationManager animations, int animationnumber, int elementnumber)
		{
			if (animations == null) throw new ArgumentNullException(nameof(animations));

			if (animations.HasAnimation(animationnumber) == false) return false;

			var animation = animations.m_animations[animationnumber];
			if (elementnumber < 0 || elementnumber >= animation.Elements.Count) return false;

			m_foreignanimation = true;
			SetAnimation(animation, animation.Elements[elementnumber]);
			return true;
		}

		/// <summary>
		/// Moves the currently active Animation one ticks further in its sequence.
		/// </summary>
		public void Update()
		{
			if (CurrentAnimation == null || CurrentElement == null) return;

			m_finishedanimation = false;
			++m_animationtime;

			if (m_elementswitchtime == -1) return;

			if (m_elementswitchtime > 1)
			{
				--m_elementswitchtime;
			}
			else
			{
				var newlement = CurrentAnimation.GetNextElement(CurrentElement.Id);

				if (newlement.Id <= CurrentElement.Id)
				{
					m_animationinloop = true;
					m_finishedanimation = true;
				}

				m_currentelement = newlement;
				m_elementswitchtime = CurrentElement.Gameticks;
			}
		}

		/// <summary>
		/// Changes the active Animation.
		/// </summary>
		/// <param name="animation">The new Animation.</param>
		/// <param name="element">The new AnimationElement of the given Animation.</param>
		private void SetAnimation(Animation animation, AnimationElement element)
		{
			if (animation == null) throw new ArgumentNullException(nameof(animation));
			if (element == null) throw new ArgumentNullException(nameof(element));

			m_currentanimation = animation;
			m_currentelement = element;

			m_finishedanimation = false;
			m_animationinloop = false;
			m_animationtime = CurrentAnimation.GetElementStartTime(CurrentElement.Id);
			m_elementswitchtime = CurrentElement.Gameticks;
		}

		/// <summary>
		/// Returns the path to the text file of the managed Animations.
		/// </summary>
		/// <returns>The path to the text file of the managed Animations.</returns>
		public override string ToString()
		{
			return Filepath;
		}

		/// <summary>
		/// Returns the path to the text file of the managed Animations.
		/// </summary>
		/// <returns>The path to the text file of the managed Animations.</returns>
		public string Filepath => m_filepath;

		/// <summary>
		/// Returns whether the active Animation is from another AnimationManager.
		/// </summary>
		/// <returns>true if the active Animation is from another AnimationManager; false otherwise.</returns>
		public bool IsForeignAnimation => m_foreignanimation;

		/// <summary>
		/// Returns the currently active Animation.
		/// </summary>
		/// <returns>The currently active Animation.</returns>
		public Animation CurrentAnimation => m_currentanimation;

		/// <summary>
		/// Returns the current element of the active Animation.
		/// </summary>
		/// <returns>The current element of the active Animation.</returns>
		public AnimationElement CurrentElement => m_currentelement;

		/// <summary>
		/// Returns whether the last element of the current Animation has been passed.
		/// </summary>
		/// <returns>true if the last element of the current Animation has been passed; false otherwise.</returns>
		public bool IsAnimationFinished => m_finishedanimation;

		/// <summary>
		/// Returns the time, in gameticks, spent in the current Animation.
		/// </summary>
		/// <returns>The time, in gameticks, spent in the current Animation.</returns>
		public int TimeInAnimation => m_animationtime;

		private KeyedCollection<int, Animation> Animations => m_animations;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly KeyedCollection<int, Animation> m_animations;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_foreignanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Animation m_currentanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AnimationElement m_currentelement;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_finishedanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_animationinloop;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_animationtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_elementswitchtime;

		#endregion
	}
}