using System;
using UnityEngine;

// Token: 0x02000330 RID: 816
[Serializable]
public struct TreeData
{
	// Token: 0x0600322C RID: 12844 RVA: 0x001969B8 File Offset: 0x00194BB8
	public TreeData(TreeInstance source)
	{
		this.position = source.position;
		this.widthScale = source.widthScale;
		this.heightScale = source.heightScale;
		this.rotation = source.rotation;
		this.color = source.color;
		this.lightmapColor = source.lightmapColor;
		this.prototypeIndex = source.prototypeIndex;
	}

	// Token: 0x0600322D RID: 12845 RVA: 0x00196A1C File Offset: 0x00194C1C
	public static TreeData Create(TreeInstance treeInstance)
	{
		return new TreeData
		{
			position = treeInstance.position,
			widthScale = treeInstance.widthScale,
			heightScale = treeInstance.heightScale,
			rotation = treeInstance.rotation,
			color = treeInstance.color,
			lightmapColor = treeInstance.lightmapColor,
			prototypeIndex = treeInstance.prototypeIndex
		};
	}

	// Token: 0x0600322E RID: 12846 RVA: 0x00196A90 File Offset: 0x00194C90
	public TreeInstance ToTreeInstance()
	{
		return new TreeInstance
		{
			position = this.position,
			widthScale = this.widthScale,
			heightScale = this.heightScale,
			rotation = this.rotation,
			color = this.color,
			lightmapColor = this.lightmapColor,
			prototypeIndex = this.prototypeIndex
		};
	}

	// Token: 0x040021B6 RID: 8630
	public Vector3 position;

	// Token: 0x040021B7 RID: 8631
	public float widthScale;

	// Token: 0x040021B8 RID: 8632
	public float heightScale;

	// Token: 0x040021B9 RID: 8633
	public float rotation;

	// Token: 0x040021BA RID: 8634
	public Color32 color;

	// Token: 0x040021BB RID: 8635
	public Color32 lightmapColor;

	// Token: 0x040021BC RID: 8636
	public int prototypeIndex;
}
