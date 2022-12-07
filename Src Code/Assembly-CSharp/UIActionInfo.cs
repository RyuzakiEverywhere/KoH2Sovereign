using System;
using Logic;
using UnityEngine;

// Token: 0x0200018E RID: 398
public abstract class UIActionInfo : MonoBehaviour, IListener
{
	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06001604 RID: 5636 RVA: 0x000DF448 File Offset: 0x000DD648
	protected virtual float RefreshRateSeconds
	{
		get
		{
			return -1f;
		}
	}

	// Token: 0x06001605 RID: 5637
	public abstract void Refresh();

	// Token: 0x06001606 RID: 5638 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void Awake()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x000DF458 File Offset: 0x000DD658
	protected virtual void OnDestroy()
	{
		if (this.action != null && this.action.logic != null)
		{
			this.action.logic.DelListener(this);
		}
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x000DF480 File Offset: 0x000DD680
	public virtual void Init(ActionVisuals action)
	{
		this.action = action;
		if (action.logic != null)
		{
			this.owner = action.logic.owner;
			action.logic.AddListener(this);
		}
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void Update()
	{
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000C4358 File Offset: 0x000C2558
	public virtual void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x04000E3D RID: 3645
	public float timeSinceRefresh;

	// Token: 0x04000E3E RID: 3646
	protected Logic.Object owner;

	// Token: 0x04000E3F RID: 3647
	protected ActionVisuals action;
}
