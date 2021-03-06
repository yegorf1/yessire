﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesSir.Backend.Managers;
using YesSir.Shared.Messages;

namespace YesSir.Backend.Entities.Kingdoms {
	[MoonSharp.Interpreter.MoonSharpUserData]
	public class Human {
		public Guid HumanId;
		public Guid KingdomId;
		public float DepressionLevel = 0;
		public float Mood = 0.7f;
		public float Satiety = 0.9f;
		public float KingAccpetance = 0.9f;

		public string Name;
		public ESex Sex;
		public float Age = 0;
		public List<HumanTask> TasksToDo;
		public Dictionary<string, float> Skills;
		public bool Died = false;

		public Dictionary<Guid, float> FriendShips;

		public bool IsInDepression = false;


		public Human() {
			Skills = new Dictionary<string, float>();
			FriendShips = new Dictionary<Guid, float>();
			TasksToDo = new List<HumanTask>();
		}

		public Human(string name, ESex sex, float age) : this() {
			Name = name;
			Sex = sex;
			Age = age;
		}

		public bool AddTask(HumanTask t) {
			if (IsInDepression || RandomManager.QuanticFloat(1 - KingAccpetance) > 0.5f) {
				return false;
			}

			TasksToDo.Add(t);

			return true;
		}

		public float GetSkill(string skill) {
			return Skills.ContainsKey(skill) ? Skills[skill] : 0;
		}

		public void UpgradeSkill(string skill, float power = 0.6f) {
			float c = GetSkill(skill);
			Skills[skill] = (float)Math.Pow(c, power);
		}

		public void SetSkill(string name, float skill) {
			Skills[name] = skill;
		}

		public bool Worked(float delta, float difficulty) {
			Satiety -= delta * difficulty / 10f;

			return true;
		}

		public string GetName(string language) {
			return GetJobName(language) + " " + Name;
		}

		public string GetStatus(string language) {
			if (TasksToDo.Count == 0) {
				return Locale.Get("status.idle", language);
			} else {
				switch (TasksToDo[0].TaskType) {
					case ETask.Building:
						return string.Format(Locale.Get("status.building", language), Locale.Get("buildings." + TasksToDo[0].Destination + ".name", language));

					case ETask.Creating:
					case ETask.Extracting:
						var tsk = TasksToDo[0].TaskType.ToString().ToLower();
						return string.Format(Locale.Get("status." + tsk, language), Locale.Get("resources." + TasksToDo[0].Destination + ".name", language));

					case ETask.SendingMessage:
						string kingdom_name = KingdomsManager.FindKingdom(Guid.Parse(TasksToDo[0].Destination)).Name;
						return string.Format(Locale.Get("status.sending_message", language), kingdom_name);

					default:
						return Locale.Get("status." + TasksToDo[0].ToString().ToLower(), language);
				}
			}
		}

		public float GetDefaultFriendhsip() {
			if (FriendShips.Count > 0) {
				float s = 0;
				{
					var ar = FriendShips.ToArray();
					for (int i = 0; i < ar.Length; i++) {
						s += ar[i].Value;
					}

					return s / ar.Length;
				}
			} else {
				return 0.1f;
			}
		}

		public float GetFriendShip(Human h) {
			return FriendShips.ContainsKey(h.HumanId) ? FriendShips[h.HumanId] : GetDefaultFriendhsip();
		}

		public string GetJobName(string language) {
			var skills = Skills.OrderBy(p => -p.Value);

			return ContentManager.GetJobBySkill(skills.First(s => ContentManager.IsJobSkill(s.Key)).Key, language);
		}

		public bool Eat(Kingdom kingdom) {
			foreach (string food in ContentManager.GetFood()) {
				if (kingdom.TakeResource(food, 1)) {
					Satiety += 0.05f;

					return true;
				}
			}

			return false;
		}

		public void UpdateFriendship(Guid h, float k=1) {
			float f = FriendShips[h] + k / 100;
			if (f > 0 && f < 1) {
				FriendShips[h] = f;
			}
		}
	}

	public enum ESex {
		Female,
		Male
	}
}
