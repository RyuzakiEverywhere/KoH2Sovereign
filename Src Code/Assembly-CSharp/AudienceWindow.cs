using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001A8 RID: 424
public class AudienceWindow : UIWindow, IListener
{
	// Token: 0x17000141 RID: 321
	// (get) Token: 0x0600186C RID: 6252 RVA: 0x000ED7A1 File Offset: 0x000EB9A1
	// (set) Token: 0x0600186D RID: 6253 RVA: 0x000ED7A8 File Offset: 0x000EB9A8
	public static AudienceWindow current { get; private set; }

	// Token: 0x0600186E RID: 6254 RVA: 0x000ED7B0 File Offset: 0x000EB9B0
	protected override void Awake()
	{
		base.Awake();
		base.enabled = false;
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x000ED7C0 File Offset: 0x000EB9C0
	private static bool OpenPendingOffer(global::Kingdom kCurr, global::Kingdom kOther)
	{
		Offer offer;
		if (kCurr == null)
		{
			offer = null;
		}
		else
		{
			Logic.Kingdom logic = kCurr.logic;
			offer = ((logic != null) ? logic.GetOngoingOfferWith((kOther != null) ? kOther.logic : null) : null);
		}
		Offer offer2 = offer;
		if (offer2 == null)
		{
			return false;
		}
		if ((offer2.msg_icon == null || (offer2.msg_icon as MessageIcon).gameObject == null) && kCurr != null)
		{
			kCurr.OnMessage(kCurr.logic, "offer_added", offer2);
		}
		if (offer2.msg_icon == null)
		{
			return false;
		}
		MessageIcon messageIcon = offer2.msg_icon as MessageIcon;
		if (messageIcon.gameObject == null)
		{
			return false;
		}
		Offer offer3 = messageIcon.vars.Get<Offer>("offer", null);
		BSGButton offerBtn = messageIcon.GetComponent<BSGButton>();
		string text;
		if (offer3.from == kOther.logic)
		{
			text = "IncommingOfferExistingMessage";
		}
		else
		{
			text = "OutgoingOfferExistingMessage";
		}
		Vars vars = new Vars(offer3);
		vars.Set<Logic.Kingdom>("kingdom", kOther.logic);
		MessageWnd.Create(text, vars, null, delegate(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "seeIt" && offerBtn != null)
			{
				offerBtn.onClick(offerBtn);
			}
			else
			{
				wnd.Close(false);
			}
			return true;
		});
		return true;
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x000ED8C6 File Offset: 0x000EBAC6
	public override string GetDefId()
	{
		return AudienceWindow.def_id;
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x000ED8D0 File Offset: 0x000EBAD0
	public static AudienceWindow Create(global::Kingdom kOther, string menuButtonsDef, global::Kingdom kCurr = null)
	{
		if (kCurr == null)
		{
			kCurr = global::Kingdom.Get(WorldUI.Get().kingdom);
		}
		if (kCurr == null || kOther == null || kCurr == kOther)
		{
			return null;
		}
		if (kCurr.logic.IsDefeated() || kOther.logic.IsDefeated())
		{
			return null;
		}
		if (kOther.logic.type != Logic.Kingdom.Type.Regular)
		{
			return null;
		}
		if (AudienceWindow.OpenPendingOffer(kCurr, kOther))
		{
			return null;
		}
		DT.Field defField = global::Defs.GetDefField(AudienceWindow.def_id, null);
		if (defField == null)
		{
			return null;
		}
		GameObject gameObject = defField.GetValue("ui_prefab", null, true, true, true, '.').obj_val as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		GameObject gameObject2 = global::Common.FindChildByName(baseUI.gameObject, "id_MessageContainer", true, true);
		if (AudienceWindow.current == null)
		{
			GameObject gameObject3 = global::Common.Spawn(gameObject, false, false);
			if (gameObject3 == null)
			{
				return null;
			}
			gameObject3.transform.SetParent(gameObject2.transform as RectTransform, false);
			AudienceWindow.current = gameObject3.GetOrAddComponent<AudienceWindow>();
			AudienceWindow.current.Open();
			AudienceWindow.current.enabled = true;
			AudienceWindow.current.transform.SetAsLastSibling();
			UITargetSelectWindow.Instance.Show(false);
		}
		else
		{
			AudienceWindow.current.Clear();
			AudienceWindow.current.gameObject.SetActive(true);
			AudienceWindow.current.Show(true);
		}
		AudienceWindow.current.Init();
		AudienceWindow.current.kCurrent = kCurr;
		AudienceWindow.current.kOther = kOther;
		AudienceWindow.current.def_field = defField;
		AudienceWindow.current.UpdateSovereign();
		AudienceWindow.current.ActivateScrollableContent(false);
		if (AudienceWindow.current.shield != null)
		{
			AudienceWindow.current.shield.SetObject((kOther == null) ? null : kOther.logic, null);
		}
		if (AudienceWindow.current.relHistory == null)
		{
			AudienceWindow.current.relHistory = RelationLatestHistory.Create("Horizontal", global::Common.FindChildByName(AudienceWindow.current.gameObject, "RelationsHistory", true, true), kOther.logic, kCurr.logic);
		}
		else
		{
			AudienceWindow.current.relHistory.SetRelation(kOther.logic, kCurr.logic, true);
		}
		AudienceWindow.current.InitRelationBar();
		AudienceWindow.current.ChangeMenuButtons(menuButtonsDef, null, null, null, false);
		if (kCurr != null)
		{
			kCurr.logic.AddListener(AudienceWindow.current);
		}
		if (kOther != null)
		{
			kOther.logic.AddListener(AudienceWindow.current);
		}
		if (kOther != null)
		{
			Logic.Kingdom logic = kOther.logic;
			if (logic != null)
			{
				Logic.RoyalFamily royalFamily = logic.royalFamily;
				if (royalFamily != null)
				{
					royalFamily.AddListener(AudienceWindow.current);
				}
			}
		}
		if (kOther != null)
		{
			Logic.Kingdom logic2 = kOther.logic;
			if (logic2 != null)
			{
				Game game = logic2.game;
				if (game != null)
				{
					game.religions.AddListener(AudienceWindow.current);
				}
			}
		}
		kCurr.logic.inAudienceWith = kOther.logic;
		kOther.logic.inAudienceWith = kCurr.logic;
		return AudienceWindow.current;
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x000EDBB8 File Offset: 0x000EBDB8
	public static void BringToFront()
	{
		if (AudienceWindow.current == null)
		{
			return;
		}
		AudienceWindow.current.transform.SetAsLastSibling();
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x000EDBD8 File Offset: 0x000EBDD8
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		this.closeButton = global::Common.FindChildComponent<BSGButton>(base.gameObject, "id_Close_Button");
		this.closeButton.onClick = delegate(BSGButton b)
		{
			this.Hide(false);
		};
		this.closeButton.SetAudioSet("DefaultAudioSetPaper");
		this.def_field = global::Defs.GetDefField(AudienceWindow.def_id, null);
		this.scrollableContent = global::Common.FindChildByName(base.gameObject, "id_ScrollableContent", true, true);
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Scrollable", true, true);
		this.scrollableScrollRect = ((gameObject != null) ? gameObject.GetComponent<ScrollRect>() : null);
		this.scrollableRowsContainer = global::Common.FindChildByName(base.gameObject, "id_ScrollableRowsContainer", true, true);
		GameObject gameObject2 = global::Common.FindChildByName(base.gameObject, "id_ScrollableTitle", true, true);
		this.scrollableTitle = ((gameObject2 != null) ? gameObject2.GetComponent<TextMeshProUGUI>() : null);
		this.message = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_MessageText");
		this.shield = global::Common.FindChildComponent<UIKingdomIcon>(base.gameObject, "id_Shield");
		this.sovereign = global::Common.FindChildComponent<UICharacterIcon>(base.gameObject, "id_IconKing");
		if (this.sovereign != null)
		{
			this.sovereign.OnSelect += delegate(UICharacterIcon b)
			{
			};
			this.sovereign.ShowCrest(false);
			this.sovereign.ShowCrown(false);
			this.sovereign.ShowStatus(false);
			this.sovereign.ShowArmyBanner(false);
			this.sovereign.ShowPrisonKingdomCrest(true);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x000EDD74 File Offset: 0x000EBF74
	private void UpdateSovereign()
	{
		Logic.Character character = this.kOther.logic.royalFamily.Sovereign;
		this.sovereign = global::Common.FindChildComponent<UICharacterIcon>(base.gameObject, "id_IconKing");
		if (this.sovereign != null)
		{
			this.sovereign.SetObject(character, null);
		}
		Vars vars = new Vars(character);
		UIText.SetText(base.gameObject, "id_KingName", this.def_field, "kingName", vars, null);
		UIText.SetText(base.gameObject, "id_KingAge", this.def_field, "kingAge", vars, null);
		UIText.SetText(base.gameObject, "id_KingTitle", this.def_field, "kingOf", vars, null);
		UIText.SetText(base.gameObject, "id_KingdomName", this.def_field, "kingdomOf", vars, null);
		UIText.SetText(base.gameObject, "id_Caption", this.def_field, "audience", vars, null);
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x000EDE65 File Offset: 0x000EC065
	public override bool OnBackInputAction()
	{
		this.Hide(false);
		return true;
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x000EDE6F File Offset: 0x000EC06F
	public override void Hide(bool silent = false)
	{
		this.Clear();
		base.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x000EDE8A File Offset: 0x000EC08A
	public override void Close(bool silent = false)
	{
		this.Clear();
		base.Close(silent);
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x000EDE9C File Offset: 0x000EC09C
	private void Clear()
	{
		this.ClearTargetSelector(false);
		global::Kingdom kingdom = this.kCurrent;
		if (((kingdom != null) ? kingdom.logic : null) != null)
		{
			this.kCurrent.logic.inAudienceWith = null;
		}
		global::Kingdom kingdom2 = this.kCurrent;
		if (((kingdom2 != null) ? kingdom2.logic : null) != null)
		{
			this.kOther.logic.inAudienceWith = null;
		}
		global::Kingdom kingdom3 = this.kCurrent;
		if (kingdom3 != null)
		{
			Logic.Kingdom logic = kingdom3.logic;
			if (logic != null)
			{
				logic.DelListener(this);
			}
		}
		global::Kingdom kingdom4 = this.kOther;
		if (kingdom4 != null)
		{
			Logic.Kingdom logic2 = kingdom4.logic;
			if (logic2 != null)
			{
				logic2.DelListener(this);
			}
		}
		global::Kingdom kingdom5 = this.kOther;
		if (kingdom5 != null)
		{
			Logic.Kingdom logic3 = kingdom5.logic;
			if (logic3 != null)
			{
				Logic.RoyalFamily royalFamily = logic3.royalFamily;
				if (royalFamily != null)
				{
					royalFamily.DelListener(this);
				}
			}
		}
		global::Kingdom kingdom6 = this.kOther;
		if (kingdom6 != null)
		{
			Logic.Kingdom logic4 = kingdom6.logic;
			if (logic4 != null)
			{
				Game game = logic4.game;
				if (game != null)
				{
					game.religions.DelListener(this);
				}
			}
		}
		this.ActivateScrollableContent(false);
		this.kCurrent = null;
		this.kOther = null;
		this.currentPickerOffer = null;
		this.currentPickerActionSelect = null;
		this.currentPickerActionCancel = null;
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x000EDFB3 File Offset: 0x000EC1B3
	protected override void OnDestroy()
	{
		AudienceWindow.current = null;
		base.OnDestroy();
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x000EDFC1 File Offset: 0x000EC1C1
	public void ActivateScrollableContent(bool enabled)
	{
		this.scrollableContent.SetActive(enabled);
		if (enabled && this.scrollableScrollRect != null)
		{
			this.scrollableScrollRect.verticalNormalizedPosition = 1f;
		}
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x000EDFF0 File Offset: 0x000EC1F0
	public new void Update()
	{
		if (this.kCurrent.logic != BaseUI.LogicKingdom())
		{
			AudienceWindow.Create(this.kOther, "Main", null);
			return;
		}
		this.elapsed_time += UnityEngine.Time.unscaledDeltaTime;
		if (this.elapsed_time > this.delayedUpdateTime)
		{
			if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Low, "PC_War debug tooltip", true))
			{
				ProsAndCons prosAndCons = ProsAndCons.Get("PC_War", this.kOther.logic, this.kCurrent.logic);
				if (prosAndCons != null)
				{
					prosAndCons.Calc(false);
				}
				string text = "";
				if (prosAndCons != null)
				{
					text = prosAndCons.Dump();
					if (text != "")
					{
						text = global::Defs.ReplaceVars("#{align:left}" + text.Substring(1), null, true, '\0');
					}
				}
				string txt = text;
				UICharacterIcon uicharacterIcon = this.sovereign;
				Tooltip tooltip = Tooltip.Get((uicharacterIcon != null) ? uicharacterIcon.gameObject : null, true);
				tooltip.def = null;
				tooltip.SetText(txt, null, null);
			}
			this.elapsed_time -= this.delayedUpdateTime;
			this.relHistory.Refresh(new RelationLatestHistory.DelegateEvent(AudienceWindow.HoverRelationHistory), new RelationLatestHistory.DelegateClick(AudienceWindow.SelectRelationHistory), new List<object>
			{
				this
			});
			if (this.currentMenuName == "SendOfferLagDelay")
			{
				this.lagDelayTimes++;
				if (this.lagDelayTimes >= 4)
				{
					this.ChangeMenuButtons("SendOffer", this.currentMenuParameters, this.currentMenuEventType, this.currentMenuEventName, false);
				}
			}
			else
			{
				this.ChangeMenuButtons(this.currentMenuName, this.currentMenuParameters, this.currentMenuEventType, this.currentMenuEventName, true);
				this.lagDelayTimes = 0;
			}
			if (this.currentPickerOffer != null)
			{
				int num = -1;
				for (int i = 0; i < this.currentPickerOffer.def.max_args; i++)
				{
					if (this.currentPickerOffer.GetArg(i).obj_val == null)
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					this.ShowTargetPicker(this.currentPickerOffer, this.currentPickerActionSelect, this.currentPickerActionCancel, true);
				}
			}
		}
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x000EE208 File Offset: 0x000EC408
	private bool CheckValidSelection()
	{
		return this.kOther != null && this.kCurrent != null && (this.currentPickerOffer == null || this.currentPickerOffer.to as Logic.Kingdom == this.kOther.logic);
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x000EE244 File Offset: 0x000EC444
	private bool HasOpenTargetSelector()
	{
		return this.currentPickerActionSelect != null;
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x000EE24F File Offset: 0x000EC44F
	private void ClearTargetSelector(bool clearCallbacks = false)
	{
		this.currentPickerOffer = null;
		this.currentPickerActionSelect = null;
		this.currentPickerActionCancel = null;
		if (clearCallbacks)
		{
			UITargetSelectWindow.Instance.ClearValidateCallbacks();
		}
		UITargetSelectWindow.Instance.Show(false);
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x000EE280 File Offset: 0x000EC480
	private void BeautifyButtonText(BSGButton btn, AudienceWindow.BeautyfyTextFlags flags = AudienceWindow.BeautyfyTextFlags.None)
	{
		if (btn == null)
		{
			return;
		}
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(btn.gameObject, "id_Text");
		if (textMeshProUGUI != null)
		{
			string text = Regex.Replace(textMeshProUGUI.text, "<[^>]*color[^>]*>", "");
			if (btn.IsEnabled())
			{
				flags |= AudienceWindow.BeautyfyTextFlags.Active;
			}
			if (btn.IsSelected())
			{
				flags |= AudienceWindow.BeautyfyTextFlags.Selected;
			}
			UIText.SetText(textMeshProUGUI, this.BeautifyText(text, flags));
		}
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x000EE2F0 File Offset: 0x000EC4F0
	private string BeautifyText(string text, AudienceWindow.BeautyfyTextFlags flags = AudienceWindow.BeautyfyTextFlags.Active)
	{
		if (text == null || text.Length == 0)
		{
			return "";
		}
		if ((flags & AudienceWindow.BeautyfyTextFlags.Active) == AudienceWindow.BeautyfyTextFlags.None)
		{
			return "<color=#666666>" + text + "</color>";
		}
		if ((flags & AudienceWindow.BeautyfyTextFlags.HighLighted) != AudienceWindow.BeautyfyTextFlags.None)
		{
			return "<color=#93270e>" + text + "</color>";
		}
		if ((flags & AudienceWindow.BeautyfyTextFlags.Invalid) != AudienceWindow.BeautyfyTextFlags.None)
		{
			return "<color=#865757>" + text + "</color>";
		}
		if ((flags & AudienceWindow.BeautyfyTextFlags.Selected) != AudienceWindow.BeautyfyTextFlags.None)
		{
			return "<color=#9c630e>" + text + "</color>";
		}
		if ((flags & AudienceWindow.BeautyfyTextFlags.AcceptOnly) != AudienceWindow.BeautyfyTextFlags.None)
		{
			return "<color=#283F68>" + text + "</color>";
		}
		if (text[0] == '<')
		{
			return text;
		}
		return "<color=#93270e>" + text.Substring(0, 1) + "</color>" + text.Substring(1);
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x000EE3A8 File Offset: 0x000EC5A8
	private void InitRelationBar()
	{
		this.relations = global::Common.FindChildComponent<UIKingdomRelations>(base.gameObject, "id_Relations");
		if (this.relations != null)
		{
			this.relations.SetData(this.kCurrent.logic, this.kOther.logic);
		}
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x000EE3FC File Offset: 0x000EC5FC
	public List<Logic.Object> ToObjectList(List<Value> targets)
	{
		List<Logic.Object> list = new List<Logic.Object>();
		foreach (Value value in targets)
		{
			if (value.obj_val is Logic.Object)
			{
				list.Add(value.obj_val as Logic.Object);
			}
		}
		return list;
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x000EE468 File Offset: 0x000EC668
	public void ShowTargetPicker(Offer offer, Action<Value> targetPicked, Action targetPickCancelled, bool refresh = false)
	{
		if (offer == null)
		{
			return;
		}
		this.currentPickerOffer = offer;
		this.currentPickerActionSelect = targetPicked;
		this.currentPickerActionCancel = targetPickCancelled;
		targetPickCancelled = (Action)Delegate.Combine(targetPickCancelled, new Action(delegate()
		{
			this.currentPickerOffer = null;
		}));
		int num = -1;
		for (int i = 0; i < offer.def.max_args; i++)
		{
			if (offer.GetArg(i).obj_val == null)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			offer.GetPossibleArgValues(num, Offer.tmp_values);
			Vars vars = new Vars();
			vars.Set<Logic.Kingdom>("owner", this.kCurrent.logic);
			vars.Set<Logic.Kingdom>("target", this.kOther.logic);
			if (refresh)
			{
				bool flag = false;
				List<UITargetItem> items = UITargetSelectWindow.Instance.GetItems();
				if (Offer.tmp_values.Count != items.Count)
				{
					flag = true;
				}
				if (!flag)
				{
					for (int j = 0; j < items.Count; j++)
					{
						bool flag2 = false;
						for (int k = 0; k < Offer.tmp_values.Count; k++)
						{
							if (items[j].Data.Target.obj_val == Offer.tmp_values[k].obj_val as Logic.Object)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					UITargetSelectWindow.Instance.SetData(TargetPickerData.Create(this.ToObjectList(Offer.tmp_values), vars, null), Value.Null, targetPicked, targetPickCancelled, null, "TargetSelect", "");
					return;
				}
			}
			else
			{
				List<TargetPickerData> eligableTargets = TargetPickerData.Create(this.ToObjectList(Offer.tmp_values), vars, null);
				Value @null = Value.Null;
				Action cancel = targetPickCancelled;
				Vars additionalData = null;
				Func<bool> validate = null;
				Func<Value, bool> validateOkButtton = null;
				Action<BSGButton> setupOkButtton = null;
				string str;
				if (offer == null)
				{
					str = null;
				}
				else
				{
					Offer.Def def = offer.def;
					str = ((def != null) ? def.field.key : null);
				}
				if (!UITargetSelectWindow.ShowDialog(eligableTargets, @null, targetPicked, cancel, additionalData, validate, validateOkButtton, setupOkButtton, str + "TargetPicker"))
				{
					targetPickCancelled();
				}
			}
		}
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x000EE63C File Offset: 0x000EC83C
	private void AddTributeButton(Offer tributeOffer, GameObject parent, global::Kingdom sourceKingdom, int index, bool functionalRefresh)
	{
		Offer offer = tributeOffer.args[index].obj_val as Offer;
		if (this.tributeButtonPrefab == null)
		{
			this.tributeButtonPrefab = (this.def_field.GetValue("tributeButtonPrefab", null, true, true, true, '.').obj_val as GameObject);
		}
		if (this.tributeButtonPrefab == null)
		{
			return;
		}
		GameObject gameObject;
		if (functionalRefresh)
		{
			if (index >= parent.transform.childCount)
			{
				return;
			}
			gameObject = parent.transform.GetChild(index).gameObject;
		}
		else
		{
			gameObject = global::Common.Spawn(this.tributeButtonPrefab, parent.transform, false, "");
			gameObject.name = offer.def.field.type;
		}
		if (gameObject == null)
		{
			return;
		}
		AudienceWindowTributeBtn component = gameObject.GetComponent<AudienceWindowTributeBtn>();
		if (component != null)
		{
			component.SetOffer(offer);
			component.SetSelected(this.currentTributeMenuIndex == index);
			component.onClick = delegate(BSGButton BSGbtn)
			{
				this.SelectTributeSlot(BSGbtn, sourceKingdom, index, tributeOffer);
			};
			component.onReset = delegate(BSGButton BSGbtn)
			{
				tributeOffer.SetArg(index, new Value(Offer.Create("EmptyOffer", this.kCurrent.logic, this.kOther.logic, tributeOffer, Array.Empty<Value>())));
				this.ShowTribute(tributeOffer, sourceKingdom, true);
			};
		}
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x000EE788 File Offset: 0x000EC988
	private void ShowTribute(Offer tributeOffer, global::Kingdom sourceKingdom, bool functionalRefresh)
	{
		if (this.scrollableRowsContainer == null)
		{
			return;
		}
		if (!functionalRefresh)
		{
			UICommon.DeleteChildren(this.scrollableRowsContainer.transform);
			if (tributeOffer is PeaceDemandTribute)
			{
				UIText.SetText(this.scrollableTitle, this.def_field, "ourDemandTribute", null, null);
			}
			else if (tributeOffer is PeaceOfferTribute)
			{
				UIText.SetText(this.scrollableTitle, this.def_field, "ourOfferTribute", null, null);
			}
		}
		for (int i = 0; i < tributeOffer.args.Count; i++)
		{
			this.AddTributeButton(tributeOffer, this.scrollableRowsContainer, sourceKingdom, i, functionalRefresh);
		}
		this.ActivateScrollableContent(true);
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x000EE828 File Offset: 0x000ECA28
	private void SetupCannotDeclareWarTooltip(BSGButton btn, Logic.Kingdom src_kingdom, Logic.Kingdom tgt_kingdom)
	{
		List<Logic.Kingdom> list = new List<Logic.Kingdom>();
		for (int i = 0; i < src_kingdom.wars.Count; i++)
		{
			War war = src_kingdom.wars[i];
			if (war.IsAlly(src_kingdom, tgt_kingdom))
			{
				Logic.Kingdom enemyLeader = war.GetEnemyLeader(src_kingdom);
				list.Add(enemyLeader);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<Logic.Kingdom>("src_kingdom", src_kingdom);
		vars.Set<Logic.Kingdom>("tgt_kingdom", tgt_kingdom);
		vars.Set<List<Logic.Kingdom>>("allied_against", list);
		Tooltip.Get(btn.gameObject, true).SetDef("CannotDeclareWarOnAllyTooltip", vars);
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x000EE8C4 File Offset: 0x000ECAC4
	private void DecideMenuButtonFunctionality(GameObject btnObj, string text, string cameFrom, List<object> parameters, int index, bool refresh)
	{
		AudienceWindow.<>c__DisplayClass59_0 CS$<>8__locals1 = new AudienceWindow.<>c__DisplayClass59_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.parameters = parameters;
		CS$<>8__locals1.index = index;
		BSGButton component = btnObj.GetComponent<BSGButton>();
		if (component == null)
		{
			return;
		}
		component.SetAudioSet("DefaultAudioSetPaper");
		if (refresh)
		{
			component.Enable(true, false);
		}
		CS$<>8__locals1.textFlags = AudienceWindow.BeautyfyTextFlags.None;
		CS$<>8__locals1.destroyButton = false;
		CS$<>8__locals1.onCancelMenuName = cameFrom;
		CS$<>8__locals1.offer = null;
		string text2 = null;
		if (!CS$<>8__locals1.onCancelMenuName.Contains("Continue"))
		{
			CS$<>8__locals1.onCancelMenuName = "Continue" + CS$<>8__locals1.onCancelMenuName;
		}
		if (component.name == "Offer")
		{
			if (!this.kCurrent.logic.IsEnemy(this.kOther.logic))
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		else if (component.name == "Demand")
		{
			if (!this.kCurrent.logic.IsEnemy(this.kOther.logic))
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		else if (component.name == "CancelOffer")
		{
			component.onClick = delegate(BSGButton BSGbtn)
			{
				Offer outgoingOfferTo = CS$<>8__locals1.<>4__this.kCurrent.logic.GetOutgoingOfferTo(CS$<>8__locals1.<>4__this.kOther.logic);
				if (outgoingOfferTo != null)
				{
					outgoingOfferTo.Cancel();
				}
				CS$<>8__locals1.<>4__this.onClickChangeMenu("ContinueMain", null, null, null);
			};
		}
		else if (component.name == "OfferGold")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferGold", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			component.onClick = delegate(BSGButton BSGbtn)
			{
				CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, new List<object>
				{
					new OfferGold(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, 0)
				}, null, null);
			};
			if (text2 != "ok")
			{
				CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.Invalid;
				text2 = null;
			}
		}
		else if (component.name.Contains("OfferGoldComplete"))
		{
			component.name = "OfferGoldComplete";
			CS$<>8__locals1.offer = (CS$<>8__locals1.parameters[0] as Offer);
			CS$<>8__locals1.offer.GetPossibleArgValues(0, Offer.tmp_values);
			int s_level = Offer.tmp_values[CS$<>8__locals1.index].int_val;
			CS$<>8__locals1.offer.SetArg(0, s_level);
			text = (CS$<>8__locals1.offer as OfferGold).GetGoldAmount().ToString();
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGBtn)
				{
					CS$<>8__locals1.offer.SetArg(0, s_level);
					CS$<>8__locals1.<>4__this.onClickSendOffer(CS$<>8__locals1.offer);
				};
			}
		}
		else if (component.name == "DemandGold")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandGold", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			component.onClick = delegate(BSGButton BSGbtn)
			{
				CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, new List<object>
				{
					new DemandGold(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, 0)
				}, null, null);
			};
			if (text2 != "ok")
			{
				CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.Invalid;
				text2 = null;
			}
		}
		else if (component.name.Contains("DemandGoldComplete"))
		{
			component.name = "DemandGoldComplete";
			CS$<>8__locals1.offer = (CS$<>8__locals1.parameters[0] as Offer);
			CS$<>8__locals1.offer.GetPossibleArgValues(0, Offer.tmp_values);
			int s_level = Offer.tmp_values[CS$<>8__locals1.index].int_val;
			CS$<>8__locals1.offer.SetArg(0, s_level);
			text = (CS$<>8__locals1.offer as DemandGold).GetGoldAmount().ToString();
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGBtn)
				{
					CS$<>8__locals1.offer.SetArg(0, s_level);
					CS$<>8__locals1.<>4__this.onClickSendOffer(CS$<>8__locals1.offer);
				};
			}
		}
		else if (component.name == "OfferRealm")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferLand", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					OfferLand newoffer = new OfferLand(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newoffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newoffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__43) == null)
					{
						action = (CS$<>8__locals1.<>9__43 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newoffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "OfferSupportInWar")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferSupportInWar", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					OfferSupportInWar newoffer = new OfferSupportInWar(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newoffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newoffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__45) == null)
					{
						action = (CS$<>8__locals1.<>9__45 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newoffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "DemandRealm")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandLand", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					DemandLand newOffer = new DemandLand(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__47) == null)
					{
						action = (CS$<>8__locals1.<>9__47 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "DemandAttackKingdom")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandAttackKingdom", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					DemandAttackKingdom newOffer = new DemandAttackKingdom(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__49) == null)
					{
						action = (CS$<>8__locals1.<>9__49 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "DemandReleasePrisoners")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandReleasePrisoners", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new DemandReleasePrisoners(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic));
				};
			}
		}
		else if (component.name == "OfferMarriage")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("MarriageOffer", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					AudienceWindow.<>c__DisplayClass59_7 CS$<>8__locals19 = new AudienceWindow.<>c__DisplayClass59_7();
					CS$<>8__locals19.CS$<>8__locals7 = CS$<>8__locals1;
					CS$<>8__locals19.newOffer = new MarriageOffer(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					AudienceWindow.<>c__DisplayClass59_7 CS$<>8__locals20 = CS$<>8__locals19;
					Action cancelSelect;
					if ((cancelSelect = CS$<>8__locals1.<>9__50) == null)
					{
						cancelSelect = (CS$<>8__locals1.<>9__50 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					CS$<>8__locals20.cancelSelect = cancelSelect;
					Action<Value> targetPicked = delegate(Value o)
					{
						CS$<>8__locals19.newOffer.SetArg(0, o);
						CS$<>8__locals19.CS$<>8__locals7.<>4__this.onClickChangeMenu("OfferMarriageSecondSelect", new List<object>
						{
							CS$<>8__locals19.newOffer
						}, null, null);
						Action<Value> action;
						if ((action = CS$<>8__locals19.<>9__52) == null)
						{
							action = (CS$<>8__locals19.<>9__52 = delegate(Value secO)
							{
								CS$<>8__locals19.newOffer.SetArg(1, secO);
								CS$<>8__locals19.CS$<>8__locals7.<>4__this.onClickSendOffer(CS$<>8__locals19.newOffer);
							});
						}
						Action<Value> targetPicked2 = action;
						CS$<>8__locals19.CS$<>8__locals7.<>4__this.ShowTargetPicker(CS$<>8__locals19.newOffer, targetPicked2, CS$<>8__locals19.cancelSelect, false);
					};
					CS$<>8__locals1.<>4__this.ShowTargetPicker(CS$<>8__locals19.newOffer, targetPicked, CS$<>8__locals19.cancelSelect, false);
				};
			}
		}
		else if (component.name == "DemandJoinInDefensivePact")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandJoinInDefensivePact", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					DemandJoinInDefensivePact newOffer = new DemandJoinInDefensivePact(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__54) == null)
					{
						action = (CS$<>8__locals1.<>9__54 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "OfferJoinInDefensivePact")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferJoinInDefensivePact", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					OfferJoinInDefensivePact newOffer = new OfferJoinInDefensivePact(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__56) == null)
					{
						action = (CS$<>8__locals1.<>9__56 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "DemandJoinInOffensivePact")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandJoinInOffensivePact", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					DemandJoinInOffensivePact newOffer = new DemandJoinInOffensivePact(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__58) == null)
					{
						action = (CS$<>8__locals1.<>9__58 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "DeclareWar")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DeclareWar", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.AcceptOnly;
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, new List<object>
					{
						new DeclareWar(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic)
					}, null, null);
				};
			}
		}
		else if (component.name == "ConfirmDeclareWar")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DeclareWar", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new DeclareWar(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic));
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		else if (component.name == "Dynasty")
		{
			if (this.kCurrent.logic.GetRoyalMarriage(this.kOther.logic))
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		else if (component.name == "AskForLoan")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("AskForLoan", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			component.onClick = delegate(BSGButton BSGbtn)
			{
				CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, new List<object>
				{
					new AskForLoan(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, 0)
				}, null, null);
			};
			if (text2 != "ok")
			{
				CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.Invalid;
				text2 = null;
			}
		}
		else if (component.name.Contains("AskForLoanComplete"))
		{
			component.name = "AskForLoanComplete";
			CS$<>8__locals1.offer = (CS$<>8__locals1.parameters[0] as AskForLoan);
			CS$<>8__locals1.offer.GetPossibleArgValues(0, Offer.tmp_values);
			int amount = Offer.tmp_values[CS$<>8__locals1.index];
			text = amount.ToString();
			CS$<>8__locals1.offer.SetArg(0, amount);
			if (CS$<>8__locals1.offer.Validate() == "ok")
			{
				component.onClick = delegate(BSGButton BSGBtn)
				{
					CS$<>8__locals1.offer.SetArg(0, amount);
					CS$<>8__locals1.<>4__this.onClickSendOffer(CS$<>8__locals1.offer);
				};
			}
		}
		else if (component.name == "RepayLoan")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("PayLoan", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			component.onClick = delegate(BSGButton BSGbtn)
			{
				CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, new List<object>
				{
					new PayLoan(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, 0)
				}, null, null);
			};
			if (text2 != "ok")
			{
				CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.Invalid;
				text2 = null;
			}
		}
		else if (component.name.Contains("RepayLoanComplete"))
		{
			component.name = "RepayLoan";
			CS$<>8__locals1.offer = (CS$<>8__locals1.parameters[0] as PayLoan);
			CS$<>8__locals1.offer.GetPossibleArgValues(0, Offer.tmp_values);
			int amount = Offer.tmp_values[CS$<>8__locals1.index];
			text = amount.ToString();
			CS$<>8__locals1.offer.SetArg(0, amount);
			if (CS$<>8__locals1.offer.Validate() == "ok")
			{
				component.onClick = delegate(BSGButton BSGBtn)
				{
					CS$<>8__locals1.offer.SetArg(0, amount);
					CS$<>8__locals1.<>4__this.onClickSendOffer(CS$<>8__locals1.offer);
				};
			}
		}
		else if (component.name == "DemandHelpWithRebels")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandHelpWithRebels", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new DemandHelpWithRebels(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic));
				};
			}
		}
		else if (component.name == "BreakRoyalTies")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("BreakRoyalTies", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.AcceptOnly;
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new BreakRoyalTies(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic));
				};
			}
		}
		else if (component.name == "Alliance")
		{
			CS$<>8__locals1.destroyButton = true;
		}
		else if (component.name == "Lordship")
		{
			if (this.kCurrent.logic.sovereignState == this.kOther.logic || this.kOther.logic.sovereignState == this.kCurrent.logic)
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		else if (component.name == "InvokeAlliance")
		{
			CS$<>8__locals1.destroyButton = true;
		}
		else if (component.name == "AskForProtection")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("AskForProtection", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					AskForProtection newOffer = new AskForProtection(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__62) == null)
					{
						action = (CS$<>8__locals1.<>9__62 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "DemandSupportInWar")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandSupportInWar", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					DemandSupportInWar newOffer = new DemandSupportInWar(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
					};
					Action action;
					if ((action = CS$<>8__locals1.<>9__64) == null)
					{
						action = (CS$<>8__locals1.<>9__64 = delegate()
						{
							CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "JoinVassal")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("JoinVassalOffer", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new JoinVassalOffer(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic));
				};
			}
		}
		else if (component.name == "GrantIndependence")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("GrantIndependence", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new GrantIndependence(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic));
				};
			}
		}
		else if (component.name == "ClaimIndependence")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("ClaimIndependence", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new ClaimIndependence(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic));
				};
			}
		}
		else if (component.name == "SignPeace" || component.name == "SignSeparatePeace")
		{
			War war = this.kCurrent.logic.FindWarWith(this.kOther.logic);
			bool flag = war != null && (!war.IsLeader(this.kCurrent.logic) || !war.IsLeader(this.kOther.logic));
			if (component.name == "SignPeace")
			{
				if (flag)
				{
					CS$<>8__locals1.destroyButton = true;
				}
			}
			else if (!flag)
			{
				CS$<>8__locals1.destroyButton = true;
			}
			CS$<>8__locals1.offer = Offer.GetCachedOffer("WhitePeaceOffer", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
				};
			}
		}
		else if (component.name == "DemandTribute")
		{
			if (War.CanStop(this.kCurrent.logic, this.kOther.logic))
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, new List<object>
					{
						new PeaceDemandTribute(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, Array.Empty<Value>()),
						CS$<>8__locals1.<>4__this.kOther
					}, null, null);
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		else if (component.name == "DemandWhitePeace")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("WhitePeaceOffer", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new WhitePeaceOffer(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic));
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		else if (component.name == "OfferTribute")
		{
			if (War.CanStop(this.kCurrent.logic, this.kOther.logic))
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, new List<object>
					{
						new PeaceOfferTribute(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, Array.Empty<Value>()),
						CS$<>8__locals1.<>4__this.kCurrent
					}, null, null);
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		else if (component.name == "DemandOfferTribute")
		{
			component.onClick = delegate(BSGButton BSGbtn)
			{
				CS$<>8__locals1.<>4__this.ClearTargetSelector(false);
				CS$<>8__locals1.<>4__this.ChangeMenuButtons((CS$<>8__locals1.parameters[0] as global::Kingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
				{
					CS$<>8__locals1.parameters[0],
					CS$<>8__locals1.parameters[1]
				}, null, null, false);
			};
		}
		else if (component.name == "ProposeTribute")
		{
			Offer offer = CS$<>8__locals1.parameters[0] as Offer;
			this.currentTributeMenuIndex = -1;
			this.ShowTribute(offer, CS$<>8__locals1.parameters[1] as global::Kingdom, refresh);
			if (offer.Validate() == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(CS$<>8__locals1.parameters[0] as Offer);
				};
			}
			else
			{
				component.Enable(false, false);
			}
		}
		else if (component.name == "TributeGold")
		{
			Offer offer2 = CS$<>8__locals1.parameters[0] as Offer;
			if (CS$<>8__locals1.parameters[1] as global::Kingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferGold", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandGold", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			int num = (int)CS$<>8__locals1.parameters[2];
			Value value = offer2.args[num];
			CS$<>8__locals1.offer.SetParent(offer2);
			offer2.SetArg(num, CS$<>8__locals1.offer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			offer2.SetArg(num, value);
			component.onClick = delegate(BSGButton BSGbtn)
			{
				CS$<>8__locals1.<>4__this.ChangeMenuButtons(BSGbtn.name, CS$<>8__locals1.parameters, null, null, false);
			};
			if (text2 != "ok")
			{
				CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.Invalid;
				text2 = null;
			}
		}
		else if (component.name.Contains("TributeGoldComplete"))
		{
			component.name = "TributeGoldComplete";
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferGold", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandGold", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			CS$<>8__locals1.offer.GetPossibleArgValues(0, Offer.tmp_values);
			int s_level = Offer.tmp_values[CS$<>8__locals1.index];
			CS$<>8__locals1.offer.SetArg(0, s_level);
			text = (CS$<>8__locals1.offer as DemandGold).GetGoldAmount().ToString();
			Value value2 = tributeOffer.args[tributeIndex];
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			tributeOffer.SetArg(tributeIndex, CS$<>8__locals1.offer);
			text2 = CS$<>8__locals1.offer.Validate();
			tributeOffer.SetArg(tributeIndex, value2);
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGBtn)
				{
					Offer offer4;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						offer4 = new OfferGold(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, 0);
					}
					else
					{
						offer4 = new DemandGold(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, 0);
					}
					offer4.SetParent(tributeOffer);
					offer4.SetArg(0, s_level);
					tributeOffer.SetArg(tributeIndex, new Value(offer4));
					CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
					{
						tributeOffer,
						sourceKingdom
					}, null, null, false);
				};
			}
		}
		else if (component.name == "TributeLand")
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			CS$<>8__locals1.offer = null;
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferLand", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandLand", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			Value value3 = tributeOffer.args[tributeIndex];
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			tributeOffer.SetArg(tributeIndex, CS$<>8__locals1.offer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			tributeOffer.SetArg(tributeIndex, value3);
			if (text2 == "ok")
			{
				Action <>9__71;
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer newOffer;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						newOffer = new OfferLand(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					}
					else
					{
						newOffer = new DemandLand(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					}
					newOffer.SetParent(tributeOffer);
					CS$<>8__locals1.parameters.Add(newOffer);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, CS$<>8__locals1.parameters, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						tributeOffer.SetArg(tributeIndex, new Value(newOffer));
						CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
						{
							tributeOffer,
							sourceKingdom
						}, null, null, false);
					};
					Action action;
					if ((action = <>9__71) == null)
					{
						action = (<>9__71 = delegate()
						{
							CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
							{
								tributeOffer,
								sourceKingdom
							}, null, null, false);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
					CS$<>8__locals1.destroyButton = true;
				};
			}
		}
		else if (component.name == "TributeMarriage")
		{
			AudienceWindow.<>c__DisplayClass59_18 CS$<>8__locals8 = new AudienceWindow.<>c__DisplayClass59_18();
			CS$<>8__locals8.CS$<>8__locals18 = CS$<>8__locals1;
			CS$<>8__locals8.tributeOffer = (CS$<>8__locals8.CS$<>8__locals18.parameters[0] as Offer);
			CS$<>8__locals8.sourceKingdom = (CS$<>8__locals8.CS$<>8__locals18.parameters[1] as global::Kingdom);
			CS$<>8__locals8.tributeIndex = (int)CS$<>8__locals8.CS$<>8__locals18.parameters[2];
			CS$<>8__locals8.CS$<>8__locals18.offer = Offer.GetCachedOffer("MarriageOffer", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			Value value4 = CS$<>8__locals8.tributeOffer.args[CS$<>8__locals8.tributeIndex];
			CS$<>8__locals8.CS$<>8__locals18.offer.SetParent(CS$<>8__locals8.tributeOffer);
			CS$<>8__locals8.tributeOffer.SetArg(CS$<>8__locals8.tributeIndex, CS$<>8__locals8.CS$<>8__locals18.offer);
			text2 = CS$<>8__locals8.CS$<>8__locals18.offer.ValidateWithoutArgs();
			CS$<>8__locals8.tributeOffer.SetArg(CS$<>8__locals8.tributeIndex, value4);
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					AudienceWindow.<>c__DisplayClass59_19 CS$<>8__locals19 = new AudienceWindow.<>c__DisplayClass59_19();
					CS$<>8__locals19.CS$<>8__locals19 = CS$<>8__locals8;
					CS$<>8__locals19.newOffer = new MarriageOffer(CS$<>8__locals8.CS$<>8__locals18.<>4__this.kCurrent.logic, CS$<>8__locals8.CS$<>8__locals18.<>4__this.kOther.logic, null, null);
					CS$<>8__locals19.newOffer.SetParent(CS$<>8__locals8.tributeOffer);
					CS$<>8__locals8.CS$<>8__locals18.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
					AudienceWindow.<>c__DisplayClass59_19 CS$<>8__locals20 = CS$<>8__locals19;
					Action cancelSelect;
					if ((cancelSelect = CS$<>8__locals8.<>9__73) == null)
					{
						cancelSelect = (CS$<>8__locals8.<>9__73 = delegate()
						{
							CS$<>8__locals8.CS$<>8__locals18.<>4__this.ChangeMenuButtons((CS$<>8__locals8.sourceKingdom == CS$<>8__locals8.CS$<>8__locals18.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
							{
								CS$<>8__locals8.tributeOffer,
								CS$<>8__locals8.sourceKingdom
							}, null, null, false);
						});
					}
					CS$<>8__locals20.cancelSelect = cancelSelect;
					Action<Value> targetPicked = delegate(Value o)
					{
						CS$<>8__locals19.newOffer.SetArg(0, o);
						CS$<>8__locals19.CS$<>8__locals19.CS$<>8__locals18.<>4__this.onClickChangeMenu("TributeMarriageSecondSelect", CS$<>8__locals19.CS$<>8__locals19.CS$<>8__locals18.parameters, null, null);
						Action<Value> action;
						if ((action = CS$<>8__locals19.<>9__75) == null)
						{
							action = (CS$<>8__locals19.<>9__75 = delegate(Value secO)
							{
								CS$<>8__locals19.newOffer.SetArg(1, secO);
								CS$<>8__locals19.CS$<>8__locals19.tributeOffer.SetArg(CS$<>8__locals19.CS$<>8__locals19.tributeIndex, new Value(CS$<>8__locals19.newOffer));
								CS$<>8__locals19.CS$<>8__locals19.CS$<>8__locals18.<>4__this.ChangeMenuButtons((CS$<>8__locals19.CS$<>8__locals19.sourceKingdom == CS$<>8__locals19.CS$<>8__locals19.CS$<>8__locals18.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
								{
									CS$<>8__locals19.CS$<>8__locals19.tributeOffer,
									CS$<>8__locals19.CS$<>8__locals19.sourceKingdom
								}, null, null, false);
							});
						}
						Action<Value> targetPicked2 = action;
						CS$<>8__locals19.CS$<>8__locals19.CS$<>8__locals18.<>4__this.ShowTargetPicker(CS$<>8__locals19.newOffer, targetPicked2, CS$<>8__locals19.cancelSelect, false);
					};
					CS$<>8__locals8.CS$<>8__locals18.<>4__this.ShowTargetPicker(CS$<>8__locals19.newOffer, targetPicked, CS$<>8__locals19.cancelSelect, false);
				};
			}
		}
		else if (component.name.Contains("TributeVassalage"))
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			CS$<>8__locals1.offer = null;
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferVassalage", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandVassalage", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			Value value5 = tributeOffer.args[tributeIndex];
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			tributeOffer.SetArg(tributeIndex, CS$<>8__locals1.offer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			tributeOffer.SetArg(tributeIndex, value5);
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer offer4;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						offer4 = new OfferVassalage(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic);
					}
					else
					{
						offer4 = new DemandVassalage(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic);
					}
					offer4.SetParent(tributeOffer);
					tributeOffer.SetArg(tributeIndex, new Value(offer4));
					CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
					{
						tributeOffer,
						sourceKingdom
					}, null, null, false);
				};
			}
		}
		else if (component.name.Contains("TributeAbandonCaliphate"))
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			CS$<>8__locals1.offer = null;
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferAbandonCaliphate", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandAbandonCaliphate", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			Value value6 = tributeOffer.args[tributeIndex];
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			tributeOffer.SetArg(tributeIndex, CS$<>8__locals1.offer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			tributeOffer.SetArg(tributeIndex, value6);
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer offer4;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						offer4 = new OfferAbandonCaliphate(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic);
					}
					else
					{
						offer4 = new DemandAbandonCaliphate(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic);
					}
					offer4.SetParent(tributeOffer);
					tributeOffer.SetArg(tributeIndex, new Value(offer4));
					CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
					{
						tributeOffer,
						sourceKingdom
					}, null, null, false);
				};
			}
		}
		else if (component.name == "TributeBook")
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			CS$<>8__locals1.offer = null;
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferBook", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandBook", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			Value value7 = tributeOffer.args[tributeIndex];
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			tributeOffer.SetArg(tributeIndex, CS$<>8__locals1.offer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			tributeOffer.SetArg(tributeIndex, value7);
			if (text2 == "ok")
			{
				Action <>9__80;
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer newOffer = null;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						newOffer = new OfferBook(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					}
					else
					{
						newOffer = new DemandBook(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					}
					newOffer.SetParent(tributeOffer);
					CS$<>8__locals1.parameters.Add(newOffer);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, CS$<>8__locals1.parameters, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						tributeOffer.SetArg(tributeIndex, new Value(newOffer));
						CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
						{
							tributeOffer,
							sourceKingdom
						}, null, null, false);
					};
					Action action;
					if ((action = <>9__80) == null)
					{
						action = (<>9__80 = delegate()
						{
							CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
							{
								tributeOffer,
								sourceKingdom
							}, null, null, false);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
			else
			{
				component.Enable(false, false);
			}
		}
		else if (component.name == "TributeReleasePrisoners")
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferReleasePrisoners", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandReleasePrisoners", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			Value value8 = tributeOffer.args[tributeIndex];
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			tributeOffer.SetArg(tributeIndex, CS$<>8__locals1.offer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			tributeOffer.SetArg(tributeIndex, value8);
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer offer4;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						offer4 = new OfferReleasePrisoners(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic);
					}
					else
					{
						offer4 = new DemandReleasePrisoners(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic);
					}
					offer4.SetParent(tributeOffer);
					tributeOffer.SetArg(tributeIndex, new Value(offer4));
					CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
					{
						tributeOffer,
						sourceKingdom
					}, null, null, false);
				};
			}
		}
		else if (component.name == "TributeTradeAgreement")
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			CS$<>8__locals1.offer = Offer.GetCachedOffer("SignTrade", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			Value value9 = tributeOffer.args[tributeIndex];
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			tributeOffer.SetArg(tributeIndex, CS$<>8__locals1.offer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			tributeOffer.SetArg(tributeIndex, value9);
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer offer4 = new SignTrade(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, Array.Empty<Value>());
					offer4.SetParent(tributeOffer);
					tributeOffer.SetArg(tributeIndex, new Value(offer4));
					CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
					{
						tributeOffer,
						sourceKingdom
					}, null, null, false);
				};
			}
		}
		else if (component.name == "TributeReleaseVassalage")
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferReleaseVassalage", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandReleaseVassalage", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				Action <>9__85;
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer newOffer = null;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						newOffer = new OfferReleaseVassalage(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					}
					else
					{
						newOffer = new OfferReleaseVassalage(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					}
					newOffer.SetParent(tributeOffer);
					CS$<>8__locals1.parameters.Add(newOffer);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, CS$<>8__locals1.parameters, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						tributeOffer.SetArg(tributeIndex, new Value(newOffer));
						CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
						{
							tributeOffer,
							sourceKingdom
						}, null, null, false);
					};
					Action action;
					if ((action = <>9__85) == null)
					{
						action = (<>9__85 = delegate()
						{
							CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
							{
								tributeOffer,
								sourceKingdom
							}, null, null, false);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "TributeChangeReligion")
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferChangeReligion", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandChangeReligion", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer offer4;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						offer4 = new OfferChangeReligion(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic);
					}
					else
					{
						offer4 = new DemandChangeReligion(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic);
					}
					offer4.SetParent(tributeOffer);
					tributeOffer.SetArg(tributeIndex, new Value(offer4));
					CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
					{
						tributeOffer,
						sourceKingdom
					}, null, null, false);
				};
			}
		}
		else if (component.name == "TributeAttackKingdom")
		{
			Offer tributeOffer = CS$<>8__locals1.parameters[0] as Offer;
			global::Kingdom sourceKingdom = CS$<>8__locals1.parameters[1] as global::Kingdom;
			int tributeIndex = (int)CS$<>8__locals1.parameters[2];
			if (sourceKingdom == this.kCurrent)
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("OfferAttackKingdom", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			else
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("DemandAttackKingdom", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			}
			CS$<>8__locals1.offer.SetParent(tributeOffer);
			text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
			if (text2 == "ok")
			{
				Action <>9__89;
				component.onClick = delegate(BSGButton BSGbtn)
				{
					Offer newOffer = null;
					if (sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent)
					{
						newOffer = new OfferAttackKingdom(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					}
					else
					{
						newOffer = new DemandAttackKingdom(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
					}
					newOffer.SetParent(tributeOffer);
					CS$<>8__locals1.parameters.Add(newOffer);
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, CS$<>8__locals1.parameters, null, null);
					Action<Value> targetPicked = delegate(Value o)
					{
						newOffer.SetArg(0, o);
						tributeOffer.SetArg(tributeIndex, new Value(newOffer));
						CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
						{
							tributeOffer,
							sourceKingdom
						}, null, null, false);
					};
					Action action;
					if ((action = <>9__89) == null)
					{
						action = (<>9__89 = delegate()
						{
							CS$<>8__locals1.<>4__this.ChangeMenuButtons((sourceKingdom == CS$<>8__locals1.<>4__this.kCurrent) ? "OfferTribute" : "DemandTribute", new List<object>
							{
								tributeOffer,
								sourceKingdom
							}, null, null, false);
						});
					}
					Action targetPickCancelled = action;
					CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
				};
			}
		}
		else if (component.name == "SignTrade")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("SignTrade", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new SignTrade(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, Array.Empty<Value>()));
				};
			}
		}
		else if (component.name == "ExclusiveDeal")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("SignExclusiveTrade", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new SignExclusiveTrade(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, Array.Empty<Value>()));
				};
			}
		}
		else if (component.name == "SignNonAgression")
		{
			CS$<>8__locals1.offer = Offer.GetCachedOffer("SignNonAggression", this.kCurrent.logic, this.kOther.logic, null, 0, null);
			text2 = CS$<>8__locals1.offer.Validate();
			if (text2 == "ok")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickSendOffer(new SignNonAggression(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, Array.Empty<Value>()));
				};
			}
		}
		else if (!(component.name == "SignAlliance"))
		{
			if (component.name == "AcceptOffer")
			{
				Vars vars = CS$<>8__locals1.parameters[0] as Vars;
				CS$<>8__locals1.offer = vars.Get<Offer>("offer", null);
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.offer.Answer("accept", true);
					CS$<>8__locals1.<>4__this.onClickChangeMenu("ContinueMain", null, null, null);
				};
			}
			else if (component.name == "DeclineOffer")
			{
				Vars vars2 = CS$<>8__locals1.parameters[0] as Vars;
				CS$<>8__locals1.offer = vars2.Get<Offer>("offer", null);
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.offer.Answer("decline", true);
					CS$<>8__locals1.<>4__this.onClickChangeMenu("ContinueMain", null, null, null);
				};
			}
			else if (component.name.Contains("CustomCounterOfferAnswer"))
			{
				string answer = "custom_counter_offer_answer";
				if (component.name != "CustomCounterOfferAnswer")
				{
					float num2 = DT.ParseFloat(component.name.Replace("CustomCounterOfferAnswer", ""), 0f);
					answer += (int)num2;
				}
				Vars vars3 = CS$<>8__locals1.parameters[0] as Vars;
				CS$<>8__locals1.offer = vars3.Get<CounterOffer>("offer", null);
				Offer offer3 = CS$<>8__locals1.offer;
				DT.Field field;
				if (offer3 == null)
				{
					field = null;
				}
				else
				{
					Offer.Def def = offer3.def;
					if (def == null)
					{
						field = null;
					}
					else
					{
						OutcomeDef outcomes = def.outcomes;
						if (outcomes == null)
						{
							field = null;
						}
						else
						{
							List<OutcomeDef> options = outcomes.options;
							if (options == null)
							{
								field = null;
							}
							else
							{
								OutcomeDef outcomeDef = options.Find((OutcomeDef o) => o.key == answer);
								field = ((outcomeDef != null) ? outcomeDef.condition_field : null);
							}
						}
					}
				}
				DT.Field field2 = field;
				if (field2 == null || !field2.Bool(CS$<>8__locals1.offer, false))
				{
					CS$<>8__locals1.destroyButton = true;
				}
				else
				{
					component.onClick = delegate(BSGButton BSGbtn)
					{
						CS$<>8__locals1.offer.Answer(answer, true);
						CS$<>8__locals1.<>4__this.onClickChangeMenu("ContinueMain", null, null, null);
					};
				}
			}
			else if (component.name == "AskForCrusade")
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("AskForCrusadeOffer", this.kCurrent.logic, this.kOther.logic, null, 0, null);
				text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
				if (text2 == "ok")
				{
					component.onClick = delegate(BSGButton BSGbtn)
					{
						AskForCrusadeOffer newOffer = new AskForCrusadeOffer(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
						CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
						Action<Value> targetPicked = delegate(Value o)
						{
							newOffer.SetArg(0, o);
							CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
						};
						Action action;
						if ((action = CS$<>8__locals1.<>9__95) == null)
						{
							action = (CS$<>8__locals1.<>9__95 = delegate()
							{
								CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
							});
						}
						Action targetPickCancelled = action;
						CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
					};
				}
			}
			else if (component.name == "AskForExcommunication")
			{
				CS$<>8__locals1.offer = Offer.GetCachedOffer("AskForExcommunicationOffer", this.kCurrent.logic, this.kOther.logic, null, 0, null);
				text2 = CS$<>8__locals1.offer.ValidateWithoutArgs();
				if (text2 == "ok")
				{
					component.onClick = delegate(BSGButton BSGbtn)
					{
						AskForExcommunicationOffer newOffer = new AskForExcommunicationOffer(CS$<>8__locals1.<>4__this.kCurrent.logic, CS$<>8__locals1.<>4__this.kOther.logic, null);
						CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
						Action<Value> targetPicked = delegate(Value o)
						{
							newOffer.SetArg(0, o);
							CS$<>8__locals1.<>4__this.onClickSendOffer(newOffer);
						};
						Action action;
						if ((action = CS$<>8__locals1.<>9__97) == null)
						{
							action = (CS$<>8__locals1.<>9__97 = delegate()
							{
								CS$<>8__locals1.<>4__this.onClickChangeMenu(CS$<>8__locals1.onCancelMenuName, null, null, null);
							});
						}
						Action targetPickCancelled = action;
						CS$<>8__locals1.<>4__this.ShowTargetPicker(newOffer, targetPicked, targetPickCancelled, false);
					};
				}
			}
			else if (component.name == "Close")
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.Hide(false);
				};
			}
			else if (component.name.Contains("ContinueMain"))
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, null, null, null);
				};
			}
			else
			{
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, CS$<>8__locals1.parameters, null, null);
				};
			}
		}
		if (text2 != null && text2 != "ok")
		{
			CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.Invalid;
			if (text2.StartsWith("_", StringComparison.Ordinal))
			{
				string validateStrId = CS$<>8__locals1.offer.def.field.GetString("diplomacy_event_id", null, "", true, true, true, '.') + "." + text2;
				if (text2.Contains("."))
				{
					validateStrId = text2;
				}
				component.onClick = delegate(BSGButton BSGbtn)
				{
					CS$<>8__locals1.<>4__this.onClickChangeMenu(BSGbtn.name, CS$<>8__locals1.parameters, "InvalidOffer", validateStrId);
				};
			}
			else
			{
				CS$<>8__locals1.destroyButton = true;
			}
		}
		if (CS$<>8__locals1.destroyButton)
		{
			global::Common.DestroyObj(component.gameObject);
		}
		else
		{
			if (component.IsEnabled())
			{
				CS$<>8__locals1.textFlags |= AudienceWindow.BeautyfyTextFlags.Active;
			}
			UIText.SetText(component.gameObject, "id_Text", this.BeautifyText(text, CS$<>8__locals1.textFlags));
			if (this.currentMenuIndex == CS$<>8__locals1.index)
			{
				this.BeautifyButtonText(component, AudienceWindow.BeautyfyTextFlags.HighLighted);
			}
		}
		component.onEvent = delegate(BSGButton BSGbtn, BSGButton.Event ev, PointerEventData evd)
		{
			if (ev == BSGButton.Event.Enter)
			{
				CS$<>8__locals1.<>4__this.currentMenuIndex = CS$<>8__locals1.index;
				CS$<>8__locals1.<>4__this.BeautifyButtonText(BSGbtn, AudienceWindow.BeautyfyTextFlags.HighLighted);
				return;
			}
			if (ev == BSGButton.Event.Leave)
			{
				CS$<>8__locals1.<>4__this.BeautifyButtonText(BSGbtn, CS$<>8__locals1.textFlags);
				CS$<>8__locals1.<>4__this.currentMenuIndex = -1;
			}
		};
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x000F12F8 File Offset: 0x000EF4F8
	private void AddButton(GameObject parent, string name, string cameFrom, string dtText, List<object> parameters, int index, GameObject existing = null)
	{
		if (this.menuButtonsPrefab == null)
		{
			this.menuButtonsPrefab = (this.def_field.GetValue("menuButtonsPrefab", null, true, true, true, '.').obj_val as GameObject);
		}
		if (this.menuButtonsPrefab == null)
		{
			return;
		}
		GameObject gameObject;
		if (existing != null)
		{
			gameObject = existing;
		}
		else
		{
			gameObject = global::Common.Spawn(this.menuButtonsPrefab, false, false);
		}
		if (gameObject == null)
		{
			return;
		}
		gameObject.name = name;
		gameObject.transform.SetParent(parent.transform, false);
		this.DecideMenuButtonFunctionality(gameObject, dtText, cameFrom, parameters, index, existing != null);
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x000F13A0 File Offset: 0x000EF5A0
	private void ChangeMenuButtons(string buttonDef, List<object> parameters, string eventType = null, string eventName = null, bool funcionalRefresh = false)
	{
		if (buttonDef == "Main" || buttonDef == "ContinueMain")
		{
			global::Kingdom kingdom = this.kOther;
			Logic.Character character;
			if (kingdom == null)
			{
				character = null;
			}
			else
			{
				Logic.Kingdom logic = kingdom.logic;
				character = ((logic != null) ? logic.GetKing() : null);
			}
			Logic.Character character2 = character;
			if (character2 != null && character2.IsPrisoner())
			{
				if (this.kOther.logic.GetKing().prison_kingdom == this.kCurrent.logic)
				{
					buttonDef += "OurPrisoner";
				}
				else
				{
					buttonDef += "Prisoner";
				}
			}
			if (this.kCurrent.logic.IsEnemy(this.kOther.logic))
			{
				buttonDef += "War";
			}
		}
		this.currentMenuName = buttonDef;
		this.currentMenuParameters = parameters;
		this.currentMenuEventType = eventType;
		this.currentMenuEventName = eventName;
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_MenuButtons", true, true);
		if (gameObject == null)
		{
			return;
		}
		Vars vars = null;
		if (parameters != null && parameters.Count > 0)
		{
			vars = (parameters[0] as Vars);
		}
		if (vars == null)
		{
			vars = new Vars();
		}
		if (!vars.ContainsKey("plr_kingdom"))
		{
			vars.Set<Logic.Kingdom>("plr_kingdom", this.kCurrent.logic);
		}
		if (!vars.ContainsKey("kingdom"))
		{
			vars.Set<Logic.Kingdom>("kingdom", this.kOther.logic);
		}
		Tooltip.Get(gameObject, true).SetDef(null, vars);
		string text = vars.Get<string>("audience_text", null);
		DT.Field field = this.def_field.FindChild(buttonDef, null, true, true, true, '.');
		if (!funcionalRefresh)
		{
			UICommon.DeleteChildren(gameObject.transform);
			if (string.IsNullOrEmpty(text))
			{
				if (eventType == null)
				{
					eventType = field.GetString("DiplomacyEventType", null, "", true, true, true, '.');
				}
				if (eventName == null)
				{
					eventName = field.GetString("DiplomacyEventName", null, "", true, true, true, '.');
				}
				if (eventType != null && eventType.Length != 0)
				{
					Reason reason = Diplomacy.FindRelationshipReason((Diplomacy.TextType)Enum.Parse(typeof(Diplomacy.TextType), eventType), eventName, this.kOther.logic, this.kCurrent.logic);
					string textKey = Diplomacy.GetTextKey((Diplomacy.TextType)Enum.Parse(typeof(Diplomacy.TextType), eventType), eventName, this.kOther.logic, this.kCurrent.logic, reason);
					if (textKey != null)
					{
						Value obj = vars.obj;
						if (obj.is_valid)
						{
							Debug.LogWarning("Replaced " + obj.ToString() + " with " + ((reason == null) ? "null" : reason.ToString()));
						}
						vars.obj = new Value(reason);
						UIText.SetTextKey(this.message, textKey, vars, null);
						vars.obj = obj;
					}
					else
					{
						UIText.SetText(this.message, "Error no text key for event " + eventName + " of type " + eventType);
					}
					this.relHistory.Refresh(new RelationLatestHistory.DelegateEvent(AudienceWindow.HoverRelationHistory), new RelationLatestHistory.DelegateClick(AudienceWindow.SelectRelationHistory), new List<object>
					{
						this
					});
					this.relHistory.SelectButton(reason);
				}
			}
			else
			{
				UIText.SetText(this.message, text);
			}
		}
		AudienceWindow.lastMessage = null;
		DT.Field field2 = field.FindChild("buttons", null, true, true, true, '.');
		if (field2 == null)
		{
			return;
		}
		List<string> list = field2.Keys(true, true);
		int index = 0;
		int i = 0;
		while (i < list.Count)
		{
			string text2 = list[i];
			GameObject gameObject2;
			if (!funcionalRefresh)
			{
				gameObject2 = null;
				goto IL_3C8;
			}
			if (!(text2 != gameObject.transform.GetChild(index).name))
			{
				Transform child = gameObject.transform.GetChild(index++);
				gameObject2 = ((child != null) ? child.gameObject : null);
				if (gameObject2 == null)
				{
					Debug.LogError("Trying to recreate menu without recreating children, but the children amount is incorrect!");
					return;
				}
				goto IL_3C8;
			}
			IL_3E8:
			i++;
			continue;
			IL_3C8:
			string dtText = global::Defs.Localize(field2, text2, vars, null, false, true);
			this.AddButton(gameObject, text2, buttonDef, dtText, parameters, i, gameObject2);
			goto IL_3E8;
		}
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x000F17AC File Offset: 0x000EF9AC
	private static void HoverRelationHistory(BSGButton relBtn, BSGButton.Event ev, PointerEventData eventData, RelationLatestHistory relHist, List<object> parameters)
	{
		AudienceWindow audienceWindow = parameters[0] as AudienceWindow;
		Reason reason = parameters[1] as Reason;
		relHist.DeselectAll();
		if (ev == BSGButton.Event.Enter && AudienceWindow.lastMessage == null)
		{
			AudienceWindow.lastMessage = audienceWindow.message.text;
			UIText.SetText(audienceWindow.message, Diplomacy.GetText((reason.value < 0f) ? Diplomacy.TextType.Angry : Diplomacy.TextType.Pleased, "", null, audienceWindow.kOther.logic, audienceWindow.kCurrent.logic, reason));
			return;
		}
		if (ev == BSGButton.Event.Enter && AudienceWindow.lastMessage != null)
		{
			UIText.SetText(audienceWindow.message, Diplomacy.GetText((reason.value < 0f) ? Diplomacy.TextType.Angry : Diplomacy.TextType.Pleased, "", null, audienceWindow.kOther.logic, audienceWindow.kCurrent.logic, reason));
			return;
		}
		if (ev == BSGButton.Event.Leave && AudienceWindow.lastMessage != null)
		{
			UIText.SetText(audienceWindow.message, AudienceWindow.lastMessage);
			AudienceWindow.lastMessage = null;
		}
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x000F18A0 File Offset: 0x000EFAA0
	private static void SelectRelationHistory(BSGButton relBtn, RelationLatestHistory relHist, List<object> parameters)
	{
		if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
		{
			AudienceWindow audienceWindow = parameters[0] as AudienceWindow;
			Reason reason = parameters[1] as Reason;
			relHist.RemoveModifier(reason);
			relHist.Refresh(new RelationLatestHistory.DelegateEvent(AudienceWindow.HoverRelationHistory), new RelationLatestHistory.DelegateClick(AudienceWindow.SelectRelationHistory), new List<object>
			{
				parameters[0] as AudienceWindow
			});
			UIText.SetText(audienceWindow.message, AudienceWindow.lastMessage);
		}
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x000F192C File Offset: 0x000EFB2C
	private void SelectTributeSlot(BSGButton tributeBtn, global::Kingdom sourceKingdom, int tributeIndex, Offer tributeOffer)
	{
		foreach (object obj in tributeBtn.transform.parent.transform)
		{
			BSGButton component = ((Transform)obj).GetComponent<BSGButton>();
			if (!(component == null))
			{
				if (component == tributeBtn)
				{
					component.SetSelected(true, false);
				}
				else
				{
					component.SetSelected(false, false);
				}
				this.BeautifyButtonText(component, AudienceWindow.BeautyfyTextFlags.None);
			}
		}
		this.ChangeMenuButtons((sourceKingdom == this.kCurrent) ? "TributeOptionsOffer" : "TributeOptionsDemand", new List<object>
		{
			tributeOffer,
			sourceKingdom,
			tributeIndex
		}, null, null, false);
		this.currentTributeMenuIndex = tributeIndex;
		this.ShowTribute(tributeOffer, sourceKingdom, true);
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x000F1A08 File Offset: 0x000EFC08
	private void onClickChangeMenu(string BtnName, List<object> parameters = null, string eventType = null, string eventName = null)
	{
		this.ActivateScrollableContent(false);
		this.ClearTargetSelector(false);
		this.ChangeMenuButtons(BtnName, parameters, eventType, eventName, false);
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x000F1A24 File Offset: 0x000EFC24
	private void onClickSendOffer(Offer offer)
	{
		string text = offer.Validate();
		if (text != "ok")
		{
			Vars vars = new Vars();
			vars.Set<Offer>("offer", offer);
			string eventName = offer.def.field.GetString("diplomacy_event_id", null, "", true, true, true, '.') + "." + text;
			if (text.Contains("."))
			{
				eventName = text;
			}
			this.onClickChangeMenu("InvalidOffer", new List<object>
			{
				vars
			}, "InvalidOffer", eventName);
			return;
		}
		Vars vars2 = new Vars();
		vars2.Set<Offer>("offer", offer);
		this.onClickChangeMenu("SendOfferLagDelay", new List<object>
		{
			vars2
		}, null, null);
		this.elapsed_time = 0f;
		if (UICommon.GetKey(KeyCode.LeftControl, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "accept offer", true))
		{
			offer.answer = "accept";
		}
		else if (UICommon.GetKey(KeyCode.LeftShift, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "decline offer", true))
		{
			offer.answer = "decline";
		}
		else
		{
			offer.answer = null;
		}
		offer.AddListener(this);
		offer.Send(true);
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x000F1B4C File Offset: 0x000EFD4C
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1039780190U)
		{
			if (num != 190343346U)
			{
				if (num != 810626458U)
				{
					if (num != 1039780190U)
					{
						return;
					}
					if (!(message == "defeat"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "stance_changed"))
					{
						return;
					}
					if (!this.kCurrent.logic.IsEnemy(this.kOther.logic))
					{
						return;
					}
					DeclareWar declareWar = null;
					if (this.currentMenuParameters != null && this.currentMenuParameters.Count > 0)
					{
						declareWar = (this.currentMenuParameters[0] as DeclareWar);
					}
					if (declareWar == null || this.currentMenuName != "OfferAnswered")
					{
						this.Close(false);
						return;
					}
					return;
				}
			}
			else
			{
				if (!(message == "royal_new_sovereign"))
				{
					return;
				}
				this.UpdateSovereign();
				return;
			}
		}
		else if (num <= 2827744275U)
		{
			if (num != 2751835415U)
			{
				if (num != 2827744275U)
				{
					return;
				}
				if (!(message == "draw"))
				{
					return;
				}
			}
			else
			{
				if (!(message == "character_died"))
				{
					return;
				}
				Logic.Character character = param as Logic.Character;
				if (character != null && character.title == "PontificalLegate")
				{
					this.UpdateSovereign();
					return;
				}
				return;
			}
		}
		else if (num != 3347338849U)
		{
			if (num != 3547312040U)
			{
				return;
			}
			if (!(message == "offer_proposed"))
			{
				return;
			}
			Offer offer = obj as Offer;
			if ((offer.from == this.kCurrent.logic && offer.to == this.kOther.logic) || (offer.to == this.kCurrent.logic && offer.from == this.kOther.logic))
			{
				this.ActivateScrollableContent(false);
				if (this.HasOpenTargetSelector())
				{
					this.ClearTargetSelector(true);
				}
				this.ChangeMenuButtons("OfferProposed", new List<object>
				{
					param
				}, null, null, false);
				this.proposed_offer = offer;
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
			Offer offer2 = param as Offer;
			if (offer2 == null)
			{
				offer2 = ((param as Vars).obj.obj_val as Offer);
			}
			if (offer2 == null || ((offer2.from != this.kCurrent.logic || offer2.to != this.kOther.logic) && (offer2.to != this.kCurrent.logic || offer2.from != this.kOther.logic)))
			{
				return;
			}
			this.ActivateScrollableContent(false);
			this.ChangeMenuButtons("OfferAnswered", new List<object>
			{
				param
			}, null, null, false);
			if (offer2 == this.proposed_offer)
			{
				this.proposed_offer = null;
				return;
			}
			return;
		}
		this.Close(false);
	}

	// Token: 0x04000FB3 RID: 4019
	private GameObject menuButtonsPrefab;

	// Token: 0x04000FB4 RID: 4020
	private GameObject tributeButtonPrefab;

	// Token: 0x04000FB5 RID: 4021
	public global::Kingdom kCurrent;

	// Token: 0x04000FB6 RID: 4022
	public global::Kingdom kOther;

	// Token: 0x04000FB7 RID: 4023
	private UIKingdomRelations relations;

	// Token: 0x04000FB8 RID: 4024
	private UICharacterIcon sovereign;

	// Token: 0x04000FB9 RID: 4025
	private BSGButton closeButton;

	// Token: 0x04000FBA RID: 4026
	private UIKingdomIcon shield;

	// Token: 0x04000FBB RID: 4027
	private TextMeshProUGUI message;

	// Token: 0x04000FBC RID: 4028
	private static string lastMessage = null;

	// Token: 0x04000FBD RID: 4029
	private GameObject scrollableContent;

	// Token: 0x04000FBE RID: 4030
	private ScrollRect scrollableScrollRect;

	// Token: 0x04000FBF RID: 4031
	private GameObject scrollableRowsContainer;

	// Token: 0x04000FC0 RID: 4032
	private TextMeshProUGUI scrollableTitle;

	// Token: 0x04000FC1 RID: 4033
	private RelationLatestHistory relHistory;

	// Token: 0x04000FC2 RID: 4034
	[HideInInspector]
	public DT.Field def_field;

	// Token: 0x04000FC3 RID: 4035
	private Offer currentPickerOffer;

	// Token: 0x04000FC4 RID: 4036
	private Action<Value> currentPickerActionSelect;

	// Token: 0x04000FC5 RID: 4037
	private Action currentPickerActionCancel;

	// Token: 0x04000FC6 RID: 4038
	private int currentMenuIndex = -1;

	// Token: 0x04000FC7 RID: 4039
	private int currentTributeMenuIndex = -1;

	// Token: 0x04000FC8 RID: 4040
	private string currentMenuName;

	// Token: 0x04000FC9 RID: 4041
	private List<object> currentMenuParameters;

	// Token: 0x04000FCA RID: 4042
	private string currentMenuEventType;

	// Token: 0x04000FCB RID: 4043
	private string currentMenuEventName;

	// Token: 0x04000FCD RID: 4045
	private static string def_id = "AudienceWindow";

	// Token: 0x04000FCE RID: 4046
	private float elapsed_time;

	// Token: 0x04000FCF RID: 4047
	private float delayedUpdateTime = 0.1f;

	// Token: 0x04000FD0 RID: 4048
	private int lagDelayTimes;

	// Token: 0x04000FD1 RID: 4049
	public Offer proposed_offer;

	// Token: 0x020006D5 RID: 1749
	[Flags]
	public enum BeautyfyTextFlags
	{
		// Token: 0x0400373D RID: 14141
		None = 0,
		// Token: 0x0400373E RID: 14142
		Active = 1,
		// Token: 0x0400373F RID: 14143
		HighLighted = 2,
		// Token: 0x04003740 RID: 14144
		Selected = 4,
		// Token: 0x04003741 RID: 14145
		AcceptOnly = 8,
		// Token: 0x04003742 RID: 14146
		Invalid = 16
	}
}
