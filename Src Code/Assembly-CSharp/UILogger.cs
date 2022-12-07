using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002F9 RID: 761
public class UILogger : MonoBehaviour
{
	// Token: 0x06002FD8 RID: 12248 RVA: 0x00186A0C File Offset: 0x00184C0C
	public void Awake()
	{
		this.Init();
		this.Show(true);
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x00186A1C File Offset: 0x00184C1C
	private void Init()
	{
		if (this.m_Inited)
		{
			return;
		}
		this.window_def = global::Defs.GetDefField("WorldRadio", null);
		this.m_MaxActiveMessages = this.window_def.GetInt("max_messages", null, this.m_MaxActiveMessages, true, true, true, '.');
		this.ui = BaseUI.Get();
		UICommon.FindComponents(this, false);
		if (this.m_ShrinkBtn != null)
		{
			this.m_ShrinkBtn.onClick = new BSGButton.OnClick(this.Shrink);
		}
		if (this.m_ExpandBtn != null)
		{
			this.m_ExpandBtn.onClick = new BSGButton.OnClick(this.Expand);
		}
		if (this.m_HideBtn != null)
		{
			this.m_HideBtn.onClick = new BSGButton.OnClick(this.ToggleHide);
		}
		if (this.m_ShowBtn != null)
		{
			this.m_ShowBtn.onClick = new BSGButton.OnClick(this.ToggleHide);
		}
		if (this.m_CopyToClipboardBtn != null)
		{
			this.m_CopyToClipboardBtn.onClick = new BSGButton.OnClick(this.CopyToClipboard);
		}
		if (this.m_Prototype != null)
		{
			this.m_Prototype.SetActive(false);
			this.msg_Container = this.m_Prototype.transform.parent;
		}
		this.scrollbar = this.m_Scrollbar.GetComponent<Scrollbar>();
		this.scrollViewRect = this.m_ScrollView.GetComponent<ScrollRect>();
		this.m_ScrollView.gameObject.SetActive(true);
		this.isHidden = false;
		this.Shrink(null);
		this.m_Inited = true;
		DT.Field defField;
		if (Game.isLoadingSaveGame)
		{
			defField = global::Defs.GetDefField("GameLoadedMessage", null);
		}
		else
		{
			defField = global::Defs.GetDefField("GameStartedMessage", null);
		}
		Vars vars = new Vars();
		vars.Set<string>("_localized_radio", global::Defs.Localize(defField, "radio", null, null, true, true));
		if (this.m_CategoryTogglePrototype != null)
		{
			this.m_CategoryTogglePrototype.gameObject.SetActive(false);
		}
		this.AddMessage(defField, vars);
		this.PopulateFilters();
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x00186C18 File Offset: 0x00184E18
	private void ToggleHide(BSGButton btn)
	{
		this.isHidden = !this.isHidden;
		if (this.m_DataContainer != null)
		{
			this.m_DataContainer.gameObject.SetActive(!this.isHidden);
		}
		if (this.m_ShowBtn != null)
		{
			this.m_ShowBtn.gameObject.SetActive(this.isHidden);
		}
		this.ui.m_InvalidateSelection = true;
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x00186C8C File Offset: 0x00184E8C
	private void Shrink(BSGButton btn)
	{
		this.m_ShrinkBtn.gameObject.SetActive(false);
		this.m_ExpandBtn.gameObject.SetActive(true);
		this.m_ScrollView.GetComponent<LayoutElement>().preferredHeight = this.shrinkSize;
		if (this.scrollbar != null)
		{
			this.scrollbar.value = 0f;
		}
		this.m_Scrollbar.SetActive(false);
		if (this.scrollViewRect != null)
		{
			this.scrollViewRect.enabled = false;
		}
		this.expanded = false;
		this.ui.m_InvalidateSelection = true;
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x00186D28 File Offset: 0x00184F28
	private void Expand(BSGButton btn)
	{
		this.m_ShrinkBtn.gameObject.SetActive(true);
		this.m_ExpandBtn.gameObject.SetActive(false);
		this.m_ScrollView.GetComponent<LayoutElement>().preferredHeight = this.expandSize;
		if (this.scrollbar != null)
		{
			this.scrollbar.value = 0f;
		}
		this.m_Scrollbar.SetActive(true);
		if (this.scrollViewRect != null)
		{
			this.scrollViewRect.enabled = true;
		}
		this.expanded = true;
		this.ui.m_InvalidateSelection = true;
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x00186DC4 File Offset: 0x00184FC4
	private void CopyToClipboard(BSGButton btn)
	{
		string text = string.Empty;
		if (WorldRadio.logs != null)
		{
			for (int i = 0; i < WorldRadio.logs.Count; i++)
			{
				UIText.StripLinks(WorldRadio.logs[i].text);
				text = text + "[" + WorldRadio.logs[i].timestamp + "]";
				text += WorldRadio.logs[i].text;
			}
			text = UIText.ToPlainText(text);
			Game.CopyToClipboard(text);
		}
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x00186E4E File Offset: 0x0018504E
	public void Show(bool show)
	{
		if (!this.m_Inited)
		{
			this.Init();
		}
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		base.gameObject.SetActive(show);
		if (!activeInHierarchy && base.gameObject.activeInHierarchy)
		{
			this.Refresh();
		}
	}

	// Token: 0x06002FDF RID: 12255 RVA: 0x00186E8A File Offset: 0x0018508A
	public void Refresh()
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
	}

	// Token: 0x06002FE0 RID: 12256 RVA: 0x00186E98 File Offset: 0x00185098
	public void Clear()
	{
		WorldRadio.Clear();
		if (this.msg_Container != null)
		{
			this.m_Messages.Clear();
			UICommon.DeleteActiveChildren(this.msg_Container);
		}
	}

	// Token: 0x06002FE1 RID: 12257 RVA: 0x00186EC4 File Offset: 0x001850C4
	private void AddMessage(WorldRadio.Log log)
	{
		UILogger.UIRadioMessage uiradioMessage = UILogger.UIRadioMessage.Create(log.category, log.timestamp, log.text, this.m_Prototype, this.msg_Container);
		if (uiradioMessage != null)
		{
			this.m_Messages.Add(uiradioMessage);
			uiradioMessage.gameObject.SetActive(this.IsActiveCategory(log.category));
		}
		this.Refresh();
		if (this.m_Messages.Count > this.m_MaxActiveMessages)
		{
			UILogger.UIRadioMessage uiradioMessage2 = this.m_Messages[0];
			this.m_Messages.RemoveAt(0);
			if (uiradioMessage2 != null && uiradioMessage2.gameObject != null)
			{
				global::Common.DestroyObj(uiradioMessage2.gameObject);
			}
		}
	}

	// Token: 0x06002FE2 RID: 12258 RVA: 0x00186F78 File Offset: 0x00185178
	public void RecreateAllMessages()
	{
		if (this.msg_Container != null)
		{
			this.m_Messages.Clear();
			UICommon.DeleteActiveChildren(this.msg_Container);
		}
		for (int i = 0; i < WorldRadio.logs.Count; i++)
		{
			this.AddMessage(WorldRadio.logs[i]);
		}
	}

	// Token: 0x06002FE3 RID: 12259 RVA: 0x00186FCF File Offset: 0x001851CF
	public void AddMessage(DT.Field def_field, Vars vars)
	{
		if (def_field == null)
		{
			return;
		}
		WorldRadio.AddMessage(def_field, vars, this.m_MaxActiveMessages);
		this.AddMessage(WorldRadio.last);
	}

	// Token: 0x06002FE4 RID: 12260 RVA: 0x00186FF0 File Offset: 0x001851F0
	private void PopulateFilters()
	{
		this.m_Filters.Clear();
		DT.Field field = this.window_def;
		DT.Field field2 = (field != null) ? field.FindChild("categories", null, true, true, true, '.') : null;
		if (field2 == null)
		{
			return;
		}
		List<DT.Field> list = field2.Children();
		if (list == null || list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field3 = list[i];
			UILogger.FilterToggleButton filterToggleButton = UILogger.FilterToggleButton.Create(field3, this.m_CategoryTogglePrototype, this.m_FiltersContainer);
			filterToggleButton.onValueChange = new Action<UILogger.FilterToggleButton, bool>(this.HandleOnToggle);
			filterToggleButton.Set(true, false);
			this.m_Filters.Add(new UILogger.CategoryData
			{
				toggle = filterToggleButton,
				def = field3,
				category = field3.GetString("category", null, "common", true, true, true, '.')
			});
		}
		this.ApplyFilters();
	}

	// Token: 0x06002FE5 RID: 12261 RVA: 0x001870C8 File Offset: 0x001852C8
	private bool IsActiveCategory(string cat)
	{
		for (int i = 0; i < this.m_Filters.Count; i++)
		{
			UILogger.CategoryData categoryData = this.m_Filters[i];
			if (categoryData.toggle.isOn && categoryData.category == cat)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002FE6 RID: 12262 RVA: 0x00187118 File Offset: 0x00185318
	private void ApplyFilters()
	{
		if (this.m_Messages == null || this.m_Messages.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_Messages.Count; i++)
		{
			UILogger.UIRadioMessage uiradioMessage = this.m_Messages[i];
			uiradioMessage.gameObject.SetActive(this.IsActiveCategory(uiradioMessage.category));
		}
	}

	// Token: 0x06002FE7 RID: 12263 RVA: 0x00187175 File Offset: 0x00185375
	private void HandleOnToggle(UILogger.FilterToggleButton b, bool v)
	{
		this.ApplyFilters();
	}

	// Token: 0x04002030 RID: 8240
	public float shrinkSize = 75f;

	// Token: 0x04002031 RID: 8241
	public float expandSize = 180f;

	// Token: 0x04002032 RID: 8242
	[UIFieldTarget("id_DataContainer")]
	private RectTransform m_DataContainer;

	// Token: 0x04002033 RID: 8243
	[UIFieldTarget("id_ScrollView")]
	private RectTransform m_ScrollView;

	// Token: 0x04002034 RID: 8244
	[UIFieldTarget("id_Scrollbar")]
	private GameObject m_Scrollbar;

	// Token: 0x04002035 RID: 8245
	[UIFieldTarget("id_Background")]
	private GameObject m_Background;

	// Token: 0x04002036 RID: 8246
	[UIFieldTarget("id_Prototype")]
	private GameObject m_Prototype;

	// Token: 0x04002037 RID: 8247
	[UIFieldTarget("id_ShrinkBtn")]
	private BSGButton m_ShrinkBtn;

	// Token: 0x04002038 RID: 8248
	[UIFieldTarget("id_ExpandBtn")]
	private BSGButton m_ExpandBtn;

	// Token: 0x04002039 RID: 8249
	[UIFieldTarget("id_CopyToClipboard")]
	private BSGButton m_CopyToClipboardBtn;

	// Token: 0x0400203A RID: 8250
	[UIFieldTarget("id_HideBtn")]
	private BSGButton m_HideBtn;

	// Token: 0x0400203B RID: 8251
	[UIFieldTarget("id_ShowBtn")]
	private BSGButton m_ShowBtn;

	// Token: 0x0400203C RID: 8252
	[UIFieldTarget("id_FiltersContainer")]
	private RectTransform m_FiltersContainer;

	// Token: 0x0400203D RID: 8253
	[UIFieldTarget("id_CategoryTogglePrototype")]
	private GameObject m_CategoryTogglePrototype;

	// Token: 0x0400203E RID: 8254
	private bool m_Inited;

	// Token: 0x0400203F RID: 8255
	private BaseUI ui;

	// Token: 0x04002040 RID: 8256
	private Scrollbar scrollbar;

	// Token: 0x04002041 RID: 8257
	private ScrollRect scrollViewRect;

	// Token: 0x04002042 RID: 8258
	public bool isHidden;

	// Token: 0x04002043 RID: 8259
	public bool expanded;

	// Token: 0x04002044 RID: 8260
	private int m_MaxActiveMessages = 120;

	// Token: 0x04002045 RID: 8261
	private Transform msg_Container;

	// Token: 0x04002046 RID: 8262
	private DT.Field window_def;

	// Token: 0x04002047 RID: 8263
	private List<UILogger.UIRadioMessage> m_Messages = new List<UILogger.UIRadioMessage>();

	// Token: 0x04002048 RID: 8264
	private List<UILogger.CategoryData> m_Filters = new List<UILogger.CategoryData>();

	// Token: 0x02000869 RID: 2153
	private class CategoryData
	{
		// Token: 0x04003F1D RID: 16157
		public UILogger.FilterToggleButton toggle;

		// Token: 0x04003F1E RID: 16158
		public DT.Field def;

		// Token: 0x04003F1F RID: 16159
		public string category;
	}

	// Token: 0x0200086A RID: 2154
	private class FilterToggleButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06005102 RID: 20738 RVA: 0x0023BC22 File Offset: 0x00239E22
		// (set) Token: 0x06005103 RID: 20739 RVA: 0x0023BC2A File Offset: 0x00239E2A
		public bool isOn
		{
			get
			{
				return this.m_IsOn;
			}
			set
			{
				this.Set(value, true);
			}
		}

		// Token: 0x06005104 RID: 20740 RVA: 0x0023BC34 File Offset: 0x00239E34
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialized = true;
		}

		// Token: 0x06005105 RID: 20741 RVA: 0x0023BC50 File Offset: 0x00239E50
		public void SetData(DT.Field def)
		{
			this.Init();
			this.def = def;
			if (this.m_CategoryLabel != null)
			{
				UIText.SetText(this.m_CategoryLabel, global::Defs.Localize(def, "name", null, null, true, true));
			}
			if (this.m_Icon != null)
			{
				this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(def, "icon", null);
			}
			Tooltip.Get(base.gameObject, true).SetDef("WorldRadioCategoryTooltip", new Vars(def));
			this.UpdateVisuals();
		}

		// Token: 0x06005106 RID: 20742 RVA: 0x0023BCD9 File Offset: 0x00239ED9
		public void SetIsOnWithoutNotify(bool value)
		{
			this.Set(value, false);
		}

		// Token: 0x06005107 RID: 20743 RVA: 0x0023BCE3 File Offset: 0x00239EE3
		public void Set(bool value, bool sendCallback = true)
		{
			if (this.m_IsOn == value)
			{
				return;
			}
			this.m_IsOn = value;
			this.UpdateVisuals();
			if (sendCallback)
			{
				Action<UILogger.FilterToggleButton, bool> action = this.onValueChange;
				if (action == null)
				{
					return;
				}
				action(this, this.m_IsOn);
			}
		}

		// Token: 0x06005108 RID: 20744 RVA: 0x0023BD18 File Offset: 0x00239F18
		private void UpdateVisuals()
		{
			if (this.def == null)
			{
				return;
			}
			bool @bool = this.def.GetBool("show_label", null, false, true, true, true, '.');
			if (this.m_CategoryLabel != null)
			{
				if (@bool)
				{
					this.m_CategoryLabel.color = global::Defs.GetColor(this.def, this.isOn ? "category_text_color_on" : "category_text_color_off", null);
				}
				this.m_CategoryLabel.gameObject.SetActive(@bool);
			}
			if (this.m_ToggleIcon != null)
			{
				string key;
				if (!this.isOn && !this.mouse_in)
				{
					key = "toggle_icon_off_normal";
				}
				else if (!this.isOn && this.mouse_in)
				{
					key = "toggle_icon_off_hover";
				}
				else if (this.isOn && !this.mouse_in)
				{
					key = "toggle_icon_on_normal";
				}
				else
				{
					key = "toggle_icon_on_hover";
				}
				Sprite obj = global::Defs.GetObj<Sprite>(this.def, key, null);
				this.m_ToggleIcon.overrideSprite = obj;
			}
		}

		// Token: 0x06005109 RID: 20745 RVA: 0x0023BE09 File Offset: 0x0023A009
		private void InternalToggle()
		{
			this.isOn = !this.isOn;
		}

		// Token: 0x0600510A RID: 20746 RVA: 0x0023BE1A File Offset: 0x0023A01A
		public void OnPointerClick(PointerEventData eventData)
		{
			this.InternalToggle();
		}

		// Token: 0x0600510B RID: 20747 RVA: 0x0023BE24 File Offset: 0x0023A024
		public static UILogger.FilterToggleButton Create(DT.Field def, GameObject prototype, Transform parent = null)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				parent = prototype.transform.parent;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
			gameObject.SetActive(true);
			UILogger.FilterToggleButton orAddComponent = gameObject.GetOrAddComponent<UILogger.FilterToggleButton>();
			orAddComponent.SetData(def);
			return orAddComponent;
		}

		// Token: 0x0600510C RID: 20748 RVA: 0x0023BE72 File Offset: 0x0023A072
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.mouse_in = true;
			this.UpdateVisuals();
		}

		// Token: 0x0600510D RID: 20749 RVA: 0x0023BE81 File Offset: 0x0023A081
		public void OnPointerExit(PointerEventData eventData)
		{
			this.mouse_in = false;
			this.UpdateVisuals();
		}

		// Token: 0x04003F20 RID: 16160
		[UIFieldTarget("id_ToggleIcon")]
		private Image m_ToggleIcon;

		// Token: 0x04003F21 RID: 16161
		[UIFieldTarget("id_Icon")]
		private Image m_Icon;

		// Token: 0x04003F22 RID: 16162
		[UIFieldTarget("id_CategoryLabel")]
		private TMP_Text m_CategoryLabel;

		// Token: 0x04003F23 RID: 16163
		private bool m_IsOn;

		// Token: 0x04003F24 RID: 16164
		private bool mouse_in;

		// Token: 0x04003F25 RID: 16165
		private bool m_Initialized;

		// Token: 0x04003F26 RID: 16166
		private DT.Field def;

		// Token: 0x04003F27 RID: 16167
		public Action<UILogger.FilterToggleButton, bool> onValueChange;
	}

	// Token: 0x0200086B RID: 2155
	private class UIRadioMessage : MonoBehaviour, IPoolable
	{
		// Token: 0x0600510F RID: 20751 RVA: 0x0023BE90 File Offset: 0x0023A090
		public static UILogger.UIRadioMessage Create(string category, string timestamp, string msg, GameObject prototype, Transform parent = null)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				parent = prototype.transform.parent;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
			gameObject.SetActive(true);
			UILogger.UIRadioMessage uiradioMessage = gameObject.AddComponent<UILogger.UIRadioMessage>();
			UICommon.FindComponents(uiradioMessage, false);
			UIText.SetText(uiradioMessage.m_Timestamp, timestamp);
			UIText.SetText(uiradioMessage.m_Text, msg);
			uiradioMessage.category = category;
			return uiradioMessage;
		}

		// Token: 0x06005110 RID: 20752 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnPoolActivated()
		{
		}

		// Token: 0x06005111 RID: 20753 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnPoolDeactivated()
		{
		}

		// Token: 0x06005112 RID: 20754 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnPoolDestroyed()
		{
		}

		// Token: 0x06005113 RID: 20755 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnPoolSpawned()
		{
		}

		// Token: 0x04003F28 RID: 16168
		[UIFieldTarget("id_Timestamp")]
		private TMP_Text m_Timestamp;

		// Token: 0x04003F29 RID: 16169
		[UIFieldTarget("id_LogTextField")]
		private TMP_Text m_Text;

		// Token: 0x04003F2A RID: 16170
		public string category;
	}
}
