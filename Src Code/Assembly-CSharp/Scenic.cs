using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class Scenic : CameraMode
{
	// Token: 0x06000B01 RID: 2817 RVA: 0x0007EBAE File Offset: 0x0007CDAE
	public Scenic(GameCamera gameCamera) : base(gameCamera)
	{
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0007EFAC File Offset: 0x0007D1AC
	public override void LookAt(Vector3 newPoint)
	{
		if (float.IsInfinity(newPoint.x) || float.IsInfinity(newPoint.y) || float.IsInfinity(newPoint.z))
		{
			Debug.LogError(string.Format("Invalid lookAt point: {0}", newPoint));
			return;
		}
		this.owner.Camera.transform.rotation = Quaternion.Euler(this.cam_pitch, this.cam_yaw, 0f);
		Vector3 vector2;
		if (this.owner.Settings.snapToTerrain)
		{
			newPoint.y = this.owner.Settings.lookAtHeight;
			this.ptLookAt = base.GetValidLookAtPoint(newPoint, this.owner);
			this.ptLookAtPreHeight = this.ptLookAt;
			Vector3 vector = this.ptLookAt;
			this.owner.Camera.transform.position = vector;
			this.ptLookAtPreHeight = this.owner.Camera.transform.TransformPoint(new Vector3(0f, 0f, -this.cam_dist - this.owner.Settings.lookAtHeight));
			vector = Common.SnapToTerrain(vector, 0f, null, -1f, true);
			this.owner.Camera.transform.position = vector;
			vector2 = this.owner.Camera.transform.TransformPoint(new Vector3(0f, 0f, -this.cam_dist - this.owner.Settings.lookAtHeight));
		}
		else
		{
			Vector3 vector3 = this.owner.Camera.transform.rotation * Vector3.forward;
			if (vector3.y != 0f)
			{
				float d = (this.owner.Settings.lookAtHeight - newPoint.y) / vector3.y;
				newPoint += vector3 * d;
			}
			newPoint.y = this.owner.Settings.lookAtHeight;
			this.ptLookAt = base.GetValidLookAtPoint(newPoint, this.owner);
			this.owner.Camera.transform.position = this.ptLookAt;
			vector2 = this.owner.Camera.transform.TransformPoint(new Vector3(0f, 0f, -this.cam_dist));
			this.ptLookAtPreHeight = vector2;
		}
		if (vector2.y < this.owner.Settings.minHeight)
		{
			vector2.y = this.owner.Settings.minHeight;
		}
		this.owner.Camera.transform.position = vector2;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0007F250 File Offset: 0x0007D450
	public override void Zoom(float zoom)
	{
		this.cam_dist = Mathf.Lerp(this.owner.Settings.dist[0], this.owner.Settings.dist[1], zoom);
		this.cam_pitch = Common.map(this.cam_dist, this.owner.Settings.dist[0], this.owner.Settings.dist[1], this.owner.Settings.pitch[0], this.owner.Settings.pitch[1], false);
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x0007F300 File Offset: 0x0007D500
	public override void Yaw(float yaw)
	{
		this.cam_yaw = yaw;
		if (this.cam_yaw >= 360f)
		{
			this.cam_yaw -= 360f;
			return;
		}
		if (this.cam_yaw < 0f)
		{
			this.cam_yaw += 360f;
		}
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0007F354 File Offset: 0x0007D554
	public override void Set(Vector3 pos, Quaternion rot)
	{
		this.cam_pitch = rot.eulerAngles.x;
		this.cam_yaw = rot.eulerAngles.y;
		this.owner.Camera.transform.position = pos;
		this.owner.Camera.transform.rotation = rot;
		Vector3 vector = this.owner.Camera.transform.TransformPoint(new Vector3(0f, 0f, -this.cam_dist));
		this.ptLookAtPreHeight = vector;
		this.ptLookAt = vector;
		this.LookAt(vector);
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x000023FD File Offset: 0x000005FD
	public override void UpdateInput(Transform cam, CameraSettings cameraSettings)
	{
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x0007F3F4 File Offset: 0x0007D5F4
	public override void UpdateCamera(Transform t, CameraSettings cameraSettings)
	{
		if (CameraPath.Playing != null)
		{
			return;
		}
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if (this.vScroll.y != 0f)
		{
			this.cam_pitch += this.vScroll.y;
			if (this.cam_pitch < 0f)
			{
				this.cam_pitch = 0f;
			}
			else if (this.cam_pitch > 90f)
			{
				this.cam_pitch = 90f;
			}
		}
		this.cam_yaw += this.orbit_speed * unscaledDeltaTime;
		if (this.cam_yaw >= 360f)
		{
			this.cam_yaw -= 360f;
		}
		else if (this.cam_yaw < 0f)
		{
			this.cam_yaw += 360f;
		}
		this.vScroll.y = 0f;
		Vector3 right = t.right;
		right.y = 0f;
		right.Normalize();
		Vector3 a = -Common.GetRightVector(right, 0f);
		this.LookAt(this.ptLookAt + this.vScroll.x * unscaledDeltaTime * right + this.vScroll.z * unscaledDeltaTime * a);
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0007F540 File Offset: 0x0007D740
	public override void RecalcCameraSettings()
	{
		if (this.cam_dist > this.owner.Settings.dist[1])
		{
			this.cam_dist = this.owner.Settings.dist[1];
		}
		else if (this.cam_dist < this.owner.Settings.dist[0])
		{
			this.cam_dist = this.owner.Settings.dist[0];
		}
		this.cam_pitch = Common.map(this.cam_dist, this.owner.Settings.dist[0], this.owner.Settings.dist[1], this.owner.Settings.pitch[0], this.owner.Settings.pitch[1], false);
		this.owner.Camera.farClipPlane = Common.map(this.cam_dist, this.owner.Settings.dist[0], this.owner.Settings.dist[1], this.owner.Settings.farPlane[0], this.owner.Settings.farPlane[1], false);
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0007E441 File Offset: 0x0007C641
	public override float GetDistanceToAimPoint()
	{
		return this.cam_dist;
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0007EB6F File Offset: 0x0007CD6F
	public override Vector3 GetScreenToWorldPoint(Vector2 screenPoint)
	{
		return Vector3.zero;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0007F6A4 File Offset: 0x0007D8A4
	public override KeyValuePair<Vector3, Quaternion> CalcPositionAndRotations(Vector3 lookAtPoint, float zoom)
	{
		float num = Mathf.Lerp(this.owner.Settings.dist[0], this.owner.Settings.dist[1], zoom);
		float x = Common.map(num, this.owner.Settings.dist[0], this.owner.Settings.dist[1], this.owner.Settings.pitch[0], this.owner.Settings.pitch[1], false);
		lookAtPoint.y = this.owner.Settings.lookAtHeight;
		Quaternion quaternion = Quaternion.Euler(x, this.cam_yaw, 0f);
		Vector3 vector = lookAtPoint + quaternion * new Vector3(0f, 0f, -num);
		if (vector.y < this.owner.Settings.minHeight)
		{
			vector.y = this.owner.Settings.minHeight;
		}
		return new KeyValuePair<Vector3, Quaternion>(vector, quaternion);
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0007EB87 File Offset: 0x0007CD87
	public override ValueTuple<Vector3, Quaternion> GetAudioListenerPositionAndRotation()
	{
		return new ValueTuple<Vector3, Quaternion>(this.owner.transform.position, this.owner.transform.rotation);
	}

	// Token: 0x0400088C RID: 2188
	private Vector3 ptPan;

	// Token: 0x0400088D RID: 2189
	private float cam_pitch;

	// Token: 0x0400088E RID: 2190
	private float cam_yaw;

	// Token: 0x0400088F RID: 2191
	private float orbit_speed;
}
