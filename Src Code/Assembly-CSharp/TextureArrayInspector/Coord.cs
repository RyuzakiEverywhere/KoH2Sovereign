using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000486 RID: 1158
	[Serializable]
	public struct Coord : CustomSerialization.IStruct
	{
		// Token: 0x06003D03 RID: 15619 RVA: 0x001D0E01 File Offset: 0x001CF001
		public static bool operator >(Coord c1, Coord c2)
		{
			return c1.x > c2.x && c1.z > c2.z;
		}

		// Token: 0x06003D04 RID: 15620 RVA: 0x001D0E21 File Offset: 0x001CF021
		public static bool operator <(Coord c1, Coord c2)
		{
			return c1.x < c2.x && c1.z < c2.z;
		}

		// Token: 0x06003D05 RID: 15621 RVA: 0x001D0E41 File Offset: 0x001CF041
		public static bool operator ==(Coord c1, Coord c2)
		{
			return c1.x == c2.x && c1.z == c2.z;
		}

		// Token: 0x06003D06 RID: 15622 RVA: 0x001D0E61 File Offset: 0x001CF061
		public static bool operator !=(Coord c1, Coord c2)
		{
			return c1.x != c2.x || c1.z != c2.z;
		}

		// Token: 0x06003D07 RID: 15623 RVA: 0x001D0E84 File Offset: 0x001CF084
		public static Coord operator +(Coord c, int s)
		{
			return new Coord(c.x + s, c.z + s);
		}

		// Token: 0x06003D08 RID: 15624 RVA: 0x001D0E9B File Offset: 0x001CF09B
		public static Coord operator +(Coord c1, Coord c2)
		{
			return new Coord(c1.x + c2.x, c1.z + c2.z);
		}

		// Token: 0x06003D09 RID: 15625 RVA: 0x001D0EBC File Offset: 0x001CF0BC
		public static Coord operator -(Coord c, int s)
		{
			return new Coord(c.x - s, c.z - s);
		}

		// Token: 0x06003D0A RID: 15626 RVA: 0x001D0ED3 File Offset: 0x001CF0D3
		public static Coord operator -(Coord c1, Coord c2)
		{
			return new Coord(c1.x - c2.x, c1.z - c2.z);
		}

		// Token: 0x06003D0B RID: 15627 RVA: 0x001D0EF4 File Offset: 0x001CF0F4
		public static Coord operator *(Coord c, int s)
		{
			return new Coord(c.x * s, c.z * s);
		}

		// Token: 0x06003D0C RID: 15628 RVA: 0x001D0F0B File Offset: 0x001CF10B
		public static Vector2 operator *(Coord c, Vector2 s)
		{
			return new Vector2((float)c.x * s.x, (float)c.z * s.y);
		}

		// Token: 0x06003D0D RID: 15629 RVA: 0x001D0F2E File Offset: 0x001CF12E
		public static Vector3 operator *(Coord c, Vector3 s)
		{
			return new Vector3((float)c.x * s.x, s.y, (float)c.z * s.z);
		}

		// Token: 0x06003D0E RID: 15630 RVA: 0x001D0F57 File Offset: 0x001CF157
		public static Coord operator *(Coord c, float s)
		{
			return new Coord((int)((float)c.x * s), (int)((float)c.z * s));
		}

		// Token: 0x06003D0F RID: 15631 RVA: 0x001D0F72 File Offset: 0x001CF172
		public static Coord operator /(Coord c, int s)
		{
			return new Coord(c.x / s, c.z / s);
		}

		// Token: 0x06003D10 RID: 15632 RVA: 0x001D0F89 File Offset: 0x001CF189
		public static Coord operator /(Coord c, float s)
		{
			return new Coord((int)((float)c.x / s), (int)((float)c.z / s));
		}

		// Token: 0x06003D11 RID: 15633 RVA: 0x001D0FA4 File Offset: 0x001CF1A4
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		// Token: 0x06003D12 RID: 15634 RVA: 0x001D0FB7 File Offset: 0x001CF1B7
		public override int GetHashCode()
		{
			return this.x * 10000000 + this.z;
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06003D13 RID: 15635 RVA: 0x001D0FCC File Offset: 0x001CF1CC
		public int Minimal
		{
			get
			{
				return Mathf.Min(this.x, this.z);
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06003D14 RID: 15636 RVA: 0x001D0FDF File Offset: 0x001CF1DF
		public int SqrMagnitude
		{
			get
			{
				return this.x * this.x + this.z * this.z;
			}
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06003D15 RID: 15637 RVA: 0x001D0FFC File Offset: 0x001CF1FC
		public Vector3 vector3
		{
			get
			{
				return new Vector3((float)this.x, 0f, (float)this.z);
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06003D16 RID: 15638 RVA: 0x001D1016 File Offset: 0x001CF216
		public Vector2 vector2
		{
			get
			{
				return new Vector2((float)this.x, (float)this.z);
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06003D17 RID: 15639 RVA: 0x001D102B File Offset: 0x001CF22B
		public static Coord zero
		{
			get
			{
				return new Coord(0, 0);
			}
		}

		// Token: 0x06003D18 RID: 15640 RVA: 0x001D1034 File Offset: 0x001CF234
		public Coord(int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		// Token: 0x06003D19 RID: 15641 RVA: 0x001D1044 File Offset: 0x001CF244
		public static Coord PickCell(int ix, int iz, int cellRes)
		{
			int num = ix / cellRes;
			if (ix < 0 && ix != num * cellRes)
			{
				num--;
			}
			int num2 = iz / cellRes;
			if (iz < 0 && iz != num2 * cellRes)
			{
				num2--;
			}
			return new Coord(num, num2);
		}

		// Token: 0x06003D1A RID: 15642 RVA: 0x001D107C File Offset: 0x001CF27C
		public static Coord PickCell(Coord c, int cellRes)
		{
			return Coord.PickCell(c.x, c.z, cellRes);
		}

		// Token: 0x06003D1B RID: 15643 RVA: 0x001D1090 File Offset: 0x001CF290
		public static Coord PickCellByPos(float fx, float fz, float cellSize = 1f)
		{
			int num = (int)(fx / cellSize);
			if (fx < 0f && fx != (float)num * cellSize)
			{
				num--;
			}
			int num2 = (int)(fz / cellSize);
			if (fz < 0f && fz != (float)num2 * cellSize)
			{
				num2--;
			}
			return new Coord(num, num2);
		}

		// Token: 0x06003D1C RID: 15644 RVA: 0x001D10D4 File Offset: 0x001CF2D4
		public static Coord PickCellByPos(Vector3 v, float cellSize = 1f)
		{
			return Coord.PickCellByPos(v.x, v.z, cellSize);
		}

		// Token: 0x06003D1D RID: 15645 RVA: 0x001D10E8 File Offset: 0x001CF2E8
		public static Coord FloorToCoord(float fx, float fz)
		{
			int num = (int)fx;
			if (fx < 0f && fx != (float)num)
			{
				num--;
			}
			int num2 = (int)fz;
			if (fz < 0f && fz != (float)num2)
			{
				num2--;
			}
			return new Coord(num, num2);
		}

		// Token: 0x06003D1E RID: 15646 RVA: 0x001D1124 File Offset: 0x001CF324
		public void ClampPositive()
		{
			this.x = Mathf.Max(0, this.x);
			this.z = Mathf.Max(0, this.z);
		}

		// Token: 0x06003D1F RID: 15647 RVA: 0x001D114C File Offset: 0x001CF34C
		public void ClampByRect(CoordRect rect)
		{
			if (this.x < rect.offset.x)
			{
				this.x = rect.offset.x;
			}
			if (this.x >= rect.offset.x + rect.size.x)
			{
				this.x = rect.offset.x + rect.size.x - 1;
			}
			if (this.z < rect.offset.z)
			{
				this.z = rect.offset.z;
			}
			if (this.z >= rect.offset.z + rect.size.z)
			{
				this.z = rect.offset.z + rect.size.z - 1;
			}
		}

		// Token: 0x06003D20 RID: 15648 RVA: 0x001D1220 File Offset: 0x001CF420
		public static Coord Min(Coord c1, Coord c2)
		{
			int num = (c1.x < c2.x) ? c1.x : c2.x;
			int num2 = (c1.z < c2.z) ? c1.z : c2.z;
			return new Coord(num, num2);
		}

		// Token: 0x06003D21 RID: 15649 RVA: 0x001D126C File Offset: 0x001CF46C
		public static Coord Max(Coord c1, Coord c2)
		{
			int num = (c1.x > c2.x) ? c1.x : c2.x;
			int num2 = (c1.z > c2.z) ? c1.z : c2.z;
			return new Coord(num, num2);
		}

		// Token: 0x06003D22 RID: 15650 RVA: 0x001D12B8 File Offset: 0x001CF4B8
		public Coord BaseFloor(int cellSize)
		{
			return new Coord((this.x >= 0) ? (this.x / cellSize) : ((this.x + 1) / cellSize - 1), (this.z >= 0) ? (this.z / cellSize) : ((this.z + 1) / cellSize - 1));
		}

		// Token: 0x06003D23 RID: 15651 RVA: 0x001D1308 File Offset: 0x001CF508
		public Coord BaseCeil(int cellSize)
		{
			return new Coord((this.x > 0) ? ((this.x - 1) / cellSize + 1) : (this.x / cellSize), (this.z > 0) ? ((this.z - 1) / cellSize + 1) : (this.z / cellSize));
		}

		// Token: 0x06003D24 RID: 15652 RVA: 0x001D1358 File Offset: 0x001CF558
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				" x:",
				this.x,
				" z:",
				this.z
			});
		}

		// Token: 0x06003D25 RID: 15653 RVA: 0x001D13B0 File Offset: 0x001CF5B0
		public static int DistanceAxisAligned(Coord c1, Coord c2)
		{
			int num = c1.x - c2.x;
			if (num < 0)
			{
				num = -num;
			}
			int num2 = c1.z - c2.z;
			if (num2 < 0)
			{
				num2 = -num2;
			}
			if (num <= num2)
			{
				return num2;
			}
			return num;
		}

		// Token: 0x06003D26 RID: 15654 RVA: 0x001D13F0 File Offset: 0x001CF5F0
		public static int DistanceAxisAligned(Coord c, CoordRect rect)
		{
			int num = rect.offset.x - c.x;
			int num2 = c.x - rect.offset.x - rect.size.x;
			int num3;
			if (num >= 0)
			{
				num3 = num;
			}
			else if (num2 >= 0)
			{
				num3 = num2;
			}
			else
			{
				num3 = 0;
			}
			int num4 = rect.offset.z - c.z;
			int num5 = c.z - rect.offset.z - rect.size.z;
			int num6;
			if (num4 >= 0)
			{
				num6 = num4;
			}
			else if (num5 >= 0)
			{
				num6 = num5;
			}
			else
			{
				num6 = 0;
			}
			if (num3 > num6)
			{
				return num3;
			}
			return num6;
		}

		// Token: 0x06003D27 RID: 15655 RVA: 0x001D1494 File Offset: 0x001CF694
		public static float Distance(Coord c1, Coord c2)
		{
			int num = c1.x - c2.x;
			int num2 = c1.z - c2.z;
			return Mathf.Sqrt((float)(num * num + num2 * num2));
		}

		// Token: 0x06003D28 RID: 15656 RVA: 0x001D14C8 File Offset: 0x001CF6C8
		public static float DistanceSq(Coord c1, Coord c2)
		{
			int num = c1.x - c2.x;
			if (num < 0)
			{
				num = -num;
			}
			int num2 = c1.z - c2.z;
			if (num2 < 0)
			{
				num2 = -num2;
			}
			return (float)(num * num + num2 * num2);
		}

		// Token: 0x06003D29 RID: 15657 RVA: 0x001D1507 File Offset: 0x001CF707
		public IEnumerable<Coord> DistanceStep(int i, int dist)
		{
			yield return new Coord(this.x - i, this.z - dist);
			yield return new Coord(this.x - dist, this.z + i);
			yield return new Coord(this.x + i, this.z + dist);
			yield return new Coord(this.x + dist, this.z - i);
			yield return new Coord(this.x + i + 1, this.z - dist);
			yield return new Coord(this.x - dist, this.z - i - 1);
			yield return new Coord(this.x - i - 1, this.z + dist);
			yield return new Coord(this.x + dist, this.z + i + 1);
			yield break;
		}

		// Token: 0x06003D2A RID: 15658 RVA: 0x001D152A File Offset: 0x001CF72A
		public IEnumerable<Coord> DistancePerimeter(int dist)
		{
			int num;
			for (int i = 0; i < dist; i = num + 1)
			{
				foreach (Coord coord in this.DistanceStep(i, dist))
				{
					yield return coord;
				}
				IEnumerator<Coord> enumerator = null;
				num = i;
			}
			yield break;
			yield break;
		}

		// Token: 0x06003D2B RID: 15659 RVA: 0x001D1546 File Offset: 0x001CF746
		public IEnumerable<Coord> DistanceArea(int maxDist)
		{
			yield return ref this;
			int num;
			for (int i = 0; i < maxDist; i = num + 1)
			{
				foreach (Coord coord in this.DistancePerimeter(i))
				{
					yield return coord;
				}
				IEnumerator<Coord> enumerator = null;
				num = i;
			}
			yield break;
			yield break;
		}

		// Token: 0x06003D2C RID: 15660 RVA: 0x001D1562 File Offset: 0x001CF762
		public IEnumerable<Coord> DistanceArea(CoordRect rect)
		{
			int maxDist = Mathf.Max(new int[]
			{
				this.x - rect.offset.x,
				rect.Max.x - this.x,
				this.z - rect.offset.z,
				rect.Max.z - this.z
			}) + 1;
			if (rect.CheckInRange(ref this))
			{
				yield return ref this;
			}
			int num;
			for (int i = 0; i < maxDist; i = num + 1)
			{
				foreach (Coord coord in this.DistancePerimeter(i))
				{
					if (rect.CheckInRange(coord))
					{
						yield return coord;
					}
				}
				IEnumerator<Coord> enumerator = null;
				num = i;
			}
			yield break;
			yield break;
		}

		// Token: 0x06003D2D RID: 15661 RVA: 0x001D157E File Offset: 0x001CF77E
		public static IEnumerable<Coord> MultiDistanceArea(Coord[] coords, int maxDist)
		{
			if (coords.Length == 0)
			{
				yield break;
			}
			int num;
			for (int c = 0; c < coords.Length; c = num + 1)
			{
				yield return coords[c];
				num = c;
			}
			for (int c = 0; c < maxDist; c = num + 1)
			{
				for (int i = 0; i < c; i = num + 1)
				{
					for (int c2 = 0; c2 < coords.Length; c2 = num + 1)
					{
						foreach (Coord coord in coords[c2].DistanceStep(i, c))
						{
							yield return coord;
						}
						IEnumerator<Coord> enumerator = null;
						num = c2;
					}
					num = i;
				}
				num = c;
			}
			yield break;
			yield break;
		}

		// Token: 0x06003D2E RID: 15662 RVA: 0x001D1595 File Offset: 0x001CF795
		public Vector3 ToVector3(float cellSize)
		{
			return new Vector3((float)this.x * cellSize, 0f, (float)this.z * cellSize);
		}

		// Token: 0x06003D2F RID: 15663 RVA: 0x001D15B3 File Offset: 0x001CF7B3
		public Vector2 ToVector2(float cellSize)
		{
			return new Vector2((float)this.x * cellSize, (float)this.z * cellSize);
		}

		// Token: 0x06003D30 RID: 15664 RVA: 0x001D15CC File Offset: 0x001CF7CC
		public Rect ToRect(float cellSize)
		{
			return new Rect((float)this.x * cellSize, (float)this.z * cellSize, cellSize, cellSize);
		}

		// Token: 0x06003D31 RID: 15665 RVA: 0x001D15E7 File Offset: 0x001CF7E7
		public CoordRect ToCoordRect(int cellSize)
		{
			return new CoordRect(this.x * cellSize, this.z * cellSize, cellSize, cellSize);
		}

		// Token: 0x06003D32 RID: 15666 RVA: 0x001D1600 File Offset: 0x001CF800
		public string Encode()
		{
			return string.Concat(new object[]
			{
				"x=",
				this.x,
				" z=",
				this.z
			});
		}

		// Token: 0x06003D33 RID: 15667 RVA: 0x001D1639 File Offset: 0x001CF839
		public void Decode(string[] lineMembers)
		{
			this.x = (int)lineMembers[2].Parse(typeof(int));
			this.z = (int)lineMembers[3].Parse(typeof(int));
		}

		// Token: 0x04002BB8 RID: 11192
		public int x;

		// Token: 0x04002BB9 RID: 11193
		public int z;
	}
}
