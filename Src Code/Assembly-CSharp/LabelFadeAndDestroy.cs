using System;
using TMPro;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class LabelFadeAndDestroy : MonoBehaviour
{
	// Token: 0x0600106A RID: 4202 RVA: 0x000AEB88 File Offset: 0x000ACD88
	private void Update()
	{
		if (!this.fadingIn && !this.fadingOut)
		{
			base.enabled = false;
			return;
		}
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if (this.fadingOut)
		{
			this.text.alpha -= unscaledDeltaTime / this.fadeOutTime;
			if (this.text.alpha < 0f)
			{
				this.text.alpha = 0f;
				this.fadingIn = false;
				Common.DestroyObj(base.gameObject);
			}
			return;
		}
		if (this.fadingIn)
		{
			this.text.alpha += unscaledDeltaTime / this.fadeInTime;
			if (this.text.alpha > 1f)
			{
				this.text.alpha = 1f;
				this.fadingIn = false;
			}
			return;
		}
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x000AEC56 File Offset: 0x000ACE56
	public bool CanFade()
	{
		return ViewMode.IsPoliticalView();
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x000AEC60 File Offset: 0x000ACE60
	public void FadeIn(bool useCurrentOpacity = true, float startingOpactiy = 0f)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.text == null)
		{
			this.text = base.gameObject.GetComponent<TextMeshPro>();
		}
		base.enabled = true;
		if (this.fadingIn)
		{
			return;
		}
		this.fadingIn = this.CanFade();
		this.fadingOut = false;
		if (!useCurrentOpacity)
		{
			this.text.alpha = (this.fadingIn ? startingOpactiy : 1f);
		}
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x000AECD8 File Offset: 0x000ACED8
	public void FadeOutAndDestroy(bool useCurrentOpacity = true, float startingOpactiy = 1f)
	{
		if (!Application.isPlaying || !this.CanFade())
		{
			Common.DestroyObj(base.gameObject);
			return;
		}
		if (this.text == null)
		{
			this.text = base.gameObject.GetComponent<TextMeshPro>();
		}
		base.enabled = true;
		if (this.fadingOut)
		{
			return;
		}
		this.fadingIn = false;
		this.fadingOut = true;
		if (!useCurrentOpacity)
		{
			this.text.alpha = startingOpactiy;
		}
	}

	// Token: 0x04000AD3 RID: 2771
	private bool fadingIn;

	// Token: 0x04000AD4 RID: 2772
	private bool fadingOut;

	// Token: 0x04000AD5 RID: 2773
	public float fadeInTime = 0.5f;

	// Token: 0x04000AD6 RID: 2774
	public float fadeOutTime = 0.5f;

	// Token: 0x04000AD7 RID: 2775
	private TextMeshPro text;
}
