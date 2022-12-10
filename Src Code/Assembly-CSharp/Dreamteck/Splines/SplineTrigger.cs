using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamteck.Splines
{
	// Token: 0x020004D2 RID: 1234
	[Serializable]
	public class SplineTrigger
	{
		// Token: 0x14000049 RID: 73
		// (add) Token: 0x06004196 RID: 16790 RVA: 0x001F2FB4 File Offset: 0x001F11B4
		// (remove) Token: 0x06004197 RID: 16791 RVA: 0x001F2FEC File Offset: 0x001F11EC
		public event Action<SplineUser> onUserCross;

		// Token: 0x06004198 RID: 16792 RVA: 0x001F3024 File Offset: 0x001F1224
		public SplineTrigger(SplineTrigger.Type t)
		{
			this.type = t;
			this.enabled = true;
			this.onCross = new UnityEvent();
		}

		// Token: 0x06004199 RID: 16793 RVA: 0x001F3087 File Offset: 0x001F1287
		public void AddListener(UnityAction action)
		{
			this.onCross.AddListener(action);
		}

		// Token: 0x0600419A RID: 16794 RVA: 0x001F3095 File Offset: 0x001F1295
		public void Reset()
		{
			this.worked = false;
		}

		// Token: 0x0600419B RID: 16795 RVA: 0x001F30A0 File Offset: 0x001F12A0
		public bool Check(double previousPercent, double currentPercent)
		{
			if (!this.enabled)
			{
				return false;
			}
			if (this.workOnce && this.worked)
			{
				return false;
			}
			bool flag = false;
			switch (this.type)
			{
			case SplineTrigger.Type.Double:
				flag = ((previousPercent <= this.position && currentPercent >= this.position) || (currentPercent <= this.position && previousPercent >= this.position));
				break;
			case SplineTrigger.Type.Forward:
				flag = (previousPercent <= this.position && currentPercent >= this.position);
				break;
			case SplineTrigger.Type.Backward:
				flag = (currentPercent <= this.position && previousPercent >= this.position);
				break;
			}
			if (flag)
			{
				this.worked = true;
			}
			return flag;
		}

		// Token: 0x0600419C RID: 16796 RVA: 0x001F3155 File Offset: 0x001F1355
		public void Invoke(SplineUser user = null)
		{
			this.onCross.Invoke();
			if (user && this.onUserCross != null)
			{
				this.onUserCross(user);
			}
		}

		// Token: 0x04002DA8 RID: 11688
		public string name = "Trigger";

		// Token: 0x04002DA9 RID: 11689
		[SerializeField]
		public SplineTrigger.Type type;

		// Token: 0x04002DAA RID: 11690
		public bool workOnce;

		// Token: 0x04002DAB RID: 11691
		private bool worked;

		// Token: 0x04002DAC RID: 11692
		[Range(0f, 1f)]
		public double position = 0.5;

		// Token: 0x04002DAD RID: 11693
		[SerializeField]
		public bool enabled = true;

		// Token: 0x04002DAE RID: 11694
		[SerializeField]
		public Color color = Color.white;

		// Token: 0x04002DAF RID: 11695
		[SerializeField]
		[HideInInspector]
		public UnityEvent onCross = new UnityEvent();

		// Token: 0x020009B4 RID: 2484
		public enum Type
		{
			// Token: 0x0400452A RID: 17706
			Double,
			// Token: 0x0400452B RID: 17707
			Forward,
			// Token: 0x0400452C RID: 17708
			Backward
		}
	}
}
