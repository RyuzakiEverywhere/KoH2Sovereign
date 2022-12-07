using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C8 RID: 456
[RequireComponent(typeof(Image))]
public class UIBattleViewCounterIcon : MonoBehaviour
{
	// Token: 0x06001AE5 RID: 6885 RVA: 0x0010371D File Offset: 0x0010191D
	private void FindImages()
	{
		if (this.m_bgImage == null)
		{
			this.m_bgImage = Common.GetComponent<Image>(this, null);
		}
		if (this.m_iconImage == null)
		{
			this.m_iconImage = Common.GetComponent<Image>(this, "id_IconImage");
		}
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x00103759 File Offset: 0x00101959
	public void SetUp(string unitTypeName, bool positive, Sprite bgSprite, Sprite iconSprite)
	{
		this.FindImages();
		this.m_unitTypeName = unitTypeName;
		this.m_positive = positive;
		this.m_bgSprite = bgSprite;
		this.m_iconSprite = iconSprite;
		this.UpdateTooltip();
		this.UpdateImage();
		this.UpdateUnitTypeImage();
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x00103790 File Offset: 0x00101990
	private void UpdateTooltip()
	{
		string text = this.m_positive ? "Counter" : "Countered";
		text = this.m_unitTypeName + text;
		Tooltip.Get(this.m_bgImage.gameObject, true).SetDef(text, null);
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x001037D7 File Offset: 0x001019D7
	private void UpdateImage()
	{
		this.m_bgImage.sprite = this.m_bgSprite;
	}

	// Token: 0x06001AE9 RID: 6889 RVA: 0x001037EA File Offset: 0x001019EA
	private void UpdateUnitTypeImage()
	{
		this.m_iconImage.sprite = this.m_iconSprite;
	}

	// Token: 0x04001183 RID: 4483
	[SerializeField]
	private Image m_bgImage;

	// Token: 0x04001184 RID: 4484
	[UIFieldTarget("id_IconImage")]
	private Image m_iconImage;

	// Token: 0x04001185 RID: 4485
	private bool m_positive;

	// Token: 0x04001186 RID: 4486
	private string m_unitTypeName;

	// Token: 0x04001187 RID: 4487
	private Sprite m_bgSprite;

	// Token: 0x04001188 RID: 4488
	private Sprite m_iconSprite;
}
