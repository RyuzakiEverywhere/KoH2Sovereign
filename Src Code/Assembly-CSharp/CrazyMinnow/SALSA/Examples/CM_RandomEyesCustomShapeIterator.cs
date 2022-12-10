using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000496 RID: 1174
	public class CM_RandomEyesCustomShapeIterator : MonoBehaviour
	{
		// Token: 0x06003DF6 RID: 15862 RVA: 0x001DACB5 File Offset: 0x001D8EB5
		private void Start()
		{
			if (!this.randomEyes3D)
			{
				this.randomEyes3D = base.GetComponent<RandomEyes3D>();
			}
		}

		// Token: 0x06003DF7 RID: 15863 RVA: 0x001DACD0 File Offset: 0x001D8ED0
		private void RandomEyes_OnCustomShapeChanged(RandomEyesCustomShapeStatus customShape)
		{
			if (customShape.isOn)
			{
				if (this.customIndex < this.randomEyes3D.customShapes.Length - 1)
				{
					this.customIndex++;
				}
				else
				{
					this.customIndex = 0;
				}
				this.randomEyes3D.SetCustomShape(this.randomEyes3D.customShapes[this.customIndex].shapeName);
			}
		}

		// Token: 0x04002C04 RID: 11268
		public RandomEyes3D randomEyes3D;

		// Token: 0x04002C05 RID: 11269
		private int customIndex;
	}
}
