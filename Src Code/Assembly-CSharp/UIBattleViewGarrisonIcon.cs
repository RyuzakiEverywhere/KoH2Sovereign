using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001CB RID: 459
public class UIBattleViewGarrisonIcon : ObjectIcon
{
	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06001B03 RID: 6915 RVA: 0x00103E78 File Offset: 0x00102078
	// (remove) Token: 0x06001B04 RID: 6916 RVA: 0x00103EB0 File Offset: 0x001020B0
	public event Action<UIBattleViewGarrisonIcon, PointerEventData> onClick;

	// Token: 0x06001B05 RID: 6917 RVA: 0x00103EE5 File Offset: 0x001020E5
	public override void Awake()
	{
		base.Awake();
		UICommon.FindComponents(this, false);
		if (this.logicObject == null)
		{
			this.SetObject(null, null);
		}
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x00103F04 File Offset: 0x00102104
	public override void SetObject(object obj, Vars vars = null)
	{
		if (this.logicObject != null && this.logicObject == obj)
		{
			return;
		}
		base.SetObject(obj, vars);
		this.m_logic = (obj as Logic.Settlement);
		if (this.m_logic == null)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.Refresh();
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x00103F42 File Offset: 0x00102142
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.onClick != null)
		{
			this.onClick(this, e);
		}
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x00103F60 File Offset: 0x00102160
	private void OnDestroy()
	{
		this.onClick = null;
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x00103F69 File Offset: 0x00102169
	private void Refresh()
	{
		this.UpdateGarrisonIcon();
	}

	// Token: 0x06001B0A RID: 6922 RVA: 0x00103F74 File Offset: 0x00102174
	private void UpdateGarrisonIcon()
	{
		if (this.m_garrisonIcon == null)
		{
			return;
		}
		Sprite obj = global::Defs.GetObj<Sprite>("BattleViewGarrisonIconSettings", this.m_logic.type, null);
		if (obj == null)
		{
			Debug.LogWarning("Not found a sprite for settlement of type \"" + this.m_logic.type + "\" - selecting default option");
			obj = global::Defs.GetObj<Sprite>("BattleViewGarrisonIconSettings", "Default", null);
		}
		this.m_garrisonIcon.sprite = obj;
	}

	// Token: 0x0400119C RID: 4508
	[UIFieldTarget("id_GarrisonIcon")]
	private Image m_garrisonIcon;

	// Token: 0x0400119D RID: 4509
	public Logic.Settlement m_logic;
}
