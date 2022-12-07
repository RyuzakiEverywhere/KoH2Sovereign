using System;
using Logic;
using UnityEngine;

// Token: 0x020002AC RID: 684
public class UICastleLiftOccupation : MonoBehaviour
{
	// Token: 0x17000217 RID: 535
	// (get) Token: 0x06002AE0 RID: 10976 RVA: 0x0016B27E File Offset: 0x0016947E
	// (set) Token: 0x06002AE1 RID: 10977 RVA: 0x0016B286 File Offset: 0x00169486
	public Logic.Realm realm { get; private set; }

	// Token: 0x06002AE2 RID: 10978 RVA: 0x000DF44F File Offset: 0x000DD64F
	public void Awake()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x0016B290 File Offset: 0x00169490
	public void SetObject(Castle castle)
	{
		this.realm = castle.GetRealm();
		if (!this.realm.IsOccupied())
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
		actionUIIcon.SetObject(this.realm.GetComponent<Actions>().Find("LiftOccupationAction"), null);
	}

	// Token: 0x04001D11 RID: 7441
	[UIFieldTarget("id_LiftOccupationAction")]
	private UIActionIcon m_actionUIIcon;
}
