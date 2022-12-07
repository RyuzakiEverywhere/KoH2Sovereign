using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200007E RID: 126
[ExecuteInEditMode]
public class TreesBatching : MonoBehaviour
{
	// Token: 0x060004B2 RID: 1202 RVA: 0x00036B3B File Offset: 0x00034D3B
	[ConsoleMethod("debug_trees")]
	public static void DebugTrees(int val)
	{
		TreesBatching.debug_trees = (val == 1);
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x00036B48 File Offset: 0x00034D48
	[ConsoleMethod("disable_tree")]
	public static void DisableTree(string name)
	{
		if (TreesBatching.disabled_trees.Contains(name))
		{
			return;
		}
		TreesBatching.disabled_trees.Add(name);
		TreesBatching.need_rebuild = true;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < TreesBatching.disabled_trees.Count; i++)
		{
			stringBuilder.AppendLine(TreesBatching.disabled_trees[i]);
		}
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x00036BAC File Offset: 0x00034DAC
	[ConsoleMethod("enable_tree")]
	public static void EnableTree(string name)
	{
		if (!TreesBatching.disabled_trees.Contains(name))
		{
			return;
		}
		TreesBatching.disabled_trees.Remove(name);
		TreesBatching.need_rebuild = true;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < TreesBatching.disabled_trees.Count; i++)
		{
			stringBuilder.AppendLine(TreesBatching.disabled_trees[i]);
		}
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060004B5 RID: 1205 RVA: 0x00036C11 File Offset: 0x00034E11
	// (set) Token: 0x060004B6 RID: 1206 RVA: 0x00036C1C File Offset: 0x00034E1C
	public bool hideTrees
	{
		get
		{
			return this._hideTrees;
		}
		set
		{
			if (this._hideTrees == value)
			{
				return;
			}
			this._hideTrees = value;
			if (this.tree_types != null)
			{
				for (int i = 0; i < this.tree_types.Length; i++)
				{
					TreesBatching.TreeType treeType = this.tree_types[i];
					MeshRenderer meshRenderer;
					if (treeType == null)
					{
						meshRenderer = null;
					}
					else
					{
						TreesBatching.SpeedTreeInfo speed_tree_info = treeType.speed_tree_info;
						meshRenderer = ((speed_tree_info != null) ? speed_tree_info.active_renderer : null);
					}
					MeshRenderer meshRenderer2 = meshRenderer;
					if (!(meshRenderer2 == null))
					{
						meshRenderer2.gameObject.SetActive(!this._hideTrees);
					}
				}
			}
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060004B7 RID: 1207 RVA: 0x00036C93 File Offset: 0x00034E93
	public static int hide_trees_layer
	{
		get
		{
			if (TreesBatching._hide_trees_layer == -1)
			{
				TreesBatching._hide_trees_layer = LayerMask.NameToLayer("Facegen");
			}
			return TreesBatching._hide_trees_layer;
		}
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x00036CB4 File Offset: 0x00034EB4
	private void ClearHideTrees()
	{
		if (TreesBatching.hide_trees_tex_default == null)
		{
			TreesBatching.hide_trees_tex_default = new Texture2D(1, 1, TextureFormat.R8, false, false);
			TreesBatching.hide_trees_tex_default.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
			TreesBatching.hide_trees_tex_default.Apply();
		}
		Shader.SetGlobalTexture("_HideTreesTexture", TreesBatching.hide_trees_tex_default);
		if (TreesBatching.hide_trees_tex_dither_default == null)
		{
			TreesBatching.hide_trees_tex_dither_default = new Texture2D(1, 1, TextureFormat.R8, false, false);
			TreesBatching.hide_trees_tex_dither_default.SetPixel(0, 0, Color.black);
			TreesBatching.hide_trees_tex_dither_default.Apply();
		}
		Shader.SetGlobalTexture("_HideTreesDitherTexture", TreesBatching.hide_trees_tex_dither_default);
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x00036D64 File Offset: 0x00034F64
	private void InitHideTrees()
	{
		if (!Application.isPlaying)
		{
			this.ClearHideTrees();
			return;
		}
		if (BattleMap.Get() == null)
		{
			return;
		}
		if (this.hide_trees_texture == null)
		{
			this.hide_trees_texture = RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Default);
		}
		if (TreesBatching.dither_tex == null)
		{
			TreesBatching.dither_tex = BattleMap.Get().Dither_Tex;
		}
		if (this.hide_trees_camera == null)
		{
			GameObject gameObject = new GameObject("HideTreesCamera");
			this.hide_trees_camera = gameObject.AddComponent<Camera>();
			this.hide_trees_camera.targetTexture = this.hide_trees_texture;
			this.hide_trees_camera.cullingMask = 1 << TreesBatching.hide_trees_layer;
			this.hide_trees_camera.clearFlags = CameraClearFlags.Color;
			this.hide_trees_camera.backgroundColor = Color.black;
			this.hide_trees_camera.enabled = false;
		}
		Shader.SetGlobalTexture("_HideTreesTexture", this.hide_trees_texture);
		Shader.SetGlobalTexture("_HideTreesDitherTexture", TreesBatching.dither_tex);
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x00036E64 File Offset: 0x00035064
	private void UpdateHideTreesCamera(Camera cam)
	{
		if (this.hide_trees_camera == null)
		{
			return;
		}
		this.hide_trees_camera.transform.position = cam.transform.position;
		this.hide_trees_camera.transform.rotation = cam.transform.rotation;
		this.hide_trees_camera.fieldOfView = cam.fieldOfView;
		this.hide_trees_camera.nearClipPlane = cam.nearClipPlane;
		this.hide_trees_camera.aspect = cam.aspect;
		this.hide_trees_camera.farClipPlane = cam.farClipPlane;
		this.hide_trees_camera.Render();
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x00036F05 File Offset: 0x00035105
	private void OnEnable()
	{
		this.t_changed = UnityEngine.Time.time;
		this.do_rebuild = true;
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(this.DrawTrees));
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00036F3C File Offset: 0x0003513C
	private void OnDisable()
	{
		if (this.hide_trees_texture != null)
		{
			RenderTexture.ReleaseTemporary(this.hide_trees_texture);
			this.hide_trees_texture = null;
		}
		this.ClearHideTrees();
		this.Clear();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(this.DrawTrees));
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00036F95 File Offset: 0x00035195
	private void OnTerrainChanged(TerrainChangedFlags flags)
	{
		if (TreesBatching.ignore_terrain_changes)
		{
			return;
		}
		if (this.is_enabled && (flags & TerrainChangedFlags.TreeInstances) != (TerrainChangedFlags)0)
		{
			this.EditFromUnity();
		}
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00036FB2 File Offset: 0x000351B2
	public void EditFromUnity()
	{
		this.t_changed = UnityEngine.Time.time;
		this.terr_draw_trees = this.AreTreesRendered;
		this.EnableTerrainTrees();
		this.disabled_for_unity = true;
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060004BF RID: 1215 RVA: 0x00036FD8 File Offset: 0x000351D8
	private bool AreTreesRendered
	{
		get
		{
			return this.terrain.treeDistance > 0f;
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x00036FEC File Offset: 0x000351EC
	private void EnableTerrainTrees()
	{
		this.terrain.treeDistance = this.DefaultTerrainRenderDistance;
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x00036FFF File Offset: 0x000351FF
	private void DisableTerrainTrees()
	{
		this.terrain.treeDistance = 0f;
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x00037011 File Offset: 0x00035211
	private void Start()
	{
		this.Rebuild();
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0003701C File Offset: 0x0003521C
	public void Rebuild()
	{
		this.Clear();
		this.InitHideTrees();
		if (this.terrain == null)
		{
			this.terrain = base.GetComponent<Terrain>();
		}
		if (this.terrain == null)
		{
			return;
		}
		TerrainHeightsRenderer component = this.terrain.GetComponent<TerrainHeightsRenderer>();
		this.heights_ready = (component != null);
		this.map_bounds = this.terrain.terrainData.bounds;
		this.map_bounds.center = this.map_bounds.center + this.terrain.transform.position;
		this.BuildTreesData();
		this.cs_kernel_cull = this.cs_cull.FindKernel("WVCullTrees");
		if (this.cs_kernel_cull == -1)
		{
			return;
		}
		this.terr_draw_trees = this.AreTreesRendered;
		this.DisableTerrainTrees();
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x000370EB File Offset: 0x000352EB
	private void HandleProjectChanged()
	{
		this.do_rebuild = true;
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x000370F4 File Offset: 0x000352F4
	private void DrawTrees(Camera cam)
	{
		using (Game.Profile("TreesBatching.DrawTrees", false, 0f, null))
		{
			if (!this.is_enabled)
			{
				this.EnableTerrainTrees();
			}
			else
			{
				if (this.disabled_for_unity)
				{
					this.EnableTerrainTrees();
				}
				else
				{
					this.DisableTerrainTrees();
				}
				float time = UnityEngine.Time.time;
				if (this.disabled_for_unity && time > this.t_changed + this.t_delay)
				{
					this.disabled_for_unity = false;
					this.DisableTerrainTrees();
					this.do_rebuild = true;
				}
				if (this.do_rebuild || TreesBatching.need_rebuild)
				{
					this.Rebuild();
					this.do_rebuild = false;
					this.disabled_for_unity = false;
				}
				if (!this.disabled_for_unity)
				{
					if (!(cam != CameraController.MainCamera) || !(cam.name != "SceneCamera"))
					{
						if (!(this.terrain == null) && this.tree_types != null)
						{
							if (!(this.cs_cull == null) && this.cs_kernel_cull >= 0)
							{
								this.UpdateHideTreesCamera(cam);
								int num = this.tree_types.Length;
								this.curr_count = 0;
								this.frustum_planes = GeometryUtility.CalculateFrustumPlanes(cam);
								for (int i = 0; i < 6; i++)
								{
									Vector3 normal = this.frustum_planes[i].normal;
									float distance = this.frustum_planes[i].distance;
									this.f_planes[i].x = normal.x;
									this.f_planes[i].y = normal.y;
									this.f_planes[i].z = normal.z;
									this.f_planes[i].w = distance;
								}
								this.cam_pos = cam.transform.position;
								this.cs_cull.SetVectorArray("f_planes", this.f_planes);
								this.cs_cull.SetVector("cam_pos", this.cam_pos);
								this.cs_cull.SetFloat("cull_dist", this.cull_dist);
								uint num2;
								uint num3;
								uint num4;
								this.cs_cull.GetKernelThreadGroupSizes(this.cs_kernel_cull, out num2, out num3, out num4);
								Vector4 zero = Vector4.zero;
								if (this.wind != null && this.wind.mode == WindZoneMode.Directional)
								{
									new Vector4(this.wind.windTurbulence, this.wind.windPulseMagnitude, this.wind.windPulseFrequency, this.wind.windMain);
								}
								if (!this.hideTrees)
								{
									if (this.logs_count > 0)
									{
										Debug.Log(string.Concat(new object[]
										{
											"TB: ",
											UnityEngine.Time.frameCount,
											" ",
											cam.name
										}));
										this.logs_count--;
									}
									for (int j = 0; j < num; j++)
									{
										if (this.tree_idx < 0 || j == this.tree_idx)
										{
											TreesBatching.TreeType treeType = this.tree_types[j];
											if (treeType.Validate() && treeType.count != 0)
											{
												this.cs_cull.SetBuffer(this.cs_kernel_cull, "allTreesBuff", treeType.pos_buf);
												for (int k = 0; k < treeType.lods.Count; k++)
												{
													TreesBatching.TreeType.LODInfo lodinfo = treeType.lods[k];
													this.cs_cull.SetBuffer(this.cs_kernel_cull, "drawBuff", lodinfo.draw_buf);
													lodinfo.draw_buf.SetCounterValue(0U);
													this.cs_cull.SetFloat("min_dist", lodinfo.min * this.cull_dist);
													this.cs_cull.SetFloat("max_dist", lodinfo.max * this.cull_dist);
													int num5 = Mathf.CeilToInt((float)treeType.count / num2);
													if (num5 > 65535)
													{
														if (TreesBatching.warning_shown % 120 == 0)
														{
															TreesBatching.warning_shown = 0;
															Debug.LogWarning(string.Format("TreesBatching: Number of tread groups too high: {0}!", num5));
														}
														TreesBatching.warning_shown++;
														num5 = 65535;
													}
													this.cs_cull.Dispatch(this.cs_kernel_cull, num5, 1, 1);
													ComputeBuffer.CopyCount(lodinfo.draw_buf, lodinfo.arg_buf, 4);
													if (this.debug_count)
													{
														int[] array = new int[5];
														lodinfo.arg_buf.GetData(array);
														this.curr_count += array[1];
													}
													if (this.snappingEnabled && this.heights_ready)
													{
														lodinfo.material.SetFloat("_SnapEnabled", 1f);
													}
													else
													{
														lodinfo.material.SetFloat("_SnapEnabled", 0f);
													}
													lodinfo.material.SetFloat("_WindEnabled", 1f);
													MaterialPropertyBlock properties = null;
													TreesBatching.SpeedTreeInfo speed_tree_info = treeType.speed_tree_info;
													if (((speed_tree_info != null) ? speed_tree_info.active_renderer : null) != null && this.wind_enabled && this.debug_tree == null)
													{
														treeType.speed_tree_info.active_renderer.transform.position = cam.transform.position - cam.transform.forward * 3f;
														treeType.speed_tree_info.active_renderer.GetPropertyBlock(treeType.speed_tree_info.block);
														properties = treeType.speed_tree_info.block;
													}
													Graphics.DrawMeshInstancedIndirect(lodinfo.mesh, lodinfo.sub_mesh_idx, lodinfo.material, this.map_bounds, lodinfo.arg_buf, 0, properties, lodinfo.shadows, true, LayerMask.NameToLayer("Vegetation"), cam, LightProbeUsage.Off);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x000376D8 File Offset: 0x000358D8
	private void BuildTreesData()
	{
		TreesBatching.need_rebuild = false;
		if (this.terrain == null)
		{
			return;
		}
		Vector3 position = this.terrain.transform.position;
		TerrainData terrainData = this.terrain.terrainData;
		int num = terrainData.treePrototypes.Length;
		if (num == 0)
		{
			return;
		}
		int treeInstanceCount = terrainData.treeInstanceCount;
		if (treeInstanceCount == 0)
		{
			return;
		}
		int num2 = 0;
		this.buf_stride = sizeof(TreesBatching.TreeShaderData);
		this.tree_types = new TreesBatching.TreeType[num];
		if (this.debug_tree_prefab != null && this.debug_tree == null)
		{
			this.debug_tree = this.FillTreeTypeInfo(this.debug_tree_prefab);
			for (int i = 0; i < this.debug_tree.lods.Count; i++)
			{
				TreesBatching.TreeType.LODInfo lodinfo = this.debug_tree.lods[i];
				if (lodinfo.arg_buf == null)
				{
					lodinfo.arg_buf = new ComputeBuffer(1, this.args.Length * 4, ComputeBufferType.DrawIndirect);
				}
				this.args[0] = (int)lodinfo.mesh.GetIndexCount(0);
				this.args[1] = 1;
				this.args[2] = (int)lodinfo.mesh.GetIndexStart(0);
				this.args[3] = (int)lodinfo.mesh.GetBaseVertex(0);
				this.args[4] = 0;
				lodinfo.material.SetVector("_tree_scale", this.debug_tree.scale);
				lodinfo.arg_buf.SetData(this.args);
			}
		}
		for (int j = 0; j < num; j++)
		{
			TreePrototype tree_proto = terrainData.treePrototypes[j];
			this.tree_types[j] = this.FillTreeTypeInfo(tree_proto);
			this.tree_types[j].tree_index = j;
		}
		for (int k = 0; k < treeInstanceCount; k++)
		{
			int prototypeIndex = terrainData.GetTreeInstance(k).prototypeIndex;
			if (this.tree_types[prototypeIndex].valid)
			{
				this.tree_types[prototypeIndex].count++;
			}
		}
		for (int l = 0; l < num; l++)
		{
			if (this.tree_types[l].valid && this.tree_types[l].count != 0)
			{
				this.total_count += this.tree_types[l].count;
				this.tree_types[l].position_data = new TreesBatching.TreeShaderData[this.tree_types[l].count];
				if (this.tree_types[l].count > num2)
				{
					num2 = this.tree_types[l].count;
				}
			}
		}
		for (int m = 0; m < treeInstanceCount; m++)
		{
			TreeInstance treeInstance = terrainData.GetTreeInstance(m);
			int prototypeIndex2 = treeInstance.prototypeIndex;
			if (this.tree_types[prototypeIndex2].valid)
			{
				Vector3 vector = treeInstance.position;
				vector.Scale(terrainData.size);
				vector += position;
				Vector3 scale = new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale);
				TreesBatching.TreeShaderData treeShaderData = new TreesBatching.TreeShaderData(vector, treeInstance.rotation, scale);
				this.tree_types[prototypeIndex2].position_data[(int)this.tree_types[prototypeIndex2].buff_idx] = treeShaderData;
				this.tree_types[prototypeIndex2].buff_idx += 1U;
			}
		}
		for (int n = 0; n < num; n++)
		{
			this.UpdateTreesData(n, this.tree_types[n].position_data, false);
			this.tree_types[n].position_data = null;
		}
		if (Application.isPlaying && TreesBatching.debug_trees)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Active trees on " + this.terrain.terrainData.name + ":");
			for (int num3 = 0; num3 < this.tree_types.Length; num3++)
			{
				TreesBatching.TreeType treeType = this.tree_types[num3];
				if (treeType.Validate())
				{
					stringBuilder.AppendLine(treeType.name);
				}
			}
			Debug.Log(stringBuilder.ToString());
		}
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x00037AEC File Offset: 0x00035CEC
	public bool UpdateTreesData(int type_idx, TreesBatching.TreeShaderData[] pos_data, bool is_external = true)
	{
		if (this.tree_types == null)
		{
			return false;
		}
		TreesBatching.TreeType treeType = this.tree_types[type_idx];
		int count = treeType.count;
		int num = (pos_data == null) ? 0 : pos_data.Length;
		if (is_external)
		{
			treeType.position_data = new TreesBatching.TreeShaderData[pos_data.Length];
			Vector3 position = this.terrain.transform.position;
			for (int i = 0; i < treeType.position_data.Length; i++)
			{
				TreesBatching.TreeShaderData treeShaderData = pos_data[i];
				Vector3 vector = new Vector3(treeShaderData.x, treeShaderData.y, treeShaderData.z);
				vector += position;
				treeShaderData.x = vector.x;
				treeShaderData.y = vector.y;
				treeShaderData.z = vector.z;
				treeType.position_data[i] = treeShaderData;
			}
			treeType.count = num;
		}
		if (count != num || num == 0)
		{
			ComputeBuffer pos_buf = treeType.pos_buf;
			if (pos_buf != null)
			{
				pos_buf.Release();
			}
			treeType.pos_buf = null;
			for (int j = 0; j < treeType.lods.Count; j++)
			{
				TreesBatching.TreeType.LODInfo lodinfo = treeType.lods[j];
				ComputeBuffer draw_buf = lodinfo.draw_buf;
				if (draw_buf != null)
				{
					draw_buf.Release();
				}
				lodinfo.draw_buf = null;
			}
		}
		if (!treeType.valid || treeType.count == 0)
		{
			return false;
		}
		if (treeType.pos_buf == null)
		{
			treeType.pos_buf = new ComputeBuffer(treeType.count, this.buf_stride);
			for (int k = 0; k < treeType.lods.Count; k++)
			{
				treeType.lods[k].draw_buf = new ComputeBuffer(treeType.count, this.buf_stride, ComputeBufferType.Append);
			}
		}
		treeType.pos_buf.SetData(treeType.position_data);
		for (int l = 0; l < treeType.lods.Count; l++)
		{
			TreesBatching.TreeType.LODInfo lodinfo2 = treeType.lods[l];
			lodinfo2.material.SetVector("_tree_scale", treeType.scale);
			lodinfo2.material.SetBuffer("positionBuffer", lodinfo2.draw_buf);
			if (lodinfo2.arg_buf == null)
			{
				lodinfo2.arg_buf = new ComputeBuffer(1, this.args.Length * 4, ComputeBufferType.DrawIndirect);
			}
			this.args[0] = (int)lodinfo2.mesh.GetIndexCount(0);
			this.args[1] = treeType.count;
			this.args[2] = (int)lodinfo2.mesh.GetIndexStart(0);
			this.args[3] = (int)lodinfo2.mesh.GetBaseVertex(0);
			this.args[4] = 0;
			lodinfo2.arg_buf.SetData(this.args);
		}
		return true;
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x00037D97 File Offset: 0x00035F97
	private TreesBatching.TreeType FillTreeTypeInfo(TreePrototype tree_proto)
	{
		return this.FillTreeTypeInfo(tree_proto.prefab);
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x00037DA8 File Offset: 0x00035FA8
	private TreesBatching.TreeType FillTreeTypeInfo(GameObject prefab)
	{
		TreesBatching.TreeType treeType = new TreesBatching.TreeType
		{
			count = 0,
			buff_idx = 0U,
			valid = false
		};
		if (prefab == null)
		{
			return treeType;
		}
		if (prefab == null)
		{
			return treeType;
		}
		treeType.name = prefab.name;
		MeshRenderer meshRenderer = null;
		MeshFilter meshFilter = null;
		LODGroup component = prefab.GetComponent<LODGroup>();
		if (component != null)
		{
			LOD[] lods = component.GetLODs();
			float min = 0f;
			for (int i = 0; i < lods.Length; i++)
			{
				LOD lod = lods[i];
				if (lod.renderers != null && lod.renderers.Length != 0)
				{
					meshRenderer = (lod.renderers[0] as MeshRenderer);
					meshFilter = meshRenderer.GetComponent<MeshFilter>();
					TreesBatching.TreeType.LODInfo lodinfo = new TreesBatching.TreeType.LODInfo();
					treeType.lods.Add(lodinfo);
					if (this.debug_tree != null)
					{
						lodinfo.mesh = this.debug_tree.lods[i].mesh;
						lodinfo.material = new Material(this.debug_tree.lods[i].material);
						lodinfo.sub_mesh_idx = this.debug_tree.lods[i].sub_mesh_idx;
					}
					else
					{
						lodinfo.mesh = UnityEngine.Object.Instantiate<Mesh>(meshFilter.sharedMesh);
						lodinfo.material = new Material(meshRenderer.sharedMaterial);
						lodinfo.material.enableInstancing = false;
						lodinfo.sub_mesh_idx = meshRenderer.subMeshStartIndex;
					}
					lodinfo.min = min;
					lodinfo.max = 1f - lod.screenRelativeTransitionHeight;
					lodinfo.shadows = meshRenderer.shadowCastingMode;
					min = lodinfo.max;
					this.ValidateType(lodinfo.material, treeType, meshRenderer.gameObject);
				}
			}
			if (treeType.lods.Count > 0)
			{
				Bounds bounds = treeType.lods[0].mesh.bounds;
				treeType.radius = Mathf.Max(bounds.extents[0], Mathf.Max(bounds.extents[1], bounds.extents[2]));
			}
		}
		if (treeType.lods.Count == 0)
		{
			if (meshRenderer == null)
			{
				meshRenderer = prefab.GetComponent<MeshRenderer>();
				meshFilter = prefab.GetComponent<MeshFilter>();
			}
			if (meshRenderer == null || meshFilter == null)
			{
				this.CleanUp(treeType);
				return treeType;
			}
			Mesh sharedMesh = meshFilter.sharedMesh;
			if (sharedMesh == null)
			{
				this.CleanUp(treeType);
				return treeType;
			}
			int subMeshStartIndex = meshRenderer.subMeshStartIndex;
			if (sharedMesh == null)
			{
				this.CleanUp(treeType);
				return treeType;
			}
			Material material = new Material(meshRenderer.sharedMaterial);
			material.enableInstancing = false;
			TreesBatching.TreeType.LODInfo lodinfo2 = new TreesBatching.TreeType.LODInfo();
			treeType.lods.Add(lodinfo2);
			lodinfo2.mesh = sharedMesh;
			lodinfo2.material = material;
			lodinfo2.sub_mesh_idx = subMeshStartIndex;
			lodinfo2.min = 0f;
			lodinfo2.max = float.MaxValue;
			lodinfo2.shadows = meshRenderer.shadowCastingMode;
			Bounds bounds2 = lodinfo2.mesh.bounds;
			treeType.radius = Mathf.Max(bounds2.extents[0], Mathf.Max(bounds2.extents[1], bounds2.extents[2]));
			this.ValidateType(lodinfo2.material, treeType, meshRenderer.gameObject);
		}
		if (treeType.speed_tree_info != null)
		{
			if (treeType.speed_tree_info.active_renderer != null)
			{
				treeType.speed_tree_info.active_renderer.shadowCastingMode = ShadowCastingMode.Off;
				treeType.speed_tree_info.active_renderer.gameObject.hideFlags = HideFlags.HideAndDontSave;
				treeType.speed_tree_info.active_renderer.GetComponent<MeshFilter>().sharedMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);
			}
			treeType.speed_tree_info.block = new MaterialPropertyBlock();
		}
		treeType.scale = prefab.transform.localScale;
		treeType.valid = true;
		return treeType;
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x000381D4 File Offset: 0x000363D4
	private void ValidateType(Material mat, TreesBatching.TreeType type, GameObject prefab)
	{
		string name = mat.shader.name;
		if (!(name == "BSG/WV_Trees") && !(name == "BSG/GrassShader"))
		{
			if (name == "Custom/WindShader")
			{
				mat.shader = Shader.Find("BSG/GrassShader");
				return;
			}
			if (!(name == "BSG/SpeedTree8"))
			{
				if (name == "Nature/SpeedTree8")
				{
					if (type.speed_tree_info == null)
					{
						TreesBatching.SpeedTreeInfo speedTreeInfo = new TreesBatching.SpeedTreeInfo();
						type.speed_tree_info = speedTreeInfo;
						if (this.wind_enabled)
						{
							MeshRenderer componentInChildren = UnityEngine.Object.Instantiate<GameObject>(prefab).GetComponentInChildren<MeshRenderer>();
							speedTreeInfo.active_renderer = componentInChildren;
						}
					}
					mat.shader = Shader.Find("BSG/SpeedTree8");
					return;
				}
				mat.shader = Shader.Find("BSG/WV_Trees");
			}
			else if (type.speed_tree_info == null)
			{
				TreesBatching.SpeedTreeInfo speedTreeInfo2 = new TreesBatching.SpeedTreeInfo();
				type.speed_tree_info = speedTreeInfo2;
				if (this.wind_enabled)
				{
					MeshRenderer component = UnityEngine.Object.Instantiate<GameObject>(prefab).GetComponent<MeshRenderer>();
					speedTreeInfo2.active_renderer = component;
					return;
				}
			}
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x000382D0 File Offset: 0x000364D0
	private void CleanUp(TreesBatching.TreeType tree)
	{
		if (tree == null)
		{
			return;
		}
		ComputeBuffer pos_buf = tree.pos_buf;
		if (pos_buf != null)
		{
			pos_buf.Release();
		}
		tree.pos_buf = null;
		for (int i = 0; i < tree.lods.Count; i++)
		{
			TreesBatching.TreeType.LODInfo lodinfo = tree.lods[i];
			ComputeBuffer draw_buf = lodinfo.draw_buf;
			if (draw_buf != null)
			{
				draw_buf.Release();
			}
			lodinfo.draw_buf = null;
			ComputeBuffer arg_buf = lodinfo.arg_buf;
			if (arg_buf != null)
			{
				arg_buf.Release();
			}
			lodinfo.arg_buf = null;
			global::Common.DestroyObj(lodinfo.material);
		}
		if (tree.speed_tree_info != null)
		{
			if (tree.speed_tree_info.active_renderer != null)
			{
				global::Common.DestroyObj(tree.speed_tree_info.active_renderer.gameObject);
			}
			tree.speed_tree_info.block = null;
			tree.speed_tree_info = null;
		}
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00038398 File Offset: 0x00036598
	private void Clear()
	{
		if (this.tree_types != null)
		{
			int num = this.tree_types.Length;
			for (int i = 0; i < num; i++)
			{
				this.CleanUp(this.tree_types[i]);
			}
			this.tree_types = null;
		}
		if (this.terrain != null)
		{
			if (this.terr_draw_trees)
			{
				this.EnableTerrainTrees();
			}
			else
			{
				this.DisableTerrainTrees();
			}
		}
		if (this.debug_tree != null)
		{
			this.CleanUp(this.debug_tree);
			this.debug_tree = null;
		}
		this.total_count = 0;
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0003841D File Offset: 0x0003661D
	private void OnDestroy()
	{
		this.Clear();
	}

	// Token: 0x0400048C RID: 1164
	private int buf_stride;

	// Token: 0x0400048D RID: 1165
	public float DefaultTerrainRenderDistance = 1000f;

	// Token: 0x0400048E RID: 1166
	public static List<string> disabled_trees = new List<string>();

	// Token: 0x0400048F RID: 1167
	public static bool debug_trees = false;

	// Token: 0x04000490 RID: 1168
	public TreesBatching.TreeType[] tree_types;

	// Token: 0x04000491 RID: 1169
	public TreesBatching.TreeType debug_tree;

	// Token: 0x04000492 RID: 1170
	public GameObject debug_tree_prefab;

	// Token: 0x04000493 RID: 1171
	public int tree_idx = -1;

	// Token: 0x04000494 RID: 1172
	public int total_count;

	// Token: 0x04000495 RID: 1173
	public int curr_count;

	// Token: 0x04000496 RID: 1174
	public bool debug_count;

	// Token: 0x04000497 RID: 1175
	private int[] args = new int[5];

	// Token: 0x04000498 RID: 1176
	public Terrain terrain;

	// Token: 0x04000499 RID: 1177
	public WindZone wind;

	// Token: 0x0400049A RID: 1178
	public ComputeShader cs_cull;

	// Token: 0x0400049B RID: 1179
	private int cs_kernel_cull;

	// Token: 0x0400049C RID: 1180
	public float cull_dist = 300f;

	// Token: 0x0400049D RID: 1181
	public bool is_enabled = true;

	// Token: 0x0400049E RID: 1182
	private bool _hideTrees;

	// Token: 0x0400049F RID: 1183
	private static bool need_rebuild;

	// Token: 0x040004A0 RID: 1184
	private bool terr_draw_trees = true;

	// Token: 0x040004A1 RID: 1185
	public bool do_rebuild = true;

	// Token: 0x040004A2 RID: 1186
	private Vector3 cam_pos;

	// Token: 0x040004A3 RID: 1187
	private Plane[] frustum_planes = new Plane[6];

	// Token: 0x040004A4 RID: 1188
	private Vector4[] f_planes = new Vector4[6];

	// Token: 0x040004A5 RID: 1189
	private Bounds map_bounds;

	// Token: 0x040004A6 RID: 1190
	private float t_changed;

	// Token: 0x040004A7 RID: 1191
	public bool disabled_for_unity;

	// Token: 0x040004A8 RID: 1192
	public float t_delay = 0.5f;

	// Token: 0x040004A9 RID: 1193
	public bool wind_enabled = true;

	// Token: 0x040004AA RID: 1194
	public bool snappingEnabled = true;

	// Token: 0x040004AB RID: 1195
	private bool heights_ready = true;

	// Token: 0x040004AC RID: 1196
	public RenderTexture hide_trees_texture;

	// Token: 0x040004AD RID: 1197
	private Camera hide_trees_camera;

	// Token: 0x040004AE RID: 1198
	private static int _hide_trees_layer = -1;

	// Token: 0x040004AF RID: 1199
	public int logs_count;

	// Token: 0x040004B0 RID: 1200
	private static Texture2D hide_trees_tex_default;

	// Token: 0x040004B1 RID: 1201
	private static Texture2D hide_trees_tex_dither_default;

	// Token: 0x040004B2 RID: 1202
	private static Texture2D dither_tex;

	// Token: 0x040004B3 RID: 1203
	public static bool hide_camera_enabled = true;

	// Token: 0x040004B4 RID: 1204
	public static bool ignore_terrain_changes = false;

	// Token: 0x040004B5 RID: 1205
	private static int warning_shown = 0;

	// Token: 0x0200054E RID: 1358
	public struct TreeShaderData
	{
		// Token: 0x06004399 RID: 17305 RVA: 0x001FDF3C File Offset: 0x001FC13C
		public TreeShaderData(Vector3 pos, float angle, Vector3 scale)
		{
			this.x = pos.x;
			this.y = pos.y;
			this.z = pos.z;
			this.angle = angle;
			this.sx = scale.x;
			this.sy = scale.y;
			this.sz = scale.z;
			this.next_free_idx = 0f;
		}

		// Token: 0x04002FD6 RID: 12246
		public float x;

		// Token: 0x04002FD7 RID: 12247
		public float y;

		// Token: 0x04002FD8 RID: 12248
		public float z;

		// Token: 0x04002FD9 RID: 12249
		public float angle;

		// Token: 0x04002FDA RID: 12250
		public float sx;

		// Token: 0x04002FDB RID: 12251
		public float sy;

		// Token: 0x04002FDC RID: 12252
		public float sz;

		// Token: 0x04002FDD RID: 12253
		public float next_free_idx;
	}

	// Token: 0x0200054F RID: 1359
	public class TreeType
	{
		// Token: 0x0600439A RID: 17306 RVA: 0x001FDFA3 File Offset: 0x001FC1A3
		public bool Validate()
		{
			return !TreesBatching.disabled_trees.Contains(this.name) && this.valid;
		}

		// Token: 0x04002FDE RID: 12254
		public List<TreesBatching.TreeType.LODInfo> lods = new List<TreesBatching.TreeType.LODInfo>();

		// Token: 0x04002FDF RID: 12255
		public float radius;

		// Token: 0x04002FE0 RID: 12256
		public Vector3 scale;

		// Token: 0x04002FE1 RID: 12257
		public int tree_index;

		// Token: 0x04002FE2 RID: 12258
		public bool valid;

		// Token: 0x04002FE3 RID: 12259
		public TreesBatching.TreeShaderData[] position_data;

		// Token: 0x04002FE4 RID: 12260
		public int count;

		// Token: 0x04002FE5 RID: 12261
		public uint buff_idx;

		// Token: 0x04002FE6 RID: 12262
		public ComputeBuffer pos_buf;

		// Token: 0x04002FE7 RID: 12263
		public TreesBatching.SpeedTreeInfo speed_tree_info;

		// Token: 0x04002FE8 RID: 12264
		public string name;

		// Token: 0x020009D6 RID: 2518
		public class LODInfo
		{
			// Token: 0x04004567 RID: 17767
			public Mesh mesh;

			// Token: 0x04004568 RID: 17768
			public Material material;

			// Token: 0x04004569 RID: 17769
			public ComputeBuffer draw_buf;

			// Token: 0x0400456A RID: 17770
			public ComputeBuffer arg_buf;

			// Token: 0x0400456B RID: 17771
			public float min;

			// Token: 0x0400456C RID: 17772
			public float max;

			// Token: 0x0400456D RID: 17773
			public int sub_mesh_idx;

			// Token: 0x0400456E RID: 17774
			public ShadowCastingMode shadows;
		}
	}

	// Token: 0x02000550 RID: 1360
	public class SpeedTreeInfo
	{
		// Token: 0x04002FE9 RID: 12265
		public MaterialPropertyBlock block;

		// Token: 0x04002FEA RID: 12266
		public MeshRenderer active_renderer;
	}
}
