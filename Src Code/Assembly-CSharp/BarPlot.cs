using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000200 RID: 512
public class BarPlot
{
	// Token: 0x06001F3E RID: 7998 RVA: 0x001212FC File Offset: 0x0011F4FC
	public BarPlot(Sampler s, GameObject p)
	{
		if (p == null)
		{
			return;
		}
		this.sampler = s;
		this.graphRenderer = Common.FindChildByName(p, "Graph", true, true).GetComponent<CanvasRenderer>();
		this.maxFpsText = Common.FindChildByName(p, "GraphMax", true, true).GetComponent<Text>();
		this.graphMesh = new Mesh();
		this.graphRect = this.graphRenderer.GetComponent<RectTransform>();
		this.newVertices = new Vector3[this.sampler.samples.Length * 2];
		this.newUV = new Vector2[this.sampler.samples.Length * 2];
		this.newTriangles = new int[(this.sampler.samples.Length - 1) * 6];
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x001213C8 File Offset: 0x0011F5C8
	public void DrawGraph(float deltaTime)
	{
		if (this.graphRenderer == null)
		{
			return;
		}
		float width = this.graphRect.rect.width;
		float height = this.graphRect.rect.height;
		float num = -width / 2f;
		float num2 = -height / 2f;
		float num3 = width / (float)(this.sampler.samples.Length - 1);
		this.graphMesh.Clear();
		if (this.sampler.max > 0f)
		{
			float num4 = height / this.sampler.max;
			this.maxMult = Mathf.Lerp(this.maxMult, num4, deltaTime * ((this.maxMult < num4) ? 0.5f : 4f));
		}
		for (int i = 0; i < this.sampler.samples.Length; i++)
		{
			float x = (float)i * num3 + num;
			int num5 = i * 2;
			int num6 = i * 6;
			if (i < this.sampler.samples.Length - 1)
			{
				this.newTriangles[num6] = num5;
				this.newTriangles[num6 + 1] = num5 + 1;
				this.newTriangles[num6 + 2] = num5 + 2;
				this.newTriangles[num6 + 3] = num5 + 1;
				this.newTriangles[num6 + 4] = num5 + 2;
				this.newTriangles[num6 + 5] = num5 + 3;
			}
			this.newVertices[num5] = new Vector3(x, num2 + Mathf.Clamp(this.sampler.samples[i] * this.maxMult, 0f, height), 0f);
			this.newVertices[num5 + 1] = new Vector3(x, num2, 0f);
			this.newUV[num5] = new Vector2((float)i * num3 / width, this.sampler.samples[i] * this.maxMult / height);
			this.newUV[num5 + 1] = new Vector2((float)i * num3 / width, 0f);
		}
		this.graphMesh.vertices = this.newVertices;
		this.graphMesh.uv = this.newUV;
		this.graphMesh.triangles = this.newTriangles;
		this.graphRenderer.SetMesh(this.graphMesh);
		this.maxFpsText.text = this.sampler.ToString();
	}

	// Token: 0x040014B5 RID: 5301
	private Sampler sampler;

	// Token: 0x040014B6 RID: 5302
	private CanvasRenderer graphRenderer;

	// Token: 0x040014B7 RID: 5303
	private RectTransform graphRect;

	// Token: 0x040014B8 RID: 5304
	private Text maxFpsText;

	// Token: 0x040014B9 RID: 5305
	private Mesh graphMesh;

	// Token: 0x040014BA RID: 5306
	private Vector3[] newVertices;

	// Token: 0x040014BB RID: 5307
	private Vector2[] newUV;

	// Token: 0x040014BC RID: 5308
	private int[] newTriangles;

	// Token: 0x040014BD RID: 5309
	private float maxMult = 1f;
}
