using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000206 RID: 518
[RequireComponent(typeof(Hotspot))]
public class HotspotImage : MonoBehaviour
{
	// Token: 0x06001F8C RID: 8076 RVA: 0x001235B6 File Offset: 0x001217B6
	private void OnEnable()
	{
		if (this.image == null)
		{
			this.image = base.GetComponent<Image>();
		}
		this.hotspot = base.GetComponent<Hotspot>();
		this.SetState(BSGButton.State.Normal);
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x001235E8 File Offset: 0x001217E8
	private void LateUpdate()
	{
		BSGButton.State state = BSGButton.State.Normal;
		if (this.hotspot != null)
		{
			if (this.hotspot.mouse_in)
			{
				state = BSGButton.State.Rollover;
			}
			if (this.hotspot.mouse_down)
			{
				state = BSGButton.State.Pressed;
			}
		}
		if (this.state != state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06001F8E RID: 8078 RVA: 0x00123634 File Offset: 0x00121834
	public void SetState(BSGButton.State state)
	{
		Sprite sprite = this.normalImage;
		Color color = this.normalColor;
		if (state == BSGButton.State.Rollover)
		{
			sprite = this.rolloverImage;
			color = this.rolloverColor;
		}
		else if (state == BSGButton.State.Pressed)
		{
			sprite = this.pressedImage;
			color = this.pressedColor;
		}
		if (sprite == null)
		{
			sprite = this.normalImage;
		}
		if (this.image == null)
		{
			this.image = base.GetComponent<Image>();
		}
		if (sprite != null && this.image == null)
		{
			this.image = base.gameObject.AddComponent<Image>();
			this.image.hideFlags = HideFlags.HideAndDontSave;
		}
		if (this.image != null)
		{
			this.image.overrideSprite = sprite;
			this.image.color = color;
		}
		this.state = state;
	}

	// Token: 0x040014E5 RID: 5349
	public Sprite normalImage;

	// Token: 0x040014E6 RID: 5350
	public Sprite rolloverImage;

	// Token: 0x040014E7 RID: 5351
	public Sprite pressedImage;

	// Token: 0x040014E8 RID: 5352
	public Color normalColor = Color.white;

	// Token: 0x040014E9 RID: 5353
	public Color rolloverColor = Color.white;

	// Token: 0x040014EA RID: 5354
	public Color pressedColor = Color.white;

	// Token: 0x040014EB RID: 5355
	[SerializeField]
	private Image image;

	// Token: 0x040014EC RID: 5356
	private BSGButton.State state;

	// Token: 0x040014ED RID: 5357
	private Hotspot hotspot;
}
