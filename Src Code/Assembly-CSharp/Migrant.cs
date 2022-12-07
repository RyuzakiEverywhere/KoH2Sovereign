using System;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class Migrant : GameLogic.Behaviour, VisibilityDetector.IVisibilityChanged
{
	// Token: 0x06000D4D RID: 3405 RVA: 0x0009698C File Offset: 0x00094B8C
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00096994 File Offset: 0x00094B94
	public static GameObject Prefab()
	{
		return global::Defs.GetObj<GameObject>("Migrant", "prefab_unit", null);
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x000969A8 File Offset: 0x00094BA8
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.Migrant migrant = logic_obj as Logic.Migrant;
		if (migrant == null)
		{
			return;
		}
		GameObject gameObject = global::Migrant.Prefab();
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2;
		if (GameLogic.instance != null)
		{
			gameObject2 = global::Common.Spawn(gameObject, GameLogic.instance.transform, false, "Armies");
		}
		else
		{
			gameObject2 = global::Common.Spawn(gameObject, false, false);
		}
		if (gameObject2 == null)
		{
			return;
		}
		global::Migrant component = gameObject2.GetComponent<global::Migrant>();
		if (component == null)
		{
			return;
		}
		component.logic = migrant;
		migrant.visuals = component;
		migrant.movement.speed = component.MovementSpeed;
		component.nid = migrant.GetNid(true);
		component.kingdom.id = migrant.kingdom_id;
		component.transform.position = global::Common.SnapToTerrain(migrant.position, 0f, null, -1f, false);
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x00096A80 File Offset: 0x00094C80
	public override void OnMessage(object obj, string message, object param)
	{
		if (message == "realm_crossed")
		{
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		if (message == "moved")
		{
			this.Moved();
			return;
		}
		if (message == "path_changed")
		{
			if (this.selected)
			{
				this.CreatePathArrows();
			}
			return;
		}
		if (message == "reward_changed")
		{
			this.castle_dest = this.logic.castle_destination;
			this.rp_count = this.logic.GetRPCount();
			return;
		}
		if (message == "destroying" || message == "finishing")
		{
			this.logic.DelListener(this);
			this.logic = null;
			if (this.selected)
			{
				this.ui.SelectObj(null, false, true, true, true);
			}
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x00096B55 File Offset: 0x00094D55
	public static global::Migrant Get(Logic.Army logic)
	{
		if (logic == null)
		{
			return null;
		}
		return logic.visuals as global::Migrant;
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00096B67 File Offset: 0x00094D67
	public static global::Migrant Get(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		return go.GetComponent<global::Migrant>();
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00096B7A File Offset: 0x00094D7A
	public static global::Migrant Get(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		return t.GetComponent<global::Migrant>();
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00096B8D File Offset: 0x00094D8D
	public void RecreateSelectionUI()
	{
		if (this.ui != null)
		{
			this.ui.RefreshSelection(base.gameObject);
		}
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00096BB0 File Offset: 0x00094DB0
	public void CreateSelection()
	{
		if (this.selection != null)
		{
			return;
		}
		if (this.logic == null || !this.logic.started)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || !baseUI.SelectionShown())
		{
			return;
		}
		this.selection = MeshUtils.CreateSelectionCircle(base.gameObject, 1.75f, baseUI.GetSelectionMaterial(base.gameObject, null, this.primarySelection, false), 0.25f);
		MeshUtils.SnapSelectionToTerrain(this.selection, null);
		this.CreatePathArrows();
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00096C3B File Offset: 0x00094E3B
	public void DestroySelection()
	{
		this.DestroyPathArrows();
		if (this.selection == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.selection.gameObject);
		this.selection = null;
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x00096C6C File Offset: 0x00094E6C
	private void UpdateSelection()
	{
		if (this.selection == null)
		{
			return;
		}
		this.selection.material = this.ui.GetSelectionMaterial(base.gameObject, this.selection.material, true, false);
		MeshUtils.SnapSelectionToTerrain(this.selection, null);
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x00096CBD File Offset: 0x00094EBD
	private void DestroyPathArrows()
	{
		if (this.path_arrows == null)
		{
			return;
		}
		global::Common.DestroyObj(this.path_arrows.gameObject);
		this.path_arrows = null;
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x00096CE8 File Offset: 0x00094EE8
	private void CreatePathArrows()
	{
		this.DestroyPathArrows();
		if (!this.ui.PathArrowsShown())
		{
			return;
		}
		Path path = this.logic.movement.path;
		if (path == null || path.IsDone())
		{
			return;
		}
		this.path_arrows = PathArrows.Create(this.logic.movement, 3, false);
		this.path_arrows.gameObject.layer = base.gameObject.layer;
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x00096D59 File Offset: 0x00094F59
	public void SetSelected(bool bSelected, bool bPrimarySelection = true)
	{
		this.selected = bSelected;
		this.primarySelection = bPrimarySelection;
		if (this.selected)
		{
			this.CreateSelection();
			return;
		}
		this.DestroySelection();
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00096D7E File Offset: 0x00094F7E
	public bool IsSelected()
	{
		return this.selected;
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00096D86 File Offset: 0x00094F86
	public void Stop()
	{
		this.logic.Stop(true);
		this.DestroyPathArrows();
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00096D9C File Offset: 0x00094F9C
	public void MoveTo(global::Settlement s, bool confirmation = true)
	{
		if (confirmation)
		{
			if (this.logic.IsEnemy(s.logic))
			{
				this.PlayVoiceLine(this.AttackVoiceLine);
			}
			else
			{
				this.PlayVoiceLine(this.MoveVoiceLine);
			}
		}
		this.logic.MoveTo(s.logic, s.GetRadius());
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00096DF0 File Offset: 0x00094FF0
	public void MoveTo(Vector3 pt, bool confirmation = true)
	{
		if (!this.logic.IsAuthority())
		{
			return;
		}
		bool key = UICommon.GetKey(KeyCode.LeftShift, false);
		bool flag = UICommon.GetKey(KeyCode.LeftControl, false) && key && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat teleport migrant", true);
		if (confirmation)
		{
			this.PlayVoiceLine(this.MoveVoiceLine);
		}
		if (flag)
		{
			this.logic.SetPosition(pt);
			this.logic.Stop(true);
			base.transform.position = pt;
			if (this.visibility_index >= 0)
			{
				VisibilityDetector.Move(this.visibility_index, pt, -1f);
			}
			this.UpdateSelection();
			return;
		}
		this.logic.MoveTo(pt, 0f);
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00096EA6 File Offset: 0x000950A6
	public void PlayVoiceLine(string path)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		FMODWrapper.PlayOneShot(path, base.transform.position);
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x00096ED0 File Offset: 0x000950D0
	private void UpdateSoundLoop()
	{
		if (this.audio_source_loop == null)
		{
			return;
		}
		string text = null;
		if (this.logic.movement.path != null && !this.logic.movement.path.IsDone())
		{
			text = this.MarchingSound;
		}
		this.audio_source_loop.Event = text;
		if (text != null)
		{
			if (!this.audio_source_loop.IsPlaying())
			{
				this.audio_source_loop.Play();
				return;
			}
		}
		else if (this.audio_source_loop.IsPlaying())
		{
			this.audio_source_loop.Stop();
		}
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00096F60 File Offset: 0x00095160
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			this.ui = WorldUI.Get();
			if (this.audio_source_loop == null)
			{
				this.audio_source_loop = base.GetComponent<StudioEventEmitter>();
				if (this.audio_source_loop == null)
				{
					this.audio_source_loop = base.gameObject.AddComponent<StudioEventEmitter>();
				}
			}
		}
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x00096FB8 File Offset: 0x000951B8
	private void Start()
	{
		if (Application.isPlaying)
		{
			SphereCollider sphereCollider = base.GetComponent<SphereCollider>();
			if (sphereCollider == null)
			{
				sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			}
			sphereCollider.center = Vector3.zero;
			sphereCollider.radius = 1.75f;
			this.visibility_index = VisibilityDetector.Add(base.transform.position, 10f, null, this, base.gameObject.layer);
		}
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x00097028 File Offset: 0x00095228
	private void OnDestroy()
	{
		if (Application.isPlaying)
		{
			if (this.castle_dest != null && this.castle_dest.IsValid())
			{
				global::Settlement settlement = this.castle_dest.visuals as global::Settlement;
				if (settlement != null && settlement.visible)
				{
					Vars vars = new Vars();
					Resource resource = new Resource();
					Logic.Migrant.Def @base = GameLogic.Get(true).defs.GetBase<Logic.Migrant.Def>();
					resource.Add(@base.reward.GetResources(1, true, true), (float)this.rp_count, Array.Empty<ResourceType>());
					vars.Set<Resource>("reward", resource);
					FloatingText.Create(settlement.gameObject, "FloatingTexts.MigrantsReached", "migrants_reached", vars, false);
				}
			}
			this.audio_source_loop.Stop();
			this.DestroySelection();
			if (this.visibility_index >= 0)
			{
				VisibilityDetector.Del(this.visibility_index);
				this.visibility_index = -1;
			}
			if (this.logic != null)
			{
				Logic.Object @object = this.logic;
				this.logic = null;
				@object.Destroy(false);
			}
		}
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x00097124 File Offset: 0x00095324
	public void VisibilityChanged(bool visible)
	{
		if (visible)
		{
			this.UpdateSelection();
		}
		this.visible = visible;
		base.enabled = visible;
		base.gameObject.SetActive(visible);
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x0009714C File Offset: 0x0009534C
	private void Moved()
	{
		if (this.logic == null)
		{
			return;
		}
		this.Moved(this.logic.position, (this.logic.movement.path == null) ? 0f : this.logic.movement.path.t);
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x000971A8 File Offset: 0x000953A8
	private void Moved(Point pt, float path_t)
	{
		Vector3 vector = global::Common.SnapToTerrain(pt, 0f, null, -1f, false);
		Vector3 vector2 = vector - base.transform.position;
		vector2.y = 0f;
		base.transform.position = vector;
		if (this.visibility_index >= 0)
		{
			VisibilityDetector.Move(this.visibility_index, vector, -1f);
		}
		if (!this.visible)
		{
			return;
		}
		if (this.visible)
		{
			if (vector2 != Vector3.zero)
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(vector2), this.m_RotationSpeed * UnityEngine.Time.deltaTime);
			}
			this.UpdateSelection();
		}
		Logic.Realm realm = this.logic.game.GetRealm(base.transform.position);
		bool flag = realm != null && realm.id < 0;
		this.ship.SetActive(flag);
		this.cart.SetActive(!flag);
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x000972B0 File Offset: 0x000954B0
	private void Update()
	{
		if (this.logic.movement.path != null)
		{
			PPos pt;
			float path_t;
			this.logic.movement.CalcPosition(out pt, out path_t);
			this.Moved(pt, path_t);
		}
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x000972F0 File Offset: 0x000954F0
	public int GetKingdomID()
	{
		if (this.logic == null)
		{
			return this.kingdom;
		}
		return this.logic.kingdom_id;
	}

	// Token: 0x04000A39 RID: 2617
	public global::Kingdom.ID kingdom = 0;

	// Token: 0x04000A3A RID: 2618
	public const float Radius = 1.75f;

	// Token: 0x04000A3B RID: 2619
	public float MovementSpeed = 1.5f;

	// Token: 0x04000A3C RID: 2620
	private WorldUI ui;

	// Token: 0x04000A3D RID: 2621
	private bool selected;

	// Token: 0x04000A3E RID: 2622
	private bool primarySelection = true;

	// Token: 0x04000A3F RID: 2623
	private MeshRenderer selection;

	// Token: 0x04000A40 RID: 2624
	private int visibility_index = -1;

	// Token: 0x04000A41 RID: 2625
	private bool visible = true;

	// Token: 0x04000A42 RID: 2626
	private PathArrows path_arrows;

	// Token: 0x04000A43 RID: 2627
	[EventRef]
	public string MarchingSound;

	// Token: 0x04000A44 RID: 2628
	[EventRef]
	public string AttackVoiceLine;

	// Token: 0x04000A45 RID: 2629
	[EventRef]
	public string MoveVoiceLine;

	// Token: 0x04000A46 RID: 2630
	public Logic.Migrant logic;

	// Token: 0x04000A47 RID: 2631
	public GameObject cart;

	// Token: 0x04000A48 RID: 2632
	public GameObject ship;

	// Token: 0x04000A49 RID: 2633
	private Castle castle_dest;

	// Token: 0x04000A4A RID: 2634
	private int rp_count;

	// Token: 0x04000A4B RID: 2635
	private StudioEventEmitter audio_source_loop;

	// Token: 0x04000A4C RID: 2636
	private float m_RotationSpeed = 45f;
}
