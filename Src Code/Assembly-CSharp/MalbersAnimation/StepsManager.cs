using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x0200040D RID: 1037
	public class StepsManager : MonoBehaviour
	{
		// Token: 0x0600386B RID: 14443 RVA: 0x001BBB44 File Offset: 0x001B9D44
		public void EnterStep(StepTrigger foot)
		{
			if (this.Tracks && !this.Tracks.gameObject.activeInHierarchy)
			{
				this.Tracks = Object.Instantiate<ParticleSystem>(this.Tracks, base.transform, false);
				this.Tracks.transform.localScale = this.Scale;
			}
			if (this.Dust && !this.Dust.gameObject.activeInHierarchy)
			{
				this.Dust = Object.Instantiate<ParticleSystem>(this.Dust, base.transform, false);
				this.Dust.transform.localScale = this.Scale;
			}
			if (!this.active)
			{
				return;
			}
			if (foot.StepAudio && this.clips.Length != 0)
			{
				foot.StepAudio.clip = this.clips[Random.Range(0, this.clips.Length)];
				foot.StepAudio.Play();
			}
			RaycastHit raycastHit;
			if (!foot.HasTrack && Physics.Raycast(foot.transform.position, -base.transform.up, out raycastHit, 1f, this.GroundLayer))
			{
				if (this.Tracks)
				{
					ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
					emitParams.rotation3D = (Quaternion.FromToRotation(-foot.transform.forward, raycastHit.normal) * foot.transform.rotation).eulerAngles;
					emitParams.position = new Vector3(foot.transform.position.x, raycastHit.point.y + this.trackOffset, foot.transform.position.z);
					this.Tracks.Emit(emitParams, 1);
				}
				if (this.Dust)
				{
					this.Dust.transform.position = new Vector3(foot.transform.position.x, raycastHit.point.y + this.trackOffset, foot.transform.position.z);
					this.Dust.transform.rotation = Quaternion.FromToRotation(-foot.transform.forward, raycastHit.normal) * foot.transform.rotation;
					this.Dust.transform.Rotate(-90f, 0f, 0f);
					this.Dust.Emit(this.DustParticles);
				}
			}
		}

		// Token: 0x0600386C RID: 14444 RVA: 0x001BBDDA File Offset: 0x001B9FDA
		public virtual void EnableSteps(bool value)
		{
			this.active = value;
		}

		// Token: 0x040028B2 RID: 10418
		public bool Active = true;

		// Token: 0x040028B3 RID: 10419
		public LayerMask GroundLayer = 1;

		// Token: 0x040028B4 RID: 10420
		public ParticleSystem Tracks;

		// Token: 0x040028B5 RID: 10421
		public ParticleSystem Dust;

		// Token: 0x040028B6 RID: 10422
		public float StepsVolume = 0.2f;

		// Token: 0x040028B7 RID: 10423
		public int DustParticles = 30;

		// Token: 0x040028B8 RID: 10424
		[Tooltip("Scale of the dust and track particles")]
		public Vector3 Scale = Vector3.one;

		// Token: 0x040028B9 RID: 10425
		public AudioClip[] clips;

		// Token: 0x040028BA RID: 10426
		[Tooltip("Distance to Instantiate the tracks on a terrain")]
		public float trackOffset = 0.0085f;

		// Token: 0x040028BB RID: 10427
		protected bool active = true;
	}
}
