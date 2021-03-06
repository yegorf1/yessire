﻿using System;
using System.Collections.Generic;

namespace YesSir.Backend.Commands.Parts {
	public class CommandPart {
		public readonly PartType Type;
		public readonly CommandPart[] NextCommands;
		public readonly string ValueName;
		public readonly Func<string, Tuple<bool, int>> Check;
		public readonly Func<string, object> Parse;

		public CommandPart() {
			this.Type = PartType.Word;
			this.NextCommands = null;
			this.Check = (s) => new Tuple<bool, int>(true, 0);
			this.Parse = (s) => null;
		}

		public CommandPart(string[] names, CommandPart[] nextCommands = null) {
			this.Type = PartType.Word;
			this.NextCommands = nextCommands;
			this.Check = (s) => {
				foreach (string n in names) {
					if (s.Contains(n.ToLower())) {
						return new Tuple<bool, int>(true, n.Length + s.IndexOf(n.ToLower()));
					}
				}

				return new Tuple<bool, int>(false, 0);
			};
			this.Parse = (s) => null;
		}

		public CommandPart(string name, Tuple<string, object>[] pairs, CommandPart[] nextCommands = null) {
			this.Type = PartType.Value;
			this.ValueName = name;
			this.NextCommands = nextCommands;
			this.Check = (s) => {
				foreach (Tuple<string, object> t in pairs) {
					if (s.Contains(t.Item1.ToLower())) {
						return new Tuple<bool, int>(true, s.IndexOf(t.Item1.ToLower()) + t.Item1.Length);
					}
				}

				return new Tuple<bool, int>(false, 0);
			};
			this.Parse = (s) => {
				foreach (Tuple<string, object> t in pairs) {
					if (s.Contains(t.Item1.ToLower())) {
						return t.Item2;
					}
				}

				return null;
			};
		}

		public CommandPart(string name, Func<string, Tuple<bool, int>> checker, Func<string, object> parser, CommandPart[] nextCommands = null) {
			this.Type = PartType.Value;
			this.ValueName = name;
			this.Check = checker;
			this.Parse = parser;
			this.NextCommands = nextCommands;
		}

		public int ParseCommand(string arg, ref Dictionary<string, object> parsed) {
			if (parsed == null) {
				parsed = new Dictionary<string, object>();
			}

			arg = arg.ToLower().Trim();
			var res = Check(arg);

			if (res.Item1) {
				string sub = arg.Substring(res.Item2).Trim();
				string start = arg.Substring(0, res.Item2).Trim();

				if (Type == PartType.Value) {
					parsed[ValueName] = Parse(start);
				}

				if (NextCommands != null && NextCommands.Length > 0) {
					foreach (CommandPart cp in NextCommands) {
						int len = cp.ParseCommand(sub, ref parsed);
						if (len >= 0) {
							return len + res.Item2;
						}
					}

					if (Type == PartType.Value) {
						parsed.Remove(ValueName);
					}

					return -1;
				} else {
					return res.Item2;
				}
			} else {
				return -1;
			}	
		}
	}

	public enum PartType {
		Word,
		Value
	}
}
