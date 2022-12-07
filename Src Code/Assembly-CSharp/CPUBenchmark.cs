using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class CPUBenchmark : MonoBehaviour
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000FDF8 File Offset: 0x0000DFF8
	public float allocation_test_result_ms
	{
		get
		{
			if (this.gathered_metrics.Count != 0)
			{
				return this.gathered_metrics.Average((CPUBenchmark.TestMetrics m) => m.allocation_ms);
			}
			return -1f;
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000FE37 File Offset: 0x0000E037
	public float branch_test_result_ms
	{
		get
		{
			if (this.gathered_metrics.Count != 0)
			{
				return this.gathered_metrics.Average((CPUBenchmark.TestMetrics m) => m.branch_ms);
			}
			return -1f;
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x060001AA RID: 426 RVA: 0x0000FE76 File Offset: 0x0000E076
	public float compute_test_result_ms
	{
		get
		{
			if (this.gathered_metrics.Count != 0)
			{
				return this.gathered_metrics.Average((CPUBenchmark.TestMetrics m) => m.compute_ms);
			}
			return -1f;
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x060001AB RID: 427 RVA: 0x0000FEB5 File Offset: 0x0000E0B5
	public float sort_test_result_ms
	{
		get
		{
			if (this.gathered_metrics.Count != 0)
			{
				return this.gathered_metrics.Average((CPUBenchmark.TestMetrics m) => m.sort_ms);
			}
			return -1f;
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x060001AC RID: 428 RVA: 0x0000FEF4 File Offset: 0x0000E0F4
	public float last_result_ms
	{
		get
		{
			if (this.gathered_metrics.Count != 0)
			{
				return this.allocation_test_result_ms + this.branch_test_result_ms + this.compute_test_result_ms + this.sort_test_result_ms;
			}
			return -1f;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x060001AD RID: 429 RVA: 0x0000FF24 File Offset: 0x0000E124
	public float branch_multithread_test_result_ms
	{
		get
		{
			if (this.gathered_multithread_metrics.Count != 0)
			{
				return this.gathered_multithread_metrics.Average((CPUBenchmark.TestMetrics m) => m.branch_ms);
			}
			return -1f;
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060001AE RID: 430 RVA: 0x0000FF63 File Offset: 0x0000E163
	public float compute_multithread_test_result_ms
	{
		get
		{
			if (this.gathered_multithread_metrics.Count != 0)
			{
				return this.gathered_multithread_metrics.Average((CPUBenchmark.TestMetrics m) => m.compute_ms);
			}
			return -1f;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060001AF RID: 431 RVA: 0x0000FFA2 File Offset: 0x0000E1A2
	public float sort_multithread_test_result_ms
	{
		get
		{
			if (this.gathered_multithread_metrics.Count != 0)
			{
				return this.gathered_multithread_metrics.Average((CPUBenchmark.TestMetrics m) => m.sort_ms);
			}
			return -1f;
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000FFE1 File Offset: 0x0000E1E1
	public float last_multithread_result_ms
	{
		get
		{
			if (this.gathered_multithread_metrics.Count != 0)
			{
				return this.branch_multithread_test_result_ms + this.compute_multithread_test_result_ms + this.sort_multithread_test_result_ms;
			}
			return -1f;
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060001B1 RID: 433 RVA: 0x0001000C File Offset: 0x0000E20C
	public float rating_0_10
	{
		get
		{
			if (this.gathered_metrics.Count != 0)
			{
				return Mathf.Clamp(this.assets.RatingCurve.Evaluate(this.last_result_ms + this.last_multithread_result_ms), 0f, 10f);
			}
			return -1f;
		}
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x00010058 File Offset: 0x0000E258
	[ContextMenu("Run")]
	private void _Run()
	{
		this.Run(null);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x00010064 File Offset: 0x0000E264
	public void Run(Action<CPUBenchmark> on_complete = null)
	{
		this.processor_count = SystemInfo.processorCount;
		if (this.is_test_running)
		{
			this.on_complete_action = (Action<CPUBenchmark>)Delegate.Combine(this.on_complete_action, on_complete);
			return;
		}
		this.on_complete_action = on_complete;
		this.gathered_metrics.Clear();
		this.gathered_multithread_metrics.Clear();
		this.is_test_running = true;
		this.test_index = 0;
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x000100C8 File Offset: 0x0000E2C8
	private void Update()
	{
		if (this.is_test_running)
		{
			if (this.test_index >= 10)
			{
				this.gathered_metrics.Add(this.RunSingleTest());
				this.gathered_multithread_metrics.Add(this.RunSingleMultithreadTest());
			}
			else
			{
				this.RunSingleTest();
				this.RunSingleMultithreadTest();
			}
			this.test_index++;
			if (this.test_index >= 60)
			{
				this.is_test_running = false;
				Action<CPUBenchmark> action = this.on_complete_action;
				if (action != null)
				{
					action(this);
				}
				new StringBuilder();
			}
		}
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x00010150 File Offset: 0x0000E350
	private CPUBenchmark.TestMetrics RunSingleTest()
	{
		Random random = new Random();
		float allocation_ms = this.MeasureMethodExecution(delegate
		{
			this.AllocationTest(random, 64);
		});
		float branch_ms = this.MeasureMethodExecution(delegate
		{
			this.RandomBranchingTest(8000);
		});
		float compute_ms = this.MeasureMethodExecution(delegate
		{
			this.ComputeTest(12000);
		});
		float sort_ms = this.MeasureMethodExecution(delegate
		{
			List<int> list = this.GenerateRandomList(14336, random);
			this.SortTest(list);
		});
		return new CPUBenchmark.TestMetrics
		{
			allocation_ms = allocation_ms,
			branch_ms = branch_ms,
			compute_ms = compute_ms,
			sort_ms = sort_ms
		};
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x000101F0 File Offset: 0x0000E3F0
	private CPUBenchmark.TestMetrics RunSingleMultithreadTest()
	{
		new Random();
		float branch_ms = this.MeasureMethodExecutionMultithread(delegate
		{
			this.RandomBranchingTest(8000 / this.processor_count);
		});
		float compute_ms = this.MeasureMethodExecutionMultithread(delegate
		{
			this.ComputeTest(12000 / this.processor_count);
		});
		float sort_ms = this.MeasureMethodExecutionMultithread(delegate
		{
			Random random = new Random();
			List<int> list = this.GenerateRandomList(14336 / this.processor_count, random);
			this.SortTest(list);
		});
		return new CPUBenchmark.TestMetrics
		{
			allocation_ms = 0f,
			branch_ms = branch_ms,
			compute_ms = compute_ms,
			sort_ms = sort_ms
		};
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0001026C File Offset: 0x0000E46C
	private List<int> GenerateRandomList(int length, Random random)
	{
		List<int> list = new List<int>(length);
		for (int i = 0; i < length; i++)
		{
			list.Add(random.Next());
		}
		return list;
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x00010299 File Offset: 0x0000E499
	private void SortTest(List<int> list)
	{
		list.Sort();
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x000102A4 File Offset: 0x0000E4A4
	private void ShuffleList(List<int> list, Random random)
	{
		for (int i = 0; i < list.Count; i++)
		{
			int index = random.Next() % (list.Count - 1);
			int value = list[i];
			list[i] = list[index];
			list[index] = value;
		}
	}

	// Token: 0x060001BA RID: 442 RVA: 0x000102F0 File Offset: 0x0000E4F0
	[MethodImpl(MethodImplOptions.NoOptimization)]
	private void ComputeTest(int count)
	{
		float num = 0f;
		for (int i = 0; i < count; i++)
		{
			num += Mathf.Atan(Mathf.Tan(Mathf.Atan(Mathf.Tan((float)i)))) * 0.001f;
		}
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00010330 File Offset: 0x0000E530
	[MethodImpl(MethodImplOptions.NoOptimization)]
	private void RandomBranchingTest(int count)
	{
		float num = 0f;
		Random random = new Random();
		for (int i = 0; i < count; i++)
		{
			num += (float)this.RandomBranchingMethod(random) / 100000f;
		}
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00010368 File Offset: 0x0000E568
	[MethodImpl(MethodImplOptions.NoOptimization)]
	private void AllocationTest(Random random, int count)
	{
		float num = 0f;
		for (int i = 0; i < count; i++)
		{
			int num2 = random.Next();
			List<int> list = new List<int>();
			for (int j = 0; j < 514; j++)
			{
				list.Add(num2);
				list.Add(num2 + 2);
				list.Add(num2 + 4);
				num2 += num2 % 100;
			}
			num += (float)(list.Last<int>() % 2 + 1);
		}
	}

	// Token: 0x060001BD RID: 445 RVA: 0x000103D8 File Offset: 0x0000E5D8
	private float MeasureMethodExecution(Action action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		if (action != null)
		{
			action();
		}
		float result = (float)stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Stop();
		return result;
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0001040C File Offset: 0x0000E60C
	private float MeasureMethodExecutionMultithread(Action action)
	{
		int processorCount = SystemInfo.processorCount;
		float[] times = new float[processorCount];
		Stopwatch stopwatch = Stopwatch.StartNew();
		Parallel.For(0, this.processor_count, delegate(int threadIndex)
		{
			Stopwatch stopwatch2 = Stopwatch.StartNew();
			Action action2 = action;
			if (action2 != null)
			{
				action2();
			}
			times[threadIndex] = (float)stopwatch2.Elapsed.TotalMilliseconds;
			stopwatch2.Stop();
		});
		float result = (float)stopwatch.Elapsed.TotalMilliseconds + times.Min() * 1E-06f;
		stopwatch.Stop();
		return result;
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00010480 File Offset: 0x0000E680
	[ContextMenu("LogRandomBranchCode")]
	private void LogRandomBranchCode()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("\tlong RandomBranchingMethod(System.Random random)");
		stringBuilder.AppendLine("{");
		stringBuilder.AppendLine("long y = 0;");
		stringBuilder.AppendLine("int x = random.Next();");
		this.BuildIfStatementCodeRecursive(new Random(), stringBuilder, 1.0);
		stringBuilder.AppendLine("return y;");
		stringBuilder.AppendLine("}");
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x000104FC File Offset: 0x0000E6FC
	private void BuildIfStatementCodeRecursive(Random random, StringBuilder sb, double nestProbability)
	{
		int num = random.Next() % 8 + 2;
		for (int i = 0; i < num; i++)
		{
			int num2 = random.Next() % 10 + 2;
			sb.AppendLine(string.Format("if (x % {0} == {1})", num2, random.Next() % num2));
			sb.AppendLine("{");
			sb.AppendLine(string.Format("y += x % {0};", random.Next() % num2 + 2));
			if (random.NextDouble() < nestProbability)
			{
				this.BuildIfStatementCodeRecursive(random, sb, nestProbability - 0.2);
			}
			sb.AppendLine("}");
		}
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x000105AC File Offset: 0x0000E7AC
	[MethodImpl(MethodImplOptions.NoOptimization)]
	private long RandomBranchingMethod(Random random)
	{
		long num = 0L;
		int num2 = random.Next();
		if (num2 % 9 == 6)
		{
			num += (long)(num2 % 3);
			if (num2 % 9 == 6)
			{
				num += (long)(num2 % 8);
				if (num2 % 10 == 7)
				{
					num += (long)(num2 % 5);
					if (num2 % 10 == 5)
					{
						num += (long)(num2 % 6);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 6);
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 10 == 7)
				{
					num += (long)(num2 % 6);
					if (num2 % 11 == 9)
					{
						num += (long)(num2 % 4);
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 8);
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 8 == 5)
					{
						num += (long)(num2 % 8);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 4);
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 8);
							}
						}
					}
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 8);
						}
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 5);
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 6);
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 5);
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 11);
							}
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 10 == 7)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 10);
					}
				}
			}
			if (num2 % 10 == 6)
			{
				num += (long)(num2 % 6);
				if (num2 % 5 == 4)
				{
					num += (long)(num2 % 6);
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 9);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 8)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 4);
							}
						}
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 9 == 3)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 4);
					}
				}
				if (num2 % 2 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 11 == 5)
					{
						num += (long)(num2 % 12);
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 8);
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 5);
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 8);
							}
						}
					}
				}
			}
			if (num2 % 5 == 3)
			{
				num += (long)(num2 % 3);
				if (num2 % 3 == 2)
				{
					num += (long)(num2 % 2);
					if (num2 % 10 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 8 == 7)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 11 == 9)
					{
						num += (long)(num2 % 2);
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 4);
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 10 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 4);
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 7);
							}
						}
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 8);
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 9);
						}
					}
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 8 == 2)
				{
					num += (long)(num2 % 3);
					if (num2 % 10 == 7)
					{
						num += (long)(num2 % 10);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 8);
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 4);
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 2);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 8)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 5);
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 2);
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 6);
							}
						}
					}
				}
				if (num2 % 4 == 2)
				{
					num += (long)(num2 % 2);
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 6 == 3)
					{
						num += (long)(num2 % 6);
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 5);
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 11 == 3)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 7);
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 8);
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 8 == 0)
				{
					num += (long)(num2 % 6);
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 7);
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 5);
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 5);
							}
						}
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 5);
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 4);
							}
						}
					}
					if (num2 % 9 == 6)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 10)
					{
						num += (long)(num2 % 12);
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 9);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 8);
							}
						}
					}
					if (num2 % 11 == 10)
					{
						num += (long)(num2 % 7);
					}
				}
				if (num2 % 5 == 4)
				{
					num += (long)(num2 % 3);
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
						}
					}
				}
				if (num2 % 6 == 4)
				{
					num += (long)(num2 % 2);
				}
			}
			if (num2 % 9 == 6)
			{
				num += (long)(num2 % 7);
				if (num2 % 7 == 6)
				{
					num += (long)(num2 % 7);
				}
				if (num2 % 2 == 0)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 4 == 0)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 8 == 1)
				{
					num += (long)(num2 % 4);
					if (num2 % 8 == 7)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 12);
						}
					}
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 10 == 3)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 10 == 7)
					{
						num += (long)(num2 % 3);
					}
				}
			}
			if (num2 % 4 == 3)
			{
				num += (long)(num2 % 2);
				if (num2 % 5 == 0)
				{
					num += (long)(num2 % 3);
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 3);
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 2);
							}
						}
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 4);
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 7);
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 3);
							}
						}
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 7 == 5)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 7);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 3)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 4)
					{
						num += (long)(num2 % 11);
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 2);
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 2);
							}
						}
					}
				}
				if (num2 % 10 == 9)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 3 == 2)
				{
					num += (long)(num2 % 2);
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 5 == 2)
				{
					num += (long)(num2 % 6);
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 5);
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 11 == 5)
				{
					num += (long)(num2 % 2);
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 3);
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 2);
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 7);
						}
					}
				}
				if (num2 % 10 == 4)
				{
					num += (long)(num2 % 6);
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 6);
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 8);
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 7);
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 11 == 6)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 7);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 11);
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 9);
						}
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 10 == 0)
					{
						num += (long)(num2 % 9);
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
					}
				}
				if (num2 % 5 == 3)
				{
					num += (long)(num2 % 4);
				}
			}
			if (num2 % 11 == 4)
			{
				num += (long)(num2 % 7);
				if (num2 % 10 == 3)
				{
					num += (long)(num2 % 5);
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 8)
					{
						num += (long)(num2 % 6);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 8);
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 4);
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 11);
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 10);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
					}
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 5);
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 8 == 6)
				{
					num += (long)(num2 % 6);
					if (num2 % 10 == 4)
					{
						num += (long)(num2 % 10);
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 5);
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 3);
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 6 == 2)
					{
						num += (long)(num2 % 6);
						if (num2 % 11 == 9)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 4);
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 10 == 0)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 10 == 8)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 11 == 4)
				{
					num += (long)(num2 % 9);
				}
				if (num2 % 10 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 11 == 1)
					{
						num += (long)(num2 % 7);
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 8 == 5)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 9 == 2)
				{
					num += (long)(num2 % 2);
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 8)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 10 == 8)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 4);
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 11 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 6);
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 10 == 0)
				{
					num += (long)(num2 % 5);
				}
			}
			if (num2 % 4 == 3)
			{
				num += (long)(num2 % 5);
				if (num2 % 5 == 1)
				{
					num += (long)(num2 % 4);
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 4);
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 3);
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 10);
							}
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 6);
							if (num2 % 11 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 4)
					{
						num += (long)(num2 % 12);
					}
					if (num2 % 11 == 9)
					{
						num += (long)(num2 % 2);
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 9);
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 8);
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
					}
				}
				if (num2 % 11 == 6)
				{
					num += (long)(num2 % 10);
				}
				if (num2 % 3 == 1)
				{
					num += (long)(num2 % 4);
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 3);
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 3);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 6);
							}
						}
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 5 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 5);
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 7);
						}
					}
				}
				if (num2 % 8 == 0)
				{
					num += (long)(num2 % 9);
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 5);
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 8 == 7)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 2);
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 10);
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 7);
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 1)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 11);
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 7 == 3)
				{
					num += (long)(num2 % 8);
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 11);
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 9);
							}
						}
					}
				}
				if (num2 % 11 == 2)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 6 == 4)
				{
					num += (long)(num2 % 7);
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 10);
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
						}
					}
					if (num2 % 8 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 10 == 7)
					{
						num += (long)(num2 % 7);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 7);
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 7);
						}
					}
				}
				if (num2 % 7 == 0)
				{
					num += (long)(num2 % 4);
				}
			}
		}
		if (num2 % 8 == 0)
		{
			num += (long)(num2 % 7);
			if (num2 % 2 == 0)
			{
				num += (long)(num2 % 3);
				if (num2 % 9 == 3)
				{
					num += (long)(num2 % 7);
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 1)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 8 == 1)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 9 == 3)
					{
						num += (long)(num2 % 7);
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 9)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 9 == 0)
				{
					num += (long)(num2 % 7);
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 8);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 5);
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 6);
					}
				}
				if (num2 % 11 == 8)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 5 == 1)
				{
					num += (long)(num2 % 6);
					if (num2 % 8 == 5)
					{
						num += (long)(num2 % 9);
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 9);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 5);
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 7);
						}
					}
				}
				if (num2 % 8 == 5)
				{
					num += (long)(num2 % 4);
					if (num2 % 8 == 5)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 4);
					}
				}
				if (num2 % 9 == 5)
				{
					num += (long)(num2 % 2);
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 10 == 6)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 3);
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 6);
						}
					}
				}
			}
			if (num2 % 9 == 7)
			{
				num += (long)(num2 % 3);
			}
			if (num2 % 2 == 1)
			{
				num += (long)(num2 % 2);
				if (num2 % 7 == 6)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 5 == 4)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 3 == 1)
				{
					num += (long)(num2 % 4);
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 8);
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 7);
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 7);
					}
				}
				if (num2 % 3 == 1)
				{
					num += (long)(num2 % 2);
				}
			}
			if (num2 % 6 == 5)
			{
				num += (long)(num2 % 5);
				if (num2 % 10 == 1)
				{
					num += (long)(num2 % 4);
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 10 == 2)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 3);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 9)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 10);
						}
					}
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 7);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 10);
							}
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 7);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 5);
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 12);
							}
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 10 == 1)
				{
					num += (long)(num2 % 8);
				}
				if (num2 % 3 == 2)
				{
					num += (long)(num2 % 3);
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 7);
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 11 == 3)
					{
						num += (long)(num2 % 12);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 9 == 3)
					{
						num += (long)(num2 % 5);
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 7);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 7);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 5);
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 9 == 8)
				{
					num += (long)(num2 % 8);
				}
				if (num2 % 5 == 1)
				{
					num += (long)(num2 % 5);
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 4);
					}
				}
				if (num2 % 4 == 1)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 6 == 3)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 9 == 8)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 10 == 1)
				{
					num += (long)(num2 % 3);
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 6)
					{
						num += (long)(num2 % 4);
					}
				}
			}
		}
		if (num2 % 7 == 1)
		{
			num += (long)(num2 % 3);
			if (num2 % 8 == 5)
			{
				num += (long)(num2 % 8);
				if (num2 % 8 == 5)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 9);
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 8);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 10 == 6)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 10 == 8)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 5 == 2)
				{
					num += (long)(num2 % 4);
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 6);
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 3);
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 12);
						}
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 2)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 10 == 8)
					{
						num += (long)(num2 % 10);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 5);
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 8)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 4);
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 2);
							}
						}
					}
					if (num2 % 7 == 2)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 11 == 0)
				{
					num += (long)(num2 % 5);
				}
			}
			if (num2 % 8 == 0)
			{
				num += (long)(num2 % 9);
			}
			if (num2 % 4 == 0)
			{
				num += (long)(num2 % 2);
				if (num2 % 2 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 10 == 5)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 6);
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 10);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 8 == 2)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 6 == 2)
				{
					num += (long)(num2 % 6);
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 2)
					{
						num += (long)(num2 % 7);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 8);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 10 == 5)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 7)
						{
							num += (long)(num2 % 6);
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 6 == 5)
				{
					num += (long)(num2 % 4);
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 11 == 4)
					{
						num += (long)(num2 % 10);
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
					}
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 7 == 5)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 7);
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 10);
					}
					if (num2 % 7 == 5)
					{
						num += (long)(num2 % 7);
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 4);
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 11 == 4)
					{
						num += (long)(num2 % 10);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 10 == 9)
					{
						num += (long)(num2 % 11);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 2);
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 4 == 1)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 8 == 1)
				{
					num += (long)(num2 % 4);
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 8);
					}
				}
			}
			if (num2 % 7 == 6)
			{
				num += (long)(num2 % 7);
				if (num2 % 6 == 5)
				{
					num += (long)(num2 % 6);
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 7);
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 11 == 8)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 9);
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 7);
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
						}
					}
				}
				if (num2 % 11 == 7)
				{
					num += (long)(num2 % 4);
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 10 == 8)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 10 == 6)
					{
						num += (long)(num2 % 5);
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 5);
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 8);
					}
				}
				if (num2 % 3 == 1)
				{
					num += (long)(num2 % 3);
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 8);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 4);
						}
					}
				}
				if (num2 % 4 == 1)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 10 == 6)
				{
					num += (long)(num2 % 8);
				}
			}
			if (num2 % 7 == 5)
			{
				num += (long)(num2 % 6);
				if (num2 % 6 == 1)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 7 == 4)
				{
					num += (long)(num2 % 5);
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 7)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 8 == 3)
					{
						num += (long)(num2 % 5);
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 10 == 0)
					{
						num += (long)(num2 % 9);
					}
				}
				if (num2 % 7 == 5)
				{
					num += (long)(num2 % 4);
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 6);
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 10);
						}
					}
					if (num2 % 9 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
				}
				if (num2 % 4 == 0)
				{
					num += (long)(num2 % 5);
				}
				if (num2 % 8 == 1)
				{
					num += (long)(num2 % 9);
					if (num2 % 10 == 5)
					{
						num += (long)(num2 % 7);
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 8 == 3)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 8)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 7);
					}
				}
				if (num2 % 6 == 4)
				{
					num += (long)(num2 % 5);
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 5);
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 2);
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 3);
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 2);
						}
					}
				}
			}
		}
		if (num2 % 5 == 0)
		{
			num += (long)(num2 % 6);
			if (num2 % 2 == 0)
			{
				num += (long)(num2 % 2);
				if (num2 % 7 == 2)
				{
					num += (long)(num2 % 4);
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 10 == 8)
				{
					num += (long)(num2 % 7);
					if (num2 % 10 == 3)
					{
						num += (long)(num2 % 9);
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 12);
					}
					if (num2 % 11 == 1)
					{
						num += (long)(num2 % 9);
					}
				}
				if (num2 % 5 == 2)
				{
					num += (long)(num2 % 2);
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 9 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 10 == 4)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 11 == 6)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 8);
						}
					}
				}
				if (num2 % 10 == 2)
				{
					num += (long)(num2 % 7);
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 6);
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
					}
					if (num2 % 7 == 5)
					{
						num += (long)(num2 % 4);
					}
				}
				if (num2 % 11 == 8)
				{
					num += (long)(num2 % 9);
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 10);
						}
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 6);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 5);
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 9)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 12);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 10 == 8)
					{
						num += (long)(num2 % 2);
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
					}
				}
			}
			if (num2 % 8 == 0)
			{
				num += (long)(num2 % 8);
				if (num2 % 5 == 2)
				{
					num += (long)(num2 % 5);
				}
				if (num2 % 11 == 3)
				{
					num += (long)(num2 % 9);
				}
				if (num2 % 11 == 10)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 5 == 4)
				{
					num += (long)(num2 % 2);
					if (num2 % 11 == 1)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 11 == 6)
					{
						num += (long)(num2 % 12);
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 9 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 7 == 1)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 10 == 3)
				{
					num += (long)(num2 % 9);
					if (num2 % 10 == 2)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 9);
						}
					}
					if (num2 % 10 == 8)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 11 == 2)
					{
						num += (long)(num2 % 11);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 11 == 5)
				{
					num += (long)(num2 % 9);
				}
			}
			if (num2 % 4 == 0)
			{
				num += (long)(num2 % 3);
				if (num2 % 6 == 0)
				{
					num += (long)(num2 % 3);
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 11 == 10)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 11 == 3)
					{
						num += (long)(num2 % 11);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 5);
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 5);
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 8 == 6)
				{
					num += (long)(num2 % 7);
				}
			}
			if (num2 % 5 == 4)
			{
				num += (long)(num2 % 5);
				if (num2 % 4 == 2)
				{
					num += (long)(num2 % 2);
					if (num2 % 8 == 3)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 11 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 7 == 3)
				{
					num += (long)(num2 % 2);
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 5);
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 11);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 11 == 2)
					{
						num += (long)(num2 % 5);
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 6);
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 7);
							}
						}
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 7);
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 5);
							}
						}
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 5);
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 8);
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 11 == 9)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 9 == 8)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 3);
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 11 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 2);
						}
					}
				}
				if (num2 % 5 == 3)
				{
					num += (long)(num2 % 3);
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 8)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 11 == 9)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 12);
					}
					if (num2 % 7 == 5)
					{
						num += (long)(num2 % 5);
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 9);
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 6);
						}
					}
				}
				if (num2 % 7 == 2)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 6 == 1)
				{
					num += (long)(num2 % 7);
				}
				if (num2 % 6 == 5)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 4 == 3)
				{
					num += (long)(num2 % 3);
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 3);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 5);
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 3)
					{
						num += (long)(num2 % 8);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 11);
							}
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 5);
						}
					}
				}
			}
			if (num2 % 6 == 2)
			{
				num += (long)(num2 % 2);
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 8 == 6)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 7 == 4)
				{
					num += (long)(num2 % 7);
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 7 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 11 == 7)
						{
							num += (long)(num2 % 12);
						}
					}
					if (num2 % 11 == 3)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 7 == 1)
				{
					num += (long)(num2 % 7);
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 3)
					{
						num += (long)(num2 % 4);
					}
				}
			}
		}
		if (num2 % 3 == 0)
		{
			num += (long)(num2 % 4);
			if (num2 % 5 == 1)
			{
				num += (long)(num2 % 3);
				if (num2 % 2 == 1)
				{
					num += (long)(num2 % 3);
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 10);
						}
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 4);
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 5);
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 12);
						}
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 2);
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 4);
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 8);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
				}
				if (num2 % 2 == 1)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 5 == 3)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 7 == 1)
				{
					num += (long)(num2 % 3);
					if (num2 % 8 == 5)
					{
						num += (long)(num2 % 8);
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 6 == 2)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 3);
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 6);
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 10 == 6)
				{
					num += (long)(num2 % 7);
				}
				if (num2 % 4 == 0)
				{
					num += (long)(num2 % 2);
				}
			}
			if (num2 % 7 == 4)
			{
				num += (long)(num2 % 7);
				if (num2 % 3 == 1)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 5 == 4)
				{
					num += (long)(num2 % 6);
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 3);
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 7 == 5)
					{
						num += (long)(num2 % 6);
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 10 == 4)
					{
						num += (long)(num2 % 6);
					}
				}
				if (num2 % 9 == 8)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 10 == 4)
				{
					num += (long)(num2 % 6);
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 7);
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 8 == 7)
				{
					num += (long)(num2 % 6);
					if (num2 % 9 == 6)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 8 == 3)
				{
					num += (long)(num2 % 3);
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 5);
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 5 == 4)
				{
					num += (long)(num2 % 6);
				}
			}
			if (num2 % 10 == 9)
			{
				num += (long)(num2 % 8);
				if (num2 % 9 == 2)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 9 == 2)
				{
					num += (long)(num2 % 7);
					if (num2 % 10 == 6)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
					}
				}
				if (num2 % 7 == 1)
				{
					num += (long)(num2 % 7);
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 11 == 2)
					{
						num += (long)(num2 % 6);
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 9);
						}
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 7);
					}
				}
				if (num2 % 11 == 0)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 8 == 1)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 3 == 1)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 5 == 4)
				{
					num += (long)(num2 % 3);
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 5);
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 6);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 9 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 11 == 3)
					{
						num += (long)(num2 % 12);
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 3);
					}
				}
			}
			if (num2 % 6 == 1)
			{
				num += (long)(num2 % 4);
				if (num2 % 6 == 2)
				{
					num += (long)(num2 % 7);
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 9 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 5);
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 7);
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 4);
						}
					}
				}
				if (num2 % 2 == 1)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 2 == 1)
				{
					num += (long)(num2 % 3);
					if (num2 % 8 == 1)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 7 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 10 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
				}
				if (num2 % 9 == 1)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 5 == 4)
				{
					num += (long)(num2 % 2);
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 2);
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 9 == 1)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 11 == 4)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 6 == 3)
					{
						num += (long)(num2 % 2);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 9);
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 8);
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 11);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 5);
							}
						}
					}
				}
			}
			if (num2 % 4 == 2)
			{
				num += (long)(num2 % 3);
				if (num2 % 8 == 0)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 8 == 3)
				{
					num += (long)(num2 % 5);
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 7);
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 4);
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 8 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 11 == 9)
					{
						num += (long)(num2 % 5);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 8);
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 6 == 5)
				{
					num += (long)(num2 % 7);
					if (num2 % 10 == 3)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 9 == 8)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 6);
						}
					}
				}
			}
			if (num2 % 6 == 4)
			{
				num += (long)(num2 % 3);
				if (num2 % 8 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 1)
					{
						num += (long)(num2 % 6);
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 3);
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 4);
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 11 == 5)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 4 == 0)
				{
					num += (long)(num2 % 5);
					if (num2 % 10 == 6)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 3);
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 2)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 10 == 4)
				{
					num += (long)(num2 % 8);
				}
				if (num2 % 10 == 9)
				{
					num += (long)(num2 % 11);
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 11 == 0)
					{
						num += (long)(num2 % 10);
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 3);
					}
				}
			}
		}
		if (num2 % 6 == 5)
		{
			num += (long)(num2 % 7);
			if (num2 % 7 == 3)
			{
				num += (long)(num2 % 8);
				if (num2 % 2 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 10 == 8)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 6);
						}
					}
				}
				if (num2 % 7 == 0)
				{
					num += (long)(num2 % 5);
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 8)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 11 == 7)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 7);
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 7 == 3)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 4 == 2)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 4 == 2)
				{
					num += (long)(num2 % 5);
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 11 == 1)
				{
					num += (long)(num2 % 8);
				}
				if (num2 % 10 == 6)
				{
					num += (long)(num2 % 11);
				}
			}
			if (num2 % 8 == 2)
			{
				num += (long)(num2 % 3);
				if (num2 % 11 == 9)
				{
					num += (long)(num2 % 9);
				}
				if (num2 % 11 == 8)
				{
					num += (long)(num2 % 8);
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 3);
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 4);
					}
				}
				if (num2 % 5 == 0)
				{
					num += (long)(num2 % 4);
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 2);
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 7);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 8 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 4);
					}
				}
			}
			if (num2 % 10 == 7)
			{
				num += (long)(num2 % 3);
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 6);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 10);
							}
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 10 == 3)
					{
						num += (long)(num2 % 10);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 8);
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 7);
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 3)
					{
						num += (long)(num2 % 2);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 8);
						}
					}
				}
				if (num2 % 9 == 4)
				{
					num += (long)(num2 % 4);
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 5)
					{
						num += (long)(num2 % 11);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 6 == 1)
				{
					num += (long)(num2 % 3);
				}
			}
			if (num2 % 6 == 1)
			{
				num += (long)(num2 % 2);
				if (num2 % 10 == 7)
				{
					num += (long)(num2 % 10);
					if (num2 % 10 == 8)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 8);
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 2 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 9 == 8)
					{
						num += (long)(num2 % 9);
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 6);
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 6);
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 4 == 0)
				{
					num += (long)(num2 % 5);
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 11 == 2)
					{
						num += (long)(num2 % 10);
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 11);
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 7);
							}
						}
					}
					if (num2 % 6 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 7);
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 8);
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 7);
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 5);
							if (num2 % 11 == 1)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 2);
							}
						}
					}
				}
				if (num2 % 5 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 5);
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
						}
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 1)
					{
						num += (long)(num2 % 8);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 6);
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 7);
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 3);
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 9)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 7 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 10);
						}
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 7);
					}
				}
				if (num2 % 11 == 10)
				{
					num += (long)(num2 % 2);
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 8);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 0)
					{
						num += (long)(num2 % 12);
					}
					if (num2 % 11 == 6)
					{
						num += (long)(num2 % 3);
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 2);
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 9);
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 10);
							}
						}
					}
					if (num2 % 11 == 8)
					{
						num += (long)(num2 % 3);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 7);
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 7);
					}
				}
				if (num2 % 10 == 3)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 10 == 6)
				{
					num += (long)(num2 % 4);
					if (num2 % 11 == 5)
					{
						num += (long)(num2 % 2);
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 6);
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 11);
							}
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 6);
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 5);
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 6);
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
					}
					if (num2 % 8 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 6);
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 8);
						}
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 4);
					}
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 3);
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 2);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 11);
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 10);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 8 == 3)
					{
						num += (long)(num2 % 5);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
					}
				}
				if (num2 % 3 == 2)
				{
					num += (long)(num2 % 4);
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 4);
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 9);
							}
						}
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 4);
					}
				}
			}
			if (num2 % 9 == 1)
			{
				num += (long)(num2 % 2);
				if (num2 % 4 == 2)
				{
					num += (long)(num2 % 2);
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 11 == 10)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 2);
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 8);
						}
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 7);
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 2 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 11 == 9)
					{
						num += (long)(num2 % 8);
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 9 == 4)
					{
						num += (long)(num2 % 5);
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 5);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 6);
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 10 == 9)
					{
						num += (long)(num2 % 4);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 6);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 8)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
						}
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 7);
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 11 == 4)
					{
						num += (long)(num2 % 12);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 4);
						}
					}
				}
				if (num2 % 6 == 2)
				{
					num += (long)(num2 % 5);
					if (num2 % 9 == 3)
					{
						num += (long)(num2 % 8);
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 8 == 3)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 9);
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 5);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 10 == 7)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 7 == 1)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 10 == 9)
				{
					num += (long)(num2 % 5);
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 5);
							}
						}
					}
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 9 == 7)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 7 == 2)
				{
					num += (long)(num2 % 8);
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 10 == 0)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 9 == 5)
				{
					num += (long)(num2 % 6);
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 6)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 11 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 10 == 5)
					{
						num += (long)(num2 % 7);
					}
				}
				if (num2 % 7 == 3)
				{
					num += (long)(num2 % 4);
					if (num2 % 11 == 0)
					{
						num += (long)(num2 % 12);
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 11 == 3)
					{
						num += (long)(num2 % 3);
						if (num2 % 11 == 7)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 9)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 11);
						}
					}
				}
			}
		}
		if (num2 % 5 == 4)
		{
			num += (long)(num2 % 6);
			if (num2 % 3 == 1)
			{
				num += (long)(num2 % 3);
				if (num2 % 11 == 3)
				{
					num += (long)(num2 % 5);
				}
				if (num2 % 7 == 5)
				{
					num += (long)(num2 % 5);
				}
				if (num2 % 9 == 8)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 8 == 3)
				{
					num += (long)(num2 % 7);
					if (num2 % 10 == 0)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 10 == 6)
					{
						num += (long)(num2 % 6);
						if (num2 % 11 == 7)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 4);
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 9 == 5)
				{
					num += (long)(num2 % 6);
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 9 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 4);
					}
				}
				if (num2 % 10 == 5)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 9 == 1)
				{
					num += (long)(num2 % 6);
					if (num2 % 9 == 6)
					{
						num += (long)(num2 % 4);
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 3);
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 5);
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 3);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 10 == 5)
					{
						num += (long)(num2 % 10);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 10 == 8)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 5);
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 10);
							}
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 8);
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
				}
			}
			if (num2 % 11 == 7)
			{
				num += (long)(num2 % 4);
				if (num2 % 6 == 4)
				{
					num += (long)(num2 % 5);
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 9);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 2);
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 6 == 1)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 9 == 6)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 6 == 3)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 5 == 2)
				{
					num += (long)(num2 % 3);
					if (num2 % 10 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 10);
						}
					}
				}
				if (num2 % 3 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 8);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 2);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 4);
							}
						}
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 7);
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 11 == 1)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 7 == 2)
				{
					num += (long)(num2 % 4);
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 6)
						{
							num += (long)(num2 % 6);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
					}
					if (num2 % 8 == 1)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 11 == 3)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 10 == 3)
				{
					num += (long)(num2 % 6);
				}
			}
		}
		if (num2 % 5 == 2)
		{
			num += (long)(num2 % 3);
			if (num2 % 3 == 0)
			{
				num += (long)(num2 % 2);
				if (num2 % 6 == 0)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 8 == 7)
				{
					num += (long)(num2 % 5);
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 8);
							}
						}
					}
					if (num2 % 8 == 5)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 7)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 6);
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 11 == 2)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 9);
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 4);
							if (num2 % 6 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 2)
							{
								num += (long)(num2 % 5);
							}
						}
					}
				}
				if (num2 % 7 == 6)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 11 == 7)
				{
					num += (long)(num2 % 3);
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 10);
					}
				}
				if (num2 % 4 == 1)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 4 == 1)
				{
					num += (long)(num2 % 3);
					if (num2 % 10 == 9)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 5);
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 3 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 3);
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 10 == 3)
					{
						num += (long)(num2 % 3);
					}
				}
			}
			if (num2 % 2 == 1)
			{
				num += (long)(num2 % 3);
				if (num2 % 5 == 3)
				{
					num += (long)(num2 % 5);
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 7 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 7 == 1)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 8);
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 9);
						}
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 9 == 8)
					{
						num += (long)(num2 % 5);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 11);
							}
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 8 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
				}
				if (num2 % 6 == 2)
				{
					num += (long)(num2 % 6);
				}
				if (num2 % 4 == 1)
				{
					num += (long)(num2 % 4);
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 5);
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 5);
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 10);
						}
					}
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 5);
					}
				}
				if (num2 % 6 == 2)
				{
					num += (long)(num2 % 6);
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 8);
							}
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 11 == 8)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 4 == 1)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 9 == 4)
				{
					num += (long)(num2 % 5);
				}
				if (num2 % 4 == 3)
				{
					num += (long)(num2 % 5);
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 2);
					}
				}
			}
			if (num2 % 10 == 5)
			{
				num += (long)(num2 % 10);
			}
			if (num2 % 9 == 0)
			{
				num += (long)(num2 % 3);
				if (num2 % 4 == 0)
				{
					num += (long)(num2 % 5);
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 2)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 6 == 2)
				{
					num += (long)(num2 % 6);
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 9 == 3)
					{
						num += (long)(num2 % 9);
					}
				}
				if (num2 % 4 == 0)
				{
					num += (long)(num2 % 5);
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 10 == 6)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 9 == 6)
					{
						num += (long)(num2 % 10);
					}
					if (num2 % 6 == 3)
					{
						num += (long)(num2 % 4);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 11 == 6)
					{
						num += (long)(num2 % 11);
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 6)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 5);
							}
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 8);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 9)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 7 == 5)
					{
						num += (long)(num2 % 8);
					}
					if (num2 % 9 == 6)
					{
						num += (long)(num2 % 4);
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 4);
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 12);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 6);
					}
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 4);
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 4);
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 11);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 5)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 11 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 10);
							}
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 8);
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 8);
							}
						}
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 6);
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 10 == 3)
					{
						num += (long)(num2 % 9);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 10);
							}
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 11 == 7)
					{
						num += (long)(num2 % 6);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 6);
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 11 == 8)
						{
							num += (long)(num2 % 2);
						}
					}
				}
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 2);
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 12);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 4);
							}
						}
					}
					if (num2 % 7 == 4)
					{
						num += (long)(num2 % 2);
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
				}
				if (num2 % 10 == 0)
				{
					num += (long)(num2 % 9);
				}
				if (num2 % 3 == 2)
				{
					num += (long)(num2 % 4);
				}
			}
			if (num2 % 5 == 1)
			{
				num += (long)(num2 % 5);
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 8 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 7 == 1)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 1)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 11 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 9 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 3);
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 5);
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 8)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 5)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 5);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 11 == 2)
					{
						num += (long)(num2 % 9);
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 9 == 7)
				{
					num += (long)(num2 % 3);
				}
			}
			if (num2 % 10 == 7)
			{
				num += (long)(num2 % 9);
			}
			if (num2 % 4 == 1)
			{
				num += (long)(num2 % 4);
				if (num2 % 9 == 8)
				{
					num += (long)(num2 % 10);
				}
				if (num2 % 7 == 4)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 10 == 2)
				{
					num += (long)(num2 % 9);
				}
				if (num2 % 3 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 7);
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 10);
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
					}
					if (num2 % 11 == 10)
					{
						num += (long)(num2 % 6);
					}
				}
				if (num2 % 10 == 8)
				{
					num += (long)(num2 % 8);
					if (num2 % 9 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 4);
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 6);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 8);
						}
					}
					if (num2 % 10 == 3)
					{
						num += (long)(num2 % 9);
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 10);
							}
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 4);
					}
				}
			}
			if (num2 % 11 == 5)
			{
				num += (long)(num2 % 9);
				if (num2 % 3 == 0)
				{
					num += (long)(num2 % 2);
				}
				if (num2 % 9 == 1)
				{
					num += (long)(num2 % 8);
					if (num2 % 6 == 0)
					{
						num += (long)(num2 % 5);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 4)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 2);
							}
						}
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 7 == 3)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 7);
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 7)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 4 == 2)
					{
						num += (long)(num2 % 5);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 6);
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 10);
							}
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 11 == 5)
					{
						num += (long)(num2 % 6);
					}
				}
				if (num2 % 8 == 3)
				{
					num += (long)(num2 % 3);
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 5)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 5 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 3 == 2)
					{
						num += (long)(num2 % 3);
					}
				}
				if (num2 % 8 == 7)
				{
					num += (long)(num2 % 3);
				}
				if (num2 % 3 == 2)
				{
					num += (long)(num2 % 2);
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 2);
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 11 == 0)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 5 == 2)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 9);
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 11);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 11 == 6)
					{
						num += (long)(num2 % 12);
					}
					if (num2 % 6 == 2)
					{
						num += (long)(num2 % 2);
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 10 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 10 == 4)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 3)
					{
						num += (long)(num2 % 6);
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 11 == 8)
						{
							num += (long)(num2 % 6);
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 2)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
				}
				if (num2 % 10 == 6)
				{
					num += (long)(num2 % 4);
					if (num2 % 8 == 3)
					{
						num += (long)(num2 % 9);
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 7);
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 6 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 4);
						}
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 4);
					}
				}
			}
		}
		if (num2 % 10 == 0)
		{
			num += (long)(num2 % 4);
			if (num2 % 8 == 6)
			{
				num += (long)(num2 % 7);
				if (num2 % 9 == 1)
				{
					num += (long)(num2 % 2);
					if (num2 % 7 == 0)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 4 == 0)
					{
						num += (long)(num2 % 5);
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 5)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 10 == 7)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 3);
					}
					if (num2 % 6 == 3)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 2 == 0)
					{
						num += (long)(num2 % 3);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 11 == 10)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 8 == 2)
						{
							num += (long)(num2 % 6);
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 2);
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 5 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 6 == 5)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 5 == 2)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 11);
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 10 == 6)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 3);
							}
						}
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 7);
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 10 == 4)
						{
							num += (long)(num2 % 9);
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 8 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 2)
						{
							num += (long)(num2 % 5);
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 2)
							{
								num += (long)(num2 % 9);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
					}
				}
				if (num2 % 6 == 2)
				{
					num += (long)(num2 % 4);
				}
				if (num2 % 11 == 1)
				{
					num += (long)(num2 % 3);
					if (num2 % 11 == 6)
					{
						num += (long)(num2 % 6);
					}
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 10 == 9)
						{
							num += (long)(num2 % 7);
						}
					}
				}
				if (num2 % 10 == 9)
				{
					num += (long)(num2 % 4);
					if (num2 % 3 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 11 == 9)
						{
							num += (long)(num2 % 10);
						}
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 9);
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 8 == 0)
					{
						num += (long)(num2 % 8);
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 10 == 1)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 6 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 4)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 9 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 7 == 2)
				{
					num += (long)(num2 % 8);
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 10 == 4)
					{
						num += (long)(num2 % 2);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 5);
							if (num2 % 9 == 1)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 11 == 3)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 7 == 2)
					{
						num += (long)(num2 % 7);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 7)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 6 == 0)
						{
							num += (long)(num2 % 2);
						}
					}
					if (num2 % 9 == 5)
					{
						num += (long)(num2 % 10);
					}
					if (num2 % 9 == 1)
					{
						num += (long)(num2 % 10);
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 3);
							if (num2 % 9 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 10 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 5)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 7)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 3);
							if (num2 % 11 == 7)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 9 == 2)
							{
								num += (long)(num2 % 9);
							}
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 5);
						}
					}
				}
				if (num2 % 7 == 4)
				{
					num += (long)(num2 % 8);
					if (num2 % 5 == 1)
					{
						num += (long)(num2 % 6);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 3)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 10 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 7 == 5)
						{
							num += (long)(num2 % 2);
							if (num2 % 10 == 8)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 5 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 10 == 4)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 6 == 4)
							{
								num += (long)(num2 % 7);
							}
						}
						if (num2 % 3 == 0)
						{
							num += (long)(num2 % 2);
							if (num2 % 8 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 6 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 9 == 7)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 7 == 3)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 3 == 0)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 9 == 5)
						{
							num += (long)(num2 % 3);
							if (num2 % 11 == 6)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 4)
							{
								num += (long)(num2 % 12);
							}
							if (num2 % 7 == 6)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 9);
							}
						}
					}
					if (num2 % 10 == 6)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 2);
					}
					if (num2 % 5 == 3)
					{
						num += (long)(num2 % 6);
						if (num2 % 4 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 9 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 4);
						}
					}
				}
				if (num2 % 4 == 3)
				{
					num += (long)(num2 % 4);
					if (num2 % 8 == 4)
					{
						num += (long)(num2 % 6);
						if (num2 % 9 == 4)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 5 == 4)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 5 == 0)
					{
						num += (long)(num2 % 4);
						if (num2 % 9 == 7)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 4 == 3)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 4)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 6);
						}
					}
					if (num2 % 7 == 6)
					{
						num += (long)(num2 % 8);
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
							if (num2 % 4 == 0)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 8 == 5)
							{
								num += (long)(num2 % 7);
							}
							if (num2 % 9 == 6)
							{
								num += (long)(num2 % 10);
							}
							if (num2 % 10 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 6 == 0)
							{
								num += (long)(num2 % 6);
							}
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 7 == 0)
							{
								num += (long)(num2 % 4);
							}
						}
						if (num2 % 9 == 6)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 8 == 0)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 3 == 1)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 6 == 4)
						{
							num += (long)(num2 % 7);
						}
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 4);
						if (num2 % 4 == 2)
						{
							num += (long)(num2 % 2);
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 11 == 10)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 8 == 3)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 3)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 2 == 0)
							{
								num += (long)(num2 % 2);
							}
						}
						if (num2 % 9 == 8)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 11 == 8)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 11 == 5)
						{
							num += (long)(num2 % 7);
						}
					}
				}
				if (num2 % 4 == 1)
				{
					num += (long)(num2 % 3);
					if (num2 % 4 == 3)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 6 == 1)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 8 == 6)
					{
						num += (long)(num2 % 4);
					}
					if (num2 % 4 == 1)
					{
						num += (long)(num2 % 2);
						if (num2 % 5 == 4)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 5);
						}
					}
					if (num2 % 6 == 4)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 8);
						}
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 9 == 2)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 6 == 1)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 7 == 0)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 1)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 2 == 1)
					{
						num += (long)(num2 % 3);
						if (num2 % 5 == 0)
						{
							num += (long)(num2 % 6);
						}
						if (num2 % 10 == 2)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 4 == 0)
						{
							num += (long)(num2 % 4);
						}
						if (num2 % 8 == 1)
						{
							num += (long)(num2 % 9);
						}
					}
					if (num2 % 8 == 2)
					{
						num += (long)(num2 % 5);
					}
					if (num2 % 10 == 7)
					{
						num += (long)(num2 % 2);
						if (num2 % 7 == 1)
						{
							num += (long)(num2 % 7);
						}
						if (num2 % 7 == 4)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 8 == 6)
						{
							num += (long)(num2 % 3);
						}
						if (num2 % 11 == 8)
						{
							num += (long)(num2 % 9);
						}
						if (num2 % 5 == 3)
						{
							num += (long)(num2 % 5);
						}
						if (num2 % 6 == 5)
						{
							num += (long)(num2 % 2);
						}
						if (num2 % 2 == 0)
						{
							num += (long)(num2 % 3);
							if (num2 % 7 == 2)
							{
								num += (long)(num2 % 4);
							}
							if (num2 % 5 == 0)
							{
								num += (long)(num2 % 5);
							}
							if (num2 % 11 == 8)
							{
								num += (long)(num2 % 8);
							}
							if (num2 % 3 == 1)
							{
								num += (long)(num2 % 3);
							}
							if (num2 % 11 == 9)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 2 == 1)
							{
								num += (long)(num2 % 2);
							}
							if (num2 % 4 == 2)
							{
								num += (long)(num2 % 3);
							}
						}
						if (num2 % 7 == 6)
						{
							num += (long)(num2 % 3);
						}
					}
					if (num2 % 10 == 1)
					{
						num += (long)(num2 % 10);
					}
				}
			}
			if (num2 % 8 == 4)
			{
				num += (long)(num2 % 7);
			}
		}
		return num;
	}

	// Token: 0x040002BB RID: 699
	[SerializeField]
	private CPUBenchmarkAssets assets;

	// Token: 0x040002BC RID: 700
	private const int NUMBER_OF_TESTS = 50;

	// Token: 0x040002BD RID: 701
	private const int WARMUP_TESTS_COUNT = 10;

	// Token: 0x040002BE RID: 702
	private int processor_count;

	// Token: 0x040002BF RID: 703
	private bool is_test_running;

	// Token: 0x040002C0 RID: 704
	private int test_index;

	// Token: 0x040002C1 RID: 705
	private List<CPUBenchmark.TestMetrics> gathered_metrics = new List<CPUBenchmark.TestMetrics>();

	// Token: 0x040002C2 RID: 706
	private List<CPUBenchmark.TestMetrics> gathered_multithread_metrics = new List<CPUBenchmark.TestMetrics>();

	// Token: 0x040002C3 RID: 707
	private Action<CPUBenchmark> on_complete_action;

	// Token: 0x040002C4 RID: 708
	private const int ALLOCATION_BATCH_COUNT = 64;

	// Token: 0x040002C5 RID: 709
	private const int BRANCHING_BATCH_COUNT = 8000;

	// Token: 0x040002C6 RID: 710
	private const int COMPUTE_BATCH_COUNT = 12000;

	// Token: 0x02000506 RID: 1286
	public struct TestMetrics
	{
		// Token: 0x0600427D RID: 17021 RVA: 0x001F90F0 File Offset: 0x001F72F0
		public override string ToString()
		{
			return string.Format("allocation_ms: {0}, \tbranch_ms: {1}, \tcompute_ms: {2}, \tsort_ms: {3}", new object[]
			{
				this.allocation_ms,
				this.branch_ms,
				this.compute_ms,
				this.sort_ms
			});
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x0600427E RID: 17022 RVA: 0x001F9148 File Offset: 0x001F7348
		public static CPUBenchmark.TestMetrics fake_singlethreaded_metrics
		{
			get
			{
				return new CPUBenchmark.TestMetrics
				{
					allocation_ms = 2.4f,
					compute_ms = 0.82f,
					branch_ms = 1.89f,
					sort_ms = 1.81f
				};
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x0600427F RID: 17023 RVA: 0x001F9190 File Offset: 0x001F7390
		public static CPUBenchmark.TestMetrics fake_multithreaded_metrics
		{
			get
			{
				return new CPUBenchmark.TestMetrics
				{
					allocation_ms = 0f,
					compute_ms = 0.32f,
					branch_ms = 0.21f,
					sort_ms = 0.37f
				};
			}
		}

		// Token: 0x04002EA2 RID: 11938
		public float allocation_ms;

		// Token: 0x04002EA3 RID: 11939
		public float branch_ms;

		// Token: 0x04002EA4 RID: 11940
		public float compute_ms;

		// Token: 0x04002EA5 RID: 11941
		public float sort_ms;
	}
}
