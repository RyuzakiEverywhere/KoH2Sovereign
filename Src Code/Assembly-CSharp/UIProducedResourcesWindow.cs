using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200026E RID: 622
public class UIProducedResourcesWindow : UIPoliticalViewFilter
{
	// Token: 0x0600262F RID: 9775 RVA: 0x0014FE30 File Offset: 0x0014E030
	private GameObject SpawnPFButton(Resource.Def goodDef, GameObject container, GameObject icon)
	{
		GameObject gameObject = global::Common.Spawn(icon, container.transform, false, "");
		if (this.def == null)
		{
			return gameObject;
		}
		gameObject.name = goodDef.field.key;
		UIProducedResourcesWindow.GoodsIcon orAddComponent = gameObject.GetOrAddComponent<UIProducedResourcesWindow.GoodsIcon>();
		if (orAddComponent)
		{
			orAddComponent.DataDef = goodDef;
			orAddComponent.ButtonDef = this.def;
			orAddComponent.SetDef(goodDef.dt_def, BaseUI.LogicKingdom());
			orAddComponent.SetActive(UIProducedResourcesWindow.selectedGoods != null && UIProducedResourcesWindow.selectedGoods.Contains(goodDef.field.key));
		}
		return gameObject;
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x0014FEC4 File Offset: 0x0014E0C4
	protected override void SpawnFilterButtons()
	{
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		List<Resource.Def> defs = game.defs.GetDefs<Resource.Def>();
		if (defs == null || defs.Count == 0)
		{
			return;
		}
		GameObject obj = global::Defs.GetObj<GameObject>("ProducedResourcesViewScreen", "buttons.icon_prefab", null);
		if (obj == null || this.buttons == null)
		{
			return;
		}
		defs.Sort(delegate(Resource.Def x, Resource.Def y)
		{
			string text = global::Defs.Localize(x.field, "name", null, null, true, true);
			string strB = global::Defs.Localize(y.field, "name", null, null, true, true);
			return text.CompareTo(strB);
		});
		for (int i = 0; i < defs.Count; i++)
		{
			this.SpawnPFButton(defs[i], this.buttons, obj);
		}
		this.ApplySelected();
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x0014FF6E File Offset: 0x0014E16E
	private void ApplySelected()
	{
		if (UIProducedResourcesWindow.selectedGoods == null)
		{
			return;
		}
		ProducedResourcesView producedResourcesView = ViewMode.Get("ProducedResources") as ProducedResourcesView;
		producedResourcesView.selectedGoods = UIProducedResourcesWindow.selectedGoods;
		producedResourcesView.Apply();
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x0014FF98 File Offset: 0x0014E198
	public override void SelectFilter(PoliticalViewFilterIcon icon, PointerEventData e)
	{
		ProducedResourcesView producedResourcesView = ViewMode.Get("ProducedResources") as ProducedResourcesView;
		if (!producedResourcesView.IsActive())
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
		UIProducedResourcesWindow.selectedGoods = new List<string>();
		for (int j = 0; j < this.selected.Count; j++)
		{
			UIProducedResourcesWindow.selectedGoods.Add(this.selected[j].name);
		}
		producedResourcesView.selectedGoods = UIProducedResourcesWindow.selectedGoods;
		producedResourcesView.Apply();
	}

	// Token: 0x040019D4 RID: 6612
	private static List<string> selectedGoods;

	// Token: 0x020007C3 RID: 1987
	public class GoodsIcon : PoliticalViewFilterIcon
	{
		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x06004DB1 RID: 19889 RVA: 0x0022F86C File Offset: 0x0022DA6C
		// (set) Token: 0x06004DB2 RID: 19890 RVA: 0x0022F874 File Offset: 0x0022DA74
		public DT.Def Def { get; private set; }

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06004DB3 RID: 19891 RVA: 0x0022F87D File Offset: 0x0022DA7D
		// (set) Token: 0x06004DB4 RID: 19892 RVA: 0x0022F885 File Offset: 0x0022DA85
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06004DB5 RID: 19893 RVA: 0x0022F88E File Offset: 0x0022DA8E
		// (set) Token: 0x06004DB6 RID: 19894 RVA: 0x0022F896 File Offset: 0x0022DA96
		public UIProducedResourcesWindow.GoodsIcon.State state { get; private set; }

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06004DB7 RID: 19895 RVA: 0x0022F89F File Offset: 0x0022DA9F
		// (set) Token: 0x06004DB8 RID: 19896 RVA: 0x0022F8A7 File Offset: 0x0022DAA7
		public DT.Field state_def { get; private set; }

		// Token: 0x06004DB9 RID: 19897 RVA: 0x0022F8B0 File Offset: 0x0022DAB0
		private void Init()
		{
			if (this.m_Initialzed)
			{
				return;
			}
			this.ui_def = global::Defs.GetDefField("ResourcesSlot", null);
			UICommon.FindComponents(this, false);
			UIGoodsIcon resourceIcon = this.m_ResourceIcon;
			resourceIcon.OnSelected = (Action<UIGoodsIcon, PointerEventData>)Delegate.Combine(resourceIcon.OnSelected, new Action<UIGoodsIcon, PointerEventData>(this.HandleOnIconSelect));
			this.m_Initialzed = true;
		}

		// Token: 0x06004DBA RID: 19898 RVA: 0x0022F90C File Offset: 0x0022DB0C
		private void HandleOnIconSelect(UIGoodsIcon icon, PointerEventData e)
		{
			this.OnClick(e);
		}

		// Token: 0x06004DBB RID: 19899 RVA: 0x0022F918 File Offset: 0x0022DB18
		public void SetDef(DT.Def def, Logic.Kingdom kingodm)
		{
			this.Init();
			this.Def = def;
			this.Kingdom = kingodm;
			if (this.m_ResourceIcon != null)
			{
				this.m_ResourceIcon.SetObject(this.Def, null);
			}
			this.UpdateState();
			this.UpdateHighlight();
		}

		// Token: 0x06004DBC RID: 19900 RVA: 0x0022F965 File Offset: 0x0022DB65
		public void SetKingdom(Logic.Kingdom kingodm)
		{
			this.Kingdom = kingodm;
			this.UpdateState();
			this.UpdateHighlight();
		}

		// Token: 0x06004DBD RID: 19901 RVA: 0x0022F97C File Offset: 0x0022DB7C
		public void SetState(UIProducedResourcesWindow.GoodsIcon.State state)
		{
			if (this.state == state && this.state_def != null)
			{
				return;
			}
			this.state = state;
			if (this.ui_def == null)
			{
				this.ui_def = global::Defs.GetDefField("ResourcesSlot", null);
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

		// Token: 0x06004DBE RID: 19902 RVA: 0x0022FA2C File Offset: 0x0022DC2C
		public UIProducedResourcesWindow.GoodsIcon.State DecideState()
		{
			if (this.Def == null)
			{
				return UIProducedResourcesWindow.GoodsIcon.State.Default;
			}
			ResourceInfo resourceInfo = this.Kingdom.GetResourceInfo(this.Def.field.key, true, true);
			if (resourceInfo != null)
			{
				ResourceInfo.Availability own_availability = resourceInfo.own_availability;
				if (resourceInfo.availability == ResourceInfo.Availability.DirectlyObtainable)
				{
					return UIProducedResourcesWindow.GoodsIcon.State.CanProduce;
				}
				if (resourceInfo.availability == ResourceInfo.Availability.IndirectlyObtainable)
				{
					return UIProducedResourcesWindow.GoodsIcon.State.CanProduce;
				}
				if (resourceInfo.availability == ResourceInfo.Availability.Available && own_availability != ResourceInfo.Availability.Available)
				{
					return UIProducedResourcesWindow.GoodsIcon.State.Imported;
				}
			}
			Logic.Kingdom kingdom = this.Kingdom;
			if (((kingdom != null) ? kingdom.GetRealmTag(this.Def.field.key) : 0) <= 0)
			{
				return UIProducedResourcesWindow.GoodsIcon.State.Missing;
			}
			return UIProducedResourcesWindow.GoodsIcon.State.Default;
		}

		// Token: 0x06004DBF RID: 19903 RVA: 0x0022FAB8 File Offset: 0x0022DCB8
		public void UpdateState()
		{
			UIProducedResourcesWindow.GoodsIcon.State state = this.DecideState();
			this.SetState(state);
			this.UpdateVisualState();
		}

		// Token: 0x06004DC0 RID: 19904 RVA: 0x0022FADC File Offset: 0x0022DCDC
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
				this.m_Available.gameObject.SetActive(this.state_def.GetBool("show_available_border", null, false, true, true, true, '.'));
			}
			if (this.m_CanProduce != null)
			{
				this.m_CanProduce.gameObject.SetActive(this.state_def.GetBool("show_can_produce_border", null, false, true, true, true, '.'));
			}
			if (this.m_Imported != null)
			{
				this.m_Imported.gameObject.SetActive(this.state_def.GetBool("show_imported", null, false, true, true, true, '.'));
			}
		}

		// Token: 0x06004DC1 RID: 19905 RVA: 0x0022FBBB File Offset: 0x0022DDBB
		public void UpdateHighlight()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			GameObject focused = this.m_Focused;
			if (focused == null)
			{
				return;
			}
			focused.SetActive(this.mouse_in);
		}

		// Token: 0x06004DC2 RID: 19906 RVA: 0x0022FBDB File Offset: 0x0022DDDB
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
			Action<UIProducedResourcesWindow.GoodsIcon, PointerEventData> onHover = this.OnHover;
			if (onHover == null)
			{
				return;
			}
			onHover(this, eventData);
		}

		// Token: 0x06004DC3 RID: 19907 RVA: 0x0022FBFC File Offset: 0x0022DDFC
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
			Action<UIProducedResourcesWindow.GoodsIcon, PointerEventData> onHover = this.OnHover;
			if (onHover == null)
			{
				return;
			}
			onHover(this, eventData);
		}

		// Token: 0x06004DC4 RID: 19908 RVA: 0x0022FC1D File Offset: 0x0022DE1D
		private void Clear()
		{
			this.Def = null;
			this.Kingdom = null;
		}

		// Token: 0x04003C52 RID: 15442
		[UIFieldTarget("id_ResourceIcon")]
		private UIGoodsIcon m_ResourceIcon;

		// Token: 0x04003C53 RID: 15443
		[UIFieldTarget("id_Focused")]
		private GameObject m_Focused;

		// Token: 0x04003C54 RID: 15444
		[UIFieldTarget("id_CanProduce")]
		private GameObject m_CanProduce;

		// Token: 0x04003C55 RID: 15445
		[UIFieldTarget("id_Imported")]
		private GameObject m_Imported;

		// Token: 0x04003C58 RID: 15448
		[HideInInspector]
		public DT.Field ui_def;

		// Token: 0x04003C5B RID: 15451
		public Action<UIProducedResourcesWindow.GoodsIcon, PointerEventData> OnHover;

		// Token: 0x04003C5C RID: 15452
		private bool m_Initialzed;

		// Token: 0x02000A2D RID: 2605
		public enum State
		{
			// Token: 0x040046AB RID: 18091
			Default,
			// Token: 0x040046AC RID: 18092
			Imported,
			// Token: 0x040046AD RID: 18093
			CanProduce,
			// Token: 0x040046AE RID: 18094
			Missing
		}
	}
}
