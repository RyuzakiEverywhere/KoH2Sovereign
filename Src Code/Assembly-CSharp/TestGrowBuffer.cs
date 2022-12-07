using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class TestGrowBuffer : MonoBehaviour
{
	// Token: 0x06000560 RID: 1376 RVA: 0x0003C2B1 File Offset: 0x0003A4B1
	public Allocator GetAllocator()
	{
		if (this.ForceAllocator > Allocator.None)
		{
			return this.ForceAllocator;
		}
		return Allocator.TempJob;
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0003C2C4 File Offset: 0x0003A4C4
	public long Perform<T>(T job) where T : struct, IJobParallelFor
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		if (this.Managed)
		{
			if (this.SingleThreaded)
			{
				for (int j = 0; j < this.ItemCount; j++)
				{
					job.Execute(j);
				}
			}
			else
			{
				Parallel.For(0, this.ItemCount, delegate(int i)
				{
					job.Execute(i);
				});
			}
		}
		else if (this.SingleThreaded)
		{
			job.Run(this.ItemCount);
		}
		else
		{
			job.Schedule(this.ItemCount, 1, default(JobHandle)).Complete();
		}
		return stopwatch.ElapsedMilliseconds;
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0003C378 File Offset: 0x0003A578
	public void PerformTest(string type, Func<bool, long> test_func)
	{
		if (this.ExtensiveTest)
		{
			this.PerformExtensiveTest(type, test_func);
			return;
		}
		test_func(false);
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0003C394 File Offset: 0x0003A594
	public void PerformExtensiveTest(string type, Func<bool, long> test_func)
	{
		if (this.Repetitions <= 0)
		{
			return;
		}
		"Testing " + type;
		long num = 0L;
		long num2 = long.MaxValue;
		long num3 = long.MinValue;
		float num4 = 0f;
		try
		{
			for (int i = 0; i < this.Repetitions; i++)
			{
				long num5 = test_func(true);
				if (num5 < 0L)
				{
					break;
				}
				if (i != 0)
				{
					num4 += 1f;
					num += num5;
					if (num5 < num2)
					{
						num2 = num5;
					}
					if (num5 > num3)
					{
						num3 = num5;
					}
				}
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		if (num4 <= 0f)
		{
			return;
		}
		Debug.Log(string.Format("{0}: {1:F1} ({2} - {3}) ms", new object[]
		{
			type,
			(float)num / num4,
			num2,
			num3
		}));
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x0003C474 File Offset: 0x0003A674
	private bool Verify(int[] arr, string type)
	{
		Array.Sort<int>(arr);
		for (int i = 0; i < this.ItemCount; i++)
		{
			int num = arr[i];
			if (num != i)
			{
				Debug.LogError(string.Format("{0}: Wrong content: [{1}] = {2}", type, i, num));
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0003C4C0 File Offset: 0x0003A6C0
	public long TestWithGrowBuffer(bool extensive = false)
	{
		GrowBuffer<int> growBuffer = new GrowBuffer<int>(this.GetAllocator(), this.InitialCapacity);
		long num = this.Perform<TestGrowBuffer.FillGrowBufferJob>(new TestGrowBuffer.FillGrowBufferJob
		{
			buf = growBuffer
		});
		if (growBuffer.Count != this.ItemCount)
		{
			Debug.LogError(string.Format("GrowBuffer: Wrong count: {0} instead of {1}\n{2}", growBuffer.Count, this.ItemCount, growBuffer));
			return -1L;
		}
		string arg = growBuffer.ToString();
		int[] arr = growBuffer.ToArray();
		growBuffer.Dispose();
		if (!this.Verify(arr, "GrowBuffer"))
		{
			return -2L;
		}
		if (!extensive)
		{
			Debug.Log(string.Format("GrowBuffer: Filled in {0}ms\n{1}", num, arg));
		}
		return num;
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0003C584 File Offset: 0x0003A784
	public long TestWithMultiGrowBuffer(bool extensive = false)
	{
		if (this.Managed & !this.SingleThreaded)
		{
			Debug.LogError("MultiGrowBuffer cannot be tested Managed MultiThreaded");
			return -100L;
		}
		MultiGrowBuffer<int> buf = new MultiGrowBuffer<int>(this.GetAllocator(), 0);
		long num = this.Perform<TestGrowBuffer.FillMultiGrowBufferJob>(new TestGrowBuffer.FillMultiGrowBufferJob
		{
			buf = buf
		});
		int num2 = buf.CalcCount();
		if (num2 != this.ItemCount)
		{
			Debug.LogError(string.Format("MultiGrowBuffer: Wrong count: {0} instead of {1}", num2, this.ItemCount));
			return -1L;
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		int[] arr = buf.ToArray();
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		buf.Dispose();
		if (!this.Verify(arr, "MultiGrowBuffer"))
		{
			return -2L;
		}
		if (!extensive)
		{
			Debug.Log(string.Format("MultiGrowBuffer: Filled in {0}ms, Vectorized in {1}ms", num, elapsedMilliseconds));
		}
		return num + elapsedMilliseconds;
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0003C660 File Offset: 0x0003A860
	public long TestWithNativeList(bool extensive = false)
	{
		NativeList<int> nativeList = new NativeList<int>(this.ItemCount, this.GetAllocator());
		long num = this.Perform<TestGrowBuffer.FillNativeListJob>(new TestGrowBuffer.FillNativeListJob
		{
			writer = nativeList.AsParallelWriter()
		});
		if (nativeList.Length != this.ItemCount)
		{
			Debug.LogError(string.Format("NativeList: Wrong count: {0} instead of {1}", nativeList.Length, this.ItemCount));
			nativeList.Dispose();
			return -1L;
		}
		int[] arr = nativeList.ToArray();
		nativeList.Dispose();
		if (!this.Verify(arr, "NativeList"))
		{
			return -2L;
		}
		if (!extensive)
		{
			Debug.Log(string.Format("NativeList: Filled in {0}ms", num));
		}
		return num;
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0003C718 File Offset: 0x0003A918
	public long TestWithNativeQueue(bool extensive = false)
	{
		if (this.Managed & !this.SingleThreaded)
		{
			Debug.LogError("NativeQueue cannot be tested Managed MultiThreaded");
			return -100L;
		}
		Allocator allocator = this.GetAllocator();
		NativeQueue<int> nativeQueue = new NativeQueue<int>(allocator);
		long num = this.Perform<TestGrowBuffer.FillNativeQueueJob>(new TestGrowBuffer.FillNativeQueueJob
		{
			writer = nativeQueue.AsParallelWriter()
		});
		Stopwatch stopwatch = Stopwatch.StartNew();
		NativeArray<int> nativeArray = nativeQueue.ToArray(allocator);
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		nativeQueue.Dispose();
		if (nativeArray.Length != this.ItemCount)
		{
			Debug.LogError(string.Format("NativeQueue: Wrong count: {0} instead of {1}", nativeArray.Length, this.ItemCount));
			nativeArray.Dispose();
			return -1L;
		}
		int[] arr = nativeArray.ToArray();
		nativeArray.Dispose();
		if (!this.Verify(arr, "NativeQueue"))
		{
			return -2L;
		}
		if (!extensive)
		{
			Debug.Log(string.Format("NativeQueue: Filled in {0}ms, Vectorized in {1} ms", num, elapsedMilliseconds));
		}
		return num + elapsedMilliseconds;
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0003C814 File Offset: 0x0003AA14
	public unsafe long TestWithRawBuffer(bool extensive = false)
	{
		int[] array = new int[this.ItemCount];
		int num = 0;
		int[] array2;
		int* buf;
		if ((array2 = array) == null || array2.Length == 0)
		{
			buf = null;
		}
		else
		{
			buf = &array2[0];
		}
		long num2 = this.Perform<TestGrowBuffer.FillRawBufferJob>(new TestGrowBuffer.FillRawBufferJob
		{
			buf = buf,
			pcount = &num,
			capacity = this.ItemCount
		});
		array2 = null;
		if (num != this.ItemCount)
		{
			Debug.LogError(string.Format("Raw buffer: Wrong count: {0} instead of {1}", num, this.ItemCount));
			return -1L;
		}
		if (!this.Verify(array, "Raw buffer"))
		{
			return -2L;
		}
		if (!extensive)
		{
			Debug.Log(string.Format("Raw buffer: Filled in {0}ms", num2));
		}
		return num2;
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0003C8D4 File Offset: 0x0003AAD4
	public void TestAll()
	{
		this.PerformTest("GrowBuffer", new Func<bool, long>(this.TestWithGrowBuffer));
		this.PerformTest("MultiGrowBuffer", new Func<bool, long>(this.TestWithMultiGrowBuffer));
		this.PerformTest("NativeList", new Func<bool, long>(this.TestWithNativeList));
		this.PerformTest("NativeQueue", new Func<bool, long>(this.TestWithNativeQueue));
		this.PerformTest("Raw buffer", new Func<bool, long>(this.TestWithRawBuffer));
	}

	// Token: 0x04000501 RID: 1281
	public int ItemCount = 100000;

	// Token: 0x04000502 RID: 1282
	public int InitialCapacity;

	// Token: 0x04000503 RID: 1283
	public Allocator ForceAllocator;

	// Token: 0x04000504 RID: 1284
	public bool SingleThreaded;

	// Token: 0x04000505 RID: 1285
	public bool Managed;

	// Token: 0x04000506 RID: 1286
	public bool ExtensiveTest;

	// Token: 0x04000507 RID: 1287
	public int Repetitions = 1000;

	// Token: 0x02000559 RID: 1369
	[BurstCompile]
	private struct FillGrowBufferJob : IJobParallelFor
	{
		// Token: 0x060043A9 RID: 17321 RVA: 0x001FE1B4 File Offset: 0x001FC3B4
		public void Execute(int index)
		{
			this.buf.Add(index);
		}

		// Token: 0x0400300F RID: 12303
		public GrowBuffer<int> buf;
	}

	// Token: 0x0200055A RID: 1370
	[BurstCompile]
	private struct FillMultiGrowBufferJob : IJobParallelFor
	{
		// Token: 0x060043AA RID: 17322 RVA: 0x001FE1C2 File Offset: 0x001FC3C2
		public void Execute(int index)
		{
			this.buf.Add(ref index, this.thread_idx);
		}

		// Token: 0x04003010 RID: 12304
		public MultiGrowBuffer<int> buf;

		// Token: 0x04003011 RID: 12305
		[NativeSetThreadIndex]
		public int thread_idx;
	}

	// Token: 0x0200055B RID: 1371
	[BurstCompile]
	private struct FillNativeListJob : IJobParallelFor
	{
		// Token: 0x060043AB RID: 17323 RVA: 0x001FE1D7 File Offset: 0x001FC3D7
		public void Execute(int index)
		{
			this.writer.AddNoResize(index);
		}

		// Token: 0x04003012 RID: 12306
		public NativeList<int>.ParallelWriter writer;
	}

	// Token: 0x0200055C RID: 1372
	[BurstCompile]
	private struct FillNativeQueueJob : IJobParallelFor
	{
		// Token: 0x060043AC RID: 17324 RVA: 0x001FE1E5 File Offset: 0x001FC3E5
		public void Execute(int index)
		{
			this.writer.Enqueue(index);
		}

		// Token: 0x04003013 RID: 12307
		public NativeQueue<int>.ParallelWriter writer;
	}

	// Token: 0x0200055D RID: 1373
	[BurstCompile]
	private struct FillRawBufferJob : IJobParallelFor
	{
		// Token: 0x060043AD RID: 17325 RVA: 0x001FE1F4 File Offset: 0x001FC3F4
		public unsafe void Execute(int index)
		{
			int num = Interlocked.Increment(ref *this.pcount) - 1;
			if (num < this.capacity)
			{
				this.buf[num] = index;
			}
		}

		// Token: 0x04003014 RID: 12308
		[NativeDisableUnsafePtrRestriction]
		public unsafe int* buf;

		// Token: 0x04003015 RID: 12309
		[NativeDisableUnsafePtrRestriction]
		public unsafe int* pcount;

		// Token: 0x04003016 RID: 12310
		public int capacity;
	}
}
