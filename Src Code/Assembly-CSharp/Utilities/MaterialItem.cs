using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000476 RID: 1142
	[Serializable]
	public class MaterialItem
	{
		// Token: 0x06003BBE RID: 15294 RVA: 0x001C8659 File Offset: 0x001C6859
		public MaterialItem()
		{
			this.Name = "NameHere";
			this.mesh = null;
			this.materials = new Material[0];
		}

		// Token: 0x06003BBF RID: 15295 RVA: 0x001C868A File Offset: 0x001C688A
		public MaterialItem(MeshRenderer MR)
		{
			this.Name = "NameHere";
			this.mesh = MR;
			this.materials = new Material[0];
		}

		// Token: 0x06003BC0 RID: 15296 RVA: 0x001C86BB File Offset: 0x001C68BB
		public MaterialItem(string name, MeshRenderer MR, Material[] mats)
		{
			this.Name = name;
			this.mesh = MR;
			this.materials = mats;
		}

		// Token: 0x06003BC1 RID: 15297 RVA: 0x001C86E3 File Offset: 0x001C68E3
		public MaterialItem(string name, MeshRenderer MR)
		{
			this.Name = name;
			this.mesh = MR;
			this.materials = new Material[0];
		}

		// Token: 0x06003BC2 RID: 15298 RVA: 0x001C8710 File Offset: 0x001C6910
		public virtual void ChangeMaterial()
		{
			this.current++;
			if (this.current < 0)
			{
				this.current = 0;
			}
			this.current %= this.materials.Length;
			Material[] sharedMaterials = this.mesh.sharedMaterials;
			if (this.materials[this.current] != null)
			{
				sharedMaterials[this.indexM] = this.materials[this.current];
				this.mesh.sharedMaterials = sharedMaterials;
				this.ChangeLOD(this.current);
				this.OnMaterialChanged.Invoke(this.current);
				return;
			}
			Debug.LogWarning("The Material on the Slot: " + this.current + " is empty");
		}

		// Token: 0x06003BC3 RID: 15299 RVA: 0x001C87D0 File Offset: 0x001C69D0
		public virtual void Set_by_BinaryIndex(int binaryCurrent)
		{
			int index = 0;
			for (int i = 0; i < this.materials.Length; i++)
			{
				if (MalbersTools.IsBitActive(binaryCurrent, i))
				{
					index = i;
					break;
				}
			}
			this.ChangeMaterial(index);
		}

		// Token: 0x06003BC4 RID: 15300 RVA: 0x001C8808 File Offset: 0x001C6A08
		internal void ChangeLOD(int index)
		{
			if (!this.HasLODs)
			{
				return;
			}
			foreach (Renderer renderer in this.LODs)
			{
				if (renderer == null)
				{
					break;
				}
				Material[] sharedMaterials = renderer.sharedMaterials;
				sharedMaterials[this.indexM] = this.materials[this.current];
				if (this.materials[this.current] != null)
				{
					renderer.sharedMaterials = sharedMaterials;
				}
			}
		}

		// Token: 0x06003BC5 RID: 15301 RVA: 0x001C8878 File Offset: 0x001C6A78
		internal void ChangeLOD(Material mat)
		{
			if (!this.HasLODs)
			{
				return;
			}
			Material[] sharedMaterials = this.mesh.sharedMaterials;
			sharedMaterials[this.indexM] = mat;
			Renderer[] lods = this.LODs;
			for (int i = 0; i < lods.Length; i++)
			{
				lods[i].sharedMaterials = sharedMaterials;
			}
		}

		// Token: 0x06003BC6 RID: 15302 RVA: 0x001C88C1 File Offset: 0x001C6AC1
		public virtual void NextMaterial()
		{
			this.ChangeMaterial();
		}

		// Token: 0x06003BC7 RID: 15303 RVA: 0x001C88CC File Offset: 0x001C6ACC
		public virtual void ChangeMaterial(int index)
		{
			if (index < 0)
			{
				index = 0;
			}
			index %= this.materials.Length;
			if (!(this.materials[index] != null))
			{
				Debug.LogWarning("The material on the Slot: " + index + "  is empty");
				return;
			}
			Material[] sharedMaterials = this.mesh.sharedMaterials;
			if (sharedMaterials.Length - 1 < this.indexM)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"The Meshes on the ",
					this.Name,
					" Material Item, does not have ",
					this.indexM + 1,
					" Materials, please change the ID parameter to value lower than ",
					sharedMaterials.Length
				}));
				return;
			}
			sharedMaterials[this.indexM] = this.materials[index];
			this.mesh.sharedMaterials = sharedMaterials;
			this.current = index;
			this.ChangeLOD(index);
			this.OnMaterialChanged.Invoke(this.current);
		}

		// Token: 0x06003BC8 RID: 15304 RVA: 0x001C89BC File Offset: 0x001C6BBC
		public virtual void PreviousMaterial()
		{
			this.current--;
			if (this.current < 0)
			{
				this.current = this.materials.Length - 1;
			}
			if (this.materials[this.current] != null)
			{
				Material[] sharedMaterials = this.mesh.sharedMaterials;
				sharedMaterials[this.indexM] = this.materials[this.current];
				this.mesh.sharedMaterials = sharedMaterials;
				this.ChangeLOD(this.current);
				this.OnMaterialChanged.Invoke(this.current);
				return;
			}
			Debug.LogWarning("The Material on the Slot: " + this.current + " is empty");
		}

		// Token: 0x06003BC9 RID: 15305 RVA: 0x001C8A70 File Offset: 0x001C6C70
		public virtual void ChangeMaterial(Material mat)
		{
			Material[] sharedMaterials = this.mesh.sharedMaterials;
			sharedMaterials[this.indexM] = mat;
			this.mesh.sharedMaterials = sharedMaterials;
			this.ChangeLOD(mat);
		}

		// Token: 0x06003BCA RID: 15306 RVA: 0x001C8AA5 File Offset: 0x001C6CA5
		public virtual void ChangeMaterial(bool Next = true)
		{
			if (Next)
			{
				this.NextMaterial();
				return;
			}
			this.PreviousMaterial();
		}

		// Token: 0x04002B56 RID: 11094
		[SerializeField]
		[HideInInspector]
		public string Name;

		// Token: 0x04002B57 RID: 11095
		public Renderer mesh;

		// Token: 0x04002B58 RID: 11096
		public Material[] materials;

		// Token: 0x04002B59 RID: 11097
		public bool Linked;

		// Token: 0x04002B5A RID: 11098
		[Range(0f, 100f)]
		public int Master;

		// Token: 0x04002B5B RID: 11099
		[HideInInspector]
		[SerializeField]
		public int current;

		// Token: 0x04002B5C RID: 11100
		public bool HasLODs;

		// Token: 0x04002B5D RID: 11101
		public Renderer[] LODs;

		// Token: 0x04002B5E RID: 11102
		[Tooltip("Material ID")]
		public int indexM;

		// Token: 0x04002B5F RID: 11103
		public IntEvent OnMaterialChanged = new IntEvent();
	}
}
