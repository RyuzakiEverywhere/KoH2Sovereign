using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

// Token: 0x0200008B RID: 139
public struct JobTLS<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x0600053F RID: 1343 RVA: 0x0003B398 File Offset: 0x00039598
	public unsafe static T* Ptr(JobTLSData* data, int thread_idx)
	{
		int num = 64;
		return (T*)(data + (num + thread_idx * data->item_size) / sizeof(JobTLSData));
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0003B3B8 File Offset: 0x000395B8
	public unsafe T* Ptr(int thread_idx)
	{
		return JobTLS<T>.Ptr(this.data, thread_idx);
	}

	// Token: 0x17000046 RID: 70
	public unsafe T this[int thread_idx]
	{
		get
		{
			return ref *this.Ptr(thread_idx);
		}
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x06000542 RID: 1346 RVA: 0x0003B391 File Offset: 0x00039591
	public int Count
	{
		get
		{
			return 128;
		}
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0003B3D0 File Offset: 0x000395D0
	public unsafe JobTLS(Allocator allocator, T def_val = default(T))
	{
		int num = 64;
		int num2 = UnsafeUtility.SizeOf<T>();
		int num3 = num2 % num;
		if (num3 != 0)
		{
			num2 += num - num3;
		}
		int num4 = 128;
		this.data = (JobTLSData*)UnsafeUtility.Malloc((long)(num + num2 * (num4 + 1)), num, allocator);
		this.data->allocator = allocator;
		this.data->item_size = num2;
		for (int i = 0; i <= num4; i++)
		{
			*this[i] = def_val;
		}
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0003B444 File Offset: 0x00039644
	public unsafe JobTLS(void* data)
	{
		this.data = (JobTLSData*)data;
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0003B44D File Offset: 0x0003964D
	public unsafe JobTLS(IntPtr data)
	{
		this.data = (JobTLSData*)((void*)data);
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0003B45B File Offset: 0x0003965B
	public unsafe IntPtr ToIntPtr()
	{
		return (IntPtr)((void*)this.data);
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x0003B468 File Offset: 0x00039668
	public void Dispose()
	{
		JobTLS<T>.Dispose(this.data);
		this.data = null;
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x0003B480 File Offset: 0x00039680
	public unsafe static void Dispose(JobTLSData* data)
	{
		if (data == null)
		{
			return;
		}
		Allocator allocator = data->allocator;
		if (allocator <= Allocator.None)
		{
			return;
		}
		data->allocator = Allocator.None;
		UnsafeUtility.Free((void*)data, allocator);
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0003B4B0 File Offset: 0x000396B0
	public unsafe static void Test()
	{
		JobTLS<int> jobTLS = new JobTLS<int>(Allocator.TempJob, 0);
		new JobTLS<T>.TestJob
		{
			tls_data = jobTLS.data
		}.Schedule(1000000, 1, default(JobHandle)).Complete();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 16; i++)
		{
			int num3 = *jobTLS[i];
			if (num3 != 0)
			{
				num += num3;
				num2++;
			}
		}
		Debug.Log(string.Format("sum: {0}, cnt: {1}", num, num2));
		jobTLS.Dispose();
	}

	// Token: 0x040004E2 RID: 1250
	[NativeDisableUnsafePtrRestriction]
	public unsafe JobTLSData* data;

	// Token: 0x02000558 RID: 1368
	[BurstCompile]
	private struct TestJob : IJobParallelFor
	{
		// Token: 0x060043A8 RID: 17320 RVA: 0x001FE188 File Offset: 0x001FC388
		public unsafe void Execute(int index)
		{
			JobTLS<int> jobTLS = new JobTLS<int>((void*)this.tls_data);
			(*jobTLS[this.thread_idx])++;
		}

		// Token: 0x0400300D RID: 12301
		[NativeDisableUnsafePtrRestriction]
		public unsafe JobTLSData* tls_data;

		// Token: 0x0400300E RID: 12302
		[NativeSetThreadIndex]
		public int thread_idx;
	}
}
