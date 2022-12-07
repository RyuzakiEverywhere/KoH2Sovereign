using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A1 RID: 673
public class UIBuildEfficiency : MonoBehaviour
{
	// Token: 0x060029A2 RID: 10658 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void Start()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x0016193D File Offset: 0x0015FB3D
	public void SetData(Castle castle, Building.Def building)
	{
		if (castle == null)
		{
			return;
		}
		if (building == null)
		{
			return;
		}
		this.m_Castle = castle;
		if (this.m_Castle != null && this.m_Castle.GetCurrentBuildingBuild() != null)
		{
			this.AddListeners();
			return;
		}
		this.Hide(true);
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x00161974 File Offset: 0x0015FB74
	private void Update()
	{
		if (this.m_Castle == null)
		{
			return;
		}
		if (this.m_Castle.GetBuildPorgress() == -1f)
		{
			this.Hide(false);
			base.enabled = false;
			return;
		}
		if (this.m_Animation != null)
		{
			this.m_Animation.Speed = this.CalcEfficiency() * this.m_AnimationSpeedMod;
		}
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x001619D4 File Offset: 0x0015FBD4
	private float CalcEfficiency()
	{
		this.m_Castle.population.Recalc(false);
		return 0.1f + (float)(this.m_Castle.population.Count(Population.Type.TOTAL, true) - this.m_Castle.population.Count(Population.Type.Rebel, true)) / (float)this.m_Castle.population.Slots(Population.Type.TOTAL, true);
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x00161A34 File Offset: 0x0015FC34
	public void Show(bool instant = false)
	{
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
			}
		}
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x00161A88 File Offset: 0x0015FC88
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
			tween.delay = 0.5f;
			tween.from = 1f;
			tween.to = 0f;
			tween.to = 0.5f;
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

	// Token: 0x060029A8 RID: 10664 RVA: 0x000023FD File Offset: 0x000005FD
	private void AddListeners()
	{
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x000023FD File Offset: 0x000005FD
	private void RemoveListeners()
	{
	}

	// Token: 0x04001C45 RID: 7237
	[UIFieldTarget("id_Animation")]
	private SpriteAnimation m_Animation;

	// Token: 0x04001C46 RID: 7238
	private Castle m_Castle;

	// Token: 0x04001C47 RID: 7239
	[SerializeField]
	private float m_AnimationSpeedMod = 2f;
}
