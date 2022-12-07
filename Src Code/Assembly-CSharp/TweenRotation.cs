using System;
using UnityEngine;

// Token: 0x020002F2 RID: 754
[AddComponentMenu("NGUI/Tween/Tween Rotation")]
public class TweenRotation : UITweener
{
	// Token: 0x06002F77 RID: 12151 RVA: 0x001847A6 File Offset: 0x001829A6
	private void Cache()
	{
		this.mCached = true;
		this.mTransform = base.GetComponent<Transform>();
		if (this.mTransform == null)
		{
			this.mTransform = base.GetComponentInChildren<Transform>();
		}
	}

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x06002F78 RID: 12152 RVA: 0x001847D5 File Offset: 0x001829D5
	// (set) Token: 0x06002F79 RID: 12153 RVA: 0x00184804 File Offset: 0x00182A04
	public Vector3 value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mTransform != null)
			{
				return this.mTransform.eulerAngles;
			}
			return Vector3.zero;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mTransform != null)
			{
				this.mTransform.eulerAngles = value;
			}
		}
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x0018482E File Offset: 0x00182A2E
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Vector3.Lerp(this.from, this.to, factor);
	}

	// Token: 0x06002F7B RID: 12155 RVA: 0x00184848 File Offset: 0x00182A48
	public static TweenRotation Begin(GameObject go, float duration, Vector3 position)
	{
		TweenRotation tweenRotation = UITweener.Begin<TweenRotation>(go, duration);
		tweenRotation.from = tweenRotation.value;
		tweenRotation.to = position;
		if (duration <= 0f)
		{
			tweenRotation.Sample(1f, true);
			tweenRotation.enabled = false;
		}
		return tweenRotation;
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x0018488C File Offset: 0x00182A8C
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F7D RID: 12157 RVA: 0x0018489A File Offset: 0x00182A9A
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x001848A8 File Offset: 0x00182AA8
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x001848B6 File Offset: 0x00182AB6
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x04001FF2 RID: 8178
	public Vector3 from = Vector3.zero;

	// Token: 0x04001FF3 RID: 8179
	public Vector3 to = Vector3.zero;

	// Token: 0x04001FF4 RID: 8180
	private bool mCached;

	// Token: 0x04001FF5 RID: 8181
	private Transform mTransform;
}
