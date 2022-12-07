using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Logic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000119 RID: 281
public class TextureBaker
{
	// Token: 0x06000CCA RID: 3274 RVA: 0x0008DE14 File Offset: 0x0008C014
	public static bool Visible(float3 pos, float4[] f_planes)
	{
		bool flag = true;
		for (int i = 0; i < 6; i++)
		{
			if (!f_planes[i].Equals(float4.zero))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			return false;
		}
		for (int j = 0; j < 6; j++)
		{
			if (math.dot(pos, f_planes[j].xyz) + f_planes[j].w + 20f < 0f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x0008DE88 File Offset: 0x0008C088
	public TextureBaker()
	{
		TextureBaker.InstancedSkinningDrawerBatched.Init();
		TextureBaker.InstancedArrowDrawerBatched.Init();
		TextureBaker.InstancedTrailDrawerBatched.Init();
		TextureBaker.InstancedSelectionDrawerBatched.Init();
		TextureBaker.InstancedDustDrawerBatched.Init();
		TextureBaker.InstancedDecalDrawerBatched.Init();
		this.SetStanceColors();
		this.troop_selection_arrows_drawer = new TextureBaker.InstancedSelectionDrawerBatched(this.stance_colors, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.TroopSelectionArrow);
		this.troop_selection_circles_drawer = new TextureBaker.InstancedSelectionDrawerBatched(this.stance_colors, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.TroopSelectionCircle);
		this.path_arrows_straight_drawer = new TextureBaker.InstancedSelectionDrawerBatched(this.stance_colors, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow);
		this.path_arrows_shooting_range_straight_drawer = new TextureBaker.InstancedSelectionDrawerBatched(this.stance_colors, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrowShootRange);
		this.dust_drawer = new TextureBaker.InstancedDustDrawerBatched();
		this.trail_drawer = new TextureBaker.InstancedTrailDrawerBatched();
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x0008DF34 File Offset: 0x0008C134
	public void Dispose()
	{
		if (this.kingdom_colors != null)
		{
			this.kingdom_colors.Dispose();
		}
		foreach (KeyValuePair<string, List<TextureBaker.PerModelData>> keyValuePair in this.skinning_drawers)
		{
			List<TextureBaker.PerModelData> value = keyValuePair.Value;
			for (int i = 0; i < value.Count; i++)
			{
				value[i].Dispose();
			}
		}
		this.ClearSkinningData();
		if (this.stance_colors != null)
		{
			this.stance_colors.Dispose();
		}
		if (this.stance_colors_outline != null)
		{
			this.stance_colors_outline.Dispose();
		}
		if (this.troop_selection_arrows_drawer != null)
		{
			this.troop_selection_arrows_drawer.Dispose();
		}
		if (this.troop_selection_circles_drawer != null)
		{
			this.troop_selection_circles_drawer.Dispose();
		}
		if (this.path_arrows_straight_drawer != null)
		{
			this.path_arrows_straight_drawer.Dispose();
		}
		if (this.path_arrows_shooting_range_straight_drawer != null)
		{
			this.path_arrows_shooting_range_straight_drawer.Dispose();
		}
		if (this.dust_drawer != null)
		{
			this.dust_drawer.Dispose();
		}
		if (this.decal_drawer != null)
		{
			this.decal_drawer.Dispose();
		}
		foreach (KeyValuePair<string, TextureBaker.PerArrowModelData> keyValuePair2 in this.arrow_drawers)
		{
			keyValuePair2.Value.Dispose();
		}
		this.trail_drawer.Dispose();
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0008E0AC File Offset: 0x0008C2AC
	public void Draw(Camera cam)
	{
		if (cam == null || !cam.gameObject.activeInHierarchy || !cam.enabled)
		{
			return;
		}
		Profile.BeginSection("TextureBaker.Draw");
		this.troop_selection_arrows_drawer.Draw(cam);
		this.troop_selection_circles_drawer.Draw(cam);
		this.path_arrows_straight_drawer.Draw(cam);
		this.path_arrows_shooting_range_straight_drawer.Draw(cam);
		this.dust_drawer.Draw(cam);
		TextureBaker.InstancedDecalDrawerBatched instancedDecalDrawerBatched = this.decal_drawer;
		if (instancedDecalDrawerBatched != null)
		{
			instancedDecalDrawerBatched.Draw(cam);
		}
		foreach (KeyValuePair<string, List<TextureBaker.PerModelData>> keyValuePair in this.skinning_drawers)
		{
			List<TextureBaker.PerModelData> value = keyValuePair.Value;
			for (int i = 0; i < value.Count; i++)
			{
				value[i].Draw(cam, this.stance_colors);
			}
		}
		foreach (KeyValuePair<string, TextureBaker.PerArrowModelData> keyValuePair2 in this.arrow_drawers)
		{
			keyValuePair2.Value.Draw(cam);
		}
		this.trail_drawer.Draw(cam);
		Profile.EndSection("TextureBaker.Draw");
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0008E1FC File Offset: 0x0008C3FC
	public void ClearSkinningBuffers()
	{
		foreach (KeyValuePair<string, List<TextureBaker.PerModelData>> keyValuePair in this.skinning_drawers)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				keyValuePair.Value[i].model_data_buffer.Clear();
			}
		}
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0008E278 File Offset: 0x0008C478
	public void ClearSkinningData()
	{
		this.skinning_drawers.Clear();
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x0008E288 File Offset: 0x0008C488
	public TextureBaker.PerArrowModelData AddArrowDrawer(DT.Field def)
	{
		TextureBaker.PerArrowModelData perArrowModelData = new TextureBaker.PerArrowModelData();
		perArrowModelData.drawer = new TextureBaker.InstancedArrowDrawerBatched(def);
		perArrowModelData.model_data_buffer = new GrowBuffer<TextureBaker.InstancedArrowDrawerBatched.DrawCallData>(Allocator.Persistent, 64);
		perArrowModelData.model_compute_buffer = new ComputeBuffer(10000, TextureBaker.InstancedArrowDrawerBatched.DrawCallDataSize);
		this.arrow_drawers[def.key] = perArrowModelData;
		return perArrowModelData;
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x0008E2E0 File Offset: 0x0008C4E0
	public void ReloadMaterials()
	{
		this.troop_selection_arrows_drawer.Reload(this.stance_colors, this.troop_selection_arrows_drawer.circle_mesh);
		this.troop_selection_circles_drawer.Reload(this.stance_colors, this.troop_selection_circles_drawer.circle_mesh);
		this.path_arrows_straight_drawer.Reload(this.stance_colors, this.path_arrows_straight_drawer.circle_mesh);
		this.path_arrows_shooting_range_straight_drawer.Reload(this.stance_colors, this.path_arrows_shooting_range_straight_drawer.circle_mesh);
		this.dust_drawer.Reload();
		TextureBaker.InstancedDecalDrawerBatched instancedDecalDrawerBatched = this.decal_drawer;
		if (instancedDecalDrawerBatched != null)
		{
			instancedDecalDrawerBatched.Reload();
		}
		foreach (KeyValuePair<string, List<TextureBaker.PerModelData>> keyValuePair in this.skinning_drawers)
		{
			List<TextureBaker.PerModelData> value = keyValuePair.Value;
			for (int i = 0; i < value.Count; i++)
			{
				TextureBaker.PerModelData perModelData = value[i];
				for (int j = 0; j < perModelData.drawers.Count; j++)
				{
					perModelData.drawers[j].Reload(this.kingdom_colors);
				}
			}
		}
		this.trail_drawer.Reload();
		foreach (KeyValuePair<string, TextureBaker.PerArrowModelData> keyValuePair2 in this.arrow_drawers)
		{
			keyValuePair2.Value.drawer.Reload();
		}
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x0008E468 File Offset: 0x0008C668
	public void SetKingdomColors(Color[] colors)
	{
		this.kingdom_colors = new ComputeBuffer(colors.Length, 16);
		this.kingdom_colors.SetData(colors);
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0008E488 File Offset: 0x0008C688
	public void SetStanceColors()
	{
		this.colors = new Color[8];
		DT.Field field = global::Defs.Get(false).dt.Find("Unit", null);
		DT.Field field2 = field.FindChild("selection_stance_colors", null, true, true, true, '.');
		for (int i = 0; i < field2.children.Count; i++)
		{
			DT.Field field3 = field2.children[i];
			TextureBaker.StanceColors stanceColors;
			if (!(field3.type != "color") && Enum.TryParse<TextureBaker.StanceColors>(field3.key, out stanceColors))
			{
				Color color = global::Defs.GetColor(field3, Color.green, null);
				this.colors[(int)stanceColors] = color;
			}
		}
		if (this.stance_colors == null || this.stance_colors.count != this.colors.Length)
		{
			if (this.stance_colors != null)
			{
				this.stance_colors.Dispose();
			}
			this.stance_colors = new ComputeBuffer(this.colors.Length, 16);
		}
		this.stance_colors.SetData(this.colors);
		field2 = field.FindChild("outline_stance_colors", null, true, true, true, '.');
		for (int j = 0; j < field2.children.Count; j++)
		{
			DT.Field field4 = field2.children[j];
			TextureBaker.StanceColors stanceColors2;
			if (!(field4.type != "color") && Enum.TryParse<TextureBaker.StanceColors>(field4.key, out stanceColors2))
			{
				Color color2 = global::Defs.GetColor(field4, Color.green, null);
				this.colors[(int)stanceColors2] = color2;
			}
		}
		if (this.stance_colors_outline == null || this.stance_colors_outline.count != this.colors.Length)
		{
			if (this.stance_colors_outline != null)
			{
				this.stance_colors_outline.Dispose();
			}
			this.stance_colors_outline = new ComputeBuffer(this.colors.Length, 16);
		}
		this.stance_colors_outline.SetData(this.colors);
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0008E654 File Offset: 0x0008C854
	public static TextureBaker.UnitTypeData BakeUnitType(KeyframeTextureBaker.BakedData[] data, global::Squad sq = null)
	{
		TextureBaker.UnitTypeData unitTypeData = new TextureBaker.UnitTypeData(sq);
		Color col = Color.white;
		if (sq != null)
		{
			Logic.Kingdom kingdom = sq.logic.GetKingdom();
			if (kingdom != null)
			{
				global::Kingdom kingdom2 = kingdom.visuals as global::Kingdom;
				BattleMap battleMap = BattleMap.Get();
				Color color;
				if (battleMap.kingdom_colors != null && battleMap.kingdom_colors.TryGetValue(sq.kingdom, out color))
				{
					col = color;
				}
				else if (kingdom2 != null)
				{
					col = kingdom2.PrimaryArmyColor;
				}
				else
				{
					col = global::Defs.GetColor(kingdom.def, "primary_color", null);
				}
			}
		}
		for (int i = 0; i < data.Length; i++)
		{
			TextureBaker.InstancedSkinningDrawer item = new TextureBaker.InstancedSkinningDrawer(sq, data[i].NewMesh, data[i], col);
			unitTypeData.Drawers.Add(item);
		}
		return unitTypeData;
	}

	// Token: 0x040009EA RID: 2538
	private const int PreallocatedBufferSize = 100;

	// Token: 0x040009EB RID: 2539
	public static bool disable_troops_highlight;

	// Token: 0x040009EC RID: 2540
	public ComputeBuffer kingdom_colors;

	// Token: 0x040009ED RID: 2541
	public Dictionary<string, List<TextureBaker.PerModelData>> skinning_drawers = new Dictionary<string, List<TextureBaker.PerModelData>>();

	// Token: 0x040009EE RID: 2542
	public Dictionary<string, TextureBaker.PerArrowModelData> arrow_drawers = new Dictionary<string, TextureBaker.PerArrowModelData>();

	// Token: 0x040009EF RID: 2543
	public TextureBaker.InstancedTrailDrawerBatched trail_drawer;

	// Token: 0x040009F0 RID: 2544
	public Color[] colors;

	// Token: 0x040009F1 RID: 2545
	public ComputeBuffer stance_colors;

	// Token: 0x040009F2 RID: 2546
	public ComputeBuffer stance_colors_outline;

	// Token: 0x040009F3 RID: 2547
	public TextureBaker.InstancedSelectionDrawerBatched troop_selection_arrows_drawer;

	// Token: 0x040009F4 RID: 2548
	public TextureBaker.InstancedSelectionDrawerBatched troop_selection_circles_drawer;

	// Token: 0x040009F5 RID: 2549
	public TextureBaker.InstancedSelectionDrawerBatched path_arrows_straight_drawer;

	// Token: 0x040009F6 RID: 2550
	public TextureBaker.InstancedSelectionDrawerBatched path_arrows_shooting_range_straight_drawer;

	// Token: 0x040009F7 RID: 2551
	public TextureBaker.InstancedDustDrawerBatched dust_drawer;

	// Token: 0x040009F8 RID: 2552
	public TextureBaker.InstancedDecalDrawerBatched decal_drawer;

	// Token: 0x0200061A RID: 1562
	public enum StanceColors
	{
		// Token: 0x040033D8 RID: 13272
		Own,
		// Token: 0x040033D9 RID: 13273
		Enemy,
		// Token: 0x040033DA RID: 13274
		Ally,
		// Token: 0x040033DB RID: 13275
		OwnHighlighted,
		// Token: 0x040033DC RID: 13276
		EnemyHighlighted,
		// Token: 0x040033DD RID: 13277
		AllyHighlighted,
		// Token: 0x040033DE RID: 13278
		Preview,
		// Token: 0x040033DF RID: 13279
		RunningAway,
		// Token: 0x040033E0 RID: 13280
		COUNT
	}

	// Token: 0x0200061B RID: 1563
	public class UnitTypeData
	{
		// Token: 0x060046C7 RID: 18119 RVA: 0x0020FD8C File Offset: 0x0020DF8C
		public unsafe static Bounds CalcBounds(global::Squad squad)
		{
			Bounds result = default(Bounds);
			if (((squad != null) ? squad.logic : null) == null)
			{
				result.center = Vector3.zero;
				result.size = new Vector3(40f, 40f, 40f);
			}
			else if (squad.logic.simulation.state == BattleSimulation.Squad.State.Dead)
			{
				result.center = squad.transform.position;
				result.size = new Vector3(40f, 40f, 40f);
			}
			else
			{
				float2 @float = squad.data->BoundingBoxCenter;
				float2 float2 = squad.data->BoundingBoxSize;
				if (squad.subsquads_data_ids.Count > 0)
				{
					@float = squad.logic.game.world_size / 2f;
					float2 = squad.logic.game.world_size;
				}
				result.center = global::Common.SnapToTerrain(new Vector3(@float.x, 0f, @float.y), 0f, null, -1f, false);
				result.size = new Vector3(float2.x, 200f, float2.y);
			}
			return result;
		}

		// Token: 0x060046C8 RID: 18120 RVA: 0x0020FEC8 File Offset: 0x0020E0C8
		public UnitTypeData(global::Squad sq = null)
		{
			int num = (sq == null) ? 100 : sq.logic.InitialTroops();
			if (num == 0)
			{
				return;
			}
			this.positionsBuffer = new ComputeBuffer(num, 16);
			this.rotationsBuffer = new ComputeBuffer(num, 16);
			this.textureCoordinatesBuffer = new ComputeBuffer(num, 12);
			this.previewPositions = new NativeArray<float4>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.squadPosition = new NativeArray<float4>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.arrowRangeBuffer = new ComputeBuffer(1, 16);
			this.previewPositionsBuffer = new ComputeBuffer(num, 16);
			this.dustBuffer = new ComputeBuffer(num * 5, 16);
			if (BaseUI.Get() == null)
			{
				return;
			}
			Mesh mesh = MeshUtils.CreateCircle(((sq == null) ? 0.5f : sq.def.selection_radius) + 0.125f, 0.125f, 32);
			mesh.name = "InstancedSelectionMesh(Old)";
			this.selDrawer = new TextureBaker.InstancedSelectionDrawer(mesh, sq, null);
			Material material = new Material(Shader.Find("BSG/Instanced/SquadSelectionInstancedOverlay"));
			material.SetColor("_Color", new Color(1f, 1f, 0f, 1f));
			this.selPreviewDrawer = new TextureBaker.InstancedSelectionDrawer(mesh, sq, material);
			if (sq != null && sq.def.is_ranged && sq.arrow_range_material != null)
			{
				Mesh mesh2 = MeshUtils.CreateCircle(sq.logic.simulation.unit.GetVar("max_shoot_range", null, true), 1f, 64);
				Mesh mesh3 = MeshUtils.CreateCircle(sq.salvo_def.min_shoot_range, -1f, 64);
				CombineInstance[] array = new CombineInstance[2];
				array[0].mesh = mesh2;
				array[0].transform = Matrix4x4.identity;
				array[1].mesh = mesh3;
				array[1].transform = Matrix4x4.identity;
				Mesh mesh4 = new Mesh();
				mesh4.CombineMeshes(array);
				mesh4.name = "InstancedArrowRange(Old)";
				material = new Material(sq.arrow_range_material);
				this.selArrowRangeDrawer = new TextureBaker.InstancedSelectionDrawer(mesh4, sq, new Material(material));
				global::Common.DestroyObj(mesh3);
				global::Common.DestroyObj(mesh2);
			}
		}

		// Token: 0x060046C9 RID: 18121 RVA: 0x00210114 File Offset: 0x0020E314
		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			if (this.positionsBuffer != null)
			{
				this.positionsBuffer.Dispose();
			}
			if (this.rotationsBuffer != null)
			{
				this.rotationsBuffer.Dispose();
			}
			if (this.previewPositionsBuffer != null)
			{
				this.previewPositionsBuffer.Dispose();
			}
			if (this.dustBuffer != null)
			{
				this.dustBuffer.Dispose();
			}
			if (this.previewPositions.IsCreated)
			{
				this.previewPositions.Dispose();
			}
			if (this.selPreviewDrawer != null)
			{
				this.selPreviewDrawer.Dispose();
			}
			if (this.textureCoordinatesBuffer != null)
			{
				this.textureCoordinatesBuffer.Dispose();
			}
			if (this.selDrawer != null)
			{
				this.selDrawer.Dispose();
			}
			if (this.dustDrawer != null)
			{
				this.dustDrawer.Dispose();
			}
			NativeArray<float4> nativeArray = this.squadPosition;
			if (this.squadPosition.IsCreated)
			{
				this.squadPosition.Dispose();
			}
			if (this.selArrowRangeDrawer != null)
			{
				this.selArrowRangeDrawer.Dispose();
			}
			if (this.arrowRangeBuffer != null)
			{
				this.arrowRangeBuffer.Dispose();
			}
			foreach (TextureBaker.InstancedSkinningDrawer instancedSkinningDrawer in this.Drawers)
			{
				if (instancedSkinningDrawer != null)
				{
					instancedSkinningDrawer.Dispose();
				}
			}
			this.disposed = true;
		}

		// Token: 0x040033E1 RID: 13281
		public ComputeBuffer arrowRangeBuffer;

		// Token: 0x040033E2 RID: 13282
		public ComputeBuffer positionsBuffer;

		// Token: 0x040033E3 RID: 13283
		public ComputeBuffer rotationsBuffer;

		// Token: 0x040033E4 RID: 13284
		public ComputeBuffer previewPositionsBuffer;

		// Token: 0x040033E5 RID: 13285
		public ComputeBuffer dustBuffer;

		// Token: 0x040033E6 RID: 13286
		public ComputeBuffer textureCoordinatesBuffer;

		// Token: 0x040033E7 RID: 13287
		public List<TextureBaker.InstancedSkinningDrawer> Drawers = new List<TextureBaker.InstancedSkinningDrawer>();

		// Token: 0x040033E8 RID: 13288
		public NativeArray<float4> previewPositions;

		// Token: 0x040033E9 RID: 13289
		public NativeArray<float4> squadPosition;

		// Token: 0x040033EA RID: 13290
		public TextureBaker.InstancedSelectionDrawer selDrawer;

		// Token: 0x040033EB RID: 13291
		public TextureBaker.InstancedSelectionDrawer selPreviewDrawer;

		// Token: 0x040033EC RID: 13292
		public TextureBaker.InstancedSelectionDrawer selArrowRangeDrawer;

		// Token: 0x040033ED RID: 13293
		public TextureBaker.InstancedDustDrawerBatched dustDrawer;

		// Token: 0x040033EE RID: 13294
		public GameObject ragdoll_prefab;

		// Token: 0x040033EF RID: 13295
		public int[] death_decals;

		// Token: 0x040033F0 RID: 13296
		public int baked_data_id;

		// Token: 0x040033F1 RID: 13297
		public int id;

		// Token: 0x040033F2 RID: 13298
		public bool disposed;

		// Token: 0x040033F3 RID: 13299
		public int Count;
	}

	// Token: 0x0200061C RID: 1564
	public class InstancedArrowDrawer : IDisposable
	{
		// Token: 0x060046CA RID: 18122 RVA: 0x00210270 File Offset: 0x0020E470
		public static void ClearMeshes()
		{
			TextureBaker.InstancedArrowDrawer.arrow_meshes.Clear();
		}

		// Token: 0x060046CB RID: 18123 RVA: 0x0021027C File Offset: 0x0020E47C
		public static TextureBaker.InstancedArrowDrawer.ArrowData LoadAssets(DT.Field def)
		{
			if (!TextureBaker.InstancedArrowDrawer.arrow_meshes.ContainsKey(def) && def != null)
			{
				GameObject gameObject = def.GetValue("arrow_model", null, true, true, true, '.').Get<GameObject>();
				Material material = new Material(gameObject.GetComponent<MeshRenderer>().sharedMaterial);
				material.shader = Shader.Find("BSG/Instanced/ArrowsInstanced");
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				TextureBaker.InstancedArrowDrawer.arrow_meshes[def] = new TextureBaker.InstancedArrowDrawer.ArrowData
				{
					mesh = component.sharedMesh,
					mat = material
				};
				if (TextureBaker.InstancedArrowDrawer.trail_mesh == null)
				{
					List<Vector3> list = new List<Vector3>();
					int num = 16;
					for (float num2 = 0f; num2 < (float)num; num2 += 1f)
					{
						list.Add(new Vector3(0f, 0f, num2 / (float)num));
					}
					TextureBaker.InstancedArrowDrawer.trail_mesh = MeshUtils.CreateLinesMesh(list, 0.05f, 2, true, false, null, null);
					TextureBaker.InstancedArrowDrawer.trail_mesh.name = "InstancedTrailMesh(Old)";
				}
			}
			TextureBaker.InstancedArrowDrawer.ArrowData result;
			TextureBaker.InstancedArrowDrawer.arrow_meshes.TryGetValue(def, out result);
			return result;
		}

		// Token: 0x060046CC RID: 18124 RVA: 0x0021038C File Offset: 0x0020E58C
		public void ReloadData(DT.Field def)
		{
			if (this.def == def)
			{
				return;
			}
			this.def = def;
			TextureBaker.InstancedArrowDrawer.ArrowData arrowData = TextureBaker.InstancedArrowDrawer.LoadAssets(def);
			this.mesh = arrowData.mesh;
			if (this.mesh == null)
			{
				Debug.LogWarning("Warning, no arrow model for " + def);
			}
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.argsBuffer_trail != null)
			{
				this.argsBuffer_trail.Dispose();
			}
			this.argsBuffer = new ComputeBuffer(1, this.indirectArgs.Length * 4, ComputeBufferType.DrawIndirect);
			this.argsBuffer_trail = new ComputeBuffer(1, this.indirectArgs_trail.Length * 4, ComputeBufferType.DrawIndirect);
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			this.indirectArgs_trail[0] = TextureBaker.InstancedArrowDrawer.trail_mesh.GetIndexCount(0);
			this.indirectArgs_trail[1] = 0U;
			this.argsBuffer_trail.SetData(this.indirectArgs_trail);
			this.material = new Material(arrowData.mat);
			if (this.positionsBuffer != null)
			{
				this.material.SetBuffer("positions", this.positionsBuffer);
				this.material.SetBuffer("rotations", this.rotationsBuffer);
			}
			Texture texture = def.GetValue("arrow_trail_texture", null, true, true, true, '.').Get<Texture>();
			if (texture != null)
			{
				this.trail_material.SetTexture("_MainTex", texture);
				this.draw_trail = true;
				return;
			}
			this.draw_trail = false;
		}

		// Token: 0x060046CD RID: 18125 RVA: 0x0021051C File Offset: 0x0020E71C
		public InstancedArrowDrawer(DT.Field def)
		{
			this.progressBuffer = new ComputeBuffer(100, 4);
			this.startBuffer = new ComputeBuffer(100, 12);
			this.midBuffer = new ComputeBuffer(100, 12);
			this.endBuffer = new ComputeBuffer(100, 12);
			this.trail_material = new Material(Shader.Find("BSG/Instanced/TrailInstanced"));
			this.trail_material.SetBuffer("progress", this.progressBuffer);
			this.trail_material.SetBuffer("start", this.startBuffer);
			this.trail_material.SetBuffer("mid", this.midBuffer);
			this.trail_material.SetBuffer("end", this.endBuffer);
			this.trail_material.SetBuffer("positions", this.positionsBuffer);
			this.trail_material.SetBuffer("rotations", this.rotationsBuffer);
			this.positionsBuffer = new ComputeBuffer(100, 16);
			this.rotationsBuffer = new ComputeBuffer(100, 16);
			this.ReloadData(def);
		}

		// Token: 0x060046CE RID: 18126 RVA: 0x00210648 File Offset: 0x0020E848
		public void Dispose()
		{
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.argsBuffer_trail != null)
			{
				this.argsBuffer_trail.Dispose();
			}
			if (this.progressBuffer != null)
			{
				this.progressBuffer.Dispose();
			}
			if (this.startBuffer != null)
			{
				this.startBuffer.Dispose();
			}
			if (this.midBuffer != null)
			{
				this.midBuffer.Dispose();
			}
			if (this.endBuffer != null)
			{
				this.endBuffer.Dispose();
			}
			if (this.positionsBuffer != null)
			{
				this.positionsBuffer.Dispose();
			}
			if (this.rotationsBuffer != null)
			{
				this.rotationsBuffer.Dispose();
			}
			if (TextureBaker.InstancedArrowDrawer.trail_mesh != null)
			{
				global::Common.DestroyObj(TextureBaker.InstancedArrowDrawer.trail_mesh);
				TextureBaker.InstancedArrowDrawer.trail_mesh = null;
			}
			if (this.trail_material != null)
			{
				global::Common.DestroyObj(this.trail_material);
			}
			if (this.material != null)
			{
				global::Common.DestroyObj(this.material);
			}
			TextureBaker.InstancedArrowDrawer.arrow_meshes.Remove(this.def);
		}

		// Token: 0x060046CF RID: 18127 RVA: 0x00210750 File Offset: 0x0020E950
		public void Draw(uint count = 100U)
		{
			if (this.mesh == null)
			{
				return;
			}
			this.indirectArgs[1] = count;
			this.argsBuffer.SetData(this.indirectArgs);
			this.indirectArgs_trail[1] = count;
			this.argsBuffer_trail.SetData(this.indirectArgs_trail);
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, new Bounds(Vector3.zero, 1000000f * Vector3.one), this.argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.On, true);
			if (this.draw_trail)
			{
				Graphics.DrawMeshInstancedIndirect(TextureBaker.InstancedArrowDrawer.trail_mesh, 0, this.trail_material, new Bounds(Vector3.zero, 1000000f * Vector3.one), this.argsBuffer_trail, 0, new MaterialPropertyBlock(), ShadowCastingMode.Off, false);
			}
		}

		// Token: 0x040033F4 RID: 13300
		private DT.Field def;

		// Token: 0x040033F5 RID: 13301
		private ComputeBuffer argsBuffer;

		// Token: 0x040033F6 RID: 13302
		private ComputeBuffer argsBuffer_trail;

		// Token: 0x040033F7 RID: 13303
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x040033F8 RID: 13304
		private readonly uint[] indirectArgs_trail = new uint[5];

		// Token: 0x040033F9 RID: 13305
		private Material material;

		// Token: 0x040033FA RID: 13306
		private Material trail_material;

		// Token: 0x040033FB RID: 13307
		private Mesh mesh;

		// Token: 0x040033FC RID: 13308
		private static Mesh trail_mesh;

		// Token: 0x040033FD RID: 13309
		public ComputeBuffer progressBuffer;

		// Token: 0x040033FE RID: 13310
		public ComputeBuffer startBuffer;

		// Token: 0x040033FF RID: 13311
		public ComputeBuffer midBuffer;

		// Token: 0x04003400 RID: 13312
		public ComputeBuffer endBuffer;

		// Token: 0x04003401 RID: 13313
		public ComputeBuffer positionsBuffer;

		// Token: 0x04003402 RID: 13314
		public ComputeBuffer rotationsBuffer;

		// Token: 0x04003403 RID: 13315
		private bool draw_trail = true;

		// Token: 0x04003404 RID: 13316
		private static Dictionary<DT.Field, TextureBaker.InstancedArrowDrawer.ArrowData> arrow_meshes = new Dictionary<DT.Field, TextureBaker.InstancedArrowDrawer.ArrowData>();

		// Token: 0x020009EC RID: 2540
		public struct ArrowData
		{
			// Token: 0x040045E0 RID: 17888
			public Mesh mesh;

			// Token: 0x040045E1 RID: 17889
			public Material mat;
		}
	}

	// Token: 0x0200061D RID: 1565
	public class InstancedSelectionDrawer : IDisposable
	{
		// Token: 0x060046D1 RID: 18129 RVA: 0x00210828 File Offset: 0x0020EA28
		public InstancedSelectionDrawer(Mesh meshToDraw, global::Squad sq, Material mat = null)
		{
			this.mesh = meshToDraw;
			this.squad = sq;
			this.argsBuffer = new ComputeBuffer(1, this.indirectArgs.Length * 4, ComputeBufferType.DrawIndirect);
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			this.material = mat;
		}

		// Token: 0x060046D2 RID: 18130 RVA: 0x002108A8 File Offset: 0x0020EAA8
		public void Dispose()
		{
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.mesh != null)
			{
				global::Common.DestroyObj(this.mesh);
			}
			if (this.material != null)
			{
				global::Common.DestroyObj(this.material);
			}
		}

		// Token: 0x060046D3 RID: 18131 RVA: 0x002108FC File Offset: 0x0020EAFC
		public void Draw(ComputeBuffer pos, ComputeBuffer rot, bool no_bounds = false)
		{
			int count = pos.count;
			if (count == 0)
			{
				return;
			}
			if (this.mesh == null)
			{
				return;
			}
			if (this.material == null)
			{
				BaseUI baseUI = BaseUI.Get();
				Color value = Color.green;
				Material material;
				if (baseUI.selectionSettings != null)
				{
					value = ((this.squad == null) ? Color.green : baseUI.GetStanceColor(this.squad.logic, true));
					material = new Material(baseUI.selectionSettings.armySelectionMaterial);
					material.shader = Shader.Find("BSG/Instanced/SquadSelectionInstanced");
				}
				else
				{
					material = new Material(Shader.Find("BSG/Instanced/SquadSelectionInstanced"));
				}
				material.SetColor("_Color", value);
				this.material = material;
			}
			this.material.SetBuffer("positions", pos);
			this.material.SetBuffer("rotations", rot);
			this.indirectArgs[1] = (uint)count;
			this.argsBuffer.SetData(this.indirectArgs);
			Bounds bounds;
			if (no_bounds)
			{
				bounds = new Bounds(Vector3.zero, new Vector3(10000f, 200f, 10000f));
			}
			else
			{
				bounds = TextureBaker.UnitTypeData.CalcBounds(this.squad);
			}
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, bounds, this.argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.Off, false);
		}

		// Token: 0x04003405 RID: 13317
		private ComputeBuffer argsBuffer;

		// Token: 0x04003406 RID: 13318
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x04003407 RID: 13319
		private Material material;

		// Token: 0x04003408 RID: 13320
		private Mesh mesh;

		// Token: 0x04003409 RID: 13321
		private global::Squad squad;
	}

	// Token: 0x0200061E RID: 1566
	public class PerModelData
	{
		// Token: 0x060046D4 RID: 18132 RVA: 0x00210A4C File Offset: 0x0020EC4C
		public void Draw(Camera cam, ComputeBuffer stance_colors_buffer)
		{
			GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData> growBuffer = this.model_data_buffer;
			if (growBuffer.Count == 0)
			{
				return;
			}
			if (this.model_compute_buffer == null)
			{
				return;
			}
			this.model_compute_buffer.SetData(growBuffer.ToArray());
			for (int i = 0; i < this.drawers.Count; i++)
			{
				this.drawers[i].Draw(this.model_compute_buffer, stance_colors_buffer, (uint)growBuffer.Count, this.draw_layer, cam);
			}
		}

		// Token: 0x060046D5 RID: 18133 RVA: 0x00210AC4 File Offset: 0x0020ECC4
		public void Dispose()
		{
			if (this.model_compute_buffer != null)
			{
				this.model_compute_buffer.Dispose();
			}
			for (int i = 0; i < this.drawers.Count; i++)
			{
				this.drawers[i].Dispose();
			}
		}

		// Token: 0x0400340A RID: 13322
		public int draw_layer;

		// Token: 0x0400340B RID: 13323
		public int baked_data_id;

		// Token: 0x0400340C RID: 13324
		public ComputeBuffer model_compute_buffer;

		// Token: 0x0400340D RID: 13325
		public GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData> model_data_buffer;

		// Token: 0x0400340E RID: 13326
		public List<TextureBaker.InstancedSkinningDrawerBatched> drawers = new List<TextureBaker.InstancedSkinningDrawerBatched>();

		// Token: 0x0400340F RID: 13327
		public GameObject ragdoll_prefab;

		// Token: 0x04003410 RID: 13328
		public Dictionary<string, int> key_to_idx;
	}

	// Token: 0x0200061F RID: 1567
	public class PerArrowModelData
	{
		// Token: 0x060046D7 RID: 18135 RVA: 0x00210B20 File Offset: 0x0020ED20
		public void Draw(Camera cam)
		{
			if (this.model_data_buffer.Count == 0)
			{
				return;
			}
			if (this.model_compute_buffer == null)
			{
				return;
			}
			this.model_compute_buffer.SetData(this.model_data_buffer.ToArray());
			this.drawer.Draw(this.model_compute_buffer, (uint)this.model_data_buffer.Count, cam);
		}

		// Token: 0x060046D8 RID: 18136 RVA: 0x00210B77 File Offset: 0x0020ED77
		public void Dispose()
		{
			if (this.model_compute_buffer != null)
			{
				this.model_compute_buffer.Dispose();
			}
			if (this.drawer != null)
			{
				this.drawer.Dispose();
			}
		}

		// Token: 0x04003411 RID: 13329
		public ComputeBuffer model_compute_buffer;

		// Token: 0x04003412 RID: 13330
		public GrowBuffer<TextureBaker.InstancedArrowDrawerBatched.DrawCallData> model_data_buffer;

		// Token: 0x04003413 RID: 13331
		public TextureBaker.InstancedArrowDrawerBatched drawer;
	}

	// Token: 0x02000620 RID: 1568
	public class InstancedDustDrawerBatched : IDisposable
	{
		// Token: 0x060046DA RID: 18138 RVA: 0x00210B9F File Offset: 0x0020ED9F
		public static void Init()
		{
			TextureBaker.InstancedDustDrawerBatched.DrawCallDataSize = Marshal.SizeOf(typeof(TextureBaker.InstancedDustDrawerBatched.DrawCallData));
		}

		// Token: 0x060046DB RID: 18139 RVA: 0x00210BB8 File Offset: 0x0020EDB8
		public void Reload()
		{
			this.mesh = MeshUtils.CreateGridMesh(new Vector3(-0.5f, 0.5f, -0.5f), 1, 1, -Vector3.up, Vector3.right, false, false);
			this.mesh.name = "InstancedDustMesh";
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			Material material = new Material(Shader.Find("BSG/Instanced/DustInstanced"));
			material.SetColor("_Color", new Color(0.9372f, 0.7803f, 0.54509f, 1f));
			material.SetFloat("duration", 4f);
			DT.Field field = global::Defs.Get(false).dt.Find("Unit", null);
			material.SetTexture("_MainTex", field.GetValue("dust_texture", null, true, true, true, '.').Get<Texture>());
			material.SetFloat("scale", 1f);
			this.material = material;
		}

		// Token: 0x060046DC RID: 18140 RVA: 0x00210CCC File Offset: 0x0020EECC
		public InstancedDustDrawerBatched()
		{
			this.model_data = new GrowBuffer<TextureBaker.InstancedDustDrawerBatched.DrawCallData>(Allocator.Persistent, 64);
			this.model_data_buffer = new ComputeBuffer(50000, TextureBaker.InstancedDustDrawerBatched.DrawCallDataSize);
			this.argsBuffer = new ComputeBuffer(1, this.indirectArgs.Length * 4, ComputeBufferType.DrawIndirect);
			this.Reload();
		}

		// Token: 0x060046DD RID: 18141 RVA: 0x00210D30 File Offset: 0x0020EF30
		public void Dispose()
		{
			if (this.model_data_buffer != null)
			{
				this.model_data_buffer.Dispose();
			}
			if (this.model_data.IsCreated)
			{
				this.model_data.Dispose();
			}
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.mesh != null)
			{
				global::Common.DestroyObj(this.mesh);
			}
			if (this.material != null)
			{
				global::Common.DestroyObj(this.material);
			}
		}

		// Token: 0x060046DE RID: 18142 RVA: 0x00210DB0 File Offset: 0x0020EFB0
		public void Draw(Camera cam)
		{
			uint count = (uint)this.model_data.Count;
			GameCamera gameCamera = CameraController.GameCamera;
			if (gameCamera == null)
			{
				return;
			}
			Vector3 eulerAngles = gameCamera.transform.rotation.eulerAngles;
			if (count == 0U)
			{
				return;
			}
			if (this.mesh == null)
			{
				return;
			}
			this.model_data_buffer.SetData(this.model_data.ToArray());
			this.material.SetBuffer("positionsAndDurations", this.model_data_buffer);
			this.material.SetVector("cam_rot", new Vector4(eulerAngles.x, eulerAngles.y, 0f, 0f));
			this.indirectArgs[1] = count;
			this.argsBuffer.SetData(this.indirectArgs);
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, new Bounds(Vector3.zero, new Vector3(10000f, 200f, 10000f)), this.argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.Off, false, 0, cam);
		}

		// Token: 0x04003414 RID: 13332
		public static int DrawCallDataSize;

		// Token: 0x04003415 RID: 13333
		private ComputeBuffer model_data_buffer;

		// Token: 0x04003416 RID: 13334
		public GrowBuffer<TextureBaker.InstancedDustDrawerBatched.DrawCallData> model_data;

		// Token: 0x04003417 RID: 13335
		public const float scale = 1f;

		// Token: 0x04003418 RID: 13336
		public const float emission = 1.5f;

		// Token: 0x04003419 RID: 13337
		public const float duration = 4f;

		// Token: 0x0400341A RID: 13338
		private ComputeBuffer argsBuffer;

		// Token: 0x0400341B RID: 13339
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x0400341C RID: 13340
		private Material material;

		// Token: 0x0400341D RID: 13341
		private Mesh mesh;

		// Token: 0x020009ED RID: 2541
		public struct DrawCallData
		{
			// Token: 0x040045E2 RID: 17890
			public float4 pos_progress;
		}
	}

	// Token: 0x02000621 RID: 1569
	public class InstancedSkinningDrawerBatched : IDisposable
	{
		// Token: 0x060046DF RID: 18143 RVA: 0x00210EB1 File Offset: 0x0020F0B1
		public static void Init()
		{
			TextureBaker.InstancedSkinningDrawerBatched.DrawCallDataSize = Marshal.SizeOf(typeof(TextureBaker.InstancedSkinningDrawerBatched.DrawCallData));
		}

		// Token: 0x060046E0 RID: 18144 RVA: 0x00210EC8 File Offset: 0x0020F0C8
		public void Reload(ComputeBuffer colors_buffer)
		{
			TextureBaker.InstancedSkinningDrawerBatched.<>c__DisplayClass14_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.colors_buffer = colors_buffer;
			this.material = new Material(this.baked_data.Mat);
			this.material.name = string.Format("{0} - Instance ({1})", this.baked_data.Mat.name, DateTime.Now);
			this.material.shader = Shader.Find("BSG/Instanced/UnitTintedBatched");
			this.mpb = new MaterialPropertyBlock();
			this.mesh = this.baked_data.NewMesh;
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			this.<Reload>g__SetupMaterialProperties|14_0(this.material, ref CS$<>8__locals1);
			this.outlineMaterial = new Material(this.baked_data.Mat);
			this.outlineMaterial.name = string.Format("{0}-Backlight - Instance ({1})", this.baked_data.Mat.name, DateTime.Now);
			this.outlineMaterial.shader = Shader.Find("BSG/Instanced/UnitTintedBatchedOutline");
			this.<Reload>g__SetupMaterialProperties|14_0(this.outlineMaterial, ref CS$<>8__locals1);
			this.outlineMaterial.renderQueue = 2459;
		}

		// Token: 0x060046E1 RID: 18145 RVA: 0x00211010 File Offset: 0x0020F210
		public InstancedSkinningDrawerBatched(KeyframeTextureBaker.BakedData baked_data, ComputeBuffer colors_buffer, float scale_mod = 1f, bool enable_highlight = true)
		{
			this.baked_data = baked_data;
			this.id = TextureBaker.InstancedSkinningDrawerBatched.drawerCount;
			TextureBaker.InstancedSkinningDrawerBatched.drawerCount++;
			this.argsBuffer = new ComputeBuffer(5, 4, ComputeBufferType.DrawIndirect);
			this.scale = scale_mod;
			this.enable_highlight = enable_highlight;
			this.Reload(colors_buffer);
		}

		// Token: 0x060046E2 RID: 18146 RVA: 0x00211080 File Offset: 0x0020F280
		public void Dispose()
		{
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.material != null)
			{
				global::Common.DestroyObj(this.material);
				global::Common.DestroyObj(this.outlineMaterial);
			}
		}

		// Token: 0x060046E3 RID: 18147 RVA: 0x002110BC File Offset: 0x0020F2BC
		public void Draw(ComputeBuffer model_data_buffer, ComputeBuffer stance_colors_buffer, uint count, int layer, Camera cam)
		{
			this.material.SetBuffer("model_data_buffer", model_data_buffer);
			this.outlineMaterial.SetBuffer("model_data_buffer", model_data_buffer);
			this.outlineMaterial.SetBuffer("stance_colors", stance_colors_buffer);
			if (this.mesh == null || this.material == null)
			{
				return;
			}
			this.indirectArgs[1] = count;
			this.argsBuffer.SetData(this.indirectArgs);
			this.mpb.SetFloat("_Bla", (float)this.id);
			Bounds bounds = new Bounds(Vector3.zero, new Vector3(10000f, 600f, 10000f));
			bounds.center = new Vector3(bounds.center.x + Mathf.Cos((float)this.id * 5.1f) * 2000f, bounds.center.y, bounds.center.z + Mathf.Sin((float)this.id) * 2000f);
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, bounds, this.argsBuffer, 0, this.mpb, ShadowCastingMode.On, true, layer, cam);
			int shaderPass = this.material.FindPass("MotionVectors");
			if (cam.depthTextureMode.HasFlag(DepthTextureMode.MotionVectors))
			{
				CustomMotionVectors.DrawMeshInstancedIndirect(this.mesh, 0, this.material, shaderPass, this.argsBuffer, 0, this.mpb);
			}
			if (this.enable_highlight && !TextureBaker.disable_troops_highlight)
			{
				this.mpb.SetFloat("_Bla", (float)this.id);
				Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.outlineMaterial, bounds, this.argsBuffer, 0, this.mpb, ShadowCastingMode.Off, false, layer, cam);
			}
		}

