using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200005C RID: 92
[ExecuteInEditMode]
[RequireComponent(typeof(Terrain))]
[DisallowMultipleComponent]
public class MicroSplatTerrain : MicroSplatObject
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000220 RID: 544 RVA: 0x00020324 File Offset: 0x0001E524
	// (remove) Token: 0x06000221 RID: 545 RVA: 0x00020358 File Offset: 0x0001E558
	public static event MicroSplatTerrain.MaterialSyncAll OnMaterialSyncAll;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000222 RID: 546 RVA: 0x0002038C File Offset: 0x0001E58C
	// (remove) Token: 0x06000223 RID: 547 RVA: 0x000203C4 File Offset: 0x0001E5C4
	public event MicroSplatTerrain.MaterialSync OnMaterialSync;

	// Token: 0x06000224 RID: 548 RVA: 0x000203F9 File Offset: 0x0001E5F9
	private void Awake()
	{
		this.terrain = base.GetComponent<Terrain>();
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00020407 File Offset: 0x0001E607
	private void OnEnable()
	{
		this.terrain = base.GetComponent<Terrain>();
		MicroSplatTerrain.sInstances.Add(this);
		if (this.reenabled)
		{
			this.Sync();
		}
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0002042E File Offset: 0x0001E62E
	private void Start()
	{
		this.Sync();
	}

	// Token: 0x06000227 RID: 551 RVA: 0x00020436 File Offset: 0x0001E636
	private void OnDisable()
	{
		MicroSplatTerrain.sInstances.Remove(this);
		this.Cleanup();
		this.reenabled = true;
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00020451 File Offset: 0x0001E651
	private void Cleanup()
	{
		if (this.matInstance != null && this.matInstance != this.templateMaterial)
		{
			Object.DestroyImmediate(this.matInstance);
			this.terrain.materialTemplate = null;
		}
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0002048C File Offset: 0x0001E68C
	public void Sync()
	{
		if (this.templateMaterial == null)
		{
			return;
		}
		Material material;
		if (this.terrain.materialTemplate == this.matInstance && this.matInstance != null)
		{
			this.terrain.materialTemplate.CopyPropertiesFromMaterial(this.templateMaterial);
			material = this.terrain.materialTemplate;
		}
		else
		{
			material = new Material(this.templateMaterial);
		}
		if (this.terrain.drawInstanced && this.keywordSO.IsKeywordEnabled("_TESSDISTANCE") && this.keywordSO.IsKeywordEnabled("_MSRENDERLOOP_SURFACESHADER"))
		{
			Debug.LogWarning("Disabling terrain instancing when tessellation is enabled, as Unity has not made surface shader tessellation compatible with terrain instancing");
			this.terrain.drawInstanced = false;
		}
		material.hideFlags = HideFlags.HideAndDontSave;
		this.terrain.materialTemplate = material;
		this.matInstance = material;
		base.ApplyMaps(material);
		if (this.keywordSO.IsKeywordEnabled("_CUSTOMSPLATTEXTURES"))
		{
			material.SetTexture("_CustomControl0", (this.customControl0 != null) ? this.customControl0 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl1", (this.customControl1 != null) ? this.customControl1 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl2", (this.customControl2 != null) ? this.customControl2 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl3", (this.customControl3 != null) ? this.customControl3 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl4", (this.customControl4 != null) ? this.customControl4 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl5", (this.customControl5 != null) ? this.customControl5 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl6", (this.customControl6 != null) ? this.customControl6 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl7", (this.customControl7 != null) ? this.customControl7 : Texture2D.blackTexture);
		}
		else
		{
			Texture2D[] alphamapTextures = this.terrain.terrainData.alphamapTextures;
			base.ApplyControlTextures(alphamapTextures, material);
		}
		if (this.OnMaterialSync != null)
		{
			this.OnMaterialSync(material);
		}
		base.ApplyBlendMap();
	}

	// Token: 0x0600022A RID: 554 RVA: 0x000206E3 File Offset: 0x0001E8E3
	public override Bounds GetBounds()
	{
		return this.terrain.terrainData.bounds;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x000206F8 File Offset: 0x0001E8F8
	public static void SyncAll()
	{
		for (int i = 0; i < MicroSplatTerrain.sInstances.Count; i++)
		{
			MicroSplatTerrain.sInstances[i].Sync();
		}
		if (MicroSplatTerrain.OnMaterialSyncAll != null)
		{
			MicroSplatTerrain.OnMaterialSyncAll();
		}
	}

	// Token: 0x0400033A RID: 826
	[HideInInspector]
	public Shader addPass;

	// Token: 0x0400033D RID: 829
	private static List<MicroSplatTerrain> sInstances = new List<MicroSplatTerrain>();

	// Token: 0x0400033E RID: 830
	public Terrain terrain;

	// Token: 0x0400033F RID: 831
	[HideInInspector]
	public bool reenabled;

	// Token: 0x0200050F RID: 1295
	// (Invoke) Token: 0x060042A1 RID: 17057
	public delegate void MaterialSyncAll();

	// Token: 0x02000510 RID: 1296
	// (Invoke) Token: 0x060042A5 RID: 17061
	public delegate void MaterialSync(Material m);
}
