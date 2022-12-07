using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002AA RID: 682
public class UICastleFortifications : MonoBehaviour, IListener, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000215 RID: 533
	// (get) Token: 0x06002ABA RID: 10938 RVA: 0x0016A28D File Offset: 0x0016848D
	// (set) Token: 0x06002ABB RID: 10939 RVA: 0x0016A295 File Offset: 0x00168495
	public Castle Castle { get; private set; }

	// Token: 0x06002ABC RID: 10940 RVA: 0x0016A2A0 File Offset: 0x001684A0
	public void SetObject(Castle castle)
	{
		this.Init();
		Castle castle2 = this.Castle;
		if (castle2 != null)
		{
			castle2.DelListener(this);
		}
		this.Castle = castle;
		Castle castle3 = this.Castle;
		if (castle3 != null)
		{
			castle3.AddListener(this);
		}
		if (this.m_TownGuards != null)
		{
			this.m_TownGuards.SetObject(ResourceType.TownGuards, this.Castle, null);
		}
		UIActionIcon citadel = this.m_Citadel;
		if (citadel != null)
		{
			Castle castle4 = this.Castle;
			citadel.SetObject((castle4 != null) ? castle4.GetRealm().actions.Find("UpgradeFortificationsAction") : null, null);
		}
		Castle castle5 = this.Castle;
		if (((castle5 != null) ? castle5.GetKingdom() : null) != BaseUI.LogicKingdom())
		{
			Tooltip.Get(base.gameObject, true).SetDef("ForeignTownFortificationsTooltip", new Vars(this.Castle));
		}
		else
		{
			Tooltip.Get(base.gameObject, true).SetDef(null, null);
		}
		this.Refresh();
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x0016A38C File Offset: 0x0016858C
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_SiegeDefence != null)
		{
			this.m_SiegeDefenceTooltip = Tooltip.Get(this.m_SiegeDefence.gameObject, true);
			this.m_SiegeDefenceTooltip.SetDef("SiegeDefenseTooltip", null);
		}
		if (this.m_RepairIcon != null)
		{
			this.m_RepairIcon.gameObject.SetActive(false);
		}
		if (this.m_UpgradeIcon != null)
		{
			this.m_UpgradeIcon.gameObject.SetActive(false);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x0016A424 File Offset: 0x00168624
	private void Refresh()
	{
		this.UpdateSiegeDefense();
		this.UpdateCitadelLevel();
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x0016A432 File Offset: 0x00168632
	private void Update()
	{
		if (UnityEngine.Time.unscaledTime > this.m_NextRefresh)
		{
			this.UpdateSiegeDefense();
			this.m_NextRefresh = UnityEngine.Time.unscaledTime + this.m_RefreshInterval;
		}
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x0016A45C File Offset: 0x0016865C
	private void UpdateSiegeDefense()
	{
		if (this.m_SiegeDefence == null)
		{
			return;
		}
		if (this.Castle == null || this.Castle.keep_effects == null)
		{
			return;
		}
		if (this.m_SiegeDefenceTooltip.vars == null)
		{
			this.m_SiegeDefenceTooltip.SetVars(new Vars(this.Castle));
		}
		Castle castle = this.Castle;
		float? num;
		if (castle == null)
		{
			num = null;
		}
		else
		{
			ComputableValue siege_defense_condition = castle.keep_effects.siege_defense_condition;
			num = ((siege_defense_condition != null) ? new float?(siege_defense_condition.Get()) : null);
		}
		float num2 = (num ?? 100f) / 100f;
		Logic.Realm realm = this.Castle.GetRealm();
		float num3 = (realm != null) ? realm.GetStat(Stats.rs_siege_defense, false) : 0f;
		this.m_SiegeDefenceValue.text = Mathf.Round(num3 * num2).ToString();
		bool flag = num2 < 0.95f;
		bool active_defence_recovery_boost = this.Castle.keep_effects.active_defence_recovery_boost;
		bool flag2 = this.Castle.CanUpgradeFortification();
		bool flag3 = this.Castle.fortifications.IsUpgrading();
		float fillAmount = 1f;
		if (flag3)
		{
			fillAmount = this.Castle.fortifications.GetUpgradeProgress();
		}
		else if (flag)
		{
			fillAmount = num2;
		}
		if (this.m_CitadelIcon != null)
		{
			this.m_CitadelIcon.fillAmount = fillAmount;
		}
		if (this.m_RepairPorgressBar != null)
		{
			this.m_RepairPorgressBar.fillAmount = fillAmount;
		}
		if (this.m_CitadelIconDesaturated != null)
		{
			Color color;
			if (flag3)
			{
				color = this.tintUpgrade;
			}
			else if (flag)
			{
				color = this.tintRepair;
			}
			else
			{
				color = Color.white;
			}
			this.m_CitadelIconDesaturated.color = color;
		}
		if (this.m_RepairIcon != null)
		{
			this.m_RepairIcon.gameObject.SetActive(this.m_MouseIn && flag && !active_defence_recovery_boost);
		}
		if (this.m_UpgradeIcon != null)
		{
			this.m_UpgradeIcon.gameObject.SetActive(this.m_MouseIn && flag2 && !flag);
		}
		if (this.m_UpgradOrRepaireInProgress != null)
		{
			this.m_UpgradOrRepaireInProgress.SetActive(flag3 || active_defence_recovery_boost);
		}
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x0016A6A8 File Offset: 0x001688A8
	private void UpdateCitadelLevel()
	{
		int num = this.Castle.fortifications.level;
		if (this.Castle.fortifications.IsUpgrading())
		{
			num++;
		}
		if (this.m_CitadelIcon != null)
		{
			this.m_CitadelIcon.overrideSprite = global::Defs.GetObj<Sprite>(num, this.Castle.fortifications.def.field, "icon", null);
		}
		if (this.m_CitadelIconDesaturated != null)
		{
			this.m_CitadelIconDesaturated.overrideSprite = global::Defs.GetObj<Sprite>(num, this.Castle.fortifications.def.field, "icon", null);
		}
		if (this.m_ForificationLevelValue)
		{
			this.m_ForificationLevelValue.text = this.Castle.fortifications.level.ToString();
		}
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x0016A780 File Offset: 0x00168980
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "fortification_upgarde_started"))
		{
			if (!(message == "fortification_upgrade_complete"))
			{
				if (!(message == "fortification_repair_boost_started"))
				{
					if (!(message == "fortification_repair_boost_complete"))
					{
						if (!(message == "fortification_changed") && !(message == "fortification_upgrade_canceled"))
						{
							return;
						}
						this.Refresh();
						return;
					}
					else
					{
						this.Refresh();
						if (!this.Castle.IsOwnStance(BaseUI.LogicKingdom()))
						{
							return;
						}
						DT.Field soundsDef = BaseUI.soundsDef;
						BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("fortifications_repair_finished", null, "", true, true, true, '.') : null, null);
						return;
					}
				}
				else
				{
					this.Refresh();
					if (!this.Castle.IsOwnStance(BaseUI.LogicKingdom()))
					{
						return;
					}
					DT.Field soundsDef2 = BaseUI.soundsDef;
					BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("fortifications_repair_started", null, "", true, true, true, '.') : null, null);
					return;
				}
			}
			else
			{
				this.Refresh();
				if (!this.Castle.IsOwnStance(BaseUI.LogicKingdom()))
				{
					return;
				}
				DT.Field soundsDef3 = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef3 != null) ? soundsDef3.GetString("fortifications_upgrade_finished", null, "", true, true, true, '.') : null, null);
				DT.Field soundsDef4 = BaseUI.soundsDef;
				BaseUI.PlayVoiceEvent((soundsDef4 != null) ? soundsDef4.GetString("narrator_fortifications_finished", null, "", true, true, true, '.') : null, null);
				return;
			}
		}
		else
		{
			this.Refresh();
			if (!this.Castle.IsOwnStance(BaseUI.LogicKingdom()))
			{
				return;
			}
			DT.Field soundsDef5 = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef5 != null) ? soundsDef5.GetString("fortifications_upgrade_started", null, "", true, true, true, '.') : null, null);
			return;
		}
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x0016A91C File Offset: 0x00168B1C
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.m_MouseIn = true;
		this.UpdateSiegeDefense();
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x0016A92B File Offset: 0x00168B2B
	public void OnPointerExit(PointerEventData eventData)
	{
		this.m_MouseIn = false;
		this.UpdateSiegeDefense();
	}

	// Token: 0x04001CED RID: 7405
	[UIFieldTarget("id_SiegeDefence")]
	private GameObject m_SiegeDefence;

	// Token: 0x04001CEE RID: 7406
	[UIFieldTarget("id_SiegeDefenceValue")]
	private TextMeshProUGUI m_SiegeDefenceValue;

	// Token: 0x04001CEF RID: 7407
	[UIFieldTarget("id_ForificationLevelValue")]
	private TextMeshProUGUI m_ForificationLevelValue;

	// Token: 0x04001CF0 RID: 7408
	[UIFieldTarget("id_TownGuards")]
	private UIResourceIncome m_TownGuards;

	// Token: 0x04001CF1 RID: 7409
	[UIFieldTarget("id_Citadel")]
	private UIActionIcon m_Citadel;

	// Token: 0x04001CF2 RID: 7410
	[UIFieldTarget("id_CitadelIcon")]
	private Image m_CitadelIcon;

	// Token: 0x04001CF3 RID: 7411
	[UIFieldTarget("id_RepairPorgressBar")]
	private Image m_RepairPorgressBar;

	// Token: 0x04001CF4 RID: 7412
	[UIFieldTarget("id_CitadelIconDesaturated")]
	private Image m_CitadelIconDesaturated;

	// Token: 0x04001CF5 RID: 7413
	[UIFieldTarget("id_UpgradOrRepaireInProgress")]
	private GameObject m_UpgradOrRepaireInProgress;

	// Token: 0x04001CF6 RID: 7414
	[UIFieldTarget("id_RepairIcon")]
	private Image m_RepairIcon;

	// Token: 0x04001CF7 RID: 7415
	[UIFieldTarget("id_UpgradeIcon")]
	private Image m_UpgradeIcon;

	// Token: 0x04001CF9 RID: 7417
	private bool m_Initialzied;

	// Token: 0x04001CFA RID: 7418
	private bool m_MouseIn;

	// Token: 0x04001CFB RID: 7419
	private Tooltip m_SiegeDefenceTooltip;

	// Token: 0x04001CFC RID: 7420
	private float m_RefreshInterval = 0.5f;

	// Token: 0x04001CFD RID: 7421
	private float m_NextRefresh;

	// Token: 0x04001CFE RID: 7422
	private Color32 tintRepair = new Color32(159, 60, 226, 63);

	// Token: 0x04001CFF RID: 7423
	private Color32 tintUpgrade = new Color32(108, 108, 230, 63);
}
