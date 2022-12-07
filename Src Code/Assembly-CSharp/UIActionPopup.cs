using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000304 RID: 772
public class UIActionPopup : MonoBehaviour
{
	// Token: 0x06003056 RID: 12374 RVA: 0x0018871A File Offset: 0x0018691A
	private void Awake()
	{
		if (this.ItemPrototype != null)
		{
			this.ItemPrototype.SetActive(false);
		}
	}

	// Token: 0x06003057 RID: 12375 RVA: 0x00188736 File Offset: 0x00186936
	private void OnDestroy()
	{
		this.HanldeClose();
	}

	// Token: 0x06003058 RID: 12376 RVA: 0x00188740 File Offset: 0x00186940
	private void Update()
	{
		if ((UICommon.GetKeyDown(KeyCode.Mouse0, UICommon.ModifierKey.None, UICommon.ModifierKey.None) || UICommon.GetKeyDown(KeyCode.Mouse1, UICommon.ModifierKey.None, UICommon.ModifierKey.None) || UICommon.GetKeyDown(KeyCode.Mouse2, UICommon.ModifierKey.None, UICommon.ModifierKey.None)) && !RectTransformUtility.RectangleContainsScreenPoint(this.Container, Input.mousePosition, null))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06003059 RID: 12377 RVA: 0x0018879C File Offset: 0x0018699C
	public void Rebuild()
	{
		if (this.ItemPrototype == null)
		{
			return;
		}
		if (this.data == null)
		{
			return;
		}
		for (int i = 0; i < this.data.Length; i++)
		{
			this.BuildElement(this.data[i], this.ItemPrototype, this.Container);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
	}

	// Token: 0x0600305A RID: 12378 RVA: 0x001887FF File Offset: 0x001869FF
	private void HanldeClose()
	{
		if (this.m_CloseCallback != null)
		{
			this.m_CloseCallback();
		}
	}

	// Token: 0x0600305B RID: 12379 RVA: 0x00188814 File Offset: 0x00186A14
	private GameObject BuildElement(UIActionPopup.OptionData data, GameObject elementProrotype, RectTransform parent)
	{
		if (elementProrotype == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(elementProrotype, Vector3.zero, Quaternion.identity, parent);
		gameObject.SetActive(true);
		UIActionPopup.ActionItem actionItem = gameObject.GetComponent<UIActionPopup.ActionItem>();
		if (actionItem == null)
		{
			actionItem = gameObject.AddComponent<UIActionPopup.ActionItem>();
		}
		actionItem.Fetch();
		if (actionItem.image != null)
		{
			actionItem.image.overrideSprite = data.icon;
			actionItem.image.color = data.color;
		}
		UIText.SetText(actionItem.text, data.text);
		gameObject.SetActive(true);
		if (data.tooltipVars != null)
		{
			Tooltip.Get(actionItem.gameObject, true).SetDef(data.tooltipVars.Get<string>("tooltip", null), data.tooltipVars);
		}
		actionItem.action = data.action;
		return gameObject;
	}

	// Token: 0x0600305C RID: 12380 RVA: 0x001888E8 File Offset: 0x00186AE8
	public static GameObject Create(UIActionPopup.OptionData[] data, GameObject prototype, RectTransform parent, Action closeCallback)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		if (data == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		UIActionPopup uiactionPopup = gameObject.GetComponent<UIActionPopup>();
		if (uiactionPopup == null)
		{
			uiactionPopup = gameObject.AddComponent<UIActionPopup>();
		}
		uiactionPopup.data = data;
		uiactionPopup.Rebuild();
		uiactionPopup.m_CloseCallback = closeCallback;
		return gameObject;
	}

	// Token: 0x04002071 RID: 8305
	[SerializeField]
	private GameObject ItemPrototype;

	// Token: 0x04002072 RID: 8306
	[SerializeField]
	private RectTransform Container;

	// Token: 0x04002073 RID: 8307
	private UIActionPopup.OptionData[] data;

	// Token: 0x04002074 RID: 8308
	private Action m_CloseCallback;

	// Token: 0x0200086F RID: 2159
	public class OptionData
	{
		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x0600511C RID: 20764 RVA: 0x0023BF22 File Offset: 0x0023A122
		// (set) Token: 0x0600511D RID: 20765 RVA: 0x0023BF2A File Offset: 0x0023A12A
		public string text { get; set; }

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x0600511E RID: 20766 RVA: 0x0023BF33 File Offset: 0x0023A133
		// (set) Token: 0x0600511F RID: 20767 RVA: 0x0023BF3B File Offset: 0x0023A13B
		public Vars tooltipVars { get; set; }

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06005120 RID: 20768 RVA: 0x0023BF44 File Offset: 0x0023A144
		// (set) Token: 0x06005121 RID: 20769 RVA: 0x0023BF4C File Offset: 0x0023A14C
		public Sprite icon { get; set; }

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06005122 RID: 20770 RVA: 0x0023BF55 File Offset: 0x0023A155
		// (set) Token: 0x06005123 RID: 20771 RVA: 0x0023BF5D File Offset: 0x0023A15D
		public Color color { get; set; }

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06005124 RID: 20772 RVA: 0x0023BF66 File Offset: 0x0023A166
		// (set) Token: 0x06005125 RID: 20773 RVA: 0x0023BF6E File Offset: 0x0023A16E
		public Action action { get; set; }

		// Token: 0x06005126 RID: 20774 RVA: 0x0023BF78 File Offset: 0x0023A178
		public override string ToString()
		{
			return string.Format("OptinsData text: {0} tooltip {1} icon {2} color {3}", new object[]
			{
				this.text,
				this.tooltipVars,
				(this.icon == null) ? "null" : this.icon.name,
				this.color
			});
		}
	}

	// Token: 0x02000870 RID: 2160
	protected internal class ActionItem : Hotspot
	{
		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06005127 RID: 20775 RVA: 0x0023BFD8 File Offset: 0x0023A1D8
		// (set) Token: 0x06005128 RID: 20776 RVA: 0x0023BFE0 File Offset: 0x0023A1E0
		public TextMeshProUGUI text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				this.m_Text = value;
			}
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06005129 RID: 20777 RVA: 0x0023BFE9 File Offset: 0x0023A1E9
		// (set) Token: 0x0600512A RID: 20778 RVA: 0x0023BFF1 File Offset: 0x0023A1F1
		public Image image
		{
			get
			{
				return this.m_Image;
			}
			set
			{
				this.m_Image = value;
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x0600512B RID: 20779 RVA: 0x0023BFFA File Offset: 0x0023A1FA
		// (set) Token: 0x0600512C RID: 20780 RVA: 0x0023C002 File Offset: 0x0023A202
		public RectTransform rectTransform
		{
			get
			{
				return this.m_RectTransform;
			}
			set
			{
				this.m_RectTransform = value;
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x0600512D RID: 20781 RVA: 0x0023C00B File Offset: 0x0023A20B
		// (set) Token: 0x0600512E RID: 20782 RVA: 0x0023C013 File Offset: 0x0023A213
		public Action action { get; set; }

		// Token: 0x0600512F RID: 20783 RVA: 0x0023C01C File Offset: 0x0023A21C
		public void Fetch()
		{
			if (this.m_Image == null)
			{
				this.m_Image = global::Common.GetComponent<Image>(base.gameObject, "id_Icon");
			}
			if (this.m_Text == null)
			{
				this.m_Text = global::Common.GetComponent<TextMeshProUGUI>(base.gameObject, "id_Text");
			}
		}

		// Token: 0x06005130 RID: 20784 RVA: 0x0023C071 File Offset: 0x0023A271
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			if (this.action != null)
			{
				this.action();
			}
		}

		// Token: 0x04003F35 RID: 16181
		[SerializeField]
		private TextMeshProUGUI m_Text;

		// Token: 0x04003F36 RID: 16182
		[SerializeField]
		private Image m_Image;

		// Token: 0x04003F37 RID: 16183
		[SerializeField]
		private RectTransform m_RectTransform;
	}
}
