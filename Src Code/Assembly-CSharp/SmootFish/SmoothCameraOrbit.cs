using System;
using UnityEngine;

namespace SmootFish
{
	// Token: 0x020004E0 RID: 1248
	[AddComponentMenu("Camera-Control/Smooth Mouse Orbit - Unluck Software")]
	public class SmoothCameraOrbit : MonoBehaviour
	{
		// Token: 0x06004207 RID: 16903 RVA: 0x001F784C File Offset: 0x001F5A4C
		private void Start()
		{
			this.Init();
		}

		// Token: 0x06004208 RID: 16904 RVA: 0x001F784C File Offset: 0x001F5A4C
		private void OnEnable()
		{
			this.Init();
		}

		// Token: 0x06004209 RID: 16905 RVA: 0x001F7854 File Offset: 0x001F5A54
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

		// Token: 0x0600420A RID: 16906 RVA: 0x001F7984 File Offset: 0x001F5B84
		private void LateUpdate()
		{
			if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
			{
				this.desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * (float)this.zoomRate * 0.125f * Mathf.Abs(this.desiredDistance);
			}
			else if (Input.GetMouseButton(0))
			{
				this.xDeg += Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
				this.yDeg -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
				this.yDeg = SmoothCameraOrbit.ClampAngle(this.yDeg, (float)this.yMinLimit, (float)this.yMaxLimit);
				this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0f);
				this.currentRotation = base.transform.rotation;
				this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, Time.deltaTime * this.zoomDampening);
				base.transform.rotation = this.rotation;
				this.idleTimer = 0f;
				this.idleSmooth = 0f;
			}
			else
			{
				this.idleTimer += Time.deltaTime;
				if (this.idleTimer > this.autoRotate && this.autoRotate > 0f)
				{
					this.idleSmooth += (Time.deltaTime + this.idleSmooth) * 0.005f;
					this.idleSmooth = Mathf.Clamp(this.idleSmooth, 0f, 1f);
					this.xDeg += this.xSpeed * 0.001f * this.idleSmooth;
				}
				this.yDeg = SmoothCameraOrbit.ClampAngle(this.yDeg, (float)this.yMinLimit, (float)this.yMaxLimit);
				this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0f);
				this.currentRotation = base.transform.rotation;
				this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, Time.deltaTime * this.zoomDampening * 2f);
				base.transform.rotation = this.rotation;
			}
			this.desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * (float)this.zoomRate * Mathf.Abs(this.desiredDistance);
			this.desiredDistance = Mathf.Clamp(this.desiredDistance, this.minDistance, this.maxDistance);
			this.currentDistance = Mathf.Lerp(this.currentDistance, this.desiredDistance, Time.deltaTime * this.zoomDampening);
			this.position = this.target.position - (this.rotation * Vector3.forward * this.currentDistance + this.targetOffset);
			base.transform.position = this.position;
		}

		// Token: 0x0600420B RID: 16907 RVA: 0x00002EA7 File Offset: 0x000010A7
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

		// Token: 0x04002DE7 RID: 11751
		public Transform target;

		// Token: 0x04002DE8 RID: 11752
		public Vector3 targetOffset;

		// Token: 0x04002DE9 RID: 11753
		public float distance = 5f;

		// Token: 0x04002DEA RID: 11754
		public float maxDistance = 20f;

		// Token: 0x04002DEB RID: 11755
		public float minDistance = 0.6f;

		// Token: 0x04002DEC RID: 11756
		public float xSpeed = 200f;

		// Token: 0x04002DED RID: 11757
		public float ySpeed = 200f;

		// Token: 0x04002DEE RID: 11758
		public int yMinLimit = -80;

		// Token: 0x04002DEF RID: 11759
		public int yMaxLimit = 80;

		// Token: 0x04002DF0 RID: 11760
		public int zoomRate = 40;

		// Token: 0x04002DF1 RID: 11761
		public float panSpeed = 0.3f;

		// Token: 0x04002DF2 RID: 11762
		public float zoomDampening = 5f;

		// Token: 0x04002DF3 RID: 11763
		public float autoRotate = 1f;

		// Token: 0x04002DF4 RID: 11764
		private float xDeg;

		// Token: 0x04002DF5 RID: 11765
		private float yDeg;

		// Token: 0x04002DF6 RID: 11766
		private float currentDistance;

		// Token: 0x04002DF7 RID: 11767
		private float desiredDistance;

		// Token: 0x04002DF8 RID: 11768
		private Quaternion currentRotation;

		// Token: 0x04002DF9 RID: 11769
		private Quaternion desiredRotation;

		// Token: 0x04002DFA RID: 11770
		private Quaternion rotation;

		// Token: 0x04002DFB RID: 11771
		private Vector3 position;

		// Token: 0x04002DFC RID: 11772
		private float idleTimer;

		// Token: 0x04002DFD RID: 11773
		private float idleSmooth;
	}
}
