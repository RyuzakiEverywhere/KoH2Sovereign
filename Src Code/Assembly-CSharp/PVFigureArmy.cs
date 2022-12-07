using System;
using Logic;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class PVFigureArmy : PVFigureIcon
{
	// Token: 0x060011E1 RID: 4577 RVA: 0x000BC3C9 File Offset: 0x000BA5C9
	public override bool IsVisible()
	{
		return this.parent != null || (base.IsVisible() && !this.inBattle);
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x000BC3EE File Offset: 0x000BA5EE
	public void SetArmy(global::Army army)
	{
		if (army == null)
		{
			return;
		}
		if (this.army == null)
		{
			this.Init();
		}
		this.army = army;
		this.UpdateArmy();
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x000BC41B File Offset: 0x000BA61B
	public override void Init()
	{
		this.last_icon_hostilityType = PVFigureArmy.HostilityType.Count;
		this.last_def_hostilityType = PVFigureArmy.HostilityType.Count;
		this.last_icon_armyType = PVFigureArmy.ArmyType.Count;
		this.last_def_armyType = PVFigureArmy.ArmyType.Count;
		base.Init();
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x000BC43F File Offset: 0x000BA63F
	public void SetFaceDirection(PVFigureArmy.FaceDirection faceDirection)
	{
		this.faceDirection = faceDirection;
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x000BC448 File Offset: 0x000BA648
	public override bool IsFlipped()
	{
		if (this.faceDirection != PVFigureArmy.FaceDirection.Rotation)
		{
			return this.faceDirection != PVFigureArmy.FaceDirection.Left && this.faceDirection == PVFigureArmy.FaceDirection.Right;
		}
		global::Army army = this.army;
		bool? flag;
		if (army == null)
		{
			flag = null;
		}
		else
		{
			Logic.Army logic = army.logic;
			flag = ((logic != null) ? new bool?(logic.is_in_water) : null);
		}
		if ((flag ?? false) && this.army.ship != null)
		{
			return this.army.ship.transform.eulerAngles.y < 180f;
		}
		return this.army.transform.eulerAngles.y < 180f;
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x000BC512 File Offset: 0x000BA712
	private void UpdateAllowedType()
	{
		if (this.army.logic.mercenary != null)
		{
			base.SetAllowedType(ViewMode.AllowedFigures.Mercenary);
			return;
		}
		base.SetAllowedType(ViewMode.AllowedFigures.Army);
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x000BC538 File Offset: 0x000BA738
	private void UpdateArmyType()
	{
		Logic.Army logic = this.army.logic;
		if (logic == null)
		{
			return;
		}
		if (logic.is_in_water)
		{
			this.armyType = PVFigureArmy.ArmyType.Ship;
			return;
		}
		if (logic.leader != null && logic.leader.IsCrusader())
		{
			this.armyType = PVFigureArmy.ArmyType.Crusade;
			return;
		}
		if (logic.mercenary != null && !logic.IsHiredMercenary())
		{
			this.armyType = PVFigureArmy.ArmyType.Mercenary;
			return;
		}
		if (logic.rebel != null)
		{
			this.armyType = PVFigureArmy.ArmyType.Rebel;
			return;
		}
		this.armyType = PVFigureArmy.ArmyType.Normal;
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x000BC5B4 File Offset: 0x000BA7B4
	private void UpdateHostilityType()
	{
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		Logic.Army logic = this.army.logic;
		if (logic.IsEnemy(obj))
		{
			this.hostilityType = PVFigureArmy.HostilityType.Enemy;
			return;
		}
		if (logic.IsOwnStance(obj))
		{
			this.hostilityType = PVFigureArmy.HostilityType.Own;
			return;
		}
		if (logic.leader != null && logic.leader.IsRebel() && logic.IsAllyOrVassal(obj))
		{
			this.hostilityType = PVFigureArmy.HostilityType.Own;
			return;
		}
		if (logic.leader != null && logic.leader.IsCrusader() && logic.leader.IsOwnStance(obj))
		{
			this.hostilityType = PVFigureArmy.HostilityType.Own;
			return;
		}
		this.hostilityType = PVFigureArmy.HostilityType.Neutral;
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x000BC64D File Offset: 0x000BA84D
	private void UpdateInBattle()
	{
		this.inBattle = (this.parent == null && this.army.logic.battle != null);
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x000BC67C File Offset: 0x000BA87C
	public void UpdateArmy()
	{
		if (this.army == null)
		{
			return;
		}
		this.UpdateInBattle();
		this.UpdateAllowedType();
		this.UpdateHostilityType();
		this.UpdateArmyType();
		this.RefreshDefField();
		this.army.UpdateVisibility(false);
		int crestKingdomId = this.GetCrestKingdomId();
		CrestObject crest = this.crest;
		if (crest != null)
		{
			crest.SetKingdomId(this.GetCrestKingdomId());
		}
		CrestObject crest2 = this.crest;
		if (crest2 != null)
		{
			crest2.gameObject.SetActive(crestKingdomId != 0);
		}
		this.Refresh();
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x000BC700 File Offset: 0x000BA900
	public override Texture2D GetIconTexture()
	{
		string key = this.hostilityType.ToString().ToLowerInvariant() + "_texture";
		return global::Defs.GetObj<Texture2D>(this.field, key, null);
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x000BC73C File Offset: 0x000BA93C
	public override void RefreshDefField()
	{
		if (this.armyType == this.last_def_armyType && this.hostilityType == this.last_def_hostilityType)
		{
			return;
		}
		this.last_def_armyType = this.armyType;
		this.last_def_hostilityType = this.hostilityType;
		DT.Field defField = global::Defs.GetDefField("PoliticalView", "pv_figures.Army");
		this.field = defField.FindChild(this.armyType.ToString(), null, true, true, true, '.');
		base.RefreshDefField();
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x000BC7B7 File Offset: 0x000BA9B7
	public override void RefreshIcon()
	{
		if (this.armyType == this.last_icon_armyType && this.hostilityType == this.last_icon_hostilityType)
		{
			return;
		}
		this.last_icon_armyType = this.armyType;
		this.last_icon_hostilityType = this.hostilityType;
		base.RefreshIcon();
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x000BC7F4 File Offset: 0x000BA9F4
	public void RefreshPosition()
	{
		if (this.parent != null)
		{
			return;
		}
		Vector3 position = this.army.transform.position;
		base.transform.position = new Vector3(position.x, (float)base.transform.GetSiblingIndex() * 0.001f, position.z);
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x000BC84F File Offset: 0x000BAA4F
	public override void Refresh()
	{
		base.Refresh();
		this.RefreshPosition();
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x000BC860 File Offset: 0x000BAA60
	private int GetCrestKingdomId()
	{
		if (this.armyType == PVFigureArmy.ArmyType.Mercenary && this.army.logic.mercenary.former_owner_id == BaseUI.LogicKingdom().id)
		{
			return this.army.logic.mercenary.former_owner_id;
		}
		return this.army.logic.kingdom_id;
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDestroy()
	{
	}

	// Token: 0x04000C0C RID: 3084
	public global::Army army;

	// Token: 0x04000C0D RID: 3085
	public PVFigureArmy.ArmyType armyType;

	// Token: 0x04000C0E RID: 3086
	public PVFigureArmy.HostilityType hostilityType;

	// Token: 0x04000C0F RID: 3087
	public PVFigureArmy.FaceDirection faceDirection;

	// Token: 0x04000C10 RID: 3088
	private bool inBattle;

	// Token: 0x04000C11 RID: 3089
	public PVFigureArmy.HostilityType last_icon_hostilityType;

	// Token: 0x04000C12 RID: 3090
	public PVFigureArmy.HostilityType last_def_hostilityType;

	// Token: 0x04000C13 RID: 3091
	public PVFigureArmy.ArmyType last_icon_armyType;

	// Token: 0x04000C14 RID: 3092
	public PVFigureArmy.ArmyType last_def_armyType;

	// Token: 0x02000684 RID: 1668
	public enum ArmyType
	{
		// Token: 0x040035C8 RID: 13768
		Normal,
		// Token: 0x040035C9 RID: 13769
		Crusade,
		// Token: 0x040035CA RID: 13770
		Rebel,
		// Token: 0x040035CB RID: 13771
		Ship,
		// Token: 0x040035CC RID: 13772
		Mercenary,
		// Token: 0x040035CD RID: 13773
		Count
	}

	// Token: 0x02000685 RID: 1669
	public enum HostilityType
	{
		// Token: 0x040035CF RID: 13775
		Enemy,
		// Token: 0x040035D0 RID: 13776
		Own,
		// Token: 0x040035D1 RID: 13777
		Neutral,
		// Token: 0x040035D2 RID: 13778
		Count
	}

	// Token: 0x02000686 RID: 1670
	public enum FaceDirection
	{
		// Token: 0x040035D4 RID: 13780
		Rotation,
		// Token: 0x040035D5 RID: 13781
		Left,
		// Token: 0x040035D6 RID: 13782
		Right
	}
}
