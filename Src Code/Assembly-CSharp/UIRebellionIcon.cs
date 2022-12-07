using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001F4 RID: 500
public class UIRebellionIcon : ObjectIcon, IListener
{
	// Token: 0x17000190 RID: 400
	// (get) Token: 0x06001E35 RID: 7733 RVA: 0x0011985D File Offset: 0x00117A5D
	// (set) Token: 0x06001E36 RID: 7734 RVA: 0x00119865 File Offset: 0x00117A65
	public Rebellion Data { get; private set; }

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x06001E37 RID: 7735 RVA: 0x0011986E File Offset: 0x00117A6E
	// (set) Token: 0x06001E38 RID: 7736 RVA: 0x00119876 File Offset: 0x00117A76
	public Vars Vars { get; private set; }

	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06001E39 RID: 7737 RVA: 0x00119880 File Offset: 0x00117A80
	// (remove) Token: 0x06001E3A RID: 7738 RVA: 0x001198B8 File Offset: 0x00117AB8
	public event Action<UIRebellionIcon> OnSelect;

	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06001E3B RID: 7739 RVA: 0x001198F0 File Offset: 0x00117AF0
	// (remove) Token: 0x06001E3C RID: 7740 RVA: 0x00119928 File Offset: 0x00117B28
	public event Action<UIRebellionIcon> OnFocus;

	// Token: 0x06001E3D RID: 7741 RVA: 0x00112285 File Offset: 0x00110485
	public override void Awake()
	{
		base.Awake();
		if (this.logicObject == null)
		{
			this.SetObject(null, null);
		}
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x0011995D File Offset: 0x00117B5D
	private void OnDestroy()
	{
		this.OnSelect = null;
		this.OnFocus = null;
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x00119984 File Offset: 0x00117B84
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_LeaderIcon != null)
		{
			this.m_LeaderIcon.ShowCrest(false);
			this.m_LeaderIcon.OnSelect += this.LeaderOnSelect;
			this.m_LeaderIcon.OnFocus += this.LeaderOnFocus;
			this.m_LeaderIcon.DisableTooltip(true);
			this.m_LeaderIcon.EnableClassLevel(true);
		}
		this.m_Initialzed = true;
	}

