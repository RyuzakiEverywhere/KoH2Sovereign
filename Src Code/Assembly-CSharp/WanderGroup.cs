using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031A RID: 794
[ExecuteInEditMode]
public class WanderGroup : MonoBehaviour
{
	// Token: 0x060031B9 RID: 12729 RVA: 0x0019323E File Offset: 0x0019143E
	private static bool floatEquals(float x, float y, float epsilon)
	{
		return Mathf.Abs(x - y) < epsilon;
	}

	// Token: 0x060031BA RID: 12730 RVA: 0x0019324C File Offset: 0x0019144C
	private void initData()
	{
		this.maxWalkDist = Mathf.Max(this.minWalkDist, this.maxWalkDist);
		this.maxIdleTime = Mathf.Max(this.minIdleTime, this.maxIdleTime);
		this.migartionMaxTime = Mathf.Max(this.migartionMinTime, this.migartionMaxTime);
		this.currentOrign = base.transform.position;
		this.nextMigrationCheck = Time.time + Random.Range(this.migartionMinTime, this.migartionMaxTime);
		this.migrationWaypoints.Add(base.transform.position);
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x001932E4 File Offset: 0x001914E4
	private void createGroup()
	{
		if (this.instancePrefab == null)
		{
			return;
		}
		for (int i = 0; i < this.numberOfInstances; i++)
		{
			new WanderGroup.Instance(this);
		}
	}

	// Token: 0x060031BC RID: 12732 RVA: 0x00193318 File Offset: 0x00191518
	private void Start()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.initData();
		if (this.instances.Count <= 0)
		{
			this.createGroup();
		}
	}

