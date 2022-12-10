using System;
using System.Collections;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003E5 RID: 997
	[CreateAssetMenu(menuName = "Malbers Animations/Camera/FreeLook Camera Manager")]
	public class FreeLockCameraManager : ScriptableObject
	{
		// Token: 0x06003798 RID: 14232 RVA: 0x001B86D8 File Offset: 0x001B68D8
		public void SetCamera(MFreeLookCamera Freecamera)
		{
			this.mCamera = Freecamera;
			if (this.mCamera)
			{
				this.cam = this.mCamera.Cam.GetComponent<Camera>();
			}
			this.ChangeStates = this.StateTransition(this.transition);
			this.currentState = null;
			this.NextState = null;
			this.Mounted = null;
			this.MountedTarget = null;
		}

		// Token: 0x06003799 RID: 14233 RVA: 0x001B873D File Offset: 0x001B693D
		public void ChangeTarget(Transform tranform)
		{
			if (this.mCamera == null)
			{
				return;
			}
			this.mCamera.SetTarget(tranform);
		}

		// Token: 0x0600379A RID: 14234 RVA: 0x001B875A File Offset: 0x001B695A
		public void SetRiderTarget(Transform tranform)
		{
			this.RiderTarget = tranform;
		}

		// Token: 0x0600379B RID: 14235 RVA: 0x001B8763 File Offset: 0x001B6963
		public void SetMountedTarget(Transform tranform)
		{
			this.MountedTarget = tranform;
			if (this.mCamera == null)
			{
				return;
			}
			this.ChangeTarget(tranform);
		}

		// Token: 0x0600379C RID: 14236 RVA: 0x001B8782 File Offset: 0x001B6982
		public void SetMountedState(FreeLookCameraState state)
		{
			this.Mounted = state;
			this.SetCameraState(state);
		}

		// Token: 0x0600379D RID: 14237 RVA: 0x001B8794 File Offset: 0x001B6994
		private void UpdateState(FreeLookCameraState state)
		{
			if (this.mCamera == null)
			{
				return;
			}
			if (state == null)
			{
				return;
			}
			this.mCamera.Pivot.localPosition = state.PivotPos;
			this.mCamera.Cam.localPosition = state.CamPos;
			this.cam.fieldOfView = state.CamFOV;
		}

		// Token: 0x0600379E RID: 14238 RVA: 0x001B87F8 File Offset: 0x001B69F8
		public void SetAim(int ID)
		{
			if (this.mCamera == null)
			{
				return;
			}
			if (ID == -1 && this.AimLeft)
			{
				this.SetCameraState(this.AimLeft);
				this.mCamera.SetTarget(this.RiderTarget);
				return;
			}
			if (ID == 1 && this.AimRight)
			{
				this.SetCameraState(this.AimRight);
				this.mCamera.SetTarget(this.RiderTarget);
				return;
			}
			this.SetCameraState(this.Mounted ?? this.Default);
			if (this.MountedTarget)
			{
				this.mCamera.SetTarget(this.MountedTarget);
			}
		}

		// Token: 0x0600379F RID: 14239 RVA: 0x001B88A8 File Offset: 0x001B6AA8
		public void SetCameraState(FreeLookCameraState state)
		{
			if (this.mCamera == null)
			{
				return;
			}
			if (state == null)
			{
				return;
			}
			this.NextState = state;
			if (this.currentState && this.NextState == this.currentState)
			{
				return;
			}
			this.mCamera.StopCoroutine(this.ChangeStates);
			this.ChangeStates = this.StateTransition(this.transition);
			this.mCamera.StartCoroutine(this.ChangeStates);
		}

		// Token: 0x060037A0 RID: 14240 RVA: 0x001B892B File Offset: 0x001B6B2B
		private IEnumerator StateTransition(float time)
		{
			float elapsedTime = 0f;
			this.currentState = this.NextState;
			while (elapsedTime < time)
			{
				this.mCamera.Pivot.localPosition = Vector3.Lerp(this.mCamera.Pivot.localPosition, this.NextState.PivotPos, Mathf.SmoothStep(0f, 1f, elapsedTime / time));
				this.mCamera.Cam.localPosition = Vector3.Lerp(this.mCamera.Cam.localPosition, this.NextState.CamPos, Mathf.SmoothStep(0f, 1f, elapsedTime / time));
				this.cam.fieldOfView = Mathf.Lerp(this.cam.fieldOfView, this.NextState.CamFOV, Mathf.SmoothStep(0f, 1f, elapsedTime / time));
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			this.UpdateState(this.NextState);
			this.NextState = null;
			yield break;
		}

		// Token: 0x040027CB RID: 10187
		public float transition = 1f;

		// Token: 0x040027CC RID: 10188
		public FreeLookCameraState Default;

		// Token: 0x040027CD RID: 10189
		public FreeLookCameraState AimRight;

		// Token: 0x040027CE RID: 10190
		public FreeLookCameraState AimLeft;

		// Token: 0x040027CF RID: 10191
		public FreeLookCameraState Mounted;

		// Token: 0x040027D0 RID: 10192
		private MFreeLookCamera mCamera;

		// Token: 0x040027D1 RID: 10193
		private Camera cam;

		// Token: 0x040027D2 RID: 10194
		private FreeLookCameraState NextState;

		// Token: 0x040027D3 RID: 10195
		protected FreeLookCameraState currentState;

		// Token: 0x040027D4 RID: 10196
		private IEnumerator ChangeStates;

		// Token: 0x040027D5 RID: 10197
		protected Transform MountedTarget;

		// Token: 0x040027D6 RID: 10198
		protected Transform RiderTarget;
	}
}
