using System;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x0200045C RID: 1116
	[Serializable]
	public class MMovementSettings
	{
		// Token: 0x06003AFF RID: 15103 RVA: 0x001C559C File Offset: 0x001C379C
		public void UpdateDesiredTargetSpeed(Vector2 input)
		{
			if (input == Vector2.zero)
			{
				return;
			}
			if (input.x > 0f || input.x < 0f)
			{
				this.CurrentTargetSpeed = this.StrafeSpeed;
			}
			if (input.y < 0f)
			{
				this.CurrentTargetSpeed = this.BackwardSpeed;
			}
			if (input.y > 0f)
			{
				this.CurrentTargetSpeed = this.ForwardSpeed;
			}
			if (this.RunKey.GetInput)
			{
				this.CurrentTargetSpeed *= this.RunMultiplier;
				this.m_Running = true;
				return;
			}
			this.m_Running = false;
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06003B00 RID: 15104 RVA: 0x001C563E File Offset: 0x001C383E
		public bool Running
		{
			get
			{
				return this.m_Running;
			}
		}

		// Token: 0x04002AD5 RID: 10965
		public float ForwardSpeed = 8f;

		// Token: 0x04002AD6 RID: 10966
		public float BackwardSpeed = 4f;

		// Token: 0x04002AD7 RID: 10967
		public float StrafeSpeed = 4f;

		// Token: 0x04002AD8 RID: 10968
		public float RunMultiplier = 2f;

		// Token: 0x04002AD9 RID: 10969
		public InputRow RunKey = new InputRow("Shift", KeyCode.LeftShift, InputButton.Press);

		// Token: 0x04002ADA RID: 10970
		public float JumpForce = 30f;

		// Token: 0x04002ADB RID: 10971
		public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(-90f, 1f),
			new Keyframe(0f, 1f),
			new Keyframe(90f, 0f)
		});

		// Token: 0x04002ADC RID: 10972
		[HideInInspector]
		public float CurrentTargetSpeed = 8f;

		// Token: 0x04002ADD RID: 10973
		private bool m_Running;
	}
}
