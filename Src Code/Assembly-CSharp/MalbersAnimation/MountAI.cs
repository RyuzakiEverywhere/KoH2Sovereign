using System;
using MalbersAnimations.HAP;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x02000415 RID: 1045
	public class MountAI : AnimalAIControl, IMountAI
	{
		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06003889 RID: 14473 RVA: 0x001BC172 File Offset: 0x001BA372
		// (set) Token: 0x0600388A RID: 14474 RVA: 0x001BC17A File Offset: 0x001BA37A
		public bool CanBeCalled
		{
			get
			{
				return this.canBeCalled;
			}
			set
			{
				this.canBeCalled = value;
			}
		}

		// Token: 0x0600388B RID: 14475 RVA: 0x001BC183 File Offset: 0x001BA383
		private void Start()
		{
			this.animalMount = base.GetComponent<Mountable>();
			this.StartAgent();
		}

		// Token: 0x0600388C RID: 14476 RVA: 0x001BC198 File Offset: 0x001BA398
		private void Update()
		{
			if (this.animalMount.Mounted)
			{
				base.Agent.enabled = false;
				return;
			}
			base.Agent.nextPosition = base.Agent.transform.position;
			if (!base.Agent.isOnNavMesh || !base.Agent.enabled)
			{
				return;
			}
			if (this.isBeingCalled)
			{
				base.Agent.SetDestination(this.target.position);
			}
			this.UpdateAgent();
		}

		// Token: 0x0600388D RID: 14477 RVA: 0x001BC21A File Offset: 0x001BA41A
		protected override void OnAnimationChanged(int animTag)
		{
			if (this.animalMount.Mounted)
			{
				return;
			}
			base.OnAnimationChanged(animTag);
		}

		// Token: 0x0600388E RID: 14478 RVA: 0x001BC234 File Offset: 0x001BA434
		public virtual void CallAnimal(Transform target, bool call)
		{
			if (!this.CanBeCalled)
			{
				return;
			}
			if (!base.Agent || !base.Agent.isOnNavMesh)
			{
				return;
			}
			this.isBeingCalled = call;
			if (this.isBeingCalled)
			{
				base.Agent.enabled = true;
				this.SetTarget(target);
				base.Agent.isStopped = false;
				return;
			}
			base.Agent.enabled = true;
			base.Agent.isStopped = true;
		}

		// Token: 0x040028E6 RID: 10470
		public bool canBeCalled;

		// Token: 0x040028E7 RID: 10471
		protected Mountable animalMount;

		// Token: 0x040028E8 RID: 10472
		protected bool isBeingCalled;
	}
}
