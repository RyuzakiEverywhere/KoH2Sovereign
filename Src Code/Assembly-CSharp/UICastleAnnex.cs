using System;
using Logic;
using UnityEngine;

// Token: 0x020002A6 RID: 678
public class UICastleAnnex : MonoBehaviour
{
	// Token: 0x1700020F RID: 527
	// (get) Token: 0x06002A6F RID: 10863 RVA: 0x0016898E File Offset: 0x00166B8E
	// (set) Token: 0x06002A70 RID: 10864 RVA: 0x00168996 File Offset: 0x00166B96
	public Logic.Realm realm { get; private set; }

	// Token: 0x06002A71 RID: 10865 RVA: 0x000DF44F File Offset: 0x000DD64F
	public void Awake()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06002A72 RID: 10866 RVA: 0x001689A0 File Offset: 0x00166BA0
	public void SetObject(Castle castle)
	{
		this.realm = castle.GetRealm();
		if (!this.realm.IsOccupied() || !this.RefreshAction())
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

	// Token: 0x06002A73 RID: 10867 RVA: 0x00168A00 File Offset: 0x00166C00
	public bool RefreshAction()
	{
		Actions component = this.realm.GetComponent<Actions>();
		for (int i = 0; i < this.actionNames.Length; i++)
		{
			this.action = component.Find(this.actionNames[i]);
			if (!(this.action.Validate(true) != "ok"))
			{
				break;
			}
			this.action = null;
		}
		return this.action != null;
	}

	// Token: 0x04001CC2 RID: 7362
	[UIFieldTarget("id_AnnexAction")]
	private UIActionIcon m_actionUIIcon;

	// Token: 0x04001CC3 RID: 7363
	private RectTransform m_BuildWindowContianer;

	// Token: 0x04001CC5 RID: 7365
	public Action action;

	// Token: 0x04001CC6 RID: 7366
	[NonSerialized]
	public string[] actionNames = new string[]
	{
		"AnnexRealmWithPopMajorityAction",
		"AnnexRealmRuthlesslyAction"
	};
}
