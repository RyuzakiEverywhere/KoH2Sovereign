using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001EF RID: 495
public class UIHireWidnow : UIWindow
{
	// Token: 0x06001DD2 RID: 7634 RVA: 0x00117891 File Offset: 0x00115A91
	public override string GetDefId()
	{
		return UIHireWidnow.def_id;
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x00117898 File Offset: 0x00115A98
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.window_def = global::Defs.GetDefField(this.GetDefId(), null);
		if (this.m_CharacterClassContainerPrototype != null)
		{
			this.m_CharacterClassContainerPrototype.gameObject.SetActive(false);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x001178ED File Offset: 0x00115AED
	private void HandleOffRectClick(UIWindow obj)
	{
		this.OnBackInputAction();
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x001178F6 File Offset: 0x00115AF6
	public override bool OnBackInputAction()
	{
		if (this.m_OpenFrame == UnityEngine.Time.frameCount)
		{
			return false;
		}
		this.Show(false);
		return true;
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x0011790F File Offset: 0x00115B0F
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
		this.OnOutOfBoundsClick = (Action<UIWindow>)Delegate.Combine(this.OnOutOfBoundsClick, new Action<UIWindow>(this.HandleOffRectClick));
	}

	// Token: 0x06001DD7 RID: 7639 RVA: 0x0011793F File Offset: 0x00115B3F
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
		this.OnOutOfBoundsClick = (Action<UIWindow>)Delegate.Remove(this.OnOutOfBoundsClick, new Action<UIWindow>(this.HandleOffRectClick));
	}

	// Token: 0x06001DD8 RID: 7640 RVA: 0x0011796E File Offset: 0x00115B6E
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06001DD9 RID: 7641 RVA: 0x00117976 File Offset: 0x00115B76
	public void SetTargetSlotIndex(int index)
	{
		this.m_TargetCourtSlot = index;
	}

	// Token: 0x06001DDA RID: 7642 RVA: 0x0011797F File Offset: 0x00115B7F
	public void Refresh()
	{
		this.Populate();
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x00117988 File Offset: 0x00115B88
	public void Populate()
	{
		this.Init();
		RectTransform containerCharacters = this.m_ContainerCharacters;
		if (containerCharacters != null)
		{
			containerCharacters.gameObject.SetActive(true);
		}
		RectTransform containerClassses = this.m_ContainerClassses;
		if (containerClassses != null)
		{
			containerClassses.gameObject.SetActive(true);
		}
		foreach (KeyValuePair<string, RectTransform> keyValuePair in this.m_ClassContaindersBind)
		{
			global::Common.DestroyObj(keyValuePair.Value.gameObject);
		}
		this.m_ClassContaindersBind.Clear();
		UICommon.DeleteActiveChildren(this.m_ContainerClassses);
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		global::Kingdom kingdom = global::Kingdom.Get(worldUI.GetCurrentKingdomId());
		if (kingdom == null)
		{
			return;
		}
		List<KeyValuePair<string, CharacterClass.Def>> baseCharacterClassDefs = CharacterFactory.GetBaseCharacterClassDefs(GameLogic.Get(true));
		if (this.m_ContainerCharacters != null && this.m_CharacterClassContainerPrototype != null)
		{
			this.m_ClassContaindersBind.Clear();
			for (int i = 0; i < baseCharacterClassDefs.Count; i++)
			{
				string key = baseCharacterClassDefs[i].Key;
				if (!string.IsNullOrEmpty(key))
				{
					GameObject gameObject = global::Common.Spawn(this.m_CharacterClassContainerPrototype, this.m_ContainerCharacters, false, "");
					gameObject.gameObject.SetActive(true);
					this.m_ClassContaindersBind.Add(key, gameObject.transform as RectTransform);
				}
			}
			HashSet<Logic.Character> hashSet = new HashSet<Logic.Character>();
			for (int j = 0; j < kingdom.logic.royalFamily.Children.Count; j++)
			{
				if (this.IsEligableForHire(kingdom.logic, kingdom.logic.royalFamily.Children[j]))
				{
					hashSet.Add(kingdom.logic.royalFamily.Children[j]);
				}
			}
			for (int k = 0; k < kingdom.logic.royalFamily.Relatives.Count; k++)
			{
				if (this.IsEligableForHire(kingdom.logic, kingdom.logic.royalFamily.Relatives[k]))
				{
					hashSet.Add(kingdom.logic.royalFamily.Relatives[k]);
				}
			}
			if (kingdom.logic.is_orthodox && !kingdom.logic.subordinated && this.IsEligableForHire(kingdom.logic, kingdom.logic.patriarch))
			{
				hashSet.Add(kingdom.logic.patriarch);
			}
			foreach (Logic.Character character in hashSet)
			{
				this.AddCharacter(character);
			}
			this.m_ContainerCharacters.gameObject.SetActive(hashSet.Count > 0);
		}
		Game game = GameLogic.Get(true);
		if (this.IconHireClassPrototype != null)
		{
			int targetCourtSlot = this.m_TargetCourtSlot;
			for (int l = 0; l < baseCharacterClassDefs.Count; l++)
			{
				KeyValuePair<string, CharacterClass.Def> keyValuePair2 = baseCharacterClassDefs[l];
				string className = keyValuePair2.Key;
				if (!string.IsNullOrEmpty(className))
				{
					Resource cost = ForHireStatus.GetCost(game, (kingdom != null) ? kingdom.logic : null, className);
					Vars vars = new Vars();
					vars.Set<Sprite>("sprite", global::Defs.GetObj<Sprite>(keyValuePair2.Value.dt_def.field, "icon_extended", null));
					vars.Set<string>("tooltip", "HireNobleTooltip");
					vars.Set<CharacterClass.Def>("cls", keyValuePair2.Value);
					vars.Set<Resource>("cost", cost);
					vars.Set<Logic.Kingdom>("kingdom", kingdom.logic);
					if (className == "Marshal")
					{
						vars.Set<bool>("is_marshal", true);
						this.SetCannotHireMarshal(kingdom.logic, vars);
					}
					UIGenericActionIcon.Create(delegate
					{
						this.HandleClassSelected(className, null);
					}, this.IconHireClassPrototype, this.m_ContainerClassses, vars);
				}
			}
		}
		this.m_ContainerClassses.gameObject.SetActive(this.m_ContainerClassses.childCount > 0);
		this.m_ContainerCharacters.ForceUpdateRectTransforms();
		this.m_ContainerClassses.ForceUpdateRectTransforms();
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x00117DF8 File Offset: 0x00115FF8
	private void SetCannotHireMarshal(Logic.Kingdom kingdom, Vars vars)
	{
		for (int i = 0; i < kingdom.realms.Count; i++)
		{
			Logic.Realm realm = kingdom.realms[i];
			if (realm.castle.battle == null && !realm.IsOccupied() && !realm.IsDisorder())
			{
				return;
			}
		}
		vars.Set<bool>("cannot_hire_marshal", true);
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x00117E52 File Offset: 0x00116052
	public void SetCourt(UIRoyalCourt uiCourt)
	{
		this.m_RoyalCourt = uiCourt;
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x00117E5C File Offset: 0x0011605C
	private void AddCharacter(Logic.Character character)
	{
		if (character.GetStatus() == null)
		{
			if (!character.IsKingOrPrince() && !character.IsRoyalRelative())
			{
				Logic.Kingdom kingdom = character.GetKingdom();
				bool? flag;
				if (kingdom == null)
				{
					flag = null;
				}
				else
				{
					Logic.RoyalFamily royalFamily = kingdom.royalFamily;
					if (royalFamily == null)
					{
						flag = null;
					}
					else
					{
						List<Logic.Character> relatives = royalFamily.Relatives;
						flag = ((relatives != null) ? new bool?(relatives.Contains(character)) : null);
					}
				}
				if (!(flag ?? false))
				{
					if (!character.IsPatriarch())
					{
						character.SetStatus<ForHireStatus>();
						goto IL_91;
					}
					goto IL_91;
				}
			}
			character.SetStatus<AvailableForAssignmentStatus>();
		}
		IL_91:
		RectTransform rectTransform = null;
		if (!this.m_ClassContaindersBind.TryGetValue(character.class_def.field.key, out rectTransform))
		{
			return;
		}
		UICharacterIcon component = UnityEngine.Object.Instantiate<GameObject>(this.IconProtptype, Vector3.zero, Quaternion.identity, rectTransform).GetComponent<UICharacterIcon>();
		if (component != null)
		{
			component.SetObject(character, null);
			component.OnSelect += this.HandleOnCharacterSelect;
			component.ShowCrest(false);
		}
		if (rectTransform.childCount > 1)
		{
			GameObject gameObject = global::Common.FindChildByName(rectTransform.gameObject, "id_CharacterClassGroupBackground", true, true);
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001DDF RID: 7647 RVA: 0x00117F91 File Offset: 0x00116191
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIHireWidnow.def_id, null);
	}

	// Token: 0x06001DE0 RID: 7648 RVA: 0x00117F9E File Offset: 0x0011619E
	private void HandleOnCharacterSelect(UICharacterIcon obj)
	{
		this.ExecuteDefaultOnHireAction(obj.Data.class_def.name, obj.Data, this.m_TargetCourtSlot);
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x00117FC4 File Offset: 0x001161C4
	private bool CanAfforClasslesCharacter()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return false;
		}
		Resource cost = ForHireStatus.GetCost(kingdom.game, kingdom, null);
		return kingdom.resources.CanAfford(cost, 1f, Array.Empty<ResourceType>());
	}

	// Token: 0x06001DE2 RID: 7650 RVA: 0x00118000 File Offset: 0x00116200
	private RectTransform GetCourtSlotRect(int index)
	{
		if (this.m_RoyalCourt != null)
		{
			return this.m_RoyalCourt.GetIconRect(index);
		}
		return null;
	}

	// Token: 0x06001DE3 RID: 7651 RVA: 0x0011801E File Offset: 0x0011621E
	private void HandleClassSelected(string className, Logic.Character character)
	{
		this.ExecuteDefaultOnHireAction(className, character, this.m_TargetCourtSlot);
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x00118030 File Offset: 0x00116230
	private void ExecuteDefaultOnHireAction(string className, Logic.Character character, int courtSlot)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		global::Kingdom kingdom = global::Kingdom.Get(worldUI.GetCurrentKingdomId());
		if (className == "Marshal")
		{
			if (character != null || this.CheckHireCost(className))
			{
				Action<Value> select;
				if (character == null)
				{
					select = delegate(Value target)
					{
						if (kingdom.logic.IsAuthority())
						{
							Logic.Character character2 = kingdom.logic.HireCharacter(className, courtSlot, false);
							if (character2 != null)
							{
								character2.SpawnArmy(target.obj_val as Castle);
								this.SelectCharacter(character2);
								return;
							}
						}
						else
						{
							kingdom.logic.SendEvent(new Logic.Kingdom.HireOrdinaryCharacterEvent(courtSlot, className, target.obj_val as Castle));
						}
					};
				}
				else
				{
					select = delegate(Value target)
					{
						this.HireCharacter(character, courtSlot, target.obj_val as Castle);
					};
				}
				List<Logic.Object> list = new List<Logic.Object>();
				for (int i = 0; i < kingdom.realms.Count; i++)
				{
					global::Realm realm = kingdom.realms[i];
					if (realm.logic.castle.battle == null && !realm.logic.IsOccupied() && !realm.logic.IsDisorder())
					{
						list.Add(realm.logic.castle);
					}
				}
				Action cancel = delegate()
				{
					this.Show(false);
				};
				Logic.Object suggestedObject = (worldUI != null) ? worldUI.selected_logic_obj : null;
				Vars additionalData = new Vars();
				UITargetSelectWindow.ShowDialog(TargetPickerData.Create(list, null, null), suggestedObject, select, cancel, additionalData, null, null, null, "", "");
				return;
			}
		}
		else if (character == null)
		{
			if (this.CheckHireCost(className))
			{
				if (kingdom.logic.IsAuthority())
				{
					character = kingdom.logic.HireCharacter(className, courtSlot, false);
					this.SelectCharacter(character);
					return;
				}
				kingdom.logic.SendEvent(new Logic.Kingdom.HireOrdinaryCharacterEvent(courtSlot, className, null));
				return;
			}
		}
		else
		{
			Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
			if (kingdom2.realms.Count > 0)
			{
				this.HireCharacter(character, courtSlot, kingdom2.realms[0].castle);
			}
		}
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x00118250 File Offset: 0x00116450
	private bool CheckHireCost(string className)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Resource cost = ForHireStatus.GetCost(GameLogic.Get(true), kingdom, className);
		return kingdom.resources.CanAfford(cost, 1f, Array.Empty<ResourceType>());
	}

	// Token: 0x06001DE6 RID: 7654 RVA: 0x00118288 File Offset: 0x00116488
	private bool CheckHireCost(Logic.Character c)
	{
		if (c == null)
		{
			return false;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return false;
		}
		ForHireStatus forHireStatus = c.FindStatus<ForHireStatus>();
		if (forHireStatus != null)
		{
			return kingdom.resources.CanAfford(forHireStatus.GetCost(), 1f, Array.Empty<ResourceType>());
		}
		return c.FindStatus<AvailableForAssignmentStatus>() != null;
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x001182D8 File Offset: 0x001164D8
	private void HireCharacter(Logic.Character character, int courtSlot, Logic.Object target = null)
	{
		Action action = Action.Find(character, "HireAction");
		if (action != null)
		{
			if (action.args == null)
			{
				action.args = new List<Value>();
			}
			action.args.Add(courtSlot);
			if (!action.ValidateArg(target, 0))
			{
				Debug.Log("Invalid court slot");
			}
			action.Execute(target);
			this.SelectCharacter(character);
		}
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x00118340 File Offset: 0x00116540
	private void SelectCharacter(Logic.Character c)
	{
		if (c == null || c.visuals == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		worldUI.SelectObj((c.visuals as global::Character).Obj, false, true, true, true);
	}

	// Token: 0x06001DE9 RID: 7657 RVA: 0x00118384 File Offset: 0x00116584
	private bool IsEligableForHire(Logic.Kingdom kingdom, Logic.Character character)
	{
		return !kingdom.court.Contains(character) && character.IsValid() && !character.IsRebel() && !character.IsDead() && !character.IsPope() && character.age >= Logic.Character.Age.Young && character.sex != Logic.Character.Sex.Female;
	}

	// Token: 0x06001DEA RID: 7658 RVA: 0x001183E0 File Offset: 0x001165E0
	public override void Show()
	{
		this.m_OpenFrame = UnityEngine.Time.frameCount;
		if (base.gameObject.activeInHierarchy)
		{
			return;
		}
		base.Show();
		base.gameObject.SetActive(true);
		this.Populate();
	}

	// Token: 0x06001DEB RID: 7659 RVA: 0x00118414 File Offset: 0x00116614
	public override void Hide(bool silent = false)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		base.gameObject.SetActive(false);
		this.m_TargetCourtSlot = -1;
		if (this.m_RoyalCourt != null)
		{
			this.m_RoyalCourt.ClearHireSelection();
		}
		base.Hide(false);
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x00118464 File Offset: 0x00116664
	public static UIHireWidnow Create(GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIHireWidnow orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIHireWidnow>();
		orAddComponent.gameObject.SetActive(true);
		orAddComponent.gameObject.transform.localPosition = Vector3.zero;
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x04001393 RID: 5011
	private static string def_id = "ForHireCandidatesWindow";

	// Token: 0x04001394 RID: 5012
	[SerializeField]
	private GameObject IconProtptype;

	// Token: 0x04001395 RID: 5013
	[SerializeField]
	private GameObject IconHireClassPrototype;

	// Token: 0x04001396 RID: 5014
	[UIFieldTarget("id_ContainerCharacters")]
	private RectTransform m_ContainerCharacters;

	// Token: 0x04001397 RID: 5015
	[UIFieldTarget("id_CharacterClassContainerPrototype")]
	private GameObject m_CharacterClassContainerPrototype;

	// Token: 0x04001398 RID: 5016
	[UIFieldTarget("id_ContainerClassses")]
	private RectTransform m_ContainerClassses;

	// Token: 0x04001399 RID: 5017
	private int m_TargetCourtSlot;

	// Token: 0x0400139A RID: 5018
	private UIRoyalCourt m_RoyalCourt;

	// Token: 0x0400139B RID: 5019
	private int m_OpenFrame;

	// Token: 0x0400139C RID: 5020
	private bool m_Initialzied;

	// Token: 0x0400139D RID: 5021
	private Dictionary<string, RectTransform> m_ClassContaindersBind = new Dictionary<string, RectTransform>();
}
