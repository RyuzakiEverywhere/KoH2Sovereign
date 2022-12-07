using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class PathWalker
{
	// Token: 0x0600158F RID: 5519 RVA: 0x000DBBA1 File Offset: 0x000D9DA1
	public void Clear()
	{
		this.path_points = null;
		this.path_len = 0f;
		this.t = 0f;
		this.segment_idx = 0;
		this.segment_t = 0f;
		this.segment_len = 0f;
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x000DBBE0 File Offset: 0x000D9DE0
	public void SetPath(List<Vector3> points, float len = -1f)
	{
		this.Clear();
		if (points == null || points.Count < 2)
		{
			return;
		}
		this.path_points = points;
		this.segment_len = (points[1] - points[0]).magnitude;
		if (len < 0f)
		{
			len = this.segment_len;
			int num = 1;
			while (num + 1 < points.Count)
			{
				len += (points[num + 1] - points[num]).magnitude;
				num++;
			}
		}
		this.path_len = len;
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x000DBC73 File Offset: 0x000D9E73
	public void SetPath(TerrainPathFinder pf)
	{
		if (pf == null)
		{
			this.Clear();
			return;
		}
		this.SetPath(pf.path_points, pf.path_len);
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x000DBC91 File Offset: 0x000D9E91
	public bool IsValid()
	{
		return this.path_points != null;
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x000DBC9C File Offset: 0x000D9E9C
	public bool IsDone()
	{
		return this.t >= this.path_len;
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x000DBCB0 File Offset: 0x000D9EB0
	public void GetSegmentPoints(int seg_idx, float fOffset, out Vector3 ptStart, out Vector3 ptEnd)
	{
		ptStart = this.path_points[seg_idx];
		ptEnd = this.path_points[seg_idx + 1];
		if (fOffset == 0f)
		{
			return;
		}
		Vector3 rightVector = Common.GetRightVector(ptEnd - ptStart, fOffset);
		ptStart += rightVector;
		if (seg_idx + 2 >= this.path_points.Count)
		{
			ptEnd += rightVector;
			return;
		}
		Vector3 rightVector2 = Common.GetRightVector(this.path_points[seg_idx + 2] - ptEnd, fOffset);
		ptEnd += rightVector2;
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x000DBD74 File Offset: 0x000D9F74
	public bool GetPathPoint(float path_t, out Vector3 pt, out Vector3 ptDest, bool advance = false, float fOffset = 0f)
	{
		if (this.path_points == null)
		{
			pt = (ptDest = Vector3.zero);
			return false;
		}
		if (path_t >= this.path_len)
		{
			this.GetSegmentPoints(this.path_points.Count - 2, fOffset, out pt, out ptDest);
			pt = ptDest;
			if (advance)
			{
				this.t = this.path_len;
				this.segment_idx = this.path_points.Count - 1;
				this.segment_t = this.path_len;
				this.segment_len = 0f;
			}
			return false;
		}
		bool flag = path_t >= 0f;
		if (!flag)
		{
			path_t = 0f;
		}
		int num;
		float num2;
		float magnitude;
		if (path_t < this.t)
		{
			num = 0;
			num2 = 0f;
			magnitude = (this.path_points[1] - this.path_points[0]).magnitude;
		}
		else
		{
			num = this.segment_idx;
			num2 = this.segment_t;
			magnitude = this.segment_len;
		}
		pt = Vector3.zero;
		while (num + 1 < this.path_points.Count)
		{
			float num3 = num2 + magnitude;
			if (num3 >= path_t && magnitude != 0f)
			{
				this.GetSegmentPoints(num, fOffset, out pt, out ptDest);
				Vector3 a = ptDest - pt;
				pt += a * (path_t - num2) / magnitude;
				if (advance)
				{
					this.t = path_t;
					this.segment_idx = num;
					this.segment_t = num2;
					this.segment_len = magnitude;
				}
				return flag;
			}
			num++;
			num2 = num3;
			magnitude = (this.path_points[num + 1] - this.path_points[num]).magnitude;
		}
		this.GetSegmentPoints(this.path_points.Count - 2, fOffset, out pt, out ptDest);
		pt = ptDest;
		if (advance)
		{
			this.t = this.path_len;
			this.segment_idx = this.path_points.Count - 1;
			this.segment_t = this.path_len;
			this.segment_len = 0f;
		}
		return false;
	}

	// Token: 0x04000DEF RID: 3567
	public List<Vector3> path_points;

	// Token: 0x04000DF0 RID: 3568
	public float path_len;

	// Token: 0x04000DF1 RID: 3569
	public float t;

	// Token: 0x04000DF2 RID: 3570
	public int segment_idx;

	// Token: 0x04000DF3 RID: 3571
	public float segment_t;

	// Token: 0x04000DF4 RID: 3572
	public float segment_len;

	// Token: 0x04000DF5 RID: 3573
	public Vector3 ptCur = Vector3.zero;

	// Token: 0x04000DF6 RID: 3574
	public Vector3 ptLookAt = Vector3.zero;
}
