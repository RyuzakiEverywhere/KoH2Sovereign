using System;
using UnityEngine;

// Token: 0x020002F4 RID: 756
[RequireComponent(typeof(SpriteRenderer))]
public class TweenSpriteAlpha : UITweener
{
	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06002F8D RID: 12173 RVA: 0x001849F6 File Offset: 0x00182BF6
	private SpriteRenderer CachedSprite
	{
		get
		{
			if (this.mSprite == null)
			{
				this.mSprite = base.GetComponent<SpriteRenderer>();
			}
			return this.mSprite;
		}
	}

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06002F8E RID: 12174 RVA: 0x00184A18 File Offset: 0x00182C18
	// (set) Token: 0x06002F8F RID: 12175 RVA: 0x00184A2C File Offset: 0x00182C2C
	private float alpha
	{
		get
		{
			return this.CachedSprite.color.a;
		}
		set
		{
			Color color = this.CachedSprite.color;
			color.a = value;
			this.CachedSprite.color = color;
		}
	}

	// Token: 0x06002F90 RID: 12176 RVA: 0x00184A59 File Offset: 0x00182C59
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.alpha = Mathf.Lerp(this.from, this.to, factor);
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x00184A74 File Offset: 0x00182C74
	public static TweenSpriteAlpha Begin(GameObject go, float duration, float alpha)
	{
		TweenSpriteAlpha tweenSpriteAlpha = UITweener.Begin<TweenSpriteAlpha>(go, duration);
		tweenSpriteAlpha.from = tweenSpriteAlpha.alpha;
		tweenSpriteAlpha.to = alpha;
		if (duration < 0f)
		{
			tweenSpriteAlpha.Sample(1f, true);
			tweenSpriteAlpha.enabled = false;
		}
		return tweenSpriteAlpha;
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x00184AB8 File Offset: 0x00182CB8
	public override void SetStartToCurrentValue()
	{
		this.from = this.alpha;
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x00184AC6 File Offset: 0x00182CC6
	public override void SetEndToCurrentValue()
	{
		this.to = this.alpha;
	}

	// Token: 0x04001FF9 RID: 8185
	[Range(0f, 1f)]
	public float from = 1f;

	// Token: 0x04001FFA RID: 8186
	[Range(0f, 1f)]
	public float to = 1f;

	// Token: 0x04001FFB RID: 8187
	private SpriteRenderer mSprite;
}
