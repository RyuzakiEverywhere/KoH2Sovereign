using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200026B RID: 619
public class UIBuildingsFilter : UIPoliticalViewFilter
{
	// Token: 0x06002618 RID: 9752 RVA: 0x0014F82C File Offset: 0x0014DA2C
	private GameObject SpawnPFButton(Building.Def buiildingDef, GameObject container, GameObject icon)
	{
		GameObject gameObject = global::Common.Spawn(icon, false, false);
		gameObject.transform.SetParent(container.transform, false);
		if (this.def == null)
		{
			return gameObject;
		}
		gameObject.name = buiildingDef.field.key;
		UIBuildingsFilter.BuildingIcon orAddComponent = gameObject.GetOrAddComponent<UIBuildingsFilter.BuildingIcon>();
		if (orAddComponent)
		{
			orAddComponent.DataDef = buiildingDef;
			orAddComponent.ButtonDef = this.def;
			orAddComponent.SetIcon(global::Defs.GetObj<Sprite>(buiildingDef.field, "icon", BaseUI.LogicKingdom()));
			orAddComponent.SetKingdom(BaseUI.LogicKingdom());
			orAddComponent.SetActive(UIBuildingsFilter.selectedBuildings != null && UIBuildingsFilter.selectedBuildings.Contains(buiildingDef));
		}
		return gameObject;
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x0014F8D4 File Offset: 0x0014DAD4
	protected override void SpawnFilterButtons()
	{
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		List<Building.Def> defs = game.defs.GetDefs<Building.Def>();
		if (defs == null || defs.Count == 0)
		{
			return;
		}
		defs.Sort(delegate(Building.Def x, Building.Def y)
		{
			string text = global::Defs.Localize(x.field, "name", null, null, true, true);
			string strB = global::Defs.Localize(y.field, "name", null, null, true, true);
			return text.CompareTo(strB);
		});
		GameObject obj = global::Defs.GetObj<GameObject>("BuildingsViewScreen", "buttons.icon_prefab", null);
		if (obj == null || this.buttons == null)
		{
			return;
		}
		for (int i = 0; i < defs.Count; i++)
		{
			if (defs[i].buildable && !defs[i].IsUpgrade())
			{
				this.SpawnPFButton(defs[i], this.buttons, obj);
			}
		}
		this.ApplySelected();
	}

	// Token: 0x0600261A RID: 9754 RVA: 0x0014F99A File Offset: 0x0014DB9A
	private void ApplySelected()
	{
		if (UIBuildingsFilter.selectedBuildings == null)
		{
			return;
		}
		BuildingsView buildingsView = ViewMode.Get("Buildings") as BuildingsView;
		buildingsView.selectedBuildings = UIBuildingsFilter.selectedBuildings;
		buildingsView.Apply();
	}

	// Token: 0x0600261B RID: 9755 RVA: 0x0014F9C4 File Offset: 0x0014DBC4
	public override void SelectFilter(PoliticalViewFilterIcon icon, PointerEventData e)
	{
		BuildingsView buildingsView = ViewMode.Get("Buildings") as BuildingsView;
		if (!buildingsView.IsActive())
		{
			return;
		}
		int num = this.selected.IndexOf(icon);
		if (num >= 0)
		{
			this.selected.RemoveAt(num);
		}
		else
		{
			this.selected.Clear();
			this.selected.Add(icon);
		}
		PoliticalViewFilterIcon[] componentsInChildren = base.GetComponentsInChildren<PoliticalViewFilterIcon>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (this.selected.IndexOf(componentsInChildren[i]) == -1)
			{
				componentsInChildren[i].SetActive(false);
			}
			else
			{
				componentsInChildren[i].SetActive(true);
			}
		}
		UIBuildingsFilter.selectedBuildings = new List<Building.Def>();
		for (int j = 0; j < this.selected.Count; j++)
		{
			UIBuildingsFilter.selectedBuildings.Add(this.selected[j].DataDef as Building.Def);
		}
		buildingsView.selectedBuildings = UIBuildingsFilter.selectedBuildings;
		buildingsView.Apply();
	}

