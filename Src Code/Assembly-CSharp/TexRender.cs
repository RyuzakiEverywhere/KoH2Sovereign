using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000152 RID: 338
public static class TexRender
{
	// Token: 0x06001172 RID: 4466 RVA: 0x000B758C File Offset: 0x000B578C
	public static RenderTexture Begin(RenderTexture rt = null, int width = 0, int height = 0)
	{
		if (TexRender.rt != null)
		{
			Debug.LogError("TexRender.Begin(): reentrant call");
			return null;
		}
		if (width <= 0)
		{
			if (rt == null)
			{
				Debug.LogError("TexRender.Begin(): invalid arguments");
				return null;
			}
			width = rt.width;
		}
		if (height <= 0)
		{
			if (rt == null)
			{
				Debug.LogError("TexRender.Begin(): invalid arguments");
				return null;
			}
			height = rt.height;
		}
		if (rt == null || rt.width != width || rt.height != height)
		{
			if (rt != null)
			{
				rt.Release();
			}
			rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		}
		TexRender.rt = rt;
		TexRender.width = width;
		TexRender.height = height;
		Graphics.SetRenderTarget(rt);
		GL.PushMatrix();
		GL.LoadOrtho();
		return rt;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x000B7650 File Offset: 0x000B5850
	public static RenderTexture End()
	{
		RenderTexture renderTexture = TexRender.rt;
		if (renderTexture == null)
		{
			return null;
		}
		TexRender.rt = null;
		GL.PopMatrix();
		Graphics.SetRenderTarget(null);
		return renderTexture;
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x000B7680 File Offset: 0x000B5880
	public static void Clear(Color clr)
	{
		GL.Clear(true, true, clr);
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x000B768C File Offset: 0x000B588C
	public static void Draw(TexRender.Quad q)
	{
		if (q.mat != null)
		{
			q.mat.SetPass(q.mat_pass);
		}
		Vector2[] array = new Vector2[]
		{
			new Vector2(0f - q.pivot.x, 0f - q.pivot.y),
			new Vector2(0f - q.pivot.x, 1f - q.pivot.y),
			new Vector2(1f - q.pivot.x, 1f - q.pivot.y),
			new Vector2(1f - q.pivot.x, 0f - q.pivot.y)
		};
		float num = (float)((TexRender.width < TexRender.height) ? TexRender.width : TexRender.height);
		float num2 = (q.size.x < 0f) ? (-q.size.x) : q.size.x;
		float num3 = (q.size.y < 0f) ? (-q.size.y) : q.size.y;
		num2 *= q.scale.x;
		num3 *= q.scale.y;
		for (int i = 0; i < 4; i++)
		{
			Vector2[] array2 = array;
			int num4 = i;
			array2[num4].x = array2[num4].x * num2;
			Vector2[] array3 = array;
			int num5 = i;
			array3[num5].y = array3[num5].y * num3;
		}
		float f = q.rot * 0.017453292f;
		float num6 = Mathf.Sin(f);
		float num7 = Mathf.Cos(f);
		float num8 = num / (float)TexRender.width;
		float num9 = num / (float)TexRender.height;
		for (int j = 0; j < 4; j++)
		{
			float num10 = (array[j].x * num7 - array[j].y * num6) * num8;
			float num11 = (array[j].x * num6 + array[j].y * num7) * num9;
			array[j].x = num10 / num;
			array[j].y = num11 / num;
		}
		Vector2 vector = new Vector2(q.pos.x / (float)(TexRender.width - 1), q.pos.y / (float)(TexRender.height - 1));
		GL.Begin(7);
		GL.TexCoord2(q.uv.xMin, q.uv.yMin);
		GL.Vertex3(vector.x + array[0].x, vector.y + array[0].y, 0f);
		GL.TexCoord2(q.uv.xMin, q.uv.yMax);
		GL.Vertex3(vector.x + array[1].x, vector.y + array[1].y, 0f);
		GL.TexCoord2(q.uv.xMax, q.uv.yMax);
		GL.Vertex3(vector.x + array[2].x, vector.y + array[2].y, 0f);
		GL.TexCoord2(q.uv.xMax, q.uv.yMin);
		GL.Vertex3(vector.x + array[3].x, vector.y + array[3].y, 0f);
		GL.End();
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x000B7A58 File Offset: 0x000B5C58
	public static void Draw(TexRender.Lines l)
	{
		new Material(Shader.Find("Unlit/Color"))
		{
			color = l.clr
		}.SetPass(0);
		GL.Begin(1);
		for (int i = 0; i < l.points.Count; i++)
		{
			Vector2 vector = l.points[i];
			GL.Vertex3(vector.x / (float)(TexRender.width - 1), vector.y / (float)(TexRender.height - 1), 0f);
		}
		GL.End();
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x000B7AE0 File Offset: 0x000B5CE0
	public static void Draw(TexRender.Grid g)
	{
		if (g.tile_size.x <= 0f || g.tile_size.y <= 0f)
		{
			return;
		}
		new Material(Shader.Find("Unlit/Color"))
		{
			color = g.clr
		}.SetPass(0);
		float num = g.tile_size.x / (float)TexRender.width;
		for (float num2 = num; num2 < 1f; num2 += num)
		{
			GL.Begin(1);
			GL.Vertex3(num2, 0f, 0f);
			GL.Vertex3(num2, 1f, 0f);
			GL.End();
		}
		float num3 = g.tile_size.y / (float)TexRender.height;
		for (float num4 = num3; num4 < 1f; num4 += num3)
		{
			GL.Begin(1);
			GL.Vertex3(0f, num4, 0f);
			GL.Vertex3(1f, num4, 0f);
			GL.End();
		}
	}

	// Token: 0x04000B8E RID: 2958
	public static RenderTexture rt;

	// Token: 0x04000B8F RID: 2959
	public static int width;

	// Token: 0x04000B90 RID: 2960
	public static int height;

	// Token: 0x0200066D RID: 1645
	public class Quad
	{
		// Token: 0x060047BA RID: 18362 RVA: 0x002152A0 File Offset: 0x002134A0
		public Quad(Color clr)
		{
			this.mat = new Material(Shader.Find("Unlit/Color"));
			this.clr = clr;
		}

		// Token: 0x060047BB RID: 18363 RVA: 0x0021532C File Offset: 0x0021352C
		public Quad(Material mat)
		{
			this.mat = mat;
		}

		// Token: 0x060047BC RID: 18364 RVA: 0x002153A4 File Offset: 0x002135A4
		public Quad(Texture tex, Shader shader = null)
		{
			if (shader == null)
			{
				shader = Shader.Find("Sprites/Default");
			}
			this.size = new Vector2((float)tex.width, (float)tex.height);
			this.mat = new Material(shader);
			this.mat.mainTexture = tex;
		}

		// Token: 0x060047BD RID: 18365 RVA: 0x00215458 File Offset: 0x00213658
		public Quad(Texture tex, Material mat)
		{
			this.mat = mat;
			this.size = new Vector2((float)tex.width, (float)tex.height);
			this.mat.mainTexture = tex;
		}

		// Token: 0x060047BE RID: 18366 RVA: 0x002154F4 File Offset: 0x002136F4
		public Quad(Sprite sprite, Shader shader = null)
		{
			if (shader == null)
			{
				shader = Shader.Find("Sprites/Default");
			}
			Texture texture = sprite.texture;
			this.mat = new Material(shader);
			this.mat.mainTexture = texture;
			Rect rect = sprite.rect;
			this.size = rect.size;
			this.pivot = new Vector2(sprite.pivot.x / this.size.x, sprite.pivot.y / this.size.y);
			this.uv = new Rect((rect.xMin + 0.5f) / (float)texture.width, (rect.yMin + 0.5f) / (float)texture.height, (rect.width - 1f) / (float)texture.width, (rect.height - 1f) / (float)texture.height);
		}

		// Token: 0x04003586 RID: 13702
		public Material mat;

		// Token: 0x04003587 RID: 13703
		public int mat_pass;

		// Token: 0x04003588 RID: 13704
		public Color clr = Color.white;

		// Token: 0x04003589 RID: 13705
		public Vector2 pos = Vector2.zero;

		// Token: 0x0400358A RID: 13706
		public Vector2 size = -Vector2.one;

		// Token: 0x0400358B RID: 13707
		public Vector2 scale = Vector2.one;

		// Token: 0x0400358C RID: 13708
		public float rot;

		// Token: 0x0400358D RID: 13709
		public Vector2 pivot = Vector2.zero;

		// Token: 0x0400358E RID: 13710
		public Rect uv = new Rect(0f, 0f, 1f, 1f);
	}

	// Token: 0x0200066E RID: 1646
	public class Lines
	{
		// Token: 0x060047BF RID: 18367 RVA: 0x00215642 File Offset: 0x00213842
		public Lines(List<Vector2> points)
		{
			this.points = points;
		}

		// Token: 0x060047C0 RID: 18368 RVA: 0x0021565C File Offset: 0x0021385C
		public Lines(params Vector2[] points)
		{
			this.points = new List<Vector2>(points);
		}

		// Token: 0x0400358F RID: 13711
		public List<Vector2> points;

		// Token: 0x04003590 RID: 13712
		public Color clr = Color.white;
	}

	// Token: 0x0200066F RID: 1647
	public class Grid
	{
		// Token: 0x060047C1 RID: 18369 RVA: 0x0021567B File Offset: 0x0021387B
		public Grid(float tile_size)
		{
			this.tile_size = new Vector2(tile_size, tile_size);
		}

		// Token: 0x060047C2 RID: 18370 RVA: 0x002156A6 File Offset: 0x002138A6
		public Grid(Vector2 tile_size)
		{
			this.tile_size = tile_size;
		}

		// Token: 0x04003591 RID: 13713
		public Vector2 tile_size = Vector2.zero;

		// Token: 0x04003592 RID: 13714
		public Color clr = Color.white;
	}
}
