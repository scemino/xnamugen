using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;
using xnaMugen.IO;

namespace xnaMugen.Animations
{
	/// <summary>
	/// Used to create Animations, AnimationElements, and Clsns from AIR files.
	/// </summary>
	internal class AnimationLoader
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="animationsystem">The animation system.</param>
		public AnimationLoader(AnimationSystem animationsystem)
		{
			if (animationsystem == null) throw new ArgumentNullException(nameof(animationsystem));

			m_animationsystem = animationsystem;
			m_animationtitleregex = new Regex(@"^\s*Begin action\s+(-?\d+)(,.+)?\s*$", RegexOptions.IgnoreCase);
			m_clsnregex = new Regex(@"Clsn([12])(Default)?:\s*(\d+)", RegexOptions.IgnoreCase);
			m_clsnlineregex = new Regex(@"Clsn([12])?\[(-?\d+)\]\s*=\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)", RegexOptions.IgnoreCase);
			m_elementregex = new Regex(@"\s*,\s*", RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Creates Animations from a xnaMugen.IO.TextFile.
		/// </summary>
		/// <param name="textfile">A textfile whose xnaMugen.IO.TextSection can be used to create Animations.</param>
		/// <returns>A collection of Animations created from the supplied textfile.</returns>
		public KeyedCollection<int, Animation> LoadAnimations(TextFile textfile)
		{
			if (textfile == null) throw new ArgumentNullException(nameof(textfile));

			var animations = new KeyedCollection<int, Animation>(x => x.Number);

			foreach (var section in textfile)
			{
				var animation = CreateAnimation(section);
				if (animation != null)
				{
					if (animations.Contains(animation.Number) == false)
					{
						animations.Add(animation);
					}
					else
					{
						Log.Write(LogLevel.Warning, LogSystem.AnimationSystem, "Duplicate animation #{0}. Discarding duplicate.", animation.Number);
					}
				}
			}

			return animations;
		}

		/// <summary>
		/// Determines whether a line of text represents a Clsn.
		/// </summary>
		/// <param name="line">A line of text.</param>
		/// <returns>true is the supplied line represents a Clsn; false otherwise.</returns>
		public bool IsClsnLine(string line)
		{
			if (line == null) throw new ArgumentNullException(nameof(line));

			return m_clsnlineregex.IsMatch(line);
		}

		/// <summary>
		/// Creates a new Animation.
		/// </summary>
		/// <param name="section">The information used to intialize the Animation.</param>
		/// <returns>A new instance of the Animation class if it could be created; null otherwise.</returns>
		private Animation CreateAnimation(TextSection section)
		{
			if (section == null) throw new ArgumentNullException(nameof(section));

			var titlematch = m_animationtitleregex.Match(section.Title);
			if (titlematch.Success == false) return null;

			var sc = StringComparer.OrdinalIgnoreCase;

			var animationnumber = int.Parse(titlematch.Groups[1].Value);
			var loopstart = 0;
			var starttick = 0;
			var elements = new List<AnimationElement>();

			var loading_type1 = new List<Clsn>();
			var loading_type2 = new List<Clsn>();
			var default_type1 = new List<Clsn>();
			var default_type2 = new List<Clsn>();

			var loaddefault = false;
			var loadtype = ClsnType.None;
			var loadcount = 0;

			foreach (var line in section.Lines)
			{
				if (loadcount > 0)
				{
					var clsn = CreateClsn(line, loadtype);
					if (clsn != null)
					{
						if (loaddefault)
						{
							if (loadtype == ClsnType.Type1Attack) default_type1.Add(clsn.Value);
							if (loadtype == ClsnType.Type2Normal) default_type2.Add(clsn.Value);
						}
						else
						{
							if (loadtype == ClsnType.Type1Attack) loading_type1.Add(clsn.Value);
							if (loadtype == ClsnType.Type2Normal) loading_type2.Add(clsn.Value);
						}
					}
					else
					{
						Log.Write(LogLevel.Warning, LogSystem.AnimationSystem, "Could not create Clsn from line: {0}", line);
					}

					--loadcount;
					continue;
				}

				var clsnmatch = m_clsnregex.Match(line);
				if (clsnmatch.Success)
				{
					var clsntype = ClsnType.None;
					if (sc.Equals(clsnmatch.Groups[1].Value, "1")) clsntype = ClsnType.Type1Attack;
					if (sc.Equals(clsnmatch.Groups[1].Value, "2")) clsntype = ClsnType.Type2Normal;

					var isdefault = sc.Equals(clsnmatch.Groups[2].Value, "default");
					if (isdefault)
					{
						if (clsntype == ClsnType.Type1Attack) default_type1.Clear();
						if (clsntype == ClsnType.Type2Normal) default_type2.Clear();
					}

					loadcount = int.Parse(clsnmatch.Groups[3].Value);
					loaddefault = isdefault;
					loadtype = clsntype;
					continue;
				}

				if (sc.Equals(line, "Loopstart"))
				{
					loopstart = elements.Count;
					continue;
				}

				var element = CreateElement(line, elements.Count, starttick, default_type1, default_type2, loading_type1, loading_type2);
				if (element != null)
				{
					if (element.Gameticks == -1)
					{
						starttick = -1;
					}
					else
					{
						starttick += element.Gameticks;
					}

					elements.Add(element);

					loading_type1.Clear();
					loading_type2.Clear();
				}
				else
				{
					Log.Write(LogLevel.Warning, LogSystem.AnimationSystem, "Error parsing element for Animation #{0} - {1}", animationnumber, line);
				}
			}

			if (elements.Count == 0)
			{
				Log.Write(LogLevel.Warning, LogSystem.AnimationSystem, "No elements defined for Animation #{0}", animationnumber);
				return null;
			}

			if (loopstart == elements.Count) loopstart = 0;

			return new Animation(animationnumber, loopstart, elements);
		}

		/// <summary>
		/// Creates a new collision box.
		/// </summary>
		/// <param name="line">A line of text representing a collision box.</param>
		/// <param name="overridetype">The type of collision box to be created.</param>
		/// <returns>A new instance of the Clsn class if it could be created; null otherwise.</returns>
		private Clsn? CreateClsn(string line, ClsnType overridetype)
		{
			if (string.Compare(line, 0, "Clsn", 0, 4, StringComparison.OrdinalIgnoreCase) != 0) return null;

			var match = m_clsnlineregex.Match(line);
			if (match.Success == false) return null;

			//if (string.Equals(match.Groups[1].Value, "1") == true) clsn.Type = ClsnType.Type1Attack;
			//if (string.Equals(match.Groups[1].Value, "2") == true) clsn.Type = ClsnType.Type2Normal;

			var x1 = int.Parse(match.Groups[3].Value);
			var y1 = int.Parse(match.Groups[4].Value);
			var x2 = int.Parse(match.Groups[5].Value);
			var y2 = int.Parse(match.Groups[6].Value);

			if (x1 > x2) Misc.Swap(ref x1, ref x2);
			if (y1 > y2) Misc.Swap(ref y1, ref y2);

			return new Clsn(overridetype, new Rectangle(x1, y1, x2 - x1, y2 - y1));
		}

		/// <summary>
		/// Creates a new AnimationElement initialized from a line of text.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="elementid"></param>
		/// <param name="default_clsn"></param>
		/// <param name="loading_clsn"></param>
		/// <returns></returns>
		private AnimationElement CreateElement(string line, int elementid, int starttick, List<Clsn> default_type1, List<Clsn> default_type2, List<Clsn> loading_type1, List<Clsn> loading_type2)
		{
			if (line == null) throw new ArgumentNullException(nameof(line));
			if (elementid < 0) throw new ArgumentOutOfRangeException(nameof(elementid));
			if (starttick < 0) throw new ArgumentOutOfRangeException(nameof(starttick));
			if (default_type1 == null) throw new ArgumentNullException(nameof(default_type1));
			if (default_type2 == null) throw new ArgumentNullException(nameof(default_type2));
			if (loading_type1 == null) throw new ArgumentNullException(nameof(loading_type1));
			if (loading_type2 == null) throw new ArgumentNullException(nameof(loading_type2));

			var elements = m_elementregex.Split(line);
			if (elements == null) return null;

			int groupnumber;
			if (int.TryParse(elements[0], out groupnumber) == false) return null;

			int imagenumber;
			if (int.TryParse(elements[1], out imagenumber) == false) return null;

			int offset_x;
			if (int.TryParse(elements[2], out offset_x) == false) return null;

			int offset_y;
			if (int.TryParse(elements[3], out offset_y) == false) return null;

			int gameticks;
			if (int.TryParse(elements[4], out gameticks) == false) return null;

			var flip = SpriteEffects.None;
			if (elements.Length >= 6)
			{
				if (elements[5].IndexOf('H') != -1 || elements[5].IndexOf('h') != -1) flip |= SpriteEffects.FlipHorizontally;
				if (elements[5].IndexOf('V') != -1 || elements[5].IndexOf('v') != -1) flip |= SpriteEffects.FlipVertically;
			}

			var blending = new Blending();
			if (elements.Length >= 7)
			{
				blending = m_animationsystem.GetSubSystem<StringConverter>().Convert<Blending>(elements[6]);
			}

			var clsn = new List<Clsn>();
			clsn.AddRange(loading_type1.Count != 0 ? loading_type1 : default_type1);
			clsn.AddRange(loading_type2.Count != 0 ? loading_type2 : default_type2);

			var element = new AnimationElement(elementid, clsn, gameticks, starttick, new SpriteId(groupnumber, imagenumber), new Point(offset_x, offset_y), flip, blending);
			return element;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AnimationSystem m_animationsystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_animationtitleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_clsnregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_clsnlineregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_elementregex;

		#endregion
	}
}