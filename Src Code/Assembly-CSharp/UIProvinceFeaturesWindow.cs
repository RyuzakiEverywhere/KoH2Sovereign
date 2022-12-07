using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200026F RID: 623
public class UIProvinceFeaturesWindow : UIPoliticalViewFilter
{
	// Token: 0x06002635 RID: 9781 RVA: 0x00150080 File Offset: 0x0014E280
	private GameObject SpawnPFButton(ProvinceFeature.Def featureDef, GameObject container, GameObject icon)
	{
		GameObject gameObject = global::Common.Spawn(icon, false, false);
		gameObject.transform.SetParent(container.transform, false);
		if (this.def == null)
		{
			return gameObject;
		}
		gameObject.name = featureDef.field.key;
		UIProvinceFeaturesWindow.PFIcon orAddComponent = gameObject.GetOrAddComponent<UIProvinceFeaturesWindow.PFIcon>();
		if (orAddComponent)
		{
			orAddComponent.DataDef = featureDef;
			orAddComponent.ButtonDef = this.def;
			orAddComponent.SetDef(featureDef.dt_def, BaseUI.LogicKingdom());
			orAddComponent.SetActive(UIProvinceFeaturesWindow.selectedFeatures != null && UIProvinceFeaturesWindow.selectedFeatures.Contains(featureDef.field.key));
		}
		return gameObject;
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x0015011C File Offset: 0x0014E31C
	protected override void SpawnFilterButtons()
	{
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		List<ProvinceFeature.Def> defs = game.defs.GetDefs<ProvinceFeature.Def>();
		if (defs == null || defs.Count == 0)
		{
			return;
		}
		defs.Sort(delegate(ProvinceFeature.Def x, ProvinceFeature.Def y)
		{
			string text = global::Defs.Localize(x.field, "name", null, null, true, true);
			string strB = global::Defs.Localize(y.field, "name", null, null, true, true);
			return text.CompareTo(strB);
		});
		GameObject obj = global::Defs.GetObj<GameObject>("ProvinceFeaturesViewScreen", "buttons.icon_prefab", null);
		if (obj == null || this.buttons == null)
		{
			return;
		}
		for (int i = 0; i < defs.Count; i++)
		{
			if (defs[i].show_in_political_view)
			{
				this.SpawnPFButton(defs[i], this.buttons, obj);
			}
		}
		this.ApplySelected();
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x001501D4 File Offset: 0x0014E3D4
	private void ApplySelected()
	{
		if (UIProvinceFeaturesWindow.selectedFeatures == null)
		{
			return;
		}
		ProvinceFeaturesView provinceFeaturesView = ViewMode.Get("ProvinceFeatures") as ProvinceFeaturesView;
		provinceFeaturesView.selectedFeatures = UIProvinceFeaturesWindow.selectedFeatures;
		provinceFeaturesView.Apply();
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x00150200 File Offset: 0x0014E400
	public override void SelectFilter(PoliticalViewFilterIcon icon, PointerEventData e)
	{
		ProvinceFeaturesView provinceFeaturesView = ViewMode.Get("ProvinceFeatures") as ProvinceFeaturesView;
		if (!provinceFeaturesView.IsActive())
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
		UIProvinceFeaturesWindow.selectedFeatures = new List<string>();
		for (int j = 0; j < this.selected.Count; j++)
		{
			UIProvinceFeaturesWindow.selectedFeatures.Add(this.selected[j].name);
		}
		provinceFeaturesView.selectedFeatures = UIProvinceFeaturesWindow.selectedFeatures;
		provinceFeaturesView.Apply();
	}

	// Token: 0x040019D5 RID: 6613
	private static List<string> selectedFeatures;

	// Token: 0x020007C5 RID: 1989
	public class PFIcon : PoliticalViewFilterIcon
	{
		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x06004DC9 RID: 19913 RVA: 0x0022FC78 File Offset: 0x0022DE78
		// (set) Token: 0x06004DCA RID: 19914 RVA: 0x0022FC80 File Offset: 0x0022DE80
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x06004DCB RID: 19915 RVA: 0x0022FC89 File Offset: 0x0022DE89
		// (set) Token: 0x06004DCC RID: 19916 RVA: 0x0022FC91 File Offset: 0x0022DE91
		public DT.Def Def { get; private set; }

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06004DCD RID: 19917 RVA: 0x0022FC9A File Offset: 0x0022DE9A
		// (set) Token: 0x06004DCE RID: 19918 RVA: 0x0022FCA2 File Offset: 0x0022DEA2
		public UIProvinceFeaturesWindow.PFIcon.State state { get; private set; }

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06004DCF RID: 19919 RVA: 0x0022FCAB File Offset: 0x0022DEAB
		// (set) Token: 0x06004DD0 RID: 19920 RVA: 0x0022FCB3 File Offset: 0x0022DEB3
		public DT.Field state_def { get; private set; }

		// Token: 0x06004DD1 RID: 19921 RVA: 0x0022FCBC File Offset: 0x0022DEBC
		private void Init()
		{
			if (this.m_Initialzed)
			{
				return;
			}
			this.ui_def = global::Defs.GetDefField("ResourcesSlot", null);
			UICommon.FindComponents(this, false);
			UIProvinceFeature provinceFeature = this.m_ProvinceFeature;
			provinceFeature.OnSelected = (Action<UIProvinceFeature, PointerEventData>)Delegate.Combine(provinceFeature.OnSelected, new Action<UIProvinceFeature, PointerEventData>(this.HandleOnIconSelect));
			this.m_Initialzed = true;
		}

		// Token: 0x06004DD2 RID: 19922 RVA: 0x0022FD18 File Offset: 0x0022DF18
		public void SetDef(DT.Def def, Logic.Kingdom kingodm)
		{
			this.Init();
			this.Def = def;
			this.Kingdom = kingodm;
			if (this.m_ProvinceFeature != null)
			{
				this.m_ProvinceFeature.SetObject(this.Def.field.key, null);
			}
			this.UpdateState();
			this.UpdateHighlight();
		}

		// Token: 0x06004DD3 RID: 19923 RVA: 0x0022FD70 File Offset: 0x0022DF70
		public void SetState(UIProvinceFeaturesWindow.PFIcon.State state)
		{
			if (this.state == state && this.state_def != null)
			{
				return;
			}
			this.state = state;
			if (this.ui_def == null)
			{
				this.ui_def = global::Defs.GetDefField("ProvinceFeatureSlot", null);
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

		// Token: 0x06004DD4 RID: 19924 RVA: 0x0022FE1D File Offset: 0x0022E01D
		public UIProvinceFeaturesWindow.PFIcon.State DecideState()
		{
			if (base.DataDef == null)
			{
				return UIProvinceFeaturesWindow.PFIcon.State.Available;
			}
			if (this.Kingdom != null && this.Kingdom.GetResourceInfo(base.DataDef.field.key, true, true).availability != ResourceInfo.Availability.Available)
			{
				return UIProvinceFeaturesWindow.PFIcon.State.Missing;
			}
			return UIProvinceFeaturesWindow.PFIcon.State.Available;
		}

		// Token: 0x06004DD5 RID: 19925 RVA: 0x0022FE58 File Offset: 0x0022E058
		public void UpdateState()
		{
			UIProvinceFeaturesWindow.PFIcon.State state = this.DecideState();
			this.SetState(state);
			this.UpdateVisualState();
		}

		// Token: 0x06004DD6 RID: 19926 RVA: 0x0022FE7C File Offset: 0x0022E07C
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
		}

		// Token: 0x06004DD7 RID: 19927 RVA: 0x0022F90C File Offset: 0x0022DB0C
		private void HandleOnIconSelect(UIProvinceFeature icon, PointerEventData e)
		{
			this.OnClick(e);
		}

		// Token: 0x06004DD8 RID: 19928 RVA: 0x0022FEF1 File Offset: 0x0022E0F1
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004DD9 RID: 19929 RVA: 0x0022FF00 File Offset: 0x0022E100
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004DDA RID: 19930 RVA: 0x0022FF0F File Offset: 0x0022E10F
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

		// Token: 0x04003C5F RID: 15455
		[UIFieldTarget("id_ProvinceFeature")]
		private UIProvinceFeature m_ProvinceFeature;

		// Token: 0x04003C60 RID: 15456
		[UIFieldTarget("id_Available")]
		private new GameObject m_Available;

		// Token: 0x04003C61 RID: 15457
		[UIFieldTarget("id_Focused")]
		private GameObject m_Focused;

		// Token: 0x04003C64 RID: 15460
		[HideInInspector]
		public DT.Field ui_def;

		// Token: 0x04003C67 RID: 15463
		private bool m_Initialzed;

		// Token: 0x02000A2E RID: 2606
		public enum State
		{
			// Token: 0x040046B0 RID: 18096
			Available,
			// Token: 0x040046B1 RID: 18097
			Missing
		}
	}
}
