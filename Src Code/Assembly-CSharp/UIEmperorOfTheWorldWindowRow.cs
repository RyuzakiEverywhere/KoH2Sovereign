using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001FD RID: 509
public class UIEmperorOfTheWorldWindowRow : MonoBehaviour
{
	// Token: 0x06001F1B RID: 7963 RVA: 0x00120438 File Offset: 0x0011E638
	public void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialized = true;
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x00120454 File Offset: 0x0011E654
	public void SetData(UIEmperorOfTheWorldWindow parent, int voterIndex, int candidateIndex)
	{
		this.Init();
		Logic.Kingdom voter = parent.GetVoter(voterIndex);
		Logic.Character king = voter.GetKing();
		Logic.Kingdom kingdom = parent.candidates[candidateIndex];
		string voteText = parent.GetVoteText(voterIndex, candidateIndex);
		int playerVote = parent.votes[voterIndex];
		int num = parent.votes_weights[voterIndex][candidateIndex];
		Vars vars = new Vars(voter);
		vars.Set<string>("vote_reason", "#" + voteText);
		vars.Set<int>("vote_weight", num);
		if (kingdom.is_player && kingdom == BaseUI.LogicKingdom())
		{
			vars.Set<string>("pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(voter, kingdom, "reason_to_them", true));
			vars.Set<string>("con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(voter, kingdom, "reason_to_them", false));
		}
		if (parent.candidates != null)
		{
			for (int i = 0; i < parent.candidates.Count; i++)
			{
				Logic.Kingdom kingdom2 = parent.candidates[i];
				vars.Set<string>("rel_key_claimant" + (i + 1), global::Kingdom.GetKingdomRelationsKey(kingdom2, voter));
				vars.Set<Logic.Kingdom>("claimant" + (i + 1), kingdom2);
			}
		}
		if (this.m_VotingDecision != null)
		{
			Tooltip.Get(this.m_VotingDecision, true).SetDef("EmperorVoteTooltip", vars);
		}
		if (this.m_VoteWeight != null)
		{
			UIText.SetText(this.m_VoteWeight, num.ToString());
		}
		if (this.m_DecisionBackground != null)
		{
			EmperorOfTheWorld.Slant slant = parent.logic.CalcSlant(voter, kingdom, playerVote);
			this.m_DecisionBackground.overrideSprite = global::Defs.GetObj<Sprite>(parent.logic.def.field, "SlantBackgrounds.Voters." + slant.ToString(), null);
		}
		if (this.m_DecisionFor != null)
		{
			this.m_DecisionFor.SetActive(num >= 0);
		}
		if (this.m_DecisionAgainst != null)
		{
			this.m_DecisionAgainst.SetActive(num < 0);
		}
		if (this.m_VoteText != null)
		{
			UIText.SetText(this.m_VoteText, voteText);
		}
		if (this.m_KingIcon != null)
		{
			this.m_KingIcon.SetObject(king, null);
		}
		if (this.m_KingName != null)
		{
			UIText.SetTextKey(this.m_KingName, "Character.name", new Vars(king), null);
		}
		if (this.m_KingAge != null)
		{
			UIText.SetTextKey(this.m_KingAge, "Character.age." + king.age.ToString(), null, null);
		}
		if (this.m_SupportedKingdomShield != null)
		{
			if (kingdom != null)
			{
				this.m_SupportedKingdomShield.gameObject.SetActive(true);
				this.m_SupportedKingdomShield.SetObject(kingdom, null);
				return;
			}
			this.m_SupportedKingdomShield.gameObject.SetActive(true);
		}
	}

	// Token: 0x0400148F RID: 5263
	[UIFieldTarget("id_VoteText")]
	private TextMeshProUGUI m_VoteText;

	// Token: 0x04001490 RID: 5264
	[UIFieldTarget("id_KingIcon")]
	private UICharacterIcon m_KingIcon;

	// Token: 0x04001491 RID: 5265
	[UIFieldTarget("id_KingName")]
	private TextMeshProUGUI m_KingName;

	// Token: 0x04001492 RID: 5266
	[UIFieldTarget("id_KingAge")]
	private TextMeshProUGUI m_KingAge;

	// Token: 0x04001493 RID: 5267
	[UIFieldTarget("id_SupportedKingdomShield")]
	private UIKingdomIcon m_SupportedKingdomShield;

	// Token: 0x04001494 RID: 5268
	[UIFieldTarget("id_VotingDecision")]
	private GameObject m_VotingDecision;

	// Token: 0x04001495 RID: 5269
	[UIFieldTarget("id_DecisionBackground")]
	private Image m_DecisionBackground;

	// Token: 0x04001496 RID: 5270
	[UIFieldTarget("id_DecisionFor")]
	private GameObject m_DecisionFor;

	// Token: 0x04001497 RID: 5271
	[UIFieldTarget("id_DecisionAgainst")]
	private GameObject m_DecisionAgainst;

	// Token: 0x04001498 RID: 5272
	[UIFieldTarget("id_VoteWeight")]
	private TextMeshProUGUI m_VoteWeight;

	// Token: 0x04001499 RID: 5273
	private UIEmperorOfTheWorldWindow parent;

	// Token: 0x0400149A RID: 5274
	private bool m_Initialized;
}
