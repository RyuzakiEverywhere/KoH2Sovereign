using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class UIPlayerReminder : MonoBehaviour, IListener
{
	// Token: 0x060022BF RID: 8895 RVA: 0x0013B253 File Offset: 0x00139453
	private void Init()
	{
		if (this.init)
		{
			return;
		}
		this.init = true;
		this.LoadDefs();
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x0013B26C File Offset: 0x0013946C
	private void LoadDefs()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		DT.Def def = game.dt.FindDef("PlayerReminder");
		List<DT.Def> list = (def != null) ? def.defs : null;
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.datas.Add(new UIPlayerReminder.Data(list[i].field));
		}
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x0013B2D2 File Offset: 0x001394D2
	public void SetKingdom(Logic.Kingdom kingdom)
	{
		this.Init();
		Logic.Kingdom kingdom2 = this.kingdom;
		if (kingdom2 != null)
		{
			kingdom2.DelListener(this);
		}
		this.kingdom = kingdom;
		Logic.Kingdom kingdom3 = this.kingdom;
		if (kingdom3 == null)
		{
			return;
		}
		kingdom3.AddListener(this);
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x0013B304 File Offset: 0x00139504
	private void LateUpdate()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		float seconds = game.real_time_played.seconds;
		for (int i = 0; i < this.datas.Count; i++)
		{
			UIPlayerReminder.Data data = this.datas[i];
			if (data.auto_recheck_interval > 0f && data.last_message_time + data.cooldown <= seconds && data.last_recheck_time + data.auto_recheck_interval <= seconds)
			{
				this.CheckReminder(data, seconds, null);
			}
		}
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x0013B384 File Offset: 0x00139584
	public void OnMessage(object obj, string message, object param)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		float seconds = game.real_time_played.seconds;
		for (int i = 0; i < this.datas.Count; i++)
		{
			UIPlayerReminder.Data data = this.datas[i];
			if (data.trigger_on == message && data.last_message_time + data.cooldown <= seconds)
			{
				this.CheckReminder(data, seconds, param);
			}
			else if (data.reset_cooldown_on == message)
			{
				data.last_message_time = seconds;
				data.last_recheck_time = seconds;
			}
		}
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x0013B410 File Offset: 0x00139610
	private Func<bool> GetValidationFunc(UIPlayerReminder.Data data)
	{
		string message = data.message;
		if (message == "CanAdoptTraditionMessage")
		{
			return delegate()
			{
				int slot_index = this.kingdom.NumTraditions(Tradition.Type.All);
				bool flag = this.kingdom.NumFreeTraditionSlots(Tradition.Type.All, false) > 0;
				bool flag2 = this.kingdom.HasNewTraditionOptions(Tradition.Type.All);
				bool flag3 = this.kingdom.CanAffordTradition(slot_index);
				return flag && flag2 && flag3;
			};
		}
		if (message == "MaximumBooksMessage")
		{
			return delegate()
			{
				List<Logic.Character> court = this.kingdom.court;
				bool flag;
				if (court == null)
				{
					flag = (null != null);
				}
				else
				{
					flag = (court.Find((Logic.Character c) => c != null && c.CanImproveSkills()) != null);
				}
				float stat = this.kingdom.GetStat(Stats.ks_max_books, true);
				return flag && stat > 0f && this.kingdom.resources[ResourceType.Books] >= stat;
			};
		}
		if (message == "LowStabilityInRealmMessage")
		{
			return delegate()
			{
				if (this.kingdom.rebellions.Count > 0)
				{
					return false;
				}
				for (int i = 0; i < this.kingdom.realms.Count; i++)
				{
					if (this.kingdom.realms[i].GetTotalRebellionRisk() < this.kingdom.realms[i].rebellionRisk.def.low_stability_in_realm_message_threshold)
					{
						return true;
					}
				}
				return false;
			};
		}
		if (!(message == "CanBuildBuildingMessage"))
		{
			return () => true;
		}
		return () => this.kingdom.game.real_time_played.seconds <= 7200f && this.kingdom.SuggestCastlesToBuildIn().Count > 0;
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x0013B4B0 File Offset: 0x001396B0
	private Vars GetVars(UIPlayerReminder.Data data, object param = null)
	{
		Vars vars;
		if (data.inherit_trigger_params)
		{
			if (param is Vars)
			{
				vars = (Vars)param;
			}
			else
			{
				vars = new Vars(param);
			}
		}
		else
		{
			vars = new Vars();
		}
		string message = data.message;
		if (!(message == "MaximumBooksMessage"))
		{
			if (!(message == "LowStabilityInRealmMessage"))
			{
				if (!(message == "CanBuildBuildingMessage"))
				{
					if (message == "AdvantageHalfReadyMessage")
					{
						Value val = vars.Get("def_id", true);
						vars.Set<Logic.Kingdom>("kingdom", this.kingdom);
						vars.Set<KingdomAdvantage.Def>("advantage", this.kingdom.advantages.FindById(val).def);
					}
				}
				else
				{
					List<Castle> list = this.kingdom.SuggestCastlesToBuildIn();
					if (list.Count > 0)
					{
						vars.SetVar("castles", new Value(list));
					}
				}
			}
			else
			{
				for (int i = 0; i < this.kingdom.realms.Count; i++)
				{
					if (this.kingdom.realms[i].GetTotalRebellionRisk() < this.kingdom.realms[i].rebellionRisk.def.low_stability_in_realm_message_threshold)
					{
						vars.SetVar("realm", this.kingdom.realms[i]);
						vars.SetVar("goto_target", this.kingdom.realms[i].castle);
						break;
					}
				}
			}
		}
		else
		{
			List<Logic.Character> list2 = this.kingdom.court.FindAll((Logic.Character c) => c != null && c.CanImproveSkills());
			if (list2.Count > 0)
			{
				vars.SetVar("characters", new Value(list2));
			}
		}
		return vars;
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x0013B698 File Offset: 0x00139898
	private void CheckReminder(UIPlayerReminder.Data data, float time, object param = null)
	{
		data.last_recheck_time = time;
		UserSettings.SettingData setting = UserSettings.GetSetting(data.def.key);
		if (setting != null && setting.value == false)
		{
			return;
		}
		Func<bool> Validate = this.GetValidationFunc(data);
		if (Validate())
		{
			MessageIcon messageIcon = MessageIcon.Create(data.message, this.GetVars(data, param), true, null);
			if (messageIcon != null)
			{
				data.last_message_time = time;
				messageIcon.on_update = delegate(MessageIcon i)
				{
					if (!Validate())
					{
						i.Dismiss(true);
					}
				};
			}
		}
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x0013B72C File Offset: 0x0013992C
	public string ShowReminder(string id, object param = null)
	{
		int i = 0;
		while (i < this.datas.Count)
		{
			UIPlayerReminder.Data data = this.datas[i];
			if (data.def.key.IndexOf(id, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				bool flag = this.GetValidationFunc(data)();
				if (MessageIcon.Create(data.message, this.GetVars(data, param), true, null) == null)
				{
					return string.Concat(new string[]
					{
						"failed to create icon for message ",
						data.def.key,
						" (",
						data.message,
						")"
					});
				}
				if (!flag)
				{
					return string.Concat(new string[]
					{
						"created invalid ",
						data.def.key,
						" (",
						data.message,
						")"
					});
				}
				return string.Concat(new string[]
				{
					"created ",
					data.def.key,
					" (",
					data.message,
					")"
				});
			}
			else
			{
				i++;
			}
		}
		return "not found";
	}

	// Token: 0x04001750 RID: 5968
	private Logic.Kingdom kingdom;

	// Token: 0x04001751 RID: 5969
	private bool init;

	// Token: 0x04001752 RID: 5970
	private List<UIPlayerReminder.Data> datas = new List<UIPlayerReminder.Data>();

	// Token: 0x02000794 RID: 1940
	public class Data
	{
		// Token: 0x06004CAB RID: 19627 RVA: 0x0022A5A4 File Offset: 0x002287A4
		public Data(DT.Field def_field)
		{
			this.def = def_field;
			this.trigger_on = def_field.FindChild("trigger_on", null, true, true, true, '.').String(null, "");
			this.reset_cooldown_on = def_field.FindChild("reset_cooldown_on", null, true, true, true, '.').String(null, "");
			this.message = def_field.FindChild("message", null, true, true, true, '.').String(null, "");
			this.cooldown = def_field.FindChild("cooldown", null, true, true, true, '.').Float(null, 0f);
			this.starting_cooldown = def_field.FindChild("starting_cooldown", null, true, true, true, '.').Float(null, 0f);
			this.auto_recheck_interval = def_field.FindChild("auto_recheck_interval", null, true, true, true, '.').Float(null, 0f);
			this.last_message_time = this.starting_cooldown - this.cooldown;
			this.last_recheck_time = this.starting_cooldown - this.auto_recheck_interval;
			this.inherit_trigger_params = def_field.FindChild("inherit_trigger_params", null, true, true, true, '.').Bool(null, false);
		}

		// Token: 0x04003B2C RID: 15148
		public string trigger_on;

		// Token: 0x04003B2D RID: 15149
		public string reset_cooldown_on;

		// Token: 0x04003B2E RID: 15150
		public string message;

		// Token: 0x04003B2F RID: 15151
		public float cooldown;

		// Token: 0x04003B30 RID: 15152
		public float last_message_time;

		// Token: 0x04003B31 RID: 15153
		public float last_recheck_time;

		// Token: 0x04003B32 RID: 15154
		public float starting_cooldown;

		// Token: 0x04003B33 RID: 15155
		public float auto_recheck_interval;

		// Token: 0x04003B34 RID: 15156
		public bool inherit_trigger_params;

		// Token: 0x04003B35 RID: 15157
		public DT.Field def;
	}
}
