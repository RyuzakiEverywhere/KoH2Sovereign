using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class Benchmark : MonoBehaviour
{
	// Token: 0x17000004 RID: 4
	// (get) Token: 0x0600019B RID: 411 RVA: 0x0000FD1C File Offset: 0x0000DF1C
	// (set) Token: 0x0600019C RID: 412 RVA: 0x0000FD23 File Offset: 0x0000DF23
	public static Benchmark Instance { get; private set; }

	// Token: 0x0600019D RID: 413 RVA: 0x0000FD2C File Offset: 0x0000DF2C
	private void Awake()
	{
		if (Benchmark.Instance != null)
		{
			Common.DestroyObj(base.gameObject);
		}
		Benchmark.Instance = this;
		this.benchmarkCamera.enabled = false;
		base.transform.SetParent(null);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600019E RID: 414 RVA: 0x0000FD7A File Offset: 0x0000DF7A
	// (set) Token: 0x0600019F RID: 415 RVA: 0x0000FD81 File Offset: 0x0000DF81
	public static Benchmark.BenchmarkResult LastResult { get; private set; }

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000FD89 File Offset: 0x0000DF89
	public GPUBenchmark GPUBenchmark
	{
		get
		{
			return this.gpuBenchmark;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000FD91 File Offset: 0x0000DF91
	public CPUBenchmark CPUBenchmark
	{
		get
		{
			return this.cpuBenchmark;
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x060001A2 RID: 418 RVA: 0x0000FD99 File Offset: 0x0000DF99
	public bool IsBenchmarkRunning
	{
		get
		{
			return this.benchmarkingCoroutine != null;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000FDA4 File Offset: 0x0000DFA4
	public bool IsBenchmarkDone
	{
		get
		{
			return Benchmark.LastResult != null;
		}
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000FDAE File Offset: 0x0000DFAE
	public void Run(Action<Benchmark.BenchmarkResult> onComplete = null)
	{
		if (this.benchmarkingCoroutine != null)
		{
			this.onBenchmarkComplete = (Action<Benchmark.BenchmarkResult>)Delegate.Combine(this.onBenchmarkComplete, onComplete);
			return;
		}
		this.onBenchmarkComplete = onComplete;
		this.benchmarkingCoroutine = base.StartCoroutine(this.BenchmarkingCoroutine());
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000FDE9 File Offset: 0x0000DFE9
	private IEnumerator BenchmarkingCoroutine()
	{
		bool cpuBenchDone = false;
		this.cpuBenchmark.Run(delegate(CPUBenchmark _)
		{
			cpuBenchDone = true;
		});
		yield return new WaitWhile(() => !cpuBenchDone);
		yield return null;
		this.benchmarkCamera.enabled = true;
		yield return null;
		bool gpuBenchDone = false;
		this.gpuBenchmark.Run(delegate(GPUBenchmark _)
		{
			gpuBenchDone = true;
		});
		yield return new WaitWhile(() => !gpuBenchDone);
		yield return null;
		this.benchmarkCamera.enabled = false;
		Benchmark.BenchmarkResult benchmarkResult = new Benchmark.BenchmarkResult
		{
			cpu_allication_ms = this.cpuBenchmark.allocation_test_result_ms,
			cpu_branch_ms = this.cpuBenchmark.branch_test_result_ms,
			cpu_compute_ms = this.cpuBenchmark.compute_test_result_ms,
			cpu_sort_ms = this.cpuBenchmark.sort_test_result_ms,
			cpu_multithread_branch_ms = this.cpuBenchmark.branch_multithread_test_result_ms,
			cpu_multithread_compute_ms = this.cpuBenchmark.compute_multithread_test_result_ms,
			cpu_multithread_sort_ms = this.cpuBenchmark.sort_multithread_test_result_ms,
			cpu_average_ms = this.cpuBenchmark.last_result_ms + this.cpuBenchmark.last_multithread_result_ms,
			cpu_rating = this.cpuBenchmark.rating_0_10,
			gpu_average_ms = this.gpuBenchmark.last_result_ms,
			gpu_rating = this.gpuBenchmark.rating_0_10,
			cpu_name = SystemInfo.processorType,
			cpu_frequency = SystemInfo.processorFrequency.ToString(),
			cpu_processor_count = SystemInfo.processorCount.ToString(),
			gpu_device_version = SystemInfo.graphicsDeviceVersion,
			gpu_device_name = SystemInfo.graphicsDeviceName,
			gpu_device_type = SystemInfo.graphicsDeviceType.ToString(),
			gpu_device_vendor = SystemInfo.graphicsDeviceVendor.ToString(),
			gpu_device_vendor_id = SystemInfo.graphicsDeviceVendorID.ToString(),
			gpu_multithreaded = SystemInfo.graphicsMultiThreaded.ToString()
		};
		Benchmark.LastResult = benchmarkResult;
		Action<Benchmark.BenchmarkResult> action = this.onBenchmarkComplete;
		if (action != null)
		{
			action(benchmarkResult);
		}
		this.onBenchmarkComplete = null;
		this.benchmarkingCoroutine = null;
		yield break;
	}

	// Token: 0x040002B6 RID: 694
	[SerializeField]
	private GPUBenchmark gpuBenchmark;

	// Token: 0x040002B7 RID: 695
	[SerializeField]
	private CPUBenchmark cpuBenchmark;

	// Token: 0x040002B8 RID: 696
	[SerializeField]
	private Camera benchmarkCamera;

	// Token: 0x040002B9 RID: 697
	private Action<Benchmark.BenchmarkResult> onBenchmarkComplete;

	// Token: 0x040002BA RID: 698
	private Coroutine benchmarkingCoroutine;

	// Token: 0x02000503 RID: 1283
	[Serializable]
	public class BenchmarkResult
	{
		// Token: 0x04002E88 RID: 11912
		public float cpu_allication_ms;

		// Token: 0x04002E89 RID: 11913
		public float cpu_branch_ms;

		// Token: 0x04002E8A RID: 11914
		public float cpu_compute_ms;

		// Token: 0x04002E8B RID: 11915
		public float cpu_sort_ms;

		// Token: 0x04002E8C RID: 11916
		public float cpu_multithread_branch_ms;

		// Token: 0x04002E8D RID: 11917
		public float cpu_multithread_compute_ms;

		// Token: 0x04002E8E RID: 11918
		public float cpu_multithread_sort_ms;

		// Token: 0x04002E8F RID: 11919
		public float cpu_average_ms;

		// Token: 0x04002E90 RID: 11920
		public float gpu_average_ms;

		// Token: 0x04002E91 RID: 11921
		public float cpu_rating;

		// Token: 0x04002E92 RID: 11922
		public float gpu_rating;

		// Token: 0x04002E93 RID: 11923
		public string cpu_name;

		// Token: 0x04002E94 RID: 11924
		public string cpu_frequency;

		// Token: 0x04002E95 RID: 11925
		public string cpu_processor_count;

		// Token: 0x04002E96 RID: 11926
		public string gpu_device_version;

		// Token: 0x04002E97 RID: 11927
		public string gpu_device_name;

		// Token: 0x04002E98 RID: 11928
		public string gpu_device_type;

		// Token: 0x04002E99 RID: 11929
		public string gpu_device_vendor;

		// Token: 0x04002E9A RID: 11930
		public string gpu_device_vendor_id;

		// Token: 0x04002E9B RID: 11931
		public string gpu_multithreaded;
	}
}
