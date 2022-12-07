using System;
using System.Collections;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001FA RID: 506
public class CrownAuthority : MonoBehaviour, IListener, Tooltip.IHandler
{
	// Token: 0x06001EC6 RID: 7878 RVA: 0x0011D4BE File Offset: 0x0011B6BE
	private IEnumerator Start()
	{
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
		yield break;
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x0011D4C8 File Offset: 0x0011B6C8
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Icon != null)
		{
			this.m_Icon.SetAudioSet(string.Empty);
			this.m_Icon.onEvent = new BSGButton.OnEvent(this.OnHoverIcon);
			this.m_Icon.onClick = delegate(BSGButton b)
			{
				if (UICommon.GetKey(KeyCode.LeftControl, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat decrease crown authority", true))
				{
					this.logic.ChangeValue(-1);
					return;
				}
				if (UICommon.GetKey(KeyCode.LeftShift, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat increase crown authority", true))
				{
					this.logic.ChangeValue(1);
					return;
				}
				if (this.logic.IncreaseValueWithGold(true))
				{
					DT.Field soundsDef = BaseUI.soundsDef;
					BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("crown_authority_increase_button", null, "", true, true, true, '.') : null, null);
					return;
				}
				DT.Field soundsDef2 = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("action_unavailable", null, "", true, true, true, '.') : null, null);
			};
		}
		this.tooltip_vars = new Vars();
		this.tooltip = Tooltip.Get(base.gameObject, true);
		this.tooltip.handler = new Tooltip.Handler(this.HandleTooltip);
		this.tooltip.SetDef("CrownAuthorityBarTooltip", null);
		this.tooltip.SetVars(this.tooltip_vars);
		if (this.m_KingdomRebellionRiskContianer != null)
		{
			this.m_RebellionRiskTooltip = Tooltip.Get(this.m_KingdomRebellionRiskContianer.gameObject, true);
			this.m_RebellionRiskTooltip.SetDef("KingodmStabilityTooltip", null);
			this.m_RebellionRiskTooltip.SetVars(this.tooltip_vars);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06001EC8 RID: 7880 RVA: 0x000DF539 File Offset: 0x000DD739
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x0011D5D8 File Offset: 0x0011B7D8
	private void Update()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if ((this.logic == null || this.logic.obj == null || this.logic.obj != kingdom) && kingdom != null && kingdom.IsValid())
		{
			this.SetKingdom(kingdom);
		}
		if (this.logic == null)
		{
			return;
		}
		this.UpdateRebellionRisk();
		this.RefreshIconTooltip();
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x0011D638 File Offset: 0x0011B838
	public void SetKingdom(Logic.Kingdom kingdom)
	{
		this.Init();
		if (this.logic != null)
		{
			(this.logic.obj as Logic.Kingdom).DelListener(this);
		}
		if (kingdom != null)
		{
			kingdom.AddListener(this);
			this.logic = kingdom.GetCrownAuthority();
		}
		else
		{
			this.logic = null;
		}
		this.Refresh();
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x0011D690 File Offset: 0x0011B890
	public void OnHoverIcon(BSGButton btn, BSGButton.Event e, PointerEventData eventData)
	{
		if (e == BSGButton.Event.Enter)
		{
			this.RefreshIconTooltip();
			if (this.m_IconCrown != null)
			{
				this.m_IconCrown.SetActive(true);
				return;
			}
		}
		else if (e == BSGButton.Event.Leave && this.m_IconCrown != null)
		{
			this.m_IconCrown.SetActive(false);
		}
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x0011D6E0 File Offset: 0x0011B8E0
	private void RefreshIconTooltip()
	{
		Vars vars = this.tooltip_vars;
		Logic.CrownAuthority crownAuthority = this.logic;
		vars.obj = ((crownAuthority != null) ? crownAuthority.obj : null);
		WorldUI.FillWorstRealmVars(this.tooltip_vars);
		if (this.m_Icon != null)
		{
			Tooltip.Get(this.m_Icon.gameObject, true).SetDef("CrownAuthorityIconTooltip", this.tooltip_vars);
		}
	}

	// Token: 0x06001ECE RID: 7886 RVA: 0x0011D74C File Offset: 0x0011B94C
	private void Refresh()
	{
		if (this.logic == null)
		{
			return;
		}
		int num = this.logic.Min();
		this.logic.Max();
		int value = this.logic.GetValue();
		int num2 = value - num;
		this.RefreshIconTooltip();
		string text = (value >= 0) ? value.ToString() : string.Format("<color=red>{0}</color>", value);
		UIText.SetText(this.m_IconText, text);
		if (this.m_SegmentContainer != null)
		{
			int num3 = this.m_SegmentContainer.childCount / 2;
			for (int i = 0; i < this.m_SegmentContainer.childCount; i++)
			{
				UnityEngine.Component component = this.m_SegmentContainer.GetChild(i).GetComponent<Image>();
				bool flag = false;
				if (i >= num2 && i < num3)
				{
					flag = true;
				}
				if (i <= num2 && i > num3)
				{
					flag = true;
				}
				TweenColorGradient component2 = component.gameObject.GetComponent<TweenColorGradient>();
				if (component2 != null && flag)
				{
					component2.PlayForward();
				}
				if (component2 != null && !flag)
				{
					component2.PlayReverse();
				}
			}
		}
		if (this.m_PowerBar != null && this.m_Tumb != null)
		{
			int num4 = this.logic.Max() - this.logic.Min() + 1;
			float width = this.m_PowerBar.rect.width;
			float num5 = width / (float)num4;
			Vector3 localPosition = this.m_Tumb.localPosition;
			Vector3 vector = new Vector3(-width / 2f + (float)num2 * num5 + num5 / 2f, localPosition.y, localPosition.z);
			TweenPosition component3 = this.m_Tumb.gameObject.GetComponent<TweenPosition>();
			if (component3 != null)
			{
				component3.from = localPosition;
				component3.to = vector;
				component3.ResetToBeginning();
				component3.PlayForward();
				return;
			}
			this.m_Tumb.localPosition = vector;
		}
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x0011D93C File Offset: 0x0011BB3C
	private void UpdateRebellionRisk()
	{
		if (!(this.m_KingdomRebellionRiskLabel == null))
		{
			Logic.CrownAuthority crownAuthority = this.logic;
			if (((crownAuthority != null) ? crownAuthority.kingdom : null) != null)
			{
				this.logic.kingdom.GetRebellionRiskGlobal();
				UIText.SetText(this.m_KingdomRebellionRiskLabel, global::Defs.Localize("Kingdom.stability.value", this.logic.obj as Logic.Kingdom, null, true, true));
				return;
			}
		}
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x0011D9A5 File Offset: 0x0011BBA5
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt == Tooltip.Event.Fill || evt == Tooltip.Event.Update)
		{
			WorldUI.FillWorstRealmVars(tooltip.vars);
		}
		return false;
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x0011D9BB File Offset: 0x0011BBBB
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "crown_authority_change")
		{
			this.Refresh();
			return;
		}
		if (message == "destroying" || message == "finishing")
		{
			this.SetKingdom(null);
			base.enabled = true;
			return;
		}
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x0011D9FA File Offset: 0x0011BBFA
	private void OnDestroy()
	{
		if (this.logic != null)
		{
			(this.logic.obj as Logic.Kingdom).DelListener(this);
		}
	}

	// Token: 0x04001424 RID: 5156
	[UIFieldTarget("id_GridContainer")]
	private RectTransform m_SegmentContainer;

	// Token: 0x04001425 RID: 5157
	[UIFieldTarget("id_Icon")]
	private BSGButton m_Icon;

	// Token: 0x04001426 RID: 5158
	[UIFieldTarget("id_CrownAuthorityPowerText")]
	private TextMeshProUGUI m_IconText;

	// Token: 0x04001427 RID: 5159
	[UIFieldTarget("CrownAuthorityPowerText")]
	private GameObject m_IconCrown;

	// Token: 0x04001428 RID: 5160
	[UIFieldTarget("id_PowerBar")]
	private RectTransform m_PowerBar;

	// Token: 0x04001429 RID: 5161
	[UIFieldTarget("id_Tumb")]
	private RectTransform m_Tumb;

	// Token: 0x0400142A RID: 5162
	[UIFieldTarget("id_KingdomRebellionRisk")]
	private RectTransform m_KingdomRebellionRiskContianer;

	// Token: 0x0400142B RID: 5163
	[UIFieldTarget("id_KingdomRebelionRiskLabel")]
	private TextMeshProUGUI m_KingdomRebellionRiskLabel;

	// Token: 0x0400142C RID: 5164
	private Logic.CrownAuthority logic;

	// Token: 0x0400142D RID: 5165
	private Tooltip tooltip;

	// Token: 0x0400142E RID: 5166
	private Tooltip m_RebellionRiskTooltip;

	// Token: 0x0400142F RID: 5167
	private bool m_Initialzied;

	// Token: 0x04001430 RID: 5168
	private Vars tooltip_vars;

	// Token: 0x02000737 RID: 1847
	private class CategoryValue
	{
		// Token: 0x06004A26 RID: 18982 RVA: 0x0021F746 File Offset: 0x0021D946
		public CategoryValue(RebellionRiskCategory.Def def, float value)
		{
			this.def = def;
			this.value = value;
		}

		// Token: 0x04003900 RID: 14592
		public RebellionRiskCategory.Def def;

		// Token: 0x04003901 RID: 14593
		public float value;
	}
}
