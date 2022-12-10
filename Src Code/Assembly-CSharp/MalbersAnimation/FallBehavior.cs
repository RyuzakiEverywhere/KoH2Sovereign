using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D0 RID: 976
	public class FallBehavior : StateMachineBehaviour
	{
		// Token: 0x06003747 RID: 14151 RVA: 0x001B54C4 File Offset: 0x001B36C4
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			this.rb = animator.GetComponent<Rigidbody>();
			this.GroundLayer = this.animal.GroundLayer;
			this.IncomingSpeed = this.rb.velocity;
			this.IncomingSpeed.y = 0f;
			this.animal.SetIntID(1);
			this.animal.IsInAir = true;
			animator.SetFloat(Hash.IDFloat, 1f);
			this.MaxHeight = float.MinValue;
			animator.applyRootMotion = false;
			this.rb.drag = 0f;
			this.rb.useGravity = true;
			this.FallBlend = 1f;
			this.check_Water = false;
			this.animal.Waterlevel = Animal.LowWaterLevel;
			this.waterlevel = Animal.LowWaterLevel;
			this.animal.RaycastWater();
			this.PivotsRayInterval = this.animal.PivotsRayInterval;
			this.FallRayInterval = this.animal.FallRayInterval;
			this.WaterRayInterval = this.animal.WaterRayInterval;
			this.animal.PivotsRayInterval = (this.animal.FallRayInterval = (this.animal.WaterRayInterval = 1));
		}

		// Token: 0x06003748 RID: 14152 RVA: 0x001B560C File Offset: 0x001B380C
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.animal.debug)
			{
				Debug.DrawRay(this.animal.Main_Pivot_Point, -this.animal.transform.up * 50f, Color.magenta);
			}
			if (Physics.Raycast(this.animal.Main_Pivot_Point, -this.animal.transform.up, out this.FallRay, 50f, this.GroundLayer))
			{
				if (this.MaxHeight < this.FallRay.distance)
				{
					this.MaxHeight = this.FallRay.distance;
				}
				this.FallBlend = Mathf.Lerp(this.FallBlend, (this.FallRay.distance - this.LowerDistance) / (this.MaxHeight - this.LowerDistance), Time.deltaTime * 20f);
				animator.SetFloat(Hash.IDFloat, this.FallBlend);
			}
			this.CheckforWater();
		}

		// Token: 0x06003749 RID: 14153 RVA: 0x001B570C File Offset: 0x001B390C
		private void CheckforWater()
		{
			if (this.waterlevel != this.animal.Waterlevel && this.animal.Waterlevel != Animal.LowWaterLevel)
			{
				this.waterlevel = this.animal.Waterlevel;
			}
			if (!this.check_Water && this.waterlevel > this.animal.Main_Pivot_Point.y)
			{
				this.rb.velocity = new Vector3(this.rb.velocity.x, 0f, this.rb.velocity.z);
				this.check_Water = true;
				this.animal.Swim = true;
				this.animal.Waterlevel = this.waterlevel;
			}
		}

		// Token: 0x0600374A RID: 14154 RVA: 0x001B57C8 File Offset: 0x001B39C8
		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.animal.AirControl)
			{
				this.AirControl();
			}
		}

		// Token: 0x0600374B RID: 14155 RVA: 0x001B57E0 File Offset: 0x001B39E0
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal.PivotsRayInterval = this.PivotsRayInterval;
			this.animal.FallRayInterval = this.FallRayInterval;
			this.animal.WaterRayInterval = this.WaterRayInterval;
			this.animal.AirControlDir = Vector3.zero;
		}

		// Token: 0x0600374C RID: 14156 RVA: 0x001B5830 File Offset: 0x001B3A30
		private void AirControl()
		{
			float deltaTime = Time.deltaTime;
			float y = this.rb.velocity.y;
			Vector3 rawDirection = this.animal.RawDirection;
			rawDirection.y = 0f;
			this.animal.AirControlDir = Vector3.Lerp(this.animal.AirControlDir, rawDirection, deltaTime * this.animal.airSmoothness);
			Debug.DrawRay(this.animal.transform.position, this.animal.AirControlDir, Color.yellow);
			Vector3 vector = this.animal.AirControlDir * this.animal.airMaxSpeed;
			if (!this.animal.DirectionalMovement)
			{
				vector = this.animal.transform.TransformDirection(vector);
			}
			vector.y = y;
			this.rb.velocity = vector;
		}

		// Token: 0x04002710 RID: 10000
		private RaycastHit FallRay;

		// Token: 0x04002711 RID: 10001
		[Tooltip("The Lower Fall animation will set to 1 if this distance the current distance to the ground")]
		public float LowerDistance;

		// Token: 0x04002712 RID: 10002
		private Animal animal;

		// Token: 0x04002713 RID: 10003
		private Rigidbody rb;

		// Token: 0x04002714 RID: 10004
		private float MaxHeight;

		// Token: 0x04002715 RID: 10005
		private float FallBlend;

		// Token: 0x04002716 RID: 10006
		private bool check_Water;

		// Token: 0x04002717 RID: 10007
		private int PivotsRayInterval;

		// Token: 0x04002718 RID: 10008
		private int FallRayInterval;

		// Token: 0x04002719 RID: 10009
		private int WaterRayInterval;

		// Token: 0x0400271A RID: 10010
		private int GroundLayer;

		// Token: 0x0400271B RID: 10011
		private Vector3 IncomingSpeed;

		// Token: 0x0400271C RID: 10012
		private float waterlevel;
	}
}
