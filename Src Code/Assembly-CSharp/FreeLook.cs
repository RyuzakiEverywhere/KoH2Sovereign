using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class FreeLook : CameraMode
{
	// Token: 0x06000AF2 RID: 2802 RVA: 0x0007EBAE File Offset: 0x0007CDAE
	public FreeLook(GameCamera gameCamera) : base(gameCamera)
	{
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0007E5F1 File Offset: 0x0007C7F1
	public override void LookAt(Vector3 point)
	{
		this.ptLookAt = point;
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x000023FD File Offset: 0x000005FD
	public override void Zoom(float zoom)
	{
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x000023FD File Offset: 0x000005FD
	public override void Yaw(float yaw)
	{
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x0007EBB7 File Offset: 0x0007CDB7
	public override void Set(Vector3 pos, Quaternion rot)
	{
		this.ptLookAt = pos;
		this.FPPitch = rot.eulerAngles.x;
		this.FPYaw = rot.eulerAngles.y;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x0007EBE4 File Offset: 0x0007CDE4
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
			Vector2 vector = Vector2.zero;
			if (KeyBindings.GetBind("camera_move_up", false) || UICommon.GetKey(KeyCode.UpArrow, false))
			{
				vector.y += 1f;
			}
			if (KeyBindings.GetBind("camera_move_down", false) || UICommon.GetKey(KeyCode.DownArrow, false))
			{
				vector.y -= 1f;
			}
			if (KeyBindings.GetBind("camera_move_left", false) || UICommon.GetKey(KeyCode.LeftArrow, false))
			{
				vector.x -= 1f;
			}
			if (KeyBindings.GetBind("camera_move_right", false) || UICommon.GetKey(KeyCode.RightArrow, false))
			{
				vector.x += 1f;
			}
			vector = vector.normalized;
			num = vector.x;
			num2 = (this.over_ui ? 0f : (-Input.GetAxis("Mouse ScrollWheel")));
			num3 = vector.y;
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

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0007EE20 File Offset: 0x0007D020
	private void OnMouseMove(Transform cam, Vector3 mousePosition, int btn)
	{
		if (btn != 2)
		{
			return;
		}
		Vector2 vector = mousePosition - this.ptsMouseDown;
		this.FPPitch = this.fp_down_pitch - 90f * vector.y / (float)Screen.height;
		this.FPYaw = this.fp_down_yaw + 180f * vector.x / (float)Screen.width;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnMouseUp(Transform cam, Vector3 mousePosition, int btn)
	{
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0007EE84 File Offset: 0x0007D084
	private void OnMouseDown(Transform cam, Vector3 mousePosition, int btn)
	{
		if (btn == 2)
		{
			this.ptsMouseDown = mousePosition;
			this.fp_down_pitch = this.FPPitch;
			this.fp_down_yaw = this.FPYaw;
		}
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x0007EEAC File Offset: 0x0007D0AC
	public override void UpdateCamera(Transform cam, CameraSettings cameraSettings)
	{
		if (!(CameraPath.Playing != null))
		{
			float num = -this.vScroll.y * 0.015f;
			Vector3 right = cam.transform.right;
			Vector3 forward = cam.transform.forward;
			this.ptLookAt += this.vScroll.x * Time.unscaledDeltaTime * right + this.vScroll.z * Time.unscaledDeltaTime * forward;
			this.ptLookAt.y = this.ptLookAt.y + num;
			cam.transform.position = this.ptLookAt;
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

	// Token: 0x06000AFC RID: 2812 RVA: 0x000023FD File Offset: 0x000005FD
	public override void RecalcCameraSettings()
	{
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0007EB68 File Offset: 0x0007CD68
	public override float GetDistanceToAimPoint()
	{
		return 0f;
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0007EB6F File Offset: 0x0007CD6F
	public override Vector3 GetScreenToWorldPoint(Vector2 screenPoint)
	{
		return Vector3.zero;
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0007EB76 File Offset: 0x0007CD76
	public override KeyValuePair<Vector3, Quaternion> CalcPositionAndRotations(Vector3 lookAtPoint, float zoom)
	{
		return new KeyValuePair<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0007EB87 File Offset: 0x0007CD87
	public override ValueTuple<Vector3, Quaternion> GetAudioListenerPositionAndRotation()
	{
		return new ValueTuple<Vector3, Quaternion>(this.owner.transform.position, this.owner.transform.rotation);
	}

	// Token: 0x04000888 RID: 2184
	private float FPPitch;

	// Token: 0x04000889 RID: 2185
	private float FPYaw;

	// Token: 0x0400088A RID: 2186
	private float fp_down_pitch;

	// Token: 0x0400088B RID: 2187
	private float fp_down_yaw;
}
