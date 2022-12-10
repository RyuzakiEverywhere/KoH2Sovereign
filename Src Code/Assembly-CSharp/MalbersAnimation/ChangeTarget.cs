using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003E4 RID: 996
	public class ChangeTarget : MonoBehaviour
	{
		// Token: 0x06003795 RID: 14229 RVA: 0x001B8524 File Offset: 0x001B6724
		private void Start()
		{
			if (this.NoInputs)
			{
				for (int i = 0; i < this.targets.Length; i++)
				{
					if (this.targets[i])
					{
						MalbersInput component = this.targets[i].GetComponent<MalbersInput>();
						if (component)
						{
							component.enabled = false;
						}
					}
				}
				this.m = base.GetComponent<MFreeLookCamera>();
				if (this.m)
				{
					MalbersInput component = this.m.Target.GetComponent<MalbersInput>();
					if (component)
					{
						component.enabled = true;
					}
					for (int j = 0; j < this.targets.Length; j++)
					{
						if (this.targets[j] == this.m.Target)
						{
							this.current = j;
							return;
						}
					}
				}
			}
		}

		// Token: 0x06003796 RID: 14230 RVA: 0x001B85EC File Offset: 0x001B67EC
		private void Update()
		{
			if (this.targets.Length == 0)
			{
				return;
			}
			if (this.targets.Length > this.current && this.targets[this.current] == null)
			{
				return;
			}
			if (Input.GetKeyDown(this.key))
			{
				if (this.NoInputs)
				{
					MalbersInput component = this.targets[this.current].GetComponent<MalbersInput>();
					if (component)
					{
						component.enabled = false;
					}
				}
				this.current++;
				this.current %= this.targets.Length;
				base.SendMessage("SetTarget", this.targets[this.current]);
				if (this.NoInputs)
				{
					MalbersInput component2 = this.targets[this.current].GetComponent<MalbersInput>();
					if (component2)
					{
						component2.enabled = true;
					}
				}
			}
		}

		// Token: 0x040027C6 RID: 10182
		public Transform[] targets;

		// Token: 0x040027C7 RID: 10183
		public KeyCode key = KeyCode.T;

		// Token: 0x040027C8 RID: 10184
		private int current;

		// Token: 0x040027C9 RID: 10185
		[Tooltip("Deactivate the Inputs of the other targets to keep them from moving")]
		public bool NoInputs;

		// Token: 0x040027CA RID: 10186
		private MFreeLookCamera m;
	}
}