	// Token: 0x06001E40 RID: 7744 RVA: 0x00119A08 File Offset: 0x00117C08
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		if (this.logicObject != null && this.logicObject == obj)
		{
			return;
		}
		base.SetObject(obj, vars);
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		if (this.logicObject != null)
		{
			Tooltip.Get(base.gameObject, true).SetObj(obj, null, vars);
			if (obj is Rebellion)
			{
				this.Data = (obj as Rebellion);
				this.Data.AddListener(this);
			}
		}
		else
		{
			this.Data = null;
			Tooltip tooltip = Tooltip.Get(base.gameObject, false);
			if (tooltip != null)
			{
				tooltip.Clear(true);
			}
		}
		this.Refresh();
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x00119AB4 File Offset: 0x00117CB4
	private void LeaderOnSelect(UICharacterIcon obj)
	{
		base.HandleOnClick(null);
		if (this.OnSelect != null)
		{
			this.OnSelect(this);
			return;
		}
		Rebellion data = this.Data;
		bool flag;
		if (data == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Rebel leader = data.leader;
			if (leader == null)
			{
				flag = (null != null);
			}
			else
			{
				Logic.Army army = leader.army;
				flag = (((army != null) ? army.visuals : null) != null);
			}
		}
		if (flag)
		{
			WorldUI.Get().SelectObj((this.Data.leader.army.visuals as global::Army).gameObject, false, true, true, true);
		}
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x00119B38 File Offset: 0x00117D38
	private void LeaderOnFocus(UICharacterIcon obj)
	{
		if (this.OnFocus != null)
		{
			this.OnFocus(this);
			return;
		}
		Rebellion data = this.Data;
		bool flag;
		if (data == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Rebel leader = data.leader;
			flag = (((leader != null) ? leader.army : null) != null);
		}
		if (flag)
		{
			WorldUI.Get().LookAt((this.Data.leader.army.visuals as global::Army).transform.position, false);
		}
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x00119BA9 File Offset: 0x00117DA9
	public void Select(bool selected)
	{
		this.m_Selected = selected;
		UICharacterIcon leaderIcon = this.m_LeaderIcon;
		if (leaderIcon != null)
		{
			leaderIcon.Select(selected);
		}
		this.UpdateHighlight();
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x00119BCA File Offset: 0x00117DCA
	public void ShowPowerValue(bool show)
	{
		this.m_ShowPowerValue = show;
		if (this.Group_Power != null)
		{
			this.Group_Power.gameObject.SetActive(this.m_ShowPowerValue);
		}
	}

	// Token: 0x06001E45 RID: 7749 RVA: 0x00119BF8 File Offset: 0x00117DF8
	private void UpdateLeader()
	{
		if (this.m_LeaderIcon != null)
		{
			ObjectIcon leaderIcon = this.m_LeaderIcon;
			Rebellion data = this.Data;
			object obj;
			if (data == null)
			{
				obj = null;
			}
			else
			{
				Logic.Rebel leader = data.leader;
				obj = ((leader != null) ? leader.character : null);
			}
			leaderIcon.SetObject(obj, null);
		}
		if (this.m_ClassLevel != null)
		{
			Image classLevel = this.m_ClassLevel;
			DT.Field defField = global::Defs.GetDefField("RebellionIcon", null);
			string str = "leader_class_level_borders.";
			Rebellion data2 = this.Data;
			classLevel.overrideSprite = global::Defs.GetObj<Sprite>(defField, str + ((data2 != null && data2.IsFamous()) ? "famous" : "normal"), null);
		}
	}

	// Token: 0x06001E46 RID: 7750 RVA: 0x00119C94 File Offset: 0x00117E94
	private void Refresh()
	{
		if (this.Data == null)
		{
			if (this.Group_Empty != null)
			{
				this.Group_Empty.gameObject.SetActive(true);
			}
			if (this.Group_Populated != null)
			{
				this.Group_Populated.gameObject.SetActive(false);
			}
		}
		else
		{
			if (this.Group_Empty != null)
			{
				this.Group_Empty.gameObject.SetActive(false);
			}
			if (this.Group_Populated != null)
			{
				this.Group_Populated.gameObject.SetActive(true);
			}
			if (this.m_KingdomShield != null)
			{
				this.m_KingdomShield.SetObject(this.Data, null);
			}
		}
		if (this.Group_Power != null)
		{
			this.Group_Power.gameObject.SetActive(this.m_ShowPowerValue);
			Tooltip.Get(this.Group_Power.gameObject, true).SetDef("RebellionStrengthTooltip", new Vars(this.Data));
		}
		this.UpdateLeader();
		this.UpdatePowerValue();
		this.UpdateHighlight();
	}

	// Token: 0x06001E47 RID: 7751 RVA: 0x00119DA8 File Offset: 0x00117FA8
	private void UpdatePowerValue()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_PowerValue != null)
		{
			UIText.SetText(this.m_PowerValue, this.Data.GetPower().ToString());
		}
	}

	// Token: 0x06001E48 RID: 7752 RVA: 0x00119DEC File Offset: 0x00117FEC
	public override void OnClick(PointerEventData e)
	{
		if (e.clickCount == 1)
		{
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
			else
			{
				this.ExecuteDefaultSelectAction();
			}
		}
		if (e.clickCount > 1)
		{
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
			if (this.OnFocus != null)
			{
				this.OnFocus(this);
			}
		}
	}

	// Token: 0x06001E49 RID: 7753 RVA: 0x00119E4F File Offset: 0x0011804F
	public void ExecuteDefaultSelectAction()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001E4A RID: 7754 RVA: 0x00119E56 File Offset: 0x00118056
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001E4B RID: 7755 RVA: 0x00119E65 File Offset: 0x00118065
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x000DB26E File Offset: 0x000D946E
	public void UpdateHighlight()
	{
		bool isPlaying = Application.isPlaying;
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x00119E74 File Offset: 0x00118074
	public static UIRebellionIcon Create(Rebellion rebelion, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (rebelion == null)
		{
			Debug.LogWarning("Fail to create character icon! Reson: no character data e provided.");
			return null;
		}
		if (prototype == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no prototype provided.");
			return null;
		}
		if (parent == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no parent provided.");
			return null;
		}
		UIRebellionIcon orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UIRebellionIcon>();
		orAddComponent.SetObject(rebelion, vars);
		return orAddComponent;
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x00119ED8 File Offset: 0x001180D8
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "rebellion_new_leader" || message == "rebellion_leader_started" || message == "leader_changed" || message == "rebellion_ended" || message == "rebel_type_changed" || message == "rebellion_famous_state_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x040013CE RID: 5070
	[UIFieldTarget("id_LeaderIcon")]
	private UICharacterIcon m_LeaderIcon;

	// Token: 0x040013CF RID: 5071
	[UIFieldTarget("_Crest")]
	private UIKingdomIcon m_KingdomShield;

	// Token: 0x040013D0 RID: 5072
	[UIFieldTarget("id_Group_Empty")]
	[SerializeField]
	private RectTransform Group_Empty;

	// Token: 0x040013D1 RID: 5073
	[UIFieldTarget("id_Group_Populated")]
	[SerializeField]
	private RectTransform Group_Populated;

	// Token: 0x040013D2 RID: 5074
	[UIFieldTarget("id_PowerLevel")]
	[SerializeField]
	private RectTransform Group_Power;

	// Token: 0x040013D3 RID: 5075
	[UIFieldTarget("id_PowerLevelValue")]
	[SerializeField]
	private TextMeshProUGUI m_PowerValue;

	// Token: 0x040013D4 RID: 5076
	[UIFieldTarget("id_ClassLevel")]
	[SerializeField]
	private Image m_ClassLevel;

	// Token: 0x040013D5 RID: 5077
	private bool m_Selected;

	// Token: 0x040013DA RID: 5082
	private bool m_Initialzed;

	// Token: 0x040013DB RID: 5083
	private bool m_ShowPowerValue = true;
}
