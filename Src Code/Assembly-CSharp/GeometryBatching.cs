using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000132 RID: 306
public class GeometryBatching : MonoBehaviour
{
	// Token: 0x06001056 RID: 4182 RVA: 0x000AD3E4 File Offset: 0x000AB5E4
	private void Start()
	{
		this.buf_stride = sizeof(GeometryBatching.ModelShaderData);
		if (this.camera == null)
		{
			Debug.LogError("Set main game camera for GeometryBatcher(s) to work...");
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.terrain == null)
		{
			Debug.LogError("Set terrain to get bounds from...");
			return;
		}
		this.map_bounds = this.terrain.terrainData.bounds;
		this.map_bounds.center = this.map_bounds.center + this.terrain.transform.position;
		this.cs_cull = (ComputeShader)Resources.Load("Compute\\GeomCulling");
		if (this.cs_cull == null)
		{
			Debug.LogError("Missing compute shader \"GeomCulling\" in folder \"resources\\compute\"...");
			return;
		}
		this.cs_kernel_cull = this.cs_cull.FindKernel("BSGGeometryCull");
		if (this.cs_kernel_cull == -1)
		{
			Debug.LogError("Missing kernel \"BSGGeometryCull\" in compute shader \"GeomCulling\"...");
			return;
		}
		using (Game.Profile("Geometry batching collecting info...", true, 0f, null))
		{
			this.BuildData();
		}
		if (this.totalCount > 0)
		{
			Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(this.DrawModels));
		}
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x000AD52C File Offset: 0x000AB72C
	private void SetRenderersState(List<Renderer> renderers, bool state)
	{
		int count = renderers.Count;
		for (int i = 0; i < count; i++)
		{
			renderers[i].enabled = state;
		}
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x000AD55C File Offset: 0x000AB75C
	private void UpdateSmallBatchesAndCalcTotals()
	{
		this.modelCount = this.models.Count;
		for (int i = 0; i < this.modelCount; i++)
		{
			int count = this.models[i].data.Count;
			this.totalCount += count;
			if (this.maxInstances < count)
			{
				this.maxInstances = count;
			}
			if (this.minInstances > count)
			{
				this.minInstances = count;
			}
			if (count < this.minBatches)
			{
				this.models[i].too_few = this.minBatches;
				this.SetRenderersState(this.models[i].renderers, true);
				this.tooFewInstances++;
			}
			else
			{
				this.totalActiveCount += count;
				if (this.maxActiveInstances < count)
				{
					this.maxActiveInstances = count;
				}
				if (this.minActiveInstances > count)
				{
					this.minActiveInstances = count;
				}
			}
		}
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x000AD64C File Offset: 0x000AB84C
	public void BuildData()
	{
		this.meshes = new Dictionary<Mesh, GeometryBatching.ModelInfo>();
		this.models = new List<GeometryBatching.ModelInfo>();
		this.CollectNodeInfo(base.transform);
		this.UpdateSmallBatchesAndCalcTotals();
		if (this.totalCount == 0)
		{
			Debug.LogError("Geometry Batcher didn't find any instances to bach! What are you doing??");
			return;
		}
		this.BuildBuffers();
		this.meshes = null;
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x000AD6A4 File Offset: 0x000AB8A4
	private void BuildBuffers()
	{
		int count = this.models.Count;
		for (int i = 0; i < count; i++)
		{
			GeometryBatching.ModelInfo modelInfo = this.models[i];
			modelInfo.models_buf = new ComputeBuffer(modelInfo.data.Count, this.buf_stride);
			modelInfo.models_buf.SetData<GeometryBatching.ModelShaderData>(modelInfo.data);
			modelInfo.draw_buf = new ComputeBuffer(modelInfo.data.Count, this.buf_stride, ComputeBufferType.Append);
			modelInfo.arg_buf = new ComputeBuffer(1, this.args.Length * 4, ComputeBufferType.DrawIndirect);
			this.args[0] = (int)modelInfo.mesh.GetIndexCount(0);
			this.args[1] = modelInfo.data.Count;
			this.args[2] = (int)modelInfo.mesh.GetIndexStart(0);
			this.args[3] = (int)modelInfo.mesh.GetBaseVertex(0);
			this.args[4] = 0;
			modelInfo.arg_buf.SetData(this.args);
			modelInfo.material.SetBuffer("draw_buff", modelInfo.draw_buf);
		}
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x000AD7C0 File Offset: 0x000AB9C0
	private void CollectNodeInfo(Transform t)
	{
		this.TryAddModelInfo(t);
		int childCount = t.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = t.GetChild(i);
			this.CollectNodeInfo(child);
		}
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x000AD7F8 File Offset: 0x000AB9F8
	private void TryAddModelInfo(Transform t)
	{
		GameObject gameObject = t.gameObject;
		MeshFilter component = gameObject.GetComponent<MeshFilter>();
		if (component == null)
		{
			return;
		}
		if (component.sharedMesh == null)
		{
			return;
		}
		MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
		if (component2 == null)
		{
			return;
		}
		Material sharedMaterial = component2.sharedMaterial;
		if (sharedMaterial == null)
		{
			return;
		}
		Shader shader = sharedMaterial.shader;
		if (shader == null)
		{
			return;
		}
		if (!this.ShaderNeedsInstancing(shader))
		{
			return;
		}
		GeometryBatching.ModelInfo modelInfo;
		if (!this.meshes.TryGetValue(component.sharedMesh, out modelInfo))
		{
			modelInfo = new GeometryBatching.ModelInfo();
			modelInfo.mesh = component.sharedMesh;
			modelInfo.material = new Material(component2.sharedMaterial);
			modelInfo.data = new List<GeometryBatching.ModelShaderData>();
			modelInfo.renderers = new List<Renderer>();
			modelInfo.sub_mesh_idx = 0;
			modelInfo.mpb = new MaterialPropertyBlock();
			this.meshes.Add(component.sharedMesh, modelInfo);
			this.models.Add(modelInfo);
		}
		GeometryBatching.ModelShaderData item = new GeometryBatching.ModelShaderData(component2.localToWorldMatrix, component2.localToWorldMatrix.inverse, 10f);
		item.o2w.m33 = component2.bounds.extents.magnitude;
		modelInfo.data.Add(item);
		modelInfo.renderers.Add(component2);
		component2.enabled = false;
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x000AD95C File Offset: 0x000ABB5C
	private bool ShaderNeedsInstancing(Shader shader)
	{
		int count = this.shaders.Count;
		for (int i = 0; i < count; i++)
		{
			if (shader.name == this.shaders[i].name)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x000AD9A4 File Offset: 0x000ABBA4
	public void OnDestroy()
	{
		if (this.meshes != null)
		{
			this.meshes = null;
		}
		if (this.models == null)
		{
			return;
		}
		for (int i = 0; i < this.models.Count; i++)
		{
			GeometryBatching.ModelInfo modelInfo = this.models[i];
			ComputeBuffer arg_buf = modelInfo.arg_buf;
			if (arg_buf != null)
			{
				arg_buf.Release();
			}
			ComputeBuffer models_buf = modelInfo.models_buf;
			if (models_buf != null)
			{
				models_buf.Release();
			}
			ComputeBuffer draw_buf = modelInfo.draw_buf;
			if (draw_buf != null)
			{
				draw_buf.Release();
			}
			modelInfo.renderers = null;
		}
		this.models = null;
		if (this.totalCount > 0)
		{
			Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(this.DrawModels));
		}
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x000ADA54 File Offset: 0x000ABC54
	private void DrawModels(Camera cam)
	{
		if (cam != CameraController.MainCamera)
		{
			return;
		}
		if (cam.name == "SceneCamera")
		{
			return;
		}
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
		this.cs_cull.SetVectorArray("f_planes", this.f_planes);
		uint num;
		uint num2;
		uint num3;
		this.cs_cull.GetKernelThreadGroupSizes(this.cs_kernel_cull, out num, out num2, out num3);
		int count = this.models.Count;
		for (int j = 0; j < count; j++)
		{
			GeometryBatching.ModelInfo modelInfo = this.models[j];
			if (modelInfo.too_few <= 0 && (this.drawOnlyThis < 0 || j == this.drawOnlyThis))
			{
				this.cs_cull.SetBuffer(this.cs_kernel_cull, "models_buff", modelInfo.models_buf);
				this.cs_cull.SetBuffer(this.cs_kernel_cull, "draw_buff", modelInfo.draw_buf);
				modelInfo.draw_buf.SetCounterValue(0U);
				modelInfo.mpb.SetFloat("_Bla", (float)j);
				int threadGroupsX = Mathf.CeilToInt((float)modelInfo.data.Count / num);
				this.cs_cull.Dispatch(this.cs_kernel_cull, threadGroupsX, 1, 1);
				ComputeBuffer.CopyCount(modelInfo.draw_buf, modelInfo.arg_buf, 4);
				Graphics.DrawMeshInstancedIndirect(modelInfo.mesh, modelInfo.sub_mesh_idx, modelInfo.material, this.map_bounds, modelInfo.arg_buf, 0, modelInfo.mpb, ShadowCastingMode.On, true, LayerMask.NameToLayer("Settlements"), cam, LightProbeUsage.Off);
			}
		}
	}

	// Token: 0x04000AB9 RID: 2745
	private int buf_stride;

	// Token: 0x04000ABA RID: 2746
	private List<GeometryBatching.ModelInfo> models;

	// Token: 0x04000ABB RID: 2747
	private int[] args = new int[5];

	// Token: 0x04000ABC RID: 2748
	public Camera camera;

	// Token: 0x04000ABD RID: 2749
	public Terrain terrain;

	// Token: 0x04000ABE RID: 2750
	private Bounds map_bounds;

	// Token: 0x04000ABF RID: 2751
	public int minBatches = 16;

	// Token: 0x04000AC0 RID: 2752
	public List<Shader> shaders;

	// Token: 0x04000AC1 RID: 2753
	private Dictionary<Mesh, GeometryBatching.ModelInfo> meshes;

	// Token: 0x04000AC2 RID: 2754
	public int modelCount;

	// Token: 0x04000AC3 RID: 2755
	public int totalCount;

	// Token: 0x04000AC4 RID: 2756
	private int minInstances = int.MaxValue;

	// Token: 0x04000AC5 RID: 2757
	public int maxInstances;

	// Token: 0x04000AC6 RID: 2758
	public int totalActiveCount;

	// Token: 0x04000AC7 RID: 2759
	public int minActiveInstances = int.MaxValue;

	// Token: 0x04000AC8 RID: 2760
	public int maxActiveInstances;

	// Token: 0x04000AC9 RID: 2761
	public int tooFewInstances;

	// Token: 0x04000ACA RID: 2762
	private Plane[] frustum_planes = new Plane[6];

	// Token: 0x04000ACB RID: 2763
	private Vector4[] f_planes = new Vector4[6];

	// Token: 0x04000ACC RID: 2764
	private ComputeShader cs_cull;

	// Token: 0x04000ACD RID: 2765
	private int cs_kernel_cull;

	// Token: 0x04000ACE RID: 2766
	public int drawOnlyThis = -1;

	// Token: 0x0200064C RID: 1612
	public struct ModelShaderData
	{
		// Token: 0x06004761 RID: 18273 RVA: 0x0021380E File Offset: 0x00211A0E
		public ModelShaderData(Matrix4x4 o2w, Matrix4x4 w2o, float radius)
		{
			this.o2w = o2w;
			this.w2o = w2o;
		}

		// Token: 0x040034DB RID: 13531
		public Matrix4x4 o2w;

		// Token: 0x040034DC RID: 13532
		public Matrix4x4 w2o;
	}

	// Token: 0x0200064D RID: 1613
	public class ModelInfo
	{
		// Token: 0x040034DD RID: 13533
		public Mesh mesh;

		// Token: 0x040034DE RID: 13534
		public Material material;

		// Token: 0x040034DF RID: 13535
		public List<GeometryBatching.ModelShaderData> data;

		// Token: 0x040034E0 RID: 13536
		public List<Renderer> renderers;

		// Token: 0x040034E1 RID: 13537
		public int too_few;

		// Token: 0x040034E2 RID: 13538
		public ComputeBuffer draw_buf;

		// Token: 0x040034E3 RID: 13539
		public ComputeBuffer arg_buf;

		// Token: 0x040034E4 RID: 13540
		public ComputeBuffer models_buf;

		// Token: 0x040034E5 RID: 13541
		public MaterialPropertyBlock mpb;

		// Token: 0x040034E6 RID: 13542
		public int sub_mesh_idx;
	}
}
