using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers.Redirection
{
	[StateRedirection("Parent")]
	internal static class Parent
	{
		public static object RedirectState(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			var helper = character as Combat.Helper;
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

			var child = parsestate.BuildNode(false);
			if (child == null) return null;
			parsestate.BaseNode.Children.Add(child);

			return parsestate.BaseNode;
		}
	}

	[StateRedirection("Root")]
	internal static class Root
	{
		public static object RedirectState(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			var helper = character as Combat.Helper;
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

			var child = parsestate.BuildNode(false);
			if (child == null) return null;
			parsestate.BaseNode.Children.Add(child);

			return parsestate.BaseNode;
		}
	}

	[StateRedirection("Helper")]
	internal static class Helper
	{
		public static object RedirectState(object state, ref bool error, int helperId)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			List<Combat.Helper> helpers;
			if (character.BasePlayer.Helpers.TryGetValue(helperId, out helpers) && helpers.Count > 0) return helpers[0];

			error = true;
			return null;
		}

		public static Node Parse(ParseState parsestate)
		{
			var node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(new Node(new Token("-1", new Tokenizing.IntData())));
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

	[StateRedirection("Target")]
	internal static class Target
	{
		public static object RedirectState(object state, ref bool error, int target_id)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			foreach (var target in character.GetTargets(target_id))
			{
				return target;
			}

			error = true;
			return null;
		}

		public static Node Parse(ParseState parsestate)
		{
			var node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(new Node(new Token("-1", new Tokenizing.IntData())));
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

	[StateRedirection("Partner")]
	internal static class Partner
	{
		public static object RedirectState(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}


			var partner = GetTeamMate(character);
			if (partner == null)
			{
				error = true;
				return null;
			}

			return partner;
		}

		private static Combat.Player GetTeamMate(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			if (character.BasePlayer == character.Team.MainPlayer)
			{
				return character.Team.TeamMate;
			}

			return character.Team.MainPlayer;
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var child = parsestate.BuildNode(false);
			if (child == null) return null;
			parsestate.BaseNode.Children.Add(child);

			return parsestate.BaseNode;
		}
	}

	[StateRedirection("Enemy")]
	internal static class Enemy
	{
		public static object RedirectState(object state, ref bool error, int nth)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			var count = 0;
			foreach (var entity in character.Engine.Entities)
			{
				var enemy = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (enemy == null) continue;

				var enemyplayer = enemy as Combat.Player;
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
			var node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(Node.ZeroNode);
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

	[StateRedirection("EnemyNear")]
	internal static class EnemyNear
	{
		public static object RedirectState(object state, ref bool error, int nth)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			var playerlist = BuildPlayerList(character);

			//SortPlayerList(character);

			if (nth >= playerlist.Count)
			{
				error = true;
				return null;
			}

			return playerlist[nth];
		}

		private static void SortPlayerList(Combat.Character character)
		{
		}

		private static List<Combat.Player> BuildPlayerList(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			var otherteam = character.Team.OtherTeam;

			var players = new List<Combat.Player>();
			if (otherteam.MainPlayer != null) players.Add(otherteam.MainPlayer);
			if (otherteam.TeamMate != null) players.Add(otherteam.TeamMate);

			return players;
		}

		public static Node Parse(ParseState parsestate)
		{
			var node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
				node = parsestate.BaseNode;

#warning Hack
				node.Children.Add(Node.ZeroNode);
			}

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}

	[StateRedirection("PlayerID")]
	internal static class PlayerID
	{
		public static object RedirectState(object state, ref bool error, int characterId)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return null;
			}

			foreach (var entity in character.Engine.Entities)
			{
				var character2 = entity as Combat.Character;
				if (character2 == null || character2.Id != characterId) continue;

				return character2;
			}

			error = true;
			return null;
		}

		public static Node Parse(ParseState parsestate)
		{
			var node = parsestate.BuildParenNumberNode(true);
			if (node == null) return null;

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var child = parsestate.BuildNode(false);
			if (child == null) return null;

			node.Children.Add(child);

			return node;
		}
	}
}