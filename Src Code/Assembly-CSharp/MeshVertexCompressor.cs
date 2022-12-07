using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200007B RID: 123
public static class MeshVertexCompressor
{
	// Token: 0x060004A9 RID: 1193 RVA: 0x000366EC File Offset: 0x000348EC
	public static bool Compress<[IsUnmanaged] TVertexLayout>(Mesh mesh, out MeshVertexCompressor.MeshCompressionTask<TVertexLayout> compression_task, VertexAttributeDescriptor[] attributes) where TVertexLayout : struct, ValueType
	{
		compression_task = null;
		MeshVertexCompressor.NativeMeshData<TVertexLayout> nativeMeshData = default(MeshVertexCompressor.NativeMeshData<TVertexLayout>);
		foreach (VertexAttributeDescriptor vertexAttributeDescriptor in attributes)
		{
			if (!SystemInfo.SupportsVertexAttributeFormat(vertexAttributeDescriptor.format, vertexAttributeDescriptor.dimension))
			{
				Debug.LogWarning(string.Format("Cant compress mesh data. Vertex attribute is not supported on this platform: {0} dim:{1}", vertexAttributeDescriptor.format, vertexAttributeDescriptor.dimension));
				return false;
			}
		}
		MeshVertexCompressor.InputMeshData inputMeshData = new MeshVertexCompressor.InputMeshData(mesh);
		nativeMeshData.vertex_buffer = new NativeArray<TVertexLayout>(mesh.vertexCount, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		nativeMeshData.attributes = new NativeArray<VertexAttributeDescriptor>(attributes, Allocator.TempJob);
		nativeMeshData.vertex_count = mesh.vertexCount;
		nativeMeshData.bounds = mesh.bounds;
		nativeMeshData.index_count = inputMeshData.indices.count;
		nativeMeshData.index_format = mesh.indexFormat;
		if (nativeMeshData.index_format == IndexFormat.UInt32)
		{
			nativeMeshData.index32_buffer = new NativeArray<uint>(nativeMeshData.index_count, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			nativeMeshData.index16_buffer = new NativeArray<ushort>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		}
		else
		{
			nativeMeshData.index16_buffer = new NativeArray<ushort>(nativeMeshData.index_count, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			nativeMeshData.index32_buffer = new NativeArray<uint>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		}
		JobHandle job_handle = new MeshVertexCompressor.MeshCompressionJob<TVertexLayout>
		{
			in_mesh = inputMeshData,
			vertex_buffer = nativeMeshData.vertex_buffer,
			index16_buffer = nativeMeshData.index16_buffer,
			index32_buffer = nativeMeshData.index32_buffer,
			index_format = nativeMeshData.index_format,
			attributes = nativeMeshData.attributes
		}.Schedule(default(JobHandle));
		JobHandle.ScheduleBatchedJobs();
		compression_task = new MeshVertexCompressor.MeshCompressionTask<TVertexLayout>(inputMeshData, nativeMeshData, job_handle);
		return true;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00036880 File Offset: 0x00034A80
	public static bool Compress<[IsUnmanaged] T>(Mesh mesh, out Mesh out_mesh, VertexAttributeDescriptor[] new_attributes) where T : struct, ValueType
	{
		MeshVertexCompressor.MeshCompressionTask<T> meshCompressionTask;
		if (MeshVertexCompressor.Compress<T>(mesh, out meshCompressionTask, new_attributes))
		{
			meshCompressionTask.Complete();
			out_mesh = meshCompressionTask.compressed_mesh;
			meshCompressionTask.Dispose();
			return true;
		}
		out_mesh = null;
		return false;
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x000368B4 File Offset: 0x00034AB4
	public unsafe static void WriteToPtrWithSpecifiedFormat(byte* buffer_ptr, float value, VertexAttributeFormat format)
	{
		switch (format)
		{
		case VertexAttributeFormat.Float32:
			UnsafeUtility.CopyStructureToPtr<float>(ref value, (void*)buffer_ptr);
			return;
		case VertexAttributeFormat.Float16:
		{
			half half = math.half(value);
			UnsafeUtility.CopyStructureToPtr<half>(ref half, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.UNorm8:
		{
			byte b = Convert.ToByte(value * 255f);
			UnsafeUtility.CopyStructureToPtr<byte>(ref b, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.SNorm8:
		{
			sbyte b2 = Convert.ToSByte(value * 127f);
			UnsafeUtility.CopyStructureToPtr<sbyte>(ref b2, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.UNorm16:
		{
			ushort num = Convert.ToUInt16(value * 65535f);
			UnsafeUtility.CopyStructureToPtr<ushort>(ref num, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.SNorm16:
		{
			short num2 = Convert.ToInt16(value * 32767f);
			UnsafeUtility.CopyStructureToPtr<short>(ref num2, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.UInt8:
		{
			byte b3 = Convert.ToByte(value);
			UnsafeUtility.CopyStructureToPtr<byte>(ref b3, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.SInt8:
		{
			sbyte b4 = Convert.ToSByte(value);
			UnsafeUtility.CopyStructureToPtr<sbyte>(ref b4, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.UInt16:
		{
			ushort num3 = (ushort)((int)value);
			UnsafeUtility.CopyStructureToPtr<ushort>(ref num3, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.SInt16:
		{
			short num4 = (short)((int)value);
			UnsafeUtility.CopyStructureToPtr<short>(ref num4, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.UInt32:
		{
			uint num5 = (uint)value;
			UnsafeUtility.CopyStructureToPtr<uint>(ref num5, (void*)buffer_ptr);
			return;
		}
		case VertexAttributeFormat.SInt32:
		{
			int num6 = (int)value;
			UnsafeUtility.CopyStructureToPtr<int>(ref num6, (void*)buffer_ptr);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x000369C4 File Offset: 0x00034BC4
	public static int GetVertexDataSize(VertexAttributeDescriptor[] attributes)
	{
		int num = 0;
		foreach (VertexAttributeDescriptor vertexAttributeDescriptor in attributes)
		{
			num += MeshVertexCompressor.GetAttributeFormatSize(vertexAttributeDescriptor.format) * vertexAttributeDescriptor.dimension;
		}
		return num;
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00036A04 File Offset: 0x00034C04
	public static int GetAttributeFormatSize(VertexAttributeFormat format)
	{
		switch (format)
		{
		case VertexAttributeFormat.Float32:
		case VertexAttributeFormat.UInt32:
		case VertexAttributeFormat.SInt32:
			return 4;
		case VertexAttributeFormat.Float16:
		case VertexAttributeFormat.UNorm16:
		case VertexAttributeFormat.SNorm16:
		case VertexAttributeFormat.UInt16:
		case VertexAttributeFormat.SInt16:
			return 2;
		case VertexAttributeFormat.UNorm8:
		case VertexAttributeFormat.SNorm8:
		case VertexAttributeFormat.UInt8:
		case VertexAttributeFormat.SInt8:
			return 1;
		default:
			return 0;
		}
	}

	// Token: 0x02000547 RID: 1351
	public struct InputMeshData : IDisposable
	{
		// Token: 0x06004388 RID: 17288 RVA: 0x001FD8CC File Offset: 0x001FBACC
		public InputMeshData(Mesh mesh)
		{
			this = default(MeshVertexCompressor.InputMeshData);
			this.positions = new MeshVertexCompressor.PinnedArray<Vector3>(mesh.vertices);
			this.normals = new MeshVertexCompressor.PinnedArray<Vector3>(mesh.normals);
			this.tangents = new MeshVertexCompressor.PinnedArray<Vector4>(mesh.tangents);
			this.uv1s = new MeshVertexCompressor.PinnedArray<Vector2>(mesh.uv);
			this.uv2s = new MeshVertexCompressor.PinnedArray<Vector2>(mesh.uv2);
			this.colors = new MeshVertexCompressor.PinnedArray<Color>(mesh.colors);
			this.indices = new MeshVertexCompressor.PinnedArray<int>(mesh.GetIndices(0, true));
		}

		// Token: 0x06004389 RID: 17289 RVA: 0x001FD95C File Offset: 0x001FBB5C
		public void Dispose()
		{
			this.positions.Dispose();
			this.normals.Dispose();
			this.tangents.Dispose();
			this.uv1s.Dispose();
			this.uv2s.Dispose();
			this.colors.Dispose();
			this.indices.Dispose();
		}

		// Token: 0x04002FB3 RID: 12211
		public MeshVertexCompressor.PinnedArray<Vector3> positions;

		// Token: 0x04002FB4 RID: 12212
		public MeshVertexCompressor.PinnedArray<Vector3> normals;

		// Token: 0x04002FB5 RID: 12213
		public MeshVertexCompressor.PinnedArray<Vector4> tangents;

		// Token: 0x04002FB6 RID: 12214
		public MeshVertexCompressor.PinnedArray<Vector2> uv1s;

		// Token: 0x04002FB7 RID: 12215
		public MeshVertexCompressor.PinnedArray<Vector2> uv2s;

		// Token: 0x04002FB8 RID: 12216
		public MeshVertexCompressor.PinnedArray<Color> colors;

		// Token: 0x04002FB9 RID: 12217
		public MeshVertexCompressor.PinnedArray<int> indices;
	}

	// Token: 0x02000548 RID: 1352
	public struct NativeMeshData<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
	{
		// Token: 0x0600438A RID: 17290 RVA: 0x001FD9B8 File Offset: 0x001FBBB8
		public Mesh CreateMesh()
		{
			Mesh mesh = new Mesh();
			SubMeshDescriptor desc = new SubMeshDescriptor
			{
				vertexCount = this.vertex_count,
				baseVertex = 0,
				bounds = mesh.bounds,
				firstVertex = 0,
				topology = MeshTopology.Triangles,
				indexStart = 0,
				indexCount = this.index_count
			};
			VertexAttributeDescriptor[] array = new VertexAttributeDescriptor[this.attributes.Length];
			this.attributes.CopyTo(array);
			mesh.SetVertexBufferParams(this.vertex_count, array);
			mesh.SetIndexBufferParams(this.index_count, this.index_format);
			mesh.SetVertexBufferData<T>(this.vertex_buffer, 0, 0, this.vertex_count, 0, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
			if (this.index_format == IndexFormat.UInt16)
			{
				mesh.SetIndexBufferData<ushort>(this.index16_buffer, 0, 0, this.index_count, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
			}
			else
			{
				mesh.SetIndexBufferData<uint>(this.index32_buffer, 0, 0, this.index_count, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
			}
			mesh.SetSubMesh(0, desc, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
			mesh.bounds = this.bounds;
			return mesh;
		}

		// Token: 0x0600438B RID: 17291 RVA: 0x001FDABC File Offset: 0x001FBCBC
		public void Dispose()
		{
			if (this.vertex_buffer.IsCreated)
			{
				this.vertex_buffer.Dispose();
				this.index16_buffer.Dispose();
				this.index32_buffer.Dispose();
				this.attributes.Dispose();
			}
		}

		// Token: 0x04002FBA RID: 12218
		public NativeArray<T> vertex_buffer;

		// Token: 0x04002FBB RID: 12219
		public NativeArray<ushort> index16_buffer;

		// Token: 0x04002FBC RID: 12220
		public NativeArray<uint> index32_buffer;

		// Token: 0x04002FBD RID: 12221
		public NativeArray<VertexAttributeDescriptor> attributes;

		// Token: 0x04002FBE RID: 12222
		public IndexFormat index_format;

		// Token: 0x04002FBF RID: 12223
		public int vertex_count;

		// Token: 0x04002FC0 RID: 12224
		public int index_count;

		// Token: 0x04002FC1 RID: 12225
		public Bounds bounds;

		// Token: 0x04002FC2 RID: 12226
		public const MeshUpdateFlags mesh_update_flags = MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds;
	}

	// Token: 0x02000549 RID: 1353
	public interface IMeshCompressionTask : IDisposable
	{
		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x0600438C RID: 17292
		Mesh compressed_mesh { get; }

		// Token: 0x0600438D RID: 17293
		void Complete();
	}

	// Token: 0x0200054A RID: 1354
	public class MeshCompressionTask<[IsUnmanaged] T> : MeshVertexCompressor.IMeshCompressionTask, IDisposable where T : struct, ValueType
	{
		// Token: 0x0600438E RID: 17294 RVA: 0x001FDAF7 File Offset: 0x001FBCF7
		public MeshCompressionTask(MeshVertexCompressor.InputMeshData input_mesh_data, MeshVertexCompressor.NativeMeshData<T> output_mesh_data, JobHandle job_handle)
		{
			this.input_mesh_data = input_mesh_data;
			this.output_mesh_data = output_mesh_data;
			this.job_handle = job_handle;
			this.compressed_mesh = null;
		}

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x0600438F RID: 17295 RVA: 0x001FDB1B File Offset: 0x001FBD1B
		// (set) Token: 0x06004390 RID: 17296 RVA: 0x001FDB23 File Offset: 0x001FBD23
		public Mesh compressed_mesh { get; private set; }

		// Token: 0x06004391 RID: 17297 RVA: 0x001FDB2C File Offset: 0x001FBD2C
		public void Complete()
		{
			this.job_handle.Complete();
			this.compressed_mesh = this.output_mesh_data.CreateMesh();
		}

		// Token: 0x06004392 RID: 17298 RVA: 0x001FDB4A File Offset: 0x001FBD4A
		public void Dispose()
		{
			if (!this.is_disposed)
			{
				if (!this.job_handle.IsCompleted)
				{
					this.job_handle.Complete();
				}
				this.input_mesh_data.Dispose();
				this.output_mesh_data.Dispose();
				this.is_disposed = true;
			}
		}

		// Token: 0x04002FC3 RID: 12227
		private MeshVertexCompressor.InputMeshData input_mesh_data;

		// Token: 0x04002FC4 RID: 12228
		private MeshVertexCompressor.NativeMeshData<T> output_mesh_data;

		// Token: 0x04002FC5 RID: 12229
		private JobHandle job_handle;

		// Token: 0x04002FC6 RID: 12230
		private bool is_disposed;
	}

	// Token: 0x0200054B RID: 1355
	public struct PinnedArray<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
	{
		// Token: 0x06004393 RID: 17299 RVA: 0x001FDB89 File Offset: 0x001FBD89
		public unsafe PinnedArray(T[] array)
		{
			this = default(MeshVertexCompressor.PinnedArray<T>);
			if (array == null)
			{
				this.ptr = null;
				this.gc_handle = 0UL;
				this.count = 0;
				return;
			}
			this.ptr = (T*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref this.gc_handle);
			this.count = array.Length;
		}

		// Token: 0x06004394 RID: 17300 RVA: 0x001FDBC8 File Offset: 0x001FBDC8
		public void Dispose()
		{
			if (this.ptr != null)
			{
				AllocationManager.ReleaseGCObject(ref this.gc_handle);
			}
		}

		// Token: 0x06004395 RID: 17301 RVA: 0x001FDBDF File Offset: 0x001FBDDF
		public unsafe T Get(int index)
		{
			return this.ptr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}

		// Token: 0x170004F0 RID: 1264
		public unsafe T this[int index]
		{
			get
			{
				return this.ptr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			}
		}

		// Token: 0x04002FC8 RID: 12232
		[NativeDisableUnsafePtrRestriction]
		public unsafe T* ptr;

		// Token: 0x04002FC9 RID: 12233
		public int count;

		// Token: 0x04002FCA RID: 12234
		public ulong gc_handle;
	}

	// Token: 0x0200054C RID: 1356
	[BurstCompile]
	public struct MeshCompressionJob<[IsUnmanaged] T> : IJob where T : struct, ValueType
	{
		// Token: 0x06004397 RID: 17303 RVA: 0x001FDBF8 File Offset: 0x001FBDF8
		public unsafe void Execute()
		{
			byte* ptr = (byte*)this.vertex_buffer.GetUnsafePtr<T>();
			for (int i = 0; i < this.vertex_buffer.Length; i++)
			{
				for (int j = 0; j < this.attributes.Length; j++)
				{
					VertexAttributeDescriptor vertexAttributeDescriptor = this.attributes[j];
					for (int k = 0; k < vertexAttributeDescriptor.dimension; k++)
					{
						float value = 0f;
						switch (vertexAttributeDescriptor.attribute)
						{
						case VertexAttribute.Position:
							value = math.float4(this.in_mesh.positions[i], 1f)[k];
							break;
						case VertexAttribute.Normal:
							if (this.in_mesh.normals.ptr != null)
							{
								value = math.float4(this.in_mesh.normals[i], 0f)[k];
							}
							break;
						case VertexAttribute.Tangent:
							if (this.in_mesh.tangents.ptr != null)
							{
								value = math.float4(this.in_mesh.tangents[i])[k];
							}
							break;
						case VertexAttribute.Color:
							if (this.in_mesh.colors.ptr != null)
							{
								value = math.float4(this.in_mesh.colors[i])[k];
							}
							break;
						case VertexAttribute.TexCoord0:
							if (this.in_mesh.uv1s.ptr != null)
							{
								value = math.float4(this.in_mesh.uv1s[i], 0f, 0f)[k];
							}
							break;
						case VertexAttribute.TexCoord1:
							if (this.in_mesh.uv2s.ptr != null)
							{
								value = math.float4(this.in_mesh.uv2s[i], 0f, 0f)[k];
							}
							break;
						}
						MeshVertexCompressor.WriteToPtrWithSpecifiedFormat(ptr, value, vertexAttributeDescriptor.format);
						ptr += MeshVertexCompressor.GetAttributeFormatSize(vertexAttributeDescriptor.format);
					}
				}
			}
			if (this.index_format == IndexFormat.UInt32)
			{
				for (int l = 0; l < this.in_mesh.indices.count; l++)
				{
					this.index32_buffer[l] = (uint)this.in_mesh.indices[l];
				}
				return;
			}
			for (int m = 0; m < this.in_mesh.indices.count; m++)
			{
				this.index16_buffer[m] = (ushort)this.in_mesh.indices[m];
			}
		}

		// Token: 0x04002FCB RID: 12235
		public MeshVertexCompressor.InputMeshData in_mesh;

		// Token: 0x04002FCC RID: 12236
		[WriteOnly]
		public NativeArray<T> vertex_buffer;

		// Token: 0x04002FCD RID: 12237
		[WriteOnly]
		public NativeArray<uint> index32_buffer;

		// Token: 0x04002FCE RID: 12238
		[WriteOnly]
		public NativeArray<ushort> index16_buffer;

		// Token: 0x04002FCF RID: 12239
		[ReadOnly]
		public NativeArray<VertexAttributeDescriptor> attributes;

		// Token: 0x04002FD0 RID: 12240
		public IndexFormat index_format;
	}

	// Token: 0x0200054D RID: 1357
	private struct ExampleVertexDataLayout
	{
		// Token: 0x04002FD1 RID: 12241
		private float3 position;

		// Token: 0x04002FD2 RID: 12242
		private int normal;

		// Token: 0x04002FD3 RID: 12243
		private int tangent;

		// Token: 0x04002FD4 RID: 12244
		private half2 uv;

		// Token: 0x04002FD5 RID: 12245
		public static readonly VertexAttributeDescriptor[] Attibutes = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.SNorm8, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.SNorm8, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 2, 0)
		};
	}
}
