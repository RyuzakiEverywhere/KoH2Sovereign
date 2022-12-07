using System;
using Logic;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class PVFigureSettlement : PVFigureIcon
{
	// Token: 0x06001205 RID: 4613 RVA: 0x000BD299 File Offset: 0x000BB499
	public override bool IsVisible()
	{
		return this.parent != null || base.IsVisible();
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x000BD2B4 File Offset: 0x000BB4B4
	public void SetSettlement(global::Settlement settlement)
	{
		if (settlement == null)
		{
			return;
		}
		if (settlement.IsCastle())
		{
			this.settlementType = PVFigureSettlement.SettlementType.Castle;
			base.SetAllowedType(ViewMode.AllowedFigures.Castle);
			base.UpdateVisibilityFromObject(true);
		}
		else
		{
			this.settlementType = PVFigureSettlement.SettlementType.Settlement;
			base.SetAllowedType(ViewMode.AllowedFigures.None);
			base.UpdateVisibilityFromObject(false);
		}
		if (this.settlement == null)
		{
			this.Init();
		}
		this.settlement = settlement;
		Vector3 position = settlement.transform.position;
		base.gameObject.transform.position = new Vector3(position.x, (float)base.transform.GetSiblingIndex() * 0.001f, position.z);
		this.Refresh();
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x000BD360 File Offset: 0x000BB560
	public override void RefreshDefField()
	{
		string text = this.settlementType.ToString();
		if (this.field != null && this.field.key == text)
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("PoliticalView", "pv_figures.Settlement");
		this.field = defField.FindChild(text, null, true, true, true, '.');
		base.RefreshDefField();
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x000BD3C4 File Offset: 0x000BB5C4
	public override Texture2D GetIconTexture()
	{
		return global::Defs.GetObj<Texture2D>(this.field, "texture", null);
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x000BD3D7 File Offset: 0x000BB5D7
	public override void Refresh()
	{
		CrestObject crest = this.crest;
		if (crest != null)
		{
			crest.SetKingdomId(this.settlement.logic.kingdom_id);
		}
		base.Refresh();
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDestroy()
	{
	}

	// Token: 0x04000C24 RID: 3108
	public global::Settlement settlement;

	// Token: 0x04000C25 RID: 3109
	public PVFigureSettlement.SettlementType settlementType;

	// Token: 0x02000688 RID: 1672
	public enum SettlementType
	{
		// Token: 0x040035DA RID: 13786
		Castle,
		// Token: 0x040035DB RID: 13787
		Settlement,
		// Token: 0x040035DC RID: 13788
		Count
	}
}
