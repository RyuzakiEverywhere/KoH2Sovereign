using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000117 RID: 279
public static class SilhuetteUtility
{
	// Token: 0x06000CB3 RID: 3251 RVA: 0x0008D9D4 File Offset: 0x0008BBD4
	public static Material GetNoSilhuetteMaterial(Material material)
	{
		if (material.renderQueue > 2459)
		{
			return material;
		}
		Material result;
		if (SilhuetteUtility.no_silhuette_material_map.TryGetValue(material, out result))
		{
			return result;
		}
		Material material2 = new Material(material);
		material2.renderQueue = 2460;
		SilhuetteUtility.no_silhuette_material_map.Add(material, material2);
		SilhuetteUtility.no_silhuette_material_map.Add(material2, material2);
		return material2;
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0008DA2C File Offset: 0x0008BC2C
	public static void Cleanup()
	{
		foreach (Material obj in SilhuetteUtility.no_silhuette_material_map.Values)
		{
			Common.DestroyObj(obj);
		}
		SilhuetteUtility.no_silhuette_material_map.Clear();
	}

	// Token: 0x040009E3 RID: 2531
	private static Dictionary<Material, Material> no_silhuette_material_map = new Dictionary<Material, Material>();
}
