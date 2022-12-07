using System;
using Logic;
using UnityEngine;

// Token: 0x020000A2 RID: 162
[ExecuteInEditMode]
public class TerrainHeightsRenderer : MonoBehaviour
{
	// Token: 0x060005AA RID: 1450 RVA: 0x0003E32C File Offset: 0x0003C52C
	public static TerrainHeightsRenderer Get()
	{
		return TerrainHeightsRenderer.instance;
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x0003E334 File Offset: 0x0003C534
	public float GetLowResHeight(Point pos)
	{
		if (this.low_res_heights == null)
		{
			return -1f;
		}
		float num = pos.x;
		float num2 = pos.y;
		num /= this.low_res_cell_size;
		num2 /= this.low_res_cell_size;
		int num3 = (int)Math.Floor((double)num);
		int num4 = (int)Math.Floor((double)num2);
		if (!TerrainHeightsRenderer.InBounds(num3, num4, this.low_res_width, this.low_res_width))
		{
			return -1f;
		}
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		float num7 = this.low_res_heights[num3, num4];
		if (!TerrainHeightsRenderer.InBounds(num3 + 1, num4, this.low_res_width, this.low_res_width))
		{
			return num7;
		}
		float num8 = this.low_res_heights[num3 + 1, num4];
		if (!TerrainHeightsRenderer.InBounds(num3, num4 + 1, this.low_res_width, this.low_res_width))
		{
			return num7;
		}
		float num9 = this.low_res_heights[num3, num4 + 1];
		if (!TerrainHeightsRenderer.InBounds(num3 + 1, num4 + 1, this.low_res_width, this.low_res_width))
		{
			return num7;
		}
		float num10 = this.low_res_heights[num3 + 1, num4 + 1];
		float num11 = num7 + (num8 - num7) * num5;
		float num12 = num9 + (num10 - num9) * num5;
		return num11 + (num12 - num11) * num6;
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x0003E462 File Offset: 0x0003C662
	public static bool InBounds(int x, int y, int grid_width, int grid_height)
	{
		return x > 0 && x < grid_width && y > 0 && y < grid_height;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0003E476 File Offset: 0x0003C676
	private void Init()
	{
		TerrainHeightsRenderer.instance = this;
		this.Render(this.useUnityTexture, false);
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x0003E48C File Offset: 0x0003C68C
	private void SetupTerrain()
	{
		this.terrain = base.GetComponent<Terrain>();
		if (this.terrain == null)
		{
			this.terrain = Terrain.activeTerrain;
			if (this.terrain == null)
			{
				return;
			}
		}
		Shader.SetGlobalVector("_TerrainSize", this.terrain.terrainData.size);
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x0003E4EC File Offset: 0x0003C6EC
	private void SetupCam()
	{
		this.cam = global::Common.CreateTerrainCam(this.terrain);
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x0003E500 File Offset: 0x0003C700
	private void SetupRT()
	{
		this.resolution_x = (int)this.terrain.terrainData.size.x;
		this.resolution_y = (int)this.terrain.terrainData.size.z;
		if (this.rt == null)
		{
			this.rt = new RenderTexture(this.resolution_x, this.resolution_y, 0, RenderTextureFormat.ARGBFloat);
			this.rt.isPowerOfTwo = false;
		}
		this.cam.targetTexture = this.rt;
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x0003E58A File Offset: 0x0003C78A
	private void Cleanup()
	{
		if (this.cam != null)
		{
			this.cam.targetTexture = null;
		}
		if (this.rt != null)
		{
			UnityEngine.Object.DestroyImmediate(this.rt);
			this.rt = null;
		}
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x0003E5C8 File Offset: 0x0003C7C8
	private void CalcLowResHeights(Texture tex)
	{
		Texture2D texture2D = tex as Texture2D;
		if (texture2D == null)
		{
			RenderTexture renderTexture = tex as RenderTexture;
			if (renderTexture == null)
			{
				return;
			}
			RenderTexture.active = renderTexture;
			texture2D = new Texture2D(tex.width, tex.height);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)tex.width, (float)tex.height), 0, 0, false);
			RenderTexture.active = null;
			texture2D.Apply();
		}
		int width = tex.width;
		int height = tex.height;
		int num = (int)((float)width / this.low_res_cell_size);
		int num2 = (int)((float)height / this.low_res_cell_size);
		this.low_res_heights = new float[num, num2];
		this.low_res_width = num;
		Color[] pixels = texture2D.GetPixels();
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				float num3 = 0f;
				for (int k = -1; k <= 1; k++)
				{
					for (int l = -1; l <= 1; l++)
					{
						num3 += this.GetHeight(i + k, j + l, pixels, width, height, this.low_res_cell_size);
					}
				}
				this.low_res_heights[i, j] = num3 / 9f;
			}
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x0003E708 File Offset: 0x0003C908
	private float GetHeight(int x, int y, Color[] pixels, int width, int height, float low_res_cell_size)
	{
		int num = (int)((float)x * low_res_cell_size);
		if (num < 0)
		{
			num = 0;
		}
		if (num > width - 1)
		{
			num = width - 1;
		}
		int num2 = (int)((float)y * low_res_cell_size);
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num > height - 1)
		{
			num = height - 1;
		}
		int num3 = num + num2 * height;
		if (num3 < 0 || num3 >= pixels.Length)
		{
			return 0f;
		}
		return pixels[num3].r * 500f;
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x0003E770 File Offset: 0x0003C970
	public void UseUnityHeightmap()
	{
		this.SetupTerrain();
		if (this.terrain == null)
		{
			return;
		}
		Shader.SetGlobalTexture("_TerrainHeights", this.terrain.terrainData.heightmapTexture);
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x0003E7A4 File Offset: 0x0003C9A4
	public void Render(bool use_unity_texture, bool force = false)
	{
		if (Application.isPlaying && !force && !use_unity_texture)
		{
			Debug.LogError("Attempting to render Terrain Heights in game!");
			return;
		}
		this.SetupTerrain();
		if (this.terrain == null)
		{
			return;
		}
		if (use_unity_texture)
		{
			Shader.SetGlobalTexture("_TerrainHeights", this.terrain.terrainData.heightmapTexture);
			this.CalcLowResHeights(this.terrain.terrainData.heightmapTexture);
			return;
		}
		this.SetupCam();
		if (this.cam == null)
		{
			return;
		}
		this.SetupRT();
		bool activeSelf = this.terrain.gameObject.activeSelf;
		bool enabled = this.terrain.enabled;
		bool drawHeightmap = this.terrain.drawHeightmap;
		bool drawTreesAndFoliage = this.terrain.drawTreesAndFoliage;
		bool drawInstanced = this.terrain.drawInstanced;
		float heightmapPixelError = this.terrain.heightmapPixelError;
		this.terrain.gameObject.SetActive(true);
		this.terrain.enabled = true;
		this.terrain.drawHeightmap = true;
		this.terrain.drawTreesAndFoliage = false;
		this.terrain.drawInstanced = false;
		this.terrain.heightmapPixelError = 1f;
		this.cam.RenderWithShader(Shader.Find("Hidden/TerrainHeights"), "");
		this.terrain.gameObject.SetActive(activeSelf);
		this.terrain.enabled = enabled;
		this.terrain.drawHeightmap = drawHeightmap;
		this.terrain.drawTreesAndFoliage = drawTreesAndFoliage;
		this.terrain.drawInstanced = drawInstanced;
		this.terrain.heightmapPixelError = heightmapPixelError;
		Texture2D texture2D = this.tex;
		if (texture2D == null || texture2D.width != this.resolution_x || texture2D.height != this.resolution_y || texture2D.format != TextureFormat.RGBA32)
		{
			texture2D = new Texture2D(this.cam.targetTexture.width, this.cam.targetTexture.height, TextureFormat.RGBA32, false, true);
		}
		RenderTexture.active = this.rt;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)this.resolution_x, (float)this.resolution_y), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		this.tex = texture2D;
		this.tex.Apply();
		Shader.SetGlobalTexture("_TerrainHeights", this.tex);
		this.Cleanup();
		this.CalcLowResHeights(this.tex);
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x0003EA0D File Offset: 0x0003CC0D
	private void OnTerrainChanged(TerrainChangedFlags flags)
	{
		if ((flags & (TerrainChangedFlags.Heightmap | TerrainChangedFlags.FlushEverythingImmediately)) != (TerrainChangedFlags)0)
		{
			this.Render(true, false);
		}
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x0003EA1D File Offset: 0x0003CC1D
	public void OnHeighstModified(BSGTerrainEdit editor, Terrain terrain, Bounds bounds)
	{
		this.Render(true, false);
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x0003EA27 File Offset: 0x0003CC27
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x0003EA2F File Offset: 0x0003CC2F
	private void OnDisable()
	{
		if (TerrainHeightsRenderer.instance == this)
		{
			TerrainHeightsRenderer.instance = null;
		}
		if (this.cam != null)
		{
			UnityEngine.Object.DestroyImmediate(this.cam.gameObject);
			this.cam = null;
		}
	}

	// Token: 0x04000536 RID: 1334
	private int resolution_x = 2048;

	// Token: 0x04000537 RID: 1335
	private int resolution_y = 2048;

	// Token: 0x04000538 RID: 1336
	public Texture2D tex;

	// Token: 0x04000539 RID: 1337
	public bool useUnityTexture = true;

	// Token: 0x0400053A RID: 1338
	private Terrain terrain;

	// Token: 0x0400053B RID: 1339
	private Camera cam;

	// Token: 0x0400053C RID: 1340
	private RenderTexture rt;

	// Token: 0x0400053D RID: 1341
	private float[,] low_res_heights;

	// Token: 0x0400053E RID: 1342
	private int low_res_width;

	// Token: 0x0400053F RID: 1343
	public float low_res_cell_size = 8f;

	// Token: 0x04000540 RID: 1344
	private static TerrainHeightsRenderer instance;
}
