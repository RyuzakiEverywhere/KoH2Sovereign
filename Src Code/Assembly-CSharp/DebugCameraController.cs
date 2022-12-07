using System;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class DebugCameraController : MonoBehaviour
{
	// Token: 0x06000609 RID: 1545 RVA: 0x000419B4 File Offset: 0x0003FBB4
	private void Update()
	{
		if (!Input.GetKey(KeyCode.Mouse1))
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			vector += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector += Vector3.right;
		}
		if (Input.GetKey(KeyCode.E))
		{
			vector += Vector3.up;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			vector += Vector3.down;
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			vector *= 5f;
		}
		this.pitch += Input.GetAxis("Mouse Y") * this.rotateSpeed.y;
		this.yaw += Input.GetAxis("Mouse X") * this.rotateSpeed.x;
		this.pitch = Mathf.Clamp(this.pitch, -90f, 90f);
		Vector3 a = Vector3.zero;
		a += vector.z * base.transform.forward;
		a += vector.x * base.transform.right;
		a += vector.y * base.transform.up;
		base.transform.position += a * Time.deltaTime * this.moveSpeed;
		base.transform.rotation = Quaternion.Euler(this.pitch, this.yaw, 0f);
	}

	// Token: 0x040005A1 RID: 1441
	public Vector2 rotateSpeed = new Vector2(10f, 10f);

	// Token: 0x040005A2 RID: 1442
	public float moveSpeed = 10f;

	// Token: 0x040005A3 RID: 1443
	private float pitch;

	// Token: 0x040005A4 RID: 1444
	private float yaw;
}
