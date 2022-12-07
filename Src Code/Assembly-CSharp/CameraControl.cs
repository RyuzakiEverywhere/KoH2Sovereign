using System;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class CameraControl : MonoBehaviour
{
	// Token: 0x06000013 RID: 19 RVA: 0x000023FD File Offset: 0x000005FD
	private void Start()
	{
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002400 File Offset: 0x00000600
	private void Update()
	{
		if (Input.GetKey(KeyCode.KeypadMultiply))
		{
			Vector3 vector = base.transform.position + base.transform.forward * 20f;
			base.transform.RotateAround(vector, Vector3.up, Time.deltaTime * this.Speed);
			base.transform.LookAt(vector);
		}
		if (Input.GetKey(KeyCode.KeypadDivide))
		{
			Vector3 vector2 = base.transform.position + base.transform.forward * 20f;
			base.transform.RotateAround(vector2, Vector3.up, 0f - Time.deltaTime * this.Speed);
			base.transform.LookAt(vector2);
		}
		if (Input.GetKey(KeyCode.KeypadPlus))
		{
			base.transform.position += base.transform.up * this.Speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.KeypadMinus))
		{
			base.transform.position -= base.transform.up * this.Speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.Minus))
		{
			base.GetComponent<Camera>().fieldOfView += 1f;
		}
		if (Input.GetKey(KeyCode.Equals))
		{
			base.GetComponent<Camera>().fieldOfView -= 1f;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetMouseButton(0))
		{
			base.transform.position += base.transform.forward * this.Speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetMouseButton(1))
		{
			base.transform.position -= base.transform.forward * this.Speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			base.transform.position += base.transform.right * this.Speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			base.transform.position -= base.transform.right * this.Speed * Time.deltaTime;
		}
		base.transform.localEulerAngles += new Vector3(Input.GetAxis("Mouse Y") * (float)this.MouseSensitivity * Time.deltaTime, Input.GetAxis("Mouse X") * (float)this.MouseSensitivity * Time.deltaTime, 0f);
		this.Speed += Input.GetAxis("Mouse ScrollWheel") * 10f;
	}

	// Token: 0x0400000F RID: 15
	public float Speed = 10f;

	// Token: 0x04000010 RID: 16
	public int MouseSensitivity = 100;
}
