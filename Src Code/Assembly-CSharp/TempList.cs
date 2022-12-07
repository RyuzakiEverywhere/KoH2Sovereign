using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x020000D4 RID: 212
[DebuggerDisplay("[{Length}]")]
[DebuggerTypeProxy(typeof(TempListDebugView<>))]
public struct TempList<T> where T : struct
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000A34 RID: 2612 RVA: 0x0007571D File Offset: 0x0007391D
	// (set) Token: 0x06000A35 RID: 2613 RVA: 0x00075725 File Offset: 0x00073925
	public int Length { get; private set; }

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000A36 RID: 2614 RVA: 0x0007572E File Offset: 0x0007392E
	public int Capacity
	{
		get
		{
			return this.chunks_count * this.chunk_capacity;
		}
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x00075740 File Offset: 0x00073940
	public TempList(bool preallocate, int chunk_elements = 0)
	{
		int num = UnsafeUtility.SizeOf<T>();
		if (num <= 0)
		{
			num = 1;
		}
		if (chunk_elements <= 0)
		{
			this.chunk_size = 64;
			this.chunk_capacity = this.chunk_size / num;
			if (this.chunk_capacity <= 0)
			{
				this.chunk_size = num;
				this.chunk_capacity = 1;
			}
		}
		else
		{
			this.chunk_capacity = chunk_elements;
			this.chunk_size = this.chunk_capacity * num;
		}
		this.chunks = null;
		this.chunks_capacity = 0;
		this.chunks_count = 0;
		this.Length = 0;
		if (preallocate)
		{
			this.Grow();
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x000757C8 File Offset: 0x000739C8
	public unsafe void Dispose()
	{
		if (this.chunks == null)
		{
			return;
		}
		for (int i = 0; i < this.chunks_count; i++)
		{
			this.FreeChunk(this.chunks[i]);
		}
		UnsafeUtility.Free((void*)this.chunks, Allocator.Temp);
		this.chunks = null;
		this.Length = 0;
		this.chunks_count = 0;
		this.chunks_capacity = 0;
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x00075835 File Offset: 0x00073A35
	public void Clear()
	{
		this.Length = 0;
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00075840 File Offset: 0x00073A40
	public void Add(T val)
	{
		if (this.Length >= this.Capacity)
		{
			this.Grow();
		}
		int length = this.Length;
		this.Length = length + 1;
		int idx = length;
		this[idx] = val;
	}

	// Token: 0x1700007D RID: 125
	public unsafe T this[int idx]
	{
		get
		{
			ref TempListChunk ptr = this.chunks[idx / this.chunk_capacity];
			idx %= this.chunk_capacity;
			return UnsafeUtility.ReadArrayElement<T>(ptr.data, idx);
		}
		set
		{
			ref TempListChunk ptr = this.chunks[idx / this.chunk_capacity];
			idx %= this.chunk_capacity;
			UnsafeUtility.WriteArrayElement<T>(ptr.data, idx, value);
		}
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x000758E4 File Offset: 0x00073AE4
	private unsafe TempListChunk AllocChunk()
	{
		void* data = UnsafeUtility.Malloc((long)this.chunk_size, UnsafeUtility.AlignOf<T>(), Allocator.Temp);
		return new TempListChunk
		{
			data = data
		};
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x00075915 File Offset: 0x00073B15
	private void FreeChunk(TempListChunk chunk)
	{
		UnsafeUtility.Free(chunk.data, Allocator.Temp);
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x00075924 File Offset: 0x00073B24
	private unsafe void Grow()
	{
		if (this.chunks_count >= this.chunks_capacity)
		{
			this.Realloc();
		}
		ref TempListChunk ptr = ref *this.chunks;
		int num = this.chunks_count;
		this.chunks_count = num + 1;
		*(ref ptr + (IntPtr)num * (IntPtr)sizeof(TempListChunk)) = this.AllocChunk();
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x00075970 File Offset: 0x00073B70
	private unsafe void Realloc()
	{
		int num = UnsafeUtility.SizeOf<TempListChunk>();
		this.chunks_capacity += 64 / num;
		TempListChunk* destination = (TempListChunk*)UnsafeUtility.Malloc((long)(this.chunks_capacity * num), 64, Allocator.Temp);
		if (this.chunks != null && this.chunks_count > 0)
		{
			UnsafeUtility.MemCpy((void*)destination, (void*)this.chunks, (long)(this.chunks_count * num));
			UnsafeUtility.Free((void*)this.chunks, Allocator.Temp);
		}
		this.chunks = destination;
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x000759E4 File Offset: 0x00073BE4
	public T[] ToArray()
	{
		T[] array = new T[this.Length];
		for (int i = 0; i < this.Length; i++)
		{
			array[i] = this[i];
		}
		return array;
	}

	// Token: 0x04000838 RID: 2104
	private unsafe TempListChunk* chunks;

	// Token: 0x04000839 RID: 2105
	private int chunks_count;

	// Token: 0x0400083A RID: 2106
	private int chunks_capacity;

	// Token: 0x0400083B RID: 2107
	private int chunk_capacity;

	// Token: 0x0400083C RID: 2108
	private int chunk_size;

	// Token: 0x0400083D RID: 2109
	private const int cache_line_size = 64;
}
