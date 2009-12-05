using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers.Redirection
{
	[CustomFunction("Parent")]
	static class Parent
	{
		public static Number RedirectState(Object state, EvaluationCallback callback)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return new Number();

			return callback(helper.Parent);
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
	static class Root
	{
		public static Number RedirectState(Object state, EvaluationCallback callback)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return new Number();

			return callback(helper.BasePlayer);
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
	static class Helper
	{
		public static Number RedirectState(Object state, EvaluationCallback id, EvaluationCallback callback)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number helper_id = id(state);
			if (helper_id.NumberType != NumberType.Int) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Helper helper = character.FilterEntityAsHelper(entity, helper_id.IntValue);
				if (helper == null) continue;

				return callback(helper);
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
	static class Target
	{
		public static Number RedirectState(Object state, EvaluationCallback id, EvaluationCallback callback)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number target_id = id(state);
			if (target_id.NumberType != NumberType.Int) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, target_id.IntValue);
				if (target == null) continue;

				return callback(target);
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
	static class Partner
	{
		public static Number RedirectState(Object state, EvaluationCallback callback)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Player partner = GetTeamMate(character);
			if (partner == null) return new Number();

			return callback(partner);
		}

		static Combat.Player GetTeamMate(Combat.Character character)
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
	static class Enemy
	{
		public static Number RedirectState(Object state, EvaluationCallback id, EvaluationCallback callback)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number nth = id(state);
			if (nth.NumberType == NumberType.None) return new Number();

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character enemy = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (enemy == null) continue;

				Combat.Player enemyplayer = enemy as Combat.Player;
				if (enemyplayer == null) continue;

				if (count != nth.IntValue)
				{
					++count;
					continue;
				}

				return callback(enemy);
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
				node.Children.Add(Node.ZeroNode);
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

	[CustomFunction("EnemyNear")]
	static class EnemyNear
	{
		public static Number RedirectState(Object state, EvaluationCallback id, EvaluationCallback callback)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number nth = id(state);
			if (nth.NumberType == NumberType.None) return new Number();

			List<Combat.Player> playerlist = BuildPlayerList(character);

			//SortPlayerList(character);

			if (nth.IntValue >= playerlist.Count) return new Number();

			Combat.Player enemy = playerlist[nth.IntValue];
			return callback(enemy);
		}

		static void SortPlayerList(Combat.Character character)
		{
		}

		static List<Combat.Player> BuildPlayerList(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			List<Combat.Player> players = new List<Combat.Player>();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character enemy = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (enemy == null) continue;

				Combat.Player enemy_player = enemy as Combat.Player;
				if (enemy_player == null) continue;

				players.Add(enemy_player);
			}

			return players;
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(Node.ZeroNode);
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

	[CustomFunction("PlayerID")]
	static class PlayerID
	{
		public static Number RedirectState(Object state, EvaluationCallback id, EvaluationCallback callback)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number character_id = id(state);
			if (character_id.NumberType != NumberType.Int) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character character2 = entity as Combat.Character;
				if (character2 == null || character2.Id != character_id.IntValue) continue;

				return callback(character2);
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