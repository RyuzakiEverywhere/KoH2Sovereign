using System;
using UnityEngine;

// Token: 0x020002EA RID: 746
[AddComponentMenu("UGUI/Tween/Tween CanvasGroup Aplha")]
public class TweenCanvasGroupAplha : UITweener
{
	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06002F2D RID: 12077 RVA: 0x00183E02 File Offset: 0x00182002
	public CanvasGroup cachedGraphic
	{
		get
		{
			if (this.mGraphic == null)
			{
				this.mGraphic = base.GetComponent<CanvasGroup>();
				if (this.mGraphic == null)
				{
					this.mGraphic = base.GetComponentInChildren<CanvasGroup>();
				}
			}
			return this.mGraphic;
		}
	}

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06002F2E RID: 12078 RVA: 0x00183E3E File Offset: 0x0018203E
	// (set) Token: 0x06002F2F RID: 12079 RVA: 0x00183E4B File Offset: 0x0018204B
	public float value
	{
		get
		{
			return this.cachedGraphic.alpha;
		}
		set
		{
			this.cachedGraphic.alpha = value;
		}
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x00183E59 File Offset: 0x00182059
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x00183E74 File Offset: 0x00182074
	public static TweenCanvasGroupAplha Begin(GameObject go, float duration, float alpha)
	{
		TweenCanvasGroupAplha tweenCanvasGroupAplha = UITweener.Begin<TweenCanvasGroupAplha>(go, duration);
		tweenCanvasGroupAplha.from = tweenCanvasGroupAplha.value;
		tweenCanvasGroupAplha.to = alpha;
		if (duration <= 0f)
		{
			tweenCanvasGroupAplha.Sample(1f, true);
			tweenCanvasGroupAplha.enabled = false;
		}
		return tweenCanvasGroupAplha;
	}

	// Token: 0x06002F32 RID: 12082 RVA: 0x00183EB8 File Offset: 0x001820B8
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x00183EC6 File Offset: 0x001820C6
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x04001FD5 RID: 8149
	[Range(0f, 1f)]
	public float from = 1f;

	// Token: 0x04001FD6 RID: 8150
	[Range(0f, 1f)]
	public float to = 1f;

	// Token: 0x04001FD7 RID: 8151
	private CanvasGroup mGraphic;
}
