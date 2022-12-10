using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004CC RID: 1228
	[Serializable]
	public class SampleCollection
	{
		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x0600412F RID: 16687 RVA: 0x001EF6A7 File Offset: 0x001ED8A7
		public int Count
		{
			get
			{
				return this.samples.Length;
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06004130 RID: 16688 RVA: 0x001EF6B1 File Offset: 0x001ED8B1
		private bool hasSamples
		{
			get
			{
				return this.Count > 0;
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06004131 RID: 16689 RVA: 0x001EF6BC File Offset: 0x001ED8BC
		public bool samplesAreLooped
		{
			get
			{
				return this.loopSamples && this.clipFrom >= this.clipTo;
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06004132 RID: 16690 RVA: 0x001EF6D9 File Offset: 0x001ED8D9
		public double span
		{
			get
			{
				if (this.samplesAreLooped)
				{
					return 1.0 - this.clipFrom + this.clipTo;
				}
				return this.clipTo - this.clipFrom;
			}
		}

		// Token: 0x06004133 RID: 16691 RVA: 0x001EF708 File Offset: 0x001ED908
		public SampleCollection()
		{
		}

		// Token: 0x06004134 RID: 16692 RVA: 0x001EF738 File Offset: 0x001ED938
		public SampleCollection(SampleCollection input)
		{
			this.samples = input.samples;
			this.optimizedIndices = input.optimizedIndices;
			this.sampleMode = input.sampleMode;
			this.clipFrom = input.clipFrom;
			this.clipTo = input.clipTo;
		}

		// Token: 0x06004135 RID: 16693 RVA: 0x001EF7B0 File Offset: 0x001ED9B0
		public int GetClippedSampleCount(out int startIndex, out int endIndex)
		{
			startIndex = (endIndex = 0);
			if (this.sampleMode == SplineComputer.SampleMode.Default)
			{
				startIndex = DMath.FloorInt((double)(this.Count - 1) * this.clipFrom);
				endIndex = DMath.CeilInt((double)(this.Count - 1) * this.clipTo);
			}
			else
			{
				double num = 0.0;
				double num2 = 0.0;
				this.GetSamplingValues(this.clipFrom, out startIndex, out num);
				this.GetSamplingValues(this.clipTo, out endIndex, out num2);
				if (num2 > 0.0 && endIndex < this.Count - 1)
				{
					endIndex++;
				}
			}
			if (this.samplesAreLooped)
			{
				int num3 = endIndex + 1;
				int num4 = this.Count - startIndex;
				return num3 + num4;
			}
			return endIndex - startIndex + 1;
		}

		// Token: 0x06004136 RID: 16694 RVA: 0x001EF86D File Offset: 0x001EDA6D
		public double ClipPercent(double percent)
		{
			this.ClipPercent(ref percent);
			return percent;
		}

		// Token: 0x06004137 RID: 16695 RVA: 0x001EF878 File Offset: 0x001EDA78
		public void ClipPercent(ref double percent)
		{
			if (this.Count == 0)
			{
				percent = 0.0;
				return;
			}
			if (!this.samplesAreLooped)
			{
				percent = DMath.InverseLerp(this.clipFrom, this.clipTo, percent);
				return;
			}
			if (percent >= this.clipFrom && percent <= 1.0)
			{
				percent = DMath.InverseLerp(this.clipFrom, this.clipFrom + this.span, percent);
				return;
			}
			if (percent <= this.clipTo)
			{
				percent = DMath.InverseLerp(this.clipTo - this.span, this.clipTo, percent);
				return;
			}
			if (DMath.InverseLerp(this.clipTo, this.clipFrom, percent) < 0.5)
			{
				percent = 1.0;
				return;
			}
			percent = 0.0;
		}

		// Token: 0x06004138 RID: 16696 RVA: 0x001EF94A File Offset: 0x001EDB4A
		public double UnclipPercent(double percent)
		{
			this.UnclipPercent(ref percent);
			return percent;
		}

		// Token: 0x06004139 RID: 16697 RVA: 0x001EF958 File Offset: 0x001EDB58
		public void UnclipPercent(ref double percent)
		{
			if (percent == 0.0)
			{
				percent = this.clipFrom;
				return;
			}
			if (percent == 1.0)
			{
				percent = this.clipTo;
				return;
			}
			if (this.samplesAreLooped)
			{
				double num = (1.0 - this.clipFrom) / this.span;
				if (num == 0.0)
				{
					percent = 0.0;
					return;
				}
				if (percent < num)
				{
					percent = DMath.Lerp(this.clipFrom, 1.0, percent / num);
				}
				else
				{
					if (this.clipTo == 0.0)
					{
						percent = 0.0;
						return;
					}
					percent = DMath.Lerp(0.0, this.clipTo, (percent - num) / (this.clipTo / this.span));
				}
			}
			else
			{
				percent = DMath.Lerp(this.clipFrom, this.clipTo, percent);
			}
			percent = DMath.Clamp01(percent);
		}

		// Token: 0x0600413A RID: 16698 RVA: 0x001EFA54 File Offset: 0x001EDC54
		public void GetSamplingValues(double percent, out int sampleIndex, out double lerp)
		{
			lerp = 0.0;
			if (this.sampleMode == SplineComputer.SampleMode.Optimized)
			{
				double num = percent * (double)(this.optimizedIndices.Length - 1);
				int num2 = DMath.FloorInt(num);
				sampleIndex = this.optimizedIndices[num2];
				double t = 0.0;
				if (num2 < this.optimizedIndices.Length - 1)
				{
					double t2 = num - (double)num2;
					double a = (double)num2 / (double)(this.optimizedIndices.Length - 1);
					double b = (double)(num2 + 1) / (double)(this.optimizedIndices.Length - 1);
					t = DMath.Lerp(a, b, t2);
				}
				if (sampleIndex < this.Count - 1)
				{
					lerp = DMath.InverseLerp(this.samples[sampleIndex].percent, this.samples[sampleIndex + 1].percent, t);
				}
				return;
			}
			sampleIndex = DMath.FloorInt(percent * (double)(this.Count - 1));
			lerp = (double)(this.Count - 1) * percent - (double)sampleIndex;
		}

		// Token: 0x0600413B RID: 16699 RVA: 0x001EFB34 File Offset: 0x001EDD34
		public Vector3 EvaluatePosition(double percent)
		{
			if (!this.hasSamples)
			{
				return Vector3.zero;
			}
			this.UnclipPercent(ref percent);
			int num;
			double num2;
			this.GetSamplingValues(percent, out num, out num2);
			if (num2 > 0.0)
			{
				return Vector3.Lerp(this.samples[num].position, this.samples[num + 1].position, (float)num2);
			}
			return this.samples[num].position;
		}

		// Token: 0x0600413C RID: 16700 RVA: 0x001EFBA0 File Offset: 0x001EDDA0
		public SplineSample Evaluate(double percent)
		{
			SplineSample result = new SplineSample();
			this.Evaluate(percent, result);
			return result;
		}

		// Token: 0x0600413D RID: 16701 RVA: 0x001EFBBC File Offset: 0x001EDDBC
		public void Evaluate(double percent, SplineSample result)
		{
			if (!this.hasSamples)
			{
				result = new SplineSample();
				return;
			}
			this.UnclipPercent(ref percent);
			int num;
			double num2;
			this.GetSamplingValues(percent, out num, out num2);
			if (num2 > 0.0)
			{
				SplineSample.Lerp(this.samples[num], this.samples[num + 1], num2, result);
				return;
			}
			result.CopyFrom(this.samples[num]);
		}

		// Token: 0x0600413E RID: 16702 RVA: 0x001EFC20 File Offset: 0x001EDE20
		public void Evaluate(ref SplineSample[] results, double from = 0.0, double to = 1.0)
		{
			if (!this.hasSamples)
			{
				results = new SplineSample[0];
				return;
			}
			Spline.FormatFromTo(ref from, ref to, true);
			int num;
			double num2;
			this.GetSamplingValues(from, out num, out num2);
			int num3;
			this.GetSamplingValues(to, out num3, out num2);
			if (num2 > 0.0 && num3 < this.Count - 1)
			{
				num3++;
			}
			int num4 = num3 - num + 1;
			if (results == null)
			{
				results = new SplineSample[num4];
			}
			else if (results.Length != num4)
			{
				results = new SplineSample[num4];
			}
			results[0] = this.Evaluate(from);
			results[results.Length - 1] = this.Evaluate(to);
			for (int i = 1; i < results.Length - 1; i++)
			{
				results[i] = this.samples[i + num];
			}
		}

		// Token: 0x0600413F RID: 16703 RVA: 0x001EFCDC File Offset: 0x001EDEDC
		public void EvaluatePositions(ref Vector3[] positions, double from = 0.0, double to = 1.0)
		{
			if (!this.hasSamples)
			{
				positions = new Vector3[0];
				return;
			}
			Spline.FormatFromTo(ref from, ref to, true);
			int num;
			double num2;
			this.GetSamplingValues(from, out num, out num2);
			int num3;
			this.GetSamplingValues(to, out num3, out num2);
			if (num2 > 0.0 && num3 < this.Count - 1)
			{
				num3++;
			}
			int num4 = num3 - num + 1;
			if (positions == null)
			{
				positions = new Vector3[num4];
			}
			else if (positions.Length != num4)
			{
				positions = new Vector3[num4];
			}
			positions[0] = this.EvaluatePosition(from);
			positions[positions.Length - 1] = this.EvaluatePosition(to);
			for (int i = 1; i < positions.Length - 1; i++)
			{
				positions[i] = this.samples[i + num].position;
			}
		}

		// Token: 0x06004140 RID: 16704 RVA: 0x001EFDAC File Offset: 0x001EDFAC
		public double Travel(double start, float distance, Spline.Direction direction, out float moved)
		{
			moved = 0f;
			if (!this.hasSamples)
			{
				return 0.0;
			}
			if (direction == Spline.Direction.Forward && start >= 1.0)
			{
				return this.clipTo;
			}
			if (direction == Spline.Direction.Backward && start <= 0.0)
			{
				return this.clipFrom;
			}
			double num = this.UnclipPercent(DMath.Clamp01(start));
			if (distance == 0f)
			{
				return num;
			}
			Vector3 b = this.EvaluatePosition(start);
			int num2;
			double num3;
			this.GetSamplingValues(num, out num2, out num3);
			if (direction == Spline.Direction.Forward && num3 > 0.0)
			{
				num2++;
			}
			float num4 = 0f;
			int num5 = 0;
			int num6 = this.Count - 1;
			if (this.samplesAreLooped)
			{
				this.GetSamplingValues(this.clipFrom, out num5, out num3);
				this.GetSamplingValues(this.clipTo, out num6, out num3);
				if (num3 > 0.0)
				{
					num6++;
				}
			}
			while (moved < distance)
			{
				num4 = Vector3.Distance(this.samples[num2].position, b);
				moved += num4;
				if (moved >= distance)
				{
					break;
				}
				b = this.samples[num2].position;
				num = this.samples[num2].percent;
				if (direction == Spline.Direction.Forward)
				{
					if (num2 == this.Count - 1)
					{
						if (!this.samplesAreLooped)
						{
							break;
						}
						b = this.samples[0].position;
						num = this.samples[0].percent;
						num2 = 1;
					}
					if (this.samplesAreLooped && num2 == num6)
					{
						break;
					}
					num2++;
				}
				else
				{
					if (num2 == 0)
					{
						if (!this.samplesAreLooped)
						{
							break;
						}
						b = this.samples[this.Count - 1].position;
						num = this.samples[this.Count - 1].percent;
						num2 = this.Count - 2;
					}
					if (this.samplesAreLooped && num2 == num5)
					{
						break;
					}
					num2--;
				}
			}
			float num7 = 0f;
			if (moved > distance)
			{
				num7 = moved - distance;
			}
			double result = DMath.Lerp(num, this.samples[num2].percent, (double)(1f - num7 / num4));
			moved -= num7;
			return result;
		}

		// Token: 0x06004141 RID: 16705 RVA: 0x001EFFBC File Offset: 0x001EE1BC
		public double Travel(double start, float distance, Spline.Direction direction = Spline.Direction.Forward)
		{
			float num;
			return this.Travel(start, distance, direction, out num);
		}

		// Token: 0x06004142 RID: 16706 RVA: 0x001EFFD4 File Offset: 0x001EE1D4
		public void Project(Vector3 position, int controlPointCount, SplineSample result, double from = 0.0, double to = 1.0)
		{
			if (!this.hasSamples)
			{
				return;
			}
			if (this.Count != 1)
			{
				Spline.FormatFromTo(ref from, ref to, true);
				int num = (controlPointCount - 1) * 6;
				int num2 = this.Count / num;
				if (num2 < 1)
				{
					num2 = 1;
				}
				float num3 = (position - this.samples[0].position).sqrMagnitude;
				int num4 = 0;
				int num5 = this.Count - 1;
				if (from != 0.0)
				{
					double num6;
					this.GetSamplingValues(from, out num4, out num6);
				}
				if (to != 1.0)
				{
					double num6;
					this.GetSamplingValues(to, out num5, out num6);
					if (num6 > 0.0 && num5 < this.Count - 1)
					{
						num5++;
					}
				}
				int num7 = num4;
				int num8 = num5;
				for (int i = num4; i <= num5; i += num2)
				{
					if (i > num5)
					{
						i = num5;
					}
					float sqrMagnitude = (position - this.samples[i].position).sqrMagnitude;
					if (sqrMagnitude < num3)
					{
						num3 = sqrMagnitude;
						num7 = Mathf.Max(i - num2, 0);
						num8 = Mathf.Min(i + num2, this.Count - 1);
					}
					if (i == num5)
					{
						break;
					}
				}
				num3 = (position - this.samples[num7].position).sqrMagnitude;
				int num9 = num7;
				for (int j = num7 + 1; j <= num8; j++)
				{
					float sqrMagnitude2 = (position - this.samples[j].position).sqrMagnitude;
					if (sqrMagnitude2 < num3)
					{
						num3 = sqrMagnitude2;
						num9 = j;
					}
				}
				int num10 = num9 - 1;
				if (num10 < 0)
				{
					num10 = 0;
				}
				int num11 = num9 + 1;
				if (num11 > this.Count - 1)
				{
					num11 = this.Count - 1;
				}
				Vector3 vector = LinearAlgebraUtility.ProjectOnLine(this.samples[num10].position, this.samples[num9].position, position);
				Vector3 vector2 = LinearAlgebraUtility.ProjectOnLine(this.samples[num9].position, this.samples[num11].position, position);
				float magnitude = (this.samples[num9].position - this.samples[num10].position).magnitude;
				float magnitude2 = (this.samples[num9].position - this.samples[num11].position).magnitude;
				float magnitude3 = (vector - this.samples[num10].position).magnitude;
				float magnitude4 = (vector2 - this.samples[num11].position).magnitude;
				if (num10 < num9 && num9 < num11)
				{
					if ((position - vector).sqrMagnitude < (position - vector2).sqrMagnitude)
					{
						SplineSample.Lerp(this.samples[num10], this.samples[num9], magnitude3 / magnitude, result);
						if (this.sampleMode == SplineComputer.SampleMode.Uniform)
						{
							result.percent = DMath.Lerp(this.GetSamplePercent(num10), this.GetSamplePercent(num9), (double)(magnitude3 / magnitude));
						}
					}
					else
					{
						SplineSample.Lerp(this.samples[num11], this.samples[num9], magnitude4 / magnitude2, result);
						if (this.sampleMode == SplineComputer.SampleMode.Uniform)
						{
							result.percent = DMath.Lerp(this.GetSamplePercent(num11), this.GetSamplePercent(num9), (double)(magnitude4 / magnitude2));
						}
					}
				}
				else if (num10 < num9)
				{
					SplineSample.Lerp(this.samples[num10], this.samples[num9], magnitude3 / magnitude, result);
					if (this.sampleMode == SplineComputer.SampleMode.Uniform)
					{
						result.percent = DMath.Lerp(this.GetSamplePercent(num10), this.GetSamplePercent(num9), (double)(magnitude3 / magnitude));
					}
				}
				else
				{
					SplineSample.Lerp(this.samples[num11], this.samples[num9], magnitude4 / magnitude2, result);
					if (this.sampleMode == SplineComputer.SampleMode.Uniform)
					{
						result.percent = DMath.Lerp(this.GetSamplePercent(num11), this.GetSamplePercent(num9), (double)(magnitude4 / magnitude2));
					}
				}
				if (this.Count > 1 && from == 0.0 && to == 1.0 && result.percent < this.samples[1].percent)
				{
					Vector3 vector3 = LinearAlgebraUtility.ProjectOnLine(this.samples[this.Count - 1].position, this.samples[this.Count - 2].position, position);
					if ((position - vector3).sqrMagnitude < (position - result.position).sqrMagnitude)
					{
						double t = (double)LinearAlgebraUtility.InverseLerp(this.samples[this.Count - 1].position, this.samples[this.Count - 2].position, vector3);
						SplineSample.Lerp(this.samples[this.Count - 1], this.samples[this.Count - 2], t, result);
						if (this.sampleMode == SplineComputer.SampleMode.Uniform)
						{
							result.percent = DMath.Lerp(this.GetSamplePercent(this.Count - 1), this.GetSamplePercent(this.Count - 2), t);
						}
					}
				}
				return;
			}
			if (result == null)
			{
				result = new SplineSample(this.samples[0]);
				return;
			}
			result.CopyFrom(this.samples[0]);
		}

		// Token: 0x06004143 RID: 16707 RVA: 0x001F051A File Offset: 0x001EE71A
		private double GetSamplePercent(int sampleIndex)
		{
			if (this.sampleMode == SplineComputer.SampleMode.Optimized)
			{
				return this.samples[this.optimizedIndices[sampleIndex]].percent;
			}
			return (double)sampleIndex / (double)(this.Count - 1);
		}

		// Token: 0x06004144 RID: 16708 RVA: 0x001F0548 File Offset: 0x001EE748
		public float CalculateLength(double from = 0.0, double to = 1.0)
		{
			if (!this.hasSamples)
			{
				return 0f;
			}
			Spline.FormatFromTo(ref from, ref to, true);
			float num = 0f;
			Vector3 b = this.EvaluatePosition(from);
			int num2;
			double num3;
			this.GetSamplingValues(from, out num2, out num3);
			int num4;
			this.GetSamplingValues(to, out num4, out num3);
			if (num3 > 0.0 && num4 < this.Count - 1)
			{
				num4++;
			}
			for (int i = num2 + 1; i < num4; i++)
			{
				num += Vector3.Distance(this.samples[i].position, b);
				b = this.samples[i].position;
			}
			return num + Vector3.Distance(this.EvaluatePosition(to), b);
		}

		// Token: 0x04002D87 RID: 11655
		[HideInInspector]
		public SplineSample[] samples = new SplineSample[0];

		// Token: 0x04002D88 RID: 11656
		public int[] optimizedIndices = new int[0];

		// Token: 0x04002D89 RID: 11657
		public SplineComputer.SampleMode sampleMode;

		// Token: 0x04002D8A RID: 11658
		public double clipFrom;

		// Token: 0x04002D8B RID: 11659
		public double clipTo = 1.0;

		// Token: 0x04002D8C RID: 11660
		public bool loopSamples;
	}
}
