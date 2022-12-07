using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Token: 0x02000331 RID: 817
[Serializable]
public class ShapeSnapshot
{
	// Token: 0x0600322F RID: 12847 RVA: 0x00196B04 File Offset: 0x00194D04
	public void SetHeights(float[,] argHeights)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, argHeights);
		this.heightsSerialized = memoryStream.ToArray();
	}

	// Token: 0x06003230 RID: 12848 RVA: 0x00196B30 File Offset: 0x00194D30
	public float[,] GetHeights()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream serializationStream = new MemoryStream(this.heightsSerialized);
		this.heights = (binaryFormatter.Deserialize(serializationStream) as float[,]);
		return this.heights;
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x00196B68 File Offset: 0x00194D68
	public void SetSplats(float[,,] argSplats)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, argSplats);
		this.splatsSerialized = memoryStream.ToArray();
	}

	// Token: 0x06003232 RID: 12850 RVA: 0x00196B94 File Offset: 0x00194D94
	public float[,,] GetSplats()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream serializationStream = new MemoryStream(this.splatsSerialized);
		this.splats = (binaryFormatter.Deserialize(serializationStream) as float[,,]);
		return this.splats;
	}

	// Token: 0x06003233 RID: 12851 RVA: 0x00196BCB File Offset: 0x00194DCB
	public ShapeSnapshot(Bounds argBounds, Vector3 argWorldPosition)
	{
		this.bounds = argBounds;
		this.worldPosition = argWorldPosition;
	}

	// Token: 0x040021BD RID: 8637
	public float[] testArray;

	// Token: 0x040021BE RID: 8638
	[HideInInspector]
	public byte[] heightsSerialized;

	// Token: 0x040021BF RID: 8639
	[HideInInspector]
	public byte[] splatsSerialized;

	// Token: 0x040021C0 RID: 8640
	public Bounds bounds;

	// Token: 0x040021C1 RID: 8641
	public Vector3 worldPosition;

	// Token: 0x040021C2 RID: 8642
	private float[,] heights;

	// Token: 0x040021C3 RID: 8643
	private float[,,] splats;

	// Token: 0x040021C4 RID: 8644
	public List<TreeData> trees = new List<TreeData>();

	// Token: 0x040021C5 RID: 8645
	public List<GameObject> objects = new List<GameObject>();
}
