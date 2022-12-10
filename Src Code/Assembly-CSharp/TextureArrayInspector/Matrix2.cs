using System;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x0200048A RID: 1162
	public class Matrix2<T> : ICloneable
	{
		// Token: 0x06003DB7 RID: 15799 RVA: 0x0000B82A File Offset: 0x00009A2A
		public Matrix2()
		{
		}

		// Token: 0x06003DB8 RID: 15800 RVA: 0x001D8624 File Offset: 0x001D6824
		public Matrix2(int x, int z, T[] array = null)
		{
			this.rect = new CoordRect(0, 0, x, z);
			this.count = x * z;
			if (array != null && array.Length < this.count)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Array length: ",
					array.Length,
					" is lower then matrix capacity: ",
					this.count
				}));
			}
			if (array != null && array.Length >= this.count)
			{
				this.array = array;
				return;
			}
			this.array = new T[this.count];
		}

		// Token: 0x06003DB9 RID: 15801 RVA: 0x001D86C0 File Offset: 0x001D68C0
		public Matrix2(CoordRect rect, T[] array = null)
		{
			this.rect = rect;
			this.count = rect.size.x * rect.size.z;
			if (array != null && array.Length < this.count)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Array length: ",
					array.Length,
					" is lower then matrix capacity: ",
					this.count
				}));
			}
			if (array != null && array.Length >= this.count)
			{
				this.array = array;
				return;
			}
			this.array = new T[this.count];
		}

		// Token: 0x06003DBA RID: 15802 RVA: 0x001D8768 File Offset: 0x001D6968
		public Matrix2(Coord offset, Coord size, T[] array = null)
		{
			this.rect = new CoordRect(offset, size);
			this.count = this.rect.size.x * this.rect.size.z;
			if (array != null && array.Length < this.count)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Array length: ",
					array.Length,
					" is lower then matrix capacity: ",
					this.count
				}));
			}
			if (array != null && array.Length >= this.count)
			{
				this.array = array;
				return;
			}
			this.array = new T[this.count];
		}

		// Token: 0x17000414 RID: 1044
		public T this[int x, int z]
		{
			get
			{
				return this.array[(z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x];
			}
			set
			{
				this.array[(z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x] = value;
			}
		}

		// Token: 0x17000415 RID: 1045
		public T this[float x, float z]
		{
			get
			{
				int num = (int)x;
				if (x < 1f)
				{
					num--;
				}
				int num2 = (int)z;
				if (z < 1f)
				{
					num2--;
				}
				return this.array[(num2 - this.rect.offset.z) * this.rect.size.x + num - this.rect.offset.x];
			}
			set
			{
				int num = (int)x;
				if (x < 1f)
				{
					num--;
				}
				int num2 = (int)z;
				if (z < 1f)
				{
					num2--;
				}
				this.array[(num2 - this.rect.offset.z) * this.rect.size.x + num - this.rect.offset.x] = value;
			}
		}

		// Token: 0x17000416 RID: 1046
		public T this[Coord c]
		{
			get
			{
				return this.array[(c.z - this.rect.offset.z) * this.rect.size.x + c.x - this.rect.offset.x];
			}
			set
			{
				this.array[(c.z - this.rect.offset.z) * this.rect.size.x + c.x - this.rect.offset.x] = value;
			}
		}

		// Token: 0x06003DC1 RID: 15809 RVA: 0x001D8A50 File Offset: 0x001D6C50
		public T CheckGet(int x, int z)
		{
			if (x >= this.rect.offset.x && x < this.rect.offset.x + this.rect.size.x && z >= this.rect.offset.z && z < this.rect.offset.z + this.rect.size.z)
			{
				return this.array[(z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x];
			}
			return default(T);
		}

		// Token: 0x17000417 RID: 1047
		public T this[Vector2 pos]
		{
			get
			{
				int num = (int)(pos.x + 0.5f);
				if (pos.x < 0f)
				{
					num--;
				}
				int num2 = (int)(pos.y + 0.5f);
				if (pos.y < 0f)
				{
					num2--;
				}
				return this.array[(num2 - this.rect.offset.z) * this.rect.size.x + num - this.rect.offset.x];
			}
			set
			{
				int num = (int)(pos.x + 0.5f);
				if (pos.x < 0f)
				{
					num--;
				}
				int num2 = (int)(pos.y + 0.5f);
				if (pos.y < 0f)
				{
					num2--;
				}
				this.array[(num2 - this.rect.offset.z) * this.rect.size.x + num - this.rect.offset.x] = value;
			}
		}

		// Token: 0x06003DC4 RID: 15812 RVA: 0x001D8C38 File Offset: 0x001D6E38
		public void Clear()
		{
			for (int i = 0; i < this.array.Length; i++)
			{
				this.array[i] = default(T);
			}
		}

		// Token: 0x06003DC5 RID: 15813 RVA: 0x001D8C70 File Offset: 0x001D6E70
		public void ChangeRect(CoordRect newRect, bool forceNewArray = false)
		{
			this.rect = newRect;
			this.count = newRect.size.x * newRect.size.z;
			if (this.array.Length < this.count || forceNewArray)
			{
				this.array = new T[this.count];
			}
		}

		// Token: 0x06003DC6 RID: 15814 RVA: 0x001D8CC6 File Offset: 0x001D6EC6
		public virtual object Clone()
		{
			return this.Clone(null);
		}

		// Token: 0x06003DC7 RID: 15815 RVA: 0x001D8CD0 File Offset: 0x001D6ED0
		public Matrix2<T> Clone(Matrix2<T> result)
		{
			if (result == null)
			{
				result = new Matrix2<T>(this.rect, null);
			}
			result.rect = this.rect;
			result.pos = this.pos;
			result.count = this.count;
			if (result.array.Length != this.array.Length)
			{
				result.array = new T[this.array.Length];
			}
			for (int i = 0; i < this.array.Length; i++)
			{
				result.array[i] = this.array[i];
			}
			return result;
		}

		// Token: 0x06003DC8 RID: 15816 RVA: 0x001D8D64 File Offset: 0x001D6F64
		public void Fill(T v)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] = v;
			}
		}

		// Token: 0x06003DC9 RID: 15817 RVA: 0x001D8D90 File Offset: 0x001D6F90
		public void Fill(Matrix2<T> m, bool removeBorders = false)
		{
			CoordRect centerRect = CoordRect.Intersect(this.rect, m.rect);
			Coord min = centerRect.Min;
			Coord max = centerRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					this[i, j] = m[i, j];
				}
			}
			if (removeBorders)
			{
				this.RemoveBorders(centerRect);
			}
		}

		// Token: 0x06003DCA RID: 15818 RVA: 0x001D8E0B File Offset: 0x001D700B
		public void SetPos(int x, int z)
		{
			this.pos = (z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x;
		}

		// Token: 0x06003DCB RID: 15819 RVA: 0x001D8E4C File Offset: 0x001D704C
		public void SetPos(int x, int z, int s)
		{
			this.pos = (z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x + s * this.rect.size.x * this.rect.size.z;
		}

		// Token: 0x06003DCC RID: 15820 RVA: 0x001D8EB9 File Offset: 0x001D70B9
		public void MoveX()
		{
			this.pos++;
		}

		// Token: 0x06003DCD RID: 15821 RVA: 0x001D8EC9 File Offset: 0x001D70C9
		public void MoveZ()
		{
			this.pos += this.rect.size.x;
		}

		// Token: 0x06003DCE RID: 15822 RVA: 0x001D8EE8 File Offset: 0x001D70E8
		public void MovePrevX()
		{
			this.pos--;
		}

		// Token: 0x06003DCF RID: 15823 RVA: 0x001D8EF8 File Offset: 0x001D70F8
		public void MovePrevZ()
		{
			this.pos -= this.rect.size.x;
		}

		// Token: 0x06003DD0 RID: 15824 RVA: 0x001D8F18 File Offset: 0x001D7118
		public void RemoveBorders()
		{
			Coord min = this.rect.Min;
			Coord coord = this.rect.Max - 1;
			for (int i = min.x; i <= coord.x; i++)
			{
				this.SetPos(i, min.z);
				this.array[this.pos] = this.array[this.pos + this.rect.size.x];
			}
			for (int j = min.x; j <= coord.x; j++)
			{
				this.SetPos(j, coord.z);
				this.array[this.pos] = this.array[this.pos - this.rect.size.x];
			}
			for (int k = min.z; k <= coord.z; k++)
			{
				this.SetPos(min.x, k);
				this.array[this.pos] = this.array[this.pos + 1];
			}
			for (int l = min.z; l <= coord.z; l++)
			{
				this.SetPos(coord.x, l);
				this.array[this.pos] = this.array[this.pos - 1];
			}
		}

		// Token: 0x06003DD1 RID: 15825 RVA: 0x001D9088 File Offset: 0x001D7288
		public void RemoveBorders(int borderMinX, int borderMinZ, int borderMaxX, int borderMaxZ)
		{
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			if (borderMinZ != 0)
			{
				for (int i = min.x; i < max.x; i++)
				{
					T value = this[i, min.z + borderMinZ];
					for (int j = min.z; j < min.z + borderMinZ; j++)
					{
						this[i, j] = value;
					}
				}
			}
			if (borderMaxZ != 0)
			{
				for (int k = min.x; k < max.x; k++)
				{
					T value2 = this[k, max.z - borderMaxZ];
					for (int l = max.z - borderMaxZ; l < max.z; l++)
					{
						this[k, l] = value2;
					}
				}
			}
			if (borderMinX != 0)
			{
				for (int m = min.z; m < max.z; m++)
				{
					T value3 = this[min.x + borderMinX, m];
					for (int n = min.x; n < min.x + borderMinX; n++)
					{
						this[n, m] = value3;
					}
				}
			}
			if (borderMaxX != 0)
			{
				for (int num = min.z; num < max.z; num++)
				{
					T value4 = this[max.x - borderMaxX, num];
					for (int num2 = max.x - borderMaxX; num2 < max.x; num2++)
					{
						this[num2, num] = value4;
					}
				}
			}
		}

		// Token: 0x06003DD2 RID: 15826 RVA: 0x001D9204 File Offset: 0x001D7404
		public void RemoveBorders(CoordRect centerRect)
		{
			this.RemoveBorders(Mathf.Max(0, centerRect.offset.x - this.rect.offset.x), Mathf.Max(0, centerRect.offset.z - this.rect.offset.z), Mathf.Max(0, this.rect.Max.x - centerRect.Max.x + 1), Mathf.Max(0, this.rect.Max.z - centerRect.Max.z + 1));
		}

		// Token: 0x04002BBC RID: 11196
		public CoordRect rect;

		// Token: 0x04002BBD RID: 11197
		public int count;

		// Token: 0x04002BBE RID: 11198
		public T[] array;

		// Token: 0x04002BBF RID: 11199
		public int pos;
	}
}
