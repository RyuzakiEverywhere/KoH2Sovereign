using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002B0 RID: 688
public class UIDistrictPanel : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06002B43 RID: 11075 RVA: 0x00161BCA File Offset: 0x0015FDCA
	public Game game
	{
		get
		{
			return GameLogic.Get(false);
		}
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x0016E7B4 File Offset: 0x0016C9B4
	public void Init(District.Def def, Logic.Kingdom kingdom, Castle castle)
	{
		UICommon.FindComponents(this, false);
		this.def = def;
		this.kingdom = (kingdom ?? ((castle != null) ? castle.GetKingdom() : null));
		this.castle = castle;
		this.defs_version = global::Defs.Version;
		if (def == null)
		{
			return;
		}
		bool flag = true;
		if (this.id_Background != null)
		{
			if (def.panel_background <= 0f)
			{
				this.id_Background.enabled = false;
			}
			else
			{
				flag = true;
				if (def.panel_background_colorize)
				{
					Color color = this.id_Background.color;
					color.a = def.panel_background;
					this.id_Background.color = color;
				}
				else
				{
					Color white = Color.white;
					white.a = def.panel_background;
					this.id_Background.color = white;
				}
				this.id_Background.enabled = true;
			}
		}
		if (this.id_Focused != null)
		{
			Color backgrounColor = UIBuildingSlot.GetBackgrounColor(def);
			this.id_Focused.color = backgrounColor;
		}
		bool flag2 = false;
		if (this.id_Frame != null)
		{
			Color color2 = global::Defs.GetColor(def.panel_border, Color.clear, null);
			if (color2.a <= 0f)
			{
				this.id_Frame.gameObject.SetActive(false);
			}
			else
			{
				flag2 = true;
				this.id_Frame.color = color2;
				this.id_Frame.gameObject.SetActive(true);
			}
		}
		if (this.id_Shadow != null)
		{
			Color color3 = global::Defs.GetColor(def.panel_shadow, Color.clear, null);
			if (color3.a <= 0f || (!flag && !flag2))
			{
				this.id_Shadow.gameObject.SetActive(false);
			}
			else
			{
				this.id_Shadow.color = color3;
				this.id_Shadow.gameObject.SetActive(true);
			}
		}
		string text = global::Defs.Localize(def.field, "name", null, null, false, true);
		if (string.IsNullOrEmpty(text))
		{
			if (this.id_DistrictCaption != null)
			{
				this.id_DistrictCaption.SetActive(false);
			}
		}
		else
		{
			UIText.SetText(this.id_DisctrictName, text);
			if (this.id_DistrictCaption != null)
			{
				this.id_DistrictCaption.SetActive(true);
			}
		}
		if (this.id_BuildingsPanel != null)
		{
			this.id_BuildingsPanel.preview_def_id = null;
			this.id_BuildingsPanel.Init(def, kingdom, castle);
			if (this.id_SettlementCount != null)
			{
				int num = Application.isPlaying ? this.id_BuildingsPanel.GetRelevatSettlementCount() : 3;
				this.id_SettlementCount.gameObject.SetActive(num > 0);
				UIText.SetText(this.id_SettlementCountLabel, num.ToString());
			}
			if (!this.id_BuildingsPanel.def.IsUpgrades())
			{
				this.id_BuildingsPanel.OnSelected = new Action<UIBuildingSlot, PointerEventData>(this.HandelOnSlotSelected);
			}
		}
		float num2 = def.panel_size.x;
		if (this.id_Background != null && (flag || flag2))
		{
			num2 += 2f * def.panel_padding;
		}
		if (def.panel_min_width > 0f && num2 < def.panel_min_width)
		{
			num2 = def.panel_min_width;
		}
		base.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num2);
		LayoutElement component = base.GetComponent<LayoutElement>();
		if (component != null)
		{
			component.preferredWidth = num2;
		}
		this.UpdateHightlight();
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x0016EAEC File Offset: 0x0016CCEC
	private void HandelOnSlotSelected(UIBuildingSlot slot, PointerEventData e)
	{
		Castle castle = ((slot != null) ? slot.Castle : null) ?? this.castle;
		if (this.kingdom == null && castle != null)
		{
			castle.GetKingdom();
		}
		bool flag;
		UIBuildingWindow uibuildingWindow = UIBuildingWindow.Create(castle, null, slot.Def, this.castle, out flag);
		if (!flag)
		{
			return;
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(uibuildingWindow.transform as RectTransform);
		Vector3[] array = new Vector3[4];
		(base.transform as RectTransform).GetWorldCorners(array);
		Rect rect = (uibuildingWindow.transform as RectTransform).rect;
		if (array[3].x + rect.width < (float)Screen.width)
		{
			uibuildingWindow.transform.position = array[3] + new Vector3(this.spacing, 0f, 0f);
		}
		else
		{
			uibuildingWindow.transform.position = array[0] - new Vector3(this.spacing + rect.width, 0f, 0f);
		}
		UICommon.EnsureInScreen(uibuildingWindow.gameObject, 0.85f);
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x0016EC04 File Offset: 0x0016CE04
	private void UpdateHightlight()
	{
		if (this.id_Focused != null)
		{
			this.id_Focused.enabled = this.mouse_in;
		}
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x0016EC25 File Offset: 0x0016CE25
	public void OnPointerEnter(PointerEventData e)
	{
		this.mouse_in = true;
		this.UpdateHightlight();
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x0016EC34 File Offset: 0x0016CE34
	public void OnPointerExit(PointerEventData e)
	{
		this.mouse_in = false;
		this.UpdateHightlight();
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x00168944 File Offset: 0x00166B44
	private void OnEnable()
	{
		if (base.GetComponents<TooltipBlocker>() != null)
		{
			TooltipPlacement.AddBlocker(base.gameObject, base.transform);
		}
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x0016895F File Offset: 0x00166B5F
	private void OnDisable()
	{
		if (base.GetComponents<TooltipBlocker>() != null)
		{
			TooltipPlacement.DelBlocker(base.gameObject);
		}
	}

	// Token: 0x04001D7E RID: 7550
	[UIFieldTarget("id_Background")]
	private Image id_Background;

	// Token: 0x04001D7F RID: 7551
	[UIFieldTarget("id_Frame")]
	private Image id_Frame;

	// Token: 0x04001D80 RID: 7552
	[UIFieldTarget("id_Focused")]
	private Image id_Focused;

	// Token: 0x04001D81 RID: 7553
	[UIFieldTarget("id_Shadow")]
	private Image id_Shadow;

	// Token: 0x04001D82 RID: 7554
	[UIFieldTarget("id_DistrictCaption")]
	private GameObject id_DistrictCaption;

	// Token: 0x04001D83 RID: 7555
	[UIFieldTarget("id_DisctrictName")]
	private TextMeshProUGUI id_DisctrictName;

	// Token: 0x04001D84 RID: 7556
	[UIFieldTarget("id_SettlementCount")]
	private GameObject id_SettlementCount;

	// Token: 0x04001D85 RID: 7557
	[UIFieldTarget("id_SettlementCountLabel")]
	private TextMeshProUGUI id_SettlementCountLabel;

	// Token: 0x04001D86 RID: 7558
	[UIFieldTarget("id_BuildingsPanel")]
	private UIBuildingsPanel id_BuildingsPanel;

	// Token: 0x04001D87 RID: 7559
	public District.Def def;

	// Token: 0x04001D88 RID: 7560
	public Logic.Kingdom kingdom;

	// Token: 0x04001D89 RID: 7561
	public Castle castle;

	// Token: 0x04001D8A RID: 7562
	public string preview_def_id = "TestDistrict1";

	// Token: 0x04001D8B RID: 7563
	private int defs_version;

	// Token: 0x04001D8C RID: 7564
	private float spacing = 20f;

	// Token: 0x04001D8D RID: 7565
	private bool mouse_in;
}
