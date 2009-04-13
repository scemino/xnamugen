using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using xnaMugen.Collections;

namespace xnaMugen.Evaluation
{
	class EvaluationSystem : SubSystem
	{
		public EvaluationSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_expressioncache = new KeyedCollection<String, Expression>(x => x.ToString(), StringComparer.OrdinalIgnoreCase);
			m_numbercache = new KeyedCollection<Number, NumberReturn>(x => x.Evaluate(null));
			m_tokenizer = new Tokenizer();
			m_treebuilder = new TreeBuilder(this);
			//BuildSwitchText();
		}

		static public void BuildSwitchText()
		{
			StringBuilder builder = new StringBuilder();

			foreach (FieldInfo fi in typeof(Operator).GetFields())
			{
				FunctionMappingAttribute attrib = (FunctionMappingAttribute)Attribute.GetCustomAttribute(fi, typeof(FunctionMappingAttribute));
				if (attrib != null)
				{
					builder.AppendFormat("case \"{0}\":{1}", attrib.Type.FullName, Environment.NewLine);
					builder.AppendFormat("\treturn new {0}(children, arguments);{1}{1}", attrib.Type.FullName, Environment.NewLine);
				}
			}

			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsAbstract == true || type.IsClass == false || type.IsInterface == true) continue;

				CustomFunctionAttribute attr = (CustomFunctionAttribute)Attribute.GetCustomAttribute(type, typeof(CustomFunctionAttribute));
				if (attr == null) continue;

