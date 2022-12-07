using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000288 RID: 648
public class UICourtMemberActivePlotIcon : Hotspot, IListener
{
	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06002793 RID: 10131 RVA: 0x00157B02 File Offset: 0x00155D02
	// (set) Token: 0x06002794 RID: 10132 RVA: 0x00157B0A File Offset: 0x00155D0A
	public Logic.Character Character { get; private set; }

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x06002795 RID: 10133 RVA: 0x00157B13 File Offset: 0x00155D13
	// (set) Token: 0x06002796 RID: 10134 RVA: 0x00157B1B File Offset: 0x00155D1B
	public Vars Vars { get; private set; }

	// Token: 0x06002797 RID: 10135 RVA: 0x00157B24 File Offset: 0x00155D24
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initalized = true;
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x00157B3D File Offset: 0x00155D3D
	public void SetData(Logic.Character character, Vars vars)
	{
		this.Init();
		this.RemoveListeners();
		this.Character = character;
		this.Vars = vars;
		this.AddListeners();
		this.Refresh();
		this.UpdateHighlight();
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x00157B6B File Offset: 0x00155D6B
	private void LateUpdate()
	{
		this.UpdateProgress();
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x00157B74 File Offset: 0x00155D74
	private void UpdateProgress()
	{
		if (this.m_ActionProgress == null)
		{
			return;
		}
		Logic.Character character = this.Character;
		if (((character != null) ? character.cur_action : null) == null)
		{
			return;
		}
		float num;
		float num2;
		this.Character.cur_action.GetProgress(out num, out num2);
		this.m_ActionProgress.fillAmount = ((num2 != 0f) ? (num / num2) : 0f);
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x00157BD8 File Offset: 0x00155DD8
	private void Refresh()
	{
		if (this.Character == null)
		{
			return;
		}
		if (this.Character.cur_action == null)
		{
			return;
		}
		if (this.m_Icon != null && this.m_Icon.Length != 0)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(this.Character.cur_action.def.dt_def.field, "icon", this.Character.cur_action);
			for (int i = 0; i < this.m_Icon.Length; i++)
			{
				this.m_Icon[i].overrideSprite = obj;
			}
		}
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x00157C5E File Offset: 0x00155E5E
	public void Enabled(bool enabled)
	{
		if (this.m_Enabled == enabled)
		{
			return;
		}
		this.m_Enabled = enabled;
		this.UpdateHighlight();
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x00157C77 File Offset: 0x00155E77
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x00157C90 File Offset: 0x00155E90
	public bool HaveUnseenOpportunities()
	{
		Logic.Character character = this.Character;
		List<Opportunity> list;
		if (character == null)
		{
			list = null;
		}
		else
		{
			Actions actions = character.actions;
			list = ((actions != null) ? actions.opportunities : null);
		}
		List<Opportunity> list2 = list;
		if (list2 != null && list2.Count > 0)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				Opportunity opportunity = list2[i];
				if (opportunity.active && opportunity.action != null && !opportunity.action.is_active && !opportunity.seen)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x00157D09 File Offset: 0x00155F09
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x00157D18 File Offset: 0x00155F18
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060027A1 RID: 10145 RVA: 0x00157D27 File Offset: 0x00155F27
	public override void OnClick(PointerEventData e)
	{
		if (!this.m_Enabled)
		{
			return;
		}
		base.OnClick(e);
		if (this.OnSelected != null)
		{
			this.OnSelected(this, e);
		}
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x00157D50 File Offset: 0x00155F50
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_GroupNormal != null)
		{
			this.m_GroupNormal.gameObject.SetActive(!this.m_Selected);
		}
		if (this.m_GroupSelected != null)
		{
			this.m_GroupSelected.gameObject.SetActive(this.m_Selected);
		}
		if (this.m_Glow != null && this.m_Glow.Length != 0)
		{
			for (int i = 0; i < this.m_Glow.Length; i++)
			{
				this.m_Glow[i].SetActive(this.mouse_in);
			}
		}
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x00157DE7 File Offset: 0x00155FE7
	public void Close()
	{
		this.RemoveListeners();
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x00157DFA File Offset: 0x00155FFA
	private void AddListeners()
	{
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.AddListener(this);
		}
		Logic.Character character2 = this.Character;
		if (character2 == null)
		{
			return;
		}
		Logic.Kingdom kingdom = character2.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.RoyalFamily royalFamily = kingdom.royalFamily;
		if (royalFamily == null)
		{
			return;
		}
		royalFamily.AddListener(this);
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x00157E33 File Offset: 0x00156033
	private void RemoveListeners()
	{
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.DelListener(this);
		}
		Logic.Character character2 = this.Character;
		if (character2 == null)
		{
			return;
		}
		Logic.Kingdom kingdom = character2.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.RoyalFamily royalFamily = kingdom.royalFamily;
		if (royalFamily == null)
		{
			return;
		}
		royalFamily.DelListener(this);
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x00157E6C File Offset: 0x0015606C
	private void OnDestroy()
	{
		this.RemoveListeners();
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x00157E74 File Offset: 0x00156074
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberActivePlotIcon", null);
	}

	// Token: 0x060027A9 RID: 10153 RVA: 0x00157E81 File Offset: 0x00156081
	public static UICourtMemberActivePlotIcon Create(Logic.Character character, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberActivePlotIcon orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberActivePlotIcon>();
		orAddComponent.SetData(character, vars);
		return orAddComponent;
	}

	// Token: 0x04001AF1 RID: 6897
	[UIFieldTarget("id_GroupNormal")]
	private GameObject m_GroupNormal;

	// Token: 0x04001AF2 RID: 6898
	[UIFieldTarget("id_GroupSelected")]
	private GameObject m_GroupSelected;

	// Token: 0x04001AF3 RID: 6899
	[UIFieldTarget("id_Icon")]
	private Image[] m_Icon;

	// Token: 0x04001AF4 RID: 6900
	[UIFieldTarget("id_Glow")]
	private GameObject[] m_Glow;

	// Token: 0x04001AF5 RID: 6901
	[UIFieldTarget("id_ActionProgressValue")]
	private Image m_ActionProgress;

	// Token: 0x04001AF8 RID: 6904
	private bool m_Selected;

	// Token: 0x04001AF9 RID: 6905
	private bool m_Enabled = true;

	// Token: 0x04001AFA RID: 6906
	private bool m_Initalized;

	// Token: 0x04001AFB RID: 6907
	public Action<UICourtMemberActivePlotIcon, PointerEventData> OnSelected;
}
