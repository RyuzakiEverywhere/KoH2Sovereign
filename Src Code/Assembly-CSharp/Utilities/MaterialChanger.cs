using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000475 RID: 1141
	public class MaterialChanger : MonoBehaviour
	{
		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06003BAF RID: 15279 RVA: 0x001C8208 File Offset: 0x001C6408
		// (set) Token: 0x06003BAE RID: 15278 RVA: 0x001C81A8 File Offset: 0x001C63A8
		public string AllIndex
		{
			get
			{
				string text = "";
				for (int i = 0; i < this.materialList.Count; i++)
				{
					text = text + this.materialList[i].current.ToString() + " ";
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
				for (int i = 0; i < this.materialList.Count; i++)
				{
					int num;
					if (array.Length > i && int.TryParse(array[i], out num) && num != -1)
					{
						this.materialList[i].ChangeMaterial(num);
					}
				}
			}
		}

		// Token: 0x06003BB0 RID: 15280 RVA: 0x001C8264 File Offset: 0x001C6464
		private void Awake()
		{
			foreach (MaterialItem materialItem in this.materialList)
			{
				if (materialItem.Linked && materialItem.Master >= 0 && materialItem.Master < this.materialList.Count)
				{
					this.materialList[materialItem.Master].OnMaterialChanged.AddListener(new UnityAction<int>(materialItem.ChangeMaterial));
				}
			}
			if (this.random)
			{
				this.Randomize();
			}
		}

		// Token: 0x06003BB1 RID: 15281 RVA: 0x001C830C File Offset: 0x001C650C
		public virtual void Randomize()
		{
			foreach (MaterialItem materialItem in this.materialList)
			{
				materialItem.ChangeMaterial(Random.Range(0, materialItem.materials.Length));
			}
		}

		// Token: 0x06003BB2 RID: 15282 RVA: 0x001C836C File Offset: 0x001C656C
		public virtual void SetAllMaterials(bool Next = true)
		{
			foreach (MaterialItem materialItem in this.materialList)
			{
				materialItem.ChangeMaterial(Next);
			}
		}

		// Token: 0x06003BB3 RID: 15283 RVA: 0x001C83C0 File Offset: 0x001C65C0
		public virtual void SetAllMaterials(int index)
		{
			foreach (MaterialItem materialItem in this.materialList)
			{
				materialItem.ChangeMaterial(index);
			}
		}

		// Token: 0x06003BB4 RID: 15284 RVA: 0x001C8414 File Offset: 0x001C6614
		public virtual void SetMaterial(int indexList, int indexCurrent)
		{
			if (indexList < 0)
			{
				indexList = 0;
			}
			indexList %= this.materialList.Count;
			if (this.materialList[indexList] != null)
			{
				this.materialList[indexList].ChangeMaterial(indexCurrent);
			}
		}

		// Token: 0x06003BB5 RID: 15285 RVA: 0x001C844C File Offset: 0x001C664C
		public virtual void SetMaterial(int index, bool next = true)
		{
			if (index < 0)
			{
				index = 0;
			}
			index %= this.materialList.Count;
			if (this.materialList[index] != null)
			{
				this.materialList[index].ChangeMaterial(next);
			}
		}

		// Token: 0x06003BB6 RID: 15286 RVA: 0x001C8484 File Offset: 0x001C6684
		public virtual void SetMaterial(string name, int Index)
		{
			MaterialItem materialItem = this.materialList.Find((MaterialItem item) => item.Name == name);
			if (materialItem != null)
			{
				materialItem.ChangeMaterial(Index);
				return;
			}
			Debug.LogWarning("No material Item Found with the name: " + name);
		}

		// Token: 0x06003BB7 RID: 15287 RVA: 0x001C84D8 File Offset: 0x001C66D8
		public virtual void SetMaterial(string name, bool next = true)
		{
			MaterialItem materialItem = this.materialList.Find((MaterialItem item) => item.Name == name);
			if (materialItem != null)
			{
				materialItem.ChangeMaterial(next);
				return;
			}
			Debug.LogWarning("No material Item Found with the name: " + name);
		}

		// Token: 0x06003BB8 RID: 15288 RVA: 0x001C852C File Offset: 0x001C672C
		public virtual void SetAllMaterials(Material mat)
		{
			foreach (MaterialItem materialItem in this.materialList)
			{
				materialItem.ChangeMaterial(mat);
			}
		}

		// Token: 0x06003BB9 RID: 15289 RVA: 0x001C8580 File Offset: 0x001C6780
		public virtual void NextMaterialItem(int index)
		{
			if (index < 0)
			{
				index = 0;
			}
			index %= this.materialList.Count;
			this.materialList[index].NextMaterial();
		}

		// Token: 0x06003BBA RID: 15290 RVA: 0x001C85AC File Offset: 0x001C67AC
		public virtual void NextMaterialItem(string name)
		{
			MaterialItem materialItem = this.materialList.Find((MaterialItem item) => item.Name.ToUpperInvariant() == name.ToUpperInvariant());
			if (materialItem != null)
			{
				materialItem.NextMaterial();
			}
		}

		// Token: 0x06003BBB RID: 15291 RVA: 0x001C85E7 File Offset: 0x001C67E7
		public virtual int CurrentMaterialIndex(int index)
		{
			return this.materialList[index].current;
		}

		// Token: 0x06003BBC RID: 15292 RVA: 0x001C85FC File Offset: 0x001C67FC
		public virtual int CurrentMaterialIndex(string name)
		{
			int index = this.materialList.FindIndex((MaterialItem item) => item.Name == name);
			return this.materialList[index].current;
		}

		// Token: 0x04002B52 RID: 11090
		[SerializeField]
		public List<MaterialItem> materialList = new List<MaterialItem>();

		// Token: 0x04002B53 RID: 11091
		[HideInInspector]
		[SerializeField]
		public bool showMeshesList = true;

		// Token: 0x04002B54 RID: 11092
		public bool random;

		// Token: 0x04002B55 RID: 11093
		private MaterialItem Active;
	}
}
