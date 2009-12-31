using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers.Redirection
{
	[StateRedirection("Parent")]
	static class Parent
	{
		public static Object RedirectState(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null)
			{
				error = true;
				return null;
			}

			return helper.Parent;
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

	[StateRedirection("Root")]
	static class Root
	{
		public static Object RedirectState(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null)
			{
				error = true;
				return null;
			}

			return helper.BasePlayer;
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

	[StateRedirection("Helper")]
	static class Helper
	{
		public static Object RedirectState(Object state, ref Boolean error, Int32 helper_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			List<Combat.Helper> helpers;
			if (character.BasePlayer.Helpers.TryGetValue(helper_id, out helpers) == true && helpers.Count > 0) return helpers[0];

			error = true;
			return null;
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

	[StateRedirection("Target")]
	static class Target
	{
		public static Object RedirectState(Object state, ref Boolean error, Int32 target_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			foreach (Combat.Character target in character.GetTargets(target_id))
			{
				return target;
			}

			error = true;
			return null;
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

	[StateRedirection("Partner")]
	static class Partner
	{
		public static Object RedirectState(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}


			Combat.Player partner = GetTeamMate(character);
			if (partner == null)
			{
				error = true;
				return null;
			}

			return partner;
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

	[StateRedirection("Enemy")]
	static class Enemy
	{
		public static Object RedirectState(Object state, ref Boolean error, Int32 nth)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character enemy = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (enemy == null) continue;

				Combat.Player enemyplayer = enemy as Combat.Player;
				if (enemyplayer == null) continue;

				if (count != nth)
				{
					++count;
					continue;
				}

				return enemy;
			}

			error = true;
			return null;
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

	[StateRedirection("EnemyNear")]
	static class EnemyNear
	{
		public static Object RedirectState(Object state, ref Boolean error, Int32 nth)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			List<Combat.Player> playerlist = BuildPlayerList(character);

			//SortPlayerList(character);

			if (nth >= playerlist.Count)
			{
				error = true;
				return null;
			}

			return playerlist[nth];
		}

		static void SortPlayerList(Combat.Character character)
		{
		}

		static List<Combat.Player> BuildPlayerList(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			Combat.Team otherteam = character.Team.OtherTeam;

			List<Combat.Player> players = new List<Combat.Player>();
			if (otherteam.MainPlayer != null) players.Add(otherteam.MainPlayer);
			if (otherteam.TeamMate != null) players.Add(otherteam.TeamMate);

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

	[StateRedirection("PlayerID")]
	static class PlayerID
	{
		public static Object RedirectState(Object state, ref Boolean error, Int32 character_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character character2 = entity as Combat.Character;
				if (character2 == null || character2.Id != character_id) continue;

				return character2;
			}

			error = true;
			return null;
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