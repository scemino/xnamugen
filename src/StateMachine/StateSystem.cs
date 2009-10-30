using System;
using System.Diagnostics;
using xnaMugen.Collections;
using System.Collections.Generic;
using xnaMugen.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using System.Reflection.Emit;

namespace xnaMugen.StateMachine
{
	class StateSystem : SubSystem
	{
		delegate StateController CreationCallback(StateSystem statesystem, String label, TextSection textsection);

		public StateSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_statefiles = new Dictionary<String, ReadOnlyKeyedCollection<Int32, State>>(StringComparer.OrdinalIgnoreCase);
			m_controllertitleregex = new Regex(@"^State\s+(\S.*)$", RegexOptions.IgnoreCase);
			m_statertitleregex = new Regex("Statedef\\s*(-?\\d+).*", RegexOptions.IgnoreCase);
			m_controllermap = BuildControllerMap();
			m_internalstates = GetStates("xnaMugen.data.Internal.cns");
		}

		static CreationCallback FastConstruct(Type type, Type[] argtypes)
		{
			DynamicMethod method = new DynamicMethod(String.Empty, typeof(StateController), argtypes, typeof(StateSystem));
			ILGenerator generator = method.GetILGenerator();

			generator.Emit(OpCodes.Ldarg, 0);
			generator.Emit(OpCodes.Ldarg, 1);
			generator.Emit(OpCodes.Ldarg, 2);

			generator.Emit(OpCodes.Newobj, type.GetConstructor(argtypes));
			generator.Emit(OpCodes.Ret);

			return (CreationCallback)method.CreateDelegate(typeof(CreationCallback));
		}

		static ReadOnlyDictionary<String, CreationCallback> BuildControllerMap()
		{
			Type attrib_type = typeof(StateControllerNameAttribute);
			Type[] constructortypes = new Type[] { typeof(StateSystem), typeof(String), typeof(TextSection) };

			Dictionary<String, CreationCallback> controllermap = new Dictionary<String, CreationCallback>(StringComparer.OrdinalIgnoreCase);

			foreach (Type t in Assembly.GetCallingAssembly().GetTypes())
			{
				if (t.IsSubclassOf(typeof(StateController)) == false || Attribute.IsDefined(t, attrib_type) == false) continue;
				StateControllerNameAttribute attrib = (StateControllerNameAttribute)Attribute.GetCustomAttribute(t, attrib_type);

				ConstructorInfo constrcutor = t.GetConstructor(constructortypes);

				foreach (String name in attrib.Names)
				{
					if (controllermap.ContainsKey(name) == true)
					{
						Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Duplicate definition found for state controller - {0}.", name);
					}
					else
					{
						controllermap.Add(name, FastConstruct(t, constructortypes));
					}
				}
			}

			return new ReadOnlyDictionary<String, CreationCallback>(controllermap);
		}

		static void AddStateToCollection(KeyedCollection<Int32, State> collection, State state)
		{
			if (collection == null) throw new ArgumentNullException("collection");
			if (state == null) throw new ArgumentNullException("state");

			if (collection.Contains(state.Number) == true)
			{
				Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Duplicate state #{0}. Discarding duplicate", state.Number);
			}
			else
			{
				collection.Add(state);
			}
		}

		public StateManager CreateManager(Combat.Character character, ReadOnlyList<String> filepaths)
		{
			if (character == null) throw new ArgumentNullException("character");
			if (filepaths == null) throw new ArgumentNullException("filepaths");

			KeyedCollection<Int32, State> states = new KeyedCollection<Int32, State>(x => x.Number);

			foreach (String filepath in filepaths)
			{
				ReadOnlyKeyedCollection<Int32, State> loadedstates = GetStates(filepath);
				foreach (State state in loadedstates)
				{
					if (states.Contains(state.Number) == true) states.Remove(state.Number);
					states.Add(state);
				}
			}

			foreach (State state in m_internalstates)
			{
				if (states.Contains(state.Number) == false) states.Add(state);
			}

			return new StateManager(this, character, new ReadOnlyKeyedCollection<Int32, State>(states));
		}

		ReadOnlyKeyedCollection<Int32, State> GetStates(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			if (m_statefiles.ContainsKey(filepath) == true) return m_statefiles[filepath];

			KeyedCollection<Int32, State> states = new KeyedCollection<Int32, State>(x => x.Number);
			TextFile textfile = GetSubSystem<IO.FileSystem>().OpenTextFile(filepath);

			TextSection laststatesection = null;
			List<StateController> controllers = null;

			foreach (TextSection textsection in textfile)
			{
				if (m_statertitleregex.IsMatch(textsection.Title) == true)
				{
					if (laststatesection != null)
					{
						State newstate = CreateState(laststatesection, controllers);
						if (newstate != null) AddStateToCollection(states, newstate);

						laststatesection = null;
						controllers = null;
					}

					laststatesection = textsection;
					controllers = new List<StateController>();
				}
				else
				{
					StateController controller = CreateController(textsection);
					if (controller != null && controllers != null) controllers.Add(controller);
				}
			}

			if (laststatesection != null)
			{
				State newstate = CreateState(laststatesection, controllers);
				if (newstate != null) AddStateToCollection(states, newstate);

				laststatesection = null;
				controllers = null;
			}

			ReadOnlyKeyedCollection<Int32, State> ro_states = new ReadOnlyKeyedCollection<Int32, State>(states);
			m_statefiles.Add(filepath, ro_states);
			return ro_states;
		}

		State CreateState(TextSection textsection, List<StateController> controllers)
		{
			if (textsection == null) throw new ArgumentNullException("textsection");
			if (controllers == null) throw new ArgumentNullException("controllers");

			Match match = m_statertitleregex.Match(textsection.Title);
			if (match.Success == false) return null;

			Int32 statenumber = Int32.Parse(match.Groups[1].Value);

			foreach (StateController controller in controllers)
			{
				if (controller.IsValid() == true) continue;

				Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Error parsing state #{0}, controller {1} - '{2}'", statenumber, controller.GetType().Name, controller.Label);
			}

			controllers.RemoveAll(x => x.IsValid() == false);

			if (m_internalstates != null && m_internalstates.Contains(statenumber) == true)
			{
				controllers.AddRange(m_internalstates[statenumber].Controllers);
			}

			State state = new State(this, statenumber, textsection, controllers);
			return state;
		}

		StateController CreateController(TextSection textsection)
		{
			if (textsection == null) throw new ArgumentNullException("textsection");

			Match match = m_controllertitleregex.Match(textsection.Title);
			if (match.Success == false) return null;

			String typename = textsection.GetAttribute<String>("type", null);

			if (typename == null)
			{
				Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Controller '{0}' does not have a type.", textsection);
				return null;
			}


			if (m_controllermap.ContainsKey(typename) == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.StateSystem, "Controller '{0}' has invalid type - '{1}'.", textsection, typename);
				return null;
			}

			StateController controller = m_controllermap[typename](this, match.Groups[1].Value, textsection);
			return controller;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<String, ReadOnlyKeyedCollection<Int32, State>> m_statefiles;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyDictionary<String, CreationCallback> m_controllermap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_controllertitleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_statertitleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyKeyedCollection<Int32, State> m_internalstates;

		#endregion
	}

}