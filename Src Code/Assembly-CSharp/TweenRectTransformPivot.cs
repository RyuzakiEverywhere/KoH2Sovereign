using System;
using UnityEngine;

// Token: 0x020002F1 RID: 753
[AddComponentMenu("UGUI/Tween/Tween RectTransform Pivot")]
public class TweenRectTransformPivot : UITweener
{
	// Token: 0x17000251 RID: 593
	// (get) Token: 0x06002F6D RID: 12141 RVA: 0x0018469E File Offset: 0x0018289E
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

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x06002F6E RID: 12142 RVA: 0x001846C5 File Offset: 0x001828C5
	// (set) Token: 0x06002F6F RID: 12143 RVA: 0x001846D2 File Offset: 0x001828D2
	public Vector2 value
	{
		get
		{
			return this.cachedTransform.pivot;
		}
		set
		{
			this.cachedTransform.pivot = value;
		}
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x001846E0 File Offset: 0x001828E0
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.from * (1f - factor) + this.to * factor;
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x0018470C File Offset: 0x0018290C
	public static TweenRectTransformPivot Begin(GameObject go, float duration, Vector2 pivot)
	{
		TweenRectTransformPivot tweenRectTransformPivot = UITweener.Begin<TweenRectTransformPivot>(go, duration);
		tweenRectTransformPivot.from = tweenRectTransformPivot.value;
		tweenRectTransformPivot.to = pivot;
		if (duration <= 0f)
		{
			tweenRectTransformPivot.Sample(1f, true);
			tweenRectTransformPivot.enabled = false;
		}
		return tweenRectTransformPivot;
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x00184750 File Offset: 0x00182950
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F73 RID: 12147 RVA: 0x0018475E File Offset: 0x0018295E
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06002F74 RID: 12148 RVA: 0x0018476C File Offset: 0x0018296C
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06002F75 RID: 12149 RVA: 0x0018477A File Offset: 0x0018297A
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x04001FEF RID: 8175
	public Vector2 from = Vector2.one;

	// Token: 0x04001FF0 RID: 8176
	public Vector2 to = Vector2.one;

	// Token: 0x04001FF1 RID: 8177
	private RectTransform mTrans;
}
