using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000362 RID: 866
	public class NodeDrag : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		// Token: 0x060033D2 RID: 13266 RVA: 0x001A0D90 File Offset: 0x0019EF90
		private void Awake()
		{
			this.node = base.GetComponentInParent<UGUIMathBaseNode>();
		}

		// Token: 0x060033D3 RID: 13267 RVA: 0x001A0DA0 File Offset: 0x0019EFA0
		public void OnDrag(PointerEventData eventData)
		{
			this.node.transform.localPosition = this.node.graph.scrollRect.content.InverseTransformPoint(eventData.position) - this.offset;
		}

		// Token: 0x060033D4 RID: 13268 RVA: 0x001A0DF0 File Offset: 0x0019EFF0
		public void OnBeginDrag(PointerEventData eventData)
		{
			Vector2 a = this.node.graph.scrollRect.content.InverseTransformPoint(eventData.position);
			Vector2 b = this.node.transform.localPosition;
			this.offset = a - b;
		}

		// Token: 0x060033D5 RID: 13269 RVA: 0x001A0E50 File Offset: 0x0019F050
		public void OnEndDrag(PointerEventData eventData)
		{
			this.node.transform.localPosition = this.node.graph.scrollRect.content.InverseTransformPoint(eventData.position) - this.offset;
			Vector2 vector = this.node.transform.localPosition;
			vector.y = -vector.y;
			this.node.node.position = vector;
		}

		// Token: 0x060033D6 RID: 13270 RVA: 0x001A0ED4 File Offset: 0x0019F0D4
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Right)
			{
				return;
			}
			this.node.graph.nodeContextMenu.selectedNode = this.node.node;
			this.node.graph.nodeContextMenu.OpenAt(eventData.position);
		}

		// Token: 0x040022DA RID: 8922
		private Vector3 offset;

		// Token: 0x040022DB RID: 8923
		private UGUIMathBaseNode node;
	}
}
