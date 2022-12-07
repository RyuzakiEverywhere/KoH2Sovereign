using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Token: 0x02000136 RID: 310
public class LinesBatching : MonoBehaviour
{
	// Token: 0x06001082 RID: 4226 RVA: 0x000AFBD4 File Offset: 0x000ADDD4
	public virtual void OnBuffersChanged()
	{
		using (Game.Profile("LinesBatching.OnBuffersChanged", false, 0f, null))
		{
			LinesBatching.LineInfo lineInfo = this.line_info;
			if (((lineInfo != null) ? lineInfo.compute_data : null) != null)
			{
				LinesBatching.LineInfo lineInfo2 = this.line_info;
				if (((lineInfo2 != null) ? lineInfo2.compute_buffer : null) != null)
				{
					this.line_info.compute_buffer.SetData(this.line_info.compute_data);
					LinesBatching.LineInfo lineInfo3 = this.line_info;
					if (((lineInfo3 != null) ? lineInfo3.shader_buffer : null) != null)
					{
						LinesBatching.LineInfo lineInfo4 = this.line_info;
						if (((lineInfo4 != null) ? lineInfo4.shader_data : null) != null)
						{
							this.line_info.shader_buffer.SetData(this.line_info.shader_data);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x000AFCA0 File Offset: 0x000ADEA0
	private void CalcFrustrum(Camera cam)
	{
		if (UnityEngine.Time.frameCount == LinesBatching._frustrum_last_frame && cam == LinesBatching._last_cam)
		{
			return;
		}
		using (Game.Profile("LinesBatching.CalcFrustrum", false, 0f, null))
		{
			LinesBatching._frustrum_last_frame = UnityEngine.Time.frameCount;
			LinesBatching._last_cam = cam;
			LinesBatching.frustum_planes = GeometryUtility.CalculateFrustumPlanes(cam);
			for (int i = 0; i < 6; i++)
			{
				Vector3 normal = LinesBatching.frustum_planes[i].normal;
				float distance = LinesBatching.frustum_planes[i].distance;
				LinesBatching.f_planes[i].x = normal.x;
				LinesBatching.f_planes[i].y = normal.y;
				LinesBatching.f_planes[i].z = normal.z;
				LinesBatching.f_planes[i].w = distance;
			}
		}
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x000AFD98 File Offset: 0x000ADF98
	public void Draw(Camera cam)
	{
		if (!base.enabled)
		{
			return;
		}
		using (Game.Profile("LinesBatching.Draw", false, 0f, null))
		{
			if (this.do_rebuild)
			{
				this.MarkDirty(true, true, false, false);
				this.do_rebuild = false;
			}
			if (!(cam != CameraController.MainCamera) || !(cam.name != "SceneCamera"))
			{
				if (!(cam == null))
				{
					LinesBatching.LineInfo lineInfo = this.line_info;
					if (((lineInfo != null) ? lineInfo.compute_buffer : null) != null)
					{
						if (!(this.cs_cull == null) && this.cs_kernel_cull >= 0)
						{
							this.CalcFrustrum(cam);
							this.cam_pos = cam.transform.position;
							this.cs_cull.SetVectorArray("f_planes", LinesBatching.f_planes);
							this.cs_cull.SetVector("cam_pos", this.cam_pos);
							this.cs_cull.SetFloat("cull_dist", this.cull_dist);
							uint num;
							uint num2;
							uint num3;
							this.cs_cull.GetKernelThreadGroupSizes(this.cs_kernel_cull, out num, out num2, out num3);
							this.cs_cull.SetBuffer(this.cs_kernel_cull, "position_buffer", this.line_info.compute_buffer);
							this.cs_cull.SetBuffer(this.cs_kernel_cull, "shader_buffer", this.line_info.shader_buffer);
							this.line_info.shader_buffer.SetCounterValue(0U);
							int threadGroupsX = Mathf.CeilToInt((float)this.line_info.count / num);
							this.cs_cull.Dispatch(this.cs_kernel_cull, threadGroupsX, 1, 1);
							ComputeBuffer.CopyCount(this.line_info.shader_buffer, this.line_info.arg_buf, 4);
							if (this.Debugging)
							{
								int[] array = new int[5];
								this.line_info.arg_buf.GetData(array);
								Debug.Log(string.Format("{0} - elements : {1} Camera : {2}", this, array[1], cam));
							}
							if (this.BitonicCS != null)
							{
								if (this.sorter == null)
								{
									this.sorter = new BitonicSorter();
									this.sorter.Init(this.BitonicCS);
								}
								int[] array2 = new int[5];
								this.line_info.arg_buf.GetData(array2);
								this.sorter.bitonicSortGPU(this.line_info.shader_buffer, array2[1]);
							}
							Graphics.DrawMeshInstancedIndirect(this.line_info.mesh, 0, this.line_info.material, this.map_bounds, this.line_info.arg_buf, 0, null, ShadowCastingMode.Off, true, this.layer, cam);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x000B0058 File Offset: 0x000AE258
	protected virtual void OnEnable()
	{
		this.Clear();
		this.do_rebuild = true;
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(this.Draw));
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x000B0088 File Offset: 0x000AE288
	public void Rebuild()
	{
		using (Game.Profile("LinesBatching.Rebuild", false, 0f, null))
		{
			if (this.terrain == null)
			{
				this.terrain = Terrain.activeTerrain;
			}
			if (this.terrain != null)
			{
				this.map_bounds = this.terrain.terrainData.bounds;
				this.map_bounds.center = this.map_bounds.center + this.terrain.transform.position;
				Vector3 size = this.map_bounds.size;
				size.y = 4000f;
				this.map_bounds.size = size;
			}
			else if (TitleMap.Get() != null)
			{
				this.map_bounds = TitleMap.Get().GetTerrainBounds();
			}
			this.BuildLinesData();
			if (this.cs_cull != null)
			{
				this.cs_kernel_cull = this.cs_cull.FindKernel("WVCullLines");
			}
		}
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x000B0198 File Offset: 0x000AE398
	protected virtual void HandleProjectChanged()
	{
		this.do_rebuild = true;
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x000B01A4 File Offset: 0x000AE3A4
	public bool LoadPoints(Game game = null, bool post_process = true)
	{
		bool result;
		using (Game.Profile("LinesBatching.LoadPoints", false, 0f, null))
		{
			if (game == null)
			{
				game = GameLogic.Get(false);
			}
			string text;
			if (game != null)
			{
				text = game.map_name;
			}
			else
			{
				text = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
			}
			string path = string.Concat(new string[]
			{
				Game.maps_path,
				text,
				"/",
				this.dir,
				".bin"
			});
			if (!File.Exists(path))
			{
				result = false;
			}
			else
			{
				byte[] array = File.ReadAllBytes(path);
				int num = array.Length / 4;
				this.pos_rots_binary = new List<float3>(num / 3);
				float[] array2 = Serialization.ToArray<float>(array, num);
				for (int i = 0; i < num; i += 3)
				{
					this.pos_rots_binary.Add(new float3(array2[i], array2[i + 1], array2[i + 2]));
				}
				result = (this.pos_rots_binary.Count > 0);
			}
		}
		return result;
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x000B02B8 File Offset: 0x000AE4B8
	public void MarkDirty(bool reload, bool refresh_buffers, bool instant = false, bool recreate_buffers = false)
	{
		base.enabled = true;
		this.needs_refresh = true;
		if (!this.reload)
		{
			this.reload = reload;
		}
		if (!this.recreate_buffers)
		{
			this.recreate_buffers = recreate_buffers;
		}
		if (!this.refill_buffers)
		{
			this.refill_buffers = refresh_buffers;
		}
		if (instant)
		{
			this.Refresh();
		}
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x000B030A File Offset: 0x000AE50A
	protected virtual void Update()
	{
		if (this.needs_refresh)
		{
			this.Refresh();
		}
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x000B031C File Offset: 0x000AE51C
	protected virtual void Refresh()
	{
		this.needs_refresh = false;
		base.enabled = true;
		using (Game.Profile("LinesBatching.Refresh", false, 0f, null))
		{
			if (this.reload)
			{
				this.Clear();
				this.LoadPoints(null, true);
				this.Rebuild();
			}
			if (this.recreate_buffers && this.line_info != null)
			{
				this.UpdateLinesData(this.line_info.shader_data, this.line_info.compute_data);
			}
			if (this.refill_buffers)
			{
				this.OnBuffersChanged();
			}
		}
		this.reload = false;
		this.refill_buffers = false;
		this.recreate_buffers = false;
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x000B03D8 File Offset: 0x000AE5D8
	protected virtual void BuildLinesData()
	{
		using (Game.Profile("LinesBatching.BuildLinesData", false, 0f, null))
		{
			if (!(this.mat == null))
			{
				if (this.pos_rots_binary != null)
				{
					int num = 0;
					this.shader_buff_stride = sizeof(LinesBatching.LineShaderData);
					this.compute_buff_stride = sizeof(LinesBatching.LineComputeData);
					Mesh mesh = new Mesh();
					using (Game.Profile("LinesBatching.BuildMesh", false, 0f, null))
					{
						mesh.vertices = new Vector3[]
						{
							new Vector3(-0.5f, 0f, -0.5f),
							new Vector3(0.5f, 0f, -0.5f),
							new Vector3(0.5f, 0f, 0.5f),
							new Vector3(-0.5f, 0f, 0.5f)
						};
						mesh.triangles = new int[]
						{
							0,
							2,
							1,
							2,
							0,
							3
						};
						mesh.uv = new Vector2[]
						{
							new Vector2(0f, 0f),
							new Vector2(1f, 0f),
							new Vector2(1f, 1f),
							new Vector2(0f, 1f)
						};
						mesh.RecalculateNormals();
						Vector4[] array = new Vector4[4];
						for (int i = 0; i < 4; i++)
						{
							array[i] = new Vector4(0f, 0f, 1f, -1f);
						}
						mesh.tangents = array;
					}
					if (this.line_info != null)
					{
						this.CleanUp(this.line_info, null);
					}
					this.line_info = this.FillLineTypeInfo(mesh, this.mat);
					this.line_info.count = this.pos_rots_binary.Count;
					this.total_count += this.line_info.count;
					using (Game.Profile("LinesBatching.AllocData", false, 0f, null))
					{
						this.line_info.shader_data = new LinesBatching.LineShaderData[this.line_info.count];
						this.line_info.compute_data = new LinesBatching.LineComputeData[this.line_info.count];
					}
					if (this.line_info.count > num)
					{
						num = this.line_info.count;
					}
					using (Game.Profile("LinesBatching.InitLineSegments", false, 0f, null))
					{
						this.InitLineSegments();
					}
					this.UpdateLinesData(this.line_info.shader_data, this.line_info.compute_data);
					this.line_info.shader_data = null;
				}
			}
		}
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x000B074C File Offset: 0x000AE94C
	protected virtual void InitLineSegments()
	{
		using (Game.Profile("LinesBatching.InitLineSegments", false, 0f, null))
		{
			this.segments.Clear();
			float num = 0f;
			int num2 = 0;
			LinesBatching.LineSegment item = default(LinesBatching.LineSegment);
			TerrainData terrainData = this.terrain.terrainData;
			new Vector2(1f / terrainData.size.x, 1f / terrainData.size.y);
			for (int i = 0; i < this.pos_rots_binary.Count; i++)
			{
				float3 @float = this.pos_rots_binary[i];
				int prev_idx = i - 1;
				if (i == num2)
				{
					item = default(LinesBatching.LineSegment);
					item.start_idx = num2;
					prev_idx = -1;
				}
				else if (@float.x == -1f)
				{
					for (int j = num2; j < i; j++)
					{
						LinesBatching.LineComputeData lineComputeData = this.line_info.compute_data[j];
						lineComputeData.uv = (float)(j - num2) / num;
						this.line_info.compute_data[j] = lineComputeData;
					}
					item.end_idx = i;
					this.segments.Add(item);
					num2 = i + 1;
					num = 0f;
				}
				float3 v = @float;
				LinesBatching.LineComputeData lineComputeData2 = new LinesBatching.LineComputeData(i, v, 0f, true, prev_idx, i + 1);
				this.line_info.compute_data[(int)this.line_info.buff_idx] = lineComputeData2;
				if (i > 0)
				{
					LinesBatching.LineComputeData lineComputeData3 = this.line_info.compute_data[(int)(this.line_info.buff_idx - 1U)];
					num += Vector3.Distance(lineComputeData3.pos, v);
					if (num > this.max_path_len)
					{
						num = this.max_path_len;
					}
				}
				this.line_info.buff_idx += 1U;
			}
		}
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x000B094C File Offset: 0x000AEB4C
	public virtual bool UpdateLinesData(LinesBatching.LineShaderData[] shader_data, LinesBatching.LineComputeData[] compute_data)
	{
		bool result;
		using (Game.Profile("LinesBatching.UpdateLinesData", false, 0f, null))
		{
			LinesBatching.LineInfo lineInfo = this.line_info;
			int count = lineInfo.count;
			int num = (shader_data == null) ? 0 : shader_data.Length;
			if (count != num || num == 0)
			{
				ComputeBuffer compute_buffer = lineInfo.compute_buffer;
				if (compute_buffer != null)
				{
					compute_buffer.Release();
				}
				lineInfo.compute_buffer = null;
				ComputeBuffer shader_buffer = lineInfo.shader_buffer;
				if (shader_buffer != null)
				{
					shader_buffer.Release();
				}
				lineInfo.shader_buffer = null;
			}
			if (!lineInfo.valid || lineInfo.count == 0)
			{
				result = false;
			}
			else
			{
				if (lineInfo.compute_buffer == null)
				{
					lineInfo.compute_buffer = new ComputeBuffer(lineInfo.count, this.compute_buff_stride);
					lineInfo.shader_buffer = new ComputeBuffer(lineInfo.count, this.shader_buff_stride, ComputeBufferType.Append);
				}
				this.OnBuffersChanged();
				lineInfo.material.SetBuffer("positionBuffer", lineInfo.shader_buffer);
				lineInfo.material.SetFloat("_Width", this.width);
				if (lineInfo.arg_buf == null)
				{
					lineInfo.arg_buf = new ComputeBuffer(1, this.args.Length * 4, ComputeBufferType.DrawIndirect);
				}
				this.args[0] = (int)lineInfo.mesh.GetIndexCount(0);
				this.args[1] = lineInfo.count;
				this.args[2] = (int)lineInfo.mesh.GetIndexStart(0);
				this.args[3] = (int)lineInfo.mesh.GetBaseVertex(0);
				this.args[4] = 0;
				lineInfo.arg_buf.SetData(this.args);
				lineInfo.material.SetInt("_SNAP_TO_TERRAIN", this.snap_to_terrain ? 1 : 0);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x000B0B0C File Offset: 0x000AED0C
	private LinesBatching.LineInfo FillLineTypeInfo(Mesh p_mesh, Material p_mat)
	{
		LinesBatching.LineInfo result;
		using (Game.Profile("LinesBatching.FillLineTypeInfo", false, 0f, null))
		{
			if (p_mat == null)
			{
				result = null;
			}
			else
			{
				LinesBatching.LineInfo lineInfo = new LinesBatching.LineInfo();
				lineInfo.count = 0;
				lineInfo.buff_idx = 0U;
				lineInfo.valid = false;
				Material material = UnityEngine.Object.Instantiate<Material>(p_mat);
				if (material.shader.name != this.required_shader)
				{
					material.shader = Shader.Find(this.required_shader);
				}
				if (this.render_queue != -1)
				{
					material.renderQueue = this.render_queue;
				}
				lineInfo.mesh = UnityEngine.Object.Instantiate<Mesh>(p_mesh);
				Bounds bounds = lineInfo.mesh.bounds;
				lineInfo.radius = Mathf.Max(bounds.extents[0], Mathf.Max(bounds.extents[1], bounds.extents[2]));
				lineInfo.material = material;
				lineInfo.sub_mesh_idx = 0;
				lineInfo.valid = true;
				result = lineInfo;
			}
		}
		return result;
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x000B0C34 File Offset: 0x000AEE34
	private void CleanUp(LinesBatching.LineInfo line, GameObject prefab_inst = null)
	{
		if (prefab_inst != null)
		{
			global::Common.DestroyObj(prefab_inst);
		}
		ComputeBuffer compute_buffer = line.compute_buffer;
		if (compute_buffer != null)
		{
			compute_buffer.Dispose();
		}
		line.compute_buffer = null;
		ComputeBuffer shader_buffer = line.shader_buffer;
		if (shader_buffer != null)
		{
			shader_buffer.Dispose();
		}
		line.shader_buffer = null;
		ComputeBuffer arg_buf = line.arg_buf;
		if (arg_buf != null)
		{
			arg_buf.Dispose();
		}
		line.arg_buf = null;
		global::Common.DestroyObj(line.mesh);
		global::Common.DestroyObj(line.material);
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x000B0CAE File Offset: 0x000AEEAE
	public void Clear()
	{
		if (this.line_info != null)
		{
			this.CleanUp(this.line_info, null);
			this.line_info = null;
		}
		this.total_count = 0;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x000B0CD3 File Offset: 0x000AEED3
	private void OnDestroy()
	{
		this.Clear();
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x000B0CDB File Offset: 0x000AEEDB
	private void OnDisable()
	{
		this.Clear();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(this.Draw));
	}

	// Token: 0x04000AE1 RID: 2785
	public Material mat;

	// Token: 0x04000AE2 RID: 2786
	[NonSerialized]
	public List<float3> pos_rots_binary;

	// Token: 0x04000AE3 RID: 2787
	public float width = 1f;

	// Token: 0x04000AE4 RID: 2788
	public string dir = "RoadsTrapezoidData";

	// Token: 0x04000AE5 RID: 2789
	public float max_path_len = 30f;

	// Token: 0x04000AE6 RID: 2790
	public string required_shader = "BSG/Instanced/LinesInstanced";

	// Token: 0x04000AE7 RID: 2791
	[HideInInspector]
	public int layer;

	// Token: 0x04000AE8 RID: 2792
	public bool snap_to_terrain = true;

	// Token: 0x04000AE9 RID: 2793
	[NonSerialized]
	public int render_queue = -1;

	// Token: 0x04000AEA RID: 2794
	private bool needs_refresh;

	// Token: 0x04000AEB RID: 2795
	private bool reload;

	// Token: 0x04000AEC RID: 2796
	private bool refill_buffers;

	// Token: 0x04000AED RID: 2797
	private bool recreate_buffers;

	// Token: 0x04000AEE RID: 2798
	private int shader_buff_stride;

	// Token: 0x04000AEF RID: 2799
	private int compute_buff_stride;

	// Token: 0x04000AF0 RID: 2800
	public LinesBatching.LineInfo line_info;

	// Token: 0x04000AF1 RID: 2801
	private int total_count;

	// Token: 0x04000AF2 RID: 2802
	private int[] args = new int[5];

	// Token: 0x04000AF3 RID: 2803
	public Terrain terrain;

	// Token: 0x04000AF4 RID: 2804
	public ComputeShader cs_cull;

	// Token: 0x04000AF5 RID: 2805
	private int cs_kernel_cull;

	// Token: 0x04000AF6 RID: 2806
	public ComputeShader BitonicCS;

	// Token: 0x04000AF7 RID: 2807
	private BitonicSorter sorter;

	// Token: 0x04000AF8 RID: 2808
	public float cull_dist = 300f;

	// Token: 0x04000AF9 RID: 2809
	[NonSerialized]
	public bool do_rebuild;

	// Token: 0x04000AFA RID: 2810
	private Vector3 cam_pos;

	// Token: 0x04000AFB RID: 2811
	private static Camera _last_cam;

	// Token: 0x04000AFC RID: 2812
	private static int _frustrum_last_frame;

	// Token: 0x04000AFD RID: 2813
	private static Plane[] frustum_planes = new Plane[6];

	// Token: 0x04000AFE RID: 2814
	private static Vector4[] f_planes = new Vector4[6];

	// Token: 0x04000AFF RID: 2815
	private Bounds map_bounds = new Bounds(new Vector3(2000f, 2000f, 2000f), new Vector3(4000f, 4000f, 4000f));

	// Token: 0x04000B00 RID: 2816
	public bool Debugging;

	// Token: 0x04000B01 RID: 2817
	public List<LinesBatching.LineSegment> segments = new List<LinesBatching.LineSegment>();

	// Token: 0x02000652 RID: 1618
	public struct LineShaderData
	{
		// Token: 0x06004767 RID: 18279 RVA: 0x002138AD File Offset: 0x00211AAD
		public LineShaderData(int id, float2 pos, float2 right, float2 pos_prev, float2 right_prev)
		{
			this.pos = pos;
			this.right = right;
			this.pos_prev = pos_prev;
			this.right_prev = right_prev;
			this.uv = 0;
		}

		// Token: 0x040034FB RID: 13563
		public float2 pos;

		// Token: 0x040034FC RID: 13564
		public float2 right;

		// Token: 0x040034FD RID: 13565
		public float2 pos_prev;

		// Token: 0x040034FE RID: 13566
		public float2 right_prev;

		// Token: 0x040034FF RID: 13567
		public float2 uv;
	}

	// Token: 0x02000653 RID: 1619
	public struct LineComputeData
	{
		// Token: 0x06004768 RID: 18280 RVA: 0x002138D9 File Offset: 0x00211AD9
		public LineComputeData(int id, Vector3 pos, float uv, bool enabled, int prev_idx, int next_idx)
		{
			this.pos = pos;
			this.uv = uv;
			this.enabled = (float)(enabled ? 1 : 0);
			this.prev_idx = prev_idx;
			this.next_idx = next_idx;
		}

		// Token: 0x04003500 RID: 13568
		public float3 pos;

		// Token: 0x04003501 RID: 13569
		public float uv;

		// Token: 0x04003502 RID: 13570
		public float enabled;

		// Token: 0x04003503 RID: 13571
		public int prev_idx;

		// Token: 0x04003504 RID: 13572
		public int next_idx;
	}

	// Token: 0x02000654 RID: 1620
	public class LineInfo
	{
		// Token: 0x04003505 RID: 13573
		public Mesh mesh;

		// Token: 0x04003506 RID: 13574
		public Material material;

		// Token: 0x04003507 RID: 13575
		public float radius;

		// Token: 0x04003508 RID: 13576
		public int sub_mesh_idx;

		// Token: 0x04003509 RID: 13577
		public bool valid;

		// Token: 0x0400350A RID: 13578
		public LinesBatching.LineComputeData[] compute_data;

		// Token: 0x0400350B RID: 13579
		public LinesBatching.LineShaderData[] shader_data;

		// Token: 0x0400350C RID: 13580
		public int count;

		// Token: 0x0400350D RID: 13581
		public uint buff_idx;

		// Token: 0x0400350E RID: 13582
		public ComputeBuffer compute_buffer;

		// Token: 0x0400350F RID: 13583
		public ComputeBuffer shader_buffer;

		// Token: 0x04003510 RID: 13584
		public ComputeBuffer arg_buf;
	}

	// Token: 0x02000655 RID: 1621
	public struct LineSegment
	{
		// Token: 0x0600476A RID: 18282 RVA: 0x00213910 File Offset: 0x00211B10
		public void SetActive(bool enabled, LinesBatching.LineComputeData[] arr)
		{
			for (int i = this.start_idx; i <= this.end_idx; i++)
			{
				LinesBatching.LineComputeData lineComputeData = arr[i];
				lineComputeData.enabled = (float)(enabled ? 1 : 0);
				arr[i] = lineComputeData;
			}
		}

		// Token: 0x04003511 RID: 13585
		public int start_idx;

		// Token: 0x04003512 RID: 13586
		public int end_idx;
	}
}
