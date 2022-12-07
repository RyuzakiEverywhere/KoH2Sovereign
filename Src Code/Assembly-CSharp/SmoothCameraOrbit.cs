using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
[AddComponentMenu("Camera-Control/Smooth Mouse Orbit - Unluck Software")]
public class SmoothCameraOrbit : MonoBehaviour
{
	// Token: 0x06000082 RID: 130 RVA: 0x00004C03 File Offset: 0x00002E03
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00004C03 File Offset: 0x00002E03
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00004C0C File Offset: 0x00002E0C
	public void Init()
	{
		if (!this.target)
		{
			this.target = new GameObject("Cam Target")
			{
				transform = 
				{
					position = base.transform.position + base.transform.forward * this.distance
				}
			}.transform;
		}
		this.currentDistance = this.distance;
		this.desiredDistance = this.distance;
		this.position = base.transform.position;
		this.rotation = base.transform.rotation;
		this.currentRotation = base.transform.rotation;
		this.desiredRotation = base.transform.rotation;
		this.xDeg = Vector3.Angle(Vector3.right, base.transform.right);
		this.yDeg = Vector3.Angle(Vector3.up, base.transform.up);
		this.position = this.target.position - (this.rotation * Vector3.forward * this.currentDistance + this.targetOffset);
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00004D3C File Offset: 0x00002F3C
	private void LateUpdate()
	{
		if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
		{
			this.desiredDistance -= Input.GetAxis("Mouse Y") * 0.02f * (float)this.zoomRate * 0.125f * Mathf.Abs(this.desiredDistance);
		}
		else if (Input.GetMouseButton(0))
		{
			this.xDeg += Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
			this.yDeg -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
			this.yDeg = SmoothCameraOrbit.ClampAngle(this.yDeg, (float)this.yMinLimit, (float)this.yMaxLimit);
			this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0f);
			this.currentRotation = base.transform.rotation;
			this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, 0.02f * this.zoomDampening);
			base.transform.rotation = this.rotation;
			this.idleTimer = 0f;
			this.idleSmooth = 0f;
		}
		else
		{
			this.idleTimer += 0.02f;
			if (this.idleTimer > this.autoRotate && this.autoRotate > 0f)
			{
				this.idleSmooth += (0.02f + this.idleSmooth) * 0.005f;
				this.idleSmooth = Mathf.Clamp(this.idleSmooth, 0f, 1f);
				this.xDeg += this.xSpeed * Time.deltaTime * this.idleSmooth * this.autoRotateSpeed;
			}
			this.yDeg = SmoothCameraOrbit.ClampAngle(this.yDeg, (float)this.yMinLimit, (float)this.yMaxLimit);
			this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0f);
			this.currentRotation = base.transform.rotation;
			this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, 0.02f * this.zoomDampening * 2f);
			base.transform.rotation = this.rotation;
		}
		this.desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * 0.02f * (float)this.zoomRate * Mathf.Abs(this.desiredDistance);
		this.desiredDistance = Mathf.Clamp(this.desiredDistance, this.minDistance, this.maxDistance);
		this.currentDistance = Mathf.Lerp(this.currentDistance, this.desiredDistance, 0.02f * this.zoomDampening);
		this.position = this.target.position - (this.rotation * Vector3.forward * this.currentDistance + this.targetOffset);
		base.transform.position = this.position;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00002EA7 File Offset: 0x000010A7
	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x040000AE RID: 174
	public Transform target;

	// Token: 0x040000AF RID: 175
	public Vector3 targetOffset;

	// Token: 0x040000B0 RID: 176
	public float distance = 5f;

	// Token: 0x040000B1 RID: 177
	public float maxDistance = 20f;

	// Token: 0x040000B2 RID: 178
	public float minDistance = 0.6f;

	// Token: 0x040000B3 RID: 179
	public float xSpeed = 200f;

	// Token: 0x040000B4 RID: 180
	public float ySpeed = 200f;

	// Token: 0x040000B5 RID: 181
	public int yMinLimit = -80;

	// Token: 0x040000B6 RID: 182
	public int yMaxLimit = 80;

	// Token: 0x040000B7 RID: 183
	public int zoomRate = 40;

	// Token: 0x040000B8 RID: 184
	public float panSpeed = 0.3f;

	// Token: 0x040000B9 RID: 185
	public float zoomDampening = 5f;

	// Token: 0x040000BA RID: 186
	public float autoRotate = 1f;

	// Token: 0x040000BB RID: 187
	public float autoRotateSpeed = 0.1f;

	// Token: 0x040000BC RID: 188
	private float xDeg;

	// Token: 0x040000BD RID: 189
	private float yDeg;

	// Token: 0x040000BE RID: 190
	private float currentDistance;

	// Token: 0x040000BF RID: 191
	private float desiredDistance;

	// Token: 0x040000C0 RID: 192
	private Quaternion currentRotation;

	// Token: 0x040000C1 RID: 193
	private Quaternion desiredRotation;

	// Token: 0x040000C2 RID: 194
	private Quaternion rotation;

	// Token: 0x040000C3 RID: 195
	private Vector3 position;

	// Token: 0x040000C4 RID: 196
	private float idleTimer;

	// Token: 0x040000C5 RID: 197
	private float idleSmooth;
}
