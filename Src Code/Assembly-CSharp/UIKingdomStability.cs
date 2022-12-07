using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000211 RID: 529
public class UIKingdomStability : MonoBehaviour, IListener
{
	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06002009 RID: 8201 RVA: 0x00126A26 File Offset: 0x00124C26
	// (set) Token: 0x0600200A RID: 8202 RVA: 0x00126A2E File Offset: 0x00124C2E
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x0600200B RID: 8203 RVA: 0x00126A37 File Offset: 0x00124C37
	public void SetKingdom(Logic.Kingdom kingdom)
	{
		this.Init();
		Logic.Kingdom kingdom2 = this.Kingdom;
		if (kingdom2 != null)
		{
			kingdom2.DelListener(this);
		}
		this.Kingdom = kingdom;
		Logic.Kingdom kingdom3 = this.Kingdom;
		if (kingdom3 != null)
		{
			kingdom3.AddListener(this);
		}
		this.BuildStatic();
		this.Refresh();
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x00126A78 File Offset: 0x00124C78
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Button == null)
		{
			this.m_Button = base.GetComponent<BSGButton>();
		}
		if (this.m_ContextActionsContainer != null)
		{
			this.m_ContextActionsContainer.gameObject.SetActive(false);
			UIKingdomStability.UIDimetionsChangeListener orAddComponent = this.m_ContextActionsContainer.gameObject.GetOrAddComponent<UIKingdomStability.UIDimetionsChangeListener>();
			orAddComponent.onRectTransformDimensionsChanged = (Action<RectTransform>)Delegate.Combine(orAddComponent.onRectTransformDimensionsChanged, new Action<RectTransform>(this.HandleContextActionsContainerSizeChange));
		}
		if (this.m_Arrow != null)
		{
			this.m_Arrow.gameObject.SetActive(false);
		}
		if (this.m_Button != null)
		{
			this.m_Button.onClick = new BSGButton.OnClick(this.HandleClick);
		}
		if (this.m_kingdomStability != null)
		{
			this.m_kingdomStability.onClick = new BSGButton.OnClick(this.HandleOpenStability);
		}
		if (this.m_OpenPoliticalView != null)
		{
			this.m_OpenPoliticalView.onClick = new BSGButton.OnClick(this.HandleOpenPoliticalView);
		}
		this.tooltip = Tooltip.Get(base.gameObject, true);
		this.tooltip.SetDef("KingdomStabilityTooltip", new Vars());
		this.LoadRiskValueTresholds();
		this.m_Initialzied = true;
	}

	// Token: 0x0600200D RID: 8205 RVA: 0x00126BC4 File Offset: 0x00124DC4
	private void Update()
	{
		if (this.Kingdom != BaseUI.LogicKingdom())
		{
			this.SetKingdom(BaseUI.LogicKingdom());
			return;
		}
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
			return;
		}
		if (this.tooltip != null && this.Kingdom != null)
		{
			this.tooltip.vars.Set<string>("revolt_risk", string.Concat(new string[]
			{
				"#<color=#",
				this.GetRisktextColor((int)this.Kingdom.GetRebellionRiskGlobal()),
				">",
				((int)this.Kingdom.GetRebellionRiskGlobal()).ToString(),
				"</color>"
			}));
		}
		this.UpdateStats();
	}

	// Token: 0x0600200E RID: 8206 RVA: 0x00126C80 File Offset: 0x00124E80
	private void BuildStatic()
	{
		if (this.tooltip != null)
		{
			this.tooltip.vars.obj = this.Kingdom;
		}
		if (this.m_kingdomStability != null)
		{
			Tooltip.Get(this.m_kingdomStability.gameObject, true).SetDef("InspectStabilityTooltip", new Vars(this.Kingdom));
		}
		if (this.m_OpenPoliticalView != null)
		{
			Tooltip.Get(this.m_OpenPoliticalView.gameObject, true).SetDef("InstectPoliticalViewStabilityTooltip", new Vars(this.Kingdom));
		}
		if (this.m_RebelsCount != null)
		{
			Tooltip.Get(this.m_RebelsCount.gameObject, true).SetDef("ActiveRebelArmiesCountTooltip", new Vars(this.Kingdom));
		}
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x00126D64 File Offset: 0x00124F64
	private void ToggleOptions()
	{
		if (this.m_ContextActionsContainer != null)
		{
			if (this.m_ContextActionsContainer.activeSelf)
			{
				this.m_ContextActionsContainer.gameObject.SetActive(false);
				Image arrow = this.m_Arrow;
				if (arrow == null)
				{
					return;
				}
				arrow.gameObject.SetActive(false);
				return;
			}
			else
			{
				this.m_ContextActionsContainer.gameObject.SetActive(true);
				Image arrow2 = this.m_Arrow;
				if (arrow2 != null)
				{
					arrow2.gameObject.SetActive(true);
				}
				this.BuildOptions();
			}
		}
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x00126DE4 File Offset: 0x00124FE4
	private void BuildOptions()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_crownAuthority != null)
		{
			this.m_crownAuthority.SetKingdom(this.Kingdom);
		}
		if (this.m_RebelionIconsContainer != null)
		{
			UICommon.DeleteChildren(this.m_RebelionIconsContainer);
			if (this.Kingdom.rebellions != null && this.Kingdom.rebellions.Count > 0)
			{
				for (int i = this.Kingdom.rebellions.Count - 1; i >= 0; i--)
				{
					GameObject icon = ObjectIcon.GetIcon(this.Kingdom.rebellions[i], null, this.m_RebelionIconsContainer);
					if (!(icon == null))
					{
						UIRebellionIcon component = icon.GetComponent<UIRebellionIcon>();
						if (component != null)
						{
							component.OnSelect += this.HanldeOnRebelionIconOnSelect;
						}
					}
				}
			}
		}
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x00126EC0 File Offset: 0x001250C0
	private void HanldeOnRebelionIconOnSelect(UIRebellionIcon obj)
	{
		this.OpenStabilityWindow(obj.Data);
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x00126ED0 File Offset: 0x001250D0
	private void Refresh()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_RebelsCount != null)
		{
			UIText.SetText(this.m_RebelsCount, this.GetRebelArmiesCount().ToString());
		}
		this.BuildOptions();
	}

	// Token: 0x06002013 RID: 8211 RVA: 0x00126F14 File Offset: 0x00125114
	private void UpdateStats()
	{
		if (this.m_Stability != null)
		{
			UIText.SetText(this.m_Stability, ((int)this.Kingdom.GetRebellionRiskGlobal()).ToString());
		}
	}

	// Token: 0x06002014 RID: 8212 RVA: 0x00126F50 File Offset: 0x00125150
	private int GetRebelArmiesCount()
	{
		if (this.Kingdom == null)
		{
			return 0;
		}
		int num = 0;
		List<Logic.Realm> realms = this.Kingdom.realms;
		for (int i = 0; i < realms.Count; i++)
		{
			List<Logic.Army> armies = realms[i].armies;
			for (int j = 0; j < armies.Count; j++)
			{
				if (armies[j].rebel != null)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x00126FBC File Offset: 0x001251BC
	private void LoadRiskValueTresholds()
	{
		DT.Field defField = global::Defs.GetDefField("KingdomStabilityWindow", null);
		DT.Field field = defField.FindChild("riskTresholds", null, true, true, true, '.');
		int num = field.NumValues();
		this.rebellionRiskTresholds.Clear();
		for (int i = 0; i < num; i++)
		{
			this.rebellionRiskTresholds.Add(field.Value(i, null, true, true));
		}
		DT.Field field2 = defField.FindChild("riskTresholdColors", null, true, true, true, '.');
		num = field2.NumValues();
		this.rebellionRiskTresholdColors.Clear();
		for (int j = 0; j < num; j++)
		{
			this.rebellionRiskTresholdColors.Add(field2.String(j, null, ""));
		}
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x00127074 File Offset: 0x00125274
	private string GetRisktextColor(int riskValue)
	{
		int num = 0;
		while (num < this.rebellionRiskTresholds.Count && (float)riskValue > this.rebellionRiskTresholds[num])
		{
			num++;
		}
		return this.rebellionRiskTresholdColors[num];
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x001270B8 File Offset: 0x001252B8
	private void HandleContextActionsContainerSizeChange(RectTransform t)
	{
		if (t.rect.width <= this.maxNonRestrictedWidth)
		{
			Vector3 localPosition = t.localPosition;
			localPosition.x = 0f;
			t.localPosition = localPosition;
			return;
		}
		float num = (this.maxNonRestrictedWidth - t.rect.width) / 2f;
		Vector3 localPosition2 = t.localPosition;
		localPosition2.x = -num;
		t.localPosition = localPosition2;
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x00127129 File Offset: 0x00125329
	private void HandleOpenStability(BSGButton btn)
	{
		this.OpenStabilityWindow(null);
		this.ToggleOptions();
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x00127138 File Offset: 0x00125338
	private void HandleOpenPoliticalView(BSGButton btn)
	{
		ViewMode viewMode = ViewMode.Get("RebellionRisk");
		if (viewMode == null)
		{
			return;
		}
		viewMode.Apply();
		this.ToggleOptions();
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x000023FD File Offset: 0x000005FD
	public void OpenStabilityWindow(Rebellion r = null)
	{
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x00127160 File Offset: 0x00125360
	private void HandleClick(BSGButton btn)
	{
		this.ToggleOptions();
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x00127168 File Offset: 0x00125368
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "rebellion_leader_started" || message == "rebellion_zone_joined" || message == "rebellion_ended" || message == "rebellions_changed" || message == "rebel_type_changed" || message == "rebellion_new_leader")
		{
			this.m_Invalidate = true;
		}
	}

	// Token: 0x0600201D RID: 8221 RVA: 0x001271CA File Offset: 0x001253CA
	private void OnDestroy()
	{
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom == null)
		{
			return;
		}
		kingdom.DelListener(this);
	}

	// Token: 0x0400153F RID: 5439
	public string HostRectName = "id_MessageContainer";

	// Token: 0x04001540 RID: 5440
	[UIFieldTarget("id_Stability")]
	private TextMeshProUGUI m_Stability;

	// Token: 0x04001541 RID: 5441
	[UIFieldTarget("id_RebelsCount")]
	private TextMeshProUGUI m_RebelsCount;

	// Token: 0x04001542 RID: 5442
	[UIFieldTarget("id_Buttom")]
	private BSGButton m_Button;

	// Token: 0x04001543 RID: 5443
	[UIFieldTarget("id_ContextActionsContainer")]
	private GameObject m_ContextActionsContainer;

	// Token: 0x04001544 RID: 5444
	[UIFieldTarget("id_CrownAuthority")]
	private global::CrownAuthority m_crownAuthority;

	// Token: 0x04001545 RID: 5445
	[UIFieldTarget("id_OpenKingodmStability")]
	private BSGButton m_kingdomStability;

	// Token: 0x04001546 RID: 5446
	[UIFieldTarget("id_OpenPoliticalView")]
	private BSGButton m_OpenPoliticalView;

	// Token: 0x04001547 RID: 5447
	[UIFieldTarget("id_Arrow")]
	private Image m_Arrow;

	// Token: 0x04001548 RID: 5448
	[UIFieldTarget("id_RebelionIconsContainer")]
	private RectTransform m_RebelionIconsContainer;

	// Token: 0x0400154A RID: 5450
	private bool m_Invalidate;

	// Token: 0x0400154B RID: 5451
	private bool m_Initialzied;

	// Token: 0x0400154C RID: 5452
	private Tooltip tooltip;

	// Token: 0x0400154D RID: 5453
	private List<float> rebellionRiskTresholds = new List<float>();

	// Token: 0x0400154E RID: 5454
	private List<string> rebellionRiskTresholdColors = new List<string>();

	// Token: 0x0400154F RID: 5455
	private float maxNonRestrictedWidth = 260f;

	// Token: 0x0200074A RID: 1866
	internal class UIDimetionsChangeListener : UIBehaviour
	{
		// Token: 0x06004A7A RID: 19066 RVA: 0x00220BC9 File Offset: 0x0021EDC9
		protected override void OnRectTransformDimensionsChange()
		{
			Action<RectTransform> action = this.onRectTransformDimensionsChanged;
			if (action == null)
			{
				return;
			}
			action(base.transform as RectTransform);
		}

		// Token: 0x04003975 RID: 14709
		public Action<RectTransform> onRectTransformDimensionsChanged;
	}
}
