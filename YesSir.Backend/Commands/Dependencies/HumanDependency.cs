﻿using System;
using YesSir.Backend.Entities.Kingdoms;
using YesSir.Backend.Managers;
using YesSir.Shared.Messages;

namespace YesSir.Backend.Commands.Dependencies {
	public class HumanDependency : IDependency {
		public HumanDependency() { }

		public Tuple<bool, MessageCallback> CheckKingdom(Kingdom kingdom) {
			if (kingdom.Humans.Count > 0) {
				return new Tuple<bool, MessageCallback>(true, new MessageCallback());
			} else {
				MessageCallback cb = new MessageCallback() {
					Text = Locale.Get("problems.no_people", kingdom.Language),
					From = ECharacter.Knight
				};
				return new Tuple<bool, MessageCallback>(false, cb);
			}
		}

		public void Use(Kingdom kingdom) { }
	}
}
