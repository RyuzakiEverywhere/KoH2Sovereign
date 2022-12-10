using System;
using System.Collections;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x02000454 RID: 1108
	[Serializable]
	public class MLerpControlledBob
	{
		// Token: 0x06003ACE RID: 15054 RVA: 0x001C4369 File Offset: 0x001C2569
		public float Offset()
		{
			return this.m_Offset;
		}

		// Token: 0x06003ACF RID: 15055 RVA: 0x001C4371 File Offset: 0x001C2571
		public IEnumerator DoBobCycle()
		{
			float t = 0f;
			while (t < this.BobDuration)
			{
				this.m_Offset = Mathf.Lerp(0f, this.BobAmount, t / this.BobDuration);
				t += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}
			t = 0f;
			while (t < this.BobDuration)
			{
				this.m_Offset = Mathf.Lerp(this.BobAmount, 0f, t / this.BobDuration);
				t += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}
			this.m_Offset = 0f;
			yield break;
		}

		// Token: 0x04002A91 RID: 10897
		public float BobDuration;

		// Token: 0x04002A92 RID: 10898
		public float BobAmount;

		// Token: 0x04002A93 RID: 10899
		private float m_Offset;
	}
}
