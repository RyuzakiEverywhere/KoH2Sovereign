using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x0200042C RID: 1068
	public class MountTriggers : MonoBehaviour
	{
		// Token: 0x0600395B RID: 14683 RVA: 0x001BEEA6 File Offset: 0x001BD0A6
		private void Awake()
		{
			this.Montura = base.GetComponentInParent<Mountable>();
		}

		// Token: 0x0600395C RID: 14684 RVA: 0x001BEEB4 File Offset: 0x001BD0B4
		private void OnTriggerEnter(Collider other)
		{
			this.GetAnimal(other);
		}

		// Token: 0x0600395D RID: 14685 RVA: 0x001BEEC0 File Offset: 0x001BD0C0
		private void GetAnimal(Collider other)
		{
			if (!this.Montura)
			{
				Debug.LogError("No Mountable Script Found... please add one");
				return;
			}
			if (!this.Montura.Mounted && this.Montura.CanBeMounted)
			{
				this.rider = other.GetComponentInChildren<Rider>();
				if (this.rider == null)
				{
					this.rider = other.GetComponentInParent<Rider>();
				}
				if (this.rider != null)
				{
					if (this.rider.IsRiding)
					{
						return;
					}
					this.rider.Montura = this.Montura;
					this.rider.MountTrigger = this;
					this.rider.OnFindMount.Invoke(base.transform.root.gameObject);
					this.Montura.OnCanBeMounted.Invoke(true);
					this.Montura.NearbyRider = true;
				}
			}
		}

		// Token: 0x0600395E RID: 14686 RVA: 0x001BEFA4 File Offset: 0x001BD1A4
		private void OnTriggerExit(Collider other)
		{
			this.rider = other.GetComponentInChildren<Rider>();
			if (this.rider == null)
			{
				this.rider = other.GetComponentInParent<Rider>();
			}
			if (this.rider != null)
			{
				if (this.rider.IsRiding)
				{
					return;
				}
				if (this.rider.MountTrigger == this && !this.Montura.Mounted)
				{
					this.rider.MountTrigger = null;
					if (this.rider.Montura)
					{
						this.rider.Montura.EnableControls(false);
					}
					this.rider.Montura = null;
					this.rider.OnFindMount.Invoke(null);
					this.Montura.OnCanBeMounted.Invoke(false);
					this.Montura.NearbyRider = false;
				}
				this.rider = null;
			}
		}

		// Token: 0x04002976 RID: 10614
		public string MountAnimation = "Mount";

		// Token: 0x04002977 RID: 10615
		[Tooltip("the Transition ID value to dismount this kind of Montura.. (is Located on the Animator)")]
		public int DismountID = 1;

		// Token: 0x04002978 RID: 10616
		public TransformAnimation Adjustment;

		// Token: 0x04002979 RID: 10617
		private Mountable Montura;

		// Token: 0x0400297A RID: 10618
		private Rider rider;
	}
}
