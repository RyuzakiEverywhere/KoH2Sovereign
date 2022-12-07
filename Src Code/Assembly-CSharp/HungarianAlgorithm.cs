using System;
using System.Collections.Generic;

// Token: 0x020000D1 RID: 209
public sealed class HungarianAlgorithm
{
	// Token: 0x06000A23 RID: 2595 RVA: 0x000747E8 File Offset: 0x000729E8
	public HungarianAlgorithm(int[,] costMatrix)
	{
		this._costMatrix = costMatrix;
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x000747F8 File Offset: 0x000729F8
	public int[] Run()
	{
		this._n = this._costMatrix.GetLength(0);
		this._lx = new int[this._n];
		this._ly = new int[this._n];
		this._s = new bool[this._n];
		this._t = new bool[this._n];
		this._matchX = new int[this._n];
		this._matchY = new int[this._n];
		this._slack = new int[this._n];
		this._slackx = new int[this._n];
		this._prev = new int[this._n];
		this._inf = int.MaxValue;
		this.InitMatches();
		if (this._n != this._costMatrix.GetLength(1))
		{
			return null;
		}
		this.InitLbls();
		this._maxMatch = 0;
		this.InitialMatching();
		Queue<int> queue = new Queue<int>();
		while (this._maxMatch != this._n)
		{
			queue.Clear();
			this.InitSt();
			int num = 0;
			int i = 0;
			int j;
			for (j = 0; j < this._n; j++)
			{
				if (this._matchX[j] == -1)
				{
					queue.Enqueue(j);
					num = j;
					this._prev[j] = -2;
					this._s[j] = true;
					break;
				}
			}
			for (int k = 0; k < this._n; k++)
			{
				this._slack[k] = this._costMatrix[num, k] - this._lx[num] - this._ly[k];
				this._slackx[k] = num;
			}
			do
			{
				if (queue.Count != 0)
				{
					j = queue.Dequeue();
					int num2 = this._lx[j];
					for (i = 0; i < this._n; i++)
					{
						if (this._costMatrix[j, i] == num2 + this._ly[i] && !this._t[i])
						{
							if (this._matchY[i] == -1)
							{
								break;
							}
							this._t[i] = true;
							queue.Enqueue(this._matchY[i]);
							this.AddToTree(this._matchY[i], j);
						}
					}
					if (i >= this._n)
					{
						continue;
					}
				}
				if (i < this._n)
				{
					break;
				}
				this.UpdateLabels();
				for (i = 0; i < this._n; i++)
				{
					if (!this._t[i] && this._slack[i] == 0)
					{
						if (this._matchY[i] == -1)
						{
							j = this._slackx[i];
							break;
						}
						this._t[i] = true;
						if (!this._s[this._matchY[i]])
						{
							queue.Enqueue(this._matchY[i]);
							this.AddToTree(this._matchY[i], this._slackx[i]);
						}
					}
				}
			}
			while (i >= this._n);
			this._maxMatch++;
			int num3 = j;
			int num4 = i;
			while (num3 != -2)
			{
				int num5 = this._matchX[num3];
				this._matchY[num4] = num3;
				this._matchX[num3] = num4;
				num3 = this._prev[num3];
				num4 = num5;
			}
		}
		return this._matchX;
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x00074B14 File Offset: 0x00072D14
	private void InitMatches()
	{
		for (int i = 0; i < this._n; i++)
		{
			this._matchX[i] = -1;
			this._matchY[i] = -1;
		}
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x00074B44 File Offset: 0x00072D44
	private void InitSt()
	{
		for (int i = 0; i < this._n; i++)
		{
			this._s[i] = false;
			this._t[i] = false;
		}
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x00074B74 File Offset: 0x00072D74
	private void InitLbls()
	{
		for (int i = 0; i < this._n; i++)
		{
			int num = this._costMatrix[i, 0];
			for (int j = 0; j < this._n; j++)
			{
				if (this._costMatrix[i, j] < num)
				{
					num = this._costMatrix[i, j];
				}
				if (num == 0)
				{
					break;
				}
			}
			this._lx[i] = num;
		}
		for (int k = 0; k < this._n; k++)
		{
			int num2 = this._costMatrix[0, k] - this._lx[0];
			for (int l = 0; l < this._n; l++)
			{
				if (this._costMatrix[l, k] - this._lx[l] < num2)
				{
					num2 = this._costMatrix[l, k] - this._lx[l];
				}
				if (num2 == 0)
				{
					break;
				}
			}
			this._ly[k] = num2;
		}
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x00074C60 File Offset: 0x00072E60
	private void UpdateLabels()
	{
		int num = this._inf;
		for (int i = 0; i < this._n; i++)
		{
			if (!this._t[i] && num > this._slack[i])
			{
				num = this._slack[i];
			}
		}
		for (int j = 0; j < this._n; j++)
		{
			if (this._s[j])
			{
				this._lx[j] = this._lx[j] + num;
			}
			if (this._t[j])
			{
				this._ly[j] = this._ly[j] - num;
			}
			else
			{
				this._slack[j] = this._slack[j] - num;
			}
		}
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x00074D00 File Offset: 0x00072F00
	private void AddToTree(int x, int prevx)
	{
		this._s[x] = true;
		this._prev[x] = prevx;
		int num = this._lx[x];
		for (int i = 0; i < this._n; i++)
		{
			if (this._costMatrix[x, i] - num - this._ly[i] < this._slack[i])
			{
				this._slack[i] = this._costMatrix[x, i] - num - this._ly[i];
				this._slackx[i] = x;
			}
		}
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x00074D84 File Offset: 0x00072F84
	private void InitialMatching()
	{
		for (int i = 0; i < this._n; i++)
		{
			for (int j = 0; j < this._n; j++)
			{
				if (this._costMatrix[i, j] == this._lx[i] + this._ly[j] && this._matchY[j] == -1)
				{
					this._matchX[i] = j;
					this._matchY[j] = i;
					this._maxMatch++;
					break;
				}
			}
		}
	}

	// Token: 0x04000826 RID: 2086
	private readonly int[,] _costMatrix;

	// Token: 0x04000827 RID: 2087
	private int _inf;

	// Token: 0x04000828 RID: 2088
	private int _n;

	// Token: 0x04000829 RID: 2089
	private int[] _lx;

	// Token: 0x0400082A RID: 2090
	private int[] _ly;

	// Token: 0x0400082B RID: 2091
	private bool[] _s;

	// Token: 0x0400082C RID: 2092
	private bool[] _t;

	// Token: 0x0400082D RID: 2093
	private int[] _matchX;

	// Token: 0x0400082E RID: 2094
	private int[] _matchY;

	// Token: 0x0400082F RID: 2095
	private int _maxMatch;

	// Token: 0x04000830 RID: 2096
	private int[] _slack;

	// Token: 0x04000831 RID: 2097
	private int[] _slackx;

	// Token: 0x04000832 RID: 2098
	private int[] _prev;
}