		// Token: 0x060046E5 RID: 18149 RVA: 0x00211284 File Offset: 0x0020F484
		[CompilerGenerated]
		private void <Reload>g__SetupMaterialProperties|14_0(Material mat, ref TextureBaker.InstancedSkinningDrawerBatched.<>c__DisplayClass14_0 A_2)
		{
			mat.SetFloat("textureHeight", (float)this.baked_data.Texture.height);
			mat.SetFloat("scale", this.baked_data.scale * this.scale);
			mat.SetVector("forwardRot", new Vector4(this.baked_data.forwardRot.x, this.baked_data.forwardRot.y, this.baked_data.forwardRot.z, this.baked_data.forwardRot.w));
			mat.renderQueue = 2460;
			mat.SetTexture("_AnimationTexture", this.baked_data.Texture);
			mat.SetBuffer("colors", A_2.colors_buffer);
		}

		// Token: 0x0400341E RID: 13342
		public static int DrawCallDataSize;

		// Token: 0x0400341F RID: 13343
		public KeyframeTextureBaker.BakedData baked_data;

		// Token: 0x04003420 RID: 13344
		private ComputeBuffer argsBuffer;

		// Token: 0x04003421 RID: 13345
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x04003422 RID: 13346
		private MaterialPropertyBlock mpb;

