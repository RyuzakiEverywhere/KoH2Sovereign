using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x0200033E RID: 830
	[Serializable]
	public class MouseLook
	{
		// Token: 0x06003284 RID: 12932 RVA: 0x0019A3F4 File Offset: 0x001985F4
		public void Init(Transform character, Transform camera)
		{
			this.m_CharacterTargetRot = character.localRotation;
			this.m_CameraTargetRot = camera.localRotation;
		}

		// Token: 0x06003285 RID: 12933 RVA: 0x0019A410 File Offset: 0x00198610
		public void LookRotation(Transform character, Transform camera)
		{
			float y = CrossPlatformInputManager.GetAxis("Mouse X") * this.XSensitivity;
			float num = CrossPlatformInputManager.GetAxis("Mouse Y") * this.YSensitivity;
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
			}
			else
			{
				character.localRotation = this.m_CharacterTargetRot;
				camera.localRotation = this.m_CameraTargetRot;
			}
			this.UpdateCursorLock();
		}

		// Token: 0x06003286 RID: 12934 RVA: 0x0019A50C File Offset: 0x0019870C
		public void SetCursorLock(bool value)
		{
			this.lockCursor = value;
			if (!this.lockCursor)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		// Token: 0x06003287 RID: 12935 RVA: 0x0019A529 File Offset: 0x00198729
		public void UpdateCursorLock()
		{
			if (this.lockCursor)
			{
				this.InternalLockUpdate();
			}
		}

		// Token: 0x06003288 RID: 12936 RVA: 0x0019A53C File Offset: 0x0019873C
		private void InternalLockUpdate()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				this.m_cursorIsLocked = false;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				this.m_cursorIsLocked = true;
			}
			if (this.m_cursorIsLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				return;
			}
			if (!this.m_cursorIsLocked)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		// Token: 0x06003289 RID: 12937 RVA: 0x0019A594 File Offset: 0x00198794
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

		// Token: 0x04002214 RID: 8724
		public float XSensitivity = 2f;

		// Token: 0x04002215 RID: 8725
		public float YSensitivity = 2f;

		// Token: 0x04002216 RID: 8726
		public bool clampVerticalRotation = true;

		// Token: 0x04002217 RID: 8727
		public float MinimumX = -90f;

		// Token: 0x04002218 RID: 8728
		public float MaximumX = 90f;

		// Token: 0x04002219 RID: 8729
		public bool smooth;

		// Token: 0x0400221A RID: 8730
		public float smoothTime = 5f;

		// Token: 0x0400221B RID: 8731
		public bool lockCursor = true;

		// Token: 0x0400221C RID: 8732
		private Quaternion m_CharacterTargetRot;

		// Token: 0x0400221D RID: 8733
		private Quaternion m_CameraTargetRot;

		// Token: 0x0400221E RID: 8734
		private bool m_cursorIsLocked = true;
	}
}
