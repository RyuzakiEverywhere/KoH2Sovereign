using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

// Token: 0x02000096 RID: 150
[BurstCompile]
public struct QuickSortJob : IJob
{
	// Token: 0x06000575 RID: 1397 RVA: 0x0003CDA0 File Offset: 0x0003AFA0
	public void Execute()
	{
		if (this.entries.Length > 0)
		{
			this.Quicksort(0, this.entries.Length - 1);
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0003CDC4 File Offset: 0x0003AFC4
	private void Quicksort(int left, int right)
	{
		int i = left;
		int num = right;
		float num2 = this.entries[(left + right) / 2];
		while (i <= num)
		{
			while (this.Compare(this.entries[i], ref num2) < 0)
			{
				i++;
			}
			while (this.Compare(this.entries[num], ref num2) > 0)
			{
				num--;
			}
			if (i <= num)
			{
				float value = this.entries[i];
				this.entries[i] = this.entries[num];
				this.entries[num] = value;
				i++;
				num--;
			}
		}
		if (left < num)
		{
			this.Quicksort(left, num);
		}
		if (i < right)
		{
			this.Quicksort(i, right);
		}
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0003CE7A File Offset: 0x0003B07A
	private int Compare(float a, ref float b)
	{
		return a.CompareTo(b);
	}

	// Token: 0x04000516 RID: 1302
	public NativeArray<float> entries;
}
