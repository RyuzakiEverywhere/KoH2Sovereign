using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001FC RID: 508
public class UIEmperorOfTheWorldWindow : UIWindow, IListener
{
	// Token: 0x06001EDC RID: 7900 RVA: 0x0011DD2A File Offset: 0x0011BF2A
	public override string GetDefId()
	{
		return UIEmperorOfTheWorldWindow.def_id;
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06001EDD RID: 7901 RVA: 0x0011DD31 File Offset: 0x0011BF31
	// (set) Token: 0x06001EDE RID: 7902 RVA: 0x0011DD39 File Offset: 0x0011BF39
	public GreatPowers Data { get; private set; }

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06001EDF RID: 7903 RVA: 0x0011DD42 File Offset: 0x0011BF42
	private Logic.Kingdom currentVoter
	{
		get
		{
			if (this.currentVoterIndex < 0 || this.currentVoterIndex >= this.voters.Count)
			{
				return null;
			}
			return this.voters[this.currentVoterIndex];
		}
	}

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06001EE0 RID: 7904 RVA: 0x0011DD73 File Offset: 0x0011BF73
	// (set) Token: 0x06001EE1 RID: 7905 RVA: 0x0011DD7B File Offset: 0x0011BF7B
	private float candidateFame
	{
		get
		{
			return this.candidate1Fame;
		}
		set
		{
			this.candidate1Fame = value;
		}
	}

	// Token: 0x06001EE2 RID: 7906 RVA: 0x0011DD84 File Offset: 0x0011BF84
	public void StartVote(Vars vars)
	{
		EmperorOfTheWorld emperorOfTheWorld = GameLogic.Get(true).emperorOfTheWorld;
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
		this.logic = emperorOfTheWorld;
		if (emperorOfTheWorld != null)
		{
			emperorOfTheWorld.AddListener(this);
		}
		this.candidates = vars.Get<List<Logic.Kingdom>>("candidates", null);
		this.voters = vars.Get<List<Logic.Kingdom>>("voters", null);
		this.votes = vars.Get<List<int>>("votes", null);
		this.votes_weights = vars.Get<List<List<int>>>("votes_weights", null);
		if (this.candidates == null || this.candidates.Count == 0 || this.voters == null || this.votes == null || this.votes.Count != this.voters.Count)
		{
			this.Close(false);
			return;
		}
		this.total_voters_fame = 0f;
		this.votesPCTexts.Clear();
		this.abstainTexts.Clear();
		for (int i = 0; i < this.voters.Count; i++)
		{
			this.total_voters_fame += this.voters[i].fame;
			this.votesPCTexts.Add(this.GetProConTexts(this.voters[i], this.votes[i]));
			this.abstainTexts.Add(global::Defs.Localize("EmperorOfTheWorldWindow.Abstain_Text", null, null, true, true));
		}
		this.currentVoterIndex = -1;
		this.candidate1Fame = 0f;
		this.candidate2Fame = 0f;
		this.newEmperor = null;
		this.Init();
		if (emperorOfTheWorld.game.campaign.IsMultiplayerCampaign())
		{
			this.SetPhase(UIEmperorOfTheWorldWindow.Phase.Voting);
			this.HandleNextVoter();
		}
		else
		{
			this.SetPhase(UIEmperorOfTheWorldWindow.Phase.Preparing);
			this.timeout = emperorOfTheWorld.def.prepare_duration;
			Vars vars2 = new Vars();
			if (this.UsingSingleCandidate())
			{
				vars2.Set<Logic.Kingdom>("single_candidate", this.candidates[0]);
			}
			vars2.Set<Logic.Kingdom>("first_voter", this.voters[0]);
			UIText.SetText(this.m_PrepareDescription, global::Defs.Localize("EmperorOfTheWorldWindow.Prepare_Text", vars2, null, true, true));
		}
		this.RefreshCandidates();
		this.RefreshRanking();
		this.LocalizeStatic();
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x0011DFB0 File Offset: 0x0011C1B0
	private void LocalizeStatic()
	{
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "EmperorOfTheWorldWindow.caption", null, null);
		}
		if (this.m_VotesForText != null)
		{
			UIText.SetTextKey(this.m_VotesForText, "EmperorOfTheWorldWindow.in_favor", null, null);
		}
		if (this.m_VotesAgainstText != null)
		{
			UIText.SetTextKey(this.m_VotesAgainstText, "EmperorOfTheWorldWindow.against", null, null);
		}
		if (this.m_SupportClaimLabel != null)
		{
			UIText.SetTextKey(this.m_SupportClaimLabel, "EmperorOfTheWorldWindow.support_claim", null, null);
		}
		if (this.m_RejectClaimLabel != null)
		{
			UIText.SetTextKey(this.m_RejectClaimLabel, "EmperorOfTheWorldWindow.vote_against", null, null);
		}
		if (this.m_SupportC1Label != null)
		{
			UIText.SetTextKey(this.m_SupportC1Label, "EmperorOfTheWorldWindow.vote_for", null, null);
		}
		if (this.m_SupportC2Label != null)
		{
			UIText.SetTextKey(this.m_SupportC2Label, "EmperorOfTheWorldWindow.vote_for", null, null);
		}
		if (this.m_ButtonCloseLabel != null)
		{
			UIText.SetTextKey(this.m_ButtonCloseLabel, "EmperorOfTheWorldWindow.back_to_game", null, null);
		}
		if (this.m_WaitEmperorResponseTextOther != null)
		{
			UIText.SetTextKey(this.m_WaitEmperorResponseTextOther, "EmperorOfTheWorldWindow.waiting_for_response_from_emperor_candidate", null, null);
		}
		if (this.m_WaitEmperorResponseTextPlayer != null)
		{
			UIText.SetTextKey(this.m_WaitEmperorResponseTextPlayer, "EmperorOfTheWorldWindow.waiting_for_response_from_player_emperor_candidate", null, null);
		}
		if (this.m_AcceptLabel != null)
		{
			UIText.SetTextKey(this.m_AcceptLabel, "EmperorOfTheWorldWindow.accept", null, null);
		}
		if (this.m_RejectLabel != null)
		{
			UIText.SetTextKey(this.m_RejectLabel, "EmperorOfTheWorldWindow.reject", null, null);
		}
		if (this.m_Button_AcceptBeingEmperorLabel != null)
		{
			UIText.SetTextKey(this.m_Button_AcceptBeingEmperorLabel, "EmperorOfTheWorldWindow.accept", null, null);
		}
		if (this.m_Button_RejectBeingEmperorLabel != null)
		{
			UIText.SetTextKey(this.m_Button_RejectBeingEmperorLabel, "EmperorOfTheWorldWindow.reject", null, null);
		}
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x0011E180 File Offset: 0x0011C380
	private void SetPhase(UIEmperorOfTheWorldWindow.Phase phase)
	{
		this.phase = phase;
		GameObject group_Prepare = this.m_Group_Prepare;
		if (group_Prepare != null)
		{
			group_Prepare.SetActive(phase == UIEmperorOfTheWorldWindow.Phase.Preparing);
		}
		GameObject group_Voter = this.m_Group_Voter;
		if (group_Voter != null)
		{
			group_Voter.SetActive(phase == UIEmperorOfTheWorldWindow.Phase.Voting);
		}
		GameObject group_WaitEmperorResponse = this.m_Group_WaitEmperorResponse;
		if (group_WaitEmperorResponse != null)
		{
			group_WaitEmperorResponse.SetActive(phase == UIEmperorOfTheWorldWindow.Phase.WaitingEmperorResponse);
		}
		GameObject group_RejectAIEmperor = this.m_Group_RejectAIEmperor;
		if (group_RejectAIEmperor != null)
		{
			group_RejectAIEmperor.SetActive(phase == UIEmperorOfTheWorldWindow.Phase.RejectAIEmperor);
		}
		GameObject group_Final = this.m_Group_Final;
		if (group_Final == null)
		{
			return;
		}
		group_Final.SetActive(phase == UIEmperorOfTheWorldWindow.Phase.Final);
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x0011E1FC File Offset: 0x0011C3FC
	public int GetCurrentVoterIndex()
	{
		return this.currentVoterIndex;
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x0011E204 File Offset: 0x0011C404
	public string GetVoteText(int voterIdx, Logic.Kingdom candidate)
	{
		return this.GetVoteText(voterIdx, this.GetCandidateIdx(candidate));
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x0011E214 File Offset: 0x0011C414
	public string GetVoteText(int voterIdx, int candidateIdx)
	{
		if (candidateIdx != -1)
		{
			return this.votesPCTexts[voterIdx][candidateIdx];
		}
		if (this.UsingSingleCandidate())
		{
			return this.votesPCTexts[voterIdx][0];
		}
		return this.abstainTexts[voterIdx];
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x0011E254 File Offset: 0x0011C454
	public Logic.Kingdom GetVoter(int voterIdx)
	{
		return this.voters[voterIdx];
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x0011E264 File Offset: 0x0011C464
	public Logic.Kingdom GetVoterSupport(int voterIdx)
	{
		if (this.votes == null)
		{
			return null;
		}
		if (voterIdx == -1)
		{
			return null;
		}
		if (voterIdx > this.votes.Count)
		{
			return null;
		}
		if (this.candidates == null || this.candidates.Count == 0)
		{
			return null;
		}
		for (int i = 0; i < this.candidates.Count; i++)
		{
			if (this.votes[voterIdx] == this.candidates[i].id)
			{
				return this.candidates[i];
			}
		}
		return null;
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x0011E2EC File Offset: 0x0011C4EC
	public Logic.Kingdom GetCandidate(int vote)
	{
		if (vote == -1)
		{
			return null;
		}
		for (int i = 0; i < this.candidates.Count; i++)
		{
			if (this.candidates[i].id == vote)
			{
				return this.candidates[i];
			}
		}
		return null;
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x0011E338 File Offset: 0x0011C538
	public int GetCandidateIdx(Logic.Kingdom candidate)
	{
		if (candidate == null)
		{
			return -1;
		}
		for (int i = 0; i < this.candidates.Count; i++)
		{
			if (this.candidates[i] == candidate)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x0011E372 File Offset: 0x0011C572
	private bool UsingSingleCandidate()
	{
		return this.candidates.Count == 1;
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x0011E384 File Offset: 0x0011C584
	private bool IsCandidate(Logic.Kingdom k)
	{
		if (this.candidates == null)
		{
			return false;
		}
		for (int i = 0; i < this.candidates.Count; i++)
		{
			if (k == this.candidates[i])
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x0011E3C4 File Offset: 0x0011C5C4
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ButtonClose != null)
		{
			this.m_ButtonClose.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		if (this.m_Button_Final_Close != null)
		{
			this.m_Button_Final_Close.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		this.playerKingdom = BaseUI.LogicKingdom();
		if (this.m_KingdomSlotPrototype != null)
		{
			this.m_KingdomSlotPrototype.gameObject.SetActive(false);
		}
		if (this.m_CandidateSingleVote != null)
		{
			this.m_CandidateSingleVote.AddComponent<UIEmperorOfTheWorldWindow.TroneCandidate>();
		}
		if (this.m_Candidate1 != null)
		{
			this.m_Candidate1.AddComponent<UIEmperorOfTheWorldWindow.TroneCandidate>();
		}
		if (this.m_Candidate2 != null)
		{
			this.m_Candidate2.AddComponent<UIEmperorOfTheWorldWindow.TroneCandidate>();
		}
		if (this.m_VoterInfo != null)
		{
			this.m_VoterInfo.GetOrAddComponent<UIEmperorOfTheWorldWindow.VoterInfo>();
		}
		this.AllocateRankingIcons();
		this.AllocateVoteRows();
		this.draggable = false;
		this.m_Initialzied = true;
		FMODVoiceProvider.ClearAllInactiveVoices();
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x0011E4DC File Offset: 0x0011C6DC
	private void RefreshCandidates()
	{
		this.m_Group_VotesSingleCandidate.SetActive(this.UsingSingleCandidate());
		this.m_Group_VotesMultipleCandidates.SetActive(!this.UsingSingleCandidate());
		if (!this.UsingSingleCandidate())
		{
			GameObject candidate = this.m_Candidate1;
			if (candidate != null)
			{
				candidate.GetComponent<UIEmperorOfTheWorldWindow.TroneCandidate>().SetObject(this.candidates[0]);
			}
			GameObject candidate2 = this.m_Candidate2;
			if (candidate2 != null)
			{
				candidate2.GetComponent<UIEmperorOfTheWorldWindow.TroneCandidate>().SetObject(this.candidates[1]);
			}
			if (this.m_VotesC1Text != null)
			{
				TMP_Text votesC1Text = this.m_VotesC1Text;
				string key = "Character.name";
				Logic.Kingdom kingdom = this.candidates[0];
				UIText.SetTextKey(votesC1Text, key, new Vars((kingdom != null) ? kingdom.GetKing() : null), null);
			}
			if (this.m_VotesC2Text != null)
			{
				TMP_Text votesC2Text = this.m_VotesC2Text;
				string key2 = "Character.name";
				Logic.Kingdom kingdom2 = this.candidates[1];
				UIText.SetTextKey(votesC2Text, key2, new Vars((kingdom2 != null) ? kingdom2.GetKing() : null), null);
			}
			return;
		}
		GameObject candidateSingleVote = this.m_CandidateSingleVote;
		if (candidateSingleVote == null)
		{
			return;
		}
		candidateSingleVote.GetComponent<UIEmperorOfTheWorldWindow.TroneCandidate>().SetObject(this.candidates[0]);
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x0011E600 File Offset: 0x0011C800
	private void AllocateVoteRows()
	{
		if (this.m_voteRowPrototype == null)
		{
			return;
		}
		if (this.m_VotesForContainer != null)
		{
			global::Common.DestroyChildren(this.m_VotesForContainer);
		}
		if (this.m_VotesAgainstContainer != null)
		{
			global::Common.DestroyChildren(this.m_VotesAgainstContainer);
		}
		if (this.m_VotesForC1Container != null)
		{
			global::Common.DestroyChildren(this.m_VotesForC1Container);
		}
		if (this.m_VotesForC2Container != null)
		{
			global::Common.DestroyChildren(this.m_VotesForC2Container);
		}
		int num = this.logic.game.great_powers.MaxGreatPowers() - 1;
		for (int i = 0; i < num; i++)
		{
			if (this.m_VotesForContainer != null)
			{
				UIEmperorOfTheWorldWindowRow orAddComponent = global::Common.SpawnPooled(this.m_voteRowPrototype, this.m_VotesForContainer.transform, false, "").GetOrAddComponent<UIEmperorOfTheWorldWindowRow>();
				if (orAddComponent != null)
				{
					this.voteRowsFor.Add(orAddComponent);
				}
			}
			if (this.m_VotesAgainstContainer != null)
			{
				UIEmperorOfTheWorldWindowRow orAddComponent2 = global::Common.SpawnPooled(this.m_voteRowPrototype, this.m_VotesAgainstContainer.transform, false, "").GetOrAddComponent<UIEmperorOfTheWorldWindowRow>();
				if (orAddComponent2 != null)
				{
					this.voteRowsAgainst.Add(orAddComponent2);
				}
			}
			if (this.m_VotesForC1Container != null)
			{
				UIEmperorOfTheWorldWindowRow orAddComponent3 = global::Common.SpawnPooled(this.m_voteRowPrototype, this.m_VotesForC1Container.transform, false, "").GetOrAddComponent<UIEmperorOfTheWorldWindowRow>();
				if (orAddComponent3 != null)
				{
					this.voteRowsForC1.Add(orAddComponent3);
				}
			}
			if (this.m_VotesForC2Container != null)
			{
				UIEmperorOfTheWorldWindowRow orAddComponent4 = global::Common.SpawnPooled(this.m_voteRowPrototype, this.m_VotesForC2Container.transform, false, "").GetOrAddComponent<UIEmperorOfTheWorldWindowRow>();
				if (orAddComponent4 != null)
				{
					this.voteRowsForC2.Add(orAddComponent4);
				}
			}
		}
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x0011E7C4 File Offset: 0x0011C9C4
	private void AllocateRankingIcons()
	{
		if (this.m_KingdomSlotPrototype == null)
		{
			return;
		}
		if (this.m_GreathPowersContianer == null)
		{
			return;
		}
		int num = 9;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = global::Common.Spawn(this.m_KingdomSlotPrototype, this.m_GreathPowersContianer, false, "");
			UIEmperorOfTheWorldWindow.KingdomSlot kingdomSlot = gameObject.AddComponent<UIEmperorOfTheWorldWindow.KingdomSlot>();
			gameObject.SetActive(true);
			this.m_TopRankingKingdoms.Add(kingdomSlot);
			gameObject.GetOrAddComponent<LayoutElement>();
			if (i % 2 == 0)
			{
				gameObject.transform.SetAsFirstSibling();
			}
			else
			{
				gameObject.transform.SetAsLastSibling();
			}
			if (i != 0 && i < 3)
			{
				kingdomSlot.SetCrestScale(this.m_FirstTierScale);
			}
			else if (i >= 3)
			{
				kingdomSlot.SetCrestScale(this.m_SecoundTierScale);
			}
			gameObject.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x0011E88C File Offset: 0x0011CA8C
	private void RefreshCandidatesFame()
	{
		this.candidate1Fame = 0f;
		this.candidate2Fame = 0f;
		int num = 0;
		while (num <= this.currentVoterIndex && num < this.voters.Count)
		{
			if (num <= this.currentVoterIndex - 1 || (num == this.currentVoterIndex && this.currentVoter != null && !this.currentVoter.is_player && this.showCurrentAIVote))
			{
				this.candidate1Fame += (float)this.votes_weights[num][0];
				if (this.candidates.Count > 1)
				{
					this.candidate2Fame += (float)this.votes_weights[num][1];
				}
			}
			num++;
		}
		if (this.UsingSingleCandidate())
		{
			GameObject candidateSingleVote = this.m_CandidateSingleVote;
			if (candidateSingleVote == null)
			{
				return;
			}
			candidateSingleVote.GetComponent<UIEmperorOfTheWorldWindow.TroneCandidate>().UpdateFame(this.candidate1Fame, this.total_voters_fame * this.logic.GetFameWinTreshold());
			return;
		}
		else
		{
			GameObject candidate = this.m_Candidate1;
			if (candidate != null)
			{
				candidate.GetComponent<UIEmperorOfTheWorldWindow.TroneCandidate>().UpdateFame(this.candidate1Fame, this.total_voters_fame * this.logic.GetFameWinTreshold());
			}
			GameObject candidate2 = this.m_Candidate2;
			if (candidate2 == null)
			{
				return;
			}
			candidate2.GetComponent<UIEmperorOfTheWorldWindow.TroneCandidate>().UpdateFame(this.candidate2Fame, this.total_voters_fame * this.logic.GetFameWinTreshold());
			return;
		}
	}

	// Token: 0x06001EF3 RID: 7923 RVA: 0x0011E9E8 File Offset: 0x0011CBE8
	public static string GetSlantText(Logic.Kingdom voter, Logic.Kingdom candidate, string defPath)
	{
		Vars vars = new Vars();
		vars.Set<Logic.Kingdom>("voter", voter);
		vars.Set<Logic.Kingdom>("candidate", candidate);
		return "#" + global::Defs.Localize(defPath + "." + voter.game.emperorOfTheWorld.CalcSlant(voter, candidate, -1).ToString(), vars, null, true, true);
	}

	// Token: 0x06001EF4 RID: 7924 RVA: 0x0011EA54 File Offset: 0x0011CC54
	public static string GetProConTooltipText(Logic.Kingdom voter, Logic.Kingdom candidate, string variant = "reason_to_them", bool pro = true)
	{
		ProsAndCons prosAndCons = voter.game.emperorOfTheWorld.GetProsAndCons(voter, candidate);
		string text = "";
		List<ProsAndCons.Factor> list = pro ? prosAndCons.pros : prosAndCons.cons;
		string str = pro ? "{check_mark} " : "{x_mark} ";
		for (int i = 0; i < list.Count; i++)
		{
			ProsAndCons.Factor factor = list[i];
			bool flag;
			if (factor == null)
			{
				flag = (null != null);
			}
			else
			{
				ProsAndCons.Factor.Def def = factor.def;
				flag = (((def != null) ? def.field : null) != null);
			}
			if (flag)
			{
				bool flag2;
				if (factor == null)
				{
					flag2 = (null != null);
				}
				else
				{
					ProsAndCons.Factor.Def def2 = factor.def;
					if (def2 == null)
					{
						flag2 = (null != null);
					}
					else
					{
						DT.Field field = def2.field;
						flag2 = (((field != null) ? field.FindChild(variant, null, true, true, true, '.') : null) != null);
					}
				}
				if (flag2 && factor.value != 0)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += "{p}";
					}
					text = text + str + global::Defs.Localize(factor.def.field, variant, candidate, null, true, true);
				}
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return "@" + text;
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x0011EB5C File Offset: 0x0011CD5C
	private void RefreshVoteButtons()
	{
		if (this.UsingSingleCandidate())
		{
			if (this.m_Group_VoterButtonsSinglecandidate != null)
			{
				this.m_Group_VoterButtonsSinglecandidate.SetActive(this.currentVoter != null && this.currentVoter == this.playerKingdom);
			}
			if (this.m_Group_VoterButtonsMultipleCandidates != null)
			{
				this.m_Group_VoterButtonsMultipleCandidates.SetActive(false);
			}
			if (this.m_Button_For != null)
			{
				Vars vars = new Vars();
				vars.Set<Logic.Kingdom>("candidate", this.candidates[0]);
				vars.Set<int>("vote_weight", this.logic.CalcVoteWeight(this.playerKingdom, this.candidates[0], this.candidates[0].id));
				vars.Set<string>("slant_text", UIEmperorOfTheWorldWindow.GetSlantText(this.playerKingdom, this.candidates[0], "EmperorVoteForTooltip.SlantTexts"));
				vars.Set<string>("pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[0], "reason_to_us", true));
				vars.Set<string>("con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[0], "reason_to_us", false));
				Tooltip.Get(this.m_Button_For.gameObject, true).SetDef("EmperorVoteForTooltip", vars);
				this.m_Button_For.onClick = new BSGButton.OnClick(this.HandleOnVoteFor);
			}
			if (this.m_Button_Against != null)
			{
				Vars vars2 = new Vars();
				vars2.Set<Logic.Kingdom>("candidate", this.candidates[0]);
				vars2.Set<int>("vote_weight", this.logic.CalcVoteWeight(this.playerKingdom, this.candidates[0], -1));
				vars2.Set<string>("slant_text", UIEmperorOfTheWorldWindow.GetSlantText(this.playerKingdom, this.candidates[0], "EmperorVoteAgainstTooltip.SlantTexts"));
				vars2.Set<string>("pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[0], "reason_to_us", true));
				vars2.Set<string>("con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[0], "reason_to_us", false));
				Tooltip.Get(this.m_Button_Against.gameObject, true).SetDef("EmperorVoteAgainstTooltip", vars2);
				this.m_Button_Against.onClick = new BSGButton.OnClick(this.HandleOnVoteAgainst);
			}
		}
		else
		{
			if (this.m_Group_VoterButtonsSinglecandidate != null)
			{
				this.m_Group_VoterButtonsSinglecandidate.SetActive(false);
			}
			if (this.m_Group_VoterButtonsMultipleCandidates != null)
			{
				this.m_Group_VoterButtonsMultipleCandidates.SetActive(this.currentVoter != null && this.currentVoter == this.playerKingdom);
			}
			if (this.m_Button_ForC1 != null)
			{
				Vars vars3 = new Vars();
				vars3.Set<Logic.Kingdom>("candidate", this.candidates[0]);
				vars3.Set<int>("vote_weight", this.logic.CalcVoteWeight(this.playerKingdom, this.candidates[0], this.candidates[0].id));
				vars3.Set<string>("slant_text", UIEmperorOfTheWorldWindow.GetSlantText(this.playerKingdom, this.candidates[0], "EmperorVoteForTooltip.SlantTexts"));
				vars3.Set<string>("pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[0], "reason_to_us", true));
				vars3.Set<string>("con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[0], "reason_to_us", false));
				Tooltip.Get(this.m_Button_ForC1.gameObject, true).SetDef("EmperorVoteForTooltip", vars3);
				this.m_Button_ForC1.onClick = new BSGButton.OnClick(this.HandleOnVoteFor);
			}
			if (this.m_Button_ForC2 != null)
			{
				Vars vars4 = new Vars();
				vars4.Set<Logic.Kingdom>("candidate", this.candidates[1]);
				vars4.Set<int>("vote_weight", this.logic.CalcVoteWeight(this.playerKingdom, this.candidates[1], this.candidates[1].id));
				vars4.Set<string>("slant_text", UIEmperorOfTheWorldWindow.GetSlantText(this.playerKingdom, this.candidates[1], "EmperorVoteForTooltip.SlantTexts"));
				vars4.Set<string>("pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[1], "reason_to_us", true));
				vars4.Set<string>("con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[1], "reason_to_us", false));
				Tooltip.Get(this.m_Button_ForC2.gameObject, true).SetDef("EmperorVoteForTooltip", vars4);
				this.m_Button_ForC2.onClick = new BSGButton.OnClick(this.HandleOnVoteForC2);
			}
			if (this.m_Button_Abstain != null)
			{
				Vars vars5 = new Vars();
				vars5.Set<Logic.Kingdom>("candidate1", this.candidates[1]);
				vars5.Set<int>("c1_vote_weight", this.logic.CalcVoteWeight(this.playerKingdom, this.candidates[0], -1));
				vars5.Set<string>("c1_slant_text", UIEmperorOfTheWorldWindow.GetSlantText(this.playerKingdom, this.candidates[0], "EmperorVoteAbstainTooltip.SlantTexts"));
				vars5.Set<string>("c1_pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[0], "reason_to_us", true));
				vars5.Set<string>("c1_con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[0], "reason_to_us", false));
				vars5.Set<Logic.Kingdom>("candidate2", this.candidates[1]);
				vars5.Set<int>("c2_vote_weight", this.logic.CalcVoteWeight(this.playerKingdom, this.candidates[1], -1));
				vars5.Set<string>("c2_slant_text", UIEmperorOfTheWorldWindow.GetSlantText(this.playerKingdom, this.candidates[1], "EmperorVoteAbstainTooltip.SlantTexts"));
				vars5.Set<string>("c2_pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[1], "reason_to_us", true));
				vars5.Set<string>("c2_con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.playerKingdom, this.candidates[1], "reason_to_us", false));
				Tooltip.Get(this.m_Button_Abstain.gameObject, true).SetDef("EmperorVoteAbstainTooltip", vars5);
				this.m_Button_Abstain.onClick = new BSGButton.OnClick(this.HandleOnVoteAgainst);
			}
			if (this.m_CrestC1 != null)
			{
				Logic.Kingdom kingdom = (this.candidates != null && this.candidates.Count > 0) ? this.candidates[0] : null;
				this.m_CrestC1.SetObject(kingdom, null);
				this.m_CrestC1.gameObject.SetActive(kingdom != null);
			}
			if (this.m_CrestC2 != null)
			{
				Logic.Kingdom kingdom2 = (this.candidates != null && this.candidates.Count > 1) ? this.candidates[1] : null;
				this.m_CrestC2.SetObject(kingdom2, null);
				this.m_CrestC2.gameObject.SetActive(kingdom2 != null);
			}
		}
		if (this.m_Button_SkipAIVoter != null)
		{
			Tooltip.Get(this.m_Button_SkipAIVoter.gameObject, true).SetDef("EmperorNextVoteTooltip", null);
			if (!this.currentVoter.is_player && !this.currentVoter.game.IsMultiplayer())
			{
				this.m_Button_SkipAIVoter.gameObject.SetActive(true);
				this.m_Button_SkipAIVoter.onClick = new BSGButton.OnClick(this.HandleSkipAIVoter);
				return;
			}
			this.m_Button_SkipAIVoter.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x0011F33C File Offset: 0x0011D53C
	private void RefreshVoter()
	{
		if (this.currentVoterIndex >= this.voters.Count)
		{
			this.m_Group_Voter.SetActive(false);
			return;
		}
		this.m_Group_Voter.SetActive(true);
		if (this.m_VoterInfo != null)
		{
			this.m_VoterInfo.GetOrAddComponent<UIEmperorOfTheWorldWindow.VoterInfo>().SetObject(this, this.currentVoter);
		}
		this.RefreshVoteButtons();
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x0011F3A0 File Offset: 0x0011D5A0
	private void RefreshVotes()
	{
		this.RefreshCandidatesFame();
		bool flag = this.UsingSingleCandidate();
		int num = this.logic.game.great_powers.MaxGreatPowers() - 1;
		for (int i = 0; i < num; i++)
		{
			bool flag2 = i < this.votes.Count;
			flag2 &= (i <= this.currentVoterIndex - 1 || (i == this.currentVoterIndex && this.currentVoter != null && !this.currentVoter.is_player && this.showCurrentAIVote));
			if (flag)
			{
				this.voteRowsFor[i].gameObject.SetActive(flag2 && this.votes[i] == this.candidates[0].id);
				this.voteRowsAgainst[i].gameObject.SetActive(flag2 && this.votes[i] != this.candidates[0].id);
				if (this.voteRowsFor[i].gameObject.activeSelf)
				{
					this.voteRowsFor[i].SetData(this, i, 0);
				}
				if (this.voteRowsAgainst[i].gameObject.activeSelf)
				{
					this.voteRowsAgainst[i].SetData(this, i, 0);
				}
			}
			else
			{
				this.voteRowsForC1[i].gameObject.SetActive(flag2 && (this.votes[i] == this.candidates[0].id || this.votes[i] == -1));
				this.voteRowsForC2[i].gameObject.SetActive(flag2 && (this.votes[i] == this.candidates[1].id || this.votes[i] == -1));
				if (this.voteRowsForC1[i].gameObject.activeSelf)
				{
					this.voteRowsForC1[i].SetData(this, i, 0);
				}
				if (this.voteRowsForC2[i].gameObject.activeSelf)
				{
					this.voteRowsForC2[i].SetData(this, i, 1);
				}
			}
		}
		for (int j = 0; j < this.m_TopRankingKingdoms.Count; j++)
		{
			if (this.m_TopRankingKingdoms[j].Kingdom == null)
			{
				this.m_TopRankingKingdoms[j].UpdateState(flag, false, -1, false, false, null, false, this.candidates);
			}
			else
			{
				int voterIndex = this.GetVoterIndex(this.m_TopRankingKingdoms[j].Kingdom);
				bool flag3 = this.candidates.Contains(this.m_TopRankingKingdoms[j].Kingdom);
				int vote = this.GetVote(this.m_TopRankingKingdoms[j].Kingdom);
				bool flag4 = !flag3 && this.currentVoterIndex < voterIndex;
				bool flag5 = !flag3 && this.currentVoterIndex == voterIndex;
				bool flag6 = this.currentVoter != null && this.currentVoter == this.m_TopRankingKingdoms[j].Kingdom && !this.currentVoter.is_player && this.showCurrentAIVote;
				Logic.Kingdom votedFor = (!flag3 && ((!flag4 && !flag5) || flag6)) ? this.GetVoterSupport(voterIndex) : null;
				this.m_TopRankingKingdoms[j].UpdateState(flag, flag4, vote, flag5, flag6, votedFor, flag3, this.candidates);
			}
		}
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x0011F750 File Offset: 0x0011D950
	private int GetVoterIndex(Logic.Kingdom k)
	{
		if (k == null)
		{
			return -1;
		}
		for (int i = 0; i < this.voters.Count; i++)
		{
			if (k == this.voters[i])
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x0011F78C File Offset: 0x0011D98C
	private int GetVote(Logic.Kingdom k)
	{
		if (k == null)
		{
			return -1;
		}
		for (int i = 0; i < this.voters.Count; i++)
		{
			if (k == this.voters[i])
			{
				return this.votes[i];
			}
		}
		return -1;
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x0011F7D4 File Offset: 0x0011D9D4
	private void RefreshRanking()
	{
		EmperorOfTheWorld emperorOfTheWorld = this.logic;
		List<Logic.Kingdom> list;
		if (emperorOfTheWorld == null)
		{
			list = null;
		}
		else
		{
			Game game = emperorOfTheWorld.game;
			if (game == null)
			{
				list = null;
			}
			else
			{
				GreatPowers great_powers = game.great_powers;
				list = ((great_powers != null) ? great_powers.TopKingdoms(false) : null);
			}
		}
		List<Logic.Kingdom> list2 = list;
		for (int i = 0; i < this.m_TopRankingKingdoms.Count; i++)
		{
			Logic.Kingdom k = (i < list2.Count) ? list2[i] : null;
			this.m_TopRankingKingdoms[i].SetData(this, k);
		}
		this.RefreshVotes();
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x0011F850 File Offset: 0x0011DA50
	private ProsAndCons.Factor GetBestPro(Logic.Kingdom voter, Logic.Kingdom candidate)
	{
		ProsAndCons prosAndCons = ProsAndCons.Get("PC_NominateEmperorOfTheWorld", voter, candidate);
		if (prosAndCons == null)
		{
			return null;
		}
		prosAndCons.Calc(false);
		if (voter.is_player)
		{
			return prosAndCons.pros.Find((ProsAndCons.Factor p) => p.def.field.key == "pc_we_have_good_relation");
		}
		List<ProsAndCons.Factor> list = new List<ProsAndCons.Factor>(prosAndCons.pros);
		list.Sort((ProsAndCons.Factor x, ProsAndCons.Factor y) => y.value.CompareTo(x.value));
		return list[0];
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x0011F8E0 File Offset: 0x0011DAE0
	private ProsAndCons.Factor GetWorstCon(Logic.Kingdom voter, Logic.Kingdom candidate)
	{
		ProsAndCons prosAndCons = this.logic.GetProsAndCons(voter, candidate);
		if (prosAndCons == null)
		{
			return null;
		}
		if (voter.is_player)
		{
			return prosAndCons.cons.Find((ProsAndCons.Factor c) => c.def.field.key == "pc_we_have_bad_relation");
		}
		List<ProsAndCons.Factor> list = new List<ProsAndCons.Factor>(prosAndCons.cons);
		list.Sort((ProsAndCons.Factor x, ProsAndCons.Factor y) => y.value.CompareTo(x.value));
		return list[0];
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x0011F96C File Offset: 0x0011DB6C
	private List<string> GetProConTexts(Logic.Kingdom voter, int vote)
	{
		List<string> list = new List<string>(this.candidates.Count);
		for (int i = 0; i < this.candidates.Count; i++)
		{
			list.Add(this.GetProConText(voter, this.candidates[i], vote));
		}
		return list;
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x0011F9BC File Offset: 0x0011DBBC
	private string GetProConText(Logic.Kingdom voter, Logic.Kingdom candidate, int vote)
	{
		if (candidate != null && candidate.id != vote)
		{
			Vars vars = new Vars(candidate);
			vars.Set<Logic.Kingdom>("voter", voter);
			if (this.UsingSingleCandidate())
			{
				ProsAndCons.Factor worstCon = this.GetWorstCon(voter, candidate);
				DT.Field field;
				if (worstCon == null)
				{
					field = null;
				}
				else
				{
					ProsAndCons.Factor.Def def = worstCon.def;
					field = ((def != null) ? def.field : null);
				}
				return global::Defs.Localize(field, "eotw_against", vars, null, true, true);
			}
			return global::Defs.Localize("EmperorOfTheWorldWindow.Abstain_Text_Individual", vars, null, true, true);
		}
		else
		{
			Vars vars2 = new Vars(candidate);
			vars2.Set<Logic.Kingdom>("voter", voter);
			if (this.UsingSingleCandidate())
			{
				ProsAndCons.Factor bestPro = this.GetBestPro(voter, candidate);
				DT.Field field2;
				if (bestPro == null)
				{
					field2 = null;
				}
				else
				{
					ProsAndCons.Factor.Def def2 = bestPro.def;
					field2 = ((def2 != null) ? def2.field : null);
				}
				return global::Defs.Localize(field2, "eotw_support", vars2, null, true, true);
			}
			ProsAndCons.Factor bestPro2 = this.GetBestPro(voter, candidate);
			DT.Field field3;
			if (bestPro2 == null)
			{
				field3 = null;
			}
			else
			{
				ProsAndCons.Factor.Def def3 = bestPro2.def;
				field3 = ((def3 != null) ? def3.field : null);
			}
			return global::Defs.Localize(field3, "eotw_support_kingdom", vars2, null, true, true);
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06001EFF RID: 7935 RVA: 0x0011FAB0 File Offset: 0x0011DCB0
	// (set) Token: 0x06001F00 RID: 7936 RVA: 0x0011FAB8 File Offset: 0x0011DCB8
	public bool showCurrentAIVote { get; private set; }

	// Token: 0x06001F01 RID: 7937 RVA: 0x0011FAC1 File Offset: 0x0011DCC1
	private void ShowCurrentAIVote()
	{
		this.showCurrentAIVote = true;
		this.RefreshVotes();
		this.RefreshVoter();
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x0011FAD8 File Offset: 0x0011DCD8
	private void HandleNextVoter()
	{
		this.currentVoterIndex++;
		this.showCurrentAIVote = false;
		this.RefreshVotes();
		this.RefreshVoter();
		if (this.currentVoterIndex >= this.voters.Count)
		{
			this.timeout = 2f;
			this.logic.SetEndVoting();
			return;
		}
		Logic.Kingdom kingdom = this.voters[this.currentVoterIndex];
		this.timeout = ((kingdom != null && kingdom.is_player) ? this.logic.def.player_action_time_out : this.logic.def.ai_action_time_out);
		this.maxTimeOut = this.timeout;
		this.aiVoteDelay = this.logic.def.ai_think_delay;
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x0011FB98 File Offset: 0x0011DD98
	private void SetWaitEmperorResponse(Logic.Kingdom emperor)
	{
		this.SetPhase(UIEmperorOfTheWorldWindow.Phase.WaitingEmperorResponse);
		this.newEmperor = emperor;
		this.timeout = this.logic.def.player_action_time_out;
		if (BaseUI.LogicKingdom() == emperor)
		{
			this.m_EmperorResponsePlayer.SetActive(true);
			this.m_EmperorResponseOther.SetActive(false);
			this.m_Button_AcceptBeingEmperor.onClick = new BSGButton.OnClick(this.HandleOnAcceptBeingEmperor);
			this.m_Button_RejectBeingEmperor.onClick = new BSGButton.OnClick(this.HandleOnRejectBeingEmperor);
			return;
		}
		this.m_EmperorResponsePlayer.SetActive(false);
		this.m_EmperorResponseOther.SetActive(true);
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x0011FC30 File Offset: 0x0011DE30
	private void SetPlayerRejectAIEmperor(Logic.Kingdom emperor)
	{
		this.SetPhase(UIEmperorOfTheWorldWindow.Phase.RejectAIEmperor);
		this.newEmperor = emperor;
		UIText.SetTextKey(this.m_RejectAIEmperorText, "EmperorOfTheWorldWindow.RejectAIEmperor", emperor, null);
		this.m_Button_AcceptAIEmperor.onClick = new BSGButton.OnClick(this.HandleOnAIEmperorAccept);
		this.m_Button_RejectAIEmperor.onClick = new BSGButton.OnClick(this.HandleOnAIEmperorReject);
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x0011FC8C File Offset: 0x0011DE8C
	public static bool TimersPaused()
	{
		Game game = GameLogic.Get(false);
		return ((game != null) ? game.pause : null) != null && game.pause.HasRequest("EoWPause", -2) && game.pause.requests.Count > 1;
	}

	// Token: 0x06001F06 RID: 7942 RVA: 0x0011FCDC File Offset: 0x0011DEDC
	protected override void Update()
	{
		base.Update();
		if (UIEmperorOfTheWorldWindow.TimersPaused())
		{
			return;
		}
		float unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
		this.timeout -= unscaledDeltaTime;
		this.aiVoteDelay -= unscaledDeltaTime;
		UIText.SetText(this.id_tmp_timeout, "Timeout: " + this.timeout.ToString());
		UIText.SetText(this.id_tmp_ai_timeout, "AI Delay: " + this.aiVoteDelay.ToString());
		UIText.SetText(this.id_tmp_phase, "Phase: " + this.phase.ToString());
		Game game = GameLogic.Get(false);
		bool flag = ((game != null) ? game.pause : null) != null && game.pause.IsMultiplayer();
		if (this.phase == UIEmperorOfTheWorldWindow.Phase.Preparing)
		{
			this.UpdatePreparingTimeoutTimer();
			if (this.timeout < 0f)
			{
				this.SetPhase(UIEmperorOfTheWorldWindow.Phase.Voting);
				this.HandleNextVoter();
			}
		}
		if (this.phase == UIEmperorOfTheWorldWindow.Phase.Voting)
		{
			if (!flag && (!this.currentVoter.is_player || this.currentVoter == this.playerKingdom))
			{
				this.timeout = this.maxTimeOut;
				this.aiVoteDelay = -1f;
			}
			this.UpdateVoterTimeoutTimer();
			if (this.currentVoterIndex >= this.voters.Count)
			{
				if (this.timeout < 0f)
				{
					this.HandleNextVoter();
				}
				return;
			}
			if (this.timeout < 0f && this.currentVoter == this.playerKingdom)
			{
				this.logic.SetPlayerVote(this.currentVoterIndex, -1);
			}
			else if (this.timeout < 0f)
			{
				this.HandleNextVoter();
			}
			else if (!this.currentVoter.is_player && this.aiVoteDelay < 0f)
			{
				if (flag)
				{
					this.HandleNextVoter();
				}
				else
				{
					this.ShowCurrentAIVote();
				}
			}
		}
		if (this.phase == UIEmperorOfTheWorldWindow.Phase.WaitingEmperorResponse && this.timeout < 0f)
		{
			this.logic.TrySetEmperorOfTheWorld(this.newEmperor, null);
		}
	}

	// Token: 0x06001F07 RID: 7943 RVA: 0x0011FED0 File Offset: 0x0011E0D0
	private void UpdatePreparingTimeoutTimer()
	{
		if (this.m_PrepareTimeValue != null)
		{
			this.m_PrepareTimeValue.fillAmount = 1f - Mathf.Clamp01(this.timeout / this.logic.def.prepare_duration);
		}
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x0011FF0D File Offset: 0x0011E10D
	private void UpdateVoterTimeoutTimer()
	{
		if (this.m_VoterTimeValue != null)
		{
			this.m_VoterTimeValue.fillAmount = 1f - Mathf.Clamp01(this.timeout / this.maxTimeOut);
		}
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x0011FF40 File Offset: 0x0011E140
	private void HandleSkipAIVoter(BSGButton b)
	{
		this.HandleNextVoter();
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x0011FF48 File Offset: 0x0011E148
	private void HandleOnVoteFor(BSGButton b)
	{
		this.logic.SetPlayerVote(this.currentVoterIndex, this.candidates[0].id);
	}

	// Token: 0x06001F0B RID: 7947 RVA: 0x0011FF6C File Offset: 0x0011E16C
	private void HandleOnVoteForC2(BSGButton b)
	{
		this.logic.SetPlayerVote(this.currentVoterIndex, this.candidates[1].id);
	}

	// Token: 0x06001F0C RID: 7948 RVA: 0x0011FF90 File Offset: 0x0011E190
	private void HandleOnVoteAgainst(BSGButton b)
	{
		this.logic.SetPlayerVote(this.currentVoterIndex, -1);
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x0011FFA4 File Offset: 0x0011E1A4
	private void HandleOnAcceptBeingEmperor(BSGButton b)
	{
		this.logic.TrySetEmperorOfTheWorld(this.playerKingdom, null);
	}

	// Token: 0x06001F0E RID: 7950 RVA: 0x0011FFB8 File Offset: 0x0011E1B8
	private void HandleOnRejectBeingEmperor(BSGButton b)
	{
		this.logic.RejectBeingEmperor(this.playerKingdom);
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x0011FFCB File Offset: 0x0011E1CB
	private void HandleOnAIEmperorAccept(BSGButton b)
	{
		this.logic.SetEmperorOfTheWorld(this.newEmperor, null);
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x0011FFDF File Offset: 0x0011E1DF
	private void HandleOnAIEmperorReject(BSGButton b)
	{
		this.logic.RejectAIEmperor(this.playerKingdom, this.newEmperor);
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnClose(BSGButton bnt)
	{
		this.Close(false);
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x00120001 File Offset: 0x0011E201
	public override bool OnBackInputAction()
	{
		bool flag = this.phase == UIEmperorOfTheWorldWindow.Phase.Final;
		if (flag)
		{
			this.Close(false);
		}
		return flag;
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x00120016 File Offset: 0x0011E216
	protected override void OnDestroy()
	{
		this.logic.SetVotingActive(false, true);
		this.logic.DelListener(this);
		base.OnDestroy();
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x00120038 File Offset: 0x0011E238
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "ranking_updated")
		{
			this.RefreshRanking();
			return;
		}
		if (message == "vote_updated")
		{
			Vars vars = param as Vars;
			int index = vars.GetVar("voter_idx", null, true);
			int num = vars.GetVar("vote", null, true);
			List<int> value = vars.GetVar("vote_weights", null, true).obj_val as List<int>;
			this.votes[index] = num;
			this.votes_weights[index] = value;
			this.votesPCTexts[index] = this.GetProConTexts(this.voters[index], num);
			this.HandleNextVoter();
			return;
		}
		if (message == "wait_for_emperor_of_the_world_response")
		{
			this.SetWaitEmperorResponse(param as Logic.Kingdom);
			return;
		}
		if (message == "new_ai_emperor_of_the_world")
		{
			this.SetPlayerRejectAIEmperor(param as Logic.Kingdom);
			this.SendAnalytics("ai_emperor");
			return;
		}
		if (message == "new_emperor_of_the_world")
		{
			this.SetPhase(UIEmperorOfTheWorldWindow.Phase.Final);
			this.SendAnalytics("new_emperor");
			UIText.SetText(this.m_FinalText, global::Defs.Localize("EmperorOfTheWorldWindow.Final_new_emperor", this.newEmperor, null, true, true));
			return;
		}
		if (!(message == "no_new_emperor_of_the_world"))
		{
			return;
		}
		this.SetPhase(UIEmperorOfTheWorldWindow.Phase.Final);
		this.SendAnalytics("no_new_emperor");
		UIText.SetText(this.m_FinalText, global::Defs.Localize("EmperorOfTheWorldWindow.Final_no_emperor", null, null, true, true));
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x001201AC File Offset: 0x0011E3AC
	public void SendAnalytics(string outcome)
	{
		if (this.candidates == null || this.candidates.Count == 0)
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<string>("candidate1", this.candidates[0].Name);
		vars.Set<int>("candidate1votes", this.logic.CountVotesFor(this.candidates[0]));
		if (this.candidates.Count > 1)
		{
			vars.Set<string>("candidate2", this.candidates[1].Name);
			vars.Set<int>("candidate2votes", this.logic.CountVotesFor(this.candidates[1]));
		}
		else
		{
			vars.Set<string>("candidate2", "none");
			vars.Set<int>("candidate2votes", 0);
		}
		vars.Set<string>("outcome", outcome);
		Game game = this.logic.game;
		if (game == null)
		{
			return;
		}
		Logic.Kingdom localPlayerKingdom = game.GetLocalPlayerKingdom();
		if (localPlayerKingdom == null)
		{
			return;
		}
		localPlayerKingdom.NotifyListeners("analytics_eotw", vars);
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x001202AE File Offset: 0x0011E4AE
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIEmperorOfTheWorldWindow.def_id, null);
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x001202BC File Offset: 0x0011E4BC
	public static void ToggleOpen(Vars vars)
	{
		if (vars == null)
		{
			if (UIEmperorOfTheWorldWindow.current != null)
			{
				UIEmperorOfTheWorldWindow.current.Close(false);
				UIEmperorOfTheWorldWindow.current = null;
			}
			return;
		}
		if (UIEmperorOfTheWorldWindow.current != null)
		{
			UIEmperorOfTheWorldWindow.current.StartVote(vars);
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		GameObject prefab = UIEmperorOfTheWorldWindow.GetPrefab();
		if (prefab == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
		if (gameObject != null)
		{
			UICommon.DeleteChildren(gameObject.transform, typeof(UIEmperorOfTheWorldWindow));
			UIEmperorOfTheWorldWindow.current = UIEmperorOfTheWorldWindow.Create(vars, prefab, gameObject.transform as RectTransform);
		}
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x0012036D File Offset: 0x0011E56D
	public static UIEmperorOfTheWorldWindow Create(Vars vars, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (vars == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIEmperorOfTheWorldWindow orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIEmperorOfTheWorldWindow>();
		orAddComponent.Open();
		orAddComponent.StartVote(vars);
		return orAddComponent;
	}

	// Token: 0x04001436 RID: 5174
	private static string def_id = "EmperorOfTheWorldWindow";

	// Token: 0x04001437 RID: 5175
	[UIFieldTarget("id_tmp_timeout")]
	private TextMeshProUGUI id_tmp_timeout;

	// Token: 0x04001438 RID: 5176
	[UIFieldTarget("id_tmp_ai_timeout")]
	private TextMeshProUGUI id_tmp_ai_timeout;

	// Token: 0x04001439 RID: 5177
	[UIFieldTarget("id_tmp_phase")]
	private TextMeshProUGUI id_tmp_phase;

	// Token: 0x0400143A RID: 5178
	[UIFieldTarget("id_GreathPowersContianer")]
	private RectTransform m_GreathPowersContianer;

	// Token: 0x0400143B RID: 5179
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x0400143C RID: 5180
	[UIFieldTarget("id_VotesForText")]
	private TextMeshProUGUI m_VotesForText;

	// Token: 0x0400143D RID: 5181
	[UIFieldTarget("id_VotesAgainstText")]
	private TextMeshProUGUI m_VotesAgainstText;

	// Token: 0x0400143E RID: 5182
	[UIFieldTarget("id_Group_Prepare")]
	private GameObject m_Group_Prepare;

	// Token: 0x0400143F RID: 5183
	[UIFieldTarget("id_PrepareDescription")]
	private TextMeshProUGUI m_PrepareDescription;

	// Token: 0x04001440 RID: 5184
	[UIFieldTarget("id_PrepareTimeValue")]
	private Image m_PrepareTimeValue;

	// Token: 0x04001441 RID: 5185
	[UIFieldTarget("id_Group_Voter")]
	private GameObject m_Group_Voter;

	// Token: 0x04001442 RID: 5186
	[UIFieldTarget("id_VoterInfo")]
	private GameObject m_VoterInfo;

	// Token: 0x04001443 RID: 5187
	[UIFieldTarget("id_Button_SkipAIVoter")]
	private BSGButton m_Button_SkipAIVoter;

	// Token: 0x04001444 RID: 5188
	[UIFieldTarget("id_Group_VoterButtonsSinglecandidate")]
	private GameObject m_Group_VoterButtonsSinglecandidate;

	// Token: 0x04001445 RID: 5189
	[UIFieldTarget("id_Button_For")]
	private BSGButton m_Button_For;

	// Token: 0x04001446 RID: 5190
	[UIFieldTarget("id_SupportClaimLabel")]
	private TextMeshProUGUI m_SupportClaimLabel;

	// Token: 0x04001447 RID: 5191
	[UIFieldTarget("id_Button_Against")]
	private BSGButton m_Button_Against;

	// Token: 0x04001448 RID: 5192
	[UIFieldTarget("id_RejectClaimLabel")]
	private TextMeshProUGUI m_RejectClaimLabel;

	// Token: 0x04001449 RID: 5193
	[UIFieldTarget("id_Group_VoterButtonsMultipleCandidates")]
	private GameObject m_Group_VoterButtonsMultipleCandidates;

	// Token: 0x0400144A RID: 5194
	[UIFieldTarget("id_Button_ForC1")]
	private BSGButton m_Button_ForC1;

	// Token: 0x0400144B RID: 5195
	[UIFieldTarget("id_SupportC1Label")]
	private TextMeshProUGUI m_SupportC1Label;

	// Token: 0x0400144C RID: 5196
	[UIFieldTarget("id_Button_ForC2")]
	private BSGButton m_Button_ForC2;

	// Token: 0x0400144D RID: 5197
	[UIFieldTarget("id_SupportC2Label")]
	private TextMeshProUGUI m_SupportC2Label;

	// Token: 0x0400144E RID: 5198
	[UIFieldTarget("id_Button_Abstain")]
	private BSGButton m_Button_Abstain;

	// Token: 0x0400144F RID: 5199
	[UIFieldTarget("id_CrestC1")]
	private UIKingdomIcon m_CrestC1;

	// Token: 0x04001450 RID: 5200
	[UIFieldTarget("id_CrestC2")]
	private UIKingdomIcon m_CrestC2;

	// Token: 0x04001451 RID: 5201
	[UIFieldTarget("id_VoterTimeValue")]
	private Image m_VoterTimeValue;

	// Token: 0x04001452 RID: 5202
	[UIFieldTarget("id_Group_VotesSingleCandidate")]
	private GameObject m_Group_VotesSingleCandidate;

	// Token: 0x04001453 RID: 5203
	[UIFieldTarget("id_Candidate")]
	private GameObject m_CandidateSingleVote;

	// Token: 0x04001454 RID: 5204
	[UIFieldTarget("id_VotesForContainer")]
	private GameObject m_VotesForContainer;

	// Token: 0x04001455 RID: 5205
	[UIFieldTarget("id_VotesAgainstContainer")]
	private GameObject m_VotesAgainstContainer;

	// Token: 0x04001456 RID: 5206
	[UIFieldTarget("id_Group_VotesMultipleCandidates")]
	private GameObject m_Group_VotesMultipleCandidates;

	// Token: 0x04001457 RID: 5207
	[UIFieldTarget("id_Candidate1")]
	private GameObject m_Candidate1;

	// Token: 0x04001458 RID: 5208
	[UIFieldTarget("id_Candidate2")]
	private GameObject m_Candidate2;

	// Token: 0x04001459 RID: 5209
	[UIFieldTarget("id_VotesC1Text")]
	private TextMeshProUGUI m_VotesC1Text;

	// Token: 0x0400145A RID: 5210
	[UIFieldTarget("id_VotesC2Text")]
	private TextMeshProUGUI m_VotesC2Text;

	// Token: 0x0400145B RID: 5211
	[UIFieldTarget("id_VotesForC1Container")]
	private GameObject m_VotesForC1Container;

	// Token: 0x0400145C RID: 5212
	[UIFieldTarget("id_VotesForC2Container")]
	private GameObject m_VotesForC2Container;

	// Token: 0x0400145D RID: 5213
	[UIFieldTarget("id_Group_WaitEmperorResponse")]
	private GameObject m_Group_WaitEmperorResponse;

	// Token: 0x0400145E RID: 5214
	[UIFieldTarget("id_EmperorResponsePlayer")]
	private GameObject m_EmperorResponsePlayer;

	// Token: 0x0400145F RID: 5215
	[UIFieldTarget("id_EmperorResponseOther")]
	private GameObject m_EmperorResponseOther;

	// Token: 0x04001460 RID: 5216
	[UIFieldTarget("id_Button_AcceptBeingEmperor")]
	private BSGButton m_Button_AcceptBeingEmperor;

	// Token: 0x04001461 RID: 5217
	[UIFieldTarget("id_Button_AcceptBeingEmperorLabel")]
	private TextMeshProUGUI m_Button_AcceptBeingEmperorLabel;

	// Token: 0x04001462 RID: 5218
	[UIFieldTarget("id_Button_RejectBeingEmperor")]
	private BSGButton m_Button_RejectBeingEmperor;

	// Token: 0x04001463 RID: 5219
	[UIFieldTarget("id_Button_RejectBeingEmperorLabel")]
	private TextMeshProUGUI m_Button_RejectBeingEmperorLabel;

	// Token: 0x04001464 RID: 5220
	[UIFieldTarget("id_Group_RejectAIEmperor")]
	private GameObject m_Group_RejectAIEmperor;

	// Token: 0x04001465 RID: 5221
	[UIFieldTarget("id_RejectAIEmperorText")]
	private TextMeshProUGUI m_RejectAIEmperorText;

	// Token: 0x04001466 RID: 5222
	[UIFieldTarget("id_Button_AcceptAIEmperor")]
	private BSGButton m_Button_AcceptAIEmperor;

	// Token: 0x04001467 RID: 5223
	[UIFieldTarget("id_AcceptLabel")]
	private TextMeshProUGUI m_AcceptLabel;

	// Token: 0x04001468 RID: 5224
	[UIFieldTarget("id_Button_RejectAIEmperor")]
	private BSGButton m_Button_RejectAIEmperor;

	// Token: 0x04001469 RID: 5225
	[UIFieldTarget("id_RejectLabel")]
	private TextMeshProUGUI m_RejectLabel;

	// Token: 0x0400146A RID: 5226
	[UIFieldTarget("id_Group_Final")]
	private GameObject m_Group_Final;

	// Token: 0x0400146B RID: 5227
	[UIFieldTarget("id_FinalText")]
	private TextMeshProUGUI m_FinalText;

	// Token: 0x0400146C RID: 5228
	[UIFieldTarget("id_Button_Final_Close")]
	private BSGButton m_Button_Final_Close;

	// Token: 0x0400146D RID: 5229
	[UIFieldTarget("id_ButtonCloseLabel")]
	private TextMeshProUGUI m_ButtonCloseLabel;

	// Token: 0x0400146E RID: 5230
	[UIFieldTarget("id_WaitEmperorResponseTextOther")]
	private TextMeshProUGUI m_WaitEmperorResponseTextOther;

	// Token: 0x0400146F RID: 5231
	[UIFieldTarget("id_WaitEmperorResponseTextPlayer")]
	private TextMeshProUGUI m_WaitEmperorResponseTextPlayer;

	// Token: 0x04001470 RID: 5232
	[UIFieldTarget("id_KingdomSlotPrototype")]
	private GameObject m_KingdomSlotPrototype;

	// Token: 0x04001471 RID: 5233
	[UIFieldTarget("id_VoteRowPrototype")]
	private GameObject m_voteRowPrototype;

	// Token: 0x04001472 RID: 5234
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_ButtonClose;

	// Token: 0x04001473 RID: 5235
	[SerializeField]
	private float m_FirstTierScale = 0.8f;

	// Token: 0x04001474 RID: 5236
	[SerializeField]
	private float m_SecoundTierScale = 0.72f;

	// Token: 0x04001475 RID: 5237
	private UIEmperorOfTheWorldWindow.Phase phase;

	// Token: 0x04001476 RID: 5238
	private List<UIEmperorOfTheWorldWindowRow> voteRowsFor = new List<UIEmperorOfTheWorldWindowRow>();

	// Token: 0x04001477 RID: 5239
	private List<UIEmperorOfTheWorldWindowRow> voteRowsAgainst = new List<UIEmperorOfTheWorldWindowRow>();

	// Token: 0x04001478 RID: 5240
	private List<UIEmperorOfTheWorldWindowRow> voteRowsForC2 = new List<UIEmperorOfTheWorldWindowRow>();

	// Token: 0x04001479 RID: 5241
	private List<UIEmperorOfTheWorldWindowRow> voteRowsForC1 = new List<UIEmperorOfTheWorldWindowRow>();

	// Token: 0x0400147A RID: 5242
	public EmperorOfTheWorld logic;

	// Token: 0x0400147B RID: 5243
	public Logic.Kingdom playerKingdom;

	// Token: 0x0400147D RID: 5245
	public Logic.Kingdom newEmperor;

	// Token: 0x0400147E RID: 5246
	private List<UIEmperorOfTheWorldWindow.KingdomSlot> m_TopRankingKingdoms = new List<UIEmperorOfTheWorldWindow.KingdomSlot>();

	// Token: 0x0400147F RID: 5247
	public List<Logic.Kingdom> candidates;

	// Token: 0x04001480 RID: 5248
	public List<Logic.Kingdom> voters;

	// Token: 0x04001481 RID: 5249
	public List<int> votes;

	// Token: 0x04001482 RID: 5250
	public List<List<int>> votes_weights;

	// Token: 0x04001483 RID: 5251
	private List<List<string>> votesPCTexts = new List<List<string>>();

	// Token: 0x04001484 RID: 5252
	private List<string> abstainTexts = new List<string>();

	// Token: 0x04001485 RID: 5253
	private float timeout;

	// Token: 0x04001486 RID: 5254
	private float aiVoteDelay;

	// Token: 0x04001487 RID: 5255
	private float total_voters_fame;

	// Token: 0x04001488 RID: 5256
	private int currentVoterIndex = -1;

	// Token: 0x04001489 RID: 5257
	private float candidate1Fame;

	// Token: 0x0400148A RID: 5258
	private float candidate2Fame;

	// Token: 0x0400148B RID: 5259
	private bool m_Initialzied;

	// Token: 0x0400148D RID: 5261
	private float maxTimeOut = 30f;

	// Token: 0x0400148E RID: 5262
	public static UIEmperorOfTheWorldWindow current;

	// Token: 0x02000739 RID: 1849
	private enum Phase
	{
		// Token: 0x04003906 RID: 14598
		Preparing,
		// Token: 0x04003907 RID: 14599
		Voting,
		// Token: 0x04003908 RID: 14600
		WaitingEmperorResponse,
		// Token: 0x04003909 RID: 14601
		RejectAIEmperor,
		// Token: 0x0400390A RID: 14602
		Final
	}

	// Token: 0x0200073A RID: 1850
	internal class KingdomSlot : MonoBehaviour
	{
		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06004A2D RID: 18989 RVA: 0x0021F80D File Offset: 0x0021DA0D
		// (set) Token: 0x06004A2E RID: 18990 RVA: 0x0021F815 File Offset: 0x0021DA15
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x06004A2F RID: 18991 RVA: 0x0021F81E File Offset: 0x0021DA1E
		// (set) Token: 0x06004A30 RID: 18992 RVA: 0x0021F826 File Offset: 0x0021DA26
		public UIEmperorOfTheWorldWindow Eow { get; private set; }

		// Token: 0x06004A31 RID: 18993 RVA: 0x0021F830 File Offset: 0x0021DA30
		private void Init()
		{
			if (this.m_Initalized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_Icon != null)
			{
				KingdomShield primary = this.m_Icon.GetPrimary();
				if (primary != null)
				{
					KingdomShield kingdomShield = primary;
					kingdomShield.onClick = (KingdomShield.OnShieldClick)Delegate.Combine(kingdomShield.onClick, new KingdomShield.OnShieldClick(this.HandleShieldClick));
				}
			}
			if (this.m_Selected != null)
			{
				this.m_Selected.gameObject.SetActive(false);
			}
			if (this.m_Icon != null)
			{
				this.m_Icon.gameObject.SetActive(false);
			}
			this.m_Initalized = true;
		}

		// Token: 0x06004A32 RID: 18994 RVA: 0x0021F8D7 File Offset: 0x0021DAD7
		private bool HandleShieldClick(PointerEventData e, KingdomShield s)
		{
			Action<UIEmperorOfTheWorldWindow.KingdomSlot> onSelect = this.OnSelect;
			if (onSelect != null)
			{
				onSelect(this);
			}
			return true;
		}

		// Token: 0x06004A33 RID: 18995 RVA: 0x0021F8EC File Offset: 0x0021DAEC
		public void SetData(UIEmperorOfTheWorldWindow eow, Logic.Kingdom k)
		{
			this.Init();
			this.Kingdom = k;
			this.Eow = eow;
			this.Refresh();
		}

		// Token: 0x06004A34 RID: 18996 RVA: 0x0021F908 File Offset: 0x0021DB08
		private void Refresh()
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			this.m_Empty.gameObject.SetActive(this.Kingdom == null);
			if (this.m_VoteStateContainer != null)
			{
				this.m_VoteStateContainer.gameObject.SetActive(this.Kingdom != null);
			}
			if (this.m_Icon != null)
			{
				this.m_Icon.SetObject(this.Kingdom, null);
				this.m_Icon.gameObject.SetActive(this.Kingdom != null);
			}
			if (this.m_Fame != null && this.Kingdom != null)
			{
				this.m_Fame.text = ((int)this.Kingdom.fame).ToString();
			}
			if (this.Kingdom != null)
			{
				if (this.m_FameBackground != null)
				{
					if (this.Kingdom != kingdom && this.Eow.IsCandidate(kingdom) && !this.Eow.IsCandidate(this.Kingdom))
					{
						this.m_FameBackground.gameObject.SetActive(true);
						if (this.m_FameBackground != null)
						{
							EmperorOfTheWorld.Slant slant = this.Eow.logic.CalcSlant(this.Kingdom, kingdom, this.Eow.votes[this.Eow.GetVoterIndex(this.Kingdom)]);
							this.m_FameBackground.overrideSprite = global::Defs.GetObj<Sprite>(this.Eow.logic.def.field, "SlantBackgrounds.GreatPowers." + slant.ToString(), null);
						}
					}
					else
					{
						this.m_FameBackground.gameObject.SetActive(false);
					}
				}
				if (this.m_IconPrimary != null && this.Kingdom != kingdom && this.Eow.IsCandidate(kingdom) && !this.Eow.IsCandidate(this.Kingdom))
				{
					Vars vars = new Vars();
					vars.Set<Logic.Kingdom>("voter", this.Kingdom);
					vars.Set<Logic.Kingdom>("candidate", kingdom);
					vars.Set<float>("vote_weight", this.Eow.logic.CalcVoteWeightBase(this.Kingdom, kingdom));
					vars.Set<string>("slant_text", UIEmperorOfTheWorldWindow.GetSlantText(this.Kingdom, kingdom, "EmperorGreatPowerVoteTooltip.SlantTexts"));
					vars.Set<string>("pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.Kingdom, kingdom, "reason_to_them", true));
					vars.Set<string>("con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.Kingdom, kingdom, "reason_to_them", false));
					Tooltip.Get(this.m_IconPrimary, true).SetDef("EmperorGreatPowerVoteTooltip", vars);
				}
			}
			this.UpdateHighlight();
		}

		// Token: 0x06004A35 RID: 18997 RVA: 0x0021FBC0 File Offset: 0x0021DDC0
		public void SetCrestScale(float scale)
		{
			this.Init();
			if (this.m_KingdomIconContainer != null)
			{
				float num = this.m_KingdomIconContainer.rect.width;
				float num2 = this.m_KingdomIconContainer.rect.height;
				num = (float)Mathf.RoundToInt(num * scale);
				num2 = (float)Mathf.RoundToInt(num2 * scale);
				this.m_KingdomIconContainer.sizeDelta = new Vector2(num, num2);
			}
			if (this.m_CurrentVoterGlow != null)
			{
				float num3 = this.m_CurrentVoterGlow.rect.width;
				float num4 = this.m_CurrentVoterGlow.rect.height;
				num3 = (float)Mathf.RoundToInt(num3 * scale);
				num4 = (float)Mathf.RoundToInt(num4 * scale);
				this.m_CurrentVoterGlow.sizeDelta = new Vector2(num3, num4);
			}
		}

		// Token: 0x06004A36 RID: 18998 RVA: 0x0021FC90 File Offset: 0x0021DE90
		public void UpdateState(bool singleCandidate, bool pendingVote, int vote, bool currentVoter, bool showVote, Logic.Kingdom votedFor, bool isCandidate, List<Logic.Kingdom> candidates)
		{
			if (this.m_VoteStateContainer != null)
			{
				this.m_VoteStateContainer.gameObject.SetActive(this.Kingdom != null);
			}
			if (this.Kingdom == null)
			{
				return;
			}
			if (this.m_CurrentVoter != null)
			{
				this.m_CurrentVoter.gameObject.SetActive(currentVoter);
			}
			if (this.m_CurrentVoterGlow != null)
			{
				this.m_CurrentVoterGlow.gameObject.SetActive(currentVoter);
			}
			if (this.m_VotedForKingdom != null)
			{
				this.m_VotedForKingdom.gameObject.SetActive(votedFor != null && !singleCandidate);
				this.m_VotedForKingdom.SetObject(votedFor, null);
			}
			if (this.m_Candidate != null)
			{
				this.m_Candidate.gameObject.SetActive(isCandidate);
			}
			if (isCandidate)
			{
				this.m_PendingtVote.gameObject.SetActive(false);
				this.m_SupportlVote.gameObject.SetActive(false);
				this.m_RejectVote.gameObject.SetActive(false);
				this.m_NeutralVote.gameObject.SetActive(false);
			}
			else if (singleCandidate)
			{
				this.m_PendingtVote.gameObject.SetActive(pendingVote);
				this.m_SupportlVote.gameObject.SetActive((!pendingVote || showVote) && vote != -1);
				this.m_RejectVote.gameObject.SetActive((!pendingVote || showVote) && vote == -1);
				this.m_NeutralVote.gameObject.SetActive(false);
			}
			else
			{
				this.m_PendingtVote.gameObject.SetActive(false);
				this.m_SupportlVote.gameObject.SetActive(false);
				this.m_RejectVote.gameObject.SetActive(false);
				this.m_NeutralVote.gameObject.SetActive(((!pendingVote && !currentVoter) || showVote) && vote == -1);
			}
			if (this.m_VoteStateContainer != null)
			{
				Vars vars = new Vars(this.Kingdom);
				for (int i = 0; i < candidates.Count; i++)
				{
					vars.Set<Logic.Kingdom>("claimant" + (i + 1), candidates[i]);
				}
				string str = "";
				if (pendingVote)
				{
					str = "vote_pending";
				}
				else if (isCandidate)
				{
					str = "pretender";
				}
				else if (!pendingVote && (!currentVoter || showVote) && !singleCandidate && vote == -1)
				{
					str = "abstained";
				}
				else if (!pendingVote && (!currentVoter || showVote) && singleCandidate && vote == -1)
				{
					str = "voted_against";
				}
				else if (!pendingVote && (!currentVoter || showVote) && singleCandidate && vote != -1)
				{
					str = "voted_in_favor";
				}
				else if (!pendingVote && (!currentVoter || showVote) && !singleCandidate && vote == candidates[0].id)
				{
					str = "voted_for_claimant1";
				}
				else if (!pendingVote && (!currentVoter || showVote) && !singleCandidate && vote == candidates[1].id)
				{
					str = "voted_for_claimant2";
				}
				else if (currentVoter)
				{
					str = "vote_pending";
				}
				Tooltip.Get(this.m_VoteStateContainer, true).SetText("#" + global::Defs.Localize("EmperorOfTheWorldWindow." + str, vars, null, true, true), null, null);
			}
		}

		// Token: 0x06004A37 RID: 18999 RVA: 0x0021FFC0 File Offset: 0x0021E1C0
		public void SetFocused(bool focused)
		{
			this.m_Focused = focused;
			this.UpdateHighlight();
		}

		// Token: 0x06004A38 RID: 19000 RVA: 0x0021FFCF File Offset: 0x0021E1CF
		private void UpdateHighlight()
		{
			if (this.m_Selected != null)
			{
				this.m_Selected.gameObject.SetActive(this.m_Focused);
			}
		}

		// Token: 0x0400390B RID: 14603
		[UIFieldTarget("id_IconContainer")]
		private RectTransform m_KingdomIconContainer;

		// Token: 0x0400390C RID: 14604
		[UIFieldTarget("id_KingdomCrest")]
		private UIKingdomIcon m_Icon;

		// Token: 0x0400390D RID: 14605
		[UIFieldTarget("id_Primary")]
		private GameObject m_IconPrimary;

		// Token: 0x0400390E RID: 14606
		[UIFieldTarget("id_Empty")]
		private GameObject m_Empty;

		// Token: 0x0400390F RID: 14607
		[UIFieldTarget("id_Selected")]
		private GameObject m_Selected;

		// Token: 0x04003910 RID: 14608
		[UIFieldTarget("id_VoteStateContainer")]
		private GameObject m_VoteStateContainer;

		// Token: 0x04003911 RID: 14609
		[UIFieldTarget("id_Fame")]
		private TextMeshProUGUI m_Fame;

		// Token: 0x04003912 RID: 14610
		[UIFieldTarget("id_FameBackground")]
		private Image m_FameBackground;

		// Token: 0x04003913 RID: 14611
		[UIFieldTarget("id_Candidate")]
		private GameObject m_Candidate;

		// Token: 0x04003914 RID: 14612
		[UIFieldTarget("id_CurrentVoter")]
		private GameObject m_CurrentVoter;

		// Token: 0x04003915 RID: 14613
		[UIFieldTarget("id_CurrentVoterGlow")]
		private RectTransform m_CurrentVoterGlow;

		// Token: 0x04003916 RID: 14614
		[UIFieldTarget("id_VotedForKingdom")]
		private UIKingdomIcon m_VotedForKingdom;

		// Token: 0x04003917 RID: 14615
		[UIFieldTarget("id_PendingtVote")]
		private GameObject m_PendingtVote;

		// Token: 0x04003918 RID: 14616
		[UIFieldTarget("id_NeutralVote")]
		private GameObject m_NeutralVote;

		// Token: 0x04003919 RID: 14617
		[UIFieldTarget("id_SupportlVote")]
		private GameObject m_SupportlVote;

		// Token: 0x0400391A RID: 14618
		[UIFieldTarget("id_RejectVote")]
		private GameObject m_RejectVote;

		// Token: 0x0400391D RID: 14621
		public Action<UIEmperorOfTheWorldWindow.KingdomSlot> OnSelect;

		// Token: 0x0400391E RID: 14622
		private bool m_Initalized;

		// Token: 0x0400391F RID: 14623
		private bool m_Focused;
	}

	// Token: 0x0200073B RID: 1851
	internal class TroneCandidate : MonoBehaviour
	{
		// Token: 0x06004A3A RID: 19002 RVA: 0x0021FFF5 File Offset: 0x0021E1F5
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialized = true;
		}

		// Token: 0x06004A3B RID: 19003 RVA: 0x0022000E File Offset: 0x0021E20E
		public void SetObject(Logic.Kingdom k)
		{
			this.Init();
			this.Candidate = k;
			this.Refresh();
		}

		// Token: 0x06004A3C RID: 19004 RVA: 0x00220024 File Offset: 0x0021E224
		private void Refresh()
		{
			Logic.Character king = this.Candidate.GetKing();
			if (this.m_CandidateIcon != null)
			{
				this.m_CandidateIcon.SetObject(king, null);
			}
			if (this.m_CandidateName != null)
			{
				UIText.SetTextKey(this.m_CandidateName, "Character.name", new Vars(king), null);
			}
			if (this.m_CandidateAge != null)
			{
				UIText.SetTextKey(this.m_CandidateAge, "Character.age." + king.age.ToString(), null, null);
			}
		}

		// Token: 0x06004A3D RID: 19005 RVA: 0x002200B8 File Offset: 0x0021E2B8
		public void UpdateFame(float cur, float target)
		{
			if (this.m_CandidateFame != null)
			{
				this.fameVars.Set<int>("current_fame", (int)cur);
				this.fameVars.Set<int>("goal_fame", (int)target);
				this.fameVars.Set<int>("goal_fame_perc", (int)(Mathf.Clamp01(cur / target) * 100f));
				UIText.SetTextKey(this.m_CandidateFame, "EmperorOfTheWorldWindow.candidate_fame", this.fameVars, null);
				UIText.SetTextKey(this.m_CandidateFameText, "EmperorOfTheWorldWindow.candidate_fame_text", this.fameVars, null);
			}
			if (this.m_ProgressForegorund != null)
			{
				this.m_ProgressForegorund.fillAmount = cur / target;
			}
		}

		// Token: 0x04003920 RID: 14624
		[UIFieldTarget("id_CandidateIcon")]
		private UICharacterIcon m_CandidateIcon;

		// Token: 0x04003921 RID: 14625
		[UIFieldTarget("id_CandidateName")]
		private TextMeshProUGUI m_CandidateName;

		// Token: 0x04003922 RID: 14626
		[UIFieldTarget("id_CandidateAge")]
		private TextMeshProUGUI m_CandidateAge;

		// Token: 0x04003923 RID: 14627
		[UIFieldTarget("id_CandidateFame")]
		private TextMeshProUGUI m_CandidateFame;

		// Token: 0x04003924 RID: 14628
		[UIFieldTarget("id_CandidateFameText")]
		private TextMeshProUGUI m_CandidateFameText;

		// Token: 0x04003925 RID: 14629
		[UIFieldTarget("id_ProgressForegorund")]
		protected Image m_ProgressForegorund;

		// Token: 0x04003926 RID: 14630
		public Logic.Kingdom Candidate;

		// Token: 0x04003927 RID: 14631
		private bool m_Initialized;

		// Token: 0x04003928 RID: 14632
		private Vars fameVars = new Vars();
	}

	// Token: 0x0200073C RID: 1852
	internal class VoterInfo : MonoBehaviour
	{
		// Token: 0x06004A3F RID: 19007 RVA: 0x00220173 File Offset: 0x0021E373
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialized = true;
		}

		// Token: 0x06004A40 RID: 19008 RVA: 0x0022018C File Offset: 0x0021E38C
		public void SetObject(UIEmperorOfTheWorldWindow parent, Logic.Kingdom voter)
		{
			this.Init();
			this.Voter = voter;
			this.m_Parent = parent;
			this.Refresh();
		}

		// Token: 0x06004A41 RID: 19009 RVA: 0x002201A8 File Offset: 0x0021E3A8
		private void Refresh()
		{
			Logic.Character king = this.Voter.GetKing();
			if (this.m_VoterIcon != null)
			{
				this.m_VoterIcon.SetObject(king, null);
			}
			if (this.m_VoterName != null)
			{
				UIText.SetTextKey(this.m_VoterName, "Character.name", new Vars(king), null);
			}
			if (this.m_VoterAge != null)
			{
				UIText.SetTextKey(this.m_VoterAge, "Character.age." + king.age.ToString(), null, null);
			}
			if (this.m_VoterDecision != null)
			{
				if (this.m_Parent.showCurrentAIVote && !this.Voter.is_player)
				{
					this.m_VoterDecision.SetActive(true);
					Logic.Kingdom voterSupport = this.m_Parent.GetVoterSupport(this.m_Parent.currentVoterIndex);
					int candidateIdx = this.m_Parent.GetCandidateIdx(voterSupport);
					int num = -1;
					if (voterSupport != null)
					{
						num = this.m_Parent.votes_weights[this.m_Parent.currentVoterIndex][candidateIdx];
					}
					else if (this.m_Parent.UsingSingleCandidate())
					{
						num = this.m_Parent.votes_weights[this.m_Parent.currentVoterIndex][0];
					}
					bool flag = num < 0 && !this.m_Parent.UsingSingleCandidate();
					if (this.m_VoterDecisionAbstain != null)
					{
						this.m_VoterDecisionAbstain.SetActive(flag);
					}
					if (this.m_VoterDecisionNormal != null)
					{
						this.m_VoterDecisionNormal.SetActive(!flag);
					}
					if (!flag)
					{
						if (this.m_VoterDecisionBackground != null)
						{
							EmperorOfTheWorld.Slant slant = EmperorOfTheWorld.Slant.Against;
							if (voterSupport != null || this.m_Parent.UsingSingleCandidate())
							{
								slant = this.m_Parent.logic.CalcSlant(this.Voter, voterSupport ?? this.m_Parent.candidates[0], this.m_Parent.votes[this.m_Parent.currentVoterIndex]);
							}
							this.m_VoterDecisionBackground.overrideSprite = global::Defs.GetObj<Sprite>(this.m_Parent.logic.def.field, "SlantBackgrounds.Voters." + slant.ToString(), null);
						}
						if (this.m_VoterVoteWeight != null)
						{
							this.m_VoterVoteWeight.gameObject.SetActive(true);
							UIText.SetText(this.m_VoterVoteWeight, num.ToString());
						}
						if (this.m_VoterDecisionFor != null)
						{
							this.m_VoterDecisionFor.SetActive(num >= 0);
						}
						if (this.m_VoterDecisionAgainst != null)
						{
							this.m_VoterDecisionAgainst.SetActive(num < 0);
						}
					}
				}
				else
				{
					this.m_VoterDecision.SetActive(false);
				}
			}
			if (this.m_VoterDescription != null)
			{
				if (this.m_Parent.showCurrentAIVote && !this.Voter.is_player)
				{
					UIText.SetText(this.m_VoterDescription, global::Defs.Localize("#" + this.m_Parent.GetVoteText(this.m_Parent.currentVoterIndex, this.m_Parent.GetVoterSupport(this.m_Parent.GetCurrentVoterIndex())), null, null, true, true));
					return;
				}
				Vars vars = new Vars(this.Voter);
				UIEmperorOfTheWorldWindow parent = this.m_Parent;
				if (((parent != null) ? parent.candidates : null) != null)
				{
					Vars vars2 = vars;
					string key = "single_claimant";
					UIEmperorOfTheWorldWindow parent2 = this.m_Parent;
					vars2.Set<bool>(key, parent2 != null && parent2.candidates.Count == 0);
					int num2 = 0;
					for (;;)
					{
						int num3 = num2;
						UIEmperorOfTheWorldWindow parent3 = this.m_Parent;
						int? num4 = (parent3 != null) ? new int?(parent3.candidates.Count) : null;
						if (!(num3 < num4.GetValueOrDefault() & num4 != null))
						{
							break;
						}
						Vars vars3 = vars;
						string key2 = "claimant" + (num2 + 1);
						UIEmperorOfTheWorldWindow parent4 = this.m_Parent;
						vars3.Set<Logic.Kingdom>(key2, (parent4 != null) ? parent4.candidates[num2] : null);
						num2++;
					}
				}
				if (BaseUI.LogicKingdom() == this.Voter)
				{
					UIText.SetTextKey(this.m_VoterDescription, "EmperorOfTheWorldWindow.player_voting", vars, null);
					return;
				}
				UIText.SetTextKey(this.m_VoterDescription, "EmperorOfTheWorldWindow.kingdom_voting", vars, null);
			}
		}

		// Token: 0x04003929 RID: 14633
		[UIFieldTarget("id_VoterIcon")]
		private UICharacterIcon m_VoterIcon;

		// Token: 0x0400392A RID: 14634
		[UIFieldTarget("id_VoterName")]
		private TextMeshProUGUI m_VoterName;

		// Token: 0x0400392B RID: 14635
		[UIFieldTarget("id_VoterAge")]
		private TextMeshProUGUI m_VoterAge;

		// Token: 0x0400392C RID: 14636
		[UIFieldTarget("id_VoterDecision")]
		private GameObject m_VoterDecision;

		// Token: 0x0400392D RID: 14637
		[UIFieldTarget("id_VoterDecisionNormal")]
		private GameObject m_VoterDecisionNormal;

		// Token: 0x0400392E RID: 14638
		[UIFieldTarget("id_VoterDecisionBackground")]
		private Image m_VoterDecisionBackground;

		// Token: 0x0400392F RID: 14639
		[UIFieldTarget("id_VoterDecisionFor")]
		private GameObject m_VoterDecisionFor;

		// Token: 0x04003930 RID: 14640
		[UIFieldTarget("id_VoterDecisionAgainst")]
		private GameObject m_VoterDecisionAgainst;

		// Token: 0x04003931 RID: 14641
		[UIFieldTarget("id_VoterDecisionAbstain")]
		private GameObject m_VoterDecisionAbstain;

		// Token: 0x04003932 RID: 14642
		[UIFieldTarget("id_VoterVoteWeight")]
		private TextMeshProUGUI m_VoterVoteWeight;

		// Token: 0x04003933 RID: 14643
		[UIFieldTarget("id_VoterDescription")]
		private TextMeshProUGUI m_VoterDescription;

		// Token: 0x04003934 RID: 14644
		public Logic.Kingdom Voter;

		// Token: 0x04003935 RID: 14645
		private UIEmperorOfTheWorldWindow m_Parent;

		// Token: 0x04003936 RID: 14646
		private bool m_Initialized;
	}
}
