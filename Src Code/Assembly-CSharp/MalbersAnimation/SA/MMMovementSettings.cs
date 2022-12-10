using System;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x02000459 RID: 1113
	[Serializable]
	public class MMMovementSettings
	{
		// Token: 0x06003AEB RID: 15083 RVA: 0x001C4E6C File Offset: 0x001C306C
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

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06003AEC RID: 15084 RVA: 0x001C4F0E File Offset: 0x001C310E
		public bool Running
		{
			get
			{
				return this.m_Running;
			}
		}

		// Token: 0x04002ABB RID: 10939
		public float ForwardSpeed = 8f;

		// Token: 0x04002ABC RID: 10940
		public float BackwardSpeed = 4f;

		// Token: 0x04002ABD RID: 10941
		public float StrafeSpeed = 4f;

		// Token: 0x04002ABE RID: 10942
		public float RunMultiplier = 2f;

		// Token: 0x04002ABF RID: 10943
		public InputRow RunKey = new InputRow("Shift", KeyCode.LeftShift, InputButton.Press);

		// Token: 0x04002AC0 RID: 10944
		public float JumpForce = 30f;

		// Token: 0x04002AC1 RID: 10945
		public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(-90f, 1f),
			new Keyframe(0f, 1f),
			new Keyframe(90f, 0f)
		});

		// Token: 0x04002AC2 RID: 10946
		[HideInInspector]
		public float CurrentTargetSpeed = 8f;

		// Token: 0x04002AC3 RID: 10947
		private bool m_Running;
	}
}
