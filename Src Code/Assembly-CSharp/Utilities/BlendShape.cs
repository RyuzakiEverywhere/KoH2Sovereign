using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000468 RID: 1128
	public class BlendShape : MonoBehaviour
	{
		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06003B44 RID: 15172 RVA: 0x001C6834 File Offset: 0x001C4A34
		public bool HasBlendShapes
		{
			get
			{
				return this.mesh && this.mesh.sharedMesh.blendShapeCount > 0;
			}
		}

		// Token: 0x06003B45 RID: 15173 RVA: 0x001C6858 File Offset: 0x001C4A58
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

		// Token: 0x06003B46 RID: 15174 RVA: 0x001C68A3 File Offset: 0x001C4AA3
		private void Awake()
		{
			if (this.random)
			{
				this.RandomizeShapes();
			}
		}

		// Token: 0x06003B47 RID: 15175 RVA: 0x001C68B4 File Offset: 0x001C4AB4
		private void Reset()
		{
			this.mesh = base.GetComponentInChildren<SkinnedMeshRenderer>();
			if (this.mesh)
			{
				this.blendShapes = new float[this.mesh.sharedMesh.blendShapeCount];
				for (int i = 0; i < this.blendShapes.Length; i++)
				{
					this.blendShapes[i] = this.mesh.GetBlendShapeWeight(i);
				}
			}
		}

		// Token: 0x06003B48 RID: 15176 RVA: 0x001C691C File Offset: 0x001C4B1C
		public virtual void RandomizeShapes()
		{
			if (this.HasBlendShapes)
			{
				for (int i = 0; i < this.blendShapes.Length; i++)
				{
					this.blendShapes[i] = (float)Random.Range(0, 100);
					this.mesh.SetBlendShapeWeight(i, this.blendShapes[i]);
				}
				this.UpdateLODs();
			}
		}

		// Token: 0x06003B49 RID: 15177 RVA: 0x001C696F File Offset: 0x001C4B6F
		public virtual void SetBlendShape(string name, float value)
		{
			if (this.HasBlendShapes)
			{
				this.PinnedShape = this.mesh.sharedMesh.GetBlendShapeIndex(name);
				if (this.PinnedShape != -1)
				{
					this.mesh.SetBlendShapeWeight(this.PinnedShape, value);
				}
			}
		}

		// Token: 0x06003B4A RID: 15178 RVA: 0x001C69AC File Offset: 0x001C4BAC
		public virtual void SetBlendShape(int index, float value)
		{
			if (this.HasBlendShapes)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = this.mesh;
				this.PinnedShape = index;
				skinnedMeshRenderer.SetBlendShapeWeight(index, value);
			}
		}

		// Token: 0x06003B4B RID: 15179 RVA: 0x001C69D7 File Offset: 0x001C4BD7
		public virtual void _PinShape(string name)
		{
			this.PinnedShape = this.mesh.sharedMesh.GetBlendShapeIndex(name);
		}

		// Token: 0x06003B4C RID: 15180 RVA: 0x001C69F0 File Offset: 0x001C4BF0
		public virtual void _PinShape(int index)
		{
			this.PinnedShape = index;
		}

		// Token: 0x06003B4D RID: 15181 RVA: 0x001C69FC File Offset: 0x001C4BFC
		public virtual void _PinnedShapeSetValue(float value)
		{
			if (this.PinnedShape != -1)
			{
				value = Mathf.Clamp(value, 0f, 100f);
				this.blendShapes[this.PinnedShape] = value;
				this.mesh.SetBlendShapeWeight(this.PinnedShape, value);
				this.UpdateLODs(this.PinnedShape);
			}
		}

		// Token: 0x06003B4E RID: 15182 RVA: 0x001C6A50 File Offset: 0x001C4C50
		public virtual void UpdateBlendShapes()
		{
			if (this.mesh && this.blendShapes != null)
			{
				if (this.blendShapes.Length != this.mesh.sharedMesh.blendShapeCount)
				{
					this.blendShapes = new float[this.mesh.sharedMesh.blendShapeCount];
				}
				for (int i = 0; i < this.blendShapes.Length; i++)
				{
					this.mesh.SetBlendShapeWeight(i, this.blendShapes[i]);
				}
				this.UpdateLODs();
			}
		}

		// Token: 0x06003B4F RID: 15183 RVA: 0x001C6AD4 File Offset: 0x001C4CD4
		protected virtual void UpdateLODs()
		{
			for (int i = 0; i < this.blendShapes.Length; i++)
			{
				this.UpdateLODs(i);
			}
		}

		// Token: 0x06003B50 RID: 15184 RVA: 0x001C6AFC File Offset: 0x001C4CFC
		protected virtual void UpdateLODs(int index)
		{
			if (this.LODs != null)
			{
				SkinnedMeshRenderer[] lods = this.LODs;
				for (int i = 0; i < lods.Length; i++)
				{
					lods[i].SetBlendShapeWeight(index, this.blendShapes[index]);
				}
			}
		}

		// Token: 0x04002B1D RID: 11037
		public SkinnedMeshRenderer mesh;

		// Token: 0x04002B1E RID: 11038
		public SkinnedMeshRenderer[] LODs;

		// Token: 0x04002B1F RID: 11039
		[Range(0f, 100f)]
		public float[] blendShapes;

		// Token: 0x04002B20 RID: 11040
		public bool random;

		// Token: 0x04002B21 RID: 11041
		public int PinnedShape;
	}
}
