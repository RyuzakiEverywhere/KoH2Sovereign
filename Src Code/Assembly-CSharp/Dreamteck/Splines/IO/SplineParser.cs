using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines.IO
{
	// Token: 0x020004DF RID: 1247
	public class SplineParser
	{
		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06004201 RID: 16897 RVA: 0x001F7683 File Offset: 0x001F5883
		public string name
		{
			get
			{
				return this.fileName;
			}
		}

		// Token: 0x06004202 RID: 16898 RVA: 0x001F768C File Offset: 0x001F588C
		internal Vector2[] ParseVector2(string coord)
		{
			List<float> list = this.ParseFloatArray(coord.Substring(1));
			int num = list.Count / 2;
			if (num == 0)
			{
				Debug.Log("Error in " + coord);
				return new Vector2[]
				{
					Vector2.zero
				};
			}
			Vector2[] array = new Vector2[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new Vector2(list[i * 2], -list[1 + i * 2]);
			}
			return array;
		}

		// Token: 0x06004203 RID: 16899 RVA: 0x001F770C File Offset: 0x001F590C
		internal float[] ParseFloat(string coord)
		{
			List<float> list = this.ParseFloatArray(coord.Substring(1));
			if (list.Count < 1)
			{
				Debug.Log("Error in " + coord);
				return new float[1];
			}
			return list.ToArray();
		}

		// Token: 0x06004204 RID: 16900 RVA: 0x001F7750 File Offset: 0x001F5950
		internal List<float> ParseFloatArray(string content)
		{
			string text = "";
			List<float> list = new List<float>();
			foreach (char c in content)
			{
				if ((c == ',' || c == '-' || char.IsWhiteSpace(c)) && !this.IsWHiteSpace(text))
				{
					float item = 0f;
					float.TryParse(text, out item);
					list.Add(item);
					text = "";
					if (c == '-')
					{
						text = "-";
					}
				}
				else if (!char.IsWhiteSpace(c))
				{
					text += c.ToString();
				}
			}
			if (!this.IsWHiteSpace(text))
			{
				float item2 = 0f;
				float.TryParse(text, out item2);
				list.Add(item2);
			}
			return list;
		}

		// Token: 0x06004205 RID: 16901 RVA: 0x001F7808 File Offset: 0x001F5A08
		public bool IsWHiteSpace(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (!char.IsWhiteSpace(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04002DE5 RID: 11749
		protected string fileName = "";

		// Token: 0x04002DE6 RID: 11750
		internal SplineParser.SplineDefinition buffer;

		// Token: 0x020009BB RID: 2491
		internal class Transformation
		{
			// Token: 0x060054C0 RID: 21696 RVA: 0x0024747C File Offset: 0x0024567C
			internal static void ResetMatrix()
			{
				SplineParser.Transformation.matrix.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
			}

			// Token: 0x060054C1 RID: 21697 RVA: 0x000023FD File Offset: 0x000005FD
			internal virtual void Push()
			{
			}

			// Token: 0x060054C2 RID: 21698 RVA: 0x00247498 File Offset: 0x00245698
			internal static void Apply(SplinePoint[] points)
			{
				for (int i = 0; i < points.Length; i++)
				{
					SplinePoint splinePoint = points[i];
					splinePoint.position = SplineParser.Transformation.matrix.MultiplyPoint(splinePoint.position);
					splinePoint.tangent = SplineParser.Transformation.matrix.MultiplyPoint(splinePoint.tangent);
					splinePoint.tangent2 = SplineParser.Transformation.matrix.MultiplyPoint(splinePoint.tangent2);
					points[i] = splinePoint;
				}
			}

			// Token: 0x04004549 RID: 17737
			protected static Matrix4x4 matrix;
		}

		// Token: 0x020009BC RID: 2492
		internal class Translate : SplineParser.Transformation
		{
			// Token: 0x060054C5 RID: 21701 RVA: 0x00247508 File Offset: 0x00245708
			public Translate(Vector2 o)
			{
				this.offset = o;
			}

			// Token: 0x060054C6 RID: 21702 RVA: 0x00247524 File Offset: 0x00245724
			internal override void Push()
			{
				Matrix4x4 rhs = default(Matrix4x4);
				rhs.SetTRS(new Vector2(this.offset.x, -this.offset.y), Quaternion.identity, Vector3.one);
				SplineParser.Transformation.matrix *= rhs;
			}

			// Token: 0x0400454A RID: 17738
			private Vector2 offset = Vector2.zero;
		}

		// Token: 0x020009BD RID: 2493
		internal class Rotate : SplineParser.Transformation
		{
			// Token: 0x060054C7 RID: 21703 RVA: 0x0024757B File Offset: 0x0024577B
			public Rotate(float a)
			{
				this.angle = a;
			}

			// Token: 0x060054C8 RID: 21704 RVA: 0x0024758C File Offset: 0x0024578C
			internal override void Push()
			{
				Matrix4x4 rhs = default(Matrix4x4);
				rhs.SetTRS(Vector3.zero, Quaternion.AngleAxis(this.angle, Vector3.back), Vector3.one);
				SplineParser.Transformation.matrix *= rhs;
			}

			// Token: 0x0400454B RID: 17739
			private float angle;
		}

		// Token: 0x020009BE RID: 2494
		internal class Scale : SplineParser.Transformation
		{
			// Token: 0x060054C9 RID: 21705 RVA: 0x002475D2 File Offset: 0x002457D2
			public Scale(Vector2 s)
			{
				this.multiplier = s;
			}

			// Token: 0x060054CA RID: 21706 RVA: 0x002475EC File Offset: 0x002457EC
			internal override void Push()
			{
				Matrix4x4 rhs = default(Matrix4x4);
				rhs.SetTRS(Vector3.zero, Quaternion.identity, this.multiplier);
				SplineParser.Transformation.matrix *= rhs;
			}

			// Token: 0x0400454C RID: 17740
			private Vector2 multiplier = Vector2.one;
		}

		// Token: 0x020009BF RID: 2495
		internal class SkewX : SplineParser.Transformation
		{
			// Token: 0x060054CB RID: 21707 RVA: 0x0024762D File Offset: 0x0024582D
			public SkewX(float a)
			{
				this.amount = a;
			}

			// Token: 0x060054CC RID: 21708 RVA: 0x0024763C File Offset: 0x0024583C
			internal override void Push()
			{
				Matrix4x4 rhs = default(Matrix4x4);
				rhs[0, 0] = 1f;
				rhs[1, 1] = 1f;
				rhs[2, 2] = 1f;
				rhs[3, 3] = 1f;
				rhs[0, 1] = Mathf.Tan(-this.amount * 0.017453292f);
				SplineParser.Transformation.matrix *= rhs;
			}

			// Token: 0x0400454D RID: 17741
			private float amount;
		}

		// Token: 0x020009C0 RID: 2496
		internal class SkewY : SplineParser.Transformation
		{
			// Token: 0x060054CD RID: 21709 RVA: 0x002476B4 File Offset: 0x002458B4
			public SkewY(float a)
			{
				this.amount = a;
			}

			// Token: 0x060054CE RID: 21710 RVA: 0x002476C4 File Offset: 0x002458C4
			internal override void Push()
			{
				Matrix4x4 rhs = default(Matrix4x4);
				rhs[0, 0] = 1f;
				rhs[1, 1] = 1f;
				rhs[2, 2] = 1f;
				rhs[3, 3] = 1f;
				rhs[1, 0] = Mathf.Tan(-this.amount * 0.017453292f);
				SplineParser.Transformation.matrix *= rhs;
			}

			// Token: 0x0400454E RID: 17742
			private float amount;
		}

		// Token: 0x020009C1 RID: 2497
		internal class MatrixTransform : SplineParser.Transformation
		{
			// Token: 0x060054CF RID: 21711 RVA: 0x0024773C File Offset: 0x0024593C
			public MatrixTransform(float a, float b, float c, float d, float e, float f)
			{
				this.transformMatrix.SetRow(0, new Vector4(a, c, 0f, e));
				this.transformMatrix.SetRow(1, new Vector4(b, d, 0f, -f));
				this.transformMatrix.SetRow(2, new Vector4(0f, 0f, 1f, 0f));
				this.transformMatrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			}

			// Token: 0x060054D0 RID: 21712 RVA: 0x002477CF File Offset: 0x002459CF
			internal override void Push()
			{
				SplineParser.Transformation.matrix *= this.transformMatrix;
			}

			// Token: 0x0400454F RID: 17743
			private Matrix4x4 transformMatrix;
		}

		// Token: 0x020009C2 RID: 2498
		internal class SplineDefinition
		{
			// Token: 0x17000724 RID: 1828
			// (get) Token: 0x060054D1 RID: 21713 RVA: 0x002477E6 File Offset: 0x002459E6
			internal int pointCount
			{
				get
				{
					return this.points.Count;
				}
			}

			// Token: 0x060054D2 RID: 21714 RVA: 0x002477F4 File Offset: 0x002459F4
			internal SplineDefinition(string n, Spline.Type t)
			{
				this.name = n;
				this.type = t;
			}

			// Token: 0x060054D3 RID: 21715 RVA: 0x00247874 File Offset: 0x00245A74
			internal SplineDefinition(string n, Spline spline)
			{
				this.name = n;
				this.type = spline.type;
				this.closed = spline.isClosed;
				this.points = new List<SplinePoint>(spline.points);
			}

			// Token: 0x060054D4 RID: 21716 RVA: 0x00247918 File Offset: 0x00245B18
			internal SplinePoint GetLastPoint()
			{
				if (this.points.Count == 0)
				{
					return default(SplinePoint);
				}
				return this.points[this.points.Count - 1];
			}

			// Token: 0x060054D5 RID: 21717 RVA: 0x00247954 File Offset: 0x00245B54
			internal void SetLastPoint(SplinePoint point)
			{
				if (this.points.Count == 0)
				{
					return;
				}
				this.points[this.points.Count - 1] = point;
			}

			// Token: 0x060054D6 RID: 21718 RVA: 0x00247980 File Offset: 0x00245B80
			internal void CreateClosingPoint()
			{
				SplinePoint item = new SplinePoint(this.points[0]);
				this.points.Add(item);
			}

			// Token: 0x060054D7 RID: 21719 RVA: 0x002479AC File Offset: 0x00245BAC
			internal void CreateSmooth()
			{
				this.points.Add(new SplinePoint(this.position, this.tangent, this.normal, this.size, this.color));
			}

			// Token: 0x060054D8 RID: 21720 RVA: 0x002479DC File Offset: 0x00245BDC
			internal void CreateBroken()
			{
				SplinePoint splinePoint = new SplinePoint(new SplinePoint(this.position, this.tangent, this.normal, this.size, this.color));
				splinePoint.type = SplinePoint.Type.Broken;
				splinePoint.SetTangent2Position(splinePoint.position);
				splinePoint.normal = this.normal;
				splinePoint.color = this.color;
				splinePoint.size = this.size;
				this.points.Add(splinePoint);
			}

			// Token: 0x060054D9 RID: 21721 RVA: 0x00247A5B File Offset: 0x00245C5B
			internal void CreateLinear()
			{
				this.tangent = this.position;
				this.CreateSmooth();
			}

			// Token: 0x060054DA RID: 21722 RVA: 0x00247A70 File Offset: 0x00245C70
			internal SplineComputer CreateSplineComputer(Vector3 position, Quaternion rotation)
			{
				SplineComputer splineComputer = new GameObject(this.name)
				{
					transform = 
					{
						position = position,
						rotation = rotation
					}
				}.AddComponent<SplineComputer>();
				splineComputer.type = this.type;
				if (this.closed && this.points[0].type == SplinePoint.Type.Broken)
				{
					this.points[0].SetTangentPosition(this.GetLastPoint().tangent2);
				}
				splineComputer.SetPoints(this.points.ToArray(), SplineComputer.Space.Local);
				if (this.closed)
				{
					splineComputer.Close();
				}
				return splineComputer;
			}

			// Token: 0x060054DB RID: 21723 RVA: 0x00247B14 File Offset: 0x00245D14
			internal Spline CreateSpline()
			{
				Spline spline = new Spline(this.type);
				spline.points = this.points.ToArray();
				if (this.closed)
				{
					spline.Close();
				}
				return spline;
			}

			// Token: 0x060054DC RID: 21724 RVA: 0x00247B50 File Offset: 0x00245D50
			internal void Transform(List<SplineParser.Transformation> trs)
			{
				SplinePoint[] array = this.points.ToArray();
				SplineParser.Transformation.ResetMatrix();
				foreach (SplineParser.Transformation transformation in trs)
				{
					transformation.Push();
				}
				SplineParser.Transformation.Apply(array);
				for (int i = 0; i < array.Length; i++)
				{
					this.points[i] = array[i];
				}
				SplineParser.Transformation.Apply(new SplinePoint[]
				{
					default(SplinePoint)
				});
			}

			// Token: 0x04004550 RID: 17744
			internal string name = "";

			// Token: 0x04004551 RID: 17745
			internal Spline.Type type = Spline.Type.Linear;

			// Token: 0x04004552 RID: 17746
			internal List<SplinePoint> points = new List<SplinePoint>();

			// Token: 0x04004553 RID: 17747
			internal bool closed;

			// Token: 0x04004554 RID: 17748
			internal Vector3 position = Vector3.zero;

			// Token: 0x04004555 RID: 17749
			internal Vector3 tangent = Vector3.zero;

			// Token: 0x04004556 RID: 17750
			internal Vector3 tangent2 = Vector3.zero;

			// Token: 0x04004557 RID: 17751
			internal Vector3 normal = Vector3.back;

			// Token: 0x04004558 RID: 17752
			internal float size = 1f;

			// Token: 0x04004559 RID: 17753
			internal Color color = Color.white;
		}
	}
}
