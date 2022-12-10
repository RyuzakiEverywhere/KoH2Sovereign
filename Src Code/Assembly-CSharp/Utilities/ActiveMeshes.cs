using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000464 RID: 1124
	public class ActiveMeshes : MonoBehaviour
	{
		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06003B2B RID: 15147 RVA: 0x001C621C File Offset: 0x001C441C
		// (set) Token: 0x06003B2A RID: 15146 RVA: 0x001C61BC File Offset: 0x001C43BC
		public string AllIndex
		{
			get
			{
				string text = "";
				for (int i = 0; i < this.Meshes.Count; i++)
				{
					text = text + this.Meshes[i].Current.ToString() + " ";
				}
				text.Remove(text.Length - 1);
				return text;
			}
			set
			{
				string[] array = value.Split(new char[]
				{
					' '
				});
				for (int i = 0; i < this.Meshes.Count; i++)
				{
					int num;
					if (array.Length > i && int.TryParse(array[i], out num) && num != -1)
					{
						this.Meshes[i].ChangeMesh(num);
					}
				}
			}
		}

		// Token: 0x06003B2C RID: 15148 RVA: 0x001C6278 File Offset: 0x001C4478
		private void Awake()
		{
			if (this.random)
			{
				foreach (ActiveSMesh activeSMesh in this.Meshes)
				{
					activeSMesh.ChangeMesh(Random.Range(0, activeSMesh.meshes.Length));
				}
			}
		}

		// Token: 0x06003B2D RID: 15149 RVA: 0x001C62E0 File Offset: 0x001C44E0
		public void SetActiveMeshesIndex(int[] MeshesIndex)
		{
			if (MeshesIndex.Length != this.Meshes.Count)
			{
				Debug.LogError("Meshes Index array Lenghts don't match");
				return;
			}
			for (int i = 0; i < MeshesIndex.Length; i++)
			{
				this.Meshes[i].ChangeMesh(MeshesIndex[i]);
			}
		}

		// Token: 0x06003B2E RID: 15150 RVA: 0x001C632A File Offset: 0x001C452A
		public virtual void ChangeMesh(int index)
		{
			if (this.Meshes.Count > index)
			{
				this.Meshes[index].ChangeMesh(true);
			}
		}

		// Token: 0x06003B2F RID: 15151 RVA: 0x001C634C File Offset: 0x001C454C
		public virtual void ChangeMesh(int indexList, int IndexMesh)
		{
			if (indexList < 0)
			{
				indexList = 0;
			}
			indexList %= this.Meshes.Count;
			if (this.Meshes[indexList] != null)
			{
				this.Meshes[indexList].ChangeMesh(IndexMesh);
			}
		}

		// Token: 0x06003B30 RID: 15152 RVA: 0x001C6384 File Offset: 0x001C4584
		public virtual void ChangeMesh(string name, bool next)
		{
			ActiveSMesh activeSMesh = this.Meshes.Find((ActiveSMesh item) => item.Name == name);
			if (activeSMesh != null)
			{
				activeSMesh.ChangeMesh(next);
			}
		}

		// Token: 0x06003B31 RID: 15153 RVA: 0x001C63C0 File Offset: 0x001C45C0
		public virtual void ChangeMesh(string name)
		{
			this.ChangeMesh(name, true);
		}

		// Token: 0x06003B32 RID: 15154 RVA: 0x001C63CC File Offset: 0x001C45CC
		public virtual void ChangeMesh(string name, int CurrentIndex)
		{
			ActiveSMesh activeSMesh = this.Meshes.Find((ActiveSMesh item) => item.Name == name);
			if (activeSMesh != null)
			{
				activeSMesh.ChangeMesh(CurrentIndex);
			}
		}

		// Token: 0x06003B33 RID: 15155 RVA: 0x001C6408 File Offset: 0x001C4608
		public virtual void ChangeMesh(int index, bool next)
		{
			this.Meshes[index].ChangeMesh(next);
		}

		// Token: 0x06003B34 RID: 15156 RVA: 0x001C641C File Offset: 0x001C461C
		public virtual void ChangeMesh(bool next = true)
		{
			foreach (ActiveSMesh activeSMesh in this.Meshes)
			{
				activeSMesh.ChangeMesh(next);
			}
		}

		// Token: 0x06003B35 RID: 15157 RVA: 0x001C6470 File Offset: 0x001C4670
		public virtual ActiveSMesh GetActiveMesh(string name)
		{
			if (this.Meshes.Count == 0)
			{
				return null;
			}
			return this.Meshes.Find((ActiveSMesh item) => item.Name == name);
		}

		// Token: 0x06003B36 RID: 15158 RVA: 0x001C64B0 File Offset: 0x001C46B0
		public virtual ActiveSMesh GetActiveMesh(int index)
		{
			if (this.Meshes.Count == 0)
			{
				return null;
			}
			if (index >= this.Meshes.Count)
			{
				index = 0;
			}
			if (index < 0)
			{
				index = this.Meshes.Count - 1;
			}
			return this.Meshes[index];
		}

		// Token: 0x04002B0E RID: 11022
		[SerializeField]
		public List<ActiveSMesh> Meshes = new List<ActiveSMesh>();

		// Token: 0x04002B0F RID: 11023
		[HideInInspector]
		[SerializeField]
		public bool showMeshesList = true;

		// Token: 0x04002B10 RID: 11024
		public bool random;
	}
}
