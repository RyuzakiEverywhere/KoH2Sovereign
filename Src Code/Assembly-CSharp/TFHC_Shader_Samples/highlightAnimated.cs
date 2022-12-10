using System;
using UnityEngine;

namespace TFHC_Shader_Samples
{
	// Token: 0x020004E4 RID: 1252
	public class highlightAnimated : MonoBehaviour
	{
		// Token: 0x06004216 RID: 16918 RVA: 0x001F7E63 File Offset: 0x001F6063
		private void Start()
		{
			this.mat = base.GetComponent<Renderer>().material;
		}

		// Token: 0x06004217 RID: 16919 RVA: 0x001F7E76 File Offset: 0x001F6076
		private void OnMouseEnter()
		{
			this.switchhighlighted(true);
		}

		// Token: 0x06004218 RID: 16920 RVA: 0x001F7E7F File Offset: 0x001F607F
		private void OnMouseExit()
		{
			this.switchhighlighted(false);
		}

		// Token: 0x06004219 RID: 16921 RVA: 0x001F7E88 File Offset: 0x001F6088
		private void switchhighlighted(bool highlighted)
		{
			this.mat.SetFloat("_Highlighted", highlighted ? 1f : 0f);
		}

		// Token: 0x04002E06 RID: 11782
		private Material mat;
	}
}
