using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000079 RID: 121
[RequireComponent(typeof(Camera))]
public class EmissionRenderer : MonoBehaviour
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000494 RID: 1172 RVA: 0x00035EC0 File Offset: 0x000340C0
	private Camera camera
	{
		get
		{
			return base.GetComponent<Camera>();
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x00035EC8 File Offset: 0x000340C8
	public static void DrawInstancedIndirect(Mesh mesh, int submesh_index, Material material, int shader_pass, ComputeBuffer buffer_with_args, int args_offset, MaterialPropertyBlock properties)
	{
		if (!EmissionRenderer.enable_rendering)
		{
			return;
		}
		EmissionRenderer.pending_draws.Add(new EmissionRenderer.DrawInstancedIndirectData(mesh, submesh_index, material, shader_pass, buffer_with_args, args_offset, properties));
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00035EEC File Offset: 0x000340EC
	public static void AddRenderer(Renderer renderer)
	{
		int num = renderer.sharedMaterial.FindPass("Emission");
		if (num >= 0)
		{
			EmissionRenderer.emission_renderers.Add(new EmissionRenderer.DrawMeshData(renderer, num));
			return;
		}
		Debug.LogWarning("Added MeshRenderer does not contain Emission pass", renderer);
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00035F2C File Offset: 0x0003412C
	private void UpdateRenderTexture()
	{
		if (EmissionRenderer.emission_rt == null)
		{
			EmissionRenderer.emission_rt = this.CreateEmissionRT();
			Shader.SetGlobalTexture(EmissionRenderer.emission_rt.name, EmissionRenderer.emission_rt);
			return;
		}
		if (this.camera.pixelWidth != EmissionRenderer.emission_rt.width || this.camera.pixelHeight != EmissionRenderer.emission_rt.height)
		{
			EmissionRenderer.emission_rt.Release();
			EmissionRenderer.emission_rt = this.CreateEmissionRT();
			Shader.SetGlobalTexture(EmissionRenderer.emission_rt.name, EmissionRenderer.emission_rt);
		}
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00035FBD File Offset: 0x000341BD
	private RenderTexture CreateEmissionRT()
	{
		return new RenderTexture(this.camera.pixelWidth, this.camera.pixelHeight, 0, RenderTextureFormat.ARGB32, 0)
		{
			name = "_EmissionRendererRT"
		};
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x00035FE8 File Offset: 0x000341E8
	private void OnEnable()
	{
		if (EmissionRenderer.cmd == null)
		{
			EmissionRenderer.cmd = new CommandBuffer();
		}
		EmissionRenderer.cached_properties = new MaterialPropertyBlock();
		EmissionRenderer.cmd.name = "Emission Renderer";
		this.UpdateRenderTexture();
		this.camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, EmissionRenderer.cmd);
		Shader.EnableKeyword("EMISSION_RENDERER_ENABLED");
		EmissionRenderer.Instance = this;
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00036048 File Offset: 0x00034248
	private void OnDisable()
	{
		this.camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, EmissionRenderer.cmd);
		Shader.DisableKeyword("EMISSION_RENDERER_ENABLED");
		if (EmissionRenderer.emission_rt != null)
		{
			EmissionRenderer.emission_rt.Release();
			EmissionRenderer.emission_rt = null;
		}
		if (EmissionRenderer.Instance == this)
		{
			EmissionRenderer.Instance = null;
			return;
		}
		Debug.LogError("More than one EmissionRenderer is used!");
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x000360AC File Offset: 0x000342AC
	private void OnPreCull()
	{
		if (!EmissionRenderer.enable_rendering)
		{
			return;
		}
		EmissionRenderer.cmd.Clear();
		this.UpdateRenderTexture();
		EmissionRenderer.cmd.SetViewProjectionMatrices(this.camera.worldToCameraMatrix, this.camera.projectionMatrix);
		EmissionRenderer.cmd.SetRenderTarget(EmissionRenderer.emission_rt);
		EmissionRenderer.cmd.ClearRenderTarget(true, true, new Color(0f, 0f, 0f, 0f));
		for (int i = 0; i < EmissionRenderer.pending_draws.Count; i++)
		{
			EmissionRenderer.DrawInstancedIndirectData drawInstancedIndirectData = EmissionRenderer.pending_draws[i];
			EmissionRenderer.cmd.DrawMeshInstancedIndirect(drawInstancedIndirectData.mesh, drawInstancedIndirectData.submesh_index, drawInstancedIndirectData.material, drawInstancedIndirectData.shader_pass, drawInstancedIndirectData.buffer_with_args, drawInstancedIndirectData.args_offset, drawInstancedIndirectData.properties);
		}
		for (int j = EmissionRenderer.emission_renderers.Count - 1; j >= 0; j--)
		{
			EmissionRenderer.DrawMeshData drawMeshData = EmissionRenderer.emission_renderers[j];
			if (drawMeshData.renderer == null)
			{
				EmissionRenderer.emission_renderers.RemoveAt(j);
			}
			else if (drawMeshData.renderer.enabled && drawMeshData.renderer.gameObject.activeInHierarchy && drawMeshData.renderer.enabled)
			{
				Mesh sharedMesh = drawMeshData.renderer.GetComponent<MeshFilter>().sharedMesh;
				if (sharedMesh != null)
				{
					EmissionRenderer.cached_properties.Clear();
					drawMeshData.renderer.GetPropertyBlock(EmissionRenderer.cached_properties);
					EmissionRenderer.cmd.DrawMesh(sharedMesh, drawMeshData.renderer.transform.localToWorldMatrix, drawMeshData.renderer.sharedMaterial, 0, drawMeshData.shader_pass_index, EmissionRenderer.cached_properties);
				}
			}
		}
		EmissionRenderer.pending_draws.Clear();
	}

	// Token: 0x0400047B RID: 1147
	private const string EMISSION_RENDERER_KEYWORD = "EMISSION_RENDERER_ENABLED";

	// Token: 0x0400047C RID: 1148
	private const string COMMAND_BUFFER_NAME = "Emission Renderer";

	// Token: 0x0400047D RID: 1149
	private const string TEXTURE_NAME = "_EmissionRendererRT";

	// Token: 0x0400047E RID: 1150
	public static EmissionRenderer Instance;

	// Token: 0x0400047F RID: 1151
	public static bool enable_rendering = true;

	// Token: 0x04000480 RID: 1152
	private static CommandBuffer cmd;

	// Token: 0x04000481 RID: 1153
	private static RenderTexture emission_rt;

	// Token: 0x04000482 RID: 1154
	private static List<EmissionRenderer.DrawInstancedIndirectData> pending_draws = new List<EmissionRenderer.DrawInstancedIndirectData>();

	// Token: 0x04000483 RID: 1155
	private static List<EmissionRenderer.DrawMeshData> emission_renderers = new List<EmissionRenderer.DrawMeshData>();

	// Token: 0x04000484 RID: 1156
	private static MaterialPropertyBlock cached_properties;

	// Token: 0x02000545 RID: 1349
	private struct DrawInstancedIndirectData
	{
		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06004378 RID: 17272 RVA: 0x001FD80B File Offset: 0x001FBA0B
		// (set) Token: 0x06004379 RID: 17273 RVA: 0x001FD813 File Offset: 0x001FBA13
		public MaterialPropertyBlock properties { get; private set; }

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x0600437A RID: 17274 RVA: 0x001FD81C File Offset: 0x001FBA1C
		// (set) Token: 0x0600437B RID: 17275 RVA: 0x001FD824 File Offset: 0x001FBA24
		public int args_offset { get; private set; }

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x0600437C RID: 17276 RVA: 0x001FD82D File Offset: 0x001FBA2D
		// (set) Token: 0x0600437D RID: 17277 RVA: 0x001FD835 File Offset: 0x001FBA35
		public ComputeBuffer buffer_with_args { get; private set; }

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x0600437E RID: 17278 RVA: 0x001FD83E File Offset: 0x001FBA3E
		// (set) Token: 0x0600437F RID: 17279 RVA: 0x001FD846 File Offset: 0x001FBA46
		public int shader_pass { get; private set; }

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06004380 RID: 17280 RVA: 0x001FD84F File Offset: 0x001FBA4F
		// (set) Token: 0x06004381 RID: 17281 RVA: 0x001FD857 File Offset: 0x001FBA57
		public Material material { get; private set; }

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06004382 RID: 17282 RVA: 0x001FD860 File Offset: 0x001FBA60
		// (set) Token: 0x06004383 RID: 17283 RVA: 0x001FD868 File Offset: 0x001FBA68
		public int submesh_index { get; private set; }

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06004384 RID: 17284 RVA: 0x001FD871 File Offset: 0x001FBA71
		// (set) Token: 0x06004385 RID: 17285 RVA: 0x001FD879 File Offset: 0x001FBA79
		public Mesh mesh { get; private set; }

		// Token: 0x06004386 RID: 17286 RVA: 0x001FD882 File Offset: 0x001FBA82
		public DrawInstancedIndirectData(Mesh mesh, int submesh_index, Material material, int shader_pass, ComputeBuffer buffer_with_args, int args_offset, MaterialPropertyBlock properties)
		{
			this.properties = properties;
			this.args_offset = args_offset;
			this.buffer_with_args = buffer_with_args;
			this.shader_pass = shader_pass;
			this.material = material;
			this.submesh_index = submesh_index;
			this.mesh = mesh;
		}
	}

	// Token: 0x02000546 RID: 1350
	public struct DrawMeshData
	{
		// Token: 0x06004387 RID: 17287 RVA: 0x001FD8B9 File Offset: 0x001FBAB9
		public DrawMeshData(Renderer renderer, int shader_pass_index)
		{
			this.renderer = renderer;
			this.shader_pass_index = shader_pass_index;
		}

		// Token: 0x04002FB1 RID: 12209
		public Renderer renderer;

		// Token: 0x04002FB2 RID: 12210
		public int shader_pass_index;
	}
}
