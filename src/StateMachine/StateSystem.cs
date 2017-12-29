using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using xnaMugen.Collections;
using xnaMugen.IO;

namespace xnaMugen.StateMachine
{
	internal class StateSystem : SubSystem
	{
		public StateSystem(SubSystems subsystems)
			: base(subsystems)
		{
			_statefiles = new Dictionary<string, ReadOnlyKeyedCollection<int, State>>(StringComparer.OrdinalIgnoreCase);
			_controllertitleregex = new Regex(@"^State\s+(\S.*)$", RegexOptions.IgnoreCase);
			_staterTitleRegex = new Regex("Statedef\\s*(-?\\d+).*", RegexOptions.IgnoreCase);
			_controllermap = BuildControllerMap();
			_internalstates = GetStates("xnaMugen.data.Internal.cns");
		}

		private static ReadOnlyDictionary<string, Constructor> BuildControllerMap()
		{
			var attribType = typeof(StateControllerNameAttribute);
			var constructortypes = new[] { typeof(StateSystem), typeof(string), typeof(TextSection) };

			var controllermap = new Dictionary<string, Constructor>(StringComparer.OrdinalIgnoreCase);

			foreach (var t in Assembly.GetCallingAssembly().GetTypes())
			{
				if (t.IsSubclassOf(typeof(StateController)) == false || Attribute.IsDefined(t, attribType) == false) continue;
				var attrib = (StateControllerNameAttribute)Attribute.GetCustomAttribute(t, attribType);

				foreach (var name in attrib.Names)
				{
					if (controllermap.ContainsKey(name))
					{
						Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Duplicate definition found for state controller - {0}.", name);
					}
					else
					{
						controllermap.Add(name, ConstructorDelegate.FastConstruct(t, constructortypes));
					}
				}
			}

			return new ReadOnlyDictionary<string, Constructor>(controllermap);
		}

		private static void AddStateToCollection(KeyedCollection<int, State> collection, State state)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (state == null) throw new ArgumentNullException(nameof(state));

			if (collection.Contains(state.Number))
			{
				Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Duplicate state #{0}. Discarding duplicate", state.Number);
			}
			else
			{
				collection.Add(state);
			}
		}

		public StateManager CreateManager(Combat.Character character, ReadOnlyList<string> filepaths)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));
			if (filepaths == null) throw new ArgumentNullException(nameof(filepaths));

			var states = new KeyedCollection<int, State>(x => x.Number);

			foreach (var filepath in filepaths)
			{
				var loadedstates = GetStates(filepath);
				foreach (var state in loadedstates)
				{
					if (states.Contains(state.Number)) states.Remove(state.Number);
					states.Add(state);
				}
			}

			foreach (var state in _internalstates)
			{
				if (states.Contains(state.Number) == false) states.Add(state);
			}

			return new StateManager(this, character, new ReadOnlyKeyedCollection<int, State>(states));
		}

		private ReadOnlyKeyedCollection<int, State> GetStates(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			if (_statefiles.ContainsKey(filepath)) return _statefiles[filepath];

			var states = new KeyedCollection<int, State>(x => x.Number);
			var textfile = GetSubSystem<FileSystem>().OpenTextFile(filepath);

			TextSection laststatesection = null;
			List<StateController> controllers = null;

			foreach (var textsection in textfile)
			{
				if (_staterTitleRegex.IsMatch(textsection.Title))
				{
					if (laststatesection != null)
					{
						var newstate = CreateState(laststatesection, controllers);
						if (newstate != null) AddStateToCollection(states, newstate);
					}

					laststatesection = textsection;
					controllers = new List<StateController>();
				}
				else
				{
					var controller = CreateController(textsection);
					if (controller != null) controllers?.Add(controller);
				}
			}

			if (laststatesection != null)
			{
				var newstate = CreateState(laststatesection, controllers);
				if (newstate != null) AddStateToCollection(states, newstate);
			}

			var roStates = new ReadOnlyKeyedCollection<int, State>(states);
			_statefiles.Add(filepath, roStates);
			return roStates;
		}

		private State CreateState(TextSection textsection, List<StateController> controllers)
		{
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));
			if (controllers == null) throw new ArgumentNullException(nameof(controllers));

			var match = _staterTitleRegex.Match(textsection.Title);
			if (match.Success == false) return null;

			var statenumber = int.Parse(match.Groups[1].Value);

			foreach (var controller in controllers)
			{
				if (controller.IsValid()) continue;

				Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Error parsing state #{0}, controller {1} - '{2}'", statenumber, controller.GetType().Name, controller.Label);
			}

			controllers.RemoveAll(x => x.IsValid() == false);

			if (_internalstates != null && _internalstates.Contains(statenumber))
			{
				controllers.AddRange(_internalstates[statenumber].Controllers);
			}

			var state = new State(this, statenumber, textsection, controllers);
			return state;
		}

		private StateController CreateController(TextSection textsection)
		{
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));

			var match = _controllertitleregex.Match(textsection.Title);
			if (match.Success == false) return null;

			var typename = textsection.GetAttribute<string>("type", null);

			if (typename == null)
			{
				Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Controller '{0}' does not have a type.", textsection);
				return null;
			}


			if (_controllermap.ContainsKey(typename) == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Controller '{0}' has invalid type - '{1}'.", textsection, typename);
				return null;
			}

			var controller = (StateController)_controllermap[typename](this, match.Groups[1].Value, textsection);
			return controller;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<string, ReadOnlyKeyedCollection<int, State>> _statefiles;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyDictionary<string, Constructor> _controllermap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex _controllertitleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex _staterTitleRegex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyKeyedCollection<int, State> _internalstates;

		#endregion
	}

}