using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class FirstPerson : CameraMode
{
	// Token: 0x06000AE2 RID: 2786 RVA: 0x0007E5DD File Offset: 0x0007C7DD
	public FirstPerson(GameCamera gameCamera) : base(gameCamera)
	{
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0007E5F1 File Offset: 0x0007C7F1
	public override void LookAt(Vector3 point)
	{
		this.ptLookAt = point;
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x000023FD File Offset: 0x000005FD
	public override void Zoom(float zoom)
	{
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x000023FD File Offset: 0x000005FD
	public override void Yaw(float yaw)
	{
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0007E5FC File Offset: 0x0007C7FC
	public override void Set(Vector3 pos, Quaternion rot)
	{
		float height = Common.GetHeight(pos, null, -1f, false);
		this.ptLookAt = new Vector3(pos.x, height, pos.z);
		this.heightOffcet = pos.y - this.ptLookAt.y;
		this.FPPitch = rot.eulerAngles.x;
		this.FPYaw = rot.eulerAngles.y;
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0007E66C File Offset: 0x0007C86C
	public override void UpdateCamera(Transform cam, CameraSettings cameraSettings)
	{
		if (cam == null)
		{
			return;
		}
		if (!(CameraPath.Playing != null))
		{
			if (!base.GetInputKeys().zoom)
			{
				this.vScroll *= 0.1f;
			}
			this.heightOffcet += -this.vScroll.y * 0.015f;
			Vector3 right = cam.transform.right;
			right.y = 0f;
			right.Normalize();
			Vector3 a = -Common.GetRightVector(right, 0f);
			this.ptLookAt += this.vScroll.x * Time.unscaledDeltaTime * right + this.vScroll.z * Time.unscaledDeltaTime * a;
			this.ptLookAt.y = Common.GetHeight(this.ptLookAt, null, -1f, false);
			cam.transform.position = new Vector3(this.ptLookAt.x, this.ptLookAt.y + this.heightOffcet, this.ptLookAt.z);
			cam.transform.eulerAngles = new Vector3(this.FPPitch, this.FPYaw, 0f);
			return;
		}
		if (CameraPath.SetPhysicalCamera == 0)
		{
			this.owner.Camera.usePhysicalProperties = false;
			return;
		}
		if (CameraPath.SetPhysicalCamera == 1)
		{
			this.owner.Camera.usePhysicalProperties = true;
		}
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0007E7F0 File Offset: 0x0007C9F0
	public override void UpdateInput(Transform cam, CameraSettings cameraSettings)
	{
		if (this.mouse_btn_down < 0)
		{
			for (int i = 0; i <= 2; i++)
			{
				if (Input.GetMouseButtonDown(i))
				{
					this.mouse_btn_down = i;
					this.OnMouseDown(cam, Input.mousePosition, i);
					break;
				}
			}
		}
		if (this.mouse_btn_down >= 0 && Input.GetMouseButtonUp(this.mouse_btn_down))
		{
			this.OnMouseUp(cam, Input.mousePosition, this.mouse_btn_down);
			this.mouse_btn_down = -1;
		}
		this.OnMouseMove(cam, Input.mousePosition, this.mouse_btn_down);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (!UICommon.IsInInput() && base.IsMouseInApplicationScreen())
		{
			this.KeybindCameraMovement(ref num, ref num2, ref num3);
		}
		float num4 = cameraSettings.GetScrollSpeed;
		if (cameraSettings.scrollSpeedFar > 0f)
		{
			float y = cameraSettings.dist.y;
			float num5 = Mathf.Lerp(cameraSettings.dist.x, cameraSettings.dist.y, this.cam_dist / y);
			num5 /= y;
			num4 = Mathf.Lerp(cameraSettings.GetScrollSpeed, cameraSettings.scrollSpeedFar, num5);
		}
		this.vScroll.x = num * num4;
		this.vScroll.z = num3 * num4;
		float num6 = base.GetInputKeys().zoom ? 1000f : cameraSettings.zoomSpeed;
		this.vScroll.y = num2 * num6;
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0007E950 File Offset: 0x0007CB50
	private void KeybindCameraMovement(ref float xScroll, ref float yScroll, ref float zScroll)
	{
		Vector2 zero = Vector2.zero;
		bool flag = false;
		bool flag2 = false;
		if (KeyBindings.GetBind("camera_move_up", false) || UICommon.GetKey(KeyCode.UpArrow, false))
		{
			zero.y += 1f;
			flag2 = true;
		}
		if (KeyBindings.GetBind("camera_move_down", false) || UICommon.GetKey(KeyCode.DownArrow, false))
		{
			zero.y -= 1f;
			flag2 = true;
		}
		if (KeyBindings.GetBind("camera_move_left", false) || UICommon.GetKey(KeyCode.LeftArrow, false))
		{
			zero.x -= 1f;
			flag = true;
		}
		if (KeyBindings.GetBind("camera_move_right", false) || UICommon.GetKey(KeyCode.RightArrow, false))
		{
			zero.x += 1f;
			flag = true;
		}
		if (flag && flag2)
		{
			zero.x *= 0.7071f;
			zero.y *= 0.7071f;
		}
		xScroll = zero.x;
		yScroll = (this.over_ui ? 0f : (-Input.GetAxis("Mouse ScrollWheel")));
		zScroll = zero.y;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0007EA68 File Offset: 0x0007CC68
	private void OnMouseMove(Transform cam, Vector3 mousePosition, int mouse_btn_down)
	{
		if (mouse_btn_down == 2 && !this.mouse_down_over_ui[2])
		{
			Vector2 vector = mousePosition - this.ptsMouseDown;
			this.FPPitch = this.fp_down_pitch - 90f * vector.y / (float)Screen.height;
			this.FPYaw = this.fp_down_yaw + 180f * vector.x / (float)Screen.width;
		}
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0007EAD5 File Offset: 0x0007CCD5
	private void OnMouseUp(Transform cam, Vector3 mousePosition, int mouse_btn_down)
	{
		if (mouse_btn_down >= 0 || mouse_btn_down < 6)
		{
			this.mouse_down_over_ui[mouse_btn_down] = this.over_ui;
			this.mouse_down_pos[mouse_btn_down].Set(0f, 0f, 0f);
		}
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0007EB10 File Offset: 0x0007CD10
	private void OnMouseDown(Transform cam, Vector3 mousePosition, int btn)
	{
		if (btn >= 0 || btn < 6)
		{
			this.mouse_down_over_ui[btn] = this.over_ui;
			this.mouse_down_pos[this.mouse_btn_down] = mousePosition;
		}
		if (btn == 2)
		{
			this.ptsMouseDown = mousePosition;
			this.fp_down_pitch = this.FPPitch;
			this.fp_down_yaw = this.FPYaw;
		}
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x000023FD File Offset: 0x000005FD
	public override void RecalcCameraSettings()
	{
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0007EB68 File Offset: 0x0007CD68
	public override float GetDistanceToAimPoint()
	{
		return 0f;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0007EB6F File Offset: 0x0007CD6F
	public override Vector3 GetScreenToWorldPoint(Vector2 screenPoint)
	{
		return Vector3.zero;
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0007EB76 File Offset: 0x0007CD76
	public override KeyValuePair<Vector3, Quaternion> CalcPositionAndRotations(Vector3 lookAtPoint, float zoom)
	{
		return new KeyValuePair<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0007EB87 File Offset: 0x0007CD87
	public override ValueTuple<Vector3, Quaternion> GetAudioListenerPositionAndRotation()
	{
		return new ValueTuple<Vector3, Quaternion>(this.owner.transform.position, this.owner.transform.rotation);
	}

	// Token: 0x04000883 RID: 2179
	private float fp_down_pitch;

	// Token: 0x04000884 RID: 2180
	private float fp_down_yaw;

	// Token: 0x04000885 RID: 2181
	private float FPPitch;

	// Token: 0x04000886 RID: 2182
	private float FPYaw;

	// Token: 0x04000887 RID: 2183
	private float heightOffcet = 2f;
}
