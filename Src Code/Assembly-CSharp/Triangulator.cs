using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class Triangulator
{
	// Token: 0x06000988 RID: 2440 RVA: 0x0006C930 File Offset: 0x0006AB30
	public Triangulator(List<Point> points)
	{
		this.m_points = new List<Point>(points);
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x0006C950 File Offset: 0x0006AB50
	public int[] Triangulate()
	{
		List<int> list = new List<int>();
		int count = this.m_points.Count;
		if (count < 3)
		{
			return list.ToArray();
		}
		int[] array = new int[count];
		if (this.Area() > 0f)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = i;
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				array[j] = count - 1 - j;
			}
		}
		int k = count;
		int num = 2 * k;
		int num2 = k - 1;
		while (k > 2)
		{
			if (num-- <= 0)
			{
				return list.ToArray();
			}
			int num3 = num2;
			if (k <= num3)
			{
				num3 = 0;
			}
			num2 = num3 + 1;
			if (k <= num2)
			{
				num2 = 0;
			}
			int num4 = num2 + 1;
			if (k <= num4)
			{
				num4 = 0;
			}
			if (this.Snip(num3, num2, num4, k, array))
			{
				int item = array[num3];
				int item2 = array[num2];
				int item3 = array[num4];
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				int num5 = num2;
				for (int l = num2 + 1; l < k; l++)
				{
					array[num5] = array[l];
					num5++;
				}
				k--;
				num = 2 * k;
			}
		}
		list.Reverse();
		return list.ToArray();
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x0006CA88 File Offset: 0x0006AC88
	private float Area()
	{
		int count = this.m_points.Count;
		float num = 0f;
		int index = count - 1;
		int i = 0;
		while (i < count)
		{
			Point point = this.m_points[index];
			Point point2 = this.m_points[i];
			num += point.x * point2.y - point2.x * point.y;
			index = i++;
		}
		return num * 0.5f;
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0006CB00 File Offset: 0x0006AD00
	private bool Snip(int u, int v, int w, int n, int[] V)
	{
		Point point = this.m_points[V[u]];
		Point point2 = this.m_points[V[v]];
		Point point3 = this.m_points[V[w]];
		if (Mathf.Epsilon > (point2.x - point.x) * (point3.y - point.y) - (point2.y - point.y) * (point3.x - point.x))
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w)
			{
				Point p = this.m_points[V[i]];
				if (this.InsideTriangle(point, point2, point3, p))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x0006CBB8 File Offset: 0x0006ADB8
	private bool InsideTriangle(Point A, Point B, Point C, Point P)
	{
		float num = C.x - B.x;
		float num2 = C.y - B.y;
		float num3 = A.x - C.x;
		float num4 = A.y - C.y;
		float num5 = B.x - A.x;
		float num6 = B.y - A.y;
		float num7 = P.x - A.x;
		float num8 = P.y - A.y;
		float num9 = P.x - B.x;
		float num10 = P.y - B.y;
		float num11 = P.x - C.x;
		float num12 = P.y - C.y;
		float num13 = num * num10 - num2 * num9;
		float num14 = num5 * num8 - num6 * num7;
		float num15 = num3 * num12 - num4 * num11;
		return num13 >= 0f && num15 >= 0f && num14 >= 0f;
	}

	// Token: 0x040007C3 RID: 1987
	private List<Point> m_points = new List<Point>();
}
