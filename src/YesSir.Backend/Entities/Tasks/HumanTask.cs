﻿using System;
using System.Collections.Generic;
using YesSir.Backend.Entities.Kingdoms;
using YesSir.Backend.Managers;

namespace YesSir.Backend.Entities {
	public class HumanTask {
		public List<IUsable> InUse = new List<IUsable>();
		public bool Repeating = false;
		public string Destination;
		public ETask TaskType;
		public float TimeLeft; // In days
		public Guid HumanId;

		public object Context;

		public float Difficulty;
		public string Skill;

		public HumanTask(Guid hid) {
			HumanId = hid;
		}

		public HumanTask(Human h) : this(h.HumanId) { }

		public void CalculateTaskTime(Human h, float difficulty = 1f, string skill = null) {
			this.Difficulty = difficulty;

			switch (this.TaskType) {
				case ETask.Learning:
					Skill = skill ?? "learning";
					TimeLeft = h.GetSkill(this.Destination) / h.GetSkill(Skill);
					break;

				case ETask.Building:
					Skill = "building";
					TimeLeft = 0.3f / h.GetSkill(Skill);
					break;

				case ETask.Creating:
				case ETask.Extracting:
					Skill = skill ?? "mining";
					TimeLeft = 0.05f / h.GetSkill(Skill);
					break;

				case ETask.Other:
				case ETask.ListeningKing:
					TimeLeft = float.PositiveInfinity;
					break;

				case ETask.SendingMessage:
					Skill = skill ?? "diplomacy";
					float dist = KingdomsManager.Distance(h.KingdomId, Guid.Parse(Destination));
					TimeLeft = dist / 60;
					break;

				default:
					Skill = skill;
					TimeLeft = 1;
					break;
			}

			TimeLeft *= Difficulty;
		}

		public void Use(IUsable usable) {
			if (usable != null) {
				InUse.Add(usable);
				usable.OnUse(HumanId);
				usable.IsBusy = true;
			}
		}
	}
}
