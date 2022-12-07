using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000099 RID: 153
public class TestBurst : MonoBehaviour
{
	// Token: 0x0600057A RID: 1402 RVA: 0x0003CE88 File Offset: 0x0003B088
	public void Test(string mode)
	{
		Terrain component = base.GetComponent<Terrain>();
		if (component == null)
		{
			return;
		}
		this.td = component.terrainData;
		try
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.sd1 = TestBurst.ImportSplats(this.td1, false);
			this.sd2 = TestBurst.ImportSplats(this.td2, false);
			this.sd = TestBurst.ImportSplats(this.td, true);
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			this.BuildRemaps();
			this.Run(mode);
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			TestBurst.ExportSplats(this.td, this.sd);
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("{0}: Import: {1}ms, Run: {2}ms, Export: {3}ms", new object[]
			{
				mode,
				elapsedMilliseconds,
				elapsedMilliseconds2,
				elapsedMilliseconds3
			}));
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		this.sd1.Dispose();
		this.sd2.Dispose();
		this.sd.Dispose();
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0003CF9C File Offset: 0x0003B19C
	public static void DisposeHandle(IntPtr h)
	{
		if (h == IntPtr.Zero)
		{
			return;
		}
		GCHandle gchandle = GCHandle.FromIntPtr(h);
		AllocationManager.Free(ref gchandle);
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0003CFC8 File Offset: 0x0003B1C8
	public unsafe static TestBurst.SplatsData ImportSplats(TerrainData td, bool empty = false)
	{
		TestBurst.SplatsData splatsData = default(TestBurst.SplatsData);
		splatsData.resolution = td.alphamapResolution;
		splatsData.layers = td.alphamapLayers;
		float[,,] obj;
		if (empty)
		{
			obj = new float[splatsData.resolution, splatsData.resolution, splatsData.layers];
		}
		else
		{
			obj = td.GetAlphamaps(0, 0, splatsData.resolution, splatsData.resolution);
		}
		GCHandle value = AllocationManager.AllocPinned(obj);
		splatsData.alphas = (float*)((void*)value.AddrOfPinnedObject());
		splatsData.h_alphas = GCHandle.ToIntPtr(value);
		return splatsData;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0003D054 File Offset: 0x0003B254
	public static void ExportSplats(TerrainData td, TestBurst.SplatsData sd)
	{
		if (sd.h_alphas == IntPtr.Zero)
		{
			return;
		}
		float[,,] map = (float[,,])GCHandle.FromIntPtr(sd.h_alphas).Target;
		td.SetAlphamaps(0, 0, map);
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x000023FD File Offset: 0x000005FD
	private void BuildRemaps()
	{
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0003D098 File Offset: 0x0003B298
	private void Run(string mode)
	{
		TestBurst.MixJob job = default(TestBurst.MixJob);
		job.sd1 = this.sd1;
		job.sd2 = this.sd2;
		job.sd = this.sd;
		job.strength = this.strength;
		if (mode == "Single Threaded")
		{
			for (int i = 0; i < this.sd.resolution; i++)
			{
				job.Execute(i);
			}
			return;
		}
		if (mode == "Burst Single Threaded")
		{
			job.Run(this.sd.resolution);
			return;
		}
		if (mode == "Parallel For")
		{
			Parallel.For(0, this.sd.resolution, delegate(int y)
			{
				job.Execute(y);
			});
			return;
		}
		if (mode == "Jobs")
		{
			job.Schedule(this.sd.resolution, 16, default(JobHandle)).Complete();
			return;
		}
		if (mode == "Callbacks Single Threaded")
		{
			this.RunWithCallbacks(true);
			return;
		}
		if (!(mode == "Callbacks Jobs"))
		{
			return;
		}
		this.RunWithCallbacks(false);
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0003D1E4 File Offset: 0x0003B3E4
	private unsafe void RunWithCallbacks(bool single_threaded)
	{
		fixed (TestBurst.SplatsData* ptr = &this.sd1)
		{
			TestBurst.SplatsData* data = ptr;
			fixed (TestBurst.SplatsData* ptr2 = &this.sd2)
			{
				TestBurst.SplatsData* data2 = ptr2;
				TestBurst.MixData mixData = default(TestBurst.MixData);
				mixData.in1 = TestBurst.SplatsData.GetCB(data);
				mixData.in2 = TestBurst.SplatsData.GetCB(data2);
				mixData.strength = this.strength;
				TestBurst.CalcAllJob jobData = default(TestBurst.CalcAllJob);
				jobData.sd = this.sd;
				jobData.cb = TestBurst.MixData.GetCB(&mixData);
				if (single_threaded)
				{
					jobData.Run(this.sd.resolution);
				}
				else
				{
					jobData.Schedule(this.sd.resolution, 16, default(JobHandle)).Complete();
				}
			}
		}
	}

	// Token: 0x04000517 RID: 1303
	public TerrainData td1;

	// Token: 0x04000518 RID: 1304
	public TerrainData td2;

	// Token: 0x04000519 RID: 1305
	public float strength = 0.5f;

	// Token: 0x0400051A RID: 1306
	private TerrainData td;

	// Token: 0x0400051B RID: 1307
	private TestBurst.SplatsData sd1;

	// Token: 0x0400051C RID: 1308
	private TestBurst.SplatsData sd2;

	// Token: 0x0400051D RID: 1309
	private TestBurst.SplatsData sd;

	// Token: 0x0200055F RID: 1375
	// (Invoke) Token: 0x060043B1 RID: 17329
	public unsafe delegate float GetSplatFunc(int x, int y, int l, void* data);

	// Token: 0x02000560 RID: 1376
	[Serializable]
	public struct GetSplatCB
	{
		// Token: 0x060043B4 RID: 17332 RVA: 0x001FE23C File Offset: 0x001FC43C
		public float Get(int x, int y, int l)
		{
			FunctionPointer<TestBurst.GetSplatFunc> functionPointer = new FunctionPointer<TestBurst.GetSplatFunc>(this.get_func);
			return functionPointer.Invoke(x, y, l, this.data);
		}

		// Token: 0x04003018 RID: 12312
		[NativeDisableUnsafePtrRestriction]
		public IntPtr get_func;

		// Token: 0x04003019 RID: 12313
		[NativeDisableUnsafePtrRestriction]
		public unsafe void* data;
	}

	// Token: 0x02000561 RID: 1377
	[BurstCompile(CompileSynchronously = true)]
	public struct SplatsData : IDisposable
	{
		// Token: 0x060043B5 RID: 17333 RVA: 0x001FE26C File Offset: 0x001FC46C
		public unsafe float Get(int x, int y, int l)
		{
			if (l >= this.layers)
			{
				return 0f;
			}
			x %= this.resolution;
			y %= this.resolution;
			return this.alphas[y * this.resolution * this.layers + x * this.layers + l];
		}

		// Token: 0x060043B6 RID: 17334 RVA: 0x001FE2C1 File Offset: 0x001FC4C1
		public unsafe void Set(int x, int y, int l, float a)
		{
			this.alphas[y * this.resolution * this.layers + x * this.layers + l] = a;
		}

		// Token: 0x060043B7 RID: 17335 RVA: 0x001FE2EC File Offset: 0x001FC4EC
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetFunc(int x, int y, int l, void* data)
		{
			return ((TestBurst.SplatsData*)data)->Get(x, y, l);
		}

		// Token: 0x060043B8 RID: 17336 RVA: 0x001FE304 File Offset: 0x001FC504
		public unsafe static TestBurst.GetSplatCB GetCB(TestBurst.SplatsData* data)
		{
			return new TestBurst.GetSplatCB
			{
				get_func = TestBurst.SplatsData.get_func,
				data = (void*)data
			};
		}

		// Token: 0x060043B9 RID: 17337 RVA: 0x001FE32E File Offset: 0x001FC52E
		public void Dispose()
		{
			TestBurst.DisposeHandle(this.h_remap);
			TestBurst.DisposeHandle(this.h_alphas);
		}

		// Token: 0x0400301A RID: 12314
		public int resolution;

		// Token: 0x0400301B RID: 12315
		public int layers;

		// Token: 0x0400301C RID: 12316
		[NativeDisableUnsafePtrRestriction]
		public unsafe int* remap;

		// Token: 0x0400301D RID: 12317
		[NativeDisableUnsafePtrRestriction]
		public unsafe float* alphas;

		// Token: 0x0400301E RID: 12318
		[NativeDisableUnsafePtrRestriction]
		public IntPtr h_remap;

		// Token: 0x0400301F RID: 12319
		[NativeDisableUnsafePtrRestriction]
		public IntPtr h_alphas;

		// Token: 0x04003020 RID: 12320
		public static IntPtr get_func = BurstCompiler.CompileFunctionPointer<TestBurst.GetSplatFunc>(new TestBurst.GetSplatFunc(TestBurst.SplatsData.GetFunc)).Value;
	}

	// Token: 0x02000562 RID: 1378
	[BurstCompile(CompileSynchronously = true)]
	private struct MixJob : IJobParallelFor
	{
		// Token: 0x060043BB RID: 17339 RVA: 0x001FE374 File Offset: 0x001FC574
		public void Execute(int y)
		{
			for (int i = 0; i < this.sd.resolution; i++)
			{
				for (int j = 0; j < this.sd.layers; j++)
				{
					float num = this.sd1.Get(i, y, j);
					float num2 = this.sd2.Get(i, y, j);
					float a = this.strength * num + (1f - this.strength) * num2;
					this.sd.Set(i, y, j, a);
				}
			}
		}

		// Token: 0x04003021 RID: 12321
		public TestBurst.SplatsData sd1;

		// Token: 0x04003022 RID: 12322
		public TestBurst.SplatsData sd2;

		// Token: 0x04003023 RID: 12323
		public TestBurst.SplatsData sd;

		// Token: 0x04003024 RID: 12324
		public float strength;
	}

	// Token: 0x02000563 RID: 1379
	[BurstCompile(CompileSynchronously = true)]
	public struct MixData
	{
		// Token: 0x060043BC RID: 17340 RVA: 0x001FE3F4 File Offset: 0x001FC5F4
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetFunc(int x, int y, int l, void* data)
		{
			float num = ((TestBurst.MixData*)data)->in1.Get(x, y, l);
			float num2 = ((TestBurst.MixData*)data)->in2.Get(x, y, l);
			return ((TestBurst.MixData*)data)->strength * num + (1f - ((TestBurst.MixData*)data)->strength) * num2;
		}

		// Token: 0x060043BD RID: 17341 RVA: 0x001FE438 File Offset: 0x001FC638
		public unsafe static TestBurst.GetSplatCB GetCB(TestBurst.MixData* data)
		{
			return new TestBurst.GetSplatCB
			{
				get_func = TestBurst.MixData.get_func,
				data = (void*)data
			};
		}

		// Token: 0x04003025 RID: 12325
		public TestBurst.GetSplatCB in1;

		// Token: 0x04003026 RID: 12326
		public TestBurst.GetSplatCB in2;

		// Token: 0x04003027 RID: 12327
		public float strength;

		// Token: 0x04003028 RID: 12328
		public static IntPtr get_func = BurstCompiler.CompileFunctionPointer<TestBurst.GetSplatFunc>(new TestBurst.GetSplatFunc(TestBurst.MixData.GetFunc)).Value;
	}

	// Token: 0x02000564 RID: 1380
	[BurstCompile(CompileSynchronously = true)]
	private struct CalcAllJob : IJobParallelFor
	{
		// Token: 0x060043BF RID: 17343 RVA: 0x001FE490 File Offset: 0x001FC690
		public void Execute(int y)
		{
			for (int i = 0; i < this.sd.resolution; i++)
			{
				for (int j = 0; j < this.sd.layers; j++)
				{
					float a = this.cb.Get(i, y, j);
					this.sd.Set(i, y, j, a);
				}
			}
		}

		// Token: 0x04003029 RID: 12329
		public TestBurst.SplatsData sd;

		// Token: 0x0400302A RID: 12330
		public TestBurst.GetSplatCB cb;
	}
}
