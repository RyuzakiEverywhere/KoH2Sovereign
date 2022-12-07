using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001AB RID: 427
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
public class BSGButtonImage : MonoBehaviour
{
	// Token: 0x060018BA RID: 6330 RVA: 0x000F274E File Offset: 0x000F094E
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x000F2756 File Offset: 0x000F0956
	private void Init()
	{
		if (this.initialzied)
		{
			return;
		}
		this.txt = base.GetComponent<TextMeshProUGUI>();
		this.img = base.GetComponent<Image>();
		this.initialzied = true;
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x000F2780 File Offset: 0x000F0980
	private void Offset(float x, float y)
	{
		Vector3 position = base.transform.position;
		position.x += x;
		position.y -= y;
		base.transform.position = position;
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x000F27C0 File Offset: 0x000F09C0
	public void SetState(BSGButton.State state)
	{
		this.Init();
		Sprite sprite = this.normalImage;
		Color color = this.normalColor;
		Material material = this.normalTextMaterialPreset;
		if (state == BSGButton.State.Rollover)
		{
			sprite = this.rolloverImage;
			color = this.rolloverColor;
			material = this.rolloverTextMaterialPreset;
		}
		else if (state == BSGButton.State.Pressed)
		{
			sprite = this.pressedImage;
			color = this.pressedColor;
			material = this.pressedTextMaterialPreset;
		}
		else if (state == BSGButton.State.Disabled)
		{
			sprite = this.disabledImage;
			color = this.disabledColor;
			material = this.disbaledTextMaterialPreset;
		}
		else if (state == BSGButton.State.Selected)
		{
			sprite = this.selectedImage;
			color = this.selectedColor;
			material = this.selectedTextMaterialPreset;
		}
		if (sprite == null)
		{
			sprite = this.normalImage;
		}
		if (this.img == null)
		{
			this.img = base.GetComponent<Image>();
		}
		if (sprite != null && this.img == null)
		{
			this.img = base.gameObject.AddComponent<Image>();
			this.img.hideFlags = HideFlags.HideAndDontSave;
		}
		if (this.img != null)
		{
			this.img.overrideSprite = sprite;
			this.img.color = color;
		}
		if (this.normalImage == null && this.txt != null)
		{
			this.txt.color = color;
			if (material != null)
			{
				this.txt.fontSharedMaterial = material;
			}
		}
		if (state == BSGButton.State.Pressed && this.state != BSGButton.State.Pressed)
		{
			this.Offset(this.ofsX, this.ofsY);
		}
		else if (state != BSGButton.State.Pressed && this.state == BSGButton.State.Pressed)
		{
			this.Offset(-this.ofsX, -this.ofsY);
		}
		this.state = state;
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x000F295D File Offset: 0x000F0B5D
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.SetState(BSGButton.State.Normal);
		}
	}

	// Token: 0x04000FF3 RID: 4083
	public Sprite normalImage;

	// Token: 0x04000FF4 RID: 4084
	public Sprite rolloverImage;

	// Token: 0x04000FF5 RID: 4085
	public Sprite pressedImage;

	// Token: 0x04000FF6 RID: 4086
	public Sprite disabledImage;

	// Token: 0x04000FF7 RID: 4087
	public Sprite selectedImage;

	// Token: 0x04000FF8 RID: 4088
	public Color normalColor = Color.white;

	// Token: 0x04000FF9 RID: 4089
	public Color rolloverColor = Color.white;

	// Token: 0x04000FFA RID: 4090
	public Color pressedColor = Color.white;

	// Token: 0x04000FFB RID: 4091
	public Color disabledColor = Color.white;

	// Token: 0x04000FFC RID: 4092
	public Color selectedColor = Color.white;

	// Token: 0x04000FFD RID: 4093
	public Material normalTextMaterialPreset;

	// Token: 0x04000FFE RID: 4094
	public Material rolloverTextMaterialPreset;

	// Token: 0x04000FFF RID: 4095
	public Material pressedTextMaterialPreset;

	// Token: 0x04001000 RID: 4096
	public Material disbaledTextMaterialPreset;

	// Token: 0x04001001 RID: 4097
	public Material selectedTextMaterialPreset;

	// Token: 0x04001002 RID: 4098
	public float ofsX;

	// Token: 0x04001003 RID: 4099
	public float ofsY;

	// Token: 0x04001004 RID: 4100
	private BSGButton.State state;

	// Token: 0x04001005 RID: 4101
	private Image img;

	// Token: 0x04001006 RID: 4102
	private TextMeshProUGUI txt;

	// Token: 0x04001007 RID: 4103
	private bool initialzied;
}
