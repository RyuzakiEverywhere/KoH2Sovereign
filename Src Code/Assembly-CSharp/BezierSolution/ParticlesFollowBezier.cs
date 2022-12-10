using System;
using System.Collections.Generic;
using UnityEngine;

namespace BezierSolution
{
	// Token: 0x02000359 RID: 857
	[ExecuteInEditMode]
	public class ParticlesFollowBezier : MonoBehaviour
	{
		// Token: 0x0600336D RID: 13165 RVA: 0x0019F4E0 File Offset: 0x0019D6E0
		private void Awake()
		{
			this.cachedTransform = base.transform;
			this.cachedPS = base.GetComponent<ParticleSystem>();
			this.cachedMainModule = this.cachedPS.main;
			this.particles = new ParticleSystem.Particle[this.cachedMainModule.maxParticles];
			if (this.followMode == ParticlesFollowBezier.FollowMode.Relaxed)
			{
				this.particleData = new List<Vector4>(this.particles.Length);
			}
		}

		// Token: 0x0600336E RID: 13166 RVA: 0x0019F548 File Offset: 0x0019D748
		private void LateUpdate()
		{
			if (this.spline == null || this.cachedPS == null)
			{
				return;
			}
			if (this.particles.Length < this.cachedMainModule.maxParticles && this.particles.Length < 25000)
			{
				this.particles = new ParticleSystem.Particle[Mathf.Min(this.cachedMainModule.maxParticles, 25000)];
			}
			bool flag = this.cachedMainModule.simulationSpace != ParticleSystemSimulationSpace.World;
			int num = this.cachedPS.GetParticles(this.particles);
			if (this.followMode == ParticlesFollowBezier.FollowMode.Relaxed)
			{
				if (this.particleData == null)
				{
					this.particleData = new List<Vector4>(this.particles.Length);
				}
				this.cachedPS.GetCustomParticleData(this.particleData, ParticleSystemCustomData.Custom1);
				for (int i = 0; i < num; i++)
				{
					Vector4 vector = this.particleData[i];
					Vector3 vector2 = this.spline.GetPoint(1f - this.particles[i].remainingLifetime / this.particles[i].startLifetime);
					if (flag)
					{
						vector2 = this.cachedTransform.InverseTransformPoint(vector2);
					}
					if (vector.w != 0f)
					{
						ParticleSystem.Particle[] array = this.particles;
						int num2 = i;
						array[num2].position = array[num2].position + (vector2 - vector);
					}
					vector = vector2;
					vector.w = 1f;
					this.particleData[i] = vector;
				}
				this.cachedPS.SetCustomParticleData(this.particleData, ParticleSystemCustomData.Custom1);
			}
			else
			{
				Vector3 b = this.cachedTransform.position - this.spline.GetPoint(0f);
				for (int j = 0; j < num; j++)
				{
					Vector3 position = this.spline.GetPoint(1f - this.particles[j].remainingLifetime / this.particles[j].startLifetime) + b;
					if (flag)
					{
						position = this.cachedTransform.InverseTransformPoint(position);
					}
					this.particles[j].position = position;
				}
			}
			this.cachedPS.SetParticles(this.particles, num);
		}

		// Token: 0x040022BF RID: 8895
		private const int MAX_PARTICLE_COUNT = 25000;

		// Token: 0x040022C0 RID: 8896
		public BezierSpline spline;

		// Token: 0x040022C1 RID: 8897
		public ParticlesFollowBezier.FollowMode followMode;

		// Token: 0x040022C2 RID: 8898
		private Transform cachedTransform;

		// Token: 0x040022C3 RID: 8899
		private ParticleSystem cachedPS;

		// Token: 0x040022C4 RID: 8900
		private ParticleSystem.MainModule cachedMainModule;

		// Token: 0x040022C5 RID: 8901
		private ParticleSystem.Particle[] particles;

		// Token: 0x040022C6 RID: 8902
		private List<Vector4> particleData;

		// Token: 0x0200089A RID: 2202
		public enum FollowMode
		{
			// Token: 0x0400404E RID: 16462
			Relaxed,
			// Token: 0x0400404F RID: 16463
			Strict
		}
	}
}
