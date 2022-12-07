using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

// Token: 0x0200004A RID: 74
[RequireComponent(typeof(Camera))]
public class GPUBenchmark : MonoBehaviour
{
	// Token: 0x17000015 RID: 21
	// (get) Token: 0x060001C8 RID: 456 RVA: 0x0001DC66 File Offset: 0x0001BE66
	public float last_result_ms
	{
		get
		{
			if (this.gathered_metrics.Count <= 0)
			{
				return -1f;
			}
			return this.gathered_metrics.Average();
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x060001C9 RID: 457 RVA: 0x0001DC87 File Offset: 0x0001BE87
	public float rating_0_10
	{
		get
		{
			return Mathf.Clamp(this.settings.GPURatingCurve.Evaluate(this.last_result_ms), 0f, 10f);
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x060001CA RID: 458 RVA: 0x0001DCAE File Offset: 0x0001BEAE
	public RenderTexture created_texture
	{
		get
		{
			return this.render_texture;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x060001CB RID: 459 RVA: 0x0001DCB6 File Offset: 0x0001BEB6
	private static List<Matrix4x4[]> batches
	{
		get
		{
			if (GPUBenchmark._batches == null)
			{
				return GPUBenchmark._batches = GPUBenchmark.GenerateDeterministicBatches();
			}
			return GPUBenchmark._batches;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x060001CC RID: 460 RVA: 0x0001DCD0 File Offset: 0x0001BED0
	private static MaterialPropertyBlock property_block
	{
		get
		{
			if (GPUBenchmark._property_block == null)
			{
				return GPUBenchmark._property_block = new MaterialPropertyBlock();
			}
			return GPUBenchmark._property_block;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060001CD RID: 461 RVA: 0x0001DCEA File Offset: 0x0001BEEA
	public static Mesh fullscreen_mesh
	{
		get
		{
			if (!(GPUBenchmark._fullscreen_mesh != null))
			{
				return GPUBenchmark._fullscreen_mesh = GPUBenchmark.CreateFullscreenMesh();
			}
			return GPUBenchmark._fullscreen_mesh;
		}
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0001DD0A File Offset: 0x0001BF0A
	[ContextMenu("Run")]
	private void _Run()
	{
		this.Run(null);
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0001DD14 File Offset: 0x0001BF14
	private void OnPostRender()
	{
		if (this.is_test_running)
		{
			if (this.test_index >= 10)
			{
				this.gathered_metrics.Add(this.RunSingleBatch(this.render_texture, null, null));
			}
			else
			{
				this.RunSingleBatch(this.render_texture, null, null);
			}
			this.test_index++;
			if (this.test_index >= 60)
			{
				this.is_test_running = false;
				this.average_test_time = this.last_result_ms;
				Action<GPUBenchmark> action = this.on_complete_action;
				if (action == null)
				{
					return;
				}
				action(this);
			}
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0001DD9C File Offset: 0x0001BF9C
	public void Run(Action<GPUBenchmark> on_complete = null)
	{
		if (this.render_texture == null)
		{
			this.render_texture = new RenderTexture(Screen.width, Screen.height, 32, GraphicsFormat.R16G16B16A16_SFloat, 0);
			this.render_texture.Create();
		}
		if (this.is_test_running)
		{
			this.on_complete_action = (Action<GPUBenchmark>)Delegate.Combine(this.on_complete_action, on_complete);
			return;
		}
		this.on_complete_action = on_complete;
		this.gathered_metrics.Clear();
		this.is_test_running = true;
		this.test_index = 0;
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0001DE20 File Offset: 0x0001C020
	private float RunSingleBatch(RenderTexture target_texture, RenderTexture source, RenderTexture destination)
	{
		CommandBuffer commandBuffer = new CommandBuffer();
		commandBuffer.name = "GPU Benchmark";
		RenderTextureDescriptor descriptor = new RenderTextureDescriptor(1024, 1024, GraphicsFormat.R8G8B8A8_UNorm, 0, 0);
		int nameID = this.GenerateRandomTexture(commandBuffer, "_Texture1", descriptor);
		int nameID2 = this.GenerateRandomTexture(commandBuffer, "_Texture2", descriptor);
		int nameID3 = this.GenerateRandomTexture(commandBuffer, "_Texture3", descriptor);
		int nameID4 = this.GenerateRandomTexture(commandBuffer, "_Texture4", descriptor);
		int nameID5 = this.GenerateRandomTexture(commandBuffer, "_Texture5", descriptor);
		commandBuffer.SetRenderTarget(target_texture);
		commandBuffer.ClearRenderTarget(true, true, Color.white);
		commandBuffer.SetViewMatrix(Matrix4x4.Translate(new Vector3(0f, 0f, -12f)));
		commandBuffer.SetProjectionMatrix(Matrix4x4.Perspective(60f, 1.7777778f, 0.01f, 100f));
		this.settings.RenderMaterial.enableInstancing = true;
		for (int i = 0; i < GPUBenchmark.batches.Count; i++)
		{
			if (i % 2 == 0)
			{
				commandBuffer.DrawMeshInstanced(this.settings.Mesh, 0, this.settings.RenderMaterial, 0, GPUBenchmark.batches[i], GPUBenchmark.batches[i].Length);
			}
			else
			{
				commandBuffer.DrawMeshInstanced(this.settings.Mesh, 0, this.settings.SecondRenderMaterial, 0, GPUBenchmark.batches[i], GPUBenchmark.batches[i].Length);
			}
		}
		commandBuffer.ReleaseTemporaryRT(nameID);
		commandBuffer.ReleaseTemporaryRT(nameID2);
		commandBuffer.ReleaseTemporaryRT(nameID3);
		commandBuffer.ReleaseTemporaryRT(nameID4);
		commandBuffer.ReleaseTemporaryRT(nameID5);
		RenderTexture renderTexture = new RenderTexture(2, 2, 0);
		renderTexture.Create();
		Texture2D texture2D = new Texture2D(1, 1);
		Stopwatch stopwatch = Stopwatch.StartNew();
		Graphics.ExecuteCommandBuffer(commandBuffer);
		Graphics.Blit(target_texture, renderTexture);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, 1f, 1f), 0, 0, false);
		texture2D.Apply();
		float result = (float)stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Stop();
		Graphics.Blit(texture2D, renderTexture);
		RenderTexture.active = null;
		renderTexture.Release();
		Object.Destroy(texture2D);
		return result;
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0001E050 File Offset: 0x0001C250
	private int GenerateRandomTexture(CommandBuffer cmd, string texture_name, RenderTextureDescriptor descriptor)
	{
		int num = Shader.PropertyToID(texture_name);
		GPUBenchmark.property_block.SetFloat("_Seed", Random.Range(0f, 1f));
		cmd.GetTemporaryRT(num, descriptor);
		cmd.SetRenderTarget(num);
		cmd.DrawMesh(GPUBenchmark.fullscreen_mesh, Matrix4x4.identity, this.settings.TextureGenerationmaterial, 0, 0, GPUBenchmark.property_block);
		return num;
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0001E0BC File Offset: 0x0001C2BC
	private static Matrix4x4 RandomTransformMatrix(Random random, float position_delta = 10f, float rotation_range = 180f, float min_scale = 0.1f, float max_scale = 1f)
	{
		Vector3 pos = new Vector3(GPUBenchmark.RandomRange(random, -position_delta, position_delta), GPUBenchmark.RandomRange(random, -position_delta, position_delta), GPUBenchmark.RandomRange(random, -position_delta, position_delta));
		Quaternion q = Quaternion.Euler(GPUBenchmark.RandomRange(random, -rotation_range, rotation_range), GPUBenchmark.RandomRange(random, -rotation_range, rotation_range), GPUBenchmark.RandomRange(random, -rotation_range, rotation_range));
		Vector3 s = new Vector3(GPUBenchmark.RandomRange(random, min_scale, max_scale), GPUBenchmark.RandomRange(random, min_scale, max_scale), GPUBenchmark.RandomRange(random, min_scale, max_scale));
		return Matrix4x4.TRS(pos, q, s);
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0001E134 File Offset: 0x0001C334
	private static Mesh CreateFullscreenMesh()
	{
		Mesh mesh = new Mesh();
		mesh.SetVertices(new Vector3[]
		{
			new Vector3(-1f, -1f, 0f),
			new Vector3(-1f, 1f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, -1f, 0f)
		});
		mesh.SetUVs(0, new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		});
		mesh.SetIndices(new int[]
		{
			0,
			1,
			2,
			2,
			3,
			0
		}, MeshTopology.Triangles, 0);
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);
		return mesh;
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0001E260 File Offset: 0x0001C460
	private static List<Matrix4x4[]> GenerateDeterministicBatches()
	{
		Random random = new Random(4268);
		List<Matrix4x4[]> list = new List<Matrix4x4[]>(220);
		for (int i = 0; i < 220; i++)
		{
			Matrix4x4[] array = new Matrix4x4[8];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = GPUBenchmark.RandomTransformMatrix(random, 5f, 180f, 0.01f, 0.6f);
			}
			list.Add(array);
		}
		return list;
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0001E2D6 File Offset: 0x0001C4D6
	private static float RandomRange(Random random, float min, float max)
	{
		return (float)random.NextDouble() * (max - min) + min;
	}

	// Token: 0x040002C8 RID: 712
	[SerializeField]
	private GPUBenchmarkAssets settings;

	// Token: 0x040002C9 RID: 713
	private RenderTexture render_texture;

	// Token: 0x040002CA RID: 714
	private float average_test_time;

	// Token: 0x040002CB RID: 715
	private static List<Matrix4x4[]> _batches;

	// Token: 0x040002CC RID: 716
	private static MaterialPropertyBlock _property_block;

	// Token: 0x040002CD RID: 717
	private static Mesh _fullscreen_mesh;

	// Token: 0x040002CE RID: 718
	private const int NUMBER_OF_TESTS = 50;

	// Token: 0x040002CF RID: 719
	private const int WARMUP_TESTS_COUNT = 10;

	// Token: 0x040002D0 RID: 720
	private const int NUMBER_OF_BATCHES = 220;

	// Token: 0x040002D1 RID: 721
	private const int NUMBER_OF_BATCH_INSTANCES = 8;

	// Token: 0x040002D2 RID: 722
	private const int USED_TEXTURES_RESOLUTION = 1024;

	// Token: 0x040002D3 RID: 723
	private bool is_test_running;

	// Token: 0x040002D4 RID: 724
	private int test_index;

	// Token: 0x040002D5 RID: 725
	private List<float> gathered_metrics = new List<float>(50);

	// Token: 0x040002D6 RID: 726
	private Action<GPUBenchmark> on_complete_action;
}
