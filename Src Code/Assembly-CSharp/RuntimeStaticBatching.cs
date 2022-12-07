using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000145 RID: 325
public class RuntimeStaticBatching : MonoBehaviour
{
	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06001132 RID: 4402 RVA: 0x000B637A File Offset: 0x000B457A
	public Transform original_parent
	{
		get
		{
			return base.transform.FindGrandChild("Houses");
		}
	}

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06001133 RID: 4403 RVA: 0x000B638C File Offset: 0x000B458C
	// (remove) Token: 0x06001134 RID: 4404 RVA: 0x000B63C4 File Offset: 0x000B45C4
	public event Action OnBatchingComplete;

	// Token: 0x06001135 RID: 4405 RVA: 0x000B63FC File Offset: 0x000B45FC
	private void OnEnable()
	{
		this.settlementsLayer = LayerMask.NameToLayer("Settlements");
		foreach (MeshRenderer meshRenderer in this.original_renderers)
		{
			meshRenderer.enabled = false;
		}
		foreach (MeshRenderer meshRenderer2 in this.static_batched_renderers)
		{
			meshRenderer2.enabled = true;
		}
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x000B64A0 File Offset: 0x000B46A0
	private void OnDisable()
	{
		this.original_renderers.RemoveAll((MeshRenderer r) => r == null);
		this.static_batched_renderers.RemoveAll((MeshRenderer r) => r == null);
		foreach (MeshRenderer meshRenderer in this.original_renderers)
		{
			meshRenderer.enabled = true;
		}
		foreach (MeshRenderer meshRenderer2 in this.static_batched_renderers)
		{
			meshRenderer2.enabled = false;
		}
		foreach (MeshVertexCompressor.IMeshCompressionTask meshCompressionTask in this.pending_mesh_compressions)
		{
			meshCompressionTask.Complete();
			Common.DestroyObj(meshCompressionTask.compressed_mesh);
			meshCompressionTask.Dispose();
		}
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x000B65D8 File Offset: 0x000B47D8
	[ContextMenu("Generate static batches")]
	public void GenerateStaticBatches()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.isDuringBatching)
		{
			Debug.LogWarning("Cant start batching, another one is pending.", this);
			return;
		}
		if (this.original_parent == null)
		{
			Debug.LogWarning("There is no object to batch", this);
			return;
		}
		base.StartCoroutine(this.GenerateStaticBatchesIE());
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x000B6629 File Offset: 0x000B4829
	private IEnumerator GenerateStaticBatchesIE()
	{
		yield return null;
		this.isDuringBatching = true;
		this.Cleanup();
		Transform batch_parent = this.original_parent;
		int num;
		for (int i = 0; i < batch_parent.childCount; i = num + 1)
		{
			Transform child = batch_parent.GetChild(i);
			for (int j = 0; j < child.childCount; j = num + 1)
			{
				Transform child2 = child.GetChild(j);
				GameObject batch = this.GenerateStaticBatch(child2.gameObject);
				batch.transform.SetParent(child, false);
				batch.transform.SetSiblingIndex(j + 1);
				num = j;
				j = num + 1;
				RuntimeStaticBatching.CopyLocalTransformProperties(child2, batch.transform);
				yield return null;
				MeshFilter[] componentsInChildren = batch.GetComponentsInChildren<MeshFilter>();
				foreach (MeshFilter mf in componentsInChildren)
				{
					MeshVertexCompressor.MeshCompressionTask<RuntimeStaticBatching.VertexData> compression_task;
					if (MeshVertexCompressor.Compress<RuntimeStaticBatching.VertexData>(mf.sharedMesh, out compression_task, RuntimeStaticBatching.VertexData.Attibutes))
					{
						this.pending_mesh_compressions.Add(compression_task);
						yield return null;
						compression_task.Complete();
						Object sharedMesh = mf.sharedMesh;
						mf.sharedMesh = compression_task.compressed_mesh;
						Common.DestroyObj(sharedMesh);
						compression_task.Dispose();
						this.pending_mesh_compressions.Remove(compression_task);
						yield return null;
					}
					compression_task = null;
					mf = null;
				}
				MeshFilter[] array = null;
				batch = null;
				num = j;
			}
			child = null;
			num = i;
		}
		foreach (MeshRenderer meshRenderer in this.original_renderers)
		{
			meshRenderer.enabled = false;
		}
		foreach (MeshRenderer meshRenderer2 in this.original_renderers)
		{
			Common.DestroyObj(meshRenderer2.GetComponent<MeshFilter>());
			Common.DestroyObj(meshRenderer2);
		}
		Resources.UnloadUnusedAssets();
		this.isDuringBatching = false;
		Action onBatchingComplete = this.OnBatchingComplete;
		if (onBatchingComplete != null)
		{
			onBatchingComplete();
		}
		yield break;
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x000B6638 File Offset: 0x000B4838
	[ContextMenu("Cleanup")]
	private void Cleanup()
	{
		this.OnDisable();
		this.original_renderers.Clear();
		this.static_batched_renderers.Clear();
		foreach (GameObject obj in this.created_objects)
		{
			Common.DestroyObj(obj);
		}
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x000B66A4 File Offset: 0x000B48A4
	private static void CopyLocalTransformProperties(Transform from, Transform to)
	{
		to.localPosition = from.localPosition;
		to.localRotation = from.localRotation;
		to.localScale = from.localScale;
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x000B66CC File Offset: 0x000B48CC
	private GameObject GenerateStaticBatch(GameObject object_to_batch)
	{
		RuntimeStaticBatching.<>c__DisplayClass21_0 CS$<>8__locals1 = new RuntimeStaticBatching.<>c__DisplayClass21_0();
		CS$<>8__locals1.object_to_batch = object_to_batch;
		CS$<>8__locals1.<>4__this = this;
		foreach (KeyValuePair<Material, List<CombineInstance>> keyValuePair in this.material_combiners)
		{
			keyValuePair.Value.Clear();
		}
		this.material_combiners.Clear();
		MeshRenderer component = CS$<>8__locals1.object_to_batch.GetComponent<MeshRenderer>();
		if (component != null)
		{
			CS$<>8__locals1.<GenerateStaticBatch>g__ProcessRenderer|0(component);
		}
		Common.IterateThroughChildrenRecursive<MeshRenderer>(CS$<>8__locals1.object_to_batch.transform, new Action<MeshRenderer>(CS$<>8__locals1.<GenerateStaticBatch>g__ProcessRenderer|0));
		this.created_objects_cache.Clear();
		foreach (KeyValuePair<Material, List<CombineInstance>> keyValuePair2 in this.material_combiners)
		{
			Material key = keyValuePair2.Key;
			List<CombineInstance> value = keyValuePair2.Value;
			Mesh mesh = new Mesh();
			int num = value.Sum((CombineInstance i) => i.mesh.vertexCount);
			this.batched_vertex_count += num;
			mesh.indexFormat = ((num < 65535) ? IndexFormat.UInt16 : IndexFormat.UInt32);
			mesh.CombineMeshes(value.ToArray(), true, true, false);
			GameObject item = this.CreateObjectForMesh(mesh, key);
			this.created_objects_cache.Add(item);
		}
		GameObject gameObject = new GameObject(CS$<>8__locals1.object_to_batch.name + "(static_batch)");
		this.created_objects.Add(gameObject);
		for (int j = 0; j < this.created_objects_cache.Count; j++)
		{
			this.created_objects_cache[j].transform.SetParent(gameObject.transform, false);
		}
		return gameObject;
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x000B68B8 File Offset: 0x000B4AB8
	private GameObject CreateObjectForMesh(Mesh mesh, Material material)
	{
		GameObject gameObject = new GameObject("Batch for " + material.name);
		gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = material;
		meshRenderer.lightProbeUsage = LightProbeUsage.Off;
		meshRenderer.receiveShadows = true;
		gameObject.SetLayer(this.settlementsLayer, true);
		this.static_batched_renderers.Add(meshRenderer);
		return gameObject;
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x000B691C File Offset: 0x000B4B1C
	private static CombineInstance GetMeshInstance(Transform relative_parent, MeshRenderer renderer)
	{
		Mesh sharedMesh = renderer.GetComponent<MeshFilter>().sharedMesh;
		Transform transform = renderer.transform;
		Matrix4x4 matrix4x = Matrix4x4.identity;
		while (transform != relative_parent)
		{
			matrix4x = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale) * matrix4x;
			transform = transform.parent;
		}
		return new CombineInstance
		{
			mesh = sharedMesh,
			subMeshIndex = 0,
			transform = matrix4x
		};
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x000B6994 File Offset: 0x000B4B94
	private static bool IsRendererValid(MeshRenderer renderer)
	{
		if (renderer.sharedMaterial == null)
		{
			return false;
		}
		MeshFilter component = renderer.GetComponent<MeshFilter>();
		return !(component == null) && !(component.sharedMesh == null) && component.sharedMesh.isReadable;
	}

	// Token: 0x04000B70 RID: 2928
	public int batched_vertex_count;

	// Token: 0x04000B71 RID: 2929
	private List<MeshRenderer> original_renderers = new List<MeshRenderer>();

	// Token: 0x04000B72 RID: 2930
	private List<MeshRenderer> static_batched_renderers = new List<MeshRenderer>();

	// Token: 0x04000B73 RID: 2931
	private List<GameObject> created_objects = new List<GameObject>();

	// Token: 0x04000B74 RID: 2932
	private List<MeshVertexCompressor.IMeshCompressionTask> pending_mesh_compressions = new List<MeshVertexCompressor.IMeshCompressionTask>();

	// Token: 0x04000B75 RID: 2933
	public List<Mesh> invalid_meshes = new List<Mesh>();

	// Token: 0x04000B77 RID: 2935
	private bool isDuringBatching;

	// Token: 0x04000B78 RID: 2936
	private int settlementsLayer;

	// Token: 0x04000B79 RID: 2937
	private Dictionary<Material, List<CombineInstance>> material_combiners = new Dictionary<Material, List<CombineInstance>>();

	// Token: 0x04000B7A RID: 2938
	private List<GameObject> created_objects_cache = new List<GameObject>();

	// Token: 0x02000666 RID: 1638
	private struct VertexData
	{
		// Token: 0x0400356B RID: 13675
		private float3 position;

		// Token: 0x0400356C RID: 13676
		private int normal;

		// Token: 0x0400356D RID: 13677
		private int tangent;

		// Token: 0x0400356E RID: 13678
		private half2 uv;

		// Token: 0x0400356F RID: 13679
		public static readonly VertexAttributeDescriptor[] Attibutes = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.SNorm8, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.SNorm8, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 2, 0)
		};
	}
}
