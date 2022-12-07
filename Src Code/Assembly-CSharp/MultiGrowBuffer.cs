using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x0200008C RID: 140
public struct MultiGrowBuffer<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x0600054A RID: 1354 RVA: 0x0003B54C File Offset: 0x0003974C
	public MultiGrowBuffer(Allocator allocator, int initial_capacity_per_thread = 0)
	{
		JobTLS<GrowBufferData> jobTLS = new JobTLS<GrowBufferData>(allocator, default(GrowBufferData));
		this.data = jobTLS.data;
		for (int i = 0; i <= JobTLSData.Count; i++)
		{
			GrowBuffer<T>.InitData(jobTLS.Ptr(i), allocator, initial_capacity_per_thread);
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0003B595 File Offset: 0x00039795
	public unsafe MultiGrowBuffer(void* data)
	{
		this.data = (JobTLSData*)data;
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0003B59E File Offset: 0x0003979E
	public unsafe MultiGrowBuffer(IntPtr data)
	{
		this.data = (JobTLSData*)((void*)data);
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0003B5AC File Offset: 0x000397AC
	public unsafe IntPtr ToIntPtr()
	{
		return (IntPtr)((void*)this.data);
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0003B5BC File Offset: 0x000397BC
	public unsafe void Dispose()
	{
		for (int i = 0; i <= JobTLSData.Count; i++)
		{
			this.BufData(i)->Dispose();
		}
		JobTLS<T>.Dispose(this.data);
		this.data = null;
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0003B5F8 File Offset: 0x000397F8
	private unsafe GrowBufferData* BufData(int thread_idx)
	{
		JobTLS<GrowBufferData> jobTLS = new JobTLS<GrowBufferData>((void*)this.data);
		return jobTLS.Ptr(thread_idx);
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0003B61A File Offset: 0x0003981A
	public void Add(ref T item, int thread_idx)
	{
		GrowBuffer<T>.AddNonConcurrent(this.BufData(thread_idx), ref item);
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0003B62C File Offset: 0x0003982C
	public unsafe int CalcCount()
	{
		int num = 0;
		for (int i = 0; i <= JobTLSData.Count; i++)
		{
			GrowBufferData* ptr = this.BufData(i);
			num += ptr->count;
		}
		return num;
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0003B660 File Offset: 0x00039860
	public unsafe T[] ToArray()
	{
		T[] array = new T[this.CalcCount()];
		int num = UnsafeUtility.SizeOf<T>();
		T[] array2;
		T* ptr;
		if ((array2 = array) == null || array2.Length == 0)
		{
			ptr = null;
		}
		else
		{
			ptr = &array2[0];
		}
		T* ptr2 = ptr;
		for (int i = 0; i <= JobTLSData.Count; i++)
		{
			GrowBufferData* ptr3 = this.BufData(i);
			if (ptr3->count > 0)
			{
				UnsafeUtility.MemCpy((void*)ptr2, ptr3->ptr, (long)(ptr3->count * num));
				ptr2 += (IntPtr)ptr3->count * (IntPtr)sizeof(T) / (IntPtr)sizeof(T);
			}
		}
		array2 = null;
		return array;
	}

	// Token: 0x040004E3 RID: 1251
	[NativeDisableUnsafePtrRestriction]
	public unsafe JobTLSData* data;
}