				builder.AppendFormat("case \"{0}\":{1}", type.FullName, Environment.NewLine);
				builder.AppendFormat("\treturn new {0}(children, arguments);{1}{1}", type.FullName, Environment.NewLine);
			}
		}

		public Expression CreateExpression(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			Expression expression = BuildExpression(input);

			if (expression.IsValid == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.EvaluationSystem, "Error parsing line: {0}", input);
			}

			return expression;
		}

		public PrefixedExpression CreatePrefixedExpression(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			Expression with = BuildExpression(input);
			Char prefix = input[0];
			Expression without = BuildExpression(input.Substring(1));

			Boolean? common = null;
			Expression expression = null;

			if (prefix == 's' || prefix == 'S')
			{
				if (without.IsValid == true)
				{
					common = false;
					expression = without;
				}
				else
				{
					common = false;
					expression = with;
				}
			}
			else if (prefix == 'f' || prefix == 'F')
			{
				if (without.IsValid == true)
				{
					common = true;
					expression = without;
				}
				else
				{
					common = false;
					expression = with;
				}
			}
			else
			{
				common = null;
				expression = with;
			}

			if (expression.IsValid == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.EvaluationSystem, "Error parsing line: {0}", input);
			}

			return new PrefixedExpression(expression, common);
		}

		Expression BuildExpression(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			//Quoted strings must be case sensitive & cache is not case sensitive
			if (input.IndexOf('"') == -1 && m_expressioncache.Contains(input) == true) return m_expressioncache[input];

			List<Token> tokens = m_tokenizer.Tokenize(input);
			List<Node> nodes = m_treebuilder.BuildTree(tokens);

			List<IFunction> functions = new List<IFunction>();
			foreach (Node node in nodes) functions.Add(BuildNode(node));

			Expression expression = new Expression(input, functions);

			if (input.IndexOf('"') == -1) m_expressioncache.Add(expression);

			return expression;
		}

		IFunction BuildNode(Node node)
		{
			if (node == null) throw new ArgumentNullException("node");

			if (node.Token.Data is Tokenizing.NumberData)
			{
				Number number = (node.Token.Data as Tokenizing.NumberData).GetNumber(node.Token.ToString());

				if (m_numbercache.Contains(number) == true) return m_numbercache[number];

				NumberReturn @return = new NumberReturn(number);
				m_numbercache.Add(@return);

				return @return;
			}

			List<IFunction> children = new List<IFunction>();
			foreach (Node child in node.Children) children.Add(BuildNode(child));

			if (node.Token.Data is Tokenizing.RangeData) return new Operations.Range(children, node.Arguments);

			return CreateCallBack(GetFunctionName(node), children, node.Arguments);
		}

		static String GetFunctionName(Node node)
		{
			if (node == null) throw new ArgumentNullException("node");

			Tokenizing.NodeData data = node.Token.Data as Tokenizing.NodeData;
			return (data != null) ? data.Name : String.Empty;
		}

		static IFunction CreateCallBack(String name, List<IFunction> children, List<Object> arguments)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (children == null) throw new ArgumentNullException("children");
			if (arguments == null) throw new ArgumentNullException("arguments");

			switch (name)
			{
				case "xnaMugen.Evaluation.Operations.LogicOr":
					return new xnaMugen.Evaluation.Operations.LogicOr(children, arguments);

				case "xnaMugen.Evaluation.Operations.LogicXor":
					return new xnaMugen.Evaluation.Operations.LogicXor(children, arguments);

				case "xnaMugen.Evaluation.Operations.LogicAnd":
					return new xnaMugen.Evaluation.Operations.LogicAnd(children, arguments);

				case "xnaMugen.Evaluation.Operations.BitOr":
					return new xnaMugen.Evaluation.Operations.BitOr(children, arguments);

				case "xnaMugen.Evaluation.Operations.BitXor":
					return new xnaMugen.Evaluation.Operations.BitXor(children, arguments);

				case "xnaMugen.Evaluation.Operations.BitAnd":
					return new xnaMugen.Evaluation.Operations.BitAnd(children, arguments);

				case "xnaMugen.Evaluation.Operations.Equality":
					return new xnaMugen.Evaluation.Operations.Equality(children, arguments);

				case "xnaMugen.Evaluation.Operations.Inequality":
					return new xnaMugen.Evaluation.Operations.Inequality(children, arguments);

				case "xnaMugen.Evaluation.Operations.LesserThan":
					return new xnaMugen.Evaluation.Operations.LesserThan(children, arguments);

				case "xnaMugen.Evaluation.Operations.LesserEquals":
					return new xnaMugen.Evaluation.Operations.LesserEquals(children, arguments);

				case "xnaMugen.Evaluation.Operations.GreaterThan":
					return new xnaMugen.Evaluation.Operations.GreaterThan(children, arguments);

				case "xnaMugen.Evaluation.Operations.GreaterEquals":
					return new xnaMugen.Evaluation.Operations.GreaterEquals(children, arguments);

				case "xnaMugen.Evaluation.Operations.Addition":
					return new xnaMugen.Evaluation.Operations.Addition(children, arguments);

				case "xnaMugen.Evaluation.Operations.Substraction":
					return new xnaMugen.Evaluation.Operations.Substraction(children, arguments);

				case "xnaMugen.Evaluation.Operations.Division":
					return new xnaMugen.Evaluation.Operations.Division(children, arguments);

				case "xnaMugen.Evaluation.Operations.Multiplication":
					return new xnaMugen.Evaluation.Operations.Multiplication(children, arguments);

				case "xnaMugen.Evaluation.Operations.Modulus":
					return new xnaMugen.Evaluation.Operations.Modulus(children, arguments);

				case "xnaMugen.Evaluation.Operations.Exponent":
					return new xnaMugen.Evaluation.Operations.Exponent(children, arguments);

				case "xnaMugen.Evaluation.Operations.Null":
					return new xnaMugen.Evaluation.Operations.Null(children, arguments);

				case "xnaMugen.Evaluation.Operations.LogicNot":
					return new xnaMugen.Evaluation.Operations.LogicNot(children, arguments);

				case "xnaMugen.Evaluation.Operations.BitNot":
					return new xnaMugen.Evaluation.Operations.BitNot(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Vel":
					return new xnaMugen.Evaluation.Triggers.Vel(children, arguments);

				case "xnaMugen.Evaluation.Triggers.UniqHitCount":
					return new xnaMugen.Evaluation.Triggers.UniqHitCount(children, arguments);

				case "xnaMugen.Evaluation.Triggers.TimeMod":
					return new xnaMugen.Evaluation.Triggers.TimeMod(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ProjCancelTime":
					return new xnaMugen.Evaluation.Triggers.ProjCancelTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Win":
					return new xnaMugen.Evaluation.Triggers.Win(children, arguments);

				case "xnaMugen.Evaluation.Triggers.WinKO":
					return new xnaMugen.Evaluation.Triggers.WinKO(children, arguments);

				case "xnaMugen.Evaluation.Triggers.WinTime":
					return new xnaMugen.Evaluation.Triggers.WinTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.WinPerfect":
					return new xnaMugen.Evaluation.Triggers.WinPerfect(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ProjHitTime":
					return new xnaMugen.Evaluation.Triggers.ProjHitTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P2Dist":
					return new xnaMugen.Evaluation.Triggers.P2Dist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P2BodyDist":
					return new xnaMugen.Evaluation.Triggers.P2BodyDist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.NumEnemy":
					return new xnaMugen.Evaluation.Triggers.NumEnemy(children, arguments);

				case "xnaMugen.Evaluation.Triggers.AnimTime":
					return new xnaMugen.Evaluation.Triggers.AnimTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.PalNo":
					return new xnaMugen.Evaluation.Triggers.PalNo(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P2StateType":
					return new xnaMugen.Evaluation.Triggers.P2StateType(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P2MoveType":
					return new xnaMugen.Evaluation.Triggers.P2MoveType(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P1Name":
					return new xnaMugen.Evaluation.Triggers.P1Name(children, arguments);

				case "xnaMugen.Evaluation.Triggers.HitFall":
					return new xnaMugen.Evaluation.Triggers.HitFall(children, arguments);

				case "xnaMugen.Evaluation.Triggers.AnimElemNo":
					return new xnaMugen.Evaluation.Triggers.AnimElemNo(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P4Name":
					return new xnaMugen.Evaluation.Triggers.P4Name(children, arguments);

				case "xnaMugen.Evaluation.Triggers.MoveType":
					return new xnaMugen.Evaluation.Triggers.MoveType(children, arguments);

				case "xnaMugen.Evaluation.Triggers.LifeMax":
					return new xnaMugen.Evaluation.Triggers.LifeMax(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ID":
					return new xnaMugen.Evaluation.Triggers.ID(children, arguments);

				case "xnaMugen.Evaluation.Triggers.GetHitVar":
					return new xnaMugen.Evaluation.Triggers.GetHitVar(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Ctrl":
					return new xnaMugen.Evaluation.Triggers.Ctrl(children, arguments);

				case "xnaMugen.Evaluation.Triggers.RoundNo":
					return new xnaMugen.Evaluation.Triggers.RoundNo(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ProjContact":
					return new xnaMugen.Evaluation.Triggers.ProjContact(children, arguments);

				case "xnaMugen.Evaluation.Triggers.PrevStateNo":
					return new xnaMugen.Evaluation.Triggers.PrevStateNo(children, arguments);

				case "xnaMugen.Evaluation.Triggers.HitPauseTime":
					return new xnaMugen.Evaluation.Triggers.HitPauseTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.AnimElemTime":
					return new xnaMugen.Evaluation.Triggers.AnimElemTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.TeamSide":
					return new xnaMugen.Evaluation.Triggers.TeamSide(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Sin":
					return new xnaMugen.Evaluation.Triggers.Sin(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Redirection.Parent":
					return new xnaMugen.Evaluation.Triggers.Redirection.Parent(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Redirection.Root":
					return new xnaMugen.Evaluation.Triggers.Redirection.Root(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Redirection.Helper":
					return new xnaMugen.Evaluation.Triggers.Redirection.Helper(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Redirection.Target":
					return new xnaMugen.Evaluation.Triggers.Redirection.Target(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Redirection.Partner":
					return new xnaMugen.Evaluation.Triggers.Redirection.Partner(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Redirection.Enemy":
					return new xnaMugen.Evaluation.Triggers.Redirection.Enemy(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Redirection.EnemyNear":
					return new xnaMugen.Evaluation.Triggers.Redirection.EnemyNear(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Redirection.PlayerID":
					return new xnaMugen.Evaluation.Triggers.Redirection.PlayerID(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Random":
					return new xnaMugen.Evaluation.Triggers.Random(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Pi":
					return new xnaMugen.Evaluation.Triggers.Pi(children, arguments);

				case "xnaMugen.Evaluation.Triggers.NumProjID":
					return new xnaMugen.Evaluation.Triggers.NumProjID(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Lose":
					return new xnaMugen.Evaluation.Triggers.Lose(children, arguments);

				case "xnaMugen.Evaluation.Triggers.LoseKO":
					return new xnaMugen.Evaluation.Triggers.LoseKO(children, arguments);

				case "xnaMugen.Evaluation.Triggers.LoseTime":
					return new xnaMugen.Evaluation.Triggers.LoseTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.IsHelper":
					return new xnaMugen.Evaluation.Triggers.IsHelper(children, arguments);

				case "xnaMugen.Evaluation.Triggers.HitDefAttr":
					return new xnaMugen.Evaluation.Triggers.HitDefAttr(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Facing":
					return new xnaMugen.Evaluation.Triggers.Facing(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Cos":
					return new xnaMugen.Evaluation.Triggers.Cos(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Assertion":
					return new xnaMugen.Evaluation.Triggers.Assertion(children, arguments);

				case "xnaMugen.Evaluation.Triggers.AnimElem":
					return new xnaMugen.Evaluation.Triggers.AnimElem(children, arguments);

				case "xnaMugen.Evaluation.Triggers.TicksPerSecond":
					return new xnaMugen.Evaluation.Triggers.TicksPerSecond(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ProjGuarded":
					return new xnaMugen.Evaluation.Triggers.ProjGuarded(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Power":
					return new xnaMugen.Evaluation.Triggers.Power(children, arguments);

				case "xnaMugen.Evaluation.Triggers.HitShakeOver":
					return new xnaMugen.Evaluation.Triggers.HitShakeOver(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Var":
					return new xnaMugen.Evaluation.Triggers.Var(children, arguments);

				case "xnaMugen.Evaluation.Triggers.SelfAnimExist":
					return new xnaMugen.Evaluation.Triggers.SelfAnimExist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ParentDist":
					return new xnaMugen.Evaluation.Triggers.ParentDist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.IsHomeTeam":
					return new xnaMugen.Evaluation.Triggers.IsHomeTeam(children, arguments);

				case "xnaMugen.Evaluation.Triggers.DrawGame":
					return new xnaMugen.Evaluation.Triggers.DrawGame(children, arguments);

				case "xnaMugen.Evaluation.Triggers.SysFVar":
					return new xnaMugen.Evaluation.Triggers.SysFVar(children, arguments);

				case "xnaMugen.Evaluation.Triggers.RoundState":
					return new xnaMugen.Evaluation.Triggers.RoundState(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P2Name":
					return new xnaMugen.Evaluation.Triggers.P2Name(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P2Life":
					return new xnaMugen.Evaluation.Triggers.P2Life(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Log":
					return new xnaMugen.Evaluation.Triggers.Log(children, arguments);

				case "xnaMugen.Evaluation.Triggers.HitOver":
					return new xnaMugen.Evaluation.Triggers.HitOver(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Asin":
					return new xnaMugen.Evaluation.Triggers.Asin(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Alive":
					return new xnaMugen.Evaluation.Triggers.Alive(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Acos":
					return new xnaMugen.Evaluation.Triggers.Acos(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Pos":
					return new xnaMugen.Evaluation.Triggers.Pos(children, arguments);

				case "xnaMugen.Evaluation.Triggers.NumTarget":
					return new xnaMugen.Evaluation.Triggers.NumTarget(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Ceil":
					return new xnaMugen.Evaluation.Triggers.Ceil(children, arguments);

				case "xnaMugen.Evaluation.Triggers.StateNo":
					return new xnaMugen.Evaluation.Triggers.StateNo(children, arguments);

				case "xnaMugen.Evaluation.Triggers.NumProj":
					return new xnaMugen.Evaluation.Triggers.NumProj(children, arguments);

				case "xnaMugen.Evaluation.Triggers.MatchNo":
					return new xnaMugen.Evaluation.Triggers.MatchNo(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Time":
					return new xnaMugen.Evaluation.Triggers.Time(children, arguments);

				case "xnaMugen.Evaluation.Triggers.StateTime":
					return new xnaMugen.Evaluation.Triggers.StateTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ScreenPos":
					return new xnaMugen.Evaluation.Triggers.ScreenPos(children, arguments);

				case "xnaMugen.Evaluation.Triggers.PowerMax":
					return new xnaMugen.Evaluation.Triggers.PowerMax(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P2StateNo":
					return new xnaMugen.Evaluation.Triggers.P2StateNo(children, arguments);

				case "xnaMugen.Evaluation.Triggers.MoveReversed":
					return new xnaMugen.Evaluation.Triggers.MoveReversed(children, arguments);

				case "xnaMugen.Evaluation.Triggers.GameTime":
					return new xnaMugen.Evaluation.Triggers.GameTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.E":
					return new xnaMugen.Evaluation.Triggers.E(children, arguments);

				case "xnaMugen.Evaluation.Triggers.BackEdgeBodyDist":
					return new xnaMugen.Evaluation.Triggers.BackEdgeBodyDist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Abs":
					return new xnaMugen.Evaluation.Triggers.Abs(children, arguments);

				case "xnaMugen.Evaluation.Triggers.TeamMode":
					return new xnaMugen.Evaluation.Triggers.TeamMode(children, arguments);

				case "xnaMugen.Evaluation.Triggers.SysVar":
					return new xnaMugen.Evaluation.Triggers.SysVar(children, arguments);

				case "xnaMugen.Evaluation.Triggers.StateType":
					return new xnaMugen.Evaluation.Triggers.StateType(children, arguments);

				case "xnaMugen.Evaluation.Triggers.RoundsExisted":
					return new xnaMugen.Evaluation.Triggers.RoundsExisted(children, arguments);

				case "xnaMugen.Evaluation.Triggers.PlayerIDExist":
					return new xnaMugen.Evaluation.Triggers.PlayerIDExist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.NumHelper":
					return new xnaMugen.Evaluation.Triggers.NumHelper(children, arguments);

				case "xnaMugen.Evaluation.Triggers.MoveContact":
					return new xnaMugen.Evaluation.Triggers.MoveContact(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Floor":
					return new xnaMugen.Evaluation.Triggers.Floor(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Tan":
					return new xnaMugen.Evaluation.Triggers.Tan(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Ln":
					return new xnaMugen.Evaluation.Triggers.Ln(children, arguments);

				case "xnaMugen.Evaluation.Triggers.InGuardDist":
					return new xnaMugen.Evaluation.Triggers.InGuardDist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.HitCount":
					return new xnaMugen.Evaluation.Triggers.HitCount(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Command":
					return new xnaMugen.Evaluation.Triggers.Command(children, arguments);

				case "xnaMugen.Evaluation.Triggers.BackEdgeDist":
					return new xnaMugen.Evaluation.Triggers.BackEdgeDist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.AuthorName":
					return new xnaMugen.Evaluation.Triggers.AuthorName(children, arguments);

				case "xnaMugen.Evaluation.Triggers.AnimExist":
					return new xnaMugen.Evaluation.Triggers.AnimExist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ProjGuardedTime":
					return new xnaMugen.Evaluation.Triggers.ProjGuardedTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.P3Name":
					return new xnaMugen.Evaluation.Triggers.P3Name(children, arguments);

				case "xnaMugen.Evaluation.Triggers.NumPartner":
					return new xnaMugen.Evaluation.Triggers.NumPartner(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Name":
					return new xnaMugen.Evaluation.Triggers.Name(children, arguments);

				case "xnaMugen.Evaluation.Triggers.MoveGuarded":
					return new xnaMugen.Evaluation.Triggers.MoveGuarded(children, arguments);

				case "xnaMugen.Evaluation.Triggers.MatchOver":
					return new xnaMugen.Evaluation.Triggers.MatchOver(children, arguments);

				case "xnaMugen.Evaluation.Triggers.IfElse":
					return new xnaMugen.Evaluation.Triggers.IfElse(children, arguments);

				case "xnaMugen.Evaluation.Triggers.FVar":
					return new xnaMugen.Evaluation.Triggers.FVar(children, arguments);

				case "xnaMugen.Evaluation.Triggers.FrontEdgeDist":
					return new xnaMugen.Evaluation.Triggers.FrontEdgeDist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Anim":
					return new xnaMugen.Evaluation.Triggers.Anim(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ProjContactTime":
					return new xnaMugen.Evaluation.Triggers.ProjContactTime(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Life":
					return new xnaMugen.Evaluation.Triggers.Life(children, arguments);

				case "xnaMugen.Evaluation.Triggers.HitVel":
					return new xnaMugen.Evaluation.Triggers.HitVel(children, arguments);

				case "xnaMugen.Evaluation.Triggers.FrontEdgeBodyDist":
					return new xnaMugen.Evaluation.Triggers.FrontEdgeBodyDist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Exp":
					return new xnaMugen.Evaluation.Triggers.Exp(children, arguments);

				case "xnaMugen.Evaluation.Triggers.CanRecover":
					return new xnaMugen.Evaluation.Triggers.CanRecover(children, arguments);

				case "xnaMugen.Evaluation.Triggers.RootDist":
					return new xnaMugen.Evaluation.Triggers.RootDist(children, arguments);

				case "xnaMugen.Evaluation.Triggers.ProjHit":
					return new xnaMugen.Evaluation.Triggers.ProjHit(children, arguments);

				case "xnaMugen.Evaluation.Triggers.NumExplod":
					return new xnaMugen.Evaluation.Triggers.NumExplod(children, arguments);

				case "xnaMugen.Evaluation.Triggers.MoveHit":
					return new xnaMugen.Evaluation.Triggers.MoveHit(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Const":
					return new xnaMugen.Evaluation.Triggers.Const(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Atan":
					return new xnaMugen.Evaluation.Triggers.Atan(children, arguments);

				case "xnaMugen.Evaluation.Operations.Assignment":
					return new xnaMugen.Evaluation.Operations.Assignment(children, arguments);

				case "xnaMugen.Evaluation.Triggers.Physics":
					return new xnaMugen.Evaluation.Triggers.Physics(children, arguments);

				default:
					return new Operations.Null(children, arguments);
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Tokenizer m_tokenizer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TreeBuilder m_treebuilder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly KeyedCollection<String, Expression> m_expressioncache;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly KeyedCollection<Number, NumberReturn> m_numbercache;

		#endregion
	}
}