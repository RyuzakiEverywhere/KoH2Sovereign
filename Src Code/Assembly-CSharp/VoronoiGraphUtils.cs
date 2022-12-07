using System;
using Unity.Mathematics;

// Token: 0x0200015A RID: 346
public static class VoronoiGraphUtils
{
	// Token: 0x060011AA RID: 4522 RVA: 0x000BA1BF File Offset: 0x000B83BF
	public static float2 Rotate90Clockwise(float2 vector)
	{
		return new float2(vector.y, -vector.x);
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x000BA1D4 File Offset: 0x000B83D4
	public static bool Intersect(VoronoiGraphUtils.Ray2D ray, VoronoiGraphUtils.Line2D line, out float2 hitPoint)
	{
		ray.direction = math.normalize(ray.direction);
		line.tangent = math.normalize(line.tangent);
		float2 @float = new float2(-ray.direction.y, ray.direction.x);
		float lhs = math.dot(line.origin - ray.origin, @float) / math.dot(-@float, line.tangent);
		hitPoint = lhs * line.tangent + line.origin;
		return (double)math.dot(hitPoint - ray.origin, ray.direction) > 0.0;
	}

	// Token: 0x0200067C RID: 1660
	public struct Line2D
	{
		// Token: 0x060047DC RID: 18396 RVA: 0x00215B30 File Offset: 0x00213D30
		public float DistanceTo(float2 point)
		{
			float2 y = math.normalize(VoronoiGraphUtils.Rotate90Clockwise(this.tangent));
			return math.abs(math.dot(point - this.origin, y));
		}

		// Token: 0x040035B1 RID: 13745
		public float2 origin;

		// Token: 0x040035B2 RID: 13746
		public float2 tangent;
	}

	// Token: 0x0200067D RID: 1661
	public struct Ray2D
	{
		// Token: 0x060047DD RID: 18397 RVA: 0x00215B65 File Offset: 0x00213D65
		public Ray2D(float2 origin, float2 direction)
		{
			this.origin = origin;
			this.direction = direction;
		}

		// Token: 0x040035B3 RID: 13747
		public float2 origin;

		// Token: 0x040035B4 RID: 13748
		public float2 direction;
	}

	// Token: 0x0200067E RID: 1662
	public struct CellEdge
	{
		// Token: 0x040035B5 RID: 13749
		public VoronoiGraph.Cell cell;

		// Token: 0x040035B6 RID: 13750
		public VoronoiGraphUtils.Line2D line;
	}
}
