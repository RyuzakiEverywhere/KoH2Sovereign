using System;
using System.Collections;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x0200040B RID: 1035
	public class SlowMotion : MonoBehaviour
	{
		// Token: 0x06003860 RID: 14432 RVA: 0x001BB96C File Offset: 0x001B9B6C
		private void Update()
		{
			if (this.ISlowMotion.GetInput)
			{
				if (Time.timeScale == 1f)
				{
					base.StartCoroutine(this.SlowTime());
				}
				else
				{
					base.StartCoroutine(this.RestartTime());
				}
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
			}
		}

		// Token: 0x06003861 RID: 14433 RVA: 0x001BB9BE File Offset: 0x001B9BBE
		private IEnumerator SlowTime()
		{
			while (Time.timeScale > this.slowMoTimeScale)
			{
				Time.timeScale = Mathf.Clamp(Time.timeScale - 1f / this.slowMoSpeed * Time.unscaledDeltaTime, 0f, 100f);
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
				yield return null;
			}
			Time.timeScale = this.slowMoTimeScale;
			yield break;
		}

		// Token: 0x06003862 RID: 14434 RVA: 0x001BB9CD File Offset: 0x001B9BCD
		private IEnumerator RestartTime()
		{
			while (Time.timeScale < 1f)
			{
				Time.timeScale += 1f / this.slowMoSpeed * Time.unscaledDeltaTime;
				yield return null;
			}
			Time.timeScale = 1f;
			yield break;
		}

		// Token: 0x040028A9 RID: 10409
		[Space]
		public InputRow ISlowMotion = new InputRow("Fire2", KeyCode.Mouse2, InputButton.Down);

		// Token: 0x040028AA RID: 10410
		[Space]
		[Range(0.05f, 1f)]
		[SerializeField]
		private float slowMoTimeScale = 0.25f;

		// Token: 0x040028AB RID: 10411
		[Range(0.1f, 10f)]
		[SerializeField]
		private float slowMoSpeed = 0.2f;
	}
}
