using System;
using UnityEngine;

// Token: 0x020002EF RID: 751
[AddComponentMenu("NGUI/Tween/Tween Position")]
public class TweenPosition : UITweener
{
	// Token: 0x06002F59 RID: 12121 RVA: 0x001843AA File Offset: 0x001825AA
	private void Cache()
	{
		this.mCached = true;
		this.mTransform = base.GetComponent<Transform>();
		if (this.mTransform == null)
		{
			this.mTransform = base.GetComponentInChildren<Transform>();
		}
	}

	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06002F5A RID: 12122 RVA: 0x001843DC File Offset: 0x001825DC
	// (set) Token: 0x06002F5B RID: 12123 RVA: 0x0018442B File Offset: 0x0018262B
	public Vector3 value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (!(this.mTransform != null))
			{
				return Vector3.zero;
			}
			if (this.mode == UITweener.Mode.Absolute)
			{
				return this.mTransform.position;
			}
			return this.mTransform.localPosition;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mTransform != null)
			{
				if (this.mode == UITweener.Mode.Absolute)
				{
					this.mTransform.position = value;
					return;
				}
				this.mTransform.localPosition = value;
			}
		}
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x0018446B File Offset: 0x0018266B
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Vector3.Lerp(this.from, this.to, factor);
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x00184488 File Offset: 0x00182688
	public static TweenPosition Begin(GameObject go, float duration, Vector3 position)
	{
		TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration);
		tweenPosition.from = tweenPosition.value;
		tweenPosition.to = position;
		if (duration <= 0f)
		{
			tweenPosition.Sample(1f, true);
			tweenPosition.enabled = false;
		}
		return tweenPosition;
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x001844CC File Offset: 0x001826CC
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x001844DA File Offset: 0x001826DA
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x001844E8 File Offset: 0x001826E8
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x001844F6 File Offset: 0x001826F6
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x04001FE7 RID: 8167
	public UITweener.Mode mode;

	// Token: 0x04001FE8 RID: 8168
	public Vector3 from = Vector3.zero;

	// Token: 0x04001FE9 RID: 8169
	public Vector3 to = Vector3.zero;

	// Token: 0x04001FEA RID: 8170
	private bool mCached;

	// Token: 0x04001FEB RID: 8171
	private Transform mTransform;
}
