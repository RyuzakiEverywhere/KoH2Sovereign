using System;
using Logic;

// Token: 0x0200019C RID: 412
public class UIArmyTransferWindow : ObjectWindow, IPoolable
{
	// Token: 0x0600172E RID: 5934 RVA: 0x000E58AB File Offset: 0x000E3AAB
	private void Start()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x000E58AB File Offset: 0x000E3AAB
	private void OnEnable()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x000E58B4 File Offset: 0x000E3AB4
	private void ExtractLogicObject()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null || worldUI.selected_obj == null)
		{
			return;
		}
		global::Army component = worldUI.selected_obj.GetComponent<global::Army>();
		Logic.Army army = (component != null) ? component.logic : null;
		if (army != null && army != this.army)
		{
			this.SetObject(army, null);
		}
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x000E5914 File Offset: 0x000E3B14
	public override void SetObject(Object obj, Vars vars = null)
	{
		this.Init();
		if (vars == null)
		{
			vars = new Vars(obj);
		}
		base.SetObject(obj, vars);
		this.army = (obj as Logic.Army);
		this.otherArmy = null;
		if (this.army != null)
		{
			if (this.army.interact_target != null)
			{
				this.otherArmy = this.army.interact_target;
			}
			else if (this.army.interactor != null)
			{
				this.otherArmy = this.army.interactor;
			}
		}
		if (this.m_ProvinceIllustration != null)
		{
			this.m_ProvinceIllustration.SetObject(this.logicObject as Logic.Army);
		}
		this.DesideVisualSide();
		this.Refresh();
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x000E59C8 File Offset: 0x000E3BC8
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ArmyLeft != null)
		{
			this.m_ArmyLeft.SetTransferWindow(this);
		}
		if (this.m_ArmyRight != null)
		{
			this.m_ArmyRight.SetTransferWindow(this);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x000E5A20 File Offset: 0x000E3C20
	private void DesideVisualSide()
	{
		if (this.m_ArmyLeft == null || this.m_ArmyRight == null)
		{
			return;
		}
		Logic.Army army;
		Logic.Army army2;
		if (this.army.position.x > this.otherArmy.position.x)
		{
			army = this.army;
			army2 = this.otherArmy;
		}
		else if (this.army.position.x < this.otherArmy.position.x)
		{
			army = this.otherArmy;
			army2 = this.army;
		}
		else
		{
			army = ((this.army.uid < this.otherArmy.uid) ? this.army : this.otherArmy);
			army2 = ((this.army.uid < this.otherArmy.uid) ? this.otherArmy : this.army);
		}
		this.m_ArmyLeft.SetObject(army2, null);
		this.m_ArmyRight.SetObject(army, null);
		this.m_ArmyLeft.SetUnitTransferTarget(army);
		this.m_ArmyRight.SetUnitTransferTarget(army2);
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x000E5B35 File Offset: 0x000E3D35
	public override void Refresh()
	{
		base.Refresh();
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x000E5B3D File Offset: 0x000E3D3D
	public override void AddListeners()
	{
		base.AddListeners();
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x000E5B45 File Offset: 0x000E3D45
	public override void RemoveListeners()
	{
		base.RemoveListeners();
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x000E5B4D File Offset: 0x000E3D4D
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		base.HandleLogicMessage(obj, message, param);
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x000E5B58 File Offset: 0x000E3D58
	public override void Release()
	{
		this.RemoveListeners();
		this.logicObject = null;
		this.army = null;
		this.otherArmy = null;
		base.Release();
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x000E58AB File Offset: 0x000E3AAB
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x000023FD File Offset: 0x000005FD
	protected override void OnDestroy()
	{
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x000E5B7B File Offset: 0x000E3D7B
	public void OnPoolDeactivated()
	{
		this.Release();
		this.OnDestroy();
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x04000EF2 RID: 3826
	[UIFieldTarget("id_SourceArmy")]
	private UIArmyWindow m_ArmyLeft;

	// Token: 0x04000EF3 RID: 3827
	[UIFieldTarget("id_DestinationArmy")]
	private UIArmyWindow m_ArmyRight;

	// Token: 0x04000EF4 RID: 3828
	[UIFieldTarget("id_ProvinceIllustration")]
	private UIProvinceIllustration m_ProvinceIllustration;

	// Token: 0x04000EF5 RID: 3829
	private Logic.Army army;

	// Token: 0x04000EF6 RID: 3830
	private Logic.Army otherArmy;

	// Token: 0x04000EF7 RID: 3831
	private bool m_Initialzied;
}
