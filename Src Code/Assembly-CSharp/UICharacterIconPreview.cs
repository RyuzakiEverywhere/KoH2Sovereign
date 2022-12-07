using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001E6 RID: 486
public class UICharacterIconPreview : Hotspot
{
	// Token: 0x06001D54 RID: 7508 RVA: 0x001149AC File Offset: 0x00112BAC
	protected override void OnEnable()
	{
		UICommon.FindComponents(this, false);
		base.OnEnable();
	}

	// Token: 0x06001D55 RID: 7509 RVA: 0x001149BC File Offset: 0x00112BBC
	public void PreviewCharacter()
	{
		if (this.Portrait_Image != null)
		{
			base.gameObject.SetActive(true);
			DynamicIconBuilder.CharacterData.VariantData variant = DynamicIconBuilder.Instance.GetVariant(this.characterId, this.age, this.variantId);
			this.Portrait_Image.sprite = variant.GetSprite(128f, false);
		}
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x00114A18 File Offset: 0x00112C18
	public void PreviewTexture(Texture2D tex)
	{
		if (this.Portrait_Image != null)
		{
			base.gameObject.SetActive(true);
			this.Portrait_Image.sprite = Sprite.Create(tex, new Rect(0f, 0f, (float)tex.width, (float)tex.height), new Vector2(0.5f, 0.5f), 100f);
		}
	}

	// Token: 0x06001D57 RID: 7511 RVA: 0x00114A81 File Offset: 0x00112C81
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (e.pointerId == -2)
		{
			this.Close();
			return;
		}
	}

	// Token: 0x06001D58 RID: 7512 RVA: 0x000C4358 File Offset: 0x000C2558
	public void Close()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001332 RID: 4914
	[UIFieldTarget("id_PortraitPreviewImage")]
	[SerializeField]
	private Image Portrait_Image;

	// Token: 0x04001333 RID: 4915
	[UIFieldTarget("btn_Close")]
	[SerializeField]
	private Button btn_Close;

	// Token: 0x04001334 RID: 4916
	public int characterId;

	// Token: 0x04001335 RID: 4917
	public int age;

	// Token: 0x04001336 RID: 4918
	public int variantId;
}
