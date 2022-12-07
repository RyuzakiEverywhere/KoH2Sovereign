using System;
using UnityEngine;

// Token: 0x020002F0 RID: 752
[AddComponentMenu("UGUI/Tween/Tween RectTransform Anchor")]
public class TweenRectTransformAnchor : UITweener
{
	// Token: 0x1700024F RID: 591
	// (get) Token: 0x06002F63 RID: 12131 RVA: 0x00184522 File Offset: 0x00182722
	public RectTransform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = (base.transform as RectTransform);
			}
			return this.mTrans;
		}
	}

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x06002F64 RID: 12132 RVA: 0x0018454C File Offset: 0x0018274C
	// (set) Token: 0x06002F65 RID: 12133 RVA: 0x0018459E File Offset: 0x0018279E
	public Vector4 value
	{
		get
		{
			return new Vector4(this.cachedTransform.anchorMin.x, this.cachedTransform.anchorMin.y, this.cachedTransform.anchorMax.x, this.cachedTransform.anchorMax.y);
		}
		set
		{
			this.cachedTransform.anchorMin = new Vector2(value.x, value.y);
			this.cachedTransform.anchorMax = new Vector2(value.z, value.w);
		}
	}

	// Token: 0x06002F66 RID: 12134 RVA: 0x001845D8 File Offset: 0x001827D8
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.from * (1f - factor) + this.to * factor;
	}

	// Token: 0x06002F67 RID: 12135 RVA: 0x00184604 File Offset: 0x00182804
	public static TweenRectTransformAnchor Begin(GameObject go, float duration, Vector4 anchors)
	{
		TweenRectTransformAnchor tweenRectTransformAnchor = UITweener.Begin<TweenRectTransformAnchor>(go, duration);
		tweenRectTransformAnchor.from = tweenRectTransformAnchor.value;
		tweenRectTransformAnchor.to = anchors;
		if (duration <= 0f)
		{
			tweenRectTransformAnchor.Sample(1f, true);
			tweenRectTransformAnchor.enabled = false;
		}
		return tweenRectTransformAnchor;
	}

	// Token: 0x06002F68 RID: 12136 RVA: 0x00184648 File Offset: 0x00182848
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F69 RID: 12137 RVA: 0x00184656 File Offset: 0x00182856
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06002F6A RID: 12138 RVA: 0x00184664 File Offset: 0x00182864
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x00184672 File Offset: 0x00182872
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x04001FEC RID: 8172
	public Vector4 from = Vector4.one;

	// Token: 0x04001FED RID: 8173
	public Vector4 to = Vector4.one;

	// Token: 0x04001FEE RID: 8174
	private RectTransform mTrans;
}
