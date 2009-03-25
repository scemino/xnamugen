using System;
using System.Collections.Generic;
using System.Diagnostics;
using xnaMugen.Collections;
using xnaMugen.IO;

namespace xnaMugen.Animations
{
	/// <summary>
	/// Controls the creation of Animations and AnimationManagers. 
	/// </summary>
	class AnimationSystem : SubSystem
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="subsystems">A collection of subsystems to be referenced.</param>
		public AnimationSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_animationcache = new Dictionary<String, KeyedCollection<Int32, Animation>>(StringComparer.OrdinalIgnoreCase);
			m_loader = new AnimationLoader(this);
		}

		/// <summary>
		/// Creates a new AnimationManager that governs the animations contained in an AIR file.
		/// </summary>
		/// <param name="filepath">Path to a text file containing animations in the AIR format.</param>
		/// <returns>A new AnimationManager for the animations found in the given file.</returns>
		public AnimationManager CreateManager(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			return new AnimationManager(filepath, GetAnimations(filepath));
		}

		/// <summary>
		/// Determines whether a line of text represents a Clsn.
		/// </summary>
		/// <param name="line">A line of text.</param>
		/// <returns>true is the supplied line represents a Clsn; false otherwise.</returns>
		public Boolean IsClsnLine(String line)
		{
			if (line == null) throw new ArgumentNullException("line");

			return m_loader.IsClsnLine(line);
		}

		/// <summary>
		/// Returns a collection of Animations from a file.
		/// </summary>
		/// <param name="filepath">The file to parsed Animations out of.</param>
		/// <returns>A collection containing all the Animations found in the given file.</returns>
		KeyedCollection<Int32, Animation> GetAnimations(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			KeyedCollection<Int32, Animation> animations = null;
			if (m_animationcache.TryGetValue(filepath, out animations) == true) return animations;

			TextFile textfile = GetSubSystem<IO.FileSystem>().OpenTextFile(filepath);

			animations = m_loader.LoadAnimations(textfile);
			m_animationcache.Add(filepath, animations);

			return animations;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Dictionary<String, KeyedCollection<Int32, Animation>> m_animationcache;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AnimationLoader m_loader;

		#endregion
	}
}