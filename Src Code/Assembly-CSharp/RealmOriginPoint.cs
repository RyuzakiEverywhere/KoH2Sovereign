using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class RealmOriginPoint : MonoBehaviour
{
	// Token: 0x060012B5 RID: 4789 RVA: 0x000C3940 File Offset: 0x000C1B40
	public static RealmOriginPoint First()
	{
		return RealmOriginPoint.first;
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x000C3947 File Offset: 0x000C1B47
	public RealmOriginPoint Next()
	{
		return this.next;
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x000C394F File Offset: 0x000C1B4F
	private void Awake()
	{
		if (Application.isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x000C3963 File Offset: 0x000C1B63
	public void onStart()
	{
		this.Register();
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x000C396C File Offset: 0x000C1B6C
	public void Register()
	{
		if (this.registered)
		{
			return;
		}
		this.registered = true;
		this.prev = RealmOriginPoint.last;
		if (RealmOriginPoint.last != null)
		{
			RealmOriginPoint.last.next = this;
		}
		else
		{
			RealmOriginPoint.first = this;
		}
		RealmOriginPoint.last = this;
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x000C39BC File Offset: 0x000C1BBC
	public void Unregister()
	{
		if (!this.registered)
		{
			return;
		}
		this.registered = false;
		if (this.next != null)
		{
			this.next.prev = this.prev;
		}
		else
		{
			RealmOriginPoint.last = this.prev;
		}
		if (this.prev != null)
		{
			this.prev.next = this.next;
		}
		else
		{
			RealmOriginPoint.first = this.next;
		}
		this.prev = (this.next = null);
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x000C3A41 File Offset: 0x000C1C41
	private void OnEnable()
	{
		base.gameObject.hideFlags |= HideFlags.DontSaveInBuild;
		this.Register();
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x000C3A5D File Offset: 0x000C1C5D
	private void OnDisable()
	{
		this.Unregister();
	}

	// Token: 0x04000C8F RID: 3215
	public string realmName;

	// Token: 0x04000C90 RID: 3216
	private bool registered;

	// Token: 0x04000C91 RID: 3217
	public bool hasNegativeId = true;

	// Token: 0x04000C92 RID: 3218
	private static RealmOriginPoint first;

	// Token: 0x04000C93 RID: 3219
	private static RealmOriginPoint last;

	// Token: 0x04000C94 RID: 3220
	private RealmOriginPoint prev;

	// Token: 0x04000C95 RID: 3221
	private RealmOriginPoint next;
}
