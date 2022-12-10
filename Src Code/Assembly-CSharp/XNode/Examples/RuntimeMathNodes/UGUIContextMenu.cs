using System;
using UnityEngine;
using UnityEngine.EventSystems;
using XNode.Examples.MathNodes;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000368 RID: 872
	public class UGUIContextMenu : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
	{
		// Token: 0x060033F9 RID: 13305 RVA: 0x001A15F4 File Offset: 0x0019F7F4
		private void Start()
		{
			this.Close();
		}

		// Token: 0x060033FA RID: 13306 RVA: 0x001A15FC File Offset: 0x0019F7FC
		public void OpenAt(Vector2 pos)
		{
			base.transform.position = pos;
			this.group.alpha = 1f;
			this.group.interactable = true;
			this.group.blocksRaycasts = true;
			base.transform.SetAsLastSibling();
		}

		// Token: 0x060033FB RID: 13307 RVA: 0x001A164D File Offset: 0x0019F84D
		public void Close()
		{
			this.group.alpha = 0f;
			this.group.interactable = false;
			this.group.blocksRaycasts = false;
		}

		// Token: 0x060033FC RID: 13308 RVA: 0x001A1677 File Offset: 0x0019F877
		public void SpawnMathNode()
		{
			this.SpawnNode(typeof(MathNode));
		}

		// Token: 0x060033FD RID: 13309 RVA: 0x001A1689 File Offset: 0x0019F889
		public void SpawnDisplayNode()
		{
			this.SpawnNode(typeof(DisplayValue));
		}

		// Token: 0x060033FE RID: 13310 RVA: 0x001A169B File Offset: 0x0019F89B
		public void SpawnVectorNode()
		{
			this.SpawnNode(typeof(Vector));
		}

		// Token: 0x060033FF RID: 13311 RVA: 0x001A16B0 File Offset: 0x0019F8B0
		private void SpawnNode(Type nodeType)
		{
			Vector2 arg = new Vector2(base.transform.localPosition.x, -base.transform.localPosition.y);
			this.onClickSpawn(nodeType, arg);
		}

		// Token: 0x06003400 RID: 13312 RVA: 0x001A16F2 File Offset: 0x0019F8F2
		public void RemoveNode()
		{
			RuntimeMathGraph componentInParent = base.GetComponentInParent<RuntimeMathGraph>();
			componentInParent.graph.RemoveNode(this.selectedNode);
			componentInParent.Refresh();
			this.Close();
		}

		// Token: 0x06003401 RID: 13313 RVA: 0x001A15F4 File Offset: 0x0019F7F4
		public void OnPointerExit(PointerEventData eventData)
		{
			this.Close();
		}

		// Token: 0x040022F3 RID: 8947
		public Action<Type, Vector2> onClickSpawn;

		// Token: 0x040022F4 RID: 8948
		public CanvasGroup group;

		// Token: 0x040022F5 RID: 8949
		[HideInInspector]
		public Node selectedNode;

		// Token: 0x040022F6 RID: 8950
		private Vector2 pos;
	}
}
