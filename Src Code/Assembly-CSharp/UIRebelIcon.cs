using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001F3 RID: 499
public class UIRebelIcon : ObjectIcon, IListener
{
	// Token: 0x1700018E RID: 398
	// (get) Token: 0x06001E1B RID: 7707 RVA: 0x00119310 File Offset: 0x00117510
	// (set) Token: 0x06001E1C RID: 7708 RVA: 0x00119318 File Offset: 0x00117518
	public Logic.Rebel Data { get; private set; }

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x06001E1D RID: 7709 RVA: 0x00119321 File Offset: 0x00117521
	// (set) Token: 0x06001E1E RID: 7710 RVA: 0x00119329 File Offset: 0x00117529
	public Vars Vars { get; private set; }

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x06001E1F RID: 7711 RVA: 0x00119334 File Offset: 0x00117534
	// (remove) Token: 0x06001E20 RID: 7712 RVA: 0x0011936C File Offset: 0x0011756C
	public event Action<UIRebelIcon> OnSelect;

	// Token: 0x14000020 RID: 32
	// (add) Token: 0x06001E21 RID: 7713 RVA: 0x001193A4 File Offset: 0x001175A4
	// (remove) Token: 0x06001E22 RID: 7714 RVA: 0x001193DC File Offset: 0x001175DC
	public event Action<UIRebelIcon> OnFocus;

	// Token: 0x06001E23 RID: 7715 RVA: 0x00112285 File Offset: 0x00110485
	public override void Awake()
	{
		base.Awake();
		if (this.logicObject == null)
		{
			this.SetObject(null, null);
		}
	}

	// Token: 0x06001E24 RID: 7716 RVA: 0x00119411 File Offset: 0x00117611
	private void OnDestroy()
	{
		this.OnSelect = null;
		this.OnFocus = null;
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x00119435 File Offset: 0x00117635
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzed = true;
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x00119450 File Offset: 0x00117650
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
			Tooltip.Get(base.gameObject, true).SetObj(obj, null, null);
			if (obj is Logic.Rebel)
			{
				this.Data = (obj as Logic.Rebel);
				this.Data.AddListener(this);
				if (this.m_LeaderIcon != null)
				{
					ObjectIcon leaderIcon = this.m_LeaderIcon;
					Logic.Rebel data = this.Data;
					object obj2;
					if (data == null)
					{
						obj2 = null;
					}
					else
					{
						Logic.Army army = data.army;
						obj2 = ((army != null) ? army.leader : null);
					}
					leaderIcon.SetObject(obj2, null);
					this.m_LeaderIcon.OnSelect += this.LeaderOnSelect;
					this.m_LeaderIcon.OnFocus += this.LeaderOnFocus;
					this.m_LeaderIcon.DisableTooltip(true);
				}
			}
		}
		else
		{
			this.Data = null;
			Tooltip tooltip = Tooltip.Get(base.gameObject, false);
			if (tooltip != null)
			{
				UnityEngine.Object.Destroy(tooltip);
			}
		}
		this.Refresh();
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x00119573 File Offset: 0x00117773
	private void LeaderOnFocus(UICharacterIcon obj)
	{
		if (this.OnFocus != null)
		{
			this.OnFocus(this);
			return;
		}
		this.ExecuteDefaultSelectAndFcousAction();
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x00119590 File Offset: 0x00117790
	private void LeaderOnSelect(UICharacterIcon obj)
	{
		if (this.OnSelect != null)
		{
			this.OnSelect(this);
			return;
		}
		this.ExecuteDefaultSelectAction();
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x001195AD File Offset: 0x001177AD
	public void Select(bool selected)
	{
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x001195BC File Offset: 0x001177BC
	public void Refresh()
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
			if (this.m_Leader != null)
			{
				this.m_Leader.gameObject.SetActive(this.Data.IsLeader());
			}
			if (this.m_General != null)
			{
				this.m_General.gameObject.SetActive(this.Data.IsGeneral() && !this.Data.IsLeader());
			}
		}
		this.UpdateHighlight();
	}

	// Token: 0x06001E2B RID: 7723 RVA: 0x001196C0 File Offset: 0x001178C0
	private void UpdatePowerValue()
	{
		Logic.Rebel data = this.Data;
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x001196CC File Offset: 0x001178CC
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
			bool flag = true;
			if (this.OnSelect != null)
			{
				flag = false;
				this.OnSelect(this);
			}
			if (this.OnFocus != null)
			{
				flag = false;
				this.OnFocus(this);
			}
			if (flag)
			{
				this.ExecuteDefaultSelectAndFcousAction();
			}
		}
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x00119740 File Offset: 0x00117940
	public void ExecuteDefaultSelectAction()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.Data.army != null)
		{
			BaseUI baseUI = WorldUI.Get();
			Logic.Army army = this.Data.army;
			baseUI.SelectObj((((army != null) ? army.visuals : null) as global::Army).gameObject, false, true, true, true);
		}
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x00119794 File Offset: 0x00117994
	public void ExecuteDefaultSelectAndFcousAction()
	{
		if (this.Data == null)
		{
			return;
		}
		Logic.Rebel data = this.Data;
		if (((data != null) ? data.army : null) != null)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			Logic.Army army = this.Data.army;
			worldUI.LookAt((((army != null) ? army.visuals : null) as global::Army).transform.position, false);
		}
	}

	// Token: 0x06001E2F RID: 7727 RVA: 0x001197F4 File Offset: 0x001179F4
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001E30 RID: 7728 RVA: 0x00119803 File Offset: 0x00117A03
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x000DB26E File Offset: 0x000D946E
	public void UpdateHighlight()
	{
		bool isPlaying = Application.isPlaying;
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x00119812 File Offset: 0x00117A12
	public static UIRebelIcon Create(Logic.Rebel rebel, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIRebelIcon orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UIRebelIcon>();
		orAddComponent.SetObject(rebel, vars);
		return orAddComponent;
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x00119848 File Offset: 0x00117A48
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "rebel_defeated")
		{
			this.Refresh();
		}
	}

	// Token: 0x040013C3 RID: 5059
	[UIFieldTarget("id_LeaderIcon")]
	private UICharacterIcon m_LeaderIcon;

	// Token: 0x040013C4 RID: 5060
	[UIFieldTarget("id_Group_Empty")]
	[SerializeField]
	private RectTransform Group_Empty;

	// Token: 0x040013C5 RID: 5061
	[UIFieldTarget("id_Group_Populated")]
	[SerializeField]
	private RectTransform Group_Populated;

	// Token: 0x040013C6 RID: 5062
	[UIFieldTarget("id_Leader")]
	private RectTransform m_Leader;

	// Token: 0x040013C7 RID: 5063
	[UIFieldTarget("id_General")]
	private RectTransform m_General;

	// Token: 0x040013C8 RID: 5064
	private bool m_Selected;

	// Token: 0x040013CD RID: 5069
	private bool m_Initialzed;
}
