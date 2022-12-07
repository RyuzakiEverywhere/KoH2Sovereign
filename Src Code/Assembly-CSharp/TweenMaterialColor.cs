using System;
using UnityEngine;

// Token: 0x020002ED RID: 749
[AddComponentMenu("NGUI/Tween/Tween Color")]
public class TweenMaterialColor : UITweener
{
	// Token: 0x06002F45 RID: 12101 RVA: 0x00184110 File Offset: 0x00182310
	private void Cache()
	{
		this.mCached = true;
		this.mMaterial = base.gameObject.GetComponent<Renderer>().material;
		if (this.mMaterial == null)
		{
			this.mMaterial = base.gameObject.GetComponent<Renderer>().material;
		}
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06002F46 RID: 12102 RVA: 0x0018415E File Offset: 0x0018235E
	// (set) Token: 0x06002F47 RID: 12103 RVA: 0x0018418D File Offset: 0x0018238D
	public Color value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mMaterial != null)
			{
				return this.mMaterial.color;
			}
			return Color.black;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mMaterial != null)
			{
				this.mMaterial.color = value;
			}
		}
	}

	// Token: 0x06002F48 RID: 12104 RVA: 0x001841B7 File Offset: 0x001823B7
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Color.Lerp(this.from, this.to, factor);
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x001841D4 File Offset: 0x001823D4
	public static TweenColor Begin(GameObject go, float duration, Color color)
	{
		TweenColor tweenColor = UITweener.Begin<TweenColor>(go, duration);
		tweenColor.from = tweenColor.value;
		tweenColor.to = color;
		if (duration <= 0f)
		{
			tweenColor.Sample(1f, true);
			tweenColor.enabled = false;
		}
		return tweenColor;
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x00184218 File Offset: 0x00182418
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x00184226 File Offset: 0x00182426
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x00184234 File Offset: 0x00182434
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x00184242 File Offset: 0x00182442
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x04001FDF RID: 8159
	public Color from = Color.white;

	// Token: 0x04001FE0 RID: 8160
	public Color to = Color.white;

	// Token: 0x04001FE1 RID: 8161
	private bool mCached;

	// Token: 0x04001FE2 RID: 8162
	private Material mMaterial;
}
