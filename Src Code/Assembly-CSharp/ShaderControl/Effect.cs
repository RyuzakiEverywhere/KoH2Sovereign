using System;
using UnityEngine;

namespace ShaderControl
{
	// Token: 0x0200048B RID: 1163
	[ExecuteInEditMode]
	public class Effect : MonoBehaviour
	{
		// Token: 0x06003DD3 RID: 15827 RVA: 0x001D92A5 File Offset: 0x001D74A5
		private void Start()
		{
			this.mat = Resources.Load<Material>("ChromaScreen");
		}

		// Token: 0x06003DD4 RID: 15828 RVA: 0x001D92B7 File Offset: 0x001D74B7
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.mat.shaderKeywords = this.keywords;
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x04002BC0 RID: 11200
		private Material mat;

		// Token: 0x04002BC1 RID: 11201
		private string[] keywords = new string[]
		{
			"ENABLE_RED_CHANNEL",
			"ENABLE_GREEN_CHANNEL",
			"ENABLE_BLUE_CHANNEL"
		};
	}
}
