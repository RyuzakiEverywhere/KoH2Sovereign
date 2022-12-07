using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class SetupCutoutShaders : MonoBehaviour
{
	// Token: 0x0600115E RID: 4446 RVA: 0x000B723D File Offset: 0x000B543D
	[ContextMenu("Force fix cutout shaders")]
	private void ForceFixCutoutShaders()
	{
		this.is_fixed = false;
		this.FixCutoutShaders();
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x000B724C File Offset: 0x000B544C
	public void FixCutoutShaders()
	{
		if (this.is_fixed)
		{
			return;
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		Common.IterateThroughChildrenRecursive<MeshRenderer>(base.transform, new Action<MeshRenderer>(this.FixRenderer));
		Debug.Log(string.Format("Fixed cutout shaders in {0}ms", stopwatch.Elapsed.TotalMilliseconds));
		stopwatch.Stop();
		this.is_fixed = true;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x000B72B0 File Offset: 0x000B54B0
	private void FixRenderer(MeshRenderer renderer)
	{
		MeshFilter component = renderer.GetComponent<MeshFilter>();
		if (this.ShouldFix(renderer, component))
		{
			if (this.ShouldRenderCutout(component))
			{
				renderer.sharedMaterial = this.GetCutoutVariant(renderer.sharedMaterial);
				return;
			}
			renderer.sharedMaterial = this.GetNoCutoutVariant(renderer.sharedMaterial);
		}
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x000B72FC File Offset: 0x000B54FC
	private bool ShouldFix(MeshRenderer renderer, MeshFilter mesh_filter)
	{
		return !(mesh_filter == null) && !(mesh_filter.sharedMesh == null) && !(renderer.sharedMaterial == null);
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x000B732C File Offset: 0x000B552C
	private bool ShouldRenderCutout(MeshFilter mesh_filter)
	{
		string name = mesh_filter.sharedMesh.name;
		for (int i = 0; i < this.search_words.Length; i++)
		{
			if (name.Contains(this.search_words[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x000B736C File Offset: 0x000B556C
	private Material GetCutoutVariant(Material normal_material)
	{
		Material material;
		if (this.cutout_mappings.TryGetValue(normal_material, out material))
		{
			return material;
		}
		material = new Material(normal_material);
		material.EnableKeyword("ENABLE_CUTOUT");
		material.renderQueue = normal_material.renderQueue - 1;
		this.cutout_mappings.Add(normal_material, material);
		this.cutout_mappings.Add(material, material);
		return material;
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x000B73C8 File Offset: 0x000B55C8
	private Material GetNoCutoutVariant(Material cutout_material)
	{
		Material material;
		if (this.no_cutout_mappings.TryGetValue(cutout_material, out material))
		{
			return material;
		}
		material = new Material(cutout_material);
		material.DisableKeyword("ENABLE_CUTOUT");
		material.renderQueue = cutout_material.renderQueue + 1;
		this.no_cutout_mappings.Add(cutout_material, material);
		this.no_cutout_mappings.Add(material, material);
		return material;
	}

	// Token: 0x04000B85 RID: 2949
	private Dictionary<Material, Material> cutout_mappings = new Dictionary<Material, Material>();

	// Token: 0x04000B86 RID: 2950
	private Dictionary<Material, Material> no_cutout_mappings = new Dictionary<Material, Material>();

	// Token: 0x04000B87 RID: 2951
	private bool is_fixed;

	// Token: 0x04000B88 RID: 2952
	public string[] search_words = new string[]
	{
		"straw",
		"roof",
		"shed",
		"gate"
	};
}
