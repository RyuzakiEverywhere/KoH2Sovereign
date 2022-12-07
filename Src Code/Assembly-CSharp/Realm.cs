using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class Realm : IListener
{
	// Token: 0x0600129D RID: 4765 RVA: 0x000C2196 File Offset: 0x000C0396
	public global::Kingdom GetKingdom()
	{
		return global::Kingdom.Get(this.kingdom);
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x000C21A8 File Offset: 0x000C03A8
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			this.Name,
			"(",
			this.id,
			"), ",
			this.kingdom.ToString()
		});
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x000C21FC File Offset: 0x000C03FC
	public static int Count()
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			return worldMap.Realms.Count;
		}
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			return titleMap.Realms.Count;
		}
		return 0;
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x000C2240 File Offset: 0x000C0440
	public static global::Realm Get(int id)
	{
		id = Mathf.Abs(id);
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			if (id < 1 || id > worldMap.Realms.Count)
			{
				return null;
			}
			return worldMap.Realms[id - 1];
		}
		else
		{
			TitleMap titleMap = TitleMap.Get();
			if (!(titleMap != null))
			{
				return null;
			}
			if (id < 1 || id > titleMap.Realms.Count)
			{
				return null;
			}
			return titleMap.Realms[id - 1];
		}
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x000C22BC File Offset: 0x000C04BC
	public static global::Realm At(Vector3 pos)
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			return worldMap.RealmAt(pos.x, pos.z);
		}
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			return titleMap.RealmAt(pos.x, pos.z);
		}
		return null;
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x000C2310 File Offset: 0x000C0510
	public static global::Realm New(string name)
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return null;
		}
		global::Realm realm = new global::Realm();
		worldMap.Realms.Add(realm);
		realm.id = worldMap.Realms.Count;
		realm.Name = name;
		return realm;
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x000C2359 File Offset: 0x000C0559
	public string GetNameKey()
	{
		if (!string.IsNullOrEmpty(this.TownName))
		{
			return "tn_" + this.TownName;
		}
		return "tn_" + this.Name;
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x000C2389 File Offset: 0x000C0589
	public string LocalizedName()
	{
		return global::Defs.Localize(this.GetNameKey(), null, "", true, true);
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x000C239E File Offset: 0x000C059E
	public bool HasTag(string tag)
	{
		return this.logic != null && this.logic.HasTag(tag, 1);
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x000C23B7 File Offset: 0x000C05B7
	public int GetTag(string tag)
	{
		if (this.logic == null)
		{
			return 0;
		}
		return this.logic.GetTag(tag);
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x000C23D0 File Offset: 0x000C05D0
	public void Load(DT.Field field)
	{
		this.MapColor = global::Defs.GetColor(field, "map_color", null);
		DT.Field field2 = field.FindChild("neighbors", null, true, true, true, '.');
		if (field2 != null && field2.children != null && field2.children.Count > 0)
		{
			for (int i = 0; i < field2.children.Count; i++)
			{
				DT.Field field3 = field2.children[i];
				if (field3 != null && !string.IsNullOrEmpty(field3.key))
				{
					this.neighborNames.Add(field3.key);
				}
			}
		}
		DT.Field field4 = field.FindChild("neighbors_through_sea", null, true, true, true, '.');
		if (field4 != null && field4.children != null && field4.children.Count > 0)
		{
			for (int j = 0; j < field4.children.Count; j++)
			{
				DT.Field field5 = field4.children[j];
				if (field5 != null && !string.IsNullOrEmpty(field5.key))
				{
					this.neighborThroughSeaNames.Add(field5.key);
				}
			}
		}
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x000C24D8 File Offset: 0x000C06D8
	public void SaveNeighbors(DT.Field field)
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		DT.Field field2 = new DT.Field(null);
		field2.key = "neighbors";
		field.AddChild(field2);
		if (this.Neighbors.Count == 0)
		{
			using (List<string>.Enumerator enumerator = this.neighborNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string text = enumerator.Current;
					if (worldMap.FindRealm(text) != null || text == "Empty" || text == "OutOfBounds")
					{
						field2.AddChild(new DT.Field(null)
						{
							key = text
						});
					}
					else
					{
						Debug.Log(string.Format("Realms Nieighbor Export: Skip Nieighbor{0} in Realm {1}.  (Not Found)", text, this.Name));
					}
				}
				goto IL_14C;
			}
		}
		foreach (global::Realm.Neighbor neighbor in this.Neighbors)
		{
			string key;
			if (neighbor.rid == 0)
			{
				key = "Empty";
			}
			else if (neighbor.rid > worldMap.Realms.Count)
			{
				key = "OutOfBounds";
			}
			else
			{
				key = global::Realm.GetRealmName(worldMap, neighbor.rid);
			}
			field2.AddChild(new DT.Field(null)
			{
				key = key
			});
		}
		IL_14C:
		DT.Field field3 = new DT.Field(null);
		field3.key = "neighbors_through_sea";
		field.AddChild(field3);
		if (this.LogicNeighbors.Count == 0)
		{
			using (List<string>.Enumerator enumerator = this.neighborThroughSeaNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string text2 = enumerator.Current;
					if (worldMap.FindRealm(text2) != null || text2 == "Empty" || text2 == "OutOfBounds")
					{
						field3.AddChild(new DT.Field(null)
						{
							key = text2
						});
					}
					else
					{
						Debug.Log(string.Format("Realms Nieighbor Through Sea Export: Skip Nieighbor{0} in Realm {1}.  (Not Found)", text2, this.Name));
					}
				}
				return;
			}
		}
		foreach (global::Realm.Neighbor neighbor2 in this.LogicNeighbors)
		{
			if (!this.Neighbors.Contains(neighbor2))
			{
				string key2;
				if (neighbor2.rid == 0)
				{
					key2 = "Empty";
				}
				else if (neighbor2.rid > worldMap.Realms.Count)
				{
					key2 = "OutOfBounds";
				}
				else
				{
					key2 = global::Realm.GetRealmName(worldMap, neighbor2.rid);
				}
				field3.AddChild(new DT.Field(null)
				{
					key = key2
				});
			}
		}
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x000C27B0 File Offset: 0x000C09B0
	public void Save(DT.Field field)
	{
		global::Defs.SetColor(field, "map_color", this.MapColor);
		if (this.IsSeaRealm())
		{
			field.AddChild(new DT.Field(null)
			{
				type = "bool",
				key = "isSeaRealm",
				value_str = "true"
			});
		}
		this.SaveNeighbors(field);
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x000C2810 File Offset: 0x000C0A10
	private static string GetRealmName(WorldMap wm, int rid)
	{
		if (wm == null)
		{
			return "";
		}
		for (int i = 0; i < wm.Realms.Count; i++)
		{
			if (Mathf.Abs(wm.Realms[i].id) == Mathf.Abs(rid))
			{
				return wm.Realms[i].Name;
			}
		}
		return "NotFound " + rid;
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x000C2884 File Offset: 0x000C0A84
	private void CreateActions()
	{
		if (this.logic.actions == null)
		{
			return;
		}
		for (int i = 0; i < this.logic.actions.Count; i++)
		{
			Action action = this.logic.actions[i];
			if (action.visuals == null)
			{
				ActionVisuals.Create(action);
			}
		}
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x000C28DC File Offset: 0x000C0ADC
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.Realm realm = logic_obj as Logic.Realm;
		if (realm == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(realm.game.map_name))
		{
			return;
		}
		global::Realm realm2 = global::Realm.Get(realm.id);
		if (realm2 == null)
		{
			Debug.LogWarning("Could not create visual realm for " + realm.ToString());
			return;
		}
		if (realm2.logic != null)
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"Changing realm '",
				realm2.Name,
				"' logic from ",
				realm2.logic.ToString(),
				" to ",
				realm.ToString()
			}));
			realm2.logic.visuals = null;
		}
		realm2.logic = realm;
		realm.visuals = realm2;
		realm2.kingdom = realm.init_kingdom_id;
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x000C29A8 File Offset: 0x000C0BA8
	public void UpdateBorders()
	{
		global::Kingdom kingdom = global::Kingdom.Get(this.kingdom);
		this.kingdom = this.logic.kingdom_id;
		global::Kingdom kingdom2 = global::Kingdom.Get(this.kingdom);
		if (kingdom == kingdom2)
		{
			return;
		}
		WorldMap worldMap = WorldMap.Get();
		if (kingdom != null)
		{
			LabelUpdater labelUpdater = LabelUpdater.Get(true);
			if (labelUpdater != null)
			{
				labelUpdater.RecreateKingdomLabel(kingdom);
			}
		}
		if (kingdom2 != null)
		{
			LabelUpdater labelUpdater2 = LabelUpdater.Get(true);
			if (labelUpdater2 != null)
			{
				labelUpdater2.RecreateKingdomLabel(kingdom2);
			}
		}
		bool bPV = ViewMode.IsPoliticalView();
		worldMap.UpdateRealmBorders(this, bPV);
		foreach (global::Realm.Neighbor neighbor in this.Neighbors)
		{
			worldMap.UpdateRealmBorders(global::Realm.Get(neighbor.rid), bPV);
		}
		worldMap.wv_kingdom_borders.OnRealmKingdomChanged(this, true);
		LabelUpdater labelUpdater3 = LabelUpdater.Get(true);
		if (labelUpdater3 != null)
		{
			labelUpdater3.UpdateLabels();
		}
		worldMap.ReloadView();
		if (kingdom != null)
		{
			kingdom.RefreshUI();
		}
		if (kingdom2 != null)
		{
			kingdom2.RefreshUI();
		}
		MapData.MarkDirtyBorders(false, false, false, false);
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x000C2AC4 File Offset: 0x000C0CC4
	public Sprite GetIcon()
	{
		if (this.logic == null)
		{
			return null;
		}
		if (this.logic.castle == null)
		{
			return null;
		}
		global::Settlement settlement = this.logic.castle.visuals as global::Settlement;
		if (settlement == null)
		{
			return null;
		}
		Game game = GameLogic.Get(true);
		string text = settlement.houses_architecture;
		if (settlement.citadel != null)
		{
			text = settlement.citadel_architecture;
		}
		if (text == null)
		{
			text = "Default";
		}
		if (this.thresholds == null)
		{
			DT.Field field = game.dt.FindDef("RealmIcons").field.FindChild("thresholds", null, true, true, true, '.');
			int num = field.NumValues();
			if (num > 0)
			{
				this.thresholds = new int[num];
				for (int i = 0; i < num; i++)
				{
					this.thresholds[i] = field.Value(i, null, true, true);
				}
			}
		}
		int num2 = 0;
		int j = this.logic.GetLevel();
		if (this.thresholds != null)
		{
			while (j > this.thresholds[num2])
			{
				num2++;
				if (num2 == this.thresholds.Length)
				{
					num2--;
					break;
				}
			}
		}
		Sprite obj = global::Defs.GetObj<Sprite>(num2, "RealmIcons", text, null);
		if (obj == null)
		{
			obj = global::Defs.GetObj<Sprite>(num2, "RealmIcons", "Default", null);
		}
		return obj;
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x000C2C14 File Offset: 0x000C0E14
	public void UpdateFow(bool forceUpdate, bool neighbor = true)
	{
		Game game = GameLogic.Get(true);
		if (game == null || this.logic == null)
		{
			return;
		}
		if (!forceUpdate && !game.fow)
		{
			return;
		}
		this.RecalcFow(neighbor);
		for (int i = 0; i < this.logic.armies.Count; i++)
		{
			global::Army army = this.logic.armies[i].visuals as global::Army;
			if (!(army == null) && (forceUpdate || this.visibility <= 0 || !army.IsVisible()))
			{
				army.UpdateVisibility(false);
				global::Battle battle = army.GetBattle();
				if (battle != null)
				{
					battle.UpdateVisibility();
				}
			}
		}
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x000C2CB8 File Offset: 0x000C0EB8
	public void RecalcFow(bool neighbor = false)
	{
		Logic.Kingdom k = BaseUI.LogicKingdom();
		int num = this.logic.CalcVisibleBy(k, true);
		this.visibility = num;
		if (!neighbor)
		{
			return;
		}
		for (int i = 0; i < this.logic.neighbors.Count; i++)
		{
			global::Realm realm = this.logic.neighbors[i].visuals as global::Realm;
			if (realm != null)
			{
				num = this.logic.neighbors[i].CalcVisibleBy(k, true);
				realm.visibility = num;
			}
		}
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x000C2D40 File Offset: 0x000C0F40
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1649643086U)
		{
			if (num <= 930526643U)
			{
				if (num <= 501687755U)
				{
					if (num != 156873198U)
					{
						if (num != 501687755U)
						{
							return;
						}
						if (!(message == "rebellion_started"))
						{
							return;
						}
						Logic.Kingdom kingdom = BaseUI.LogicKingdom();
						Logic.Kingdom kingdom2 = kingdom;
						Logic.Realm realm = this.logic;
						if (kingdom2 != ((realm != null) ? realm.GetKingdom() : null))
						{
							return;
						}
						Rebellion rebellion = param as Rebellion;
						if (rebellion == null)
						{
							return;
						}
						if (rebellion.IsEnemy(kingdom))
						{
							BaseUI.PlayVoiceEvent(rebellion.rebel_def.field.GetString("rebellion_start_voice_line", null, "", true, true, true, '.'), rebellion);
						}
						return;
					}
					else
					{
						if (!(message == "rebel_spawned"))
						{
							return;
						}
						Logic.Army army = param as Logic.Army;
						WorldUI worldUI = WorldUI.Get();
						if (worldUI == null || worldUI.kingdom != this.kingdom.id)
						{
							return;
						}
						Vars vars = new Vars(army);
						Logic.Rebel rebel = army.rebel;
						RebelSpawnCondition.Def condition_def = rebel.condition_def;
						string val;
						if (!string.IsNullOrEmpty((condition_def != null) ? condition_def.reason : null))
						{
							val = "#" + global::Defs.Localize("@" + rebel.condition_def.reason, rebel.CreateSpawnReasonVars(), null, true, true);
						}
						else
						{
							val = "#";
						}
						vars.Set<string>("reason", val);
						MessageIcon.Create("RebelsMessage", vars, true, null);
						return;
					}
				}
				else if (num != 845558108U)
				{
					if (num != 925884271U)
					{
						if (num != 930526643U)
						{
							return;
						}
						if (!(message == "trade_center_moved"))
						{
							return;
						}
						Logic.Kingdom kingdom3 = BaseUI.LogicKingdom();
						if (kingdom3 == null)
						{
							return;
						}
						Logic.Realm realm2 = param as Logic.Realm;
						Vars vars2 = new Vars(realm2.castle);
						for (int i = 0; i < kingdom3.court.Count; i++)
						{
							if (kingdom3.court[i] != null && this.logic.merchants.Contains(kingdom3.court[i]))
							{
								vars2.Set<Logic.Character>("merchant", kingdom3.court[i]);
								break;
							}
						}
						vars2.Set<Logic.Realm>("old_realm", this.logic);
						vars2.Set<Logic.Realm>("new_realm", realm2);
						MessageIcon.Create("TradeCenterMovedMessage", vars2, true, null);
						return;
					}
					else
					{
						if (!(message == "controlling_obj_changed"))
						{
							return;
						}
						WorldMap.Get().ReloadView();
						Logic.Realm realm3 = this.logic;
						object obj2;
						if (realm3 == null)
						{
							obj2 = null;
						}
						else
						{
							Logic.Kingdom kingdom4 = realm3.GetKingdom();
							obj2 = ((kingdom4 != null) ? kingdom4.visuals : null);
						}
						global::Kingdom kingdom5 = obj2 as global::Kingdom;
						if (kingdom5 != null)
						{
							kingdom5.RefreshUI();
						}
						Logic.Realm realm4 = this.logic;
						object obj3;
						if (realm4 == null)
						{
							obj3 = null;
						}
						else
						{
							Logic.Object controller = realm4.controller;
							if (controller == null)
							{
								obj3 = null;
							}
							else
							{
								Logic.Kingdom kingdom6 = controller.GetKingdom();
								obj3 = ((kingdom6 != null) ? kingdom6.visuals : null);
							}
						}
						global::Kingdom kingdom7 = obj3 as global::Kingdom;
						if (kingdom7 != null)
						{
							kingdom7.RefreshUI();
						}
						if (this.logic.settlements != null)
						{
							for (int j = 0; j < this.logic.settlements.Count; j++)
							{
								global::Settlement settlement = this.logic.settlements[j].visuals as global::Settlement;
								if (settlement != null)
								{
									settlement.UpdatePVFigure();
								}
							}
						}
						return;
					}
				}
				else
				{
					if (!(message == "refresh_tags"))
					{
						return;
					}
					Logic.Realm realm5 = this.logic;
					PrefabGrid.ChooserContext.RefreshRealmTags((realm5 != null) ? realm5.castle : null);
					return;
				}
			}
			else if (num <= 1090120562U)
			{
				if (num != 952494118U)
				{
					if (num != 1090120562U)
					{
						return;
					}
					if (!(message == "rebel_defeated"))
					{
						return;
					}
					Logic.Rebel rebel2 = param as Logic.Rebel;
					WorldUI worldUI2 = WorldUI.Get();
					if (worldUI2 == null || worldUI2.kingdom != this.kingdom.id)
					{
						return;
					}
					if (((rebel2 != null) ? rebel2.rebellion : null) == null || !rebel2.rebellion.affectedKingdoms.Contains(BaseUI.LogicKingdom()))
					{
						return;
					}
					Vars vars3 = new Vars(rebel2.army);
					MessageIcon.Create("RebelDefeatedMessage", vars3, true, null);
					return;
				}
				else
				{
					if (!(message == "trade_center_spawned"))
					{
						return;
					}
					Logic.Kingdom kingdom8 = BaseUI.LogicKingdom();
					if (kingdom8 == null)
					{
						return;
					}
					if (kingdom8.realms.Contains(this.logic))
					{
						MessageIcon.Create("NewTradeCenterOurMessage", new Vars(this.logic.castle), true, null);
						return;
					}
					MessageIcon.Create("NewTradeCenterTheirsMessage", new Vars(this.logic.castle), true, null);
					return;
				}
			}
			else
			{
				if (num != 1104538373U)
				{
					if (num != 1211309691U)
					{
						if (num != 1649643086U)
						{
							return;
						}
						if (!(message == "finishing"))
						{
							return;
						}
					}
					else if (!(message == "destroying"))
					{
						return;
					}
					this.logic.DelListener(this);
					this.logic = null;
					return;
				}
				if (!(message == "trade_centers_merged"))
				{
					return;
				}
				if (BaseUI.LogicKingdom() == null)
				{
					return;
				}
				Logic.Realm realm6 = this.logic;
				List<Logic.Realm> list = param as List<Logic.Realm>;
				List<Value> list2 = new List<Value>();
				foreach (Logic.Realm val2 in list)
				{
					list2.Add(val2);
				}
				Vars vars4 = new Vars(realm6.castle);
				vars4.Set<List<Value>>("centers", list2);
				MessageIcon.Create("TradeCentersMergedMessage", vars4, true, null);
				return;
			}
		}
		else if (num <= 2153339806U)
		{
			if (num <= 2005696266U)
			{
				if (num != 1713771407U)
				{
					if (num != 2005696266U)
					{
						return;
					}
					if (!(message == "banished_governor"))
					{
						return;
					}
					Logic.Kingdom kingdom9 = BaseUI.LogicKingdom();
					Logic.Realm realm7 = this.logic;
					object obj4;
					if (realm7 == null)
					{
						obj4 = null;
					}
					else
					{
						Castle castle = realm7.castle;
						obj4 = ((castle != null) ? castle.GetKingdom() : null);
					}
					if (kingdom9 != obj4)
					{
						return;
					}
					Vars vars5 = new Vars();
					vars5.Set<Logic.Character>("governor", param as Logic.Character);
					vars5.Set<Logic.Realm>("realm", this.logic);
					vars5.Set<Logic.Kingdom>("occupator", this.logic.controller.GetKingdom());
					MessageIcon.Create("BanishedGovernorMessage", vars5, true, null);
					return;
				}
				else
				{
					if (!(message == "actions_created"))
					{
						return;
					}
					this.CreateActions();
					return;
				}
			}
			else if (num != 2074659719U)
			{
				if (num != 2105032289U)
				{
					if (num != 2153339806U)
					{
						return;
					}
					if (!(message == "started"))
					{
						return;
					}
					this.CreateActions();
					this.UpdateBorders();
					return;
				}
				else
				{
					if (!(message == "religion_changed"))
					{
						return;
					}
					Logic.Kingdom kingdom10 = BaseUI.LogicKingdom();
					Logic.Kingdom kingdom11 = kingdom10;
					Logic.Realm realm8 = this.logic;
					if (kingdom11 != ((realm8 != null) ? realm8.GetKingdom() : null))
					{
						return;
					}
					if (kingdom10.religion != this.logic.religion)
					{
						DT.Field soundsDef = BaseUI.soundsDef;
						BaseUI.PlayVoiceEvent((soundsDef != null) ? soundsDef.GetString("narrator_our_province_religion_converted", null, "", true, true, true, '.') : null, this.logic);
					}
					return;
				}
			}
			else
			{
				if (!(message == "trade_center_despawned"))
				{
					return;
				}
				Logic.Kingdom kingdom12 = BaseUI.LogicKingdom();
				if (kingdom12 == null)
				{
					return;
				}
				Vars vars6 = new Vars(this.logic.castle);
				for (int k = 0; k < kingdom12.court.Count; k++)
				{
					if (kingdom12.court[k] != null && this.logic.merchants.Contains(kingdom12.court[k]))
					{
						vars6.Set<Logic.Character>("merchant", kingdom12.court[k]);
						break;
					}
				}
				if (kingdom12.realms.Contains(this.logic))
				{
					MessageIcon.Create("TradeCenterDespawnOurMessage", vars6, true, null);
					return;
				}
				MessageIcon.Create("TradeCenterDespawnOtherMessage", vars6, true, null);
				return;
			}
		}
		else if (num <= 2333132228U)
		{
			if (num != 2282658765U)
			{
				if (num != 2333132228U)
				{
					return;
				}
				if (!(message == "province_converted"))
				{
					return;
				}
				Logic.Kingdom kingdom13 = param as Logic.Kingdom;
				if (BaseUI.LogicKingdom() != kingdom13)
				{
					return;
				}
				Vars vars7 = new Vars();
				vars7.Set<Logic.Realm>("realm", this.logic);
				MessageIcon.Create("ProvinceConvertedMessage", vars7, true, null);
				return;
			}
			else
			{
				if (!(message == "pop_majority_changed"))
				{
					return;
				}
				if (Game.isLoadingSaveGame)
				{
					return;
				}
				Logic.Realm realm9 = this.logic;
				if (((realm9 != null) ? realm9.GetKingdom() : null) == BaseUI.LogicKingdom())
				{
					Logic.Kingdom kingdom14 = this.logic.pop_majority.kingdom;
					string key = "PopMajorityChangedTrigger";
					string religion;
					if (kingdom14 == null)
					{
						religion = null;
					}
					else
					{
						Religion religion2 = kingdom14.religion;
						religion = ((religion2 != null) ? religion2.name : null);
					}
					BackgroundMusic.OnTrigger(key, religion);
					DT.Field soundsDef2 = BaseUI.soundsDef;
					BaseUI.PlayVoiceEvent((soundsDef2 != null) ? soundsDef2.GetString("pop_majority_kingdom_changed", null, "", true, true, true, '.') : null, null);
				}
				if (ViewMode.current is CulturePowerView || ViewMode.current is CulturesView)
				{
					ViewMode.current.Apply();
				}
				return;
			}
		}
		else if (num != 3454424252U)
		{
			if (num != 3510242896U)
			{
				if (num != 3780989157U)
				{
					return;
				}
				if (!(message == "kingdom_changed"))
				{
					return;
				}
				this.UpdateBorders();
				Logic.Realm realm10 = this.logic;
				Logic.Kingdom kingdom15 = (realm10 != null) ? realm10.GetKingdom() : null;
				if (kingdom15 != null && kingdom15 == BaseUI.LogicKingdom())
				{
					MessageIcon.RecreateRealmBuildings(WorldUI.Get(), this.logic);
				}
				return;
			}
			else
			{
				if (!(message == "trade_centre_lost"))
				{
					return;
				}
				Logic.Kingdom kingdom16 = param as Logic.Kingdom;
				Logic.Kingdom kingdom17 = BaseUI.LogicKingdom();
				if (kingdom17 != kingdom16)
				{
					return;
				}
				if (kingdom16.game.session_time.seconds < 2f)
				{
					return;
				}
				Vars vars8 = new Vars(this.logic.castle);
				for (int l = 0; l < kingdom17.court.Count; l++)
				{
					if (kingdom17.court[l] != null && this.logic.merchants.Contains(kingdom17.court[l]))
					{
						vars8.Set<Logic.Character>("merchant", kingdom17.court[l]);
						break;
					}
				}
				MessageIcon.Create("TradeCenterLostMessage", vars8, true, null);
				return;
			}
		}
		else
		{
			if (!(message == "disorder_state_changed"))
			{
				return;
			}
			if (ViewMode.Stances.IsActive() || ViewMode.WorldView.IsActive())
			{
				WorldMap.Get().ReloadView();
			}
			Logic.Kingdom kingdom18 = BaseUI.LogicKingdom();
			Logic.Realm realm11 = this.logic;
			if (kingdom18 != ((realm11 != null) ? realm11.GetKingdom() : null))
			{
				return;
			}
			if (this.logic.castle.battle == null && this.logic.controller == this.logic.GetKingdom())
			{
				if (this.logic.IsDisorder())
				{
					DT.Field soundsDef3 = BaseUI.soundsDef;
					BaseUI.PlayVoiceEvent((soundsDef3 != null) ? soundsDef3.GetString("disorder_started", null, "", true, true, true, '.') : null, null);
					return;
				}
				DT.Field soundsDef4 = BaseUI.soundsDef;
				BaseUI.PlayVoiceEvent((soundsDef4 != null) ? soundsDef4.GetString("disorder_ended", null, "", true, true, true, '.') : null, null);
			}
			return;
		}
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x000C3844 File Offset: 0x000C1A44
	public global::Realm.Neighbor FindNeighbor(int rid)
	{
		foreach (global::Realm.Neighbor neighbor in this.Neighbors)
		{
			if (neighbor.rid == rid)
			{
				return neighbor;
			}
		}
		return null;
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x000C38A0 File Offset: 0x000C1AA0
	public bool IsSeaRealm()
	{
		return this.id < 0;
	}

	// Token: 0x04000C7E RID: 3198
	public int id;

	// Token: 0x04000C7F RID: 3199
	public string Name = "";

	// Token: 0x04000C80 RID: 3200
	public string TownName = "";

	// Token: 0x04000C81 RID: 3201
	public global::Kingdom.ID kingdom = 0;

	// Token: 0x04000C82 RID: 3202
	public Color MapColor = Color.black;

	// Token: 0x04000C83 RID: 3203
	private List<string> neighborNames = new List<string>();

	// Token: 0x04000C84 RID: 3204
	private List<string> neighborThroughSeaNames = new List<string>();

	// Token: 0x04000C85 RID: 3205
	public Vector3 CastlePos = Vector3.zero;

	// Token: 0x04000C86 RID: 3206
	public PrefabGrid.ChooserContext pg_chooser;

	// Token: 0x04000C87 RID: 3207
	public int visibility = -1;

	// Token: 0x04000C88 RID: 3208
	public Logic.Realm logic;

	// Token: 0x04000C89 RID: 3209
	private int[] thresholds;

	// Token: 0x04000C8A RID: 3210
	public Bounds bounds;

	// Token: 0x04000C8B RID: 3211
	public List<global::Realm.BorderSegment> BorderSegments = new List<global::Realm.BorderSegment>();

	// Token: 0x04000C8C RID: 3212
	public List<global::Realm.Neighbor> Neighbors = new List<global::Realm.Neighbor>();

	// Token: 0x04000C8D RID: 3213
	public List<global::Realm.Neighbor> LogicNeighbors = new List<global::Realm.Neighbor>();

	// Token: 0x04000C8E RID: 3214
	public List<global::Realm.RegionRealm> RegionRealms = new List<global::Realm.RegionRealm>();

	// Token: 0x02000694 RID: 1684
	[Serializable]
	public struct ID
	{
		// Token: 0x06004806 RID: 18438 RVA: 0x0021674A File Offset: 0x0021494A
		public ID(int id)
		{
			this.id = id;
		}

		// Token: 0x06004807 RID: 18439 RVA: 0x00216753 File Offset: 0x00214953
		public static implicit operator global::Realm.ID(int id)
		{
			return new global::Realm.ID(id);
		}

		// Token: 0x06004808 RID: 18440 RVA: 0x0021675B File Offset: 0x0021495B
		public static implicit operator int(global::Realm.ID id)
		{
			return id.id;
		}

		// Token: 0x06004809 RID: 18441 RVA: 0x00216764 File Offset: 0x00214964
		public override string ToString()
		{
			string text = this.id.ToString();
			global::Realm realm = global::Realm.Get(this.id);
			if (realm != null)
			{
				text = text + "(" + realm.Name + ")";
			}
			return text;
		}

		// Token: 0x04003604 RID: 13828
		public int id;
	}

	// Token: 0x02000695 RID: 1685
	[Serializable]
	public class BorderSegment
	{
		// Token: 0x04003605 RID: 13829
		public List<Vector3> points = new List<Vector3>();
	}

	// Token: 0x02000696 RID: 1686
	[Serializable]
	public class RegionRealm
	{
		// Token: 0x0600480B RID: 18443 RVA: 0x002167B7 File Offset: 0x002149B7
		public bool IsSeaRealm()
		{
			return this.rid < 0;
		}

		// Token: 0x04003606 RID: 13830
		public int rid;

		// Token: 0x04003607 RID: 13831
		public float modifiedDistance;

		// Token: 0x04003608 RID: 13832
		public float mountainSegments;

		// Token: 0x04003609 RID: 13833
		public float landSegments;

		// Token: 0x0400360A RID: 13834
		public float seaSegments;

		// Token: 0x0400360B RID: 13835
		public float riverSegments;
	}

	// Token: 0x02000697 RID: 1687
	[Serializable]
	public class Neighbor
	{
		// Token: 0x0600480D RID: 18445 RVA: 0x002167C2 File Offset: 0x002149C2
		public bool IsSeaRealm()
		{
			return this.rid < 0;
		}

		// Token: 0x0400360C RID: 13836
		public int rid;

		// Token: 0x0400360D RID: 13837
		public List<global::Realm.BorderSegment> BorderSegments = new List<global::Realm.BorderSegment>();
	}
}
