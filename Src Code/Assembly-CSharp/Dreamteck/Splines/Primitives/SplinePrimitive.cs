using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004DB RID: 1243
	public class SplinePrimitive
	{
		// Token: 0x060041C9 RID: 16841 RVA: 0x001F4AF5 File Offset: 0x001F2CF5
		public virtual void Calculate()
		{
			this.Generate();
			this.ApplyOffset();
		}

		// Token: 0x060041CA RID: 16842 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void Generate()
		{
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x001F4B04 File Offset: 0x001F2D04
		public Spline CreateSpline()
		{
			this.Generate();
			this.ApplyOffset();
			Spline spline = new Spline(this.GetSplineType());
			spline.points = this.points;
			if (this.closed)
			{
				spline.Close();
			}
			return spline;
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x001F4B44 File Offset: 0x001F2D44
		public void UpdateSpline(Spline spline)
		{
			this.Generate();
			this.ApplyOffset();
			spline.type = this.GetSplineType();
			spline.points = this.points;
			if (this.closed)
			{
				spline.Close();
				return;
			}
			if (spline.isClosed)
			{
				spline.Break();
			}
		}

		// Token: 0x060041CD RID: 16845 RVA: 0x001F4B94 File Offset: 0x001F2D94
		public SplineComputer CreateSplineComputer(string name, Vector3 position, Quaternion rotation)
		{
			this.Generate();
			this.ApplyOffset();
			SplineComputer splineComputer = new GameObject(name).AddComponent<SplineComputer>();
			splineComputer.SetPoints(this.points, SplineComputer.Space.Local);
			if (this.closed)
			{
				splineComputer.Close();
			}
			splineComputer.transform.position = position;
			splineComputer.transform.rotation = rotation;
			return splineComputer;
		}

		// Token: 0x060041CE RID: 16846 RVA: 0x001F4BF0 File Offset: 0x001F2DF0
		public void UpdateSplineComputer(SplineComputer comp)
		{
			this.Generate();
			this.ApplyOffset();
			comp.type = this.GetSplineType();
			comp.SetPoints(this.points, SplineComputer.Space.Local);
			if (this.closed)
			{
				comp.Close();
				return;
			}
			if (comp.isClosed)
			{
				comp.Break();
			}
		}

		// Token: 0x060041CF RID: 16847 RVA: 0x001F4C3F File Offset: 0x001F2E3F
		public SplinePoint[] GetPoints()
		{
			return this.points;
		}

		// Token: 0x060041D0 RID: 16848 RVA: 0x0002C538 File Offset: 0x0002A738
		public virtual Spline.Type GetSplineType()
		{
			return Spline.Type.CatmullRom;
		}

		// Token: 0x060041D1 RID: 16849 RVA: 0x001F4C47 File Offset: 0x001F2E47
		public bool GetIsClosed()
		{
			return this.closed;
		}

		// Token: 0x060041D2 RID: 16850 RVA: 0x001F4C50 File Offset: 0x001F2E50
		private void ApplyOffset()
		{
			Quaternion quaternion = Quaternion.Euler(this.rotation);
			if (this.is2D)
			{
				quaternion = Quaternion.AngleAxis(-this.rotation.z, Vector3.forward) * Quaternion.AngleAxis(90f, Vector3.right);
			}
			for (int i = 0; i < this.points.Length; i++)
			{
				this.points[i].position = quaternion * this.points[i].position;
				this.points[i].tangent = quaternion * this.points[i].tangent;
				this.points[i].tangent2 = quaternion * this.points[i].tangent2;
				this.points[i].normal = quaternion * this.points[i].normal;
			}
			for (int j = 0; j < this.points.Length; j++)
			{
				this.points[j].SetPosition(this.points[j].position + this.offset);
			}
		}

		// Token: 0x060041D3 RID: 16851 RVA: 0x001F4D98 File Offset: 0x001F2F98
		protected void CreatePoints(int count, SplinePoint.Type type)
		{
			if (this.points.Length != count)
			{
				this.points = new SplinePoint[count];
			}
			for (int i = 0; i < this.points.Length; i++)
			{
				this.points[i].type = type;
				this.points[i].normal = Vector3.up;
				this.points[i].color = Color.white;
				this.points[i].size = 1f;
			}
		}

		// Token: 0x04002DD6 RID: 11734
		protected bool closed;

		// Token: 0x04002DD7 RID: 11735
		protected SplinePoint[] points = new SplinePoint[0];

		// Token: 0x04002DD8 RID: 11736
		public Vector3 offset = Vector3.zero;

		// Token: 0x04002DD9 RID: 11737
		public Vector3 rotation = Vector3.zero;

		// Token: 0x04002DDA RID: 11738
		public bool is2D;
	}
}
