using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000196 RID: 406
public class UIArmyHealthBar : MonoBehaviour
{
	// Token: 0x0600169B RID: 5787 RVA: 0x000E2072 File Offset: 0x000E0272
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialized = true;
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x000E208B File Offset: 0x000E028B
	public void SetData(Logic.Unit unit)
	{
		this.Init();
		this.m_CurrentDeadIndex = this.m_Segments.Count;
		this.PreCalc();
		this.unit = unit;
		if (unit == null)
		{
			this.Hide(true);
			return;
		}
		this.Show(true);
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x000E20C4 File Offset: 0x000E02C4
	private void PreCalc()
	{
		if (this.unit == null)
		{
			return;
		}
		if (this.unit.simulation == null)
		{
			this.m_CurrentDeadIndex = Mathf.CeilToInt((1f - this.unit.damage) * (float)this.unit.def.health_segments);
			return;
		}
		this.m_CurrentDeadIndex = Mathf.CeilToInt((1f - this.unit.damage) * (float)this.unit.def.health_segments / this.unit.simulation.max_damage);
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x000E2155 File Offset: 0x000E0355
	private void Update()
	{
		if (!this.m_Initialized)
		{
			return;
		}
		if (this.style == UIArmyHealthBar.Style.Segmented)
		{
			this.CheckSegmentChange(false);
		}
		if (this.style == UIArmyHealthBar.Style.Normal)
		{
			this.CheckBarChange(false);
		}
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x000E2180 File Offset: 0x000E0380
	private void CheckSegmentChange(bool instant = false)
	{
		if (this.unit == null)
		{
			return;
		}
		bool flag = false;
		int num;
		if (this.unit.simulation == null)
		{
			num = Mathf.CeilToInt((1f - this.unit.damage) * (float)this.unit.def.health_segments);
		}
		else
		{
			num = Mathf.CeilToInt((1f - this.unit.damage) * (float)this.unit.def.health_segments / this.unit.simulation.max_damage);
		}
		if (this.unit.IsDefeated() && this.unit.damage < 1f)
		{
			num = Mathf.Max(num, 1);
		}
		if (num != this.m_CurrentDeadIndex)
		{
			this.m_CurrentDeadIndex = num;
			flag = true;
		}
		if (flag || instant)
		{
			this.RefreshColours(instant);
		}
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x000E2250 File Offset: 0x000E0450
	private void RefreshColours(bool instant = false)
	{
		if (this.m_GridContainer != null)
		{
			for (int i = 0; i < this.m_GridContainer.childCount; i++)
			{
				Image component = this.m_GridContainer.GetChild(i).GetComponent<Image>();
				TweenColorGradient component2 = component.gameObject.GetComponent<TweenColorGradient>();
				if (i >= this.m_CurrentDeadIndex)
				{
					component.color = this.segmentColorDead;
					if (component2 != null)
					{
						component2.gradient = this.gradient_dead;
					}
				}
				else
				{
					component.color = this.segmentColorActive;
				}
			}
		}
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x000E22D8 File Offset: 0x000E04D8
	public void Rebuild(bool instant = false)
	{
		HorizontalLayoutGroup barContainer = this.m_BarContainer;
		if (barContainer != null)
		{
			barContainer.gameObject.SetActive(this.style == UIArmyHealthBar.Style.Normal);
		}
		RectTransform gridContainer = this.m_GridContainer;
		if (gridContainer != null)
		{
			gridContainer.gameObject.SetActive(this.style == UIArmyHealthBar.Style.Segmented);
		}
		if (this.style == UIArmyHealthBar.Style.Segmented)
		{
			if (this.Segment != null && this.unit.def.health_segments > 0 && this.m_GridContainer != null)
			{
				if (this.m_Segments == null)
				{
					this.m_Segments = new List<GameObject>();
				}
				int num = this.unit.def.health_segments - this.m_Segments.Count;
				for (int i = 0; i < num; i++)
				{
					this.m_Segments.Add(UnityEngine.Object.Instantiate<GameObject>(this.Segment, this.m_GridContainer));
				}
				for (int j = 0; j < this.m_GridContainer.childCount; j++)
				{
					Image component = this.m_GridContainer.GetChild(j).GetComponent<Image>();
					TweenColorGradient tweenColorGradient = (component != null) ? component.gameObject.GetComponent<TweenColorGradient>() : null;
					if (tweenColorGradient != null)
					{
						tweenColorGradient.Stop();
					}
				}
			}
			this.CheckSegmentChange(instant);
		}
		if (this.style == UIArmyHealthBar.Style.Normal)
		{
			this.CheckBarChange(instant);
		}
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x000E2418 File Offset: 0x000E0618
	private void CheckBarChange(bool instant = false)
	{
		if (this.unit == null)
		{
			return;
		}
		bool flag = false;
		float num = this.unit.damage;
		if (this.unit.IsDefeated() && this.unit.damage < 1f)
		{
			num = 1f;
		}
		if (num != this.m_CurrentDeadPerc)
		{
			this.m_CurrentDeadPerc = num;
			flag = true;
		}
		if (flag || instant)
		{
			float num2 = 1f - num;
			float num3 = (this.m_BarContainer.transform as RectTransform).rect.width;
			num3 -= (float)(this.m_BarContainer.padding.left + this.m_BarContainer.padding.right);
			int num4 = 0;
			if ((double)(num3 * num2) > 0.8)
			{
				num4++;
			}
			if ((double)(num3 * num) > 0.8)
			{
				num4++;
			}
			num3 -= this.m_BarContainer.spacing * (float)Mathf.Max(0, num4 - 1);
			this.m_Healty.preferredWidth = num3 * num2;
			this.m_Healty.gameObject.SetActive(this.m_Healty.preferredWidth > 0.8f);
			this.m_Dead.preferredWidth = num3 * num;
			this.m_Dead.gameObject.SetActive(this.m_Dead.preferredWidth > 0.8f);
		}
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x000E2570 File Offset: 0x000E0770
	private void Hide(bool instant = false)
	{
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

	// Token: 0x060016A4 RID: 5796 RVA: 0x000E2618 File Offset: 0x000E0818
	private void Show(bool instant = false)
	{
		base.gameObject.SetActive(true);
		this.Rebuild(instant);
		if (!instant)
		{
			TweenCanvasGroupAplha component = base.GetComponent<TweenCanvasGroupAplha>();
			if (component != null)
			{
				component.delay = 0f;
				component.from = 0f;
				component.to = 1f;
				component.PlayForward();
			}
		}
	}

	// Token: 0x04000E98 RID: 3736
	[SerializeField]
	private Color segmentColorActive = new Color(0f, 1f, 0f, 1f);

	// Token: 0x04000E99 RID: 3737
	[SerializeField]
	private Color segmentColorDead = new Color(1f, 0f, 0f, 1f);

	// Token: 0x04000E9A RID: 3738
	[SerializeField]
	private Gradient gradient_dead = new Gradient();

	// Token: 0x04000E9B RID: 3739
	[SerializeField]
	private GameObject Segment;

	// Token: 0x04000E9C RID: 3740
	[UIFieldTarget("id_GridContainer")]
	private RectTransform m_GridContainer;

	// Token: 0x04000E9D RID: 3741
	[UIFieldTarget("id_BarContainer")]
	private HorizontalLayoutGroup m_BarContainer;

	// Token: 0x04000E9E RID: 3742
	[UIFieldTarget("id_Healty")]
	private LayoutElement m_Healty;

	// Token: 0x04000E9F RID: 3743
	[UIFieldTarget("id_Dead")]
	private LayoutElement m_Dead;

	// Token: 0x04000EA0 RID: 3744
	public UIArmyHealthBar.Style style = UIArmyHealthBar.Style.Segmented;

	// Token: 0x04000EA1 RID: 3745
	private Logic.Unit unit;

	// Token: 0x04000EA2 RID: 3746
	private int m_CurrentDeadIndex = -1;

	// Token: 0x04000EA3 RID: 3747
	private List<GameObject> m_Segments = new List<GameObject>();

	// Token: 0x04000EA4 RID: 3748
	private bool m_Initialized;

	// Token: 0x04000EA5 RID: 3749
	private float m_CurrentDeadPerc;

	// Token: 0x04000EA6 RID: 3750
	private LayoutElement m_elementHealty;

	// Token: 0x04000EA7 RID: 3751
	private LayoutElement m_elementDead;

	// Token: 0x020006CD RID: 1741
	public enum Style
	{
		// Token: 0x0400371F RID: 14111
		Normal,
		// Token: 0x04003720 RID: 14112
		Segmented
	}
}
