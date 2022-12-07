using System;
using UnityEngine;

// Token: 0x02000010 RID: 16
[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
public class DragMouseOrbit : MonoBehaviour
{
	// Token: 0x06000026 RID: 38 RVA: 0x00002CA0 File Offset: 0x00000EA0
	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.rotationYAxis = eulerAngles.y;
		this.rotationXAxis = eulerAngles.x;
		if (base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002CEC File Offset: 0x00000EEC
	private void LateUpdate()
	{
		if (this.target)
		{
			if (Input.GetMouseButton(1))
			{
				this.velocityX += this.xSpeed * Input.GetAxis("Mouse X") * this.distance * 0.02f;
				this.velocityY += this.ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
			}
			this.rotationYAxis += this.velocityX;
			this.rotationXAxis -= this.velocityY;
			this.rotationXAxis = DragMouseOrbit.ClampAngle(this.rotationXAxis, this.yMinLimit, this.yMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.rotationXAxis, this.rotationYAxis, 0f);
			this.distance = Mathf.Clamp(this.distance - Input.GetAxis("Mouse ScrollWheel") * 5f, this.distanceMin, this.distanceMax);
			RaycastHit raycastHit;
			if (Physics.Linecast(this.target.position, base.transform.position, out raycastHit))
			{
				this.distance -= raycastHit.distance;
			}
			Vector3 point = new Vector3(0f, 0f, -this.distance);
			Vector3 position = rotation * point + this.target.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
			this.velocityX = Mathf.Lerp(this.velocityX, 0f, Time.deltaTime * this.smoothTime);
			this.velocityY = Mathf.Lerp(this.velocityY, 0f, Time.deltaTime * this.smoothTime);
		}
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002EA7 File Offset: 0x000010A7
	public static float ClampAngle(float angle, float min, float max)
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

	// Token: 0x04000031 RID: 49
	public Transform target;

	// Token: 0x04000032 RID: 50
	public float distance = 5f;

	// Token: 0x04000033 RID: 51
	public float xSpeed = 120f;

	// Token: 0x04000034 RID: 52
	public float ySpeed = 120f;

	// Token: 0x04000035 RID: 53
	public float yMinLimit = -20f;

	// Token: 0x04000036 RID: 54
	public float yMaxLimit = 80f;

	// Token: 0x04000037 RID: 55
	public float distanceMin = 0.5f;

	// Token: 0x04000038 RID: 56
	public float distanceMax = 15f;

	// Token: 0x04000039 RID: 57
	public float smoothTime = 2f;

	// Token: 0x0400003A RID: 58
	private float rotationYAxis;

	// Token: 0x0400003B RID: 59
	private float rotationXAxis;

	// Token: 0x0400003C RID: 60
	private float velocityX;

	// Token: 0x0400003D RID: 61
	private float velocityY;
}
