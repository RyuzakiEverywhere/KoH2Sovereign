using System;
using System.Collections;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x0200040A RID: 1034
	public class ScaleBonesFix : MonoBehaviour, IAnimatorListener
	{
		// Token: 0x0600385C RID: 14428 RVA: 0x001BB944 File Offset: 0x001B9B44
		public void FixHeight(bool active)
		{
			base.StartCoroutine(this.SmoothFix(active));
		}

		// Token: 0x0600385D RID: 14429 RVA: 0x001AF9E6 File Offset: 0x001ADBE6
		public virtual void OnAnimatorBehaviourMessage(string message, object value)
		{
			this.InvokeWithParams(message, value);
		}

		// Token: 0x0600385E RID: 14430 RVA: 0x001BB954 File Offset: 0x001B9B54
		public IEnumerator SmoothFix(bool active)
		{
			float t = 0f;
			Vector3 startpos = this.fixGameObject.localPosition;
			Vector3 endpos = startpos + (active ? this.Offset : (-this.Offset));
			while (t < this.duration)
			{
				this.fixGameObject.localPosition = Vector3.Lerp(startpos, endpos, t / this.duration);
				t += Time.deltaTime;
				yield return null;
			}
			yield break;
		}

		// Token: 0x040028A6 RID: 10406
		public Transform fixGameObject;

		// Token: 0x040028A7 RID: 10407
		public Vector3 Offset;

		// Token: 0x040028A8 RID: 10408
		public float duration;
	}
}
