using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000487 RID: 1159
	[Serializable]
	public struct CoordRect : CustomSerialization.IStruct
	{
		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06003D34 RID: 15668 RVA: 0x001D1675 File Offset: 0x001CF875
		public bool isZero
		{
			get
			{
				return this.size.x == 0 || this.size.z == 0;
			}
		}

		// Token: 0x06003D35 RID: 15669 RVA: 0x001D1694 File Offset: 0x001CF894
		public CoordRect(Coord offset, Coord size)
		{
			this.offset = offset;
			this.size = size;
		}

		// Token: 0x06003D36 RID: 15670 RVA: 0x001D16A4 File Offset: 0x001CF8A4
		public CoordRect(int offsetX, int offsetZ, int sizeX, int sizeZ)
		{
			this.offset = new Coord(offsetX, offsetZ);
			this.size = new Coord(sizeX, sizeZ);
		}

		// Token: 0x06003D37 RID: 15671 RVA: 0x001D16C1 File Offset: 0x001CF8C1
		public CoordRect(float offsetX, float offsetZ, float sizeX, float sizeZ)
		{
			this.offset = new Coord((int)offsetX, (int)offsetZ);
			this.size = new Coord((int)sizeX, (int)sizeZ);
		}

		// Token: 0x06003D38 RID: 15672 RVA: 0x001D16E2 File Offset: 0x001CF8E2
		public CoordRect(Rect r)
		{
			this.offset = new Coord((int)r.x, (int)r.y);
			this.size = new Coord((int)r.width, (int)r.height);
		}

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06003D39 RID: 15673 RVA: 0x001D171A File Offset: 0x001CF91A
		public Rect rect
		{
			get
			{
				return new Rect((float)this.offset.x, (float)this.offset.z, (float)this.size.x, (float)this.size.z);
			}
		}

		// Token: 0x06003D3A RID: 15674 RVA: 0x001D1754 File Offset: 0x001CF954
		public static CoordRect PickIntersectingCells(CoordRect rect, int cellRes)
		{
			int num = rect.offset.x + rect.size.x;
			int num2 = rect.offset.z + rect.size.z;
			int num3 = rect.offset.x / cellRes;
			if (rect.offset.x < 0 && rect.offset.x % cellRes != 0)
			{
				num3--;
			}
			int num4 = rect.offset.z / cellRes;
			if (rect.offset.z < 0 && rect.offset.z % cellRes != 0)
			{
				num4--;
			}
			int num5 = num / cellRes;
			if (num >= 0 && num % cellRes != 0)
			{
				num5++;
			}
			int num6 = num2 / cellRes;
			if (num2 >= 0 && num2 % cellRes != 0)
			{
				num6++;
			}
			return new CoordRect(num3, num4, num5 - num3, num6 - num4);
		}

		// Token: 0x06003D3B RID: 15675 RVA: 0x001D1828 File Offset: 0x001CFA28
		public static CoordRect PickIntersectingCellsByPos(float rectMinX, float rectMinZ, float rectMaxX, float rectMaxZ, float cellSize)
		{
			int num = (int)(rectMinX / cellSize);
			if (rectMinX < 0f && rectMinX != (float)num * cellSize)
			{
				num--;
			}
			int num2 = (int)(rectMinZ / cellSize);
			if (rectMinZ < 0f && rectMinZ != (float)num2 * cellSize)
			{
				num2--;
			}
			int num3 = (int)(rectMaxX / cellSize);
			if (rectMaxX >= 0f && rectMaxX != (float)num3 * cellSize)
			{
				num3++;
			}
			int num4 = (int)(rectMaxZ / cellSize);
			if (rectMaxZ >= 0f && rectMaxZ != (float)num4 * cellSize)
			{
				num4++;
			}
			return new CoordRect(num, num2, num3 - num, num4 - num2);
		}

		// Token: 0x06003D3C RID: 15676 RVA: 0x001D18AA File Offset: 0x001CFAAA
		public static CoordRect PickIntersectingCellsByPos(Vector3 pos, float range, float cellSize = 1f)
		{
			return CoordRect.PickIntersectingCellsByPos(pos.x - range, pos.z - range, pos.x + range, pos.z + range, cellSize);
		}

		// Token: 0x06003D3D RID: 15677 RVA: 0x001D18D4 File Offset: 0x001CFAD4
		public static CoordRect PickIntersectingCellsByPos(Rect rect, float cellSize = 1f)
		{
			return CoordRect.PickIntersectingCellsByPos(rect.position.x, rect.position.y, rect.position.x + rect.size.x, rect.position.y + rect.size.y, cellSize);
		}

		// Token: 0x06003D3E RID: 15678 RVA: 0x001D1931 File Offset: 0x001CFB31
		public int GetPos(int x, int z)
		{
			return (z - this.offset.z) * this.size.x + x - this.offset.x;
		}

		// Token: 0x06003D3F RID: 15679 RVA: 0x001D195C File Offset: 0x001CFB5C
		public int GetPos(Vector2 v)
		{
			int num = (int)(v.x + 0.5f);
			if (v.x < 0f)
			{
				num--;
			}
			int num2 = (int)(v.y + 0.5f);
			if (v.y < 0f)
			{
				num2--;
			}
			return (num2 - this.offset.z) * this.size.x + num - this.offset.x;
		}

		// Token: 0x06003D40 RID: 15680 RVA: 0x001D19D0 File Offset: 0x001CFBD0
		public int GetPos(float x, float z)
		{
			int num = (int)(x + 0.5f);
			if (x < 1f)
			{
				num--;
			}
			int num2 = (int)(z + 0.5f);
			if (z < 1f)
			{
				num2--;
			}
			return (num2 - this.offset.z) * this.size.x + num - this.offset.x;
		}

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06003D41 RID: 15681 RVA: 0x001D1A2E File Offset: 0x001CFC2E
		// (set) Token: 0x06003D42 RID: 15682 RVA: 0x001D1A41 File Offset: 0x001CFC41
		public Coord Max
		{
			get
			{
				return this.offset + this.size;
			}
			set
			{
				this.offset = value - this.size;
			}
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06003D43 RID: 15683 RVA: 0x001D1A55 File Offset: 0x001CFC55
		// (set) Token: 0x06003D44 RID: 15684 RVA: 0x001D1A5D File Offset: 0x001CFC5D
		public Coord Min
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06003D45 RID: 15685 RVA: 0x001D1A66 File Offset: 0x001CFC66
		public Coord Center
		{
			get
			{
				return this.offset + this.size / 2;
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06003D46 RID: 15686 RVA: 0x001D1A7F File Offset: 0x001CFC7F
		public int Count
		{
			get
			{
				return this.size.x * this.size.z;
			}
		}

		// Token: 0x06003D47 RID: 15687 RVA: 0x001D1A98 File Offset: 0x001CFC98
		public static bool operator >(CoordRect c1, CoordRect c2)
		{
			return c1.size > c2.size;
		}

		// Token: 0x06003D48 RID: 15688 RVA: 0x001D1AAB File Offset: 0x001CFCAB
		public static bool operator <(CoordRect c1, CoordRect c2)
		{
			return c1.size < c2.size;
		}

		// Token: 0x06003D49 RID: 15689 RVA: 0x001D1ABE File Offset: 0x001CFCBE
		public static bool operator ==(CoordRect c1, CoordRect c2)
		{
			return c1.offset == c2.offset && c1.size == c2.size;
		}

		// Token: 0x06003D4A RID: 15690 RVA: 0x001D1AE6 File Offset: 0x001CFCE6
		public static bool operator !=(CoordRect c1, CoordRect c2)
		{
			return c1.offset != c2.offset || c1.size != c2.size;
		}

		// Token: 0x06003D4B RID: 15691 RVA: 0x001D1B0E File Offset: 0x001CFD0E
		public static CoordRect operator *(CoordRect c, int s)
		{
			return new CoordRect(c.offset * s, c.size * s);
		}

		// Token: 0x06003D4C RID: 15692 RVA: 0x001D1B2D File Offset: 0x001CFD2D
		public static CoordRect operator *(CoordRect c, float s)
		{
			return new CoordRect(c.offset * s, c.size * s);
		}

		// Token: 0x06003D4D RID: 15693 RVA: 0x001D1B4C File Offset: 0x001CFD4C
		public static CoordRect operator /(CoordRect c, int s)
		{
			return new CoordRect(c.offset / s, c.size / s);
		}

		// Token: 0x06003D4E RID: 15694 RVA: 0x001D1B6C File Offset: 0x001CFD6C
		public CoordRect MapSized(int resolution)
		{
			return new CoordRect(Mathf.RoundToInt((float)this.offset.x / (1f * (float)this.size.x / (float)resolution)), Mathf.RoundToInt((float)this.offset.z / (1f * (float)this.size.z / (float)resolution)), resolution, resolution);
		}

		// Token: 0x06003D4F RID: 15695 RVA: 0x001D1BD0 File Offset: 0x001CFDD0
		public void Expand(int v)
		{
			this.offset.x = this.offset.x - v;
			this.offset.z = this.offset.z - v;
			this.size.x = this.size.x + v * 2;
			this.size.z = this.size.z + v * 2;
		}

		// Token: 0x06003D50 RID: 15696 RVA: 0x001D1C21 File Offset: 0x001CFE21
		public CoordRect Expanded(int v)
		{
			return new CoordRect(this.offset.x - v, this.offset.z - v, this.size.x + v * 2, this.size.z + v * 2);
		}

		// Token: 0x06003D51 RID: 15697 RVA: 0x001D1C60 File Offset: 0x001CFE60
		public void Contract(int v)
		{
			this.offset.x = this.offset.x + v;
			this.offset.z = this.offset.z + v;
			this.size.x = this.size.x - v * 2;
			this.size.z = this.size.z - v * 2;
		}

		// Token: 0x06003D52 RID: 15698 RVA: 0x001D1CB1 File Offset: 0x001CFEB1
		public CoordRect Contracted(int v)
		{
			return new CoordRect(this.offset.x + v, this.offset.z + v, this.size.x - v * 2, this.size.z - v * 2);
		}

		// Token: 0x06003D53 RID: 15699 RVA: 0x001D1CF0 File Offset: 0x001CFEF0
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		// Token: 0x06003D54 RID: 15700 RVA: 0x001D1D04 File Offset: 0x001CFF04
		public override int GetHashCode()
		{
			return this.offset.x * 100000000 + this.offset.z * 1000000 + this.size.x * 1000 + this.size.z;
		}

		// Token: 0x06003D55 RID: 15701 RVA: 0x001D1D54 File Offset: 0x001CFF54
		public void Clamp(Coord min, Coord max)
		{
			Coord max2 = this.Max;
			this.offset = Coord.Max(min, this.offset);
			this.size = Coord.Min(max - this.offset, max2 - this.offset);
			this.size.ClampPositive();
		}

		// Token: 0x06003D56 RID: 15702 RVA: 0x001D1DA8 File Offset: 0x001CFFA8
		public static CoordRect Intersect(CoordRect c1, CoordRect c2)
		{
			c1.Clamp(c2.Min, c2.Max);
			return c1;
		}

		// Token: 0x06003D57 RID: 15703 RVA: 0x001D1DC0 File Offset: 0x001CFFC0
		public static bool IsIntersecting(CoordRect c1, CoordRect c2)
		{
			return c2.Contains(c1.offset.x, c1.offset.z) || c2.Contains(c1.offset.x + c1.size.x, c1.offset.z) || c2.Contains(c1.offset.x, c1.offset.z + c1.size.z) || c2.Contains(c1.offset.x + c1.size.x, c1.offset.z + c1.size.z) || (c1.Contains(c2.offset.x, c2.offset.z) || c1.Contains(c2.offset.x + c2.size.x, c2.offset.z) || c1.Contains(c2.offset.x, c2.offset.z + c1.size.z) || c1.Contains(c2.offset.x + c2.size.x, c2.offset.z + c2.size.z));
		}

		// Token: 0x06003D58 RID: 15704 RVA: 0x001D1F30 File Offset: 0x001D0130
		public static CoordRect Combine(CoordRect[] rects)
		{
			Coord coord = new Coord(2000000000, 2000000000);
			Coord coord2 = new Coord(-2000000000, -2000000000);
			for (int i = 0; i < rects.Length; i++)
			{
				if (rects[i].offset.x < coord.x)
				{
					coord.x = rects[i].offset.x;
				}
				if (rects[i].offset.z < coord.z)
				{
					coord.z = rects[i].offset.z;
				}
				if (rects[i].offset.x + rects[i].size.x > coord2.x)
				{
					coord2.x = rects[i].offset.x + rects[i].size.x;
				}
				if (rects[i].offset.z + rects[i].size.z > coord2.z)
				{
					coord2.z = rects[i].offset.z + rects[i].size.z;
				}
			}
			return new CoordRect(coord, coord2 - coord);
		}

		// Token: 0x06003D59 RID: 15705 RVA: 0x001D208C File Offset: 0x001D028C
		public Coord CoordByNum(int num)
		{
			int num2 = num / this.size.x;
			return new Coord(num - num2 * this.size.x + this.offset.x, num2 + this.offset.z);
		}

		// Token: 0x06003D5A RID: 15706 RVA: 0x001D20D4 File Offset: 0x001D02D4
		public bool Contains(int x, int z)
		{
			return x - this.offset.x >= 0 && x - this.offset.x < this.size.x && z - this.offset.z >= 0 && z - this.offset.z < this.size.z;
		}

		// Token: 0x06003D5B RID: 15707 RVA: 0x001D2138 File Offset: 0x001D0338
		public bool Contains(float x, float z)
		{
			return x - (float)this.offset.x >= 0f && x - (float)this.offset.x < (float)this.size.x && z - (float)this.offset.z >= 0f && z - (float)this.offset.z < (float)this.size.z;
		}

		// Token: 0x06003D5C RID: 15708 RVA: 0x001D21AC File Offset: 0x001D03AC
		public bool Contains(float x, float z, float margins)
		{
			return x - (float)this.offset.x >= margins && x - (float)this.offset.x < (float)this.size.x - margins && z - (float)this.offset.z >= margins && z - (float)this.offset.z < (float)this.size.z - margins;
		}

		// Token: 0x06003D5D RID: 15709 RVA: 0x001D221C File Offset: 0x001D041C
		public bool CheckInRange(float x, float z)
		{
			return x - (float)this.offset.x >= 0f && x - (float)this.offset.x < (float)this.size.x && z - (float)this.offset.z >= 0f && z - (float)this.offset.z < (float)this.size.z;
		}

		// Token: 0x06003D5E RID: 15710 RVA: 0x001D2290 File Offset: 0x001D0490
		public bool CheckInRange(int x, int z)
		{
			return x - this.offset.x >= 0 && x - this.offset.x < this.size.x && z - this.offset.z >= 0 && z - this.offset.z < this.size.z;
		}

		// Token: 0x06003D5F RID: 15711 RVA: 0x001D22F4 File Offset: 0x001D04F4
		public bool CheckInRange(Coord coord)
		{
			return coord.x >= this.offset.x && coord.x < this.offset.x + this.size.x && coord.z >= this.offset.z && coord.z < this.offset.z + this.size.z;
		}

		// Token: 0x06003D60 RID: 15712 RVA: 0x001D2368 File Offset: 0x001D0568
		public bool CheckInRangeAndBounds(int x, int z)
		{
			return x > this.offset.x && x < this.offset.x + this.size.x - 1 && z > this.offset.z && z < this.offset.z + this.size.z - 1;
		}

		// Token: 0x06003D61 RID: 15713 RVA: 0x001D23CC File Offset: 0x001D05CC
		public bool CheckInRangeAndBounds(Coord coord)
		{
			return coord.x > this.offset.x && coord.x < this.offset.x + this.size.x - 1 && coord.z > this.offset.z && coord.z < this.offset.z + this.size.z - 1;
		}

		// Token: 0x06003D62 RID: 15714 RVA: 0x001D2444 File Offset: 0x001D0644
		public bool Contains(CoordRect r)
		{
			return r.offset.x >= this.offset.x && r.offset.x + r.size.x <= this.offset.x + this.size.x && r.offset.z >= this.offset.z && r.offset.z + r.size.z <= this.offset.z + this.size.z;
		}

		// Token: 0x06003D63 RID: 15715 RVA: 0x001D24E8 File Offset: 0x001D06E8
		public bool ContainsOrIntersects(CoordRect r)
		{
			return r.offset.x > this.offset.x - r.size.x && r.offset.x + r.size.x < this.offset.x + this.size.x + r.size.x && r.offset.z > this.offset.z - r.size.z && r.offset.z + r.size.z < this.offset.z + this.size.z + r.size.z;
		}

		// Token: 0x06003D64 RID: 15716 RVA: 0x001D25BC File Offset: 0x001D07BC
		public bool Divisible(float factor)
		{
			return (float)this.offset.x % factor == 0f && (float)this.offset.z % factor == 0f && (float)this.size.x % factor == 0f && (float)this.size.z % factor == 0f;
		}

		// Token: 0x06003D65 RID: 15717 RVA: 0x001D2620 File Offset: 0x001D0820
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				": offsetX:",
				this.offset.x,
				" offsetZ:",
				this.offset.z,
				" sizeX:",
				this.size.x,
				" sizeZ:",
				this.size.z
			});
		}

		// Token: 0x06003D66 RID: 15718 RVA: 0x001D26B8 File Offset: 0x001D08B8
		public IEnumerable<Coord> Cells(int cellSize)
		{
			Coord min = this.offset / cellSize;
			Coord max = (this.Max - 1) / cellSize + 1;
			int num;
			for (int x = min.x; x < max.x; x = num + 1)
			{
				for (int z = min.z; z < max.z; z = num + 1)
				{
					yield return new Coord(x, z);
					num = z;
				}
				num = x;
			}
			yield break;
		}

		// Token: 0x06003D67 RID: 15719 RVA: 0x001D26D4 File Offset: 0x001D08D4
		public CoordRect Approximate(int val)
		{
			CoordRect coordRect = default(CoordRect);
			coordRect.size.x = (this.size.x / val + 1) * val;
			coordRect.size.z = (this.size.z / val + 1) * val;
			coordRect.offset.x = this.offset.x - (coordRect.size.x - this.size.x) / 2;
			coordRect.offset.z = this.offset.z - (coordRect.size.z - this.size.z) / 2;
			coordRect.offset.x = (int)((float)(coordRect.offset.x / val) + 0.5f) * val;
			coordRect.offset.z = (int)((float)(coordRect.offset.z / val) + 0.5f) * val;
			return coordRect;
		}

		// Token: 0x06003D68 RID: 15720 RVA: 0x000023FD File Offset: 0x000005FD
		public void DrawGizmo()
		{
		}

		// Token: 0x06003D69 RID: 15721 RVA: 0x001D27CC File Offset: 0x001D09CC
		public string Encode()
		{
			return string.Concat(new object[]
			{
				"offsetX=",
				this.offset.x,
				" offsetZ=",
				this.offset.z,
				" sizeX=",
				this.size.x,
				" sizeZ=",
				this.size.z
			});
		}

		// Token: 0x06003D6A RID: 15722 RVA: 0x001D2850 File Offset: 0x001D0A50
		public void Decode(string[] lineMembers)
		{
			this.offset.x = (int)lineMembers[2].Parse(typeof(int));
			this.offset.z = (int)lineMembers[3].Parse(typeof(int));
			this.size.x = (int)lineMembers[4].Parse(typeof(int));
			this.size.z = (int)lineMembers[5].Parse(typeof(int));
		}

		// Token: 0x04002BBA RID: 11194
		public Coord offset;

		// Token: 0x04002BBB RID: 11195
		public Coord size;
	}
}
