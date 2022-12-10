using System;
using UnityEngine.Events;
using UnityEngine.UI;
using XNode.Examples.MathNodes;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000366 RID: 870
	public class UGUIMathNode : UGUIMathBaseNode
	{
		// Token: 0x060033ED RID: 13293 RVA: 0x001A12CC File Offset: 0x0019F4CC
		public override void Start()
		{
			base.Start();
			this.mathNode = (this.node as MathNode);
			this.valA.onValueChanged.AddListener(new UnityAction<string>(this.OnChangeValA));
			this.valB.onValueChanged.AddListener(new UnityAction<string>(this.OnChangeValB));
			this.dropDown.onValueChanged.AddListener(new UnityAction<int>(this.OnChangeDropdown));
			this.UpdateGUI();
		}

		// Token: 0x060033EE RID: 13294 RVA: 0x001A134C File Offset: 0x0019F54C
		public override void UpdateGUI()
		{
			NodePort inputPort = this.node.GetInputPort("a");
			NodePort inputPort2 = this.node.GetInputPort("b");
			this.valA.gameObject.SetActive(!inputPort.IsConnected);
			this.valB.gameObject.SetActive(!inputPort2.IsConnected);
			this.valA.text = this.mathNode.a.ToString();
			this.valB.text = this.mathNode.b.ToString();
			this.dropDown.value = (int)this.mathNode.mathType;
		}

		// Token: 0x060033EF RID: 13295 RVA: 0x001A13F9 File Offset: 0x0019F5F9
		private void OnChangeValA(string val)
		{
			this.mathNode.a = float.Parse(this.valA.text);
		}

		// Token: 0x060033F0 RID: 13296 RVA: 0x001A1416 File Offset: 0x0019F616
		private void OnChangeValB(string val)
		{
			this.mathNode.b = float.Parse(this.valB.text);
		}

		// Token: 0x060033F1 RID: 13297 RVA: 0x001A1433 File Offset: 0x0019F633
		private void OnChangeDropdown(int val)
		{
			this.mathNode.mathType = (MathNode.MathType)val;
		}

		// Token: 0x040022EB RID: 8939
		public InputField valA;

		// Token: 0x040022EC RID: 8940
		public InputField valB;

		// Token: 0x040022ED RID: 8941
		public Dropdown dropDown;

		// Token: 0x040022EE RID: 8942
		private MathNode mathNode;
	}
}
