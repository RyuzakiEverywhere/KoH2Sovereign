using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class VoronoiGraphDisplay : MonoBehaviour
{
	// Token: 0x060011A7 RID: 4519 RVA: 0x000B9A53 File Offset: 0x000B7C53
	[ContextMenu("Generate")]
	public void Generate()
	{
		this.graph = VoronoiGraph.Generate(this.parameters);
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x000B9A68 File Offset: 0x000B7C68
	private void OnDrawGizmos()
	{
		if (this.graph == null)
		{
			return;
		}
		if (!this.graph.IsInitialized)
		{
			return;
		}
		VoronoiGraph.Cell[,] cells_grid = this.graph.cells_grid;
		int upperBound = cells_grid.GetUpperBound(0);
		int upperBound2 = cells_grid.GetUpperBound(1);
		for (int i = cells_grid.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = cells_grid.GetLowerBound(1); j <= upperBound2; j++)
			{
				VoronoiGraph.Cell cell = cells_grid[i, j];
				Gizmos.color = this.cellCenterColor;
				Gizmos.DrawSphere(new Vector3(cell.center.x, cell.center.y, 0f), this.cellCenterSize);
			}
		}
		foreach (VoronoiGraph.Edge edge in this.graph.edges)
		{
			Gizmos.color = this.edgeColor;
			Gizmos.DrawLine(new Vector3(edge.end1.position.x, edge.end1.position.y, 0f), new Vector3(edge.end2.position.x, edge.end2.position.y, 0f));
		}
		foreach (VoronoiGraph.Vertex vertex in this.graph.vertices)
		{
			Gizmos.color = this.vertexColor;
			Gizmos.DrawSphere(new Vector3(vertex.position.x, vertex.position.y, 0f), this.vertexSize);
		}
		VoronoiGraph.Cell cell2 = this.graph.cells_grid[this.cellIndex.y, this.cellIndex.x];
		HashSet<VoronoiGraph.Edge> hashSet = new HashSet<VoronoiGraph.Edge>();
		hashSet.UnionWith(cell2.Neigbourhood.SelectMany((VoronoiGraph.Cell c) => c.edges));
		foreach (VoronoiGraph.Cell cell3 in cell2.Neigbourhood)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(new Vector3(cell3.center.x, cell3.center.y, 0f), this.cellCenterSize);
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(new Vector3(cell2.center.x, cell2.center.y, 0f), this.cellCenterSize);
		foreach (VoronoiGraph.Edge edge2 in hashSet)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(new Vector3(edge2.end1.position.x, edge2.end1.position.y, -0.01f), new Vector3(edge2.end2.position.x, edge2.end2.position.y, -0.01f));
		}
		if (this.vertexIndex > 0 && this.vertexIndex < this.graph.vertices.Count)
		{
			foreach (VoronoiGraph.Edge edge3 in this.graph.vertices[this.vertexIndex].edges)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(new Vector3(edge3.end1.position.x, edge3.end1.position.y, -0.01f), new Vector3(edge3.end2.position.x, edge3.end2.position.y, -0.01f));
			}
		}
		if (this.edgeIndex > 0 && this.edgeIndex < this.graph.edges.Count)
		{
			VoronoiGraph.Edge edge4 = this.graph.edges[this.edgeIndex];
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(new Vector3(edge4.end1.position.x, edge4.end1.position.y, 0f), this.vertexSize);
			Gizmos.DrawSphere(new Vector3(edge4.end2.position.x, edge4.end2.position.y, 0f), this.vertexSize);
			Gizmos.DrawLine(new Vector3(edge4.end1.position.x, edge4.end1.position.y, -0.01f), new Vector3(edge4.end2.position.x, edge4.end2.position.y, -0.01f));
			Gizmos.DrawSphere(new Vector3(edge4.cell1.center.x, edge4.cell1.center.y, 0f), this.cellCenterSize);
			Gizmos.DrawSphere(new Vector3(edge4.cell2.center.x, edge4.cell2.center.y, 0f), this.cellCenterSize);
		}
		VoronoiGraph.Cell closestCell = this.graph.GetClosestCell(this.position);
		VoronoiGraph.Edge closestEdge = this.graph.GetClosestEdge(this.position);
		VoronoiGraph.Vertex closestVertex = this.graph.GetClosestVertex(this.position);
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(new Vector3(this.position.x, this.position.y, 0f), this.cellCenterSize * 0.8f);
		Gizmos.DrawSphere(new Vector3(closestCell.center.x, closestCell.center.y, 0f), this.cellCenterSize * 1.5f);
		Gizmos.DrawLine(new Vector3(closestEdge.end1.position.x, closestEdge.end1.position.y, -0.01f), new Vector3(closestEdge.end2.position.x, closestEdge.end2.position.y, -0.01f));
		Gizmos.DrawSphere(new Vector3(closestVertex.position.x, closestVertex.position.y, 0f), this.vertexSize * 1.5f);
	}

	// Token: 0x04000BE2 RID: 3042
	[SerializeField]
	private VoronoiGraph.GenerationParameters parameters;

	// Token: 0x04000BE3 RID: 3043
	[Header("Gizmo parameters")]
	[SerializeField]
	private Color vertexColor = Color.red;

	// Token: 0x04000BE4 RID: 3044
	[SerializeField]
	private Color edgeColor = Color.blue;

	// Token: 0x04000BE5 RID: 3045
	[SerializeField]
	private Color cellCenterColor = Color.green;

	// Token: 0x04000BE6 RID: 3046
	[SerializeField]
	private float vertexSize = 0.05f;

	// Token: 0x04000BE7 RID: 3047
	[SerializeField]
	private float cellCenterSize = 0.1f;

	// Token: 0x04000BE8 RID: 3048
	[SerializeField]
	private int vertexIndex;

	// Token: 0x04000BE9 RID: 3049
	[SerializeField]
	private int edgeIndex;

	// Token: 0x04000BEA RID: 3050
	[SerializeField]
	private int2 cellIndex;

	// Token: 0x04000BEB RID: 3051
	[SerializeField]
	private float2 position;

	// Token: 0x04000BEC RID: 3052
	public VoronoiGraph graph;
}
