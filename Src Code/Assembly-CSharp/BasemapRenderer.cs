using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

// Token: 0x02000075 RID: 117
public class BasemapRenderer
{
	// Token: 0x06000473 RID: 1139 RVA: 0x00034B7C File Offset: 0x00032D7C
	public static RenderTexture CreateBasemap(TerrainData terrainData, int resolution)
	{
		RenderTexture renderTexture = new RenderTexture(resolution, resolution, 0, DefaultFormat.HDR);
		renderTexture.Create();
		BasemapRenderer.RenderBasemap(terrainData, renderTexture);
		return renderTexture;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00034BA4 File Offset: 0x00032DA4
	public static void RenderBasemap(TerrainData terrainData, RenderTexture basemapTarget)
	{
		BasemapRenderer.<>c__DisplayClass4_0 CS$<>8__locals1;
		CS$<>8__locals1.terrainData = terrainData;
		CommandBuffer commandBuffer = new CommandBuffer();
		commandBuffer.name = "Basemap Renderer";
		if (BasemapRenderer.rendererMaterial == null)
		{
			BasemapRenderer.rendererMaterial = new Material(Shader.Find("Hidden/BasemapRenderer"));
		}
		commandBuffer.SetRenderTarget(basemapTarget);
		commandBuffer.ClearRenderTarget(false, true, new Color(0f, 0f, 0f, 0f));
		commandBuffer.SetGlobalFloat("_BasemapRenderer_TargetResolution", (float)basemapTarget.width);
		commandBuffer.SetGlobalTexture("_BasemapRenderer_Heightmap", CS$<>8__locals1.terrainData.heightmapTexture);
		for (int i = 0; i < CS$<>8__locals1.terrainData.alphamapTextures.Length; i++)
		{
			Texture2D texture2D = CS$<>8__locals1.terrainData.alphamapTextures[i];
			Texture2D tex = BasemapRenderer.<RenderBasemap>g__GetTerrainDiffuseTexture|4_0(i * 4, ref CS$<>8__locals1);
			Texture2D tex2 = BasemapRenderer.<RenderBasemap>g__GetTerrainDiffuseTexture|4_0(i * 4 + 1, ref CS$<>8__locals1);
			Texture2D tex3 = BasemapRenderer.<RenderBasemap>g__GetTerrainDiffuseTexture|4_0(i * 4 + 2, ref CS$<>8__locals1);
			Texture2D tex4 = BasemapRenderer.<RenderBasemap>g__GetTerrainDiffuseTexture|4_0(i * 4 + 3, ref CS$<>8__locals1);
			if (texture2D == null || texture2D == Texture2D.whiteTexture)
			{
				texture2D = Texture2D.blackTexture;
			}
			commandBuffer.SetGlobalTexture("_BasemapRenderer_Alphamap", texture2D);
			commandBuffer.SetGlobalTexture("_BasemapRenderer_TextureR", tex);
			commandBuffer.SetGlobalTexture("_BasemapRenderer_TextureG", tex2);
			commandBuffer.SetGlobalTexture("_BasemapRenderer_TextureB", tex3);
			commandBuffer.SetGlobalTexture("_BasemapRenderer_TextureA", tex4);
			if (i == 0)
			{
				commandBuffer.EnableShaderKeyword("FIRST_PASS");
			}
			else
			{
				commandBuffer.DisableShaderKeyword("FIRST_PASS");
			}
			commandBuffer.Blit(basemapTarget, basemapTarget, BasemapRenderer.rendererMaterial);
		}
		commandBuffer.DisableShaderKeyword("FIRST_PASS");
		commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
		Graphics.ExecuteCommandBuffer(commandBuffer);
		Shader.SetGlobalTexture("_BasemapRenderer_LastRendered", basemapTarget);
		Vector4 value = CS$<>8__locals1.terrainData.bounds.min;
		Vector4 value2 = CS$<>8__locals1.terrainData.bounds.max;
		Shader.SetGlobalVector("_TerrainBounds_Min", value);
		Shader.SetGlobalVector("_TerrainBounds_Max", value2);
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x00034DCA File Offset: 0x00032FCA
	[CompilerGenerated]
	internal static Texture2D <RenderBasemap>g__GetTerrainDiffuseTexture|4_0(int i, ref BasemapRenderer.<>c__DisplayClass4_0 A_1)
	{
		return A_1.terrainData.terrainLayers[Mathf.Min(i, A_1.terrainData.terrainLayers.Length - 1)].diffuseTexture;
	}

	// Token: 0x04000472 RID: 1138
	private const string BASEMAP_RENDERER_SHADER_NAME = "Hidden/BasemapRenderer";

	// Token: 0x04000473 RID: 1139
	private static Material rendererMaterial;

	// Token: 0x04000474 RID: 1140
	private const string FIRST_PASS_KEYWORD = "FIRST_PASS";
}
