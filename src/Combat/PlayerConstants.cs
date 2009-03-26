using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;
using System.Collections.Generic;
using System.Text;

namespace xnaMugen.Combat
{
	class PlayerConstants
	{
		public PlayerConstants(Player player, TextFile textfile)
		{
			if (player == null) throw new ArgumentNullException("player");
			if (textfile == null) throw new ArgumentNullException("textfile");

			TextSection datasection = textfile.GetSection("Data");
			TextSection sizesection = textfile.GetSection("Size");
			TextSection velocitysection = textfile.GetSection("Velocity");
			TextSection movementsection = textfile.GetSection("Movement");

			StringConverter converter = player.Engine.GetSubSystem<StringConverter>();

			if (datasection == null) throw new ArgumentException("Constants file '" + textfile.Filepath + "' does not have a 'Data' section");
			if (sizesection == null) throw new ArgumentException("Constants file '" + textfile.Filepath + "' does not have a 'Size' section");
			if (velocitysection == null) throw new ArgumentException("Constants file '" + textfile.Filepath + "' does not have a 'Velocity' section");
			if (movementsection == null) throw new ArgumentException("Constants file '" + textfile.Filepath + "' does not have a 'Movement' section");

			m_scale = new Vector2(1, 1);

			m_life = datasection.GetAttribute<Int32>("life", 1000);
			m_maxpower = datasection.GetAttribute<Int32>("power", 3000);
			m_attackpower = datasection.GetAttribute<Int32>("attack", 100);
			m_defensivepower = datasection.GetAttribute<Int32>("defence", 100);
			m_falldefenseincrease = datasection.GetAttribute<Int32>("fall.defence_up", 50);
			m_liedowntime = datasection.GetAttribute<Int32>("liedown.time", 60);
			m_airjuggle = datasection.GetAttribute<Int32>("airjuggle", 15);
			m_defaultspark = datasection.GetAttribute<Evaluation.PrefixedExpression>("sparkno", null);
			m_defaultguardspark = datasection.GetAttribute<Evaluation.PrefixedExpression>("guard.sparkno", null);
			m_KOecho = datasection.GetAttribute<Boolean>("KO.echo", false);
			m_volumeoffset = datasection.GetAttribute<Int32>("volume", 0);
			m_persistanceintindex = datasection.GetAttribute<Int32>("IntPersistIndex", 60);
			m_persistancefloatindex = datasection.GetAttribute<Int32>("FloatPersistIndex", 40);

			m_scale.X = sizesection.GetAttribute<Single>("xscale", 1.0f);
			m_scale.Y = sizesection.GetAttribute<Single>("yscale", 1.0f);
			m_groundback = sizesection.GetAttribute<Int32>("ground.back", 15);
			m_groundfront = sizesection.GetAttribute<Int32>("ground.front", 16);
			m_airback = sizesection.GetAttribute<Int32>("air.back", 12);
			m_airfront = sizesection.GetAttribute<Int32>("air.front", 12);
			m_height = sizesection.GetAttribute<Int32>("height", 60);
			m_attackdistance = sizesection.GetAttribute<Int32>("attack.dist", 160);
			m_projectileattackdist = sizesection.GetAttribute<Int32>("proj.attack.dist", 90);
			m_projectilescaling = sizesection.GetAttribute<Boolean>("proj.doscale", false);
			m_headposition = sizesection.GetAttribute<Vector2>("head.pos", Vector2.Zero);
			m_midposition = sizesection.GetAttribute<Vector2>("mid.pos", Vector2.Zero);
			m_shadowoffset = sizesection.GetAttribute<Int32>("shadowoffset", 0);
			m_drawoffset = (Vector2)sizesection.GetAttribute<Point>("draw.offset", new Point(0, 0));

			m_walk_forward = velocitysection.GetAttribute<Single>("walk.fwd");
			m_walk_back = velocitysection.GetAttribute<Single>("walk.back");
			m_run_fwd = velocitysection.GetAttribute<Vector2>("run.fwd");
			m_run_back = velocitysection.GetAttribute<Vector2>("run.back");

			m_jump_neutral = velocitysection.GetAttribute<Vector2>("jump.neu");

			m_jump_back = velocitysection.GetAttribute<Vector2>("jump.back");
			if (m_jump_back.Y == 0) m_jump_back.Y = m_jump_neutral.Y;

			m_jump_forward = velocitysection.GetAttribute<Vector2>("jump.fwd");
			if (m_jump_forward.Y == 0) m_jump_forward.Y = m_jump_neutral.Y;

			m_runjump_back = velocitysection.GetAttribute<Vector2>("runjump.back", m_run_back);
			m_runjump_fwd = velocitysection.GetAttribute<Vector2>("runjump.fwd", m_run_fwd);
			m_airjump_neutral = velocitysection.GetAttribute<Vector2>("airjump.neu", Jump_neutral);

			m_airjump_back = velocitysection.GetAttribute<Vector2>("airjump.back", Jump_back);
			if (m_airjump_back.Y == 0) m_airjump_back.Y = m_airjump_neutral.Y;

			m_airjump_forward = velocitysection.GetAttribute<Vector2>("airjump.fwd", Jump_forward);
			if (m_airjump_forward.Y == 0) m_airjump_forward.Y = m_airjump_neutral.Y;

			m_airjumps = movementsection.GetAttribute<Int32>("airjump.num", 0);
			m_airjumpheight = movementsection.GetAttribute<Int32>("airjump.height", 0);
			m_vert_acceleration = movementsection.GetAttribute<Single>("yaccel");
			m_standfriction = movementsection.GetAttribute<Single>("stand.friction");
			m_crouchfriction = movementsection.GetAttribute<Single>("crouch.friction");

			m_airjump_forward.Y = m_airjump_neutral.Y;
			m_airjump_back.Y = m_airjump_neutral.Y;
		}

