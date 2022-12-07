using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000073 RID: 115
[CreateAssetMenu(menuName = "BSG/BSGGrassData", fileName = "BSGGrassData")]
public class BSGGrassData : ScriptableObject
{
	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000462 RID: 1122 RVA: 0x0003448B File Offset: 0x0003268B
	public static BSGGrassData Instance
	{
		get
		{
			if (!(BSGGrassData.instance != null))
			{
				return BSGGrassData.instance = Resources.Load<BSGGrassData>("BSGGrassData");
			}
			return BSGGrassData.instance;
		}
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x000344B0 File Offset: 0x000326B0
	public static IEnumerable<BSGGrassPreset> GetAllPresets()
	{
		return BSGGrassData.Instance.grassPresets;
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x000344BC File Offset: 0x000326BC
	public static BSGGrassPreset GetPreset(int index)
	{
		return BSGGrassData.Instance.grassPresets.FirstOrDefault((BSGGrassPreset preset) => preset.index == index);
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x000344F1 File Offset: 0x000326F1
	public static BSGGrassPreset GetPreset(DetailPrototype detailPrototype)
	{
		if (!BSGGrassData.IsDetailBSGGrassPreset(detailPrototype))
		{
			return null;
		}
		return BSGGrassData.GetPreset(BSGGrassData.PresetColorToIndex(detailPrototype.dryColor));
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00034510 File Offset: 0x00032710
	public static bool IsDetailBSGGrassPreset(DetailPrototype detail)
	{
		return detail.prototype == null && detail.dryColor.r == detail.healthyColor.r && BSGGrassData.IsValidIndex(detail.dryColor) && detail.renderMode == DetailRenderMode.GrassBillboard && !detail.usePrototypeMesh;
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00034564 File Offset: 0x00032764
	public static Color PresetIndexToColor(int index)
	{
		int num = index / 16;
		return new Color((float)(index % 16) / 16f, (float)num / 16f, 0f, 1f);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00034598 File Offset: 0x00032798
	public static int PresetColorToIndex(Color color)
	{
		int num = Mathf.RoundToInt(color.g * 16f);
		int num2 = Mathf.RoundToInt(color.r * 16f);
		return num * 16 + num2;
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x000345D0 File Offset: 0x000327D0
	public static void SetShaderProperties()
	{
		BSGGrassData.Instance.SyncUniformArray();
		Shader.SetGlobalVectorArray("_BSGGrassPresets_FirstColors", (from p in BSGGrassData.Instance.uniformGrassPresets
		select p.firstColor).ToArray<Vector4>());
		Shader.SetGlobalVectorArray("_BSGGrassPresets_SecondColors", (from p in BSGGrassData.Instance.uniformGrassPresets
		select p.secondColor).ToArray<Vector4>());
		Shader.SetGlobalVectorArray("_BSGGrassPresets_Params1", (from p in BSGGrassData.Instance.uniformGrassPresets
		select new Vector4(p.minPitch, p.maxPitch, p.heightOffset, p.basemapBlend)).ToArray<Vector4>());
		Shader.SetGlobalVectorArray("_BSGGrassPresets_Params2", (from p in BSGGrassData.Instance.uniformGrassPresets
		select new Vector4(p.randomizeColor, p.wind, p.closeCutout, p.noiseSpread)).ToArray<Vector4>());
		Shader.SetGlobalVectorArray("_BSGGrassPresets_Params3", (from p in BSGGrassData.Instance.uniformGrassPresets
		select new Vector4(p.distanceCutout, p.startFadeDistance, p.endFadeDistance, p.translucency)).ToArray<Vector4>());
		Shader.SetGlobalVectorArray("_BSGGrassPresets_Params4", (from p in BSGGrassData.Instance.uniformGrassPresets
		select new Vector4(p.fakeAO, 0f, 0f, 0f)).ToArray<Vector4>());
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00034758 File Offset: 0x00032958
	private void OnValidate()
	{
		foreach (BSGGrassPreset bsggrassPreset in this.grassPresets)
		{
			bsggrassPreset.Validate();
		}
		BSGGrassData.SetShaderProperties();
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x000347B0 File Offset: 0x000329B0
	private void SyncUniformArray()
	{
		this.uniformGrassPresets.Clear();
		int i;
		int j;
		for (i = 0; i < 256; i = j + 1)
		{
			BSGGrassPreset bsggrassPreset = this.grassPresets.FirstOrDefault((BSGGrassPreset p) => p.index == i);
			if (bsggrassPreset != null)
			{
				this.uniformGrassPresets.Add(bsggrassPreset);
			}
			else
			{
				this.uniformGrassPresets.Add(BSGGrassPreset.Default);
			}
			j = i;
		}
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00034834 File Offset: 0x00032A34
	private static bool IsValidIndex(int index)
	{
		return BSGGrassData.GetAllPresets().Any((BSGGrassPreset p) => p.index == index);
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00034864 File Offset: 0x00032A64
	private static bool IsValidIndex(Color index)
	{
		return BSGGrassData.IsValidIndex(BSGGrassData.PresetColorToIndex(index));
	}

	// Token: 0x04000458 RID: 1112
	public const int MAX_GRASS_PRESETS_COUNT = 256;

	// Token: 0x04000459 RID: 1113
	public List<BSGGrassPreset> grassPresets = new List<BSGGrassPreset>();

	// Token: 0x0400045A RID: 1114
	private List<BSGGrassPreset> uniformGrassPresets = new List<BSGGrassPreset>();

	// Token: 0x0400045B RID: 1115
	private static BSGGrassData instance;
}
