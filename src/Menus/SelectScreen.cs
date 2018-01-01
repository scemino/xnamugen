using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace xnaMugen.Menus
{
	internal class SelectScreen : NonCombatScreen
	{
		public SelectScreen(MenuSystem screensystem, TextSection textsection, string spritepath, string animationpath, string soundpath)
			: base(screensystem, textsection, spritepath, animationpath, soundpath)
		{
			m_selectmap = new Dictionary<Point, PlayerSelect>();
			m_selectmovemap = new Dictionary<Point, PlayerSelect>();

			m_gridsize = new Point();
			m_gridsize.X = textsection.GetAttribute<int>("columns");
			m_gridsize.Y = textsection.GetAttribute<int>("rows");

			m_wrapping = textsection.GetAttribute<bool>("wrapping");
			m_showemptyboxes = textsection.GetAttribute<bool>("showEmptyBoxes");
			m_moveoveremptyboxes = textsection.GetAttribute<bool>("moveOverEmptyBoxes");
			m_gridposition = textsection.GetAttribute<Point>("pos");
			m_cellsize = textsection.GetAttribute<Point>("cell.size");
			m_cellspacing = textsection.GetAttribute<int>("cell.spacing");

			m_elements = new Elements.Collection(SpriteManager, AnimationManager, SoundManager, MenuSystem.FontMap);
			m_elements.Build(textsection, "cell.bg");
			m_elements.Build(textsection, "cell.random");

			m_cursorblinking = textsection.GetAttribute<bool>("p2.cursor.blink");
			m_soundcancel = textsection.GetAttribute<SoundId>("cancel.snd");
			m_titlelocation = textsection.GetAttribute<Point>("title.offset");
			m_titlefont = textsection.GetAttribute<PrintData>("title.font");
			m_stageposition = textsection.GetAttribute<Point>("stage.pos");
			m_soundstagemove = textsection.GetAttribute<SoundId>("stage.move.snd");
			m_soundstageselect = textsection.GetAttribute<SoundId>("stage.done.snd");
			m_stagefont1 = textsection.GetAttribute<PrintData>("stage.active.font");
			m_stagefont2 = textsection.GetAttribute<PrintData>("stage.active2.font");
			m_stagedonefont = textsection.GetAttribute<PrintData>("stage.done.font");
			m_randomswitchtime = textsection.GetAttribute("cell.random.switchtime", 5);
			m_p1info = new SelectData(this, MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[1], textsection, "p1", m_moveoveremptyboxes);
			m_p2info = new SelectData(this, MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[2], textsection, "p2", m_moveoveremptyboxes);
			m_isdone = false;
			m_stagedisplaybuilder = new StringBuilder();

#warning Hack for now
			VersusMode = "Versus Mode";
		}

		public override void SetInput(Input.InputState inputstate)
		{
			base.SetInput(inputstate);

			inputstate[0].Add(SystemButton.Quit, BackToTitleScreen);

			SetCharacterSelectionInput(m_p1info);
			SetCharacterSelectionInput(m_p2info);
		}

		public override void Reset()
		{
			base.Reset();

			m_blinkval = 0;
			m_p1info.Reset();
			m_p2info.Reset();

			m_stageselected = false;
			m_stageselector = null;
			m_currentstage = -1;

			m_isdone = false;

			m_elements.Reset();
		}

		public override void Update(GameTime gametime)
		{
			base.Update(gametime);

			if (++m_blinkval > 6) m_blinkval = -6;

			m_p1info.Update();
			m_p2info.Update();

			m_elements.Update();

			CheckReady();
		}

		public override void Draw(bool debugdraw)
		{
			base.Draw(debugdraw);

			DrawGrid();
			DrawFace(m_p1info);
			DrawFace(m_p2info);
			DrawStage();
			Print(m_titlefont, (Vector2)m_titlelocation, m_title, null);
		}

		private void DrawStage()
		{
			if (m_stageselector == null) return;

			var pd = m_blinkval > 0 ? m_stagefont1 : m_stagefont2;
			if (m_stageselected) pd = m_stagedonefont;

			var sp = m_currentstage >= 0 && m_currentstage < StageProfiles.Count ? StageProfiles[m_currentstage] : null;

			m_stagedisplaybuilder.Length = 0;
			if (sp != null)
			{
				m_stagedisplaybuilder.AppendFormat("Stage {0}: {1}", m_currentstage + 1, sp.Name);
			}
			else
			{
				m_stagedisplaybuilder.Append("Stage: Random");
			}

			Print(pd, (Vector2)m_stageposition, m_stagedisplaybuilder.ToString(), null);
		}

		private void DrawGrid()
		{
			var cellbg = m_elements.GetElement("cell.bg") as Elements.StaticImage;
			if (cellbg != null)
			{
				var sprite = cellbg.SpriteManager.GetSprite(cellbg.DataMap.SpriteId);
				if (sprite != null)
				{
					var drawstate = cellbg.SpriteManager.DrawState;
					drawstate.Reset();
					drawstate.Set(sprite);

					for (var y = 0; y != m_gridsize.Y; ++y)
					{
						for (var x = 0; x != m_gridsize.X; ++x)
						{
							var location = m_gridposition;
							location.X += (m_cellsize.X + m_cellspacing) * x;
							location.Y += (m_cellsize.Y + m_cellspacing) * y;

							var selection = GetSelection(new Point(x, y), false);
							if (selection == null && m_showemptyboxes == false) continue;

							drawstate.AddData((Vector2)location, null);
						}
					}

					drawstate.Use();
				}
			}

			for (var y = 0; y != m_gridsize.Y; ++y)
			{
				for (var x = 0; x != m_gridsize.X; ++x)
				{
					var xy = new Point(x, y);

					var location = (Vector2)m_gridposition;
					location.X += (m_cellsize.X + m_cellspacing) * x;
					location.Y += (m_cellsize.Y + m_cellspacing) * y;

					var selection = GetSelection(xy, false);
					if (selection != null && selection.SelectionType == PlayerSelectType.Profile) selection.Profile.SpriteManager.Draw(SpriteId.SmallPortrait, location, Vector2.Zero, Vector2.One, SpriteEffects.None);

					if (selection != null && selection.SelectionType == PlayerSelectType.Random)
					{
						var randomimage = m_elements.GetElement("cell.random") as Elements.StaticImage;
						if (randomimage != null) randomimage.Draw(location);
					}

					if (m_p1info.CurrentCell == xy && m_p2info.CurrentCell == xy)
					{
						if (m_blinkval > 0) m_p1info.DrawCursorActive(location);
						else m_p2info.DrawCursorActive(location);
					}
					else if (m_p1info.CurrentCell == xy)
					{
						m_p1info.DrawCursorActive(location);
					}
					else if (m_p2info.CurrentCell == xy)
					{
						m_p2info.DrawCursorActive(location);
					}
				}
			}
		}

		private void DrawFace(SelectData data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			var selection = GetSelection(data.CurrentCell, false);
			if (selection == null) return;

			if (selection.SelectionType == PlayerSelectType.Profile)
			{
				data.DrawProfile(selection.Profile);
			}

			if (selection.SelectionType == PlayerSelectType.Random)
			{
			}
		}

		private void SetCharacterSelectionInput(SelectData data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			data.ButtonMap.Clear();

			data.ButtonMap.Add(PlayerButton.Up, x => { if (x) MoveCharacterSelection(data, CursorDirection.Up); });
			data.ButtonMap.Add(PlayerButton.Down, x => { if (x) MoveCharacterSelection(data, CursorDirection.Down); });
			data.ButtonMap.Add(PlayerButton.Left, x => { if (x) MoveCharacterSelection(data, CursorDirection.Left); });
			data.ButtonMap.Add(PlayerButton.Right, x => { if (x) MoveCharacterSelection(data, CursorDirection.Right); });
			data.ButtonMap.Add(PlayerButton.Taunt, x => { CharacterPalletteShift(data, x); });
			data.ButtonMap.Add(PlayerButton.A, x => { if (x) SelectCharacter(data, 0); });
			data.ButtonMap.Add(PlayerButton.B, x => { if (x) SelectCharacter(data, 1); });
			data.ButtonMap.Add(PlayerButton.C, x => { if (x) SelectCharacter(data, 2); });
			data.ButtonMap.Add(PlayerButton.X, x => { if (x) SelectCharacter(data, 3); });
			data.ButtonMap.Add(PlayerButton.Y, x => { if (x) SelectCharacter(data, 4); });
			data.ButtonMap.Add(PlayerButton.Z, x => { if (x) SelectCharacter(data, 5); });
		}

		private void CharacterPalletteShift(SelectData data, bool pressed)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			if (pressed)
			{
				data.PaletteIndex += 6;
			}
			else
			{
				data.PaletteIndex -= 6;
			}
		}

		private void MoveCharacterSelection(SelectData data, CursorDirection direction)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			if (data.MoveCharacterCursor(direction, m_gridsize, m_wrapping))
			{
				data.PlayCursorMoveSound();
			}
		}

		private void SelectCharacter(SelectData data, int index)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			var selection = GetSelection(data.CurrentCell, false);
			if (selection == null || selection.SelectionType != PlayerSelectType.Profile) return;

			data.PlaySelectSound();
			data.ButtonMap.Clear();
			data.IsSelected = true;
			data.PaletteIndex += index;

			SetStageSelectionInput(data);
		}

		private void SetStageSelectionInput(SelectData data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			if (m_stageselector != null) return;

			data.ButtonMap.Clear();

			data.ButtonMap.Add(PlayerButton.Left, delegate(bool pressed) { if (pressed) { MoveStageSelection(-1); } });
			data.ButtonMap.Add(PlayerButton.Right, delegate(bool pressed) { if (pressed) { MoveStageSelection(+1); } });
			data.ButtonMap.Add(PlayerButton.A, SelectCurrentStage);
			data.ButtonMap.Add(PlayerButton.B, SelectCurrentStage);
			data.ButtonMap.Add(PlayerButton.C, SelectCurrentStage);
			data.ButtonMap.Add(PlayerButton.X, SelectCurrentStage);
			data.ButtonMap.Add(PlayerButton.Y, SelectCurrentStage);
			data.ButtonMap.Add(PlayerButton.Z, SelectCurrentStage);

			m_stageselector = data;
			m_currentstage = 0;
		}

		private void MoveStageSelection(int offset)
		{
			if (offset == 0) return;
			SoundManager.Play(m_soundstagemove);

			offset = offset % StageProfiles.Count;

			if (offset > 0)
			{
				m_currentstage += offset;
				if (m_currentstage >= StageProfiles.Count)
				{
					var diff = m_currentstage - StageProfiles.Count;
					m_currentstage = -1 + diff;
				}
			}

			if (offset < 0)
			{
				m_currentstage += offset;
				if (m_currentstage < -1)
				{
					var diff = m_currentstage + 2;
					m_currentstage = StageProfiles.Count - 1 + diff;
				}
			}
		}

		private void SelectCurrentStage(bool pressed)
		{
			if (pressed)
			{
				m_stageselected = true;
				SoundManager.Play(m_soundstageselect);

				m_stageselector.ButtonMap.Clear();
			}
		}

		private void CheckReady()
		{
			if (m_isdone) return;
			if (m_stageselected == false || m_p1info.IsSelected == false || m_p2info.IsSelected == false) return;

			m_isdone = true;

			if (m_currentstage == -1) m_currentstage = MenuSystem.GetSubSystem<Random>().NewInt(0, StageProfiles.Count - 1);

			var p1index = m_p1info.CurrentCell.Y * m_gridsize.X + m_p1info.CurrentCell.X;
			var p2index = m_p2info.CurrentCell.Y * m_gridsize.X + m_p2info.CurrentCell.X;

			var p1 = PlayerProfiles[p1index];
			var p2 = PlayerProfiles[p2index];
			var stage = StageProfiles[m_currentstage];

			var init = new Combat.EngineInitialization(CombatMode.Versus, p1.Profile, m_p1info.PaletteIndex, p2.Profile, m_p2info.PaletteIndex, stage);

			MenuSystem.PostEvent(new Events.SetupCombat(init));
			MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Versus));
		}

		private void BackToTitleScreen(bool pressed)
		{
			if (pressed)
			{
				SoundManager.Play(m_soundcancel);

				MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Title));
			}
		}

		public PlayerSelect GetSelection(Point location, bool movement)
		{
			if (location.X < 0 || location.X >= m_gridsize.X || location.Y < 0 || location.Y >= m_gridsize.Y) return null;

			PlayerSelect select = null;
			var map = movement ? m_selectmovemap : m_selectmap;

			if (map.TryGetValue(location, out @select)) return select;

			var index = 0;
			var profiles = PlayerProfiles;

			for (var y = 0; y != m_gridsize.Y; ++y)
			{
				for (var x = 0; x != m_gridsize.X; ++x)
				{
					var selection = index < profiles.Count ? profiles[index] : null;
					++index;

					if (location == new Point(x, y))
					{
						if (selection == null && movement && m_moveoveremptyboxes == false) return null;

						map[location] = selection;
						return selection;
					}

					//if (selection == null && movement == true && m_moveoveremptyboxes == false) continue;
					//if (location == new Point(x, y)) return selection;
				}
			}

			return null;
		}

		public ListIterator<StageProfile> StageProfiles => MenuSystem.GetSubSystem<ProfileLoader>().StageProfiles;

		public ListIterator<PlayerSelect> PlayerProfiles => MenuSystem.GetSubSystem<ProfileLoader>().PlayerProfiles;

		public string VersusMode
		{
			get => m_title;

			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));

				m_title = value;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_gridsize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_wrapping;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_gridposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_cellsize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_cellspacing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_cursorblinking;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_blinkval;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundcancel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundstagemove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundstageselect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string m_title;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_titlelocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_titlefont;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_randomswitchtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SelectData m_p1info;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SelectData m_p2info;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_stageposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_stagefont1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_stagefont2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_stagedonefont;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_showemptyboxes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_moveoveremptyboxes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Elements.Collection m_elements;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StringBuilder m_stagedisplaybuilder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_currentstage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_stageselected;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private SelectData m_stageselector;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isdone;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<Point, PlayerSelect> m_selectmap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<Point, PlayerSelect> m_selectmovemap;

		#endregion
	}
}