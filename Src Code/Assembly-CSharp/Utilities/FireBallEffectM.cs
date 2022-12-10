using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000463 RID: 1123
	[CreateAssetMenu(menuName = "Malbers Animations/Effect Modifiers/FireBall")]
	public class FireBallEffectM : EffectModifier
	{
		// Token: 0x06003B27 RID: 15143 RVA: 0x000023FD File Offset: 0x000005FD
		public override void AwakeEffect(Effect effect)
		{
		}

		// Token: 0x06003B28 RID: 15144 RVA: 0x001C60B0 File Offset: 0x001C42B0
		public override void StartEffect(Effect effect)
		{
			this.rb = effect.Instance.GetComponent<Rigidbody>();
			this.hasLookAt = effect.Owner.GetComponent<LookAt>();
			effect.Instance.SendMessage("SetOwner", effect.Owner.gameObject, SendMessageOptions.DontRequireReceiver);
			if (this.hasLookAt && this.hasLookAt.Active && this.hasLookAt.IsAiming)
			{
				this.rb.AddForce(this.hasLookAt.Direction.normalized * this.velocity);
				return;
			}
			Animator component = effect.Owner.GetComponent<Animator>();
			Vector3 a = component.velocity.normalized;
			if ((double)component.velocity.magnitude < 0.1)
			{
				a = effect.Owner.transform.forward;
			}
			this.rb.AddForce(a * this.velocity);
		}

		// Token: 0x04002B0B RID: 11019
		public float velocity = 300f;

		// Token: 0x04002B0C RID: 11020
		private Rigidbody rb;

		// Token: 0x04002B0D RID: 11021
		private LookAt hasLookAt;
	}
}