	// Token: 0x060031BD RID: 12733 RVA: 0x0019333C File Offset: 0x0019153C
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			this.refreshVisability();
			return;
		}
		if (this.instances.Count <= 0)
		{
			this.createGroup();
		}
	}

	// Token: 0x060031BE RID: 12734 RVA: 0x00193360 File Offset: 0x00191560
	private void OnValidate()
	{
		this.dirty = true;
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x0019336C File Offset: 0x0019156C
	private void Clear()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
		}
		foreach (GameObject obj2 in list)
		{
			Object.DestroyImmediate(obj2);
		}
		this.instances.Clear();
	}

	// Token: 0x060031C0 RID: 12736 RVA: 0x00193418 File Offset: 0x00191618
	private void Update()
	{
		if (Application.isPlaying)
		{
			this.checkMigration();
			foreach (WanderGroup.Instance instance in this.instances)
			{
				instance.act();
			}
			return;
		}
		if (this.dirty)
		{
			this.dirty = false;
			this.Clear();
			this.createGroup();
		}
		if (!base.transform.hasChanged)
		{
			return;
		}
		foreach (WanderGroup.Instance instance2 in this.instances)
		{
			instance2.snapToTerrain();
		}
		base.transform.hasChanged = false;
	}

	// Token: 0x060031C1 RID: 12737 RVA: 0x001934EC File Offset: 0x001916EC
	private void refreshVisability()
	{
		foreach (WanderGroup.Instance instance in this.instances)
		{
			instance.refreshVisability();
		}
	}

	// Token: 0x060031C2 RID: 12738 RVA: 0x0019353C File Offset: 0x0019173C
	private void checkMigration()
	{
		if (this.migrationWaypoints == null || this.migrationWaypoints.Count < 2)
		{
			return;
		}
		if (Time.time > this.nextMigrationCheck)
		{
			this.currentOrign = this.migrationWaypoints[Random.Range(0, this.migrationWaypoints.Count)];
			for (int i = 0; i < this.instances.Count; i++)
			{
				WanderGroup.Instance instance = this.instances[i];
				if (instance != null)
				{
					instance.migrating = true;
					instance.nextMoveTime = Time.time + (float)Random.Range(1, 2);
				}
			}
			this.nextMigrationCheck = Time.time + Random.Range(this.migartionMinTime, this.migartionMaxTime);
		}
	}

	// Token: 0x060031C3 RID: 12739 RVA: 0x001935F0 File Offset: 0x001917F0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x0400213C RID: 8508
	public int numberOfInstances = 10;

	// Token: 0x0400213D RID: 8509
	public float radius = 10f;

	// Token: 0x0400213E RID: 8510
	public float minWalkDist = 5f;

	// Token: 0x0400213F RID: 8511
	public float maxWalkDist = 10f;

	// Token: 0x04002140 RID: 8512
	public float minIdleTime = 5f;

	// Token: 0x04002141 RID: 8513
	public float maxIdleTime = 10f;

	// Token: 0x04002142 RID: 8514
	public float movementSpeed = 0.5f;

	// Token: 0x04002143 RID: 8515
	public float rotationSpeed = 90f;

	// Token: 0x04002144 RID: 8516
	public Vector2 sizeVariation = new Vector2(0.8f, 1.2f);

	// Token: 0x04002145 RID: 8517
	public GameObject instancePrefab;

	// Token: 0x04002146 RID: 8518
	public string idleAnim = "idle";

	// Token: 0x04002147 RID: 8519
	public string walkAnim = "walk";

	// Token: 0x04002148 RID: 8520
	public float walkSpeedMod = 1f;

	// Token: 0x04002149 RID: 8521
	public float idleSpeedMod = 1f;

	// Token: 0x0400214A RID: 8522
	[Header("Migration")]
	public float migartionMinTime = 120f;

	// Token: 0x0400214B RID: 8523
	public float migartionMaxTime = 300f;

	// Token: 0x0400214C RID: 8524
	private float nextMigrationCheck;

	// Token: 0x0400214D RID: 8525
	[HideInInspector]
	public List<Vector3> migrationWaypoints = new List<Vector3>();

	// Token: 0x0400214E RID: 8526
	private Vector3 currentOrign;

	// Token: 0x0400214F RID: 8527
	private List<WanderGroup.Instance> instances = new List<WanderGroup.Instance>();

	// Token: 0x04002150 RID: 8528
	private bool dirty;

	// Token: 0x02000881 RID: 2177
	private class Instance
	{
		// Token: 0x06005160 RID: 20832 RVA: 0x0023CE2B File Offset: 0x0023B02B
		public bool IsLegacyAnimation()
		{
			return this.animation != null;
		}

		// Token: 0x06005161 RID: 20833 RVA: 0x0023CE3C File Offset: 0x0023B03C
		public Instance(WanderGroup group)
		{
			this.group = group;
			this.obj = Object.Instantiate<GameObject>(group.instancePrefab);
			this.obj.transform.parent = group.transform;
			this.obj.hideFlags = HideFlags.DontSave;
			this.scale = Random.Range(group.sizeVariation.x, group.sizeVariation.y);
			this.obj.transform.localScale *= this.scale;
			this.nextMoveTime = -1f;
			this.idle = (Random.value > 0.5f);
			this.obj.transform.position = group.transform.position + Random.insideUnitSphere * group.radius;
			this.obj.transform.position = new Vector3(this.obj.transform.position.x, 0f, this.obj.transform.position.z);
			this.targetRotation = (float)Random.Range(0, 360);
			this.obj.transform.eulerAngles = new Vector3(0f, this.targetRotation, 0f);
			this.dstPos = this.obj.transform.position;
			this.snapToTerrain();
			this.animation = this.obj.GetComponentInChildren<Animation>();
			this.animator = this.obj.GetComponentInChildren<Animator>();
			this.applySpeedModidfers();
			group.instances.Add(this);
		}

		// Token: 0x06005162 RID: 20834 RVA: 0x0023CFE4 File Offset: 0x0023B1E4
		public void destroy()
		{
			Object.Destroy(this.obj);
		}

		// Token: 0x06005163 RID: 20835 RVA: 0x0023CFF1 File Offset: 0x0023B1F1
		public void setActive(bool value)
		{
			this.obj.SetActive(value);
		}

		// Token: 0x06005164 RID: 20836 RVA: 0x0023D000 File Offset: 0x0023B200
		private void applySpeedModidfers()
		{
			if (this.animation != null)
			{
				string walkAnim = this.group.walkAnim;
				this.animation[walkAnim].speed = this.group.walkSpeedMod;
				string idleAnim = this.group.idleAnim;
				this.animation[idleAnim].speed = this.group.idleSpeedMod;
				return;
			}
			if (this.animator != null)
			{
				this.animator.SetFloat("walkSpeedMod", this.group.walkSpeedMod);
				this.animator.SetFloat("idleSpeedMod", this.group.idleSpeedMod);
			}
		}

		// Token: 0x06005165 RID: 20837 RVA: 0x0023D0B0 File Offset: 0x0023B2B0
		private void setAnimation(string animState)
		{
			if (this.animation != null)
			{
				this.animation[animState].time = Random.Range(0f, this.animation[animState].length);
				this.animation.Play(animState);
				this.animation.wrapMode = WrapMode.Loop;
				return;
			}
			if (this.animator != null)
			{
				this.animator.SetTrigger(animState);
			}
		}

		// Token: 0x06005166 RID: 20838 RVA: 0x0023D12C File Offset: 0x0023B32C
		private bool srcEqualsDst()
		{
			return WanderGroup.floatEquals(this.obj.transform.position.x, this.dstPos.x, 0.1f) && WanderGroup.floatEquals(this.obj.transform.position.z, this.dstPos.z, 0.1f);
		}

		// Token: 0x06005167 RID: 20839 RVA: 0x0023D191 File Offset: 0x0023B391
		private bool hasCorrectOrientation()
		{
			return WanderGroup.floatEquals(this.obj.transform.eulerAngles.y, this.targetRotation, 0.1f);
		}

		// Token: 0x06005168 RID: 20840 RVA: 0x0023D1B8 File Offset: 0x0023B3B8
		public void act()
		{
			if (this.srcEqualsDst())
			{
				if (!this.idle)
				{
					this.nextMoveTime = Time.time + Random.Range(this.group.minIdleTime, this.group.maxIdleTime);
					this.setAnimation(this.IsLegacyAnimation() ? this.group.idleAnim : "idle");
					this.idle = true;
					return;
				}
				if (Time.time > this.nextMoveTime)
				{
					Vector3 a = this.GetCurrentOrigin() + Random.insideUnitSphere * this.group.radius;
					a.y = this.obj.transform.position.y;
					float magnitude = (a - this.obj.transform.position).magnitude;
					if (this.migrating || (magnitude >= this.group.minWalkDist && magnitude <= this.group.maxWalkDist))
					{
						this.dstPos = a;
						this.targetRotation = Mathf.Atan2(this.dstPos.z - this.obj.transform.position.z, this.dstPos.x - this.obj.transform.position.x) * 57.29578f;
						this.targetRotation = 90f - this.targetRotation;
						if (this.targetRotation < 0f)
						{
							this.targetRotation += 360f;
						}
						this.setAnimation(this.IsLegacyAnimation() ? this.group.walkAnim : "walk");
						this.idle = false;
						this.migrating = false;
						return;
					}
				}
			}
			else
			{
				if (!this.hasCorrectOrientation())
				{
					this.rotate();
				}
				this.move();
			}
		}

		// Token: 0x06005169 RID: 20841 RVA: 0x0023D38C File Offset: 0x0023B58C
		public void refreshVisability()
		{
			if (this.idle)
			{
				this.setAnimation(this.IsLegacyAnimation() ? this.group.idleAnim : "idle");
			}
			else
			{
				this.setAnimation(this.IsLegacyAnimation() ? this.group.walkAnim : "walk");
			}
			this.applySpeedModidfers();
		}

		// Token: 0x0600516A RID: 20842 RVA: 0x0023D3EC File Offset: 0x0023B5EC
		private void move()
		{
			Vector3 position = this.obj.transform.position;
			this.obj.transform.position = Vector3.MoveTowards(this.obj.transform.position, this.dstPos, this.scale * this.group.movementSpeed * Time.deltaTime);
			Debug.DrawLine(position, this.obj.transform.position, Color.magenta);
			this.snapToTerrain();
		}

		// Token: 0x0600516B RID: 20843 RVA: 0x0023D46C File Offset: 0x0023B66C
		public void snapToTerrain()
		{
			Common.SnapToTerrain(this.obj, 0f, null, -1f);
		}

		// Token: 0x0600516C RID: 20844 RVA: 0x0023D484 File Offset: 0x0023B684
		private void rotate()
		{
			Vector3 eulerAngles = this.obj.transform.eulerAngles;
			eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, this.targetRotation, this.group.rotationSpeed * Time.deltaTime);
			this.obj.transform.eulerAngles = eulerAngles;
		}

		// Token: 0x0600516D RID: 20845 RVA: 0x0023D4DC File Offset: 0x0023B6DC
		private Vector3 GetCurrentOrigin()
		{
			return this.group.currentOrign;
		}

		// Token: 0x04003FB9 RID: 16313
		private WanderGroup group;

		// Token: 0x04003FBA RID: 16314
		private GameObject obj;

		// Token: 0x04003FBB RID: 16315
		private float scale;

		// Token: 0x04003FBC RID: 16316
		private bool idle;

		// Token: 0x04003FBD RID: 16317
		public bool migrating;

		// Token: 0x04003FBE RID: 16318
		public float nextMoveTime;

		// Token: 0x04003FBF RID: 16319
		private Vector3 dstPos;

		// Token: 0x04003FC0 RID: 16320
		private float targetRotation;

		// Token: 0x04003FC1 RID: 16321
		private Animator animator;

		// Token: 0x04003FC2 RID: 16322
		private Animation animation;
	}
}
