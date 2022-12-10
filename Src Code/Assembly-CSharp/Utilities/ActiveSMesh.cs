using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000465 RID: 1125
	[Serializable]
	public class ActiveSMesh
	{
		// Token: 0x06003B38 RID: 15160 RVA: 0x001C6518 File Offset: 0x001C4718
		public virtual void ChangeMesh(bool next = true)
		{
			if (next)
			{
				this.Current++;
			}
			else
			{
				this.Current--;
			}
			if (this.Current >= this.meshes.Length)
			{
				this.Current = 0;
			}
			if (this.Current < 0)
			{
				this.Current = this.meshes.Length - 1;
			}
			foreach (Transform transform in this.meshes)
			{
				if (transform)
				{
					transform.gameObject.SetActive(false);
				}
			}
			if (this.meshes[this.Current])
			{
				this.meshes[this.Current].gameObject.SetActive(true);
				this.OnActiveMesh.Invoke(this.meshes[this.Current]);
			}
		}

		// Token: 0x06003B39 RID: 15161 RVA: 0x001C65E7 File Offset: 0x001C47E7
		public virtual Transform GetCurrentActiveMesh()
		{
			return this.meshes[this.Current];
		}

		// Token: 0x06003B3A RID: 15162 RVA: 0x001C65F6 File Offset: 0x001C47F6
		public virtual void ChangeMesh(int Index)
		{
			this.Current = Index - 1;
			this.ChangeMesh(true);
		}

		// Token: 0x06003B3B RID: 15163 RVA: 0x001C6608 File Offset: 0x001C4808
		public void Set_by_BinaryIndex(int binaryCurrent)
		{
			int index = 0;
			for (int i = 0; i < this.meshes.Length; i++)
			{
				if (MalbersTools.IsBitActive(binaryCurrent, i))
				{
					index = i;
					break;
				}
			}
			this.ChangeMesh(index);
		}

		// Token: 0x04002B11 RID: 11025
		[HideInInspector]
		public string Name = "NameHere";

		// Token: 0x04002B12 RID: 11026
		public Transform[] meshes;

		// Token: 0x04002B13 RID: 11027
		[HideInInspector]
		[SerializeField]
		public int Current;

		// Token: 0x04002B14 RID: 11028
		[Space]
		[Header("Invoked when the Active mesh is changed")]
		public TransformEvent OnActiveMesh = new TransformEvent();
	}
}
