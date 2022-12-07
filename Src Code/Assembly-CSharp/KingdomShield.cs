using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200022C RID: 556
public class KingdomShield : ObjectIcon
{
	// Token: 0x060021B1 RID: 8625 RVA: 0x00131698 File Offset: 0x0012F898
	public override void SetObject(object obj, Vars vars)
	{
		if (obj == null)
		{
			return;
		}
		base.SetObject(obj, vars);
		this.forceIndex = -1;
		Logic.Object @object = obj as Logic.Object;
		if (@object == null)
		{
			return;
		}
		this.SetKingdom(@object);
		this.SetAudioSet("DefaultAudioSetMetal");
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x001316D5 File Offset: 0x0012F8D5
	public void SetKingdom(Logic.Object obj)
	{
		this.obj = obj;
		this.forceIndex = -1;
		Tooltip.Get(base.gameObject, true);
		this.PopulateTooltip();
		this.UpdateShield();
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x00131700 File Offset: 0x0012F900
	public void DisbaleTooltip(bool disbale)
	{
		if (disbale)
		{
			Tooltip x = Tooltip.Get(base.gameObject, false);
			if (x != null)
			{
				global::Common.DestroyObj(x);
				return;
			}
		}
		else
		{
			this.PopulateTooltip();
		}
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x00131734 File Offset: 0x0012F934
	private void PopulateTooltip()
	{
		Tooltip tooltip = Tooltip.Get(base.gameObject, true);
		Logic.Kingdom kingdom = this.obj as Logic.Kingdom;
		if (kingdom == null)
		{
			Logic.Object @object = this.obj;
			kingdom = ((@object != null) ? @object.GetKingdom() : null);
		}
		if (tooltip != null)
		{
			tooltip.SetObj(kingdom, null, null);
		}
		if (this.vars != null)
		{
			this.vars.obj = kingdom;
			if (tooltip != null)
			{
				tooltip.SetVars(this.vars);
			}
		}
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x001317A4 File Offset: 0x0012F9A4
	public void ForceSetIndex(int index)
	{
		this.forceIndex = index;
		this.obj = null;
		this.UpdateShield();
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x001317BC File Offset: 0x0012F9BC
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.onClick != null && this.onClick(e, this))
		{
			return;
		}
		if (e.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		Logic.Object @object = this.obj;
		Logic.Kingdom kingdom = (@object != null) ? @object.GetKingdom() : null;
		if (kingdom == null || kingdom.IsDefeated())
		{
			return;
		}
		int num = (kingdom != null) ? kingdom.id : 0;
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		if (worldUI.selected_kingdom == null || worldUI.selected_kingdom.id != num)
		{
			worldUI.SelectKingdom(num, true);
		}
		if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
		{
			AudienceWindow.Create(worldUI.selected_kingdom, "Main", BaseUI.LogicKingdom().visuals as global::Kingdom);
			AudienceWindow.BringToFront();
		}
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x00131890 File Offset: 0x0012FA90
	public override void OnDoubleClick(PointerEventData e)
	{
		base.OnDoubleClick(e);
		if (e.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		Logic.Object @object = this.obj;
		Logic.Kingdom kingdom = (@object != null) ? @object.GetKingdom() : null;
		if (kingdom == null || kingdom.IsDefeated())
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		Logic.Realm capital = worldUI.selected_kingdom.logic.GetCapital();
		if (capital != null)
		{
			worldUI.LookAt(capital.castle.position, false);
		}
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x0013190C File Offset: 0x0012FB0C
	private void UpdateShield()
	{
		if (this.shieldImage == null)
		{
			this.shieldImage = base.GetComponent<UIShieldRawImage>();
		}
		if (this.shieldImage == null)
		{
			return;
		}
		if (this.forceIndex > 0)
		{
			this.shieldImage.SetCrestId(this.forceIndex, "regular", "");
		}
		Logic.Object @object = this.obj;
		object obj;
		if (@object == null)
		{
			obj = null;
		}
		else
		{
			Logic.Kingdom kingdom = @object.GetKingdom();
			obj = ((kingdom != null) ? kingdom.visuals : null);
		}
		global::Kingdom kingdom2 = obj as global::Kingdom;
		if (kingdom2 != null)
		{
			this.shieldImage.SetCrestId(kingdom2.crest_id, kingdom2.GetShieldMaterialType(), null);
			return;
		}
		Logic.Object object2 = this.obj;
		Logic.Kingdom kingdom3 = (object2 != null) ? object2.GetKingdom() : null;
		if (kingdom3 == null)
		{
			return;
		}
		string mapName = null;
		if (kingdom3.game.map_name != null)
		{
			mapName = kingdom3.game.map_name;
		}
		else if (kingdom3.game.subgames != null && kingdom3.game.subgames.Count > 0 && kingdom3.game.subgames[0] != null)
		{
			mapName = kingdom3.game.subgames[0].map_name;
		}
		this.shieldImage.SetCrestId(kingdom3.CoAIndex, global::Kingdom.GetShieldMaterialType(kingdom3), mapName);
	}

	// Token: 0x040016A2 RID: 5794
	public KingdomShield.OnShieldClick onClick;

	// Token: 0x040016A3 RID: 5795
	private Logic.Object obj;

	// Token: 0x040016A4 RID: 5796
	private UIShieldRawImage shieldImage;

	// Token: 0x040016A5 RID: 5797
	private int forceIndex = -1;

	// Token: 0x0200077A RID: 1914
	// (Invoke) Token: 0x06004C36 RID: 19510
	public delegate bool OnShieldClick(PointerEventData e, KingdomShield s);
}
