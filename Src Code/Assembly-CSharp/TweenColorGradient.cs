using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002EC RID: 748
[AddComponentMenu("NGUI/Tween/Tween Color Gradient")]
public class TweenColorGradient : UITweener
{
	// Token: 0x06002F3F RID: 12095 RVA: 0x0018402E File Offset: 0x0018222E
	private void Cache()
	{
		this.mCached = true;
		this.mGraphic = base.GetComponent<Graphic>();
		if (this.mGraphic == null)
		{
			this.mGraphic = base.GetComponentInChildren<Graphic>();
		}
	}

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06002F40 RID: 12096 RVA: 0x0018405D File Offset: 0x0018225D
	// (set) Token: 0x06002F41 RID: 12097 RVA: 0x0018408C File Offset: 0x0018228C
	public Color value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mGraphic != null)
			{
				return this.mGraphic.color;
			}
			return Color.black;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mGraphic != null)
			{
				this.mGraphic.color = value;
			}
		}
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x001840B6 File Offset: 0x001822B6
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.gradient.Evaluate(factor);
	}

	// Token: 0x06002F43 RID: 12099 RVA: 0x001840CC File Offset: 0x001822CC
	public static TweenColorGradient Begin(GameObject go, float duration, Color color)
	{
		TweenColorGradient tweenColorGradient = UITweener.Begin<TweenColorGradient>(go, duration);
		if (duration <= 0f)
		{
			tweenColorGradient.Sample(1f, true);
			tweenColorGradient.enabled = false;
		}
		return tweenColorGradient;
	}

	// Token: 0x04001FDC RID: 8156
	public Gradient gradient = new Gradient();

	// Token: 0x04001FDD RID: 8157
	private bool mCached;

	// Token: 0x04001FDE RID: 8158
	private Graphic mGraphic;
}
