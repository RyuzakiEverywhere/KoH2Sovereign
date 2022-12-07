using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class HerdSimCore : MonoBehaviour
{
	// Token: 0x060000F4 RID: 244 RVA: 0x00008CB8 File Offset: 0x00006EB8
	public void Disable(bool disableModel, bool disableCollider)
	{
		if (this._enabled)
		{
			this._enabled = false;
			base.CancelInvoke();
			if (disableModel)
			{
				this._model.gameObject.SetActive(false);
			}
			if (disableCollider)
			{
				this._collider.gameObject.SetActive(false);
			}
			this._thisTR.GetComponent<HerdSimCore>().enabled = false;
			this._model.GetComponent<Animation>().Stop();
		}
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00008D24 File Offset: 0x00006F24
	public void Enable()
	{
		if (!this._enabled)
		{
			this._enabled = true;
			this.Init();
			if (!this._model.gameObject.activeInHierarchy)
			{
				this._model.gameObject.SetActive(true);
			}
			if (!this._collider.gameObject.activeInHierarchy)
			{
				this._collider.gameObject.SetActive(true);
			}
			this._thisTR.GetComponent<HerdSimCore>().enabled = true;
			this._model.GetComponent<Animation>().Play();
		}
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00008DAE File Offset: 0x00006FAE
	public void Damage(float d)
	{
		this._hitPoints -= d;
		if (this._hitPoints <= 0f)
		{
			this.Death();
		}
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00008DD4 File Offset: 0x00006FD4
	public void Effects()
	{
		if (this._controller != null && this._mode == 2 && this._controller._runPS != null && this._speed > 1f)
		{
			this._controller._runPS.transform.position = this._thisTR.position;
			this._controller._runPS.Emit(1);
		}
		if (this._dead && this._controller != null && this._controller._deadPS != null)
		{
			this._controller._deadPS.transform.position = this._collider.transform.position;
			this._controller._deadPS.Emit(1);
		}
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x00008EA8 File Offset: 0x000070A8
	public void Death()
	{
		if (!this._dead)
		{
			this._dead = true;
			this._mode = 0;
			base.CancelInvoke("Wander");
			base.CancelInvoke("WalkTimeOut");
			base.CancelInvoke("FindLeader");
			if (this._leader != null)
			{
				if (this._leader != this)
				{
					this._leader._leaderSize--;
				}
				else
				{
					this._leaderSize = 0;
				}
				this._leader = null;
			}
			if (this._deadMaterial != null)
			{
				this._renderer.sharedMaterial = this._deadMaterial;
			}
			this._model.GetComponent<Animation>()[this._animDead].speed = 1f;
			this._model.GetComponent<Animation>().CrossFade(this._animDead, 0.1f);
			if (this._scaryCorpse)
			{
				base.InvokeRepeating("Corpse", 1f, 1f);
			}
		}
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x00008FA8 File Offset: 0x000071A8
	public void Corpse()
	{
		Collider[] array = Physics.OverlapSphere(this._thisTR.position, 10f);
		HerdSimCore herdSimCore = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform.parent != null)
			{
				herdSimCore = array[i].transform.parent.GetComponent<HerdSimCore>();
			}
			if (this._scaryCorpse && herdSimCore != null && !herdSimCore._dead && herdSimCore._mode < 1)
			{
				herdSimCore.Scare(base.transform);
			}
		}
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00009034 File Offset: 0x00007234
	public void FindLeader()
	{
		if (this._leader == this && this._leaderSize <= 1)
		{
			this._leader = null;
			this._leaderSize = 0;
			return;
		}
		if (this._leader != this)
		{
			if (this._leader != null && this._leader._dead)
			{
				this._leader = null;
			}
			this._leaderSize = 0;
			Collider[] array = Physics.OverlapSphere(this._thisTR.position, this._herdDistance, this._herdLayerMask);
			HerdSimCore herdSimCore = null;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].transform.parent != null)
				{
					herdSimCore = array[i].transform.parent.GetComponent<HerdSimCore>();
				}
				if (herdSimCore != null && herdSimCore != this && this._type == herdSimCore._type)
				{
					if (this._leader == null && herdSimCore._leader == null)
					{
						this._leader = this;
						herdSimCore._leader = this;
						this._leaderSize += 2;
						return;
					}
					if (this._leader == null && herdSimCore._leader != null && herdSimCore._leader._leaderSize < herdSimCore._leader._herdSize)
					{
						this._leader = herdSimCore._leader;
						this._leader._leaderSize++;
						return;
					}
					if (this._leader != null && herdSimCore._leader != this._leader && herdSimCore._leader != null && herdSimCore._leader._leaderSize >= this._leader._leaderSize && herdSimCore._leader._leaderSize < herdSimCore._leader._herdSize)
					{
						this._leader._leaderSize--;
						herdSimCore._leader._leaderSize++;
						this._leader = herdSimCore._leader;
						return;
					}
				}
			}
		}
	}

	// Token: 0x060000FB RID: 251 RVA: 0x0000924C File Offset: 0x0000744C
	public void Wander()
	{
		Vector3 waypoint = Vector3.zero;
		if (this._leader == this)
		{
			this._leaderArea = Vector3.one * ((float)this._leaderSize * this._leaderAreaMultiplier + 1f);
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if (this._leader != null && this._leader != this)
		{
			vector = this._leader._leaderArea;
			vector2 = this._leader.transform.position;
		}
		else if (this._controller == null)
		{
			vector = this._roamingArea;
			vector2 = this._startPosition;
		}
		else
		{
			vector = this._controller._roamingArea;
			vector2 = this._controller.transform.position;
		}
		waypoint.x = Random.Range(-vector.x, vector.x) + vector2.x;
		waypoint.z = Random.Range(-vector.z, vector.z) + vector2.z;
		if (this._food != null)
		{
			waypoint = this._food.position;
			this._mode = 2;
		}
		else if (this != null)
		{
			if (this._thisTR.position.x < -vector.x + vector2.x || this._thisTR.position.x > vector.x + vector2.x || this._thisTR.position.z < -vector.z + vector2.z || this._thisTR.position.z > vector.z + vector2.z)
			{
				if (Random.value < 0.1f)
				{
					this._mode = 2;
				}
				else
				{
					this._mode = 1;
				}
				this._waypoint = waypoint;
			}
			else if (this._leader != null && this._leader != this && Random.value < 0.75f)
			{
				this._mode = 0;
			}
			else if (this._reachedWaypoint)
			{
				this._mode = Random.Range(-this._idleProbablity, 2);
				if (this._mode == 1 && Random.value < this._runChance && (this._leader == null || this._leader == this))
				{
					this._mode = 2;
				}
			}
		}
		if (this._reachedWaypoint && this._mode > 0)
		{
			this._waypoint = waypoint;
			base.CancelInvoke("WalkTimeOut");
			base.Invoke("WalkTimeOut", 30f);
			this._reachedWaypoint = false;
		}
		this._waypoint.y = this._collider.transform.position.y;
		this._lerpCounter = 0;
		if (Random.value < this._randomDeath)
		{
			this.Death();
		}
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00009524 File Offset: 0x00007724
	public void Init()
	{
		if (this._controller != null)
		{
			base.InvokeRepeating("Effects", 1f + Random.value, 0.1f);
		}
		base.InvokeRepeating("Wander", 1f + Random.value, 1f);
		base.InvokeRepeating("GroundCheck", this._groundCheckInterval * Random.value + 1f, this._groundCheckInterval);
		base.InvokeRepeating("FindLeader", Random.value * 3f, 3f);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x000095B4 File Offset: 0x000077B4
	public void Start()
	{
		this._thisTR = base.transform;
		this._enabled = true;
		this._groundIndex = LayerMask.NameToLayer(this._groundTag);
		this._herdSimIndex = LayerMask.NameToLayer(this._herdSimLayerName);
		if (this._updateDivisor > 1)
		{
			int num = this._updateDivisor - 1;
			HerdSimCore._updateNextSeed++;
			this._updateSeed = HerdSimCore._updateNextSeed;
			HerdSimCore._updateNextSeed %= num;
		}
		if (this._groundTag == null)
		{
			this._groundTag = "Ground";
		}
		this.Init();
		this._startPosition = this._thisTR.position;
		if (this._pushDistance <= 0f)
		{
			this._pushDistance = this._avoidDistance * 0.25f;
		}
		if (this._stopDistance <= 0f)
		{
			this._stopDistance = this._avoidDistance * 0.25f;
		}
		this._ground = (this._waypoint = this._thisTR.position);
		float maxFall = this._maxFall;
		this._maxFall = 1000000f;
		this.GroundCheck();
		this._maxFall = maxFall;
		if (this._collider == null)
		{
			this._collider = this._thisTR.Find("Collider");
		}
		this._herdSize = Random.Range(this._minHerdSize, this._maxHerdSize);
		if (this._minSize < 1f)
		{
			this._thisTR.localScale = Vector3.one * Random.Range(this._minSize, 1f);
		}
		this._model.GetComponent<Animation>()[this._animIdle].speed = this._animIdleSpeed;
		this._model.GetComponent<Animation>()[this._animDead].speed = this._animDeadSpeed;
		this._model.GetComponent<Animation>()[this._animSleep].speed = this._animSleepSpeed;
	}

	// Token: 0x060000FE RID: 254 RVA: 0x0000979C File Offset: 0x0000799C
	public void AnimationHandler()
	{
		if (!this._dead)
		{
			if (this._mode == 1)
			{
				if (this._speed > 0f)
				{
					this._model.GetComponent<Animation>()[this._animWalk].speed = this._speed * this._animWalkSpeed + 0.051f;
				}
				else
				{
					this._model.GetComponent<Animation>()[this._animWalk].speed = 0.1f;
				}
				this._model.GetComponent<Animation>().CrossFade(this._animWalk, 0.5f);
				this._idle = false;
				return;
			}
			if (this._mode == 2)
			{
				if (this._speed > this._runSpeed * 0.35f)
				{
					this._model.GetComponent<Animation>().CrossFade(this._animRun, 0.5f);
					this._model.GetComponent<Animation>()[this._animRun].speed = this._speed * this._animRunSpeed + 0.051f;
				}
				else
				{
					this._model.GetComponent<Animation>().CrossFade(this._animWalk, 0.5f);
					this._model.GetComponent<Animation>()[this._animWalk].speed = this._speed * this._animWalkSpeed + 0.051f;
				}
				this._idle = false;
				return;
			}
			if (!this._idle && this._speed < 0.5f)
			{
				this._sleepCounter = 0f;
				this._model.GetComponent<Animation>().CrossFade(this._animIdle, 1f);
				this._idle = true;
			}
			if (this._idle && this._sleepCounter > this._idleToSleepSeconds)
			{
				this._model.GetComponent<Animation>().CrossFade(this._animSleep, 1f);
				return;
			}
			this._sleepCounter += this._newDelta;
		}
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00009984 File Offset: 0x00007B84
	public void Scare(Transform t)
	{
		if (this._scaredOf == null)
		{
			this._scaredOf = t;
		}
		this._mode = 2;
		if (!this._scared)
		{
			this._scared = true;
			this.UnFlock();
			base.Invoke("EndScare", 3f);
			return;
		}
		if (Vector3.Distance(this._scaredOf.position, this._thisTR.position) > Vector3.Distance(t.position, this._thisTR.position))
		{
			this._scaredOf = t;
		}
	}

	// Token: 0x06000100 RID: 256 RVA: 0x00009A0D File Offset: 0x00007C0D
	public void EndScare()
	{
		this._scared = false;
		this.Wander();
		this._reachedWaypoint = true;
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00009A23 File Offset: 0x00007C23
	public void Food(Transform t)
	{
		if (this._food == null)
		{
			this._food = t;
		}
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00009A3C File Offset: 0x00007C3C
	public void Pushy()
	{
		RaycastHit raycastHit = default(RaycastHit);
		Vector3 forward = this._scanner.forward;
		if (this._scan)
		{
			this._scanner.Rotate(new Vector3(0f, 1000f * this._newDelta, 0f));
		}
		else
		{
			this._scanner.Rotate(new Vector3(0f, 250f * this._newDelta, 0f));
		}
		if (!Physics.Raycast(this._collider.transform.position, forward, out raycastHit, this._pushDistance, this._pushyLayerMask))
		{
			this._scan = true;
			return;
		}
		Transform transform = raycastHit.transform;
		if (transform.gameObject.layer != this._groundIndex || (transform.gameObject.layer == this._groundIndex && Vector3.Angle(Vector3.up, raycastHit.normal) > this._maxGroundAngle))
		{
			float distance = raycastHit.distance;
			float num = (this._pushDistance - distance) / this._pushDistance;
			if (base.gameObject.layer != this._herdSimIndex)
			{
				this._thisTR.position -= forward * this._newDelta * num * this._pushForce;
			}
			else if (distance < this._pushDistance * 0.5f)
			{
				this._thisTR.position -= forward * this._newDelta * (num - 0.5f) * this._pushForce;
			}
			this._scan = false;
			return;
		}
		this._scan = true;
	}

	// Token: 0x06000103 RID: 259 RVA: 0x00009BF8 File Offset: 0x00007DF8
	public void UnFlock()
	{
		if (this._leader != null && this._leader != this)
		{
			this._reachedWaypoint = true;
			this._leader._leaderSize--;
			this._leader = null;
			this.Wander();
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00009C48 File Offset: 0x00007E48
	public void WalkTimeOut()
	{
		this._reachedWaypoint = true;
		this.UnFlock();
		this.Wander();
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00009C60 File Offset: 0x00007E60
	public void Update()
	{
		if (this._updateDivisor > 1)
		{
			this._updateCounter++;
			if (this._updateCounter != this._updateSeed)
			{
				this._updateCounter %= this._updateDivisor;
				return;
			}
			this._updateCounter %= this._updateDivisor;
			this._newDelta = Time.deltaTime * (float)this._updateDivisor;
		}
		else
		{
			this._newDelta = Time.deltaTime;
		}
		if ((!this._pushHalfTheTime || this._pushToggle) && this._mode > 0)
		{
			this.Pushy();
		}
		this._pushToggle = !this._pushToggle;
		Vector3 position = this._thisTR.position;
		position.y -= (this._thisTR.position.y - this._ground.y) * this._newDelta * this._fakeGravity;
		this._thisTR.position = position;
		if (!this._dead)
		{
			this.AnimationHandler();
			Vector3 vector = Vector3.zero;
			Quaternion b = Quaternion.identity;
			this._model.transform.rotation = Quaternion.Slerp(this._model.transform.rotation, this._groundRot, this._newDelta * 5f);
			Quaternion localRotation = this._model.transform.localRotation;
			localRotation.eulerAngles = new Vector3(localRotation.eulerAngles.x, 0f, localRotation.eulerAngles.y);
			this._model.transform.localRotation = localRotation;
			if (!this._scared && this._mode > 0)
			{
				vector = this._waypoint - this._thisTR.position;
				if (vector != Vector3.zero)
				{
					b = Quaternion.LookRotation(vector);
				}
			}
			else if (this._scared && this._scaredOf != null)
			{
				vector = this._scaredOf.position - this._thisTR.position;
				if (vector != Vector3.zero)
				{
					b = Quaternion.LookRotation(-vector);
				}
			}
			if ((this._thisTR.position - this._waypoint).sqrMagnitude < 10f)
			{
				if (this._mode > 0)
				{
					this._mode = 1;
				}
				this._reachedWaypoint = true;
			}
			else
			{
				this._eating = false;
			}
			if (this._scared || (this._leader != null && this._leader != this && this._leader._mode == 2))
			{
				this._mode = 2;
			}
			else if (this._eating)
			{
				this._mode = 0;
			}
			if (this._mode == 1)
			{
				if (this._leader != this)
				{
					this._targetSpeed = this._walkSpeed;
				}
				else
				{
					this._targetSpeed = this._walkSpeed * 0.75f;
				}
			}
			else if (this._mode == 2)
			{
				this._targetSpeed = this._runSpeed;
			}
			this._speed = Mathf.Lerp(this._speed, this._targetSpeed, (float)this._lerpCounter * this._newDelta * 0.05f);
			this._lerpCounter++;
			if (this._speed > 0.01f && !this.Avoidance())
			{
				this._thisTR.rotation = Quaternion.Slerp(this._thisTR.rotation, b, this._newDelta * this._damping);
			}
			if (this._mode == 1)
			{
				this._targetSpeed = this._walkSpeed;
			}
			else if (this._mode == 2)
			{
				this._targetSpeed = this._runSpeed;
			}
			else if (this._mode <= 0)
			{
				this._targetSpeed = 0f;
			}
			this._thisTR.rotation = Quaternion.Euler(0f, this._thisTR.rotation.eulerAngles.y, 0f);
		}
		if (!this._grounded)
		{
			this._scared = false;
			this.UnFlock();
			Vector3 position2 = Vector3.zero;
			position2 = this._thisTR.position;
			position2.x -= (this._thisTR.position.x - this._ground.x) * this._newDelta * 15f;
			position2.z -= (this._thisTR.position.z - this._ground.z) * this._newDelta * 15f;
			this._thisTR.position = position2;
			return;
		}
		if (!this._dead)
		{
			this._thisTR.position += this._thisTR.TransformDirection(Vector3.forward) * this._speed * this._newDelta;
		}
	}

	// Token: 0x06000106 RID: 262 RVA: 0x0000A12C File Offset: 0x0000832C
	public void GroundCheck()
	{
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.Raycast(new Vector3(this._thisTR.position.x, this._collider.transform.position.y, this._thisTR.position.z), -this._thisTR.up, out raycastHit, this._maxFall, this._groundLayerMask))
		{
			this._grounded = true;
			this._groundRot = Quaternion.FromToRotation(this._model.transform.up, raycastHit.normal) * this._model.transform.rotation;
			this._ground = raycastHit.point;
			return;
		}
		this._grounded = false;
		this._waypoint = this._thisTR.position + this._thisTR.right * 5f;
		this._speed = 0f;
	}

	// Token: 0x06000107 RID: 263 RVA: 0x0000A230 File Offset: 0x00008430
	public bool Avoidance()
	{
		bool result = false;
		RaycastHit raycastHit = default(RaycastHit);
		Vector3 forward = this._model.transform.forward;
		Vector3 right = this._model.transform.right;
		float num = Mathf.Clamp(this._speed, 0.5f, 1f);
		Quaternion rotation = Quaternion.identity;
		if (this._mode == 0 && this._speed < 0.21f)
		{
			return true;
		}
		if (this._mode > 0 && this._rotateCounterR == 0f && Physics.Raycast(this._collider.transform.position, forward + right * (this._avoidAngle + this._rotateCounterL), out raycastHit, this._avoidDistance, this._pushyLayerMask))
		{
			Transform transform = raycastHit.transform;
			if (transform.gameObject.layer != this._groundIndex || (transform.gameObject.layer == this._groundIndex && Vector3.Angle(Vector3.up, raycastHit.normal) > this._maxGroundAngle))
			{
				this._rotateCounterL += this._newDelta;
				float num2 = (this._avoidDistance - raycastHit.distance) / this._avoidDistance;
				rotation = this._thisTR.rotation;
				rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y - this._avoidSpeed * this._newDelta * num2 * this._rotateCounterL * num, rotation.eulerAngles.z);
				this._thisTR.rotation = rotation;
				if (this._rotateCounterL > 1.5f)
				{
					this._rotateCounterL = 1.5f;
					this._rotateCounterR = 0f;
					result = true;
				}
			}
		}
		else if (this._mode > 0 && this._rotateCounterL == 0f && Physics.Raycast(this._collider.transform.position, forward + right * -(this._avoidAngle + this._rotateCounterR), out raycastHit, this._avoidDistance, this._pushyLayerMask))
		{
			Transform transform = raycastHit.transform;
			if (transform.gameObject.layer != this._groundIndex || (transform.gameObject.layer == this._groundIndex && Vector3.Angle(Vector3.up, raycastHit.normal) > this._maxGroundAngle))
			{
				this._rotateCounterR += this._newDelta;
				float num2 = (this._avoidDistance - raycastHit.distance) / this._avoidDistance;
				rotation = this._thisTR.rotation;
				rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y + this._avoidSpeed * this._newDelta * num2 * this._rotateCounterR * num, rotation.eulerAngles.z);
				this._thisTR.rotation = rotation;
				if (this._rotateCounterR > 1.5f)
				{
					this._rotateCounterR = 1.5f;
					this._rotateCounterL = 0f;
					result = true;
				}
			}
		}
		else
		{
			this._rotateCounterL -= this._newDelta;
			if (this._rotateCounterL < 0f)
			{
				this._rotateCounterL = 0f;
			}
			this._rotateCounterR -= this._newDelta;
			if (this._rotateCounterR < 0f)
			{
				this._rotateCounterR = 0f;
			}
		}
		if (Physics.Raycast(this._collider.transform.position, forward + right * Random.Range(-0.1f, 0.1f), out raycastHit, this._avoidDistance * 0.9f, this._pushyLayerMask))
		{
			Transform transform = raycastHit.transform;
			if (transform.gameObject.layer != this._groundIndex || (transform.gameObject.layer == this._groundIndex && Vector3.Angle(Vector3.up, raycastHit.normal) > this._maxGroundAngle))
			{
				float distance = raycastHit.distance;
				float num2 = (this._avoidDistance - raycastHit.distance) / this._avoidDistance;
				rotation = this._thisTR.rotation;
				if (this._rotateCounterL > this._rotateCounterR)
				{
					rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y - this._avoidSpeed * this._newDelta * num2 * this._rotateCounterL, rotation.eulerAngles.z);
				}
				else
				{
					rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y + this._avoidSpeed * this._newDelta * num2 * this._rotateCounterR, rotation.eulerAngles.z);
				}
				base.transform.rotation = rotation;
				if (distance < this._stopDistance * 0.5f)
				{
					this._speed = -0.2f;
					result = true;
				}
				if (distance < this._stopDistance && this._speed > 0.2f)
				{
					this._speed -= this._newDelta * (1f - num2) * 25f;
				}
				if (this._speed < -0.2f)
				{
					this._speed = -0.2f;
				}
			}
		}
		return result;
	}

	// Token: 0x06000108 RID: 264 RVA: 0x0000A7B4 File Offset: 0x000089B4
	public void OnDrawGizmos()
	{
		GUIStyle guistyle = new GUIStyle();
		Color blue = Color.blue;
		Color color = new Color32(0, byte.MaxValue, 246, byte.MaxValue);
		Color color2 = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
		guistyle.normal.textColor = Color.yellow;
		if (!Application.isPlaying)
		{
			this._startPosition = base.transform.position;
		}
		else
		{
			Gizmos.color = color;
			Gizmos.DrawLine(this._collider.transform.position, this._waypoint);
		}
		if (this._controller == null)
		{
			Gizmos.color = blue;
			Gizmos.DrawWireCube(this._startPosition, this._roamingArea * 2f);
		}
		if (this._leader == this)
		{
			Gizmos.color = color2;
			Gizmos.DrawWireCube(this._thisTR.position, new Vector3(this._leaderArea.x * 2f, 0f, this._leaderArea.y * 2f));
			Gizmos.DrawIcon(this._collider.transform.position, "leader.png", false);
		}
		else if (this._leader != null)
		{
			Gizmos.color = color2;
			Gizmos.DrawLine(this._collider.transform.position, this._leader._collider.transform.position);
		}
		if (this._scared)
		{
			Gizmos.DrawIcon(this._collider.transform.position, "scared.png", false);
		}
		if (this._dead)
		{
			Gizmos.DrawIcon(this._collider.transform.position, "dead.png", false);
		}
	}

	// Token: 0x04000189 RID: 393
	public HerdSimController _controller;

	// Token: 0x0400018A RID: 394
	public Transform _scanner;

	// Token: 0x0400018B RID: 395
	public Transform _collider;

	// Token: 0x0400018C RID: 396
	public Transform _model;

	// Token: 0x0400018D RID: 397
	public Renderer _renderer;

	// Token: 0x0400018E RID: 398
	public float _hitPoints = 100f;

	// Token: 0x0400018F RID: 399
	public int _type;

	// Token: 0x04000190 RID: 400
	public float _minSize = 1f;

	// Token: 0x04000191 RID: 401
	public float _avoidAngle = 0.35f;

	// Token: 0x04000192 RID: 402
	public float _avoidDistance;

	// Token: 0x04000193 RID: 403
	public float _avoidSpeed = 75f;

	// Token: 0x04000194 RID: 404
	public float _stopDistance;

	// Token: 0x04000195 RID: 405
	private float _rotateCounterR;

	// Token: 0x04000196 RID: 406
	private float _rotateCounterL;

	// Token: 0x04000197 RID: 407
	public bool _pushHalfTheTime;

	// Token: 0x04000198 RID: 408
	private bool _pushToggle;

	// Token: 0x04000199 RID: 409
	public float _pushDistance;

	// Token: 0x0400019A RID: 410
	public float _pushForce = 5f;

	// Token: 0x0400019B RID: 411
	private bool _scan;

	// Token: 0x0400019C RID: 412
	public Vector3 _roamingArea;

	// Token: 0x0400019D RID: 413
	public float _walkSpeed = 0.5f;

	// Token: 0x0400019E RID: 414
	public float _runSpeed = 1.5f;

	// Token: 0x0400019F RID: 415
	public float _damping;

	// Token: 0x040001A0 RID: 416
	public int _idleProbablity = 20;

	// Token: 0x040001A1 RID: 417
	public float _runChance = 0.1f;

	// Token: 0x040001A2 RID: 418
	public Vector3 _waypoint;

	// Token: 0x040001A3 RID: 419
	public float _speed;

	// Token: 0x040001A4 RID: 420
	public float _targetSpeed;

	// Token: 0x040001A5 RID: 421
	public int _mode;

	// Token: 0x040001A6 RID: 422
	public Vector3 _startPosition;

	// Token: 0x040001A7 RID: 423
	private bool _reachedWaypoint = true;

	// Token: 0x040001A8 RID: 424
	private int _lerpCounter;

	// Token: 0x040001A9 RID: 425
	public bool _scared;

	// Token: 0x040001AA RID: 426
	public Transform _scaredOf;

	// Token: 0x040001AB RID: 427
	public bool _eating;

	// Token: 0x040001AC RID: 428
	public Transform _food;

	// Token: 0x040001AD RID: 429
	public float _groundCheckInterval = 0.1f;

	// Token: 0x040001AE RID: 430
	public string _groundTag = "Ground";

	// Token: 0x040001AF RID: 431
	private Vector3 _ground;

	// Token: 0x040001B0 RID: 432
	private Quaternion _groundRot;

	// Token: 0x040001B1 RID: 433
	private bool _grounded;

	// Token: 0x040001B2 RID: 434
	public float _maxGroundAngle = 45f;

	// Token: 0x040001B3 RID: 435
	public float _maxFall = 3f;

	// Token: 0x040001B4 RID: 436
	public float _fakeGravity = 5f;

	// Token: 0x040001B5 RID: 437
	public LayerMask _herdLayerMask = -1;

	// Token: 0x040001B6 RID: 438
	public HerdSimCore _leader;

	// Token: 0x040001B7 RID: 439
	public Vector3 _leaderArea;

	// Token: 0x040001B8 RID: 440
	public int _leaderSize;

	// Token: 0x040001B9 RID: 441
	public float _leaderAreaMultiplier = 0.2f;

	// Token: 0x040001BA RID: 442
	public int _maxHerdSize = 25;

	// Token: 0x040001BB RID: 443
	public int _minHerdSize = 10;

	// Token: 0x040001BC RID: 444
	public float _herdDistance = 2f;

	// Token: 0x040001BD RID: 445
	private int _herdSize;

	// Token: 0x040001BE RID: 446
	public bool _dead;

	// Token: 0x040001BF RID: 447
	public float _randomDeath = 0.001f;

	// Token: 0x040001C0 RID: 448
	public Material _deadMaterial;

	// Token: 0x040001C1 RID: 449
	public bool _scaryCorpse;

	// Token: 0x040001C2 RID: 450
	public string _animIdle = "idle";

	// Token: 0x040001C3 RID: 451
	public float _animIdleSpeed = 1f;

	// Token: 0x040001C4 RID: 452
	public string _animSleep = "sleep";

	// Token: 0x040001C5 RID: 453
	public float _animSleepSpeed = 1f;

	// Token: 0x040001C6 RID: 454
	public string _animWalk = "walk";

	// Token: 0x040001C7 RID: 455
	public float _animWalkSpeed = 1f;

	// Token: 0x040001C8 RID: 456
	public string _animRun = "run";

	// Token: 0x040001C9 RID: 457
	public float _animRunSpeed = 1f;

	// Token: 0x040001CA RID: 458
	public string _animDead = "dead";

	// Token: 0x040001CB RID: 459
	public float _animDeadSpeed = 1f;

	// Token: 0x040001CC RID: 460
	public float _idleToSleepSeconds = 1f;

	// Token: 0x040001CD RID: 461
	private float _sleepCounter;

	// Token: 0x040001CE RID: 462
	private bool _idle;

	// Token: 0x040001CF RID: 463
	private int _updateCounter;

	// Token: 0x040001D0 RID: 464
	public int _updateDivisor = 1;

	// Token: 0x040001D1 RID: 465
	private static int _updateNextSeed;

	// Token: 0x040001D2 RID: 466
	private int _updateSeed = -1;

	// Token: 0x040001D3 RID: 467
	private float _newDelta;

	// Token: 0x040001D4 RID: 468
	public bool _enabled;

	// Token: 0x040001D5 RID: 469
	public LayerMask _groundLayerMask = -1;

	// Token: 0x040001D6 RID: 470
	public LayerMask _pushyLayerMask = -1;

	// Token: 0x040001D7 RID: 471
	public string _herdSimLayerName = "HerdSim";

	// Token: 0x040001D8 RID: 472
	private int _groundIndex = 25;

	// Token: 0x040001D9 RID: 473
	private int _herdSimIndex = 26;

	// Token: 0x040001DA RID: 474
	private Transform _thisTR;
}
