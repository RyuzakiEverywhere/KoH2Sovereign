using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000497 RID: 1175
	public class CM_RandomEyesTriggerTracking : MonoBehaviour
	{
		// Token: 0x06003DF9 RID: 15865 RVA: 0x001DAD38 File Offset: 0x001D8F38
		private void Start()
		{
			this.randomEyes2D = base.GetComponent<RandomEyes2D>();
			if (this.randomEyes2D)
			{
				this.randomEyes = this.randomEyes2D.gameObject;
			}
			this.randomEyes3D = base.GetComponent<RandomEyes3D>();
			if (this.randomEyes3D)
			{
				this.randomEyes = this.randomEyes3D.gameObject;
			}
		}

		// Token: 0x06003DFA RID: 15866 RVA: 0x001DAD9C File Offset: 0x001D8F9C
		private void OnTriggerEnter(Collider col)
		{
			if (this.randomEyes2D)
			{
				this.randomEyes2D.SetLookTarget(col.gameObject);
			}
			if (this.randomEyes3D)
			{
				this.randomEyes3D.SetLookTarget(col.gameObject);
			}
			if (this.emitDebug)
			{
				Debug.Log(this.randomEyes.name + " OnTriggerEnter2D triggered");
			}
		}

		// Token: 0x06003DFB RID: 15867 RVA: 0x001DAE08 File Offset: 0x001D9008
		private void OnTriggerExit(Collider col)
		{
			if (this.randomEyes2D)
			{
				this.randomEyes2D.SetLookTarget(null);
			}
			if (this.randomEyes3D)
			{
				this.randomEyes3D.SetLookTarget(null);
			}
			if (this.emitDebug)
			{
				Debug.Log(this.randomEyes.name + " OnTriggerExit2D triggered");
			}
		}

		// Token: 0x04002C06 RID: 11270
		public GameObject lookTarget;

		// Token: 0x04002C07 RID: 11271
		public bool emitDebug = true;

		// Token: 0x04002C08 RID: 11272
		private RandomEyes2D randomEyes2D;

		// Token: 0x04002C09 RID: 11273
		private RandomEyes3D randomEyes3D;

		// Token: 0x04002C0A RID: 11274
		private GameObject randomEyes;
	}
}
