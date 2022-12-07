using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class SchoolChild : MonoBehaviour
{
	// Token: 0x060000CB RID: 203 RVA: 0x0000775C File Offset: 0x0000595C
	public void Start()
	{
		if (this._cacheTransform == null)
		{
			this._cacheTransform = base.transform;
		}
		if (this._spawner != null)
		{
			this.SetRandomScale();
			this.LocateRequiredChildren();
			this.RandomizeStartAnimationFrame();
			this.SkewModelForLessUniformedMovement();
			this._speed = Random.Range(this._spawner._minSpeed, this._spawner._maxSpeed);
			this.Wander(0f);
			this.SetRandomWaypoint();
			this.CheckForBubblesThenInvoke();
			this._instantiated = true;
			this.GetStartPos();
			this.FrameSkipSeedInit();
			this._spawner._activeChildren++;
			return;
		}
		base.enabled = false;
		Debug.Log(string.Concat(new object[]
		{
			base.gameObject,
			" found no school to swim in: ",
			this,
			" disabled... Standalone fish not supported, please use the SchoolController"
		}));
	}

	// Token: 0x060000CC RID: 204 RVA: 0x0000783C File Offset: 0x00005A3C
	public void Update()
	{
		if (this._spawner._updateDivisor <= 1 || this._spawner._updateCounter == this._updateSeed)
		{
			this.CheckForDistanceToWaypoint();
			this.RotationBasedOnWaypointOrAvoidance();
			this.ForwardMovement();
			this.RayCastToPushAwayFromObstacles();
			this.SetAnimationSpeed();
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00007888 File Offset: 0x00005A88
	public void FrameSkipSeedInit()
	{
		if (this._spawner._updateDivisor > 1)
		{
			int num = this._spawner._updateDivisor - 1;
			SchoolChild._updateNextSeed++;
			this._updateSeed = SchoolChild._updateNextSeed;
			SchoolChild._updateNextSeed %= num;
		}
	}

	// Token: 0x060000CE RID: 206 RVA: 0x000078D4 File Offset: 0x00005AD4
	public void CheckForBubblesThenInvoke()
	{
		if (this._spawner._bubbles != null)
		{
			base.InvokeRepeating("EmitBubbles", this._spawner._bubbles._emitEverySecond * Random.value + 1f, this._spawner._bubbles._emitEverySecond);
		}
	}

	// Token: 0x060000CF RID: 207 RVA: 0x0000792B File Offset: 0x00005B2B
	public void EmitBubbles()
	{
		this._spawner._bubbles.EmitBubbles(this._cacheTransform.position, this._speed);
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x0000794E File Offset: 0x00005B4E
	public void OnDisable()
	{
		base.CancelInvoke();
		this._spawner._activeChildren--;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00007969 File Offset: 0x00005B69
	public void OnEnable()
	{
		if (this._instantiated)
		{
			this.CheckForBubblesThenInvoke();
			this._spawner._activeChildren++;
		}
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x0000798C File Offset: 0x00005B8C
	public void LocateRequiredChildren()
	{
		if (this._model == null)
		{
			this._model = this._cacheTransform.Find("Model");
		}
		if (this._scanner == null)
		{
			this._scanner = new GameObject().transform;
			this._scanner.parent = base.transform;
			this._scanner.localRotation = Quaternion.identity;
			this._scanner.localPosition = Vector3.zero;
		}
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00007A0C File Offset: 0x00005C0C
	public void SkewModelForLessUniformedMovement()
	{
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, 0f, (float)Random.Range(-25, 25));
		this._model.rotation = identity;
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00007A4C File Offset: 0x00005C4C
	public void SetRandomScale()
	{
		float d = Random.Range(this._spawner._minScale, this._spawner._maxScale);
		this._cacheTransform.localScale = Vector3.one * d;
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00007A8C File Offset: 0x00005C8C
	public void RandomizeStartAnimationFrame()
	{
		foreach (object obj in this._model.GetComponent<Animation>())
		{
			AnimationState animationState = (AnimationState)obj;
			animationState.time = Random.value * animationState.length;
		}
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00007AF8 File Offset: 0x00005CF8
	public void GetStartPos()
	{
		this._cacheTransform.position = this._wayPoint - new Vector3(0.1f, 0.1f, 0.1f);
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00007B24 File Offset: 0x00005D24
	public Vector3 findWaypoint()
	{
		Vector3 zero = Vector3.zero;
		zero.x = Random.Range(-this._spawner._spawnSphere, this._spawner._spawnSphere) + this._spawner._posBuffer.x;
		zero.z = Random.Range(-this._spawner._spawnSphereDepth, this._spawner._spawnSphereDepth) + this._spawner._posBuffer.z;
		zero.y = Random.Range(-this._spawner._spawnSphereHeight, this._spawner._spawnSphereHeight) + this._spawner._posBuffer.y;
		return zero;
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00007BD4 File Offset: 0x00005DD4
	public void RayCastToPushAwayFromObstacles()
	{
		if (this._spawner._push)
		{
			this.RotateScanner();
			this.RayCastToPushAwayFromObstaclesCheckForCollision();
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00007BF0 File Offset: 0x00005DF0
	public void RayCastToPushAwayFromObstaclesCheckForCollision()
	{
		RaycastHit raycastHit = default(RaycastHit);
		Vector3 forward = this._scanner.forward;
		if (!Physics.Raycast(this._cacheTransform.position, forward, out raycastHit, this._spawner._pushDistance, this._spawner._avoidanceMask))
		{
			this._scan = true;
			return;
		}
		Object component = raycastHit.transform.GetComponent<SchoolChild>();
		float d = (this._spawner._pushDistance - raycastHit.distance) / this._spawner._pushDistance;
		if (component != null)
		{
			this._cacheTransform.position -= forward * this._spawner._newDelta * d * this._spawner._pushForce;
			return;
		}
		this._speed -= 0.01f * this._spawner._newDelta;
		if (this._speed < 0.1f)
		{
			this._speed = 0.1f;
		}
		this._cacheTransform.position -= forward * this._spawner._newDelta * d * this._spawner._pushForce * 2f;
		this._scan = false;
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00007D48 File Offset: 0x00005F48
	public void RotateScanner()
	{
		if (this._scan)
		{
			this._scanner.rotation = Random.rotation;
			return;
		}
		this._scanner.Rotate(new Vector3(150f * this._spawner._newDelta, 0f, 0f));
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00007D9C File Offset: 0x00005F9C
	public bool Avoidance()
	{
		if (!this._spawner._avoidance)
		{
			return false;
		}
		RaycastHit raycastHit = default(RaycastHit);
		Quaternion rotation = this._cacheTransform.rotation;
		Vector3 eulerAngles = this._cacheTransform.rotation.eulerAngles;
		Vector3 forward = this._cacheTransform.forward;
		Vector3 right = this._cacheTransform.right;
		if (Physics.Raycast(this._cacheTransform.position, -Vector3.up + forward * 0.1f, out raycastHit, this._spawner._avoidDistance, this._spawner._avoidanceMask))
		{
			float num = (this._spawner._avoidDistance - raycastHit.distance) / this._spawner._avoidDistance;
			eulerAngles.x -= this._spawner._avoidSpeed * num * this._spawner._newDelta * (this._speed + 1f);
			rotation.eulerAngles = eulerAngles;
			this._cacheTransform.rotation = rotation;
		}
		if (Physics.Raycast(this._cacheTransform.position, Vector3.up + forward * 0.1f, out raycastHit, this._spawner._avoidDistance, this._spawner._avoidanceMask))
		{
			float num = (this._spawner._avoidDistance - raycastHit.distance) / this._spawner._avoidDistance;
			eulerAngles.x += this._spawner._avoidSpeed * num * this._spawner._newDelta * (this._speed + 1f);
			rotation.eulerAngles = eulerAngles;
			this._cacheTransform.rotation = rotation;
		}
		if (Physics.Raycast(this._cacheTransform.position, forward + right * Random.Range(-0.1f, 0.1f), out raycastHit, this._spawner._stopDistance, this._spawner._avoidanceMask))
		{
			float num = (this._spawner._stopDistance - raycastHit.distance) / this._spawner._stopDistance;
			eulerAngles.y -= this._spawner._avoidSpeed * num * this._spawner._newDelta * (this._targetSpeed + 3f);
			rotation.eulerAngles = eulerAngles;
			this._cacheTransform.rotation = rotation;
			this._speed -= num * this._spawner._newDelta * this._spawner._stopSpeedMultiplier * this._speed;
			if (this._speed < 0.01f)
			{
				this._speed = 0.01f;
			}
			return true;
		}
		if (Physics.Raycast(this._cacheTransform.position, forward + right * (this._spawner._avoidAngle + this._rotateCounterL), out raycastHit, this._spawner._avoidDistance, this._spawner._avoidanceMask))
		{
			float num = (this._spawner._avoidDistance - raycastHit.distance) / this._spawner._avoidDistance;
			this._rotateCounterL += 0.1f;
			eulerAngles.y -= this._spawner._avoidSpeed * num * this._spawner._newDelta * this._rotateCounterL * (this._speed + 1f);
			rotation.eulerAngles = eulerAngles;
			this._cacheTransform.rotation = rotation;
			if (this._rotateCounterL > 1.5f)
			{
				this._rotateCounterL = 1.5f;
			}
			this._rotateCounterR = 0f;
			return true;
		}
		if (Physics.Raycast(this._cacheTransform.position, forward + right * -(this._spawner._avoidAngle + this._rotateCounterR), out raycastHit, this._spawner._avoidDistance, this._spawner._avoidanceMask))
		{
			float num = (this._spawner._avoidDistance - raycastHit.distance) / this._spawner._avoidDistance;
			if (raycastHit.point.y < this._cacheTransform.position.y)
			{
				eulerAngles.y -= this._spawner._avoidSpeed * num * this._spawner._newDelta * (this._speed + 1f);
			}
			else
			{
				eulerAngles.x += this._spawner._avoidSpeed * num * this._spawner._newDelta * (this._speed + 1f);
			}
			this._rotateCounterR += 0.1f;
			eulerAngles.y += this._spawner._avoidSpeed * num * this._spawner._newDelta * this._rotateCounterR * (this._speed + 1f);
			rotation.eulerAngles = eulerAngles;
			this._cacheTransform.rotation = rotation;
			if (this._rotateCounterR > 1.5f)
			{
				this._rotateCounterR = 1.5f;
			}
			this._rotateCounterL = 0f;
			return true;
		}
		this._rotateCounterL = 0f;
		this._rotateCounterR = 0f;
		return false;
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000082DC File Offset: 0x000064DC
	public void ForwardMovement()
	{
		this._cacheTransform.position += this._cacheTransform.TransformDirection(Vector3.forward) * this._speed * this._spawner._newDelta;
		if (this.tParam < 1f)
		{
			if (this._speed > this._targetSpeed)
			{
				this.tParam += this._spawner._newDelta * this._spawner._acceleration;
			}
			else
			{
				this.tParam += this._spawner._newDelta * this._spawner._brake;
			}
			this._speed = Mathf.Lerp(this._speed, this._targetSpeed, this.tParam);
		}
	}

	// Token: 0x060000DD RID: 221 RVA: 0x000083AC File Offset: 0x000065AC
	public void RotationBasedOnWaypointOrAvoidance()
	{
		Quaternion b = Quaternion.identity;
		b = Quaternion.LookRotation(this._wayPoint - this._cacheTransform.position);
		if (!this.Avoidance())
		{
			this._cacheTransform.rotation = Quaternion.Slerp(this._cacheTransform.rotation, b, this._spawner._newDelta * this._damping);
		}
		float num = this._cacheTransform.localEulerAngles.x;
		num = ((num > 180f) ? (num - 360f) : num);
		Quaternion rotation = this._cacheTransform.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		eulerAngles.x = SchoolChild.ClampAngle(num, -50f, 50f);
		rotation.eulerAngles = eulerAngles;
		this._cacheTransform.rotation = rotation;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00008474 File Offset: 0x00006674
	public void CheckForDistanceToWaypoint()
	{
		if ((this._cacheTransform.position - this._wayPoint).magnitude < this._spawner._waypointDistance + this._stuckCounter)
		{
			this.Wander(0f);
			this._stuckCounter = 0f;
			this.CheckIfThisShouldTriggerNewFlockWaypoint();
			return;
		}
		this._stuckCounter += this._spawner._newDelta * (this._spawner._waypointDistance * 0.25f);
	}

	// Token: 0x060000DF RID: 223 RVA: 0x000084FA File Offset: 0x000066FA
	public void CheckIfThisShouldTriggerNewFlockWaypoint()
	{
		if (this._spawner._childTriggerPos)
		{
			this._spawner.SetRandomWaypointPosition();
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00002EA7 File Offset: 0x000010A7
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

	// Token: 0x060000E1 RID: 225 RVA: 0x00008514 File Offset: 0x00006714
	public void SetAnimationSpeed()
	{
		foreach (object obj in this._model.GetComponent<Animation>())
		{
			((AnimationState)obj).speed = Random.Range(this._spawner._minAnimationSpeed, this._spawner._maxAnimationSpeed) * this._spawner._schoolSpeed * this._speed + 0.1f;
		}
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x000085A4 File Offset: 0x000067A4
	public void Wander(float delay)
	{
		this._damping = Random.Range(this._spawner._minDamping, this._spawner._maxDamping);
		this._targetSpeed = Random.Range(this._spawner._minSpeed, this._spawner._maxSpeed) * this._spawner._speedCurveMultiplier.Evaluate(Random.value) * this._spawner._schoolSpeed;
		base.Invoke("SetRandomWaypoint", delay);
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00008621 File Offset: 0x00006821
	public void SetRandomWaypoint()
	{
		this.tParam = 0f;
		this._wayPoint = this.findWaypoint();
	}

	// Token: 0x04000144 RID: 324
	[HideInInspector]
	public SchoolController _spawner;

	// Token: 0x04000145 RID: 325
	private Vector3 _wayPoint;

	// Token: 0x04000146 RID: 326
	[HideInInspector]
	public float _speed = 10f;

	// Token: 0x04000147 RID: 327
	private float _stuckCounter;

	// Token: 0x04000148 RID: 328
	private float _damping;

	// Token: 0x04000149 RID: 329
	private Transform _model;

	// Token: 0x0400014A RID: 330
	private float _targetSpeed;

	// Token: 0x0400014B RID: 331
	private float tParam;

	// Token: 0x0400014C RID: 332
	private float _rotateCounterR;

	// Token: 0x0400014D RID: 333
	private float _rotateCounterL;

	// Token: 0x0400014E RID: 334
	public Transform _scanner;

	// Token: 0x0400014F RID: 335
	private bool _scan = true;

	// Token: 0x04000150 RID: 336
	private bool _instantiated;

	// Token: 0x04000151 RID: 337
	private static int _updateNextSeed;

	// Token: 0x04000152 RID: 338
	private int _updateSeed = -1;

	// Token: 0x04000153 RID: 339
	[HideInInspector]
	public Transform _cacheTransform;
}
