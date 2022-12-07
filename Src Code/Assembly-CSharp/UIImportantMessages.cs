using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000237 RID: 567
public class UIImportantMessages : MonoBehaviour
{
	// Token: 0x060022B4 RID: 8884 RVA: 0x0013AE3F File Offset: 0x0013903F
	private IEnumerator Start()
	{
		yield return null;
		bool flag = true;
		while (flag)
		{
			WorldUI ui = WorldUI.Get();
			if (ui == null)
			{
				yield return null;
			}
			if (ui.kingdom == 0)
			{
				yield return null;
			}
			flag = false;
			ui = null;
		}
		this.Init();
		this.Refresh(true);
		yield break;
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x0013AE50 File Offset: 0x00139050
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		WorldUI worldUI = WorldUI.Get();
		this.royalCourt = worldUI.gameObject.GetComponentInChildren<UIRoyalCourt>();
		RectTransform dynamic = this.m_Dynamic;
		this.showTween = ((dynamic != null) ? dynamic.GetComponent<TweenPosition>() : null);
		this.m_Initialzied = true;
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x0013AEA3 File Offset: 0x001390A3
	private void OnEnable()
	{
		IconsBar iconsBar = this.iconsBar;
		if (iconsBar == null)
		{
			return;
		}
		iconsBar.Recalc();
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x0013AEB5 File Offset: 0x001390B5
	private void Update()
	{
		this.Refresh(false);
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x0013AEC0 File Offset: 0x001390C0
	private void Refresh(bool force = false)
	{
		if (this.royalCourt == null)
		{
			this.Hide(force);
			return;
		}
		if (!this.royalCourt.HasSelecion & this.iconsBar.NumIcons > 0)
		{
			this.Show(force);
		}
		else
		{
			this.Hide(force);
		}
		if (this.m_RefreshIcons)
		{
			IconsBar iconsBar = this.iconsBar;
			if (iconsBar != null)
			{
				iconsBar.Recalc();
			}
			this.m_RefreshIcons = false;
		}
		this.ValidateIconsContainerSize();
		this.SyncSizes();
		if (force)
		{
			IconsBar iconsBar2 = this.iconsBar;
			if (iconsBar2 == null)
			{
				return;
			}
			iconsBar2.RecalcIconsSize();
		}
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x0013AF54 File Offset: 0x00139154
	private void SyncSizes()
	{
		if (this.m_Dynamic == null || this.m_Static == null)
		{
			return;
		}
		this.m_Static.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.m_Dynamic.rect.width);
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x0013AFA0 File Offset: 0x001391A0
	private void ValidateIconsContainerSize()
	{
		if (this.iconsBar == null)
		{
			return;
		}
		LayoutElement component = this.iconsBar.GetComponent<LayoutElement>();
		if (component == null)
		{
			return;
		}
		bool flag = this.iconsBar.IsVertical();
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < this.iconsBar.icons.Count; i++)
		{
			IconsBar.IconInfo iconInfo = this.iconsBar.icons[i];
			if (flag)
			{
				num = ((num < iconInfo.width) ? iconInfo.width : num);
				num2 += iconInfo.height;
				if (i > 0)
				{
					num2 += this.iconsBar.spacing;
				}
			}
			else
			{
				num += iconInfo.width;
				if (i > 0)
				{
					num += this.iconsBar.spacing;
				}
				num2 = ((num2 < iconInfo.height) ? iconInfo.height : num2);
			}
		}
		if (this.prevTotalWidth != num || this.prevTotalHeight != num2)
		{
			component.preferredWidth = num;
			component.preferredHeight = num2;
			this.m_RefreshIcons = true;
			this.prevTotalWidth = num;
			this.prevTotalHeight = num2;
		}
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x0013B0C4 File Offset: 0x001392C4
	private void Show(bool instant = true)
	{
		if (this.iconsBar != null)
		{
			this.iconsBar.gameObject.SetActive(true);
			this.iconsBar.Recalc();
		}
		if (instant)
		{
			Vector3 localPosition = (this.showTween != null) ? this.showTween.to : new Vector3(0f, 0f, 0f);
			this.m_Dynamic.transform.localPosition = localPosition;
		}
		else
		{
			if (this.shown)
			{
				return;
			}
			if (this.showTween != null)
			{
				this.showTween.PlayForward();
			}
		}
		this.shown = true;
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x0013B16C File Offset: 0x0013936C
	private void Hide(bool instant = true)
	{
		if (instant)
		{
			Vector3 localPosition = (this.showTween != null) ? this.showTween.from : new Vector3(0f, 62f, 0f);
			this.m_Dynamic.transform.localPosition = localPosition;
			IconsBar iconsBar = this.iconsBar;
			if (iconsBar != null)
			{
				iconsBar.gameObject.SetActive(false);
			}
		}
		else
		{
			if (!this.shown)
			{
				return;
			}
			if (this.showTween != null)
			{
				this.showTween.PlayReverse();
				this.showTween.onFinished.AddListener(delegate()
				{
					IconsBar iconsBar2 = this.iconsBar;
					if (iconsBar2 != null)
					{
						iconsBar2.gameObject.SetActive(false);
					}
					this.showTween.onFinished.RemoveAllListeners();
				});
			}
		}
		this.shown = false;
	}

	// Token: 0x04001744 RID: 5956
	[UIFieldTarget("id_IOIcons")]
	private IconsBar iconsBar;

	// Token: 0x04001745 RID: 5957
	[UIFieldTarget("id_Dynamic")]
	private RectTransform m_Dynamic;

	// Token: 0x04001746 RID: 5958
	[UIFieldTarget("id_Static")]
	private RectTransform m_Static;

	// Token: 0x04001747 RID: 5959
	[UIFieldTarget("id_Header")]
	private TweenColor blinkerTween;

	// Token: 0x04001748 RID: 5960
	private const int newMessageEffectDuration = 3;

	// Token: 0x04001749 RID: 5961
	private TweenPosition showTween;

	// Token: 0x0400174A RID: 5962
	private UIRoyalCourt royalCourt;

	// Token: 0x0400174B RID: 5963
	private bool m_Initialzied;

	// Token: 0x0400174C RID: 5964
	private bool shown = true;

	// Token: 0x0400174D RID: 5965
	private bool m_RefreshIcons;

	// Token: 0x0400174E RID: 5966
	private float prevTotalWidth;

	// Token: 0x0400174F RID: 5967
	private float prevTotalHeight;
}
