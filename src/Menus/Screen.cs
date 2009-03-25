using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
	abstract class Screen : Resource
	{
		protected Screen(MenuSystem menusystem)
		{
			if (menusystem == null) throw new ArgumentNullException("menusystem");

			m_menusystem = menusystem;
		}

		public virtual void SetInput(Input.InputState inputstate)
		{
			if (inputstate == null) throw new ArgumentNullException("inputstate");
		}

		public virtual void FadingIn()
		{
		}

		public virtual void FadeInComplete()
		{
		}

		public virtual void FadingOut()
		{
		}

		public virtual void FadeOutComplete()
		{
		}

		public virtual void Reset()
		{
		}

		public virtual void Update(GameTime gametime)
		{
		}

		public virtual void Draw(Boolean debugdraw)
		{
		}

		public void Print(PrintData printdata, Vector2 location, String text, Rectangle? scissorrect)
		{
			if (text == null) throw new ArgumentNullException("text");

			MenuSystem.FontMap.Print(printdata, location, text, scissorrect);
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
			}

			base.Dispose(disposing);
		}

		public MenuSystem MenuSystem
		{
			get { return m_menusystem; }
		}

		public abstract Int32 FadeInTime { get; }

		public abstract Int32 FadeOutTime { get; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly MenuSystem m_menusystem;

		#endregion
	}
}