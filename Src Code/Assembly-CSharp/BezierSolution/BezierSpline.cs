using System;
using System.Collections.Generic;
using UnityEngine;

namespace BezierSolution
{
	// Token: 0x02000356 RID: 854
	[ExecuteInEditMode]
	public class BezierSpline : MonoBehaviour
	{
		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06003349 RID: 13129 RVA: 0x0019DDB9 File Offset: 0x0019BFB9
		public int Count
		{
			get
			{
				return this.endPoints.Count;
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x0600334A RID: 13130 RVA: 0x0019DDC6 File Offset: 0x0019BFC6
		public float Length
		{
			get
			{
				return this.GetLengthApproximately(0f, 1f, 50f);
			}
		}

		// Token: 0x170002AD RID: 685
		public BezierPoint this[int index]
		{
			get
			{
				if (index < this.Count)
				{
					return this.endPoints[index];
				}
				Debug.LogError(string.Concat(new object[]
				{
					"Bezier index ",
					index,
					" is out of range: ",
					this.Count
				}));
				return null;
			}
		}

		// Token: 0x0600334C RID: 13132 RVA: 0x0019DE3B File Offset: 0x0019C03B
		private void Awake()
		{
			this.Refresh();
		}

		// Token: 0x0600334D RID: 13133 RVA: 0x0019DE44 File Offset: 0x0019C044
		public void Initialize(int endPointsCount)
		{
			if (endPointsCount < 2)
			{
				Debug.LogError("Can't initialize spline with " + endPointsCount + " point(s). At least 2 points are needed");
				return;
			}
			this.Refresh();
			for (int i = this.endPoints.Count - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(this.endPoints[i].gameObject);
			}
			this.endPoints.Clear();
			for (int j = 0; j < endPointsCount; j++)
			{
				this.InsertNewPointAt(j);
			}
			this.Refresh();
		}

		// Token: 0x0600334E RID: 13134 RVA: 0x0019DEC9 File Offset: 0x0019C0C9
		public void Refresh()
		{
			this.endPoints.Clear();
			base.GetComponentsInChildren<BezierPoint>(this.endPoints);
		}

		// Token: 0x0600334F RID: 13135 RVA: 0x0019DEE4 File Offset: 0x0019C0E4
		public BezierPoint InsertNewPointAt(int index)
		{
			if (index < 0 || index > this.endPoints.Count)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Index ",
					index,
					" is out of range: [0,",
					this.endPoints.Count,
					"]"
				}));
				return null;
			}
			int count = this.endPoints.Count;
			BezierPoint bezierPoint = new GameObject("Point").AddComponent<BezierPoint>();
			bezierPoint.transform.SetParent((this.endPoints.Count == 0) ? base.transform : ((index == 0) ? this.endPoints[0].transform.parent : this.endPoints[index - 1].transform.parent), false);
			bezierPoint.transform.SetSiblingIndex((index == 0) ? 0 : (this.endPoints[index - 1].transform.GetSiblingIndex() + 1));
			if (this.endPoints.Count == count)
			{
				this.endPoints.Insert(index, bezierPoint);
			}
			return bezierPoint;
		}

		// Token: 0x06003350 RID: 13136 RVA: 0x0019E000 File Offset: 0x0019C200
		public BezierPoint DuplicatePointAt(int index)
		{
			if (index < 0 || index >= this.endPoints.Count)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Index ",
					index,
					" is out of range: [0,",
					this.endPoints.Count - 1,
					"]"
				}));
				return null;
			}
			BezierPoint bezierPoint = this.InsertNewPointAt(index + 1);
			this.endPoints[index].CopyTo(bezierPoint);
			return bezierPoint;
		}

		// Token: 0x06003351 RID: 13137 RVA: 0x0019E084 File Offset: 0x0019C284
		public void RemovePointAt(int index)
		{
			if (this.endPoints.Count <= 2)
			{
				Debug.LogError("Can't remove point: spline must consist of at least two points!");
				return;
			}
			if (index < 0 || index >= this.endPoints.Count)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Index ",
					index,
					" is out of range: [0,",
					this.endPoints.Count,
					")"
				}));
				return;
			}
			Component component = this.endPoints[index];
			this.endPoints.RemoveAt(index);
			Object.DestroyImmediate(component.gameObject);
		}

		// Token: 0x06003352 RID: 13138 RVA: 0x0019E124 File Offset: 0x0019C324
		public void SwapPointsAt(int index1, int index2)
		{
			if (index1 == index2)
			{
				Debug.LogError("Indices can't be equal to each other");
				return;
			}
			if (index1 < 0 || index1 >= this.endPoints.Count || index2 < 0 || index2 >= this.endPoints.Count)
			{
				Debug.LogError("Indices must be in range [0," + (this.endPoints.Count - 1) + "]");
				return;
			}
			BezierPoint bezierPoint = this.endPoints[index1];
			int siblingIndex = bezierPoint.transform.GetSiblingIndex();
			this.endPoints[index1] = this.endPoints[index2];
			this.endPoints[index2] = bezierPoint;
			bezierPoint.transform.SetSiblingIndex(this.endPoints[index1].transform.GetSiblingIndex());
			this.endPoints[index1].transform.SetSiblingIndex(siblingIndex);
		}

		// Token: 0x06003353 RID: 13139 RVA: 0x0019E201 File Offset: 0x0019C401
		public int IndexOf(BezierPoint point)
		{
			return this.endPoints.IndexOf(point);
		}

		// Token: 0x06003354 RID: 13140 RVA: 0x0019E20F File Offset: 0x0019C40F
		public void DrawGizmos(Color color, int smoothness = 4)
		{
			this.drawGizmos = true;
			this.gizmoColor = color;
			this.gizmoStep = 1f / (float)(this.endPoints.Count * Mathf.Clamp(smoothness, 1, 30));
		}

		// Token: 0x06003355 RID: 13141 RVA: 0x0019E241 File Offset: 0x0019C441
		public void HideGizmos()
		{
			this.drawGizmos = false;
		}

		// Token: 0x06003356 RID: 13142 RVA: 0x0019E24C File Offset: 0x0019C44C
		public Vector3 GetPoint(float normalizedT)
		{
			if (normalizedT <= 0f)
			{
				return this.endPoints[0].position;
			}
			if (normalizedT < 1f)
			{
				float num = normalizedT * (float)(this.loop ? this.endPoints.Count : (this.endPoints.Count - 1));
				int num2 = (int)num;
				int num3 = num2 + 1;
				if (num3 == this.endPoints.Count)
				{
					num3 = 0;
				}
				BezierPoint bezierPoint = this.endPoints[num2];
				BezierPoint bezierPoint2 = this.endPoints[num3];
				float num4 = num - (float)num2;
				float num5 = 1f - num4;
				return num5 * num5 * num5 * bezierPoint.position + 3f * num5 * num5 * num4 * bezierPoint.followingControlPointPosition + 3f * num5 * num4 * num4 * bezierPoint2.precedingControlPointPosition + num4 * num4 * num4 * bezierPoint2.position;
			}
			if (this.loop)
			{
				return this.endPoints[0].position;
			}
			return this.endPoints[this.endPoints.Count - 1].position;
		}

		// Token: 0x06003357 RID: 13143 RVA: 0x0019E380 File Offset: 0x0019C580
		public Vector3 GetTangent(float normalizedT)
		{
			if (normalizedT <= 0f)
			{
				return 3f * (this.endPoints[0].followingControlPointPosition - this.endPoints[0].position);
			}
			if (normalizedT < 1f)
			{
				float num = normalizedT * (float)(this.loop ? this.endPoints.Count : (this.endPoints.Count - 1));
				int num2 = (int)num;
				int num3 = num2 + 1;
				if (num3 == this.endPoints.Count)
				{
					num3 = 0;
				}
				BezierPoint bezierPoint = this.endPoints[num2];
				BezierPoint bezierPoint2 = this.endPoints[num3];
				float num4 = num - (float)num2;
				float num5 = 1f - num4;
				return 3f * num5 * num5 * (bezierPoint.followingControlPointPosition - bezierPoint.position) + 6f * num5 * num4 * (bezierPoint2.precedingControlPointPosition - bezierPoint.followingControlPointPosition) + 3f * num4 * num4 * (bezierPoint2.position - bezierPoint2.precedingControlPointPosition);
			}
			if (this.loop)
			{
				return 3f * (this.endPoints[0].position - this.endPoints[0].precedingControlPointPosition);
			}
			int index = this.endPoints.Count - 1;
			return 3f * (this.endPoints[index].position - this.endPoints[index].precedingControlPointPosition);
		}

		// Token: 0x06003358 RID: 13144 RVA: 0x0019E520 File Offset: 0x0019C720
		public float GetLengthApproximately(float startNormalizedT, float endNormalizedT, float accuracy = 50f)
		{
			if (endNormalizedT < startNormalizedT)
			{
				float num = startNormalizedT;
				startNormalizedT = endNormalizedT;
				endNormalizedT = num;
			}
			if (startNormalizedT < 0f)
			{
				startNormalizedT = 0f;
			}
			if (endNormalizedT > 1f)
			{
				endNormalizedT = 1f;
			}
			float num2 = this.AccuracyToStepSize(accuracy) * (endNormalizedT - startNormalizedT);
			float num3 = 0f;
			Vector3 vector = this.GetPoint(startNormalizedT);
			for (float num4 = startNormalizedT + num2; num4 < endNormalizedT; num4 += num2)
			{
				Vector3 point = this.GetPoint(num4);
				num3 += Vector3.Distance(point, vector);
				vector = point;
			}
			return num3 + Vector3.Distance(vector, this.GetPoint(endNormalizedT));
		}

		// Token: 0x06003359 RID: 13145 RVA: 0x0019E5A8 File Offset: 0x0019C7A8
		public Vector3 FindNearestPointTo(Vector3 worldPos, float accuracy = 100f)
		{
			float num;
			return this.FindNearestPointTo(worldPos, out num, accuracy);
		}

		// Token: 0x0600335A RID: 13146 RVA: 0x0019E5C0 File Offset: 0x0019C7C0
		public Vector3 FindNearestPointTo(Vector3 worldPos, out float normalizedT, float accuracy = 100f)
		{
			Vector3 result = Vector3.zero;
			normalizedT = -1f;
			float num = this.AccuracyToStepSize(accuracy);
			float num2 = float.PositiveInfinity;
			for (float num3 = 0f; num3 < 1f; num3 += num)
			{
				Vector3 point = this.GetPoint(num3);
				float sqrMagnitude = (worldPos - point).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					result = point;
					normalizedT = num3;
				}
			}
			return result;
		}

		// Token: 0x0600335B RID: 13147 RVA: 0x0019E628 File Offset: 0x0019C828
		public Vector3 MoveAlongSpline(ref float normalizedT, float deltaMovement, int accuracy = 3)
		{
			float num = 1f / (float)this.endPoints.Count;
			for (int i = 0; i < accuracy; i++)
			{
				normalizedT += deltaMovement * num / ((float)accuracy * this.GetTangent(normalizedT).magnitude);
			}
			return this.GetPoint(normalizedT);
		}

		// Token: 0x0600335C RID: 13148 RVA: 0x0019E678 File Offset: 0x0019C878
		public void ConstructLinearPath()
		{
			for (int i = 0; i < this.endPoints.Count; i++)
			{
				this.endPoints[i].handleMode = BezierPoint.HandleMode.Free;
				if (i < this.endPoints.Count - 1)
				{
					Vector3 vector = (this.endPoints[i].position + this.endPoints[i + 1].position) * 0.5f;
					this.endPoints[i].followingControlPointPosition = vector;
					this.endPoints[i + 1].precedingControlPointPosition = vector;
				}
				else
				{
					Vector3 vector2 = (this.endPoints[i].position + this.endPoints[0].position) * 0.5f;
					this.endPoints[i].followingControlPointPosition = vector2;
					this.endPoints[0].precedingControlPointPosition = vector2;
				}
			}
		}

		// Token: 0x0600335D RID: 13149 RVA: 0x0019E778 File Offset: 0x0019C978
		public void AutoConstructSpline()
		{
			for (int i = 0; i < this.endPoints.Count; i++)
			{
				this.endPoints[i].handleMode = BezierPoint.HandleMode.Mirrored;
			}
			int num = this.endPoints.Count - 1;
			if (num == 1)
			{
				this.endPoints[0].followingControlPointPosition = (2f * this.endPoints[0].position + this.endPoints[1].position) / 3f;
				this.endPoints[1].precedingControlPointPosition = 2f * this.endPoints[0].followingControlPointPosition - this.endPoints[0].position;
				return;
			}
			Vector3[] array;
			if (this.loop)
			{
				array = new Vector3[num + 1];
			}
			else
			{
				array = new Vector3[num];
			}
			for (int j = 1; j < num - 1; j++)
			{
				array[j] = 4f * this.endPoints[j].position + 2f * this.endPoints[j + 1].position;
			}
			array[0] = this.endPoints[0].position + 2f * this.endPoints[1].position;
			if (!this.loop)
			{
				array[num - 1] = (8f * this.endPoints[num - 1].position + this.endPoints[num].position) * 0.5f;
			}
			else
			{
				array[num - 1] = 4f * this.endPoints[num - 1].position + 2f * this.endPoints[num].position;
				array[num] = (8f * this.endPoints[num].position + this.endPoints[0].position) * 0.5f;
			}
			Vector3[] firstControlPoints = BezierSpline.GetFirstControlPoints(array);
			for (int k = 0; k < num; k++)
			{
				this.endPoints[k].followingControlPointPosition = firstControlPoints[k];
				if (this.loop)
				{
					this.endPoints[k + 1].precedingControlPointPosition = 2f * this.endPoints[k + 1].position - firstControlPoints[k + 1];
				}
				else if (k < num - 1)
				{
					this.endPoints[k + 1].precedingControlPointPosition = 2f * this.endPoints[k + 1].position - firstControlPoints[k + 1];
				}
				else
				{
					this.endPoints[k + 1].precedingControlPointPosition = (this.endPoints[num].position + firstControlPoints[num - 1]) * 0.5f;
				}
			}
			if (this.loop)
			{
				float d = Vector3.Distance(this.endPoints[0].followingControlPointPosition, this.endPoints[0].position);
				Vector3 a = Vector3.Normalize(this.endPoints[num].position - this.endPoints[1].position);
				this.endPoints[0].precedingControlPointPosition = this.endPoints[0].position + a * d * 0.5f;
				this.endPoints[0].followingControlPointLocalPosition = -this.endPoints[0].precedingControlPointLocalPosition;
			}
		}

		// Token: 0x0600335E RID: 13150 RVA: 0x0019EBA8 File Offset: 0x0019CDA8
		private static Vector3[] GetFirstControlPoints(Vector3[] rhs)
		{
			int num = rhs.Length;
			Vector3[] array = new Vector3[num];
			float[] array2 = new float[num];
			float num2 = 2f;
			array[0] = rhs[0] / num2;
			for (int i = 1; i < num; i++)
			{
				float num3 = 1f / num2;
				array2[i] = num3;
				num2 = ((i < num - 1) ? 4f : 3.5f) - num3;
				array[i] = (rhs[i] - array[i - 1]) / num2;
			}
			for (int j = 1; j < num; j++)
			{
				array[num - j - 1] -= array2[num - j] * array[num - j];
			}
			return array;
		}

		// Token: 0x0600335F RID: 13151 RVA: 0x0019EC80 File Offset: 0x0019CE80
		public void AutoConstructSpline2()
		{
			for (int i = 0; i < this.endPoints.Count; i++)
			{
				Vector3 position;
				if (i == 0)
				{
					if (this.loop)
					{
						position = this.endPoints[this.endPoints.Count - 1].position;
					}
					else
					{
						position = this.endPoints[0].position;
					}
				}
				else
				{
					position = this.endPoints[i - 1].position;
				}
				Vector3 position2;
				Vector3 position3;
				if (this.loop)
				{
					position2 = this.endPoints[(i + 1) % this.endPoints.Count].position;
					position3 = this.endPoints[(i + 2) % this.endPoints.Count].position;
				}
				else if (i < this.endPoints.Count - 2)
				{
					position2 = this.endPoints[i + 1].position;
					position3 = this.endPoints[i + 2].position;
				}
				else if (i == this.endPoints.Count - 2)
				{
					position2 = this.endPoints[i + 1].position;
					position3 = this.endPoints[i + 1].position;
				}
				else
				{
					position2 = this.endPoints[i].position;
					position3 = this.endPoints[i].position;
				}
				this.endPoints[i].followingControlPointPosition = this.endPoints[i].position + (position2 - position) / 6f;
				this.endPoints[i].handleMode = BezierPoint.HandleMode.Mirrored;
				if (i < this.endPoints.Count - 1)
				{
					this.endPoints[i + 1].precedingControlPointPosition = position2 - (position3 - this.endPoints[i].position) / 6f;
				}
				else if (this.loop)
				{
					this.endPoints[0].precedingControlPointPosition = position2 - (position3 - this.endPoints[i].position) / 6f;
				}
			}
		}

		// Token: 0x06003360 RID: 13152 RVA: 0x0019EEB7 File Offset: 0x0019D0B7
		private float AccuracyToStepSize(float accuracy)
		{
			if (accuracy <= 0f)
			{
				return 0.2f;
			}
			return Mathf.Clamp(1f / accuracy, 0.001f, 0.2f);
		}

		// Token: 0x06003361 RID: 13153 RVA: 0x0019EEE0 File Offset: 0x0019D0E0
		private void OnRenderObject()
		{
			if (!this.drawGizmos || this.endPoints.Count < 2)
			{
				return;
			}
			if (!BezierSpline.gizmoMaterial)
			{
				BezierSpline.gizmoMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				BezierSpline.gizmoMaterial.SetInt("_SrcBlend", 5);
				BezierSpline.gizmoMaterial.SetInt("_DstBlend", 10);
				BezierSpline.gizmoMaterial.SetInt("_Cull", 0);
				BezierSpline.gizmoMaterial.SetInt("_ZWrite", 0);
			}
			BezierSpline.gizmoMaterial.SetPass(0);
			GL.Begin(1);
			GL.Color(this.gizmoColor);
			Vector3 vector = this.endPoints[0].position;
			for (float num = this.gizmoStep; num < 1f; num += this.gizmoStep)
			{
				GL.Vertex3(vector.x, vector.y, vector.z);
				vector = this.GetPoint(num);
				GL.Vertex3(vector.x, vector.y, vector.z);
			}
			GL.Vertex3(vector.x, vector.y, vector.z);
			vector = this.GetPoint(1f);
			GL.Vertex3(vector.x, vector.y, vector.z);
			GL.End();
		}

		// Token: 0x040022A1 RID: 8865
		private static Material gizmoMaterial;

		// Token: 0x040022A2 RID: 8866
		private Color gizmoColor = Color.white;

		// Token: 0x040022A3 RID: 8867
		private float gizmoStep = 0.05f;

		// Token: 0x040022A4 RID: 8868
		private List<BezierPoint> endPoints = new List<BezierPoint>();

		// Token: 0x040022A5 RID: 8869
		public bool loop;

		// Token: 0x040022A6 RID: 8870
		public bool drawGizmos;
	}
}
