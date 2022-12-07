using System;
using UnityEngine;

// Token: 0x0200032B RID: 811
public class TestGL : MonoBehaviour
{
	// Token: 0x0600321F RID: 12831 RVA: 0x0019669C File Offset: 0x0019489C
	public void Render()
	{
		bool sRGBWrite = GL.sRGBWrite;
		this.renderTexture = TexRender.Begin(this.renderTexture, this.renderWidth, this.renderHeight);
		TexRender.Clear(this.backgroundColor);
		if (this.texture != null)
		{
			TexRender.Draw(new TexRender.Quad(this.texture, this.textureShader)
			{
				pos = this.texturePosition,
				scale = this.textureScale,
				rot = this.textureRotation,
				pivot = this.texturePivot,
				uv = this.textureUV
			});
		}
		if (this.sprite != null)
		{
			TexRender.Draw(new TexRender.Quad(this.sprite, this.spriteShader)
			{
				pos = this.spritePosition,
				scale = this.spriteScale,
				rot = this.spriteRotation,
				clr = this.spriteTint
			});
		}
		TexRender.Draw(new TexRender.Grid(this.gridTileSize)
		{
			clr = this.gridColor
		});
		TexRender.End();
		GL.sRGBWrite = sRGBWrite;
	}

	// Token: 0x04002195 RID: 8597
	public Color backgroundColor = Color.black;

	// Token: 0x04002196 RID: 8598
	public Shader textureShader;

	// Token: 0x04002197 RID: 8599
	public Texture texture;

	// Token: 0x04002198 RID: 8600
	public Vector2 texturePosition = Vector2.zero;

	// Token: 0x04002199 RID: 8601
	public Vector2 textureScale = Vector2.one;

	// Token: 0x0400219A RID: 8602
	public float textureRotation;

	// Token: 0x0400219B RID: 8603
	public Vector2 texturePivot = Vector2.zero;

	// Token: 0x0400219C RID: 8604
	public Rect textureUV = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x0400219D RID: 8605
	public Shader spriteShader;

	// Token: 0x0400219E RID: 8606
	public Sprite sprite;

	// Token: 0x0400219F RID: 8607
	public Color spriteTint = Color.white;

	// Token: 0x040021A0 RID: 8608
	public Vector2 spritePosition = Vector2.zero;

	// Token: 0x040021A1 RID: 8609
	public Vector2 spriteScale = Vector2.one;

	// Token: 0x040021A2 RID: 8610
	public float spriteRotation;

	// Token: 0x040021A3 RID: 8611
	public Vector2 gridTileSize = new Vector2(32f, 32f);

	// Token: 0x040021A4 RID: 8612
	public Color gridColor = Color.gray;

	// Token: 0x040021A5 RID: 8613
	public int renderWidth = 256;

	// Token: 0x040021A6 RID: 8614
	public int renderHeight = 256;

	// Token: 0x040021A7 RID: 8615
	public RenderTexture renderTexture;
}
