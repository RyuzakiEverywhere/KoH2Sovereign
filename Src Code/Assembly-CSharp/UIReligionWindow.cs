using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200021C RID: 540
public class UIReligionWindow : UIWindow, IListener
{
	// Token: 0x060020A8 RID: 8360 RVA: 0x0012A0D3 File Offset: 0x001282D3
	public override string GetDefId()
	{
		return UIReligionWindow.def_id;
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x0012A0DC File Offset: 0x001282DC
	protected override void OnDestroy()
	{
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		Game game = GameLogic.Get(false);
		if (((game != null) ? game.religions : null) != null)
		{
			game.religions.DelListener(this);
		}
		base.OnDestroy();
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x0012A124 File Offset: 0x00128324
	private void Start()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x0012A138 File Offset: 0x00128338
	public void SetData(Logic.Kingdom k)
	{
		this.Init();
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		this.Kingdom = k;
		if (this.Kingdom != null)
		{
			this.Kingdom.AddListener(this);
		}
		if (this.m_KingdomReligionIcon)
		{
			this.m_KingdomReligionIcon.SetData(k);
		}
		if (this.m_KingdomIcon != null)
		{
			this.m_KingdomIcon.SetObject(k, null);
		}
		UIReligionWindow.RestorePapacy restorePapacy = this.m_RestorePapacy;
		if (restorePapacy != null)
		{
			restorePapacy.SetData(this.Kingdom);
		}
		UIIncomePanel incomesPiety = this.m_IncomesPiety;
		if (incomesPiety != null)
		{
			incomesPiety.SetObject(this.Kingdom.incomes[ResourceType.Piety]);
		}
		UIIncomePanel incomesBooks = this.m_IncomesBooks;
		if (incomesBooks != null)
		{
			incomesBooks.SetObject(this.Kingdom.incomes[ResourceType.Books]);
		}
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "ReligionWidnow.caption", this.Kingdom, null);
		}
		for (int i = 0; i < this.availabaleStates.Count; i++)
		{
			UIReligionWindow.ReligionSubState religionSubState = this.availabaleStates[i];
			if (!(religionSubState == null))
			{
				religionSubState.SetData(this.Kingdom);
			}
		}
		this.Refresh();
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x0012A26C File Offset: 0x0012846C
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleClose);
		}
		this.BuildSubstates();
		if (this.m_RestorePapacyContainer != null)
		{
			this.m_RestorePapacy = this.m_RestorePapacyContainer.AddComponent<UIReligionWindow.RestorePapacy>();
		}
		if (GameLogic.Get(true).religions != null)
		{
			GameLogic.Get(true).religions.AddListener(this);
		}
		this.m_Initialized = true;
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x0012A2F8 File Offset: 0x001284F8
	private void BuildSubstates()
	{
		foreach (KeyValuePair<string, Type> keyValuePair in this.states)
		{
			string key = keyValuePair.Key;
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, key, true, true);
			if (gameObject != null)
			{
				UnityEngine.Component component = gameObject.AddComponent(keyValuePair.Value);
				if (component != null && component is UIReligionWindow.ReligionSubState)
				{
					this.availabaleStates.Add(component as UIReligionWindow.ReligionSubState);
				}
			}
		}
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x0012A398 File Offset: 0x00128598
	private void BuildChangeReligionActions()
	{
		if (this.m_ChanageReligionActionsContainer != null && this.Kingdom.actions != null)
		{
			UICommon.DeleteChildren(this.m_ChanageReligionActionsContainer.transform);
			if (this.Kingdom.actions == null)
			{
				return;
			}
			for (int i = 0; i < this.Kingdom.actions.Count; i++)
			{
				Action action = this.Kingdom.actions[i];
				if (action is ChangeReligionAction)
				{
					string text = action.Validate(false);
					if (text == "ok" || (!string.IsNullOrEmpty(text) && text[0] == '_'))
					{
						Vars vars = new Vars(this.Kingdom);
						vars.Set<string>("variant", "change_religion");
						ObjectIcon.GetIcon(action, vars, this.m_ChanageReligionActionsContainer.transform as RectTransform);
					}
				}
			}
		}
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x0012A488 File Offset: 0x00128688
	private void CheckReligionAction()
	{
		if (this.m_CurrentReligionAction == null)
		{
			return;
		}
		if (this.m_CurentReligonAction != null && this.m_CurentReligonAction.state == Action.State.Inactive)
		{
			this.m_CurentReligonAction = null;
		}
		for (int i = 0; i < this.Kingdom.actions.Count; i++)
		{
			Action action = this.Kingdom.actions[i];
			if (action is ChangeReligionAction && action.state != Action.State.Inactive && action != this.m_CurentReligonAction)
			{
				this.m_CurentReligonAction = action;
				this.m_CurrentReligionAction.SetData(action);
			}
		}
		bool flag = this.m_CurentReligonAction != null;
		this.m_CurrentReligionAction.gameObject.SetActive(flag);
		if (this.m_ChanageReligionActionsContainer != null)
		{
			this.m_ChanageReligionActionsContainer.gameObject.SetActive(!flag);
		}
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x0012A558 File Offset: 0x00128758
	private void Refresh()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		this.PopulateIncomes();
		this.ChooseSubState();
		this.BuildChangeReligionActions();
		this.PopulateCultureAndReleigionLabels();
		if (this.m_PietyLabel != null)
		{
			UIText.SetTextKey(this.m_PietyLabel, this.Kingdom.religion.name + ".piety", null, null);
		}
		if (this.m_Illustration != null)
		{
			this.m_Illustration.overrideSprite = global::Defs.GetObj<Sprite>(this.Kingdom.religion.def.field, global::Religions.GetRelgionIllustrationbKey(this.Kingdom), null);
		}
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x0012A601 File Offset: 0x00128801
	protected override void Update()
	{
		base.Update();
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
		if (this.m_InvalidateIncomes)
		{
			this.PopulateIncomes();
			this.m_InvalidateIncomes = false;
		}
		this.CheckReligionAction();
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x0012A639 File Offset: 0x00128839
	private void PopulateIncomes()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		UIIncomePanel incomesPiety = this.m_IncomesPiety;
		if (incomesPiety != null)
		{
			incomesPiety.Refresh();
		}
		UIIncomePanel incomesBooks = this.m_IncomesBooks;
		if (incomesBooks == null)
		{
			return;
		}
		incomesBooks.Refresh();
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x0012A668 File Offset: 0x00128868
	private void PopulateCultureAndReleigionLabels()
	{
		if (this.m_ReligionLabel)
		{
			UIText.SetTextKey(this.m_ReligionLabel, "ReligionWidnow.religion_label", this.Kingdom, null);
		}
		if (this.m_ReligionValue)
		{
			string relgionNameKey = global::Religions.GetRelgionNameKey(this.Kingdom);
			if (!string.IsNullOrEmpty(relgionNameKey))
			{
				UIText.SetTextKey(this.m_ReligionValue, relgionNameKey, this.Kingdom, null);
			}
			else
			{
				UIText.SetText(this.m_ReligionValue, global::Defs.Localize(this.Kingdom.religion.def.field, "name", this.Kingdom.religion, null, true, true));
			}
		}
		if (this.m_CultureLabel)
		{
			UIText.SetTextKey(this.m_CultureLabel, "ReligionWidnow.culture_label", this.Kingdom, null);
		}
		if (this.m_CultureValue)
		{
			UIText.SetTextKey(this.m_CultureValue, "ReligionWidnow.culture", this.Kingdom, null);
		}
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x0012A750 File Offset: 0x00128950
	private void ChooseSubState()
	{
		for (int i = 0; i < this.availabaleStates.Count; i++)
		{
			UIReligionWindow.ReligionSubState religionSubState = this.availabaleStates[i];
			if (!(religionSubState == null))
			{
				bool flag = religionSubState.Validate();
				religionSubState.Active(flag);
				if (flag)
				{
					religionSubState.Build();
				}
			}
		}
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleClose(BSGButton b)
	{
		this.Close(false);
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x0012A7A0 File Offset: 0x001289A0
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 2105032289U)
		{
			if (num <= 466651415U)
			{
				if (num <= 133440116U)
				{
					if (num != 2922875U)
					{
						if (num != 133440116U)
						{
							return;
						}
						if (!(message == "caliphate_claimed"))
						{
							return;
						}
					}
					else if (!(message == "promote_pagan_belief_started"))
					{
						return;
					}
				}
				else if (num != 380521037U)
				{
					if (num != 466651415U)
					{
						return;
					}
					if (!(message == "new_pope_chosen"))
					{
						return;
					}
				}
				else if (!(message == "papacy_destroyed"))
				{
					return;
				}
			}
			else if (num <= 1101300521U)
			{
				if (num != 708058303U)
				{
					if (num != 1101300521U)
					{
						return;
					}
					if (!(message == "income_changed"))
					{
						return;
					}
					this.m_InvalidateIncomes = true;
					return;
				}
				else if (!(message == "crusade_ended"))
				{
					return;
				}
			}
			else if (num != 1319443009U)
			{
				if (num != 2105032289U)
				{
					return;
				}
				if (!(message == "religion_changed"))
				{
					return;
				}
			}
			else if (!(message == "new_crusade"))
			{
				return;
			}
		}
		else if (num <= 2599339670U)
		{
			if (num <= 2186266987U)
			{
				if (num != 2185301385U)
				{
					if (num != 2186266987U)
					{
						return;
					}
					if (!(message == "subordinated"))
					{
						return;
					}
				}
				else if (!(message == "caliphate_abandoned"))
				{
					return;
				}
			}
			else if (num != 2462250079U)
			{
				if (num != 2599339670U)
				{
					return;
				}
				if (!(message == "autocephaly"))
				{
					return;
				}
			}
			else if (!(message == "new_pope_created"))
			{
				return;
			}
		}
		else if (num <= 3435649623U)
		{
			if (num != 2650101716U)
			{
				if (num != 3435649623U)
				{
					return;
				}
				if (!(message == "choose_patriarch"))
				{
					return;
				}
			}
			else if (!(message == "papacy_restored"))
			{
				return;
			}
		}
		else if (num != 3448667035U)
		{
			if (num != 3920287649U)
			{
				if (num != 4137619576U)
				{
					return;
				}
				if (!(message == "unexcommunicated"))
				{
					return;
				}
			}
			else if (!(message == "new_patriarch_chosen"))
			{
				return;
			}
		}
		else if (!(message == "excommunicated"))
		{
			return;
		}
		this.m_Invalidate = true;
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x0012A9B2 File Offset: 0x00128BB2
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIReligionWindow.def_id, null);
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x0012A9BF File Offset: 0x00128BBF
	public static bool IsActive()
	{
		return UIReligionWindow.current != null;
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x0012A9CC File Offset: 0x00128BCC
	public override void Close(bool silent = false)
	{
		UIReligionWindow.current = null;
		base.Close(silent);
	}

	// Token: 0x060020BA RID: 8378 RVA: 0x0012A9DC File Offset: 0x00128BDC
	public static void ToggleOpen(Logic.Kingdom kingdom)
	{
		if (kingdom == null)
		{
			if (UIReligionWindow.current != null)
			{
				UIReligionWindow.current.Close(false);
			}
			return;
		}
		if (UIReligionWindow.current != null)
		{
			UIReligionWindow uireligionWindow = UIReligionWindow.current;
			if (((uireligionWindow != null) ? uireligionWindow.Kingdom : null) == kingdom)
			{
				UIReligionWindow.current.Close(false);
				return;
			}
			UIReligionWindow.current.SetData(kingdom);
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIReligionWindow.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIReligionWindow));
				UIReligionWindow.current = UIReligionWindow.Create(kingdom, prefab, gameObject.transform as RectTransform);
			}
			return;
		}
	}

	// Token: 0x060020BB RID: 8379 RVA: 0x0012AAA8 File Offset: 0x00128CA8
	public static UIReligionWindow Create(Logic.Kingdom kingdom, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (kingdom == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
		UIReligionWindow uireligionWindow = gameObject.GetComponent<UIReligionWindow>();
		if (uireligionWindow == null)
		{
			uireligionWindow = gameObject.AddComponent<UIReligionWindow>();
		}
		uireligionWindow.SetData(kingdom);
		uireligionWindow.Open();
		return uireligionWindow;
	}

	// Token: 0x040015C1 RID: 5569
	private static string def_id = "ReligionWidnow";

	// Token: 0x040015C2 RID: 5570
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040015C3 RID: 5571
	[UIFieldTarget("id_ReligionLabel")]
	private TextMeshProUGUI m_ReligionLabel;

	// Token: 0x040015C4 RID: 5572
	[UIFieldTarget("id_ReligionValue")]
	private TextMeshProUGUI m_ReligionValue;

	// Token: 0x040015C5 RID: 5573
	[UIFieldTarget("id_CultureLabel")]
	private TextMeshProUGUI m_CultureLabel;

	// Token: 0x040015C6 RID: 5574
	[UIFieldTarget("id_CultureValue")]
	private TextMeshProUGUI m_CultureValue;

	// Token: 0x040015C7 RID: 5575
	[UIFieldTarget("id_KingdomIcon")]
	private UIKingdomIcon m_KingdomIcon;

	// Token: 0x040015C8 RID: 5576
	[UIFieldTarget("id_KingdomReligionIcon")]
	private UIReligion m_KingdomReligionIcon;

	// Token: 0x040015C9 RID: 5577
	[UIFieldTarget("id_Close_Button")]
	private BSGButton m_Close;

	// Token: 0x040015CA RID: 5578
	[UIFieldTarget("id_ChanageReligionActionsContainer")]
	private GameObject m_ChanageReligionActionsContainer;

	// Token: 0x040015CB RID: 5579
	[UIFieldTarget("id_CurrentReligionAction")]
	private UIActionProgressInfo m_CurrentReligionAction;

	// Token: 0x040015CC RID: 5580
	[UIFieldTarget("id_PietyLabel")]
	private TextMeshProUGUI m_PietyLabel;

	// Token: 0x040015CD RID: 5581
	[UIFieldTarget("id_Illustration")]
	private Image m_Illustration;

	// Token: 0x040015CE RID: 5582
	[UIFieldTarget("id_IncomesPiety")]
	private UIIncomePanel m_IncomesPiety;

	// Token: 0x040015CF RID: 5583
	[UIFieldTarget("id_IncomesBooks")]
	private UIIncomePanel m_IncomesBooks;

	// Token: 0x040015D0 RID: 5584
	[UIFieldTarget("id_RestorePapacyContainer")]
	private GameObject m_RestorePapacyContainer;

	// Token: 0x040015D1 RID: 5585
	private Logic.Kingdom Kingdom;

	// Token: 0x040015D2 RID: 5586
	private List<UIReligionWindow.ReligionSubState> availabaleStates = new List<UIReligionWindow.ReligionSubState>();

	// Token: 0x040015D3 RID: 5587
	private UIReligionWindow.RestorePapacy m_RestorePapacy;

	// Token: 0x040015D4 RID: 5588
	private bool m_Invalidate;

	// Token: 0x040015D5 RID: 5589
	private bool m_InvalidateIncomes;

	// Token: 0x040015D6 RID: 5590
	private Action m_CurentReligonAction;

	// Token: 0x040015D7 RID: 5591
	private static UIReligionWindow current;

	// Token: 0x040015D8 RID: 5592
	private Dictionary<string, Type> states = new Dictionary<string, Type>
	{
		{
			"id_Catholic",
			typeof(UIReligionWindow.Catholic)
		},
		{
			"id_CatholicExcommunicated",
			typeof(UIReligionWindow.CatholicExcommunicated)
		},
		{
			"id_CatholicNoPope",
			typeof(UIReligionWindow.CatholicNoPope)
		},
		{
			"id_CatholicOwningRome",
			typeof(UIReligionWindow.CatholicOwningRome)
		},
		{
			"id_OrthodoxEcumenical",
			typeof(UIReligionWindow.OrthodoxEcumenical)
		},
		{
			"id_OrthodoxIndependent",
			typeof(UIReligionWindow.OrthodoxIndependent)
		},
		{
			"id_OrthodoxSubordinated",
			typeof(UIReligionWindow.OrthodoxSubordinated)
		},
		{
			"id_IslamCaliphate",
			typeof(UIReligionWindow.IslamCaliphate)
		},
		{
			"id_IslamNoCaliphate",
			typeof(UIReligionWindow.IslamNoCaliphate)
		},
		{
			"id_Pagan",
			typeof(UIReligionWindow.Pagan)
		}
	};

	// Token: 0x02000759 RID: 1881
	internal abstract class ReligionSubState : MonoBehaviour
	{
		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06004B03 RID: 19203
		protected abstract string id { get; }

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06004B04 RID: 19204
		protected abstract string descriptionKey { get; }

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x06004B05 RID: 19205 RVA: 0x002231C9 File Offset: 0x002213C9
		// (set) Token: 0x06004B06 RID: 19206 RVA: 0x002231D1 File Offset: 0x002213D1
		private protected Logic.Kingdom Data { protected get; private set; }

		// Token: 0x06004B07 RID: 19207
		public abstract bool Validate();

		// Token: 0x06004B08 RID: 19208 RVA: 0x002231DA File Offset: 0x002213DA
		public virtual void SetData(Logic.Kingdom k)
		{
			this.Data = k;
			this.Build();
		}

		// Token: 0x06004B09 RID: 19209 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void Update()
		{
		}

		// Token: 0x06004B0A RID: 19210 RVA: 0x002231E9 File Offset: 0x002213E9
		public virtual void Build()
		{
			UICommon.FindComponents(this, false);
			this.BuildDescription();
			this.BuildReligionEffects();
		}

		// Token: 0x06004B0B RID: 19211 RVA: 0x00101716 File Offset: 0x000FF916
		public virtual void Active(bool active)
		{
			base.gameObject.SetActive(active);
		}

		// Token: 0x06004B0C RID: 19212 RVA: 0x00223200 File Offset: 0x00221400
		protected virtual void BuildDescription()
		{
			Vars vars = new Vars(this.Data);
			if (this.m_Description != null)
			{
				UIText.SetTextKey(this.m_Description, this.descriptionKey, vars, null);
			}
			if (this.m_EffectsLabel != null)
			{
				UIText.SetTextKey(this.m_EffectsLabel, "Religion.effect_label", vars, null);
			}
		}

		// Token: 0x06004B0D RID: 19213 RVA: 0x0022325F File Offset: 0x0022145F
		protected virtual void BuildReligionEffects()
		{
			if (this.m_ReligionEffect != null)
			{
				UIText.SetText(this.m_ReligionEffect, global::Religions.GetReligionModsText(this.Data, "\n"));
			}
		}

		// Token: 0x06004B0E RID: 19214 RVA: 0x0022328C File Offset: 0x0022148C
		protected UIActionIcon BuildAction(string actionKey, GameObject button)
		{
			for (int i = 0; i < this.Data.actions.Count; i++)
			{
				Action action = this.Data.actions[i];
				if (!(action.def.field.key != actionKey))
				{
					Vars vars = new Vars(this.Data);
					vars.Set<string>("variant", "action_religion");
					UIActionIcon uiactionIcon = UIActionIcon.Possess(action.visuals as ActionVisuals, button, vars);
					if (uiactionIcon != null)
					{
						uiactionIcon.SetSkin("Neutral");
					}
					return uiactionIcon;
				}
			}
			return null;
		}

		// Token: 0x06004B0F RID: 19215 RVA: 0x0022332C File Offset: 0x0022152C
		protected void UpdateCrusade()
		{
			if (this.Data == null)
			{
				return;
			}
			if (this.m_CrusaderRoot == null)
			{
				this.m_CrusaderRoot = global::Common.FindChildByName(base.gameObject, "id_Crusade", true, true);
			}
			if (this.m_CrusaderCaption == null)
			{
				this.m_CrusaderCaption = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_CrusadeCaption");
			}
			if (this.m_CrusaderDescription == null)
			{
				this.m_CrusaderDescription = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_CrusadeDescription");
			}
			if (this.Data.game.religions.catholic.crusade != null)
			{
				if (this.m_CrusaderCaption != null)
				{
					UIText.SetTextKey(this.m_CrusaderCaption, "Crusade.cursader_status.caption", new Vars(this.Data.game.religions.catholic.crusade), null);
				}
				if (this.Data.game.religions.catholic.crusade.target == this.Data)
				{
					if (this.m_CrusaderDescription != null)
					{
						UIText.SetTextKey(this.m_CrusaderDescription, "Crusade.cursader_status.target", new Vars(this.Data.game.religions.catholic.crusade), null);
					}
				}
				else if (this.Data.game.religions.catholic.crusade.leader.kingdom_id == this.Data.id)
				{
					if (this.m_CrusaderDescription != null)
					{
						UIText.SetTextKey(this.m_CrusaderDescription, "Crusade.cursader_status.own", new Vars(this.Data.game.religions.catholic.crusade), null);
					}
				}
				else if (this.Data.is_christian && this.m_CrusaderDescription != null)
				{
					UIText.SetTextKey(this.m_CrusaderDescription, "Crusade.cursader_status.default", new Vars(this.Data.game.religions.catholic.crusade), null);
				}
				this.m_CrusaderRoot.gameObject.SetActive(true);
				return;
			}
			this.m_CrusaderRoot.gameObject.SetActive(false);
		}

		// Token: 0x06004B10 RID: 19216 RVA: 0x00223570 File Offset: 0x00221770
		protected void BuildAutocephalies()
		{
			if (this.m_Autocephalies == null)
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			if (this.Data.game.kingdoms == null)
			{
				return;
			}
			UICommon.DeleteChildren(this.m_Autocephalies.transform);
			int count = this.Data.game.kingdoms.Count;
			for (int i = 0; i < count; i++)
			{
				Logic.Kingdom kingdom = this.Data.game.kingdoms[i];
				if (!kingdom.IsDefeated() && kingdom.is_orthodox && !kingdom.subordinated && !kingdom.is_ecumenical_patriarchate)
				{
					Vars vars = new Vars(kingdom);
					vars.Set<string>("variant", "compact");
					ObjectIcon.GetIcon(kingdom, vars, this.m_Autocephalies.transform as RectTransform);
				}
			}
			this.m_Autocephalies.Refresh();
			if (this.m_LabelAutocephalies != null)
			{
				UIText.SetTextKey(this.m_LabelAutocephalies, "ReligionWidnow.current_autocephalies_label", null, null);
			}
		}

		// Token: 0x040039D5 RID: 14805
		[UIFieldTarget("id_Description")]
		protected TextMeshProUGUI m_Description;

		// Token: 0x040039D6 RID: 14806
		[UIFieldTarget("id_Effects")]
		protected TextMeshProUGUI m_ReligionEffect;

		// Token: 0x040039D7 RID: 14807
		[UIFieldTarget("id_EffectsLabel")]
		protected TextMeshProUGUI m_EffectsLabel;

		// Token: 0x040039D8 RID: 14808
		[UIFieldTarget("id_Autocephalies")]
		protected StackableIconsContainer m_Autocephalies;

		// Token: 0x040039D9 RID: 14809
		[UIFieldTarget("id_LabelAutocephalies")]
		protected TextMeshProUGUI m_LabelAutocephalies;

		// Token: 0x040039DB RID: 14811
		private GameObject m_CrusaderRoot;

		// Token: 0x040039DC RID: 14812
		private TextMeshProUGUI m_CrusaderCaption;

		// Token: 0x040039DD RID: 14813
		private TextMeshProUGUI m_CrusaderDescription;
	}

	// Token: 0x0200075A RID: 1882
	internal class Catholic : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06004B12 RID: 19218 RVA: 0x00223675 File Offset: 0x00221875
		protected override string id
		{
			get
			{
				return "id_Catholic";
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x06004B13 RID: 19219 RVA: 0x0022367C File Offset: 0x0022187C
		protected override string descriptionKey
		{
			get
			{
				return "Catholic.descritpion.default";
			}
		}

		// Token: 0x06004B14 RID: 19220 RVA: 0x00223684 File Offset: 0x00221884
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_catholic && !base.Data.excommunicated && base.Data.religion.hq_realm.GetKingdom() == base.Data.religion.hq_kingdom;
		}

		// Token: 0x06004B15 RID: 19221 RVA: 0x002236DC File Offset: 0x002218DC
		public override void Build()
		{
			base.Build();
			if (this.m_Audiance != null)
			{
				this.m_Audiance.onClick = new BSGButton.OnClick(this.HandleAudianceWithPope);
				if (this.m_AudianceLabel != null)
				{
					UIText.SetTextKey(this.m_AudianceLabel, "Catholic.audienceLabel.default", base.Data.religion, null);
				}
			}
			if (this.m_Relations != null)
			{
				this.m_Relations.SetData(base.Data, base.Data.religion.hq_kingdom);
			}
			this.PopulatePope();
			base.UpdateCrusade();
		}

		// Token: 0x06004B16 RID: 19222 RVA: 0x00223779 File Offset: 0x00221979
		protected override void Update()
		{
			base.Update();
			base.UpdateCrusade();
		}

		// Token: 0x06004B17 RID: 19223 RVA: 0x00223788 File Offset: 0x00221988
		private void PopulatePope()
		{
			Logic.Character pope = global::Religions.GetPope();
			if (pope == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Leader", true, true);
			if (gameObject != null)
			{
				gameObject.AddComponent<UIReligionWindow.ReligionLeader>().SetCharacter(pope, null);
			}
			if (base.Data.HasPope())
			{
				GameObject popeEffectsContainer = this.m_PopeEffectsContainer;
				if (popeEffectsContainer != null)
				{
					popeEffectsContainer.SetActive(true);
				}
				if (this.m_PopeEffectsLabel != null)
				{
					UIText.SetTextKey(this.m_PopeEffectsLabel, "Catholic.pope_effect_label", new Vars(base.Data), null);
				}
				if (this.m_PopeEffects != null)
				{
					this.m_PopeEffects.gameObject.SetActive(true);
					UIText.SetText(this.m_PopeEffects, global::Religions.GetPopeBonusesText(base.Data, "\n"));
				}
			}
			else
			{
				GameObject popeEffectsContainer2 = this.m_PopeEffectsContainer;
				if (popeEffectsContainer2 != null)
				{
					popeEffectsContainer2.SetActive(false);
				}
			}
			if (this.m_PopeEffects != null && base.Data.HasPope())
			{
				UIText.SetText(this.m_PopeEffects, global::Religions.GetPopeBonusesText(base.Data, "\n"));
			}
		}

		// Token: 0x06004B18 RID: 19224 RVA: 0x0022389A File Offset: 0x00221A9A
		private void HandleAudianceWithPope(BSGButton b)
		{
			AudienceWindow.Create(base.Data.religion.head_kingdom.visuals as global::Kingdom, "Main", base.Data.visuals as global::Kingdom);
		}

		// Token: 0x040039DE RID: 14814
		[UIFieldTarget("id_Audience")]
		protected BSGButton m_Audiance;

		// Token: 0x040039DF RID: 14815
		[UIFieldTarget("id_AudienceLabel")]
		protected TextMeshProUGUI m_AudianceLabel;

		// Token: 0x040039E0 RID: 14816
		[UIFieldTarget("id_Relations")]
		protected UIKingdomRelations m_Relations;

		// Token: 0x040039E1 RID: 14817
		[UIFieldTarget("id_PopeEffectsContainer")]
		protected GameObject m_PopeEffectsContainer;

		// Token: 0x040039E2 RID: 14818
		[UIFieldTarget("id_PopeEffectsLabel")]
		protected TextMeshProUGUI m_PopeEffectsLabel;

		// Token: 0x040039E3 RID: 14819
		[UIFieldTarget("id_PopeEffects")]
		protected TextMeshProUGUI m_PopeEffects;
	}

	// Token: 0x0200075B RID: 1883
	internal class CatholicExcommunicated : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06004B1A RID: 19226 RVA: 0x002238D9 File Offset: 0x00221AD9
		protected override string id
		{
			get
			{
				return "id_CatholicExcommunicated";
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06004B1B RID: 19227 RVA: 0x002238E0 File Offset: 0x00221AE0
		protected override string descriptionKey
		{
			get
			{
				return "Catholic.descritpion.excommunicated";
			}
		}

		// Token: 0x06004B1C RID: 19228 RVA: 0x002238E7 File Offset: 0x00221AE7
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_catholic && base.Data.excommunicated;
		}

		// Token: 0x06004B1D RID: 19229 RVA: 0x0022390C File Offset: 0x00221B0C
		public override void Build()
		{
			base.Build();
			if (this.m_Audiance != null)
			{
				this.m_Audiance.onClick = new BSGButton.OnClick(this.HandleAudianceWithPope);
				if (this.m_AudianceLabel != null)
				{
					UIText.SetTextKey(this.m_AudianceLabel, "Catholic.audienceLabel.excommunicated", base.Data.religion, null);
				}
			}
			if (this.m_Relations != null)
			{
				this.m_Relations.SetData(base.Data, base.Data.religion.hq_kingdom);
			}
			if (this.m_ExcomunicatedLabel != null)
			{
				UIText.SetTextKey(this.m_ExcomunicatedLabel, "ReligionWidnow.excommunicated_caption", null, null);
			}
			if (this.m_ExcomunicatedBody != null)
			{
				UIText.SetTextKey(this.m_ExcomunicatedBody, "ReligionWidnow.excommunicated_description", null, null);
			}
			this.PopulatePope();
			base.UpdateCrusade();
			this.BuildActions();
			this.CheckReligionAction();
		}

		// Token: 0x06004B1E RID: 19230 RVA: 0x002239F5 File Offset: 0x00221BF5
		protected override void Update()
		{
			base.Update();
			base.UpdateCrusade();
			this.CheckReligionAction();
		}

		// Token: 0x06004B1F RID: 19231 RVA: 0x00223A0C File Offset: 0x00221C0C
		private void PopulatePope()
		{
			Logic.Character pope = global::Religions.GetPope();
			if (pope == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Leader", true, true);
			if (gameObject != null)
			{
				gameObject.GetOrAddComponent<UIReligionWindow.ReligionLeader>().SetCharacter(pope, null);
			}
		}

		// Token: 0x06004B20 RID: 19232 RVA: 0x00223A4C File Offset: 0x00221C4C
		private void BuildActions()
		{
			base.BuildAction("UnExcommunicateAction", this.m_UnExcommunicateAction);
		}

		// Token: 0x06004B21 RID: 19233 RVA: 0x00223A60 File Offset: 0x00221C60
		private void CheckReligionAction()
		{
			if (this.m_CurrentActionProgress == null)
			{
				return;
			}
			if (this.m_ActiveAction != null && this.m_ActiveAction.state == Action.State.Inactive)
			{
				this.m_ActiveAction = null;
			}
			for (int i = 0; i < base.Data.actions.Count; i++)
			{
				Action action = base.Data.actions[i];
				if (action is UnExcommunicateAction && action.state != Action.State.Inactive && action != this.m_ActiveAction)
				{
					this.m_ActiveAction = action;
					this.m_CurrentActionProgress.SetData(action);
				}
			}
			bool flag = this.m_ActiveAction != null;
			this.m_CurrentActionProgress.gameObject.SetActive(flag);
			if (this.m_ActionsContainer != null)
			{
				this.m_ActionsContainer.gameObject.SetActive(!flag);
			}
		}

		// Token: 0x06004B22 RID: 19234 RVA: 0x0022389A File Offset: 0x00221A9A
		private void HandleAudianceWithPope(BSGButton b)
		{
			AudienceWindow.Create(base.Data.religion.head_kingdom.visuals as global::Kingdom, "Main", base.Data.visuals as global::Kingdom);
		}

		// Token: 0x040039E4 RID: 14820
		[UIFieldTarget("id_Audience")]
		protected BSGButton m_Audiance;

		// Token: 0x040039E5 RID: 14821
		[UIFieldTarget("id_AudienceLabel")]
		protected TextMeshProUGUI m_AudianceLabel;

		// Token: 0x040039E6 RID: 14822
		[UIFieldTarget("id_UnExcommunicateAction")]
		protected GameObject m_UnExcommunicateAction;

		// Token: 0x040039E7 RID: 14823
		[UIFieldTarget("id_ExcomunicatedLabel")]
		protected TextMeshProUGUI m_ExcomunicatedLabel;

		// Token: 0x040039E8 RID: 14824
		[UIFieldTarget("id_ExcomunicatedBody")]
		protected TextMeshProUGUI m_ExcomunicatedBody;

		// Token: 0x040039E9 RID: 14825
		[UIFieldTarget("id_CurrentAction")]
		private UIActionProgressInfo m_CurrentActionProgress;

		// Token: 0x040039EA RID: 14826
		[UIFieldTarget("id_ActionsContainer")]
		private GameObject m_ActionsContainer;

		// Token: 0x040039EB RID: 14827
		[UIFieldTarget("id_Relations")]
		protected UIKingdomRelations m_Relations;

		// Token: 0x040039EC RID: 14828
		private Action m_ActiveAction;
	}

	// Token: 0x0200075C RID: 1884
	internal class CatholicNoPope : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x06004B24 RID: 19236 RVA: 0x00223B2E File Offset: 0x00221D2E
		protected override string id
		{
			get
			{
				return "id_CatholicNoPope";
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06004B25 RID: 19237 RVA: 0x00223B35 File Offset: 0x00221D35
		protected override string descriptionKey
		{
			get
			{
				return "Catholic.no_papacy";
			}
		}

		// Token: 0x06004B26 RID: 19238 RVA: 0x00223B3C File Offset: 0x00221D3C
		public override bool Validate()
		{
			if (base.Data == null || !base.Data.is_catholic)
			{
				return false;
			}
			if (base.Data.religion.hq_kingdom == base.Data)
			{
				return false;
			}
			Logic.Kingdom kingdom = base.Data.religion.hq_realm.GetKingdom();
			return kingdom != base.Data.religion.hq_kingdom && kingdom != base.Data;
		}

		// Token: 0x06004B27 RID: 19239 RVA: 0x00223BB1 File Offset: 0x00221DB1
		public override void Build()
		{
			base.Build();
			this.PopulateUsurper();
		}

		// Token: 0x06004B28 RID: 19240 RVA: 0x00223BC0 File Offset: 0x00221DC0
		private void PopulateUsurper()
		{
			if (base.Data == null)
			{
				return;
			}
			if (base.Data.religion == null)
			{
				return;
			}
			if (base.Data.religion.hq_realm == null)
			{
				return;
			}
			Logic.Kingdom kingdom = base.Data.religion.hq_realm.GetKingdom();
			if (kingdom == null)
			{
				return;
			}
			if (this.m_UsuperKingdomIcon != null)
			{
				this.m_UsuperKingdomIcon.SetObject(kingdom, null);
			}
			if (this.m_UsurperLabel != null)
			{
				UIText.SetTextKey(this.m_UsurperLabel, "ReligionWidnow.rome_owner", kingdom, null);
			}
			if (this.m_Caption != null)
			{
				UIText.SetTextKey(this.m_Caption, "ReligionWidnow.papacy_destroyed", kingdom, null);
			}
		}

		// Token: 0x06004B29 RID: 19241 RVA: 0x00223C6C File Offset: 0x00221E6C
		private void HandleAudianceWithTheUsurper(BSGButton b)
		{
			AudienceWindow.Create(base.Data.religion.head_kingdom.GetKingdom().visuals as global::Kingdom, "Main", base.Data.visuals as global::Kingdom);
		}

		// Token: 0x040039ED RID: 14829
		[UIFieldTarget("id_UsuperKingdomIcon")]
		protected UIKingdomIcon m_UsuperKingdomIcon;

		// Token: 0x040039EE RID: 14830
		[UIFieldTarget("id_Caption")]
		protected TextMeshProUGUI m_Caption;

		// Token: 0x040039EF RID: 14831
		[UIFieldTarget("id_UsurperLabel")]
		protected TextMeshProUGUI m_UsurperLabel;
	}

	// Token: 0x0200075D RID: 1885
	internal class CatholicOwningRome : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06004B2B RID: 19243 RVA: 0x00223CA8 File Offset: 0x00221EA8
		protected override string id
		{
			get
			{
				return "id_CatholicOwningRome";
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06004B2C RID: 19244 RVA: 0x00223CAF File Offset: 0x00221EAF
		protected override string descriptionKey
		{
			get
			{
				return "Catholic.descritpion.own_rome";
			}
		}

		// Token: 0x06004B2D RID: 19245 RVA: 0x00223CB8 File Offset: 0x00221EB8
		public override bool Validate()
		{
			return base.Data != null && base.Data.religion.hq_kingdom != base.Data && base.Data.is_catholic && base.Data.religion.hq_realm.GetKingdom() == base.Data;
		}

		// Token: 0x06004B2E RID: 19246 RVA: 0x00223D15 File Offset: 0x00221F15
		public override void Build()
		{
			base.Build();
			this.PopulateUsurper();
			this.BuildActions();
		}

		// Token: 0x06004B2F RID: 19247 RVA: 0x00223D2C File Offset: 0x00221F2C
		private void PopulateUsurper()
		{
			if (base.Data == null)
			{
				return;
			}
			if (base.Data.religion == null)
			{
				return;
			}
			if (base.Data.religion.hq_realm == null)
			{
				return;
			}
			Logic.Kingdom kingdom = base.Data.religion.hq_realm.GetKingdom();
			if (kingdom == null)
			{
				return;
			}
			if (this.m_UsuperKingdomIcon != null)
			{
				this.m_UsuperKingdomIcon.SetObject(kingdom, null);
			}
			if (this.m_UsurperLabel != null)
			{
				UIText.SetTextKey(this.m_UsurperLabel, "ReligionWidnow.rome_owner_player", kingdom, null);
			}
			if (this.m_Caption != null)
			{
				UIText.SetTextKey(this.m_Caption, "ReligionWidnow.papacy_destroyed", kingdom, null);
			}
		}

		// Token: 0x06004B30 RID: 19248 RVA: 0x00223DD8 File Offset: 0x00221FD8
		private void BuildActions()
		{
			base.BuildAction("RestorePapacyAction", this.m_RestorePapacy);
		}

		// Token: 0x040039F0 RID: 14832
		[UIFieldTarget("id_RestorePapacy")]
		protected GameObject m_RestorePapacy;

		// Token: 0x040039F1 RID: 14833
		[UIFieldTarget("id_UsuperKingdomIcon")]
		protected UIKingdomIcon m_UsuperKingdomIcon;

		// Token: 0x040039F2 RID: 14834
		[UIFieldTarget("id_Caption")]
		protected TextMeshProUGUI m_Caption;

		// Token: 0x040039F3 RID: 14835
		[UIFieldTarget("id_UsurperLabel")]
		protected TextMeshProUGUI m_UsurperLabel;
	}

	// Token: 0x0200075E RID: 1886
	internal class OrthodoxEcumenical : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06004B32 RID: 19250 RVA: 0x00223DEC File Offset: 0x00221FEC
		protected override string id
		{
			get
			{
				return "id_OrthodoxEcumenical";
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06004B33 RID: 19251 RVA: 0x00223DF3 File Offset: 0x00221FF3
		protected override string descriptionKey
		{
			get
			{
				return "Orthodox.descritpion.default";
			}
		}

		// Token: 0x06004B34 RID: 19252 RVA: 0x00223DFA File Offset: 0x00221FFA
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_orthodox && base.Data.HasEcumenicalPatriarch();
		}

		// Token: 0x06004B35 RID: 19253 RVA: 0x00223E1E File Offset: 0x0022201E
		public override void Build()
		{
			base.Build();
			this.PopulatePatriarch();
			this.BuildDescription();
			base.BuildAutocephalies();
		}

		// Token: 0x06004B36 RID: 19254 RVA: 0x00223E38 File Offset: 0x00222038
		protected override void Update()
		{
			base.Update();
		}

		// Token: 0x06004B37 RID: 19255 RVA: 0x00223E40 File Offset: 0x00222040
		private void PopulatePatriarch()
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Leader", true, true);
			if (gameObject != null)
			{
				gameObject.GetOrAddComponent<UIReligionWindow.ReligionLeader>().SetCharacter(global::Religions.GetPatriarch(), null);
			}
			if (this.m_PatriarchEffectsLabel != null)
			{
				UIText.SetTextKey(this.m_PatriarchEffectsLabel, "Orthodox.patriarch_effect_label", new Vars(base.Data), null);
			}
			if (this.m_PatriarchEffects != null)
			{
				UIText.SetText(this.m_PatriarchEffects, global::Religions.GetPatriarchBonusesText(base.Data, "\n"));
			}
		}

		// Token: 0x040039F4 RID: 14836
		[UIFieldTarget("id_PatriarchEffectsLabel")]
		protected TextMeshProUGUI m_PatriarchEffectsLabel;

		// Token: 0x040039F5 RID: 14837
		[UIFieldTarget("id_PatriarchEffects")]
		protected TextMeshProUGUI m_PatriarchEffects;
	}

	// Token: 0x0200075F RID: 1887
	internal class OrthodoxIndependent : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x06004B39 RID: 19257 RVA: 0x00223ED2 File Offset: 0x002220D2
		protected override string id
		{
			get
			{
				return "id_OrthodoxIndependent";
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06004B3A RID: 19258 RVA: 0x00223ED9 File Offset: 0x002220D9
		protected override string descriptionKey
		{
			get
			{
				return "Orthodox.descritpion.independent";
			}
		}

		// Token: 0x06004B3B RID: 19259 RVA: 0x00223EE0 File Offset: 0x002220E0
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_orthodox && !base.Data.subordinated && !base.Data.HasEcumenicalPatriarch();
		}

		// Token: 0x06004B3C RID: 19260 RVA: 0x00223F14 File Offset: 0x00222114
		public override void Build()
		{
			base.Build();
			this.PopulatePatriarch();
			this.BuildDescription();
			base.BuildAutocephalies();
		}

		// Token: 0x06004B3D RID: 19261 RVA: 0x00223E38 File Offset: 0x00222038
		protected override void Update()
		{
			base.Update();
		}

		// Token: 0x06004B3E RID: 19262 RVA: 0x00223F30 File Offset: 0x00222130
		private void PopulatePatriarch()
		{
			Logic.Character patriarch = base.Data.patriarch;
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Leader", true, true);
			if (gameObject != null)
			{
				gameObject.GetOrAddComponent<UIReligionWindow.ReligionLeader>().SetCharacter(patriarch, null);
			}
			if (patriarch != null)
			{
				TextMeshProUGUI patriarchEffectsLabel = this.m_PatriarchEffectsLabel;
				if (patriarchEffectsLabel != null)
				{
					patriarchEffectsLabel.gameObject.SetActive(true);
				}
				TextMeshProUGUI patriarchEffects = this.m_PatriarchEffects;
				if (patriarchEffects != null)
				{
					patriarchEffects.gameObject.SetActive(true);
				}
				if (this.m_PatriarchEffectsLabel != null)
				{
					UIText.SetTextKey(this.m_PatriarchEffectsLabel, "Orthodox.patriarch_effect_label", new Vars(base.Data), null);
				}
				if (this.m_PatriarchEffects != null)
				{
					UIText.SetText(this.m_PatriarchEffects, global::Religions.GetPatriarchBonusesText(base.Data, "\n"));
				}
				return;
			}
			TextMeshProUGUI patriarchEffectsLabel2 = this.m_PatriarchEffectsLabel;
			if (patriarchEffectsLabel2 != null)
			{
				patriarchEffectsLabel2.gameObject.SetActive(false);
			}
			TextMeshProUGUI patriarchEffects2 = this.m_PatriarchEffects;
			if (patriarchEffects2 == null)
			{
				return;
			}
			patriarchEffects2.gameObject.SetActive(false);
		}

		// Token: 0x040039F6 RID: 14838
		[UIFieldTarget("id_PatriarchEffectsLabel")]
		protected TextMeshProUGUI m_PatriarchEffectsLabel;

		// Token: 0x040039F7 RID: 14839
		[UIFieldTarget("id_PatriarchEffects")]
		protected TextMeshProUGUI m_PatriarchEffects;
	}

	// Token: 0x02000760 RID: 1888
	internal class OrthodoxSubordinated : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06004B40 RID: 19264 RVA: 0x00224029 File Offset: 0x00222229
		protected override string id
		{
			get
			{
				return "id_OrthodoxSubordinated";
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06004B41 RID: 19265 RVA: 0x00224030 File Offset: 0x00222230
		protected override string descriptionKey
		{
			get
			{
				return "Orthodox.descritpion.subordinated";
			}
		}

		// Token: 0x06004B42 RID: 19266 RVA: 0x00224037 File Offset: 0x00222237
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_orthodox && base.Data.subordinated && !base.Data.HasEcumenicalPatriarch();
		}

		// Token: 0x06004B43 RID: 19267 RVA: 0x0022406C File Offset: 0x0022226C
		public override void Build()
		{
			base.Build();
			this.BuildActions();
			this.PopulateEcumenicalPatriarch();
			this.BuildDescription();
			base.BuildAutocephalies();
			if (this.m_Audiance != null)
			{
				this.m_Audiance.onClick = new BSGButton.OnClick(this.HandleAudianceWithHeadKigndom);
				if (this.m_AudianceLabel != null)
				{
					UIText.SetTextKey(this.m_AudianceLabel, "Orthodox.audienceLabel.default", base.Data.religion, null);
				}
			}
		}

		// Token: 0x06004B44 RID: 19268 RVA: 0x002240E6 File Offset: 0x002222E6
		private void BuildActions()
		{
			if (this.m_AutocephalyAction != null)
			{
				this.m_AutocephalyAction.SetActive(false);
			}
		}

		// Token: 0x06004B45 RID: 19269 RVA: 0x00224104 File Offset: 0x00222304
		private void PopulateEcumenicalPatriarch()
		{
			Logic.Character head = base.Data.game.religions.orthodox.head;
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Leader", true, true);
			if (gameObject != null)
			{
				gameObject.GetOrAddComponent<UIReligionWindow.ReligionLeader>().SetCharacter(head, null);
			}
		}

		// Token: 0x06004B46 RID: 19270 RVA: 0x0022389A File Offset: 0x00221A9A
		private void HandleAudianceWithHeadKigndom(BSGButton b)
		{
			AudienceWindow.Create(base.Data.religion.head_kingdom.visuals as global::Kingdom, "Main", base.Data.visuals as global::Kingdom);
		}

		// Token: 0x040039F8 RID: 14840
		[UIFieldTarget("id_Audience")]
		protected BSGButton m_Audiance;

		// Token: 0x040039F9 RID: 14841
		[UIFieldTarget("id_AudienceLabel")]
		protected TextMeshProUGUI m_AudianceLabel;

		// Token: 0x040039FA RID: 14842
		[UIFieldTarget("id_AutocephalyAction")]
		protected GameObject m_AutocephalyAction;

		// Token: 0x040039FB RID: 14843
		[UIFieldTarget("id_AutocephalyActionLabel")]
		protected TextMeshProUGUI m_AutocephalyActionLabel;
	}

	// Token: 0x02000761 RID: 1889
	internal class IslamCaliphate : UIReligionWindow.ReligionSubState, IListener
	{
		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x06004B48 RID: 19272 RVA: 0x00224155 File Offset: 0x00222355
		protected override string id
		{
			get
			{
				return "id_IslamCaliphate";
			}
		}

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06004B49 RID: 19273 RVA: 0x0022415C File Offset: 0x0022235C
		protected override string descriptionKey
		{
			get
			{
				if (!base.Data.is_shia)
				{
					return "Sunni.descritpion.caliphate";
				}
				return "Shia.descritpion.caliphate";
			}
		}

		// Token: 0x06004B4A RID: 19274 RVA: 0x00224178 File Offset: 0x00222378
		public override void SetData(Logic.Kingdom k)
		{
			if (base.Data != null)
			{
				base.Data.DelListener(this);
			}
			Logic.Kingdom data = base.Data;
			bool flag;
			if (data == null)
			{
				flag = (null != null);
			}
			else
			{
				Game game = data.game;
				flag = (((game != null) ? game.religions : null) != null);
			}
			if (flag)
			{
				base.Data.game.religions.DelListener(this);
			}
			base.SetData(k);
			if (base.Data != null)
			{
				base.Data.AddListener(this);
			}
			Logic.Kingdom data2 = base.Data;
			bool flag2;
			if (data2 == null)
			{
				flag2 = (null != null);
			}
			else
			{
				Game game2 = data2.game;
				flag2 = (((game2 != null) ? game2.religions : null) != null);
			}
			if (flag2)
			{
				base.Data.game.religions.AddListener(this);
			}
		}

		// Token: 0x06004B4B RID: 19275 RVA: 0x00224220 File Offset: 0x00222420
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_muslim && base.Data.caliphate;
		}

		// Token: 0x06004B4C RID: 19276 RVA: 0x00224244 File Offset: 0x00222444
		public override void Build()
		{
			base.Build();
			if (this.m_LabelCaliphate != null)
			{
				UIText.SetTextKey(this.m_LabelCaliphate, "ReligionWidnow.current_caliphates_label", null, null);
			}
			this.BuildActions();
			this.BuildCaliphates();
			this.BuildsJihad();
		}

		// Token: 0x06004B4D RID: 19277 RVA: 0x00224280 File Offset: 0x00222480
		protected override void Update()
		{
			base.Update();
			if (this.CallJihadIcon != null)
			{
				this.CallJihadIcon.gameObject.SetActive(!(this.CallJihadIcon.Data.logic.Validate(false) == "already_leading_jihad"));
			}
			if (this.EndJihadIcon != null)
			{
				this.EndJihadIcon.gameObject.SetActive(!(this.EndJihadIcon.Data.logic.Validate(false) == "no_jihad"));
			}
			if (this.m_CallJihad != null)
			{
				this.m_CallJihad.SetActive(base.Data.jihad == null);
			}
		}

		// Token: 0x06004B4E RID: 19278 RVA: 0x0022433C File Offset: 0x0022253C
		private void BuildsJihad()
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Jihads", true, true);
			if (gameObject != null)
			{
				UIReligionWindow.JihadsInfo orAddComponent = gameObject.GetOrAddComponent<UIReligionWindow.JihadsInfo>();
				if (orAddComponent == null)
				{
					return;
				}
				orAddComponent.SetData(base.Data);
			}
		}

		// Token: 0x06004B4F RID: 19279 RVA: 0x0022437C File Offset: 0x0022257C
		private void BuildActions()
		{
			if (base.Data.jihad != null)
			{
				UIActionIcon callJihadIcon = this.CallJihadIcon;
				if (callJihadIcon != null)
				{
					callJihadIcon.SetObject(null, null);
				}
				UIActionIcon component = this.m_CallJihad.GetComponent<UIActionIcon>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
					return;
				}
			}
			else
			{
				this.CallJihadIcon = base.BuildAction("CallForJihadAction", this.m_CallJihad);
				if (this.m_CallJihadLabel != null)
				{
					UIText.SetTextKey(this.m_CallJihadLabel, "Muslim.callJihad", null, null);
				}
			}
		}

		// Token: 0x06004B50 RID: 19280 RVA: 0x002243FC File Offset: 0x002225FC
		private void BuildCaliphates()
		{
			new Vars(base.Data).Set<string>("variant", "compact");
			if (this.m_KingodmIcon != null)
			{
				this.m_KingodmIcon.SetObject(base.Data, null);
			}
			if (this.m_Caliphates == null)
			{
				return;
			}
			if (base.Data == null)
			{
				return;
			}
			if (base.Data.game.kingdoms == null)
			{
				return;
			}
			int count = base.Data.game.kingdoms.Count;
			UICommon.DeleteChildren(this.m_Caliphates.transform);
			for (int i = 0; i < count; i++)
			{
				Logic.Kingdom kingdom = base.Data.game.kingdoms[i];
				if (kingdom.IsCaliphate() && !kingdom.IsDefeated())
				{
					Vars vars = new Vars(kingdom);
					vars.Set<string>("variant", "compact");
					ObjectIcon.GetIcon(kingdom, vars, this.m_Caliphates.transform as RectTransform);
				}
			}
			this.m_Caliphates.Refresh();
		}

		// Token: 0x06004B51 RID: 19281 RVA: 0x00224510 File Offset: 0x00222710
		public void OnMessage(object obj, string message, object param)
		{
			if (this == null)
			{
				Logic.Kingdom data = base.Data;
				if (data != null)
				{
					data.DelListener(this);
				}
				Logic.Kingdom data2 = base.Data;
				if (data2 == null)
				{
					return;
				}
				Game game = data2.game;
				if (game == null)
				{
					return;
				}
				game.religions.DelListener(this);
				return;
			}
			else
			{
				if (message == "new_jihad" || message == "end_jihad" || message == "jihad_changed")
				{
					this.BuildActions();
					return;
				}
				if (!(message == "destroying") && !(message == "finishing"))
				{
					return;
				}
				Logic.Kingdom data3 = base.Data;
				if (data3 != null)
				{
					data3.DelListener(this);
				}
				Logic.Kingdom data4 = base.Data;
				if (data4 == null)
				{
					return;
				}
				Game game2 = data4.game;
				if (game2 == null)
				{
					return;
				}
				game2.religions.DelListener(this);
				return;
			}
		}

		// Token: 0x040039FC RID: 14844
		[UIFieldTarget("id_AbandonCaliphate")]
		protected GameObject m_AbandonCaliphate;

		// Token: 0x040039FD RID: 14845
		[UIFieldTarget("id_AbandonCaliphateLabel")]
		protected TextMeshProUGUI m_AbandonCaliphateLabel;

		// Token: 0x040039FE RID: 14846
		[UIFieldTarget("id_CallJihad")]
		protected GameObject m_CallJihad;

		// Token: 0x040039FF RID: 14847
		[UIFieldTarget("id_CallJihadLabel")]
		protected TextMeshProUGUI m_CallJihadLabel;

		// Token: 0x04003A00 RID: 14848
		[UIFieldTarget("id_EndJihad")]
		protected GameObject m_EndJihad;

		// Token: 0x04003A01 RID: 14849
		[UIFieldTarget("id_EndJihadLabel")]
		protected TextMeshProUGUI m_EndJihadLabel;

		// Token: 0x04003A02 RID: 14850
		[UIFieldTarget("id_LabelCaliphate")]
		protected TextMeshProUGUI m_LabelCaliphate;

		// Token: 0x04003A03 RID: 14851
		[UIFieldTarget("id_Caliphates")]
		protected StackableIconsContainer m_Caliphates;

		// Token: 0x04003A04 RID: 14852
		[UIFieldTarget("id_KingodmIcon")]
		protected UIKingdomIcon m_KingodmIcon;

		// Token: 0x04003A05 RID: 14853
		private UIActionIcon CallJihadIcon;

		// Token: 0x04003A06 RID: 14854
		private UIActionIcon EndJihadIcon;
	}

	// Token: 0x02000762 RID: 1890
	internal class IslamNoCaliphate : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06004B53 RID: 19283 RVA: 0x002245D4 File Offset: 0x002227D4
		protected override string id
		{
			get
			{
				return "id_IslamNoCaliphate";
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06004B54 RID: 19284 RVA: 0x002245DB File Offset: 0x002227DB
		protected override string descriptionKey
		{
			get
			{
				if (!base.Data.is_shia)
				{
					return "Sunni.descritpion.default";
				}
				return "Shia.descritpion.default";
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06004B55 RID: 19285 RVA: 0x002245F8 File Offset: 0x002227F8
		protected string caliphatesDescriptionKey
		{
			get
			{
				List<Logic.Kingdom> kingdoms = base.Data.game.kingdoms;
				bool flag = false;
				for (int i = 0; i < kingdoms.Count; i++)
				{
					if (kingdoms[i].IsCaliphate() && kingdoms[i] != base.Data && !kingdoms[i].IsDefeated())
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					if (!base.Data.is_shia)
					{
						return "Sunni.caliphatesDescription";
					}
					return "Shia.caliphatesDescription";
				}
				else
				{
					if (!base.Data.is_shia)
					{
						return "Sunni.noCaliphatesDescription";
					}
					return "Shia.noCaliphatesDescription";
				}
			}
		}

		// Token: 0x06004B56 RID: 19286 RVA: 0x0022468C File Offset: 0x0022288C
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_muslim && !base.Data.caliphate;
		}

		// Token: 0x06004B57 RID: 19287 RVA: 0x002246B3 File Offset: 0x002228B3
		public override void Build()
		{
			base.Build();
			if (this.m_LabelCaliphate != null)
			{
				UIText.SetTextKey(this.m_LabelCaliphate, "ReligionWidnow.current_caliphates_label", null, null);
			}
			this.BuildActions();
			this.BuildsJihad();
			this.BuildCaliphates();
		}

		// Token: 0x06004B58 RID: 19288 RVA: 0x002246F0 File Offset: 0x002228F0
		private void BuildsJihad()
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Jihads", true, true);
			if (gameObject != null)
			{
				UIReligionWindow.JihadsInfo orAddComponent = gameObject.GetOrAddComponent<UIReligionWindow.JihadsInfo>();
				if (orAddComponent == null)
				{
					return;
				}
				orAddComponent.SetData(base.Data);
			}
		}

		// Token: 0x06004B59 RID: 19289 RVA: 0x00224730 File Offset: 0x00222930
		private void BuildCaliphates()
		{
			if (this.m_Caliphates == null)
			{
				return;
			}
			if (base.Data == null)
			{
				return;
			}
			if (base.Data.game.kingdoms == null)
			{
				return;
			}
			int count = base.Data.game.kingdoms.Count;
			UICommon.DeleteChildren(this.m_Caliphates.transform);
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				Logic.Kingdom kingdom = base.Data.game.kingdoms[i];
				if (kingdom.IsCaliphate() && !kingdom.IsDefeated())
				{
					Vars vars = new Vars(kingdom);
					vars.Set<string>("variant", "compact");
					ObjectIcon.GetIcon(kingdom, vars, this.m_Caliphates.transform as RectTransform);
					num++;
				}
			}
			Vars vars2 = new Vars();
			vars2.Set<int>("num_caliphates", num);
			UIText.SetTextKey(this.m_CaliphatesDescription, this.caliphatesDescriptionKey, vars2, null);
			this.m_Caliphates.gameObject.SetActive(num > 0);
			this.m_Caliphates.Refresh();
		}

		// Token: 0x06004B5A RID: 19290 RVA: 0x0022484C File Offset: 0x00222A4C
		private void BuildActions()
		{
			base.BuildAction("ClaimCaliphateAction", this.m_EstablishCaliphate);
			if (this.m_EstablishCaliphateLabel != null)
			{
				UIText.SetTextKey(this.m_EstablishCaliphateLabel, "Muslim.establishCaliphate", null, null);
			}
		}

		// Token: 0x06004B5B RID: 19291 RVA: 0x00224880 File Offset: 0x00222A80
		protected override void Update()
		{
			base.Update();
			this.CheckReligionAction();
		}

		// Token: 0x06004B5C RID: 19292 RVA: 0x00224890 File Offset: 0x00222A90
		private void CheckReligionAction()
		{
			if (this.m_CurrentActionProgress == null)
			{
				return;
			}
			if (this.m_ClaimCaliphateAction != null && this.m_ClaimCaliphateAction.state == Action.State.Inactive)
			{
				this.m_ClaimCaliphateAction = null;
			}
			for (int i = 0; i < base.Data.actions.Count; i++)
			{
				Action action = base.Data.actions[i];
				if (action is ClaimCaliphateAction && action.state != Action.State.Inactive && action != this.m_ClaimCaliphateAction)
				{
					this.m_ClaimCaliphateAction = action;
					this.m_CurrentActionProgress.SetData(action);
				}
			}
			bool flag = this.m_ClaimCaliphateAction != null;
			this.m_CurrentActionProgress.gameObject.SetActive(flag);
			if (this.m_ActionsContainer != null)
			{
				this.m_ActionsContainer.gameObject.SetActive(!flag);
			}
		}

		// Token: 0x04003A07 RID: 14855
		[UIFieldTarget("id_EstablishCaliphate")]
		protected GameObject m_EstablishCaliphate;

		// Token: 0x04003A08 RID: 14856
		[UIFieldTarget("id_EstablishCaliphateLabel")]
		protected TextMeshProUGUI m_EstablishCaliphateLabel;

		// Token: 0x04003A09 RID: 14857
		[UIFieldTarget("id_LabelCaliphate")]
		protected TextMeshProUGUI m_LabelCaliphate;

		// Token: 0x04003A0A RID: 14858
		[UIFieldTarget("id_Caliphates")]
		protected StackableIconsContainer m_Caliphates;

		// Token: 0x04003A0B RID: 14859
		[UIFieldTarget("id_DescriptionEstablishCaliphate")]
		protected TextMeshProUGUI m_CaliphatesDescription;

		// Token: 0x04003A0C RID: 14860
		[UIFieldTarget("id_CurrentAction")]
		private UIActionProgressInfo m_CurrentActionProgress;

		// Token: 0x04003A0D RID: 14861
		[UIFieldTarget("id_ActionsContainer")]
		private GameObject m_ActionsContainer;

		// Token: 0x04003A0E RID: 14862
		private Action m_ClaimCaliphateAction;
	}

	// Token: 0x02000763 RID: 1891
	internal class Suni : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x06004B5E RID: 19294 RVA: 0x0022495E File Offset: 0x00222B5E
		protected override string id
		{
			get
			{
				return "id_Suni";
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x06004B5F RID: 19295 RVA: 0x00224965 File Offset: 0x00222B65
		protected override string descriptionKey
		{
			get
			{
				return "Religion.Islam.Effects.Label";
			}
		}

		// Token: 0x06004B60 RID: 19296 RVA: 0x0022496C File Offset: 0x00222B6C
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_sunni;
		}

		// Token: 0x06004B61 RID: 19297 RVA: 0x00224983 File Offset: 0x00222B83
		public override void Build()
		{
			base.Build();
		}
	}

	// Token: 0x02000764 RID: 1892
	internal class Shia : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x06004B63 RID: 19299 RVA: 0x0022498B File Offset: 0x00222B8B
		protected override string id
		{
			get
			{
				return "id_Shia";
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06004B64 RID: 19300 RVA: 0x00224965 File Offset: 0x00222B65
		protected override string descriptionKey
		{
			get
			{
				return "Religion.Islam.Effects.Label";
			}
		}

		// Token: 0x06004B65 RID: 19301 RVA: 0x00224992 File Offset: 0x00222B92
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_shia;
		}

		// Token: 0x06004B66 RID: 19302 RVA: 0x00224983 File Offset: 0x00222B83
		public override void Build()
		{
			base.Build();
		}
	}

	// Token: 0x02000765 RID: 1893
	internal class Pagan : UIReligionWindow.ReligionSubState
	{
		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06004B68 RID: 19304 RVA: 0x002249A9 File Offset: 0x00222BA9
		protected override string id
		{
			get
			{
				return "id_Pagan";
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x06004B69 RID: 19305 RVA: 0x002249B0 File Offset: 0x00222BB0
		protected override string descriptionKey
		{
			get
			{
				return "Pagan.descritpion.default";
			}
		}

		// Token: 0x06004B6A RID: 19306 RVA: 0x002249B7 File Offset: 0x00222BB7
		public override bool Validate()
		{
			return base.Data != null && base.Data.is_pagan;
		}

		// Token: 0x06004B6B RID: 19307 RVA: 0x002249D0 File Offset: 0x00222BD0
		public override void Build()
		{
			base.Build();
			if (this.m_BeliefPrototype != null)
			{
				this.m_BeliefPrototype.gameObject.SetActive(false);
			}
			this.m_UpkeepVars.obj = base.Data;
			this.BuildBeliefs();
			this.UpdateLabels();
		}

		// Token: 0x06004B6C RID: 19308 RVA: 0x00224A24 File Offset: 0x00222C24
		private void BuildBeliefs()
		{
			if (this.m_BeliefContainer == null)
			{
				return;
			}
			if (this.m_BeliefPrototype == null)
			{
				return;
			}
			UICommon.DeleteActiveChildren(this.m_BeliefContainer);
			if (base.Data == null)
			{
				return;
			}
			if (base.Data.pagan_beliefs == null)
			{
				return;
			}
			List<ValueTuple<Religion.PaganBelief, Logic.Character>> beliefUnderAdoption = this.GetBeliefUnderAdoption();
			int max_pagan_beliefs = base.Data.game.religions.pagan.def.max_pagan_beliefs;
			for (int i = 0; i < max_pagan_beliefs; i++)
			{
				Religion.PaganBelief paganBelief = (base.Data.pagan_beliefs.Count > i) ? base.Data.pagan_beliefs[i] : null;
				Logic.Character owner = null;
				if (paganBelief == null && beliefUnderAdoption != null && beliefUnderAdoption.Count > 0)
				{
					paganBelief = beliefUnderAdoption[0].Item1;
					owner = beliefUnderAdoption[0].Item2;
					beliefUnderAdoption.RemoveAt(0);
				}
				PaganBeliefStatus beliefStatus = UIReligionWindow.Pagan.GetBeliefStatus(base.Data, paganBelief);
				if (beliefStatus != null)
				{
					owner = (beliefStatus.owner as Logic.Character);
				}
				GameObject gameObject = global::Common.Spawn(this.m_BeliefPrototype, this.m_BeliefContainer, false, "");
				gameObject.AddComponent<UIReligionWindow.Pagan.BeliefIcon>().SetData(paganBelief, owner);
				gameObject.gameObject.SetActive(true);
			}
		}

		// Token: 0x06004B6D RID: 19309 RVA: 0x00224B58 File Offset: 0x00222D58
		private List<ValueTuple<Religion.PaganBelief, Logic.Character>> GetBeliefUnderAdoption()
		{
			if (base.Data == null)
			{
				return null;
			}
			if (base.Data.court == null)
			{
				return null;
			}
			List<ValueTuple<Religion.PaganBelief, Logic.Character>> list = new List<ValueTuple<Religion.PaganBelief, Logic.Character>>();
			for (int i = 0; i < base.Data.court.Count; i++)
			{
				Logic.Character character = base.Data.court[i];
				if (character != null && character.IsCleric())
				{
					Action action = character.FindAction("PromotePaganBeliefAction");
					if (action != null && action.state == Action.State.Preparing)
					{
						string name = action.args[0];
						Religion.PaganBelief paganBelief = base.Data.game.religions.pagan.def.FindPaganBelief(name);
						if (paganBelief != null)
						{
							list.Add(new ValueTuple<Religion.PaganBelief, Logic.Character>(paganBelief, character));
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06004B6E RID: 19310 RVA: 0x00224C24 File Offset: 0x00222E24
		protected static PaganBeliefStatus GetBeliefStatus(Logic.Kingdom k, Religion.PaganBelief v)
		{
			if (k == null)
			{
				return null;
			}
			if (v == null)
			{
				return null;
			}
			for (int i = 0; i < k.court.Count; i++)
			{
				Logic.Character character = k.court[i];
				if (character != null)
				{
					PaganBeliefStatus paganBeliefStatus = character.statuses.Find<PaganBeliefStatus>();
					if (paganBeliefStatus != null && paganBeliefStatus.own_character.paganBelief == v)
					{
						return paganBeliefStatus;
					}
				}
			}
			return null;
		}

		// Token: 0x06004B6F RID: 19311 RVA: 0x00224C81 File Offset: 0x00222E81
		protected override void Update()
		{
			base.Update();
			this.UpdateUpkeep();
		}

		// Token: 0x06004B70 RID: 19312 RVA: 0x00224C90 File Offset: 0x00222E90
		private void UpdateUpkeep()
		{
			if (this.m_UpkeepValue == null)
			{
				return;
			}
			float num = 0f;
			for (int i = 0; i < base.Data.pagan_beliefs.Count; i++)
			{
				Religion.PaganBelief paganBelief = base.Data.pagan_beliefs[i];
				if (paganBelief != null)
				{
					PaganBeliefStatus beliefStatus = UIReligionWindow.Pagan.GetBeliefStatus(base.Data, paganBelief);
					if (beliefStatus != null)
					{
						num = beliefStatus.GetVar("current_upkeep", null, true).float_val;
						break;
					}
				}
			}
			if (num != this.m_CurrentUpkeep)
			{
				this.m_CurrentUpkeep = num;
				if (this.m_CurrentUpkeep != 0f)
				{
					this.m_UpkeepVars.Set<float>("upkeep", num);
					UIText.SetTextKey(this.m_UpkeepValue, "Religion.Pagan.upkeep_value", this.m_UpkeepVars, null);
					return;
				}
				UIText.SetText(this.m_UpkeepValue, "--");
			}
		}

		// Token: 0x06004B71 RID: 19313 RVA: 0x00224D60 File Offset: 0x00222F60
		private void UpdateLabels()
		{
			if (this.m_UpkeepLabel != null)
			{
				UIText.SetTextKey(this.m_UpkeepLabel, "Pagan.beliefs_upkeep", null, null);
			}
			if (this.m_AdoptionState != null)
			{
				Logic.Kingdom data = base.Data;
				bool flag;
				if (data == null)
				{
					flag = false;
				}
				else
				{
					List<Religion.PaganBelief> pagan_beliefs = data.pagan_beliefs;
					int? num = (pagan_beliefs != null) ? new int?(pagan_beliefs.Count) : null;
					int num2 = 0;
					flag = (num.GetValueOrDefault() == num2 & num != null);
				}
				string key = flag ? "Pagan.no_adopted_beliefs" : "Pagan.adopted_beliefs";
				UIText.SetTextKey(this.m_AdoptionState, key, base.Data, null);
			}
		}

		// Token: 0x04003A0F RID: 14863
		[UIFieldTarget("id_BeliefsContainer")]
		protected RectTransform m_BeliefContainer;

		// Token: 0x04003A10 RID: 14864
		[UIFieldTarget("id_TraditionSlotProrotype")]
		protected GameObject m_BeliefPrototype;

		// Token: 0x04003A11 RID: 14865
		[UIFieldTarget("id_AdoptionState")]
		protected TextMeshProUGUI m_AdoptionState;

		// Token: 0x04003A12 RID: 14866
		[UIFieldTarget("id_UpkeepLabel")]
		protected TextMeshProUGUI m_UpkeepLabel;

		// Token: 0x04003A13 RID: 14867
		[UIFieldTarget("id_UpkeepValue")]
		protected TextMeshProUGUI m_UpkeepValue;

		// Token: 0x04003A14 RID: 14868
		private Vars m_UpkeepVars = new Vars();

		// Token: 0x04003A15 RID: 14869
		private float m_CurrentUpkeep = -1f;

		// Token: 0x02000A11 RID: 2577
		private class BeliefIcon : MonoBehaviour
		{
			// Token: 0x06005567 RID: 21863 RVA: 0x00249612 File Offset: 0x00247812
			private void Init()
			{
				if (this.m_Initalized)
				{
					return;
				}
				UICommon.FindComponents(this, false);
				this.m_Initalized = true;
			}

			// Token: 0x06005568 RID: 21864 RVA: 0x0024962B File Offset: 0x0024782B
			public void SetData(Religion.PaganBelief belief, Logic.Character owner)
			{
				this.Init();
				this.Belief = belief;
				this.Shaman = owner;
				this.Populate();
			}

			// Token: 0x06005569 RID: 21865 RVA: 0x00249648 File Offset: 0x00247848
			private void Populate()
			{
				Logic.Character shaman = this.Shaman;
				UIReligionWindow.Pagan.GetBeliefStatus((shaman != null) ? shaman.GetKingdom() : null, this.Belief);
				if (this.m_BeliefIcon != null)
				{
					if (this.Belief != null)
					{
						this.m_BeliefIcon.overrideSprite = global::Defs.GetObj<Sprite>("Pagan", this.Belief.name + ".icon", null);
					}
					this.m_BeliefIcon.gameObject.SetActive(this.Belief != null);
				}
				if (this.m_ShamanIcon != null)
				{
					this.m_ShamanIcon.SetObject(this.Shaman, null);
					this.m_ShamanIcon.DisableTooltip(true);
					this.m_ShamanIcon.gameObject.SetActive(this.Shaman != null);
				}
				if (this.m_AdoptProgress != null)
				{
					Logic.Character shaman2 = this.Shaman;
					Action action = (shaman2 != null) ? shaman2.FindAction("PromotePaganBeliefAction") : null;
					this.m_AdoptProgress.SetData(action);
					this.m_AdoptProgress.gameObject.SetActive(action != null && action.state == Action.State.Preparing);
				}
				Vars vars;
				if (this.Belief != null)
				{
					vars = new Vars(this.Belief);
					vars.Set<Logic.Character>("owner", this.Shaman);
					vars.Set<string>("belief", "Pagan." + this.Belief.name + ".name");
					vars.Set<Logic.Kingdom>("kingdom", this.Shaman.GetKingdom());
				}
				else
				{
					vars = new Vars();
					vars.Set<bool>("empty_slot", true);
				}
				Tooltip.Get(base.gameObject, true).SetDef("PaganBeliefTooltip", vars);
			}

			// Token: 0x04004669 RID: 18025
			[UIFieldTarget("id_Shaman")]
			protected UICharacterIcon m_ShamanIcon;

			// Token: 0x0400466A RID: 18026
			[UIFieldTarget("id_BeliefIcon")]
			protected Image m_BeliefIcon;

			// Token: 0x0400466B RID: 18027
			[UIFieldTarget("id_AdoptProgress")]
			protected UIActionProgressInfo m_AdoptProgress;

			// Token: 0x0400466C RID: 18028
			private Religion.PaganBelief Belief;

			// Token: 0x0400466D RID: 18029
			private Logic.Character Shaman;

			// Token: 0x0400466E RID: 18030
			private bool m_Initalized;
		}
	}

	// Token: 0x02000766 RID: 1894
	internal class ReligionLeader : MonoBehaviour
	{
		// Token: 0x06004B73 RID: 19315 RVA: 0x00224E1B File Offset: 0x0022301B
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.vars = new Vars();
			this.m_Initialized = true;
		}

		// Token: 0x06004B74 RID: 19316 RVA: 0x00224E40 File Offset: 0x00223040
		public void SetCharacter(Logic.Character c, Logic.Kingdom k = null)
		{
			this.Init();
			this.Character = c;
			this.Kingdom = k;
			this.vars.obj = c;
			this.vars.Set<Logic.Kingdom>("kingdom", this.Kingdom);
			this.UpdateIcon();
			this.UpdateName();
			this.UpdateAge();
			this.UpdateRelations();
			this.UpdateAllegiance();
		}

		// Token: 0x06004B75 RID: 19317 RVA: 0x00224EA6 File Offset: 0x002230A6
		private void UpdateIcon()
		{
			if (this.id_LeaderIcon == null)
			{
				return;
			}
			this.id_LeaderIcon.SetObject(this.Character, null);
		}

		// Token: 0x06004B76 RID: 19318 RVA: 0x00224ECC File Offset: 0x002230CC
		private void UpdateName()
		{
			if (this.m_LeaderName == null)
			{
				return;
			}
			string key = (this.Character != null) ? "Character.title_name" : "ReligionWidnow.no_leader_label";
			UIText.SetTextKey(this.m_LeaderName, key, this.vars, null);
		}

		// Token: 0x06004B77 RID: 19319 RVA: 0x00224F10 File Offset: 0x00223110
		private void UpdateAge()
		{
			if (this.m_LeaderAge == null)
			{
				return;
			}
			string key = (this.Character != null) ? ("Character.age." + this.Character.age.ToString()) : "ReligionWidnow.no_leader_successor_label";
			UIText.SetTextKey(this.m_LeaderAge, key, this.vars, null);
		}

		// Token: 0x06004B78 RID: 19320 RVA: 0x00224F6F File Offset: 0x0022316F
		private void UpdateRelations()
		{
			if (this.m_Relations == null)
			{
				return;
			}
			UIKingdomRelations relations = this.m_Relations;
			Logic.Character character = this.Character;
			relations.SetData((character != null) ? character.GetKingdom() : null, BaseUI.LogicKingdom());
		}

		// Token: 0x06004B79 RID: 19321 RVA: 0x00224FA4 File Offset: 0x002231A4
		private void UpdateAllegiance()
		{
			Logic.Character character = this.Character;
			if (character != null && character.IsPope())
			{
				Logic.Kingdom specialCourtKingdom = this.Character.GetSpecialCourtKingdom();
				bool flag = specialCourtKingdom != null && specialCourtKingdom != this.Character.GetKingdom();
				if (this.m_PopeAllegiance != null)
				{
					string key = flag ? "Catholic.pope_influenced_by" : "Catholic.pope_not_influenced";
					UIText.SetTextKey(this.m_PopeAllegiance, key, this.vars, null);
				}
				if (this.m_PopeAllegianceIcon)
				{
					if (flag)
					{
						this.m_PopeAllegianceIcon.SetObject(this.Character.GetSpecialCourtKingdom(), null);
					}
					this.m_PopeAllegianceIcon.gameObject.SetActive(flag);
					return;
				}
			}
			else if (this.m_PopeAllegianceIcon)
			{
				this.m_PopeAllegianceIcon.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004B7A RID: 19322 RVA: 0x00225074 File Offset: 0x00223274
		private void SetAsEmpty()
		{
			if (this.id_LeaderIcon != null)
			{
				this.id_LeaderIcon.SetObject(null, null);
			}
			if (this.m_LeaderName != null)
			{
				this.m_LeaderName.text = string.Empty;
			}
			if (this.m_LeaderAge != null)
			{
				this.m_LeaderAge.text = string.Empty;
			}
			if (this.m_PopeAllegiance != null)
			{
				this.m_PopeAllegiance.text = string.Empty;
			}
			if (this.m_PopeAllegianceIcon != null)
			{
				this.m_PopeAllegianceIcon.SetObject(null, null);
			}
		}

		// Token: 0x04003A16 RID: 14870
		public Logic.Character Character;

		// Token: 0x04003A17 RID: 14871
		public Logic.Kingdom Kingdom;

		// Token: 0x04003A18 RID: 14872
		[UIFieldTarget("id_LeaderIcon")]
		protected UICharacterIcon id_LeaderIcon;

		// Token: 0x04003A19 RID: 14873
		[UIFieldTarget("id_LeaderName")]
		protected TextMeshProUGUI m_LeaderName;

		// Token: 0x04003A1A RID: 14874
		[UIFieldTarget("id_LeaderAge")]
		protected TextMeshProUGUI m_LeaderAge;

		// Token: 0x04003A1B RID: 14875
		[UIFieldTarget("id_Relations")]
		protected UIKingdomRelations m_Relations;

		// Token: 0x04003A1C RID: 14876
		[UIFieldTarget("id_PopeAllegiance")]
		protected TextMeshProUGUI m_PopeAllegiance;

		// Token: 0x04003A1D RID: 14877
		[UIFieldTarget("id_PopeAllegianceIcon")]
		protected UIKingdomIcon m_PopeAllegianceIcon;

		// Token: 0x04003A1E RID: 14878
		private Vars vars;

		// Token: 0x04003A1F RID: 14879
		private bool m_Initialized;
	}

	// Token: 0x02000767 RID: 1895
	internal class JihadsInfo : MonoBehaviour, IListener
	{
		// Token: 0x06004B7C RID: 19324 RVA: 0x00225114 File Offset: 0x00223314
		public void SetData(Logic.Kingdom k)
		{
			if (this.Data != null)
			{
				this.Data.DelListener(this);
			}
			Logic.Kingdom data = this.Data;
			bool flag;
			if (data == null)
			{
				flag = (null != null);
			}
			else
			{
				Game game = data.game;
				flag = (((game != null) ? game.religions : null) != null);
			}
			if (flag)
			{
				this.Data.game.religions.DelListener(this);
			}
			this.Data = k;
			UICommon.FindComponents(this, false);
			if (this.Data != null)
			{
				this.Data.AddListener(this);
			}
			Logic.Kingdom data2 = this.Data;
			bool flag2;
			if (data2 == null)
			{
				flag2 = (null != null);
			}
			else
			{
				Game game2 = data2.game;
				flag2 = (((game2 != null) ? game2.religions : null) != null);
			}
			if (flag2)
			{
				this.Data.game.religions.AddListener(this);
			}
			this.Refresh();
		}

		// Token: 0x06004B7D RID: 19325 RVA: 0x000023FD File Offset: 0x000005FD
		private void JoinSelectedJihad()
		{
		}

		// Token: 0x06004B7E RID: 19326 RVA: 0x002251C9 File Offset: 0x002233C9
		private void DeselectSelected()
		{
			if (this.selected == null)
			{
				return;
			}
			this.selected.ShowGlow(false);
			this.selected = null;
			this.selectedJihad = null;
		}

		// Token: 0x06004B7F RID: 19327 RVA: 0x000023FD File Offset: 0x000005FD
		private void RefreshJoinButton()
		{
		}

		// Token: 0x06004B80 RID: 19328 RVA: 0x002251F4 File Offset: 0x002233F4
		private void RefreshAudienceBtn()
		{
			if (this.m_btnAudience == null)
			{
				return;
			}
			Game game = GameLogic.Get(true);
			if (game == null)
			{
				return;
			}
			if (game.religions.jihad_kingdoms.Count > 0 && game.religions.jihad_kingdoms[0] != this.Data)
			{
				this.m_btnAudience.gameObject.SetActive(true);
				this.m_btnAudience.onClick = delegate(BSGButton b)
				{
					AudienceWindow.Create(game.religions.jihad_kingdoms[0].visuals as global::Kingdom, "Main", null);
				};
				UIText.SetTextKey(this.m_btnAudienceLabel, "ReligionWindow.muslim.Audience.Label", new Vars(game.religions.jihad_kingdoms[0]), null);
				return;
			}
			this.m_btnAudience.gameObject.SetActive(false);
		}

		// Token: 0x06004B81 RID: 19329 RVA: 0x002252CC File Offset: 0x002234CC
		public void Select(UIWarIcon icon)
		{
			if (icon == null || icon.War == null)
			{
				return;
			}
			if (this.Data.wars.Contains(icon.War))
			{
				UIWarsOverviewWindow.ToggleOpen(this.Data, icon.War, null);
			}
		}

		// Token: 0x06004B82 RID: 19330 RVA: 0x0022530C File Offset: 0x0022350C
		private void OnDestroy()
		{
			if (this.Data != null)
			{
				this.Data.DelListener(this);
			}
			Logic.Kingdom data = this.Data;
			bool flag;
			if (data == null)
			{
				flag = (null != null);
			}
			else
			{
				Game game = data.game;
				flag = (((game != null) ? game.religions : null) != null);
			}
			if (flag)
			{
				this.Data.game.religions.DelListener(this);
			}
			for (int i = 0; i < this.jihadIcons.Count; i++)
			{
				if (!(this.jihadIcons[i] == null))
				{
					UIWarIcon uiwarIcon = this.jihadIcons[i];
					global::Common.DestroyObj((uiwarIcon != null) ? uiwarIcon.gameObject : null);
				}
			}
		}

		// Token: 0x06004B83 RID: 19331 RVA: 0x002253AC File Offset: 0x002235AC
		private void Refresh()
		{
			bool flag = this.ActiveJihads();
			if (this.m_JihadsContainer != null)
			{
				this.m_JihadsContainer.gameObject.SetActive(flag);
			}
			if (this.m_Group_NoJihad != null)
			{
				this.m_Group_NoJihad.SetActive(!flag);
			}
			if (this.m_Group_ActiveJihad != null)
			{
				this.m_Group_ActiveJihad.SetActive(flag);
			}
			if (this.m_JihadLabel != null)
			{
				this.m_JihadLabel.gameObject.SetActive(true);
			}
			if (this.m_NoJihadsLabel != null)
			{
				this.m_NoJihadsLabel.gameObject.SetActive(!flag);
			}
			this.UpdateJihadTexts();
			this.RefreshAudienceBtn();
			if (!flag)
			{
				return;
			}
			this.UpdateJihads();
			this.RefreshJoinButton();
		}

		// Token: 0x06004B84 RID: 19332 RVA: 0x00225474 File Offset: 0x00223674
		protected void UpdateJihadTexts()
		{
			if (this.Data == null || !this.Data.is_muslim)
			{
				return;
			}
			base.gameObject.SetActive(true);
			this.m_LablesVars.Clear();
			this.m_LablesVars.obj = this.Data;
			List<War> allJihads = this.GetAllJihads();
			War war = (allJihads != null && allJihads.Count > 0) ? allJihads[0] : null;
			bool flag = war != null;
			if (flag)
			{
				Logic.Kingdom kingdom = war.attacker.IsCaliphate() ? war.attacker : war.defender;
				this.m_LablesVars.Set<Logic.Kingdom>("leader", kingdom);
				this.m_LablesVars.Set<Logic.Kingdom>("target", kingdom.jihad_target);
			}
			if (this.m_JihadLabel != null)
			{
				if (flag)
				{
					UIText.SetTextKey(this.m_JihadLabel, "ReligionWindow.muslim.ActiveJihads.Label", this.m_LablesVars, null);
				}
				this.m_JihadLabel.gameObject.SetActive(flag);
			}
			if (this.m_JihadLeader != null)
			{
				if (flag)
				{
					UIText.SetTextKey(this.m_JihadLeader, "ReligionWindow.muslim.ActiveJihads.Body", this.m_LablesVars, null);
				}
				this.m_JihadLeader.gameObject.SetActive(flag);
			}
			if (this.m_NoJihadsLabel != null)
			{
				if (!flag)
				{
					UIText.SetTextKey(this.m_NoJihadsLabel, "ReligionWindow.muslim.ActiveJihads.None.Description.Label", this.m_LablesVars, null);
				}
				this.m_NoJihadsLabel.gameObject.SetActive(!flag);
			}
			if (this.m_NoJihadsDescription != null)
			{
				if (!flag)
				{
					UIText.SetTextKey(this.m_NoJihadsDescription, "ReligionWindow.muslim.ActiveJihads.None.Description.Body", this.m_LablesVars, null);
				}
				this.m_NoJihadsDescription.gameObject.SetActive(!flag);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		}

		// Token: 0x06004B85 RID: 19333 RVA: 0x00225628 File Offset: 0x00223828
		protected void UpdateJihads()
		{
			List<War> allJihads = this.GetAllJihads();
			if (allJihads.Count > this.jihadIcons.Count)
			{
				int num = allJihads.Count - this.jihadIcons.Count;
				for (int i = 0; i < num; i++)
				{
					GameObject icon = ObjectIcon.GetIcon("War", null, this.m_JihadsContainer);
					UIWarIcon uiwarIcon = ((icon != null) ? icon.GetComponent<ObjectIcon>() : null) as UIWarIcon;
					if (uiwarIcon != null)
					{
						LayoutElement layoutElement = uiwarIcon.gameObject.AddComponent<LayoutElement>();
						layoutElement.preferredWidth = 30f;
						layoutElement.preferredHeight = 30f;
						this.jihadIcons.Add(uiwarIcon);
						uiwarIcon.OnSelect += this.Select;
					}
				}
			}
			for (int j = 0; j < this.jihadIcons.Count; j++)
			{
				War war = (allJihads != null && allJihads.Count > j) ? allJihads[j] : null;
				if (war != null)
				{
					this.jihadIcons[j].SetObject(war, null);
					this.jihadIcons[j].gameObject.SetActive(true);
				}
				else
				{
					this.jihadIcons[j].gameObject.SetActive(false);
				}
			}
			if (!allJihads.Contains(this.selectedJihad))
			{
				this.DeselectSelected();
			}
		}

		// Token: 0x06004B86 RID: 19334 RVA: 0x00225770 File Offset: 0x00223970
		private List<War> GetAllJihads()
		{
			List<War> list = new List<War>();
			Game game = GameLogic.Get(true);
			if (game == null)
			{
				return list;
			}
			for (int i = 0; i < game.religions.jihad_kingdoms.Count; i++)
			{
				War jihad = game.religions.jihad_kingdoms[i].jihad;
				if (jihad != null)
				{
					list.Add(jihad);
				}
			}
			return list;
		}

		// Token: 0x06004B87 RID: 19335 RVA: 0x002257CC File Offset: 0x002239CC
		public void OnMessage(object obj, string message, object param)
		{
			if (this == null)
			{
				Logic.Kingdom data = this.Data;
				if (data != null)
				{
					data.DelListener(this);
				}
				Logic.Kingdom data2 = this.Data;
				if (data2 == null)
				{
					return;
				}
				Game game = data2.game;
				if (game == null)
				{
					return;
				}
				game.religions.DelListener(this);
				return;
			}
			else
			{
				if (message == "new_jihad" || message == "end_jihad" || message == "jihad_changed")
				{
					this.Refresh();
					return;
				}
				if (!(message == "destroying") && !(message == "finishing"))
				{
					return;
				}
				Logic.Kingdom data3 = this.Data;
				if (data3 != null)
				{
					data3.DelListener(this);
				}
				Logic.Kingdom data4 = this.Data;
				if (data4 == null)
				{
					return;
				}
				Game game2 = data4.game;
				if (game2 == null)
				{
					return;
				}
				game2.religions.DelListener(this);
				return;
			}
		}

		// Token: 0x06004B88 RID: 19336 RVA: 0x00225890 File Offset: 0x00223A90
		private bool ActiveJihads()
		{
			Game game = GameLogic.Get(true);
			return ((game != null) ? game.religions : null) != null && game.religions.jihad_kingdoms.Count > 0;
		}

		// Token: 0x04003A20 RID: 14880
		[UIFieldTarget("id_JihadsContainer")]
		protected RectTransform m_JihadsContainer;

		// Token: 0x04003A21 RID: 14881
		[UIFieldTarget("id_Group_ActiveJihad")]
		protected GameObject m_Group_ActiveJihad;

		// Token: 0x04003A22 RID: 14882
		[UIFieldTarget("id_JihadLabel")]
		protected TextMeshProUGUI m_JihadLabel;

		// Token: 0x04003A23 RID: 14883
		[UIFieldTarget("id_JihadLeader")]
		protected TextMeshProUGUI m_JihadLeader;

		// Token: 0x04003A24 RID: 14884
		[UIFieldTarget("id_Group_NoJihad")]
		protected GameObject m_Group_NoJihad;

		// Token: 0x04003A25 RID: 14885
		[UIFieldTarget("id_NoJihadsLabel")]
		protected TextMeshProUGUI m_NoJihadsLabel;

		// Token: 0x04003A26 RID: 14886
		[UIFieldTarget("id_NoJihadsDescription")]
		protected TextMeshProUGUI m_NoJihadsDescription;

		// Token: 0x04003A27 RID: 14887
		[UIFieldTarget("id_btnAudience")]
		protected BSGButton m_btnAudience;

		// Token: 0x04003A28 RID: 14888
		[UIFieldTarget("id_btnAudienceLabel")]
		protected TextMeshProUGUI m_btnAudienceLabel;

		// Token: 0x04003A29 RID: 14889
		public Logic.Kingdom Data;

		// Token: 0x04003A2A RID: 14890
		private List<UIWarIcon> jihadIcons = new List<UIWarIcon>();

		// Token: 0x04003A2B RID: 14891
		private UIWarIcon selected;

		// Token: 0x04003A2C RID: 14892
		private War selectedJihad;

		// Token: 0x04003A2D RID: 14893
		private Vars m_LablesVars = new Vars();
	}

	// Token: 0x02000768 RID: 1896
	internal class RestorePapacy : MonoBehaviour
	{
		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x06004B8A RID: 19338 RVA: 0x002258E8 File Offset: 0x00223AE8
		// (set) Token: 0x06004B8B RID: 19339 RVA: 0x002258F0 File Offset: 0x00223AF0
		public Logic.Kingdom Data { get; private set; }

		// Token: 0x06004B8C RID: 19340 RVA: 0x002258F9 File Offset: 0x00223AF9
		private void Init()
		{
			if (this.m_initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_initialzied = false;
		}

		// Token: 0x06004B8D RID: 19341 RVA: 0x00225912 File Offset: 0x00223B12
		public void SetData(Logic.Kingdom k)
		{
			this.Init();
			this.Data = k;
			this.Build();
		}

		// Token: 0x06004B8E RID: 19342 RVA: 0x00225928 File Offset: 0x00223B28
		private void Build()
		{
			if (this.m_RestorePapacyAction == null || this.Data.actions == null)
			{
				return;
			}
			this.m_Action = this.Data.actions.Find("RestorePapacyAction");
			if (this.m_Action == null)
			{
				return;
			}
			Vars vars = new Vars(this.Data);
			this.m_ActionIcon = UIActionIcon.Possess(this.m_Action.visuals as ActionVisuals, this.m_RestorePapacyAction, vars);
			if (this.m_Progress != null)
			{
				this.m_Progress.SetData(this.m_Action);
			}
			if (this.m_ActionIcon != null)
			{
				this.m_ActionIcon.SetSkin("Neutral");
			}
		}

		// Token: 0x06004B8F RID: 19343 RVA: 0x002259E5 File Offset: 0x00223BE5
		private void Update()
		{
			this.CheckRestorePapacyAction();
		}

		// Token: 0x06004B90 RID: 19344 RVA: 0x002259F0 File Offset: 0x00223BF0
		private void CheckRestorePapacyAction()
		{
			if (this.Data == null)
			{
				return;
			}
			if (this.m_Action == null)
			{
				return;
			}
			bool flag = this.HasControlOverRome(this.Data);
			this.m_Visuals.gameObject.SetActive(flag);
			if (!flag)
			{
				return;
			}
			bool flag2 = this.m_Action.state > Action.State.Inactive;
			this.m_Progress.gameObject.SetActive(flag2);
			this.m_RestorePapacyAction.gameObject.SetActive(!flag2);
		}

		// Token: 0x06004B91 RID: 19345 RVA: 0x00225A65 File Offset: 0x00223C65
		private bool HasControlOverRome(Logic.Kingdom k)
		{
			return k != null && k.game.religions.catholic.hq_realm.GetKingdom() == k;
		}

		// Token: 0x04003A2E RID: 14894
		[UIFieldTarget("id_RestorePapacyAction")]
		private GameObject m_RestorePapacyAction;

		// Token: 0x04003A2F RID: 14895
		[UIFieldTarget("id_Progress")]
		private UIActionProgressInfo m_Progress;

		// Token: 0x04003A30 RID: 14896
		[UIFieldTarget("id_Visuals")]
		private GameObject m_Visuals;

		// Token: 0x04003A32 RID: 14898
		private Action m_Action;

		// Token: 0x04003A33 RID: 14899
		private UIActionIcon m_ActionIcon;

		// Token: 0x04003A34 RID: 14900
		private bool m_initialzied;
	}
}
