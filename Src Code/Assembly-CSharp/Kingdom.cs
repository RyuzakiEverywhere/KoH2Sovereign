using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class Kingdom : IListener
{
	// Token: 0x06000CD7 RID: 3287 RVA: 0x0008E729 File Offset: 0x0008C929
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			this.Name,
			"(",
			this.id,
			")"
		});
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x0008E760 File Offset: 0x0008C960
	public static int Count()
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			return worldMap.Kingdoms.Count;
		}
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			return titleMap.Kingdoms.Count;
		}
		return 0;
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x0008E7A4 File Offset: 0x0008C9A4
	public static global::Kingdom Get(int id)
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			if (id < 1 || id > worldMap.Kingdoms.Count)
			{
				return null;
			}
			return worldMap.Kingdoms[id - 1];
		}
		else
		{
			TitleMap titleMap = TitleMap.Get();
			if (!(titleMap != null))
			{
				return null;
			}
			if (id < 1 || id > titleMap.Kingdoms.Count)
			{
				return null;
			}
			return titleMap.Kingdoms[id - 1];
		}
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x0008E818 File Offset: 0x0008CA18
	public static global::Kingdom At(Vector3 pt)
	{
		global::Realm realm = global::Realm.At(pt);
		if (realm == null)
		{
			return null;
		}
		return realm.GetKingdom();
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x0008E838 File Offset: 0x0008CA38
	public static global::Kingdom New(string name)
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return null;
		}
		global::Kingdom kingdom = new global::Kingdom();
		worldMap.Kingdoms.Add(kingdom);
		kingdom.id = worldMap.Kingdoms.Count;
		global::Kingdom kingdom2 = kingdom;
		kingdom.ActiveName = name;
		kingdom2.Name = name;
		return kingdom;
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x0008E88A File Offset: 0x0008CA8A
	public string GetNameKey()
	{
		return "tn_" + ((this.ActiveName != this.Name) ? this.ActiveName : this.Name);
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0008E8B7 File Offset: 0x0008CAB7
	public string LocalizedName()
	{
		return global::Defs.Localize(this.GetNameKey(), null, "", true, true);
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000CDE RID: 3294 RVA: 0x0008E8CC File Offset: 0x0008CACC
	// (set) Token: 0x06000CDF RID: 3295 RVA: 0x0008E8D4 File Offset: 0x0008CAD4
	public bool invalidateUI { get; private set; }

	// Token: 0x06000CE0 RID: 3296 RVA: 0x0008E8E0 File Offset: 0x0008CAE0
	public void Load(Logic.Kingdom logic)
	{
		this.Name = (this.ActiveName = logic.Name);
		this.id = logic.id;
		this.realms = new List<global::Realm>(logic.realms.Count);
		for (int i = 0; i < logic.realms.Count; i++)
		{
			global::Realm item = global::Realm.Get(logic.realms[i].id);
			this.realms.Add(item);
		}
		this.UpdateColors(logic);
		this.crest_id = logic.CoAIndex;
		DT.Field def = logic.def;
		if (def == null)
		{
			return;
		}
		DT.Field field = def.FindChild("unit_set", null, true, true, true, '.');
		if (field != null && field.children != null && field.children.Count > 0)
		{
			this.unit_sets = new List<string>(field.children.Count);
			for (int j = 0; j < field.children.Count; j++)
			{
				DT.Field field2 = field.children[j];
				if (field2 != null && !string.IsNullOrEmpty(field2.key))
				{
					this.unit_sets.Add(field2.key);
				}
			}
		}
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x0008EA0C File Offset: 0x0008CC0C
	public void UpdateColors(Logic.Kingdom logic)
	{
		Texture2D obj = global::Defs.GetObj<Texture2D>("Kingdom", "colors_texture", null);
		if (obj == null)
		{
			return;
		}
		int y = logic.is_player ? 1 : 0;
		int width = obj.width;
		if (logic.map_color >= 0)
		{
			this.MapColorIndex = logic.map_color;
			this.MapColor = obj.GetPixel(this.MapColorIndex, y);
		}
		else
		{
			this.MapColor = Color.white;
		}
		if (logic.primary_army_color >= 0)
		{
			this.PrimaryArmyColorIndex = logic.primary_army_color;
		}
		else
		{
			this.PrimaryArmyColorIndex = this.MapColorIndex;
		}
		this.PrimaryArmyColor = obj.GetPixel(this.PrimaryArmyColorIndex, y);
		if (logic.secondary_army_color >= 0)
		{
			this.SecondaryArmyColorIndex = logic.secondary_army_color;
			this.SecondaryArmyColor = obj.GetPixel(this.SecondaryArmyColorIndex - 1, y);
		}
		else
		{
			this.SecondaryArmyColor = Color.white;
		}
		WorldMap worldMap = WorldMap.Get();
		Color[] array = (worldMap != null) ? worldMap.unit_colors : null;
		this.unitColorID = global::Kingdom.PickClosestColorID(array, this.MapColor);
		if (this.unitColorID == -1)
		{
			this.unitColor = this.MapColor;
			return;
		}
		this.unitColor = array[this.unitColorID];
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x0008EB38 File Offset: 0x0008CD38
	public static int PickClosestColorID(Color[] colors, Color org)
	{
		int result = -1;
		if (colors == null)
		{
			return result;
		}
		float num = float.MaxValue;
		for (int i = 0; i < colors.Length; i++)
		{
			float num2 = Vector3.SqrMagnitude(colors[i] - org);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0008EB8C File Offset: 0x0008CD8C
	public static Color PickClosestColor(Color[] colors, Color org)
	{
		int num = global::Kingdom.PickClosestColorID(colors, org);
		if (num == -1)
		{
			return org;
		}
		return colors[num];
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x0008EBB0 File Offset: 0x0008CDB0
	public static Color PickMidColor(List<Color> colors, Color closest_color, params Color[] furthest_colors)
	{
		Color result = closest_color;
		float num = float.MaxValue;
		float num2 = float.MinValue;
		for (int i = 0; i < colors.Count; i++)
		{
			Color color = colors[i];
			float num3 = 0f;
			for (int j = 0; j < furthest_colors.Length; j++)
			{
				Vector3 vector = furthest_colors[j] - color;
				num3 += vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
			}
			float num4 = Vector3.SqrMagnitude(closest_color - color);
			if (num3 > num2 && num4 < num)
			{
				num2 = num3;
				num = num4;
				result = color;
			}
		}
		return result;
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0008EC8C File Offset: 0x0008CE8C
	private int DecideColor(int idx, int num_colors, bool map_color)
	{
		if (idx >= 0)
		{
			return idx;
		}
		Random.InitState(this.id);
		idx = Random.Range(0, num_colors);
		if (!map_color)
		{
			return idx;
		}
		Logic.Kingdom kingdom = this.logic;
		if (((kingdom != null) ? kingdom.neighbors : null) == null)
		{
			return idx;
		}
		int num = 1;
		int result = 0;
		int num2 = -1;
		for (int i = 0; i <= num_colors; i++)
		{
			int neighboursColorDist = this.GetNeighboursColorDist(idx, num_colors);
			if (neighboursColorDist > num2)
			{
				result = idx;
				num2 = neighboursColorDist;
				if (neighboursColorDist >= 4)
				{
					break;
				}
			}
			idx += num;
			if (idx > num_colors)
			{
				idx -= num_colors;
			}
		}
		if (num2 == 0)
		{
			Debug.LogError("Could not find unique color for kingdom '" + this.Name + "'");
		}
		return result;
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x0008ED28 File Offset: 0x0008CF28
	private bool HasNeighbourWithMapColorIndex(int idx)
	{
		foreach (Logic.Kingdom kingdom in this.logic.neighbors)
		{
			global::Kingdom kingdom2 = ((kingdom != null) ? kingdom.visuals : null) as global::Kingdom;
			if (kingdom2 != null && kingdom2.MapColorIndex == idx)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0008EDA0 File Offset: 0x0008CFA0
	private int GetNeighboursColorDist(int idx, int num_colors)
	{
		int num = int.MaxValue;
		foreach (Logic.Kingdom kingdom in this.logic.neighbors)
		{
			global::Kingdom kingdom2 = ((kingdom != null) ? kingdom.visuals : null) as global::Kingdom;
			if (kingdom2 != null)
			{
				int num2 = this.ColorDist(idx, kingdom2.MapColorIndex, num_colors);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0008EE24 File Offset: 0x0008D024
	private int ColorDist(int idx1, int idx2, int num_colors)
	{
		int num = idx1 - idx2;
		if (num < 0)
		{
			num = -num;
		}
		if (num > num_colors / 2)
		{
			num = num_colors - num;
		}
		return num;
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0008EE48 File Offset: 0x0008D048
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

	// Token: 0x06000CEA RID: 3306 RVA: 0x0008EEA0 File Offset: 0x0008D0A0
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.Kingdom kingdom = logic_obj as Logic.Kingdom;
		if (kingdom == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(kingdom.game.map_name))
		{
			return;
		}
		global::Kingdom kingdom2 = global::Kingdom.Get(kingdom.id);
		if (kingdom2 == null)
		{
			Debug.LogWarning("Could not create visual kingdom for " + kingdom.ToString());
			return;
		}
		if (kingdom2.logic != null)
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"Changing kingdom '",
				kingdom2.Name,
				"' logic from ",
				kingdom2.logic.ToString(),
				" to ",
				kingdom.ToString()
			}));
			kingdom2.logic.visuals = null;
		}
		kingdom2.logic = kingdom;
		kingdom.visuals = kingdom2;
		kingdom2.SyncRealms();
		kingdom2.SyncName();
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x0008EF68 File Offset: 0x0008D168
	private void OnPlayersChanged()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			worldUI.InvalidateKingdom();
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			worldMap.ReloadView();
		}
		Game game = GameLogic.Get(true);
		if (game != null)
		{
			for (int i = 0; i < game.realms.Count; i++)
			{
				Logic.Realm realm = game.realms[i];
				global::Realm realm2 = realm.visuals as global::Realm;
				if (realm2 != null)
				{
					realm2.UpdateFow(true, false);
				}
				for (int j = 0; j < realm.armies.Count; j++)
				{
					Logic.Army army = realm.armies[j];
					if (army.mercenary != null)
					{
						global::Army army2 = army.visuals as global::Army;
						if (army2 != null)
						{
							UIPVFigureArmy ui_pvFigure = army2.ui_pvFigure;
							if (ui_pvFigure != null)
							{
								ui_pvFigure.UpdateArmy();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x0008F030 File Offset: 0x0008D230
	private void WarDeclaredMusicTrigger(Logic.Kingdom other_k)
	{
		if (LoadingScreen.IsShown())
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (other_k == null || !kingdom.IsEnemy(other_k))
		{
			return;
		}
		BackgroundMusic.OnTrigger("WarDeclaredTrigger", other_k.religion.name);
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0008F06D File Offset: 0x0008D26D
	private void RefreshRoyalDungeonUIOnOffer(Offer offer)
	{
		if (offer is AskForPrisonerRansom)
		{
			this.logic.NotifyListeners("refresh_dungeon_ui_on_offer", null);
		}
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x0008F088 File Offset: 0x0008D288
	public void OnMessage(object obj, string message, object param)
	{
		uint num;
		if (this.logic != null && obj == this.logic && this.logic.is_local_player)
		{
			num = <PrivateImplementationDetails>.ComputeStringHash(message);
			if (num <= 1904481269U)
			{
				if (num <= 810626458U)
				{
					if (num != 819938U)
					{
						if (num != 493550662U)
						{
							if (num != 810626458U)
							{
								goto IL_1B2;
							}
							if (!(message == "stance_changed"))
							{
								goto IL_1B2;
							}
						}
						else if (!(message == "upgrade_finished"))
						{
							goto IL_1B2;
						}
					}
					else if (!(message == "realm_deleted"))
					{
						goto IL_1B2;
					}
				}
				else if (num != 1488516451U)
				{
					if (num != 1670696893U)
					{
						if (num != 1904481269U)
						{
							goto IL_1B2;
						}
						if (!(message == "upgrade_started"))
						{
							goto IL_1B2;
						}
					}
					else if (!(message == "gained_advantage"))
					{
						goto IL_1B2;
					}
				}
				else if (!(message == "local_player_changed"))
				{
					goto IL_1B2;
				}
			}
			else if (num <= 2751835415U)
			{
				if (num != 2275136621U)
				{
					if (num != 2478043169U)
					{
						if (num != 2751835415U)
						{
							goto IL_1B2;
						}
						if (!(message == "character_died"))
						{
							goto IL_1B2;
						}
					}
					else if (!(message == "relation_modified"))
					{
						goto IL_1B2;
					}
				}
				else if (!(message == "realm_added"))
				{
					goto IL_1B2;
				}
			}
			else if (num != 2804177153U)
			{
				if (num != 3095978933U)
				{
					if (num != 4004570535U)
					{
						goto IL_1B2;
					}
					if (!(message == "crown_authority_change"))
					{
						goto IL_1B2;
					}
				}
				else if (!(message == "resources_changed"))
				{
					goto IL_1B2;
				}
			}
			else if (!(message == "lost_advantage"))
			{
				goto IL_1B2;
			}
			BaseUI baseUI = BaseUI.Get();
			if (baseUI != null)
			{
				baseUI.RefreshTooltip(baseUI.tooltip, false);
			}
		}
		IL_1B2:
		num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 2153339806U)
		{
			if (num > 1375156690U)
			{
				if (num <= 1713771407U)
				{
					if (num <= 1621643185U)
					{
						if (num <= 1488516451U)
						{
							if (num != 1400281695U)
							{
								if (num != 1434340769U)
								{
									if (num != 1488516451U)
									{
										return;
									}
									if (!(message == "local_player_changed"))
									{
										return;
									}
									for (int i = 0; i < this.logic.game.realms.Count; i++)
									{
										Logic.Realm realm = this.logic.game.realms[i];
										for (int j = 0; j < realm.settlements.Count; j++)
										{
											global::Settlement settlement = realm.settlements[j].visuals as global::Settlement;
											if (!(settlement == null))
											{
												settlement.OnLocalPlayerChanged();
											}
										}
									}
									return;
								}
								else
								{
									if (!(message == "name_changed"))
									{
										return;
									}
									this.SyncName();
									return;
								}
							}
							else
							{
								if (!(message == "become_ecumenical_patriarch_divorce"))
								{
									return;
								}
								if (this.logic != BaseUI.LogicKingdom())
								{
									return;
								}
								Marriage marriage = param as Marriage;
								Vars vars = new Vars();
								vars.Set<Logic.Character>("husband", marriage.husband);
								vars.Set<Logic.Character>("wife", marriage.wife);
								MessageIcon.Create("BecomeEcumenicalPatriarchDivorce", vars, true, null);
								return;
							}
						}
						else if (num <= 1602027444U)
						{
							if (num != 1574549897U)
							{
								if (num != 1602027444U)
								{
									return;
								}
								if (!(message == "rebellion_independence_rebels"))
								{
									return;
								}
								if (this.logic != BaseUI.LogicKingdom())
								{
									return;
								}
								Vars vars2 = (param as Vars).Copy();
								bool flag = vars2.Get<bool>("formNewKingdom", false);
								Logic.Kingdom kingdom = vars2.Get<Logic.Kingdom>("independence_kingdom", null);
								List<Logic.Realm> list = vars2.Get<List<Logic.Realm>>("realms", null);
								if (kingdom != this.logic)
								{
									List<Logic.Realm> list2 = new List<Logic.Realm>();
									for (int k = 0; k < list.Count; k++)
									{
										Logic.Realm realm2 = list[k];
										if (realm2.GetLastOwner() == this.logic)
										{
											list2.Add(realm2);
										}
									}
									vars2.Set<List<Logic.Realm>>("realms", list2);
								}
								if (flag)
								{
									vars2.Set<Castle>("goto_target", (kingdom != null) ? kingdom.GetCapital().castle : null);
									MessageIcon.Create("RebellionIndependenceNewKingdomMessage", vars2, true, null);
									return;
								}
								if (kingdom == this.logic)
								{
									MessageIcon.Create("RebellionIndependenceExistingKingdomOwnMessage", vars2, true, null);
									return;
								}
								MessageIcon.Create("RebellionIndependenceExistingKingdomOtherMessage", vars2, true, null);
								return;
							}
							else
							{
								if (!(message == "new_jihad_left_war"))
								{
									return;
								}
								MessageIcon.Create("NewJihadLeftWar", param as Vars, true, null);
								return;
							}
						}
						else if (num != 1613562777U)
						{
							if (num != 1621643185U)
							{
								return;
							}
							if (!(message == "exquisite_ransom"))
							{
								return;
							}
							if (BaseUI.LogicKingdom() != this.logic)
							{
								return;
							}
							Vars vars3 = param as Vars;
							MessageIcon.Create("ExquisiteRansomMessage", vars3, true, null);
							return;
						}
						else
						{
							if (!(message == "open_audience_with"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							AudienceWindow.Create(global::Kingdom.Get((param as Logic.Kingdom).id), "Main", null);
							AudienceWindow.BringToFront();
							return;
						}
					}
					else if (num <= 1681745168U)
					{
						if (num <= 1649643086U)
						{
							if (num != 1631227408U)
							{
								if (num != 1649643086U)
								{
									return;
								}
								if (!(message == "finishing"))
								{
									return;
								}
								goto IL_21A0;
							}
							else
							{
								if (!(message == "sovereign_removed"))
								{
									return;
								}
								goto IL_1686;
							}
						}
						else if (num != 1670696893U)
						{
							if (num != 1681745168U)
							{
								return;
							}
							if (!(message == "war_lost_battles"))
							{
								return;
							}
							goto IL_3584;
						}
						else
						{
							if (!(message == "gained_advantage"))
							{
								return;
							}
							if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
							{
								Vars vars4 = param as Vars;
								Value val = vars4.Get("def_id", true);
								vars4.Set<KingdomAdvantage.Def>("advantage", this.logic.advantages.FindById(val).def);
								MessageIcon.Create("GainedAdvantageMessages", vars4, true, null);
								return;
							}
							return;
						}
					}
					else if (num <= 1700710542U)
					{
						if (num != 1695881059U)
						{
							if (num != 1700710542U)
							{
								return;
							}
							if (!(message == "rebellion_ended"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							Vars vars5 = param as Vars;
							Rebellion rebellion = vars5.obj.obj_val as Rebellion;
							if (rebellion.defeatedByKingdom != null && rebellion.defeatedByKingdom == BaseUI.LogicKingdom())
							{
								float num2 = 0f;
								if (rebellion.defeatedBy != null)
								{
									num2 = rebellion.defeatedBy.GetStat(Stats.cs_gold_from_rebellions_perc, true);
								}
								vars5.Set<float>("gold", rebellion.GetWealth() * (1f + num2 / 100f));
							}
							if (!rebellion.GetComponent<RebellionIndependence>().isDeclaringIndependence)
							{
								MessageIcon.Create("RebellionEndedMessage", vars5, true, null);
								BaseUI.PlayVoiceEvent(rebellion.rebel_def.field.GetString("rebellion_defeated_voice_line", null, "", true, true, true, '.'), null);
							}
							return;
						}
						else
						{
							if (!(message == "new_rumor"))
							{
								return;
							}
							if (BaseUI.LogicKingdom() != this.logic)
							{
								return;
							}
							Rumor val2 = param as Rumor;
							MessageIcon.Create("RumorMessage", new Vars(val2), true, null);
							return;
						}
					}
					else if (num != 1704452134U)
					{
						if (num != 1713771407U)
						{
							return;
						}
						if (!(message == "actions_created"))
						{
							return;
						}
					}
					else
					{
						if (!(message == "pope_converted_realm"))
						{
							return;
						}
						if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
						{
							MessageIcon.Create("PopeConvertedRealmMessage", new Vars(param as Logic.Realm), true, null);
							return;
						}
						return;
					}
				}
				else if (num <= 1943223803U)
				{
					if (num <= 1864153212U)
					{
						if (num <= 1832419971U)
						{
							if (num != 1768851834U)
							{
								if (num != 1832419971U)
								{
									return;
								}
								if (!(message == "end_alert"))
								{
									return;
								}
								MessageIcon.CreateEndAlert(this.logic);
								return;
							}
							else
							{
								if (!(message == "close_audience"))
								{
									return;
								}
								Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
								Logic.Kingdom kingdom3 = param as Logic.Kingdom;
								if (kingdom3 == null)
								{
									return;
								}
								if (this.logic != kingdom3 && this.logic != kingdom2)
								{
									return;
								}
								BaseUI baseUI2 = BaseUI.Get();
								AudienceWindow audienceWindow = (baseUI2 != null) ? baseUI2.window_dispatcher.GetWindow<AudienceWindow>() : null;
								if (audienceWindow == null)
								{
									return;
								}
								if (audienceWindow.kCurrent.logic != kingdom2)
								{
									return;
								}
								if (audienceWindow.kOther.logic != kingdom3)
								{
									return;
								}
								audienceWindow.Close(false);
								return;
							}
						}
						else if (num != 1854124734U)
						{
							if (num != 1864153212U)
							{
								return;
							}
							if (!(message == "offer_added"))
							{
								return;
							}
							Logic.Kingdom kingdom4 = BaseUI.LogicKingdom();
							if (kingdom4 != this.logic)
							{
								return;
							}
							Offer offer = param as Offer;
							this.RefreshRoyalDungeonUIOnOffer(offer);
							if (offer.to != kingdom4 && offer.from != kingdom4)
							{
								return;
							}
							if (offer.from == kingdom4)
							{
								offer.msg_icon = MessageIcon.Create(offer, "consider", true);
								return;
							}
							if (offer is SweetenOffer)
							{
								offer.msg_icon = MessageIcon.Create(offer, "sweeten_offer", true);
								return;
							}
							if (offer is CounterOffer || offer is AdditionalConditionOffer)
							{
								offer.msg_icon = MessageIcon.Create(offer, "counter_offer", true);
								return;
							}
							offer.msg_icon = MessageIcon.Create(offer, "proposed", true);
							return;
						}
						else
						{
							if (!(message == "prisoners_executed"))
							{
								return;
							}
							if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
							{
								MessageIcon.Create("ExecutedPrisonersMessages", param as Vars, true, null);
								return;
							}
							return;
						}
					}
					else if (num <= 1904481269U)
					{
						if (num != 1882626573U)
						{
							if (num != 1904481269U)
							{
								return;
							}
							if (!(message == "upgrade_started"))
							{
								return;
							}
							if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
							{
								Building.Def building;
								if ((building = (param as Building.Def)) != null)
								{
									MessageIcon.Create(BaseUI.LogicKingdom(), null, building, true);
								}
								BaseUI.PlaySound(BaseUI.soundsDef.FindChild("building_upgrades_started", null, true, true, true, '.'));
								return;
							}
							return;
						}
						else
						{
							if (!(message == "alliance_revoked"))
							{
								return;
							}
							Logic.Kingdom val3 = param as Logic.Kingdom;
							Logic.Kingdom kingdom5 = BaseUI.LogicKingdom();
							if (kingdom5 != this.logic)
							{
								return;
							}
							string text = Diplomacy.GetText(Diplomacy.TextType.Prompt, "AllianceRevoked", null, this.logic, kingdom5, null);
							if (text == null)
							{
								return;
							}
							Vars vars6 = new Vars();
							vars6.Set<string>("body", "#" + text);
							vars6.Set<Logic.Kingdom>("kingdom", val3);
							MessageIcon.Create("AllianceRevoked", vars6, true, null);
							return;
						}
					}
					else if (num != 1905569372U)
					{
						if (num != 1943223803U)
						{
							return;
						}
						if (!(message == "non_agression_ended"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						MessageIcon.Create("NonAgressionBrokenMessage", new Vars(param as Logic.Kingdom), true, null);
						return;
					}
					else
					{
						if (!(message == "pretender_replaced_by_enemy"))
						{
							return;
						}
						if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
						{
							Logic.Character character = param as Logic.Character;
							Vars vars7 = new Vars();
							vars7.Set<Logic.Character>("puppet", character);
							vars7.Set<Logic.Kingdom>("kingdom", character.GetKingdom());
							MessageIcon.Create("PretenderToTheThroneLostMessage", vars7, true, null);
							return;
						}
						return;
					}
				}
				else if (num <= 2105032289U)
				{
					if (num <= 2059377048U)
					{
						if (num != 1981548864U)
						{
							if (num != 2059377048U)
							{
								return;
							}
							if (!(message == "rebellion_independce_newKingdom_loyalists"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							MessageIcon.Create("RebellionIndependenceNewKingdomLoyalistsMessage", param as Vars, true, null);
							return;
						}
						else
						{
							if (!(message == "all_alliances_revoked"))
							{
								return;
							}
							if (BaseUI.LogicKingdom() != this.logic)
							{
								return;
							}
							MessageIcon.Create("AllAlliancesRevoked", null, true, null);
							return;
						}
					}
					else if (num != 2068350458U)
					{
						if (num != 2105032289U)
						{
							return;
						}
						if (!(message == "religion_changed"))
						{
							return;
						}
						if (BaseUI.LogicKingdom() == this.logic)
						{
							BackgroundMusic.ResetDefaultMusicGroup();
						}
						return;
					}
					else
					{
						if (!(message == "great_powers_changed"))
						{
							return;
						}
						Logic.Kingdom kingdom6 = BaseUI.LogicKingdom();
						this.UpdatePVFigurniesAndSettlements();
						if (this.logic != kingdom6)
						{
							return;
						}
						bool flag2 = (bool)param;
						Vars vars8 = new Vars(this.logic);
						KingdomRanking fame_ranking = this.logic.game.great_powers.fame_ranking;
						float num3 = (float)fame_ranking.Find(kingdom6).rank / (float)fame_ranking.last_rank * 100f;
						vars8.Set<bool>("in_top_perc_X", num3 <= fame_ranking.def.in_top_perc_X);
						vars8.Set<bool>("in_bottom_perc_X", num3 >= 100f - fame_ranking.def.in_bottom_perc_X);
						vars8.Set<bool>("in_mid_perc_X", num3 < 100f - fame_ranking.def.in_bottom_perc_X && num3 > fame_ranking.def.in_top_perc_X);
						vars8.Set<KingdomRanking>("ranking", fame_ranking);
						vars8.Set<string>("ranking_name", "#" + global::Defs.Localize(fame_ranking.def.field, "name", null, null, true, true));
						vars8.Set<string>("ranking_description", "#" + global::Defs.Localize(fame_ranking.def.field, "description", null, null, true, true));
						if (flag2)
						{
							MessageIcon.Create("WeAreGreatPowerMessage", vars8, true, null);
							return;
						}
						MessageIcon.Create("WeAreNotGreatPowerMessage", vars8, true, null);
						return;
					}
				}
				else if (num <= 2140587311U)
				{
					if (num != 2116962933U)
					{
						if (num != 2140587311U)
						{
							return;
						}
						if (!(message == "war_drawn_no_battles_supporter"))
						{
							return;
						}
						goto IL_3584;
					}
					else
					{
						if (!(message == "kingdom_split"))
						{
							return;
						}
						if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
						{
							MessageIcon.Create("KingdomSplitMessage", param as Vars, true, null);
							return;
						}
						return;
					}
				}
				else if (num != 2150162212U)
				{
					if (num != 2153339806U)
					{
						return;
					}
					if (!(message == "started"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "war_won_no_battles"))
					{
						return;
					}
					goto IL_3584;
				}
				this.CreateActions();
				return;
			}
			if (num <= 608133746U)
			{
				if (num <= 212553184U)
				{
					if (num <= 71549386U)
					{
						if (num != 819938U)
						{
							if (num != 39812634U)
							{
								if (num != 71549386U)
								{
									return;
								}
								if (!(message == "become_patriarch_divorce"))
								{
									return;
								}
								if (this.logic != BaseUI.LogicKingdom())
								{
									return;
								}
								Marriage marriage2 = param as Marriage;
								Vars vars9 = new Vars();
								vars9.Set<Logic.Character>("husband", marriage2.husband);
								vars9.Set<Logic.Character>("wife", marriage2.wife);
								MessageIcon.Create("BecomePatriarchDivorce", vars9, true, null);
								return;
							}
							else
							{
								if (!(message == "become_pope_divorce"))
								{
									return;
								}
								if (this.logic != BaseUI.LogicKingdom())
								{
									return;
								}
								Marriage marriage3 = param as Marriage;
								Vars vars10 = new Vars();
								vars10.Set<Logic.Character>("husband", marriage3.husband);
								vars10.Set<Logic.Character>("princess", marriage3.wife);
								MessageIcon.Create("BecomePopeDivorce", vars10, true, null);
								return;
							}
						}
						else
						{
							if (!(message == "realm_deleted"))
							{
								return;
							}
							goto IL_170C;
						}
					}
					else if (num <= 150182469U)
					{
						if (num != 107265207U)
						{
							if (num != 150182469U)
							{
								return;
							}
							if (!(message == "rebellion_leader_started"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							Vars vars11 = param as Vars;
							Rebellion rebellion2 = vars11.obj.obj_val as Rebellion;
							vars11.Set<Logic.Army>("goto_target", rebellion2.leader.army);
							MessageIcon.Create("RebellionSpawnedMessage", vars11, true, null);
							return;
						}
						else
						{
							if (!(message == "elder_prince_rebelled"))
							{
								return;
							}
							if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
							{
								MessageIcon.Create("ElderPrinceRebelledMessage", param as Vars, true, null);
								return;
							}
							return;
						}
					}
					else if (num != 178367286U)
					{
						if (num != 212553184U)
						{
							return;
						}
						if (!(message == "grant_independence_to"))
						{
							return;
						}
						if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
						{
							DT.Field soundsDef = BaseUI.soundsDef;
							BaseUI.PlayVoiceEvent((soundsDef != null) ? soundsDef.GetString("independence_granted_to_them", null, "", true, true, true, '.') : null, null);
							return;
						}
						return;
					}
					else
					{
						if (!(message == "crusade_give_realm_to_kingdom"))
						{
							return;
						}
						Logic.Kingdom kingdom7 = BaseUI.LogicKingdom();
						Crusade crusade = param as Crusade;
						if (!crusade.IsConnectedToCrusade(this.logic))
						{
							return;
						}
						Vars vars12 = new Vars(crusade);
						vars12.Set<Castle>("goto_target", crusade.last_captured_realm.castle);
						if (kingdom7 == this.logic)
						{
							MessageIcon.Create("CrusadeTargetProvinceJoinedOurKingdom", vars12, true, null);
							return;
						}
						MessageIcon.Create("CrusadeTargetProvinceJoinedOtherKingdom", vars12, true, null);
						return;
					}
				}
				else if (num <= 491740382U)
				{
					if (num <= 298126040U)
					{
						if (num != 274589411U)
						{
							if (num != 298126040U)
							{
								return;
							}
							if (!(message == "rebellion_new_leader"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							Vars vars13 = param as Vars;
							Rebellion rebellion3 = vars13.obj.obj_val as Rebellion;
							vars13.Set<Logic.Army>("goto_target", rebellion3.leader.army);
							MessageIcon.Create("RebellionLeaderMessage", vars13, true, null);
							return;
						}
						else
						{
							if (!(message == "del_pact"))
							{
								return;
							}
							goto IL_169C;
						}
					}
					else if (num != 405303042U)
					{
						if (num != 491740382U)
						{
							return;
						}
						if (!(message == "rankings_became_leading_ruler"))
						{
							return;
						}
						Logic.Kingdom kingdom8 = BaseUI.LogicKingdom();
						Vars vars14 = param as Vars;
						if (this.logic != kingdom8)
						{
							vars14.Set<bool>("update_timers", false);
							return;
						}
						KingdomRanking kingdomRanking = vars14.Get<KingdomRanking>("ranking", null);
						float num4 = (float)kingdomRanking.Find(kingdom8).rank / (float)kingdomRanking.last_rank * 100f;
						vars14.Set<bool>("in_top_perc_X", num4 <= kingdomRanking.def.in_top_perc_X);
						vars14.Set<bool>("in_bottom_perc_X", num4 >= 100f - kingdomRanking.def.in_bottom_perc_X);
						vars14.Set<bool>("in_mid_perc_X", num4 < 100f - kingdomRanking.def.in_bottom_perc_X && num4 > kingdomRanking.def.in_top_perc_X);
						vars14.Set<string>("ranking_name", "#" + global::Defs.Localize(kingdomRanking.def.field, "name", null, null, true, true));
						vars14.Set<string>("ranking_description", "#" + global::Defs.Localize(kingdomRanking.def.field, "description", null, null, true, true));
						MessageIcon.Create("RankingsBecameLeadingRulerMessage", vars14, true, null);
						return;
					}
					else
					{
						if (!(message == "war_supporter_joined"))
						{
							return;
						}
						Logic.Kingdom kingdom9 = BaseUI.LogicKingdom();
						if (kingdom9 != this.logic)
						{
							return;
						}
						War war = param as War;
						if (!war.IsLeader(kingdom9) || !war.IsJihad() || !kingdom9.is_muslim)
						{
							return;
						}
						List<Logic.Kingdom> allies = war.GetAllies(kingdom9);
						if (allies[allies.Count - 1].is_muslim)
						{
							DT.Field soundsDef2 = BaseUI.soundsDef;
							BaseUI.PlayVoiceEvent((soundsDef2 != null) ? soundsDef2.GetString("our_jihad_gained_muslim_ally", null, "", true, true, true, '.') : null, null);
							return;
						}
						DT.Field soundsDef3 = BaseUI.soundsDef;
						BaseUI.PlayVoiceEvent((soundsDef3 != null) ? soundsDef3.GetString("our_jihad_gained_non_muslim_ally", null, "", true, true, true, '.') : null, null);
						return;
					}
				}
				else if (num <= 548558425U)
				{
					if (num != 493550662U)
					{
						if (num != 548558425U)
						{
							return;
						}
						if (!(message == "prison_event"))
						{
							return;
						}
						if (BaseUI.LogicKingdom() != this.logic)
						{
							return;
						}
						Vars vars15 = param as Vars;
						MessageIcon.Create(vars15.Get<string>("message", null), vars15, true, null);
						return;
					}
					else
					{
						if (!(message == "upgrade_finished"))
						{
							return;
						}
						if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
						{
							BaseUI.PlaySound(BaseUI.soundsDef.FindChild("building_upgraded", null, true, true, true, '.'));
							BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("building_upgraded_voice_line", null, true, true, true, '.'), null);
							return;
						}
						return;
					}
				}
				else if (num != 576266046U)
				{
					if (num != 608133746U)
					{
						return;
					}
					if (!(message == "occupied_realm_lost_bankrupcy"))
					{
						return;
					}
					if (this.logic != BaseUI.LogicKingdom())
					{
						return;
					}
					MessageIcon.Create("OccupiedRealmLostBankrupcy", new Vars(param as Logic.Realm), true, null);
					return;
				}
				else
				{
					if (!(message == "war_lost_no_battles"))
					{
						return;
					}
					goto IL_3584;
				}
			}
			else if (num <= 810626458U)
			{
				if (num <= 635839150U)
				{
					if (num != 623326961U)
					{
						if (num != 630419299U)
						{
							if (num != 635839150U)
							{
								return;
							}
							if (!(message == "new_jihad_lost_ally"))
							{
								return;
							}
							MessageIcon.Create("NewJihadLostAlly", param as Vars, true, null);
							return;
						}
						else
						{
							if (!(message == "crusade_establish_new_kingdom"))
							{
								return;
							}
							Crusade crusade2 = param as Crusade;
							Vars vars16 = new Vars(crusade2);
							vars16.Set<Castle>("goto_target", crusade2.last_captured_realm.castle);
							MessageIcon.Create("CrusadeTargetProvinceFormedNewKingdom", vars16, true, null);
							return;
						}
					}
					else
					{
						if (!(message == "prison_changed"))
						{
							return;
						}
						using (Game.Profile("Kingdim(visuals) OnMessage", false, 0f, null))
						{
							Logic.Character character2 = param as Logic.Character;
							if (character2 == null)
							{
								return;
							}
							if (Game.isLoadingSaveGame)
							{
								return;
							}
							if (character2.IsPrisoner())
							{
								if (character2.prison_reason == "battle")
								{
									return;
								}
								Logic.Kingdom kingdom10 = BaseUI.LogicKingdom();
								if (character2.kingdom_id == kingdom10.id)
								{
									DT.Field soundsDef4 = BaseUI.soundsDef;
									BaseUI.PlayVoiceEvent((soundsDef4 != null) ? soundsDef4.GetString("own_knight_imprisoned", null, "", true, true, true, '.') : null, character2);
									DT.Field soundsDef5 = BaseUI.soundsDef;
									BaseUI.PlaySoundEvent((soundsDef5 != null) ? soundsDef5.GetString("imprisoned_sfx", null, "", true, true, true, '.') : null, null);
								}
								else if (character2.prison_kingdom == kingdom10)
								{
									if (!character2.IsSpy() || character2.prison_reason == "dev_cheat")
									{
										DT.Field soundsDef6 = BaseUI.soundsDef;
										BaseUI.PlayVoiceEvent((soundsDef6 != null) ? soundsDef6.GetString("enemy_knight_imprisoned", null, "", true, true, true, '.') : null, character2);
									}
									DT.Field soundsDef7 = BaseUI.soundsDef;
									BaseUI.PlaySoundEvent((soundsDef7 != null) ? soundsDef7.GetString("imprisoned_sfx", null, "", true, true, true, '.') : null, null);
								}
							}
							else
							{
								Logic.Kingdom kingdom11 = BaseUI.LogicKingdom();
								if (character2.kingdom_id == kingdom11.id)
								{
									string prison_reason = character2.prison_reason;
									if (!(prison_reason == "diplomacy"))
									{
										if (prison_reason == "ransomed")
										{
											DT.Field soundsDef8 = BaseUI.soundsDef;
											BaseUI.PlayVoiceEvent((soundsDef8 != null) ? soundsDef8.GetString("successfull_ransom_prisoner", null, "", true, true, true, '.') : null, character2);
										}
									}
									else
									{
										DT.Field soundsDef9 = BaseUI.soundsDef;
										BaseUI.PlayVoiceEvent((soundsDef9 != null) ? soundsDef9.GetString("own_knight_released", null, "", true, true, true, '.') : null, character2);
									}
								}
							}
						}
						return;
					}
				}
				else if (num <= 780602723U)
				{
					if (num != 656179254U)
					{
						if (num != 780602723U)
						{
							return;
						}
						if (!(message == "kingdom_destroyed_recall_characters"))
						{
							return;
						}
						if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
						{
							MessageIcon.Create("CharactersRecalledKingdomDestroyedMessage", param as Vars, true, null);
							return;
						}
						return;
					}
					else
					{
						if (!(message == "players_changed"))
						{
							return;
						}
						if (param != null && (bool)param)
						{
							return;
						}
						if (WorldUI.Get() == null)
						{
							return;
						}
						Logic.Kingdom kingdom12 = this.logic;
						Logic.Multiplayer multiplayer;
						if (kingdom12 == null)
						{
							multiplayer = null;
						}
						else
						{
							Game game = kingdom12.game;
							multiplayer = ((game != null) ? game.multiplayer : null);
						}
						Logic.Multiplayer multiplayer2 = multiplayer;
						if (multiplayer2 == null)
						{
							return;
						}
						if (Logic.Multiplayer.CurrentPlayers.GetByPID(multiplayer2.playerData.pid) == null)
						{
							return;
						}
						this.m_InvalidatePlayerChanged = true;
						this.LateUpdate();
						return;
					}
				}
				else if (num != 808668720U)
				{
					if (num != 810626458U)
					{
						return;
					}
					if (!(message == "stance_changed"))
					{
						return;
					}
					goto IL_146F;
				}
				else
				{
					if (!(message == "new_rumors"))
					{
						return;
					}
					if (BaseUI.LogicKingdom() != this.logic)
					{
						return;
					}
					Vars vars17 = param as Vars;
					MessageIcon.Create("RumorsMessage", vars17, true, null);
					return;
				}
			}
			else if (num <= 955079637U)
			{
				if (num <= 842388098U)
				{
					if (num != 819120665U)
					{
						if (num != 842388098U)
						{
							return;
						}
						if (!(message == "mercenary_spawned"))
						{
							return;
						}
						Logic.Army val4 = param as Logic.Army;
						WorldUI worldUI = WorldUI.Get();
						if (worldUI == null || worldUI.kingdom != this.logic.id)
						{
							return;
						}
						Vars vars18 = new Vars(val4);
						MessageIcon.Create("MercenariesMessage", vars18, true, null);
						return;
					}
					else
					{
						if (!(message == "end_war_and_take_whats_ours_dominator"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						MessageIcon.Create("EndWarAndTakeWhatsOursMessage", new Vars(param as Offer), true, null);
						return;
					}
				}
				else if (num != 856462318U)
				{
					if (num != 955079637U)
					{
						return;
					}
					if (!(message == "victory"))
					{
						return;
					}
					Logic.Kingdom kingdom13 = BaseUI.LogicKingdom();
					if (this.id == kingdom13.id)
					{
						UIEndGameWindow.ShowVictoryWindow(param as string);
						kingdom13.game.pause.AddRequest("EndGamePause", -2);
					}
					return;
				}
				else
				{
					if (!(message == "war_won_battles"))
					{
						return;
					}
					goto IL_3584;
				}
			}
			else if (num <= 1211309691U)
			{
				if (num != 1039780190U)
				{
					if (num != 1211309691U)
					{
						return;
					}
					if (!(message == "destroying"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "defeat"))
					{
						return;
					}
					Logic.Kingdom kingdom14 = BaseUI.LogicKingdom();
					if (this.id == kingdom14.id)
					{
						MessageIcon.DeleteAll();
						BaseUI baseUI3 = BaseUI.Get();
						if (baseUI3 != null)
						{
							baseUI3.window_dispatcher.CloseAllWindows();
						}
						UIEndGameWindow.ShowDefeatWindow(kingdom14.IsDefeated() ? "KingdomDefeated" : (param as string));
						kingdom14.game.pause.AddRequest("EndGamePause", -2);
					}
					return;
				}
			}
			else if (num != 1253911156U)
			{
				if (num != 1375156690U)
				{
					return;
				}
				if (!(message == "kingdom_dominated"))
				{
					return;
				}
				Logic.Kingdom kingdom15 = BaseUI.LogicKingdom();
				if (this.logic == kingdom15)
				{
					MessageIcon.Create("OwnKingdomDominated", null, true, null);
					return;
				}
				for (int l = 0; l < this.logic.wars.Count; l++)
				{
					if (this.logic.wars[l].GetEnemies(this.logic).Contains(kingdom15))
					{
						MessageIcon.Create("EnemyKingdomDominated", new Vars(this.logic), true, null);
					}
				}
				return;
			}
			else
			{
				if (!(message == "prisoners_released"))
				{
					return;
				}
				if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
				{
					MessageIcon.Create("ReleasedPrisonersMessages", param as Vars, true, null);
					return;
				}
				return;
			}
			IL_21A0:
			this.logic.DelListener(this);
			this.logic = null;
			return;
		}
		if (num <= 3079482253U)
		{
			if (num <= 2696550754U)
			{
				if (num <= 2299906211U)
				{
					if (num <= 2261655967U)
					{
						if (num != 2158144290U)
						{
							if (num != 2212907174U)
							{
								if (num != 2261655967U)
								{
									return;
								}
								if (!(message == "war_started"))
								{
									return;
								}
							}
							else
							{
								if (!(message == "kingdom_destroyed"))
								{
									return;
								}
								Vars vars19 = (Vars)param;
								vars19.Get<Logic.Kingdom>("destroyed_kingdom", null);
								vars19.Get<Logic.Kingdom>("destroyed_by", null);
								List<Logic.Kingdom> list3 = vars19.Get<List<Logic.Kingdom>>("vassal_states", null);
								Logic.Kingdom kingdom16 = vars19.Get<Logic.Kingdom>("sovereign", null);
								Logic.Kingdom kingdom17 = BaseUI.LogicKingdom();
								UIImportantEvents.UpdateCategory("quests");
								if (list3 != null && list3.Contains(kingdom17))
								{
									vars19.Set<bool>("wasSovereign", true);
								}
								if (kingdom16 == kingdom17)
								{
									vars19.Set<bool>("wasVassal", true);
								}
								if (this.id != kingdom17.id)
								{
									MessageIcon.Create("KingdomDestroyed", vars19, true, null);
								}
								return;
							}
						}
						else
						{
							if (!(message == "new_marriage"))
							{
								return;
							}
							if (BaseUI.LogicKingdom() == this.logic)
							{
								Marriage marriage4 = param as Marriage;
								Logic.Kingdom kingdom18 = (marriage4 != null) ? marriage4.GetOtherKingdom(this.logic) : null;
								if (kingdom18 == null)
								{
									return;
								}
								Religion religion = this.logic.religion;
								Religion religion2 = kingdom18.religion;
								if (!religion.def.christian && religion2.def.christian)
								{
									string key = "WeddingDifChristianTrigger";
									string religion3;
									if (kingdom18 == null)
									{
										religion3 = null;
									}
									else
									{
										Religion religion4 = kingdom18.religion;
										religion3 = ((religion4 != null) ? religion4.name : null);
									}
									BackgroundMusic.OnTrigger(key, religion3);
									return;
								}
								if (!religion.def.muslim && religion2.def.muslim)
								{
									string key2 = "WeddingDifMuslimTrigger";
									string religion5;
									if (kingdom18 == null)
									{
										religion5 = null;
									}
									else
									{
										Religion religion6 = kingdom18.religion;
										religion5 = ((religion6 != null) ? religion6.name : null);
									}
									BackgroundMusic.OnTrigger(key2, religion5);
									return;
								}
								if (!religion.def.pagan && religion2.def.pagan)
								{
									string key3 = "WeddingDifPaganTrigger";
									string religion7;
									if (kingdom18 == null)
									{
										religion7 = null;
									}
									else
									{
										Religion religion8 = kingdom18.religion;
										religion7 = ((religion8 != null) ? religion8.name : null);
									}
									BackgroundMusic.OnTrigger(key3, religion7);
									return;
								}
								string key4 = "WeddingTrigger";
								string religion9;
								if (kingdom18 == null)
								{
									religion9 = null;
								}
								else
								{
									Religion religion10 = kingdom18.religion;
									religion9 = ((religion10 != null) ? religion10.name : null);
								}
								BackgroundMusic.OnTrigger(key4, religion9);
							}
							return;
						}
					}
					else if (num <= 2279180478U)
					{
						if (num != 2275136621U)
						{
							if (num != 2279180478U)
							{
								return;
							}
							if (!(message == "start_alert"))
							{
								return;
							}
							MessageIcon.CreateStartAlert(this.logic, param as Logic.Character);
							return;
						}
						else
						{
							if (!(message == "realm_added"))
							{
								return;
							}
							goto IL_170C;
						}
					}
					else if (num != 2287749952U)
					{
						if (num != 2299906211U)
						{
							return;
						}
						if (!(message == "rebellion_new_rebel"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						Vars vars20 = param as Vars;
						Value obj2 = vars20.obj;
						Logic.Rebel rebel = vars20.Get<Logic.Rebel>("param", null);
						vars20.Set<Logic.Army>("goto_target", rebel.army);
						Logic.Kingdom kingdom19 = rebel.GetKingdom();
						if (kingdom19.type == Logic.Kingdom.Type.LoyalistsFaction)
						{
							vars20.Set<string>("rebel_type", "loyalist");
						}
						else if (kingdom19.type == Logic.Kingdom.Type.ReligiousFaction)
						{
							vars20.Set<string>("rebel_type", "religious");
						}
						MessageIcon.Create("RebellionNewRebelMessage", vars20, true, null);
						return;
					}
					else
					{
						if (!(message == "changed_sovereign"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						MessageIcon.Create("ChangedSovereignMessage", param as Vars, true, null);
						return;
					}
				}
				else if (num <= 2474782131U)
				{
					if (num <= 2418603302U)
					{
						if (num != 2314870149U)
						{
							if (num != 2418603302U)
							{
								return;
							}
							if (!(message == "princess_becomes_queen"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							Vars vars21 = param as Vars;
							vars21.Set<string>("old_title", this.logic.nobility_key + ".Princess");
							MessageIcon.Create("PrincessBecomesQueen", vars21, true, null);
							return;
						}
						else
						{
							if (!(message == "reveal_pact"))
							{
								return;
							}
							goto IL_16EB;
						}
					}
					else if (num != 2463795085U)
					{
						if (num != 2474782131U)
						{
							return;
						}
						if (!(message == "unite_quest_complete"))
						{
							return;
						}
						if (this.logic == null)
						{
							return;
						}
						Logic.Kingdom kingdom20 = BaseUI.LogicKingdom();
						Vars vars22 = param as Vars;
						List<Logic.Kingdom> list4 = vars22.Get<List<Logic.Kingdom>>("eligible_kingdoms", null);
						if (this.logic == kingdom20)
						{
							MessageIcon.Create("UniteQuestOwnerMessage", vars22, true, null);
							return;
						}
						if (list4 != null && list4.Contains(kingdom20))
						{
							MessageIcon.Create("UniteQuestOtherEligibleKingdomMessage", vars22, true, null);
							return;
						}
						MessageIcon.Create("UniteQuestOtherKingdomMessage", vars22, true, null);
						return;
					}
					else
					{
						if (!(message == "princess_inheritance"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						MessageIcon.CreateInheritance("ClaimIneritanceMessage", this.logic, param as Logic.Kingdom, true);
						return;
					}
				}
				else if (num <= 2556358422U)
				{
					if (num != 2478043169U)
					{
						if (num != 2556358422U)
						{
							return;
						}
						if (!(message == "new_knight_hired"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						Logic.Character character3 = param as Logic.Character;
						character3.select_army_on_spawn = true;
						if (!character3.IsKing())
						{
							global::Kingdom.PlayHireSound(character3);
						}
						return;
					}
					else
					{
						if (!(message == "relation_modified"))
						{
							return;
						}
						Reason reason = param as Reason;
						if (reason == null)
						{
							return;
						}
						Logic.Kingdom kingdom21 = BaseUI.LogicKingdom();
						if (reason.source != kingdom21 || this.logic == kingdom21)
						{
							return;
						}
						Logic.Kingdom kingdom22 = reason.target as Logic.Kingdom;
						if (kingdom22 == null || kingdom22.IsDefeated())
						{
							return;
						}
						if (!reason.isDirect && (float)kingdom21.DistanceToKingdom(kingdom22) > RelationUtils.Def.max_spread_reaction_kingdom_distance)
						{
							return;
						}
						bool flag3 = reason.value > 0f;
						Offer offer2 = ((reason != null) ? reason.vars : null) as Offer;
						if (offer2 == null)
						{
							Vars vars23 = ((reason != null) ? reason.vars : null) as Vars;
							offer2 = (((vars23 != null) ? vars23.obj.obj_val : null) as Offer);
						}
						string text2 = Diplomacy.GetText(flag3 ? Diplomacy.TextType.Pleased : Diplomacy.TextType.Angry, null, offer2, this.logic, kingdom21, reason);
						if (text2 == null)
						{
							return;
						}
						text2 = "#" + text2;
						Vars vars24 = new Vars(text2);
						vars24.Set<Logic.Kingdom>("kingdom", this.logic);
						vars24.Set<Logic.Kingdom>("target", kingdom22);
						vars24.Set<float>("rel_change_amount", reason.initial_value);
						if (offer2 != null)
						{
							vars24.Set<Offer>("offer", offer2);
						}
						if (flag3)
						{
							MessageIcon.Create("KingdomIsPleased", vars24, true, null);
							return;
						}
						MessageIcon.Create("KingdomIsAngry", vars24, true, null);
						return;
					}
				}
				else if (num != 2626030674U)
				{
					if (num != 2696550754U)
					{
						return;
					}
					if (!(message == "war_drawn_battles"))
					{
						return;
					}
					goto IL_3584;
				}
				else
				{
					if (!(message == "royal_ties_broken"))
					{
						return;
					}
					if (this.logic != BaseUI.LogicKingdom())
					{
						return;
					}
					MessageIcon.Create("RoyalTiesBrokenMessage", new Vars(param as Logic.Kingdom), true, null);
					return;
				}
			}
			else if (num <= 2827744275U)
			{
				if (num <= 2767150581U)
				{
					if (num <= 2740931685U)
					{
						if (num != 2714262534U)
						{
							if (num != 2740931685U)
							{
								return;
							}
							if (!(message == "declared_inheritance_war"))
							{
								return;
							}
							if (BaseUI.LogicKingdom() == this.logic)
							{
								BaseUI.PlaySound(BaseUI.soundsDef.FindChild("war_declared_sound_effect", null, true, true, true, '.'));
								this.WarDeclaredMusicTrigger(param as Logic.Kingdom);
								return;
							}
							return;
						}
						else
						{
							if (!(message == "rebellion_famous_state_changed"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							Vars vars25 = param as Vars;
							Rebellion rebellion4 = vars25.obj.obj_val as Rebellion;
							vars25.Set<Logic.Army>("goto_target", rebellion4.leader.army);
							if (rebellion4.IsFamous())
							{
								MessageIcon.Create("RebellionBecameFamousMessage", vars25, true, null);
							}
							return;
						}
					}
					else if (num != 2751835415U)
					{
						if (num != 2767150581U)
						{
							return;
						}
						if (!(message == "eow_gained_support"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						Vars vars26 = new Vars();
						vars26.Set<Logic.Kingdom>("kingdom", param as Logic.Kingdom);
						vars26.Set<Logic.Kingdom>("plr_kingdom", this.logic);
						MessageIcon.Create("EmperorOfTheWorldGainedSupportMessage", vars26, true, null);
						return;
					}
					else
					{
						if (!(message == "character_died"))
						{
							return;
						}
						if (this.logic == BaseUI.LogicKingdom())
						{
							Vars vars27 = param as Vars;
							if (vars27 == null)
							{
								return;
							}
							if (vars27.Get<string>("title_key", null) == "King")
							{
								Religion religion11 = this.logic.religion;
								if (religion11.def.christian)
								{
									BackgroundMusic.OnTrigger("KingDiedChristianTrigger", null);
									return;
								}
								if (religion11.def.muslim)
								{
									BackgroundMusic.OnTrigger("KingDiedMuslimTrigger", null);
									return;
								}
								if (religion11.def.pagan)
								{
									BackgroundMusic.OnTrigger("KingDiedPaganTrigger", null);
								}
							}
						}
						return;
					}
				}
				else if (num <= 2799596032U)
				{
					if (num != 2775853964U)
					{
						if (num != 2799596032U)
						{
							return;
						}
						if (!(message == "rankings_leading_ruler"))
						{
							return;
						}
						Logic.Kingdom kingdom23 = BaseUI.LogicKingdom();
						Vars vars28 = param as Vars;
						KingdomRanking kingdomRanking2 = vars28.Get<KingdomRanking>("ranking", null);
						vars28.Set<string>("ranking_name", "#" + global::Defs.Localize(kingdomRanking2.def.field, "name", null, null, true, true));
						vars28.Set<string>("ranking_description", "#" + global::Defs.Localize(kingdomRanking2.def.field, "description", null, null, true, true));
						float num5 = (float)kingdomRanking2.Find(kingdom23).rank / (float)kingdomRanking2.last_rank * 100f;
						vars28.Set<bool>("in_top_perc_X", num5 <= kingdomRanking2.def.in_top_perc_X);
						vars28.Set<bool>("in_bottom_perc_X", num5 >= 100f - kingdomRanking2.def.in_bottom_perc_X);
						vars28.Set<bool>("in_mid_perc_X", num5 < 100f - kingdomRanking2.def.in_bottom_perc_X && num5 > kingdomRanking2.def.in_top_perc_X);
						if (this.logic == kingdom23)
						{
							MessageIcon.Create("RankingsWeAreLeadingRulerMessage", vars28, true, null);
							return;
						}
						MessageIcon.Create("RankingsTheyAreLeadingRulerMessage", vars28, true, null);
						return;
					}
					else
					{
						if (!(message == "rel_changed_cleric"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						MessageIcon.Create("RelationsImprovedCleric", param as Vars, true, null);
						return;
					}
				}
				else if (num != 2804177153U)
				{
					if (num != 2827744275U)
					{
						return;
					}
					if (!(message == "draw"))
					{
						return;
					}
					Logic.Kingdom kingdom24 = BaseUI.LogicKingdom();
					if (this.id == kingdom24.id)
					{
						MessageIcon.DeleteAll();
						BaseUI baseUI4 = BaseUI.Get();
						if (baseUI4 != null)
						{
							baseUI4.window_dispatcher.CloseAllWindows();
						}
						UIEndGameWindow.ShowDrawWindow(param as string);
						kingdom24.game.pause.AddRequest("EndGamePause", -2);
					}
					return;
				}
				else
				{
					if (!(message == "lost_advantage"))
					{
						return;
					}
					if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
					{
						Vars vars29 = param as Vars;
						Value val5 = vars29.Get("def_id", true);
						vars29.Set<KingdomAdvantage.Def>("advantage", this.logic.advantages.FindById(val5).def);
						MessageIcon.Create("LostAdvantageMessages", vars29, true, null);
						DT.Field soundsDef10 = BaseUI.soundsDef;
						BaseUI.PlayVoiceEvent((soundsDef10 != null) ? soundsDef10.GetString("narrator_advantage_lost", null, "", true, true, true, '.') : null, null);
						return;
					}
					return;
				}
			}
			else if (num <= 2941838617U)
			{
				if (num <= 2899143261U)
				{
					if (num != 2884388625U)
					{
						if (num != 2899143261U)
						{
							return;
						}
						if (!(message == "stance_changed_initial"))
						{
							return;
						}
						goto IL_146F;
					}
					else
					{
						if (!(message == "inheritance_war_declared"))
						{
							return;
						}
						if (BaseUI.LogicKingdom() == this.logic)
						{
							BaseUI.PlaySound(BaseUI.soundsDef.FindChild("war_declared_sound_effect", null, true, true, true, '.'));
							DT.Field soundsDef11 = BaseUI.soundsDef;
							BaseUI.PlayVoiceEvent((soundsDef11 != null) ? soundsDef11.GetString("war_declared_narrator_voice_line", null, "", true, true, true, '.') : null, null);
							this.WarDeclaredMusicTrigger(param as Logic.Kingdom);
							return;
						}
						return;
					}
				}
				else if (num != 2930094263U)
				{
					if (num != 2941838617U)
					{
						return;
					}
					if (!(message == "eow_lost_support"))
					{
						return;
					}
					if (this.logic != BaseUI.LogicKingdom())
					{
						return;
					}
					Vars vars30 = new Vars();
					vars30.Set<Logic.Kingdom>("kingdom", param as Logic.Kingdom);
					vars30.Set<Logic.Kingdom>("plr_kingdom", this.logic);
					MessageIcon.Create("EmperorOfTheWorldLostSupportMessage", vars30, true, null);
					return;
				}
				else
				{
					if (!(message == "war_start_mission_knights_oposition"))
					{
						return;
					}
					if (BaseUI.LogicKingdom() != this.logic)
					{
						return;
					}
					Vars vars31 = param as Vars;
					MessageIcon.Create("WarStartMissionKnightsOposition", vars31.Copy(), true, null);
					return;
				}
			}
			else if (num <= 2952324180U)
			{
				if (num != 2943757277U)
				{
					if (num != 2952324180U)
					{
						return;
					}
					if (!(message == "rebellion_zone_left"))
					{
						return;
					}
					if (this.logic != BaseUI.LogicKingdom())
					{
						return;
					}
					Vars vars32 = param as Vars;
					Rebellion rebellion5 = vars32.obj.obj_val as Rebellion;
					Vars vars33 = vars32;
					string key5 = "goto_target";
					Logic.Army val6;
					if (rebellion5 == null)
					{
						val6 = null;
					}
					else
					{
						Logic.Rebel leader = rebellion5.leader;
						val6 = ((leader != null) ? leader.army : null);
					}
					vars33.Set<Logic.Army>(key5, val6);
					MessageIcon.Create("RebellionZoneLeftMessage", vars32, true, null);
					return;
				}
				else
				{
					if (!(message == "pretender_not_chosen_emperor_of_the_world"))
					{
						return;
					}
					if (this.logic == BaseUI.LogicKingdom())
					{
						MessageIcon.Create("EmperorOfTheWorldPretenderNotChosenMessage", param as Vars, true, null);
						return;
					}
					Vars vars34 = new Vars();
					vars34.SetVar("result", "claim_refused");
					vars34.SetVar("kingdom", this.logic);
					DT.Field soundsDef12 = BaseUI.soundsDef;
					BaseUI.PlayVoiceEvent((soundsDef12 != null) ? soundsDef12.GetString("narrator_emperor_of_the_world_their_claim_failed", null, "", true, true, true, '.') : null, vars34);
					return;
				}
			}
			else if (num != 3000623632U)
			{
				if (num != 3079482253U)
				{
					return;
				}
				if (!(message == "joined_war"))
				{
					return;
				}
			}
			else
			{
				if (!(message == "start_emperor_of_the_world_vote"))
				{
					return;
				}
				UIEmperorOfTheWorldWindow.ToggleOpen(param as Vars);
				return;
			}
			War war2 = param as War;
			if (war2 != null)
			{
				Logic.Kingdom kingdom25 = BaseUI.LogicKingdom();
				if (this.logic == kingdom25 && (war2.defenders.Contains(this.logic) || war2.attackers.Contains(this.logic)))
				{
					if (war2.IsLeader(kingdom25))
					{
						if (war2.involvementReason == War.InvolvementReason.RejectPretenderEmperorOfTheWorld || war2.involvementReason == War.InvolvementReason.RejectAIEmperorOfTheWorld)
						{
							BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("war_declared_narrator_voice_line", null, true, true, true, '.'), war2);
						}
						else if ((war2.involvementReason == War.InvolvementReason.InheritanceClaimDeclined || war2.involvementReason == War.InvolvementReason.VassalIndependenceClaim || war2.involvementReason == War.InvolvementReason.VassalIndependenceRefuseReconsider) && war2.defender == this.logic)
						{
							BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("war_declared_narrator_voice_line", null, true, true, true, '.'), war2);
						}
					}
					if (war2.involvementReason == War.InvolvementReason.RejectPretenderEmperorOfTheWorld || war2.involvementReason == War.InvolvementReason.RejectAIEmperorOfTheWorld)
					{
						BaseUI.PlaySound(BaseUI.soundsDef.FindChild("war_declared_sound_effect", null, true, true, true, '.'));
					}
					else if (war2.involvementReason == War.InvolvementReason.InheritanceClaimDeclined || war2.involvementReason == War.InvolvementReason.VassalIndependenceClaim || war2.involvementReason == War.InvolvementReason.VassalIndependenceRefuseReconsider)
					{
						BaseUI.PlaySound(BaseUI.soundsDef.FindChild("war_declared_sound_effect", null, true, true, true, '.'));
					}
					Logic.Kingdom enemyLeader = war2.GetEnemyLeader(war2.GetSide(this.logic));
					this.WarDeclaredMusicTrigger(enemyLeader);
				}
			}
			return;
		}
		if (num <= 3715214635U)
		{
			if (num <= 3443666340U)
			{
				if (num <= 3325293173U)
				{
					if (num != 3155753816U)
					{
						if (num != 3211457550U)
						{
							if (num != 3325293173U)
							{
								return;
							}
							if (!(message == "prisoners_gained"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							MessageIcon.Create("PrisonersGainedMessage", param as Vars, true, null);
							return;
						}
						else
						{
							if (!(message == "rel_changed_diplomat"))
							{
								return;
							}
							if (this.logic != BaseUI.LogicKingdom())
							{
								return;
							}
							MessageIcon.Create("RelationsImprovedDiplomat", param as Vars, true, null);
							return;
						}
					}
					else
					{
						if (!(message == "army_entered"))
						{
							return;
						}
						Logic.Army army = param as Logic.Army;
						WorldUI worldUI2 = WorldUI.Get();
						if (worldUI2 == null || worldUI2.kingdom != this.id || !this.logic.IsEnemy(army))
						{
							return;
						}
						global::Kingdom kingdom26 = global::Kingdom.Get(army.kingdom_id);
						global::Realm realm3 = global::Realm.Get(army.realm_in.id);
						if (kingdom26 == null || realm3 == null)
						{
							return;
						}
						Vars vars35 = new Vars(army);
						vars35.Set<Logic.Realm>("realm", realm3.logic);
						vars35.Set<Logic.Kingdom>("kingdom", kingdom26.logic);
						vars35.Set<Logic.Character>("leader", army.leader);
						MessageIcon.Create("EnemyCrossedBorder", vars35, true, null);
						return;
					}
				}
				else if (num <= 3408369052U)
				{
					if (num != 3347338849U)
					{
						if (num != 3408369052U)
						{
							return;
						}
						if (!(message == "inheritance_insufficient_realms"))
						{
							return;
						}
						if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
						{
							MessageIcon.Create("InheritanceInsufficientRealmsMessage", param as Vars, true, null);
							return;
						}
						return;
					}
					else
					{
						if (!(message == "offer_answered"))
						{
							return;
						}
						Offer offer3 = param as Offer;
						Logic.Kingdom kingdom27 = BaseUI.LogicKingdom();
						if (kingdom27 != this.logic)
						{
							return;
						}
						if (offer3.from == kingdom27)
						{
							if (offer3.AI && offer3.answer != "accept")
							{
								return;
							}
							MessageIcon.Create(offer3, "answered", true);
						}
						else if (offer3.to == kingdom27 && !offer3.NeedsAnswer())
						{
							MessageIcon.Create(offer3, "message", true);
						}
						if (offer3.def.outcomes.options.Count > 0 && !offer3.Expired())
						{
							string key6;
							if (offer3.to.GetKingdom().id != kingdom27.id)
							{
								key6 = "diplomacy_" + offer3.answer;
							}
							else
							{
								key6 = "player_" + offer3.answer + "_offer";
							}
							string @string = offer3.def.field.GetString(key6, null, "", true, true, true, '.');
							int num6 = offer3.def.field.GetInt("sfx_priority", null, 0, true, true, true, '.');
							int argsCount = offer3.GetArgsCount();
							for (int m = 0; m < argsCount; m++)
							{
								Offer offer4 = offer3.GetArg(m).obj_val as Offer;
								if (offer4 != null)
								{
									int @int = offer4.def.field.GetInt("sfx_priority", null, 0, true, true, true, '.');
									if (@int > num6)
									{
										@string = offer4.def.field.GetString(key6, null, "", true, true, true, '.');
										num6 = @int;
									}
								}
							}
							BaseUI.PlaySoundEvent(@string, null);
							if (offer3.answer == "accept")
							{
								Logic.Kingdom kingdom28 = offer3.from as Logic.Kingdom;
								if (kingdom28 != null)
								{
									string key7 = "DiplomacyAcceptTrigger";
									Religion religion12 = kingdom28.religion;
									BackgroundMusic.OnTrigger(key7, (religion12 != null) ? religion12.name : null);
								}
							}
						}
						return;
					}
				}
				else if (num != 3418741288U)
				{
					if (num != 3443666340U)
					{
						return;
					}
					if (!(message == "lost_special_court_rebel"))
					{
						return;
					}
					if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
					{
						MessageIcon.Create("LostCourtRebelMessage", param as Vars, true, null);
						return;
					}
					return;
				}
				else
				{
					if (!(message == "war_drawn_no_battles"))
					{
						return;
					}
					goto IL_3584;
				}
			}
			else if (num <= 3625375939U)
			{
				if (num <= 3465720332U)
				{
					if (num != 3448667035U)
					{
						if (num != 3465720332U)
						{
							return;
						}
						if (!(message == "sovereign_set"))
						{
							return;
						}
						goto IL_1686;
					}
					else
					{
						if (!(message == "excommunicated"))
						{
							return;
						}
						Vars vars36 = new Vars(this.logic);
						vars36.Set<Logic.Character>("pope", this.logic.game.religions.catholic.head);
						if (this.logic == BaseUI.LogicKingdom())
						{
							MessageIcon.Create("OwnKingdomExcommunicated", vars36, true, null);
							return;
						}
						MessageIcon.Create("KingdomExcommunicated", vars36, true, null);
						return;
					}
				}
				else if (num != 3520311651U)
				{
					if (num != 3625375939U)
					{
						return;
					}
					if (!(message == "spy_left"))
					{
						return;
					}
					Logic.Character character4 = param as Logic.Character;
					if (character4 == null)
					{
						return;
					}
					Logic.Kingdom kingdom29 = BaseUI.LogicKingdom();
					if (character4.GetKingdom() == kingdom29)
					{
						this.UpdateFoW(true);
					}
					return;
				}
				else
				{
					if (!(message == "friend_helped_our_prisoners_escape"))
					{
						return;
					}
					if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
					{
						Vars vars37 = param as Vars;
						Vars vars38 = new Vars();
						vars38.Set<Value>("owner", vars37.Get("owner", true));
						List<Logic.Character> list5 = vars37.Get<List<Logic.Character>>("prisoners", null);
						List<Logic.Character> list6 = new List<Logic.Character>();
						for (int n = 0; n < list5.Count; n++)
						{
							Logic.Character character5 = list5[n];
							if (character5 != null && this.logic.id == character5.kingdom_id && !list6.Contains(character5))
							{
								list6.Add(character5);
							}
						}
						if (list6.Count == 1)
						{
							vars38.Set<Logic.Character>("prisoner", list6[0]);
						}
						vars38.Set<List<Logic.Character>>("prisoners", list6);
						MessageIcon.Create("FriendHelpedOurPrisonersEscapeMessage", vars38, true, null);
						return;
					}
					return;
				}
			}
			else if (num <= 3709652548U)
			{
				if (num != 3686263358U)
				{
					if (num != 3709652548U)
					{
						return;
					}
					if (!(message == "wife_widowed"))
					{
						return;
					}
					if (BaseUI.LogicKingdom() == this.logic)
					{
						MessageIcon.Create("WidowedMessage", param as Vars, true, null);
					}
					return;
				}
				else
				{
					if (!(message == "authority_capture_game"))
					{
						return;
					}
					if (!this.logic.IsAuthority())
					{
						return;
					}
					string str = param as string;
					GameCapture.Capture(null, str + "_host", null, null, null, null, false);
					return;
				}
			}
			else if (num != 3713823935U)
			{
				if (num != 3715214635U)
				{
					return;
				}
				if (!(message == "end_jihad"))
				{
					return;
				}
				Vars vars39 = param as Vars;
				MessageIcon.CreateJihadEnded(this.logic, vars39.Get<Logic.Kingdom>("ktgt", null), vars39.Get<string>("reason", null));
				return;
			}
			else
			{
				if (!(message == "independence"))
				{
					return;
				}
				if (this.logic == null || this.logic.id != BaseUI.LogicKingdom().id)
				{
					return;
				}
				DT.Field soundsDef13 = BaseUI.soundsDef;
				BaseUI.PlayVoiceEvent((soundsDef13 != null) ? soundsDef13.GetString("independence_claimed_by_us", null, "", true, true, true, '.') : null, null);
				Logic.Kingdom kingdom30 = param as Logic.Kingdom;
				if (kingdom30 != null && this.logic.FindWarWith(kingdom30) != null)
				{
					BaseUI.PlaySound(BaseUI.soundsDef.FindChild("war_declared_sound_effect", null, true, true, true, '.'));
					this.WarDeclaredMusicTrigger(kingdom30);
					return;
				}
				return;
			}
		}
		else if (num <= 4004570535U)
		{
			if (num <= 3886343244U)
			{
				if (num <= 3753147899U)
				{
					if (num != 3729888805U)
					{
						if (num != 3753147899U)
						{
							return;
						}
						if (!(message == "prisoners_changed_kingdom"))
						{
							return;
						}
						if (this.logic != BaseUI.LogicKingdom())
						{
							return;
						}
						MessageIcon.Create("PrisonersChangedKingdomMessage", param as Vars, true, null);
						return;
					}
					else
					{
						if (!(message == "war_start_mission_knights_own"))
						{
							return;
						}
						if (BaseUI.LogicKingdom() != this.logic)
						{
							return;
						}
						Vars vars40 = param as Vars;
						MessageIcon.Create("WarStartMissionKnightsOwn", vars40.Copy(), true, null);
						return;
					}
				}
				else if (num != 3771806931U)
				{
					if (num != 3886343244U)
					{
						return;
					}
					if (!(message == "own_rebellion_reinforced"))
					{
						return;
					}
					if (BaseUI.LogicKingdom() != this.logic)
					{
						return;
					}
					Logic.Rebel rebel2 = param as Logic.Rebel;
					if (rebel2 == null)
					{
						return;
					}
					Rebellion rebellion6 = rebel2.rebellion;
					if (rebellion6 == null)
					{
						return;
					}
					Vars vars41 = new Vars();
					vars41.SetVar("rebellion", rebellion6);
					vars41.Set<Logic.Army>("goto_target", rebel2.army);
					MessageIcon.Create("OurLoyalistRebellionReinforcedMessage", vars41, true, null);
					return;
				}
				else if (!(message == "conceal_pact"))
				{
					return;
				}
			}
			else if (num <= 3930612127U)
			{
				if (num != 3917709380U)
				{
					if (num != 3930612127U)
					{
						return;
					}
					if (!(message == "getting_bankrupt"))
					{
						return;
					}
					if (this.logic != BaseUI.LogicKingdom())
					{
						return;
					}
					MessageIcon.Create("BankrupcyMessage", null, true, null);
					return;
				}
				else
				{
					if (!(message == "new_jihad"))
					{
						return;
					}
					Logic.Kingdom ktgt = param as Logic.Kingdom;
					BackgroundMusic.OnTrigger("JihadTrigger", null);
					MessageIcon.CreateNewJihad(this.logic, ktgt);
					return;
				}
			}
			else if (num != 3939041780U)
			{
				if (num != 4004570535U)
				{
					return;
				}
				if (!(message == "crown_authority_change"))
				{
					return;
				}
				if (BaseUI.LogicKingdom() != this.logic)
				{
					return;
				}
				if (LoadingScreen.IsShown())
				{
					return;
				}
				if ((bool)param)
				{
					DT.Field soundsDef14 = BaseUI.soundsDef;
					BaseUI.PlayVoiceEvent((soundsDef14 != null) ? soundsDef14.GetString("narrator_crown_authority_increased", null, "", true, true, true, '.') : null, null);
					DT.Field soundsDef15 = BaseUI.soundsDef;
					BaseUI.PlaySoundEvent((soundsDef15 != null) ? soundsDef15.GetString("crown_authority_increase", null, "", true, true, true, '.') : null, null);
					return;
				}
				DT.Field soundsDef16 = BaseUI.soundsDef;
				BaseUI.PlayVoiceEvent((soundsDef16 != null) ? soundsDef16.GetString("narrator_crown_authority_dereased", null, "", true, true, true, '.') : null, null);
				DT.Field soundsDef17 = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef17 != null) ? soundsDef17.GetString("crown_authority_decrease", null, "", true, true, true, '.') : null, null);
				return;
			}
			else
			{
				if (!(message == "own_rebellion_started"))
				{
					return;
				}
				if (BaseUI.LogicKingdom() != this.logic)
				{
					return;
				}
				Rebellion rebellion7 = param as Rebellion;
				if (rebellion7 == null)
				{
					return;
				}
				Vars vars42 = new Vars();
				vars42.SetVar("rebellion", rebellion7);
				Vars vars43 = vars42;
				string key8 = "goto_target";
				Logic.Rebel leader2 = rebellion7.leader;
				vars43.Set<Logic.Army>(key8, (leader2 != null) ? leader2.army : null);
				MessageIcon.Create("OwnLoyalistRebellionStartedMessage", vars42, true, null);
				return;
			}
		}
		else if (num <= 4125982160U)
		{
			if (num <= 4026491999U)
			{
				if (num != 4023029508U)
				{
					if (num != 4026491999U)
					{
						return;
					}
					if (!(message == "rebellion_independence_loyalists"))
					{
						return;
					}
					if (this.logic != BaseUI.LogicKingdom())
					{
						return;
					}
					MessageIcon.Create("RebellionIndependenceLoyalistsMessage", param as Vars, true, null);
					return;
				}
				else
				{
					if (!(message == "rebellion_zone_joined"))
					{
						return;
					}
					if (this.logic != BaseUI.LogicKingdom())
					{
						return;
					}
					Vars vars44 = param as Vars;
					Rebellion rebellion8 = vars44.obj.obj_val as Rebellion;
					Vars vars45 = vars44;
					string key9 = "goto_target";
					Logic.Army val7;
					if (rebellion8 == null)
					{
						val7 = null;
					}
					else
					{
						Logic.Rebel leader3 = rebellion8.leader;
						val7 = ((leader3 != null) ? leader3.army : null);
					}
					vars45.Set<Logic.Army>(key9, val7);
					MessageIcon.Create("RebellionZoneJoinedMessage", vars44, true, null);
					return;
				}
			}
			else if (num != 4081519659U)
			{
				if (num != 4125982160U)
				{
					return;
				}
				if (!(message == "rankings_lost_lead"))
				{
					return;
				}
				Logic.Kingdom kingdom31 = BaseUI.LogicKingdom();
				Vars vars46 = param as Vars;
				if (this.logic != kingdom31)
				{
					vars46.Set<bool>("update_timers", false);
					return;
				}
				KingdomRanking kingdomRanking3 = vars46.Get<KingdomRanking>("ranking", null);
				Logic.Kingdom val8 = null;
				for (int num7 = 0; num7 < kingdomRanking3.cur_rows.Count; num7++)
				{
					KingdomRanking.Row row = kingdomRanking3.cur_rows[num7];
					if (row.rank == kingdomRanking3.first_rank)
					{
						val8 = row.kingdom;
						break;
					}
				}
				float num8 = (float)kingdomRanking3.Find(kingdom31).rank / (float)kingdomRanking3.last_rank * 100f;
				vars46.Set<Logic.Kingdom>("top_kingdom", val8);
				vars46.Set<bool>("in_top_perc_X", num8 <= kingdomRanking3.def.in_top_perc_X);
				vars46.Set<bool>("in_bottom_perc_X", num8 >= 100f - kingdomRanking3.def.in_bottom_perc_X);
				vars46.Set<bool>("in_mid_perc_X", num8 < 100f - kingdomRanking3.def.in_bottom_perc_X && num8 > kingdomRanking3.def.in_top_perc_X);
				vars46.Set<string>("ranking_name", "#" + global::Defs.Localize(kingdomRanking3.def.field, "name", null, null, true, true));
				vars46.Set<string>("ranking_description", "#" + global::Defs.Localize(kingdomRanking3.def.field, "description", null, null, true, true));
				MessageIcon.Create("RankingsLostLeadMessage", vars46, true, null);
				return;
			}
			else
			{
				if (!(message == "vassals_changed"))
				{
					return;
				}
				goto IL_1686;
			}
		}
		else if (num <= 4144496145U)
		{
			if (num != 4137619576U)
			{
				if (num != 4144496145U)
				{
					return;
				}
				if (!(message == "spy_entered"))
				{
					return;
				}
				Logic.Character character6 = param as Logic.Character;
				if (character6 == null)
				{
					return;
				}
				Logic.Kingdom kingdom32 = BaseUI.LogicKingdom();
				if (character6.GetKingdom() == kingdom32)
				{
					this.UpdateFoW(false);
				}
				return;
			}
			else
			{
				if (!(message == "unexcommunicated"))
				{
					return;
				}
				Vars vars47 = new Vars(this.logic);
				vars47.Set<Logic.Character>("pope", this.logic.game.religions.catholic.head);
				MessageIcon.Create((this.logic == BaseUI.LogicKingdom()) ? "OwnKingdomUnexcommunicated" : "KingdomUnExcommunicated", vars47, true, null);
				return;
			}
		}
		else if (num != 4192422753U)
		{
			if (num != 4201990322U)
			{
				return;
			}
			if (!(message == "offer_removed"))
			{
				return;
			}
			if (BaseUI.LogicKingdom() != this.logic)
			{
				return;
			}
			Offer offer5 = param as Offer;
			this.RefreshRoyalDungeonUIOnOffer(offer5);
			MessageIcon messageIcon = offer5.msg_icon as MessageIcon;
			if (messageIcon != null)
			{
				messageIcon.Dismiss(true);
			}
			MessageIcon messageIcon2 = offer5.msg_icon_left as MessageIcon;
			if (messageIcon2 != null)
			{
				messageIcon2.Dismiss(true);
			}
			return;
		}
		else
		{
			if (!(message == "add_pact"))
			{
				return;
			}
			goto IL_169C;
		}
		IL_16EB:
		if (!this.logic.started)
		{
			return;
		}
		UIPactIcon.OnPactMessage(this.logic, message, param as Vars);
		return;
		IL_146F:
		Logic.Kingdom kingdom33 = param as Logic.Kingdom;
		Logic.Kingdom kingdom34 = BaseUI.LogicKingdom();
		if (kingdom33 == null)
		{
			return;
		}
		if (ViewMode.Stances.IsActive() || ViewMode.WorldView.IsActive())
		{
			WorldMap worldMap = WorldMap.Get();
			if (worldMap != null)
			{
				worldMap.ReloadView();
			}
		}
		this.RefreshUI();
		Logic.Settlement settlement2;
		if ((settlement2 = (BaseUI.SelLO() as Logic.Settlement)) != null)
		{
			Logic.Kingdom kingdom35 = settlement2.GetKingdom();
			if (kingdom35 == this.logic || kingdom35 == kingdom33)
			{
				global::Settlement settlement3 = settlement2.visuals as global::Settlement;
				if (settlement3 != null)
				{
					settlement3.RefreshSelection();
				}
			}
		}
		if (this.logic == kingdom34)
		{
			if (kingdom33 == kingdom33.game.religions.catholic.hq_kingdom)
			{
				Crusade crusade3 = kingdom33.game.religions.catholic.crusade;
				object obj3;
				if (crusade3 == null)
				{
					obj3 = null;
				}
				else
				{
					Logic.Army army2 = crusade3.army;
					obj3 = ((army2 != null) ? army2.visuals : null);
				}
				global::Army army3 = obj3 as global::Army;
				if (army3 != null)
				{
					army3.ui_pvFigure.UpdateArmy();
				}
			}
			for (int num9 = 0; num9 < kingdom33.armies.Count; num9++)
			{
				global::Army army4 = kingdom33.armies[num9].visuals as global::Army;
				if (!(army4 == null))
				{
					UIPVFigureArmy ui_pvFigure = army4.ui_pvFigure;
					if (ui_pvFigure != null)
					{
						ui_pvFigure.UpdateArmy();
					}
					army4.UpdateSelection();
				}
			}
			return;
		}
		if (kingdom33 == kingdom34)
		{
			if (this.logic == this.logic.game.religions.catholic.hq_kingdom)
			{
				Crusade crusade4 = this.logic.game.religions.catholic.crusade;
				object obj4;
				if (crusade4 == null)
				{
					obj4 = null;
				}
				else
				{
					Logic.Army army5 = crusade4.army;
					obj4 = ((army5 != null) ? army5.visuals : null);
				}
				global::Army army6 = obj4 as global::Army;
				if (army6 != null)
				{
					army6.ui_pvFigure.UpdateArmy();
				}
			}
			for (int num10 = 0; num10 < this.logic.armies.Count; num10++)
			{
				global::Army army7 = this.logic.armies[num10].visuals as global::Army;
				if (!(army7 == null))
				{
					UIPVFigureArmy ui_pvFigure2 = army7.ui_pvFigure;
					if (ui_pvFigure2 != null)
					{
						ui_pvFigure2.UpdateArmy();
					}
					army7.UpdateSelection();
				}
			}
		}
		return;
		IL_1686:
		this.UpdatePVFigurniesAndSettlements();
		WorldMap worldMap2 = WorldMap.Get();
		if (worldMap2 == null)
		{
			return;
		}
		worldMap2.ReloadView();
		return;
		IL_169C:
		if (!this.logic.started)
		{
			return;
		}
		if (ViewMode.Stances.IsActive() || ViewMode.WorldView.IsActive())
		{
			WorldMap worldMap3 = WorldMap.Get();
			if (worldMap3 != null)
			{
				worldMap3.ReloadView();
			}
		}
		this.RefreshUI();
		UIPactIcon.OnPactMessage(this.logic, message, param as Vars);
		return;
		IL_170C:
		this.SyncRealms();
		return;
		IL_3584:
		if (this.logic != null && this.logic.id == BaseUI.LogicKingdom().id)
		{
			BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild(message, null, true, true, true, '.'), null);
			return;
		}
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x00092E60 File Offset: 0x00091060
	public static void PlayHireSound(Logic.Character c)
	{
		if (c == null)
		{
			return;
		}
		if (c.class_def == null)
		{
			return;
		}
		BaseUI.PlayVoiceEvent(c.GetVoiceLine("hire_voice_line"), c);
		BaseUI.PlaySoundEvent(c.GetSoundEffect("hire_sound_effect"), null);
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x00092E94 File Offset: 0x00091094
	public void UpdatePVFigurniesAndSettlements()
	{
		if (this.logic == null)
		{
			return;
		}
		if (this.logic.realms != null)
		{
			for (int i = 0; i < this.logic.realms.Count; i++)
			{
				Logic.Realm realm = this.logic.realms[i];
				if (realm.settlements != null && realm.settlements.Count != 0)
				{
					for (int j = 0; j < realm.settlements.Count; j++)
					{
						Logic.Settlement settlement = realm.settlements[j];
						global::Settlement settlement2 = ((settlement != null) ? settlement.visuals : null) as global::Settlement;
						if (!(settlement2 == null))
						{
							settlement2.UpdatePVFigure();
							settlement2.CreateLabel(true, true);
						}
					}
				}
			}
		}
		if (this.logic.armies != null && this.logic.armies.Count > 0)
		{
			for (int k = 0; k < this.logic.armies.Count; k++)
			{
				global::Army army = this.logic.armies[k].visuals as global::Army;
				if (army != null)
				{
					UIPVFigureArmy ui_pvFigure = army.ui_pvFigure;
					if (ui_pvFigure != null)
					{
						ui_pvFigure.UpdateArmy();
					}
				}
			}
		}
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x00092FBC File Offset: 0x000911BC
	public void UpdateFoW(bool force)
	{
		if (this.realms == null)
		{
			return;
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			worldMap.ReloadView();
		}
		for (int i = 0; i < this.realms.Count; i++)
		{
			this.realms[i].UpdateFow(force, true);
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00093010 File Offset: 0x00091210
	private void SyncName()
	{
		if (this.logic == null)
		{
			return;
		}
		if (this.logic.game == null)
		{
			return;
		}
		LabelUpdater labelUpdater = LabelUpdater.Get(true);
		if (labelUpdater != null)
		{
			labelUpdater.ClearKingdomOldLabels(this, false);
		}
		this.Name = this.logic.Name;
		this.crest_id = this.logic.CoAIndex;
		this.Load(this.logic);
		this.ActiveName = this.logic.ActiveName;
		for (int i = 0; i < this.logic.armies.Count; i++)
		{
			global::Army army = this.logic.armies[i].visuals as global::Army;
			if (!(army == null))
			{
				army.RefreshKingdomColors();
			}
		}
		for (int j = 0; j < this.logic.realms.Count; j++)
		{
			Logic.Realm realm = this.logic.realms[j];
			for (int k = 0; k < realm.settlements.Count; k++)
			{
				global::Settlement settlement = realm.settlements[k].visuals as global::Settlement;
				if (!(settlement == null))
				{
					settlement.UpdateCrest();
					settlement.CreateLabel(true, true);
				}
			}
		}
		LabelUpdater labelUpdater2 = LabelUpdater.Get(true);
		if (labelUpdater2 != null)
		{
			labelUpdater2.RecreateKingdomLabel(this);
		}
		LabelUpdater labelUpdater3 = LabelUpdater.Get(true);
		if (labelUpdater3 != null)
		{
			labelUpdater3.UpdateLabels();
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			worldMap.ReloadView();
		}
		this.UpdatePVFigurniesAndSettlements();
		this.RefreshUI();
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00093188 File Offset: 0x00091388
	private void SyncRealms()
	{
		if (this.realms == null)
		{
			this.realms = new List<global::Realm>(this.logic.realms.Count);
		}
		else
		{
			this.realms.Clear();
		}
		for (int i = 0; i < this.logic.realms.Count; i++)
		{
			global::Realm item = global::Realm.Get(this.logic.realms[i].id);
			this.realms.Add(item);
		}
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x00093208 File Offset: 0x00091408
	public void RefreshUI()
	{
		this.invalidateUI = true;
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x00093211 File Offset: 0x00091411
	private void LateUpdate()
	{
		if (this.invalidateUI)
		{
			this.UpdateUI();
			this.invalidateUI = false;
		}
		if (this.m_InvalidatePlayerChanged)
		{
			this.OnPlayersChanged();
			this.m_InvalidatePlayerChanged = false;
		}
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00093240 File Offset: 0x00091440
	private void UpdateUI()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		if (worldUI.selected_obj != null)
		{
			GameObject selected_obj = worldUI.selected_obj;
			global::Army component = selected_obj.GetComponent<global::Army>();
			if (component != null)
			{
				component.RecreateSelectionUI();
			}
			global::Settlement component2 = selected_obj.GetComponent<global::Settlement>();
			if (component2 != null)
			{
				component2.RecreateSelectionUI();
			}
			global::Battle component3 = selected_obj.GetComponent<global::Battle>();
			if (component3 != null)
			{
				component3.RecreateSelectionUI();
			}
		}
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x000932B2 File Offset: 0x000914B2
	public string GetShieldMaterialType()
	{
		return global::Kingdom.GetShieldMaterialType(this);
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x000932BA File Offset: 0x000914BA
	public static string GetShieldMaterialType(global::Kingdom k)
	{
		if (((k != null) ? k.logic : null) == null)
		{
			return "regular";
		}
		return global::Kingdom.GetShieldMaterialType(k.logic);
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x000932DC File Offset: 0x000914DC
	public static string GetShieldMaterialType(Logic.Kingdom k)
	{
		if (k == null)
		{
			return "regular";
		}
		if (k.IsGreatPower() && k.IsVassal())
		{
			return "vassal_great_power";
		}
		if (k.IsGreatPower())
		{
			return "great_power";
		}
		if (k.IsVassal())
		{
			return "vassal";
		}
		if (k.type == Logic.Kingdom.Type.Faction)
		{
			return "faction";
		}
		if (k.type == Logic.Kingdom.Type.RebelFaction || k.type == Logic.Kingdom.Type.ReligiousFaction || k.type == Logic.Kingdom.Type.LoyalistsFaction)
		{
			return "rebel";
		}
		if (k.type == Logic.Kingdom.Type.Crusade)
		{
			return "crusade";
		}
		return "regular";
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x00093368 File Offset: 0x00091568
	public static int GetShieldFrameIndex(int kid, string shieldMode, bool politicalView = false)
	{
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return 0;
		}
		global::Kingdom kingdom = global::Kingdom.Get(kid);
		if (kingdom == null)
		{
			return 0;
		}
		string shieldMaterialType = global::Kingdom.GetShieldMaterialType(kingdom);
		DT.Field field = defs.dt.Find(string.Concat(new string[]
		{
			"CoatOfArmsModes.",
			shieldMode,
			".category.",
			politicalView ? "pv_" : "",
			shieldMaterialType
		}), null);
		if (field == null)
		{
			return 0;
		}
		return field.Int(null, 0);
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x000933EC File Offset: 0x000915EC
	public static int GetShieldFrameIndex(string shieldMode, string kingdomType, bool politicalView = false)
	{
		global::Defs defs = global::Defs.Get(false);
		if (((defs != null) ? defs.dt : null) == null)
		{
			return 0;
		}
		DT.Field field = defs.dt.Find(string.Concat(new string[]
		{
			"CoatOfArmsModes.",
			shieldMode,
			".category.",
			politicalView ? "pv_" : "",
			kingdomType
		}), null);
		if (field == null)
		{
			return 0;
		}
		return field.Int(null, 0);
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00093460 File Offset: 0x00091660
	public Bounds CalcBounds()
	{
		Bounds result = default(Bounds);
		bool flag = true;
		for (int i = 1; i <= global::Realm.Count(); i++)
		{
			global::Realm realm = global::Realm.Get(i);
			if (realm != null && realm.kingdom == this.id && !(realm.bounds.size == Vector3.zero))
			{
				if (flag)
				{
					result = realm.bounds;
				}
				else
				{
					result.Encapsulate(realm.bounds);
				}
				flag = false;
			}
		}
		return result;
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x000934D8 File Offset: 0x000916D8
	private Bounds CalcRegionBounds(global::Realm realm, List<int> processed_realms)
	{
		Bounds bounds = realm.bounds;
		processed_realms.Add(realm.id);
		for (int i = 0; i < realm.Neighbors.Count; i++)
		{
			global::Realm realm2 = global::Realm.Get(realm.Neighbors[i].rid);
			if (realm2 != null && realm2.kingdom == this.id && !(realm2.bounds.size == Vector3.zero) && !processed_realms.Contains(realm2.id))
			{
				bounds.Encapsulate(this.CalcRegionBounds(realm2, processed_realms));
			}
		}
		return bounds;
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x00093570 File Offset: 0x00091770
	private void RemoveBoundsOverlap(List<Bounds> bounds)
	{
		for (int i = 0; i < bounds.Count; i++)
		{
			for (int j = i + 1; j < bounds.Count; j++)
			{
				if (bounds[i].Intersects(bounds[j]))
				{
					float num = Mathf.Max(0f, Mathf.Min(bounds[i].max.x, bounds[j].max.x) - Mathf.Max(bounds[i].min.x, bounds[j].min.x));
					float num2 = Mathf.Max(0f, Mathf.Min(bounds[i].max.z, bounds[j].max.z) - Mathf.Max(bounds[i].min.z, bounds[j].min.z));
					Bounds value = bounds[i];
					Bounds value2 = bounds[j];
					if (num < num2)
					{
						if (value.center.x < value2.center.x)
						{
							value.SetMinMax(value.min, new Vector3(value.max.x - num / 2f, 0f, value.max.z));
							value2.SetMinMax(new Vector3(value2.min.x + num / 2f, 0f, value2.min.z), value2.max);
						}
						else
						{
							value2.SetMinMax(value2.min, new Vector3(value2.max.x - num / 2f, 0f, value2.max.z));
							value.SetMinMax(new Vector3(value.min.x + num / 2f, 0f, value.min.z), value.max);
						}
					}
					else if (value.center.z < value2.center.z)
					{
						value.SetMinMax(value.min, new Vector3(value.max.x, 0f, value.max.z - num2 / 2f));
						value2.SetMinMax(new Vector3(value2.min.x, 0f, value2.min.z + num2 / 2f), value2.max);
					}
					else
					{
						value2.SetMinMax(value2.min, new Vector3(value2.max.x - num / 2f, 0f, value2.max.z));
						value.SetMinMax(new Vector3(value.min.x + num / 2f, 0f, value.min.z), value.max);
					}
					bounds[i] = value;
					bounds[j] = value2;
				}
			}
		}
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x000938C4 File Offset: 0x00091AC4
	public List<Bounds> CalcRegionsBounds()
	{
		List<Bounds> list = new List<Bounds>();
		List<int> list2 = new List<int>();
		for (int i = 1; i <= global::Realm.Count(); i++)
		{
			global::Realm realm = global::Realm.Get(i);
			if (realm != null && realm.kingdom == this.id && !(realm.bounds.size == Vector3.zero) && !list2.Contains(realm.id))
			{
				list.Add(this.CalcRegionBounds(realm, list2));
			}
		}
		this.RemoveBoundsOverlap(list);
		return list;
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x00093948 File Offset: 0x00091B48
	public static string GetKingdomRelationsKey(Logic.Kingdom Kingdom1, Logic.Kingdom Kingdom2)
	{
		float relationship = Kingdom1.GetRelationship(Kingdom2);
		if (relationship < -500f)
		{
			return "Kingdom.Relation.hostile";
		}
		if (relationship < -250f)
		{
			return "Kingdom.Relation.negative";
		}
		if (relationship < -99f)
		{
			return "Kingdom.Relation.reserved";
		}
		if (relationship < 99f)
		{
			return "Kingdom.Relation.neutral";
		}
		if (relationship < 250f)
		{
			return "Kingdom.Relation.sympathetic";
		}
		if (relationship < 500f)
		{
			return "Kingdom.Relation.trusting";
		}
		return "Kingdom.Relation.friendly";
	}

	// Token: 0x040009FA RID: 2554
	public string Name = "";

	// Token: 0x040009FB RID: 2555
	public string ActiveName = "";

	// Token: 0x040009FC RID: 2556
	public int id;

	// Token: 0x040009FD RID: 2557
	public int MapColorIndex;

	// Token: 0x040009FE RID: 2558
	public int PrimaryArmyColorIndex;

	// Token: 0x040009FF RID: 2559
	public int SecondaryArmyColorIndex;

	// Token: 0x04000A00 RID: 2560
	public Color MapColor = Color.black;

	// Token: 0x04000A01 RID: 2561
	public Color PrimaryArmyColor = Color.blue;

	// Token: 0x04000A02 RID: 2562
	public Color SecondaryArmyColor = Color.red;

	// Token: 0x04000A03 RID: 2563
	public Color unitColor = Color.white;

	// Token: 0x04000A04 RID: 2564
	public int unitColorID = -1;

	// Token: 0x04000A05 RID: 2565
	public int crest_id;

	// Token: 0x04000A06 RID: 2566
	public List<global::Realm> realms;

	// Token: 0x04000A07 RID: 2567
	public List<string> unit_sets;

	// Token: 0x04000A08 RID: 2568
	public GameObject Label;

	// Token: 0x04000A09 RID: 2569
	public Logic.Kingdom logic;

	// Token: 0x04000A0B RID: 2571
	private bool m_InvalidatePlayerChanged;

	// Token: 0x02000627 RID: 1575
	[Serializable]
	public struct ID
	{
		// Token: 0x06004705 RID: 18181 RVA: 0x00212340 File Offset: 0x00210540
		public ID(int id)
		{
			this.id = id;
		}

		// Token: 0x06004706 RID: 18182 RVA: 0x00212349 File Offset: 0x00210549
		public static implicit operator global::Kingdom.ID(int id)
		{
			return new global::Kingdom.ID(id);
		}

		// Token: 0x06004707 RID: 18183 RVA: 0x00212351 File Offset: 0x00210551
		public static implicit operator int(global::Kingdom.ID id)
		{
			return id.id;
		}

		// Token: 0x06004708 RID: 18184 RVA: 0x0021235C File Offset: 0x0021055C
		public override string ToString()
		{
			string text = this.id.ToString();
			global::Kingdom kingdom = global::Kingdom.Get(this.id);
			if (kingdom != null)
			{
				text = text + "(" + kingdom.Name + ")";
			}
			return text;
		}

		// Token: 0x04003458 RID: 13400
		public int id;
	}
}
