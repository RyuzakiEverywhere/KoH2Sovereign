using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002E0 RID: 736
public class UITargetItem : Hotspot, IPoolable
{
	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06002E84 RID: 11908 RVA: 0x00180134 File Offset: 0x0017E334
	// (set) Token: 0x06002E85 RID: 11909 RVA: 0x0018013C File Offset: 0x0017E33C
	public TargetPickerData Data { get; private set; }

	// Token: 0x06002E86 RID: 11910 RVA: 0x00180148 File Offset: 0x0017E348
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.SetAudioSet("ButtonAudioSet");
		if (this.Text_Right != null)
		{
			this.m_RightTextLayoutElement = this.Text_Right.GetComponent<LayoutElement>();
			this.Text_Right.text = string.Empty;
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x001801A6 File Offset: 0x0017E3A6
	public void Select(bool select)
	{
		this.m_Selected = select;
		this.UpdateHighlight();
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x001801B5 File Offset: 0x0017E3B5
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x001801BD File Offset: 0x0017E3BD
	private void Refresh()
	{
		this.Init();
		if (this.Data.Target.is_object)
		{
			this.UpdateAsObject();
		}
		else
		{
			this.UpdateAsValue();
		}
		this.UpdateLayout();
		this.UpdateHighlight();
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x000023FD File Offset: 0x000005FD
	private void UpdateLayout()
	{
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x001801F1 File Offset: 0x0017E3F1
	private void Update()
	{
		if (!this.m_Initialzied)
		{
			return;
		}
		if (this.m_LastUpdate + this.m_UpdateInterval < UnityEngine.Time.unscaledTime)
		{
			this.UpdateDynamicData();
			this.m_LastUpdate = UnityEngine.Time.unscaledTime;
		}
	}

	// Token: 0x06002E8C RID: 11916 RVA: 0x00180224 File Offset: 0x0017E424
	private DT.Field GetDefField(Value arg_type, Value defId)
	{
		if (!defId.is_string)
		{
			return null;
		}
		if (arg_type == "Goods")
		{
			return global::Defs.GetDefField(defId, null);
		}
		if (arg_type == "PaganBeliefs")
		{
			return global::Defs.GetDefField("Pagan", defId);
		}
		return null;
	}

	// Token: 0x06002E8D RID: 11917 RVA: 0x0018027F File Offset: 0x0017E47F
	private void HandleArgumentType(DT.Field field)
	{
		if (field == null)
		{
			return;
		}
		UIText.SetText(this.Text_ItemName, global::Defs.Localize(field, "name", this.Data.Vars, null, true, true));
	}

	// Token: 0x06002E8E RID: 11918 RVA: 0x001802AC File Offset: 0x0017E4AC
	private GameObject HandleArgumentTypeIcon(DT.Field field, RectTransform parent)
	{
		if (field == null)
		{
			return null;
		}
		Sprite obj = global::Defs.GetObj<Sprite>(field, "icon", this.Data.Vars);
		if (obj == null)
		{
			return null;
		}
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<Image>().sprite = obj;
		gameObject.transform.SetParent(parent, false);
		return gameObject;
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x001802FE File Offset: 0x0017E4FE
	private void UpdateDynamicData()
	{
		if (this.Data == null)
		{
			return;
		}
		this.PopulateAndAdjustRightText();
	}

	// Token: 0x06002E90 RID: 11920 RVA: 0x00180310 File Offset: 0x0017E510
	private void PopulateAndAdjustRightText()
	{
		if (this.Text_Right != null && this.Data.Vars != null)
		{
			string text = global::Defs.Localize(this.Data.Vars.Get("rightTextKey", true), this.Data.Vars, null, true, true);
			if (this.m_RightTextLayoutElement != null)
			{
				Vector2 preferredValues = this.Text_Right.GetPreferredValues(text);
				this.m_RightTextLayoutElement.minWidth = (float)Mathf.CeilToInt(preferredValues.x + 1f);
			}
			UIText.SetText(this.Text_Right, text);
		}
	}

	// Token: 0x06002E91 RID: 11921 RVA: 0x001803B0 File Offset: 0x0017E5B0
	private void UpdateAsValue()
	{
		if (this.Data.Vars != null)
		{
			if (this.Data.Vars.obj.is_object && this.Data.Vars.obj.obj_val is Def)
			{
				UIText.SetText(this.Text_ItemName, global::Defs.LocalizedObjName(this.Data.Vars.obj.obj_val, null, "", true));
			}
			else
			{
				Value arg_type = this.Data.Vars.Get("argument_type", true);
				if (arg_type.is_valid)
				{
					this.HandleArgumentType(this.GetDefField(arg_type, this.Data.Vars.obj));
				}
				else
				{
					Value value = this.Data.Vars.Get("localization_key", true);
					if (value.is_valid)
					{
						UIText.SetTextKey(this.Text_ItemName, value.String(null), this.Data.Vars, null);
					}
				}
			}
			this.PopulateAndAdjustRightText();
			string text = this.Data.Vars.Get("tooltip_def", true).String(null);
			if (!string.IsNullOrEmpty(text))
			{
				Tooltip.Get(base.gameObject, true).SetDef(text, this.Data.Vars);
			}
		}
		else
		{
			UIText.SetText(this.Text_ItemName, this.Data.Target.String(null));
		}
		if (this.Container_Icon != null)
		{
			UICommon.DeleteChildren(this.Container_Icon);
			GameObject gameObject = this.ExtractReleveantIcon(this.Data.Target, this.Container_Icon);
			if (gameObject == null)
			{
				gameObject = this.ExtractReleveantIcon(this.Data.Vars.obj, this.Container_Icon);
			}
			if (gameObject != null)
			{
				this.FitInParent(gameObject);
			}
		}
	}

	// Token: 0x06002E92 RID: 11922 RVA: 0x00180584 File Offset: 0x0017E784
	private void FitInParent(GameObject go)
	{
		RectTransform rectTransform = go.transform as RectTransform;
		if (rectTransform == null)
		{
			return;
		}
		LayoutElement component = go.GetComponent<LayoutElement>();
		float aspectRatio = (component != null) ? (component.preferredWidth / component.preferredHeight) : (rectTransform.rect.width / rectTransform.rect.height);
		AspectRatioFitter orAddComponent = go.GetOrAddComponent<AspectRatioFitter>();
		orAddComponent.aspectRatio = aspectRatio;
		orAddComponent.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
	}

	// Token: 0x06002E93 RID: 11923 RVA: 0x001805F8 File Offset: 0x0017E7F8
	private GameObject ExtractReleveantIcon(Value value, RectTransform container)
	{
		if (value.is_string)
		{
			DT.Field field = GameLogic.Get(true).dt.Find(value.String(null), null);
			if (field != null)
			{
				this.Data.Vars.Set<string>("variant", "compact");
				GameObject icon = ObjectIcon.GetIcon(field, this.Data.Vars, container);
				if (icon != null)
				{
					return icon;
				}
			}
			if (this.Data.Vars != null)
			{
				Value value2 = this.Data.Vars.Get("argument_type", true);
				if (value2.is_valid)
				{
					if (value2 == "Goods")
					{
						return UIGoodsIcon.GetIcon(this.Data.Vars.obj, null, container);
					}
					return this.HandleArgumentTypeIcon(this.GetDefField(value2, this.Data.Vars.obj), container);
				}
			}
		}
		if (value.is_object && value.obj_val is Def)
		{
			this.Data.Vars.Set<string>("variant", "compact");
			return this.HandleArgumentTypeIcon((value.obj_val as Def).field, container);
		}
		return null;
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x0018072C File Offset: 0x0017E92C
	private void UpdateAsObject()
	{
		object obj_val = this.Data.Target.obj_val;
		if (obj_val == null)
		{
			return;
		}
		UIText.SetText(this.Text_ItemName, global::Defs.LocalizedObjName(obj_val, null, "", true));
		if (this.Data.Vars != null)
		{
			this.PopulateAndAdjustRightText();
			string text = this.Data.Vars.Get("tooltip_def", true).String(null);
			if (!string.IsNullOrEmpty(text))
			{
				Tooltip.Get(base.gameObject, true).SetDef(text, this.Data.Vars);
			}
		}
		if (this.Container_Icon != null)
		{
			UICommon.DeleteChildren(this.Container_Icon);
			TargetPickerData data = this.Data;
			Value? value;
			if (((data != null) ? data.Vars : null) == null)
			{
				value = null;
			}
			else
			{
				TargetPickerData data2 = this.Data;
				if (data2 == null)
				{
					value = null;
				}
				else
				{
					Vars vars = data2.Vars;
					value = ((vars != null) ? new Value?(vars.Get("icon_variant", true)) : null);
				}
			}
			Value? value2 = value;
			string text2 = (value2 != null) ? value2.GetValueOrDefault() : null;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "compact";
			}
			Vars vars2 = new Vars(obj_val);
			vars2.Set<string>("variant", text2);
			GameObject icon = ObjectIcon.GetIcon(obj_val, vars2, this.Container_Icon);
			if (icon == null)
			{
				return;
			}
			this.FitInParent(icon);
			if (obj_val is Logic.Character)
			{
				UICharacterIcon component = icon.GetComponent<UICharacterIcon>();
				if (component != null)
				{
					component.ShowCrest(false);
					component.ShowCrown(false);
					component.ShowStatus(false);
					component.ShowMissonKingdomCrest(false);
					component.ShowPrisonKingdomCrest(false);
				}
			}
			if (obj_val is War)
			{
				UIWarIcon component2 = icon.GetComponent<UIWarIcon>();
				if (component2 != null)
				{
					TargetPickerData data3 = this.Data;
					if (((data3 != null) ? data3.Vars : null) != null)
					{
						Value value3 = this.Data.Vars.Get("target", true);
						if (value3.is_object && value3.obj_val is Logic.Kingdom)
						{
							component2.SetObserver(value3.obj_val as Logic.Kingdom);
						}
					}
				}
			}
		}
		if (this.AdditinalData_IconContainer != null)
		{
			UICommon.DeleteChildren(this.AdditinalData_IconContainer);
			if (this.Data.Vars != null)
			{
				Logic.Object @object = this.Data.Vars.Get<Logic.Object>("additinalObject", null);
				if (@object != null)
				{
					Vars vars3 = new Vars();
					vars3.Set<string>("variant", "secondary_target");
					GameObject icon2 = ObjectIcon.GetIcon(@object, vars3, this.AdditinalData_IconContainer);
					if (icon2 == null)
					{
						return;
					}
					UICommon.FillParent(icon2.transform as RectTransform);
				}
			}
			this.AdditinalData_IconContainer.gameObject.SetActive(this.AdditinalData_IconContainer.transform.childCount > 0);
		}
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x001809FC File Offset: 0x0017EBFC
	public bool IsValid()
	{
		return this.Data != null && (this.Data.Validate == null || this.Data.Validate(this.Data.Target));
	}

	// Token: 0x06002E96 RID: 11926 RVA: 0x00180A37 File Offset: 0x0017EC37
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x00180A46 File Offset: 0x0017EC46
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x00180A55 File Offset: 0x0017EC55
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this);
		}
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x00180A72 File Offset: 0x0017EC72
	public override void OnDoubleClick(PointerEventData e)
	{
		base.OnDoubleClick(e);
		if (this.OnConfirm != null)
		{
			this.OnConfirm(this);
		}
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x00180A90 File Offset: 0x0017EC90
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_BackgroundImage != null)
		{
			if (this.m_Selected)
			{
				if (this.mouse_in)
				{
					this.m_BackgroundImage.color = this.selected_over;
					return;
				}
				this.m_BackgroundImage.color = this.selected;
				return;
			}
			else
			{
				if (this.mouse_in)
				{
					this.m_BackgroundImage.color = this.over;
					return;
				}
				this.m_BackgroundImage.color = this.normal;
			}
		}
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x00180B14 File Offset: 0x0017ED14
	public void Release()
	{
		this.Data = null;
		this.Text_ItemName.text = string.Empty;
		this.Text_Right.text = string.Empty;
		this.m_Selected = false;
		this.OnSelect = null;
		this.OnConfirm = null;
		global::Common.DestroyObj(Tooltip.Get(base.gameObject, false));
		UICommon.DeleteChildren(this.Container_Icon);
		UICommon.DeleteChildren(this.AdditinalData_IconContainer);
		this.AdditinalData_IconContainer.gameObject.SetActive(false);
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x00180B98 File Offset: 0x0017ED98
	public static UITargetItem Create(TargetPickerData data, GameObject prototype, RectTransform parent, Vars vars = null)
	{
		if (prototype == null)
		{
			Debug.Log("Fail to create UITargetItem! Reason prorotype is null");
			return null;
		}
		if (parent == null)
		{
			Debug.Log("Fail to create UITargetItem! Reason parent is null");
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
		UITargetItem uitargetItem = gameObject.GetComponent<UITargetItem>();
		if (uitargetItem == null)
		{
			uitargetItem = gameObject.AddComponent<UITargetItem>();
		}
		uitargetItem.Data = data;
		uitargetItem.Refresh();
		return uitargetItem;
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x00180C04 File Offset: 0x0017EE04
	public bool MatchData(TargetPickerData t)
	{
		return this.Data != null && !this.Data.Target.is_valid && t.Target.is_valid && !(t.Target != this.Data.Target) && (t.Vars == null || t.Vars == this.Data.Vars);
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x06002E9F RID: 11935 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x00180C76 File Offset: 0x0017EE76
	public void OnPoolDeactivated()
	{
		this.Release();
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x04001F7C RID: 8060
	[SerializeField]
	private Color normal = Color.white;

	// Token: 0x04001F7D RID: 8061
	[SerializeField]
	private Color over = new Color(0.745283f, 0.745283f, 0.745283f, 1f);

	// Token: 0x04001F7E RID: 8062
	[SerializeField]
	private Color selected = new Color(0.6132076f, 0.6132076f, 0.6132076f, 1f);

	// Token: 0x04001F7F RID: 8063
	[SerializeField]
	private Color selected_over = new Color(0.7735849f, 0.7735849f, 0.7735849f, 1f);

	// Token: 0x04001F80 RID: 8064
	[UIFieldTarget("id_IconContainer")]
	[SerializeField]
	private RectTransform Container_Icon;

	// Token: 0x04001F81 RID: 8065
	[UIFieldTarget("id_AdditinalData_IconContainer")]
	[SerializeField]
	private RectTransform AdditinalData_IconContainer;

	// Token: 0x04001F82 RID: 8066
	[UIFieldTarget("id_ItemName")]
	[SerializeField]
	private TextMeshProUGUI Text_ItemName;

	// Token: 0x04001F83 RID: 8067
	[UIFieldTarget("id_RightText")]
	[SerializeField]
	private TextMeshProUGUI Text_Right;

	// Token: 0x04001F84 RID: 8068
	[UIFieldTarget("id_ItemBackground")]
	[SerializeField]
	private Image m_BackgroundImage;

	// Token: 0x04001F85 RID: 8069
	private LayoutElement m_RightTextLayoutElement;

	// Token: 0x04001F87 RID: 8071
	public Action<UITargetItem> OnSelect;

	// Token: 0x04001F88 RID: 8072
	public Action<UITargetItem> OnConfirm;

	// Token: 0x04001F89 RID: 8073
	private bool m_Selected;

	// Token: 0x04001F8A RID: 8074
	private bool m_Initialzied;

	// Token: 0x04001F8B RID: 8075
	private float m_UpdateInterval = 1f;

	// Token: 0x04001F8C RID: 8076
	private float m_LastUpdate;
}
