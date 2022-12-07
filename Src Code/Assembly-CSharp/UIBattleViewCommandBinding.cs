using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C6 RID: 454
public class UIBattleViewCommandBinding : MonoBehaviour
{
	// Token: 0x06001AD6 RID: 6870 RVA: 0x0010341E File Offset: 0x0010161E
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001AD7 RID: 6871 RVA: 0x00103428 File Offset: 0x00101628
	public void Set(Vars vars)
	{
		this.m_CommandLabel.text = vars.Get("command_text", true);
		this.SetupKey(this.m_Key1, this.m_KeyText1, vars.Get<Sprite>("background_1", null), vars.Get<string>("key_text_1", null));
		this.SetupKey(this.m_Key2, this.m_KeyText2, vars.Get<Sprite>("background_2", null), vars.Get<string>("key_text_2", null));
		Tooltip.Get(base.gameObject, true).SetText(vars.Get<string>("tooltip_localzied", null), null, null);
	}

	// Token: 0x06001AD8 RID: 6872 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void Initialize()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06001AD9 RID: 6873 RVA: 0x001034C4 File Offset: 0x001016C4
	private void SetupKey(Image keyImage, TextMeshProUGUI keyText, Sprite sprite, string text)
	{
		if (sprite != null && keyImage != null)
		{
			if (this.IsSpriteSliced(sprite))
			{
				keyImage.preserveAspect = false;
				keyImage.type = Image.Type.Sliced;
			}
			else
			{
				keyImage.type = Image.Type.Simple;
				keyImage.preserveAspect = true;
			}
			keyImage.sprite = sprite;
			UIText.SetText(keyText, text);
			keyImage.gameObject.SetActive(true);
			return;
		}
		keyImage.gameObject.SetActive(false);
	}

	// Token: 0x06001ADA RID: 6874 RVA: 0x00103532 File Offset: 0x00101732
	private bool IsSpriteSliced(Sprite sprite)
	{
		return sprite.border != Vector4.zero;
	}

	// Token: 0x04001179 RID: 4473
	[UIFieldTarget("id_CommandLabel")]
	private TextMeshProUGUI m_CommandLabel;

	// Token: 0x0400117A RID: 4474
	[UIFieldTarget("id_Key1")]
	private Image m_Key1;

	// Token: 0x0400117B RID: 4475
	[UIFieldTarget("id_KeyText1")]
	private TextMeshProUGUI m_KeyText1;

	// Token: 0x0400117C RID: 4476
	[UIFieldTarget("id_Key2")]
	private Image m_Key2;

	// Token: 0x0400117D RID: 4477
	[UIFieldTarget("id_KeyText2")]
	private TextMeshProUGUI m_KeyText2;
}
