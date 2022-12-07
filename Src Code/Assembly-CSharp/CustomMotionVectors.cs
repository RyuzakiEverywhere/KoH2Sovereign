using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000111 RID: 273
[RequireComponent(typeof(Camera))]
public class CustomMotionVectors : MonoBehaviour
{
	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000C85 RID: 3205 RVA: 0x0008B447 File Offset: 0x00089647
	private static CustomMotionVectors Instance
	{
		get
		{
			if (CustomMotionVectors.instances.Count <= 0)
			{
				return null;
			}
			return CustomMotionVectors.instances[0];
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000C86 RID: 3206 RVA: 0x00035EC0 File Offset: 0x000340C0
	private Camera Camera
	{
		get
		{
			return base.GetComponent<Camera>();
		}
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x0008B464 File Offset: 0x00089664
	private void OnEnable()
	{
		CustomMotionVectors.instances.Add(this);
		if (this.cmd == null)
		{
			this.cmd = new CommandBuffer();
			this.cmd.name = base.gameObject.name + " - CustomMotionVectors";
		}
		this.Camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.cmd);
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x0008B4C2 File Offset: 0x000896C2
	private void OnDisable()
	{
		CustomMotionVectors.instances.Remove(this);
		this.Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.cmd);
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0008B4E4 File Offset: 0x000896E4
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock materialPropertyBlock)
	{
		if (CustomMotionVectors.Instance == null)
		{
			return;
		}
		CustomMotionVectors.InstancedIndirectDrawData item = new CustomMotionVectors.InstancedIndirectDrawData(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, materialPropertyBlock);
		CustomMotionVectors.Instance.instancedIndirectQueue.Enqueue(item);
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0008B520 File Offset: 0x00089720
	public static void DrawRenderer(Renderer renderer, Matrix4x4 previous_local_to_world_matrix, Material material, int submesh_index, int shader_pass)
	{
		if (CustomMotionVectors.Instance == null)
		{
			return;
		}
		CustomMotionVectors.RendererDrawData item = new CustomMotionVectors.RendererDrawData(renderer, previous_local_to_world_matrix, material, submesh_index, shader_pass);
		CustomMotionVectors.Instance.rendererQueue.Enqueue(item);
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0008B558 File Offset: 0x00089758
	private void LateUpdate()
	{
		this.cmd.Clear();
		if (!this.Camera.depthTextureMode.HasFlag(DepthTextureMode.MotionVectors))
		{
			return;
		}
		this.previous_camera_local_to_world = this.camera_local_to_world;
		this.camera_local_to_world = this.Camera.transform.localToWorldMatrix;
		this.cmd.SetRenderTarget(BuiltinRenderTextureType.MotionVectors, BuiltinRenderTextureType.CameraTarget);
		this.cmd.SetGlobalVector("_CameraMotionVectorWS", this.camera_local_to_world.GetColumn(3) - this.previous_camera_local_to_world.GetColumn(3));
		while (this.instancedIndirectQueue.Count > 0)
		{
			CustomMotionVectors.InstancedIndirectDrawData instancedIndirectDrawData = this.instancedIndirectQueue.Dequeue();
			if (!(instancedIndirectDrawData.mesh == null) && !(instancedIndirectDrawData.material == null))
			{
				this.cmd.DrawMeshInstancedIndirect(instancedIndirectDrawData.mesh, instancedIndirectDrawData.submeshIndex, instancedIndirectDrawData.material, instancedIndirectDrawData.shaderPass, instancedIndirectDrawData.bufferWithArgs, instancedIndirectDrawData.argsOffset, instancedIndirectDrawData.materialPropertyBlock);
			}
		}
		while (this.rendererQueue.Count > 0)
		{
			CustomMotionVectors.RendererDrawData rendererDrawData = this.rendererQueue.Dequeue();
			if (!(rendererDrawData.renderer == null) && !(rendererDrawData.material == null))
			{
				this.cmd.SetGlobalMatrix("_PreviousM", rendererDrawData.previous_local_to_world_matrix);
				this.cmd.DrawRenderer(rendererDrawData.renderer, rendererDrawData.material, rendererDrawData.submesh_index, rendererDrawData.shader_pass);
			}
		}
	}

	// Token: 0x040009B5 RID: 2485
	private static List<CustomMotionVectors> instances = new List<CustomMotionVectors>();

	// Token: 0x040009B6 RID: 2486
	private Queue<CustomMotionVectors.InstancedIndirectDrawData> instancedIndirectQueue = new Queue<CustomMotionVectors.InstancedIndirectDrawData>();

	// Token: 0x040009B7 RID: 2487
	private Queue<CustomMotionVectors.RendererDrawData> rendererQueue = new Queue<CustomMotionVectors.RendererDrawData>();

	// Token: 0x040009B8 RID: 2488
	private CommandBuffer cmd;

	// Token: 0x040009B9 RID: 2489
	private Matrix4x4 camera_local_to_world;

	// Token: 0x040009BA RID: 2490
	private Matrix4x4 previous_camera_local_to_world;

	// Token: 0x0200060F RID: 1551
	private struct InstancedIndirectDrawData
	{
		// Token: 0x060046B8 RID: 18104 RVA: 0x0020FB7A File Offset: 0x0020DD7A
		public InstancedIndirectDrawData(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock materialPropertyBlock)
		{
			this.mesh = mesh;
			this.submeshIndex = submeshIndex;
			this.material = material;
			this.shaderPass = shaderPass;
			this.bufferWithArgs = bufferWithArgs;
			this.argsOffset = argsOffset;
			this.materialPropertyBlock = materialPropertyBlock;
		}

		// Token: 0x040033A5 RID: 13221
		public Mesh mesh;

		// Token: 0x040033A6 RID: 13222
		public int submeshIndex;

		// Token: 0x040033A7 RID: 13223
		public Material material;

		// Token: 0x040033A8 RID: 13224
		public int shaderPass;

		// Token: 0x040033A9 RID: 13225
		public ComputeBuffer bufferWithArgs;

		// Token: 0x040033AA RID: 13226
		public int argsOffset;

		// Token: 0x040033AB RID: 13227
		public MaterialPropertyBlock materialPropertyBlock;
	}

	// Token: 0x02000610 RID: 1552
	private struct RendererDrawData
	{
		// Token: 0x060046B9 RID: 18105 RVA: 0x0020FBB1 File Offset: 0x0020DDB1
		public RendererDrawData(Renderer renderer, Matrix4x4 previous_local_to_world_matrix, Material material, int submesh_index, int shader_pass)
		{
			this.renderer = renderer;
			this.previous_local_to_world_matrix = previous_local_to_world_matrix;
			this.material = material;
			this.submesh_index = submesh_index;
			this.shader_pass = shader_pass;
		}

		// Token: 0x040033AC RID: 13228
		public Renderer renderer;

		// Token: 0x040033AD RID: 13229
		public Matrix4x4 previous_local_to_world_matrix;

		// Token: 0x040033AE RID: 13230
		public Material material;

		// Token: 0x040033AF RID: 13231
		public int submesh_index;

		// Token: 0x040033B0 RID: 13232
		public int shader_pass;
	}
}
