using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200003B RID: 59
[RequireComponent(typeof(MeshFilter))]
public class RamSpline : MonoBehaviour
{
	// Token: 0x0600014C RID: 332 RVA: 0x0000C742 File Offset: 0x0000A942
	public void Start()
	{
		this.GenerateSpline(null);
	}

	// Token: 0x0600014D RID: 333 RVA: 0x0000C74C File Offset: 0x0000A94C
	public void GenerateBeginningParentBased()
	{
		this.vertsInShape = (int)Mathf.Round((float)(this.beginningSpline.vertsInShape - 1) * (this.beginningMaxWidth - this.beginningMinWidth) + 1f);
		if (this.vertsInShape < 1)
		{
			this.vertsInShape = 1;
		}
		this.beginningConnectionID = this.beginningSpline.points.Count - 1;
		Vector4 vector = this.beginningSpline.controlPoints[this.beginningSpline.controlPoints.Count - 1];
		float num = vector.w;
		num *= this.beginningMaxWidth - this.beginningMinWidth;
		vector = Vector3.Lerp(this.beginningSpline.pointsDown[this.beginningConnectionID], this.beginningSpline.pointsUp[this.beginningConnectionID], this.beginningMinWidth + (this.beginningMaxWidth - this.beginningMinWidth) * 0.5f) + this.beginningSpline.transform.position - base.transform.position;
		vector.w = num;
		this.controlPoints[0] = vector;
		if (!this.uvScaleOverride)
		{
			this.uvScale = this.beginningSpline.uvScale;
		}
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000C890 File Offset: 0x0000AA90
	public void GenerateEndingParentBased()
	{
		if (this.beginningSpline == null)
		{
			this.vertsInShape = (int)Mathf.Round((float)(this.endingSpline.vertsInShape - 1) * (this.endingMaxWidth - this.endingMinWidth) + 1f);
			if (this.vertsInShape < 1)
			{
				this.vertsInShape = 1;
			}
		}
		this.endingConnectionID = 0;
		Vector4 vector = this.endingSpline.controlPoints[0];
		float num = vector.w;
		num *= this.endingMaxWidth - this.endingMinWidth;
		vector = Vector3.Lerp(this.endingSpline.pointsDown[this.endingConnectionID], this.endingSpline.pointsUp[this.endingConnectionID], this.endingMinWidth + (this.endingMaxWidth - this.endingMinWidth) * 0.5f) + this.endingSpline.transform.position - base.transform.position;
		vector.w = num;
		this.controlPoints[this.controlPoints.Count - 1] = vector;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000C9B0 File Offset: 0x0000ABB0
	public void GenerateSpline(List<RamSpline> generatedSplines = null)
	{
		generatedSplines = new List<RamSpline>();
		if (this.beginningSpline)
		{
			this.GenerateBeginningParentBased();
		}
		if (this.endingSpline)
		{
			this.GenerateEndingParentBased();
		}
		List<Vector4> list = new List<Vector4>();
		for (int i = 0; i < this.controlPoints.Count; i++)
		{
			if (i > 0)
			{
				if (Vector3.Distance(this.controlPoints[i], this.controlPoints[i - 1]) > 0f)
				{
					list.Add(this.controlPoints[i]);
				}
			}
			else
			{
				list.Add(this.controlPoints[i]);
			}
		}
		Mesh mesh = new Mesh();
		this.meshfilter = base.GetComponent<MeshFilter>();
		if (list.Count < 2)
		{
			mesh.Clear();
			this.meshfilter.mesh = mesh;
			return;
		}
		this.controlPointsOrientation = new List<Quaternion>();
		this.lerpValues.Clear();
		this.snaps.Clear();
		this.points.Clear();
		this.pointsUp.Clear();
		this.pointsDown.Clear();
		this.orientations.Clear();
		this.tangents.Clear();
		this.normalsList.Clear();
		this.widths.Clear();
		this.controlPointsUp.Clear();
		this.controlPointsDown.Clear();
		this.verticesBeginning.Clear();
		this.verticesEnding.Clear();
		this.normalsBeginning.Clear();
		this.normalsEnding.Clear();
		if (this.beginningSpline != null && this.beginningSpline.controlPointsRotations.Count > 0)
		{
			this.controlPointsRotations[0] = Quaternion.identity;
		}
		if (this.endingSpline != null && this.endingSpline.controlPointsRotations.Count > 0)
		{
			this.controlPointsRotations[this.controlPointsRotations.Count - 1] = Quaternion.identity;
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (j <= list.Count - 2)
			{
				this.CalculateCatmullRomSideSplines(list, j);
			}
		}
		if (this.beginningSpline != null && this.beginningSpline.controlPointsRotations.Count > 0)
		{
			this.controlPointsRotations[0] = Quaternion.Inverse(this.controlPointsOrientation[0]) * this.beginningSpline.controlPointsOrientation[this.beginningSpline.controlPointsOrientation.Count - 1];
		}
		if (this.endingSpline != null && this.endingSpline.controlPointsRotations.Count > 0)
		{
			this.controlPointsRotations[this.controlPointsRotations.Count - 1] = Quaternion.Inverse(this.controlPointsOrientation[this.controlPointsOrientation.Count - 1]) * this.endingSpline.controlPointsOrientation[0];
		}
		this.controlPointsOrientation = new List<Quaternion>();
		this.controlPointsUp.Clear();
		this.controlPointsDown.Clear();
		for (int k = 0; k < list.Count; k++)
		{
			if (k <= list.Count - 2)
			{
				this.CalculateCatmullRomSideSplines(list, k);
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			if (l <= list.Count - 2)
			{
				this.CalculateCatmullRomSplineParameters(list, l, false);
			}
		}
		for (int m = 0; m < this.controlPointsUp.Count; m++)
		{
			if (m <= this.controlPointsUp.Count - 2)
			{
				this.CalculateCatmullRomSpline(this.controlPointsUp, m, ref this.pointsUp);
			}
		}
		for (int n = 0; n < this.controlPointsDown.Count; n++)
		{
			if (n <= this.controlPointsDown.Count - 2)
			{
				this.CalculateCatmullRomSpline(this.controlPointsDown, n, ref this.pointsDown);
			}
		}
		this.GenerateMesh(ref mesh);
		if (generatedSplines != null)
		{
			generatedSplines.Add(this);
			foreach (RamSpline ramSpline in this.beginnigChildSplines)
			{
				if (ramSpline != null && !generatedSplines.Contains(ramSpline) && (ramSpline.beginningSpline == this || ramSpline.endingSpline == this))
				{
					ramSpline.GenerateSpline(generatedSplines);
				}
			}
			foreach (RamSpline ramSpline2 in this.endingChildSplines)
			{
				if (ramSpline2 != null && !generatedSplines.Contains(ramSpline2) && (ramSpline2.beginningSpline == this || ramSpline2.endingSpline == this))
				{
					ramSpline2.GenerateSpline(generatedSplines);
				}
			}
		}
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000CEA0 File Offset: 0x0000B0A0
	private void CalculateCatmullRomSideSplines(List<Vector4> controlPoints, int pos)
	{
		Vector3 p = controlPoints[pos];
		Vector3 p2 = controlPoints[pos];
		Vector3 p3 = controlPoints[this.ClampListPos(pos + 1)];
		Vector3 p4 = controlPoints[this.ClampListPos(pos + 1)];
		if (pos > 0)
		{
			p = controlPoints[this.ClampListPos(pos - 1)];
		}
		if (pos < controlPoints.Count - 2)
		{
			p4 = controlPoints[this.ClampListPos(pos + 2)];
		}
		int num = 0;
		if (pos == controlPoints.Count - 2)
		{
			num = 1;
		}
		for (int i = 0; i <= num; i++)
		{
			Vector3 catmullRomPosition = this.GetCatmullRomPosition((float)i, p, p2, p3, p4);
			Vector3 normalized = this.GetCatmullRomTangent((float)i, p, p2, p3, p4).normalized;
			Vector3 normalized2 = this.CalculateNormal(normalized, Vector3.up).normalized;
			Quaternion quaternion;
			if (normalized2 == normalized && normalized2 == Vector3.zero)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				quaternion = Quaternion.LookRotation(normalized, normalized2);
			}
			quaternion *= Quaternion.Lerp(this.controlPointsRotations[pos], this.controlPointsRotations[this.ClampListPos(pos + 1)], (float)i);
			this.controlPointsOrientation.Add(quaternion);
			Vector3 item = catmullRomPosition + quaternion * (0.5f * controlPoints[pos + i].w * Vector3.right);
			Vector3 item2 = catmullRomPosition + quaternion * (0.5f * controlPoints[pos + i].w * Vector3.left);
			this.controlPointsUp.Add(item);
			this.controlPointsDown.Add(item2);
		}
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0000D070 File Offset: 0x0000B270
	private void CalculateCatmullRomSplineParameters(List<Vector4> controlPoints, int pos, bool initialPoints = false)
	{
		Vector3 p = controlPoints[pos];
		Vector3 p2 = controlPoints[pos];
		Vector3 p3 = controlPoints[this.ClampListPos(pos + 1)];
		Vector3 p4 = controlPoints[this.ClampListPos(pos + 1)];
		if (pos > 0)
		{
			p = controlPoints[this.ClampListPos(pos - 1)];
		}
		if (pos < controlPoints.Count - 2)
		{
			p4 = controlPoints[this.ClampListPos(pos + 2)];
		}
		int num = Mathf.FloorToInt(1f / this.traingleDensity);
		float num2 = 0f;
		if (pos > 0)
		{
			num2 = 1f;
		}
		float num3;
		for (num3 = num2; num3 <= (float)num; num3 += 1f)
		{
			float t = num3 * this.traingleDensity;
			this.CalculatePointParameters(controlPoints, pos, p, p2, p3, p4, t);
		}
		if (num3 < (float)num)
		{
			num3 = (float)num;
			float t2 = num3 * this.traingleDensity;
			this.CalculatePointParameters(controlPoints, pos, p, p2, p3, p4, t2);
		}
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000D17C File Offset: 0x0000B37C
	private void CalculateCatmullRomSpline(List<Vector3> controlPoints, int pos, ref List<Vector3> points)
	{
		Vector3 p = controlPoints[pos];
		Vector3 p2 = controlPoints[pos];
		Vector3 p3 = controlPoints[this.ClampListPos(pos + 1)];
		Vector3 p4 = controlPoints[this.ClampListPos(pos + 1)];
		if (pos > 0)
		{
			p = controlPoints[this.ClampListPos(pos - 1)];
		}
		if (pos < controlPoints.Count - 2)
		{
			p4 = controlPoints[this.ClampListPos(pos + 2)];
		}
		int num = Mathf.FloorToInt(1f / this.traingleDensity);
		float num2 = 0f;
		if (pos > 0)
		{
			num2 = 1f;
		}
		float num3;
		for (num3 = num2; num3 <= (float)num; num3 += 1f)
		{
			float t = num3 * this.traingleDensity;
			this.CalculatePointPosition(controlPoints, pos, p, p2, p3, p4, t, ref points);
		}
		if (num3 < (float)num)
		{
			num3 = (float)num;
			float t2 = num3 * this.traingleDensity;
			this.CalculatePointPosition(controlPoints, pos, p, p2, p3, p4, t2, ref points);
		}
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000D26C File Offset: 0x0000B46C
	private void CalculatePointPosition(List<Vector3> controlPoints, int pos, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, ref List<Vector3> points)
	{
		Vector3 catmullRomPosition = this.GetCatmullRomPosition(t, p0, p1, p2, p3);
		points.Add(catmullRomPosition);
		Vector3 normalized = this.GetCatmullRomTangent(t, p0, p1, p2, p3).normalized;
		Vector3 normalized2 = this.CalculateNormal(normalized, Vector3.up).normalized;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000D2C0 File Offset: 0x0000B4C0
	private void CalculatePointParameters(List<Vector4> controlPoints, int pos, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 catmullRomPosition = this.GetCatmullRomPosition(t, p0, p1, p2, p3);
		this.widths.Add(Mathf.Lerp(controlPoints[pos].w, controlPoints[this.ClampListPos(pos + 1)].w, t));
		if (this.controlPointsSnap.Count > pos + 1)
		{
			this.snaps.Add(Mathf.Lerp(this.controlPointsSnap[pos], this.controlPointsSnap[this.ClampListPos(pos + 1)], t));
		}
		else
		{
			this.snaps.Add(0f);
		}
		this.lerpValues.Add((float)pos + t);
		this.points.Add(catmullRomPosition);
		Vector3 normalized = this.GetCatmullRomTangent(t, p0, p1, p2, p3).normalized;
		Vector3 vector = this.CalculateNormal(normalized, Vector3.up).normalized;
		Quaternion quaternion;
		if (vector == normalized && vector == Vector3.zero)
		{
			quaternion = Quaternion.identity;
		}
		else
		{
			quaternion = Quaternion.LookRotation(normalized, vector);
		}
		quaternion *= Quaternion.Lerp(this.controlPointsRotations[pos], this.controlPointsRotations[this.ClampListPos(pos + 1)], t);
		this.orientations.Add(quaternion);
		this.tangents.Add(normalized);
		if (this.normalsList.Count > 0 && Vector3.Angle(this.normalsList[this.normalsList.Count - 1], vector) > 90f)
		{
			vector *= -1f;
		}
		this.normalsList.Add(vector);
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000D465 File Offset: 0x0000B665
	private int ClampListPos(int pos)
	{
		if (pos < 0)
		{
			pos = this.controlPoints.Count - 1;
		}
		if (pos > this.controlPoints.Count)
		{
			pos = 1;
		}
		else if (pos > this.controlPoints.Count - 1)
		{
			pos = 0;
		}
		return pos;
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000D4A4 File Offset: 0x0000B6A4
	private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 a = 2f * p1;
		Vector3 a2 = p2 - p0;
		Vector3 a3 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 a4 = -p0 + 3f * p1 - 3f * p2 + p3;
		return 0.5f * (a + a2 * t + a3 * t * t + a4 * t * t * t);
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000D56C File Offset: 0x0000B76C
	private Vector3 GetCatmullRomTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		return 0.5f * (-p0 + p2 + 2f * (2f * p0 - 5f * p1 + 4f * p2 - p3) * t + 3f * (-p0 + 3f * p1 - 3f * p2 + p3) * t * t);
	}

	// Token: 0x06000158 RID: 344 RVA: 0x0000D624 File Offset: 0x0000B824
	private Vector3 CalculateNormal(Vector3 tangent, Vector3 up)
	{
		Vector3 rhs = Vector3.Cross(up, tangent);
		return Vector3.Cross(tangent, rhs);
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000D640 File Offset: 0x0000B840
	private void GenerateMesh(ref Mesh mesh)
	{
		int num = this.points.Count - 1;
		int count = this.points.Count;
		int num2 = this.vertsInShape * count;
		List<int> list = new List<int>();
		Vector3[] array = new Vector3[num2];
		Vector3[] array2 = new Vector3[num2];
		Vector2[] array3 = new Vector2[num2];
		Vector2[] array4 = new Vector2[num2];
		Vector2[] array5 = new Vector2[num2];
		if (this.colors == null || this.colors.Length != num2)
		{
			this.colors = new Color[num2];
			for (int i = 0; i < this.colors.Length; i++)
			{
				this.colors[i] = Color.black;
			}
		}
		if (this.colorsFlowMap.Count != num2)
		{
			this.colorsFlowMap.Clear();
		}
		this.length = 0f;
		this.fulllength = 0f;
		if (this.beginningSpline != null)
		{
			this.length = this.beginningSpline.length;
		}
		this.minMaxWidth = 1f;
		this.uvWidth = 1f;
		this.uvBeginning = 0f;
		if (this.beginningSpline != null)
		{
			this.minMaxWidth = this.beginningMaxWidth - this.beginningMinWidth;
			this.uvWidth = this.minMaxWidth * this.beginningSpline.uvWidth;
			this.uvBeginning = this.beginningSpline.uvWidth * this.beginningMinWidth + this.beginningSpline.uvBeginning;
		}
		else if (this.endingSpline != null)
		{
			this.minMaxWidth = this.endingMaxWidth - this.endingMinWidth;
			this.uvWidth = this.minMaxWidth * this.endingSpline.uvWidth;
			this.uvBeginning = this.endingSpline.uvWidth * this.endingMinWidth + this.endingSpline.uvBeginning;
		}
		for (int j = 0; j < this.pointsDown.Count; j++)
		{
			float num3 = this.widths[j];
			if (j > 0)
			{
				this.fulllength += this.uvWidth * Vector3.Distance(this.pointsDown[j], this.pointsDown[j - 1]) / (this.uvScale * num3);
			}
		}
		float num4 = Mathf.Round(this.fulllength);
		for (int k = 0; k < this.pointsDown.Count; k++)
		{
			float num5 = this.widths[k];
			int num6 = k * this.vertsInShape;
			if (k > 0)
			{
				this.length += this.uvWidth * Vector3.Distance(this.pointsDown[k], this.pointsDown[k - 1]) / (this.uvScale * num5) / this.fulllength * num4;
			}
			float num7 = 0f;
			float num8 = 0f;
			for (int l = 0; l < this.vertsInShape; l++)
			{
				int num9 = num6 + l;
				float num10 = (float)l / (float)(this.vertsInShape - 1);
				if (num10 < 0.5f)
				{
					num10 *= this.minVal * 2f;
				}
				else
				{
					num10 = ((num10 - 0.5f) * (1f - this.maxVal) + 0.5f * this.maxVal) * 2f;
				}
				if (k == 0 && this.beginningSpline != null && this.beginningSpline.verticesEnding != null && this.beginningSpline.normalsEnding != null)
				{
					int num11 = (int)((float)this.beginningSpline.vertsInShape * this.beginningMinWidth);
					array[num9] = this.beginningSpline.verticesEnding[Mathf.Clamp(l + num11, 0, this.beginningSpline.verticesEnding.Count - 1)] + this.beginningSpline.transform.position - base.transform.position;
				}
				else if (k == this.pointsDown.Count - 1 && this.endingSpline != null && this.endingSpline.verticesBeginning != null && this.endingSpline.normalsBeginning != null)
				{
					int num12 = (int)((float)this.endingSpline.vertsInShape * this.endingMinWidth);
					array[num9] = this.endingSpline.verticesBeginning[Mathf.Clamp(l + num12, 0, this.endingSpline.verticesBeginning.Count - 1)] + this.endingSpline.transform.position - base.transform.position;
				}
				else
				{
					array[num9] = Vector3.Lerp(this.pointsDown[k], this.pointsUp[k], num10);
					RaycastHit raycastHit;
					if (Physics.Raycast(array[num9] + base.transform.position + Vector3.up * 5f, Vector3.down, out raycastHit, 1000f, this.snapMask.value))
					{
						array[num9] = Vector3.Lerp(array[num9], raycastHit.point - base.transform.position + new Vector3(0f, 0.1f, 0f), (Mathf.Sin(3.1415927f * this.snaps[k] - 1.5707964f) + 1f) * 0.5f);
					}
					RaycastHit raycastHit2;
					if (this.normalFromRaycast && Physics.Raycast(this.points[k] + base.transform.position + Vector3.up * 5f, Vector3.down, out raycastHit2, 1000f, this.snapMask.value))
					{
						array2[num9] = raycastHit2.normal;
					}
					Vector3[] array6 = array;
					int num13 = num9;
					array6[num13].y = array6[num13].y + Mathf.Lerp(this.controlPointsMeshCurves[Mathf.FloorToInt(this.lerpValues[k])].Evaluate(num10), this.controlPointsMeshCurves[Mathf.CeilToInt(this.lerpValues[k])].Evaluate(num10), this.lerpValues[k] - Mathf.Floor(this.lerpValues[k]));
				}
				if (k > 0 && k < 5 && this.beginningSpline != null && this.beginningSpline.verticesEnding != null)
				{
					array[num9].y = (array[num9].y + array[num9 - this.vertsInShape].y) * 0.5f;
				}
				if (k == this.pointsDown.Count - 1 && this.endingSpline != null && this.endingSpline.verticesBeginning != null)
				{
					for (int m = 1; m < 5; m++)
					{
						array[num9 - this.vertsInShape * m].y = (array[num9 - this.vertsInShape * (m - 1)].y + array[num9 - this.vertsInShape * m].y) * 0.5f;
					}
				}
				if (k == 0)
				{
					this.verticesBeginning.Add(array[num9]);
				}
				if (k == this.pointsDown.Count - 1)
				{
					this.verticesEnding.Add(array[num9]);
				}
				if (!this.normalFromRaycast)
				{
					array2[num9] = this.orientations[k] * Vector3.up;
				}
				if (k == 0)
				{
					this.normalsBeginning.Add(array2[num9]);
				}
				if (k == this.pointsDown.Count - 1)
				{
					this.normalsEnding.Add(array2[num9]);
				}
				if (l > 0)
				{
					num7 = num10 * this.uvWidth;
					num8 = num10;
				}
				if (this.beginningSpline != null || this.endingSpline != null)
				{
					num7 += this.uvBeginning;
				}
				num7 /= this.uvScale;
				float num14 = this.FlowCalculate(num8, array2[num9].y);
				int num15 = 10;
				if (this.beginnigChildSplines.Count > 0 && k <= num15)
				{
					float num16 = 0f;
					foreach (RamSpline ramSpline in this.beginnigChildSplines)
					{
						if (Mathf.CeilToInt(ramSpline.endingMaxWidth * (float)(this.vertsInShape - 1)) >= l && l >= Mathf.CeilToInt(ramSpline.endingMinWidth * (float)(this.vertsInShape - 1)))
						{
							num16 = (float)(l - Mathf.CeilToInt(ramSpline.endingMinWidth * (float)(this.vertsInShape - 1))) / (float)(Mathf.CeilToInt(ramSpline.endingMaxWidth * (float)(this.vertsInShape - 1)) - Mathf.CeilToInt(ramSpline.endingMinWidth * (float)(this.vertsInShape - 1)));
							num16 = this.FlowCalculate(num16, array2[num9].y);
						}
					}
					if (k > 0)
					{
						num14 = Mathf.Lerp(num14, num16, 1f - (float)k / (float)num15);
					}
					else
					{
						num14 = num16;
					}
				}
				if (k >= this.pointsDown.Count - num15 - 1 && this.endingChildSplines.Count > 0)
				{
					float num17 = 0f;
					foreach (RamSpline ramSpline2 in this.endingChildSplines)
					{
						if (Mathf.CeilToInt(ramSpline2.beginningMaxWidth * (float)(this.vertsInShape - 1)) >= l && l >= Mathf.CeilToInt(ramSpline2.beginningMinWidth * (float)(this.vertsInShape - 1)))
						{
							num17 = (float)(l - Mathf.CeilToInt(ramSpline2.beginningMinWidth * (float)(this.vertsInShape - 1))) / (float)(Mathf.CeilToInt(ramSpline2.beginningMaxWidth * (float)(this.vertsInShape - 1)) - Mathf.CeilToInt(ramSpline2.beginningMinWidth * (float)(this.vertsInShape - 1)));
							num17 = this.FlowCalculate(num17, array2[num9].y);
						}
					}
					if (k < this.pointsDown.Count - 1)
					{
						num14 = Mathf.Lerp(num14, num17, (float)(k - (this.pointsDown.Count - num15 - 1)) / (float)num15);
					}
					else
					{
						num14 = num17;
					}
				}
				float num18 = -(num8 - 0.5f) * 0.01f;
				if (this.uvRotation)
				{
					if (!this.invertUVDirection)
					{
						array3[num9] = new Vector2(1f - this.length, num7);
						array4[num9] = new Vector2(1f - this.length / this.fulllength, num8);
						array5[num9] = new Vector2(num14, num18);
					}
					else
					{
						array3[num9] = new Vector2(1f + this.length, num7);
						array4[num9] = new Vector2(1f + this.length / this.fulllength, num8);
						array5[num9] = new Vector2(num14, num18);
					}
				}
				else if (!this.invertUVDirection)
				{
					array3[num9] = new Vector2(num7, 1f - this.length);
					array4[num9] = new Vector2(num8, 1f - this.length / this.fulllength);
					array5[num9] = new Vector2(num18, num14);
				}
				else
				{
					array3[num9] = new Vector2(num7, 1f + this.length);
					array4[num9] = new Vector2(num8, 1f + this.length / this.fulllength);
					array5[num9] = new Vector2(num18, num14);
				}
				if (this.colorsFlowMap.Count <= num9)
				{
					this.colorsFlowMap.Add(array5[num9]);
				}
				else if (!this.overrideFlowMap)
				{
					this.colorsFlowMap[num9] = array5[num9];
				}
			}
		}
		for (int n = 0; n < num; n++)
		{
			int num19 = n * this.vertsInShape;
			for (int num20 = 0; num20 < this.vertsInShape - 1; num20++)
			{
				int item = num19 + num20;
				int item2 = num19 + num20 + this.vertsInShape;
				int item3 = num19 + num20 + 1 + this.vertsInShape;
				int item4 = num19 + num20 + 1;
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				list.Add(item3);
				list.Add(item4);
				list.Add(item);
			}
		}
		mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.uv3 = array4;
		mesh.uv4 = this.colorsFlowMap.ToArray();
		mesh.triangles = list.ToArray();
		mesh.colors = this.colors;
		mesh.RecalculateTangents();
		this.meshfilter.mesh = mesh;
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0000E3E8 File Offset: 0x0000C5E8
	private float FlowCalculate(float u, float normalY)
	{
		return Mathf.Lerp(this.flowWaterfall.Evaluate(u), this.flowFlat.Evaluate(u), Mathf.Clamp(normalY, 0f, 1f));
	}

	// Token: 0x0400020F RID: 527
	public SplineProfile currentProfile;

	// Token: 0x04000210 RID: 528
	public SplineProfile oldProfile;

	// Token: 0x04000211 RID: 529
	public List<RamSpline> beginnigChildSplines = new List<RamSpline>();

	// Token: 0x04000212 RID: 530
	public List<RamSpline> endingChildSplines = new List<RamSpline>();

	// Token: 0x04000213 RID: 531
	public RamSpline beginningSpline;

	// Token: 0x04000214 RID: 532
	public RamSpline endingSpline;

	// Token: 0x04000215 RID: 533
	public int beginningConnectionID;

	// Token: 0x04000216 RID: 534
	public int endingConnectionID;

	// Token: 0x04000217 RID: 535
	public float beginningMinWidth = 0.5f;

	// Token: 0x04000218 RID: 536
	public float beginningMaxWidth = 1f;

	// Token: 0x04000219 RID: 537
	public float endingMinWidth = 0.5f;

	// Token: 0x0400021A RID: 538
	public float endingMaxWidth = 1f;

	// Token: 0x0400021B RID: 539
	public int toolbarInt;

	// Token: 0x0400021C RID: 540
	public bool invertUVDirection;

	// Token: 0x0400021D RID: 541
	public bool uvRotation = true;

	// Token: 0x0400021E RID: 542
	public MeshFilter meshfilter;

	// Token: 0x0400021F RID: 543
	public List<Vector4> controlPoints = new List<Vector4>();

	// Token: 0x04000220 RID: 544
	public List<Quaternion> controlPointsRotations = new List<Quaternion>();

	// Token: 0x04000221 RID: 545
	public List<Quaternion> controlPointsOrientation = new List<Quaternion>();

	// Token: 0x04000222 RID: 546
	public List<Vector3> controlPointsUp = new List<Vector3>();

	// Token: 0x04000223 RID: 547
	public List<Vector3> controlPointsDown = new List<Vector3>();

	// Token: 0x04000224 RID: 548
	public List<float> controlPointsSnap = new List<float>();

	// Token: 0x04000225 RID: 549
	public AnimationCurve meshCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04000226 RID: 550
	public List<AnimationCurve> controlPointsMeshCurves = new List<AnimationCurve>();

	// Token: 0x04000227 RID: 551
	public AnimationCurve terrainCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04000228 RID: 552
	public int detailTerrain = 100;

	// Token: 0x04000229 RID: 553
	public int detailTerrainForward = 100;

	// Token: 0x0400022A RID: 554
	public bool normalFromRaycast;

	// Token: 0x0400022B RID: 555
	public bool snapToTerrain;

	// Token: 0x0400022C RID: 556
	public LayerMask snapMask = 1;

	// Token: 0x0400022D RID: 557
	public List<Vector3> points = new List<Vector3>();

	// Token: 0x0400022E RID: 558
	public List<Vector3> pointsUp = new List<Vector3>();

	// Token: 0x0400022F RID: 559
	public List<Vector3> pointsDown = new List<Vector3>();

	// Token: 0x04000230 RID: 560
	public List<Vector3> points2 = new List<Vector3>();

	// Token: 0x04000231 RID: 561
	public List<Vector3> verticesBeginning = new List<Vector3>();

	// Token: 0x04000232 RID: 562
	public List<Vector3> verticesEnding = new List<Vector3>();

	// Token: 0x04000233 RID: 563
	public List<Vector3> normalsBeginning = new List<Vector3>();

	// Token: 0x04000234 RID: 564
	public List<Vector3> normalsEnding = new List<Vector3>();

	// Token: 0x04000235 RID: 565
	public List<float> widths = new List<float>();

	// Token: 0x04000236 RID: 566
	public List<float> snaps = new List<float>();

	// Token: 0x04000237 RID: 567
	public List<float> lerpValues = new List<float>();

	// Token: 0x04000238 RID: 568
	public List<Quaternion> orientations = new List<Quaternion>();

	// Token: 0x04000239 RID: 569
	public List<Vector3> tangents = new List<Vector3>();

	// Token: 0x0400023A RID: 570
	public List<Vector3> normalsList = new List<Vector3>();

	// Token: 0x0400023B RID: 571
	public Color[] colors;

	// Token: 0x0400023C RID: 572
	public List<Vector2> colorsFlowMap = new List<Vector2>();

	// Token: 0x0400023D RID: 573
	public float minVal = 0.5f;

	// Token: 0x0400023E RID: 574
	public float maxVal = 0.5f;

	// Token: 0x0400023F RID: 575
	public float width = 4f;

	// Token: 0x04000240 RID: 576
	public int vertsInShape = 3;

	// Token: 0x04000241 RID: 577
	public float traingleDensity = 0.2f;

	// Token: 0x04000242 RID: 578
	public float uvScale = 3f;

	// Token: 0x04000243 RID: 579
	public Material oldMaterial;

	// Token: 0x04000244 RID: 580
	public bool showVertexColors;

	// Token: 0x04000245 RID: 581
	public bool showFlowMap;

	// Token: 0x04000246 RID: 582
	public bool overrideFlowMap;

	// Token: 0x04000247 RID: 583
	public bool drawOnMesh;

	// Token: 0x04000248 RID: 584
	public bool drawOnMeshFlowMap;

	// Token: 0x04000249 RID: 585
	public bool uvScaleOverride;

	// Token: 0x0400024A RID: 586
	public bool debug;

	// Token: 0x0400024B RID: 587
	public Color drawColor = Color.black;

	// Token: 0x0400024C RID: 588
	public float flowSpeed = 1f;

	// Token: 0x0400024D RID: 589
	public float flowDirection;

	// Token: 0x0400024E RID: 590
	public AnimationCurve flowFlat = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.025f),
		new Keyframe(0.5f, 0.05f),
		new Keyframe(1f, 0.025f)
	});

	// Token: 0x0400024F RID: 591
	public AnimationCurve flowWaterfall = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.25f),
		new Keyframe(1f, 0.25f)
	});

	// Token: 0x04000250 RID: 592
	public float opacity = 0.1f;

	// Token: 0x04000251 RID: 593
	public float drawSize = 1f;

	// Token: 0x04000252 RID: 594
	public float length;

	// Token: 0x04000253 RID: 595
	public float fulllength;

	// Token: 0x04000254 RID: 596
	public float minMaxWidth;

	// Token: 0x04000255 RID: 597
	public float uvWidth;

	// Token: 0x04000256 RID: 598
	public float uvBeginning;
}
