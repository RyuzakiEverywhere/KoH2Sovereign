using System;
using Logic;
using UnityEngine;

// Token: 0x020002B3 RID: 691
public class UIEstablishOrder : MonoBehaviour
{
	// Token: 0x1700021E RID: 542
	// (get) Token: 0x06002B5A RID: 11098 RVA: 0x0016EFB2 File Offset: 0x0016D1B2
	// (set) Token: 0x06002B5B RID: 11099 RVA: 0x0016EFBA File Offset: 0x0016D1BA
	public Logic.Realm realm { get; private set; }

	// Token: 0x06002B5C RID: 11100 RVA: 0x000DF44F File Offset: 0x000DD64F
	public void Awake()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x0016EFC4 File Offset: 0x0016D1C4
	public void SetObject(Castle castle)
	{
		this.realm = castle.GetRealm();
		if (!this.realm.IsDisorder() || !this.RefreshAction())
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		UIActionIcon actionUIIcon = this.m_actionUIIcon;
		if (actionUIIcon == null)
		{
			return;
		}
		actionUIIcon.SetObject(this.action, null);
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x0016F024 File Offset: 0x0016D224
	public bool RefreshAction()
	{
		Logic.Army army = this.realm.castle.army;
		Logic.Character character = (army != null) ? army.leader : null;
		if (character == null)
		{
			return false;
		}
		Actions actions = character.actions;
		this.action = ((actions != null) ? actions.Find(this.actionName) : null);
		if (this.action == null)
		{
			return false;
		}
		string a = this.action.Validate(false);
		return a == "ok" || a == "_in_progress";
	}

	// Token: 0x04001D98 RID: 7576
	[UIFieldTarget("id_EstablishOrderAction")]
	private UIActionIcon m_actionUIIcon;

	// Token: 0x04001D99 RID: 7577
	private RectTransform m_BuildWindowContianer;

	// Token: 0x04001D9B RID: 7579
	public Action action;

	// Token: 0x04001D9C RID: 7580
	[NonSerialized]
	public string actionName = "EstablishOrderAction";
}
