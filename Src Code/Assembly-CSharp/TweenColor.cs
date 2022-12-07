using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002EB RID: 747
[AddComponentMenu("NGUI/Tween/Tween Color")]
public class TweenColor : UITweener
{
	// Token: 0x06002F35 RID: 12085 RVA: 0x00183EF2 File Offset: 0x001820F2
	private void Cache()
	{
		this.mCached = true;
		this.mGraphic = base.GetComponent<Graphic>();
		if (this.mGraphic == null)
		{
			this.mGraphic = base.GetComponentInChildren<Graphic>();
		}
	}

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06002F36 RID: 12086 RVA: 0x00183F21 File Offset: 0x00182121
	// (set) Token: 0x06002F37 RID: 12087 RVA: 0x00183F50 File Offset: 0x00182150
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

	// Token: 0x06002F38 RID: 12088 RVA: 0x00183F7A File Offset: 0x0018217A
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Color.Lerp(this.from, this.to, factor);
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x00183F94 File Offset: 0x00182194
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

	// Token: 0x06002F3A RID: 12090 RVA: 0x00183FD8 File Offset: 0x001821D8
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x00183FE6 File Offset: 0x001821E6
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x00183FF4 File Offset: 0x001821F4
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x00184002 File Offset: 0x00182202
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x04001FD8 RID: 8152
	public Color from = Color.white;

	// Token: 0x04001FD9 RID: 8153
	public Color to = Color.white;

	// Token: 0x04001FDA RID: 8154
	private bool mCached;

	// Token: 0x04001FDB RID: 8155
	private Graphic mGraphic;
}
