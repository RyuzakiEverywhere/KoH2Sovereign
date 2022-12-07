using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002E9 RID: 745
[AddComponentMenu("UGUI/Tween/Tween Alpha")]
public class TweenAlpha : UITweener
{
	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06002F25 RID: 12069 RVA: 0x00183CEC File Offset: 0x00181EEC
	public Graphic cachedGraphic
	{
		get
		{
			if (this.mGraphic == null)
			{
				this.mGraphic = base.GetComponent<Graphic>();
				if (this.mGraphic == null)
				{
					this.mGraphic = base.GetComponentInChildren<Graphic>();
				}
			}
			return this.mGraphic;
		}
	}

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06002F26 RID: 12070 RVA: 0x00183D28 File Offset: 0x00181F28
	// (set) Token: 0x06002F27 RID: 12071 RVA: 0x00183D3C File Offset: 0x00181F3C
	public float value
	{
		get
		{
			return this.cachedGraphic.color.a;
		}
		set
		{
			Color color = this.cachedGraphic.color;
			color.a = value;
			this.cachedGraphic.color = color;
		}
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x00183D69 File Offset: 0x00181F69
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x00183D84 File Offset: 0x00181F84
	public static TweenAlpha Begin(GameObject go, float duration, float alpha)
	{
		TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration);
		tweenAlpha.from = tweenAlpha.value;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, true);
			tweenAlpha.enabled = false;
		}
		return tweenAlpha;
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x00183DC8 File Offset: 0x00181FC8
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x00183DD6 File Offset: 0x00181FD6
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x04001FD2 RID: 8146
	[Range(0f, 1f)]
	public float from = 1f;

	// Token: 0x04001FD3 RID: 8147
	[Range(0f, 1f)]
	public float to = 1f;

	// Token: 0x04001FD4 RID: 8148
	private Graphic mGraphic;
}
