using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000365 RID: 869
	public class UGUIMathBaseNode : MonoBehaviour, IDragHandler, IEventSystemHandler
	{
		// Token: 0x060033E5 RID: 13285 RVA: 0x001A11CC File Offset: 0x0019F3CC
		public virtual void Start()
		{
			this.ports = base.GetComponentsInChildren<UGUIPort>();
			UGUIPort[] array = this.ports;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].node = this.node;
			}
			this.header.text = this.node.name;
			this.SetPosition(this.node.position);
		}

		// Token: 0x060033E6 RID: 13286 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void UpdateGUI()
		{
		}

		// Token: 0x060033E7 RID: 13287 RVA: 0x001A1230 File Offset: 0x0019F430
		private void LateUpdate()
		{
			UGUIPort[] array = this.ports;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateConnectionTransforms();
			}
		}

		// Token: 0x060033E8 RID: 13288 RVA: 0x001A125C File Offset: 0x0019F45C
		public UGUIPort GetPort(string name)
		{
			for (int i = 0; i < this.ports.Length; i++)
			{
				if (this.ports[i].name == name)
				{
					return this.ports[i];
				}
			}
			return null;
		}

		// Token: 0x060033E9 RID: 13289 RVA: 0x001A129B File Offset: 0x0019F49B
		public void SetPosition(Vector2 pos)
		{
			pos.y = -pos.y;
			base.transform.localPosition = pos;
		}

		// Token: 0x060033EA RID: 13290 RVA: 0x001A12BC File Offset: 0x0019F4BC
		public void SetName(string name)
		{
			this.header.text = name;
		}

		// Token: 0x060033EB RID: 13291 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnDrag(PointerEventData eventData)
		{
		}

		// Token: 0x040022E7 RID: 8935
		[HideInInspector]
		public Node node;

		// Token: 0x040022E8 RID: 8936
		[HideInInspector]
		public RuntimeMathGraph graph;

		// Token: 0x040022E9 RID: 8937
		public Text header;

		// Token: 0x040022EA RID: 8938
		private UGUIPort[] ports;
	}
}
