using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A3 RID: 675
internal class UIBuildingTooltip : MonoBehaviour
{
	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06002A3C RID: 10812 RVA: 0x00166EB0 File Offset: 0x001650B0
	// (set) Token: 0x06002A3D RID: 10813 RVA: 0x00166EB8 File Offset: 0x001650B8
	public Castle Castle { get; private set; }

	// Token: 0x06002A3E RID: 10814 RVA: 0x00166EC4 File Offset: 0x001650C4
	public void SetTooltip(Tooltip t, Building.Def def, Castle castle, int SlotIndex, Vars vars)
	{
		UICommon.FindComponents(this, false);
		if (this.m_StructurePrototype != null)
		{
			this.m_StructurePrototype.gameObject.SetActive(false);
		}
		if (this.m_SpacerPrototype != null)
		{
			this.m_SpacerPrototype.gameObject.SetActive(false);
		}
		this.tooltip = t;
		this.tooltip.vars = vars;
		this.Castle = castle;
		Building building;
		this.m_showCost = (SlotIndex < 0 && (this.Castle == null || this.Castle.CheckMaxInstances(def, out building)));
		this.Build();
		this.Update();
	}

	// Token: 0x06002A3F RID: 10815 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnDestroy()
	{
	}

	// Token: 0x06002A40 RID: 10816 RVA: 0x00166F64 File Offset: 0x00165164
	private void Build()
	{
		if (this.m_StructuresContainer == null)
		{
			return;
		}
		if (this.m_StructurePrototype == null)
		{
			return;
		}
		this.elements = this.GetRelevant(this.tooltip);
		if (this.elements == null || this.elements.Count == 0)
		{
			return;
		}
		UICommon.DeleteActiveChildren(this.m_StructuresContainer);
		int i = 0;
		int count = this.elements.Count;
		while (i < count)
		{
			this.elements[i].tooltip = this.tooltip;
			this.elements[i].PopulateBuilding(this.Castle, this.m_StructurePrototype, this.m_StructuresContainer);
			if (i < count - 1 && this.m_SpacerPrototype != null)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.m_SpacerPrototype, this.m_StructuresContainer).SetActive(true);
			}
			i++;
		}
		if (this.m_Footer != null)
		{
			string text = (!this.m_showCost || this.m_Cost == null) ? null : global::Defs.Localize(this.tooltip.def.field, "cost", this.tooltip.vars, null, false, true);
			string text2 = (this.m_Requires == null) ? null : global::Defs.Localize(this.tooltip.def.field, "requirements", this.tooltip.vars, null, false, true);
			bool flag = !string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2);
			this.m_Footer.SetActive(flag);
			if (flag)
			{
				UIText.SetText(this.m_Cost, text ?? "");
				UIText.SetText(this.m_Requires, text2 ?? "");
			}
		}
	}

	// Token: 0x06002A41 RID: 10817 RVA: 0x00167120 File Offset: 0x00165320
	private void RefreshTexts()
	{
		if (this.m_Footer != null)
		{
			string text = (!this.m_showCost || this.m_Cost == null) ? null : global::Defs.Localize(this.tooltip.def.field, "cost", this.tooltip.vars, null, false, true);
			string text2 = (this.m_Requires == null) ? null : global::Defs.Localize(this.tooltip.def.field, "requirements", this.tooltip.vars, null, false, true);
			bool flag = !string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2);
			this.m_Footer.SetActive(flag);
			if (flag)
			{
				UIText.SetText(this.m_Cost, text ?? "");
				UIText.SetText(this.m_Requires, text2 ?? "");
			}
		}
	}

	// Token: 0x06002A42 RID: 10818 RVA: 0x00167208 File Offset: 0x00165408
	private List<UIBuildingTooltip.BuildingData> GetRelevant(Tooltip t)
	{
		if (t == null)
		{
			return null;
		}
		Tooltip tooltip = this.tooltip;
		Vars vars = (tooltip != null) ? tooltip.vars : null;
		if (vars == null)
		{
			return null;
		}
		Building.Def def = vars.Get<Building.Def>("building", null);
		if (def == null)
		{
			return null;
		}
		Building building = vars.Get<Building>("instance", null);
		UIBuildingTooltip.BuildingData buildingData = new UIBuildingTooltip.BuildingData();
		buildingData.Def = def;
		buildingData.Inst = building;
		buildingData.focused = (building != null);
		return new List<UIBuildingTooltip.BuildingData>
		{
			buildingData
		};
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x00167284 File Offset: 0x00165484
	public void Update()
	{
		if (this.tooltip == null)
		{
			return;
		}
		this.RefreshTexts();
		if (this.elements != null && this.elements.Count > 0)
		{
			for (int i = 0; i < this.elements.Count; i++)
			{
				this.elements[i].Update();
			}
		}
	}

	// Token: 0x04001C8F RID: 7311
	[UIFieldTarget("id_StructuresContainer")]
	private RectTransform m_StructuresContainer;

	// Token: 0x04001C90 RID: 7312
	[UIFieldTarget("id_StructurePrototype")]
	private GameObject m_StructurePrototype;

	// Token: 0x04001C91 RID: 7313
	[UIFieldTarget("id_Spacer")]
	private GameObject m_SpacerPrototype;

	// Token: 0x04001C92 RID: 7314
	[UIFieldTarget("id_Cost")]
	private TextMeshProUGUI m_Cost;

	// Token: 0x04001C93 RID: 7315
	[UIFieldTarget("id_Requires")]
	private TextMeshProUGUI m_Requires;

	// Token: 0x04001C94 RID: 7316
	[UIFieldTarget("id_Footer")]
	private GameObject m_Footer;

	// Token: 0x04001C96 RID: 7318
	private Tooltip tooltip;

	// Token: 0x04001C97 RID: 7319
	private List<UIBuildingTooltip.BuildingData> elements;

	// Token: 0x04001C98 RID: 7320
	private bool m_showCost;

	// Token: 0x02000805 RID: 2053
	private class BuildingData
	{
		// Token: 0x06004F4B RID: 20299 RVA: 0x000023FD File Offset: 0x000005FD
		public void Update()
		{
		}

		// Token: 0x06004F4C RID: 20300 RVA: 0x00234CC8 File Offset: 0x00232EC8
		public void PopulateBuilding(Castle castle, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return;
			}
			if (parent == null)
			{
				return;
			}
			if (castle == null)
			{
				return;
			}
			if (this.Def == null)
			{
				return;
			}
			this.Castle = castle;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent);
			gameObject.SetActive(true);
			Image image = global::Common.FindChildComponent<Image>(gameObject, "id_StructureIcon");
			Image image2 = global::Common.FindChildComponent<Image>(gameObject, "id_SelectionBorder");
			TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_StructureName");
			TextMeshProUGUI textMeshProUGUI2 = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_StructureDescripion");
			TextMeshProUGUI textMeshProUGUI3 = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_StructureParams");
			GameObject gameObject2 = global::Common.FindChildByName(gameObject, "id_BuildCheck", true, true);
			global::Common.FindChildComponent<Mask>(gameObject, "id_Mask");
			global::Common.FindChildByName(gameObject, "id_Border_Building", true, true);
			global::Common.FindChildByName(gameObject, "id_Border_District", true, true);
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
			if (image2 != null)
			{
				image2.gameObject.SetActive(this.Castle.MayBuildBuilding(this.Def, true));
			}
			if (image != null && this.Def != null)
			{
				image.overrideSprite = global::Defs.GetObj<Sprite>(this.Def.field, "icon", null);
			}
			Vars vars = this.tooltip.vars;
			if (textMeshProUGUI != null)
			{
				UIText.SetText(textMeshProUGUI, global::Defs.Localize(this.tooltip.def.field, "caption", vars, null, false, true) ?? "");
			}
			if (textMeshProUGUI2 != null)
			{
				UIText.SetText(textMeshProUGUI2, global::Defs.Localize(this.tooltip.def.field, "flavor", vars, null, false, true) ?? "");
			}
			if (textMeshProUGUI3 != null)
			{
				string form = (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheatmode building text", true)) ? "alt" : null;
				UIText.SetText(textMeshProUGUI3, global::Defs.Localize(this.tooltip.def.field, "text", vars, form, false, true) ?? "");
			}
		}

		// Token: 0x04003D66 RID: 15718
		public Castle Castle;

		// Token: 0x04003D67 RID: 15719
		public Building Inst;

		// Token: 0x04003D68 RID: 15720
		public Building.Def Def;

		// Token: 0x04003D69 RID: 15721
		public Tooltip tooltip;

		// Token: 0x04003D6A RID: 15722
		public bool focused;
	}
}
