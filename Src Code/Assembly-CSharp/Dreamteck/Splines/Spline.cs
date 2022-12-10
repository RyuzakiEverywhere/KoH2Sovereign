using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004CD RID: 1229
	[Serializable]
	public class Spline
	{
		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06004145 RID: 16709 RVA: 0x001F05F6 File Offset: 0x001EE7F6
		// (set) Token: 0x06004146 RID: 16710 RVA: 0x000023FD File Offset: 0x000005FD
		public bool isClosed
		{
			get
			{
				return this.closed && this.points.Length >= 4;
			}
			set
			{
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06004147 RID: 16711 RVA: 0x001F0610 File Offset: 0x001EE810
		// (set) Token: 0x06004148 RID: 16712 RVA: 0x000023FD File Offset: 0x000005FD
		public double moveStep
		{
			get
			{
				if (this.type == Spline.Type.Linear)
				{
					return (double)(1f / (float)(this.points.Length - 1));
				}
				return (double)(1f / (float)(this.iterations - 1));
			}
			set
			{
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06004149 RID: 16713 RVA: 0x001F063E File Offset: 0x001EE83E
		public int iterations
		{
			get
			{
				if (this.type == Spline.Type.Linear)
				{
					return this.points.Length;
				}
				return this.sampleRate * (this.points.Length - 1) - (this.points.Length - 1) + 1;
			}
		}

		// Token: 0x0600414A RID: 16714 RVA: 0x001F0670 File Offset: 0x001EE870
		public Spline(Spline.Type type)
		{
			this.type = type;
			this.points = new SplinePoint[0];
		}

		// Token: 0x0600414B RID: 16715 RVA: 0x001F06B0 File Offset: 0x001EE8B0
		public Spline(Spline.Type type, int sampleRate)
		{
			this.type = type;
			this.sampleRate = sampleRate;
			this.points = new SplinePoint[0];
		}

		// Token: 0x0600414C RID: 16716 RVA: 0x001F0700 File Offset: 0x001EE900
		public float CalculateLength(double from = 0.0, double to = 1.0, double resolution = 1.0)
		{
			if (this.points.Length == 0)
			{
				return 0f;
			}
			resolution = DMath.Clamp01(resolution);
			if (resolution == 0.0)
			{
				return 0f;
			}
			from = DMath.Clamp01(from);
			to = DMath.Clamp01(to);
			if (to < from)
			{
				to = from;
			}
			double num = from;
			Vector3 b = this.EvaluatePosition(num);
			float num2 = 0f;
			do
			{
				num = DMath.Move(num, to, this.moveStep / resolution);
				Vector3 vector = this.EvaluatePosition(num);
				num2 += (vector - b).magnitude;
				b = vector;
			}
			while (num != to);
			return num2;
		}

		// Token: 0x0600414D RID: 16717 RVA: 0x001F0790 File Offset: 0x001EE990
		public double Project(Vector3 position, int subdivide = 4, double from = 0.0, double to = 1.0)
		{
			if (this.points.Length == 0)
			{
				return 0.0;
			}
			if (this.closed && from == 0.0 && to == 1.0)
			{
				double closestPoint = this.GetClosestPoint(subdivide, position, from, to, Mathf.RoundToInt((float)Mathf.Max(this.iterations / this.points.Length, 10)) * 5);
				if (closestPoint < this.moveStep)
				{
					double closestPoint2 = this.GetClosestPoint(subdivide, position, 0.5, to, Mathf.RoundToInt((float)Mathf.Max(this.iterations / this.points.Length, 10)) * 5);
					if (Vector3.Distance(position, this.EvaluatePosition(closestPoint2)) < Vector3.Distance(position, this.EvaluatePosition(closestPoint)))
					{
						return closestPoint2;
					}
				}
				return closestPoint;
			}
			return this.GetClosestPoint(subdivide, position, from, to, Mathf.RoundToInt((float)Mathf.Max(this.iterations / this.points.Length, 10)) * 5);
		}

		// Token: 0x0600414E RID: 16718 RVA: 0x001F0888 File Offset: 0x001EEA88
		public bool Raycast(out RaycastHit hit, out double hitPercent, LayerMask layerMask, double resolution = 1.0, double from = 0.0, double to = 1.0, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal)
		{
			resolution = DMath.Clamp01(resolution);
			from = DMath.Clamp01(from);
			to = DMath.Clamp01(to);
			double num = from;
			Vector3 vector = this.EvaluatePosition(num);
			hitPercent = 0.0;
			if (resolution == 0.0)
			{
				hit = default(RaycastHit);
				hitPercent = 0.0;
				return false;
			}
			double a;
			Vector3 vector2;
			for (;;)
			{
				a = num;
				num = DMath.Move(num, to, this.moveStep / resolution);
				vector2 = this.EvaluatePosition(num);
				if (Physics.Linecast(vector, vector2, out hit, layerMask, hitTriggers))
				{
					break;
				}
				vector = vector2;
				if (num == to)
				{
					return false;
				}
			}
			double t = (double)((hit.point - vector).sqrMagnitude / (vector2 - vector).sqrMagnitude);
			hitPercent = DMath.Lerp(a, num, t);
			return true;
		}

		// Token: 0x0600414F RID: 16719 RVA: 0x001F0954 File Offset: 0x001EEB54
		public bool RaycastAll(out RaycastHit[] hits, out double[] hitPercents, LayerMask layerMask, double resolution = 1.0, double from = 0.0, double to = 1.0, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal)
		{
			resolution = DMath.Clamp01(resolution);
			from = DMath.Clamp01(from);
			to = DMath.Clamp01(to);
			double num = from;
			Vector3 vector = this.EvaluatePosition(num);
			List<RaycastHit> list = new List<RaycastHit>();
			List<double> list2 = new List<double>();
			if (resolution == 0.0)
			{
				hits = new RaycastHit[0];
				hitPercents = new double[0];
				return false;
			}
			bool result = false;
			do
			{
				double a = num;
				num = DMath.Move(num, to, this.moveStep / resolution);
				Vector3 vector2 = this.EvaluatePosition(num);
				RaycastHit[] array = Physics.RaycastAll(vector, vector2 - vector, Vector3.Distance(vector, vector2), layerMask, hitTriggers);
				for (int i = 0; i < array.Length; i++)
				{
					result = true;
					double t = (double)((array[i].point - vector).sqrMagnitude / (vector2 - vector).sqrMagnitude);
					list2.Add(DMath.Lerp(a, num, t));
					list.Add(array[i]);
				}
				vector = vector2;
			}
			while (num != to);
			hits = list.ToArray();
			hitPercents = list2.ToArray();
			return result;
		}

		// Token: 0x06004150 RID: 16720 RVA: 0x001F0A77 File Offset: 0x001EEC77
		public double GetPointPercent(int pointIndex)
		{
			return DMath.Clamp01((double)pointIndex / (double)(this.points.Length - 1));
		}

		// Token: 0x06004151 RID: 16721 RVA: 0x001F0A8C File Offset: 0x001EEC8C
		public Vector3 EvaluatePosition(double percent)
		{
			if (this.points.Length == 0)
			{
				return Vector3.zero;
			}
			Vector3 result = default(Vector3);
			this.EvaluatePosition(ref result, percent);
			return result;
		}

		// Token: 0x06004152 RID: 16722 RVA: 0x001F0ABC File Offset: 0x001EECBC
		public SplineSample Evaluate(double percent)
		{
			SplineSample result = new SplineSample();
			this.Evaluate(result, percent);
			return result;
		}

		// Token: 0x06004153 RID: 16723 RVA: 0x001F0AD8 File Offset: 0x001EECD8
		public SplineSample Evaluate(int pointIndex)
		{
			SplineSample result = new SplineSample();
			this.Evaluate(result, this.GetPointPercent(pointIndex));
			return result;
		}

		// Token: 0x06004154 RID: 16724 RVA: 0x001F0AFA File Offset: 0x001EECFA
		public void Evaluate(SplineSample result, int pointIndex)
		{
			this.Evaluate(result, this.GetPointPercent(pointIndex));
		}

		// Token: 0x06004155 RID: 16725 RVA: 0x001F0B0C File Offset: 0x001EED0C
		public void Evaluate(SplineSample result, double percent)
		{
			if (this.points.Length == 0)
			{
				result = new SplineSample();
				return;
			}
			percent = DMath.Clamp01(percent);
			if (this.closed && this.points.Length <= 2)
			{
				this.closed = false;
			}
			if (this.points.Length == 1)
			{
				result.position = this.points[0].position;
				result.up = this.points[0].normal;
				result.forward = Vector3.forward;
				result.size = this.points[0].size;
				result.color = this.points[0].color;
				result.percent = percent;
				return;
			}
			double num = (double)(this.points.Length - 1) * percent;
			int num2 = Mathf.Clamp(DMath.FloorInt(num), 0, this.points.Length - 2);
			double num3 = num - (double)num2;
			Vector3 position = this.EvaluatePosition(percent);
			result.position = position;
			result.percent = percent;
			if (num2 <= this.points.Length - 2)
			{
				SplinePoint splinePoint = this.points[num2 + 1];
				if (num2 == this.points.Length - 2 && this.closed)
				{
					splinePoint = this.points[0];
				}
				float num4 = (float)num3;
				if (this.customValueInterpolation != null && this.customValueInterpolation.length > 0)
				{
					num4 = this.customValueInterpolation.Evaluate(num4);
				}
				float num5 = (float)num3;
				if (this.customNormalInterpolation != null && this.customNormalInterpolation.length > 0)
				{
					num5 = this.customNormalInterpolation.Evaluate(num5);
				}
				result.size = Mathf.Lerp(this.points[num2].size, splinePoint.size, num4);
				result.color = Color.Lerp(this.points[num2].color, splinePoint.color, num4);
				result.up = Vector3.Slerp(this.points[num2].normal, splinePoint.normal, num5);
			}
			else if (this.closed)
			{
				result.size = this.points[0].size;
				result.color = this.points[0].color;
				result.up = this.points[0].normal;
			}
			else
			{
				result.size = this.points[num2].size;
				result.color = this.points[num2].color;
				result.up = this.points[num2].normal;
			}
			if (this.type == Spline.Type.BSpline)
			{
				double num6 = 1.0 / (double)(this.iterations - 1);
				if (percent <= 1.0 - num6 && percent >= num6)
				{
					result.forward = this.EvaluatePosition(percent + num6) - this.EvaluatePosition(percent - num6);
				}
				else
				{
					Vector3 b = Vector3.zero;
					Vector3 a = Vector3.zero;
					if (this.closed)
					{
						if (percent < num6)
						{
							b = this.EvaluatePosition(1.0 - (num6 - percent));
						}
						else
						{
							b = this.EvaluatePosition(percent - num6);
						}
						if (percent > 1.0 - num6)
						{
							a = this.EvaluatePosition(num6 - (1.0 - percent));
						}
						else
						{
							a = this.EvaluatePosition(percent + num6);
						}
						result.forward = a - b;
					}
					else
					{
						b = result.position - this.EvaluatePosition(percent - num6);
						a = this.EvaluatePosition(percent + num6) - result.position;
						result.forward = Vector3.Slerp(a, b, b.magnitude / a.magnitude);
					}
				}
			}
			else
			{
				this.EvaluateTangent(ref result.forward, percent);
			}
			result.forward.Normalize();
		}

		// Token: 0x06004156 RID: 16726 RVA: 0x001F0EE0 File Offset: 0x001EF0E0
		public void Evaluate(ref SplineSample[] samples, double from = 0.0, double to = 1.0)
		{
			if (this.points.Length == 0)
			{
				samples = new SplineSample[0];
				return;
			}
			from = DMath.Clamp01(from);
			to = DMath.Clamp(to, from, 1.0);
			double a = from * (double)(this.iterations - 1);
			int num = DMath.CeilInt(to * (double)(this.iterations - 1)) - DMath.FloorInt(a) + 1;
			if (samples == null)
			{
				samples = new SplineSample[num];
			}
			else if (samples.Length != num)
			{
				samples = new SplineSample[num];
			}
			double num2 = from;
			double moveStep = this.moveStep;
			int num3 = 0;
			for (;;)
			{
				samples[num3] = this.Evaluate(num2);
				num3++;
				if (num3 >= samples.Length)
				{
					break;
				}
				num2 = DMath.Move(num2, to, moveStep);
			}
		}

		// Token: 0x06004157 RID: 16727 RVA: 0x001F0F90 File Offset: 0x001EF190
		public void EvaluateUniform(ref SplineSample[] samples, ref double[] originalSamplePercents, double from = 0.0, double to = 1.0)
		{
			if (this.points.Length == 0)
			{
				samples = new SplineSample[0];
				return;
			}
			from = DMath.Clamp01(from);
			to = DMath.Clamp(to, from, 1.0);
			double a = from * (double)(this.iterations - 1);
			int num = DMath.CeilInt(to * (double)(this.iterations - 1)) - DMath.FloorInt(a) + 1;
			if (samples == null || samples.Length != num)
			{
				samples = new SplineSample[num];
			}
			if (originalSamplePercents == null || originalSamplePercents.Length != num)
			{
				originalSamplePercents = new double[num];
			}
			for (int i = 0; i < samples.Length; i++)
			{
				if (samples[i] == null)
				{
					samples[i] = new SplineSample();
				}
			}
			float distance = this.CalculateLength(from, to, 1.0) / (float)(this.iterations - 1);
			this.Evaluate(samples[0], from);
			samples[0].percent = (originalSamplePercents[0] = from);
			double num2 = from;
			float num3 = 0f;
			for (int j = 1; j < samples.Length - 1; j++)
			{
				this.Evaluate(samples[j], this.Travel(num2, distance, out num3, Spline.Direction.Forward));
				num2 = samples[j].percent;
				originalSamplePercents[j] = num2;
				samples[j].percent = DMath.Lerp(from, to, (double)j / (double)(samples.Length - 1));
			}
			this.Evaluate(samples[samples.Length - 1], to);
			samples[samples.Length - 1].percent = (originalSamplePercents[originalSamplePercents.Length - 1] = to);
		}

		// Token: 0x06004158 RID: 16728 RVA: 0x001F1108 File Offset: 0x001EF308
		public void EvaluatePositions(ref Vector3[] positions, double from = 0.0, double to = 1.0)
		{
			if (this.points.Length == 0)
			{
				positions = new Vector3[0];
				return;
			}
			from = DMath.Clamp01(from);
			to = DMath.Clamp(to, from, 1.0);
			double a = from * (double)(this.iterations - 1);
			int num = DMath.CeilInt(to * (double)(this.iterations - 1)) - DMath.FloorInt(a) + 1;
			if (positions.Length != num)
			{
				positions = new Vector3[num];
			}
			double num2 = from;
			double moveStep = this.moveStep;
			int num3 = 0;
			for (;;)
			{
				positions[num3] = this.EvaluatePosition(num2);
				num3++;
				if (num3 >= positions.Length)
				{
					break;
				}
				num2 = DMath.Move(num2, to, moveStep);
			}
		}

		// Token: 0x06004159 RID: 16729 RVA: 0x001F11AC File Offset: 0x001EF3AC
		public double Travel(double start, float distance, out float moved, Spline.Direction direction)
		{
			moved = 0f;
			if (this.points.Length <= 1)
			{
				return 0.0;
			}
			if (direction == Spline.Direction.Forward && start >= 1.0)
			{
				return 1.0;
			}
			if (direction == Spline.Direction.Backward && start <= 0.0)
			{
				return 0.0;
			}
			if (distance == 0f)
			{
				return DMath.Clamp01(start);
			}
			Vector3 vector = Vector3.zero;
			this.EvaluatePosition(ref vector, start);
			Vector3 b = vector;
			double a = start;
			int num = this.iterations - 1;
			int num2 = (direction == Spline.Direction.Forward) ? DMath.CeilInt(start * (double)num) : DMath.FloorInt(start * (double)num);
			double num3;
			float num4;
			for (;;)
			{
				num3 = (double)num2 / (double)num;
				vector = this.EvaluatePosition(num3);
				num4 = Vector3.Distance(vector, b);
				b = vector;
				moved += num4;
				if (moved >= distance)
				{
					break;
				}
				a = num3;
				if (direction == Spline.Direction.Forward)
				{
					if (num2 == num)
					{
						break;
					}
					num2++;
				}
				else
				{
					if (num2 == 0)
					{
						break;
					}
					num2--;
				}
			}
			return DMath.Lerp(a, num3, (double)(1f - (moved - distance) / num4));
		}

		// Token: 0x0600415A RID: 16730 RVA: 0x001F12BC File Offset: 0x001EF4BC
		public double Travel(double start, float distance, Spline.Direction direction = Spline.Direction.Forward)
		{
			float num;
			return this.Travel(start, distance, out num, direction);
		}

		// Token: 0x0600415B RID: 16731 RVA: 0x001F12D4 File Offset: 0x001EF4D4
		public void EvaluatePosition(ref Vector3 point, double percent)
		{
			percent = DMath.Clamp01(percent);
			double num = (double)(this.points.Length - 1) * percent;
			int num2 = DMath.FloorInt(num);
			if (this.type == Spline.Type.Bezier)
			{
				num2 = Mathf.Clamp(num2, 0, Mathf.Max(this.points.Length - 2, 0));
			}
			this.GetPoint(ref point, num - (double)num2, num2);
		}

		// Token: 0x0600415C RID: 16732 RVA: 0x001F132C File Offset: 0x001EF52C
		public void EvaluateTangent(ref Vector3 tangent, double percent)
		{
			percent = DMath.Clamp01(percent);
			double num = (double)(this.points.Length - 1) * percent;
			int num2 = DMath.FloorInt(num);
			if (this.type == Spline.Type.Bezier)
			{
				num2 = Mathf.Clamp(num2, 0, Mathf.Max(this.points.Length - 2, 0));
			}
			this.GetTangent(ref tangent, num - (double)num2, num2);
		}

		// Token: 0x0600415D RID: 16733 RVA: 0x001F1384 File Offset: 0x001EF584
		private double GetClosestPoint(int iterations, Vector3 point, double start, double end, int slices)
		{
			if (iterations > 0)
			{
				double num = 0.0;
				float num2 = float.PositiveInfinity;
				double num3 = (end - start) / (double)slices;
				double num4 = start;
				Vector3 zero = Vector3.zero;
				for (;;)
				{
					this.EvaluatePosition(ref zero, num4);
					float sqrMagnitude = (point - zero).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
						num = num4;
					}
					if (num4 == end)
					{
						break;
					}
					num4 = DMath.Move(num4, end, num3);
				}
				double num5 = num - num3;
				if (num5 < start)
				{
					num5 = start;
				}
				double num6 = num + num3;
				if (num6 > end)
				{
					num6 = end;
				}
				return this.GetClosestPoint(--iterations, point, num5, num6, slices);
			}
			float sqrMagnitude2 = (point - this.EvaluatePosition(start)).sqrMagnitude;
			float sqrMagnitude3 = (point - this.EvaluatePosition(end)).sqrMagnitude;
			if (sqrMagnitude2 < sqrMagnitude3)
			{
				return start;
			}
			if (sqrMagnitude3 < sqrMagnitude2)
			{
				return end;
			}
			return (start + end) / 2.0;
		}

		// Token: 0x0600415E RID: 16734 RVA: 0x001F146E File Offset: 0x001EF66E
		public void Break()
		{
			this.Break(0);
		}

		// Token: 0x0600415F RID: 16735 RVA: 0x001F1478 File Offset: 0x001EF678
		public void Break(int at)
		{
			if (!this.closed)
			{
				return;
			}
			if (at >= this.points.Length)
			{
				return;
			}
			SplinePoint[] array = new SplinePoint[at];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.points[i];
			}
			for (int j = at; j < this.points.Length - 1; j++)
			{
				this.points[j - at] = this.points[j];
			}
			for (int k = 0; k < array.Length; k++)
			{
				this.points[this.points.Length - at + k - 1] = array[k];
			}
			this.points[this.points.Length - 1] = this.points[0];
			this.closed = false;
		}

		// Token: 0x06004160 RID: 16736 RVA: 0x001F1547 File Offset: 0x001EF747
		public void Close()
		{
			if (this.points.Length < 4)
			{
				Debug.LogError("Points need to be at least 4 to close the spline");
				return;
			}
			this.closed = true;
		}

		// Token: 0x06004161 RID: 16737 RVA: 0x001F1568 File Offset: 0x001EF768
		public void CatToBezierTangents()
		{
			switch (this.type)
			{
			case Spline.Type.CatmullRom:
				for (int i = 0; i < this.points.Length; i++)
				{
					this.GetCatPoints(i);
					this.points[i].type = SplinePoint.Type.SmoothMirrored;
					if (i == 0)
					{
						Vector3 a = Spline.catPoints[1] - Spline.catPoints[2];
						if (this.closed)
						{
							a = this.points[this.points.Length - 2].position - this.points[i + 1].position;
							this.points[i].SetTangentPosition(this.points[i].position + a / 6f);
						}
						else
						{
							this.points[i].SetTangentPosition(this.points[i].position + a / 3f);
						}
					}
					else if (i == this.points.Length - 1)
					{
						Vector3 a2 = Spline.catPoints[2] - Spline.catPoints[3];
						this.points[i].SetTangentPosition(this.points[i].position + a2 / 3f);
					}
					else
					{
						Vector3 a3 = Spline.catPoints[0] - Spline.catPoints[2];
						this.points[i].SetTangentPosition(this.points[i].position + a3 / 6f);
					}
				}
				break;
			case Spline.Type.Linear:
				for (int j = 0; j < this.points.Length; j++)
				{
					this.points[j].type = SplinePoint.Type.Broken;
					this.points[j].SetTangentPosition(this.points[j].position);
					this.points[j].SetTangent2Position(this.points[j].position);
				}
				break;
			}
			this.type = Spline.Type.Bezier;
		}

		// Token: 0x06004162 RID: 16738 RVA: 0x001F17B8 File Offset: 0x001EF9B8
		private void GetPoint(ref Vector3 point, double percent, int pointIndex)
		{
			if (this.closed && this.points.Length > 3)
			{
				if (pointIndex == this.points.Length - 2)
				{
					this.points[0].SetTangentPosition(this.points[this.points.Length - 1].tangent);
					this.points[this.points.Length - 1] = this.points[0];
				}
			}
			else
			{
				this.closed = false;
			}
			switch (this.type)
			{
			case Spline.Type.CatmullRom:
				this.CatmullRomGetPoint(ref point, percent, pointIndex);
				return;
			case Spline.Type.BSpline:
				this.BSPGetPoint(ref point, percent, pointIndex);
				return;
			case Spline.Type.Bezier:
				this.BezierGetPoint(ref point, percent, pointIndex);
				return;
			case Spline.Type.Linear:
				this.LinearGetPoint(ref point, percent, pointIndex);
				return;
			default:
				return;
			}
		}

		// Token: 0x06004163 RID: 16739 RVA: 0x001F1880 File Offset: 0x001EFA80
		private void GetTangent(ref Vector3 tangent, double percent, int pointIndex)
		{
			switch (this.type)
			{
			case Spline.Type.CatmullRom:
				this.GetCatmullRomTangent(ref tangent, percent, pointIndex);
				return;
			case Spline.Type.BSpline:
				break;
			case Spline.Type.Bezier:
				this.BezierGetTangent(ref tangent, percent, pointIndex);
				return;
			case Spline.Type.Linear:
				this.LinearGetTangent(ref tangent, percent, pointIndex);
				break;
			default:
				return;
			}
		}

		// Token: 0x06004164 RID: 16740 RVA: 0x001F18C8 File Offset: 0x001EFAC8
		private void LinearGetPoint(ref Vector3 point, double t, int i)
		{
			if (this.points.Length == 0)
			{
				point = Vector3.zero;
				return;
			}
			if (i < this.points.Length - 1)
			{
				t = DMath.Clamp01(t);
				i = Mathf.Clamp(i, 0, this.points.Length - 2);
				point = Vector3.Lerp(this.points[i].position, this.points[i + 1].position, (float)t);
				return;
			}
			point = this.points[i].position;
		}

		// Token: 0x06004165 RID: 16741 RVA: 0x001F195C File Offset: 0x001EFB5C
		private void LinearGetTangent(ref Vector3 tangent, double t, int i)
		{
			if (this.points.Length == 0)
			{
				tangent = Vector3.forward;
				return;
			}
			this.GetCatPoints(i);
			if (this.linearAverageDirection)
			{
				tangent = Vector3.Slerp(Spline.catPoints[1] - Spline.catPoints[0], Spline.catPoints[2] - Spline.catPoints[1], 0.5f);
				return;
			}
			tangent = Spline.catPoints[2] - Spline.catPoints[1];
		}

		// Token: 0x06004166 RID: 16742 RVA: 0x001F19F8 File Offset: 0x001EFBF8
		private void BSPGetPoint(ref Vector3 point, double time, int i)
		{
			if (this.points.Length != 0)
			{
				point = this.points[0].position;
			}
			if (this.points.Length > 1)
			{
				float d = (float)DMath.Clamp01(time);
				this.GetCatPoints(i);
				point = ((-Spline.catPoints[0] + Spline.catPoints[2]) / 2f + d * ((Spline.catPoints[0] - 2f * Spline.catPoints[1] + Spline.catPoints[2]) / 2f + d * (-Spline.catPoints[0] + 3f * Spline.catPoints[1] - 3f * Spline.catPoints[2] + Spline.catPoints[3]) / 6f)) * d + (Spline.catPoints[0] + 4f * Spline.catPoints[1] + Spline.catPoints[2]) / 6f;
			}
		}

		// Token: 0x06004167 RID: 16743 RVA: 0x001F1B70 File Offset: 0x001EFD70
		private void BezierGetPoint(ref Vector3 point, double t, int i)
		{
			if (this.points.Length == 0)
			{
				return;
			}
			point = this.points[0].position;
			if (this.points.Length == 1)
			{
				return;
			}
			if (i < this.points.Length - 1)
			{
				t = DMath.Clamp01(t);
				i = Mathf.Clamp(i, 0, this.points.Length - 2);
				float num = (float)t;
				float num2 = 1f - num;
				point = num2 * num2 * num2 * this.points[i].position + 3f * num2 * num2 * num * this.points[i].tangent2 + 3f * num2 * num * num * this.points[i + 1].tangent + num * num * num * this.points[i + 1].position;
			}
		}

		// Token: 0x06004168 RID: 16744 RVA: 0x001F1C74 File Offset: 0x001EFE74
		private void BezierGetTangent(ref Vector3 tangent, double t, int i)
		{
			if (this.points.Length == 0)
			{
				return;
			}
			tangent = this.points[0].tangent2;
			if (this.points.Length == 1)
			{
				return;
			}
			if (i < this.points.Length - 1)
			{
				t = DMath.Clamp01(t);
				i = Mathf.Clamp(i, 0, this.points.Length - 2);
				float num = (float)t;
				float num2 = 1f - num;
				tangent = -3f * num2 * num2 * this.points[i].position + 3f * num2 * num2 * this.points[i].tangent2 - 6f * num * num2 * this.points[i].tangent2 - 3f * num * num * this.points[i + 1].tangent + 6f * num * num2 * this.points[i + 1].tangent + 3f * num * num * this.points[i + 1].position;
			}
		}

		// Token: 0x06004169 RID: 16745 RVA: 0x001F1DC8 File Offset: 0x001EFFC8
		private void CatmullRomGetPoint(ref Vector3 point, double t, int i)
		{
			float num = (float)t;
			float num2 = num * num;
			float d = num2 * num;
			if (this.points.Length != 0)
			{
				point = this.points[0].position;
			}
			if (i >= this.points.Length)
			{
				return;
			}
			if (this.points.Length > 1)
			{
				this.GetCatPoints(i);
				point = 0.5f * (2f * Spline.catPoints[1] + (-Spline.catPoints[0] + Spline.catPoints[2]) * num + (2f * Spline.catPoints[0] - 5f * Spline.catPoints[1] + 4f * Spline.catPoints[2] - Spline.catPoints[3]) * num2 + (-Spline.catPoints[0] + 3f * Spline.catPoints[1] - 3f * Spline.catPoints[2] + Spline.catPoints[3]) * d);
			}
		}

		// Token: 0x0600416A RID: 16746 RVA: 0x001F1F34 File Offset: 0x001F0134
		private void GetCatmullRomTangent(ref Vector3 direction, double t, int i)
		{
			float num = (float)t;
			float num2 = num * num;
			if (this.points.Length != 0)
			{
				direction = Vector3.forward;
			}
			if (i >= this.points.Length)
			{
				return;
			}
			if (this.points.Length > 1)
			{
				this.GetCatPoints(i);
				direction = (6f * num2 - 6f * num) * Spline.catPoints[1] + (3f * num2 - 4f * num + 1f) * (Spline.catPoints[2] - Spline.catPoints[0]) * 0.5f + (-6f * num2 + 6f * num) * Spline.catPoints[2] + (3f * num2 - 2f * num) * (Spline.catPoints[3] - Spline.catPoints[1]) * 0.5f;
			}
		}

		// Token: 0x0600416B RID: 16747 RVA: 0x001F2048 File Offset: 0x001F0248
		private void GetCatPoints(int i)
		{
			if (i > 0)
			{
				Spline.catPoints[0] = this.points[i - 1].position;
			}
			else if (this.closed && this.points.Length - 2 > i)
			{
				Spline.catPoints[0] = this.points[this.points.Length - 2].position;
			}
			else if (i + 1 < this.points.Length)
			{
				Spline.catPoints[0] = this.points[i].position + (this.points[i].position - this.points[i + 1].position);
			}
			else
			{
				Spline.catPoints[0] = this.points[i].position;
			}
			Spline.catPoints[1] = this.points[i].position;
			if (i + 1 < this.points.Length)
			{
				Spline.catPoints[2] = this.points[i + 1].position;
			}
			else if (this.closed && i + 2 - this.points.Length != i)
			{
				Spline.catPoints[2] = this.points[i + 2 - this.points.Length].position;
			}
			else
			{
				Spline.catPoints[2] = Spline.catPoints[1] + (Spline.catPoints[1] - Spline.catPoints[0]);
			}
			if (i + 2 < this.points.Length)
			{
				Spline.catPoints[3] = this.points[i + 2].position;
				return;
			}
			if (this.closed && i + 3 - this.points.Length != i)
			{
				Spline.catPoints[3] = this.points[i + 3 - this.points.Length].position;
				return;
			}
			Spline.catPoints[3] = Spline.catPoints[2] + (Spline.catPoints[2] - Spline.catPoints[1]);
		}

		// Token: 0x0600416C RID: 16748 RVA: 0x001F2290 File Offset: 0x001F0490
		public static void FormatFromTo(ref double from, ref double to, bool preventInvert = true)
		{
			from = DMath.Clamp01(from);
			to = DMath.Clamp01(to);
			if (preventInvert && from > to)
			{
				double num = from;
				from = to;
				to = num;
				return;
			}
			to = DMath.Clamp(to, 0.0, 1.0);
		}

		// Token: 0x04002D8D RID: 11661
		public SplinePoint[] points = new SplinePoint[0];

		// Token: 0x04002D8E RID: 11662
		[SerializeField]
		private bool closed;

		// Token: 0x04002D8F RID: 11663
		public Spline.Type type = Spline.Type.Bezier;

		// Token: 0x04002D90 RID: 11664
		public bool linearAverageDirection = true;

		// Token: 0x04002D91 RID: 11665
		public AnimationCurve customValueInterpolation;

		// Token: 0x04002D92 RID: 11666
		public AnimationCurve customNormalInterpolation;

		// Token: 0x04002D93 RID: 11667
		public int sampleRate = 10;

		// Token: 0x04002D94 RID: 11668
		private static Vector3[] catPoints = new Vector3[4];

		// Token: 0x020009AF RID: 2479
		public enum Direction
		{
			// Token: 0x0400451B RID: 17691
			Forward = 1,
			// Token: 0x0400451C RID: 17692
			Backward = -1
		}

		// Token: 0x020009B0 RID: 2480
		public enum Type
		{
			// Token: 0x0400451E RID: 17694
			CatmullRom,
			// Token: 0x0400451F RID: 17695
			BSpline,
			// Token: 0x04004520 RID: 17696
			Bezier,
			// Token: 0x04004521 RID: 17697
			Linear
		}
	}
}
