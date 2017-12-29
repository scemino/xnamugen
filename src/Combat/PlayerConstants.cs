using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal class PlayerConstants
	{
		public PlayerConstants(Player player, TextFile textfile)
		{
			if (player == null) throw new ArgumentNullException(nameof(player));
			if (textfile == null) throw new ArgumentNullException(nameof(textfile));

			var datasection = textfile.GetSection("Data");
			var sizesection = textfile.GetSection("Size");
			var velocitysection = textfile.GetSection("Velocity");
			var movementsection = textfile.GetSection("Movement");

			var converter = player.Engine.GetSubSystem<StringConverter>();

			if (datasection == null) throw new ArgumentException("Constants file '" + textfile.Filepath + "' does not have a 'Data' section");
			if (sizesection == null) throw new ArgumentException("Constants file '" + textfile.Filepath + "' does not have a 'Size' section");
			if (velocitysection == null) throw new ArgumentException("Constants file '" + textfile.Filepath + "' does not have a 'Velocity' section");
			if (movementsection == null) throw new ArgumentException("Constants file '" + textfile.Filepath + "' does not have a 'Movement' section");

			m_scale = new Vector2(1, 1);

			m_life = datasection.GetAttribute("life", 1000);
			m_maxpower = datasection.GetAttribute("power", 3000);
			m_attackpower = datasection.GetAttribute("attack", 100);
			m_defensivepower = datasection.GetAttribute("defence", 100);
			m_falldefenseincrease = datasection.GetAttribute("fall.defence_up", 50);
			m_liedowntime = datasection.GetAttribute("liedown.time", 60);
			m_airjuggle = datasection.GetAttribute("airjuggle", 15);
			m_defaultspark = datasection.GetAttribute<Evaluation.PrefixedExpression>("sparkno", null);
			m_defaultguardspark = datasection.GetAttribute<Evaluation.PrefixedExpression>("guard.sparkno", null);
			m_KOecho = datasection.GetAttribute("KO.echo", false);
			m_volumeoffset = datasection.GetAttribute("volume", 0);
			m_persistanceintindex = datasection.GetAttribute("IntPersistIndex", 60);
			m_persistancefloatindex = datasection.GetAttribute("FloatPersistIndex", 40);

			m_scale.X = sizesection.GetAttribute("xscale", 1.0f);
			m_scale.Y = sizesection.GetAttribute("yscale", 1.0f);
			m_groundback = sizesection.GetAttribute("ground.back", 15);
			m_groundfront = sizesection.GetAttribute("ground.front", 16);
			m_airback = sizesection.GetAttribute("air.back", 12);
			m_airfront = sizesection.GetAttribute("air.front", 12);
			m_height = sizesection.GetAttribute("height", 60);
			m_attackdistance = sizesection.GetAttribute("attack.dist", 160);
			m_projectileattackdist = sizesection.GetAttribute("proj.attack.dist", 90);
			m_projectilescaling = sizesection.GetAttribute("proj.doscale", false);
			m_headposition = sizesection.GetAttribute("head.pos", Vector2.Zero);
			m_midposition = sizesection.GetAttribute("mid.pos", Vector2.Zero);
			m_shadowoffset = sizesection.GetAttribute("shadowoffset", 0);
			m_drawoffset = sizesection.GetAttribute("draw.offset", new Point(0, 0));

			m_walk_forward = velocitysection.GetAttribute<float>("walk.fwd");
			m_walk_back = velocitysection.GetAttribute<float>("walk.back");
			m_run_fwd = velocitysection.GetAttribute<Vector2>("run.fwd");
			m_run_back = velocitysection.GetAttribute<Vector2>("run.back");

			m_jump_neutral = velocitysection.GetAttribute<Vector2>("jump.neu");

			m_jump_back = velocitysection.GetAttribute<Vector2>("jump.back");
			if (m_jump_back.Y == 0) m_jump_back.Y = m_jump_neutral.Y;

			m_jump_forward = velocitysection.GetAttribute<Vector2>("jump.fwd");
			if (m_jump_forward.Y == 0) m_jump_forward.Y = m_jump_neutral.Y;

			m_runjump_back = velocitysection.GetAttribute("runjump.back", m_run_back);
			m_runjump_fwd = velocitysection.GetAttribute("runjump.fwd", m_run_fwd);
			m_airjump_neutral = velocitysection.GetAttribute("airjump.neu", Jump_neutral);

			m_airjump_back = velocitysection.GetAttribute("airjump.back", Jump_back);
			if (m_airjump_back.Y == 0) m_airjump_back.Y = m_airjump_neutral.Y;

			m_airjump_forward = velocitysection.GetAttribute("airjump.fwd", Jump_forward);
			if (m_airjump_forward.Y == 0) m_airjump_forward.Y = m_airjump_neutral.Y;

			m_airjumps = movementsection.GetAttribute("airjump.num", 0);
			m_airjumpheight = movementsection.GetAttribute("airjump.height", 0);
			m_vert_acceleration = movementsection.GetAttribute<float>("yaccel");
			m_standfriction = movementsection.GetAttribute<float>("stand.friction");
			m_crouchfriction = movementsection.GetAttribute<float>("crouch.friction");

			m_airjump_forward.Y = m_airjump_neutral.Y;
			m_airjump_back.Y = m_airjump_neutral.Y;
		}

		public int MaximumLife => m_life;

		public int MaximumPower => m_maxpower;

		public int AttackPower => m_attackpower;

		public int DefensivePower => m_defensivepower;

		public int FallDefenseIncrease => m_falldefenseincrease;

		public int LieDownTime => m_liedowntime;

		public int AirJuggle => m_airjuggle;

		public Evaluation.PrefixedExpression DefaultSparkNumber => m_defaultspark;

		public Evaluation.PrefixedExpression DefaultGuardSparkNumber => m_defaultguardspark;

		public bool KOEcho => m_KOecho;

		public int VolumeOffset => m_volumeoffset;

		public int PersistanceIntIndex => m_persistanceintindex;

		public int PersistanceFloatIndex => m_persistancefloatindex;

		public Vector2 Scale => m_scale;

		public int GroundBack => m_groundback;

		public int GroundFront => m_groundfront;

		public int Airback => m_airback;

		public int Airfront => m_airfront;

		public int Height => m_height;

		public int Attackdistance => m_attackdistance;

		public int Projectileattackdist => m_projectileattackdist;

		public bool ProjectileScaling => m_projectilescaling;

		public Vector2 Headposition => m_headposition;

		public Vector2 Midposition => m_midposition;

		public int Shadowoffset => m_shadowoffset;

		public Point Drawoffset => m_drawoffset;

		public float Walk_forward => m_walk_forward;

		public float Walk_back => m_walk_back;

		public Vector2 Run_fwd => m_run_fwd;

		public Vector2 Run_back => m_run_back;

		public Vector2 Jump_neutral => m_jump_neutral;

		public Vector2 Jump_back => m_jump_back;

		public Vector2 Jump_forward => m_jump_forward;

		public Vector2 Runjump_back => m_runjump_back;

		public Vector2 Runjump_fwd => m_runjump_fwd;

		public Vector2 Airjump_neutral => m_airjump_neutral;

		public Vector2 Airjump_back => m_airjump_back;

		public Vector2 Airjump_forward => m_airjump_forward;

		public int Airjumps => m_airjumps;

		public int Airjumpheight => m_airjumpheight;

		public float Vert_acceleration => m_vert_acceleration;

		public float Standfriction => m_standfriction;

		public float Crouchfriction => m_crouchfriction;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_life;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_maxpower;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_attackpower;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_defensivepower;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_falldefenseincrease;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_liedowntime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_airjuggle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_defaultspark;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_defaultguardspark;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_KOecho;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_volumeoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_persistanceintindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_persistancefloatindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_scale; // Draw scaling factor

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_groundback; // Player width (back, ground)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_groundfront; // Player width (front, ground)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_airback; // Player width (back, air)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_airfront; //Player width (front, air)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_height; // Height of player (for opponent to jump over)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_attackdistance; // Default attack distance

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_projectileattackdist; //Default attack distance for projectiles

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_projectilescaling; //Set to scale projectiles too

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_headposition; // Approximate position of head

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_midposition; // Approximate position of midsection

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_shadowoffset; // Number of pixels to vertically offset the shadow

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_drawoffset; // Player drawing offset in pixels	

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly float m_walk_forward; // Walk forward

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly float m_walk_back; // Walk backward

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_run_fwd; // Run forward (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_run_back; // Hop backward (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_jump_neutral; // Neutral jumping velocity (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_jump_back; // Jump back Speed (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_jump_forward; // Jump forward Speed (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_runjump_back; // Running jump speeds (opt)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_runjump_fwd; //

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_airjump_neutral; //

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_airjump_back;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_airjump_forward;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_airjumps; // Number of air jumps allowed (opt)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_airjumpheight; // Minimum distance from ground before you can air jump (opt)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly float m_vert_acceleration; // Vertical acceleration

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly float m_standfriction; // Friction coefficient when standing

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly float m_crouchfriction; // Friction coefficient when crouching

		#endregion
	}
}