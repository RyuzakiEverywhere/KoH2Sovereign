using System;
using System.Collections.Generic;
using DeltaDNA;
using Logic;
using THQGDS;
using THQGDSEvent;
using THQGDSEventParser;
using THQGDSManager;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class Analytics : IListener
{
	// Token: 0x17000022 RID: 34
	// (get) Token: 0x0600025F RID: 607 RVA: 0x00022B40 File Offset: 0x00020D40
	public static Analytics instance
	{
		get
		{
			Game game = GameLogic.Get(false);
			return ((game != null) ? game.analytics : null) as Analytics;
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000260 RID: 608 RVA: 0x00022B59 File Offset: 0x00020D59
	// (set) Token: 0x06000261 RID: 609 RVA: 0x00022B61 File Offset: 0x00020D61
	public bool sdk_started { get; private set; }

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000262 RID: 610 RVA: 0x00022B6A File Offset: 0x00020D6A
	// (set) Token: 0x06000263 RID: 611 RVA: 0x00022B72 File Offset: 0x00020D72
	public int num_sdk_events { get; private set; }

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000264 RID: 612 RVA: 0x00022B7B File Offset: 0x00020D7B
	// (set) Token: 0x06000265 RID: 613 RVA: 0x00022B83 File Offset: 0x00020D83
	public int num_visual_clue_events { get; private set; }

	// Token: 0x06000266 RID: 614 RVA: 0x00022B8C File Offset: 0x00020D8C
	public static void Init(Game game)
	{
		new Analytics(game);
	}

	// Token: 0x06000267 RID: 615 RVA: 0x00022B95 File Offset: 0x00020D95
	private Analytics(Game game)
	{
		this.game = game;
		if (game != null)
		{
			game.analytics = this;
		}
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00022BD8 File Offset: 0x00020DD8
	private void StartSDK()
	{
		if (this.sdk_started)
		{
			return;
		}
		this.sdk_started = true;
		if (global::Defs.GetBool("Analytics", "use_ddna", null, false))
		{
			Analytics.EnableDDNA(true);
		}
		if (global::Defs.GetBool("Analytics", "use_thqgds", null, false))
		{
			Analytics.EnableTHQGDS(true);
		}
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00022C27 File Offset: 0x00020E27
	public static void EnableDDNA(bool enable)
	{
		if (enable == Analytics.ddna_enabled)
		{
			return;
		}
		Analytics.ddna_enabled = enable;
		if (enable)
		{
			Singleton<DDNA>.Instance.Settings.SendGameRunningEveryMinute = false;
			Singleton<DDNA>.Instance.StartSDK();
			return;
		}
		Singleton<DDNA>.Instance.StopSDK();
	}

	// Token: 0x0600026A RID: 618 RVA: 0x00022C60 File Offset: 0x00020E60
	public static void EnableTHQGDS(bool enable)
	{
		if (enable == Analytics.thqgds_enabled)
		{
			return;
		}
		Analytics.thqgds_enabled = enable;
		if (enable)
		{
			Analytics.CollectTHQNOGDSEventsInfo();
			string text = Title.BranchName();
			if (text == null)
			{
				text = "unknown";
			}
			else if (text == "")
			{
				text = "LIVE";
			}
			THQGDSWrapper.THQGDSInit("koh2", "02", "gds-koh2-pc.thqonline.net", text);
			EventParser.Init();
			return;
		}
		THQGDSWrapper.THQGDSCleanup();
	}

	// Token: 0x0600026B RID: 619 RVA: 0x00022CC9 File Offset: 0x00020EC9
	public static void OnDefsLoaded()
	{
		Analytics instance = Analytics.instance;
		if (instance == null)
		{
			return;
		}
		instance.LoadDefs();
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00022CDA File Offset: 0x00020EDA
	public static void OnAppQuit()
	{
		Analytics.EnableDDNA(false);
		Analytics.EnableTHQGDS(false);
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00022CE8 File Offset: 0x00020EE8
	public static void OnUpdate()
	{
		if (Analytics.thqgds_enabled)
		{
			using (Game.Profile("THQGDSDoFrame", false, 1f, null))
			{
				THQGDSWrapper.THQGDSDoFrame();
			}
		}
	}

	// Token: 0x0600026E RID: 622 RVA: 0x00022D34 File Offset: 0x00020F34
	private static Analytics.THQGDS_EventInfo GetTHQNOGDSEventInfo(string event_id)
	{
		for (int i = 0; i < Analytics.thqgds_events_info.Count; i++)
		{
			Analytics.THQGDS_EventInfo thqgds_EventInfo = Analytics.thqgds_events_info[i];
			if (thqgds_EventInfo.event_id == event_id)
			{
				return thqgds_EventInfo;
			}
		}
		Analytics.THQGDS_EventInfo thqgds_EventInfo2 = new Analytics.THQGDS_EventInfo
		{
			event_id = event_id
		};
		Analytics.thqgds_events_info.Add(thqgds_EventInfo2);
		return thqgds_EventInfo2;
	}

	// Token: 0x0600026F RID: 623 RVA: 0x00022D8C File Offset: 0x00020F8C
	private static void CollectTHQNOGDSEventsInfo()
	{
		Analytics.thqgds_events_info.Clear();
		DT.Field defField = global::Defs.GetDefField("Analytics", null);
		bool flag;
		if (defField == null)
		{
			flag = (null != null);
		}
		else
		{
			DT.Def def = defField.def;
			flag = (((def != null) ? def.defs : null) != null);
		}
		if (!flag)
		{
			return;
		}
		Analytics.CollectTHQNOGDSParamsInfo(Analytics.GetTHQNOGDSEventInfo("common_parameters"), defField.FindChild("common_parameters", null, true, true, true, '.'));
		for (int i = 0; i < defField.def.defs.Count; i++)
		{
			Analytics.CollectEventInfo(defField.def.defs[i].field);
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00022E20 File Offset: 0x00021020
	private static void CollectEventInfo(DT.Field def_field)
	{
		string @string = def_field.GetString("event_id", null, "", true, true, true, '.');
		if (string.IsNullOrEmpty(@string))
		{
			return;
		}
		Analytics.CollectTHQNOGDSParamsInfo(Analytics.GetTHQNOGDSEventInfo(@string), def_field.FindChild("args", null, true, true, true, '.'));
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00022E6C File Offset: 0x0002106C
	private static void CollectTHQNOGDSParamsInfo(Analytics.THQGDS_EventInfo ei, DT.Field argsf)
	{
		if (((argsf != null) ? argsf.children : null) == null)
		{
			return;
		}
		for (int i = 0; i < argsf.children.Count; i++)
		{
			DT.Field field = argsf.children[i];
			if (!string.IsNullOrEmpty(field.key))
			{
				string type = field.Type();
				ei.AddParam(type, field.key);
			}
		}
	}

	// Token: 0x06000272 RID: 626 RVA: 0x00022ECC File Offset: 0x000210CC
	private void LoadDefs()
	{
		this.root_dt_def = null;
		this.defs.Clear();
		this.listeners = null;
		this.num_sdk_events = 0;
		this.num_visual_clue_events = 0;
		Game game = this.game;
		DT.Def def;
		if (game == null)
		{
			def = null;
		}
		else
		{
			DT dt = game.dt;
			def = ((dt != null) ? dt.FindDef("Analytics") : null);
		}
		this.root_dt_def = def;
		if (this.root_dt_def == null)
		{
			return;
		}
		if (this.root_dt_def.defs == null)
		{
			Analytics.Warning("No defs found");
			return;
		}
		this.AddTargetTypes();
		for (int i = 0; i < this.root_dt_def.defs.Count; i++)
		{
			DT.Def def2 = this.root_dt_def.defs[i];
			if (def2.field.GetBool("enabled", null, true, true, true, true, '.'))
			{
				Analytics.Def def3 = new Analytics.Def();
				if (def3.Load(this, def2))
				{
					if (!def3.simulated && !string.IsNullOrEmpty(def3.event_id))
					{
						int num = this.num_sdk_events + 1;
						this.num_sdk_events = num;
					}
					if (def3.visual_clue > 0)
					{
						int num = this.num_visual_clue_events + 1;
						this.num_visual_clue_events = num;
					}
					this.defs.Add(def3);
				}
			}
		}
		if (this.num_visual_clue_events > 0)
		{
			AnalyticsVisualClues.Init();
		}
		if (this.num_sdk_events > 0)
		{
			this.StartSDK();
		}
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00023013 File Offset: 0x00021213
	private void AddTargetTypes()
	{
		if (TargetType.Find("ui_element") == null)
		{
			TargetType.Add("ui_element", typeof(GameObject), null, Array.Empty<string>());
		}
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0002303C File Offset: 0x0002123C
	private void AddListener(Analytics.Def def, Trigger.Def tdef)
	{
		if (tdef.messages == null)
		{
			return;
		}
		if (this.listeners == null)
		{
			this.listeners = new Dictionary<string, List<Analytics.Listener>>();
		}
		Analytics.Listener item = new Analytics.Listener
		{
			def = def,
			tdef = tdef
		};
		for (int i = 0; i < tdef.messages.Count; i++)
		{
			string key = tdef.messages[i];
			List<Analytics.Listener> list;
			if (!this.listeners.TryGetValue(key, out list))
			{
				list = new List<Analytics.Listener>();
				this.listeners.Add(key, list);
			}
			list.Add(item);
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x000230D0 File Offset: 0x000212D0
	public void OnMessage(object obj, string message, object param)
	{
		if (!UserSettings.DataCollection)
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null || !game.send_analytics || game.real_time_total.seconds < 0f)
		{
			return;
		}
		if (this.listeners == null)
		{
			return;
		}
		using (Game.Profile("Analytics.OnMessage", false, 10f, null))
		{
			List<Analytics.Listener> list;
			if (this.listeners.TryGetValue(message, out list))
			{
				try
				{
					for (int i = 0; i < list.Count; i++)
					{
						Analytics.Listener listener = list[i];
						this.ProcessTrigger(listener, obj, message, param);
					}
				}
				catch (Exception arg)
				{
					Analytics.Error(string.Format("Error in OnMessage({0}.{1}): ", obj, message) + arg);
				}
			}
		}
	}

	// Token: 0x06000276 RID: 630 RVA: 0x000231A8 File Offset: 0x000213A8
	private string GetClientType()
	{
		string text;
		if (this.game.multiplayer == null || !this.game.multiplayer.IsOnline())
		{
			text = "SinglePlayer";
		}
		else if (this.game.multiplayer.type == Logic.Multiplayer.Type.Client)
		{
			text = "Client";
		}
		else
		{
			text = "Server";
		}
		if (Application.isEditor)
		{
			text = "Editor" + text;
		}
		return text;
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00023210 File Offset: 0x00021410
	private void ProcessTrigger(Analytics.Listener listener, object sender, string message, object param)
	{
		if (listener.def.log >= 2)
		{
			Analytics.Log(string.Format("Processing {0}: {1}.{2}({3})", new object[]
			{
				listener,
				sender,
				message,
				param
			}));
		}
		using (Game.Profile("Analytics.ProcessTrigger", false, 10f, null))
		{
			this.tmp_vars.obj = new Value(listener.def);
			if (listener.tdef.sender_type != null)
			{
				object obj = listener.tdef.sender_type.Resolve(sender);
				if (obj == null)
				{
					return;
				}
				this.tmp_vars.Set<object>("target", obj);
			}
			else
			{
				this.tmp_vars.Set<Value>("target", Value.Null);
			}
			this.tmp_trigger.Set(listener.tdef, sender, message, param, 0, this.tmp_vars);
			if (!this.tmp_trigger.Validate(this.game, this.tmp_vars))
			{
				this.tmp_trigger.Clear();
			}
			else
			{
				listener.def.activations++;
				if (listener.def.CheckCooldown())
				{
					listener.def.last_activation_time = UnityEngine.Time.realtimeSinceStartup;
					if (listener.def.visual_clue > 0)
					{
						if (listener.def.log > 0)
						{
							Analytics.Log(string.Format("Visual clue {0} ({1})", listener.def.visual_clue, listener.def.id));
						}
						AnalyticsVisualClues.Show(listener.def.visual_clue);
					}
					if (!string.IsNullOrEmpty(listener.def.event_id))
					{
						Vars vars = new Vars();
						Vars vars2 = new Vars();
						if (listener.def.args != null)
						{
							this.tmp_vars.Set<Trigger>("trigger", this.tmp_trigger);
							for (int i = 0; i < listener.def.args.Count; i++)
							{
								DT.Field field = listener.def.args[i];
								string text = Analytics.DefTypeToAPIType(field);
								if (text != null)
								{
									string text2 = Analytics.ParamPrefix(text) + field.key;
									Value val = field.Value(this.tmp_vars, true, true);
									if (val.is_unknown && this.tmp_trigger.named_vars != null)
									{
										val = this.tmp_trigger.named_vars.Get(text2, true);
									}
									if (val.is_unknown)
									{
										DT.Field field2 = field.FindChild("optional", null, true, true, true, '.');
										if (field2 == null || !field2.Bool(null, false))
										{
											Analytics.Warning(string.Format("Could not resolve {0}.{1}", listener.def, text2));
										}
									}
									else
									{
										vars.Set<Value>(text2, val);
									}
								}
							}
						}
						this.AddCommonParams(vars2);
						Analytics.SendEvent(listener.def.event_id, vars, vars2, listener.def.log > 0, listener.def.simulated);
					}
				}
			}
		}
	}

	// Token: 0x06000278 RID: 632 RVA: 0x00023534 File Offset: 0x00021734
	public void OnSaveDeleted(string name, string action)
	{
		if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(action))
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<string>("gameName", name);
		vars.Set<string>("gameAction", action);
		Game game = this.game;
		if (((game != null) ? game.GetLocalPlayerKingdom() : null) != null)
		{
			vars.Set<string>("menuLocation", "in_game");
		}
		else
		{
			vars.Set<string>("menuLocation", "title_screen");
		}
		this.game.NotifyListeners("analytics_save_deleted", vars);
	}

	// Token: 0x06000279 RID: 633 RVA: 0x000235B8 File Offset: 0x000217B8
	public void OnDecisionTaken(MessageIcon.Type type, IVars input_vars, DT.Field def_field, float expire_time, string outcome)
	{
		UnityEngine.Object x = BaseUI.Get();
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (x == null || kingdom == null || def_field == null)
		{
			return;
		}
		Vars vars = new Vars();
		Logic.Kingdom kingdom2 = null;
		Offer offer = ((input_vars != null) ? input_vars.GetVar("offer", null, true).Object(true) : null) as Offer;
		if (offer != null)
		{
			Logic.Kingdom kingdom3 = offer.from.GetKingdom();
			Logic.Kingdom kingdom4 = offer.to.GetKingdom();
			if (kingdom3 == kingdom && kingdom4 != null)
			{
				kingdom2 = kingdom4;
			}
			else if (kingdom4 == kingdom && kingdom3 != null)
			{
				kingdom2 = kingdom3;
			}
		}
		else
		{
			Logic.Kingdom kingdom5 = ((input_vars != null) ? input_vars.GetVar("src_kingdom", null, true).Object(true) : null) as Logic.Kingdom;
			Logic.Kingdom kingdom6 = ((input_vars != null) ? input_vars.GetVar("tgt_kingdom", null, true).Object(true) : null) as Logic.Kingdom;
			if (kingdom5 == kingdom)
			{
				kingdom2 = kingdom6;
			}
			else if (kingdom6 == kingdom)
			{
				kingdom2 = kingdom5;
			}
		}
		if (kingdom2 != null)
		{
			vars.Set<string>("targetKingdom", kingdom2.Name);
			KingdomAndKingdomRelation kingdomAndKingdomRelation = KingdomAndKingdomRelation.Get(kingdom, kingdom2, true, false);
			if (kingdomAndKingdomRelation != null)
			{
				vars.Set<int>("kingdomRelation", (int)kingdomAndKingdomRelation.GetRelationship());
			}
		}
		string @string = def_field.GetString("category", null, "undefined", true, true, true, '.');
		vars.Set<string>("decisionCategory", @string);
		vars.Set<string>("decisionType", type.ToString());
		string val;
		if (offer != null)
		{
			Offer.Def def = offer.def;
			if (((def != null) ? def.id : null) != null)
			{
				val = offer.def.id;
				goto IL_179;
			}
		}
		val = def_field.key;
		IL_179:
		vars.Set<string>("decisionName", val);
		vars.Set<string>("outcome", outcome);
		if (expire_time > 0f)
		{
			vars.Set<int>("timeToRespond", (int)expire_time);
		}
		else if (offer != null && offer.def != null && offer.def.timeout > 0f)
		{
			vars.Set<int>("timeToRespond", (int)offer.def.timeout);
		}
		kingdom.NotifyListeners("analytics_decision_message", vars);
	}

	// Token: 0x0600027A RID: 634 RVA: 0x000237B4 File Offset: 0x000219B4
	private string GetMultiplayerType()
	{
		Game game = this.game;
		if (((game != null) ? game.multiplayer : null) == null)
		{
			return "None";
		}
		if (!this.game.IsMultiplayer())
		{
			return "Single";
		}
		if (this.game.IsAuthority())
		{
			return "Host";
		}
		return "Client";
	}

	// Token: 0x0600027B RID: 635 RVA: 0x00023808 File Offset: 0x00021A08
	private void AddCommonParams(Vars args)
	{
		Analytics.AddStringParam(args, "branchName", Title.BranchName());
		Game game = this.game;
		Logic.Kingdom kingdom = (game != null) ? game.GetLocalPlayerKingdom() : null;
		if (kingdom == null)
		{
			return;
		}
		string name = "gameID";
		Campaign campaign = this.game.campaign;
		Analytics.AddStringParam(args, name, (campaign != null) ? campaign.id : null);
		Analytics.AddStringParam(args, "multiplayerType", this.GetMultiplayerType());
		Analytics.AddIntParam(args, "campaignTime", (int)this.game.session_time.seconds);
		Analytics.AddFloatParam(args, "currentGameSpeed", this.game.GetSpeed());
		Analytics.AddIntParam(args, "realTimeInCampaign", (int)this.game.real_time_total.seconds);
		Analytics.AddIntParam(args, "realPlayers", Logic.Multiplayer.CurrentPlayers.Count());
		Analytics.AddIntParam(args, "aiPlayers", this.game.GetNumAliveKingdoms() - Logic.Multiplayer.CurrentPlayers.Count());
		Resource resources = kingdom.resources;
		Resource income = kingdom.income;
		Resource expenses = kingdom.expenses;
		Incomes incomes = kingdom.incomes;
		if (resources != null && income != null && expenses != null && incomes != null)
		{
			Analytics.AddIntParam(args, "goldBalance", (int)resources[ResourceType.Gold]);
			Analytics.AddIntParam(args, "booksBalance", (int)resources[ResourceType.Books]);
			Analytics.AddIntParam(args, "religionBalance", (int)resources[ResourceType.Piety]);
			Analytics.AddIntParam(args, "commerceBalance", (int)resources[ResourceType.Trade]);
			Analytics.AddIntParam(args, "foodBalance", (int)resources[ResourceType.Food]);
			Analytics.AddIntParam(args, "leviesBalance", (int)resources[ResourceType.Levy]);
			Analytics.AddIntParam(args, "goldIncome", (int)income[ResourceType.Gold]);
			Analytics.AddIntParam(args, "booksIncome", (int)income[ResourceType.Books]);
			Analytics.AddIntParam(args, "bookCap", (int)kingdom.GetStat(Stats.ks_max_books, true));
			Analytics.AddIntParam(args, "religionIncome", (int)income[ResourceType.Piety]);
			Analytics.AddIntParam(args, "religionCap", (int)kingdom.GetStat(Stats.ks_max_piety, true));
			Analytics.AddIntParam(args, "commerceIncome", (int)income[ResourceType.Trade]);
			Analytics.AddIntParam(args, "foodIncome", (int)income[ResourceType.Food]);
			Analytics.AddIntParam(args, "levyIncome", (int)income[ResourceType.Levy]);
			Analytics.AddIntParam(args, "levyCap", (int)kingdom.GetStat(Stats.ks_max_levy, true));
			Analytics.AddIntParam(args, "taxRate", (int)kingdom.GetTaxRate());
			Analytics.AddIntParam(args, "baseIncome", (int)kingdom.GetStat(Stats.ks_base_gold_income, true));
			Analytics.AddIntParam(args, "provincesIncome", incomes.per_resource[1].GetVar("BASE", null, true));
			Analytics.AddIntParam(args, "tradeIncome", (int)(kingdom.goldFromPassiveTrade + kingdom.goldFromMerchants + kingdom.goldFromRoyalMerchants + kingdom.goldFromForeignMerchants));
			Analytics.AddIntParam(args, "vassalTributes", (int)kingdom.GetVassalGold());
			Analytics.AddFloatParam(args, "incomeModifiers", incomes.per_resource[1].value.perc_value);
			Analytics.AddIntParam(args, "goldExpenditure", (int)expenses[ResourceType.Gold]);
			Analytics.AddIntParam(args, "militaryExpense", (int)(kingdom.armies_upkeep[ResourceType.Gold] + kingdom.wageGoldForMarshals + kingdom.upkeepJihad));
			Analytics.AddIntParam(args, "economyExpense", (int)(kingdom.upkeepBuildings + kingdom.upkeepGoldFromGoodsImport + kingdom.upkeepGoldFromFoodImport));
			Analytics.AddIntParam(args, "tributeExpense", (int)kingdom.taxForSovereign);
			Analytics.AddIntParam(args, "disorderExpense", (int)kingdom.upkeepDisorder);
			Analytics.AddIntParam(args, "inflationExpense", (int)kingdom.inflation);
			Analytics.AddIntParam(args, "foodConsumption", (int)kingdom.expenses[ResourceType.Food]);
			Analytics.AddIntParam(args, "commerceConsumption", (int)kingdom.expenses[ResourceType.Trade]);
		}
		if (kingdom.opinions != null)
		{
			Analytics.AddIntParam(args, "nobilityOpinion", (int)kingdom.opinions.GetVar("NobilityOpinion", null, true).float_val);
			Analytics.AddIntParam(args, "armyOpinion", (int)kingdom.opinions.GetVar("ArmyOpinion", null, true).float_val);
			Analytics.AddIntParam(args, "merchantOpinion", (int)kingdom.opinions.GetVar("MerchantsOpinion", null, true).float_val);
			Analytics.AddIntParam(args, "clergyOpinion", (int)kingdom.opinions.GetVar("ClergyOpinion", null, true).float_val);
			Analytics.AddIntParam(args, "peasantryOpinion", (int)kingdom.opinions.GetVar("PeasantryOpinion", null, true).float_val);
		}
		Analytics.AddIntParam(args, "rebellionStatus", kingdom.rebellions.Count);
		Analytics.AddStringParam(args, "kingdomName", kingdom.Name);
		Analytics.AddIntParam(args, "provinces", kingdom.realms.Count);
		Analytics.AddIntParam(args, "towns", kingdom.realms.Count);
		int num = 0;
		for (int i = 0; i < kingdom.realms.Count; i++)
		{
			num += kingdom.realms[i].GetSettlementCount("All", "realm", false);
		}
		Analytics.AddIntParam(args, "settlements", num);
		Analytics.AddIntParam(args, "population", kingdom.GetTotalPopulation());
		string name2 = "stability";
		KingdomStability stability = kingdom.stability;
		Analytics.AddIntParam(args, name2, (int)((stability != null) ? stability.value : 0f));
		Analytics.AddIntParam(args, "militaryPower", (int)kingdom.armyStrength);
		Analytics.AddIntParam(args, "siegeDefense", (int)kingdom.GetStat(Stats.ks_siege_defense, true));
		Analytics.AddIntParam(args, "fame", (int)kingdom.fame);
		Analytics.AddIntParam(args, "goodsProduced", kingdom.goods_produced.Count);
		Analytics.AddIntParam(args, "goodsImported", kingdom.goods_imported.Count);
		string name3 = "totalAdvantages";
		KingdomAdvantages advantages = kingdom.GetAdvantages(true);
		Analytics.AddIntParam(args, name3, (advantages != null) ? advantages.NumActiveAdvantages() : 0);
		Analytics.AddIntParam(args, "totalTraditions", kingdom.NumTraditions(Tradition.Type.All));
		Logic.Character king = kingdom.GetKing();
		if (king != null)
		{
			Analytics.AddStringParam(args, "monarchName", king.Name);
			Analytics.AddIntParam(args, "monarchAge", (int)king.age);
			Analytics.AddBoolParam(args, "Married", king.IsMarried());
		}
		Logic.RoyalFamily royalFamily = kingdom.royalFamily;
		if (royalFamily != null)
		{
			Analytics.AddIntParam(args, "Sons", royalFamily.NumPrinces());
			Analytics.AddIntParam(args, "Daughters", royalFamily.NumPrincesses());
		}
		Analytics.AddIntParam(args, "Marshals", kingdom.NumCourtMembersOfClass("Marshal"));
		Analytics.AddIntParam(args, "Merchants", kingdom.NumCourtMembersOfClass("Merchant"));
		Analytics.AddIntParam(args, "Diplomats", kingdom.NumCourtMembersOfClass("Diplomat"));
		Analytics.AddIntParam(args, "Spies", kingdom.NumCourtMembersOfClass("Spy"));
		Analytics.AddIntParam(args, "Clerics", kingdom.NumCourtMembersOfClass("Cleric"));
		Analytics.AddIntParam(args, "totalCourtLevels", (int)kingdom.SumCourtLevels());
		string name4 = "crownAuthority";
		Logic.CrownAuthority crownAuthority = kingdom.GetCrownAuthority();
		Analytics.AddIntParam(args, name4, (crownAuthority != null) ? crownAuthority.GetValue() : 0);
		Analytics.AddIntParam(args, "culturalPower", (int)kingdom.GetStat(Stats.ks_culture, true));
		if (kingdom.religion != null)
		{
			string name5 = "religionName";
			Religion.Def def = kingdom.religion.def;
			Analytics.AddStringParam(args, name5, ((def != null) ? def.id : null) ?? "");
			Religion religion = kingdom.religion;
			Logic.Character character = (religion != null) ? religion.head : null;
			string value;
			if (character == null)
			{
				value = "non-existent";
			}
			else
			{
				Analytics.AddStringParam(args, "religionLeaderName", kingdom.religion.head.Name);
				value = (character.IsAlive() ? "alive" : "dead");
			}
			Analytics.AddStringParam(args, "religionLeaderStatus", value);
			Crusade crusade = kingdom.game.religions.catholic.crusade;
			Analytics.AddStringParam(args, "crusadeStatus", (crusade == null) ? "non-existent" : string.Format("Crusade against {0}, led by {1}, helping {2}", crusade.target, crusade.leader, crusade.helping_kingdom));
			Analytics.AddIntParam(args, "jihadsCount", kingdom.game.religions.jihad_kingdoms.Count);
		}
		Analytics.AddIntParam(args, "activeWarsCount", kingdom.wars.Count);
		Analytics.activeEnemies.Clear();
		for (int j = 0; j < kingdom.wars.Count; j++)
		{
			List<Logic.Kingdom> enemies = kingdom.wars[j].GetEnemies(kingdom);
			for (int k = 0; k < enemies.Count; k++)
			{
				Analytics.activeEnemies.Add(enemies[k].id);
			}
		}
		Analytics.AddIntParam(args, "activeEnemiesCount", Analytics.activeEnemies.Count);
		int num2 = 0;
		int num3 = 0;
		for (int l = 0; l < kingdom.pacts.Count; l++)
		{
			if (kingdom.pacts[l].type == Pact.Type.Offensive)
			{
				num3++;
			}
			else
			{
				num2++;
			}
		}
		Analytics.AddIntParam(args, "defensivePactsCount", num2);
		Analytics.AddIntParam(args, "offensivePactsCount", num3);
	}

	// Token: 0x0600027C RID: 636 RVA: 0x00024103 File Offset: 0x00022303
	private static void AddIntParam(Vars args, string name, int value)
	{
		args.Set<int>(Analytics.ParamPrefix("INTEGER") + name, value);
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0002411C File Offset: 0x0002231C
	private static void AddFloatParam(Vars args, string name, float value)
	{
		args.Set<float>(Analytics.ParamPrefix("FLOAT") + name, value);
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00024135 File Offset: 0x00022335
	private static void AddBoolParam(Vars args, string name, bool value)
	{
		args.Set<bool>(Analytics.ParamPrefix("BOOLEAN") + name, value);
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0002414E File Offset: 0x0002234E
	private static void AddStringParam(Vars args, string name, string value)
	{
		args.Set<string>(Analytics.ParamPrefix("STRING") + name, value);
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00024168 File Offset: 0x00022368
	private static string APITypeToDefType(string api_type)
	{
		if (api_type == "STRING")
		{
			return "string";
		}
		if (api_type == "INTEGER")
		{
			return "int";
		}
		if (api_type == "FLOAT")
		{
			return "float";
		}
		if (api_type == "BOOLEAN")
		{
			return "bool";
		}
		if (!(api_type == "OBJECT"))
		{
			Debug.LogWarning("Unknown API type: " + api_type);
			return api_type;
		}
		return api_type;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x000241E4 File Offset: 0x000223E4
	private static string DefTypeToAPIType(DT.Field field)
	{
		string text = (field != null) ? field.Type() : null;
		if (text == "string")
		{
			return "STRING";
		}
		if (text == "int")
		{
			return "INTEGER";
		}
		if (text == "float")
		{
			return "FLOAT";
		}
		if (text == "bool")
		{
			return "BOOLEAN";
		}
		if (!(text == "OBJECT"))
		{
			Debug.LogError(((field != null) ? field.Path(true, false, '.') : null) + ": Unknown parameter type: " + text);
			return null;
		}
		return "OBJECT";
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00024280 File Offset: 0x00022480
	public static string EventPrefix()
	{
		return "aae_";
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00024288 File Offset: 0x00022488
	public static string ParamPrefix(string type)
	{
		string str = "aa";
		if (!string.IsNullOrEmpty(type))
		{
			str += char.ToLower(type[0]).ToString();
		}
		return str + "_";
	}

	// Token: 0x06000284 RID: 644 RVA: 0x000242CB File Offset: 0x000224CB
	public static GDSEvent CreateGDSEvent(string event_name)
	{
		return GDS.CreateEvent(event_name);
	}

	// Token: 0x06000285 RID: 645 RVA: 0x000242D4 File Offset: 0x000224D4
	private static bool SendTHQGDSEvent(string event_name, int common_seq_id, Vars args)
	{
		Analytics.THQGDS_EventInfo thqnogdseventInfo = Analytics.GetTHQNOGDSEventInfo(event_name);
		if (thqnogdseventInfo == null)
		{
			Analytics.Error("Unknown THQGDS event: '" + event_name + "'");
			return false;
		}
		GDSEvent evt = Analytics.CreateGDSEvent(event_name);
		GDS.AddIntParam(evt, "common_seq_id", common_seq_id);
		for (int i = 0; i < thqnogdseventInfo.params_info.Count; i++)
		{
			Analytics.THQGDS_ParamInfo thqgds_ParamInfo = thqnogdseventInfo.params_info[i];
			Value value = args.Get(thqgds_ParamInfo.var_name, true);
			string type = thqgds_ParamInfo.type;
			if (!(type == "bool"))
			{
				if (!(type == "int"))
				{
					if (!(type == "float"))
					{
						if (!(type == "string"))
						{
							Analytics.Error(string.Concat(new string[]
							{
								"Unknown parameter type: '",
								thqgds_ParamInfo.type,
								"' ",
								event_name,
								".",
								thqgds_ParamInfo.name
							}));
							return false;
						}
						GDS.AddStringParam(evt, thqgds_ParamInfo.name, value.String(null));
					}
					else
					{
						GDS.AddFloatParam(evt, thqgds_ParamInfo.name, value.Float(0f));
					}
				}
				else
				{
					GDS.AddIntParam(evt, thqgds_ParamInfo.name, value.Int(0));
				}
			}
			else
			{
				GDS.AddBoolParam(evt, thqgds_ParamInfo.name, value.Bool());
			}
		}
		using (Game.Profile("THQGDS.SendEvent", false, 1f, null))
		{
			GDS.SendEvent(evt);
		}
		return true;
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00024468 File Offset: 0x00022668
	private static void Log(string msg)
	{
		Debug.Log("Analytics: " + msg);
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0002447A File Offset: 0x0002267A
	private static void Warning(string msg)
	{
		Debug.LogWarning("Analytics: " + msg);
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0002448C File Offset: 0x0002268C
	private static void Error(string msg)
	{
		Debug.LogError("Analytics: " + msg);
	}

	// Token: 0x06000289 RID: 649 RVA: 0x000244A0 File Offset: 0x000226A0
	public static bool SendEvent(string event_name, Vars args = null, Vars common_args = null, bool log = false, bool simulated = false)
	{
		bool result;
		using (Game.Profile("Analytics.SendEvent", false, 10f, null))
		{
			if (!Analytics.ddna_enabled && !Analytics.thqgds_enabled)
			{
				simulated = true;
			}
			if (log)
			{
				string args_dump = "";
				if (args != null && !args.Empty())
				{
					args.EnumerateAll(delegate(string key, Value val)
					{
						args_dump += string.Format("\n  {0}: {1}", key, val);
					});
				}
				if (common_args != null && !common_args.Empty())
				{
					args_dump += "\n-------------------";
					common_args.EnumerateAll(delegate(string key, Value val)
					{
						args_dump += string.Format("\n  {0}: {1}", key, val);
					});
				}
				if (simulated)
				{
					Analytics.Log("Simulated '" + event_name + "'" + args_dump);
				}
				else
				{
					Analytics.Log("Sending '" + event_name + "'" + args_dump);
				}
			}
			if (simulated)
			{
				result = true;
			}
			else
			{
				if (Analytics.ddna_enabled)
				{
					GameEvent ddna_evt = new GameEvent(Analytics.EventPrefix() + event_name);
					if (args != null && !args.Empty())
					{
						args.EnumerateAll(delegate(string key, Value val)
						{
							ddna_evt.AddParam(key, val.Object(true));
						});
					}
					if (common_args != null && !common_args.Empty())
					{
						common_args.EnumerateAll(delegate(string key, Value val)
						{
							ddna_evt.AddParam(key, val.Object(true));
						});
					}
					using (Game.Profile("DDNA.SendEvent", false, 1f, null))
					{
						Singleton<DDNA>.Instance.RecordEvent<GameEvent>(ddna_evt);
					}
				}
				if (Analytics.thqgds_enabled)
				{
					Analytics.thqgds_last_common_seq_id++;
					Analytics.SendTHQGDSEvent("common_parameters", Analytics.thqgds_last_common_seq_id, common_args);
					Analytics.SendTHQGDSEvent(event_name, Analytics.thqgds_last_common_seq_id, args);
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0002468C File Offset: 0x0002288C
	public static void OnSaveGame(Game game, string name)
	{
		if (game != null)
		{
			Logic.Kingdom localPlayerKingdom = game.GetLocalPlayerKingdom();
			if (localPlayerKingdom == null)
			{
				return;
			}
			localPlayerKingdom.NotifyListeners("analytics_save_game", name);
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x000246A8 File Offset: 0x000228A8
	public static void OnTutorialMessage(Game game, Tutorial.Topic topic, DT.Def message_def)
	{
		Vars vars = new Vars();
		vars.Set<string>("id", topic.id);
		vars.Set<string>("cur_message", topic.cur_message);
		vars.Set<float>("index", (float)topic.FindMessageIndex(message_def.path));
		Vars vars2 = vars;
		string key = "base";
		string text;
		if (message_def == null)
		{
			text = null;
		}
		else
		{
			DT.Field field = message_def.field;
			text = ((field != null) ? field.base_path : null);
		}
		vars2.Set<string>(key, text ?? "base");
		if (game != null)
		{
			Logic.Kingdom localPlayerKingdom = game.GetLocalPlayerKingdom();
			if (localPlayerKingdom == null)
			{
				return;
			}
			localPlayerKingdom.NotifyListeners("analytics_show_tutorial_message", vars);
		}
	}

	// Token: 0x040003A9 RID: 937
	public Game game;

	// Token: 0x040003AA RID: 938
	public DT.Def root_dt_def;

	// Token: 0x040003AB RID: 939
	public List<Analytics.Def> defs = new List<Analytics.Def>();

	// Token: 0x040003AC RID: 940
	public Dictionary<string, List<Analytics.Listener>> listeners;

	// Token: 0x040003B0 RID: 944
	private static bool ddna_enabled = false;

	// Token: 0x040003B1 RID: 945
	private static bool thqgds_enabled = false;

	// Token: 0x040003B2 RID: 946
	private static List<Analytics.THQGDS_EventInfo> thqgds_events_info = new List<Analytics.THQGDS_EventInfo>();

	// Token: 0x040003B3 RID: 947
	private static int thqgds_last_common_seq_id = 0;

	// Token: 0x040003B4 RID: 948
	private Trigger tmp_trigger = new Trigger(null, null, null, null, 0, null);

	// Token: 0x040003B5 RID: 949
	private Vars tmp_vars = new Vars();

	// Token: 0x040003B6 RID: 950
	private static HashSet<int> activeEnemies = new HashSet<int>();

	// Token: 0x0200051A RID: 1306
	public class Def : IVars
	{
		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x060042BA RID: 17082 RVA: 0x001F99CA File Offset: 0x001F7BCA
		public string id
		{
			get
			{
				return this.dt_def.path;
			}
		}

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x060042BB RID: 17083 RVA: 0x001F99D7 File Offset: 0x001F7BD7
		public DT.Field field
		{
			get
			{
				return this.dt_def.field;
			}
		}

		// Token: 0x060042BC RID: 17084 RVA: 0x001F99E4 File Offset: 0x001F7BE4
		public bool Load(Analytics analytics, DT.Def dt_def)
		{
			this.dt_def = dt_def;
			DT.Field field = this.field;
			this.log = field.GetInt("log", null, 0, true, true, true, '.');
			this.simulated = field.GetBool("simulated", null, false, true, true, true, '.');
			this.event_id = field.GetString("event_id", null, "", true, true, true, '.');
			this.cooldown = field.GetInt("cooldown", null, 0, true, true, true, '.');
			this.visual_clue = field.GetInt("visual_clue", null, 0, true, true, true, '.');
			if (string.IsNullOrEmpty(this.event_id) && this.visual_clue <= 0)
			{
				Analytics.Warning(this.id + " has no 'event_id' and no 'visual_clue'");
			}
			DT.Field field2 = field.FindChild("triggers", null, true, true, true, '.');
			List<DT.Field> list = (field2 != null) ? field2.Children() : null;
			if (list != null)
			{
				this.triggers = new List<Trigger.Def>(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					DT.Field field3 = list[i];
					if (!string.IsNullOrEmpty(field3.key))
					{
						Trigger.Def def = Trigger.Def.Load(field3);
						if (def != null)
						{
							this.triggers.Add(def);
							if (!this.simulated || this.log > 0)
							{
								analytics.AddListener(this, def);
							}
						}
					}
				}
			}
			else
			{
				Analytics.Warning(this.id + " has no triggers");
				this.triggers = null;
			}
			DT.Field field4 = field.FindChild("args", null, true, true, true, '.');
			List<DT.Field> list2 = (field4 != null) ? field4.Children() : null;
			if (list2 != null)
			{
				this.args = new List<DT.Field>(list2.Count);
				for (int j = 0; j < list2.Count; j++)
				{
					DT.Field field5 = list2[j];
					if (!string.IsNullOrEmpty(field5.key) && Analytics.DefTypeToAPIType(field5) != null)
					{
						this.args.Add(field5);
					}
				}
			}
			else
			{
				this.args = null;
			}
			return true;
		}

		// Token: 0x060042BD RID: 17085 RVA: 0x001F9BD4 File Offset: 0x001F7DD4
		public bool CheckCooldown()
		{
			float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
			return this.last_activation_time < 0f || (this.cooldown >= 0 && realtimeSinceStartup - this.last_activation_time >= (float)this.cooldown);
		}

		// Token: 0x060042BE RID: 17086 RVA: 0x001F9C18 File Offset: 0x001F7E18
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			Value value = this.field.GetValue(key, vars, true, true, true, '.');
			if (!value.is_unknown)
			{
				return value;
			}
			return Value.Unknown;
		}

		// Token: 0x04002EF3 RID: 12019
		public DT.Def dt_def;

		// Token: 0x04002EF4 RID: 12020
		public int log;

		// Token: 0x04002EF5 RID: 12021
		public bool simulated;

		// Token: 0x04002EF6 RID: 12022
		public string event_id;

		// Token: 0x04002EF7 RID: 12023
		public int cooldown;

		// Token: 0x04002EF8 RID: 12024
		public List<Trigger.Def> triggers;

		// Token: 0x04002EF9 RID: 12025
		public List<DT.Field> args;

		// Token: 0x04002EFA RID: 12026
		public int visual_clue;

		// Token: 0x04002EFB RID: 12027
		public float last_activation_time = -1f;

		// Token: 0x04002EFC RID: 12028
		public int activations;
	}

	// Token: 0x0200051B RID: 1307
	public struct Listener
	{
		// Token: 0x060042C0 RID: 17088 RVA: 0x001F9C5B File Offset: 0x001F7E5B
		public override string ToString()
		{
			return this.def.id + "." + this.tdef.name;
		}

		// Token: 0x04002EFD RID: 12029
		public Analytics.Def def;

		// Token: 0x04002EFE RID: 12030
		public Trigger.Def tdef;
	}

	// Token: 0x0200051C RID: 1308
	public struct THQGDS_ParamInfo
	{
		// Token: 0x04002EFF RID: 12031
		public string type;

		// Token: 0x04002F00 RID: 12032
		public string name;

		// Token: 0x04002F01 RID: 12033
		public string var_name;
	}

	// Token: 0x0200051D RID: 1309
	public class THQGDS_EventInfo
	{
		// Token: 0x060042C1 RID: 17089 RVA: 0x001F9C80 File Offset: 0x001F7E80
		public void AddParam(string type, string name)
		{
			for (int i = 0; i < this.params_info.Count; i++)
			{
				Analytics.THQGDS_ParamInfo thqgds_ParamInfo = this.params_info[i];
				if (thqgds_ParamInfo.name == name)
				{
					if (thqgds_ParamInfo.type != type)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Event ",
							this.event_id,
							" parameter ",
							name,
							" used as ",
							type,
							" and as ",
							thqgds_ParamInfo.type,
							"!"
						}));
					}
					return;
				}
			}
			char c;
			if (!(type == "bool"))
			{
				if (!(type == "int"))
				{
					if (!(type == "float"))
					{
						if (!(type == "string"))
						{
							Analytics.Error(string.Concat(new string[]
							{
								"Unknown parameter type: '",
								type,
								"' ",
								this.event_id,
								".",
								name
							}));
							return;
						}
						c = 's';
					}
					else
					{
						c = 'f';
					}
				}
				else
				{
					c = 'i';
				}
			}
			else
			{
				c = 'b';
			}
			Analytics.THQGDS_ParamInfo item = new Analytics.THQGDS_ParamInfo
			{
				type = type,
				name = name,
				var_name = string.Format("aa{0}_{1}", c, name)
			};
			this.params_info.Add(item);
		}

		// Token: 0x04002F02 RID: 12034
		public string event_id;

		// Token: 0x04002F03 RID: 12035
		public List<Analytics.THQGDS_ParamInfo> params_info = new List<Analytics.THQGDS_ParamInfo>();
	}
}
