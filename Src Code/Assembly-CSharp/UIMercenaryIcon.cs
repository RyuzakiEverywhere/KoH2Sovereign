using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000231 RID: 561
public class UIMercenaryIcon : ObjectIcon, IListener, IPoolable
{
	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060021EA RID: 8682 RVA: 0x001328E8 File Offset: 0x00130AE8
	// (set) Token: 0x060021EB RID: 8683 RVA: 0x001328F0 File Offset: 0x00130AF0
	public Mercenary Mercenary { get; private set; }

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x060021EC RID: 8684 RVA: 0x001328FC File Offset: 0x00130AFC
	// (remove) Token: 0x060021ED RID: 8685 RVA: 0x00132934 File Offset: 0x00130B34
	public event Action<UIMercenaryIcon> OnSelect;

	// Token: 0x1400002C RID: 44
	// (add) Token: 0x060021EE RID: 8686 RVA: 0x0013296C File Offset: 0x00130B6C
	// (remove) Token: 0x060021EF RID: 8687 RVA: 0x001329A4 File Offset: 0x00130BA4
	public event Action<UIMercenaryIcon> OnFocus;

	// Token: 0x060021F0 RID: 8688 RVA: 0x001329D9 File Offset: 0x00130BD9
	public override void Awake()
	{
		base.Awake();
		this.m_WasActivated = true;
		if (this.m_AddListeners)
		{
			Mercenary mercenary = this.Mercenary;
			if (mercenary != null)
			{
				mercenary.AddListener(this);
			}
			this.Mercenary.army.AddListener(this);
		}
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x00132A13 File Offset: 0x00130C13
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x060021F2 RID: 8690 RVA: 0x00132A2C File Offset: 0x00130C2C
	private void UpdateLeader()
	{
		if (this.Mercenary.army.leader != null)
		{
			if (this.m_LeaderIcon != null)
			{
				this.m_LeaderIcon.gameObject.SetActive(true);
				this.m_LeaderIcon.SetObject(this.Mercenary.army.leader, null);
			}
			if (this.m_Leaderless != null)
			{
				this.m_Leaderless.gameObject.SetActive(false);
				return;
			}
		}
		else
		{
			if (this.m_LeaderIcon != null)
			{
				this.m_LeaderIcon.gameObject.SetActive(false);
			}
			if (this.m_Leaderless != null)
			{
				this.m_Leaderless.gameObject.SetActive(true);
			}
			if (this.m_FormerOwnerCrest != null)
			{
				this.m_FormerOwnerCrest.gameObject.SetActive(this.Mercenary.former_owner_id != 0);
				if (this.Mercenary.former_owner_id != 0)
				{
					this.m_FormerOwnerCrest.SetObject(this.Mercenary.game.GetKingdom(this.Mercenary.former_owner_id), null);
				}
			}
		}
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x00132B48 File Offset: 0x00130D48
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		if (this.logicObject == obj)
		{
			return;
		}
		this.RemoveListeners();
		base.SetObject(obj, vars);
		this.Mercenary = (obj as Mercenary);
		this.vars = ((vars != null) ? vars : new Vars(this.Mercenary));
		if (this.m_WasActivated)
		{
			this.AddListeners();
		}
		else
		{
			this.m_AddListeners = true;
		}
		this.Refresh();
		Tooltip.Get(base.gameObject, true).SetDef("MercenaryTooltip", vars);
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x00132BCF File Offset: 0x00130DCF
	private void Refresh()
	{
		if (this.Mercenary == null)
		{
			return;
		}
		this.UpdateLeader();
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x00132BE0 File Offset: 0x00130DE0
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (e.button == PointerEventData.InputButton.Left)
		{
			if (e.clickCount == 1 && this.OnSelect != null)
			{
				this.OnSelect(this);
			}
			if (e.clickCount > 1)
			{
				if (this.OnSelect != null)
				{
					this.OnSelect(this);
				}
				if (this.OnFocus != null)
				{
					this.OnFocus(this);
				}
			}
		}
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x00132C4A File Offset: 0x00130E4A
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "destroying") && !(message == "finishing"))
		{
			return;
		}
		Logic.Object @object = obj as Logic.Object;
		if (@object == null)
		{
			return;
		}
		@object.DelListener(this);
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x00132C78 File Offset: 0x00130E78
	private void AddListeners()
	{
		Mercenary mercenary = this.Mercenary;
		if (mercenary != null)
		{
			mercenary.AddListener(this);
		}
		Mercenary mercenary2 = this.Mercenary;
		if (mercenary2 == null)
		{
			return;
		}
		Logic.Army army = mercenary2.army;
		if (army == null)
		{
			return;
		}
		army.AddListener(this);
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x00132CA7 File Offset: 0x00130EA7
	private void RemoveListeners()
	{
		Mercenary mercenary = this.Mercenary;
		if (mercenary != null)
		{
			mercenary.DelListener(this);
		}
		Mercenary mercenary2 = this.Mercenary;
		if (mercenary2 == null)
		{
			return;
		}
		Logic.Army army = mercenary2.army;
		if (army == null)
		{
			return;
		}
		army.DelListener(this);
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x00132CD8 File Offset: 0x00130ED8
	private void OnDestroy()
	{
		AspectRatioFitter component = base.GetComponent<AspectRatioFitter>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		this.RemoveListeners();
		this.OnSelect = null;
		this.OnFocus = null;
		this.Mercenary = null;
		this.logicObject = null;
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x00132D1D File Offset: 0x00130F1D
	public void OnPoolDeactivated()
	{
		this.OnDestroy();
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x040016C3 RID: 5827
	[UIFieldTarget("id_Leader")]
	private UICharacterIcon m_LeaderIcon;

	// Token: 0x040016C4 RID: 5828
	[UIFieldTarget("id_Leaderless")]
	private GameObject m_Leaderless;

	// Token: 0x040016C5 RID: 5829
	[UIFieldTarget("id_FormerOwnerCrest")]
	private UIKingdomIcon m_FormerOwnerCrest;

	// Token: 0x040016C9 RID: 5833
	private bool m_Initialzied;

	// Token: 0x040016CA RID: 5834
	private bool m_WasActivated;

	// Token: 0x040016CB RID: 5835
	private bool m_AddListeners;
}
