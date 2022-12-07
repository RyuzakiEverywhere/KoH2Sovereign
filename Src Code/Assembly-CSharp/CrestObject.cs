using System;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200010B RID: 267
[ExecuteInEditMode]
public class CrestObject : MonoBehaviour
{
	// Token: 0x06000C43 RID: 3139 RVA: 0x00089E1C File Offset: 0x0008801C
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x00089E24 File Offset: 0x00088024
	private void OnEnable()
	{
		if (!Application.isPlaying)
		{
			this.Init();
		}
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x00089E33 File Offset: 0x00088033
	private void Init()
	{
		if (this.Initialzied)
		{
			return;
		}
		this.Build();
		this.SetKingdomId();
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x00089E4C File Offset: 0x0008804C
	private void Build()
	{
		if (this.Initialzied)
		{
			return;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs != null && this.shieldMode != "")
		{
			this.def = defs.dt.Find("CoatOfArmsModes." + this.shieldMode, null);
		}
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		if (this.materialBlock == null)
		{
			this.materialBlock = new MaterialPropertyBlock();
		}
		this.Initialzied = true;
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00089ECC File Offset: 0x000880CC
	public void UpdateMaterial()
	{
		if (this.meshRenderer == null)
		{
			return;
		}
		string text = SceneManager.GetActiveScene().name.ToLowerInvariant();
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Logic.Kingdom kingdom = game.GetKingdom(this.kingdomId);
		if (kingdom == null)
		{
			return;
		}
		string sceneName = text;
		if (kingdom.game.map_name != null)
		{
			sceneName = kingdom.game.map_name;
		}
		else if (kingdom.game.subgames != null && kingdom.game.subgames.Count > 0 && kingdom.game.subgames[0] != null)
		{
			sceneName = kingdom.game.subgames[0].map_name;
		}
		this.UpdateMaterial(sceneName);
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00089F8C File Offset: 0x0008818C
	public void UpdateMaterial(string sceneName)
	{
		if (this.meshRenderer == null)
		{
			return;
		}
		Texture2D texture2D = Assets.Get<Texture2D>(string.Concat(new string[]
		{
			"Assets/Maps/",
			sceneName,
			"/CoatOfArmsAtlas",
			this.shieldMode,
			".png"
		}));
		if (texture2D != null)
		{
			this.materialBlock.SetTexture("_MainTex", texture2D);
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs != null && defs.dt.Find("CoatOfArmsModes." + this.shieldMode, null) != null && this.materialBlock != null)
		{
			this.materialBlock.SetFloat("tileSizeX", (float)this.def.GetInt("width", null, 0, true, true, true, '.'));
			this.materialBlock.SetFloat("tileSizeY", (float)this.def.GetInt("height", null, 0, true, true, true, '.'));
		}
		if (this.materialBlock != null)
		{
			this.materialBlock.SetFloat("_kID", (float)this.crestId);
			this.materialBlock.SetFloat("_FrameID", (float)global::Kingdom.GetShieldFrameIndex(this.shieldMode, this.kingdomType, false));
			this.materialBlock.SetFloat("_UseColorAsData", 0f);
			this.meshRenderer.SetPropertyBlock(this.materialBlock);
		}
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x0008A0E6 File Offset: 0x000882E6
	public void SetKingdomId(int kingdomId)
	{
		this.kingdomId = kingdomId;
		this.kingdomType = global::Kingdom.GetShieldMaterialType(global::Kingdom.Get(kingdomId));
		this.Build();
		this.UpdateCrest();
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x0008A10C File Offset: 0x0008830C
	public void SetCrestId(int crestId)
	{
		this.crestId = crestId;
		this.Build();
		this.UpdateMaterial();
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x0008A124 File Offset: 0x00088324
	public void UpdateCrest()
	{
		global::Kingdom kingdom = global::Kingdom.Get(this.kingdomId);
		if (kingdom != null)
		{
			this.crestId = kingdom.crest_id;
			this.UpdateMaterial();
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Logic.Kingdom kingdom2 = game.GetKingdom(this.kingdomId);
		if (kingdom2 == null)
		{
			return;
		}
		string sceneName = null;
		if (kingdom2.game.map_name != null)
		{
			sceneName = kingdom2.game.map_name;
		}
		else if (kingdom2.game.subgames != null && kingdom2.game.subgames.Count > 0 && kingdom2.game.subgames[0] != null)
		{
			sceneName = kingdom2.game.subgames[0].map_name;
		}
		this.crestId = kingdom2.CoAIndex;
		this.UpdateMaterial(sceneName);
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x0008A1EC File Offset: 0x000883EC
	public void RefreshCrest()
	{
		if (this.stopRefreshing)
		{
			return;
		}
		global::Army parentComponent = global::Common.GetParentComponent<global::Army>(base.gameObject);
		if (parentComponent != null)
		{
			Logic.Kingdom kingdom = parentComponent.logic.GetKingdom();
			if (kingdom == null)
			{
				return;
			}
			if (parentComponent.logic != null && parentComponent.logic.rebel != null)
			{
				if (parentComponent.logic.rebel.IsLoyalist() && this.subType == CrestObject.SubType.Secondary)
				{
					global::Kingdom kingdom2 = global::Kingdom.Get(parentComponent.logic.rebel.loyal_to);
					if (kingdom2 != null)
					{
						this.SetCrestId(kingdom2.crest_id);
					}
					else
					{
						this.SetKingdomId(kingdom.id);
					}
				}
				else
				{
					this.SetKingdomId(kingdom.id);
				}
			}
			else if (parentComponent.logic != null && parentComponent.logic.mercenary != null)
			{
				int num = kingdom.id;
				int former_owner_id = parentComponent.logic.mercenary.former_owner_id;
				if (former_owner_id != 0)
				{
					num = former_owner_id;
				}
				if (this.subType == CrestObject.SubType.Primary)
				{
					this.SetKingdomId(parentComponent.logic.mercenary.kingdom_id);
				}
				else
				{
					this.SetKingdomId(num);
				}
			}
			else if (kingdom.id != this.kingdomId)
			{
				this.SetKingdomId(kingdom.id);
			}
			else
			{
				this.UpdateCrest();
			}
		}
		global::Settlement componentInParent = base.gameObject.GetComponentInParent<global::Settlement>();
		if (componentInParent != null)
		{
			int controllerKingdomID = componentInParent.GetControllerKingdomID();
			global::Kingdom kingdom3 = global::Kingdom.Get(controllerKingdomID);
			if (kingdom3 != null && kingdom3.logic != null && kingdom3.logic.type != Logic.Kingdom.Type.Regular)
			{
				this.SetCrestId(kingdom3.crest_id);
			}
			this.SetKingdomId(controllerKingdomID);
		}
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x0008A380 File Offset: 0x00088580
	public static void SetKingdomId(Transform t, int kingdomId)
	{
		CrestObject[] componentsInChildren = t.GetComponentsInChildren<CrestObject>(true);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		global::Kingdom kingdom = global::Kingdom.Get(kingdomId);
		if (kingdom == null)
		{
			return;
		}
		int crest_id = kingdom.crest_id;
		materialPropertyBlock.SetFloat("_kID", (float)crest_id);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshRenderer component = componentsInChildren[i].GetComponent<MeshRenderer>();
			if (!(component == null))
			{
				component.SetPropertyBlock(materialPropertyBlock);
			}
		}
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x0008A3EC File Offset: 0x000885EC
	public void SetKingdomId()
	{
		global::Settlement componentInParent = base.gameObject.GetComponentInParent<global::Settlement>();
		if (componentInParent != null)
		{
			if (Application.isPlaying)
			{
				this.SetKingdomId(componentInParent.GetControllerKingdomID());
			}
			return;
		}
		global::Army parentComponent = global::Common.GetParentComponent<global::Army>(base.gameObject);
		if (parentComponent != null)
		{
			this.SetKingdomId(parentComponent.GetKingdomID());
			return;
		}
		global::Squad parentComponent2 = global::Common.GetParentComponent<global::Squad>(base.gameObject);
		if (parentComponent2 != null)
		{
			this.SetKingdomId(parentComponent2.GetKingdomID());
			return;
		}
	}

	// Token: 0x0400099C RID: 2460
	[HideInInspector]
	public int crestId;

	// Token: 0x0400099D RID: 2461
	[HideInInspector]
	public int kingdomId;

	// Token: 0x0400099E RID: 2462
	[HideInInspector]
	public string kingdomType = "regular";

	// Token: 0x0400099F RID: 2463
	[HideInInspector]
	public string shieldMode = "shield";

	// Token: 0x040009A0 RID: 2464
	[HideInInspector]
	public CrestObject.SubType subType;

	// Token: 0x040009A1 RID: 2465
	private MeshRenderer meshRenderer;

	// Token: 0x040009A2 RID: 2466
	private MaterialPropertyBlock materialBlock;

	// Token: 0x040009A3 RID: 2467
	public bool stopRefreshing;

	// Token: 0x040009A4 RID: 2468
	[HideInInspector]
	public bool politicalView;

	// Token: 0x040009A5 RID: 2469
	private DT.Field def;

	// Token: 0x040009A6 RID: 2470
	private bool Initialzied;

	// Token: 0x0200060B RID: 1547
	public enum SubType
	{
		// Token: 0x0400338C RID: 13196
		Primary,
		// Token: 0x0400338D RID: 13197
		Secondary
	}
}
