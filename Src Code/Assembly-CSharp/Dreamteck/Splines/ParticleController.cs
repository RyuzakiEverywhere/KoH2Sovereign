using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B4 RID: 1204
	[ExecuteInEditMode]
	[AddComponentMenu("Dreamteck/Splines/Users/Particle Controller")]
	public class ParticleController : SplineUser
	{
		// Token: 0x06003F63 RID: 16227 RVA: 0x001E44DC File Offset: 0x001E26DC
		protected override void LateRun()
		{
			if (this._particleSystem == null)
			{
				return;
			}
			if (base.sampleCount == 0)
			{
				return;
			}
			int maxParticles = this._particleSystem.main.maxParticles;
			if (this.particles.Length != maxParticles)
			{
				this.particles = new ParticleSystem.Particle[maxParticles];
				ParticleController.Particle[] array = new ParticleController.Particle[maxParticles];
				int num = 0;
				while (num < array.Length && num < this.controllers.Length)
				{
					array[num] = this.controllers[num];
					num++;
				}
				this.controllers = array;
			}
			this.particleCount = this._particleSystem.GetParticles(this.particles);
			bool flag = this._particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local;
			Transform transform = this._particleSystem.transform;
			for (int i = this.particleCount - 1; i >= 0; i--)
			{
				if (flag)
				{
					this.TransformParticle(ref this.particles[i], transform);
				}
				if (this.controllers[i] == null)
				{
					this.controllers[i] = new ParticleController.Particle();
					this.OnParticleBorn(i);
					if (flag)
					{
						this.InverseTransformParticle(ref this.particles[i], transform);
					}
				}
				else
				{
					float num2 = this.particles[i].startLifetime - this.particles[i].remainingLifetime;
					if (num2 <= Time.deltaTime && this.controllers[i].lifeTime > num2)
					{
						this.OnParticleBorn(i);
					}
					if (flag)
					{
						this.InverseTransformParticle(ref this.particles[i], transform);
					}
				}
			}
			for (int j = 0; j < this.particleCount; j++)
			{
				if (this.controllers[j] == null)
				{
					this.controllers[j] = new ParticleController.Particle();
				}
				if (flag)
				{
					this.TransformParticle(ref this.particles[j], transform);
				}
				this.HandleParticle(j);
				if (flag)
				{
					this.InverseTransformParticle(ref this.particles[j], transform);
				}
			}
			this._particleSystem.SetParticles(this.particles, this.particleCount);
		}

		// Token: 0x06003F64 RID: 16228 RVA: 0x001E46ED File Offset: 0x001E28ED
		private void TransformParticle(ref ParticleSystem.Particle particle, Transform trs)
		{
			particle.position = trs.TransformPoint(particle.position);
			particle.velocity = trs.TransformDirection(particle.velocity);
		}

		// Token: 0x06003F65 RID: 16229 RVA: 0x001E4713 File Offset: 0x001E2913
		private void InverseTransformParticle(ref ParticleSystem.Particle particle, Transform trs)
		{
			particle.position = trs.InverseTransformPoint(particle.position);
			particle.velocity = trs.InverseTransformDirection(particle.velocity);
		}

		// Token: 0x06003F66 RID: 16230 RVA: 0x001E4739 File Offset: 0x001E2939
		protected override void Reset()
		{
			base.Reset();
			this.updateMethod = SplineUser.UpdateMethod.LateUpdate;
			if (this._particleSystem == null)
			{
				this._particleSystem = base.GetComponent<ParticleSystem>();
			}
		}

		// Token: 0x06003F67 RID: 16231 RVA: 0x001E4764 File Offset: 0x001E2964
		private void HandleParticle(int index)
		{
			float num = this.particles[index].remainingLifetime / this.particles[index].startLifetime;
			if (this.motionType == ParticleController.MotionType.FollowBackward || this.motionType == ParticleController.MotionType.FollowForward || this.motionType == ParticleController.MotionType.None)
			{
				base.Evaluate(this.controllers[index].GetSplinePercent(this.wrapMode, this.particles[index]), this.evalResult);
				base.ModifySample(this.evalResult);
				this.particles[index].position = this.evalResult.position;
				if (this.volumetric)
				{
					Vector3 a = -Vector3.Cross(this.evalResult.forward, this.evalResult.up);
					Vector2 vector = this.controllers[index].startOffset;
					if (this.motionType != ParticleController.MotionType.None)
					{
						vector = Vector2.Lerp(this.controllers[index].startOffset, this.controllers[index].endOffset, 1f - num);
					}
					ParticleSystem.Particle[] array = this.particles;
					array[index].position = array[index].position + (a * vector.x * this.scale.x * this.evalResult.size + this.evalResult.up * vector.y * this.scale.y * this.evalResult.size);
				}
				this.particles[index].velocity = this.evalResult.forward;
				this.particles[index].startColor = this.controllers[index].startColor * this.evalResult.color;
			}
			this.controllers[index].remainingLifetime = this.particles[index].remainingLifetime;
			this.controllers[index].lifeTime = this.particles[index].startLifetime - this.particles[index].remainingLifetime;
		}

		// Token: 0x06003F68 RID: 16232 RVA: 0x000023FD File Offset: 0x000005FD
		private void OnParticleDie(int index)
		{
		}

		// Token: 0x06003F69 RID: 16233 RVA: 0x001E4990 File Offset: 0x001E2B90
		private void OnParticleBorn(int index)
		{
			this.birthIndex++;
			double num = 0.0;
			float num2 = Mathf.Lerp(this._particleSystem.emission.rateOverTime.constantMin, this._particleSystem.emission.rateOverTime.constantMax, 0.5f) * this._particleSystem.main.startLifetime.constantMax;
			if ((float)this.birthIndex > num2)
			{
				this.birthIndex = 0;
			}
			switch (this.emitPoint)
			{
			case ParticleController.EmitPoint.Beginning:
				num = 0.0;
				break;
			case ParticleController.EmitPoint.Ending:
				num = 1.0;
				break;
			case ParticleController.EmitPoint.Random:
				num = (double)Random.Range(0f, 1f);
				break;
			case ParticleController.EmitPoint.Ordered:
				num = (double)((num2 > 0f) ? ((float)this.birthIndex / num2) : 0f);
				break;
			}
			base.Evaluate(num, this.evalResult);
			base.ModifySample(this.evalResult);
			this.controllers[index].startColor = this.particles[index].startColor;
			this.controllers[index].startPercent = num;
			this.controllers[index].startLifetime = this.particles[index].startLifetime;
			this.controllers[index].remainingLifetime = this.particles[index].remainingLifetime;
			this.controllers[index].cycleSpeed = Random.Range(this.minCycles, this.maxCycles);
			Vector2 a = Vector2.zero;
			if (this.volumetric)
			{
				if (this.emitFromShell)
				{
					a = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector2.right;
				}
				else
				{
					a = Random.insideUnitCircle;
				}
			}
			this.controllers[index].startOffset = a * 0.5f;
			this.controllers[index].endOffset = Random.insideUnitCircle * 0.5f;
			Vector3 a2 = Vector3.Cross(this.evalResult.forward, this.evalResult.up);
			this.particles[index].position = this.evalResult.position + a2 * this.controllers[index].startOffset.x * this.evalResult.size * this.scale.x + this.evalResult.up * this.controllers[index].startOffset.y * this.evalResult.size * this.scale.y;
			float x = this._particleSystem.forceOverLifetime.x.constantMax;
			float y = this._particleSystem.forceOverLifetime.y.constantMax;
			float z = this._particleSystem.forceOverLifetime.z.constantMax;
			if (this._particleSystem.forceOverLifetime.randomized)
			{
				x = Random.Range(this._particleSystem.forceOverLifetime.x.constantMin, this._particleSystem.forceOverLifetime.x.constantMax);
				y = Random.Range(this._particleSystem.forceOverLifetime.y.constantMin, this._particleSystem.forceOverLifetime.y.constantMax);
				z = Random.Range(this._particleSystem.forceOverLifetime.z.constantMin, this._particleSystem.forceOverLifetime.z.constantMax);
			}
			float num3 = this.particles[index].startLifetime - this.particles[index].remainingLifetime;
			Vector3 b = new Vector3(x, y, z) * 0.5f * (num3 * num3);
			float constantMax = this._particleSystem.main.startSpeed.constantMax;
			if (this.motionType == ParticleController.MotionType.ByNormal)
			{
				ParticleSystem.Particle[] array = this.particles;
				array[index].position = array[index].position + this.evalResult.up * constantMax * (this.particles[index].startLifetime - this.particles[index].remainingLifetime);
				ParticleSystem.Particle[] array2 = this.particles;
				array2[index].position = array2[index].position + b;
				this.particles[index].velocity = this.evalResult.up * constantMax + new Vector3(x, y, z) * num3;
			}
			else if (this.motionType == ParticleController.MotionType.ByNormalRandomized)
			{
				Vector3 a3 = Quaternion.AngleAxis(Random.Range(0f, 360f), this.evalResult.forward) * this.evalResult.up;
				ParticleSystem.Particle[] array3 = this.particles;
				array3[index].position = array3[index].position + a3 * constantMax * (this.particles[index].startLifetime - this.particles[index].remainingLifetime);
				ParticleSystem.Particle[] array4 = this.particles;
				array4[index].position = array4[index].position + b;
				this.particles[index].velocity = a3 * constantMax + new Vector3(x, y, z) * num3;
			}
			this.HandleParticle(index);
		}

		// Token: 0x04002CCA RID: 11466
		[HideInInspector]
		public ParticleSystem _particleSystem;

		// Token: 0x04002CCB RID: 11467
		[HideInInspector]
		public bool volumetric;

		// Token: 0x04002CCC RID: 11468
		[HideInInspector]
		public bool emitFromShell;

		// Token: 0x04002CCD RID: 11469
		[HideInInspector]
		public Vector2 scale = Vector2.one;

		// Token: 0x04002CCE RID: 11470
		[HideInInspector]
		public ParticleController.EmitPoint emitPoint;

		// Token: 0x04002CCF RID: 11471
		[HideInInspector]
		public ParticleController.MotionType motionType = ParticleController.MotionType.UseParticleSystem;

		// Token: 0x04002CD0 RID: 11472
		[HideInInspector]
		public ParticleController.Wrap wrapMode;

		// Token: 0x04002CD1 RID: 11473
		[HideInInspector]
		public float minCycles = 1f;

		// Token: 0x04002CD2 RID: 11474
		[HideInInspector]
		public float maxCycles = 1f;

		// Token: 0x04002CD3 RID: 11475
		private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];

		// Token: 0x04002CD4 RID: 11476
		private ParticleController.Particle[] controllers = new ParticleController.Particle[0];

		// Token: 0x04002CD5 RID: 11477
		private int particleCount;

		// Token: 0x04002CD6 RID: 11478
		private int birthIndex;

		// Token: 0x0200098E RID: 2446
		public enum EmitPoint
		{
			// Token: 0x04004480 RID: 17536
			Beginning,
			// Token: 0x04004481 RID: 17537
			Ending,
			// Token: 0x04004482 RID: 17538
			Random,
			// Token: 0x04004483 RID: 17539
			Ordered
		}

		// Token: 0x0200098F RID: 2447
		public enum MotionType
		{
			// Token: 0x04004485 RID: 17541
			None,
			// Token: 0x04004486 RID: 17542
			UseParticleSystem,
			// Token: 0x04004487 RID: 17543
			FollowForward,
			// Token: 0x04004488 RID: 17544
			FollowBackward,
			// Token: 0x04004489 RID: 17545
			ByNormal,
			// Token: 0x0400448A RID: 17546
			ByNormalRandomized
		}

		// Token: 0x02000990 RID: 2448
		public enum Wrap
		{
			// Token: 0x0400448C RID: 17548
			Default,
			// Token: 0x0400448D RID: 17549
			Loop
		}

		// Token: 0x02000991 RID: 2449
		public class Particle
		{
			// Token: 0x06005439 RID: 21561 RVA: 0x00245F54 File Offset: 0x00244154
			internal double GetSplinePercent(ParticleController.Wrap wrap, ParticleSystem.Particle particle)
			{
				if (wrap == ParticleController.Wrap.Default)
				{
					return DMath.Clamp01(this.startPercent + (double)((1f - particle.remainingLifetime / particle.startLifetime) * this.cycleSpeed));
				}
				if (wrap != ParticleController.Wrap.Loop)
				{
					return 0.0;
				}
				double num = this.startPercent + (1.0 - (double)(particle.remainingLifetime / particle.startLifetime)) * (double)this.cycleSpeed;
				if (num > 1.0)
				{
					num -= (double)Mathf.FloorToInt((float)num);
				}
				return num;
			}

			// Token: 0x0400448E RID: 17550
			internal Vector2 startOffset = Vector2.zero;

			// Token: 0x0400448F RID: 17551
			internal Vector2 endOffset = Vector2.zero;

			// Token: 0x04004490 RID: 17552
			internal float cycleSpeed;

			// Token: 0x04004491 RID: 17553
			internal float startLifetime;

			// Token: 0x04004492 RID: 17554
			internal Color startColor = Color.white;

			// Token: 0x04004493 RID: 17555
			internal float remainingLifetime;

			// Token: 0x04004494 RID: 17556
			internal float lifeTime;

			// Token: 0x04004495 RID: 17557
			internal double startPercent;
		}
	}
}
