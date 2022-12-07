using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F0 RID: 496
public class UIKingAbilities : MonoBehaviour
{
	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06001DEF RID: 7663 RVA: 0x001184DF File Offset: 0x001166DF
	// (set) Token: 0x06001DF0 RID: 7664 RVA: 0x001184E7 File Offset: 0x001166E7
	public Logic.Character Character { get; private set; }

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06001DF1 RID: 7665 RVA: 0x001184F0 File Offset: 0x001166F0
	// (set) Token: 0x06001DF2 RID: 7666 RVA: 0x001184F8 File Offset: 0x001166F8
	public Logic.Character Spouse { get; private set; }

	// Token: 0x06001DF3 RID: 7667 RVA: 0x00118501 File Offset: 0x00116701
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_KingAbilityPrototype != null)
		{
			this.m_KingAbilityPrototype.gameObject.SetActive(false);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06001DF4 RID: 7668 RVA: 0x0011853C File Offset: 0x0011673C
	public void SetData(Logic.Character character, Vars vars = null)
	{
		this.Init();
		if (character == this.Character)
		{
			return;
		}
		this.Character = character;
		Logic.Character character2 = this.Character;
		Logic.Character spouse;
		if (character2 == null)
		{
			spouse = null;
		}
		else
		{
			Marriage marriage = character2.GetMarriage();
			spouse = ((marriage != null) ? marriage.GetSpouse(this.Character) : null);
		}
		this.Spouse = spouse;
		this.BuildAbilities();
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x00118590 File Offset: 0x00116790
	private void BuildAbilities()
	{
		if (this.Character == null)
		{
			return;
		}
		if (this.m_KingAbilityPrototype == null)
		{
			return;
		}
		if (this.m_KingAbilitiesContainer == null)
		{
			return;
		}
		int maxAbilityCount = this.Character.GetMaxAbilityCount();
		List<Logic.Character.RoyalAbility> royalAbilities = this.Character.GetRoyalAbilities();
		List<Logic.Character.RoyalAbility> list;
		if (this.Character.sex != Logic.Character.Sex.Male)
		{
			list = null;
		}
		else
		{
			Logic.Character spouse = this.Spouse;
			list = ((spouse != null) ? spouse.GetRoyalAbilities() : null);
		}
		List<Logic.Character.RoyalAbility> list2 = list;
		bool flag = this.Character.sex == Logic.Character.Sex.Male;
		UICommon.DeleteActiveChildren(this.m_KingAbilitiesContainer);
		if (royalAbilities == null)
		{
			return;
		}
		for (int i = 0; i < royalAbilities.Count; i++)
		{
			Logic.Character.RoyalAbility royalAbility;
			if (list2 != null)
			{
				royalAbility = list2[i];
			}
			else
			{
				royalAbility = default(Logic.Character.RoyalAbility);
			}
			if (flag || royalAbilities[i].val + royalAbility.val != 0)
			{
				this.AddAbility(royalAbilities[i], royalAbility, maxAbilityCount, this.m_KingAbilityPrototype, this.m_KingAbilitiesContainer);
			}
		}
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x00118680 File Offset: 0x00116880
	private void AddAbility(Logic.Character.RoyalAbility charatcerAbility, Logic.Character.RoyalAbility spousebility, int maxStarCount, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return;
		}
		if (parent == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent);
		gameObject.gameObject.SetActive(true);
		gameObject.GetOrAddComponent<UIKingAbilities.UIKingAbility>().SetData(charatcerAbility, spousebility, maxStarCount);
	}

	// Token: 0x0400139E RID: 5022
	[UIFieldTarget("id_KingAbilityPrototype")]
	private GameObject m_KingAbilityPrototype;

	// Token: 0x0400139F RID: 5023
	[UIFieldTarget("id_KingAbilitiesContainer")]
	private RectTransform m_KingAbilitiesContainer;

	// Token: 0x040013A2 RID: 5026
	private bool m_Initialzied;

	// Token: 0x02000733 RID: 1843
	internal class UIKingAbility : MonoBehaviour
	{
		// Token: 0x06004A1A RID: 18970 RVA: 0x0021F3A4 File Offset: 0x0021D5A4
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_StarPrototype != null)
			{
				this.m_StarPrototype.gameObject.SetActive(false);
			}
			if (this.m_QueenStarPositive != null)
			{
				this.m_QueenStarPositive.gameObject.SetActive(false);
			}
			if (this.m_QueenStarNegative != null)
			{
				this.m_QueenStarNegative.gameObject.SetActive(false);
			}
			if (this.m_StarBackgroundPrototype != null)
			{
				this.m_StarBackgroundPrototype.gameObject.SetActive(false);
			}
			this.m_Initialzied = true;
		}

		// Token: 0x06004A1B RID: 18971 RVA: 0x0021F444 File Offset: 0x0021D644
		public void SetData(Logic.Character.RoyalAbility kingAbility, Logic.Character.RoyalAbility spouseAbility, int maxStarCount)
		{
			this.Init();
			this.KingAbility = kingAbility;
			this.SpauseAbility = spouseAbility;
			this.m_maxStarCount = maxStarCount;
			this.Refresh();
		}

		// Token: 0x06004A1C RID: 18972 RVA: 0x0021F468 File Offset: 0x0021D668
		private void Refresh()
		{
			Tooltip.Get(base.gameObject, true).SetDef("KingAbilityTooltip", new Vars(this.KingAbility));
			if (this.m_AbilityIcon != null)
			{
				this.m_AbilityIcon.sprite = global::Defs.GetObj<Sprite>("KingAbilitiesIconSettings", this.KingAbility.name, null);
			}
			int num = this.KingAbility.val;
			if (this.SpauseAbility.val < 0)
			{
				num += this.SpauseAbility.val;
				if (num < 0)
				{
					num = 0;
				}
			}
			if (this.m_StartsContainer != null)
			{
				this.m_StartsContainer.gameObject.SetActive(true);
				if (this.m_StarPrototype != null)
				{
					for (int i = 0; i < num; i++)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.m_StarPrototype, this.m_StartsContainer.transform).gameObject.SetActive(true);
					}
				}
				if (this.m_QueenStarPositive != null && this.SpauseAbility.val > 0)
				{
					int num2 = this.SpauseAbility.val_max - num;
					int num3 = 0;
					while (num3 < this.SpauseAbility.val && num3 < num2)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.m_QueenStarPositive, this.m_StartsContainer).gameObject.SetActive(true);
						num3++;
					}
				}
				if (this.m_QueenStarNegative != null && this.SpauseAbility.val < 0)
				{
					int num4 = 0;
					while (num4 < -this.SpauseAbility.val && num4 < this.KingAbility.val)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.m_QueenStarNegative, this.m_StartsContainer).gameObject.SetActive(true);
						num4++;
					}
				}
				if (this.m_Backgrounds != null && this.m_StarBackgroundPrototype != null)
				{
					for (int j = 0; j < this.m_maxStarCount; j++)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.m_StarBackgroundPrototype, this.m_Backgrounds).gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x040038EC RID: 14572
		[UIFieldTarget("id_AbilityIcon")]
		private Image m_AbilityIcon;

		// Token: 0x040038ED RID: 14573
		[UIFieldTarget("id_StartsContainer")]
		private RectTransform m_StartsContainer;

		// Token: 0x040038EE RID: 14574
		[UIFieldTarget("id_Backgrounds")]
		private RectTransform m_Backgrounds;

		// Token: 0x040038EF RID: 14575
		[UIFieldTarget("id_AbilityRankStar_Background")]
		private GameObject m_StarBackgroundPrototype;

		// Token: 0x040038F0 RID: 14576
		[UIFieldTarget("id_AbilityRankStar")]
		private GameObject m_StarPrototype;

		// Token: 0x040038F1 RID: 14577
		[UIFieldTarget("id_AbilityRankStarQueenPositive")]
		private GameObject m_QueenStarPositive;

		// Token: 0x040038F2 RID: 14578
		[UIFieldTarget("id_AbilityRankStarQueenNegative")]
		private GameObject m_QueenStarNegative;

		// Token: 0x040038F3 RID: 14579
		public Logic.Character.RoyalAbility KingAbility;

		// Token: 0x040038F4 RID: 14580
		public Logic.Character.RoyalAbility SpauseAbility;

		// Token: 0x040038F5 RID: 14581
		private int m_maxStarCount;

		// Token: 0x040038F6 RID: 14582
		private bool m_Initialzied;
	}
}
