using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal class CharacterBind
	{
		public CharacterBind(Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			m_character = character;
			m_bindcharacter = null;
			m_time = 0;
			m_offset = new Vector2(0, 0);
			m_facingflag = 0;
			m_isactive = false;
			m_istargetbind = false;
		}

		public void Reset()
		{
			m_bindcharacter = null;
			m_time = 0;
			m_offset = new Vector2(0, 0);
			m_facingflag = 0;
			m_isactive = false;
			m_istargetbind = false;
		}

		public void Update()
		{
			if (IsActive && (Time == -1 || Time > 0) && HelperCheck())
			{
				if (Time > 0) --m_time;

				Bind();
			}
			else
			{
				Reset();
			}
		}

		public void Set(Character bindcharacter, Vector2 offset, int time, int facingflag, bool targetbind)
		{
			if (bindcharacter == null) throw new ArgumentNullException(nameof(bindcharacter));

			m_bindcharacter = bindcharacter;
			m_time = time;
			m_offset = offset;
			m_facingflag = facingflag;
			m_istargetbind = targetbind;
			m_isactive = true;
		}

		private bool HelperCheck()
		{
			if (IsActive == false) return false;

			var bindhelper = BindTo as Helper;
			if (bindhelper == null) return true;

			if (bindhelper.RemoveCheck())
			{
				Reset();
				return false;
			}

			return true;
		}

		private void Bind()
		{
			if (BindTo == null) throw new InvalidOperationException();

			Character.CurrentLocation = Misc.GetOffset(BindTo.CurrentLocation, BindTo.CurrentFacing, Offset);

			Character.CurrentVelocity = BindTo.CurrentVelocity;
			Character.CurrentAcceleration = BindTo.CurrentAcceleration;

			if (FacingFlag > 0) Character.CurrentFacing = BindTo.CurrentFacing;
			if (FacingFlag < 0) Character.CurrentFacing = Misc.FlipFacing(BindTo.CurrentFacing);
		}

		public bool IsActive => m_isactive;

		public Character Character => m_character;

		public Character BindTo => m_bindcharacter;

		public int Time => m_time;

		public Vector2 Offset => m_offset;

		public int FacingFlag => m_facingflag;

		public bool IsTargetBind => m_istargetbind;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Character m_character;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Character m_bindcharacter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_offset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_facingflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_istargetbind;

		#endregion
	}
}