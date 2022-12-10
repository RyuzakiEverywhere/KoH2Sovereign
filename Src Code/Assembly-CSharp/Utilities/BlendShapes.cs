using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x0200046A RID: 1130
	public class BlendShapes : MonoBehaviour
	{
		// Token: 0x06003B59 RID: 15193 RVA: 0x001C6CEF File Offset: 0x001C4EEF
		private void Awake()
		{
			if (this.random)
			{
				this.RandomShapes();
			}
		}

		// Token: 0x06003B5A RID: 15194 RVA: 0x001C6D00 File Offset: 0x001C4F00
		public virtual void RandomShapes()
		{
			foreach (MeshBlendShapes meshBlendShapes in this.Shapes)
			{
				meshBlendShapes.SetRandom();
			}
		}

		// Token: 0x06003B5B RID: 15195 RVA: 0x001C6D50 File Offset: 0x001C4F50
		public virtual void UpdateBlendShapes()
		{
			foreach (MeshBlendShapes meshBlendShapes in this.Shapes)
			{
				meshBlendShapes.UpdateBlendShapes();
			}
		}

		// Token: 0x06003B5C RID: 15196 RVA: 0x001C6DA0 File Offset: 0x001C4FA0
		public virtual void SetBlendShape(string name, float value)
		{
			foreach (MeshBlendShapes meshBlendShapes in this.Shapes)
			{
				meshBlendShapes.SetBlendShape(name, value);
			}
		}

		// Token: 0x06003B5D RID: 15197 RVA: 0x001C6DF4 File Offset: 0x001C4FF4
		public virtual void SetBlendShape(int index, float value)
		{
			foreach (MeshBlendShapes meshBlendShapes in this.Shapes)
			{
				meshBlendShapes.SetBlendShape(base.name, value);
			}
		}

		// Token: 0x04002B25 RID: 11045
		[SerializeField]
		public List<MeshBlendShapes> Shapes;

		// Token: 0x04002B26 RID: 11046
		public bool random;
	}
}
