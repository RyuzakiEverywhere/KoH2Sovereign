using System;
using FMODUnity;
using Logic;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003E1 RID: 993
	public class SoundBehavior : StateMachineBehaviour
	{
		// Token: 0x06003782 RID: 14210 RVA: 0x001B7920 File Offset: 0x001B5B20
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._audio = animator.GetComponent<StudioEventEmitter>();
			this.played = false;
			this.game = GameLogic.Get(false);
			if (!this._audio)
			{
				this._audio = animator.gameObject.AddComponent<StudioEventEmitter>();
				this._audio.StopEvent = EmitterGameEvent.ObjectDisable;
				this._audio.PlayOnDistanceEnter = false;
			}
			if (this.playOnEnter && this._audio && this.game.GetSpeed() < this.mute_on_game_speed)
			{
				this.PlaySound();
			}
		}

		// Token: 0x06003783 RID: 14211 RVA: 0x001B79B4 File Offset: 0x001B5BB4
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.playOnTime && this._audio && this.game.GetSpeed() < this.mute_on_game_speed && stateInfo.normalizedTime > this.NormalizedTime && !animator.IsInTransition(layerIndex) && (this.loop || !this.played))
			{
				this.PlaySound();
			}
		}

		// Token: 0x06003784 RID: 14212 RVA: 0x001B7A17 File Offset: 0x001B5C17
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._audio && this.stopOnExit)
			{
				this._audio.Stop();
			}
		}

		// Token: 0x06003785 RID: 14213 RVA: 0x001B7A3C File Offset: 0x001B5C3C
		public virtual void PlaySound()
		{
			if (this._audio && !string.IsNullOrEmpty(this.sound) && this._audio.enabled)
			{
				if (this._audio.IsPlaying())
				{
					if (!this.restartIfAlreadyPlaying)
					{
						return;
					}
					this._audio.Stop();
				}
				this._audio.Event = this.sound;
				if (this.useTimeScale)
				{
					this._audio.Pitch = Math.Min(UnityEngine.Time.timeScale, this.max_pitch);
				}
				else
				{
					this._audio.Pitch = Math.Min(this.pitch, this.max_pitch);
				}
				this._audio.Play();
				this.played = true;
			}
		}

		// Token: 0x04002798 RID: 10136
		[EventRef]
		public string sound;

		// Token: 0x04002799 RID: 10137
		public bool playOnEnter = true;

		// Token: 0x0400279A RID: 10138
		public bool playOnTime;

		// Token: 0x0400279B RID: 10139
		public bool stopOnExit = true;

		// Token: 0x0400279C RID: 10140
		public bool restartIfAlreadyPlaying;

		// Token: 0x0400279D RID: 10141
		public bool useTimeScale;

		// Token: 0x0400279E RID: 10142
		public bool loop = true;

		// Token: 0x0400279F RID: 10143
		[Range(0f, 1f)]
		public float NormalizedTime = 0.5f;

		// Token: 0x040027A0 RID: 10144
		[Space]
		[Range(-0.5f, 3f)]
		public float pitch = 1f;

		// Token: 0x040027A1 RID: 10145
		[Range(0f, 1f)]
		public float volume = 1f;

		// Token: 0x040027A2 RID: 10146
		public float max_pitch = 2f;

		// Token: 0x040027A3 RID: 10147
		public float mute_on_game_speed = 1000f;

		// Token: 0x040027A4 RID: 10148
		private bool played;

		// Token: 0x040027A5 RID: 10149
		private StudioEventEmitter _audio;

		// Token: 0x040027A6 RID: 10150
		private Game game;
	}
}
