using System;
using System.Collections;
using Logic;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class RebelReinforcements : MonoBehaviour
{
	// Token: 0x060012C5 RID: 4805 RVA: 0x000C3EF4 File Offset: 0x000C20F4
	public void Awake()
	{
		this.anim = base.gameObject.AddComponent<UnitAnimation>();
		this.movement = new ReiforcmentsMovement(base.gameObject);
		this.movement.speed = 3.1f;
		ReiforcmentsMovement reiforcmentsMovement = this.movement;
		reiforcmentsMovement.onMove = (Action<Point>)Delegate.Combine(reiforcmentsMovement.onMove, new Action<Point>(this.HandleOnMove));
		ReiforcmentsMovement reiforcmentsMovement2 = this.movement;
		reiforcmentsMovement2.onPathChanged = (Action)Delegate.Combine(reiforcmentsMovement2.onPathChanged, new Action(this.HandleOnPathChanged));
		ReiforcmentsMovement reiforcmentsMovement3 = this.movement;
		reiforcmentsMovement3.onDestinationReached = (Action<MapObject>)Delegate.Combine(reiforcmentsMovement3.onDestinationReached, new Action<MapObject>(this.HandleOnDestinationReached));
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x000C3FA8 File Offset: 0x000C21A8
	private void OnDestroy()
	{
		if (this.movement != null)
		{
			this.movement.onMove = null;
			this.movement.onPathChanged = null;
			this.movement.onDestinationReached = null;
			this.movement = null;
		}
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x000C3FDD File Offset: 0x000C21DD
	private void Start()
	{
		global::Common.SetRendererLayer(base.gameObject, base.gameObject.layer);
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x000023FD File Offset: 0x000005FD
	private void HandleOnPathChanged()
	{
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x000C3FF8 File Offset: 0x000C21F8
	public void SetTarget(global::Rebel rebel)
	{
		this.target = rebel;
		if (this.target != null && this.target.logic != null && this.target.logic.army != null)
		{
			this.MoveTo(rebel.logic.army.position);
		}
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x000C4050 File Offset: 0x000C2250
	public void MoveTo(Point point)
	{
		if (this.movement == null)
		{
			Debug.Log("movement is null");
			return;
		}
		this.movement.MoveTo(point, 0f);
		if (this.anim != null)
		{
			this.anim.SetState(UnitAnimation.State.Move, WrapMode.Loop, -1f, false, -1f);
		}
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x000C40A7 File Offset: 0x000C22A7
	public void MoveTo(MapObject mapObj)
	{
		ReiforcmentsMovement reiforcmentsMovement = this.movement;
		if (reiforcmentsMovement == null)
		{
			return;
		}
		reiforcmentsMovement.MoveTo(mapObj, 0f);
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x000C40C0 File Offset: 0x000C22C0
	private void Update()
	{
		if (GameLogic.instance == null || GameLogic.instance.logic.path_finding == null)
		{
			this.Die();
			return;
		}
		if (!this.despawning)
		{
			if (this.target == null)
			{
				this.despawning = true;
				this.MoveTo(this.origin);
				return;
			}
			if (this.target.logic == null || !this.target.logic.IsValid())
			{
				this.despawning = true;
				this.MoveTo(this.origin);
				return;
			}
			if (this.target.logic.current_action != Logic.Rebel.Action.Rest)
			{
				this.despawning = true;
				this.MoveTo(this.origin);
				return;
			}
		}
		ReiforcmentsMovement reiforcmentsMovement = this.movement;
		if (reiforcmentsMovement == null)
		{
			return;
		}
		reiforcmentsMovement.OnUpdate();
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x000C4180 File Offset: 0x000C2380
	private void HandleOnMove(Point point)
	{
		Vector3 vector = new Vector3(point.x, base.transform.position.y, point.y) - base.transform.position;
		if (vector.magnitude < 0.001f)
		{
			return;
		}
		float num = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		num = 90f - num;
		if (num < 0f)
		{
			num += 360f;
		}
		this.tgt_facing = num;
		if (this.tgt_facing < 0f)
		{
			return;
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		float num2 = eulerAngles.y - this.tgt_facing;
		if (num2 < -180f)
		{
			num2 += 360f;
		}
		else if (num2 > 180f)
		{
			num2 -= 360f;
		}
		if (num2 < -120f || num2 > 120f)
		{
			eulerAngles.y = this.tgt_facing;
		}
		else
		{
			eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, this.tgt_facing, this.RotationSpeed * UnityEngine.Time.deltaTime);
		}
		base.transform.eulerAngles = eulerAngles;
		Vector3 position = global::Common.SnapToTerrain(point, 0f, WorldMap.GetTerrain(), -1f, false);
		base.transform.position = position;
		this.anim.SetState(UnitAnimation.State.Move, WrapMode.Loop, -1f, false, -1f);
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x000C42E0 File Offset: 0x000C24E0
	private void HandleOnDestinationReached(MapObject mapObject)
	{
		this.anim.SetState(UnitAnimation.State.Idle, WrapMode.Loop, -1f, false, -1f);
		this.hideRoutine = this.Hide(0.6f);
		base.StartCoroutine(this.hideRoutine);
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x000C4318 File Offset: 0x000C2518
	public void SetPosition(Point point)
	{
		base.transform.position = new Vector3(point.x, 16f, point.y);
		this.origin = point;
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x0006FA88 File Offset: 0x0006DC88
	public void SetPosition(Vector3 pos)
	{
		base.transform.position = global::Common.SnapToTerrain(pos, 0f, null, -1f, false);
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x000C4342 File Offset: 0x000C2542
	private IEnumerator Hide(float duration)
	{
		float rest = duration;
		Vector3 os = base.transform.localScale;
		for (;;)
		{
			rest -= UnityEngine.Time.deltaTime;
			float d = rest / duration;
			base.transform.localScale = os * d;
			if (rest <= 0f)
			{
				break;
			}
			yield return null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
		yield break;
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x000C4358 File Offset: 0x000C2558
	private void Die()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x04000C97 RID: 3223
	private UnitAnimation anim;

	// Token: 0x04000C98 RID: 3224
	private ReiforcmentsMovement movement;

	// Token: 0x04000C99 RID: 3225
	private float tgt_facing = -1f;

	// Token: 0x04000C9A RID: 3226
	private float RotationSpeed = 90f;

	// Token: 0x04000C9B RID: 3227
	private global::Rebel target;

	// Token: 0x04000C9C RID: 3228
	private Point origin;

	// Token: 0x04000C9D RID: 3229
	private bool despawning;

	// Token: 0x04000C9E RID: 3230
	private IEnumerator hideRoutine;
}