	// Token: 0x040019CA RID: 6602
	private static List<Building.Def> selectedBuildings;

	// Token: 0x020007C1 RID: 1985
	public class BuildingIcon : PoliticalViewFilterIcon
	{
		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x06004DA2 RID: 19874 RVA: 0x0022F667 File Offset: 0x0022D867
		// (set) Token: 0x06004DA3 RID: 19875 RVA: 0x0022F66F File Offset: 0x0022D86F
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x06004DA4 RID: 19876 RVA: 0x0022F678 File Offset: 0x0022D878
		// (set) Token: 0x06004DA5 RID: 19877 RVA: 0x0022F680 File Offset: 0x0022D880
		public UIBuildingsFilter.BuildingIcon.State state { get; private set; }

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x06004DA6 RID: 19878 RVA: 0x0022F689 File Offset: 0x0022D889
		// (set) Token: 0x06004DA7 RID: 19879 RVA: 0x0022F691 File Offset: 0x0022D891
		public DT.Field state_def { get; private set; }

		// Token: 0x06004DA8 RID: 19880 RVA: 0x0022F69A File Offset: 0x0022D89A
		public void SetKingdom(Logic.Kingdom kingodm)
		{
			this.Kingdom = kingodm;
			this.UpdateState();
		}

		// Token: 0x06004DA9 RID: 19881 RVA: 0x0022F6AC File Offset: 0x0022D8AC
		public void SetState(UIBuildingsFilter.BuildingIcon.State state)
		{
			if (this.state == state && this.state_def != null)
			{
				return;
			}
			this.state = state;
			if (this.ui_def == null)
			{
				this.ui_def = global::Defs.GetDefField("PVBuildingSlot", null);
			}
			if (this.ui_def != null)
			{
				this.state_def = this.ui_def.FindChild(state.ToString(), null, true, true, true, '.');
				if (this.state_def == null)
				{
					Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
					this.state_def = this.ui_def.FindChild("State", null, true, true, true, '.');
					return;
				}
			}
			else
			{
				this.state_def = null;
			}
		}

		// Token: 0x06004DAA RID: 19882 RVA: 0x0022F759 File Offset: 0x0022D959
		public UIBuildingsFilter.BuildingIcon.State DecideState()
		{
			if (base.DataDef == null)
			{
				return UIBuildingsFilter.BuildingIcon.State.Available;
			}
			if (this.Kingdom != null && this.Kingdom.GetBuildingCount(base.DataDef.field.key) <= 0)
			{
				return UIBuildingsFilter.BuildingIcon.State.Missing;
			}
			return UIBuildingsFilter.BuildingIcon.State.Available;
		}

		// Token: 0x06004DAB RID: 19883 RVA: 0x0022F790 File Offset: 0x0022D990
		public void UpdateState()
		{
			UIBuildingsFilter.BuildingIcon.State state = this.DecideState();
			this.SetState(state);
			this.UpdateVisualState();
		}

		// Token: 0x06004DAC RID: 19884 RVA: 0x0022F7B4 File Offset: 0x0022D9B4
		private void UpdateVisualState()
		{
			if (this.state_def == null)
			{
				return;
			}
			if (this.m_Icon != null)
			{
				this.m_Icon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
			}
			if (this.m_Available != null)
			{
				this.m_Available.gameObject.SetActive(this.state == UIBuildingsFilter.BuildingIcon.State.Available);
			}
		}

		// Token: 0x04003C4D RID: 15437
		[HideInInspector]
		public DT.Field ui_def;

		// Token: 0x02000A2C RID: 2604
		public enum State
		{
			// Token: 0x040046A8 RID: 18088
			Available,
			// Token: 0x040046A9 RID: 18089
			Missing
		}
	}
}