		public Int32 MaximumLife
		{
			get { return m_life; }
		}

		public Int32 MaximumPower
		{
			get { return m_maxpower; }
		}

		public Int32 AttackPower
		{
			get { return m_attackpower; }
		}

		public Int32 DefensivePower
		{
			get { return m_defensivepower; }
		}

		public Int32 FallDefenseIncrease
		{
			get { return m_falldefenseincrease; }
		}

		public Int32 LieDownTime
		{
			get { return m_liedowntime; }
		}

		public Int32 AirJuggle
		{
			get { return m_airjuggle; }
		}

		public Evaluation.PrefixedExpression DefaultSparkNumber
		{
			get { return m_defaultspark; }
		}

		public Evaluation.PrefixedExpression DefaultGuardSparkNumber
		{
			get { return m_defaultguardspark; }
		}

		public Boolean KOEcho
		{
			get { return m_KOecho; }
		}

		public Int32 VolumeOffset
		{
			get { return m_volumeoffset; }
		}

		public Int32 PersistanceIntIndex
		{
			get { return m_persistanceintindex; }
		}

		public Int32 PersistanceFloatIndex
		{
			get { return m_persistancefloatindex; }
		}

		public Vector2 Scale
		{
			get { return m_scale; }
		}

		public Int32 GroundBack
		{
			get { return m_groundback; }
		}

		public Int32 GroundFront
		{
			get { return m_groundfront; }
		}

		public Int32 Airback
		{
			get { return m_airback; }
		}

		public Int32 Airfront
		{
			get { return m_airfront; }
		}

		public Int32 Height
		{
			get { return m_height; }
		}

		public Int32 Attackdistance
		{
			get { return m_attackdistance; }
		}

		public Int32 Projectileattackdist
		{
			get { return m_projectileattackdist; }
		}

		public Boolean ProjectileScaling
		{
			get { return m_projectilescaling; }
		}

		public Vector2 Headposition
		{
			get { return m_headposition; }
		}

		public Vector2 Midposition
		{
			get { return m_midposition; }
		}

		public Int32 Shadowoffset
		{
			get { return m_shadowoffset; }
		}

		public Vector2 DrawOffset
		{
			get { return m_drawoffset; }
		}

		public Single Walk_forward
		{
			get { return m_walk_forward; }
		}

		public Single Walk_back
		{
			get { return m_walk_back; }
		}

		public Vector2 Run_fwd
		{
			get { return m_run_fwd; }
		}

		public Vector2 Run_back
		{
			get { return m_run_back; }
		}

		public Vector2 Jump_neutral
		{
			get { return m_jump_neutral; }
		}

		public Vector2 Jump_back
		{
			get { return m_jump_back; }
		}

		public Vector2 Jump_forward
		{
			get { return m_jump_forward; }
		}

		public Vector2 Runjump_back
		{
			get { return m_runjump_back; }
		}

		public Vector2 Runjump_fwd
		{
			get { return m_runjump_fwd; }
		}

		public Vector2 Airjump_neutral
		{
			get { return m_airjump_neutral; }
		}

		public Vector2 Airjump_back
		{
			get { return m_airjump_back; }
		}

		public Vector2 Airjump_forward
		{
			get { return m_airjump_forward; }
		}

		public Int32 Airjumps
		{
			get { return m_airjumps; }
		}

		public Int32 Airjumpheight
		{
			get { return m_airjumpheight; }
		}

		public Single Vert_acceleration
		{
			get { return m_vert_acceleration; }
		}

		public Single Standfriction
		{
			get { return m_standfriction; }
		}

		public Single Crouchfriction
		{
			get { return m_crouchfriction; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_life;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_maxpower;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_attackpower;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_defensivepower;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_falldefenseincrease;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_liedowntime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_airjuggle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_defaultspark;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_defaultguardspark;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_KOecho;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_volumeoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_persistanceintindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_persistancefloatindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_scale; // Draw scaling factor

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_groundback; // Player width (back, ground)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_groundfront; // Player width (front, ground)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_airback; // Player width (back, air)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_airfront; //Player width (front, air)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_height; // Height of player (for opponent to jump over)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_attackdistance; // Default attack distance

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_projectileattackdist; //Default attack distance for projectiles

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_projectilescaling; //Set to scale projectiles too

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_headposition; // Approximate position of head

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_midposition; // Approximate position of midsection

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_shadowoffset; // Number of pixels to vertically offset the shadow

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_drawoffset; // Player drawing offset in pixels	

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Single m_walk_forward; // Walk forward

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Single m_walk_back; // Walk backward

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_run_fwd; // Run forward (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_run_back; // Hop backward (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_jump_neutral; // Neutral jumping velocity (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_jump_back; // Jump back Speed (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_jump_forward; // Jump forward Speed (x, y)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_runjump_back; // Running jump speeds (opt)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_runjump_fwd; //

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_airjump_neutral; //

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_airjump_back;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_airjump_forward;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_airjumps; // Number of air jumps allowed (opt)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_airjumpheight; // Minimum distance from ground before you can air jump (opt)

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Single m_vert_acceleration; // Vertical acceleration

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Single m_standfriction; // Friction coefficient when standing

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Single m_crouchfriction; // Friction coefficient when crouching

		#endregion
	}
}