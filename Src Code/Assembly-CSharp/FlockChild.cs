using System;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class FlockChild : MonoBehaviour
{
	// Token: 0x06000088 RID: 136 RVA: 0x000050F8 File Offset: 0x000032F8
	public void Start()
	{
		this.FindRequiredComponents();
		this.Wander(0f);
		this.SetRandomScale();
		this._thisT.position = this.findWaypoint();
		this.RandomizeStartAnimationFrame();
		this.InitAvoidanceValues();
		this._speed = this._spawner._minSpeed;
		this._spawner._activeChildren += 1f;
		this._instantiated = true;
		if (this._spawner._updateDivisor > 1)
		{
			int num = this._spawner._updateDivisor - 1;
			FlockChild._updateNextSeed++;
			this._updateSeed = FlockChild._updateNextSeed;
			FlockChild._updateNextSeed %= num;
		}
	}

	// Token: 0x06000089 RID: 137 RVA: 0x000051A7 File Offset: 0x000033A7
	public void Update()
	{
		if (this._spawner._updateDivisor <= 1 || this._spawner._updateCounter == this._updateSeed)
		{
			this.SoarTimeLimit();
			this.CheckForDistanceToWaypoint();
			this.RotationBasedOnWaypointOrAvoidance();
			this.LimitRotationOfModel();
		}
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000051E2 File Offset: 0x000033E2
	public void OnDisable()
	{
		base.CancelInvoke();
		this._spawner._activeChildren -= 1f;
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00005204 File Offset: 0x00003404
	public void OnEnable()
	{
		if (this._instantiated)
		{
			this._spawner._activeChildren += 1f;
			if (this._landing)
			{
				this._model.GetComponent<Animation>().Play(this._spawner._idleAnimation);
				return;
			}
			this._model.GetComponent<Animation>().Play(this._spawner._flapAnimation);
		}
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00005274 File Offset: 0x00003474
	public void FindRequiredComponents()
	{
		if (this._thisT == null)
		{
			this._thisT = base.transform;
		}
		if (this._model == null)
		{
			this._model = this._thisT.Find("Model").gameObject;
		}
		if (this._modelT == null)
		{
			this._modelT = this._model.transform;
		}
	}

	// Token: 0x0600008D RID: 141 RVA: 0x000052E4 File Offset: 0x000034E4
	public void RandomizeStartAnimationFrame()
	{
		foreach (object obj in this._model.GetComponent<Animation>())
		{
			AnimationState animationState = (AnimationState)obj;
			animationState.time = Random.value * animationState.length;
		}
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00005350 File Offset: 0x00003550
	public void InitAvoidanceValues()
	{
		this._avoidValue = Random.Range(0.3f, 0.1f);
		if (this._spawner._birdAvoidDistanceMax != this._spawner._birdAvoidDistanceMin)
		{
			this._avoidDistance = Random.Range(this._spawner._birdAvoidDistanceMax, this._spawner._birdAvoidDistanceMin);
			return;
		}
		this._avoidDistance = this._spawner._birdAvoidDistanceMin;
	}

	// Token: 0x0600008F RID: 143 RVA: 0x000053C0 File Offset: 0x000035C0
	public void SetRandomScale()
	{
		float num = Random.Range(this._spawner._minScale, this._spawner._maxScale);
		this._thisT.localScale = new Vector3(num, num, num);
	}

	// Token: 0x06000090 RID: 144 RVA: 0x000053FC File Offset: 0x000035FC
	public void SoarTimeLimit()
	{
		if (this._soar && this._spawner._soarMaxTime > 0f)
		{
			if (this._soarTimer > this._spawner._soarMaxTime)
			{
				this.Flap();
				this._soarTimer = 0f;
				return;
			}
			this._soarTimer += this._spawner._newDelta;
		}
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00005460 File Offset: 0x00003660
	public void CheckForDistanceToWaypoint()
	{
		if (!this._landing && (this._thisT.position - this._wayPoint).magnitude < this._spawner._waypointDistance + this._stuckCounter)
		{
			this.Wander(0f);
			this._stuckCounter = 0f;
			return;
		}
		if (!this._landing)
		{
			this._stuckCounter += this._spawner._newDelta;
			return;
		}
		this._stuckCounter = 0f;
	}

	// Token: 0x06000092 RID: 146 RVA: 0x000054EC File Offset: 0x000036EC
	public void RotationBasedOnWaypointOrAvoidance()
	{
		Vector3 vector = this._wayPoint - this._thisT.position;
		if (this._targetSpeed > -1f && vector != Vector3.zero)
		{
			Quaternion b = Quaternion.LookRotation(vector);
			this._thisT.rotation = Quaternion.Slerp(this._thisT.rotation, b, this._spawner._newDelta * this._damping);
		}
		if (this._spawner._childTriggerPos && (this._thisT.position - this._spawner._posBuffer).magnitude < 1f)
		{
			this._spawner.SetFlockRandomPosition();
		}
		this._speed = Mathf.Lerp(this._speed, this._targetSpeed, this._spawner._newDelta * 2.5f);
		if (this._move)
		{
			this._thisT.position += this._thisT.forward * this._speed * this._spawner._newDelta;
			if (this._avoid && this._spawner._birdAvoid)
			{
				this.Avoidance();
			}
		}
	}

	// Token: 0x06000093 RID: 147 RVA: 0x0000562C File Offset: 0x0000382C
	public bool Avoidance()
	{
		RaycastHit raycastHit = default(RaycastHit);
		Vector3 forward = this._modelT.forward;
		bool result = false;
		Quaternion rotation = Quaternion.identity;
		Vector3 eulerAngles = Vector3.zero;
		Vector3 position = Vector3.zero;
		position = this._thisT.position;
		rotation = this._thisT.rotation;
		eulerAngles = this._thisT.rotation.eulerAngles;
		if (Physics.Raycast(this._thisT.position, forward + this._modelT.right * this._avoidValue, out raycastHit, this._avoidDistance, this._spawner._avoidanceMask))
		{
			eulerAngles.y -= (float)this._spawner._birdAvoidHorizontalForce * this._spawner._newDelta * this._damping;
			rotation.eulerAngles = eulerAngles;
			this._thisT.rotation = rotation;
			result = true;
		}
		else if (Physics.Raycast(this._thisT.position, forward + this._modelT.right * -this._avoidValue, out raycastHit, this._avoidDistance, this._spawner._avoidanceMask))
		{
			eulerAngles.y += (float)this._spawner._birdAvoidHorizontalForce * this._spawner._newDelta * this._damping;
			rotation.eulerAngles = eulerAngles;
			this._thisT.rotation = rotation;
			result = true;
		}
		if (this._spawner._birdAvoidDown && !this._landing && Physics.Raycast(this._thisT.position, -Vector3.up, out raycastHit, this._avoidDistance, this._spawner._avoidanceMask))
		{
			eulerAngles.x -= (float)this._spawner._birdAvoidVerticalForce * this._spawner._newDelta * this._damping;
			rotation.eulerAngles = eulerAngles;
			this._thisT.rotation = rotation;
			position.y += (float)this._spawner._birdAvoidVerticalForce * this._spawner._newDelta * 0.01f;
			this._thisT.position = position;
			result = true;
		}
		else if (this._spawner._birdAvoidUp && !this._landing && Physics.Raycast(this._thisT.position, Vector3.up, out raycastHit, this._avoidDistance, this._spawner._avoidanceMask))
		{
			eulerAngles.x += (float)this._spawner._birdAvoidVerticalForce * this._spawner._newDelta * this._damping;
			rotation.eulerAngles = eulerAngles;
			this._thisT.rotation = rotation;
			position.y -= (float)this._spawner._birdAvoidVerticalForce * this._spawner._newDelta * 0.01f;
			this._thisT.position = position;
			result = true;
		}
		return result;
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00005934 File Offset: 0x00003B34
	public void LimitRotationOfModel()
	{
		Quaternion localRotation = Quaternion.identity;
		Vector3 eulerAngles = Vector3.zero;
		localRotation = this._modelT.localRotation;
		eulerAngles = localRotation.eulerAngles;
		if ((((this._soar && this._spawner._flatSoar) || (this._spawner._flatFly && !this._soar)) && this._wayPoint.y > this._thisT.position.y) || this._landing)
		{
			eulerAngles.x = Mathf.LerpAngle(this._modelT.localEulerAngles.x, -this._thisT.localEulerAngles.x, this._spawner._newDelta * 1.75f);
			localRotation.eulerAngles = eulerAngles;
			this._modelT.localRotation = localRotation;
			return;
		}
		eulerAngles.x = Mathf.LerpAngle(this._modelT.localEulerAngles.x, 0f, this._spawner._newDelta * 1.75f);
		localRotation.eulerAngles = eulerAngles;
		this._modelT.localRotation = localRotation;
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00005A4C File Offset: 0x00003C4C
	public void Wander(float delay)
	{
		if (!this._landing)
		{
			this._damping = Random.Range(this._spawner._minDamping, this._spawner._maxDamping);
			this._targetSpeed = Random.Range(this._spawner._minSpeed, this._spawner._maxSpeed);
			base.Invoke("SetRandomMode", delay);
		}
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00005AB0 File Offset: 0x00003CB0
	public void SetRandomMode()
	{
		base.CancelInvoke("SetRandomMode");
		if (!this._dived && Random.value < this._spawner._soarFrequency)
		{
			this.Soar();
			return;
		}
		if (!this._dived && Random.value < this._spawner._diveFrequency)
		{
			this.Dive();
			return;
		}
		this.Flap();
	}

	// Token: 0x06000097 RID: 151 RVA: 0x00005B10 File Offset: 0x00003D10
	public void Flap()
	{
		if (this._move)
		{
			if (this._model != null)
			{
				this._model.GetComponent<Animation>().CrossFade(this._spawner._flapAnimation, 0.5f);
			}
			this._soar = false;
			this.animationSpeed();
			this._wayPoint = this.findWaypoint();
			this._dived = false;
		}
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00005B74 File Offset: 0x00003D74
	public Vector3 findWaypoint()
	{
		Vector3 zero = Vector3.zero;
		zero.x = Random.Range(-this._spawner._spawnSphere, this._spawner._spawnSphere) + this._spawner._posBuffer.x;
		zero.z = Random.Range(-this._spawner._spawnSphereDepth, this._spawner._spawnSphereDepth) + this._spawner._posBuffer.z;
		zero.y = Random.Range(-this._spawner._spawnSphereHeight, this._spawner._spawnSphereHeight) + this._spawner._posBuffer.y;
		return zero;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00005C24 File Offset: 0x00003E24
	public void Soar()
	{
		if (this._move)
		{
			this._model.GetComponent<Animation>().CrossFade(this._spawner._soarAnimation, 1.5f);
			this._wayPoint = this.findWaypoint();
			this._soar = true;
		}
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00005C64 File Offset: 0x00003E64
	public void Dive()
	{
		if (this._spawner._soarAnimation != null)
		{
			this._model.GetComponent<Animation>().CrossFade(this._spawner._soarAnimation, 1.5f);
		}
		else
		{
			foreach (object obj in this._model.GetComponent<Animation>())
			{
				AnimationState animationState = (AnimationState)obj;
				if (this._thisT.position.y < this._wayPoint.y + 25f)
				{
					animationState.speed = 0.1f;
				}
			}
		}
		this._wayPoint = this.findWaypoint();
		this._wayPoint.y = this._wayPoint.y - this._spawner._diveValue;
		this._dived = true;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00005D48 File Offset: 0x00003F48
	public void animationSpeed()
	{
		foreach (object obj in this._model.GetComponent<Animation>())
		{
			AnimationState animationState = (AnimationState)obj;
			if (!this._dived && !this._landing)
			{
				animationState.speed = Random.Range(this._spawner._minAnimationSpeed, this._spawner._maxAnimationSpeed);
			}
			else
			{
				animationState.speed = this._spawner._maxAnimationSpeed;
			}
		}
	}

	// Token: 0x040000C6 RID: 198
	public FlockController _spawner;

	// Token: 0x040000C7 RID: 199
	public Vector3 _wayPoint;

	// Token: 0x040000C8 RID: 200
	public float _speed;

	// Token: 0x040000C9 RID: 201
	public bool _dived = true;

	// Token: 0x040000CA RID: 202
	public float _stuckCounter;

	// Token: 0x040000CB RID: 203
	public float _damping;

	// Token: 0x040000CC RID: 204
	public bool _soar = true;

	// Token: 0x040000CD RID: 205
	public bool _landing;

	// Token: 0x040000CE RID: 206
	public float _targetSpeed;

	// Token: 0x040000CF RID: 207
	public bool _move = true;

	// Token: 0x040000D0 RID: 208
	public GameObject _model;

	// Token: 0x040000D1 RID: 209
	public Transform _modelT;

	// Token: 0x040000D2 RID: 210
	public float _avoidValue;

	// Token: 0x040000D3 RID: 211
	public float _avoidDistance;

	// Token: 0x040000D4 RID: 212
	private float _soarTimer;

	// Token: 0x040000D5 RID: 213
	private bool _instantiated;

	// Token: 0x040000D6 RID: 214
	private static int _updateNextSeed;

	// Token: 0x040000D7 RID: 215
	private int _updateSeed = -1;

	// Token: 0x040000D8 RID: 216
	public bool _avoid = true;

	// Token: 0x040000D9 RID: 217
	public Transform _thisT;

	// Token: 0x040000DA RID: 218
	public Vector3 _landingPosOffset;
}
