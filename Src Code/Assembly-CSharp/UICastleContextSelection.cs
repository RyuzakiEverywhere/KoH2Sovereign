using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020002A9 RID: 681
public class UICastleContextSelection : MonoBehaviour, IListener
{
	// Token: 0x17000213 RID: 531
	// (get) Token: 0x06002AAB RID: 10923 RVA: 0x00169FC0 File Offset: 0x001681C0
	// (set) Token: 0x06002AAC RID: 10924 RVA: 0x00169FC8 File Offset: 0x001681C8
	public Castle Castle { get; private set; }

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x06002AAD RID: 10925 RVA: 0x00169FD1 File Offset: 0x001681D1
	// (set) Token: 0x06002AAE RID: 10926 RVA: 0x00169FD9 File Offset: 0x001681D9
	public Logic.Army Army { get; private set; }

	// Token: 0x06002AAF RID: 10927 RVA: 0x00169FE4 File Offset: 0x001681E4
	public void SetCastle(Castle castle)
	{
		UITabController selectionTab = this.m_SelectionTab;
		int index = (selectionTab != null) ? selectionTab.GetTabIndex() : 0;
		if (this.Castle != null)
		{
			this.Castle.DelListener(this);
		}
		this.Castle = castle;
		if (this.Castle != null)
		{
			this.Castle.AddListener(this);
		}
		UICommon.FindComponents(this, false);
		this.Init();
		this.Refresh();
		this.m_SelectionTab.Select(index);
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x0016A054 File Offset: 0x00168254
	private void Init()
	{
		this.tabBindidng.Clear();
		if (this.m_SelectionTab != null)
		{
			if (this.m_TabBuildingsPrototype != null)
			{
				UITabButton button = UITabButton.Possses(this.m_SelectionTab, this.m_TabBuildingsPrototype.gameObject, new UITabButton.Data
				{
					Text = "Buildings.caption",
					Vars = new Vars(this.Castle),
					Callback = delegate()
					{
						this.UpdatePanel("Buildings");
					}
				});
				this.tabBindidng.Add(new UICastleContextSelection.TabBindData
				{
					button = button,
					panel = this.m_BuildingsPanel
				});
			}
			if (this.m_TabProvincePrototype != null)
			{
				UITabButton button2 = UITabButton.Possses(this.m_SelectionTab, this.m_TabProvincePrototype.gameObject, new UITabButton.Data
				{
					Text = "Province.caption",
					Vars = new Vars(this.Castle),
					Callback = delegate()
					{
						this.UpdatePanel("Province");
					}
				});
				this.tabBindidng.Add(new UICastleContextSelection.TabBindData
				{
					button = button2,
					panel = this.m_Province
				});
			}
		}
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x0016A180 File Offset: 0x00168380
	private void UpdatePanel(string cat)
	{
		GameObject buildingsPanel = this.m_BuildingsPanel;
		if (buildingsPanel != null)
		{
			buildingsPanel.SetActive(cat == "Buildings");
		}
		GameObject districtsSimplifedPanel = this.m_DistrictsSimplifedPanel;
		if (districtsSimplifedPanel != null)
		{
			districtsSimplifedPanel.SetActive(cat == "DistrictsSimplified");
		}
		GameObject province = this.m_Province;
		if (province == null)
		{
			return;
		}
		province.SetActive(cat == "Province");
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000023FD File Offset: 0x000005FD
	private void Refresh()
	{
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x0016A1E0 File Offset: 0x001683E0
	public void OnMessage(object obj, string message, object param)
	{
		if (obj is Castle && message == "structures_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x0016A1FD File Offset: 0x001683FD
	private void OnDestroy()
	{
		if (this.Castle != null)
		{
			this.Castle.DelListener(this);
		}
		if (this.Army != null)
		{
			this.Army.DelListener(this);
		}
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x0016A227 File Offset: 0x00168427
	public int GetStoreState()
	{
		if (this.m_SelectionTab != null)
		{
			return this.m_SelectionTab.GetTabIndex();
		}
		return 0;
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x0016A244 File Offset: 0x00168444
	public void SetlectTab(int tab_index)
	{
		if (this.m_SelectionTab != null)
		{
			this.m_SelectionTab.Select(tab_index);
		}
	}

	// Token: 0x04001CE4 RID: 7396
	[UIFieldTarget("id_SelectionTab")]
	private UITabController m_SelectionTab;

	// Token: 0x04001CE5 RID: 7397
	[UIFieldTarget("id_Buildings")]
	private GameObject m_BuildingsPanel;

	// Token: 0x04001CE6 RID: 7398
	[UIFieldTarget("id_BuildingsSimplifed")]
	private GameObject m_DistrictsSimplifedPanel;

	// Token: 0x04001CE7 RID: 7399
	[UIFieldTarget("id_Province")]
	private GameObject m_Province;

	// Token: 0x04001CE8 RID: 7400
	[UIFieldTarget("id_TabBuildingsPrototype")]
	private UITabButton m_TabBuildingsPrototype;

	// Token: 0x04001CE9 RID: 7401
	[UIFieldTarget("id_TabProvincePrototype")]
	private UITabButton m_TabProvincePrototype;

	// Token: 0x04001CEA RID: 7402
	public List<UICastleContextSelection.TabBindData> tabBindidng = new List<UICastleContextSelection.TabBindData>();

	// Token: 0x02000807 RID: 2055
	[Serializable]
	public class TabBindData
	{
		// Token: 0x04003D70 RID: 15728
		public UITabButton button;

		// Token: 0x04003D71 RID: 15729
		public GameObject panel;
	}
}
