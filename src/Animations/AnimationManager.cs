using System;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Animations
{
	/// <summary>
	/// Controls the playing of Animations.
	/// </summary>
	class AnimationManager
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="filepath">Path to the AIR file that the Animations were parsed out of.</param>
		/// <param name="animations">Collection of Animations that were parsed of the AIR file.</param>
		public AnimationManager(String filepath, KeyedCollection<Int32, Animation> animations)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");
			if (animations == null) throw new ArgumentNullException("animations");

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
		public Boolean HasAnimation(Int32 number)
		{
			return m_animations.Contains(number);
		}

		/// <summary>
		/// Changes the active Animation.
		/// </summary>
		/// <param name="animationnumber">The number of the new Animation.</param>
		/// <param name="elementnumber">The index of the starting element of the new Animation.</param>
		/// <returns>true if the requested Animation is set; false otherwise.</returns>
		public Boolean SetLocalAnimation(Int32 animationnumber, Int32 elementnumber)
		{
			if (HasAnimation(animationnumber) == false) return false;

			Animation animation = m_animations[animationnumber];
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
		public Boolean SetForeignAnimation(AnimationManager animations, Int32 animationnumber, Int32 elementnumber)
		{
			if (animations == null) throw new ArgumentNullException("animations");

			if (animations.HasAnimation(animationnumber) == false) return false;

			Animation animation = animations.m_animations[animationnumber];
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
				AnimationElement newlement = CurrentAnimation.GetNextElement(CurrentElement.Id);

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
		void SetAnimation(Animation animation, AnimationElement element)
		{
			if (animation == null) throw new ArgumentNullException("animation");
			if (element == null) throw new ArgumentNullException("element");

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
		public override String ToString()
		{
			return Filepath;
		}

		/// <summary>
		/// Returns the path to the text file of the managed Animations.
		/// </summary>
		/// <returns>The path to the text file of the managed Animations.</returns>
		public String Filepath
		{
			get { return m_filepath; }
		}

		/// <summary>
		/// Returns whether the active Animation is from another AnimationManager.
		/// </summary>
		/// <returns>true if the active Animation is from another AnimationManager; false otherwise.</returns>
		public Boolean IsForeignAnimation
		{
			get { return m_foreignanimation; }
		}

		/// <summary>
		/// Returns the currently active Animation.
		/// </summary>
		/// <returns>The currently active Animation.</returns>
		public Animation CurrentAnimation
		{
			get { return m_currentanimation; }
		}

		/// <summary>
		/// Returns the current element of the active Animation.
		/// </summary>
		/// <returns>The current element of the active Animation.</returns>
		public AnimationElement CurrentElement
		{
			get { return m_currentelement; }
		}

		/// <summary>
		/// Returns whether the last element of the current Animation has been passed.
		/// </summary>
		/// <returns>true if the last element of the current Animation has been passed; false otherwise.</returns>
		public Boolean IsAnimationFinished
		{
			get { return m_finishedanimation; }
		}

		/// <summary>
		/// Returns the time, in gameticks, spent in the current Animation.
		/// </summary>
		/// <returns>The time, in gameticks, spent in the current Animation.</returns>
		public Int32 TimeInAnimation
		{
			get { return m_animationtime; }
		}

		KeyedCollection<Int32, Animation> Animations
		{
			get { return m_animations; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly KeyedCollection<Int32, Animation> m_animations;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_foreignanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Animation m_currentanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		AnimationElement m_currentelement;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_finishedanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_animationinloop;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_animationtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_elementswitchtime;

		#endregion
	}
}