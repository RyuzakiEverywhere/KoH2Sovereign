using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000469 RID: 1129
	[Serializable]
	public class MeshBlendShapes
	{
		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06003B52 RID: 15186 RVA: 0x001C6B37 File Offset: 0x001C4D37
		public bool HasBlendShapes
		{
			get
			{
				return this.mesh && this.mesh.sharedMesh.blendShapeCount > 0;
			}
		}

		// Token: 0x06003B53 RID: 15187 RVA: 0x001C6B5C File Offset: 0x001C4D5C
		public virtual void UpdateBlendShapes()
		{
			if (this.mesh != null && this.blendShapes != null)
			{
				if (this.NameID == string.Empty)
				{
					this.NameID = this.mesh.name;
				}
				if (this.blendShapes.Length != this.mesh.sharedMesh.blendShapeCount)
				{
					this.blendShapes = new float[this.mesh.sharedMesh.blendShapeCount];
				}
				for (int i = 0; i < this.blendShapes.Length; i++)
				{
					this.mesh.SetBlendShapeWeight(i, this.blendShapes[i]);
				}
			}
		}

		// Token: 0x06003B54 RID: 15188 RVA: 0x001C6C04 File Offset: 0x001C4E04
		public virtual float[] GetBlendShapeValues()
		{
			if (this.HasBlendShapes)
			{
				float[] array = new float[this.mesh.sharedMesh.blendShapeCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.mesh.GetBlendShapeWeight(i);
				}
				return array;
			}
			return null;
		}

		// Token: 0x06003B55 RID: 15189 RVA: 0x001C6C50 File Offset: 0x001C4E50
		public void SetRandom()
		{
			if (this.HasBlendShapes)
			{
				for (int i = 0; i < this.blendShapes.Length; i++)
				{
					this.blendShapes[i] = (float)Random.Range(0, 100);
					this.mesh.SetBlendShapeWeight(i, this.blendShapes[i]);
				}
			}
		}

		// Token: 0x06003B56 RID: 15190 RVA: 0x001C6CA0 File Offset: 0x001C4EA0
		public void SetBlendShape(string name, float value)
		{
			if (this.HasBlendShapes)
			{
				int blendShapeIndex = this.mesh.sharedMesh.GetBlendShapeIndex(name);
				if (blendShapeIndex != -1)
				{
					this.mesh.SetBlendShapeWeight(blendShapeIndex, value);
				}
			}
		}

		// Token: 0x06003B57 RID: 15191 RVA: 0x001C6CD8 File Offset: 0x001C4ED8
		public void SetBlendShape(int index, float value)
		{
			if (this.HasBlendShapes)
			{
				this.mesh.SetBlendShapeWeight(index, value);
			}
		}

		// Token: 0x04002B22 RID: 11042
		public string NameID;

		// Token: 0x04002B23 RID: 11043
		public SkinnedMeshRenderer mesh;

		// Token: 0x04002B24 RID: 11044
		[Range(0f, 100f)]
		public float[] blendShapes;
	}
}
