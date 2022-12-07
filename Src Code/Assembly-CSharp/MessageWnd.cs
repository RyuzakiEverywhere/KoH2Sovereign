using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000234 RID: 564
public class MessageWnd : UIWindow, IVars
{
	// Token: 0x06002263 RID: 8803 RVA: 0x00137AAE File Offset: 0x00135CAE
	public static MessageWnd Create(string def_id, Vars vars, MessageIcon icon = null, MessageWnd.OnButton on_button = null)
	{
		return MessageWnd.Create(global::Defs.GetDefField(def_id, null), vars, icon, on_button);
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x00137ABF File Offset: 0x00135CBF
	public override string GetDefId()
	{
		DT.Field field = this.def_field;
		return ((field != null) ? field.key : null) ?? null;
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x00137AD8 File Offset: 0x00135CD8
	public static MessageWnd Create(DT.Field def_field, Vars vars, MessageIcon icon, MessageWnd.OnButton on_button = null)
	{
		if (def_field == null)
		{
			return null;
		}
		GameObject gameObject = def_field.GetValue("ui_prefab", null, true, true, true, '.').obj_val as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		string @string = def_field.GetString("parent_path", null, "id_MessageContainer", true, true, true, '.');
		GameObject gameObject2 = global::Common.FindChildByName(baseUI.gameObject, @string, true, true);
		GameObject gameObject3 = global::Common.Spawn(gameObject, gameObject2.transform as RectTransform, false, "");
		if (gameObject3 == null)
		{
			return null;
		}
		MessageWnd messageWnd = gameObject3.GetComponent<MessageWnd>();
		if (messageWnd == null)
		{
			messageWnd = gameObject3.AddComponent<MessageWnd>();
		}
		messageWnd.def_field = def_field;
		messageWnd.update_text = (vars != null && vars.Get<int>("update_message_text", 0) == 1);
		messageWnd.vars = vars;
		messageWnd.icon = icon;
		messageWnd.type = ((icon == null) ? MessageIcon.Type.Message : icon.type);
		messageWnd.on_button = on_button;
		messageWnd.Init();
		messageWnd.openVoice = def_field.GetString("open_voice_line", vars, "", true, true, true, '.');
		messageWnd.Open();
		return messageWnd;
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x00137C0C File Offset: 0x00135E0C
	public void CloseAndDismiss(bool allow_stack_dismiss = true)
	{
		bool flag = false;
		if (this.icon != null)
		{
			if (allow_stack_dismiss && (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false)))
			{
				this.icon.DismissStack();
			}
			else
			{
				this.icon.Dismiss(true);
				flag = true;
			}
		}
		if (!flag)
		{
			this.Close(false);
		}
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x00137C6C File Offset: 0x00135E6C
	private GameObject AddButton(GameObject parent, string name, string text)
	{
		if (this.buttonsPrefab == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.FindChildByName(parent, name, true, true);
		if (gameObject == null)
		{
			gameObject = global::Common.Spawn(this.buttonsPrefab, false, false);
			if (gameObject == null)
			{
				return null;
			}
			gameObject.name = name;
			gameObject.transform.SetParent(parent.transform, false);
		}
		UIText.SetText(gameObject, "id_Text", text);
		BSGButton component = gameObject.GetComponent<BSGButton>();
		if (component != null)
		{
			component.SetAudioSet("DefaultAudioSetPaper");
			component.onClick = new BSGButton.OnClick(this.OnButtonClick);
		}
		return gameObject;
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x00137D08 File Offset: 0x00135F08
	public Value GetVar(string key, IVars _vars, bool as_value)
	{
		if (this.vars != null)
		{
			Value var = this.vars.GetVar(key, _vars, as_value);
			if (!var.is_unknown)
			{
				return var;
			}
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
		if (num <= 1648726151U)
		{
			if (num <= 1361572173U)
			{
				if (num != 352990508U)
				{
					if (num != 1361572173U)
					{
						goto IL_1C2;
					}
					if (!(key == "type"))
					{
						goto IL_1C2;
					}
				}
				else
				{
					if (!(key == "message_icon"))
					{
						goto IL_1C2;
					}
					goto IL_198;
				}
			}
			else if (num != 1464693929U)
			{
				if (num != 1648726151U)
				{
					goto IL_1C2;
				}
				if (!(key == "message_def_field"))
				{
					goto IL_1C2;
				}
				goto IL_151;
			}
			else if (!(key == "message_type"))
			{
				goto IL_1C2;
			}
			return this.type.ToString();
		}
		if (num <= 3199239212U)
		{
			if (num != 2209882079U)
			{
				if (num != 3199239212U)
				{
					goto IL_1C2;
				}
				if (!(key == "def_id"))
				{
					goto IL_1C2;
				}
			}
			else
			{
				if (!(key == "tutorial_topic"))
				{
					goto IL_1C2;
				}
				DT.Field field = this.def_field;
				return new Value(Tutorial.Topic.Get((field != null) ? field.def : null, false));
			}
		}
		else if (num != 3372811136U)
		{
			if (num != 3862959600U)
			{
				if (num != 3992043587U)
				{
					goto IL_1C2;
				}
				if (!(key == "def_field"))
				{
					goto IL_1C2;
				}
				goto IL_151;
			}
			else
			{
				if (!(key == "icon"))
				{
					goto IL_1C2;
				}
				goto IL_198;
			}
		}
		else if (!(key == "message_def_id"))
		{
			goto IL_1C2;
		}
		DT.Field field2 = this.def_field;
		string val;
		if (field2 == null)
		{
			val = null;
		}
		else
		{
			DT.Def def = field2.def;
			val = ((def != null) ? def.path : null);
		}
		return val;
		IL_151:
		return new Value(this.def_field);
		IL_198:
		return new Value(this.icon);
		IL_1C2:
		return Value.Unknown;
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x00137EDC File Offset: 0x001360DC
	private Vars GetActionVars(Action action)
	{
		if (action == null)
		{
			return this.vars;
		}
		if (this.vars != null)
		{
			if (this.vars.obj == action)
			{
				return this.vars;
			}
			if (this.vars.Get("action", true).Get<Action>() == action)
			{
				return this.vars;
			}
		}
		Vars vars = new Vars(this.vars);
		vars.Set<Action>("action", action);
		return vars;
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x00137F5C File Offset: 0x0013615C
	private void AddActionButton(GameObject parent, DT.Field f)
	{
		Action action = f.Value(this.vars, true, true).Get<Action>();
		Vars actionVars = this.GetActionVars(action);
		string text = global::Defs.Localize(f, "text", actionVars, null, false, true);
		if (text == null)
		{
			text = global::Defs.Localize("Message.buttons.action_button", actionVars, null, true, true);
		}
		GameObject gameObject = this.AddButton(parent, f.key, text);
		if (gameObject != null && action != null)
		{
			Tooltip.Get(gameObject, true).SetObj(action, null, actionVars);
		}
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x00137FD8 File Offset: 0x001361D8
	private bool AddOfferButtons(GameObject buttons, Offer offer)
	{
		List<OutcomeDef> list;
		if (offer == null)
		{
			list = null;
		}
		else
		{
			OutcomeDef outcomes = offer.def.outcomes;
			list = ((outcomes != null) ? outcomes.options : null);
		}
		List<OutcomeDef> list2 = list;
		if (offer.from == BaseUI.LogicKingdom() || list2 == null || list2.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < list2.Count; i++)
		{
			OutcomeDef outcomeDef = list2[i];
			string key = outcomeDef.key;
			if (outcomeDef.condition_field == null || outcomeDef.condition_field.Bool(offer, false))
			{
				string text = global::Defs.Localize(outcomeDef.field, "label", this.vars, null, false, true);
				if (text == null)
				{
					string text_key = "Message.buttons." + key;
					if (global::Defs.FindTextField(text_key) == null)
					{
						goto IL_D8;
					}
					text = global::Defs.Localize(text_key, null, null, true, true);
				}
				if (key == offer.def.default_outcome)
				{
					text = "<u>" + text + "</u>";
				}
				this.AddButton(buttons, key, text);
			}
			IL_D8:;
		}
		return true;
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x001380D0 File Offset: 0x001362D0
	private void InitButtons()
	{
		this.buttons = global::Common.FindChildByName(base.gameObject, "id_Buttons", true, true);
		if (this.buttons == null)
		{
			return;
		}
		Tooltip.Get(this.buttons, true);
		if (this.RecreacteButtons)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.buttons.transform.childCount; i++)
			{
				GameObject gameObject = this.buttons.transform.GetChild(i).gameObject;
				if (!(gameObject.GetComponent<BSGButton>() == null))
				{
					list.Add(gameObject);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				global::Common.DestroyObj(list[j]);
			}
		}
		bool flag = !this.RecreacteButtons;
		if (this.icon != null && this.icon.type == MessageIcon.Type.PendingOffer)
		{
			Offer offer = this.icon.vars.Get<Offer>("offer", null);
			flag |= this.AddOfferButtons(this.buttons, offer);
		}
		DT.Field field = this.def_field.FindChild("buttons", null, true, true, true, '.');
		if (field != null)
		{
			List<DT.Field> list2 = field.Children();
			for (int k = 0; k < list2.Count; k++)
			{
				DT.Field field2 = list2[k];
				string key = field2.key;
				if (!string.IsNullOrEmpty(key))
				{
					if (key == "goto")
					{
						MapObject mapObject = this.vars.Get<MapObject>("goto_target", null);
						if (mapObject == null)
						{
							mapObject = this.vars.Get<MapObject>("obj", null);
						}
						if (mapObject == null)
						{
							goto IL_26E;
						}
					}
					if (field2.Type() == "action")
					{
						this.AddActionButton(this.buttons, field2);
						flag = true;
					}
					else
					{
						string text = null;
						if (field2.Type() != "text")
						{
							DT.Field field3 = field2.FindChild("text", null, true, true, true, '.');
							if (field3 != null)
							{
								text = global::Defs.Localize(field3, this.vars, null, false, true);
							}
						}
						if (text == null)
						{
							text = global::Defs.Localize(field, key, this.vars, null, false, true);
						}
						if (text == null)
						{
							text = global::Defs.Localize("Message.buttons." + key, null, null, true, true);
						}
						flag = true;
						GameObject obj = this.AddButton(this.buttons, key, text);
						DT.Field field4 = field2.FindChild("tooltip", null, true, true, true, '.');
						if (field4 != null)
						{
							Tooltip.Get(obj, true).SetText(field4.Path(false, false, '.'), null, this.vars);
						}
					}
				}
				IL_26E:;
			}
		}
		if (!flag)
		{
			this.AddButton(this.buttons, "ok", global::Defs.Localize("Message.buttons.ok", null, null, true, true));
		}
		this.UpdateButtons();
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x00138388 File Offset: 0x00136588
	private void UpdateButtons()
	{
		if (this.validate_button == null || this.buttons == null)
		{
			return;
		}
		for (int i = 0; i < this.buttons.transform.childCount; i++)
		{
			Transform child = this.buttons.transform.GetChild(i);
			string name = child.name;
			string res = this.validate_button(this, name);
			if (this == null)
			{
				return;
			}
			this.UpdateButtonState(child, res);
		}
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x00138400 File Offset: 0x00136600
	private void UpdateButtonState(Transform t, string res)
	{
		GameObject gameObject = t.gameObject;
		BSGButton component = t.GetComponent<BSGButton>();
		if (res == "ok")
		{
			gameObject.SetActive(true);
			if (component != null)
			{
				component.Enable(true, false);
			}
			return;
		}
		if (res != null && res.StartsWith("_", StringComparison.Ordinal))
		{
			gameObject.SetActive(true);
			if (component != null)
			{
				component.Enable(false, false);
			}
			return;
		}
		gameObject.SetActive(false);
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x00138474 File Offset: 0x00136674
	private void EnableRootComponents()
	{
		MessageWnd.tmp_componetList.Clear();
		base.GetComponents<MonoBehaviour>(MessageWnd.tmp_componetList);
		for (int i = 0; i < MessageWnd.tmp_componetList.Count; i++)
		{
			MessageWnd.tmp_componetList[i].enabled = true;
		}
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x001384BC File Offset: 0x001366BC
	private void Init()
	{
		this.EnableRootComponents();
		Vars vars = this.vars;
		string text = (vars != null) ? vars.Get<string>("_localized_caption", null) : null;
		if (text != null)
		{
			UIText.SetText(base.gameObject, "id_Caption", text);
		}
		else
		{
			UIText.SetText(base.gameObject, "id_Caption", this.def_field, "caption", this.vars, null);
		}
		this.message_text_ctl = global::Common.FindChildComponent<TMP_Text>(base.gameObject, "id_MessageText");
		this.message_ht_ctl = global::Common.FindChildComponent<UIHyperText>(base.gameObject, "id_MessageHyperText");
		this.UpdateText();
		if (this.message_ht_ctl != null)
		{
			DT.Field field = this.def_field.FindChild("hypertext", null, true, true, true, '.');
			if (field == null)
			{
				this.message_ht_ctl.gameObject.SetActive(false);
			}
			else
			{
				this.message_ht_ctl.gameObject.SetActive(true);
				this.message_ht_ctl.Load(field, this);
			}
		}
		Image image = global::Common.FindChildComponent<Image>(base.gameObject, "id_Illustration");
		if (image != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(this.def_field, "illustration", this.vars);
			image.gameObject.SetActive(obj != null);
			if (obj != null)
			{
				image.sprite = obj;
			}
		}
		Image image2 = global::Common.FindChildComponent<Image>(base.gameObject, "id_CategoryIllustration");
		if (image2 != null)
		{
			image2.gameObject.SetActive(false);
		}
		TMP_Text tmp_Text = this.message_text_ctl;
		GameObject gameObject = (tmp_Text != null) ? tmp_Text.transform.parent.gameObject : null;
		if (gameObject != null)
		{
			gameObject.SetActive(UICommon.HasActiveChildren(gameObject));
		}
		this.InitButtons();
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x00138664 File Offset: 0x00136864
	private void UpdateText()
	{
		UIText.cur_message_wnd = this;
		if (this.message_text_ctl != null)
		{
			string text;
			if (this.update_text)
			{
				text = global::Defs.Localize(this.def_field, "body", this.vars, null, false, true);
			}
			else
			{
				Vars vars = this.vars;
				text = ((vars != null) ? vars.Get<string>("_localized_body", null) : null);
				if (text == null)
				{
					text = global::Defs.Localize(this.def_field, "body", this.vars, null, false, true);
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				this.message_text_ctl.gameObject.SetActive(false);
			}
			else
			{
				this.message_text_ctl.gameObject.SetActive(true);
				UIText.SetText(this.message_text_ctl, text);
			}
		}
		UIText.cur_message_wnd = null;
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x00138720 File Offset: 0x00136920
	private void OnGoTo()
	{
		MapObject mapObject = this.vars.Get<MapObject>("goto_target", null);
		if (mapObject == null)
		{
			mapObject = this.vars.Get<MapObject>("obj", null);
		}
		if (mapObject == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (ViewMode.IsPoliticalView())
		{
			ViewMode.WorldView.Apply();
		}
		if (worldUI != null)
		{
			worldUI.LookAt(mapObject.position, false);
		}
	}

	// Token: 0x06002273 RID: 8819 RVA: 0x0013878C File Offset: 0x0013698C
	private void OnButtonClick(BSGButton btn)
	{
		string name = btn.name;
		if (this.buttons != null && this.buttons.transform != null && this.buttons.transform.childCount > 1)
		{
			Analytics instance = Analytics.instance;
			if (instance != null)
			{
				instance.OnDecisionTaken(this.type, this.vars, this.def_field, (this.icon != null) ? this.icon.GetExpireTime() : -1f, name);
			}
		}
		if (this.on_button != null && this.on_button(this, name))
		{
			return;
		}
		if (name == "ok")
		{
			this.CloseAndDismiss(true);
			return;
		}
		if (name == "close")
		{
			MessageIcon messageIcon = this.icon;
			string a;
			if (messageIcon == null)
			{
				a = null;
			}
			else
			{
				DT.Field field = messageIcon.def_field;
				a = ((field != null) ? field.key : null);
			}
			if (a == "DiplomacyOfferLeftMessage")
			{
				this.CloseAndDismiss(true);
				return;
			}
			this.Close(false);
			return;
		}
		else
		{
			if (name == "goto")
			{
				this.OnGoTo();
				return;
			}
			if (name == "audience")
			{
				AudienceWindow.Create(global::Kingdom.Get(this.vars.Get<Logic.Kingdom>("kingdom", null).id), "Main", null);
				this.CloseAndDismiss(true);
				return;
			}
			if (name == "family")
			{
				UIRoyalFamily.ToggleOpen(BaseUI.LogicKingdom());
				this.CloseAndDismiss(true);
				return;
			}
			if (name == "open_ranking_window")
			{
				UIKingdomRankingsWindow.Create(this.vars.Get<KingdomRanking>("ranking", null), null);
				this.CloseAndDismiss(true);
				return;
			}
			if (this.icon != null && this.icon.type == MessageIcon.Type.PendingOffer)
			{
				Offer offer = this.icon.vars.Get<Offer>("offer", null);
				if (name == "cancel")
				{
					offer.Cancel();
					return;
				}
				if (offer != null)
				{
					offer.Answer(name, true);
					return;
				}
			}
			DT.Field field2 = this.def_field.FindChild("buttons." + name, null, true, true, true, '.');
			if (field2 != null && field2.Type() == "action")
			{
				Action action = field2.Value(this.vars, true, true).Get<Action>();
				if (action != null)
				{
					Vars vars = this.vars;
					Logic.Object target = (vars != null) ? vars.Get<Logic.Object>("target", null) : null;
					Action action2 = action;
					Vars vars2 = this.vars;
					action2.args = ((vars2 != null) ? vars2.Get<List<Value>>("args", null) : null);
					if (action.Execute(target))
					{
						this.CloseAndDismiss(true);
					}
					return;
				}
			}
			if (field2 != null && field2.Type() == "cancel_opportunity")
			{
				MessageIcon messageIcon2 = this.icon;
				object obj;
				if (messageIcon2 == null)
				{
					obj = null;
				}
				else
				{
					Vars vars3 = messageIcon2.vars;
					obj = ((vars3 != null) ? vars3.obj.obj_val : null);
				}
				Opportunity opportunity = obj as Opportunity;
				if (opportunity != null)
				{
					Logic.Character character = opportunity.owner as Logic.Character;
					if (character != null)
					{
						character.actions.DelOpportunity(opportunity.action, opportunity.target, opportunity.args, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
						this.CloseAndDismiss(true);
					}
					return;
				}
			}
			this.Close(false);
			return;
		}
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x00138AB6 File Offset: 0x00136CB6
	public override bool OnBackInputAction()
	{
		return Tutorial.OnBackInputAction(this) || base.OnBackInputAction();
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x00138AC8 File Offset: 0x00136CC8
	private bool Validate()
	{
		if (this.type != MessageIcon.Type.Opportunity)
		{
			return true;
		}
		if (this.vars == null)
		{
			return false;
		}
		Opportunity opportunity = this.vars.obj.Get<Opportunity>();
		return opportunity != null && opportunity.active;
	}

	// Token: 0x06002276 RID: 8822 RVA: 0x00138B0C File Offset: 0x00136D0C
	private new void Update()
	{
		if (Input.GetMouseButtonDown(1) && RectTransformUtility.RectangleContainsScreenPoint(base.GetComponent<RectTransform>(), Input.mousePosition))
		{
			if (!BaseUI.IgnoreRightClick())
			{
				this.OnBackInputAction();
			}
			return;
		}
		if (!this.Validate())
		{
			this.CloseAndDismiss(false);
			return;
		}
		this.UpdateButtons();
		if (this.update_text)
		{
			this.UpdateText();
		}
		if (this.on_update != null)
		{
			this.on_update(this);
		}
	}

	// Token: 0x06002277 RID: 8823 RVA: 0x00138B80 File Offset: 0x00136D80
	public override bool IsSimilar(UIWindow window)
	{
		MessageWnd messageWnd = window as MessageWnd;
		return !(messageWnd == null) && messageWnd.window_def == this.window_def && messageWnd.type == this.type && ((messageWnd.vars == null && this.vars == null) || (messageWnd.vars != null && this.vars != null && !(messageWnd.vars.obj != this.vars.obj)));
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x00138C03 File Offset: 0x00136E03
	protected override void OnDestroy()
	{
		this.on_update = null;
		this.validate_button = null;
		base.OnDestroy();
	}

	// Token: 0x0400170C RID: 5900
	public GameObject buttonsPrefab;

	// Token: 0x0400170D RID: 5901
	public bool RecreacteButtons = true;

	// Token: 0x0400170E RID: 5902
	private GameObject buttons;

	// Token: 0x0400170F RID: 5903
	[HideInInspector]
	public DT.Field def_field;

	// Token: 0x04001710 RID: 5904
	[HideInInspector]
	public Vars vars;

	// Token: 0x04001711 RID: 5905
	[HideInInspector]
	public MessageIcon icon;

	// Token: 0x04001712 RID: 5906
	private bool update_text;

	// Token: 0x04001713 RID: 5907
	private TMP_Text message_text_ctl;

	// Token: 0x04001714 RID: 5908
	private UIHyperText message_ht_ctl;

	// Token: 0x04001715 RID: 5909
	[HideInInspector]
	public MessageIcon.Type type;

	// Token: 0x04001716 RID: 5910
	public MessageWnd.OnButton on_button;

	// Token: 0x04001717 RID: 5911
	public MessageWnd.ValidateButton validate_button;

	// Token: 0x04001718 RID: 5912
	public MessageWnd.OnUpdate on_update;

	// Token: 0x04001719 RID: 5913
	private static List<MonoBehaviour> tmp_componetList = new List<MonoBehaviour>();

	// Token: 0x0200078E RID: 1934
	// (Invoke) Token: 0x06004C8D RID: 19597
	public delegate bool OnButton(MessageWnd wnd, string btn_id);

	// Token: 0x0200078F RID: 1935
	// (Invoke) Token: 0x06004C91 RID: 19601
	public delegate string ValidateButton(MessageWnd wnd, string btn_id);

	// Token: 0x02000790 RID: 1936
	// (Invoke) Token: 0x06004C95 RID: 19605
	public delegate void OnUpdate(MessageWnd wnd);
}
