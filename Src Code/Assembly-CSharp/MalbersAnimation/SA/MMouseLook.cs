using System;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x02000457 RID: 1111
	[Serializable]
	public class MMouseLook
	{
		// Token: 0x06003AD7 RID: 15063 RVA: 0x001C46E4 File Offset: 0x001C28E4
		public void Init(Transform character, Transform camera)
		{
			this.m_CharacterTargetRot = character.localRotation;
			this.m_CameraTargetRot = camera.localRotation;
		}

		// Token: 0x06003AD8 RID: 15064 RVA: 0x001C4700 File Offset: 0x001C2900
		public void LookRotation(Transform character, Transform camera)
		{
			float y = Input.GetAxis("Mouse X") * this.XSensitivity;
			float num = Input.GetAxis("Mouse Y") * this.YSensitivity;
			this.m_CharacterTargetRot *= Quaternion.Euler(0f, y, 0f);
			this.m_CameraTargetRot *= Quaternion.Euler(-num, 0f, 0f);
			if (this.clampVerticalRotation)
			{
				this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot);
			}
			if (this.smooth)
			{
				character.localRotation = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, this.smoothTime * Time.deltaTime);
				camera.localRotation = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, this.smoothTime * Time.deltaTime);
				return;
			}
			character.localRotation = this.m_CharacterTargetRot;
			camera.localRotation = this.m_CameraTargetRot;
		}

		// Token: 0x06003AD9 RID: 15065 RVA: 0x001C47F8 File Offset: 0x001C29F8
		private Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1f;
			float num = 114.59156f * Mathf.Atan(q.x);
			num = Mathf.Clamp(num, this.MinimumX, this.MaximumX);
			q.x = Mathf.Tan(0.008726646f * num);
			return q;
		}

		// Token: 0x04002AA5 RID: 10917
		public float XSensitivity = 2f;

		// Token: 0x04002AA6 RID: 10918
		public float YSensitivity = 2f;

		// Token: 0x04002AA7 RID: 10919
		public bool clampVerticalRotation = true;

		// Token: 0x04002AA8 RID: 10920
		public float MinimumX = -90f;

		// Token: 0x04002AA9 RID: 10921
		public float MaximumX = 90f;

		// Token: 0x04002AAA RID: 10922
		public bool smooth;

		// Token: 0x04002AAB RID: 10923
		public float smoothTime = 5f;

		// Token: 0x04002AAC RID: 10924
		private Quaternion m_CharacterTargetRot;

		// Token: 0x04002AAD RID: 10925
		private Quaternion m_CameraTargetRot;
	}
}