		// Token: 0x04003423 RID: 13347
		public Material material;

		// Token: 0x04003424 RID: 13348
		public Material outlineMaterial;

		// Token: 0x04003425 RID: 13349
		private int id;

		// Token: 0x04003426 RID: 13350
		private static int drawerCount;

		// Token: 0x04003427 RID: 13351
		private Mesh mesh;

		// Token: 0x04003428 RID: 13352
		private bool enable_highlight;

		// Token: 0x04003429 RID: 13353
		public float scale = 1f;

		// Token: 0x020009EE RID: 2542
		public struct DrawCallData
		{
			// Token: 0x040045E3 RID: 17891
			public float3 pos;

			// Token: 0x040045E4 RID: 17892
			public float3 prevPos;

			// Token: 0x040045E5 RID: 17893
			public float3 rot;

			// Token: 0x040045E6 RID: 17894
			public float3 anim_data;

			// Token: 0x040045E7 RID: 17895
			public int kingdom_color_id;

			// Token: 0x040045E8 RID: 17896
			public int stance_color_id;
		}
	}

	// Token: 0x02000622 RID: 1570
	public class InstancedSelectionDrawerBatched : IDisposable
	{
		// Token: 0x060046E6 RID: 18150 RVA: 0x0021134C File Offset: 0x0020F54C
		public static void Init()
		{
			TextureBaker.InstancedSelectionDrawerBatched.DrawCallDataSize = Marshal.SizeOf(typeof(TextureBaker.InstancedSelectionDrawerBatched.DrawCallData));
		}

		// Token: 0x060046E7 RID: 18151 RVA: 0x00211364 File Offset: 0x0020F564
		public void Reload(ComputeBuffer colors_buffer, TextureBaker.InstancedSelectionDrawerBatched.SelectionType circle)
		{
			this.deferred_material = new Material(Shader.Find("BSG/Instanced/SquadSelectionBatched"));
			if (circle == TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow || circle == TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrowShootRange)
			{
				this.mesh = MeshUtils.CreateGridMesh(new Vector3(-0.5f, 0.5f, 0.5f), 1, 1, -Vector3.forward, Vector3.right, false, false);
				this.mesh.name = "InstancedPathArrowMesh";
				if (circle == TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow)
				{
					this.deferred_material.SetTexture("_MainTex", global::Defs.GetObj<Texture2D>("Unit", "path_arrow_circle_tex", null));
				}
				else
				{
					this.deferred_material.SetTexture("_MainTex", global::Defs.GetObj<Texture2D>("Unit", "path_arrow_shooting_range_tex", null));
				}
			}
			else
			{
				float @float = global::Defs.GetFloat("Unit", "global_selection_radius", null, 1f);
				float d = @float * 2f;
				this.mesh = MeshUtils.CreateGridMesh(new Vector3(-@float, 0.5f, @float), 1, 1, -Vector3.forward * d, Vector3.right * d, false, false);
				this.mesh.name = "InstancedSelectionMesh";
				if (circle == TextureBaker.InstancedSelectionDrawerBatched.SelectionType.TroopSelectionArrow)
				{
					this.deferred_material.SetTexture("_MainTex", global::Defs.GetObj<Texture2D>("Unit", "selection_arrow_tex", null));
				}
				else
				{
					this.deferred_material.SetTexture("_MainTex", global::Defs.GetObj<Texture2D>("Unit", "selection_circle_tex", null));
				}
			}
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			this.deferred_material.SetBuffer("colors", colors_buffer);
			this.deferred_material.renderQueue = 2458;
			this.emission_material = new Material(this.deferred_material);
			this.emission_material.SetBuffer("colors", colors_buffer);
			this.deferred_material.SetShaderPassEnabled("Emission", false);
			this.emission_material.SetShaderPassEnabled("Emission", true);
		}

		// Token: 0x060046E8 RID: 18152 RVA: 0x00211560 File Offset: 0x0020F760
		public InstancedSelectionDrawerBatched(ComputeBuffer colors_buffer, TextureBaker.InstancedSelectionDrawerBatched.SelectionType circle)
		{
			this.model_data = new GrowBuffer<TextureBaker.InstancedSelectionDrawerBatched.DrawCallData>(Allocator.Persistent, 64);
			this.model_data_buffer = new ComputeBuffer(10000, TextureBaker.InstancedSelectionDrawerBatched.DrawCallDataSize);
			this.argsBuffer = new ComputeBuffer(1, this.indirectArgs.Length * 4, ComputeBufferType.DrawIndirect);
			this.circle_mesh = circle;
			this.Reload(colors_buffer, circle);
		}

		// Token: 0x060046E9 RID: 18153 RVA: 0x002115CC File Offset: 0x0020F7CC
		public void Dispose()
		{
			if (this.model_data.IsCreated)
			{
				this.model_data.Dispose();
			}
			if (this.model_data_buffer != null)
			{
				this.model_data_buffer.Dispose();
			}
			if (this.model_data.IsCreated)
			{
				this.model_data.Dispose();
			}
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.mesh != null)
			{
				global::Common.DestroyObj(this.mesh);
			}
			if (this.deferred_material != null)
			{
				global::Common.DestroyObj(this.deferred_material);
			}
		}

		// Token: 0x060046EA RID: 18154 RVA: 0x00211664 File Offset: 0x0020F864
		public void Draw(Camera cam)
		{
			uint count = (uint)this.model_data.Count;
			if (count == 0U)
			{
				return;
			}
			this.model_data_buffer.SetData(this.model_data.ToArray());
			if (this.mesh == null)
			{
				return;
			}
			this.deferred_material.SetBuffer("model_data_buffer", this.model_data_buffer);
			this.emission_material.SetBuffer("model_data_buffer", this.model_data_buffer);
			this.indirectArgs[1] = count;
			this.argsBuffer.SetData(this.indirectArgs);
			int shader_pass = this.emission_material.FindPass("Emission");
			this.deferred_material.SetShaderPassEnabled("Emission", false);
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.deferred_material, new Bounds(Vector3.zero, new Vector3(10000f, 200f, 10000f)), this.argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.Off, false, 0, cam);
			EmissionRenderer.DrawInstancedIndirect(this.mesh, 0, this.emission_material, shader_pass, this.argsBuffer, 0, new MaterialPropertyBlock());
		}

		// Token: 0x0400342A RID: 13354
		public static int DrawCallDataSize;

		// Token: 0x0400342B RID: 13355
		private ComputeBuffer model_data_buffer;

		// Token: 0x0400342C RID: 13356
		public GrowBuffer<TextureBaker.InstancedSelectionDrawerBatched.DrawCallData> model_data;

		// Token: 0x0400342D RID: 13357
		private ComputeBuffer argsBuffer;

		// Token: 0x0400342E RID: 13358
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x0400342F RID: 13359
		private Material deferred_material;

		// Token: 0x04003430 RID: 13360
		private Material emission_material;

		// Token: 0x04003431 RID: 13361
		public TextureBaker.InstancedSelectionDrawerBatched.SelectionType circle_mesh;

		// Token: 0x04003432 RID: 13362
		public Mesh mesh;

		// Token: 0x020009F0 RID: 2544
		public struct DrawCallData
		{
			// Token: 0x040045EB RID: 17899
			public float3 pos;

			// Token: 0x040045EC RID: 17900
			public int color_id;

			// Token: 0x040045ED RID: 17901
			public float scale;

			// Token: 0x040045EE RID: 17902
			public float rot;
		}

		// Token: 0x020009F1 RID: 2545
		public enum SelectionType
		{
			// Token: 0x040045F0 RID: 17904
			PathArrow,
			// Token: 0x040045F1 RID: 17905
			TroopSelectionArrow,
			// Token: 0x040045F2 RID: 17906
			TroopSelectionCircle,
			// Token: 0x040045F3 RID: 17907
			PathArrowShootRange
		}
	}

	// Token: 0x02000623 RID: 1571
	public class InstancedArrowDrawerBatched : IDisposable
	{
		// Token: 0x060046EB RID: 18155 RVA: 0x0021176D File Offset: 0x0020F96D
		public static void Init()
		{
			TextureBaker.InstancedArrowDrawerBatched.DrawCallDataSize = Marshal.SizeOf(typeof(TextureBaker.InstancedArrowDrawerBatched.DrawCallData));
		}

		// Token: 0x060046EC RID: 18156 RVA: 0x00211783 File Offset: 0x0020F983
		public static void ClearMeshes()
		{
			TextureBaker.InstancedArrowDrawerBatched.arrow_meshes.Clear();
		}

		// Token: 0x060046ED RID: 18157 RVA: 0x00211790 File Offset: 0x0020F990
		public static TextureBaker.InstancedArrowDrawerBatched.ArrowData LoadAssets(DT.Field def)
		{
			if (!TextureBaker.InstancedArrowDrawerBatched.arrow_meshes.ContainsKey(def) && def != null)
			{
				GameObject gameObject = def.GetValue("arrow_model", null, true, true, true, '.').Get<GameObject>();
				Material material = new Material(gameObject.GetComponent<MeshRenderer>().sharedMaterial);
				material.shader = Shader.Find("BSG/Instanced/ArrowsBatched");
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				TextureBaker.InstancedArrowDrawerBatched.arrow_meshes[def] = new TextureBaker.InstancedArrowDrawerBatched.ArrowData
				{
					mesh = component.sharedMesh,
					mat = material
				};
			}
			TextureBaker.InstancedArrowDrawerBatched.ArrowData result;
			TextureBaker.InstancedArrowDrawerBatched.arrow_meshes.TryGetValue(def, out result);
			return result;
		}

		// Token: 0x060046EE RID: 18158 RVA: 0x00211828 File Offset: 0x0020FA28
		public void Reload()
		{
			TextureBaker.InstancedArrowDrawerBatched.ArrowData arrowData = TextureBaker.InstancedArrowDrawerBatched.LoadAssets(this.def);
			this.mesh = arrowData.mesh;
			if (this.mesh == null)
			{
				Debug.LogWarning("Warning, no arrow model for " + this.def);
			}
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			this.material = new Material(arrowData.mat);
		}

		// Token: 0x060046EF RID: 18159 RVA: 0x002118AF File Offset: 0x0020FAAF
		public InstancedArrowDrawerBatched(DT.Field def)
		{
			this.argsBuffer = new ComputeBuffer(1, this.indirectArgs.Length * 4, ComputeBufferType.DrawIndirect);
			this.def = def;
			this.Reload();
		}

		// Token: 0x060046F0 RID: 18160 RVA: 0x002118EB File Offset: 0x0020FAEB
		public void Dispose()
		{
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.material != null)
			{
				global::Common.DestroyObj(this.material);
			}
			TextureBaker.InstancedArrowDrawerBatched.arrow_meshes.Remove(this.def);
		}

		// Token: 0x060046F1 RID: 18161 RVA: 0x0021192C File Offset: 0x0020FB2C
		public void Draw(ComputeBuffer arrow_model_data_buffer, uint count = 100U, Camera cam = null)
		{
			if (this.mesh == null)
			{
				return;
			}
			this.material.SetBuffer("model_data", arrow_model_data_buffer);
			this.indirectArgs[1] = count;
			this.argsBuffer.SetData(this.indirectArgs);
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, new Bounds(Vector3.zero, 1000000f * Vector3.one), this.argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.On, true, 0, cam);
		}

		// Token: 0x04003433 RID: 13363
		public static int DrawCallDataSize;

		// Token: 0x04003434 RID: 13364
		private DT.Field def;

		// Token: 0x04003435 RID: 13365
		private ComputeBuffer argsBuffer;

		// Token: 0x04003436 RID: 13366
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x04003437 RID: 13367
		private Material material;

		// Token: 0x04003438 RID: 13368
		private Mesh mesh;

		// Token: 0x04003439 RID: 13369
		private static Dictionary<DT.Field, TextureBaker.InstancedArrowDrawerBatched.ArrowData> arrow_meshes = new Dictionary<DT.Field, TextureBaker.InstancedArrowDrawerBatched.ArrowData>();

		// Token: 0x020009F2 RID: 2546
		public struct DrawCallData
		{
			// Token: 0x040045F4 RID: 17908
			public float4 pos;

			// Token: 0x040045F5 RID: 17909
			public float4 rot;
		}

		// Token: 0x020009F3 RID: 2547
		public struct ArrowData
		{
			// Token: 0x040045F6 RID: 17910
			public Mesh mesh;

			// Token: 0x040045F7 RID: 17911
			public Material mat;
		}
	}

	// Token: 0x02000624 RID: 1572
	public class InstancedTrailDrawerBatched : IDisposable
	{
		// Token: 0x060046F3 RID: 18163 RVA: 0x002119BA File Offset: 0x0020FBBA
		public static void Init()
		{
			TextureBaker.InstancedTrailDrawerBatched.DrawCallDataSize = Marshal.SizeOf(typeof(TextureBaker.InstancedTrailDrawerBatched.DrawCallData));
		}

		// Token: 0x060046F4 RID: 18164 RVA: 0x002119D0 File Offset: 0x0020FBD0
		public void Reload()
		{
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			Texture texture = this.def.GetValue("arrow_trail_texture", null, true, true, true, '.').Get<Texture>();
			if (texture != null)
			{
				this.material.SetTexture("_MainTex", texture);
			}
		}

		// Token: 0x060046F5 RID: 18165 RVA: 0x00211A44 File Offset: 0x0020FC44
		public InstancedTrailDrawerBatched()
		{
			this.model_data = new GrowBuffer<TextureBaker.InstancedTrailDrawerBatched.DrawCallData>(Allocator.Persistent, 64);
			this.model_data_buffer = new ComputeBuffer(10000, TextureBaker.InstancedTrailDrawerBatched.DrawCallDataSize);
			DT.Field field = global::Defs.Get(false).dt.Find("SalvoData", null);
			this.def = field;
			List<Vector3> list = new List<Vector3>();
			int num = 16;
			for (float num2 = 0f; num2 < (float)num; num2 += 1f)
			{
				list.Add(new Vector3(0f, 0f, num2 / (float)num));
			}
			this.mesh = MeshUtils.CreateLinesMesh(list, 0.05f, 2, true, false, null, null);
			this.mesh.name = "InstancedTrailMesh";
			this.material = new Material(Shader.Find("BSG/Instanced/TrailsBatched"));
			this.argsBuffer = new ComputeBuffer(1, this.indirectArgs.Length * 4, ComputeBufferType.DrawIndirect);
			this.Reload();
		}

		// Token: 0x060046F6 RID: 18166 RVA: 0x00211B38 File Offset: 0x0020FD38
		public void Dispose()
		{
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.mesh != null)
			{
				global::Common.DestroyObj(this.mesh);
				this.mesh = null;
			}
			if (this.material != null)
			{
				global::Common.DestroyObj(this.material);
			}
			if (this.model_data_buffer != null)
			{
				this.model_data_buffer.Dispose();
			}
			if (this.model_data.IsCreated)
			{
				this.model_data.Dispose();
			}
		}

		// Token: 0x060046F7 RID: 18167 RVA: 0x00211BBC File Offset: 0x0020FDBC
		public void Draw(Camera cam)
		{
			uint count = (uint)this.model_data.Count;
			if (count == 0U)
			{
				return;
			}
			if (this.mesh == null)
			{
				return;
			}
			this.model_data_buffer.SetData(this.model_data.ToArray());
			this.material.SetBuffer("model_data", this.model_data_buffer);
			this.indirectArgs[1] = count;
			this.argsBuffer.SetData(this.indirectArgs);
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, new Bounds(Vector3.zero, 1000000f * Vector3.one), this.argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.On, true, 0, cam);
		}

		// Token: 0x0400343A RID: 13370
		public static int DrawCallDataSize;

		// Token: 0x0400343B RID: 13371
		private ComputeBuffer model_data_buffer;

		// Token: 0x0400343C RID: 13372
		public GrowBuffer<TextureBaker.InstancedTrailDrawerBatched.DrawCallData> model_data;

		// Token: 0x0400343D RID: 13373
		private DT.Field def;

		// Token: 0x0400343E RID: 13374
		private ComputeBuffer argsBuffer;

		// Token: 0x0400343F RID: 13375
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x04003440 RID: 13376
		private Material material;

		// Token: 0x04003441 RID: 13377
		private Mesh mesh;

		// Token: 0x020009F4 RID: 2548
		public struct DrawCallData
		{
			// Token: 0x040045F8 RID: 17912
			public float t;

			// Token: 0x040045F9 RID: 17913
			public float dur;

			// Token: 0x040045FA RID: 17914
			public float3 start;

			// Token: 0x040045FB RID: 17915
			public float3 end;

			// Token: 0x040045FC RID: 17916
			public float2 startVelocity;

			// Token: 0x040045FD RID: 17917
			public float gravity;
		}
	}

	// Token: 0x02000625 RID: 1573
	public class InstancedDecalDrawerBatched : IDisposable
	{
		// Token: 0x060046F8 RID: 18168 RVA: 0x00211C69 File Offset: 0x0020FE69
		public static void Init()
		{
			TextureBaker.InstancedDecalDrawerBatched.DrawCallDataSize = Marshal.SizeOf(typeof(TextureBaker.InstancedDecalDrawerBatched.DrawCallData));
		}

		// Token: 0x060046F9 RID: 18169 RVA: 0x00211C80 File Offset: 0x0020FE80
		public void Reload()
		{
			if (this.mesh == null)
			{
				float num = 1f;
				float num2 = 1f;
				int num3 = 10;
				int num4 = 10;
				float x = num / (float)num3;
				float z = num2 / (float)num4;
				this.mesh = MeshUtils.CreateGridMesh(new Vector3(-num / 2f, 0f, -num2 / 2f), num3, num4, new Vector3(x, 0f, 0f), new Vector3(0f, 0f, z), false, false);
				this.mesh.RecalculateNormals();
				this.mesh.name = "InstancedDecalMesh";
			}
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			this.material.SetFloat("appear_time", this.appear_time);
			this.material.SetVector("_tree_scale", Vector3.one);
		}

		// Token: 0x060046FA RID: 18170 RVA: 0x00211D80 File Offset: 0x0020FF80
		public InstancedDecalDrawerBatched(Material mat, float appear_time)
		{
			this.full_data = new TextureBaker.InstancedDecalDrawerBatched.DrawCallData[this.capacity];
			this.model_data = new GrowBuffer<TextureBaker.InstancedDecalDrawerBatched.DrawCallData>(Allocator.Persistent, 64);
			this.model_data_buffer = new ComputeBuffer(this.capacity, TextureBaker.InstancedDecalDrawerBatched.DrawCallDataSize);
			this.appear_time = appear_time;
			this.material = new Material(mat);
			this.atlas_count = this.material.GetInt("_Count");
			this.argsBuffer = new ComputeBuffer(1, this.indirectArgs.Length * 4, ComputeBufferType.DrawIndirect);
			this.Reload();
		}

		// Token: 0x060046FB RID: 18171 RVA: 0x00211E2C File Offset: 0x0021002C
		public void Dispose()
		{
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.mesh != null)
			{
				global::Common.DestroyObj(this.mesh);
				this.mesh = null;
			}
			if (this.material != null)
			{
				global::Common.DestroyObj(this.material);
			}
			if (this.model_data_buffer != null)
			{
				this.model_data_buffer.Dispose();
			}
			if (this.model_data.IsCreated)
			{
				this.model_data.Dispose();
			}
		}

		// Token: 0x060046FC RID: 18172 RVA: 0x00211EB0 File Offset: 0x002100B0
		public void Draw(Camera cam)
		{
			uint count = (uint)this.model_data.Count;
			if (count == 0U)
			{
				return;
			}
			if (this.mesh == null)
			{
				return;
			}
			this.model_data_buffer.SetData(this.model_data.ToArray());
			this.material.SetBuffer("positionBuffer", this.model_data_buffer);
			this.indirectArgs[1] = count;
			this.argsBuffer.SetData(this.indirectArgs);
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, new Bounds(Vector3.zero, 1000000f * Vector3.one), this.argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.On, true, 0, cam);
		}

		// Token: 0x060046FD RID: 18173 RVA: 0x00211F60 File Offset: 0x00210160
		public void AddDecal(float3 position, float angle, float scale, float target_alpha, int atlas_idx)
		{
			TextureBaker.InstancedDecalDrawerBatched.DrawCallData drawCallData = new TextureBaker.InstancedDecalDrawerBatched.DrawCallData
			{
				pos = position,
				rot = angle,
				scale = scale,
				target_alpha = target_alpha,
				atlas_id = (float)atlas_idx,
				spawn_time = UnityEngine.Time.time
			};
			this.full_data[this.buff_idx] = drawCallData;
			this.buff_idx = (this.buff_idx + 1) % this.capacity;
		}

		// Token: 0x060046FE RID: 18174 RVA: 0x00211FD8 File Offset: 0x002101D8
		public void FillBuffer(float4[] f_planes)
		{
			this.model_data.Clear();
			for (int i = 0; i < this.full_data.Length; i++)
			{
				TextureBaker.InstancedDecalDrawerBatched.DrawCallData item = this.full_data[i];
				if (TextureBaker.Visible(item.pos, f_planes))
				{
					this.model_data.Add(item);
				}
			}
		}

		// Token: 0x060046FF RID: 18175 RVA: 0x0021202B File Offset: 0x0021022B
		public void Empty()
		{
			this.full_data = new TextureBaker.InstancedDecalDrawerBatched.DrawCallData[this.capacity];
			this.buff_idx = 0;
			this.model_data.Clear();
		}

		// Token: 0x04003442 RID: 13378
		public static int DrawCallDataSize;

		// Token: 0x04003443 RID: 13379
		private ComputeBuffer model_data_buffer;

		// Token: 0x04003444 RID: 13380
		public TextureBaker.InstancedDecalDrawerBatched.DrawCallData[] full_data;

		// Token: 0x04003445 RID: 13381
		public GrowBuffer<TextureBaker.InstancedDecalDrawerBatched.DrawCallData> model_data;

		// Token: 0x04003446 RID: 13382
		public int buff_idx;

		// Token: 0x04003447 RID: 13383
		public int capacity = 1000;

		// Token: 0x04003448 RID: 13384
		public int atlas_count;

		// Token: 0x04003449 RID: 13385
		private ComputeBuffer argsBuffer;

		// Token: 0x0400344A RID: 13386
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x0400344B RID: 13387
		private Material material;

		// Token: 0x0400344C RID: 13388
		private Mesh mesh;

		// Token: 0x0400344D RID: 13389
		private float appear_time;

		// Token: 0x020009F5 RID: 2549
		public struct DrawCallData
		{
			// Token: 0x17000729 RID: 1833
			// (get) Token: 0x06005505 RID: 21765 RVA: 0x0024837D File Offset: 0x0024657D
			// (set) Token: 0x06005506 RID: 21766 RVA: 0x0024838A File Offset: 0x0024658A
			public float3 pos
			{
				get
				{
					return this.pos_rot.xyz;
				}
				set
				{
					this.pos_rot = new float4(value, this.rot);
				}
			}

			// Token: 0x1700072A RID: 1834
			// (get) Token: 0x06005507 RID: 21767 RVA: 0x0024839E File Offset: 0x0024659E
			// (set) Token: 0x06005508 RID: 21768 RVA: 0x002483AB File Offset: 0x002465AB
			public float rot
			{
				get
				{
					return this.pos_rot.w;
				}
				set
				{
					this.pos_rot = new float4(this.pos, value);
				}
			}

			// Token: 0x1700072B RID: 1835
			// (get) Token: 0x06005509 RID: 21769 RVA: 0x002483BF File Offset: 0x002465BF
			// (set) Token: 0x0600550A RID: 21770 RVA: 0x002483CC File Offset: 0x002465CC
			public float scale
			{
				get
				{
					return this.additional.x;
				}
				set
				{
					this.additional = new float4(value, this.additional.yzw);
				}
			}

			// Token: 0x1700072C RID: 1836
			// (get) Token: 0x0600550B RID: 21771 RVA: 0x002483E5 File Offset: 0x002465E5
			// (set) Token: 0x0600550C RID: 21772 RVA: 0x002483F2 File Offset: 0x002465F2
			public float target_alpha
			{
				get
				{
					return this.additional.y;
				}
				set
				{
					this.additional = new float4(this.additional.x, value, this.additional.zw);
				}
			}

			// Token: 0x1700072D RID: 1837
			// (get) Token: 0x0600550D RID: 21773 RVA: 0x00248416 File Offset: 0x00246616
			// (set) Token: 0x0600550E RID: 21774 RVA: 0x00248423 File Offset: 0x00246623
			public float spawn_time
			{
				get
				{
					return this.additional.z;
				}
				set
				{
					this.additional = new float4(this.additional.xy, value, this.additional.w);
				}
			}

			// Token: 0x1700072E RID: 1838
			// (get) Token: 0x0600550F RID: 21775 RVA: 0x00248447 File Offset: 0x00246647
			// (set) Token: 0x06005510 RID: 21776 RVA: 0x00248454 File Offset: 0x00246654
			public float atlas_id
			{
				get
				{
					return this.additional.w;
				}
				set
				{
					this.additional = new float4(this.additional.xyz, value);
				}
			}

			// Token: 0x040045FE RID: 17918
			public float4 pos_rot;

			// Token: 0x040045FF RID: 17919
			public float4 additional;
		}
	}

	// Token: 0x02000626 RID: 1574
	public class InstancedSkinningDrawer : IDisposable
	{
		// Token: 0x06004700 RID: 18176 RVA: 0x00212050 File Offset: 0x00210250
		public void Init()
		{
			this.material = new Material(this.BakedData.Mat);
			this.material.shader = Shader.Find("BSG/Instanced/UnitTintedGPUInstanced");
			this.mpb = new MaterialPropertyBlock();
			this.argsBuffer = new ComputeBuffer(5, 4, ComputeBufferType.DrawIndirect);
			this.indirectArgs[0] = this.mesh.GetIndexCount(0);
			this.indirectArgs[1] = 0U;
			this.argsBuffer.SetData(this.indirectArgs);
			this.material.SetColor("_Color", this.col1);
			this.material.SetFloat("textureHeight", (float)this.BakedData.Texture.height);
			this.material.SetFloat("scale", this.BakedData.scale);
			this.material.SetVector("forwardRot", new Vector4(this.BakedData.forwardRot.x, this.BakedData.forwardRot.y, this.BakedData.forwardRot.z, this.BakedData.forwardRot.w));
			this.material.renderQueue = 2460;
			this.material.SetTexture("_AnimationTexture", this.BakedData.Texture);
		}

		// Token: 0x06004701 RID: 18177 RVA: 0x002121A8 File Offset: 0x002103A8
		public InstancedSkinningDrawer(global::Squad squad, Mesh meshToDraw, KeyframeTextureBaker.BakedData bakedData, Color col1)
		{
			this.id = TextureBaker.InstancedSkinningDrawer.drawerCount;
			TextureBaker.InstancedSkinningDrawer.drawerCount++;
			this.mesh = meshToDraw;
			this.BakedData = bakedData;
			this.mpb = new MaterialPropertyBlock();
			this.col1 = col1;
			this.squad = squad;
			this.Init();
		}

		// Token: 0x06004702 RID: 18178 RVA: 0x0021220C File Offset: 0x0021040C
		public void Dispose()
		{
			if (this.argsBuffer != null)
			{
				this.argsBuffer.Dispose();
			}
			if (this.material != null)
			{
				global::Common.DestroyObj(this.material);
			}
		}

		// Token: 0x06004703 RID: 18179 RVA: 0x0021223C File Offset: 0x0021043C
		public void Draw(ComputeBuffer posRot, ComputeBuffer textureCoords, ComputeBuffer drawer_ids, int model_id, int object_id)
		{
			int count = posRot.count;
			if (count == 0)
			{
				return;
			}
			this.material.SetBuffer("textureCoordinatesBuffer", textureCoords);
			this.material.SetBuffer("positionsAndRotations", posRot);
			this.material.SetBuffer("drawer_ids", drawer_ids);
			this.material.SetInt("drawer_model_id", model_id);
			this.material.SetInt("drawer_obj_id", object_id);
			this.material.SetTexture("_AnimationTexture", this.BakedData.Texture);
			if (this.mesh == null || this.material == null)
			{
				return;
			}
			this.indirectArgs[1] = (uint)count;
			this.argsBuffer.SetData(this.indirectArgs);
			this.mpb.SetFloat("_Bla", (float)this.id);
			Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, TextureBaker.UnitTypeData.CalcBounds(this.squad), this.argsBuffer, 0, this.mpb, ShadowCastingMode.On, true);
		}

		// Token: 0x0400344E RID: 13390
		private ComputeBuffer argsBuffer;

		// Token: 0x0400344F RID: 13391
		private readonly uint[] indirectArgs = new uint[5];

		// Token: 0x04003450 RID: 13392
		public KeyframeTextureBaker.BakedData BakedData;

		// Token: 0x04003451 RID: 13393
		private MaterialPropertyBlock mpb;

		// Token: 0x04003452 RID: 13394
		public Material material;

		// Token: 0x04003453 RID: 13395
		private int id;

		// Token: 0x04003454 RID: 13396
		private static int drawerCount;

		// Token: 0x04003455 RID: 13397
		private Color col1;

		// Token: 0x04003456 RID: 13398
		private Mesh mesh;

		// Token: 0x04003457 RID: 13399
		private global::Squad squad;
	}
}
