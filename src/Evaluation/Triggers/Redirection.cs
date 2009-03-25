using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers.Redirection
{
	[CustomFunction("Parent")]
	class Parent : Function
	{
		public Parent(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return new Number();

			return Children[0](helper.Parent);
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;
			parsestate.BaseNode.Children.Add(child);

			return parsestate.BaseNode;
		}
	}

	[CustomFunction("Root")]
	class Root : Function
	{
		public Root(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return new Number();

			return Children[0](helper.BasePlayer);
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;
			parsestate.BaseNode.Children.Add(child);

			return parsestate.BaseNode;
		}
	}

	[CustomFunction("Helper")]
	class Helper : Function
	{
		public Helper(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 2) return new Number();

			Number helper_id = Children[0](state);
			if (helper_id.NumberType != NumberType.Int) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Helper helper = character.FilterEntityAsHelper(entity, helper_id.IntValue);
				if (helper == null) continue;

				return Children[1](helper);
			}

			return new Number();
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(new Node(new Token("-1", new Tokenizing.IntData())));
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

	[CustomFunction("Target")]
	class Target : Function
	{
		public Target(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 2) return new Number();

			Number target_id = Children[0](state);
			if (target_id.NumberType != NumberType.Int) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, target_id.IntValue);
				if (target == null) continue;

				return Children[1](target);
			}

			return new Number();
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(new Node(new Token("-1", new Tokenizing.IntData())));
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

	[CustomFunction("Partner")]
	class Partner : Function
	{
		public Partner(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			Combat.Player partner = GetTeamMate(character);
			if (partner == null) return new Number();

			return Children[0](partner);
		}

		Combat.Player GetTeamMate(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			if (character.BasePlayer == character.Team.MainPlayer)
			{
				return character.Team.TeamMate;
			}
			else
			{
				return character.Team.MainPlayer;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;
			parsestate.BaseNode.Children.Add(child);

			return parsestate.BaseNode;
		}
	}

	[CustomFunction("Enemy")]
	class Enemy : Function
	{
		public Enemy(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 2) return new Number();

			Number nth = Children[0](state);
			if (nth.NumberType == NumberType.None) return new Number();

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character enemy = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (enemy == null) continue;

				Combat.Player enemyplayer = enemy as Combat.Player;
				if(enemyplayer == null) continue;

				if (count != nth.IntValue)
				{
					++count;
					continue;
				}

				return Children[1](enemy);
			}

			return new Number();
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(new Node(new Token("0", new Tokenizing.IntData())));
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

#warning Not threadsafe
	[CustomFunction("EnemyNear")]
	class EnemyNear : Function
	{
		static EnemyNear()
		{
			s_playerlist = new List<Combat.Player>();
		}

		public EnemyNear(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 2) return new Number();

			Number nth = Children[0](state);
			if (nth.NumberType == NumberType.None) return new Number();

			BuildPlayerList(character);

			SortPlayerList(character);

			if (nth.IntValue >= s_playerlist.Count) return new Number();

			Combat.Player enemy = s_playerlist[nth.IntValue];
			return Children[1](enemy);
		}

		void SortPlayerList(Combat.Character character)
		{
		}

		void BuildPlayerList(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			s_playerlist.Clear();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character enemy = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (enemy == null) continue;

				Combat.Player enemy_player = enemy as Combat.Player;
				if (enemy_player == null) continue;

				s_playerlist.Add(enemy_player);
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(new Node(new Token("0", new Tokenizing.IntData())));
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}

		#region Fields

		static List<Combat.Player> s_playerlist;

		#endregion
	}

	[CustomFunction("PlayerID")]
	class PlayerID : Function
	{
		public PlayerID(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 2) return new Number();

			Number character_id = Children[0](state);
			if (character_id.NumberType != NumberType.Int) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character character2 = entity as Combat.Character;
				if (character2 == null || character2.Id != character_id.IntValue) continue;

				return Children[1](character2);
			}

			return new Number();
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node == null) return null;

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}
}