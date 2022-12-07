using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002EE RID: 750
[AddComponentMenu("NGUI/Tween/Tween Outline Color")]
[RequireComponent(typeof(Outline))]
public class TweenOutlineColor : UITweener
{
	// Token: 0x06002F4F RID: 12111 RVA: 0x0018426E File Offset: 0x0018246E
	private void Cache()
	{
		this.mCached = true;
		this.mGraphic = base.GetComponent<Outline>();
		if (this.mGraphic == null)
		{
			this.mGraphic = base.GetComponentInChildren<Outline>();
		}
	}

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06002F50 RID: 12112 RVA: 0x0018429D File Offset: 0x0018249D
	// (set) Token: 0x06002F51 RID: 12113 RVA: 0x001842CC File Offset: 0x001824CC
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
				return this.mGraphic.effectColor;
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
				this.mGraphic.effectColor = value;
			}
		}
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x001842F6 File Offset: 0x001824F6
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Color.Lerp(this.from, this.to, factor);
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x00184310 File Offset: 0x00182510
	public static TweenOutlineColor Begin(GameObject go, float duration, Color color)
	{
		TweenOutlineColor tweenOutlineColor = UITweener.Begin<TweenOutlineColor>(go, duration);
		tweenOutlineColor.from = tweenOutlineColor.value;
		tweenOutlineColor.to = color;
		if (duration <= 0f)
		{
			tweenOutlineColor.Sample(1f, true);
			tweenOutlineColor.enabled = false;
		}
		return tweenOutlineColor;
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x00184354 File Offset: 0x00182554
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x00184362 File Offset: 0x00182562
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x00184370 File Offset: 0x00182570
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x0018437E File Offset: 0x0018257E
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x04001FE3 RID: 8163
	public Color from = Color.white;

	// Token: 0x04001FE4 RID: 8164
	public Color to = Color.white;

	// Token: 0x04001FE5 RID: 8165
	private bool mCached;

	// Token: 0x04001FE6 RID: 8166
	private Outline mGraphic;
}
