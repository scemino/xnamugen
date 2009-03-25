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
	class AnimationLoader
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="animationsystem">The animation system.</param>
		public AnimationLoader(AnimationSystem animationsystem)
		{
			if (animationsystem == null) throw new ArgumentNullException("animationsystem");

			m_animationsystem = animationsystem;
			m_animationtitleregex = new Regex(@"^\s*Begin action\s+(-?\d+)(,.+)?\s*$", RegexOptions.IgnoreCase);
			m_clsnregex = new Regex(@"Clsn([12])(Default)?:\s*(\d+)", RegexOptions.IgnoreCase);
			m_clsnlineregex = new Regex(@"Clsn([12])?\[(-?\d+)\]\s*=\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)", RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Creates Animations from a xnaMugen.IO.TextFile.
		/// </summary>
		/// <param name="textfile">A textfile whose xnaMugen.IO.TextSection can be used to create Animations.</param>
		/// <returns>A collection of Animations created from the supplied textfile.</returns>
		public KeyedCollection<Int32, Animation> LoadAnimations(TextFile textfile)
		{
			if (textfile == null) throw new ArgumentNullException("textfile");

			KeyedCollection<Int32, Animation> animations = new KeyedCollection<Int32, Animation>(x => x.Number);

			foreach (TextSection section in textfile)
			{
				Animation animation = CreateAnimation(section);
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
		public Boolean IsClsnLine(String line)
		{
			if (line == null) throw new ArgumentNullException("line");

			return m_clsnlineregex.IsMatch(line);
		}

		/// <summary>
		/// Creates a new Animation.
		/// </summary>
		/// <param name="section">The information used to intialize the Animation.</param>
		/// <returns>A new instance of the Animation class if it could be created; null otherwise.</returns>
		Animation CreateAnimation(TextSection section)
		{
			if (section == null) throw new ArgumentNullException("section");

			Match titlematch = m_animationtitleregex.Match(section.Title);
			if (titlematch.Success == false) return null;

			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			Int32 animationnumber = Int32.Parse(titlematch.Groups[1].Value);
			Int32 loopstart = 0;
			List<AnimationElement> elements = new List<AnimationElement>();

			List<Clsn> loading_type1 = new List<Clsn>();
			List<Clsn> loading_type2 = new List<Clsn>();
			List<Clsn> default_type1 = new List<Clsn>();
			List<Clsn> default_type2 = new List<Clsn>();

			Boolean loaddefault = false;
			ClsnType loadtype = ClsnType.None;
			Int32 loadcount = 0;

			foreach (String line in section.Lines)
			{
				if (loadcount > 0)
				{
					Clsn? clsn = CreateClsn(line, loadtype);
					if (clsn != null)
					{
						if (loaddefault == true)
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

				Match clsnmatch = m_clsnregex.Match(line);
				if (clsnmatch.Success == true)
				{
					ClsnType clsntype = ClsnType.None;
					if (sc.Equals(clsnmatch.Groups[1].Value, "1") == true) clsntype = ClsnType.Type1Attack;
					if (sc.Equals(clsnmatch.Groups[1].Value, "2") == true) clsntype = ClsnType.Type2Normal;

					Boolean isdefault = sc.Equals(clsnmatch.Groups[2].Value, "default");
					if (isdefault == true)
					{
						if (clsntype == ClsnType.Type1Attack) default_type1.Clear();
						if (clsntype == ClsnType.Type2Normal) default_type2.Clear();
					}

					loadcount = Int32.Parse(clsnmatch.Groups[3].Value);
					loaddefault = isdefault;
					loadtype = clsntype;
					continue;
				}

				if (sc.Equals(line, "Loopstart") == true)
				{
					loopstart = elements.Count;
					continue;
				}

				AnimationElement element = CreateElement(line, elements.Count, default_type1, default_type2, loading_type1, loading_type2);
				if (element != null)
				{
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
			else
			{
				if (loopstart == elements.Count) loopstart = 0;

				return new Animation(animationnumber, loopstart, elements);
			}
		}

		/// <summary>
		/// Creates a new collision box.
		/// </summary>
		/// <param name="line">A line of text representing a collision box.</param>
		/// <param name="overridetype">The type of collision box to be created.</param>
		/// <returns>A new instance of the Clsn class if it could be created; null otherwise.</returns>
		Clsn? CreateClsn(String line, ClsnType overridetype)
		{
			if (String.Compare(line, 0, "Clsn", 0, 4, StringComparison.OrdinalIgnoreCase) != 0) return null;

			Match match = m_clsnlineregex.Match(line);
			if (match.Success == false) return null;

			//if (String.Equals(match.Groups[1].Value, "1") == true) clsn.Type = ClsnType.Type1Attack;
			//if (String.Equals(match.Groups[1].Value, "2") == true) clsn.Type = ClsnType.Type2Normal;

			Int32 x1 = Int32.Parse(match.Groups[3].Value);
			Int32 y1 = Int32.Parse(match.Groups[4].Value);
			Int32 x2 = Int32.Parse(match.Groups[5].Value);
			Int32 y2 = Int32.Parse(match.Groups[6].Value);

			if (x1 > x2) Misc.Swap(ref x1, ref x2);
			if (y1 > y2) Misc.Swap(ref y1, ref y2);

			return new Clsn(overridetype, new Rectangle(x1, y1, x2 - x1, y2 - y1));
		}

		/// <summary>
		/// Parsed a comma delimited line for the creation of a AnimationElement.
		/// </summary>
		/// <param name="line">A line to be parsed.</param>
		/// <returns>A String array of 7 elements is the line can be parsed; null otherwise.</returns>
		String[] ParseElementLine(String line)
		{
			if (line == null) throw new ArgumentNullException("line");

			StringSubString groupnumber = new StringSubString(line, 0, 0);
			while (groupnumber.EndIndex < line.Length && line[groupnumber.EndIndex] != ',') ++groupnumber.EndIndex;

			StringSubString imagenumber = new StringSubString(line, groupnumber.EndIndex + 1, groupnumber.EndIndex + 1);
			while (imagenumber.EndIndex < line.Length && line[imagenumber.EndIndex] != ',') ++imagenumber.EndIndex;

			StringSubString offsetx = new StringSubString(line, imagenumber.EndIndex + 1, imagenumber.EndIndex + 1);
			while (offsetx.EndIndex < line.Length && line[offsetx.EndIndex] != ',') ++offsetx.EndIndex;

			StringSubString offsety = new StringSubString(line, offsetx.EndIndex + 1, offsetx.EndIndex + 1);
			while (offsety.EndIndex < line.Length && line[offsety.EndIndex] != ',') ++offsety.EndIndex;

			StringSubString ticks = new StringSubString(line, offsety.EndIndex + 1, offsety.EndIndex + 1);
			while (ticks.EndIndex < line.Length && line[ticks.EndIndex] != ',') ++ticks.EndIndex;

			StringSubString spriteflip = new StringSubString(line, ticks.EndIndex + 1, ticks.EndIndex + 1);
			while (spriteflip.EndIndex < line.Length && line[spriteflip.EndIndex] != ',') ++spriteflip.EndIndex;

			StringSubString blend = new StringSubString(line, spriteflip.EndIndex + 1, spriteflip.EndIndex + 1);
			while (blend.EndIndex < line.Length && line[blend.EndIndex] != ',') ++blend.EndIndex;

			groupnumber.TrimWhitespace();
			imagenumber.TrimWhitespace();
			offsetx.TrimWhitespace();
			offsety.TrimWhitespace();
			ticks.TrimWhitespace();
			spriteflip.TrimWhitespace();
			blend.TrimWhitespace();

			if (groupnumber.Length == 0 || imagenumber.Length == 0 || offsetx.Length == 0 || offsety.Length == 0 || ticks.Length == 0)
			{
				return null;
			}
			else
			{
				return new String[] { groupnumber.ToString(), imagenumber.ToString(), offsetx.ToString(), offsety.ToString(), ticks.ToString(), spriteflip.ToString(), blend.ToString() };
			}
		}

		/// <summary>
		/// Creates a new AnimationElement initialized from a line of text.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="elementid"></param>
		/// <param name="default_clsn"></param>
		/// <param name="loading_clsn"></param>
		/// <returns></returns>
		AnimationElement CreateElement(String line, Int32 elementid, List<Clsn> default_type1, List<Clsn> default_type2, List<Clsn> loading_type1, List<Clsn> loading_type2)
		{
			if (line == null) throw new ArgumentNullException("line");
			if (elementid < 0) throw new ArgumentOutOfRangeException("elementid");
			if (default_type1 == null) throw new ArgumentNullException("default_type1");
			if (default_type2 == null) throw new ArgumentNullException("default_type2");
			if (loading_type1 == null) throw new ArgumentNullException("loading_type1");
			if (loading_type2 == null) throw new ArgumentNullException("loading_type2");

			String[] elements = ParseElementLine(line);
			if (elements == null) return null;

			try
			{
				Int32 gameticks = Int32.Parse(elements[4]);
				SpriteId sprite_id = new SpriteId(Int32.Parse(elements[0]), Int32.Parse(elements[1]));
				Point offset = new Point(Int32.Parse(elements[2]), Int32.Parse(elements[3]));

				SpriteEffects flip = SpriteEffects.None;
				if (elements[5].IndexOf('H') != -1 || elements[5].IndexOf('h') != -1) flip |= SpriteEffects.FlipHorizontally;
				if (elements[5].IndexOf('V') != -1 || elements[5].IndexOf('v') != -1) flip |= SpriteEffects.FlipVertically;

				Blending blending = new Blending();
				if (String.IsNullOrEmpty(elements[6]) == false)
				{
					blending = m_animationsystem.GetSubSystem<StringConverter>().Convert<Blending>(elements[6]);
				}

				List<Clsn> clsn = new List<Clsn>();
				clsn.AddRange(loading_type1.Count != 0 ? loading_type1 : default_type1);
				clsn.AddRange(loading_type2.Count != 0 ? loading_type2 : default_type2);

				AnimationElement element = new AnimationElement(elementid, clsn, gameticks, sprite_id, offset, flip, blending);
				return element;
			}
			catch(FormatException)
			{
				return null;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AnimationSystem m_animationsystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_animationtitleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_clsnregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_clsnlineregex;

		#endregion
	}
}