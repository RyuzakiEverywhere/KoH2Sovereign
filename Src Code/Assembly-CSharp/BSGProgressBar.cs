using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002FB RID: 763
public class BSGProgressBar : MonoBehaviour
{
	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06002FF3 RID: 12275 RVA: 0x001873A0 File Offset: 0x001855A0
	// (set) Token: 0x06002FF4 RID: 12276 RVA: 0x001873A8 File Offset: 0x001855A8
	public float Progress { get; private set; }

	// Token: 0x06002FF5 RID: 12277 RVA: 0x001873B1 File Offset: 0x001855B1
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x001873CA File Offset: 0x001855CA
	public void Setup(int segments, bool blink = false)
	{
		this.Init();
		this.m_ProgressBlink = blink;
		this.m_CurrentSegmentIndex = -1;
		this.Progress = 0f;
		this.m_SegmentCount = segments;
		this.Rebuild();
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x001873F8 File Offset: 0x001855F8
	public void SetProgress(float progress)
	{
		this.Progress = progress;
		if (this.style == BSGProgressBar.Style.Segmented)
		{
			this.CheckSegmentChange();
			return;
		}
		if (this.style == BSGProgressBar.Style.Normal && this.m_ClassicProgressValue)
		{
			this.m_ClassicProgressValue.fillAmount = progress;
		}
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x00187432 File Offset: 0x00185632
	public void SetStyle(BSGProgressBar.Style style)
	{
		this.style = style;
		this.Rebuild();
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x00187444 File Offset: 0x00185644
	private void CheckSegmentChange()
	{
		if (this.Progress == -1f)
		{
			return;
		}
		int num = Mathf.FloorToInt(this.Progress * (float)this.m_SegmentCount);
		if (num > this.m_CurrentSegmentIndex)
		{
			this.m_CurrentSegmentIndex = num;
			if (this.m_SegmentContainer != null)
			{
				int num2 = this.m_CurrentSegmentIndex - (this.m_ProgressBlink ? 0 : 1);
				for (int i = 0; i < this.m_SegmentContainer.childCount; i++)
				{
					Image component = this.m_SegmentContainer.GetChild(i).GetComponent<Image>();
					if (i == num2)
					{
						TweenColorGradient component2 = component.gameObject.GetComponent<TweenColorGradient>();
						if (component2 != null)
						{
							component2.style = (this.m_ProgressBlink ? UITweener.Style.PingPong : UITweener.Style.Once);
							component2.PlayForward();
						}
					}
					else
					{
						TweenColorGradient component3 = component.gameObject.GetComponent<TweenColorGradient>();
						if (component3 != null)
						{
							component3.Stop();
						}
					}
					component.color = ((i < num2) ? this.segmentColorActie : this.segmentColorDisabled);
				}
			}
		}
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x00187548 File Offset: 0x00185748
	private void Rebuild()
	{
		this.Init();
		if (this.style == BSGProgressBar.Style.Normal)
		{
			if (this.m_ClassicContainer)
			{
				this.m_ClassicContainer.gameObject.SetActive(true);
			}
			if (this.m_GridContainer)
			{
				this.m_GridContainer.gameObject.SetActive(false);
				return;
			}
		}
		else if (this.style == BSGProgressBar.Style.Segmented)
		{
			if (this.m_ClassicContainer)
			{
				this.m_ClassicContainer.gameObject.SetActive(false);
			}
			if (this.m_GridContainer)
			{
				this.m_GridContainer.gameObject.SetActive(true);
			}
			this.m_CurrentSegmentIndex = -1;
			if (this.m_SegmentCount > 0 && this.m_GridContainer != null)
			{
				this.m_SegmentContainer = (this.m_GridContainer.transform as RectTransform);
				UICommon.DeleteChildren(this.m_SegmentContainer);
				this.m_SegmentContainer.DetachChildren();
				if (this.Segment != null)
				{
					for (int i = 0; i < this.m_SegmentCount; i++)
					{
						Object.Instantiate<GameObject>(this.Segment, this.m_SegmentContainer);
					}
				}
			}
			this.CheckSegmentChange();
		}
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x0018766C File Offset: 0x0018586C
	public void Hide(bool instant = false)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (instant)
		{
			base.gameObject.SetActive(false);
			return;
		}
		TweenCanvasGroupAplha tween = base.GetComponent<TweenCanvasGroupAplha>();
		if (tween != null)
		{
			tween.delay = 1f;
			tween.from = 1f;
			tween.to = 0f;
			tween.PlayForward();
			tween.onFinished.AddListener(delegate()
			{
				this.gameObject.SetActive(false);
				tween.onFinished.RemoveAllListeners();
			});
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x00187724 File Offset: 0x00185924
	public void Show(bool instant = false)
	{
		if (base.gameObject.activeSelf)
		{
			return;
		}
		base.gameObject.SetActive(true);
		if (!instant)
		{
			TweenCanvasGroupAplha component = base.GetComponent<TweenCanvasGroupAplha>();
			if (component != null)
			{
				component.delay = 0f;
				component.from = 0f;
				component.to = 1f;
				component.PlayForward();
				return;
			}
		}
		else
		{
			CanvasGroup component2 = base.GetComponent<CanvasGroup>();
			if (component2 == null)
			{
				component2.alpha = 1f;
			}
		}
	}

	// Token: 0x0400204D RID: 8269
	[SerializeField]
	private BSGProgressBar.Style style;

	// Token: 0x0400204E RID: 8270
	[SerializeField]
	private Color segmentColorActie = new Color(1f, 1f, 1f, 1f);

	// Token: 0x0400204F RID: 8271
	[SerializeField]
	private Color segmentColorDisabled = new Color(0.6f, 0.6f, 0.6f, 1f);

	// Token: 0x04002050 RID: 8272
	[SerializeField]
	private GameObject Segment;

	// Token: 0x04002051 RID: 8273
	[UIFieldTarget("id_GridContainer")]
	private GameObject m_GridContainer;

	// Token: 0x04002052 RID: 8274
	[UIFieldTarget("id_ClassicContainer")]
	private GameObject m_ClassicContainer;

	// Token: 0x04002053 RID: 8275
	[UIFieldTarget("id_ClassicProgressValue")]
	private Image m_ClassicProgressValue;

	// Token: 0x04002054 RID: 8276
	private RectTransform m_SegmentContainer;

	// Token: 0x04002055 RID: 8277
	private int m_SegmentCount = -1;

	// Token: 0x04002056 RID: 8278
	private int m_CurrentSegmentIndex = -1;

	// Token: 0x04002058 RID: 8280
	private bool m_ProgressBlink;

	// Token: 0x04002059 RID: 8281
	private bool m_Initialzied;

	// Token: 0x0200086D RID: 2157
	public enum Style
	{
		// Token: 0x04003F2C RID: 16172
		Normal,
		// Token: 0x04003F2D RID: 16173
		Segmented
	}
}
