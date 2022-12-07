using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000245 RID: 581
public class MinimapViewModeButton : Hotspot, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06002368 RID: 9064 RVA: 0x0013FBE8 File Offset: 0x0013DDE8
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzed = true;
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x0013FC04 File Offset: 0x0013DE04
	public void SetData(DT.Field data)
	{
		this.Init();
		this.Data = data;
		this.icons.Extract(data.FindChild("icon", null, true, true, true, '.'));
		this.backgrounds.Extract(data.FindChild("background", null, true, true, true, '.'));
		this.m_ShowIcon = this.icons.HasValidSprites();
		this.m_ShowLabel = !this.icons.HasValidSprites();
		DT.Field data2 = this.Data;
		this.view_mode = ViewMode.Get((data2 != null) ? data2.key : null);
		this.Refresh();
	}

	// Token: 0x0600236A RID: 9066 RVA: 0x0013FC9E File Offset: 0x0013DE9E
	public void Select(bool selected)
	{
		this.m_Selected = selected;
		this.UpdateVisuals();
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x0013FCAD File Offset: 0x0013DEAD
	public void Disbaled(bool disabeld)
	{
		this.m_Disbaled = disabeld;
		this.UpdateVisuals();
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x0013FCBC File Offset: 0x0013DEBC
	public void Refresh()
	{
		if (this.m_Icon != null)
		{
			this.m_Icon.gameObject.SetActive(this.m_ShowIcon);
		}
		if (this.m_Label != null)
		{
			this.m_Label.gameObject.SetActive(this.m_ShowLabel);
			if (this.m_ShowLabel)
			{
				UIText.SetText(this.m_Label, global::Defs.Localize(this.Data, "label", null, null, true, true));
			}
		}
		string @string = this.Data.GetString("keybind", null, "", true, true, true, '.');
		this.keybind = @string;
		Tooltip tooltip = Tooltip.Get(base.gameObject, true);
		tooltip.SetText(this.Data.Path(false, false, '.') + ".tooltip_text", null, null);
		Vars vars = new Vars();
		string string2 = this.Data.GetString("keybind", null, "", true, true, true, '.');
		string text = KeyBindings.LocalizeKeybind(string2, 0, true);
		if (string.IsNullOrEmpty(text))
		{
			KeyCode keyCode;
			if (Enum.TryParse<KeyCode>(string2, out keyCode))
			{
				vars.Set<string>("keybind", "#" + string2);
			}
		}
		else
		{
			vars.Set<string>("keybind", text);
		}
		tooltip.SetVars(vars);
		this.UpdateVisuals();
	}

	// Token: 0x0600236D RID: 9069 RVA: 0x0013FDF8 File Offset: 0x0013DFF8
	private void UpdateVisuals()
	{
		if (this.m_Icon != null && this.m_ShowIcon)
		{
			Sprite overrideSprite;
			if (this.m_Selected)
			{
				overrideSprite = this.icons.selected;
			}
			else if (this.mouse_in)
			{
				overrideSprite = this.icons.over;
			}
			else if (this.m_Disbaled)
			{
				overrideSprite = this.icons.disabled;
			}
			else
			{
				overrideSprite = this.icons.normal;
			}
			this.m_Icon.overrideSprite = overrideSprite;
		}
		if (this.m_Background)
		{
			Sprite overrideSprite2;
			if (this.m_Selected)
			{
				overrideSprite2 = this.backgrounds.selected;
			}
			else if (!ViewMode.IsPoliticalView() && this.view_mode == ViewMode.secondary)
			{
				overrideSprite2 = this.backgrounds.secondary_seleced;
			}
			else if (this.mouse_in)
			{
				overrideSprite2 = this.backgrounds.over;
			}
			else if (this.m_Disbaled)
			{
				overrideSprite2 = this.backgrounds.disabled;
			}
			else
			{
				overrideSprite2 = this.backgrounds.normal;
			}
			this.m_Background.overrideSprite = overrideSprite2;
		}
	}

	// Token: 0x0600236E RID: 9070 RVA: 0x0013FF02 File Offset: 0x0013E102
	public void OnPointerClick(PointerEventData eventData)
	{
		Action<MinimapViewModeButton> onSelect = this.OnSelect;
		if (onSelect == null)
		{
			return;
		}
		onSelect(this);
	}

	// Token: 0x0600236F RID: 9071 RVA: 0x0013FF15 File Offset: 0x0013E115
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.m_PressStart = UnityEngine.Time.unscaledTime;
		UIMinimapOverlay uiminimapOverlay = this.minimapOverlay;
		if (((uiminimapOverlay != null) ? uiminimapOverlay.listMenuShown : null) != null)
		{
			this.minimapOverlay.listMenuShown.SetActive(false);
		}
	}

	// Token: 0x06002370 RID: 9072 RVA: 0x0013FF54 File Offset: 0x0013E154
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateVisuals();
	}

	// Token: 0x06002371 RID: 9073 RVA: 0x0013FF63 File Offset: 0x0013E163
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateVisuals();
	}

	// Token: 0x06002372 RID: 9074 RVA: 0x0013FF72 File Offset: 0x0013E172
	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x0013FF7C File Offset: 0x0013E17C
	public GameObject SpawnNewChildButton(GameObject paletteButton)
	{
		if (paletteButton == null)
		{
			return null;
		}
		WorldUI x = WorldUI.Get();
		if (paletteButton.transform.parent != null && x != null)
		{
			if (paletteButton.transform.parent == null)
			{
				return null;
			}
			MinimapPaletteButton component = global::Common.GetComponent<MinimapPaletteButton>(paletteButton.transform.parent, null);
			if (this.minimapOverlay.ButtonsScript != null && this.minimapOverlay.Defs != null && component.view_mode != null)
			{
				Image image = base.gameObject.GetComponent<Image>();
				if (image == null)
				{
					image = base.gameObject.AddComponent<Image>();
				}
				Color color = global::Defs.GetColor(this.minimapOverlay.Defs.FindChild(component.view_mode.name, null, true, true, true, '.'), "clr", null);
				image.color = color;
				this.SetMinimapButtonTooltip(paletteButton, this.minimapOverlay.Defs, this.view_mode.name, false);
			}
		}
		GameObject gameObject = global::Common.Spawn(paletteButton, false, false);
		gameObject.name = "id_VMButton";
		RectTransform component2 = gameObject.GetComponent<RectTransform>();
		if (component2 == null)
		{
			return null;
		}
		gameObject.transform.SetParent(base.transform, true);
		component2.localPosition = new Vector3(0f, 0f, 0f);
		component2.anchorMin = new Vector2(0.5f, 0.5f);
		component2.anchorMax = new Vector2(0.5f, 0.5f);
		component2.localScale = new Vector3(1f, 1f, 1f);
		component2.sizeDelta = new Vector2(45f, 28f);
		component2.pivot = new Vector2(0.5f, 0.5f);
		this.SetMinimapButtonTooltip(gameObject, this.minimapOverlay.Defs, this.view_mode.name, true);
		return gameObject;
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x0014016C File Offset: 0x0013E36C
	private void SetMinimapButtonTooltip(GameObject button, DT.Field def, string key, bool is_main = false)
	{
		Tooltip tooltip = Tooltip.Get(button.gameObject, true);
		tooltip.SetText(def.Path(false, false, '.') + "." + key + ".tooltip_text", null, null);
		Vars vars = new Vars();
		string @string = def.FindChild(key, null, true, true, true, '.').GetString("keybind", null, "", true, true, true, '.');
		string text = KeyBindings.LocalizeKeybind(@string, 0, true);
		if (string.IsNullOrEmpty(text))
		{
			KeyCode keyCode;
			if (Enum.TryParse<KeyCode>(@string, out keyCode))
			{
				vars.Set<string>("keybind", "#" + @string);
			}
		}
		else
		{
			vars.Set<string>("keybind", text);
		}
		vars.Set<bool>("is_main", is_main);
		tooltip.SetVars(vars);
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x00140222 File Offset: 0x0013E422
	private void Update()
	{
		if (this.mouse_down && UnityEngine.Time.unscaledTime - this.m_PressStart > 0.5f)
		{
			Action<MinimapViewModeButton> onLongPress = this.OnLongPress;
			if (onLongPress != null)
			{
				onLongPress(this);
			}
			this.mouse_down = false;
		}
	}

	// Token: 0x040017B5 RID: 6069
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040017B6 RID: 6070
	[UIFieldTarget("id_Label")]
	private TextMeshProUGUI m_Label;

	// Token: 0x040017B7 RID: 6071
	[UIFieldTarget("id_Background")]
	private Image m_Background;

	// Token: 0x040017B8 RID: 6072
	[UIFieldTarget("id_ListMenuButtonsPalette")]
	public GameObject listMenuPalette;

	// Token: 0x040017B9 RID: 6073
	[NonSerialized]
	public ViewMode view_mode;

	// Token: 0x040017BA RID: 6074
	[NonSerialized]
	public string keybind = "";

	// Token: 0x040017BB RID: 6075
	private GameObject minimapPaletteObject;

	// Token: 0x040017BC RID: 6076
	private MinimapPalette palette;

	// Token: 0x040017BD RID: 6077
	public UIMinimapOverlay minimapOverlay;

	// Token: 0x040017BE RID: 6078
	public DT.Field Data;

	// Token: 0x040017BF RID: 6079
	private bool m_Initialzed;

	// Token: 0x040017C0 RID: 6080
	private float m_PressStart;

	// Token: 0x040017C1 RID: 6081
	private bool m_Selected;

	// Token: 0x040017C2 RID: 6082
	private bool m_Disbaled;

	// Token: 0x040017C3 RID: 6083
	private bool m_ShowIcon = true;

	// Token: 0x040017C4 RID: 6084
	private bool m_ShowLabel;

	// Token: 0x040017C5 RID: 6085
	public Action<MinimapViewModeButton> OnSelect;

	// Token: 0x040017C6 RID: 6086
	public Action<MinimapViewModeButton> OnLongPress;

	// Token: 0x040017C7 RID: 6087
	private MinimapViewModeButton.SpriteSet icons = new MinimapViewModeButton.SpriteSet();

	// Token: 0x040017C8 RID: 6088
	private MinimapViewModeButton.SpriteSet backgrounds = new MinimapViewModeButton.SpriteSet();

	// Token: 0x020007A0 RID: 1952
	private class SpriteSet
	{
		// Token: 0x06004CC1 RID: 19649 RVA: 0x0022A7E0 File Offset: 0x002289E0
		public void Extract(DT.Field f)
		{
			if (f == null)
			{
				this.normal = (this.over = (this.selected = (this.disabled = null)));
				return;
			}
			this.normal = global::Defs.GetObj<Sprite>(f, "normal", null);
			this.over = global::Defs.GetObj<Sprite>(f, "over", null);
			this.selected = global::Defs.GetObj<Sprite>(f, "selected", null);
			this.disabled = global::Defs.GetObj<Sprite>(f, "disabled", null);
			this.secondary_seleced = global::Defs.GetObj<Sprite>(f, "secondary_seleced", null);
			if (this.over == null)
			{
				this.over = this.normal;
			}
			if (this.selected == null)
			{
				this.selected = this.normal;
			}
			if (this.disabled == null)
			{
				this.disabled = this.normal;
			}
		}

		// Token: 0x06004CC2 RID: 19650 RVA: 0x0022A8BB File Offset: 0x00228ABB
		public bool HasValidSprites()
		{
			return this.normal != null;
		}

		// Token: 0x04003B65 RID: 15205
		public Sprite normal;

		// Token: 0x04003B66 RID: 15206
		public Sprite over;

		// Token: 0x04003B67 RID: 15207
		public Sprite selected;

		// Token: 0x04003B68 RID: 15208
		public Sprite disabled;

		// Token: 0x04003B69 RID: 15209
		public Sprite secondary_seleced;
	}
}
