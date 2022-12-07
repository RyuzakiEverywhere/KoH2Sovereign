using System;
using Logic;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class DebugGizmo : MonoBehaviour
{
	// Token: 0x06000DCA RID: 3530 RVA: 0x0009A281 File Offset: 0x00098481
	public void DrawLine(Vector3 pt1, Vector3 pt2)
	{
		if (this.snap)
		{
			pt1 = global::Common.SnapToTerrain(pt1, this.heightOfs, null, -1f, false);
			pt2 = global::Common.SnapToTerrain(pt2, this.heightOfs, null, -1f, false);
		}
		Gizmos.DrawLine(pt1, pt2);
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x0009A2BC File Offset: 0x000984BC
	public void OnDrawGizmos()
	{
		if (this.hide)
		{
			return;
		}
		Gizmos.color = this.clr;
		Vector3 vector = base.transform.position;
		if (this.snap)
		{
			vector = new Logic.HexGrid(this.radius).Snap(vector);
			vector.y = base.transform.position.y;
		}
		vector.y += this.heightOfs;
		for (int i = 0; i < DebugGizmo.pts.Length; i++)
		{
			Vector3 vector2 = DebugGizmo.pts[i];
			Vector3 vector3 = DebugGizmo.pts[(i + 1) % DebugGizmo.pts.Length];
			if (this.rotate)
			{
				vector2 = base.transform.rotation * vector2;
				vector3 = base.transform.rotation * vector3;
			}
			this.DrawLine(vector + vector2 * this.radius, vector + vector3 * this.radius);
		}
		if (this.subdivide == 0f)
		{
			return;
		}
		Gizmos.color = this.subClr;
		if (this.subdivide > 0f)
		{
			float d = this.radius * this.subdivide;
			for (int j = 0; j < DebugGizmo.pts.Length; j++)
			{
				Vector3 vector4 = DebugGizmo.pts[j];
				Vector3 vector5 = DebugGizmo.pts[(j + 1) % DebugGizmo.pts.Length];
				Vector3 vector6 = DebugGizmo.pts[(j + 2) % DebugGizmo.pts.Length];
				if (this.rotate)
				{
					vector4 = base.transform.rotation * vector4;
					vector5 = base.transform.rotation * vector5;
					vector6 = base.transform.rotation * vector6;
				}
				Vector3 a = (vector4 + vector5) / 2f;
				Vector3 a2 = (vector5 + vector6) / 2f;
				this.DrawLine(vector + a * d, vector + a2 * d);
				this.DrawLine(vector + a * d, vector + a * this.radius);
			}
			return;
		}
		float d2 = -this.radius * this.subdivide;
		for (int k = 0; k < DebugGizmo.pts.Length; k++)
		{
			Vector3 vector7 = DebugGizmo.pts[k];
			Vector3 vector8 = DebugGizmo.pts[(k + 1) % DebugGizmo.pts.Length];
			if (this.rotate)
			{
				vector7 = base.transform.rotation * vector7;
				vector8 = base.transform.rotation * vector8;
			}
			this.DrawLine(vector + vector7 * d2, vector + vector8 * d2);
			this.DrawLine(vector + vector7 * d2, vector + vector7 * this.radius);
		}
	}

	// Token: 0x04000A87 RID: 2695
	public float radius = 2.5f;

	// Token: 0x04000A88 RID: 2696
	public bool snap;

	// Token: 0x04000A89 RID: 2697
	public Color clr = Color.yellow;

	// Token: 0x04000A8A RID: 2698
	public float heightOfs = 0.1f;

	// Token: 0x04000A8B RID: 2699
	public bool rotate = true;

	// Token: 0x04000A8C RID: 2700
	public bool hide;

	// Token: 0x04000A8D RID: 2701
	[Range(-1f, 1f)]
	public float subdivide;

	// Token: 0x04000A8E RID: 2702
	public Color subClr = Color.cyan;

	// Token: 0x04000A8F RID: 2703
	private const float ho = 0.5f;

	// Token: 0x04000A90 RID: 2704
	private const float vo = 0.866f;

	// Token: 0x04000A91 RID: 2705
	private static readonly Vector3[] pts = new Vector3[]
	{
		new Vector3(-1f, 0f, 0f),
		new Vector3(-0.5f, 0f, -0.866f),
		new Vector3(0.5f, 0f, -0.866f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0.5f, 0f, 0.866f),
		new Vector3(-0.5f, 0f, 0.866f)
	};
}
