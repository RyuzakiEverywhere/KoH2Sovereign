using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000439 RID: 1081
	public class RiderFPC : Rider
	{
		// Token: 0x06003A61 RID: 14945 RVA: 0x001C3300 File Offset: 0x001C1500
		private void Awake()
		{
			this._transform = base.transform;
			this._collider = base.GetComponents<Collider>();
			this._rigidbody = base.GetComponent<Rigidbody>();
			this.inputSystem = DefaultInput.GetInputSystem(this.PlayerID);
			this.MountInput.InputSystem = this.inputSystem;
			this.DismountInput.InputSystem = this.inputSystem;
			this.CallAnimalInput.InputSystem = this.inputSystem;
		}

		// Token: 0x06003A62 RID: 14946 RVA: 0x001C3375 File Offset: 0x001C1575
		private void Start()
		{
			if (this.StartMounted)
			{
				this.AlreadyMounted();
			}
		}

		// Token: 0x06003A63 RID: 14947 RVA: 0x001C3385 File Offset: 0x001C1585
		public void AlreadyMounted()
		{
			if (this.AnimalStored != null)
			{
				base.StartCoroutine(this.AlreadyMountedC());
			}
		}

		// Token: 0x06003A64 RID: 14948 RVA: 0x001C33A2 File Offset: 0x001C15A2
		private IEnumerator AlreadyMountedC()
		{
			yield return null;
			this.Mounting();
			yield break;
		}

		// Token: 0x06003A65 RID: 14949 RVA: 0x001C33B4 File Offset: 0x001C15B4
		public void Mounting()
		{
			base.Start_Mounting();
			base.IsOnHorse = true;
			this.Montura.EnableControls(true);
			if (this.CreateColliderMounted)
			{
				this.MountingCollider(true);
			}
			this.Montura.ActiveRider = this;
			Vector3 forward = this.Montura.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			base.transform.rotation = Quaternion.LookRotation(forward, -Physics.gravity);
			this.OnMount.Invoke();
		}

		// Token: 0x06003A66 RID: 14950 RVA: 0x001C3440 File Offset: 0x001C1640
		public void Dismounting()
		{
			base.Start_Dismounting();
			base.End_Dismounting();
			base.transform.position = new Vector3(base.MountTrigger.transform.position.x, base.transform.position.y, base.MountTrigger.transform.position.z);
			if (this._rigidbody)
			{
				this._rigidbody.velocity = Vector3.zero;
			}
			this.OnDismount.Invoke();
		}

		// Token: 0x06003A67 RID: 14951 RVA: 0x001C34CC File Offset: 0x001C16CC
		private void Update()
		{
			if (base.IsRiding)
			{
				base.transform.position = this.Montura.MountPoint.position;
			}
			if (base.transform.parent != null && base.IsOnHorse)
			{
				this.mounted = true;
			}
			if (this.MountInput.GetInput)
			{
				this.SetMounting();
			}
		}

		// Token: 0x06003A68 RID: 14952 RVA: 0x001C3531 File Offset: 0x001C1731
		public virtual void SetMounting()
		{
			if (base.CanMount)
			{
				this.Mounting();
				return;
			}
			if (this.CanDismount)
			{
				this.Dismounting();
				return;
			}
			if (!base.MountTrigger && !base.IsRiding)
			{
				this.CallAnimal(true);
			}
		}

		// Token: 0x04002A4A RID: 10826
		public UnityEvent OnMount = new UnityEvent();

		// Token: 0x04002A4B RID: 10827
		public UnityEvent OnDismount = new UnityEvent();
	}
}
