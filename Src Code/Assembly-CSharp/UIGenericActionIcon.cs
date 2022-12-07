using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000306 RID: 774
public class UIGenericActionIcon : Hotspot
{
	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06003064 RID: 12388 RVA: 0x00188A72 File Offset: 0x00186C72
	// (set) Token: 0x06003065 RID: 12389 RVA: 0x00188A7A File Offset: 0x00186C7A
	public Vars Vars { get; private set; }

	// Token: 0x06003066 RID: 12390 RVA: 0x00188A84 File Offset: 0x00186C84
	public void SetData(Action action, Sprite sprite, Vars vars)
	{
		this.m_Action = action;
		this.Vars = vars;
		UICommon.FindComponents(this, false);
		if (sprite == null && this.Vars != null)
		{
			sprite = this.Vars.Get<Sprite>("sprite", null);
		}
		if (this.Image_Icon != null)
		{
			this.Image_Icon.overrideSprite = sprite;
		}
		if (this.Vars != null)
		{
			Tooltip.Get(base.gameObject, true).SetDef(this.Vars.Get<string>("tooltip", null), this.Vars);
			this.m_LableUpdateCallback = this.Vars.Get<Func<string>>("label_value", null);
			string text = this.Vars.Get<string>("label", string.Empty);
			if (this.TMP_Label != null)
			{
				UIText.SetText(this.TMP_Label, text);
			}
			this.m_SlowLabel = (this.m_LableUpdateCallback != null || text != string.Empty);
			if (this.Image_Icon != null)
			{
				this.Image_Icon.color = this.Vars.Get<Color>("icon_tint", Color.white);
			}
		}
		if (this.m_Group_Label != null)
		{
			this.m_Group_Label.gameObject.SetActive(this.m_SlowLabel);
		}
		this.UpdateHighlight();
	}

	// Token: 0x06003067 RID: 12391 RVA: 0x00188BD2 File Offset: 0x00186DD2
	private void LateUpdate()
	{
		if (this.m_Group_Label && this.TMP_Label != null && this.m_LableUpdateCallback != null)
		{
			UIText.SetText(this.TMP_Label, this.m_LableUpdateCallback());
		}
	}

	// Token: 0x06003068 RID: 12392 RVA: 0x00188C0D File Offset: 0x00186E0D
	public void Enabled(bool enabled)
	{
		if (this.m_Enabled == enabled)
		{
			return;
		}
		this.m_Enabled = enabled;
		this.UpdateHighlight();
	}

	// Token: 0x06003069 RID: 12393 RVA: 0x00188C26 File Offset: 0x00186E26
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x0600306A RID: 12394 RVA: 0x00188C3F File Offset: 0x00186E3F
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x00188C4E File Offset: 0x00186E4E
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x00188C5D File Offset: 0x00186E5D
	public override void OnClick(PointerEventData e)
	{
		if (!this.m_Enabled)
		{
			return;
		}
		base.OnClick(e);
		if (this.m_Action != null)
		{
			this.m_Action();
		}
		if (this.OnSelect != null)
		{
			this.OnSelect(this, e);
		}
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x00188C98 File Offset: 0x00186E98
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		GameObject group_Selected = this.Group_Selected;
		if (group_Selected != null)
		{
			group_Selected.SetActive(this.m_Selected);
		}
		Image image_Disabled = this.Image_Disabled;
		if (image_Disabled != null)
		{
			image_Disabled.gameObject.SetActive(!this.m_Enabled);
		}
		Image image_Selected = this.Image_Selected;
		if (image_Selected != null)
		{
			image_Selected.gameObject.SetActive(this.m_Selected);
		}
		Image image_Glow = this.Image_Glow;
		if (image_Glow == null)
		{
			return;
		}
		image_Glow.gameObject.SetActive(this.mouse_in);
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x00188D1A File Offset: 0x00186F1A
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("GenericActionIcon", null);
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x00188D28 File Offset: 0x00186F28
	public static UIGenericActionIcon Posses(Action action, GameObject gameObject, Vars vars)
	{
		if (gameObject == null)
		{
			Debug.LogWarning("Fail to posses - posessee not provied");
			return null;
		}
		UIGenericActionIcon uigenericActionIcon = gameObject.GetComponent<UIGenericActionIcon>();
		if (uigenericActionIcon == null)
		{
			uigenericActionIcon = gameObject.AddComponent<UIGenericActionIcon>();
		}
		uigenericActionIcon.SetData(action, null, vars);
		return uigenericActionIcon;
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x00188D6C File Offset: 0x00186F6C
	public static UIGenericActionIcon Create(Action action, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no prototype provided.");
			return null;
		}
		if (parent == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no parent provided.");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		UIGenericActionIcon uigenericActionIcon = gameObject.GetComponent<UIGenericActionIcon>();
		if (uigenericActionIcon == null)
		{
			uigenericActionIcon = gameObject.AddComponent<UIGenericActionIcon>();
		}
		uigenericActionIcon.SetData(action, null, vars);
		return uigenericActionIcon;
	}

	// Token: 0x04002077 RID: 8311
	[UIFieldTarget("id_Icon")]
	private Image Image_Icon;

	// Token: 0x04002078 RID: 8312
	[UIFieldTarget("id_Glow")]
	private Image Image_Glow;

	// Token: 0x04002079 RID: 8313
	[UIFieldTarget("id_Selected")]
	private Image Image_Selected;

	// Token: 0x0400207A RID: 8314
	[UIFieldTarget("id_Disabled")]
	private Image Image_Disabled;

	// Token: 0x0400207B RID: 8315
	[UIFieldTarget("id_Group_Label")]
	private GameObject m_Group_Label;

	// Token: 0x0400207C RID: 8316
	[UIFieldTarget("id_Label")]
	private TextMeshProUGUI TMP_Label;

	// Token: 0x0400207D RID: 8317
	[UIFieldTarget("id_GroupSelected")]
	private GameObject Group_Selected;

	// Token: 0x0400207E RID: 8318
	private Action m_Action;

	// Token: 0x04002080 RID: 8320
	private bool m_Selected;

	// Token: 0x04002081 RID: 8321
	private bool m_Enabled = true;

	// Token: 0x04002082 RID: 8322
	private bool m_SlowLabel;

	// Token: 0x04002083 RID: 8323
	private Func<string> m_LableUpdateCallback;

	// Token: 0x04002084 RID: 8324
	public Action<UIGenericActionIcon, PointerEventData> OnSelect;
}
