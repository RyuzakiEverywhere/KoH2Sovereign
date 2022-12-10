using System;
using UnityEngine.Events;
using UnityEngine.UI;
using XNode.Examples.MathNodes;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000367 RID: 871
	public class UGUIVector : UGUIMathBaseNode
	{
		// Token: 0x060033F3 RID: 13299 RVA: 0x001A1444 File Offset: 0x0019F644
		public override void Start()
		{
			base.Start();
			this.vectorNode = (this.node as Vector);
			this.valX.onValueChanged.AddListener(new UnityAction<string>(this.OnChangeValX));
			this.valY.onValueChanged.AddListener(new UnityAction<string>(this.OnChangeValY));
			this.valZ.onValueChanged.AddListener(new UnityAction<string>(this.OnChangeValZ));
			this.UpdateGUI();
		}

		// Token: 0x060033F4 RID: 13300 RVA: 0x001A14C4 File Offset: 0x0019F6C4
		public override void UpdateGUI()
		{
			NodePort inputPort = this.node.GetInputPort("x");
			NodePort inputPort2 = this.node.GetInputPort("y");
			NodePort inputPort3 = this.node.GetInputPort("z");
			this.valX.gameObject.SetActive(!inputPort.IsConnected);
			this.valY.gameObject.SetActive(!inputPort2.IsConnected);
			this.valZ.gameObject.SetActive(!inputPort3.IsConnected);
			Vector vector = this.node as Vector;
			this.valX.text = vector.x.ToString();
			this.valY.text = vector.y.ToString();
			this.valZ.text = vector.z.ToString();
		}

		// Token: 0x060033F5 RID: 13301 RVA: 0x001A159D File Offset: 0x0019F79D
		private void OnChangeValX(string val)
		{
			this.vectorNode.x = float.Parse(this.valX.text);
		}

		// Token: 0x060033F6 RID: 13302 RVA: 0x001A15BA File Offset: 0x0019F7BA
		private void OnChangeValY(string val)
		{
			this.vectorNode.y = float.Parse(this.valY.text);
		}

		// Token: 0x060033F7 RID: 13303 RVA: 0x001A15D7 File Offset: 0x0019F7D7
		private void OnChangeValZ(string val)
		{
			this.vectorNode.z = float.Parse(this.valZ.text);
		}

		// Token: 0x040022EF RID: 8943
		public InputField valX;

		// Token: 0x040022F0 RID: 8944
		public InputField valY;

		// Token: 0x040022F1 RID: 8945
		public InputField valZ;

		// Token: 0x040022F2 RID: 8946
		private Vector vectorNode;
	}
}
