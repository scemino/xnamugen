using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.StateMachine;

namespace xnaMugen.Combat
{
	class CharacterBind
	{
		public CharacterBind(Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			m_character = character;
			m_bindcharacter = null;
			m_totaltime = 0;
            m_time = 0;
			m_offset = new Vector2(0, 0);
			m_facingflag = 0;
			m_isactive = false;
			m_istargetbind = false;
		}

		public void Reset()
		{
			m_bindcharacter = null;
			m_totaltime = 0;
            m_time = 0;
            m_offset = new Vector2(0, 0);
			m_facingflag = 0;
			m_isactive = false;
			m_istargetbind = false;
		}

		public void Update()
		{
			if (IsActive == true && (TotalTime == -1 || TimeInBind < TotalTime) && HelperCheck())
			{
                ++m_time;

				Bind();
			}
			else
			{
				Reset();
			}
		}

		public void Set(Character bindcharacter, Vector2 offset, Int32 time, Int32 facingflag, Boolean targetbind)
		{
			if (bindcharacter == null) throw new ArgumentNullException("bindcharacter");

			m_bindcharacter = bindcharacter;
			m_totaltime = time;
			m_offset = offset;
			m_facingflag = facingflag;
			m_istargetbind = targetbind;
			m_isactive = true;
		}

		Boolean HelperCheck()
		{
			if (IsActive == false) return false;

			Helper bindhelper = BindTo as Helper;
			if (bindhelper == null) return true;

			if (bindhelper.RemoveCheck() == true)
			{
				Reset();
				return false;
			}

			return true;
		}

		void Bind()
		{
			if (BindTo == null) throw new InvalidOperationException();

			Character.CurrentLocation = Misc.GetOffset(BindTo.CurrentLocation, BindTo.CurrentFacing, Offset);

			Character.CurrentVelocity = BindTo.CurrentVelocity;
			Character.CurrentAcceleration = BindTo.CurrentAcceleration;

			if (FacingFlag > 0) Character.CurrentFacing = BindTo.CurrentFacing;
			if (FacingFlag < 0) Character.CurrentFacing = Misc.FlipFacing(BindTo.CurrentFacing);
		}

		public Boolean IsActive
		{
			get { return m_isactive; }
		}

		public Character Character
		{
			get { return m_character; }
		}

		public Character BindTo
		{
			get { return m_bindcharacter; }
		}

		public Int32 TotalTime
		{
			get { return m_totaltime; }
		}

        public Int32 TimeInBind
        {
            get { return m_time; }
        }

		public Vector2 Offset
		{
			get { return m_offset; }
		}

		public Int32 FacingFlag
		{
			get { return m_facingflag; }
		}

		public Boolean IsTargetBind
		{
			get { return m_istargetbind; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Character m_character;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Character m_bindcharacter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_totaltime;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_offset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_facingflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_istargetbind;

		#endregion
	}
}