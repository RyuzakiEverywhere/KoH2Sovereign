using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class UICourtMemberOpportunityActions : MonoBehaviour, IListener
{
	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x060027FC RID: 10236 RVA: 0x00158CC3 File Offset: 0x00156EC3
	// (set) Token: 0x060027FD RID: 10237 RVA: 0x00158CCB File Offset: 0x00156ECB
	public Logic.Character Character { get; private set; }

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x060027FE RID: 10238 RVA: 0x00158CD4 File Offset: 0x00156ED4
	// (set) Token: 0x060027FF RID: 10239 RVA: 0x00158CDC File Offset: 0x00156EDC
	public Vars Vars { get; private set; }

	// Token: 0x06002800 RID: 10240 RVA: 0x00158CE5 File Offset: 0x00156EE5
	public void SetData(Logic.Character character, Vars vars)
	{
		this.Init();
		Logic.Character character2 = this.Character;
		if (character2 != null)
		{
			character2.DelListener(this);
		}
		this.Character = character;
		Logic.Character character3 = this.Character;
		if (character3 != null)
		{
			character3.AddListener(this);
		}
		this.Vars = vars;
		this.Refresh();
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x00158D25 File Offset: 0x00156F25
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.SetupIcons();
		this.m_Initalized = true;
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x00158D44 File Offset: 0x00156F44
	private void Refresh()
	{
		this.PopulateIcons();
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x00158D4C File Offset: 0x00156F4C
	private void SetupIcons()
	{
		if (this.m_OpportunityIconsContainer == null)
		{
			return;
		}
		global::Common.FindChildrenWithComponent<UIOpportunityIcon>(this.m_OpportunityIconsContainer.gameObject, this.m_Icons);
		for (int i = 0; i < this.m_Icons.Count; i++)
		{
			this.m_Icons[i].SetObject(null, null);
		}
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x00158DA8 File Offset: 0x00156FA8
	private void PopulateIcons()
	{
		Debug.LogError("outdated code - make sure to check it if using it");
		if (this.m_OpportunityIconsContainer == null)
		{
			return;
		}
		if (this.Character == null)
		{
			return;
		}
		CharacterClass.Def class_def = this.Character.class_def;
		int? num;
		if (class_def == null)
		{
			num = null;
		}
		else
		{
			Opportunity.ClassDef opportunities = class_def.opportunities;
			num = ((opportunities != null) ? new int?(opportunities.max_count) : null);
		}
		int i = num ?? this.m_Icons.Count;
		while (i > this.m_Icons.Count)
		{
			GameObject icon = ObjectIcon.GetIcon("Opportunity", null, this.m_OpportunityIconsContainer);
			UIOpportunityIcon uiopportunityIcon = (icon != null) ? icon.GetComponent<UIOpportunityIcon>() : null;
			if (uiopportunityIcon == null)
			{
				break;
			}
			this.m_Icons.Add(uiopportunityIcon);
		}
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
		int j = 0;
		int num2 = 0;
		while (j < this.m_Icons.Count)
		{
			Opportunity opportunity = (list2 != null && list2.Count > num2) ? list2[num2] : null;
			num2++;
			if (opportunity == null || !opportunity.active)
			{
				this.m_Icons[j].SetObject(null, null);
			}
			else
			{
				string text = ((opportunity != null) ? opportunity.Validate() : null) ?? "null";
				if (text == "ok" || text.StartsWith("_", StringComparison.Ordinal))
				{
					this.m_Icons[j].SetObject(opportunity, null);
				}
				else
				{
					this.m_Icons[j].SetObject(null, null);
				}
				this.m_Icons[j].gameObject.SetActive(j < i);
				j++;
			}
		}
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000023FD File Offset: 0x000005FD
	private void Clear()
	{
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x00158F6B File Offset: 0x0015716B
	public void Close()
	{
		this.Clear();
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.DelListener(this);
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x00158F90 File Offset: 0x00157190
	private void OnDestroy()
	{
		Logic.Character character = this.Character;
		if (character == null)
		{
			return;
		}
		character.DelListener(this);
	}

	// Token: 0x06002808 RID: 10248 RVA: 0x00158FA3 File Offset: 0x001571A3
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "new_opportunity" || message == "opportunity_lost" || message == "opportunities_changed")
		{
			this.PopulateIcons();
		}
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x00158FD2 File Offset: 0x001571D2
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberOpportunityActions", null);
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x00158FE0 File Offset: 0x001571E0
	public static UICourtMemberOpportunityActions Create(Logic.Character character, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberOpportunityActions orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberOpportunityActions>();
		orAddComponent.SetData(character, vars);
		UICommon.SetAligment(orAddComponent.transform as RectTransform, TextAnchor.MiddleCenter);
		return orAddComponent;
	}

	// Token: 0x04001B28 RID: 6952
	[UIFieldTarget("id_OpportunityIconsContainer")]
	private RectTransform m_OpportunityIconsContainer;

	// Token: 0x04001B2B RID: 6955
	private List<UIOpportunityIcon> m_Icons = new List<UIOpportunityIcon>();

	// Token: 0x04001B2C RID: 6956
	private bool m_Initalized;
}
