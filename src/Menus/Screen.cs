using System;
using System.Diagnostics;
using xnaMugen.Drawing;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
	internal abstract class Screen : Resource
	{
		protected Screen(MenuSystem menusystem)
		{
			if (menusystem == null) throw new ArgumentNullException(nameof(menusystem));

			m_menusystem = menusystem;
		}

		public virtual void SetInput(Input.InputState inputstate)
		{
			if (inputstate == null) throw new ArgumentNullException(nameof(inputstate));

			MenuSystem.GetSubSystem<Input.InputSystem>().SaveInputState();
		}

		public virtual void FadingIn()
		{
			SetInput(MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput);
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

		public virtual void Draw(bool debugdraw)
		{
		}

		public void Print(PrintData printdata, Vector2 location, string text, Rectangle? scissorrect)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			MenuSystem.FontMap.Print(printdata, location, text, scissorrect);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}

		public MenuSystem MenuSystem => m_menusystem;

		public abstract int FadeInTime { get; }

		public abstract int FadeOutTime { get; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly MenuSystem m_menusystem;

		#endregion
	}
}