using System;
using UnityEngine;

// Token: 0x020002F3 RID: 755
[AddComponentMenu("NGUI/Tween/Tween Scale")]
public class TweenScale : UITweener
{
	// Token: 0x17000254 RID: 596
	// (get) Token: 0x06002F81 RID: 12161 RVA: 0x001848E2 File Offset: 0x00182AE2
	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06002F82 RID: 12162 RVA: 0x00184904 File Offset: 0x00182B04
	// (set) Token: 0x06002F83 RID: 12163 RVA: 0x00184911 File Offset: 0x00182B11
	public Vector3 value
	{
		get
		{
			return this.cachedTransform.localScale;
		}
		set
		{
			this.cachedTransform.localScale = value;
		}
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06002F84 RID: 12164 RVA: 0x0018491F File Offset: 0x00182B1F
	// (set) Token: 0x06002F85 RID: 12165 RVA: 0x00184927 File Offset: 0x00182B27
	[Obsolete("Use 'value' instead")]
	public Vector3 scale
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	// Token: 0x06002F86 RID: 12166 RVA: 0x00184930 File Offset: 0x00182B30
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.from * (1f - factor) + this.to * factor;
	}

	// Token: 0x06002F87 RID: 12167 RVA: 0x0018495C File Offset: 0x00182B5C
	public static TweenScale Begin(GameObject go, float duration, Vector3 scale)
	{
		TweenScale tweenScale = UITweener.Begin<TweenScale>(go, duration);
		tweenScale.from = tweenScale.value;
		tweenScale.to = scale;
		if (duration <= 0f)
		{
			tweenScale.Sample(1f, true);
			tweenScale.enabled = false;
		}
		return tweenScale;
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x001849A0 File Offset: 0x00182BA0
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x001849AE File Offset: 0x00182BAE
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x001849BC File Offset: 0x00182BBC
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06002F8B RID: 12171 RVA: 0x001849CA File Offset: 0x00182BCA
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x04001FF6 RID: 8182
	public Vector3 from = Vector3.one;

	// Token: 0x04001FF7 RID: 8183
	public Vector3 to = Vector3.one;

	// Token: 0x04001FF8 RID: 8184
	private Transform mTrans;
}
