using System;
using UnityEngine.UI;
using XNode.Examples.MathNodes;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000364 RID: 868
	public class UGUIDisplayValue : UGUIMathBaseNode
	{
		// Token: 0x060033E3 RID: 13283 RVA: 0x001A1178 File Offset: 0x0019F378
		private void Update()
		{
			object inputValue = (this.node as DisplayValue).GetInputValue<object>("input", null);
			if (inputValue != null)
			{
				this.label.text = inputValue.ToString();
				return;
			}
			this.label.text = "n/a";
		}

		// Token: 0x040022E6 RID: 8934
		public Text label;
	}
}
