using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class SchoolController : MonoBehaviour
{
	// Token: 0x060000E6 RID: 230 RVA: 0x0000865C File Offset: 0x0000685C
	public void Start()
	{
		this._posBuffer = base.transform.position + this._posOffset;
		this._schoolSpeed = Random.Range(1f, this._childSpeedMultipler);
		this.AddFish(this._childAmount);
		base.Invoke("AutoRandomWaypointPosition", this.RandomWaypointTime());
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x000086B8 File Offset: 0x000068B8
	public void Update()
	{
		if (this._activeChildren > 0)
		{
			if (this._updateDivisor > 1)
			{
				this._updateCounter++;
				this._updateCounter %= this._updateDivisor;
				this._newDelta = Time.deltaTime * (float)this._updateDivisor;
			}
			else
			{
				this._newDelta = Time.deltaTime;
			}
			this.UpdateFishAmount();
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00008720 File Offset: 0x00006920
	public void InstantiateGroup()
	{
		if (this._groupTransform != null)
		{
			return;
		}
		GameObject gameObject = new GameObject();
		this._groupTransform = gameObject.transform;
		this._groupTransform.position = base.transform.position;
		if (this._groupName != "")
		{
			gameObject.name = this._groupName;
			return;
		}
		gameObject.name = base.transform.name + " Fish Container";
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x000087A0 File Offset: 0x000069A0
	public void AddFish(int amount)
	{
		if (this._groupChildToNewTransform)
		{
			this.InstantiateGroup();
		}
		for (int i = 0; i < amount; i++)
		{
			int num = Random.Range(0, this._childPrefab.Length);
			SchoolChild schoolChild = Object.Instantiate<SchoolChild>(this._childPrefab[num]);
			schoolChild._spawner = this;
			this._roamers.Add(schoolChild);
			this.AddChildToParent(schoolChild.transform);
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00008803 File Offset: 0x00006A03
	public void AddChildToParent(Transform obj)
	{
		if (this._groupChildToSchool)
		{
			obj.parent = base.transform;
			return;
		}
		if (this._groupChildToNewTransform)
		{
			obj.parent = this._groupTransform;
			return;
		}
	}

	// Token: 0x060000EB RID: 235 RVA: 0x0000882F File Offset: 0x00006A2F
	public void RemoveFish(int amount)
	{
		Component component = this._roamers[this._roamers.Count - 1];
		this._roamers.RemoveAt(this._roamers.Count - 1);
		Object.Destroy(component.gameObject);
	}

	// Token: 0x060000EC RID: 236 RVA: 0x0000886C File Offset: 0x00006A6C
	public void UpdateFishAmount()
	{
		if (this._childAmount >= 0 && this._childAmount < this._roamers.Count)
		{
			this.RemoveFish(1);
			return;
		}
		if (this._childAmount > this._roamers.Count)
		{
			this.AddFish(1);
			return;
		}
	}

	// Token: 0x060000ED RID: 237 RVA: 0x000088B8 File Offset: 0x00006AB8
	public void SetRandomWaypointPosition()
	{
		this._schoolSpeed = Random.Range(1f, this._childSpeedMultipler);
		Vector3 zero = Vector3.zero;
		zero.x = Random.Range(-this._positionSphere, this._positionSphere) + base.transform.position.x;
		zero.z = Random.Range(-this._positionSphereDepth, this._positionSphereDepth) + base.transform.position.z;
		zero.y = Random.Range(-this._positionSphereHeight, this._positionSphereHeight) + base.transform.position.y;
		this._posBuffer = zero;
		if (this._forceChildWaypoints)
		{
			for (int i = 0; i < this._roamers.Count; i++)
			{
				this._roamers[i].Wander(Random.value * this._forcedRandomDelay);
			}
		}
	}

	// Token: 0x060000EE RID: 238 RVA: 0x000089A1 File Offset: 0x00006BA1
	public void AutoRandomWaypointPosition()
	{
		if (this._autoRandomPosition && this._activeChildren > 0)
		{
			this.SetRandomWaypointPosition();
		}
		base.CancelInvoke("AutoRandomWaypointPosition");
		base.Invoke("AutoRandomWaypointPosition", this.RandomWaypointTime());
	}

	// Token: 0x060000EF RID: 239 RVA: 0x000089D6 File Offset: 0x00006BD6
	public float RandomWaypointTime()
	{
		return Random.Range(this._randomPositionTimerMin, this._randomPositionTimerMax);
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x000089EC File Offset: 0x00006BEC
	public void OnDrawGizmos()
	{
		if (!Application.isPlaying && this._posBuffer != base.transform.position + this._posOffset)
		{
			this._posBuffer = base.transform.position + this._posOffset;
		}
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(this._posBuffer, new Vector3(this._spawnSphere * 2f, this._spawnSphereHeight * 2f, this._spawnSphereDepth * 2f));
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(base.transform.position, new Vector3(this._positionSphere * 2f + this._spawnSphere * 2f, this._positionSphereHeight * 2f + this._spawnSphereHeight * 2f, this._positionSphereDepth * 2f + this._spawnSphereDepth * 2f));
	}

	// Token: 0x04000154 RID: 340
	public SchoolChild[] _childPrefab;

	// Token: 0x04000155 RID: 341
	public bool _groupChildToNewTransform;

	// Token: 0x04000156 RID: 342
	public Transform _groupTransform;

	// Token: 0x04000157 RID: 343
	public string _groupName = "";

	// Token: 0x04000158 RID: 344
	public bool _groupChildToSchool;

	// Token: 0x04000159 RID: 345
	public int _childAmount = 250;

	// Token: 0x0400015A RID: 346
	public float _spawnSphere = 3f;

	// Token: 0x0400015B RID: 347
	public float _spawnSphereDepth = 3f;

	// Token: 0x0400015C RID: 348
	public float _spawnSphereHeight = 1.5f;

	// Token: 0x0400015D RID: 349
	public float _childSpeedMultipler = 2f;

	// Token: 0x0400015E RID: 350
	public float _minSpeed = 6f;

	// Token: 0x0400015F RID: 351
	public float _maxSpeed = 10f;

	// Token: 0x04000160 RID: 352
	public AnimationCurve _speedCurveMultiplier = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04000161 RID: 353
	public float _minScale = 0.7f;

	// Token: 0x04000162 RID: 354
	public float _maxScale = 1f;

	// Token: 0x04000163 RID: 355
	public float _minDamping = 1f;

	// Token: 0x04000164 RID: 356
	public float _maxDamping = 2f;

	// Token: 0x04000165 RID: 357
	public float _waypointDistance = 1f;

	// Token: 0x04000166 RID: 358
	public float _minAnimationSpeed = 2f;

	// Token: 0x04000167 RID: 359
	public float _maxAnimationSpeed = 4f;

	// Token: 0x04000168 RID: 360
	public float _randomPositionTimerMax = 10f;

	// Token: 0x04000169 RID: 361
	public float _randomPositionTimerMin = 4f;

	// Token: 0x0400016A RID: 362
	public float _acceleration = 0.025f;

	// Token: 0x0400016B RID: 363
	public float _brake = 0.01f;

	// Token: 0x0400016C RID: 364
	public float _positionSphere = 25f;

	// Token: 0x0400016D RID: 365
	public float _positionSphereDepth = 5f;

	// Token: 0x0400016E RID: 366
	public float _positionSphereHeight = 5f;

	// Token: 0x0400016F RID: 367
	public bool _childTriggerPos;

	// Token: 0x04000170 RID: 368
	public bool _forceChildWaypoints;

	// Token: 0x04000171 RID: 369
	public bool _autoRandomPosition;

	// Token: 0x04000172 RID: 370
	public float _forcedRandomDelay = 1.5f;

	// Token: 0x04000173 RID: 371
	public float _schoolSpeed;

	// Token: 0x04000174 RID: 372
	public List<SchoolChild> _roamers;

	// Token: 0x04000175 RID: 373
	public Vector3 _posBuffer;

	// Token: 0x04000176 RID: 374
	public Vector3 _posOffset;

	// Token: 0x04000177 RID: 375
	public bool _avoidance;

	// Token: 0x04000178 RID: 376
	public float _avoidAngle = 0.35f;

	// Token: 0x04000179 RID: 377
	public float _avoidDistance = 1f;

	// Token: 0x0400017A RID: 378
	public float _avoidSpeed = 75f;

	// Token: 0x0400017B RID: 379
	public float _stopDistance = 0.5f;

	// Token: 0x0400017C RID: 380
	public float _stopSpeedMultiplier = 2f;

	// Token: 0x0400017D RID: 381
	public LayerMask _avoidanceMask = -1;

	// Token: 0x0400017E RID: 382
	public bool _push;

	// Token: 0x0400017F RID: 383
	public float _pushDistance;

	// Token: 0x04000180 RID: 384
	public float _pushForce = 5f;

	// Token: 0x04000181 RID: 385
	public SchoolBubbles _bubbles;

	// Token: 0x04000182 RID: 386
	public int _updateDivisor = 1;

	// Token: 0x04000183 RID: 387
	public float _newDelta;

	// Token: 0x04000184 RID: 388
	public int _updateCounter;

	// Token: 0x04000185 RID: 389
	public int _activeChildren;
}
