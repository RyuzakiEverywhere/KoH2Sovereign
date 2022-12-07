using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000194 RID: 404
public class UIStatusWindow : ObjectWindow
{
	// Token: 0x1700012B RID: 299
	// (get) Token: 0x0600167E RID: 5758 RVA: 0x000E15CD File Offset: 0x000DF7CD
	// (set) Token: 0x0600167F RID: 5759 RVA: 0x000E15D5 File Offset: 0x000DF7D5
	public Logic.Status Data { get; set; }

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06001680 RID: 5760 RVA: 0x000E15E0 File Offset: 0x000DF7E0
	// (remove) Token: 0x06001681 RID: 5761 RVA: 0x000E1618 File Offset: 0x000DF818
	public event Action<Logic.Status, string> OnButtonAction;

	// Token: 0x06001682 RID: 5762 RVA: 0x000E164D File Offset: 0x000DF84D
	protected override void Awake()
	{
		base.Awake();
		if (this.Button_Prototype != null)
		{
			this.Button_Prototype.SetActive(false);
		}
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x000E166F File Offset: 0x000DF86F
	protected override void OnDestroy()
	{
		this.Clean();
		base.OnDestroy();
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x000E167D File Offset: 0x000DF87D
	private void Clean()
	{
		this.OnButtonAction = null;
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x000E1686 File Offset: 0x000DF886
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		if (obj is Logic.Status)
		{
			this.Data = (obj as Logic.Status);
		}
		if (this.Data == null)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.Refresh();
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x000E16BC File Offset: 0x000DF8BC
	public override void Refresh()
	{
		if (this.Data == null || this.Data.def == null || this.Data.def.field == null)
		{
			return;
		}
		UIText.SetText(this.Text_Status, global::Defs.Localize(this.Data.def.field, "status_text", new Vars(this.Data), null, false, true) ?? "");
		if (this.Button_Prototype != null)
		{
			DT.Field field = this.Data.def.field.FindChild("buttons", null, true, true, true, '.');
			if (field != null)
			{
				List<DT.Field> list = field.Children();
				if (list != null)
				{
					foreach (DT.Field buttonField in list)
					{
						this.AddButton(buttonField, this.Data, this.Button_Prototype, this.Container_Buttons);
					}
				}
			}
		}
		this.BuildActions();
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x000E17D8 File Offset: 0x000DF9D8
	private void AddButton(DT.Field buttonField, Logic.Status status, GameObject prototype, RectTransform parent)
	{
		if (buttonField == null)
		{
			return;
		}
		if (prototype == null)
		{
			return;
		}
		if (parent == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		gameObject.SetActive(true);
		UIStatusWindow.StatusActionButton statusActionButton = gameObject.AddComponent<UIStatusWindow.StatusActionButton>();
		statusActionButton.Build(buttonField, new Vars(status));
		statusActionButton.OnClick += this.HandleButtonClick;
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x000E183F File Offset: 0x000DFA3F
	private void HandleButtonClick(string buttonParam)
	{
		if (this.OnButtonAction != null)
		{
			this.OnButtonAction(this.Data, buttonParam);
			return;
		}
		(this.Data.visuals as global::Status).OnButton(this.Data, buttonParam);
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x000E1878 File Offset: 0x000DFA78
	public new static GameObject GetWindow(Logic.Object obj, Vars vars, RectTransform parent)
	{
		if (obj == null)
		{
			Debug.LogWarning("Logic.Object is null");
			return null;
		}
		string text = "prefab";
		if (vars != null)
		{
			string text2 = vars.Get<string>("variant", null);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "." + text2;
			}
		}
		DT.Def def = UIStatusWindow.GetDef(obj);
		if (def == null)
		{
			return null;
		}
		GameObject obj2 = global::Defs.GetObj<GameObject>(def.field, text, null);
		if (obj2 == null)
		{
			obj2 = global::Defs.GetObj<GameObject>(UIStatusWindow.GetDef(obj).field, "ui_prefab", null);
		}
		if (obj2 == null)
		{
			Debug.Log("Fail to find a prefab for" + obj);
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj2, parent);
		ObjectWindow component = gameObject.GetComponent<ObjectWindow>();
		if (component != null)
		{
			component.SetObject(obj, vars);
		}
		else
		{
			Debug.Log("instanceGO " + gameObject + "is missing an object window logic");
		}
		return gameObject;
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x000E1954 File Offset: 0x000DFB54
	public new static DT.Def GetDef(Logic.Object obj)
	{
		if (obj == null)
		{
			return null;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return null;
		}
		Type type = obj.GetType();
		DT.Def def;
		for (;;)
		{
			string path = Logic.Object.TypeToStr(type);
			def = defs.dt.FindDef(path);
			if (def != null)
			{
				break;
			}
			type = type.BaseType;
			if (type == null)
			{
				goto Block_4;
			}
		}
		return def;
		Block_4:
		return null;
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x000E19AC File Offset: 0x000DFBAC
	private void BuildActions()
	{
		this.CleanActions();
		if (this.m_CurrentActions == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.m_ActionsContainer);
		if (this.Data == null)
		{
			return;
		}
		Logic.Character character = this.Data.owner as Logic.Character;
		if (character == null)
		{
			return;
		}
		if (character.actions == null)
		{
			return;
		}
		List<Action.Def> relevanActions = this.GetRelevanActions(this.Data);
		if (relevanActions != null && relevanActions.Count > 0)
		{
			int i = 0;
			int count = relevanActions.Count;
			while (i < count)
			{
				Action action = character.actions.Find(relevanActions[i]);
				if (action != null && action.def.field.GetBool("show_in_character_widnow", null, false, true, true, true, '.'))
				{
					action.SetStatus(this.Data);
					string text = action.Validate(false);
					bool flag = false;
					if (text == "ok")
					{
						flag = true;
					}
					else if (action.def.field.GetBool("show_if_invalid", null, false, true, true, true, '.'))
					{
						flag = true;
					}
					else if (!string.IsNullOrEmpty(text) && text[0] == '_')
					{
						flag = true;
					}
					if (flag)
					{
						DT.Field field = action.def.field.FindChild("show_in_status.target", action, true, true, true, '.');
						if (field != null)
						{
							action.target = field.Value(this.Data, true, true).Get<Logic.Object>();
							if (!action.ValidateTarget(action.target))
							{
								flag = false;
							}
						}
					}
					if (flag)
					{
						ActionVisuals actionVisuals = action.visuals as ActionVisuals;
						if (actionVisuals != null)
						{
							actionVisuals.logic.AddListener(this);
							GameObject icon = ObjectIcon.GetIcon(actionVisuals.logic, null, this.m_ActionsContainer.transform as RectTransform);
							ExileAction exileAction = action as ExileAction;
							this.m_CurrentActions.Add(actionVisuals);
							this.m_ActionIcons.Add(icon);
						}
					}
				}
				i++;
			}
		}
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x000E1B9C File Offset: 0x000DFD9C
	private List<Action.Def> GetRelevanActions(Logic.Status data)
	{
		List<Action.Def> list = new List<Action.Def>();
		Logic.Character character = data.owner as Logic.Character;
		if (character == null)
		{
			return list;
		}
		if (character.status == data && character.actions != null)
		{
			for (int i = 0; i < character.actions.Count; i++)
			{
				if (character.actions[i].def.show_in_status == null)
				{
					list.Add(character.actions[i].def);
				}
			}
		}
		this.AddActions(list, data.def);
		return list;
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x000E1C24 File Offset: 0x000DFE24
	private void AddActions(List<Action.Def> result, Logic.Status.Def def)
	{
		if (def == null)
		{
			return;
		}
		Logic.Status.Def def2 = def.BasedOn<Logic.Status.Def>();
		this.AddActions(result, def2);
		if (def.actions == null)
		{
			return;
		}
		for (int i = 0; i < def.actions.Count; i++)
		{
			result.Add(def.actions[i]);
		}
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x000E1C78 File Offset: 0x000DFE78
	private void CleanActions()
	{
		for (int i = 0; i < this.m_ActionIcons.Count; i++)
		{
			UnityEngine.Object.Destroy(this.m_ActionIcons[i]);
		}
		this.m_ActionIcons.Clear();
		this.m_CurrentActions.Clear();
	}

	// Token: 0x04000E86 RID: 3718
	[UIFieldTarget("id_StatusText")]
	[SerializeField]
	private TextMeshProUGUI Text_Status;

	// Token: 0x04000E87 RID: 3719
	[SerializeField]
	private GameObject Button_Prototype;

	// Token: 0x04000E88 RID: 3720
	[SerializeField]
	private RectTransform Container_Buttons;

	// Token: 0x04000E89 RID: 3721
	[UIFieldTarget("id_ActionsContainer")]
	private RectTransform m_ActionsContainer;

	// Token: 0x04000E8C RID: 3724
	private List<ActionVisuals> m_CurrentActions = new List<ActionVisuals>();

	// Token: 0x04000E8D RID: 3725
	private List<GameObject> m_ActionIcons = new List<GameObject>();

	// Token: 0x020006CC RID: 1740
	protected internal class StatusActionButton : MonoBehaviour
	{
		// Token: 0x1400004A RID: 74
		// (add) Token: 0x06004893 RID: 18579 RVA: 0x002181E8 File Offset: 0x002163E8
		// (remove) Token: 0x06004894 RID: 18580 RVA: 0x00218220 File Offset: 0x00216420
		public event Action<string> OnClick;

		// Token: 0x06004895 RID: 18581 RVA: 0x00218258 File Offset: 0x00216458
		public void Build(DT.Field field, Vars vars = null)
		{
			UICommon.FindComponents(this, false);
			this.m_eventData = field.key;
			BSGButton component = base.gameObject.GetComponent<BSGButton>();
			if (component != null)
			{
				BSGButton bsgbutton = component;
				bsgbutton.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton.onClick, new BSGButton.OnClick(this.HandleOnCLick));
			}
			this.status = ((vars == null) ? null : vars.obj.Get<Logic.Status>());
			this.action = ((this.status == null) ? null : this.status.GetButtonAction(field.key));
			this.RefreshDynamic();
			Action action = this.action;
			DT.Field field2;
			if (action == null)
			{
				field2 = null;
			}
			else
			{
				Action.Def def = action.def;
				field2 = ((def != null) ? def.field : null);
			}
			DT.Field field3 = field2;
			Sprite obj = global::Defs.GetObj<Sprite>(field, "icon", null);
			if (obj == null && field3 != null)
			{
				obj = global::Defs.GetObj<Sprite>(field3, "icon", null);
			}
			string text = global::Defs.Localize(field, "button_text", vars, null, false, true);
			if (text == null && field3 != null)
			{
				text = global::Defs.Localize(field3, "name", vars, null, false, true);
			}
			DT.Field field4 = field.FindChild("tooltip", null, true, true, true, '.');
			string text2 = null;
			string text3 = null;
			if (field4 == null && field3 != null)
			{
				string str = field3.Path(false, false, '.');
				text2 = str + ".tooltip";
				text3 = str + ".name";
			}
			if (!string.IsNullOrEmpty(text) && obj != null)
			{
				if (this.Icon_Status != null)
				{
					this.Icon_Status.sprite = obj;
				}
				UIText.SetText(this.Text_Status, text);
			}
			else if (obj != null)
			{
				if (this.Icon_Status != null)
				{
					this.Icon_Status.sprite = obj;
				}
				if (this.Text_Status != null)
				{
					this.Text_Status.gameObject.SetActive(false);
				}
			}
			else if (!string.IsNullOrEmpty(text))
			{
				UIText.SetText(this.Text_Status, text);
				if (this.Icon_Status != null)
				{
					this.Icon_Status.gameObject.SetActive(false);
				}
			}
			if (field4 != null)
			{
				Tooltip.Get(base.gameObject, true).SetDef(field4.def, vars, null);
				return;
			}
			if (text2 != null || text3 != null)
			{
				Tooltip.Get(base.gameObject, true).SetText(text2, text3, vars);
			}
		}

		// Token: 0x06004896 RID: 18582 RVA: 0x0021848C File Offset: 0x0021668C
		private void RefreshDynamic()
		{
			bool flag = false;
			if (this.action != null)
			{
				if (this.action.Validate(true) != "ok")
				{
					base.gameObject.SetActive(false);
					return;
				}
				flag = !this.action.CheckCost(null);
			}
			if (this.Icon_Status != null)
			{
				this.Icon_Status.color = (flag ? Color.gray : Color.white);
			}
			if (this.Text_Status != null)
			{
				this.Text_Status.color = (flag ? Color.gray : Color.black);
			}
		}

		// Token: 0x06004897 RID: 18583 RVA: 0x00218528 File Offset: 0x00216728
		private void Update()
		{
			this.RefreshDynamic();
		}

		// Token: 0x06004898 RID: 18584 RVA: 0x00218530 File Offset: 0x00216730
		private void HandleOnCLick(BSGButton btn)
		{
			if (this.OnClick != null)
			{
				this.OnClick(this.m_eventData);
			}
		}

		// Token: 0x04003717 RID: 14103
		[UIFieldTarget("id_StatusIcon")]
		private Image Icon_Status;

		// Token: 0x04003718 RID: 14104
		[UIFieldTarget("id_StatusText")]
		private TextMeshProUGUI Text_Status;

		// Token: 0x04003719 RID: 14105
		public Logic.Status characterStatus;

		// Token: 0x0400371B RID: 14107
		private string m_eventData;

		// Token: 0x0400371C RID: 14108
		private Logic.Status status;

		// Token: 0x0400371D RID: 14109
		private Action action;
	}
}
