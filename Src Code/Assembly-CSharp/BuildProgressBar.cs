using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A0 RID: 672
public class BuildProgressBar : MonoBehaviour
{
	// Token: 0x0600299A RID: 10650 RVA: 0x001614C5 File Offset: 0x0015F6C5
	private void Init()
	{
		if (this.m_Initilzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initilzed = true;
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x001614E0 File Offset: 0x0015F6E0
	public void SetData(Castle castle, Building.Def building_def)
	{
		this.Init();
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (((kingdom != null) ? kingdom.game : null) == null || building_def == null)
		{
			this.building_def = null;
			this.Hide(false);
			return;
		}
		this.building_def = building_def;
		this.castle = castle;
		this.Build();
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x0016152C File Offset: 0x0015F72C
	private void Build()
	{
		if (this.building_def == null)
		{
			return;
		}
		bool flag = this.building_def.IsUpgrade();
		this.m_Upgrade.gameObject.SetActive(flag);
		if (this.m_BuildingIcon != null)
		{
			Building.Def def = flag ? this.building_def.GetFirstUpgradeOf() : this.building_def;
			if (this.castle != null)
			{
				this.m_BuildingIcon.sprite = global::Defs.GetObj<Sprite>(def.field, "icon", this.castle);
			}
			else
			{
				this.m_BuildingIcon.sprite = global::Defs.GetObj<Sprite>(def.field, "icon", BaseUI.LogicKingdom());
			}
		}
		if (flag && this.m_UpgaradeIcon != null)
		{
			this.m_UpgaradeIcon.sprite = global::Defs.GetObj<Sprite>(this.building_def.field, "icon", this.castle);
		}
		if (this.m_BuildingBorder != null)
		{
			this.m_BuildingBorder.color = (flag ? this.color_complete : this.color_building);
		}
		if (this.m_UpgradeBorder != null)
		{
			this.m_UpgradeBorder.color = this.color_building;
		}
	}

	// Token: 0x0600299D RID: 10653 RVA: 0x00161650 File Offset: 0x0015F850
	private void LateUpdate()
	{
		if (!this.m_Initilzed)
		{
			return;
		}
		if (this.building_def == null)
		{
			return;
		}
		float num = this.CalcProgress();
		if (num < 0f || num > 1f)
		{
			GameObject buildProgressUpgrade = this.m_BuildProgressUpgrade;
			if (buildProgressUpgrade != null)
			{
				buildProgressUpgrade.gameObject.SetActive(false);
			}
			GameObject buildProgressBuilding = this.m_BuildProgressBuilding;
			if (buildProgressBuilding != null)
			{
				buildProgressBuilding.gameObject.SetActive(false);
			}
			if (this.m_BuildingBorder != null)
			{
				this.m_BuildingBorder.color = this.color_complete;
			}
			if (this.m_UpgradeBorder != null)
			{
				this.m_UpgradeBorder.color = this.color_complete;
			}
			return;
		}
		bool flag = this.building_def.IsUpgrade();
		if (flag && this.m_BuildProgressUpgradeValue != null)
		{
			this.m_BuildProgressUpgradeValue.fillAmount = num;
			GameObject buildProgressUpgrade2 = this.m_BuildProgressUpgrade;
			if (buildProgressUpgrade2 != null)
			{
				buildProgressUpgrade2.gameObject.SetActive(true);
			}
			this.m_BuildingBorder.color = this.color_complete;
		}
		if (!flag && this.m_BuildProgressBuildingValue != null)
		{
			this.m_BuildProgressBuildingValue.fillAmount = num;
			GameObject buildProgressBuilding2 = this.m_BuildProgressBuilding;
			if (buildProgressBuilding2 == null)
			{
				return;
			}
			buildProgressBuilding2.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x00161778 File Offset: 0x0015F978
	private float CalcProgress()
	{
		if (this.building_def == null)
		{
			return -1f;
		}
		if (this.building_def.IsUpgrade())
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null)
			{
				return -1f;
			}
			return kingdom.GetUpgradeProgress(this.building_def);
		}
		else
		{
			if (this.castle == null)
			{
				return -1f;
			}
			if (this.castle.GetCurrentBuildingBuild() != this.building_def)
			{
				return -1f;
			}
			return this.castle.GetBuildPorgress();
		}
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x001617F0 File Offset: 0x0015F9F0
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

	// Token: 0x060029A0 RID: 10656 RVA: 0x00161898 File Offset: 0x0015FA98
	private void Show()
	{
		base.gameObject.SetActive(true);
		TweenCanvasGroupAplha component = base.GetComponent<TweenCanvasGroupAplha>();
		if (component != null)
		{
			component.delay = 0f;
			component.from = 0f;
			component.to = 1f;
			component.PlayForward();
		}
	}

	// Token: 0x04001C35 RID: 7221
	[UIFieldTarget("id_Building")]
	private GameObject m_Building;

	// Token: 0x04001C36 RID: 7222
	[UIFieldTarget("id_BuildingIcon")]
	private Image m_BuildingIcon;

	// Token: 0x04001C37 RID: 7223
	[UIFieldTarget("id_BuildProgressBuilding")]
	private GameObject m_BuildProgressBuilding;

	// Token: 0x04001C38 RID: 7224
	[UIFieldTarget("id_BuildProgressBuildingValue")]
	private Image m_BuildProgressBuildingValue;

	// Token: 0x04001C39 RID: 7225
	[UIFieldTarget("id_Upgrade")]
	private GameObject m_Upgrade;

	// Token: 0x04001C3A RID: 7226
	[UIFieldTarget("id_UpgaradeIcon")]
	private Image m_UpgaradeIcon;

	// Token: 0x04001C3B RID: 7227
	[UIFieldTarget("id_BuildProgressUpgrade")]
	private GameObject m_BuildProgressUpgrade;

	// Token: 0x04001C3C RID: 7228
	[UIFieldTarget("id_BuildProgressUpgradeValue")]
	private Image m_BuildProgressUpgradeValue;

	// Token: 0x04001C3D RID: 7229
	[UIFieldTarget("id_BuildingBorder")]
	private Image m_BuildingBorder;

	// Token: 0x04001C3E RID: 7230
	[UIFieldTarget("id_UpgradeBorder")]
	private Image m_UpgradeBorder;

	// Token: 0x04001C3F RID: 7231
	private Castle castle;

	// Token: 0x04001C40 RID: 7232
	private Building.Def building_def;

	// Token: 0x04001C41 RID: 7233
	public bool KeepAfterComplete;

	// Token: 0x04001C42 RID: 7234
	private bool m_Initilzed;

	// Token: 0x04001C43 RID: 7235
	private Color color_building = new Color32(137, 137, 70, 31);

	// Token: 0x04001C44 RID: 7236
	private Color color_complete = new Color32(byte.MaxValue, byte.MaxValue, 135, 191);
}
