using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000426 RID: 1062
	[RequireComponent(typeof(WagonController))]
	public class MountableCarriage : Mountable
	{
		// Token: 0x17000395 RID: 917
		// (get) Token: 0x0600392D RID: 14637 RVA: 0x001BE1E8 File Offset: 0x001BC3E8
		public override Animal Animal
		{
			get
			{
				if (!this._animal)
				{
					WagonController component = base.GetComponent<WagonController>();
					if (component.HorseRigidBody == null)
					{
						return null;
					}
					PullingHorses component2 = component.HorseRigidBody.GetComponent<PullingHorses>();
					if (component2)
					{
						this._animal = component2.RightHorse.GetComponent<Animal>();
					}
					else
					{
						this._animal = component.HorseRigidBody.GetComponent<Animal>();
					}
				}
				return this._animal;
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x0600392E RID: 14638 RVA: 0x001BE257 File Offset: 0x001BC457
		public override bool CanDismount
		{
			get
			{
				return !(this.Animal != null) || this.Animal.Stand;
			}
		}

		// Token: 0x0600392F RID: 14639 RVA: 0x001BE274 File Offset: 0x001BC474
		public override void EnableControls(bool value)
		{
			WagonController component = base.GetComponent<WagonController>();
			if (component.HorseRigidBody == null)
			{
				return;
			}
			PullingHorses component2 = component.HorseRigidBody.GetComponent<PullingHorses>();
			if (component2)
			{
				if (component2.RightHorse)
				{
					component2.RightHorse.MovementAxis = Vector3.zero;
					component2.RightHorse.GetComponent<MalbersInput>().enabled = value;
				}
				if (component2.LeftHorse)
				{
					component2.RightHorse.MovementAxis = Vector3.zero;
					component2.LeftHorse.GetComponent<MalbersInput>().enabled = value;
					return;
				}
			}
			else
			{
				component.HorseRigidBody.GetComponent<Animal>().GetComponent<MalbersInput>().enabled = value;
			}
		}

		// Token: 0x04002950 RID: 10576
		protected Transform hip;
	}
}
