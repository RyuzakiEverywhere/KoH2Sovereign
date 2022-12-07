using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FMOD.Studio;
using FMODUnity;
using Logic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

// Token: 0x0200012B RID: 299
public class DevCheats : MonoBehaviour
{
	// Token: 0x06000DCE RID: 3534 RVA: 0x0009A6D2 File Offset: 0x000988D2
	private void Start()
	{
		if (DevCheats.instance != null && DevCheats.instance != this)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		DevCheats.instance = this;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x0009A6FB File Offset: 0x000988FB
	private void OnDestroy()
	{
		if (DevCheats.instance == this)
		{
			DevCheats.instance = null;
		}
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x0009A710 File Offset: 0x00098910
	[ConsoleMethod("cl", "Print current cheat level")]
	private void PrintCheatLevel()
	{
		Debug.Log(string.Format("Cheat level: {0}", Game.cheat_level));
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x0009A72C File Offset: 0x0009892C
	[ConsoleMethod("cl", "Set current cheat level")]
	private void SetCheatLevel(string level_str)
	{
		DevCheats.<>c__DisplayClass4_0 CS$<>8__locals1;
		CS$<>8__locals1.game = GameLogic.Get(false);
		if (CS$<>8__locals1.game != null && !CS$<>8__locals1.game.IsAuthority())
		{
			Debug.Log("cl: Only authority can set cheat level!");
			return;
		}
		string text = level_str.ToLowerInvariant();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1037941245U)
		{
			if (num <= 900716406U)
			{
				if (num != 873244444U)
				{
					if (num != 890022063U)
					{
						if (num != 900716406U)
						{
							goto IL_206;
						}
						if (!(text == "medium"))
						{
							goto IL_206;
						}
						goto IL_1E0;
					}
					else if (!(text == "0"))
					{
						goto IL_206;
					}
				}
				else
				{
					if (!(text == "1"))
					{
						goto IL_206;
					}
					goto IL_1CD;
				}
			}
			else if (num != 906799682U)
			{
				if (num != 923577301U)
				{
					if (num != 1037941245U)
					{
						goto IL_206;
					}
					if (!(text == "high"))
					{
						goto IL_206;
					}
					goto IL_1F3;
				}
				else
				{
					if (!(text == "2"))
					{
						goto IL_206;
					}
					goto IL_1E0;
				}
			}
			else
			{
				if (!(text == "3"))
				{
					goto IL_206;
				}
				goto IL_1F3;
			}
		}
		else if (num <= 3893112696U)
		{
			if (num != 1330735745U)
			{
				if (num != 2913447899U)
				{
					if (num != 3893112696U)
					{
						goto IL_206;
					}
					if (!(text == "m"))
					{
						goto IL_206;
					}
					goto IL_1E0;
				}
				else if (!(text == "none"))
				{
					goto IL_206;
				}
			}
			else
			{
				if (!(text == "low"))
				{
					goto IL_206;
				}
				goto IL_1CD;
			}
		}
		else if (num != 3909890315U)
		{
			if (num != 3943445553U)
			{
				if (num != 3977000791U)
				{
					goto IL_206;
				}
				if (!(text == "h"))
				{
					goto IL_206;
				}
				goto IL_1F3;
			}
			else if (!(text == "n"))
			{
				goto IL_206;
			}
		}
		else
		{
			if (!(text == "l"))
			{
				goto IL_206;
			}
			goto IL_1CD;
		}
		DevCheats.<SetCheatLevel>g__SetCL|4_0(Game.CheatLevel.None, ref CS$<>8__locals1);
		Debug.Log("Cheat level has been set to none.");
		return;
		IL_1CD:
		DevCheats.<SetCheatLevel>g__SetCL|4_0(Game.CheatLevel.Low, ref CS$<>8__locals1);
		Debug.Log("Cheat level has been set to low.");
		return;
		IL_1E0:
		DevCheats.<SetCheatLevel>g__SetCL|4_0(Game.CheatLevel.Medium, ref CS$<>8__locals1);
		Debug.Log("Cheat level has been set to medium.");
		return;
		IL_1F3:
		DevCheats.<SetCheatLevel>g__SetCL|4_0(Game.CheatLevel.High, ref CS$<>8__locals1);
		Debug.Log("Cheat level has been set to high.");
		return;
		IL_206:
		Debug.LogWarning("Unrecognized cheat level: " + level_str);
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x0009A94F File Offset: 0x00098B4F
	[ConsoleMethod("do", "Execute a macro from ConsoleMacros def")]
	private void ExecuteMacro(string name)
	{
		this.ExecuteMacro(name, "", "", "");
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x0009A967 File Offset: 0x00098B67
	[ConsoleMethod("do", "Execute a macro from ConsoleMacros def")]
	private void ExecuteMacro(string name, string param1)
	{
		this.ExecuteMacro(name, param1, "", "");
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x0009A97B File Offset: 0x00098B7B
	[ConsoleMethod("do", "Execute a macro from ConsoleMacros def")]
	private void ExecuteMacro(string name, string param1, string param2)
	{
		this.ExecuteMacro(name, param1, param2, "");
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0009A98C File Offset: 0x00098B8C
	[ConsoleMethod("do", "Execute a macro from ConsoleMacros def")]
	private void ExecuteMacro(string name, string param1, string param2, string param3)
	{
		DT.Field defField = global::Defs.GetDefField("ConsoleMacros", name);
		if (defField == null || defField.children == null)
		{
			Debug.LogError("Macro '" + name + "' not found");
			return;
		}
		AttributeConsoleManager attributeConsoleManager = AttributeConsoleManager.instance;
		if (attributeConsoleManager == null)
		{
			return;
		}
		Logic.Object vars = BaseUI.SelLO();
		for (int i = 0; i < defField.children.Count; i++)
		{
			DT.Field field = defField.children[i];
			if (!string.IsNullOrEmpty(field.key))
			{
				string text = field.String(vars, "");
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Replace("{param1}", param1).Replace("{param2}", param2).Replace("{param3}", param3);
					attributeConsoleManager.AttemptExecute(text);
				}
			}
		}
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0009AA58 File Offset: 0x00098C58
	[ConsoleMethod("ui", "show / hide UI")]
	public void ShowUI(int show)
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			if (baseUI.tCanvas == null)
			{
				return;
			}
			baseUI.tCanvas.gameObject.SetActive(show != 0);
		}
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x0009AA97 File Offset: 0x00098C97
	[ConsoleMethod("devcon", "enable / disable developer console")]
	public void EnableDevCon(int enable)
	{
		GameLogic.developer_console_enabled = (enable != 0);
		if (GameLogic.developer_console_enabled)
		{
			Debug.developerConsoleVisible = true;
		}
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x0009AAB0 File Offset: 0x00098CB0
	[ConsoleMethod("difficulty", "change difficulty")]
	public void SetDifficulty(int difficulty)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "difficulty", true))
		{
			return;
		}
		if (difficulty < 0 || difficulty > 3)
		{
			Debug.Log("Invalid difficulty level, must e 0..3");
			return;
		}
		GameLogic.Get(true).rules.ai_difficulty = difficulty;
		Debug.Log(string.Format("Difficulty set to {0}", difficulty));
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x0009AB08 File Offset: 0x00098D08
	[ConsoleMethod("kk", "kill current king")]
	private void KillSovereign()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "kk", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom != null)
		{
			kingdom.royalFamily.Sovereign.Die(null, "");
		}
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0009AB44 File Offset: 0x00098D44
	[ConsoleMethod("kk", "kill kingomd's(id) king")]
	private void KillSovereign(int kid)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "kk", true))
		{
			return;
		}
		global::Kingdom kingdom = global::Kingdom.Get(kid);
		if (kingdom != null && kingdom.logic != null)
		{
			kingdom.logic.royalFamily.Sovereign.Die(null, "");
		}
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x0009AB90 File Offset: 0x00098D90
	[ConsoleMethod("kk", "kill current king")]
	private void KillSovereignReason(string dead_reason)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "kk", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom != null)
		{
			kingdom.royalFamily.Sovereign.Die(new DeadStatus(dead_reason, kingdom.royalFamily.Sovereign), "");
		}
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x0009ABDC File Offset: 0x00098DDC
	[ConsoleMethod("kk", "kill current king with abidication reason and chnage type")]
	private void KillSovereignReason(string dead_reason, string change_type)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "kk", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom != null)
		{
			kingdom.royalFamily.Sovereign.Die(new DeadStatus(dead_reason, kingdom.royalFamily.Sovereign), change_type);
		}
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x0009AC24 File Offset: 0x00098E24
	[ConsoleMethod("add_child", "add a child to the royal family")]
	private void AddChild(int sex, int kid)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_child", true))
		{
			return;
		}
		global::Kingdom kingdom = global::Kingdom.Get(kid);
		if (kingdom == null || kingdom.logic == null)
		{
			Debug.Log("add_child command is unavailable for multiplayer clients");
			return;
		}
		if (!kingdom.logic.IsAuthority())
		{
			Debug.Log("add_child command is unavailable for multiplayer clients");
			return;
		}
		if (sex == 0)
		{
			kingdom.logic.royalFamily.AddChild(CharacterFactory.CreatePrince(kingdom.logic), true, true);
			return;
		}
		kingdom.logic.royalFamily.AddChild(CharacterFactory.CreatePrincess(kingdom.logic), true, true);
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x0009ACB8 File Offset: 0x00098EB8
	[ConsoleMethod("add_child", "add a child to the royal family")]
	private void AddChild(int sex)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_child", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("add_child - No kingdom selected");
			return;
		}
		if (!kingdom.IsAuthority())
		{
			Debug.Log("add_child command is unavailable for multiplayer clients");
			return;
		}
		Logic.Character character;
		if (sex == 0)
		{
			character = CharacterFactory.CreatePrince(kingdom);
		}
		else
		{
			character = CharacterFactory.CreatePrincess(kingdom);
		}
		kingdom.royalFamily.AddChild(character, true, true);
		Debug.Log(string.Format("Added {0} to {1}", character, kingdom));
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x0009AD30 File Offset: 0x00098F30
	[ConsoleMethod("add_child", "add a prince to the royal family")]
	private void AddChild()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_child", true))
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			global::Kingdom kingdom = global::Kingdom.Get(worldUI.GetCurrentKingdomId());
			this.AddChild(0, kingdom.id);
		}
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x0009AD74 File Offset: 0x00098F74
	[ConsoleMethod("lsg", "List Saved Games")]
	private void ListSavedGames()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "lsg", true))
		{
			return;
		}
		SaveGame.UpdateList(false, true);
		for (int i = 0; i < SaveGame.list.Count; i++)
		{
			Debug.Log(SaveGame.list[i].ToString());
		}
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0009ADC1 File Offset: 0x00098FC1
	[ConsoleMethod("save", "Save the game")]
	private void SaveTheGame(string param)
	{
		Debug.Log("Not Implemented!");
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x0009ADC1 File Offset: 0x00098FC1
	[ConsoleMethod("load", "Load the game")]
	private void LoadTheGame(string param)
	{
		Debug.Log("Not Implemented!");
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x0009ADCD File Offset: 0x00098FCD
	[ConsoleMethod("lcp", "load Camera Paths")]
	private void LoadCameraPaths()
	{
		CameraPath.LoadAll(null);
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x0009ADD8 File Offset: 0x00098FD8
	[ConsoleMethod("add_opportunity", "add opportunity to seleted character")]
	private void AddOpportunity(string action_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_opportunity", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		Actions component = character.GetComponent<Actions>();
		if (component == null)
		{
			Debug.Log(string.Format("{0} has no actions", character));
			return;
		}
		Action action = null;
		for (int i = 0; i < component.Count; i++)
		{
			Action action2 = component[i];
			Action.Def def = action2.def;
			if (((def != null) ? def.opportunity : null) != null && action2.def.field.key.IndexOf(action_name, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				action = action2;
				break;
			}
		}
		if (action == null)
		{
			Debug.LogError(string.Format("{0} has no opportunity containing '{1}'", character, action_name));
			return;
		}
		Opportunity opportunity = component.TryActivateOpportunity(action, true, false, true, null);
		if (opportunity == null)
		{
			Debug.Log(string.Format("Could not add opportunity for {0}", action));
			return;
		}
		Debug.Log(string.Format("(Normal chance to add: {0})", opportunity.def.ChanceToAddOnTick(opportunity.action)));
		Debug.Log(string.Format("Forcefully Added {0}", opportunity));
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x0009AEE4 File Offset: 0x000990E4
	[ConsoleMethod("del_opportunity", "delete opportunity from seleted character")]
	private void DelOpportunity(string action_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "del_opportunity", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		Actions component = character.GetComponent<Actions>();
		if (component == null)
		{
			Debug.Log(string.Format("{0} has no actions", character));
			return;
		}
		Action action = null;
		for (int i = 0; i < component.Count; i++)
		{
			Action action2 = component[i];
			Action.Def def = action2.def;
			if (((def != null) ? def.opportunity : null) != null && action2.def.field.key.IndexOf(action_name, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				action = action2;
				break;
			}
		}
		if (action == null)
		{
			Debug.LogError(string.Format("{0} has no opportunity containing '{1}'", character, action_name));
			return;
		}
		Opportunity opportunity = null;
		if (action.NeedsTarget())
		{
			List<Logic.Object> possibleTargets = action.GetPossibleTargets();
			if (possibleTargets != null)
			{
				for (int j = 0; j < possibleTargets.Count; j++)
				{
					opportunity = component.FindOpportunity(action, possibleTargets[j]);
					if (opportunity != null)
					{
						break;
					}
				}
			}
		}
		else
		{
			opportunity = component.FindOpportunity(action, null);
		}
		if (opportunity == null)
		{
			Debug.Log(string.Format("Could not find opportunity for {0}", action));
			return;
		}
		Debug.Log(string.Format("(Normal chance to del: {0})", opportunity.def.ChanceToDelOnTick(opportunity)));
		if (component.DelOpportunity(opportunity.action, opportunity.target, opportunity.args, false))
		{
			Debug.Log(string.Format("Deleted {0}", opportunity));
			return;
		}
		Debug.Log(string.Format("Delete failed for {0}", opportunity));
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x0009B058 File Offset: 0x00099258
	[ConsoleMethod("add_challenge", "add challenge to the player kingdom")]
	private void AddChallenge(string challenge_name)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		List<Challenge.Def> defs = GameLogic.Get(false).defs.GetDefs<Challenge.Def>();
		int i = 0;
		while (i < defs.Count)
		{
			Challenge.Def def = defs[i];
			if (def.field.key.IndexOf(challenge_name, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				float num = def.CalcWeight(kingdom, true);
				if (num <= 0f)
				{
					Debug.LogError(string.Format("Cannot create {0}.{1}: {2}", kingdom.Name, def.id, num));
					return;
				}
				Challenge challenge = Challenge.Create(def, kingdom);
				if (challenge == null)
				{
					Debug.LogError("Failed to create " + kingdom.Name + "." + def.id);
					return;
				}
				if (!challenge.Activate())
				{
					Debug.LogError("Failed to activate " + challenge.Dump());
					return;
				}
				Debug.Log("Added " + challenge.Dump());
				return;
			}
			else
			{
				i++;
			}
		}
		Debug.LogError("Unknown challenge name");
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x0009B158 File Offset: 0x00099358
	public Rumor.Def FindRumorDef(string name)
	{
		List<Rumor.Def> defs = GameLogic.Get(true).defs.GetDefs<Rumor.Def>();
		if (defs == null)
		{
			return null;
		}
		for (int i = 0; i < defs.Count; i++)
		{
			Rumor.Def def = defs[i];
			if (def.id.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return def;
			}
		}
		return null;
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x0009B1A8 File Offset: 0x000993A8
	[ConsoleMethod("new_rumor", "Add new rumor from seleted character")]
	private void NewRumor(string rumor_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "new_rumor", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		Rumor.Def def = this.FindRumorDef(rumor_name);
		if (def == null)
		{
			Debug.Log("No such rumor: " + rumor_name);
			return;
		}
		Rumor rumor = new Rumor(def, character.mission_kingdom ?? character.GetKingdom(), character.GetKingdom(), character);
		if (!rumor.Validate(false))
		{
			Debug.LogWarning(string.Format("Spreading fake news: {0}", rumor));
		}
		else
		{
			Debug.Log(string.Format("Spreading {0}", rumor));
		}
		rumor.Spread();
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x0009B244 File Offset: 0x00099444
	[ConsoleMethod("new_rumor", "Add new rumor from seleted character")]
	private void NewRumor()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "new_rumor", true))
		{
			return;
		}
		this.NewRumors(1);
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x0009B25C File Offset: 0x0009945C
	[ConsoleMethod("new_rumors", "Add new rumors from seleted character")]
	private void NewRumors(int max_count)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "new_rumors", true))
		{
			return;
		}
		this.NewRumors(0, max_count);
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x0009B278 File Offset: 0x00099478
	[ConsoleMethod("new_rumors", "Add new rumors from seleted character")]
	private void NewRumors(int min_count, int max_count)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "new_rumors", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		List<Rumor> list = Rumor.DecideNewRumors(character, max_count, min_count);
		if (list == null)
		{
			Debug.Log("No available rumors");
			return;
		}
		string text = string.Format("New rumors: {0}", list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Rumor arg = list[i];
			text += string.Format("\n{0}", arg);
		}
		Debug.Log(text);
		Rumor.SpreadRumors(list);
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x0009B310 File Offset: 0x00099510
	[ConsoleMethod("benchmark_rumors", "Benchmark rumors")]
	private void BenchmarkRumors()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "benchmark_rumors", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		int num = 10000;
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < num; i++)
		{
			Rumor.DecideNewRumors(character, 2, 4);
		}
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		Debug.Log(string.Format("{0} for {1}ms -> {2}ms", num, elapsedMilliseconds, (float)elapsedMilliseconds / (float)num));
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x0009B394 File Offset: 0x00099594
	[ConsoleMethod("q", "Perform query")]
	private void PerformQuery(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "q", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		DT.Field defField = global::Defs.GetDefField("TestQueries", name);
		if (defField == null)
		{
			Debug.Log("Query not found: TestQueries." + name);
			return;
		}
		Query q = new Query(game, defField);
		this.PerformQuery(q);
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x0009B3E8 File Offset: 0x000995E8
	private void PerformQuery(Query q)
	{
		if (!q.is_valid)
		{
			Debug.Log(string.Format("Invalid query: {0}", q));
			return;
		}
		Vars vars = new Vars(q.game.vars);
		vars.Set<Logic.Kingdom>("kingdom", BaseUI.SelKingdom() ?? BaseUI.LogicKingdom());
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		Value value = q.GetValue(vars);
		Vars.PopReflectionMode(old_mode);
		string text = Logic.Object.Dump(value);
		Game.CopyToClipboard(text);
		Debug.Log(text);
		object obj_val = value.obj_val;
		if (obj_val != null)
		{
			Logic.Object @object;
			if ((@object = (obj_val as Logic.Object)) != null)
			{
				Logic.Object obj = @object;
				BaseUI.Get().SelectObjFromLogic(obj, false, true);
				return;
			}
			List<Value> list;
			if ((list = (obj_val as List<Value>)) != null)
			{
				List<Value> list2 = list;
				if (list2.Count <= 0)
				{
					BaseUI.Get().SelectObj(null, false, true, true, true);
					return;
				}
				Logic.Object object2 = BaseUI.SelLO();
				int i = 0;
				while (i < list2.Count)
				{
					Logic.Object object3 = list2[i].Get<Logic.Object>();
					if (object3 != null && object3 == object2)
					{
						object3 = list2[(i + 1) % list2.Count].Get<Logic.Object>();
						if (object3 != null)
						{
							BaseUI.Get().SelectObjFromLogic(object3, false, true);
							return;
						}
						BaseUI.Get().SelectObjFromLogic(object3, false, true);
						return;
					}
					else
					{
						i++;
					}
				}
				Logic.Object object4 = list2[0].Get<Logic.Object>();
				if (object4 != null)
				{
					BaseUI.Get().SelectObjFromLogic(object4, false, true);
					return;
				}
				BaseUI.Get().SelectObjFromLogic(object4, false, true);
				return;
			}
		}
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x0009B56D File Offset: 0x0009976D
	[ConsoleMethod("tq", "Test Queries")]
	private void TestQueries()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tq", true))
		{
			return;
		}
		Query.Test(GameLogic.Get(true));
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x0009B589 File Offset: 0x00099789
	[ConsoleMethod("gai", "enable / disable AI globally")]
	private void EnableAI(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "gai", true))
		{
			return;
		}
		GameLogic.Get(true).ai.enabled = (enable != 0);
		this.DumpKingdomAI();
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0009B5B4 File Offset: 0x000997B4
	[ConsoleMethod("ai", "check if AI is on")]
	private void DumpKingdomAI()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ai", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		string text = "Global AI: " + DevCheats.<DumpKingdomAI>g__b2s|36_0(game.ai.enabled);
		DevCheats.<>c__DisplayClass36_0 CS$<>8__locals1;
		CS$<>8__locals1.kingdom = (BaseUI.SelKingdom() ?? BaseUI.LogicKingdom());
		if (CS$<>8__locals1.kingdom == null)
		{
			Debug.Log(text);
			return;
		}
		text = text + "\n" + CS$<>8__locals1.kingdom.Name + " AI:";
		if (CS$<>8__locals1.kingdom.ai == null)
		{
			text += " null";
		}
		else if (CS$<>8__locals1.kingdom.ai.enabled == KingdomAI.EnableFlags.Disabled)
		{
			text += " off";
		}
		else if (CS$<>8__locals1.kingdom.ai.enabled == KingdomAI.EnableFlags.All)
		{
			text += " ON";
		}
		else
		{
			for (KingdomAI.EnableFlags enableFlags = KingdomAI.EnableFlags.Kingdom; enableFlags < KingdomAI.EnableFlags.All; enableFlags <<= 1)
			{
				text += string.Format("\n  {0}: {1}", enableFlags, DevCheats.<DumpKingdomAI>g__f2s|36_1(enableFlags, ref CS$<>8__locals1));
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0009B6C8 File Offset: 0x000998C8
	private KingdomAI.EnableFlags ParseAIFlags(Logic.Kingdom k, string s)
	{
		KingdomAI.EnableFlags enableFlags = KingdomAI.EnableFlags.Disabled;
		int num = 0;
		int i = 0;
		while (i < s.Length)
		{
			char c = char.ToLowerInvariant(s[i]);
			KingdomAI.EnableFlags enableFlags2;
			if (c <= '1')
			{
				if (c != '+')
				{
					if (c != '-')
					{
						if (c == '1')
						{
							goto IL_C9;
						}
					}
					else
					{
						if (num == 0 && enableFlags == KingdomAI.EnableFlags.Disabled)
						{
							enableFlags = k.ai.enabled;
						}
						num = 2;
					}
				}
				else
				{
					if (num == 0 && enableFlags == KingdomAI.EnableFlags.Disabled)
					{
						enableFlags = k.ai.enabled;
					}
					num = 1;
				}
			}
			else if (c <= 'o')
			{
				if (c == '2')
				{
					goto IL_C9;
				}
				switch (c)
				{
				case 'a':
					enableFlags2 = KingdomAI.EnableFlags.Armies;
					goto IL_11A;
				case 'b':
					enableFlags2 = KingdomAI.EnableFlags.Buildings;
					goto IL_11A;
				case 'c':
					enableFlags2 = KingdomAI.EnableFlags.Characters;
					goto IL_11A;
				case 'd':
					enableFlags2 = KingdomAI.EnableFlags.Diplomacy;
					goto IL_11A;
				case 'g':
					enableFlags2 = KingdomAI.EnableFlags.Garrison;
					goto IL_11A;
				case 'h':
					enableFlags2 = KingdomAI.EnableFlags.HireCourt;
					goto IL_11A;
				case 'k':
					enableFlags2 = KingdomAI.EnableFlags.Kingdom;
					goto IL_11A;
				case 'm':
					enableFlags2 = (KingdomAI.EnableFlags.Armies | KingdomAI.EnableFlags.Units | KingdomAI.EnableFlags.Garrison | KingdomAI.EnableFlags.Offense);
					goto IL_11A;
				case 'o':
					enableFlags2 = KingdomAI.EnableFlags.Offense;
					goto IL_11A;
				}
			}
			else
			{
				if (c == 'u')
				{
					enableFlags2 = KingdomAI.EnableFlags.Units;
					goto IL_11A;
				}
				if (c == 'w')
				{
					enableFlags2 = KingdomAI.EnableFlags.Wars;
					goto IL_11A;
				}
			}
			IL_12B:
			i++;
			continue;
			IL_11A:
			if (num == 2)
			{
				enableFlags &= ~enableFlags2;
				goto IL_12B;
			}
			enableFlags |= enableFlags2;
			goto IL_12B;
			IL_C9:
			enableFlags2 = KingdomAI.EnableFlags.All;
			goto IL_11A;
		}
		return enableFlags;
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x0009B814 File Offset: 0x00099A14
	[ConsoleMethod("ai", "enable / disable all AI")]
	private void EnableGameAI(string mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "ai", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (((game != null) ? game.kingdoms : null) == null)
		{
			return;
		}
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (kingdom.ai != null && (!kingdom.is_player || !(mode != "2")))
			{
				kingdom.ai.enabled = this.ParseAIFlags(kingdom, mode);
			}
		}
		if (mode == "0")
		{
			Debug.Log("Disabled AI for all kingdoms");
			return;
		}
		if (mode == "1")
		{
			Debug.Log("Enabled AI for all non-player kingdoms");
			return;
		}
		if (mode == "2")
		{
			Debug.Log("Enabled AI for all kingdoms");
			return;
		}
		Debug.Log("Changed AI flags for non-player kingdoms");
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x0009B8EC File Offset: 0x00099AEC
	[ConsoleMethod("kai", "enabe / disable selected kingdom AI")]
	private void EnableKingdomAI(string mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "kai", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.ai.enabled = this.ParseAIFlags(kingdom, mode);
		this.DumpKingdomAI();
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x0009B934 File Offset: 0x00099B34
	[ConsoleMethod("tkai", "enabe / disable tracing for selected kingdom AI")]
	private void TraceKingdomAI(string mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "tkai", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.ai.trace_enabled = this.ParseAIFlags(kingdom, mode);
		Debug.Log(string.Format("{0}.trace_enabled = {1}", kingdom.ai, kingdom.ai.trace_enabled));
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0009B99C File Offset: 0x00099B9C
	[ConsoleMethod("aiel", "dump player AI expenses log")]
	private void DumpPlayerAIExpenses()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "aiel", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		string text;
		if (kingdom == null)
		{
			text = null;
		}
		else
		{
			KingdomAI ai = kingdom.ai;
			if (ai == null)
			{
				text = null;
			}
			else
			{
				StringBuilder expenses_log = ai.expenses_log;
				text = ((expenses_log != null) ? expenses_log.ToString() : null);
			}
		}
		string text2 = text;
		if (text2 == null)
		{
			Debug.Log("No AI expenses logged");
			return;
		}
		Game.CopyToClipboard(text2);
		Debug.Log(text2);
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x0009B9FC File Offset: 0x00099BFC
	[ConsoleMethod("bai", "enabe / disable battleview AI")]
	private void EnableBattleAI(int side, string mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "bai", true))
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle == null)
		{
			return;
		}
		battle.ai[side].owner_enabled = BattleAI.ParseAIFlags(battle.ai[side].owner_enabled, mode);
		battle.ai[side].supporter_enabled = BattleAI.ParseAIFlags(battle.ai[side].supporter_enabled, mode);
		battle.ai[side].second_supporter_enabled = BattleAI.ParseAIFlags(battle.ai[side].second_supporter_enabled, mode);
		battle.ai[side].mercenary_enabled = BattleAI.ParseAIFlags(battle.ai[side].mercenary_enabled, mode);
		battle.NotifyListeners("initialized_ai", null);
		Debug.Log(string.Format("Battle AI for side {0} {1}", side, battle.ai[side].owner_enabled.ToString()));
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x0009BADC File Offset: 0x00099CDC
	[ConsoleMethod("bai", "enabe / disable battleview AI")]
	private void EnableBattleAI(string mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "bai", true))
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle == null)
		{
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			battle.ai[i].owner_enabled = BattleAI.ParseAIFlags(battle.ai[i].owner_enabled, mode);
			battle.ai[i].supporter_enabled = BattleAI.ParseAIFlags(battle.ai[i].supporter_enabled, mode);
			battle.ai[i].second_supporter_enabled = BattleAI.ParseAIFlags(battle.ai[i].second_supporter_enabled, mode);
			battle.ai[i].mercenary_enabled = BattleAI.ParseAIFlags(battle.ai[i].mercenary_enabled, mode);
		}
		battle.NotifyListeners("initialized_ai", null);
		Debug.Log("Battle AI for both sides " + battle.ai[0].owner_enabled.ToString());
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x0009BBC7 File Offset: 0x00099DC7
	[ConsoleMethod("rai", "enabe / disable rebel AI")]
	private void EnableRebelAI(int mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "rai", true))
		{
			return;
		}
		Logic.Rebel.enabled = (mode != 0);
		Debug.Log("Rebel AI: " + ((mode != 0) ? "on" : "off"));
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x0009BBFF File Offset: 0x00099DFF
	[ConsoleMethod("rs", "enabe / disable rebel spawning")]
	private void EnableRebelSpawn(int mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "rs", true))
		{
			return;
		}
		RebellionRisk.enabled = (mode != 0);
		Debug.Log("Rebel Spawn " + ((mode != 0) ? "on" : "off"));
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x0009BC37 File Offset: 0x00099E37
	[ConsoleMethod("income_mul", "set gold income multiplier")]
	private void GoldIncomeMul(float mul)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "income_mul", true))
		{
			return;
		}
		Logic.Economy.Def.gold_income_multiplier = mul;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.RecalcIncomes(true);
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x0009BC5E File Offset: 0x00099E5E
	[ConsoleMethod("expense_mul", "set gold income multiplier")]
	private void GoldExpenseMul(float mul)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "expense_mul", true))
		{
			return;
		}
		Logic.Economy.Def.gold_expense_multiplier = mul;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.RecalcIncomes(true);
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x0009BC88 File Offset: 0x00099E88
	[ConsoleMethod("gold", "set player kingdom gold")]
	private void Gold(int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "gold", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.SetResources(ResourceType.Gold, (float)amount, true);
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x0009BCB8 File Offset: 0x00099EB8
	[ConsoleMethod("give_gold", "give gold to selected kingdom")]
	private void GiveGold(int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "give_gold", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.AddResources(KingdomAI.Expense.Category.Economy, ResourceType.Gold, (float)amount, true);
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x0009BCF4 File Offset: 0x00099EF4
	[ConsoleMethod("piety", "set player kingdom piety")]
	private void Piety(int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "piety", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.SetResources(ResourceType.Piety, (float)amount, true);
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x0009BD24 File Offset: 0x00099F24
	[ConsoleMethod("give_piety", "give piety to selected kingdom")]
	private void GivePiety(int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "give_piety", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.AddResources(KingdomAI.Expense.Category.Economy, ResourceType.Piety, (float)amount, true);
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x0009BD60 File Offset: 0x00099F60
	[ConsoleMethod("books", "set player kingdom books")]
	private void Books(int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "books", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.SetResources(ResourceType.Books, (float)amount, true);
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x0009BD90 File Offset: 0x00099F90
	[ConsoleMethod("give_books", "give books to selected kingdom")]
	private void GiveBooks(int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "give_books", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		kingdom.AddResources(KingdomAI.Expense.Category.Economy, ResourceType.Books, (float)amount, true);
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x0009BDCC File Offset: 0x00099FCC
	[ConsoleMethod("give", "give resources to selected kingdom")]
	private void GiveResources(string resources)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "give", true))
		{
			return;
		}
		Resource resource = Resource.Parse(resources, false);
		if (resource == null || resource.IsZero())
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		Debug.Log(string.Format("Giving {0} to {1}", resource, kingdom));
		kingdom.AddResources(KingdomAI.Expense.Category.Economy, resource, 1f, true);
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x0009BE34 File Offset: 0x0009A034
	[ConsoleMethod("cc", "chnage the class of the selected character")]
	private void ChnageClasss(string className)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "cc", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		character.SetClass(className);
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x0009BE6C File Offset: 0x0009A06C
	[ConsoleMethod("age", "Make selected character grow up soon")]
	private void GrowUp()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "age", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar() ?? (BaseUI.TTObj() as Logic.Character);
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		character.next_age_check = character.game.time;
		character.CheckAging();
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x0009BEC4 File Offset: 0x0009A0C4
	[ConsoleMethod("prestige", "add prestige to selected kingdom")]
	private void AddPrestige(string modifier, float mult)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "prestige", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		kingdom.AddPrestigeModifier(modifier, mult, null, true);
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x0009BF00 File Offset: 0x0009A100
	[ConsoleMethod("prestige", "add prestige to selected kingdom")]
	private void AddPrestige(float prestige)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "prestige", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		kingdom.AddPrestige(prestige, true);
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x0009BF38 File Offset: 0x0009A138
	[ConsoleMethod("prestige", "view selected kingdom's prestige")]
	private void ViewPrestige()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "prestige", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Debug.Log(string.Format("{0} prestige: {1}", kingdom.Name, kingdom.prestige));
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x0009BF88 File Offset: 0x0009A188
	[ConsoleMethod("fame", "add fame to selected kingdom")]
	private void AddFame(float fame)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "fame", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		kingdom.AddFame(fame, true);
		kingdom.fameObj.OnUpdate();
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x0009BFCC File Offset: 0x0009A1CC
	[ConsoleMethod("fame", "view selected kingdom's fame")]
	private void ViewFame()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "fame", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Debug.Log(string.Format("{0} fame: {1}", kingdom.Name, kingdom.fame));
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x0009C01C File Offset: 0x0009A21C
	[ConsoleMethod("max", "level up selected character to maximum using all skills (1) or random skills (0)")]
	private void LevelMax(int all_skills)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "max", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		while (character.ThinkSkills(all_skills != 0, true))
		{
		}
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x0009C05C File Offset: 0x0009A25C
	private Skill.Def FindSkillDef(string name)
	{
		List<Skill.Def> defs = GameLogic.Get(true).defs.GetDefs<Skill.Def>();
		for (int i = 0; i < defs.Count; i++)
		{
			Skill.Def def = defs[i];
			if (def.name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return def;
			}
		}
		return null;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x0009C0A8 File Offset: 0x0009A2A8
	[ConsoleMethod("skills", "View selected character's skills")]
	private void ViewSkills()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "skills", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		List<Skill> skills = character.skills;
		if (skills == null || skills.Count == 0)
		{
			Debug.Log(string.Format("{0} has no skills", character));
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("Skills of {0} ({1}):", character, skills.Count));
		for (int i = 0; i < skills.Count; i++)
		{
			Skill skill = skills[i];
			stringBuilder.AppendLine(string.Format("{0}: {1}", i, skill.def.name));
		}
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0009C168 File Offset: 0x0009A368
	[ConsoleMethod("new_skill", "Add skill to selected character's new skill options")]
	private void NewSkill(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "new_skill", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		Skill.Def def = this.FindSkillDef(name);
		if (def == null)
		{
			Debug.LogError("invalid skill: " + name);
			return;
		}
		character.GenerateNewSkills(false, false);
		if (character.new_skills == null)
		{
			Debug.LogError("cannot have new skills");
			return;
		}
		if (character.new_skills.Contains(def))
		{
			Debug.Log("already exists");
			return;
		}
		character.new_skills.Add(def);
		Debug.Log(string.Format("Added '{0}' to the new skills of {1}", def.name, character));
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x0009C20C File Offset: 0x0009A40C
	[ConsoleMethod("add_skill", "Add skill to selected character")]
	private void AddSkill(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_skill", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		Skill.Def def = this.FindSkillDef(name);
		if (def == null)
		{
			Debug.LogError("invalid skill: " + name);
			return;
		}
		Debug.Log(string.Format("Adding {0} to {1}", def.name, character));
		character.AddSkill(def, true);
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x0009C278 File Offset: 0x0009A478
	[ConsoleMethod("set_skill", "Replace selected character's skill")]
	private void SetSkill(int idx, string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "set_skill", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No selected character");
			return;
		}
		Skill.Def def = null;
		if (name != "null")
		{
			def = this.FindSkillDef(name);
			if (def == null)
			{
				Debug.LogError("invalid skill: " + name);
				return;
			}
		}
		if (character.SetSkill(idx, def, true))
		{
			Debug.Log(string.Format("Set skill {0} to {1}", idx, def));
			return;
		}
		Debug.LogError(string.Format("Failed to set skill {0} to {1}", idx, def));
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0009C30C File Offset: 0x0009A50C
	private Tradition.Def FindTraditionDef(string name)
	{
		List<Tradition.Def> defs = GameLogic.Get(true).defs.GetDefs<Tradition.Def>();
		for (int i = 0; i < defs.Count; i++)
		{
			Tradition.Def def = defs[i];
			if (def.name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return def;
			}
		}
		return null;
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x0009C358 File Offset: 0x0009A558
	[ConsoleMethod("add_tradition", "Add tradition (or increase rank of existing tradition)")]
	private void RankTradition(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_tradition", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Tradition.Def def = this.FindTraditionDef(name);
		if (def == null)
		{
			Debug.LogError("invalid tradition: " + name);
			return;
		}
		Tradition tradition = kingdom.FindTradition(def);
		if (tradition == null)
		{
			int freeTraditionIndex = kingdom.GetFreeTraditionIndex(def.type);
			if (freeTraditionIndex < 0)
			{
				Debug.LogError(string.Format("Can't add {0} to {1}", def.name, kingdom));
				return;
			}
			Debug.Log(string.Format("Adding {0} to {1}", def.name, kingdom));
			kingdom.SetTradition(freeTraditionIndex, def, 1, true);
			return;
		}
		else
		{
			if (tradition.rank >= tradition.def.max_rank)
			{
				Debug.Log(string.Format("Already at rank {0}", tradition.rank));
				return;
			}
			Debug.Log(string.Format("Setting rank of {0} of {1} to {2}", def.name, kingdom, tradition.rank + 1));
			tradition.SetRank(tradition.rank + 1, true);
			return;
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0009C459 File Offset: 0x0009A659
	private List<Logic.Character.SkillReplacement> GetSkillReplaceOptions(Logic.Character c, int slot)
	{
		c.GetSkillReplaceOptions(slot, Logic.Character.SkillReplacement.Type.Affinity, false);
		c.GetSkillReplaceOptions(slot, Logic.Character.SkillReplacement.Type.Book, true);
		c.GetSkillReplaceOptions(slot, Logic.Character.SkillReplacement.Type.Realm, true);
		return c.GetSkillReplaceOptions(slot, Logic.Character.SkillReplacement.Type.GreatPerson, true);
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x0009C484 File Offset: 0x0009A684
	[ConsoleMethod("rcs", "View character skill replacement options")]
	private void ViewCharacterReplacemtSkills(int slot)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rcs", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		string text = string.Concat(new object[]
		{
			character.ToString(),
			", slot ",
			slot,
			" ("
		});
		if (character.skills != null && slot >= 0 && slot < character.skills.Count)
		{
			Skill skill = character.skills[slot];
			text += ((skill == null) ? "null" : skill.ToString());
		}
		else if (slot == character.GetSkillsCount())
		{
			text += "new";
		}
		else
		{
			text += "invalid";
		}
		text += "): ";
		List<Logic.Character.SkillReplacement> skillReplaceOptions = this.GetSkillReplaceOptions(character, slot);
		if (skillReplaceOptions == null || skillReplaceOptions.Count == 0)
		{
			text += "none";
			Debug.Log(text);
			return;
		}
		text += skillReplaceOptions.Count.ToString();
		for (int i = 0; i < skillReplaceOptions.Count; i++)
		{
			Logic.Character.SkillReplacement skillReplacement = skillReplaceOptions[i];
			text = string.Concat(new object[]
			{
				text,
				"\n  ",
				i,
				": ",
				skillReplacement.ToString()
			});
		}
		Debug.Log(text);
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0009C5E8 File Offset: 0x0009A7E8
	[ConsoleMethod("rcs", "Replace character skill")]
	private void ReplaceCharacterSkill(int slot, int option)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "rcs", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		string text = string.Concat(new object[]
		{
			character.ToString(),
			", slot ",
			slot,
			" ("
		});
		if (character.skills != null && slot >= 0 && slot < character.skills.Count)
		{
			Skill skill = character.skills[slot];
			text += ((skill == null) ? "null" : skill.ToString());
		}
		else if (slot == character.GetSkillsCount())
		{
			text += "new";
		}
		else
		{
			text += "invalid";
		}
		text += ") -> ";
		List<Logic.Character.SkillReplacement> skillReplaceOptions = this.GetSkillReplaceOptions(character, slot);
		if (skillReplaceOptions == null || option < 0 || option >= skillReplaceOptions.Count)
		{
			text += "invalid";
			Debug.Log(text);
			return;
		}
		Logic.Character.SkillReplacement skillReplacement = skillReplaceOptions[option];
		text += skillReplacement.ToString();
		Debug.Log(text);
		if (!character.ReplaceSkill(slot, skillReplacement))
		{
			Debug.LogWarning("ReplaceSkill failed");
		}
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0009C714 File Offset: 0x0009A914
	[ConsoleMethod("ts", "have the selected character think about skills using all skills (1) or random skills (0)")]
	private void ThinkSkills(int all_skills)
	{
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		character.ThinkSkills(all_skills != 0, false);
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x0009C744 File Offset: 0x0009A944
	[ConsoleMethod("tactics", "List valid tactics for selected army")]
	private void ListTactics()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tactics", true))
		{
			return;
		}
		Logic.Object obj = BaseUI.SelLO();
		string text = "";
		this.AddTactics(obj, ref text);
		if (text == "")
		{
			text = "No valid tactics";
		}
		Debug.Log(text);
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x0009C790 File Offset: 0x0009A990
	private void AddTactics(Logic.Object obj, ref string txt)
	{
		if (obj != null)
		{
			Logic.Army army;
			if ((army = (obj as Logic.Army)) != null)
			{
				Logic.Army a = army;
				this.AddArmyTactics(a, ref txt);
				return;
			}
			Logic.Character character;
			if ((character = (obj as Logic.Character)) != null)
			{
				Logic.Character character2 = character;
				this.AddArmyTactics(character2.GetArmy(), ref txt);
				return;
			}
			Castle castle;
			if ((castle = (obj as Castle)) != null)
			{
				Castle castle2 = castle;
				this.AddArmyTactics(castle2.army, ref txt);
				return;
			}
			Logic.Battle battle;
			if ((battle = (obj as Logic.Battle)) != null)
			{
				Logic.Battle battle2 = battle;
				this.AddArmyTactics(battle2.attackers, ref txt);
				this.AddArmyTactics(battle2.defenders, ref txt);
			}
		}
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0009C820 File Offset: 0x0009AA20
	private void AddArmyTactics(List<Logic.Army> armies, ref string txt)
	{
		if (armies == null)
		{
			return;
		}
		for (int i = 0; i < armies.Count; i++)
		{
			Logic.Army a = armies[i];
			this.AddArmyTactics(a, ref txt);
		}
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0009C854 File Offset: 0x0009AA54
	private void AddArmyTactics(Logic.Army a, ref string txt)
	{
		string text = this.ListArmyTactics(a);
		if (text == "")
		{
			return;
		}
		if (txt != "")
		{
			txt += "\n";
		}
		txt += text;
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0009C8A0 File Offset: 0x0009AAA0
	private string ListArmyTactics(Logic.Army a)
	{
		if (a == null)
		{
			return "";
		}
		List<BattleTactic.Def> validTactics = BattleTactic.Def.GetValidTactics(a);
		if (validTactics == null || validTactics.Count == 0)
		{
			return "";
		}
		string text = string.Format("{0}: ", a);
		for (int i = 0; i < validTactics.Count; i++)
		{
			BattleTactic.Def def = validTactics[i];
			DT.Field field = def.field;
			string str = ((field != null) ? field.GetString("name", null, "", true, true, true, '.') : null) ?? def.id;
			if (i > 0)
			{
				text += ", ";
			}
			text += str;
		}
		return text;
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x0009C93B File Offset: 0x0009AB3B
	[ConsoleMethod("bcp", "benchmark containers performance")]
	private void BenchmarkContainers()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "bcp", true))
		{
			return;
		}
		Container.Benchmark();
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x0009C954 File Offset: 0x0009AB54
	[ConsoleMethod("bug_profiler", "bug the profiler to test how it recovers")]
	private void BugProfiler(int mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "bug_profiler", true))
		{
			return;
		}
		Profile.BeginSection("A");
		Profile.BeginSection("B");
		Profile.BeginSection("C");
		if (mode == 0)
		{
			Profile.EndSection("C");
		}
		Profile.EndSection("B");
		Profile.EndSection("A");
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x0009C9B0 File Offset: 0x0009ABB0
	[ConsoleMethod("profile_profiler", "profile the profile scopes overhead")]
	private void ProfileProfileScopes()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "profile_profiler", true))
		{
			return;
		}
		int num = 1000000;
		using (Game.Profile(string.Format("{0} profiles with Unity profiler", num), true, 0f, null))
		{
			for (int i = 0; i < num; i++)
			{
				using (Game.Profile("inner", false, 0f, null))
				{
				}
			}
		}
		using (Game.Profile(string.Format("{0} profiles without Unity profiler", num), true, 0f, null))
		{
			for (int j = 0; j < num; j++)
			{
				using (Game.Profile("inner", false, -1f, null))
				{
				}
			}
		}
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x0009CAC4 File Offset: 0x0009ACC4
	[ConsoleMethod("clear_stats_profile", "clear stats profile")]
	private void ClearStatsProfile()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "clear_stats_profile", true))
		{
			return;
		}
		Stats.ClearProfile(GameLogic.Get(true));
		Debug.Log("Stats profile cleared");
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x0009CAEA File Offset: 0x0009ACEA
	[ConsoleMethod("profile_stats", "dump stats profile")]
	private void ProfileStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "profile_stats", true))
		{
			return;
		}
		string text = Stats.ProfileText(GameLogic.Get(true), false, "tclhfa");
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x0009CB17 File Offset: 0x0009AD17
	[ConsoleMethod("profile_stats", "dump stats profile")]
	private void ProfileStats(int reset)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "profile_stats", true))
		{
			return;
		}
		string text = Stats.ProfileText(GameLogic.Get(true), reset != 0, "tclhfa");
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x0009CB47 File Offset: 0x0009AD47
	[ConsoleMethod("profile_stats", "dump stats profile")]
	private void ProfileStats(string sort_order)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "profile_stats", true))
		{
			return;
		}
		string text = Stats.ProfileText(GameLogic.Get(true), false, sort_order);
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0009CB70 File Offset: 0x0009AD70
	[ConsoleMethod("profile_stats", "dump stats profile")]
	private void ProfileStats(int reset, string sort_order)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "profile_stats", true))
		{
			return;
		}
		string text = Stats.ProfileText(GameLogic.Get(true), reset != 0, sort_order);
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0009CB9C File Offset: 0x0009AD9C
	[ConsoleMethod("stat_optimisations", "enable / disable stats optimisations")]
	private void EnableStatsOptimisations(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "stat_optimisations", true))
		{
			return;
		}
		Stat.disable_const_optimisations = (enable == 0);
		Stat.disable_force_cache_optimisations = (enable == 0);
		Debug.Log("Stat optimisations " + ((enable == 0) ? "disabled" : "enabled"));
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x0009CBE8 File Offset: 0x0009ADE8
	[ConsoleMethod("economy_optimisations", "enable / disable economy optimisations")]
	private void EnableEconomyOptimisations(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "economy_optimisations", true))
		{
			return;
		}
		IncomePerResource.disable_const_optimisations = (enable == 0);
		Debug.Log("Income optimisations " + ((enable == 0) ? "disabled" : "enabled"));
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x0009CC20 File Offset: 0x0009AE20
	[ConsoleMethod("profile_income", "profile incomes calculation")]
	private void ProfileIncome()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "profile_income", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		int num = 100;
		Game game = GameLogic.Get(true);
		bool disable_const_optimisations = IncomePerResource.disable_const_optimisations;
		IncomePerResource.disable_const_optimisations = true;
		kingdom.RecalcIncomeNow();
		Stats.ClearProfile(game);
		int num_calcs = Expression.num_calcs;
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < num; i++)
		{
			kingdom.RecalcIncomeNow();
		}
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		int total_calcs = Stat.Def.total_calcs;
		int total_lookups = Stat.Def.total_lookups;
		int num2 = Expression.num_calcs - num_calcs;
		Resource income = kingdom.income.Copy();
		Resource upkeep = kingdom.expenses.Copy();
		string text = "Old:\n====\n\n" + Stats.ProfileText(game, false, "tclhfa");
		IncomePerResource.disable_const_optimisations = false;
		kingdom.RecalcIncomeNow();
		Stats.ClearProfile(game);
		num_calcs = Expression.num_calcs;
		stopwatch.Restart();
		for (int j = 0; j < num; j++)
		{
			kingdom.RecalcIncomeNow();
		}
		long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
		int total_calcs2 = Stat.Def.total_calcs;
		int total_lookups2 = Stat.Def.total_lookups;
		int num3 = Expression.num_calcs - num_calcs;
		Resource income2 = kingdom.income.Copy();
		Resource upkeep2 = kingdom.expenses.Copy();
		text = text + "\n\n\n\nNew:\n====\n\n" + Stats.ProfileText(game, false, "tclhfa");
		IncomePerResource.disable_const_optimisations = disable_const_optimisations;
		kingdom.RecalcIncomeNow();
		Game.CopyToClipboard(text);
		string text2 = string.Format("Old: {0}ms, Stat calcs: {1}, Lookups: {2}, Expressions: {3}", new object[]
		{
			(float)elapsedMilliseconds / (float)num,
			total_calcs,
			total_lookups,
			num2
		});
		text2 += string.Format("\nNew: {0}ms, Stat calcs: {1}, Lookups: {2}, Expressions: {3}", new object[]
		{
			(float)elapsedMilliseconds2 / (float)num,
			total_calcs2,
			total_lookups2,
			num3
		});
		text2 = string.Concat(new string[]
		{
			text2,
			"\nGold: ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Gold, income, upkeep),
			" vs ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Gold, income2, upkeep2)
		});
		text2 = string.Concat(new string[]
		{
			text2,
			"\nFood: ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Food, income, upkeep),
			" vs ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Food, income2, upkeep2)
		});
		text2 = string.Concat(new string[]
		{
			text2,
			"\nBooks: ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Books, income, upkeep),
			" vs ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Books, income2, upkeep2)
		});
		text2 = string.Concat(new string[]
		{
			text2,
			"\nPiety: ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Piety, income, upkeep),
			" vs ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Piety, income2, upkeep2)
		});
		text2 = string.Concat(new string[]
		{
			text2,
			"\nTrade: ",
			DevCheats.<ProfileIncome>g__pstr1|89_0(kingdom.GetMaxCommerce(), kingdom.GetAllocatedCommerce()),
			" vs ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Trade, income2, upkeep2)
		});
		text2 = string.Concat(new string[]
		{
			text2,
			"\nLevy: ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Levy, income, upkeep),
			" vs ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Levy, income2, upkeep2)
		});
		text2 = string.Concat(new string[]
		{
			text2,
			"\nHammers: ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Hammers, income, upkeep),
			" vs ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.Hammers, income2, upkeep2)
		});
		text2 = string.Concat(new string[]
		{
			text2,
			"\nWorkerSlots: ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.WorkerSlots, income, upkeep),
			" vs ",
			DevCheats.<ProfileIncome>g__pstr|89_1(ResourceType.WorkerSlots, income2, upkeep2)
		});
		Debug.Log(text2);
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x0009D003 File Offset: 0x0009B203
	[ConsoleMethod("lobbyfilter", "Enable/disable the default lobby filter")]
	private void EnableDefaultLobbyFilter(int enabled)
	{
		Game.isDefaultLobbyFilterEnabled = (enabled > 0);
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x0009D010 File Offset: 0x0009B210
	[ConsoleMethod("inviteTest", "can player be invited")]
	private void CanPlayerBeInvited(string playerId)
	{
		DevCheats.<>c__DisplayClass91_0 CS$<>8__locals1 = new DevCheats.<>c__DisplayClass91_0();
		CS$<>8__locals1.playerId = playerId;
		Logic.Coroutine.Start("CanPlayerBeInvited", CS$<>8__locals1.<CanPlayerBeInvited>g__coro|0(), null);
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x0009D03C File Offset: 0x0009B23C
	[ConsoleMethod("smll", "set multiplayer log level")]
	private void SetMultiplayerLogLevel(int level)
	{
		Logic.Multiplayer.SetLogLevel(level, -1);
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x0009D045 File Offset: 0x0009B245
	[ConsoleMethod("thqreq", "Enable THQNO requests")]
	private void EnableTHQNORequests(int enabled)
	{
		THQNORequest.enabled = (enabled > 0);
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x0009D050 File Offset: 0x0009B250
	[ConsoleMethod("overlayposition", "Set overlay position")]
	private void SetOverlayNotificationPosition(int pos)
	{
		THQNORequest.SetOverlayNotificationPosition((Logic.Common.OverlayNotificationPosition)pos);
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x0009D059 File Offset: 0x0009B259
	[ConsoleMethod("gameoverlay", "Activate platform overlay")]
	private void ActivateGameOverlay(int mode)
	{
		THQNORequest.ActivateGameOverlay((Logic.Common.OverlayMode)mode);
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x0009D062 File Offset: 0x0009B262
	[ConsoleMethod("inviteoverlay", "Activate invite overlay")]
	private void ActivateInviteOverlay(int mode)
	{
		THQNORequest.ActivateInviteOverlay();
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0009D06A File Offset: 0x0009B26A
	[ConsoleMethod("overlaytouser", "Activate user overlay")]
	private void ActivateGameOverlayToUser(string user_id)
	{
		THQNORequest.ActivateGameOverlayToUser(Logic.Common.UserOverlayMode.Default, user_id);
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x0009D074 File Offset: 0x0009B274
	[ConsoleMethod("invcode", "Generate invite code and copy to clipboard")]
	private void GenerateInviteCode()
	{
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			Debug.Log("GenerateInviteCode failed. MPBoss is null!");
			return;
		}
		MPBoss mpboss2 = mpboss;
		Campaign campaign;
		if (mpboss == null)
		{
			campaign = null;
		}
		else
		{
			Game game = mpboss.game;
			campaign = ((game != null) ? game.campaign : null);
		}
		string text = mpboss2.GenerateInviteCode(campaign);
		Debug.Log("Invite code is: " + text);
		Game.CopyToClipboard(text);
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x0009D0CC File Offset: 0x0009B2CC
	[ConsoleMethod("joincode", "Join a campaign trough an invite code")]
	private void JoinCampaignInviteCode(string invite_code)
	{
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			Debug.Log("JoinCampaignInviteCode failed. MPBoss is null!");
			return;
		}
		Debug.Log("Joining campaign for invite code: " + invite_code);
		Campaign campaign = mpboss.FindFirstEmptyCampaign();
		if (campaign == null)
		{
			Debug.Log("Couldn't find an empty campaign slot. Please free a campaign slot first and then try to join again.");
			return;
		}
		mpboss.JoinCampaignTroughInviteCode(campaign, invite_code);
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x0009D11C File Offset: 0x0009B31C
	[ConsoleMethod("doctest", "test the getting of a thqno document")]
	private void DocumentReadTest()
	{
		DevCheats.<>c__DisplayClass100_0 CS$<>8__locals1 = new DevCheats.<>c__DisplayClass100_0();
		CS$<>8__locals1.documentName = "Settings";
		Logic.Coroutine.Start("DocumentReadTestCoro", CS$<>8__locals1.<DocumentReadTest>g__DocumentReadTestCoro|0(), null);
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x0009D14C File Offset: 0x0009B34C
	[ConsoleMethod("lobbyst", "Lobby stress test")]
	private void LobbyStressTest(int numberOfLobbies = 1000)
	{
		DevCheats.<>c__DisplayClass101_0 CS$<>8__locals1 = new DevCheats.<>c__DisplayClass101_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.numberOfLobbies = numberOfLobbies;
		Debug.Log(string.Format("Starting a lobby stress test. Creating {0} lobbies.", CS$<>8__locals1.numberOfLobbies));
		Logic.Coroutine.Start("LobbyStressTest", CS$<>8__locals1.<LobbyStressTest>g__LobbyStressTest|0(), null);
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x0009D19C File Offset: 0x0009B39C
	private void SetMockLobbyData(string lobbyId, int index)
	{
		THQNORequest.SetLobbyStringData(lobbyId, "campaign_id", string.Format("{0}", index));
		THQNORequest.SetLobbyStringData(lobbyId, "owner_name", THQNORequest.playerName);
		THQNORequest.SetLobbyStringData(lobbyId, "owner_id", THQNORequest.userId);
		string value = Title.Version(true);
		if (!string.IsNullOrEmpty(value))
		{
			THQNORequest.SetLobbyStringData(lobbyId, "client_version", value);
		}
		else
		{
			Debug.LogError("Could not extract version while setting up THQNO lobby data!");
		}
		string value2 = Title.BranchName();
		if (!string.IsNullOrEmpty(value2))
		{
			THQNORequest.SetLobbyStringData(lobbyId, "client_branch", value2);
		}
		else
		{
			Debug.LogError("Could not extract branch name while setting up THQNO lobby data!");
		}
		ModManager modManager = ModManager.Get(false);
		if (modManager != null)
		{
			THQNORequest.SetLobbyStringData(lobbyId, "mod_id", modManager.activeModID);
		}
		string[] array = new string[]
		{
			"None",
			"HaveXGold",
			"HaveXRealms",
			"FirstBlood",
			"WarForGoods",
			"DestroyKingdom"
		};
		string[] array2 = new string[]
		{
			"1",
			"2",
			"3",
			"5"
		};
		string[] array3 = new string[]
		{
			"early",
			"mid",
			"late"
		};
		string[] array4 = new string[]
		{
			"kingdom",
			"province",
			"random_kingdom",
			"random_province"
		};
		string[] array5 = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5"
		};
		string[] array6 = new string[]
		{
			"easy",
			"normal",
			"hard",
			"very_hard"
		};
		string[] array7 = new string[]
		{
			"none",
			"60m",
			"120m",
			"180m",
			"300m",
			"420m",
			"3g",
			"4g",
			"5g",
			"10g"
		};
		THQNORequest.SetLobbyStringData(lobbyId, "main_goal", array[Random.Range(0, array.Length)]);
		THQNORequest.SetLobbyStringData(lobbyId, "name", string.Format("Sample Campaign [{0}] @ {1}", index, THQNORequest.playerName));
		THQNORequest.SetLobbyIntData(lobbyId, "player_cnt", Random.Range(1, 7));
		THQNORequest.SetLobbyIntData(lobbyId, "max_players", 6);
		THQNORequest.SetLobbyStringData(lobbyId, "team_size", array2[Random.Range(0, array2.Length)]);
		THQNORequest.SetLobbyStringData(lobbyId, "start_period", array3[Random.Range(0, array3.Length)]);
		THQNORequest.SetLobbyStringData(lobbyId, "pick_kingdom", array4[Random.Range(0, array4.Length)]);
		THQNORequest.SetLobbyStringData(lobbyId, "kingdom_size", array5[Random.Range(0, array5.Length)]);
		THQNORequest.SetLobbyStringData(lobbyId, "ai_difficulty", array6[Random.Range(0, array6.Length)]);
		THQNORequest.SetLobbyStringData(lobbyId, "time_limit", array7[Random.Range(0, array7.Length)]);
		THQNORequest.SetLobbyStringData(lobbyId, "creation_time", DateTime.Now.ToString("O"));
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x0009D4C0 File Offset: 0x0009B6C0
	[ConsoleMethod("aio", "Activate invite overlay")]
	private void ActivateInviteOverlay()
	{
		THQNO_Wrapper.ActivateInviteOverlay();
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x0009D4C8 File Offset: 0x0009B6C8
	[ConsoleMethod("swcs", "Swap Campaign Slots")]
	private void SwapCampaignSlots(int idx1, int idx2)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "swap_campaign_slots", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		Campaign campaign = (game != null) ? game.campaign : null;
		if (campaign == null)
		{
			Debug.LogError("No current campaign");
			return;
		}
		Debug.Log(string.Format("Swapping slots {0} and {1}", idx1, idx2));
		campaign.SwapPlayers(idx1, idx2, true);
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x0009D529 File Offset: 0x0009B729
	[ConsoleMethod("force_endless_game")]
	private void ForceEndlessGame(int mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "force_endless_game", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		game.SetForceEndlessGame(mode != 0);
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x0009D550 File Offset: 0x0009B750
	[ConsoleMethod("dumpallfriends", "Dump all friends")]
	private void DumpAllFriends()
	{
		if (THQNORequest.platformType != Logic.Common.PlatformType.Steam)
		{
			Debug.LogWarning(string.Format("Cannot dump friends! Current platform type: {0}", THQNORequest.platformType));
			return;
		}
		uint num = 0U;
		THQNORequest friendCount = THQNORequest.GetFriendCount();
		if (friendCount.error == null)
		{
			num = (uint)friendCount.result.Int(0);
		}
		Debug.Log("Friends:");
		for (uint num2 = 0U; num2 < num; num2 += 1U)
		{
			string text = string.Empty;
			THQNORequest friendByIndex = THQNORequest.GetFriendByIndex(num2);
			if (friendByIndex.error == null)
			{
				text = friendByIndex.result.String(null);
			}
			if (!string.IsNullOrEmpty(text))
			{
				StringBuilder stringBuilder = new StringBuilder();
				THQNORequest.GetFriendPersonaName(text, stringBuilder, 128U);
				Debug.Log(stringBuilder.ToString() + " - " + text);
			}
		}
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x0009D60B File Offset: 0x0009B80B
	[ConsoleMethod("p2p", "Enable/Disable P2P")]
	private void SetP2PEnabled(int enabled)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "p2p", true))
		{
			return;
		}
		THQNORequest.devIgnoreP2P = (enabled == 0);
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x0009D628 File Offset: 0x0009B828
	[ConsoleMethod("rts", "Reset traffic stats")]
	private void ResetTrafficStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rts", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game != null && game.multiplayer != null)
		{
			NetworkProfiler tx_profiler = game.multiplayer.tx_profiler;
			if (tx_profiler != null)
			{
				tx_profiler.Reset();
			}
			NetworkProfiler rx_profiler = game.multiplayer.rx_profiler;
			if (rx_profiler != null)
			{
				rx_profiler.Reset();
			}
			Debug.Log("Traffic stats reset");
		}
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x0009D68C File Offset: 0x0009B88C
	[ConsoleMethod("dts", "Dump traffic stats")]
	private void DumpTrafficStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dts", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game != null && game.multiplayer != null)
		{
			NetworkProfiler tx_profiler = game.multiplayer.tx_profiler;
			NetworkProfiler rx_profiler = game.multiplayer.rx_profiler;
			if (tx_profiler != null)
			{
				Debug.Log("TX: " + tx_profiler.ToString());
				string contents = tx_profiler.ToCSV();
				File.WriteAllText("tx.csv", contents);
			}
			if (rx_profiler != null)
			{
				Debug.Log("RX: " + rx_profiler.ToString());
				string contents2 = rx_profiler.ToCSV();
				File.WriteAllText("rx.csv", contents2);
			}
			Debug.Log("Traffic stats dumped");
		}
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x0009D738 File Offset: 0x0009B938
	[ConsoleMethod("cp", "Show current players")]
	private void ShowCurrentPlayers()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "cp", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("UI kingdom: " + kingdom.ToString());
		Game game = GameLogic.Get(true);
		Logic.Multiplayer multiplayer = (game != null) ? game.multiplayer : null;
		if (multiplayer == null)
		{
			stringBuilder.AppendLine("Multiplayer is null");
			Debug.Log(stringBuilder.ToString());
			return;
		}
		stringBuilder.AppendLine(string.Format("Multiplayer pid: {0}, kingdom: {1}({2})", multiplayer.playerData.pid, multiplayer.playerData.kingdomId, multiplayer.playerData.kingdomName));
		stringBuilder.AppendLine("Current Players: " + Logic.Multiplayer.CurrentPlayers.Count());
		List<Logic.Multiplayer.PlayerData> all = Logic.Multiplayer.CurrentPlayers.GetAll();
		for (int i = 0; i < all.Count; i++)
		{
			Logic.Kingdom kingdom2 = game.GetKingdom(all[i].kingdomId);
			stringBuilder.AppendLine(string.Format("  {0}: {1}({2}), k.ai.enabled = {3}", new object[]
			{
				all[i].pid,
				all[i].kingdomId,
				all[i].kingdomName,
				(kingdom2 != null) ? new KingdomAI.EnableFlags?(kingdom2.ai.enabled) : null
			}));
		}
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x0009D8C0 File Offset: 0x0009BAC0
	[ConsoleMethod("reflection", "Change default reflection mode")]
	public void SetReflectionMode(int level)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "reflection", true))
		{
			return;
		}
		switch (level)
		{
		case 0:
			Vars.PushReflectionMode(Vars.ReflectionMode.Disabled);
			break;
		case 1:
			Vars.PushReflectionMode(Vars.ReflectionMode.Log);
			break;
		case 2:
			Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
			break;
		default:
			Debug.LogError("Invalid value, must be 0,1 or 2");
			return;
		}
		Debug.Log(string.Format("Reflection mode: {0}", Vars.GetReflectionMode()));
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x0009D930 File Offset: 0x0009BB30
	private Value CalcExpr(Expression ex, object obj = null, bool as_value = true, Vars.ReflectionMode reflection_mode = Vars.ReflectionMode.Enabled)
	{
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(reflection_mode);
		DefsContext context = GameLogic.Get(true).dt.context;
		DevCheats.tmp_ctx_vars.obj = new Value(obj);
		context.vars = DevCheats.tmp_ctx_vars;
		Value result = ex.Calc(context, as_value);
		context.vars = null;
		DevCheats.tmp_ctx_vars.obj = Value.Unknown;
		Vars.PopReflectionMode(old_mode);
		return result;
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x0009D998 File Offset: 0x0009BB98
	public void DumpVar(string expr_str, Vars.ReflectionMode reflection_mode, bool as_value)
	{
		Expression expression = Expression.Parse(expr_str, true);
		if (expression == null || expression.type == Expression.Type.Invalid)
		{
			Debug.Log("invalid expression");
			return;
		}
		Value value = this.CalcExpr(expression, BaseUI.SelLO(), as_value, reflection_mode);
		string text = expression.ToString() + " -> ";
		if (as_value)
		{
			text += value.ToString();
		}
		else
		{
			string text2 = Logic.Object.Dump(value);
			text += text2;
			Game.CopyToClipboard(text2);
		}
		Debug.Log(text);
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x0009DA1C File Offset: 0x0009BC1C
	[ConsoleMethod("?", "Calc expression (as value)")]
	public void DumpVarAsValue(string expr_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "?", true))
		{
			return;
		}
		this.DumpVar(expr_str, Vars.ReflectionMode.Enabled, true);
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x0009DA36 File Offset: 0x0009BC36
	[ConsoleMethod("??", "Calc expression (as container)")]
	public void DumpVarAsContainer(string expr_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "??", true))
		{
			return;
		}
		this.DumpVar(expr_str, Vars.ReflectionMode.Enabled, false);
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x0009DA50 File Offset: 0x0009BC50
	[ConsoleMethod("?", "Dump selected object")]
	public void DumpSelected()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "?", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected object");
			return;
		}
		Debug.Log(Logic.Object.Dump(@object));
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x0009DA8B File Offset: 0x0009BC8B
	[ConsoleMethod("watch", "Show Watches Window")]
	public void ShowWatches()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "watch", true))
		{
			return;
		}
		WatchesWindow.Show();
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x0009DAA4 File Offset: 0x0009BCA4
	[ConsoleMethod("watch", "Add Watch")]
	public void AddWatch(string expr_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "watch", true))
		{
			return;
		}
		string expr_str2 = expr_str;
		if (expr_str2 == "clear")
		{
			WatchesWindow.Clear();
			return;
		}
		if (expr_str2 == "hide")
		{
			WatchesWindow.Hide();
			return;
		}
		if (expr_str2 == "show")
		{
			WatchesWindow.Show();
			return;
		}
		bool needs_context = true;
		if (expr_str == "game" || expr_str.StartsWith("game.", StringComparison.Ordinal))
		{
			needs_context = false;
		}
		else
		{
			Game game = GameLogic.Get(false);
			if (game != null)
			{
				Vars vars = game.vars;
				if (vars != null)
				{
					vars.EnumerateAll(delegate(string key, Value val)
					{
						if (!expr_str.StartsWith(key, StringComparison.Ordinal))
						{
							return;
						}
						if (expr_str.Length == key.Length)
						{
							needs_context = false;
							return;
						}
						char c = expr_str[key.Length];
						if (c == '.' || c == '(')
						{
							needs_context = false;
							return;
						}
					});
				}
			}
		}
		Logic.Object obj = null;
		if (needs_context)
		{
			obj = BaseUI.SelLO();
		}
		Expression expression = Expression.Parse(expr_str, true);
		if (expression == null || expression.type == Expression.Type.Invalid)
		{
			Debug.Log("invalid expression");
			return;
		}
		WatchesWindow.AddWatch(obj, expression);
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x0009DBA8 File Offset: 0x0009BDA8
	[ConsoleMethod("??", "Filter objects based on criteria")]
	public void Filter(string list_expr_str, string filter_expr_str)
	{
		DevCheats.<>c__DisplayClass120_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		Expression expression = Expression.Parse(list_expr_str, true);
		if (expression == null || expression.type == Expression.Type.Invalid)
		{
			Debug.LogError("Invalid expression: '" + list_expr_str + "'");
			return;
		}
		CS$<>8__locals1.filter_expr = Expression.Parse(filter_expr_str, true);
		if (CS$<>8__locals1.filter_expr == null || CS$<>8__locals1.filter_expr.type == Expression.Type.Invalid)
		{
			Debug.LogError("Invalid expression: '" + filter_expr_str + "'");
			return;
		}
		CS$<>8__locals1.txt = new StringBuilder();
		object obj_val = this.CalcExpr(expression, BaseUI.SelLO(), true, Vars.ReflectionMode.Enabled).obj_val;
		IList list;
		IDictionary dictionary;
		if ((list = (obj_val as IList)) != null)
		{
			int count = list.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				object o = list[i];
				if (this.<Filter>g__filter|120_0(o, false, ref CS$<>8__locals1))
				{
					num++;
				}
			}
			CS$<>8__locals1.txt.AppendLine(string.Format("{0}/{1}", num, count));
		}
		else if ((dictionary = (obj_val as IDictionary)) != null)
		{
			int num2 = 0;
			foreach (object obj in dictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (this.<Filter>g__filter|120_0(dictionaryEntry, false, ref CS$<>8__locals1))
				{
					num2++;
				}
			}
			CS$<>8__locals1.txt.AppendLine(string.Format("{0}/{1}", num2, dictionary.Count));
		}
		else
		{
			this.<Filter>g__filter|120_0(obj_val, true, ref CS$<>8__locals1);
		}
		string text = CS$<>8__locals1.txt.ToString();
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x0009DD68 File Offset: 0x0009BF68
	[ConsoleMethod("set", "set selected object var")]
	public void SetObjVar(string key, string expr_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "set", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected object");
			return;
		}
		Expression expression = Expression.Parse(expr_str, true);
		if (expression == null || expression.type == Expression.Type.Invalid)
		{
			Debug.Log("invalid expression");
			return;
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		DefsContext context = GameLogic.Get(true).dt.context;
		context.vars = new Vars(@object);
		Value value = expression.Calc(context, false);
		context.vars = null;
		Vars.PopReflectionMode(old_mode);
		@object.SetVar(key, value);
		Debug.Log(string.Format("set '{0}' to ({1}) for {2}", key, value, @object));
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x0009DE14 File Offset: 0x0009C014
	[ConsoleMethod("mod", "stat_name value - set debug stat modifier for selected object's stat")]
	public void SetDbgMod(string stat_name, float value)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "mod", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		string str;
		if (stat_name.StartsWith("ks_", StringComparison.Ordinal))
		{
			str = "kingdom";
			@object = (((@object != null) ? @object.GetKingdom() : null) ?? BaseUI.LogicKingdom());
		}
		else if (stat_name.StartsWith("rs_", StringComparison.Ordinal))
		{
			str = "realm";
			Logic.Settlement settlement = @object as Logic.Settlement;
			@object = ((settlement != null) ? settlement.GetRealm() : null);
		}
		else
		{
			if (!stat_name.StartsWith("cs_", StringComparison.Ordinal))
			{
				Debug.LogError("Unknown stat: " + stat_name);
				return;
			}
			str = "character";
			Logic.Settlement settlement2 = @object as Logic.Settlement;
			Logic.Object object2;
			if (settlement2 == null)
			{
				object2 = null;
			}
			else
			{
				Logic.Realm realm = settlement2.GetRealm();
				object2 = ((realm != null) ? realm.castle : null);
			}
			@object = object2;
			if (@object == null)
			{
				@object = BaseUI.SelChar();
			}
		}
		if (@object == null)
		{
			Debug.LogError("No selected " + str);
		}
		Stats stats = @object.GetStats();
		Stat stat = (stats != null) ? stats.Find(stat_name, false, false) : null;
		if (stat == null)
		{
			Debug.LogError(string.Format("{0} has no stat named '{1}'", @object, stat_name));
			return;
		}
		if (stat.all_mods != null)
		{
			for (int i = 0; i < stat.all_mods.Count; i++)
			{
				Stat.Modifier modifier = stat.all_mods[i];
				if (modifier is DevCheats.DbgStatModifier)
				{
					stat.DelModifier(modifier, true);
					break;
				}
			}
		}
		stat.AddModifier(new DevCheats.DbgStatModifier
		{
			value = value
		}, false);
		Debug.Log(string.Format("Set {0}.{1} debug modifier to {2}. Final stat value: {3}", new object[]
		{
			@object,
			stat_name,
			value,
			stat.CalcValue(false)
		}));
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x0009DFA8 File Offset: 0x0009C1A8
	public Logic.Object ExprToLO(string expr_str)
	{
		Expression expression = Expression.Parse(expr_str, true);
		if (expression == null)
		{
			return null;
		}
		DefsContext context = GameLogic.Get(true).dt.context;
		Logic.Object @object = BaseUI.SelLO();
		if (@object != null)
		{
			context.vars = new Vars(@object);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		Value value = expression.Calc(context, false);
		Vars.PopReflectionMode(old_mode);
		context.vars = null;
		Logic.Object object2 = value.Get<Logic.Object>();
		if (object2 == null)
		{
			object2 = this.FindRealm(expr_str);
		}
		if (object2 == null)
		{
			object2 = this.FindKingdom(expr_str, false);
		}
		return object2;
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x0009E030 File Offset: 0x0009C230
	public void DumpVarTest(string expr_str, Vars.ReflectionMode reflection_mode)
	{
		if (expr_str == "null" | expr_str == "none")
		{
			BaseUI.Get().SelectObj(null, false, true, true, true);
			return;
		}
		Expression expression = Expression.Parse(expr_str, true);
		if (expression == null)
		{
			Debug.Log("invalid expression");
			return;
		}
		DefsContext context = GameLogic.Get(true).dt.context;
		Logic.Object @object = BaseUI.SelLO();
		if (@object != null)
		{
			context.vars = new Vars(@object);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(reflection_mode);
		Value value = expression.Calc(context, false);
		Vars.PopReflectionMode(old_mode);
		context.vars = null;
		Logic.Object object2 = value.Get<Logic.Object>();
		if (object2 == null)
		{
			object2 = this.FindRealm(expr_str);
		}
		if (object2 == null)
		{
			object2 = this.FindKingdom(expr_str, false);
		}
		if (object2 == null)
		{
			object2 = this.FindCastle(expr_str);
		}
		if (object2 == null)
		{
			Debug.Log("Null logic object");
			return;
		}
		BaseUI.Get().SelectObjFromLogic(object2, false, true);
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x0009E110 File Offset: 0x0009C310
	[ConsoleMethod("sel", "Calc expression (allowing reflection), with null logic/visual warnings")]
	public void DumpVarReflTest(string expr_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "sel", true))
		{
			return;
		}
		this.DumpVarTest(expr_str, Vars.ReflectionMode.Enabled);
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x0009E12C File Offset: 0x0009C32C
	[ConsoleMethod("sel_nid", "<object_type> <nid> - select object by nid")]
	public void DumpVarReflTest(string obj_type, string nid_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "sel_nid", true))
		{
			return;
		}
		Serialization.ObjectTypeInfo objectTypeInfo = Serialization.ObjectTypeInfo.Get(obj_type);
		if (objectTypeInfo == null)
		{
			Debug.LogError("Unknow object type");
			return;
		}
		NID nid = NID.FromString(nid_str, objectTypeInfo, -1);
		Logic.Object obj = nid.GetObj(GameLogic.Get(true));
		if (obj == null)
		{
			Debug.Log(string.Format("Unknown {0}", nid));
			return;
		}
		Debug.Log(string.Format("Selected {0}", obj));
		BaseUI.Get().SelectObjFromLogic(obj, false, true);
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x0009E1AC File Offset: 0x0009C3AC
	[ConsoleMethod("dump_fam", "info about royal families")]
	public void DumpRoyalFamiliesInfo()
	{
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (game.kingdoms.Count <= 0)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		float num8 = 0f;
		float num9 = 0f;
		int[] array = new int[7];
		int[] array2 = new int[7];
		List<Logic.Kingdom> kingdoms = game.kingdoms;
		short num10 = 0;
		while ((int)num10 < kingdoms.Count)
		{
			if (!kingdoms[(int)num10].IsDefeated() && kingdoms[(int)num10].IsRegular())
			{
				num++;
				if (kingdoms[(int)num10].royalFamily.Spouse != null)
				{
					num2++;
				}
				if (kingdoms[(int)num10].royalFamily.Children.Count > 0)
				{
					short num11 = 0;
					while ((int)num11 < kingdoms[(int)num10].royalFamily.Children.Count)
					{
						if (kingdoms[(int)num10].royalFamily.Children[(int)num11].IsPrince())
						{
							num5++;
							Logic.Character.Age age = kingdoms[(int)num10].royalFamily.Children[(int)num11].age;
							num9 += (float)Logic.Character.CalculateAgeInSeconds(age, game);
							switch (age)
							{
							case Logic.Character.Age.Infant:
								array2[0]++;
								break;
							case Logic.Character.Age.Child:
								array2[1]++;
								break;
							case Logic.Character.Age.Juvenile:
								array2[2]++;
								break;
							case Logic.Character.Age.Young:
								array2[3]++;
								break;
							case Logic.Character.Age.Adult:
								array2[4]++;
								break;
							case Logic.Character.Age.Old:
								array2[5]++;
								break;
							case Logic.Character.Age.Venerable:
								array2[6]++;
								break;
							}
						}
						if (kingdoms[(int)num10].royalFamily.Children[(int)num11].sex == Logic.Character.Sex.Male)
						{
							num3++;
						}
						else if (kingdoms[(int)num10].royalFamily.Children[(int)num11].sex == Logic.Character.Sex.Female)
						{
							num4++;
						}
						num11 += 1;
					}
				}
				else
				{
					num6++;
				}
				num7 += kingdoms[(int)num10].royalFamily.Relatives.Count;
				Logic.Character.Age age2 = kingdoms[(int)num10].GetKing().age;
				num8 += (float)Logic.Character.CalculateAgeInSeconds(age2, game);
				switch (age2)
				{
				case Logic.Character.Age.Infant:
					array[0]++;
					break;
				case Logic.Character.Age.Child:
					array[1]++;
					break;
				case Logic.Character.Age.Juvenile:
					array[2]++;
					break;
				case Logic.Character.Age.Young:
					array[3]++;
					break;
				case Logic.Character.Age.Adult:
					array[4]++;
					break;
				case Logic.Character.Age.Old:
					array[5]++;
					break;
				case Logic.Character.Age.Venerable:
					array[6]++;
					break;
				}
			}
			num10 += 1;
		}
		Debug.Log(string.Concat(new object[]
		{
			"\nKingdoms: ",
			num,
			"\nwith children: ",
			num - num6,
			"\nwithout: ",
			num6,
			"\nMarriages: ",
			num2,
			"\nPrinces: ",
			num5,
			"\n\nChildren total: ",
			num4 + num3,
			"\nMale: ",
			num3,
			"\nFemale: ",
			num4,
			"\nM / F ratio: ",
			(float)num3 / (float)num4,
			"\naverage: ",
			(float)(num3 + num4) / (float)num,
			"\n\nKing age average: ",
			num8 / (float)num / 60f,
			" minutes\nPrince age average: ",
			num9 / (float)num5 / 60f,
			" minutes\n\nInfant \tKings: ",
			array[0],
			"\tPrinces: ",
			array2[0],
			"\nChild \tKings: ",
			array[1],
			"\tPrinces: ",
			array2[1],
			"\nJuvenile \tKings: ",
			array[2],
			"\tPrinces: ",
			array2[2],
			"\nYoung \tKings: ",
			array[3],
			"\tPrinces: ",
			array2[3],
			"\nAdult \tKings: ",
			array[4],
			"\tPrinces: ",
			array2[4],
			"\nOld \tKings: ",
			array[5],
			"\tPrinces: ",
			array2[5],
			"\nVenerable \tKings: ",
			array[6],
			"\tPrinces: ",
			array2[6]
		}));
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x0009E70C File Offset: 0x0009C90C
	[ConsoleMethod("dump_trade", "trade overview")]
	public void DumpTradeInfo()
	{
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (game.kingdoms.Count <= 0)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		List<Logic.Kingdom> kingdoms = game.kingdoms;
		for (int i = 0; i < kingdoms.Count; i++)
		{
			if (!kingdoms[i].IsDefeated() && kingdoms[i].IsRegular())
			{
				num++;
				num2 += kingdoms[i].GetMerchantsCount();
				num3 += kingdoms[i].GetTradingMerchantsCount();
				num4 += kingdoms[i].tradeAgreementsWith.Count;
			}
		}
		Debug.Log(string.Concat(new object[]
		{
			"\nMerchants: ",
			num2,
			"\nAverage Merchants: ",
			(float)num2 / (float)num,
			"\n\nTrading Merchants: ",
			num3,
			"\nAverage Trading Merchants: ",
			(float)num3 / (float)num,
			"\n\nTrade Agreements: ",
			num4,
			"\nAverage Trade Agreements: ",
			(float)num4 / (float)num
		}));
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x0009E840 File Offset: 0x0009CA40
	[ConsoleMethod("bv", "Benchmark get variable (no reflection)")]
	public void BenchmarkDumpVar(string expr_str, int reps)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "bv", true))
		{
			return;
		}
		bool as_value = true;
		Expression expression = Expression.Parse(expr_str, false);
		if (expression == null)
		{
			Debug.Log("invalid expression");
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		DefsContext context = GameLogic.Get(true).dt.context;
		if (@object != null)
		{
			context.vars = new Vars(@object);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Disabled);
		bool compile_enabled = Expression.compile_enabled;
		Expression.compile_enabled = false;
		Value value = Value.Unknown;
		long num = GC.GetTotalMemory(false);
		long num2 = Value.boxed;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int i = 0; i < reps; i++)
		{
			value = expression.Calc(context, as_value);
			if (stopwatch.ElapsedMilliseconds >= 5000L)
			{
				reps = i + 1;
				break;
			}
		}
		stopwatch.Stop();
		float num3 = (float)(stopwatch.ElapsedMilliseconds * 1000L) / (float)reps;
		num = GC.GetTotalMemory(false) - num;
		num2 = Value.boxed - num2;
		string text = value.ToString();
		if (text == null)
		{
			text = "null";
		}
		string text2 = string.Concat(new object[]
		{
			num3,
			"us, boxed: ",
			(float)num2 / (float)reps,
			", mem: ",
			(float)num / (float)reps,
			", result: ",
			text
		});
		Expression.compile_enabled = true;
		if (expression.Compile())
		{
			num = GC.GetTotalMemory(false);
			num2 = Value.boxed;
			stopwatch.Restart();
			for (int j = 0; j < reps; j++)
			{
				value = expression.Calc(context, as_value);
			}
			stopwatch.Stop();
			num3 = (float)(stopwatch.ElapsedMilliseconds * 1000L) / (float)reps;
			num = GC.GetTotalMemory(false) - num;
			num2 = Value.boxed - num2;
			text = value.ToString();
			if (text == null)
			{
				text = "null";
			}
			text2 = string.Concat(new object[]
			{
				text2,
				"\nCompiled: ",
				num3,
				"us, boxed: ",
				(float)num2 / (float)reps,
				", mem: ",
				(float)num / (float)reps,
				", result: ",
				text
			});
		}
		else
		{
			text2 = text2 + "\nCould not compile '" + expr_str + "'";
		}
		Debug.Log(text2);
		Expression.compile_enabled = compile_enabled;
		Vars.PopReflectionMode(old_mode);
		context.vars = null;
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x0009EABE File Offset: 0x0009CCBE
	[ConsoleMethod("bv", "Benchmark get variable (no reflection)")]
	public void BenchmarkDumpVar(string expr_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "bv", true))
		{
			return;
		}
		this.BenchmarkDumpVar(expr_str, 100000);
	}

	// Token: 0x06000E4F RID: 3663 RVA: 0x0009EADC File Offset: 0x0009CCDC
	[ConsoleMethod("brv", "Benchmark get variable (reflection allowed)")]
	public void BenchmarkReflectedVar(string expr_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "brv", true))
		{
			return;
		}
		Expression expression = Expression.Parse(expr_str, true);
		if (expression == null)
		{
			Debug.Log("invalid expression");
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		DefsContext context = GameLogic.Get(true).dt.context;
		if (@object != null)
		{
			context.vars = new Vars(@object);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		Value value = Value.Unknown;
		long num = GC.GetTotalMemory(false);
		long num2 = Value.boxed;
		int num3 = 100000;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int i = 0; i < num3; i++)
		{
			value = expression.Calc(context, false);
		}
		stopwatch.Stop();
		float num4 = (float)stopwatch.ElapsedMilliseconds / (float)num3;
		num = GC.GetTotalMemory(false) - num;
		num2 = Value.boxed - num2;
		Vars.PopReflectionMode(old_mode);
		context.vars = null;
		string text = Logic.Object.Dump(value);
		if (text == null)
		{
			text = "null";
		}
		Debug.Log(string.Concat(new object[]
		{
			text,
			", ",
			num4,
			"ms, boxed: ",
			(float)num2 / (float)num3,
			", mem: ",
			(float)num / (float)num3
		}));
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x0009EC2B File Offset: 0x0009CE2B
	[ConsoleMethod("ratf", "Resolve All Text Fields")]
	public void ResolveAllTextFields()
	{
		global::Defs.Get(false).ResolveTextFields();
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x0009EC38 File Offset: 0x0009CE38
	[ConsoleMethod("rti", "Resolve Text Field")]
	public void ResolveTextField(string text_key)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rti", true))
		{
			return;
		}
		global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(text_key, false, true);
		if (textInfo == null)
		{
			Debug.LogError("Text key '" + text_key + "' not found");
			return;
		}
		Logic.Object vars = BaseUI.SelLO();
		textInfo.GetTextField(vars, true);
		Debug.Log(textInfo.Dump());
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x0009EC90 File Offset: 0x0009CE90
	[ConsoleMethod("ti", "Dump Text Info")]
	public void DumpTextInfo(string text_key)
	{
		global::Defs.Get(false);
		global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(text_key, false, true);
		if (textInfo == null)
		{
			Debug.LogError("Text key '" + text_key + "' not found");
			return;
		}
		Debug.Log(textInfo.Dump());
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x0009ECD4 File Offset: 0x0009CED4
	[ConsoleMethod("ln", "Get localized name")]
	public void DumpName()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ln", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected objct");
			return;
		}
		Debug.Log(global::Defs.LocalizedObjName(@object, null, "", true));
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x0009ED18 File Offset: 0x0009CF18
	[ConsoleMethod("lt", "Get localized text")]
	public void GetLocalizedText(string text)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "lt", true))
		{
			return;
		}
		Vars vars = new Vars(BaseUI.SelLO());
		DT.Field defField = global::Defs.GetDefField("TestLocalizationMessage", null);
		DT.Field field = (defField != null) ? defField.FindChild("context", null, true, true, true, '.') : null;
		if (((field != null) ? field.children : null) != null)
		{
			Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
			for (int i = 0; i < field.children.Count; i++)
			{
				DT.Field field2 = field.children[i];
				if (!string.IsNullOrEmpty(field2.key))
				{
					Value val = field2.Value(vars, true, true);
					vars.Set<Value>(field2.key, val);
				}
			}
			Vars.PopReflectionMode(old_mode);
		}
		string text2 = global::Defs.GetLocalized(text, vars);
		if (text2 == null)
		{
			text2 = "<null>";
		}
		if (defField == null)
		{
			Debug.Log(text2);
			return;
		}
		vars.Set<string>("org_text", "#" + text);
		vars.Set<string>("localized_text", "#" + text2);
		MessageWnd messageWnd = MessageWnd.Create(defField, vars, null, null);
		Tooltip tooltip = Tooltip.Get(global::Common.FindChildByName((messageWnd != null) ? messageWnd.gameObject : null, "id_Caption", true, true), true);
		if (tooltip != null)
		{
			tooltip.SetText("#" + vars.Dump(), "#Context:", null);
		}
		Debug.Log(string.Concat(new string[]
		{
			"'",
			text,
			"' -> '",
			text2,
			"'"
		}));
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x0009EE98 File Offset: 0x0009D098
	[ConsoleMethod("lv", "Get localized variable")]
	public void DumpLocalizedVar(string path)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "lv", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		string str = (@object == null) ? "" : (@object.ToString() + ".");
		str = str + path + " = ";
		string text = global::Defs.ReplaceVars("{" + path + "}", new Vars(@object), true, '\0');
		if (text == null)
		{
			Debug.Log(str + "null");
			return;
		}
		Debug.Log(str + text);
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x0009EF28 File Offset: 0x0009D128
	[ConsoleMethod("drv", "Debug ReplaceVar")]
	public void DebugReplaceVar(string key)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "drv", true))
		{
			return;
		}
		if (key == "null")
		{
			global::Defs.debug_replace_var = null;
			Debug.Log("Not debugging ReplaceVar");
			return;
		}
		global::Defs.debug_replace_var = key;
		Debug.Log("Debugging ReplaceVar(\"" + key + "\")");
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x0009EF80 File Offset: 0x0009D180
	[ConsoleMethod("blv", "Benchmark localized variable")]
	public void BemchmarkLocalizedVar(string path)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "blv", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		string text = (@object == null) ? "" : (@object.ToString() + ".");
		text = text + path + " = ";
		Vars vars = new Vars(@object);
		string text2 = "{" + path + "}";
		string text3 = null;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int i = 0; i < 10000; i++)
		{
			text3 = global::Defs.ReplaceVars(text2, vars, true, '\0');
		}
		stopwatch.Stop();
		float num = (float)stopwatch.ElapsedMilliseconds / 10000f;
		if (text3 == null)
		{
			text += "null";
		}
		else
		{
			text += text3;
		}
		Debug.Log(string.Concat(new object[]
		{
			text,
			", ",
			num,
			"ms"
		}));
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x0009F078 File Offset: 0x0009D278
	[ConsoleMethod("bln", "Benchmark get localized name")]
	public void BenchmarkDumpName()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "bln", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected objct");
			return;
		}
		string text = null;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int i = 0; i < 10000; i++)
		{
			text = global::Defs.LocalizedObjName(@object, null, "", true);
		}
		stopwatch.Stop();
		float num = (float)stopwatch.ElapsedMilliseconds / 10000f;
		string text2;
		if (text == null)
		{
			text2 = "null";
		}
		else
		{
			text2 = text;
		}
		Debug.Log(string.Concat(new object[]
		{
			text2,
			", ",
			num,
			"ms"
		}));
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x0009F12C File Offset: 0x0009D32C
	[ConsoleMethod("ln", "Get localized name")]
	public void DumpName(string path)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ln", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected objct");
			return;
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		Value value = Vars.Get(@object, path, null, true);
		Vars.PopReflectionMode(old_mode);
		if (!value.is_valid)
		{
			Debug.Log(value.ToString());
			return;
		}
		Debug.Log(global::Defs.LocalizedObjName(value, null, "", true));
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x0009F1A4 File Offset: 0x0009D3A4
	[ConsoleMethod("pcvl", "Play Character Voice Line")]
	public void PlayCharacterVoiceLine(string event_name)
	{
		Logic.Character character = (BaseUI.TTObj() as Logic.Character) ?? BaseUI.SelChar();
		if (character == null)
		{
			return;
		}
		BaseUI.PlayVoiceEvent(event_name, character);
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x0009F1D0 File Offset: 0x0009D3D0
	[ConsoleMethod("pcvl", "Play Character 'greet' Voice Line")]
	public void PlayCharacterVoiceLine()
	{
		Logic.Character character = (BaseUI.TTObj() as Logic.Character) ?? BaseUI.SelChar();
		if (character == null)
		{
			return;
		}
		BaseUI.PlayVoiceEvent("greet", character);
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x0009F200 File Offset: 0x0009D400
	[ConsoleMethod("pnvl", "Play Narrator Voice Line")]
	public void PlayNarratorVoiceLine(string event_name)
	{
		BaseUI.PlayVoiceEvent(event_name, null);
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x0009F209 File Offset: 0x0009D409
	[ConsoleMethod("pnvl", "Play Narrators 'greetings' Voice Line")]
	public void PlayNarratorVoiceLine()
	{
		BaseUI.PlayVoiceEvent("narrator_voice:greetings", null);
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0009F218 File Offset: 0x0009D418
	[ConsoleMethod("puvl", "Play Unit Voice Line")]
	public void PlayUnitVoiceLine(string event_name)
	{
		Logic.Unit unit = BaseUI.TTObj() as Logic.Unit;
		if (unit == null)
		{
			return;
		}
		BaseUI.PlayVoiceEvent(event_name, unit);
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x0009F23C File Offset: 0x0009D43C
	[ConsoleMethod("puvl", "Play Unit 'select' Voice Line")]
	public void PlayUnitVoiceLine()
	{
		Logic.Unit unit = BaseUI.TTObj() as Logic.Unit;
		if (unit == null)
		{
			return;
		}
		BaseUI.PlayVoiceEvent("select", unit);
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x0009F264 File Offset: 0x0009D464
	[ConsoleMethod("sub", "Make selected kingdom subordinated (1) or independent(0)")]
	public void Subordinate(int subordinate)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sub", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		if (!kingdom.is_orthodox)
		{
			Debug.Log(kingdom.Name + " is " + kingdom.religion.name);
			return;
		}
		kingdom.game.religions.orthodox.SetSubordinated(kingdom, subordinate != 0, null, true);
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x0009F2DC File Offset: 0x0009D4DC
	[ConsoleMethod("cal", "Make selected kingdom caliphate (1) or not(0)")]
	public void Caliphate(int caliphate)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "cal", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		if (!kingdom.is_muslim)
		{
			Debug.Log(kingdom.Name + " is " + kingdom.religion.name);
			return;
		}
		kingdom.caliphate = (caliphate != 0);
		kingdom.NotifyListeners("religion_changed", null);
		if (caliphate != 0)
		{
			kingdom.game.religions.FireEvent("caliphate_claimed", kingdom, Array.Empty<int>());
			return;
		}
		kingdom.game.religions.FireEvent("caliphate_abandoned", kingdom, Array.Empty<int>());
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x0009F388 File Offset: 0x0009D588
	[ConsoleMethod("cal", "Make typed kingdom caliphate (1) or not(0)")]
	public void Caliphate(string kName, int caliphate)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "cal", true))
		{
			return;
		}
		Logic.Kingdom kingdom = GameLogic.Get(true).GetKingdom(kName);
		if (kingdom == null)
		{
			Debug.Log("No such kingdom: " + kName);
			return;
		}
		if (!kingdom.is_muslim)
		{
			Debug.Log(kingdom.Name + " is " + kingdom.religion.name);
			return;
		}
		kingdom.caliphate = (caliphate != 0);
		Religion.RefreshModifiers(kingdom);
		kingdom.NotifyListeners("religion_changed", null);
		if (caliphate != 0)
		{
			kingdom.game.religions.FireEvent("caliphate_claimed", kingdom, Array.Empty<int>());
			return;
		}
		kingdom.game.religions.FireEvent("caliphate_abandoned", kingdom, Array.Empty<int>());
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x0009F444 File Offset: 0x0009D644
	[ConsoleMethod("jihad", "Start jihad")]
	public void StartJihadBetween()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "jihad", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		if (kingdom2 == null)
		{
			Debug.Log("No logic kingdom");
			return;
		}
		this.StartJihadBetween(kingdom2.Name, kingdom.Name);
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x0009F49C File Offset: 0x0009D69C
	[ConsoleMethod("jihad", "Start jihad")]
	public void StartJihadBetween(string kName)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "jihad", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No logic kingdom");
			return;
		}
		this.StartJihadBetween(kingdom.Name, kName);
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x0009F4DC File Offset: 0x0009D6DC
	[ConsoleMethod("jihad", "Start jihad")]
	public void StartJihadBetween(string kName1, string kName2)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "jihad", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kName1, false);
		if (kingdom == null)
		{
			Debug.Log(kName1 + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom2 = this.FindKingdom(kName2, false);
		if (kingdom2 == null)
		{
			Debug.Log(kName2 + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom3;
		Logic.Kingdom kingdom4;
		if (kingdom.is_muslim && !kingdom2.is_muslim)
		{
			kingdom3 = kingdom;
			kingdom4 = kingdom2;
		}
		else
		{
			if (!kingdom2.is_muslim || kingdom.is_muslim)
			{
				Debug.Log(string.Concat(new string[]
				{
					"the kingdoms must be muslim and non-muslim: ",
					kingdom.Name,
					"-",
					kingdom.religion.name,
					" ",
					kingdom2.Name,
					"-",
					kingdom2.religion.name
				}));
				return;
			}
			kingdom3 = kingdom2;
			kingdom4 = kingdom;
		}
		War war = kingdom3.FindWarWith(kingdom4);
		if (war != null && !war.IsLeader(kingdom3))
		{
			Debug.Log(kingdom3.Name + " is not a leader in the war against " + kingdom4.Name);
			return;
		}
		if (!kingdom3.IsCaliphate())
		{
			this.Caliphate(kingdom3.Name, 1);
		}
		if (war == null)
		{
			kingdom3.StartWarWith(kingdom4, War.InvolvementReason.InternalPurposes, "WarDeclaredMessage", null, true);
		}
		War.StartJihad(kingdom3, kingdom4);
		Debug.Log("Jihad Started: " + kingdom3.Name + " against " + kingdom4.Name);
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x0009F648 File Offset: 0x0009D848
	[ConsoleMethod("cr", "Change selected kingdom's religion")]
	public void ChangeReligion(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "cr", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Logic.Defs.Registry registry = kingdom.game.defs.Get(typeof(Religion.Def));
		Religion.Def def = null;
		foreach (KeyValuePair<string, Def> keyValuePair in registry.defs)
		{
			Religion.Def def2 = keyValuePair.Value as Religion.Def;
			if (def2 != null && def2.name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				def = def2;
				break;
			}
		}
		if (def == null)
		{
			Debug.Log("Unknown religion");
			return;
		}
		Religion religion = kingdom.game.religions.Get(def);
		if (religion == null)
		{
			Debug.Log("Unknown religion");
			return;
		}
		if (kingdom.religion == religion)
		{
			Debug.Log(kingdom.Name + " is already " + religion.name);
			return;
		}
		kingdom.ChangeReligion(religion);
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x0009F754 File Offset: 0x0009D954
	[ConsoleMethod("crr", "Change selected realm's religion")]
	public void ChangeRealmReligion(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "crr", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		Logic.Defs.Registry registry = realm.game.defs.Get(typeof(Religion.Def));
		Religion.Def def = null;
		foreach (KeyValuePair<string, Def> keyValuePair in registry.defs)
		{
			Religion.Def def2 = keyValuePair.Value as Religion.Def;
			if (def2 != null && def2.name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				def = def2;
				break;
			}
		}
		if (def == null)
		{
			Debug.Log("Unknown religion");
			return;
		}
		Religion religion = realm.game.religions.Get(def);
		if (religion == null)
		{
			Debug.Log("Unknown religion");
			return;
		}
		if (realm.religion == religion)
		{
			Debug.Log(realm.name + " is already " + religion.name);
			return;
		}
		realm.SetReligion(religion, true);
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x0009F874 File Offset: 0x0009DA74
	[ConsoleMethod("br", "Benchmark relations")]
	public void BenchmarkRelations()
	{
		Game.CheckCheatLevel(Game.CheatLevel.Low, "br", true);
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x0009F884 File Offset: 0x0009DA84
	[ConsoleMethod("pop", "Dump selected realm's population majority")]
	public void DumpPopMajority()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pop", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		if (settlement == null)
		{
			Debug.Log("No selected settlement");
			return;
		}
		Logic.Realm realm = settlement.GetRealm();
		if (realm == null)
		{
			Debug.Log(settlement.ToString() + " has no realm");
			return;
		}
		Logic.Kingdom kingdom = realm.GetKingdom();
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		string format = "{0}, {1}, culture: {2}\nMajority: {3} ({4})\n";
		object[] array = new object[5];
		array[0] = realm;
		array[1] = kingdom;
		array[2] = ((kingdom != null) ? kingdom.culture : null);
		array[3] = realm.pop_majority;
		int num = 4;
		Logic.Kingdom kingdom2 = realm.pop_majority.kingdom;
		array[num] = ((kingdom2 != null) ? kingdom2.culture : null);
		stringBuilder2.AppendLine(string.Format(format, array));
		realm.CalcPopInf(stringBuilder);
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x0009F950 File Offset: 0x0009DB50
	[ConsoleMethod("add_pop", "Add given population influence in selected realm for the given kingdom")]
	public void AddPopMajority(int amount, string kingdom_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_pop", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		if (settlement == null)
		{
			Debug.Log("No selected settlement");
			return;
		}
		Logic.Realm realm = settlement.GetRealm();
		if (realm == null)
		{
			Debug.Log(settlement.ToString() + " has no realm");
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kingdom_name, false);
		if (kingdom == null)
		{
			Debug.Log("Unknown kingdom");
			return;
		}
		if (kingdom != realm.pop_majority.kingdom)
		{
			amount = -amount;
		}
		else
		{
			kingdom = null;
		}
		realm.AdjustPopMajority((float)amount, kingdom);
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		string format = "{0}: kingdom {1}, culture: {2}\nMajority: {3} ({4})";
		object[] array = new object[5];
		array[0] = realm;
		array[1] = kingdom;
		array[2] = ((kingdom != null) ? kingdom.culture : null);
		array[3] = realm.pop_majority;
		int num = 4;
		Logic.Kingdom kingdom2 = realm.pop_majority.kingdom;
		array[num] = ((kingdom2 != null) ? kingdom2.culture : null);
		stringBuilder2.AppendLine(string.Format(format, array));
		realm.CalcPopInf(stringBuilder);
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x0009FA4C File Offset: 0x0009DC4C
	[ConsoleMethod("spawn_pop", "Spawn population in the selected realm")]
	public void SpawnPopulation(int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "spawn_pop", true))
		{
			return;
		}
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.Log("No selected castle");
			return;
		}
		castle.population.AddVillagers(amount, Population.Type.Worker, true);
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x0009FA90 File Offset: 0x0009DC90
	private void DumpTimers(Logic.Object obj)
	{
		Debug.Log("Timers of " + obj.ToString() + ":");
		if (obj.components == null)
		{
			return;
		}
		for (int i = 0; i < obj.components.Count; i++)
		{
			Timer timer = obj.components[i] as Timer;
			if (timer != null)
			{
				Debug.Log(timer.ToString());
			}
		}
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x0009FAF8 File Offset: 0x0009DCF8
	[ConsoleMethod("tmrs", "Dump selected object's timers")]
	public void DumpTimers()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tmrs", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("no selected object");
			return;
		}
		this.DumpTimers(@object);
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x0009FB30 File Offset: 0x0009DD30
	[ConsoleMethod("tmrs", "Dump object's timers")]
	public void DumpTimers(string expr)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tmrs", true))
		{
			return;
		}
		Logic.Object @object = this.ExprToLO(expr);
		if (@object == null)
		{
			Debug.Log("Could not resolve '" + expr + "' to object");
			return;
		}
		this.DumpTimers(@object);
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x0009FB74 File Offset: 0x0009DD74
	[ConsoleMethod("tmr", "Start a timer")]
	public void StartTimer(string name, float duration)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tmr", true))
		{
			return;
		}
		DevCheats.TimerLogger.Get();
		Debug.Log(GameLogic.Get(true).StartTimer(name, duration, false).ToString());
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x0009FBA3 File Offset: 0x0009DDA3
	[ConsoleMethod("rtmr", "Start a timer (force restart)")]
	public void RestartTimer(string name, float duration)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rtmr", true))
		{
			return;
		}
		DevCheats.TimerLogger.Get();
		Debug.Log(GameLogic.Get(true).StartTimer(name, duration, true).ToString());
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x0009FBD2 File Offset: 0x0009DDD2
	[ConsoleMethod("upds", "Dump updates per type")]
	public void DumpUpdatesPerType()
	{
		this.DumpUpdatesPerType(null);
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x0009FBDB File Offset: 0x0009DDDB
	[ConsoleMethod("upds", "Start / Dump updates per type")]
	public void DumpUpdatesPerType(string cmd)
	{
		GameLogic.Get(false).FireEvent("updates_per_type", cmd, Array.Empty<int>());
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x0009FBF3 File Offset: 0x0009DDF3
	[ConsoleMethod("dmi", "Delete message icons")]
	public void DeleteMessageIcons()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dmi", true))
		{
			return;
		}
		MessageIcon.DeleteAll();
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x0009FC09 File Offset: 0x0009DE09
	[ConsoleMethod("rmi", "Recreate message icons")]
	public void RecreateMessageIcons()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rmi", true))
		{
			return;
		}
		MessageIcon.DeleteAll();
		MessageIcon.RecreateAll();
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x0009FC24 File Offset: 0x0009DE24
	[ConsoleMethod("tmi", "Test message icon")]
	public void TestMessageIcon()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rmi", true))
		{
			return;
		}
		Vars vars = new Vars();
		Building.Def val = Def.Get<Building.Def>(global::Defs.GetDefField("FishMarket", null));
		DT.Field defField = global::Defs.GetDefField("Salt", null);
		vars.Set<Building.Def>("tst_building", val);
		vars.Set<DT.Field>("tst_resource", defField);
		vars.Set<Castle>("castle", BaseUI.SelLO() as Castle);
		vars.Set<Logic.Kingdom>("kingdom", BaseUI.SelKingdom() ?? BaseUI.LogicKingdom());
		MessageIcon.Create("TestDefLinksMessage", vars, true, null);
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x0009FCB7 File Offset: 0x0009DEB7
	[ConsoleMethod("pintt", "Pin current tooltip")]
	public void PinTooltip()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pintt", true))
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return;
		}
		baseUI.PinTooltip();
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0009FCD8 File Offset: 0x0009DED8
	[ConsoleMethod("rhttt", "Refresh hypertext tooltip")]
	public void RefreshHyperTextTooltip()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rhttt", true))
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		UIHyperText uihyperText;
		if (baseUI == null)
		{
			uihyperText = null;
		}
		else
		{
			Tooltip tooltip = baseUI.tooltip;
			if (tooltip == null)
			{
				uihyperText = null;
			}
			else
			{
				GameObject gameObject = tooltip.instance;
				uihyperText = ((gameObject != null) ? gameObject.GetComponent<UIHyperText>() : null);
			}
		}
		UIHyperText uihyperText2 = uihyperText;
		if (uihyperText2 == null)
		{
			Debug.Log("No hypertext tooltip");
			return;
		}
		uihyperText2.Refresh();
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0009FD38 File Offset: 0x0009DF38
	[ConsoleMethod("tt", "Dump object tooltip")]
	public void DumpTooltip(string path)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tt", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected objct");
			return;
		}
		object obj_val = Vars.Get(@object, path, null, false).obj_val;
		DT.Def def = Tooltip.GetDef(obj_val, "");
		if (def == null)
		{
			Debug.Log("null");
			return;
		}
		Vars vars = new Vars(obj_val);
		string str = global::Defs.Localize(def.field, "caption", vars, null, false, true) ?? "";
		string str2 = global::Defs.Localize(def.field, "text", vars, null, false, true) ?? "";
		Debug.Log("Caption: '" + str + "'");
		Debug.Log("Text: '" + str2 + "'");
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0009FE02 File Offset: 0x0009E002
	[ConsoleMethod("st", "Show Tutorial")]
	public void ShowTutorial()
	{
		this.ShowTutorialTopic("welcome");
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0009FE0F File Offset: 0x0009E00F
	[ConsoleMethod("stt", "Show Tutorial Topic")]
	public void ShowTutorialTopic(string id)
	{
		Tutorial.ShowTopic(id, 2);
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0009FE19 File Offset: 0x0009E019
	[ConsoleMethod("stm", "Show Tutorial Message")]
	public void ShowTutorialMessage(string id)
	{
		Tutorial.ShowMessage(id);
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0009FE22 File Offset: 0x0009E022
	[ConsoleMethod("rtt", "Reset Tutorial Topics")]
	public void ShowTutorialTopic()
	{
		Tutorial.Reset();
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0009FE2C File Offset: 0x0009E02C
	[ConsoleMethod("recall", "Recall selected character to court")]
	public void Recall()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "recall", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		character.Recall(true, false, true, true, true);
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0009FE68 File Offset: 0x0009E068
	[ConsoleMethod("imprison", "Imprison selected character in our kingdom")]
	public void Imprison()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "imprison", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		character.Imprison(kingdom, true, true, "dev_cheat", true);
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0009FEB4 File Offset: 0x0009E0B4
	[ConsoleMethod("imprison", "Imprison selected character in specified kingdom")]
	public void Imprison(string kingdom_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "imprison", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		Logic.Kingdom kingdom = character.game.GetKingdom(kingdom_name);
		if (kingdom == null)
		{
			Debug.Log("Unknown kingdom: " + kingdom_name);
			return;
		}
		character.Imprison(kingdom, true, true, "dev_cheat", true);
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0009FF14 File Offset: 0x0009E114
	[ConsoleMethod("unprison", "Free the selected character from prison")]
	public void Unprison()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "unprison", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		if (character.prison_kingdom == null)
		{
			Debug.Log(character.ToString() + " is not in prison");
			return;
		}
		character.Imprison(null, true, true, null, true);
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0009FF6D File Offset: 0x0009E16D
	[ConsoleMethod("kill_prisoners", "Kill all prisoners in local player's kingdom")]
	public void FreeAllPrisoners()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "kill_prisoners", true))
		{
			return;
		}
		Action action = BaseUI.LogicKingdom().actions.Find("GlobalKillPrisonerAction");
		if (action == null)
		{
			return;
		}
		action.Execute(null);
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0009FFA0 File Offset: 0x0009E1A0
	[ConsoleMethod("ao", "Force action outcome")]
	public void ForceActionOutcome(string str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ao", true))
		{
			return;
		}
		object obj = BaseUI.TTObj();
		Action action = obj as Action;
		if (action == null)
		{
			Opportunity opportunity = obj as Opportunity;
			action = ((opportunity != null) ? opportunity.action : null);
		}
		if (action == null)
		{
			OngoingActionStatus ongoingActionStatus = obj as OngoingActionStatus;
			action = ((ongoingActionStatus != null) ? ongoingActionStatus.GetAction() : null);
		}
		if (action == null)
		{
			Logic.Character character = obj as Logic.Character;
			action = ((character != null) ? character.cur_action : null);
		}
		if (action == null)
		{
			Debug.Log("No action tooltip");
			return;
		}
		if (action.def.outcomes == null)
		{
			Debug.Log(action.def.Name + " has no outcomes");
			return;
		}
		action.ForceOutcomes(str);
		if (action.forced_outcomes == null || action.forced_outcomes.Count == 0)
		{
			return;
		}
		string text = "";
		bool flag = true;
		for (int i = 0; i < action.forced_outcomes.Count; i++)
		{
			OutcomeDef outcomeDef = action.forced_outcomes[i];
			if (outcomeDef == null)
			{
				flag = false;
			}
			else
			{
				if (text != "")
				{
					text += ", ";
				}
				if (!flag)
				{
					text += "*";
				}
				text += outcomeDef.id;
			}
		}
		text = action.ToString() + " -> " + text;
		Debug.Log(text);
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x000A00E5 File Offset: 0x0009E2E5
	[ConsoleMethod("aoe", "Enable / Disable action outcome effects")]
	public void EnableActionOutcomeEffects(int i)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "aoe", true))
		{
			return;
		}
		Action.enable_outcome_effects = (i != 0);
		Debug.Log("Action outcome effectively " + ((i != 0) ? "on" : "off"));
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x000A011D File Offset: 0x0009E31D
	[ConsoleMethod("areq", "Enable (1) / Disable (0) action requirements checks")]
	public void EnableActionRequirementChecks(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "areq", true))
		{
			return;
		}
		Action.no_requirement_checks = (enable == 0);
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x000A0137 File Offset: 0x0009E337
	[ConsoleMethod("apd", "Force all action prepare durations to a given duration (<= 0 - no force)")]
	public void ForceActionPreapreDurations(float duration)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "apd", true))
		{
			return;
		}
		Action.force_prepare_duration = duration;
		if (duration <= 0f)
		{
			Debug.Log("Action prepare durations not forced");
			return;
		}
		Debug.Log(string.Format("Action prepare durations forced to {0} sec", duration));
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x000A0178 File Offset: 0x0009E378
	[ConsoleMethod("ta", "Test action")]
	public void TestAction(string action_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ta", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected object");
			return;
		}
		Actions component = @object.GetComponent<Actions>();
		if (component == null)
		{
			Debug.Log(@object.ToString() + " has no actions");
			return;
		}
		Action action = null;
		for (int i = 0; i < component.Count; i++)
		{
			Action action2 = component[i];
			if (action2.def.field.key.StartsWith(action_name, StringComparison.OrdinalIgnoreCase))
			{
				action = action2;
				break;
			}
		}
		if (action == null)
		{
			Debug.LogError(@object.ToString() + " has no action starting with " + action_name);
			return;
		}
		string text = action.Validate(false);
		if (text != "ok")
		{
			Debug.LogError(action.ToString() + " not valid: " + text);
			return;
		}
		Debug.Log("Performing " + action.ToString());
		(action.visuals as ActionVisuals).Begin();
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x000A0274 File Offset: 0x0009E474
	[ConsoleMethod("va", "Validate action")]
	public void VallidateAction(string action_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "va", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected object");
			return;
		}
		Actions component = @object.GetComponent<Actions>();
		if (component == null)
		{
			Debug.Log(@object.ToString() + " has no actions");
			return;
		}
		Action action = null;
		for (int i = 0; i < component.Count; i++)
		{
			Action action2 = component[i];
			if (action2.def.field.key.StartsWith(action_name, StringComparison.OrdinalIgnoreCase))
			{
				action = action2;
				break;
			}
		}
		if (action == null)
		{
			Debug.LogError(@object.ToString() + " has no action starting with " + action_name);
			return;
		}
		Debug.Log(action.Validate(false));
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x000A0328 File Offset: 0x0009E528
	[ConsoleMethod("tasf", "Test action success/fail")]
	public void TestActionSuccessFail(string action_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "tasf", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected object");
			return;
		}
		Actions component = @object.GetComponent<Actions>();
		if (component == null)
		{
			Debug.Log(@object.ToString() + " has no actions");
			return;
		}
		Action action = null;
		for (int i = 0; i < component.Count; i++)
		{
			Action action2 = component[i];
			if (action2.def.field.key.StartsWith(action_name, StringComparison.OrdinalIgnoreCase))
			{
				action = action2;
				break;
			}
		}
		if (action == null)
		{
			Debug.LogError(@object.ToString() + " has no action starting with " + action_name);
			return;
		}
		if (action.def.success_fail == null)
		{
			Debug.LogError(action.ToString() + " has no success_fail def");
			return;
		}
		Logic.SuccessAndFail successAndFail = Logic.SuccessAndFail.Get(action, true, null);
		Debug.Log(successAndFail.ToString() + "\n" + successAndFail.FactorsText());
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x000A041C File Offset: 0x0009E61C
	[ConsoleMethod("at", "View action tooltip")]
	public void ViewActionTooltip(string action_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "at", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected object");
			return;
		}
		Actions component = @object.GetComponent<Actions>();
		if (component == null)
		{
			Debug.Log(@object.ToString() + " has no actions");
			return;
		}
		Action action = null;
		for (int i = 0; i < component.Count; i++)
		{
			Action action2 = component[i];
			if (action2.def.field.key.StartsWith(action_name, StringComparison.OrdinalIgnoreCase))
			{
				action = action2;
				break;
			}
		}
		if (action == null)
		{
			Debug.LogError(@object.ToString() + " has no action starting with " + action_name);
			return;
		}
		Debug.Log(global::Defs.Localize("ActionTooltip.text", new Vars(action), null, true, true));
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x000A04DE File Offset: 0x0009E6DE
	[ConsoleMethod("tooltips", "Enable / Disable tooltips")]
	public void EnableTooltips(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "tooltips", true))
		{
			return;
		}
		BaseUI.show_tooltips = (enable != 0);
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x000A04F8 File Offset: 0x0009E6F8
	[ConsoleMethod("tpc", "Test pros and cons")]
	public void TestProsAndCons()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tpc", true))
		{
			return;
		}
		Debug.Log(ProsAndCons.Test("PC_War", BaseUI.LogicKingdom(), BaseUI.SelKingdom()));
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x000A0522 File Offset: 0x0009E722
	[ConsoleMethod("tpc", "Test their pros and cons")]
	public void TestProsAndCons(string id)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tpc", true))
		{
			return;
		}
		Debug.Log(ProsAndCons.Test(id, BaseUI.LogicKingdom(), BaseUI.SelKingdom()));
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x000A0548 File Offset: 0x0009E748
	[ConsoleMethod("ttpc", "Test our pros and cons")]
	public void TestTheirProsAndCons(string id)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ttpc", true))
		{
			return;
		}
		Debug.Log(ProsAndCons.Test(id, BaseUI.SelKingdom(), BaseUI.LogicKingdom()));
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x000A056E File Offset: 0x0009E76E
	[ConsoleMethod("bpc", "Benchmark pros and cons")]
	public void BenchmarkProsAndCons()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "bpc", true))
		{
			return;
		}
		Debug.Log(ProsAndCons.Benchmark(BaseUI.LogicKingdom(), BaseUI.SelKingdom()));
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x000A0594 File Offset: 0x0009E794
	[ConsoleMethod("offer", "Send an offer to selected kingdom")]
	public void Offer(string offer_type)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "offer", true))
		{
			return;
		}
		string text = Offers.TestSend(offer_type, BaseUI.LogicKingdom(), BaseUI.SelLO(), "accept");
		if (!string.IsNullOrEmpty(text))
		{
			Debug.Log(text);
		}
	}

	// Token: 0x06000E90 RID: 3728 RVA: 0x000A05D4 File Offset: 0x0009E7D4
	[ConsoleMethod("clear_offers", "Send an offer to selected kingdom")]
	public void ClearOffers()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "clear_offers", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelLO() as Logic.Kingdom;
		if (kingdom != null)
		{
			Offers component = kingdom.GetComponent<Offers>();
			if (component != null)
			{
				component.incoming.Clear();
			}
			if (component != null)
			{
				component.outgoing.Clear();
			}
			Debug.Log("Cleared Offers of " + kingdom.Name);
		}
		kingdom = BaseUI.LogicKingdom();
		if (kingdom != null)
		{
			Offers component2 = kingdom.GetComponent<Offers>();
			if (component2 != null)
			{
				component2.incoming.Clear();
			}
			if (component2 != null)
			{
				component2.outgoing.Clear();
			}
			Debug.Log("Cleared Offers of " + kingdom.Name);
		}
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x000A0680 File Offset: 0x0009E880
	[ConsoleMethod("offer_me", "Receive an offer from the selected kingdom")]
	public void OfferMe(string offer_type)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "offer_me", true))
		{
			return;
		}
		string text = Offers.TestSend(offer_type, BaseUI.SelKingdom(), BaseUI.LogicKingdom(), "propose");
		if (!string.IsNullOrEmpty(text))
		{
			Debug.Log(text);
		}
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x000A06C0 File Offset: 0x0009E8C0
	[ConsoleMethod("offer_to", "Make selected kingdom propose an offer to another kingdom")]
	public void OfferMe(string other_kingdom, string offer_type)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "offer_me", true))
		{
			return;
		}
		string text = Offers.TestSend(offer_type, BaseUI.SelKingdom(), GameLogic.Get(false).GetKingdom(other_kingdom), "propose");
		if (!string.IsNullOrEmpty(text))
		{
			Debug.Log(text);
		}
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x000A0708 File Offset: 0x0009E908
	[ConsoleMethod("offer_recieve", "Receive an offer from a random kingdom")]
	public void OfferRecieve(string rel_change_type)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "offer_recieve", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		int num = game.Random(0, game.kingdoms.Count);
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (rel_change_type != "positive" && rel_change_type != "negative")
		{
			rel_change_type = "neutral";
		}
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			num++;
			if (num > game.kingdoms.Count)
			{
				num = 1;
			}
			Logic.Kingdom kingdom2 = game.GetKingdom(num);
			if (kingdom2 != null && !kingdom2.IsDefeated() && kingdom != kingdom2)
			{
				OfferGenerator offerGenerator = OfferGenerator.instance;
				Offer offer = (offerGenerator != null) ? offerGenerator.TryGenerateRandomOfferHeavy("propose", kingdom2, kingdom, null, 0, true, 0f, 0f, rel_change_type, 0) : null;
				if (offer != null)
				{
					offer.Send(true);
					return;
				}
			}
		}
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x000A07E2 File Offset: 0x0009E9E2
	[ConsoleMethod("offer_recieve", "Receive an offer from a random kingdom")]
	public void OfferRecieve()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "offer_recieve", true))
		{
			return;
		}
		this.OfferRecieve("neutral");
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x000A0800 File Offset: 0x0009EA00
	[ConsoleMethod("offers", "View selected kingdom pending offers")]
	public void ListOffers()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "offers", true))
		{
			return;
		}
		Logic.Kingdom k = BaseUI.SelKingdom();
		this.ListOffers(k);
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x000A082C File Offset: 0x0009EA2C
	[ConsoleMethod("my_offers", "View our kingdom pending offers")]
	public void ListMyOffers()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "my_offers", true))
		{
			return;
		}
		Logic.Kingdom k = BaseUI.LogicKingdom();
		this.ListOffers(k);
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x000A0858 File Offset: 0x0009EA58
	public void ListOffers(Logic.Kingdom k)
	{
		if (k == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Offers offers = Offers.Get(k, false);
		if (offers == null || offers.Empty())
		{
			Debug.Log(k.ToString() + " has no pending offers");
			return;
		}
		string text = k.ToString() + " pending offers:\n";
		if (offers.outgoing != null && offers.outgoing.Count > 0)
		{
			text = string.Concat(new object[]
			{
				text,
				"Outgoing (",
				offers.outgoing.Count,
				"):\n"
			});
			for (int i = 0; i < offers.outgoing.Count; i++)
			{
				Offer offer = offers.outgoing[i];
				text = string.Concat(new object[]
				{
					text,
					i,
					": ",
					offer.ToString(),
					"\n"
				});
			}
		}
		if (offers.incoming != null && offers.incoming.Count > 0)
		{
			text = string.Concat(new object[]
			{
				text,
				"Incoming (",
				offers.incoming.Count,
				"):\n"
			});
			for (int j = 0; j < offers.incoming.Count; j++)
			{
				Offer offer2 = offers.incoming[j];
				text = string.Concat(new object[]
				{
					text,
					j,
					": ",
					offer2.ToString(),
					"\n"
				});
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x000A0A04 File Offset: 0x0009EC04
	[ConsoleMethod("accept", "Force the selected kingdom to accept incoming offer by given index")]
	public void Accept(int idx)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "accept", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Offers offers = Offers.Get(kingdom, false);
		if (offers == null || offers.incoming == null || offers.incoming.Count == 0)
		{
			Debug.Log(kingdom.ToString() + " has no pending incoming offers");
			return;
		}
		if (idx >= offers.incoming.Count)
		{
			Debug.Log("invalid offer index");
			return;
		}
		Offer offer = offers.incoming[idx];
		Debug.Log("Accepting " + offer.ToString());
		offer.Accept();
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x000A0AAC File Offset: 0x0009ECAC
	[ConsoleMethod("decline", "Force the selected kingdom to decline incoming offer by given index")]
	public void Decline(int idx)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "decline", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Offers offers = Offers.Get(kingdom, false);
		if (offers == null || offers.incoming == null || offers.incoming.Count == 0)
		{
			Debug.Log(kingdom.ToString() + " has no pending incoming offers");
			return;
		}
		if (idx >= offers.incoming.Count)
		{
			Debug.Log("invalid offer index");
			return;
		}
		Offer offer = offers.incoming[idx];
		Debug.Log("Declining " + offer.ToString());
		offer.Decline();
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x000A0B54 File Offset: 0x0009ED54
	[ConsoleMethod("nce", "new Cardinal-Elect from the selected kingdom")]
	public void NewCardinalElect()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "nce", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		Catholic catholic = kingdom.game.religions.catholic;
		Logic.Character character = catholic.CreateCardinal(kingdom);
		if (character != null)
		{
			character.Start();
		}
		catholic.AddCardinal(character);
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x000A0BA6 File Offset: 0x0009EDA6
	[ConsoleMethod("nc", "new player-led crusade against selected kingdom")]
	public void NewCrusade()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "nc", true))
		{
			return;
		}
		this.NewCrusade(true);
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x000A0BC0 File Offset: 0x0009EDC0
	[ConsoleMethod("nc", "new player-led crusade against selected kingdom")]
	public void NewCrusade(bool choose_only_player_leaders)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "nc", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Catholic catholic = kingdom.game.religions.catholic;
		if (catholic.crusade != null)
		{
			catholic.crusade.end_reason = "dev_cheat";
			catholic.crusade.End(true);
		}
		Crusade.choose_only_player_leaders = choose_only_player_leaders;
		Crusade.Start(kingdom, null, null);
		Crusade.choose_only_player_leaders = false;
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x000A0C3A File Offset: 0x0009EE3A
	[ConsoleMethod("poc", "set Player-Only Crusade leaders")]
	public void SetPlayerOnlyCrusades(int poc)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "poc", true))
		{
			return;
		}
		Crusade.choose_only_player_leaders = (poc != 0);
		Debug.Log("Player-only crusades: " + (Crusade.choose_only_player_leaders ? "YES" : "no"));
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x000A0C78 File Offset: 0x0009EE78
	[ConsoleMethod("cco", "test crusade capture town outcomes")]
	public void TestCrusadeCaptureOutcomes()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "cco", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		if (settlement == null)
		{
			Debug.Log("No selected settlement");
			return;
		}
		Logic.Realm realm = settlement.GetRealm();
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		Crusade crusade = realm.game.religions.catholic.crusade;
		if (crusade == null)
		{
			Debug.Log("No active crusade");
			return;
		}
		crusade.last_captured_realm = realm;
		List<OutcomeDef> list = crusade.def.capture_realm_outcomes.DecideOutcomes(realm.game, crusade, crusade.force_capture_realm_outcomes, new OutcomeDef.AlterChannceFunc(crusade.AlterRealmCapturedOutcome));
		if (list == null || list.Count == 0)
		{
			Debug.Log("No outcomes");
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Debug.Log(list[i].ToString());
		}
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x000A0D54 File Offset: 0x0009EF54
	[ConsoleMethod("cco", "force crusade capture town outcome")]
	public void ForceCrusadeCaptureOutcome(string outcome)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "cco", true))
		{
			return;
		}
		Crusade crusade = GameLogic.Get(true).religions.catholic.crusade;
		if (crusade == null)
		{
			Debug.Log("No active crusade");
			return;
		}
		crusade.force_capture_realm_outcomes = crusade.def.capture_realm_outcomes.Parse(outcome, true);
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x000A0DAC File Offset: 0x0009EFAC
	[ConsoleMethod("ceo", "test crusade end outcomes")]
	public void TestCrusadeEndOutcomes()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ceo", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Crusade crusade = game.religions.catholic.crusade;
		if (crusade == null)
		{
			Debug.Log("No active crusade");
			return;
		}
		List<OutcomeDef> list = crusade.def.end_outcomes.DecideOutcomes(game, crusade, crusade.force_end_outcomes, new OutcomeDef.AlterChannceFunc(crusade.AlterEndOutcome));
		if (list == null || list.Count == 0)
		{
			Debug.Log("No outcomes");
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Debug.Log(list[i].ToString());
		}
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x000A0E4C File Offset: 0x0009F04C
	[ConsoleMethod("ceo", "force crusade end outcome")]
	public void ForceCrusadeEndOutcome(string outcome)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ceo", true))
		{
			return;
		}
		Crusade crusade = GameLogic.Get(true).religions.catholic.crusade;
		if (crusade == null)
		{
			Debug.Log("No active crusade");
			return;
		}
		crusade.force_end_outcomes = crusade.def.end_outcomes.Parse(outcome, true);
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x000A0EA4 File Offset: 0x0009F0A4
	[ConsoleMethod("rbt", "View religion bonus texts for selected kingdom")]
	public void ViewReligionBonuses()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "rbt", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (kingdom.religion_mods != null)
		{
			stringBuilder.AppendLine("Current religion: " + kingdom.religion.ToString() + ":");
			string religionModsText = global::Religions.GetReligionModsText(kingdom, "\n  ");
			if (!string.IsNullOrEmpty(religionModsText))
			{
				stringBuilder.AppendLine("  " + religionModsText);
			}
		}
		if (kingdom.patriarch_bonuses != null)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			string str = "Current patriacrch: ";
			Logic.Character patriarch = kingdom.patriarch;
			stringBuilder2.AppendLine(str + (((patriarch != null) ? patriarch.ToString() : null) ?? "null") + ":");
			string patriarchBonusesText = global::Religions.GetPatriarchBonusesText(kingdom, "\n  ");
			if (!string.IsNullOrEmpty(patriarchBonusesText))
			{
				stringBuilder.AppendLine("  " + patriarchBonusesText);
			}
		}
		if (kingdom.patriarch_candidates != null)
		{
			stringBuilder.AppendLine("Candidates:");
			for (int i = 0; i < kingdom.patriarch_candidates.Count; i++)
			{
				Orthodox.PatriarchCandidate patriarchCandidate = kingdom.patriarch_candidates[i];
				stringBuilder.AppendLine("  " + patriarchCandidate.cleric.ToString() + ":");
				string patriarchBonusesText2 = global::Religions.GetPatriarchBonusesText(kingdom, patriarchCandidate, "\n    ", false);
				if (!string.IsNullOrEmpty(patriarchBonusesText2))
				{
					stringBuilder.AppendLine("    " + patriarchBonusesText2);
				}
			}
		}
		if (kingdom.is_muslim && kingdom.game.religions.jihad_targets.Count > 0)
		{
			stringBuilder.AppendLine("Jihad bonuses:");
			string jihadBonusesText = global::Religions.GetJihadBonusesText(kingdom, "\n  ");
			stringBuilder.AppendLine("  " + jihadBonusesText);
		}
		if (kingdom.is_pagan && kingdom.pagan_beliefs != null)
		{
			stringBuilder.AppendLine("Pagan beliefs:");
			for (int j = 0; j < kingdom.pagan_beliefs.Count; j++)
			{
				Religion.PaganBelief pt = kingdom.pagan_beliefs[j];
				string paganTraditionNameText = global::Religions.GetPaganTraditionNameText(kingdom, pt);
				string paganTraditionBonusesText = global::Religions.GetPaganTraditionBonusesText(kingdom, pt, "\n    ");
				if (!string.IsNullOrEmpty(paganTraditionBonusesText))
				{
					stringBuilder.AppendLine("  " + paganTraditionNameText + ":");
					stringBuilder.AppendLine("    " + paganTraditionBonusesText);
				}
			}
		}
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x000A1108 File Offset: 0x0009F308
	[ConsoleMethod("ex", "Excomunicate curent kingdom")]
	public void Excommunicated()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ex", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		if (kingdom == null || !kingdom.is_catholic || kingdom.excommunicated)
		{
			Debug.LogWarning(string.Format("{0} is not a valid excommunication target!", kingdom));
			return;
		}
		kingdom.game.religions.catholic.Excommunicate(kingdom);
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x000A1174 File Offset: 0x0009F374
	[ConsoleMethod("unex", "Excomunicate curent kingdom")]
	public void UnExcommunicated()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "unex", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		if (kingdom == null || !kingdom.is_catholic || !kingdom.excommunicated)
		{
			Debug.LogWarning(string.Format("{0} is not a valid unexcommunication target!", kingdom));
			return;
		}
		kingdom.game.religions.catholic.UnExcommunicate(kingdom);
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x000A11E0 File Offset: 0x0009F3E0
	[ConsoleMethod("restore_papacy", "Restore the papacy")]
	public void RestorePapacy()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "restore_papacy", true))
		{
			return;
		}
		if (global::Religions.GetPope() != null)
		{
			Debug.Log("Papacy already exists!");
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Logic.Religions religions = game.religions;
		if (religions == null)
		{
			return;
		}
		religions.catholic.RestorePapacy("cheat");
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x000A1234 File Offset: 0x0009F434
	private Book.Def FindBookDef(string name)
	{
		Logic.Defs.Registry registry = GameLogic.Get(true).defs.Get(typeof(Book.Def));
		if (registry == null)
		{
			return null;
		}
		foreach (KeyValuePair<string, Def> keyValuePair in registry.defs)
		{
			Book.Def def = keyValuePair.Value as Book.Def;
			if (def != null && def.name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				return def;
			}
		}
		return null;
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x000A12C8 File Offset: 0x0009F4C8
	[ConsoleMethod("add_book", "Add a new book to player kingdom")]
	public void AddBook(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_book", true))
		{
			return;
		}
		Book.Def def = this.FindBookDef(name);
		if (def == null)
		{
			Debug.Log("No such book");
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		Debug.Log("Adding book " + def.ToString());
		kingdom.AddBook(def, 1, false);
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x000A1324 File Offset: 0x0009F524
	[ConsoleMethod("add_book_bundle", "Add a number of every book type")]
	public void AddBookBundle(int copies)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_book_bundle", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.Defs.Registry registry = GameLogic.Get(true).defs.Get(typeof(Book.Def));
		if (registry == null)
		{
			Debug.Log("No books where found");
			return;
		}
		int num = 0;
		foreach (KeyValuePair<string, Def> keyValuePair in registry.defs)
		{
			Book.Def def = keyValuePair.Value as Book.Def;
			if (def != null)
			{
				kingdom.AddBook(def, copies, false);
				num++;
			}
		}
		Debug.Log(string.Format("Added {0} of {1} book types", copies, num));
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x000A13F0 File Offset: 0x0009F5F0
	[ConsoleMethod("branch", "View current steam branch")]
	public void PrintBranch()
	{
		StringBuilder stringBuilder = new StringBuilder(128);
		stringBuilder.Clear();
		THQNORequest steamBetaName = THQNORequest.GetSteamBetaName(stringBuilder, 128U);
		string text = stringBuilder.ToString();
		if (text == null)
		{
			text = "<null>";
		}
		Debug.Log(string.Format("Current steam branch: '{0}', request result: '{1}'", text, steamBetaName.result));
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x000A1444 File Offset: 0x0009F644
	[ConsoleMethod("avc", "Show Analytics Visual Clue")]
	public void ShowAnalyticsVisualClue(int val)
	{
		AnalyticsVisualClues.Show(val);
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x000A144C File Offset: 0x0009F64C
	[ConsoleMethod("avc", "Show Analytics Visual Clues")]
	public void ShowAnalyticsVisualClues2(int val1, int val2)
	{
		AnalyticsVisualClues.Show(val1);
		AnalyticsVisualClues.Show(val2);
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x000A145A File Offset: 0x0009F65A
	[ConsoleMethod("avc", "Show Analytics Visual Clues")]
	public void ShowAnalyticsVisualClues3(int val1, int val2, int val3)
	{
		AnalyticsVisualClues.Show(val1);
		AnalyticsVisualClues.Show(val2);
		AnalyticsVisualClues.Show(val3);
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x000A1470 File Offset: 0x0009F670
	[ConsoleMethod("sr", "Dump screen resolution")]
	public void DumpScreenResolution()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "sr", true))
		{
			return;
		}
		Resolution currentResolution = Screen.currentResolution;
		Debug.Log(string.Concat(new object[]
		{
			"Screen: ",
			currentResolution.width,
			"x",
			currentResolution.height,
			", view: ",
			Screen.width,
			"x",
			Screen.height
		}));
		Debug.Log("GPU: " + SystemInfo.graphicsDeviceName);
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x000A1510 File Offset: 0x0009F710
	[ConsoleMethod("rt", "Dump realm tags")]
	public void DumpRealmTags()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "sr", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		realm.GetTag("");
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(realm.name + " tags:");
		foreach (KeyValuePair<string, int> keyValuePair in realm.tags)
		{
			string key = keyValuePair.Key;
			int value = keyValuePair.Value;
			stringBuilder.AppendLine(key + ": " + value);
		}
		Debug.Log(stringBuilder);
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x000A15EC File Offset: 0x0009F7EC
	[ConsoleMethod("count_eth", "Count kingdoms and provinces per ethnicity")]
	public void CountEthnicities()
	{
		Game game = GameLogic.Get(true);
		int num = 0;
		int num2 = 0;
		Dictionary<Logic.Character.Ethnicity, DevCheats.EthnicityInfo> dictionary = new Dictionary<Logic.Character.Ethnicity, DevCheats.EthnicityInfo>();
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (kingdom != null)
			{
				int count = kingdom.realms.Count;
				if (count != 0)
				{
					string culture = kingdom.culture;
					Cultures.Defaults defaults = game.cultures.GetDefaults(culture);
					if (defaults == null)
					{
						Debug.LogWarning(string.Concat(new string[]
						{
							"Unknown ethnicity for kingdom ",
							kingdom.Name,
							" (",
							kingdom.culture,
							")"
						}));
					}
					else
					{
						Logic.Character.Ethnicity ethnicity = defaults.Ethnicity;
						DevCheats.EthnicityInfo ethnicityInfo;
						if (!dictionary.TryGetValue(ethnicity, out ethnicityInfo))
						{
							ethnicityInfo = new DevCheats.EthnicityInfo
							{
								name = ethnicity.ToString()
							};
							dictionary.Add(ethnicity, ethnicityInfo);
						}
						ethnicityInfo.kingdoms.Add(kingdom);
						ethnicityInfo.provinces += count;
						num++;
						num2 += count;
					}
				}
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("{0},,{1},{2}", game.map_period, num, num2));
		for (Logic.Character.Ethnicity ethnicity2 = Logic.Character.Ethnicity.European; ethnicity2 < Logic.Character.Ethnicity.COUNT; ethnicity2++)
		{
			DevCheats.EthnicityInfo ethnicityInfo2;
			if (dictionary.TryGetValue(ethnicity2, out ethnicityInfo2))
			{
				stringBuilder.AppendLine(string.Format(",{0},{1},{2}", ethnicityInfo2.name, ethnicityInfo2.kingdoms.Count, ethnicityInfo2.provinces));
			}
			else
			{
				stringBuilder.AppendLine(string.Format(",{0},0,0", ethnicity2));
			}
		}
		string text = stringBuilder.ToString();
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x000A17BC File Offset: 0x0009F9BC
	[ConsoleMethod("rrt", "Refresh realm tags")]
	public void RefreshRealmTags()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rrt", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		realm.RefreshTags(true);
		realm.GetTag("");
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(realm.name + " tags:");
		foreach (KeyValuePair<string, int> keyValuePair in realm.tags)
		{
			string key = keyValuePair.Key;
			int value = keyValuePair.Value;
			stringBuilder.AppendLine(key + ": " + value);
		}
		Debug.Log(stringBuilder);
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x000A18A0 File Offset: 0x0009FAA0
	[ConsoleMethod("build", "Build a building / upgrade in selected castle")]
	public void BuildBuilding(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "build", true))
		{
			return;
		}
		Building.Def def = this.FindDef<Building.Def>(name);
		if (def == null)
		{
			Debug.LogError("Unknown building / upgrade: '" + name + "'");
			return;
		}
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		Debug.Log("Building " + def.id + " in " + castle.name);
		castle.BuildBuilding(def, -1, true);
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x000A1920 File Offset: 0x0009FB20
	[ConsoleMethod("build", "Build a building in specified province")]
	public void BuildBuilding(string name, string castle_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "build", true))
		{
			return;
		}
		Building.Def def = this.FindDef<Building.Def>(name);
		if (def == null)
		{
			Debug.LogError("Unknown building / upgrade: '" + name + "'");
			return;
		}
		Logic.Realm realm = this.FindCastle(castle_name);
		Castle castle = (realm != null) ? realm.castle : null;
		if (castle == null)
		{
			Debug.LogError("Unknown realm: '" + castle_name + "'");
			return;
		}
		Debug.Log("Building " + def.id + " in " + castle.name);
		castle.BuildBuilding(def, -1, true);
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x000A19B4 File Offset: 0x0009FBB4
	[ConsoleMethod("upgrade", "Ulock building upgrade")]
	public void UnlockBuildingUpgrade(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "upgrade", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		Building.Def def = this.FindDef<Building.Def>(name);
		if (def == null)
		{
			Debug.LogError("Unknown building def: '" + name + "'");
			return;
		}
		if (kingdom.UnlockBuildingUpgrade(def, true))
		{
			Debug.Log("Unlocked '" + def.id + "'");
			return;
		}
		Debug.LogError("Failed to unlock '" + def.id + "'");
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x000A1A50 File Offset: 0x0009FC50
	[ConsoleMethod("try_upgrade", "Try to unlock building upgrade")]
	public void TryUnlockBuildingUpgrade(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "try_upgrade", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		Building.Def def = this.FindDef<Building.Def>(name);
		if (def == null)
		{
			Debug.LogError("Unknown building def: '" + name + "'");
			return;
		}
		Castle castle = kingdom.FindCastleToBuild(def, false);
		if (kingdom.UnlockBuildingUpgrade(def, false))
		{
			Debug.Log("Unlocked '" + def.id + "' from " + ((castle != null) ? castle.name : null));
			return;
		}
		Debug.LogError("Failed to unlock '" + def.id + "'");
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x000A1B00 File Offset: 0x0009FD00
	[ConsoleMethod("rkc", "Reload kingdom colors")]
	public void ReloadKingdomColors()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rkc", true))
		{
			return;
		}
		WorldMap worldMap = WorldMap.Get();
		if (((worldMap != null) ? worldMap.Kingdoms : null) == null)
		{
			return;
		}
		for (int i = 0; i < worldMap.Kingdoms.Count; i++)
		{
			global::Kingdom kingdom = worldMap.Kingdoms[i];
			kingdom.MapColorIndex = 0;
			kingdom.PrimaryArmyColorIndex = 0;
			kingdom.SecondaryArmyColorIndex = 0;
		}
		for (int j = 0; j < worldMap.Kingdoms.Count; j++)
		{
			global::Kingdom kingdom2 = worldMap.Kingdoms[j];
			if (((kingdom2 != null) ? kingdom2.logic : null) != null)
			{
				kingdom2.UpdateColors(kingdom2.logic);
			}
		}
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x000A1BA4 File Offset: 0x0009FDA4
	[ConsoleMethod("apf", "Add a province feature to selected realm")]
	public void AddReamTag(string tag)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "apf", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		if (realm.features == null)
		{
			realm.features = new List<string>();
		}
		realm.features.Add(tag);
		if (realm.castle != null)
		{
			realm.castle.may_produce_resource = null;
		}
		Castle castle = realm.castle;
		if (castle != null)
		{
			castle.ClearResourcesInfo(true, true);
		}
		realm.RefreshTags(true);
		realm.GetTag("");
		Castle castle2 = realm.castle;
		if (castle2 != null)
		{
			castle2.RefreshBuildableDistricts();
		}
		realm.SendState<Logic.Realm.FeaturesState>();
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x000A1C58 File Offset: 0x0009FE58
	[ConsoleMethod("drd", "Dump resource dependencies")]
	public void DumpResourceDependencies(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "apf", true))
		{
			return;
		}
		Def def = this.FindDef(name, new Type[]
		{
			typeof(Resource.Def),
			typeof(Building.Def),
			typeof(District.Def)
		});
		if (def == null)
		{
			Debug.LogError("Unknown resource: '" + name + "'");
			return;
		}
		string text = ResourceInfo.DumpResourceDependencies(def, "", "", null);
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x000A1CE0 File Offset: 0x0009FEE0
	[ConsoleMethod("lrd", "List resource dependencies")]
	public void ListResourceDependencies(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "lrd", true))
		{
			return;
		}
		Def def = this.FindDef(name, new Type[]
		{
			typeof(Resource.Def),
			typeof(Building.Def),
			typeof(District.Def)
		});
		if (def == null)
		{
			Debug.LogError("Unknown resource: '" + name + "'");
			return;
		}
		string text = this.ListResourceDependencies(def, 2);
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x000A1D60 File Offset: 0x0009FF60
	[ConsoleMethod("lard", "List all resource dependencies")]
	public void ListResourceDependencies(int format = 0)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "lard", true))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(4096);
		if (format == 1)
		{
			stringBuilder.AppendLine("Type;ID;Dependancies;Cost;Resources;Buildings;Districts;Province Features;Settlement Types");
		}
		this.ListResourceDependencies(stringBuilder, typeof(Resource.Def), format);
		this.ListResourceDependencies(stringBuilder, typeof(Building.Def), format);
		string text = stringBuilder.ToString();
		if (format == 1)
		{
			string fullPath = Path.GetFullPath("../Resource Dependencies.csv");
			File.WriteAllText(fullPath, text);
			Process.Start(new ProcessStartInfo(fullPath)
			{
				UseShellExecute = true
			});
			return;
		}
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x000A1DF8 File Offset: 0x0009FFF8
	private void ListResourceDependencies(StringBuilder txt, Type def_type, int format)
	{
		Logic.Defs.Registry registry = GameLogic.Get(true).defs.Get(def_type);
		if (registry == null)
		{
			return;
		}
		foreach (KeyValuePair<string, Def> keyValuePair in registry.defs)
		{
			string key = keyValuePair.Key;
			Def value = keyValuePair.Value;
			if (value.loaded)
			{
				string value2 = this.ListResourceDependencies(value, format);
				if (!string.IsNullOrEmpty(value2))
				{
					txt.AppendLine(value2);
				}
			}
		}
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x000A1E90 File Offset: 0x000A0090
	private string ListResourceDependencies(Def def, int format)
	{
		DevCheats.<>c__DisplayClass242_0 CS$<>8__locals1;
		CS$<>8__locals1.format = format;
		List<Def> list = new List<Def>();
		List<Def> list2 = new List<Def>();
		int num = ResourceInfo.ListResourceDependencies(def, list, list2, false);
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		int num12 = 0;
		int num13 = 0;
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		string text5 = "";
		for (int i = 0; i < list.Count; i++)
		{
			Def def2 = list[i];
			bool flag = list2.Contains(def2);
			string str = flag ? "*" : "";
			string text6 = "\n    " + str + def2.id;
			Def def3 = def2;
			if (def3 != null)
			{
				Resource.Def def4;
				if ((def4 = (def3 as Resource.Def)) == null)
				{
					Building.Def def5;
					if ((def5 = (def3 as Building.Def)) == null)
					{
						if (!(def3 is District.Def))
						{
							if (!(def3 is ProvinceFeature.Def))
							{
								if (def3 is Logic.Settlement.Def)
								{
									num11++;
									if (!flag)
									{
										num6++;
									}
									text5 += text6;
								}
							}
							else
							{
								num10++;
								if (!flag)
								{
									num5++;
								}
								text4 += text6;
							}
						}
						else
						{
							num9++;
							if (!flag)
							{
								num4++;
							}
							text3 += text6;
						}
					}
					else
					{
						Building.Def def6 = def5;
						num8++;
						if (!flag)
						{
							num3++;
						}
						int num14 = (def6.cost == null) ? 0 : ((int)def6.cost[ResourceType.Gold]);
						num13 += num14;
						if (!flag)
						{
							num12 += num14;
						}
						text6 += string.Format(" ({0} gold)", num14);
						text2 += text6;
					}
				}
				else
				{
					Resource.Def def7 = def4;
					num7++;
					if (!flag)
					{
						num2++;
					}
					if (def7.produced_in != null)
					{
						text6 += ": ";
						for (int j = 0; j < def7.produced_in.Count; j++)
						{
							Building.Def def8 = def7.produced_in[j];
							if (j > 0)
							{
								text6 += ", ";
							}
							text6 += def8.id;
						}
					}
					text += text6;
				}
			}
		}
		if (CS$<>8__locals1.format == 1)
		{
			string[] array = new string[17];
			int num15 = 0;
			DT.Field field = def.field;
			string text7;
			if (field == null)
			{
				text7 = null;
			}
			else
			{
				DT.Field field2 = field.BaseRoot();
				text7 = ((field2 != null) ? field2.key : null);
			}
			array[num15] = text7;
			array[1] = ";";
			array[2] = def.id;
			array[3] = ";";
			array[4] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num, list.Count, ref CS$<>8__locals1);
			array[5] = ";";
			array[6] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num12, num13, ref CS$<>8__locals1);
			array[7] = ";";
			array[8] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num2, num7, ref CS$<>8__locals1);
			array[9] = ";";
			array[10] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num3, num8, ref CS$<>8__locals1);
			array[11] = ";";
			array[12] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num4, num9, ref CS$<>8__locals1);
			array[13] = ";";
			array[14] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num5, num10, ref CS$<>8__locals1);
			array[15] = ";";
			array[16] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num6, num11, ref CS$<>8__locals1);
			return string.Concat(array);
		}
		string[] array2 = new string[17];
		int num16 = 0;
		DT.Field field3 = def.field;
		string text8;
		if (field3 == null)
		{
			text8 = null;
		}
		else
		{
			DT.Field field4 = field3.BaseRoot();
			text8 = ((field4 != null) ? field4.key : null);
		}
		array2[num16] = text8;
		array2[1] = " ";
		array2[2] = def.id;
		array2[3] = ": Dependancies: ";
		array2[4] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num, list.Count, ref CS$<>8__locals1);
		array2[5] = ", Cost: ";
		array2[6] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num12, num13, ref CS$<>8__locals1);
		array2[7] = " gold, Resources: ";
		array2[8] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num2, num7, ref CS$<>8__locals1);
		array2[9] = ", Buildings: ";
		array2[10] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num3, num8, ref CS$<>8__locals1);
		array2[11] = ", Districts:";
		array2[12] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num4, num9, ref CS$<>8__locals1);
		array2[13] = ", Province Features: ";
		array2[14] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num5, num10, ref CS$<>8__locals1);
		array2[15] = ", Settlement Types: ";
		array2[16] = DevCheats.<ListResourceDependencies>g__cntstr|242_0(num6, num11, ref CS$<>8__locals1);
		string text9 = string.Concat(array2);
		if (CS$<>8__locals1.format >= 2)
		{
			text9 = text9 + "\nResources: " + DevCheats.<ListResourceDependencies>g__cntstr|242_0(num2, num7, ref CS$<>8__locals1) + text;
			text9 = text9 + "\nBuildings: " + DevCheats.<ListResourceDependencies>g__cntstr|242_0(num3, num8, ref CS$<>8__locals1) + text2;
			text9 = text9 + "\nDistricts: " + DevCheats.<ListResourceDependencies>g__cntstr|242_0(num4, num9, ref CS$<>8__locals1) + text3;
			text9 = text9 + "\nProvince Features: " + DevCheats.<ListResourceDependencies>g__cntstr|242_0(num5, num10, ref CS$<>8__locals1) + text4;
			text9 = text9 + "\nSettlement Types: " + DevCheats.<ListResourceDependencies>g__cntstr|242_0(num6, num11, ref CS$<>8__locals1) + text5;
		}
		return text9;
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x000A2368 File Offset: 0x000A0568
	[ConsoleMethod("ri", "Dump resource information for all obtainable resources")]
	public void DumpResourceInfo()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ri", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		kingdom.RefreshResourcesInfo(false);
		List<Resource.Def> defs = GameLogic.Get(true).defs.GetDefs<Resource.Def>();
		if (defs == null)
		{
			return;
		}
		string text = "Obtainable resources in " + kingdom.Name + ":";
		for (int i = 0; i < defs.Count; i++)
		{
			Resource.Def def = defs[i];
			ResourceInfo resourceInfo = kingdom.GetResourceInfo(def.id, true, true);
			if (resourceInfo.availability != ResourceInfo.Availability.Impossible)
			{
				text = text + "\n" + resourceInfo.Dump();
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x000A2424 File Offset: 0x000A0624
	[ConsoleMethod("ri", "Dump resource information for given resource")]
	public void DumpResourceInfo(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ri", true))
		{
			return;
		}
		Def def = this.FindDef(name, new Type[]
		{
			typeof(Resource.Def),
			typeof(Building.Def),
			typeof(Logic.Settlement.Def),
			typeof(ProvinceFeature.Def),
			typeof(District.Def)
		});
		if (def == null)
		{
			Debug.LogError("Unknown resource: '" + name + "'");
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		using (Game.Profile("Build resources info", true, 0f, null))
		{
			kingdom.RefreshResourcesInfo(false);
			kingdom.GetResourcesInfo();
		}
		ResourceInfo resourceInfo;
		using (Game.Profile("Calc resource info", true, 0f, null))
		{
			resourceInfo = kingdom.GetResourceInfo(def.id, true, true);
		}
		Debug.Log(resourceInfo.Dump());
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x000A2550 File Offset: 0x000A0750
	[ConsoleMethod("rri", "Fully recalc kingdom's resource info")]
	public void RecalcResourceInfo()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rri", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		using (Game.Profile("Resource info full rebuild", true, 0f, null))
		{
			kingdom.RefreshResourcesInfo(true);
		}
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x000A25C4 File Offset: 0x000A07C4
	[ConsoleMethod("ris", "Dump resource information performance stats")]
	public void DumpResourceInfoStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ris", true))
		{
			return;
		}
		string text = ResourceInfo.StatsText("", "\n", "    ");
		Game.CopyToClipboard(text);
		Debug.Log("\n" + text);
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x000A260B File Offset: 0x000A080B
	[ConsoleMethod("cris", "Clear resource information performance stats")]
	public void ClearResourceInfoStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "cris", true))
		{
			return;
		}
		ResourceInfo.ClearStats();
		Debug.Log("Resource Info stats cleared");
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x000A262C File Offset: 0x000A082C
	[ConsoleMethod("count_buildings", "Show number of castles, buildings and upgrades in kingdom")]
	public void CountBuildings()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "count_buildings", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No kingdom");
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < kingdom.realms.Count; i++)
		{
			Logic.Realm realm = kingdom.realms[i];
			Castle castle = (realm != null) ? realm.castle : null;
			if (castle != null)
			{
				num++;
				for (int j = 0; j < castle.buildings.Count; j++)
				{
					if (castle.buildings[j] != null)
					{
						num2++;
					}
				}
				for (int k = 0; k < castle.upgrades.Count; k++)
				{
					if (castle.upgrades[k] != null)
					{
						num3++;
					}
				}
			}
		}
		Debug.Log(string.Format("Kingdom {0}: Castles: {1}, Buildings: {2}, Upgrades: {3}", new object[]
		{
			kingdom.Name,
			num,
			num2,
			num3
		}));
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x000A2748 File Offset: 0x000A0948
	[ConsoleMethod("rbs", "Recalc Building States for selected kingdom")]
	public void RecalcBuildingStates()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rbs", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No kingdom");
			return;
		}
		int num;
		float millis;
		using (Game.ProfileScope profileScope = Game.Profile("RecalcAllBuildingStates", false, 0f, null))
		{
			num = kingdom.RecalcBuildingStates(null, false, false, false);
			millis = profileScope.Millis;
		}
		Debug.Log(string.Format("Changed {0} building(s) in {1} castle(s) for {2}ms", num, Logic.Kingdom.temp_castles_changed.Count, millis));
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x000A27F0 File Offset: 0x000A09F0
	[ConsoleMethod("rabs", "Recalc Building States for ALL kingdoms")]
	public void RecalcAllBuildingStates()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "rabs", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		int num = 0;
		int num2 = 0;
		float millis;
		using (Game.ProfileScope profileScope = Game.Profile("RecalcAllBuildingStates", false, 0f, null))
		{
			for (int i = 0; i < game.kingdoms.Count; i++)
			{
				Logic.Kingdom kingdom = game.kingdoms[i];
				if (kingdom != null && !kingdom.IsDefeated())
				{
					int num3 = kingdom.RecalcBuildingStates(null, false, false, false);
					if (num3 > 0)
					{
						num += num3;
						num2++;
					}
				}
			}
			millis = profileScope.Millis;
		}
		Debug.Log(string.Format("Changed {0} building(s) in {1} kingdom(s) for {2}ms", num, num2, millis));
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x000A28C4 File Offset: 0x000A0AC4
	[ConsoleMethod("import", "Import specific good with your king for free")]
	public void ImportGood(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "import", true))
		{
			return;
		}
		Resource.Def def = this.FindDef<Resource.Def>(name);
		if (def == null)
		{
			Debug.LogError("Invaid resource name: '" + name + "'");
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		Logic.Character character = (kingdom != null) ? kingdom.GetKing() : null;
		if (character == null)
		{
			Debug.LogError("No king");
			return;
		}
		if (character.importing_goods == null)
		{
			int maxImportGoodsCount = character.GetMaxImportGoodsCount();
			character.importing_goods = new List<Logic.Character.ImportedGood>();
			while (character.importing_goods.Count < maxImportGoodsCount)
			{
				character.importing_goods.Add(default(Logic.Character.ImportedGood));
			}
		}
		character.importing_goods.Add(new Logic.Character.ImportedGood
		{
			name = def.id,
			discount = 100f
		});
		Debug.Log(character.Name + " is now importing " + def.id);
		kingdom.RefreshRealmTags();
		kingdom.RecalcBuildingStates(null, false, false, false);
		kingdom.InvalidateIncomes(true);
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x000A29CC File Offset: 0x000A0BCC
	[ConsoleMethod("tbp", "Hide buildings panel")]
	public void HideBuildingsPanel()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tbp", true))
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		GameObject gameObject = global::Common.FindChildByName((baseUI != null) ? baseUI.gameObject : null, "id_MessageContainer", true, true);
		if (gameObject == null)
		{
			return;
		}
		UIBuildingsPanel componentInChildren = gameObject.GetComponentInChildren<UIBuildingsPanel>();
		if (componentInChildren == null)
		{
			return;
		}
		global::Common.DestroyObj(componentInChildren.gameObject);
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x000A2A2C File Offset: 0x000A0C2C
	[ConsoleMethod("tbw", "Test buildings window")]
	public void TestBuildingsWindow()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tbw", true))
		{
			return;
		}
		Castle castle = BaseUI.SelLO() as Castle;
		Logic.Kingdom kingdom;
		if ((kingdom = ((castle != null) ? castle.GetKingdom() : null)) == null)
		{
			kingdom = (BaseUI.SelKingdom() ?? BaseUI.LogicKingdom());
		}
		UICastleBuildWindow.Create(kingdom, castle, -1, null);
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x000A2A7C File Offset: 0x000A0C7C
	private District.Def ResolveDistrictDef(string def_id)
	{
		List<District.Def> defs = GameLogic.Get(true).defs.GetDefs<District.Def>();
		for (int i = 0; i < defs.Count; i++)
		{
			District.Def def = defs[i];
			if (def.id.IndexOf(def_id, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return def;
			}
		}
		return null;
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x000A2AC8 File Offset: 0x000A0CC8
	[ConsoleMethod("add_district", "Add district to selected castle")]
	public void AddDistrict(string def_id)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_district", true))
		{
			return;
		}
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		District.Def def = this.ResolveDistrictDef(def_id);
		if (def == null)
		{
			Debug.LogError("Invalid district def");
			return;
		}
		List<District.Def> buildableDistricts = castle.GetBuildableDistricts();
		if (buildableDistricts.Contains(def))
		{
			Debug.Log(string.Format("{0} is already buildable in {1}", def, castle));
			return;
		}
		Debug.Log(string.Format("Adding {0} to {1}", def, castle));
		buildableDistricts.Add(def);
		castle.ClearResourcesInfo(true, true);
		if (UICastleBuildWindow.IsVisible())
		{
			UICastleBuildWindow.Create(null, castle, -1, null);
		}
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x000A2B68 File Offset: 0x000A0D68
	[ConsoleMethod("tbp", "Test buildings panel")]
	public void TestBuildingsPanel(string def_id)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "tbp", true))
		{
			return;
		}
		this.HideBuildingsPanel();
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		District.Def def = this.ResolveDistrictDef(def_id);
		if (def == null)
		{
			Debug.LogError("Unknown district def");
			return;
		}
		GameObject gameObject = Assets.Get<GameObject>("assets/test/michael/buildingspanel/uip_buildingspanelmockup.prefab");
		if (gameObject == null)
		{
			Debug.LogError("assets/test/michael/buildingspanel/uip_buildingspanelmockup.prefab not found");
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		GameObject gameObject2 = global::Common.FindChildByName((baseUI != null) ? baseUI.gameObject : null, "id_MessageContainer", true, true);
		if (gameObject2 == null)
		{
			return;
		}
		GameObject gameObject3 = global::Common.Spawn(gameObject, gameObject2.transform, false, "");
		if (gameObject3 == null)
		{
			return;
		}
		gameObject3.GetOrAddComponent<UIBuildingsPanel>().Init(def, null, castle);
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x000A2C30 File Offset: 0x000A0E30
	[ConsoleMethod("dsr", "Draw sea realms")]
	public void DrawSeaRealms()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dsr", true))
		{
			return;
		}
		PoliticalView politicalView = ViewMode.current as PoliticalView;
		if (politicalView == null)
		{
			Debug.Log("You need to be in political view to use this command.");
			return;
		}
		politicalView.drawSeaRealms = !politicalView.drawSeaRealms;
		politicalView.Apply();
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x000A2C7C File Offset: 0x000A0E7C
	[ConsoleMethod("avm", "Apply view mode")]
	public void ApplyViewMode(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "avm", true))
		{
			return;
		}
		foreach (ViewMode viewMode in ViewMode.all)
		{
			if (viewMode.name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				Debug.Log("Applying view mode: " + viewMode.name);
				viewMode.ApplySecondary();
				return;
			}
		}
		Debug.LogError("Unknown view mode: " + name);
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x000A2D14 File Offset: 0x000A0F14
	[ConsoleMethod("kdist", "Kingdom distance")]
	public void KingdomDistance(string kName1, string kName2)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "kdist", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		Logic.Kingdom kingdom = logic.GetKingdom(kName1);
		if (kingdom == null)
		{
			Debug.Log(kName1 + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom2 = logic.GetKingdom(kName2);
		if (kingdom2 == null)
		{
			Debug.Log(kName2 + " is not a valid kingdom name");
			return;
		}
		Debug.Log(string.Concat(new object[]
		{
			"Distance between kingdoms",
			kName1,
			" and ",
			kName2,
			": ",
			logic.KingdomDistance(kingdom.id, kingdom2.id, int.MaxValue)
		}));
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x000A2DD0 File Offset: 0x000A0FD0
	[ConsoleMethod("kcrk", "Kingdom closest realm of kingdom")]
	public void KingdomClosestRealmToKingdom(string kName1, string kName2)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "kcrk", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		Logic.Kingdom kingdom = logic.GetKingdom(kName1);
		if (kingdom == null)
		{
			Debug.Log(kName1 + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom2 = logic.GetKingdom(kName2);
		if (kingdom2 == null)
		{
			Debug.Log(kName2 + " is not a valid kingdom name");
			return;
		}
		Logic.Realm closestRealmOfKingdom = kingdom.GetClosestRealmOfKingdom(kingdom2.id);
		Debug.Log(string.Concat(new object[]
		{
			"Closest Realm between ",
			kName1,
			" and ",
			kName2,
			": ",
			closestRealmOfKingdom.name,
			", Distance = ",
			closestRealmOfKingdom.wave_depth
		}));
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x000A2E9C File Offset: 0x000A109C
	[ConsoleMethod("rdist", "Realm distance")]
	public void RealmDistance(string rName1, string rName2, bool goThoughSeas, bool useLogicNeighboring)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rdist", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		Logic.Realm realm = logic.GetRealm(rName1);
		if (realm == null)
		{
			Debug.Log(rName1 + " is not a valid realm name");
			return;
		}
		Logic.Realm realm2 = logic.GetRealm(rName2);
		if (realm2 == null)
		{
			Debug.Log(rName2 + " is not a valid realm name");
			return;
		}
		Debug.Log(string.Concat(new object[]
		{
			"Distance between realms ",
			rName1,
			" and ",
			rName2,
			": ",
			logic.RealmDistance(realm.id, realm2.id, goThoughSeas, useLogicNeighboring, int.MaxValue)
		}));
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x000A2F5B File Offset: 0x000A115B
	[ConsoleMethod("rdist", "Realm distance")]
	public void RealmDistance(string rName1, string rName2)
	{
		this.RealmDistance(rName1, rName2, true, false);
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x000A2F68 File Offset: 0x000A1168
	private Logic.Kingdom FindKingdom(string name, bool allow_defeated = false)
	{
		Game game = GameLogic.Get(true);
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (kingdom != null && (allow_defeated || !kingdom.IsDefeated()) && kingdom.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				return kingdom;
			}
		}
		return null;
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x000A2FC0 File Offset: 0x000A11C0
	private Logic.Realm FindRealm(string name)
	{
		Game game = GameLogic.Get(true);
		for (int i = 0; i < game.realms.Count; i++)
		{
			Logic.Realm realm = game.realms[i];
			if (realm != null && realm.name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				return realm;
			}
		}
		return null;
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x000A300C File Offset: 0x000A120C
	private Logic.Realm FindCastle(string name)
	{
		Game game = GameLogic.Get(true);
		for (int i = 0; i < game.realms.Count; i++)
		{
			Logic.Realm realm = game.realms[i];
			if (realm != null && realm.castle != null && realm.castle.name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				return realm;
			}
		}
		return null;
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x000A3068 File Offset: 0x000A1268
	private Def FindDef(string name, Logic.Defs.Registry reg)
	{
		if (((reg != null) ? reg.defs : null) == null)
		{
			return null;
		}
		foreach (KeyValuePair<string, Def> keyValuePair in reg.defs)
		{
			string key = keyValuePair.Key;
			Def value = keyValuePair.Value;
			if (key.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				return value;
			}
		}
		return null;
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x000A30E4 File Offset: 0x000A12E4
	private T FindDef<T>(string name) where T : Def
	{
		if (string.IsNullOrEmpty(name))
		{
			return default(T);
		}
		Game game = GameLogic.Get(true);
		Logic.Defs.Registry registry;
		if (game == null)
		{
			registry = null;
		}
		else
		{
			Logic.Defs defs = game.defs;
			registry = ((defs != null) ? defs.Get(typeof(T)) : null);
		}
		Logic.Defs.Registry reg = registry;
		return this.FindDef(name, reg) as T;
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x000A3140 File Offset: 0x000A1340
	private Def FindDef(string name, params Type[] def_types)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		Game game = GameLogic.Get(true);
		if (((game != null) ? game.defs : null) == null)
		{
			return null;
		}
		foreach (Type def_type in def_types)
		{
			Logic.Defs.Registry registry = game.defs.Get(def_type);
			if (registry != null)
			{
				Def def = this.FindDef(name, registry);
				if (def != null)
				{
					return def;
				}
			}
		}
		return null;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x000A31A4 File Offset: 0x000A13A4
	[ConsoleMethod("ck", "Change selected realm's kingdom")]
	public void ChangeRealmKingdom(string kname)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "ck", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kname, false);
		if (kingdom == null)
		{
			Debug.Log("Kingdom not found");
			return;
		}
		realm.SetKingdom(kingdom.id, false, true, true, true, true);
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x000A3210 File Offset: 0x000A1410
	[ConsoleMethod("conquer_sr", "Conquer a single realm of selected kingdom")]
	public void ConquerKingdomRealm()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "conquer_sr", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.SelKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No selected kingdom.");
			return;
		}
		if (kingdom2.IsDefeated())
		{
			return;
		}
		kingdom2.realms[0].SetKingdom(kingdom.id, false, true, false, true, true);
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x000A327C File Offset: 0x000A147C
	[ConsoleMethod("conquer", "Conquer selected kingdom")]
	public void ConquerKingdom()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "conquer", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.SelKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No selected kingdom.");
			return;
		}
		using (new Logic.Kingdom.CacheRBS("conquer cheat"))
		{
			Logic.Realm[] array = kingdom2.realms.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetKingdom(kingdom.id, false, true, false, true, true);
			}
		}
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x000A3320 File Offset: 0x000A1520
	[ConsoleMethod("conquer_by", "Selected kingdom is conquered by typed kingdom's name")]
	public void ConquerKingdom(string byKingdomName)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "conquer_by", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(byKingdomName, false);
		if (kingdom == null)
		{
			Debug.Log("Kingdom {defeatedKingdomName} not found!");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.SelKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No selected kingdom!");
			return;
		}
		using (new Logic.Kingdom.CacheRBS("conquer_by cheat"))
		{
			Logic.Realm[] array = kingdom2.realms.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetKingdom(kingdom.id, false, true, false, true, true);
			}
		}
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x000A33C8 File Offset: 0x000A15C8
	[ConsoleMethod("conquer_rest", "Conquer all other kingdoms but the selected one")]
	public void ConquerRest()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "conquer_rest", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.SelKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No selected kingdom.");
			return;
		}
		using (new Logic.Kingdom.CacheRBS("conquer_rest cheat"))
		{
			for (int i = 0; i < kingdom.game.kingdoms.Count; i++)
			{
				Logic.Kingdom kingdom3 = kingdom.game.kingdoms[i];
				if (kingdom3 != null && !kingdom3.IsDefeated() && kingdom3 != kingdom2 && kingdom3 != kingdom)
				{
					Logic.Realm[] array = kingdom3.realms.ToArray();
					for (int j = 0; j < array.Length; j++)
					{
						array[j].SetKingdom(kingdom.id, false, true, false, true, true);
					}
				}
			}
		}
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x000A34B4 File Offset: 0x000A16B4
	[ConsoleMethod("resreg", "Regenerate resources")]
	public void RegenerateResources()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "resreg", true))
		{
			return;
		}
		ResourcesView resourcesView = ViewMode.current as ResourcesView;
		if (resourcesView == null)
		{
			Debug.Log("You need to be in resources view to use this command.");
			return;
		}
		resourcesView.RandomizeColors();
		resourcesView.Apply();
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x000A34F8 File Offset: 0x000A16F8
	[ConsoleMethod("lastWarPeace", "Last war peace point operation")]
	public void LastWarPeace()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Kingdom kingdom2 = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("Error. Cant find logic(own) kingdom");
			return;
		}
		if (kingdom2 == null)
		{
			Debug.Log("No kingdom selected");
			return;
		}
		this.LastWarPeace(kingdom.id, kingdom2.id);
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x000A353F File Offset: 0x000A173F
	[ConsoleMethod("lastWarPeace", "Last war peace point operation")]
	public void LastWarPeace(int kid1, int kid2)
	{
		Game.CheckCheatLevel(Game.CheatLevel.Low, "lastWarPeace", true);
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x000A354E File Offset: 0x000A174E
	[ConsoleMethod("relset", "Set relationsip with selected kingdom to given value")]
	public void SetRelationship(int val)
	{
		this.SetRelationship(val, 0);
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x000A3558 File Offset: 0x000A1758
	[ConsoleMethod("relset", "Set relationsip with selected kingdom to given value")]
	public void SetRelationship(int val_perm, int val_temp)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "relset", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		KingdomAndKingdomRelation.AddRelationship(kingdom2, kingdom, (float)val_perm, (float)val_temp, null, "set");
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x000A35B0 File Offset: 0x000A17B0
	[ConsoleMethod("rel", "Check relationsip with selected kingdom")]
	public void CheckRelationship()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rel", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		float num;
		float num2;
		float relationshipEx = kingdom2.GetRelationshipEx(kingdom, out num, out num2);
		Debug.Log(string.Format("Relationship: {0}:{1} ({2}:{3})", new object[]
		{
			kingdom.Name,
			relationshipEx,
			num,
			num2
		}));
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x000A363C File Offset: 0x000A183C
	[ConsoleMethod("rel", "Check relationsip between selected kingdom and other kingdom")]
	public void CheckRelationship(string kingdom_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rel", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		if (BaseUI.LogicKingdom() == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = this.FindKingdom(kingdom_name, false);
		if (kingdom2 == null)
		{
			Debug.LogError("Invalid kingdom name");
			return;
		}
		float num;
		float num2;
		float relationshipEx = kingdom.GetRelationshipEx(kingdom2, out num, out num2);
		Debug.Log(string.Format("Relationship: {0}<->{1}:{2} ({3}:{4})", new object[]
		{
			kingdom.Name,
			kingdom2.Name,
			relationshipEx,
			num,
			num2
		}));
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x000A36E4 File Offset: 0x000A18E4
	[ConsoleMethod("rel_times", "Check relationsip times between selected kingdom and other kingdom")]
	public void CheckRelationshipTimes()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rel", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		KingdomAndKingdomRelation kingdomAndKingdomRelation = KingdomAndKingdomRelation.Get(kingdom, kingdom2, true, false);
		Game game = kingdom.game;
		Debug.Log("fade_time: " + (game.time - kingdomAndKingdomRelation.fade_time));
		Debug.Log("last_rel_change_time: " + (game.time - kingdomAndKingdomRelation.last_rel_change_time));
		Debug.Log("marriage_time: " + (game.time - kingdomAndKingdomRelation.marriage_time));
		Debug.Log("nap_broken_king_death_time: " + (game.time - kingdomAndKingdomRelation.nap_broken_king_death_time));
		Debug.Log("nap_time: " + (game.time - kingdomAndKingdomRelation.nap_time));
		Debug.Log("peace_time: " + (game.time - kingdomAndKingdomRelation.peace_time));
		Debug.Log("alliance_time: " + (game.time - kingdomAndKingdomRelation.alliance_time));
		Debug.Log("stance_time: " + (game.time - kingdomAndKingdomRelation.stance_time));
		Debug.Log("trade_time: " + (game.time - kingdomAndKingdomRelation.trade_time));
		Debug.Log("vassalage_time: " + (game.time - kingdomAndKingdomRelation.vassalage_time));
		Debug.Log("war_time: " + (game.time - kingdomAndKingdomRelation.war_time));
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x000A38D0 File Offset: 0x000A1AD0
	[ConsoleMethod("ca", "Set crown authority")]
	public void SetCrownAuthority(int val)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ca", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		kingdom.GetCrownAuthority().SetValue(val, true);
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x000A3918 File Offset: 0x000A1B18
	[ConsoleMethod("co", "Change opinion (name, amount)")]
	public void ChangeOpinion(string name, int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "co", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		List<Opinion.Def> all = Opinion.Def.all;
		Opinion.Def def = null;
		for (int i = 0; i < all.Count; i++)
		{
			Opinion.Def def2 = all[i];
			if (def2.id.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				def = def2;
				break;
			}
		}
		if (def == null)
		{
			Debug.LogError("Unknown opinion");
			return;
		}
		Debug.Log(string.Format("Changing {0} of {1} by {2}", def.id, kingdom.Name, amount));
		kingdom.opinions.Find(def.id).Modify((float)amount, "cheat");
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x000A39D5 File Offset: 0x000A1BD5
	[ConsoleMethod("cdp", "Create defensive pact")]
	public void CreateDefensivePact(string target_kingdom_name)
	{
		this.CreatePact(Pact.Type.Defensive, target_kingdom_name);
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x000A39DF File Offset: 0x000A1BDF
	[ConsoleMethod("cdp", "Create defensive pact")]
	public void CreateDefensivePact(string leader_kingdom_name, string target_kingdom_name)
	{
		this.CreatePact(Pact.Type.Defensive, leader_kingdom_name, target_kingdom_name);
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x000A39EA File Offset: 0x000A1BEA
	[ConsoleMethod("cdp", "Create defensive pact")]
	public void CreateDefensivePact()
	{
		this.CreatePact(Pact.Type.Defensive);
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x000A39F3 File Offset: 0x000A1BF3
	[ConsoleMethod("cop", "Create offensive pact")]
	public void CreateOffensivePact(string target_kingdom_name)
	{
		this.CreatePact(Pact.Type.Offensive, target_kingdom_name);
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x000A39FD File Offset: 0x000A1BFD
	[ConsoleMethod("cop", "Create offfensive pact")]
	public void CreateOfffensivePact(string leader_kingdom_name, string target_kingdom_name)
	{
		this.CreatePact(Pact.Type.Offensive, leader_kingdom_name, target_kingdom_name);
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x000A3A08 File Offset: 0x000A1C08
	[ConsoleMethod("cop", "Create offensive pact")]
	public void CreateOffensivePact()
	{
		this.CreatePact(Pact.Type.Offensive);
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x000A3A11 File Offset: 0x000A1C11
	[ConsoleMethod("jdp", "Join Defensive Pact")]
	public void JoinDefensivePact(string kingdom_name, string member_name, string target_name)
	{
		this.JoinPact(Pact.Type.Defensive, kingdom_name, member_name, target_name);
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x000A3A1D File Offset: 0x000A1C1D
	[ConsoleMethod("jop", "Join Offfensive Pact")]
	public void JoinOfffensivePact(string kingdom_name, string member_name, string target_name)
	{
		this.JoinPact(Pact.Type.Offensive, kingdom_name, member_name, target_name);
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x000A3A2C File Offset: 0x000A1C2C
	[ConsoleMethod("jp", "Join pact")]
	public void JoinPact(string kingdom_name)
	{
		Logic.Kingdom kingdom = this.FindKingdom(kingdom_name, false);
		if (kingdom == null)
		{
			Debug.LogError("Invalid kingdom name");
			return;
		}
		this.JoinPact(kingdom);
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x000A3A58 File Offset: 0x000A1C58
	[ConsoleMethod("jp", "Join pact")]
	public void JoinPact()
	{
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		this.JoinPact(kingdom);
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x000A3A80 File Offset: 0x000A1C80
	[ConsoleMethod("lp", "Leave pact")]
	public void LeavePact(string kingdom_name)
	{
		Logic.Kingdom kingdom = this.FindKingdom(kingdom_name, false);
		if (kingdom == null)
		{
			Debug.LogError("Invalid kingdom name");
			return;
		}
		this.LeavePact(kingdom);
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x000A3AAB File Offset: 0x000A1CAB
	[ConsoleMethod("ldp", "Leave Defensive Pact")]
	public void LeaveDefensivePact(string kingdom_name, string target_name)
	{
		this.LeavePact(Pact.Type.Defensive, kingdom_name, target_name);
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x000A3AB6 File Offset: 0x000A1CB6
	[ConsoleMethod("lop", "Join Offfensive Pact")]
	public void LeaveOfffensivePact(string kingdom_name, string target_name)
	{
		this.LeavePact(Pact.Type.Offensive, kingdom_name, target_name);
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x000A3AC4 File Offset: 0x000A1CC4
	[ConsoleMethod("lp", "Leave pact")]
	public void LeavePact()
	{
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		this.LeavePact(kingdom);
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x000A3AEC File Offset: 0x000A1CEC
	private Logic.Character GetDiplomatForNewPact(Logic.Kingdom k)
	{
		if (((k != null) ? k.court : null) == null)
		{
			return null;
		}
		for (int i = 0; i < k.court.Count; i++)
		{
			Logic.Character character = k.court[i];
			if (character != null && character.IsDiplomat() && character.pact == null)
			{
				return character;
			}
		}
		Logic.Character character2 = CharacterFactory.CreateCourtCandidate(k.game, k.id, "Diplomat");
		k.AddCourtMember(character2, -1, false, true, false, true);
		return character2;
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x000A3B68 File Offset: 0x000A1D68
	private void CreatePact(Pact.Type type)
	{
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		this.CreatePact(type, kingdom);
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x000A3B94 File Offset: 0x000A1D94
	private void CreatePact(Pact.Type type, string leader_kingdom_name, string target_kingdom_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "create pact", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(leader_kingdom_name, false);
		if (kingdom == null)
		{
			Debug.LogError("Invalid kingdom name: " + leader_kingdom_name);
			return;
		}
		Logic.Kingdom kingdom2 = this.FindKingdom(target_kingdom_name, false);
		if (kingdom2 == null)
		{
			Debug.LogError("Invalid kingdom name: " + target_kingdom_name);
			return;
		}
		Logic.Character diplomatForNewPact = this.GetDiplomatForNewPact(kingdom);
		if (diplomatForNewPact == null)
		{
			Debug.LogError(kingdom.Name + " doesn't have a suitable diplomat");
			return;
		}
		Pact pact = Pact.Create(type, diplomatForNewPact, kingdom2);
		if (pact == null)
		{
			Debug.LogError("Failed to create defensive pact against " + kingdom2.Name);
			return;
		}
		Debug.Log(string.Format("Created {0}", pact));
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x000A3C40 File Offset: 0x000A1E40
	private void CreatePact(Pact.Type type, string target_kingdom_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "create pact", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(target_kingdom_name, false);
		if (kingdom == null)
		{
			Debug.LogError("Invalid kingdom name");
			return;
		}
		this.CreatePact(type, kingdom);
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x000A3C7C File Offset: 0x000A1E7C
	private void CreatePact(Pact.Type type, Logic.Kingdom tgt_kingdom)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "create pact", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null || !character.IsDiplomat() || character.pact != null)
		{
			character = this.GetDiplomatForNewPact(BaseUI.LogicKingdom());
			if (character == null)
			{
				Debug.LogError("No suitable diplomat");
				return;
			}
		}
		Pact pact = Pact.Create(type, character, tgt_kingdom);
		if (pact == null)
		{
			Debug.LogError("Failed to create defensive pact against " + tgt_kingdom.Name);
			return;
		}
		Debug.Log(string.Format("Created {0}", pact));
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x000A3D00 File Offset: 0x000A1F00
	private void JoinPact(Pact.Type type, string kingdom_name, string member_name, string target_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "join pact", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kingdom_name, false);
		if (kingdom == null)
		{
			Debug.LogError("Invalid kingdom name: " + kingdom_name);
			return;
		}
		Logic.Kingdom kingdom2 = this.FindKingdom(member_name, false);
		if (kingdom2 == null)
		{
			Debug.LogError("Invalid kingdom name: " + member_name);
			return;
		}
		Logic.Kingdom kingdom3 = this.FindKingdom(target_name, false);
		if (kingdom3 == null)
		{
			Debug.LogError("Invalid kingdom name: " + target_name);
			return;
		}
		Pact pact = Pact.Find(type, kingdom2, kingdom3);
		if (pact == null)
		{
			Debug.LogError(string.Format("{0} doesn't have a {1} Pact against {2}", kingdom2.Name, type, kingdom3.Name));
			return;
		}
		if (!pact.Join(kingdom, true))
		{
			Debug.LogError(string.Format("Failed to join {0} to {1}", kingdom.Name, pact));
			return;
		}
		Debug.Log(string.Format("Joined {0} to {1}", kingdom.Name, pact));
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x000A3DDC File Offset: 0x000A1FDC
	private void LeavePact(Pact.Type type, string kingdom_name, string target_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "leave pact", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kingdom_name, false);
		if (kingdom == null)
		{
			Debug.LogError("Invalid kingdom name: " + kingdom_name);
			return;
		}
		Logic.Kingdom kingdom2 = this.FindKingdom(target_name, false);
		if (kingdom2 == null)
		{
			Debug.LogError("Invalid kingdom name: " + target_name);
			return;
		}
		Pact pact = Pact.Find(type, kingdom, kingdom2);
		if (pact == null)
		{
			Debug.LogError(string.Format("{0} doesn't have a {1} Pact against {2}", kingdom.Name, type, kingdom2.Name));
			return;
		}
		if (!pact.Leave(kingdom, null, true))
		{
			Debug.LogError(string.Format("Failed to leave {0} to {1}", kingdom.Name, pact));
			return;
		}
		Debug.Log(string.Format("Left {0} from {1}", kingdom.Name, pact));
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x000A3E98 File Offset: 0x000A2098
	private void JoinPact(Logic.Kingdom k)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "join pact", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.LogError("No character selected");
			return;
		}
		Pact pact = character.pact;
		if (pact == null)
		{
			Debug.LogError(string.Format("{0} has no pact", character));
			return;
		}
		if (!pact.Join(k, true))
		{
			Debug.LogError(string.Format("Failed to add {0} to {1}", k.Name, pact));
			return;
		}
		Debug.Log(string.Format("Joined {0} to {1}", k.Name, pact));
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x000A3F1C File Offset: 0x000A211C
	private void LeavePact(Logic.Kingdom k)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "leave pact", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.LogError("No character selected");
			return;
		}
		Pact pact = character.pact;
		if (pact == null)
		{
			Debug.LogError(string.Format("{0} has no pact", character));
			return;
		}
		if (!pact.Leave(k, null, true))
		{
			Debug.LogError(string.Format("Failed to leave {0} from {1}", k.Name, pact));
			return;
		}
		Debug.Log(string.Format("{0} left {1}", k.Name, pact));
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x000A3FA0 File Offset: 0x000A21A0
	[ConsoleMethod("war", "Start war")]
	public void StartWarBetween()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "war", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		if (kingdom2 == null)
		{
			Debug.Log("No logic kingdom");
			return;
		}
		if (kingdom.IsEnemy(kingdom2))
		{
			Debug.Log("Already in war");
			return;
		}
		kingdom.StartWarWith(kingdom2, War.InvolvementReason.InternalPurposes, "WarDeclaredMessage", null, true);
		Debug.Log(string.Concat(new string[]
		{
			"War between",
			kingdom2.Name,
			" and ",
			kingdom.Name,
			" started"
		}));
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x000A4044 File Offset: 0x000A2244
	[ConsoleMethod("war", "Start war")]
	public void StartWarBetween(string kName)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "war", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No logic kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = kingdom.game.GetKingdom(kName);
		if (kingdom2 == null)
		{
			Debug.Log(kName + " is not a valid kingdom name");
			return;
		}
		if (kingdom2.IsEnemy(kingdom))
		{
			Debug.Log("Already in war");
			return;
		}
		kingdom2.StartWarWith(kingdom, War.InvolvementReason.InternalPurposes, "WarDeclaredMessage", null, true);
		Debug.Log(string.Concat(new string[]
		{
			"War between",
			kingdom.Name,
			" and ",
			kingdom2.Name,
			" started"
		}));
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x000A40F4 File Offset: 0x000A22F4
	[ConsoleMethod("war", "Start war")]
	public void StartWarBetween(string kName1, string kName2)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "war", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kName1, false);
		if (kingdom == null)
		{
			Debug.Log(kName1 + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom2 = this.FindKingdom(kName2, false);
		if (kingdom2 == null)
		{
			Debug.Log(kName2 + " is not a valid kingdom name");
			return;
		}
		if (kingdom.StartWarWith(kingdom2, War.InvolvementReason.InternalPurposes, "WarDeclaredMessage", null, true) != null)
		{
			Debug.Log(string.Concat(new string[]
			{
				"War between ",
				kingdom.Name,
				" and ",
				kingdom2.Name,
				" started"
			}));
			return;
		}
		Debug.Log("Could not start war between " + kingdom.Name + " and " + kingdom2.Name);
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x000A41B8 File Offset: 0x000A23B8
	[ConsoleMethod("jw", "Join war")]
	public void JoinWar(string kName1, string kName2, string kName3)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "jw", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kName1, false);
		if (kingdom == null)
		{
			Debug.Log(kName1 + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom2 = this.FindKingdom(kName2, false);
		if (kingdom2 == null)
		{
			Debug.Log(kName2 + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom3 = this.FindKingdom(kName3, false);
		if (kingdom3 == null)
		{
			Debug.Log(kName3 + " is not a valid kingdom name");
			return;
		}
		War war = kingdom2.FindWarWith(kingdom3);
		if (war == null)
		{
			Debug.Log(string.Concat(new string[]
			{
				"No war between ",
				kingdom2.Name,
				" and ",
				kingdom3.Name,
				" found"
			}));
			return;
		}
		int side = war.GetSide(kingdom2);
		if (!war.CanJoin(kingdom, side))
		{
			Debug.Log(string.Concat(new string[]
			{
				kingdom.Name,
				" cannot support ",
				kingdom2.Name,
				" against ",
				kingdom3.Name
			}));
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"Adding ",
			kingdom.Name,
			" as an ally of ",
			kingdom2.Name,
			" against ",
			kingdom3.Name
		}));
		war.Join(kingdom, side, War.InvolvementReason.InternalPurposes, true);
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x000A4314 File Offset: 0x000A2514
	[ConsoleMethod("war_mass", "Start mass war")]
	public void StartMassWar()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "war_mass", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		for (int i = 0; i < logic.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = logic.kingdoms[i];
			if (!kingdom.IsDefeated())
			{
				for (int j = 0; j < logic.kingdoms.Count; j++)
				{
					Logic.Kingdom kingdom2 = logic.kingdoms[j];
					if (!kingdom2.IsDefeated() && kingdom != kingdom2 && !kingdom.IsEnemy(kingdom2))
					{
						kingdom.StartWarWith(kingdom2, War.InvolvementReason.InternalPurposes, null, null, true);
					}
				}
			}
		}
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x000A43C0 File Offset: 0x000A25C0
	[ConsoleMethod("wars_count", "Count the number of wars globally")]
	public void CountGlobalWars()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "wars_count", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		int num = 0;
		logic.num_objects_by_type.TryGetValue(typeof(War), out num);
		Debug.Log("Total number of wars: " + num);
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x000A4424 File Offset: 0x000A2624
	[ConsoleMethod("kaw_count", "Count the number of kingdoms at war globally")]
	public void CountKingdomsAtWar()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "kaw_count", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		int kingdoms_at_war = logic.kingdoms_at_war;
		int num = 0;
		for (int i = 0; i < logic.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = logic.kingdoms[i];
			if (!kingdom.IsDefeated() && kingdom.wars.Count != 0)
			{
				num++;
			}
		}
		Debug.Log("Total number of kingdoms at war: " + kingdoms_at_war);
		if (num != kingdoms_at_war)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Inccorect numbers!!! Cached: ",
				kingdoms_at_war,
				" Calculated: ",
				num
			}));
		}
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x000A44EC File Offset: 0x000A26EC
	[ConsoleMethod("peace", "Declare peace")]
	public void DeclarePeaceBetween(string kName1, string kName2)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "peace", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kName1, false);
		if (kingdom == null)
		{
			Debug.Log(kName1 + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom2 = this.FindKingdom(kName2, false);
		if (kingdom2 == null)
		{
			Debug.Log(kName2 + " is not a valid kingdom name");
			return;
		}
		if (kingdom.EndWarWith(kingdom2, null, "cheat", false))
		{
			Debug.Log(string.Concat(new string[]
			{
				"Peace between ",
				kingdom.Name,
				" and ",
				kingdom2.Name,
				" signed"
			}));
			return;
		}
		Debug.Log("Cannot sign peace between " + kingdom.Name + " and " + kingdom2.Name);
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x000A45AD File Offset: 0x000A27AD
	[ConsoleMethod("peace_mass", "Start mass war")]
	public void StartMassPeace()
	{
		this.StartMassPeace(0);
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x000A45B8 File Offset: 0x000A27B8
	[ConsoleMethod("peace_mass", "Start mass war")]
	public void StartMassPeace(int max_out_relationships)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "peace_mass", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		for (int i = 0; i < logic.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = logic.kingdoms[i];
			if (!kingdom.IsDefeated())
			{
				for (int j = 0; j < logic.kingdoms.Count; j++)
				{
					Logic.Kingdom kingdom2 = logic.kingdoms[j];
					if (!kingdom2.IsDefeated() && kingdom != kingdom2)
					{
						if (kingdom.FindWarWith(kingdom2) != null)
						{
							kingdom.EndWarWith(kingdom2, null, "cheat", false);
						}
						if (max_out_relationships != 0)
						{
							KingdomAndKingdomRelation kingdomAndKingdomRelation = KingdomAndKingdomRelation.Get(kingdom, kingdom2, true, true);
							kingdomAndKingdomRelation.perm_relationship = 1000f;
							kingdomAndKingdomRelation.OnChanged(kingdom, kingdom2, true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x000A468C File Offset: 0x000A288C
	[ConsoleMethod("vassal", "Change vassal state")]
	public void SetVassal(string vassal_name, string liege_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "vassal", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(vassal_name, false);
		if (kingdom == null)
		{
			Debug.Log(vassal_name + " is not a valid kingdom name");
			return;
		}
		Logic.Kingdom kingdom2;
		if (liege_name == "null")
		{
			kingdom2 = null;
		}
		else
		{
			kingdom2 = this.FindKingdom(liege_name, false);
			if (kingdom2 == null)
			{
				Debug.Log(liege_name + " is not a valid kingdom name");
				return;
			}
		}
		if (kingdom == kingdom2)
		{
			Debug.LogError("Cannot make " + kingdom.Name + " vassal of themselves");
			return;
		}
		Debug.Log("Making " + kingdom.Name + " vassal of " + (((kingdom2 != null) ? kingdom2.Name : null) ?? "nobody"));
		Logic.Kingdom sovereignState = kingdom.sovereignState;
		if (sovereignState != null)
		{
			sovereignState.DelVassalState(kingdom, true, true);
		}
		if (kingdom2 != null)
		{
			kingdom2.AddVassalState(kingdom, true, true);
		}
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x000A4764 File Offset: 0x000A2964
	[ConsoleMethod("vassal", "Change vassal state")]
	public void SetVassal(string liege_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "vassal", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("no selected kingom");
			return;
		}
		Logic.Kingdom kingdom2;
		if (liege_name == "null")
		{
			kingdom2 = null;
		}
		else
		{
			kingdom2 = this.FindKingdom(liege_name, false);
			if (kingdom2 == null)
			{
				Debug.Log(liege_name + " is not a valid kingdom name");
				return;
			}
		}
		if (kingdom == kingdom2)
		{
			Debug.LogError("Cannot make " + kingdom.Name + " vassal of themselves");
			return;
		}
		Debug.Log("Making " + kingdom.Name + " vassal of " + (((kingdom2 != null) ? kingdom2.Name : null) ?? "nobody"));
		Logic.Kingdom sovereignState = kingdom.sovereignState;
		if (sovereignState != null)
		{
			sovereignState.DelVassalState(kingdom, true, true);
		}
		if (kingdom2 != null)
		{
			kingdom2.AddVassalState(kingdom, true, true);
		}
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x000A483C File Offset: 0x000A2A3C
	[ConsoleMethod("vassal", "Change vassal state")]
	public void SetVassal()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "vassal", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("no selected kingom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("no player kingdom");
			return;
		}
		if (kingdom == kingdom2)
		{
			Debug.LogError("Cannot make " + kingdom.Name + " vassal of themselves");
			return;
		}
		Debug.Log("Making " + kingdom.Name + " vassal of " + (((kingdom2 != null) ? kingdom2.Name : null) ?? "nobody"));
		Logic.Kingdom sovereignState = kingdom.sovereignState;
		if (sovereignState != null)
		{
			sovereignState.DelVassalState(kingdom, true, true);
		}
		if (kingdom2 != null)
		{
			kingdom2.AddVassalState(kingdom, true, true);
		}
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x000A48F0 File Offset: 0x000A2AF0
	[ConsoleMethod("trade", "Set trade")]
	public void SetTrade(int i)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "trade", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelLO() as Logic.Kingdom;
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 != null && kingdom != null)
		{
			if (i == 0)
			{
				kingdom2.UnsetStance(kingdom, RelationUtils.Stance.Trade, null, true);
				return;
			}
			kingdom2.SetStance(kingdom, RelationUtils.Stance.Trade, null, true);
		}
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x000A4944 File Offset: 0x000A2B44
	[ConsoleMethod("trade_me", "Set trade, in reverse direction")]
	public void SetTradeMe(int i)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "trade_me", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelLO() as Logic.Kingdom;
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 != null && kingdom != null)
		{
			if (i == 0)
			{
				kingdom.UnsetStance(kingdom2, RelationUtils.Stance.Trade, null, true);
				return;
			}
			kingdom.SetStance(kingdom2, RelationUtils.Stance.Trade, null, true);
		}
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x000A4998 File Offset: 0x000A2B98
	[ConsoleMethod("trade_mass", "Start mass trade agreements")]
	public void StartMassTrade()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "trade_mass", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		for (int i = 0; i < logic.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = logic.kingdoms[i];
			if (!kingdom.IsDefeated())
			{
				for (int j = 0; j < logic.kingdoms.Count; j++)
				{
					Logic.Kingdom kingdom2 = logic.kingdoms[j];
					if (!kingdom2.IsDefeated() && kingdom != kingdom2 && !kingdom.IsEnemy(kingdom2))
					{
						kingdom.SetStance(kingdom2, RelationUtils.Stance.Trade, null, true);
					}
				}
			}
		}
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x000A4A44 File Offset: 0x000A2C44
	[ConsoleMethod("kingdoms_count", "Count the number of active kingdoms")]
	public void CountActiveKingdoms()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "kingdoms_count", true))
		{
			return;
		}
		Game logic = GameLogic.instance.logic;
		if (logic == null)
		{
			Debug.Log("No Game object!");
			return;
		}
		float num = 0f;
		for (int i = 0; i < logic.kingdoms.Count; i++)
		{
			if (!logic.kingdoms[i].IsDefeated())
			{
				num += 1f;
			}
		}
		Debug.Log("Total number of active kingdoms: " + num);
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x000A4AC5 File Offset: 0x000A2CC5
	[ConsoleMethod("dgr", "Dump Game Rules")]
	public void DumpGameRules()
	{
		this.DumpGameRulesWithFilter(null);
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x000A4ACE File Offset: 0x000A2CCE
	[ConsoleMethod("dgr", "Dump Game Rules")]
	public void DumpGameRules(string filter)
	{
		this.DumpGameRulesWithFilter(filter);
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x000A4AD7 File Offset: 0x000A2CD7
	private void DumpGameRulesWithFilter(string filter)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dump game rules", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		object obj;
		if (game == null)
		{
			obj = null;
		}
		else
		{
			GameRules game_rules = game.game_rules;
			obj = ((game_rules != null) ? game_rules.DumpRules(filter) : null);
		}
		object obj2 = obj;
		Debug.Log(obj2);
		Game.CopyToClipboard(obj2);
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x000A4B12 File Offset: 0x000A2D12
	[ConsoleMethod("dgrl", "Dump Game Rule Listeners")]
	public void DumpGameRuleListeners()
	{
		this.DumpGameRuleListenersWithFilter(null);
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x000A4B1B File Offset: 0x000A2D1B
	[ConsoleMethod("dgrl", "Dump Game Rule Listeners")]
	public void DumpGameRuleListeners(string filter)
	{
		this.DumpGameRuleListenersWithFilter(filter);
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x000A4B24 File Offset: 0x000A2D24
	[ConsoleMethod("dgrl", "Dump Game Rule Listeners")]
	public void DumpGameRuleListeners(string filter1, string filter2)
	{
		this.DumpGameRuleListenersWithFilter(filter1 + " " + filter2);
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x000A4B38 File Offset: 0x000A2D38
	private void DumpGameRuleListenersWithFilter(string filter)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dump game rule listeners", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		object obj;
		if (game == null)
		{
			obj = null;
		}
		else
		{
			GameRules game_rules = game.game_rules;
			obj = ((game_rules != null) ? game_rules.Dump(filter) : null);
		}
		object obj2 = obj;
		Debug.Log(obj2);
		Game.CopyToClipboard(obj2);
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x000A4B74 File Offset: 0x000A2D74
	[ConsoleMethod("pcstats", "Show pros and cons stats")]
	public void DumpPCStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pcstats", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (ProsAndCons.Tracker.stats == null)
		{
			Debug.Log("PC stats not available (table is NULL)");
			return;
		}
		string text = ProsAndCons.Tracker.Dump(game, ProsAndCons.Tracker.stats);
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x000A4BC4 File Offset: 0x000A2DC4
	[ConsoleMethod("pcplstats", "Show pros and cons player stats")]
	public void DumpPCPlayerStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pcstats", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (ProsAndCons.Tracker.stats_player == null)
		{
			Debug.Log("PC stats not available (table is NULL)");
			return;
		}
		string text = ProsAndCons.Tracker.Dump(game, ProsAndCons.Tracker.stats_player);
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x000A4C13 File Offset: 0x000A2E13
	[ConsoleMethod("pclw", "Show pros and cons stats for last war against player")]
	public void DumpPCLWStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pclw", true))
		{
			return;
		}
		if (ProsAndCons.Tracker.last_factors_cons == null && ProsAndCons.Tracker.last_factors_pros == null)
		{
			Debug.Log("PC last war stats not available (array is NULL)");
			return;
		}
		string text = ProsAndCons.Tracker.DumpLastPlayerWar();
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x000A4C50 File Offset: 0x000A2E50
	[ConsoleMethod("dstats", "Show diplomacy stats")]
	public void DumpDiplomacyStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dstats", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		DevCheats.DiplomacyStats diplomacyStats = new DevCheats.DiplomacyStats();
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom k = game.kingdoms[i];
			diplomacyStats.AddKingdom(k);
		}
		Debug.Log(diplomacyStats.Dump());
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x000A4CB4 File Offset: 0x000A2EB4
	[ConsoleMethod("rstats", "Show rebellion stats")]
	public void DumpRebelStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "rstats", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		float num10 = 0f;
		float num11 = 0f;
		for (int i = 1; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (!kingdom.IsDefeated())
			{
				num++;
				num11 += (float)kingdom.GetCrownAuthority().GetValue();
				num10 += kingdom.stability.GetStability();
				List<Rebellion> rebellions = kingdom.rebellions;
				if (rebellions.Count == 0)
				{
					num3++;
				}
				else
				{
					num2++;
					num4 += rebellions.Count;
					if (num8 < rebellions.Count)
					{
						num8 = rebellions.Count;
					}
					for (int j = 0; j < rebellions.Count; j++)
					{
						if (rebellions[j].famous)
						{
							num6++;
						}
						List<Logic.Rebel> rebels = rebellions[j].rebels;
						num5 += rebels.Count;
						if (num9 < rebels.Count)
						{
							num9 = rebels.Count;
						}
						for (int k = 0; k < rebels.Count; k++)
						{
							Logic.Rebel rebel = rebels[k];
							if (rebel.IsLeader() && rebel.IsLoyalist())
							{
								num7++;
							}
						}
					}
				}
			}
		}
		float num12 = (float)Math.Round((double)((float)num5 / ((num2 > 0) ? ((float)num2) : 1f)), 2);
		float num13 = (float)Math.Round((double)((float)num5 / ((num4 > 0) ? ((float)num4) : 1f)), 2);
		float num14 = (float)Math.Round((double)((float)num4 / ((num2 > 0) ? ((float)num2) : 1f)), 2);
		float num15 = (float)Math.Round((double)(num10 / (float)num), 2);
		float num16 = (float)Math.Round((double)(num11 / (float)num), 2);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("TOTAL:");
		stringBuilder.AppendLine(string.Format("  kingdoms: {0}", num));
		stringBuilder.AppendLine(string.Format("  with rebellions: {0}", num2));
		stringBuilder.AppendLine(string.Format("  rebellions: {0}", num4));
		stringBuilder.AppendLine(string.Format("    loyalist: {0}", num7));
		stringBuilder.AppendLine(string.Format("    famous: {0}", num6));
		stringBuilder.AppendLine(string.Format("    rebels: {0}", num5));
		stringBuilder.AppendLine("AVERAGE:");
		stringBuilder.AppendLine(string.Format("  rebellions: {0}", num14));
		stringBuilder.AppendLine(string.Format("  rebels in kingdom: {0}", num12));
		stringBuilder.AppendLine(string.Format("  rebels in rebellion: {0}", num13));
		stringBuilder.AppendLine(string.Format("  crown authority: {0}", num16));
		stringBuilder.AppendLine(string.Format("  stability: {0}", num15));
		stringBuilder.AppendLine("ALSO:");
		stringBuilder.AppendLine(string.Format("  Max rebellions in Kingdom: {0}", num8));
		stringBuilder.AppendLine(string.Format("  Max rebels in rebellion: {0}", num9));
		int num17 = 0;
		game.num_objects_by_type.TryGetValue(typeof(Rebellion), out num17);
		stringBuilder.AppendLine(string.Format("  Real rebellion count: {0}", num17));
		game.num_objects_by_type.TryGetValue(typeof(Logic.Rebel), out num17);
		stringBuilder.AppendLine(string.Format("  Real rebel count: {0}", num17));
		string text = stringBuilder.ToString();
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x000A50A4 File Offset: 0x000A32A4
	[ConsoleMethod("astats", "Show actions stats")]
	public void DumpActionsStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "astats", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (Action.Tracker.stats == null)
		{
			Debug.Log("Action stats not available (table is NULL)");
			return;
		}
		string text = Action.Tracker.Dump(game, Action.Tracker.stats);
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x000A50F3 File Offset: 0x000A32F3
	[ConsoleMethod("ddna", "0 | 1 - disable / enable DeltaDNA")]
	public void EnableDDNA(int enable)
	{
		Analytics.EnableDDNA(enable != 0);
		Debug.Log("DeltaDNA " + ((enable == 0) ? "disabled" : "enabled"));
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x000A511C File Offset: 0x000A331C
	[ConsoleMethod("thqgds", "0 | 1 - disable / enable THQ GDS")]
	public void EnableTHQGDS(int enable)
	{
		Analytics.EnableTHQGDS(enable != 0);
		Debug.Log("THQ GDS " + ((enable == 0) ? "disabled" : "enabled"));
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x000A5148 File Offset: 0x000A3348
	[ConsoleMethod("throw", "throw an exception")]
	public void ThrowException(string type)
	{
		if (type == "null")
		{
			Debug.Log("Chuck Norris can reference null objects!");
			((object)null).ToString();
			return;
		}
		if (type == "stack")
		{
			Debug.Log("Chuck Norris has unlimited stack!");
			this.CauseStackOverflow();
			return;
		}
		if (type == "error")
		{
			Debug.LogError("Chuck Norris makes no errors!");
			return;
		}
		if (type == "warning")
		{
			Debug.LogWarning("Chuck Norris cannot be warned!");
			return;
		}
		if (!(type == "crash"))
		{
			return;
		}
		Debug.Log("Chuck Norris can easily crash Unity!");
		Utils.ForceCrash(ForcedCrashCategory.AccessViolation);
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x000A51E0 File Offset: 0x000A33E0
	private void CauseStackOverflow()
	{
		this.CauseStackOverflow();
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x000A51E8 File Offset: 0x000A33E8
	private unsafe void CauseCrash()
	{
		byte* ptr = null;
		*ptr = 0;
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x000A51FB File Offset: 0x000A33FB
	[ConsoleMethod("rp", "enable / disable Rich Presence")]
	public void EnableRichPresence(int enable)
	{
		RichPresence.enabled = (enable != 0);
		if (RichPresence.enabled)
		{
			Debug.Log("Rich Presence enabled");
		}
		else
		{
			Debug.Log("Rich Presence disabled");
		}
		if (RichPresence.enabled)
		{
			RichPresence.Update(RichPresence.state);
		}
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x000A5238 File Offset: 0x000A3438
	private void ChangeKingdom(global::Kingdom kingdom)
	{
		Logic.Kingdom kingdom2 = (kingdom != null) ? kingdom.logic : null;
		if (kingdom2 == null || kingdom2.game == null)
		{
			return;
		}
		if (kingdom2.game.multiplayer == null || kingdom2.game.multiplayer.playerData == null)
		{
			return;
		}
		kingdom2.game.SetAnyKingdom(kingdom2.game.multiplayer.playerData.pid, kingdom2, false);
		BaseUI.Get().groups.Clear();
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x000A52B0 File Offset: 0x000A34B0
	[ConsoleMethod("switch", "Change kingdom")]
	public void ChangeKingdom(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "switch", true))
		{
			return;
		}
		int id;
		if (int.TryParse(name, out id))
		{
			this.ChangeKingdom(global::Kingdom.Get(id));
			return;
		}
		foreach (Logic.Kingdom kingdom in GameLogic.Get(true).kingdoms)
		{
			if (kingdom.Name.ToLowerInvariant() == name.ToLowerInvariant())
			{
				this.ChangeKingdom(kingdom.visuals as global::Kingdom);
				WorldMap worldMap = WorldMap.Get();
				if (worldMap == null)
				{
					break;
				}
				worldMap.ReloadView();
				break;
			}
		}
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x000A5364 File Offset: 0x000A3564
	[ConsoleMethod("switch", "Change kingdom to selected kingdom")]
	public void ChangeKingdom()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "switch", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object != null)
		{
			this.ChangeKingdom(global::Kingdom.Get(@object.GetKingdom().id));
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x000A53A0 File Offset: 0x000A35A0
	[ConsoleMethod("fow", "Enable/Disable fog of war")]
	public void EnableFOW(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "fow", true))
		{
			return;
		}
		List<VerticalTint> list = new List<VerticalTint>();
		CameraController cameraController = CameraController.Get();
		global::Common.FindChildrenWithComponent<VerticalTint>((cameraController != null) ? cameraController.gameObject : null, list);
		if (list == null || list.Count != 1)
		{
			Debug.Log("Command failed. None or more than one BSG post process scripts exist in the scene.");
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			Debug.Log("Command failed. No game.");
			return;
		}
		List<Logic.Realm> realms = game.realms;
		if (realms == null)
		{
			Debug.Log("Command failed. No game realms.");
			return;
		}
		if (game.fow && enable == 0)
		{
			list[0].showFoW = false;
			game.fow = false;
			for (int i = 0; i < realms.Count; i++)
			{
				global::Realm realm = realms[i].visuals as global::Realm;
				if (realm != null)
				{
					realm.UpdateFow(true, true);
				}
			}
		}
		else if (!game.fow && enable != 0)
		{
			list[0].showFoW = true;
			game.fow = true;
			for (int j = 0; j < realms.Count; j++)
			{
				global::Realm realm2 = realms[j].visuals as global::Realm;
				if (realm2 != null)
				{
					realm2.UpdateFow(false, true);
				}
			}
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			worldMap.ReloadView();
		}
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x000A54D7 File Offset: 0x000A36D7
	[ConsoleMethod("ls", "Show/Hide Loading Screen")]
	public void ShowLoadingScreen(int show)
	{
		LoadingScreen loadingScreen = LoadingScreen.Get();
		if (loadingScreen == null)
		{
			return;
		}
		loadingScreen.Show(show != 0, true, false);
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x000A54F0 File Offset: 0x000A36F0
	[ConsoleMethod("video_mode", "Enable/Disable video mode")]
	public void EnableVideoMode(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "video_mode", true))
		{
			return;
		}
		if (enable > 1)
		{
			Debug.Log("Spawning All Settlements before video mode...");
			this.SpawnAllSettlements();
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			Debug.Log("Command failed. No game.");
			return;
		}
		game.isInVideoMode = (enable != 0);
		this.ShowUI((enable > 0) ? 0 : 1);
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x000A554D File Offset: 0x000A374D
	[ConsoleMethod("rbars", "Enable/Disable resource bars over towns and settlements")]
	public void EnableResourceBars(int enable)
	{
		if (enable >= 2)
		{
			ResourceBar.hidden = 1;
			Debug.Log("Resource bars: Towns only");
			return;
		}
		if (enable == 1)
		{
			ResourceBar.hidden = 0;
			Debug.Log("Resource bars: Shown");
			return;
		}
		ResourceBar.hidden = 2;
		Debug.Log("Resource bars: Hidden");
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x000A5589 File Offset: 0x000A3789
	[ConsoleMethod("town_names", "Enable/Disable town_names")]
	public void EnableTownNames(int enable)
	{
		if (enable != 0)
		{
			global::Settlement.HideAllLabels(false);
			Debug.Log("Town names: Shown");
			return;
		}
		global::Settlement.HideAllLabels(true);
		Debug.Log("Town names: Hidden");
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x000A55AF File Offset: 0x000A37AF
	[ConsoleMethod("global_resources", "Use global (per-kingdom) resources")]
	public void UseGlobalResources(int global)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "global_resources", true))
		{
			return;
		}
		Building.global_resources = (global != 0);
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x000A55CC File Offset: 0x000A37CC
	[ConsoleMethod("all_tags", "Give selected kingdom all tags")]
	public void AllTags(int amount)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "all_tags", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		Logic.Kingdom kingdom = (@object != null) ? @object.GetKingdom() : null;
		if (kingdom == null)
		{
			kingdom = BaseUI.LogicKingdom();
		}
		if (kingdom == null)
		{
			return;
		}
		kingdom.all_tags = amount;
		kingdom.RecalcBuildingStates(null, false, false, false);
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x000A561C File Offset: 0x000A381C
	[ConsoleMethod("all_ranks", "Force selected kingdom's rank in all rankings")]
	public void AllRanks(int rank)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "all_ranks", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		Logic.Kingdom kingdom = (@object != null) ? @object.GetKingdom() : null;
		if (kingdom == null)
		{
			kingdom = BaseUI.LogicKingdom();
		}
		if (kingdom == null)
		{
			return;
		}
		kingdom.force_rank = rank;
		KingdomRankingCategories rankingCategories = kingdom.rankingCategories;
		List<KingdomRankingCategory> list = (rankingCategories != null) ? rankingCategories.categories : null;
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].ResetScore();
		}
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x000A5694 File Offset: 0x000A3894
	[ConsoleMethod("sas", "Spawn all settlements")]
	public void SpawnAllSettlements()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "sas", true))
		{
			return;
		}
		if (!Application.isPlaying)
		{
			Debug.Log("Not implemented in editor");
			return;
		}
		Game game = GameLogic.Get(true);
		int num = 0;
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < game.realms.Count; i++)
		{
			Logic.Realm realm = game.realms[i];
			for (int j = 0; j < realm.settlements.Count; j++)
			{
				global::Settlement settlement = realm.settlements[j].visuals as global::Settlement;
				if (!(settlement == null) && !settlement.was_visible)
				{
					num++;
					settlement.VisibilityChanged(true);
					settlement.VisibilityChanged(false);
				}
			}
		}
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		Debug.Log(string.Concat(new object[]
		{
			"Spawned ",
			num,
			" settlement(s) in ",
			elapsedMilliseconds,
			"ms"
		}));
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x000A5798 File Offset: 0x000A3998
	private static bool Match(string s, string mask)
	{
		int num = mask.IndexOf('*');
		if (num < 0)
		{
			num = s.IndexOf(mask, StringComparison.OrdinalIgnoreCase);
			return num >= 0;
		}
		string value = mask.Substring(0, num);
		if (!s.StartsWith(value, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		string value2 = mask.Substring(num + 1);
		return s.EndsWith(value2, StringComparison.OrdinalIgnoreCase);
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x000A57F0 File Offset: 0x000A39F0
	private static GameObject FindGameObject(GameObject parent, string name, bool match_parent)
	{
		int num = name.IndexOf('/');
		if (num >= 0)
		{
			string name2 = name.Substring(0, num);
			string name3 = name.Substring(num + 1);
			GameObject gameObject = DevCheats.FindGameObject(parent, name2, match_parent);
			if (gameObject == null)
			{
				return null;
			}
			return DevCheats.FindGameObject(gameObject, name3, false);
		}
		else
		{
			if (match_parent && DevCheats.Match(parent.name, name))
			{
				return parent;
			}
			for (int i = 0; i < parent.transform.childCount; i++)
			{
				GameObject gameObject2 = parent.transform.GetChild(i).gameObject;
				gameObject2 = DevCheats.FindGameObject(gameObject2, name, true);
				if (gameObject2 != null)
				{
					return gameObject2;
				}
			}
			return null;
		}
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x000A5894 File Offset: 0x000A3A94
	private static GameObject FindGameObject(Scene scene, string name)
	{
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			GameObject gameObject = DevCheats.FindGameObject(rootGameObjects[i], name, true);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x000A58D0 File Offset: 0x000A3AD0
	public static GameObject FindGameObjectByName(string name)
	{
		int sceneCount = SceneManager.sceneCount;
		for (int i = 0; i < sceneCount; i++)
		{
			GameObject gameObject = DevCheats.FindGameObject(SceneManager.GetSceneAt(i), name);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		Scene scene = GameLogic.instance.gameObject.scene;
		GameObject gameObject2 = DevCheats.FindGameObject(scene, name);
		if (gameObject2 != null)
		{
			return gameObject2;
		}
		return null;
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x000A5930 File Offset: 0x000A3B30
	[ConsoleMethod("fgo", "Find Game Object")]
	public void FindGameObjectCommand(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "fgo", true))
		{
			return;
		}
		GameObject gameObject = DevCheats.FindGameObjectByName(name);
		if (gameObject == null)
		{
			Debug.Log("Object not found");
			return;
		}
		Debug.Log(global::Common.ObjPath(gameObject));
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x000A5974 File Offset: 0x000A3B74
	[ConsoleMethod("ego", "Enable Game Object")]
	public void EnableGameObject(string name, int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ego", true))
		{
			return;
		}
		GameObject gameObject = DevCheats.FindGameObjectByName(name);
		if (gameObject == null)
		{
			Debug.Log("Object not found");
			return;
		}
		gameObject.SetActive(enable != 0);
		Debug.Log(((enable != 0) ? "Enabled " : "Disabled ") + global::Common.ObjPath(gameObject));
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x000A59D4 File Offset: 0x000A3BD4
	[ConsoleMethod("lgoc", "List Game Object Children")]
	public void ListGameObjectChildren(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "lgoc", true))
		{
			return;
		}
		GameObject gameObject = DevCheats.FindGameObjectByName(name);
		if (gameObject == null)
		{
			Debug.Log("Object not found");
			return;
		}
		string text = "Children of " + global::Common.ObjPath(gameObject) + ":\n";
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			text = string.Concat(new string[]
			{
				text,
				"  ",
				gameObject2.name,
				gameObject2.activeSelf ? " (active)" : " (disabled)",
				"\n"
			});
		}
		Debug.Log(text);
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x000A5A90 File Offset: 0x000A3C90
	[ConsoleMethod("eac", "Enable All Children")]
	public void EnableAllChildren(string name, int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "eac", true))
		{
			return;
		}
		GameObject gameObject = DevCheats.FindGameObjectByName(name);
		if (gameObject == null)
		{
			Debug.Log("Object not found");
			return;
		}
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			gameObject.transform.GetChild(i).gameObject.SetActive(enable != 0);
		}
		Debug.Log(((enable != 0) ? "Enabled " : "Disabled ") + "all children of " + global::Common.ObjPath(gameObject));
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x000A5B1C File Offset: 0x000A3D1C
	[ConsoleMethod("egob", "Enable Game Object Behavior")]
	public void EnableGameObjectBehavior(string name, string type, int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "egob", true))
		{
			return;
		}
		GameObject gameObject = DevCheats.FindGameObjectByName(name);
		if (gameObject == null)
		{
			Debug.Log("Object not found");
			return;
		}
		UnityEngine.Component[] components = gameObject.GetComponents(typeof(MonoBehaviour));
		for (int i = 0; i < components.Length; i++)
		{
			MonoBehaviour monoBehaviour = components[i] as MonoBehaviour;
			string name2 = monoBehaviour.GetType().Name;
			if (DevCheats.Match(name2, type))
			{
				monoBehaviour.enabled = (enable != 0);
				Debug.Log(((enable != 0) ? "Enabled " : "Disabled ") + global::Common.ObjPath(gameObject) + "." + name2);
				return;
			}
		}
		Debug.Log("Compoment not found in " + global::Common.ObjPath(gameObject));
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x000A5BD8 File Offset: 0x000A3DD8
	[ConsoleMethod("soac", "Set Only Active Child")]
	public void SetOnlyActiveChild(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "soac", true))
		{
			return;
		}
		GameObject gameObject = DevCheats.FindGameObjectByName(name);
		if (gameObject == null)
		{
			Debug.Log("Object not found");
			return;
		}
		Transform parent = gameObject.transform.parent;
		if (parent == null)
		{
			Debug.Log(global::Common.ObjPath(gameObject) + " is root object");
			return;
		}
		for (int i = 0; i < parent.childCount; i++)
		{
			GameObject gameObject2 = parent.GetChild(i).gameObject;
			gameObject2.SetActive(gameObject2 == gameObject);
		}
		Debug.Log(global::Common.ObjPath(gameObject) + " activated");
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x000A5C78 File Offset: 0x000A3E78
	[ConsoleMethod("cntrt", "Count Rect Transforms")]
	public void CountRectTransforms()
	{
		BaseUI baseUI = BaseUI.Get();
		RectTransform[] componentsInChildren;
		using (Game.Profile("GetRectTransforms", true, 0f, null))
		{
			componentsInChildren = baseUI.gameObject.GetComponentsInChildren<RectTransform>();
		}
		Debug.Log(string.Format("Rect transforms: {0}", componentsInChildren.Length));
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x000A5CE0 File Offset: 0x000A3EE0
	[ConsoleMethod("cntrt", "Count Rect Transforms")]
	public void CountRectTransforms(string mask, int repetitions)
	{
		BaseUI baseUI = BaseUI.Get();
		RectTransform[] componentsInChildren;
		using (Game.Profile("GetRectTransforms", true, 0f, null))
		{
			componentsInChildren = baseUI.gameObject.GetComponentsInChildren<RectTransform>();
		}
		List<RectTransform> list = new List<RectTransform>();
		using (Game.Profile("FilterRectTransforms", true, 0f, null))
		{
			foreach (RectTransform rectTransform in componentsInChildren)
			{
				string s = global::Common.ObjPath(rectTransform.gameObject);
				bool flag = Game.Match(s, mask, false, '*');
				for (int j = 1; j < repetitions; j++)
				{
					Game.Match(s, mask, false, '*');
				}
				if (flag)
				{
					list.Add(rectTransform);
				}
			}
		}
		Debug.Log(string.Format("Matching Rect transforms: {0} / {1}", list.Count, componentsInChildren.Length));
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x000A5DE4 File Offset: 0x000A3FE4
	[ConsoleMethod("cntrt", "Count Rect Transforms")]
	public void CountRectTransforms(string mask)
	{
		this.CountRectTransforms(mask, 100);
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x000A5DEF File Offset: 0x000A3FEF
	[ConsoleMethod("pcim", "Dump Physical Camera In Movies")]
	public void PhysicalCameraInMovies()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pcim", true))
		{
			return;
		}
		if (CameraPath.SetPhysicalCamera == 0)
		{
			Debug.Log("Physical Camera In Movies: Force OFF");
			return;
		}
		if (CameraPath.SetPhysicalCamera == 1)
		{
			Debug.Log("Physical Camera In Movies: Force ON");
			return;
		}
		Debug.Log("Physical Camera In Movies: Don't change");
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x000A5E30 File Offset: 0x000A4030
	[ConsoleMethod("fov", "Check camera FoV")]
	public void GetFoV()
	{
		Camera main = Camera.main;
		Debug.Log(string.Format("Current FoV: {0}", main.fieldOfView));
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x000A5E60 File Offset: 0x000A4060
	[ConsoleMethod("fov", "Set camera FoV")]
	public void SetFoV(int i)
	{
		Camera main = Camera.main;
		main.fieldOfView = (float)i;
		Debug.Log(string.Format("Current FoV: {0}", main.fieldOfView));
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x000A5E95 File Offset: 0x000A4095
	[ConsoleMethod("pcim", "Set Physical Camera In Movies")]
	public void PhysicalCameraInMovies(int i)
	{
		CameraPath.SetPhysicalCamera = i;
		this.PhysicalCameraInMovies();
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x000A5EA3 File Offset: 0x000A40A3
	[ConsoleMethod("gc", "Force garbage collection")]
	public void GarbageCollect()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "gc", true))
		{
			return;
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x000A5EBF File Offset: 0x000A40BF
	[ConsoleMethod("vsync", "Set VSYNC count")]
	public void VSync(int i)
	{
		QualitySettings.vSyncCount = i;
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x000A5EC8 File Offset: 0x000A40C8
	[ConsoleMethod("dth", "Draw Terrain Heightmap (0 - off, 1 - on, 2 - fast)")]
	public void DrawTerrainHeightmap(int i)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			Debug.LogError("No active terrain");
			return;
		}
		activeTerrain.drawHeightmap = (i != 0);
		if (i == 0)
		{
			Debug.Log("Terran Heightmap OFF");
			return;
		}
		Debug.Log("No implemented for Unity 2019.3");
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x000A5F14 File Offset: 0x000A4114
	[ConsoleMethod("dtsh", "Draw Terrain Shadows (0 - off, 1 - on)")]
	public void DrawTerrainShadows(int i)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			Debug.LogError("No active terrain");
			return;
		}
		activeTerrain.shadowCastingMode = ((i != 0) ? ShadowCastingMode.On : ShadowCastingMode.Off);
		Debug.Log("Terran Shadows " + ((i == 0) ? "OFF" : "ON"));
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x000A5F66 File Offset: 0x000A4166
	[ConsoleMethod("ddp", "Draw Dust Particles on troops")]
	public void DrawDustParticles(int i)
	{
		Troops.DrawDustParticles = (i == 1);
		Debug.Log("Draw Dust Particles " + ((!Troops.DrawDustParticles) ? "OFF" : "ON"));
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x000A5F94 File Offset: 0x000A4194
	[ConsoleMethod("dtt", "Draw Terrain Trees")]
	public void DrawTerrainTrees(int i)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			Debug.LogError("No active terrain");
			return;
		}
		TreesBatching component = activeTerrain.GetComponent<TreesBatching>();
		if (component != null)
		{
			component.hideTrees = (i == 0);
		}
		else
		{
			activeTerrain.drawTreesAndFoliage = (i != 0);
		}
		Debug.Log("Terran Trees " + ((i == 0) ? "OFF" : "ON"));
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x000A6000 File Offset: 0x000A4200
	[ConsoleMethod("dit", "Draw Instanced Terrain")]
	public void DrawInstancedTerrain(int i)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			Debug.LogError("No active terrain");
			return;
		}
		activeTerrain.drawInstanced = (i != 0);
		Debug.Log("Terran Instancing " + ((i == 0) ? "OFF" : "ON"));
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x000A6050 File Offset: 0x000A4250
	[ConsoleMethod("shadows", "Draw Shadows (0 - none, 1 - soft, 2 - hard")]
	public void DrawShadows(int mode)
	{
		Light light = null;
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			Light component = rootGameObjects[i].GetComponent<Light>();
			if (!(component == null) && component.type == LightType.Directional)
			{
				light = component;
				break;
			}
		}
		if (light == null)
		{
			Debug.LogError("Sun not found");
			return;
		}
		LightShadows shadows;
		if (mode != 0)
		{
			if (mode != 1)
			{
				shadows = LightShadows.Hard;
			}
			else
			{
				shadows = LightShadows.Soft;
			}
		}
		else
		{
			shadows = LightShadows.None;
		}
		light.shadows = shadows;
		Debug.Log("Shadows: " + shadows.ToString());
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x000A60F0 File Offset: 0x000A42F0
	[ConsoleMethod("aa", "Anti aliasing (0 - off, 1 - temporal, 2 - fast")]
	public void AntiAliasing(int mode)
	{
		Camera main = Camera.main;
		PostProcessLayer postProcessLayer = (main != null) ? main.GetComponent<PostProcessLayer>() : null;
		if (postProcessLayer == null)
		{
			Debug.LogError("PostProcessLayer not found");
			return;
		}
		PostProcessLayer.Antialiasing antialiasingMode;
		if (mode != 0)
		{
			if (mode != 1)
			{
				antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
			}
			else
			{
				antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
			}
		}
		else
		{
			antialiasingMode = PostProcessLayer.Antialiasing.None;
		}
		postProcessLayer.antialiasingMode = antialiasingMode;
		Debug.Log("Anti aliasing: " + antialiasingMode.ToString());
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x000A615C File Offset: 0x000A435C
	[ConsoleMethod("ocean", "Draw Ocean")]
	public void DrawOcean(int draw)
	{
		GameObject gameObject = null;
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		int num = LayerMask.NameToLayer("Water");
		foreach (GameObject gameObject2 in rootGameObjects)
		{
			if (gameObject2.name == "Ocean")
			{
				gameObject = gameObject2;
				break;
			}
			if (gameObject2.layer == num && gameObject == null)
			{
				gameObject = gameObject2;
			}
		}
		if (gameObject == null)
		{
			MirrorReflection[] array = UnityEngine.Object.FindObjectsOfType<MirrorReflection>();
			if (array != null && array.Length != 0)
			{
				gameObject = array[0].gameObject;
			}
		}
		if (gameObject == null)
		{
			Debug.LogError("Ocean not found");
			return;
		}
		gameObject.SetActive(draw != 0);
		Debug.Log(gameObject.name + ": " + ((draw == 0) ? "OFF" : "ON"));
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x000A6230 File Offset: 0x000A4430
	[ConsoleMethod("or", "Ocean Reflections")]
	public void EnableOceanReflections(int i)
	{
		foreach (MirrorReflection mirrorReflection in UnityEngine.Object.FindObjectsOfType<MirrorReflection>())
		{
			mirrorReflection.enabled = (i != 0);
			Material sharedMaterial = mirrorReflection.GetComponent<Renderer>().sharedMaterial;
			sharedMaterial.SetFloat("_ReflectionType", (float)((i == 0) ? 2 : 0));
			if (i == 0)
			{
				sharedMaterial.EnableKeyword("_REFLECTIONTYPE_CUBEMAP");
				sharedMaterial.DisableKeyword("_REFLECTIONTYPE_MIXED");
			}
			else
			{
				sharedMaterial.EnableKeyword("_REFLECTIONTYPE_MIXED");
				sharedMaterial.DisableKeyword("_REFLECTIONTYPE_CUBEMAP");
			}
		}
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x000A62B0 File Offset: 0x000A44B0
	[ConsoleMethod("arm", "Auto Record Movies")]
	public void AutoRecordMovies(int i)
	{
		MovieRecorder movieRecorder = MovieRecorder.instance;
		if (movieRecorder == null && i != 0)
		{
			movieRecorder = new GameObject("MovieRecorder").AddComponent<MovieRecorder>();
		}
		movieRecorder.AutoRecord = (i != 0);
		Debug.Log("Auto recording: " + ((i == 0) ? "OFF" : "ON"));
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x000A6307 File Offset: 0x000A4507
	[ConsoleMethod("pacp", "Play All Camera Paths")]
	public void PlayAllCameraPaths()
	{
		CameraPath.auto_play = true;
		CameraPath first = CameraPath.First;
		if (first == null)
		{
			return;
		}
		first.Play(true, false, true);
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x000A6321 File Offset: 0x000A4521
	[ConsoleMethod("max_fps", "Show max FPS")]
	public void ShowMaxFPS()
	{
		Debug.Log(Application.targetFrameRate);
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x000A6332 File Offset: 0x000A4532
	[ConsoleMethod("max_fps", "Set max FPS")]
	public void SetMaxFPS(int max_fps)
	{
		Application.targetFrameRate = max_fps;
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x000A633C File Offset: 0x000A453C
	[ConsoleMethod("fps", "Show/Hide FPS couter")]
	public void FPSCouter(int enabled)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.None, "fps", true))
		{
			return;
		}
		int sceneCount = SceneManager.sceneCount;
		List<FPSCounter> list = new List<FPSCounter>(5);
		for (int i = 0; i < sceneCount; i++)
		{
			GameObject[] rootGameObjects = SceneManager.GetSceneAt(i).GetRootGameObjects();
			if (rootGameObjects != null)
			{
				foreach (GameObject gameObject in rootGameObjects)
				{
					FPSCounter component = gameObject.GetComponent<FPSCounter>();
					if (component != null)
					{
						list.Add(component);
					}
					list.AddRange(gameObject.GetComponentsInChildren<FPSCounter>(true));
				}
			}
		}
		if (list.Count == 0)
		{
			Debug.Log("FPSCounter not found");
			return;
		}
		for (int k = 0; k < list.Count; k++)
		{
			list[k].gameObject.SetActive(enabled != 0);
		}
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x000A6408 File Offset: 0x000A4608
	[ConsoleMethod("trl", "Test Resources Load")]
	public void TestResourcesLoad(string path)
	{
		UnityEngine.Object @object = Resources.Load(path);
		Debug.Log((@object == null) ? "null" : @object.ToString());
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x000A6438 File Offset: 0x000A4638
	[ConsoleMethod("tal", "Test Assets Load")]
	public void TestAssetsLoad(string path)
	{
		UnityEngine.Object @object = Assets.GetObject(path, null, 1);
		Debug.Log((@object == null) ? "null" : @object.ToString());
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x000A6469 File Offset: 0x000A4669
	[ConsoleMethod("lrr", "Load Resource References")]
	public void LoadResourceRefs()
	{
		Assets.LoadResources();
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x000A6470 File Offset: 0x000A4670
	[ConsoleMethod("dc", "Disconnect multiplayer (ungracefully)")]
	private void DisconnectMultiplayer()
	{
		Game game = GameLogic.Get(false);
		Logic.Multiplayer multiplayer = (game != null) ? game.multiplayer : null;
		if (multiplayer == null)
		{
			Debug.LogError("No multiplayer object");
			return;
		}
		Debug.Log(string.Format("Ungracefully disconnecting {0}", multiplayer));
		multiplayer.Disconnect(false, true, true);
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x000A64B8 File Offset: 0x000A46B8
	[ConsoleMethod("udc", "Disconnect multiplayer (ungracefully)")]
	private void UngracefulDisconnectMultiplayer()
	{
		Game game = GameLogic.Get(false);
		Logic.Multiplayer multiplayer = (game != null) ? game.multiplayer : null;
		if (multiplayer == null)
		{
			Debug.LogError("No multiplayer object");
			return;
		}
		Debug.Log(string.Format("Ungracefully disconnecting {0}", multiplayer));
		multiplayer.DevCheatUngracefulDisconnect();
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x000A64FC File Offset: 0x000A46FC
	[ConsoleMethod("thqno_update_interval", "Set THQNO update interval")]
	private void SetTHQNOUpdateInterval(float t)
	{
		THQNORequest.update_interval = t;
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x000A6504 File Offset: 0x000A4704
	[ConsoleMethod("lcfs", "Load campaign from save")]
	private void LoadCampaignFromSave(string save_id)
	{
		MPBoss.Get().LoadCampaignFromSave(save_id);
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x000A6511 File Offset: 0x000A4711
	[ConsoleMethod("rc", "Reload Campaigns from disc")]
	private void ReloadCampaigns()
	{
		MPBoss.Get().ClearSaves(true, false, false);
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x000A6520 File Offset: 0x000A4720
	[ConsoleMethod("clearsaves", "Clear saves")]
	private void ClearSaves(int level)
	{
		MPBoss mpboss = MPBoss.Get();
		bool clear_thqno = false;
		bool clear_local = false;
		bool reset_campaigns = false;
		if (level == 1)
		{
			clear_thqno = true;
		}
		else if (level == 2)
		{
			clear_local = true;
			reset_campaigns = true;
		}
		else if (level == 3)
		{
			clear_thqno = true;
			clear_local = true;
			reset_campaigns = true;
		}
		else
		{
			Debug.Log("Invalid clear saves level. Please choose a level between 1 - 3.");
		}
		mpboss.ClearSaves(clear_thqno, clear_local, reset_campaigns);
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x000A6568 File Offset: 0x000A4768
	[ConsoleMethod("tme", "Test multiplayer events")]
	public void TestMultiplayerEvents()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "tme", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		int kid = 315;
		Logic.Kingdom kingdom = game.GetKingdom(kid);
		game.multiplayer.SendObjEventToAllClients(kingdom, new Logic.Kingdom.ChangeTaxRateEvent(10));
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x000A65AC File Offset: 0x000A47AC
	[ConsoleMethod("tmke", "Test multiplayer kingdom event")]
	public void TestKingdomEvent()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "tme", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		int num = 315;
		Logic.Kingdom kingdom = game.GetKingdom(num);
		game.multiplayer.SendObjEventToKingdom(kingdom, new Logic.Kingdom.ChangeTaxRateEvent(10), num);
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x000A65F0 File Offset: 0x000A47F0
	[ConsoleMethod("sa_mass", "SpawnArmyMass")]
	public void SpawnArmyMass()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "sa_mass", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (game.realms == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < game.realms.Count; i++)
		{
			Logic.Realm realm = game.realms[i];
			for (int j = 0; j < realm.settlements.Count; j++)
			{
				Logic.Settlement settlement = realm.settlements[j];
				BaseUI.Get().picked_terrain_point = (settlement.visuals as global::Settlement).transform.position;
				BaseUI.Get().SpawnArmy();
				num++;
			}
		}
		Debug.Log(num + "Armies spawned");
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x000A66B4 File Offset: 0x000A48B4
	[ConsoleMethod("afm", "Set army formation mode")]
	public void SetArmyFormationMode(string mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "afm", true))
		{
			return;
		}
		Logic.Army army = BaseUI.SelLO() as Logic.Army;
		global::Army army2 = ((army != null) ? army.visuals : null) as global::Army;
		if (army2 == null)
		{
			Debug.Log("No army selected");
			return;
		}
		if (!(mode == "a"))
		{
			if (!(mode == "c"))
			{
				if (!(mode == "t"))
				{
					if (!(mode == "b"))
					{
						Debug.LogError("Invallid formation mode, must be 'a', 'c', 't' or 'b'");
						return;
					}
					army2.FormationMode = global::Army.Formation.Battle;
				}
				else
				{
					army2.FormationMode = global::Army.Formation.Thread;
				}
			}
			else
			{
				army2.FormationMode = global::Army.Formation.Compact;
			}
		}
		else
		{
			army2.FormationMode = global::Army.Formation.Auto;
		}
		army2.RecalcFormation();
		Debug.Log(string.Format("Set army formation mode to {0}", army2.FormationMode));
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x000A6788 File Offset: 0x000A4988
	[ConsoleMethod("add_units", "Add units to selected army")]
	public void AddUnitsToArmy(int count)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_units", true))
		{
			return;
		}
		Logic.Army army = BaseUI.SelLO() as Logic.Army;
		if (army == null)
		{
			Debug.Log("No army selected");
			return;
		}
		List<string> recruitableUnitIDs = Logic.Army.GetRecruitableUnitIDs(army.game.dt, null);
		for (int i = 0; i < count; i++)
		{
			string def_id = recruitableUnitIDs[army.game.Random(0, recruitableUnitIDs.Count)];
			army.AddUnit(def_id, -1, false, true);
		}
		if (army.started)
		{
			army.NotifyListeners("units_changed", null);
		}
		Debug.Log(string.Format("Added {0} units for a total of {1}", count, army.units.Count));
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x000A6838 File Offset: 0x000A4A38
	[ConsoleMethod("scst", "Stance Check Speed Test")]
	public void StanceCheckSpeedTest()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "scst", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Settlement settlement = kingdom.realms[0].settlements[0];
		Stopwatch stopwatch = null;
		stopwatch = new Stopwatch();
		stopwatch.Start();
		using (Game.Profile("Enemy Check", false, 0f, null))
		{
			for (int i = 0; i < 1000000; i++)
			{
				kingdom.IsEnemy(kingdom);
			}
		}
		Debug.Log("Enemy check time: " + stopwatch.Elapsed.TotalSeconds.ToString() + " seconds");
		stopwatch.Stop();
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x000A68FC File Offset: 0x000A4AFC
	[ConsoleMethod("helper", "Helper")]
	public void Helper()
	{
		Logic.Object @object = GameLogic.Get(true).first_object;
		while (@object != null)
		{
			bool flag = false;
			Logic.Character character;
			if ((character = (@object as Logic.Character)) != null)
			{
				Logic.Component component = character.components[0];
				if (!character.IsInCourt() && !character.IsRoyalChild() && !character.IsRoyalRelative())
				{
					Logic.Character character2 = character;
					Logic.Kingdom kingdom = character.GetKingdom();
					if (character2 != ((kingdom != null) ? kingdom.GetKing() : null))
					{
						Logic.Character character3 = character;
						Logic.Kingdom kingdom2 = character.GetKingdom();
						if (character3 != ((kingdom2 != null) ? kingdom2.GetQueen() : null) && character.prison_kingdom == null)
						{
							Logic.Army army = character.GetArmy();
							if (((army != null) ? army.realm_in : null) == null)
							{
								Logic.Army army2 = character.GetArmy();
								if (((army2 != null) ? army2.mercenary : null) == null && (component == null || !component.IsRegisteredForUpdate()))
								{
									if (!character.IsMarried())
									{
										Debug.Log(string.Format("${0} is unlinked", character));
										flag = true;
									}
									else
									{
										Logic.Character spouse = character.GetSpouse();
										if (spouse == null || spouse.obj_state == Logic.Object.ObjState.Destroyed)
										{
											Debug.Log(string.Format("${0} is married to {1}", character, spouse));
											flag = true;
										}
									}
								}
							}
						}
					}
				}
			}
			Marriage marriage;
			if ((marriage = (@object as Marriage)) != null && (marriage.wife == null || marriage.wife.IsDead()))
			{
				Debug.Log(string.Format("${0} is active with no wife", marriage));
				flag = true;
			}
			Logic.Object object2 = @object;
			@object = @object.next_in_game;
			if (flag)
			{
				object2.Destroy(true);
			}
		}
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x000A6A58 File Offset: 0x000A4C58
	[ConsoleMethod("helper", "Helper")]
	public void Helper(int num)
	{
		switch (num)
		{
		case 0:
			Resources.UnloadUnusedAssets();
			return;
		case 1:
			GC.Collect();
			return;
		case 2:
			GarbageCollector.CollectIncremental(0UL);
			return;
		case 3:
			GarbageCollector.CollectIncremental(1UL);
			return;
		case 4:
			GarbageCollector.CollectIncremental(1000UL);
			return;
		case 5:
			GarbageCollector.CollectIncremental(1000000UL);
			return;
		case 6:
			GarbageCollector.CollectIncremental(1000000000UL);
			return;
		case 7:
		case 8:
		case 9:
		case 10:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
			break;
		case 11:
			GC.Collect(0, GCCollectionMode.Forced, true, true);
			return;
		case 12:
			GC.Collect(0, GCCollectionMode.Optimized, true, true);
			return;
		case 13:
			GC.Collect(0, GCCollectionMode.Optimized, false, true);
			return;
		case 21:
			GC.Collect(0, GCCollectionMode.Forced, true, false);
			return;
		case 22:
			GC.Collect(0, GCCollectionMode.Optimized, true, false);
			return;
		case 23:
			GC.Collect(0, GCCollectionMode.Optimized, false, false);
			break;
		default:
			return;
		}
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x000A6B4D File Offset: 0x000A4D4D
	[ConsoleMethod("as", "Autosave")]
	public void Autosave()
	{
		AutoSaveManager.Save(AutoSaveManager.Type.Event, null, "dev_cheat");
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x000A6B5C File Offset: 0x000A4D5C
	[ConsoleMethod("ssq", "Spawn squad of given type and side")]
	public void SpawnSquad(string type, int side)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ssq", true))
		{
			return;
		}
		List<Logic.Unit.Def> defs = GameLogic.Get(true).defs.GetDefs<Logic.Unit.Def>();
		string text = null;
		foreach (Logic.Unit.Def def in defs)
		{
			if (DevCheats.Match(def.id, type))
			{
				text = def.id;
				break;
			}
		}
		if (text == null)
		{
			Debug.Log("Unknown unit type");
			return;
		}
		Vector3 picked_terrain_point = BaseUI.Get().picked_terrain_point;
		global::Squad.Spawn(side, text, picked_terrain_point, true);
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x000A6BFC File Offset: 0x000A4DFC
	[ConsoleMethod("control_ai", "Enable commanding squads from other than the ui kingdom")]
	public void SetSquadControl(int value)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "control_ai", true))
		{
			return;
		}
		BaseUI.control_ai = (value == 1);
		Debug.Log("Control AI " + BaseUI.control_ai.ToString());
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x000A6C2F File Offset: 0x000A4E2F
	[ConsoleMethod("log_sim", "Log combat simulations")]
	public void ToggleSimulationLog(int value)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "log_sim", true))
		{
			return;
		}
		BattleSimulation.log_attacks = (value == 1);
		Debug.Log("Log simulations " + BattleSimulation.log_attacks.ToString());
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x000A6C62 File Offset: 0x000A4E62
	[ConsoleMethod("sim_morale_effects", "Log combat simulations")]
	public void ToggleSimulationMorale(int value)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "sim_morale_effects", true))
		{
			return;
		}
		BattleSimulation.MoraleEffectsActive = (value == 1);
		Debug.Log("Morale effects in simulation " + BattleSimulation.MoraleEffectsActive.ToString());
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x000A6C98 File Offset: 0x000A4E98
	[ConsoleMethod("random_squads", "Spawn random squads")]
	public void SpawnRandomSquads(int side, int count)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "random_squads", true))
		{
			return;
		}
		List<Logic.Unit.Def> valid_defs = Troops.valid_defs;
		Vector3 picked_terrain_point = BaseUI.Get().picked_terrain_point;
		for (int i = 0; i < count; i++)
		{
			Logic.Unit.Def def = valid_defs[Random.Range(1, valid_defs.Count - 1)];
			global::Squad.Spawn(side, def.name, picked_terrain_point, false);
		}
		Logic.Army army = BattleMap.battle.GetArmy(side);
		if (army == null)
		{
			return;
		}
		army.NotifyListeners("units_changed", null);
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x000A6D14 File Offset: 0x000A4F14
	[ConsoleMethod("migrant", "migrant")]
	public void SpawnMigrant(int count)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "migrant", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Logic.Realm realm = game.GetRealm("Zagreb");
		Logic.Realm realm2 = game.GetRealm("Pecs");
		new Logic.Migrant(game, realm, realm2, count);
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x000A6D58 File Offset: 0x000A4F58
	[ConsoleMethod("migrant", "Spawn migrant and move from start realm to end reaml")]
	public void SpawnMigrant(int count, string start_realm, string end_realm)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "migrant", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Logic.Realm realm = game.GetRealm(start_realm);
		Logic.Realm realm2 = game.GetRealm(end_realm);
		new Logic.Migrant(game, realm, realm2, count);
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x000A6D94 File Offset: 0x000A4F94
	[ConsoleMethod("rebel_pop", "rebel_pop")]
	public void AddRebelPop(int count)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "rebel_pop", true))
		{
			return;
		}
		GameLogic.Get(true);
		Castle castle = WorldUI.Get().selected_logic_obj as Castle;
		if (castle == null)
		{
			return;
		}
		castle.population.ConvertToRebel(count, true);
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x000A6DDC File Offset: 0x000A4FDC
	[ConsoleMethod("isCore", "Is currently selected realm core to its kingdom")]
	public void IsRealmCore(string kName)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "isCore", true))
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		if (settlement == null)
		{
			Debug.Log("No selected settlement");
			return;
		}
		Logic.Realm realm = settlement.GetRealm();
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		Logic.Kingdom k = realm.game.GetKingdom(kName);
		if (k == null)
		{
			k = realm.GetKingdom();
		}
		if (k == null)
		{
			Debug.Log("Realm has no kingdom");
			return;
		}
		Logic.Time last_captured;
		last_captured.milliseconds = -1L;
		RealmCoreData realmCoreData = realm.coreToKingdoms.Find((RealmCoreData d) => d.kingdom_id == k.id);
		string str = "";
		if (realmCoreData != null)
		{
			last_captured = realmCoreData.last_captured;
		}
		else
		{
			str = "(No core data)";
		}
		string str2 = realm.UpdateIsCoreFor(k) ? "yes" : "no";
		string str3 = (last_captured.milliseconds >= 0L) ? ("(Time:" + (realm.game.time - last_captured) + ")") : "";
		Debug.Log("Is core: " + str2 + str3 + str);
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x000A6F0C File Offset: 0x000A510C
	[ConsoleMethod("isCore", "Is currently selected realm core to its kingdom")]
	public void IsRealmCore()
	{
		this.IsRealmCore("@#$!#@");
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x000A6F1C File Offset: 0x000A511C
	[ConsoleMethod("srgp", "Spawn Random GreatPerson")]
	public void SpawnRadnomGreatPerson()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "srgp", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		global::Settlement component = WorldUI.Get().selected_orig.GetComponent<global::Settlement>();
		if (component == null)
		{
			Debug.Log("No Village selected");
			return;
		}
		if (component.logic == null || !(component.logic is Village))
		{
			Debug.Log("No Village selected");
			return;
		}
		Village village = component.logic as Village;
		FamousPersonSpawner component2 = game.GetComponent<FamousPersonSpawner>();
		if (component2 != null)
		{
			if (component2.all_famous_people.Count > 0)
			{
				FamousPerson.Def def = component2.all_famous_people[game.Random(0, component2.all_famous_people.Count - 1)];
				village.SetFamous(CharacterFactory.CreateFamousPerson(village.GetKingdom(), def), true);
				return;
			}
			Debug.Log("No availabale Great Person to Spawn");
		}
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x000A6FE8 File Offset: 0x000A51E8
	[ConsoleMethod("sgp", "Spawn GreatPerson with key")]
	public void SpawnGreatPerson(string key)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sgp", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		GameObject selected_orig = WorldUI.Get().selected_orig;
		if (selected_orig == null)
		{
			Debug.Log("No Village selected");
			return;
		}
		global::Settlement component = selected_orig.GetComponent<global::Settlement>();
		if (component == null)
		{
			Debug.Log("No Village selected");
			return;
		}
		if (component.logic == null || !(component.logic is Village))
		{
			Debug.Log("No Village selected");
			return;
		}
		Village village = component.logic as Village;
		FamousPersonSpawner component2 = game.GetComponent<FamousPersonSpawner>();
		if (component2 != null)
		{
			if (component2.all_famous_people.Count > 0)
			{
				int num = component2.all_famous_people.FindIndex((FamousPerson.Def x) => x.field.key == key);
				if (num == -1)
				{
					Debug.Log(key + " not found");
					return;
				}
				FamousPerson.Def def = component2.all_famous_people[num];
				village.SetFamous(CharacterFactory.CreateFamousPerson(village.GetKingdom(), def), true);
				return;
			}
			else
			{
				Debug.Log("No availabale Great Person to Spawn");
			}
		}
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x000A7100 File Offset: 0x000A5300
	[ConsoleMethod("imprison_all", "Imprison everyone in target realm")]
	public void ImprisonAllInRealm()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "imprison_all", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		Vector3 picked_terrain_point = WorldUI.Get().picked_terrain_point;
		Logic.Realm realm = game.GetRealm(picked_terrain_point);
		if (realm == null)
		{
			return;
		}
		Logic.Kingdom kingdom = game.GetKingdom(WorldUI.Get().GetCurrentKingdomId());
		if (kingdom == null)
		{
			return;
		}
		for (int i = realm.armies.Count - 1; i >= 0; i--)
		{
			if (realm.armies[i].leader != null && realm.armies[i].kingdom_id != kingdom.id)
			{
				realm.armies[i].leader.Imprison(kingdom, true, true, "dev_cheat", true);
			}
		}
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x000A71C4 File Offset: 0x000A53C4
	[ConsoleMethod("pam", "Pause / Resume army movement")]
	public void PauseArmyMovement(int pause)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "pam", true))
		{
			return;
		}
		Logic.Army army = null;
		Logic.Object @object = BaseUI.SelLO();
		if (@object != null)
		{
			Logic.Army army2;
			if ((army2 = (@object as Logic.Army)) == null)
			{
				Castle castle;
				if ((castle = (@object as Castle)) != null)
				{
					army = castle.army;
				}
			}
			else
			{
				army = army2;
			}
		}
		if (army == null)
		{
			Debug.LogError("No army selected");
			return;
		}
		Movement movement = army.movement;
		if (movement == null)
		{
			return;
		}
		movement.Pause(pause != 0, true);
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x000A7230 File Offset: 0x000A5430
	[ConsoleMethod("cts", "Dump CoopThread stats")]
	public void DumpCoopThreadStats()
	{
		string str = CoopThread.FrameStatsText();
		string str2 = CoopThread.AllProfileText();
		string text = str + "\n" + str2;
		Debug.Log(text);
		Game.CopyToClipboard(text);
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x000A7260 File Offset: 0x000A5460
	[ConsoleMethod("ctt", "Set CoopThread Trace Verbocity")]
	public void SetCoopThreadTraceLevel(string name, int verbosity)
	{
		CoopThread coopThread = CoopThread.Find(name, true, true);
		if (coopThread == null)
		{
			Debug.LogError("Coop Thread not found");
			return;
		}
		coopThread.trace_verbosity = verbosity;
		Debug.Log(string.Format("{0}: trace verbocity set to {1}", coopThread, verbosity));
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x000A72A4 File Offset: 0x000A54A4
	[ConsoleMethod("dgb", "Dump Governor Bonuses")]
	public void DumpGovernorBonuses()
	{
		Castle castle = BaseUI.SelLO() as Castle;
		Logic.Character character = BaseUI.SelChar();
		if (castle == null)
		{
			castle = ((character != null) ? character.governed_castle : null);
		}
		else if (character == null)
		{
			character = castle.governor;
		}
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		if (character == null)
		{
			Debug.LogError("No selected character");
			return;
		}
		StringBuilder sb = new StringBuilder();
		SkillsTable.EnumGovernorBonuses(castle, character, delegate(Castle c, Logic.Character g, Skill.StatModifier.Def m)
		{
			SkillsTable.GovernorModEval governorModEval = SkillsTable.EvalGovernorMod(c, g, m, null, null);
			sb.AppendLine(governorModEval.ToString());
		});
		string text = sb.ToString();
		Debug.Log(text);
		Game.CopyToClipboard(text);
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x000A7334 File Offset: 0x000A5534
	[ConsoleMethod("check_resource_stats", "Check resource stats")]
	public void CheckResourceStats()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		List<Stats.Def> defs = game.defs.GetDefs<Stats.Def>();
		if (defs == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		StringBuilder stringBuilder3 = new StringBuilder();
		StringBuilder stringBuilder4 = new StringBuilder();
		for (int i = 0; i < defs.Count; i++)
		{
			Stats.Def stats_def = defs[i];
			this.CheckResourceStats(stats_def, stringBuilder, stringBuilder2, stringBuilder3, stringBuilder4);
		}
		string text = string.Format("------------ Different:\n{0}\n", stringBuilder4) + string.Format("------------ Def only:\n{0}\n", stringBuilder2) + string.Format("------------ Equal:\n{0}\n", stringBuilder) + string.Format("------------ Income only:\n{0}\n", stringBuilder3);
		Debug.Log(text);
		Game.CopyToClipboard(text);
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x000A73E4 File Offset: 0x000A55E4
	private void CheckResourceStats(Stats.Def stats_def, StringBuilder sb_equal, StringBuilder sb_def_only, StringBuilder sb_income_only, StringBuilder sb_different)
	{
		if (stats_def.stats == null)
		{
			return;
		}
		for (int i = 0; i < stats_def.stats.Count; i++)
		{
			Stat.Def stat_def = stats_def.stats[i];
			this.CheckResourceStats(stat_def, sb_equal, sb_def_only, sb_income_only, sb_different);
		}
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x000A742C File Offset: 0x000A562C
	private void CheckResourceStats(Stat.Def stat_def, StringBuilder sb_equal, StringBuilder sb_def_only, StringBuilder sb_income_only, StringBuilder sb_different)
	{
		ResourceType gives_resource = stat_def.gives_resource;
		ResourceType resource = stat_def.GetResource(true);
		ResourceType resourceType = gives_resource;
		if (stat_def.multiplier_field == null)
		{
			resourceType = ResourceType.None;
		}
		else if (gives_resource == ResourceType.None)
		{
			resourceType = ResourceType.COUNT;
		}
		ResourceType resourceType2 = resource;
		if (resourceType == ResourceType.None && resourceType2 == ResourceType.None)
		{
			return;
		}
		if (resourceType == resourceType2)
		{
			this.LogResourceStat(sb_equal, stat_def, gives_resource, resource);
			return;
		}
		if (resourceType != ResourceType.None)
		{
			this.LogResourceStat(sb_def_only, stat_def, gives_resource, resource);
			return;
		}
		if (resourceType2 != ResourceType.None)
		{
			this.LogResourceStat(sb_income_only, stat_def, gives_resource, resource);
			return;
		}
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x000A7494 File Offset: 0x000A5694
	private void LogResourceStat(StringBuilder sb, Stat.Def stat_def, ResourceType drt, ResourceType irt)
	{
		sb.Append(string.Format("[{0}] {1}: Def: {2}", stat_def.field.line, stat_def.name, drt));
		if (stat_def.multiplier_field != null)
		{
			sb.Append(" x " + stat_def.multiplier_field.ValueStr());
		}
		sb.Append(string.Format(", Inc: {0}", irt));
		if (stat_def.income_mods != null)
		{
			sb.Append(" x (");
			for (int i = 0; i < stat_def.income_mods.Count; i++)
			{
				IncomeModifier.Def def = stat_def.income_mods[i];
				if (i != 0)
				{
					sb.Append(" + ");
				}
				sb.Append(def.location.field.Path(false, false, '.'));
			}
			sb.Append(")");
		}
		sb.AppendLine();
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x000A7580 File Offset: 0x000A5780
	[ConsoleMethod("ai_eg", "AI Eval Governor")]
	public void AIEvalGovernor()
	{
		Castle castle = BaseUI.SelLO() as Castle;
		Logic.Character character = BaseUI.SelChar();
		if (castle == null)
		{
			castle = ((character != null) ? character.governed_castle : null);
		}
		else if (character == null)
		{
			character = castle.governor;
		}
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		if (character == null)
		{
			Debug.LogError("No selected character");
			return;
		}
		Resource resource = castle.CalcGovernorProduction(null, character);
		float num = resource.Eval(castle.AISpecProductionWeights(character, false));
		Debug.Log(string.Format("[{0}] {1} {2} at {3}: {4}", new object[]
		{
			num,
			character.class_name,
			character.Name,
			castle.name,
			resource
		}));
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x000A762C File Offset: 0x000A582C
	[ConsoleMethod("ai_eg", "AI Eval Governor")]
	public void AIEvalGovernor(string spec_or_weights)
	{
		Castle castle = BaseUI.SelLO() as Castle;
		Logic.Character character = BaseUI.SelChar();
		if (castle == null)
		{
			castle = ((character != null) ? character.governed_castle : null);
		}
		else if (character == null)
		{
			character = castle.governor;
		}
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		if (character == null)
		{
			Debug.LogError("No selected character");
			return;
		}
		Resource resource = DevCheats.ParseProductionWeights(spec_or_weights);
		if (resource == null)
		{
			Debug.LogError("Invalid spec or weights");
			return;
		}
		Resource resource2 = castle.CalcGovernorProduction(null, character);
		float num = resource2.Eval(resource);
		Debug.Log(string.Format("[{0}] {1} {2} at {3}: {4}", new object[]
		{
			num,
			character.class_name,
			character.Name,
			castle.name,
			resource2
		}));
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x000A76F0 File Offset: 0x000A58F0
	[ConsoleMethod("ai_bg", "AI Eval Best Governor for Castle")]
	public void AIEvalBestGovernorForCastle()
	{
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		Logic.Kingdom kingdom = castle.GetKingdom();
		Logic.Character character = null;
		float num = 0f;
		for (int i = 0; i < kingdom.court.Count; i++)
		{
			Logic.Character character2 = kingdom.court[i];
			if (character2 != null)
			{
				Resource weights = castle.AISpecProductionWeights(character2, false);
				Resource resource = castle.CalcGovernorProduction(null, character2);
				float num2 = resource.Eval(weights);
				Debug.Log(string.Format("[{0}] {1} {2}: {3}", new object[]
				{
					num2,
					character2.class_name,
					character2.Name,
					resource
				}));
				if (num2 > num)
				{
					character = character2;
					num = num2;
				}
			}
		}
		Debug.Log(string.Format("Best: [{0}] {1} {2}", num, (character != null) ? character.class_name : null, (character != null) ? character.Name : null));
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x000A77E8 File Offset: 0x000A59E8
	[ConsoleMethod("ai_bg", "AI Eval Best Governor for Castle")]
	public void AIEvalBestGovernorForCastle(string spec_or_weights)
	{
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		Logic.Kingdom kingdom = castle.GetKingdom();
		Resource resource = DevCheats.ParseProductionWeights(spec_or_weights);
		if (resource == null)
		{
			Debug.LogError("Invalid spec or weights");
			return;
		}
		Logic.Character character = null;
		float num = 0f;
		for (int i = 0; i < kingdom.court.Count; i++)
		{
			Logic.Character character2 = kingdom.court[i];
			if (character2 != null)
			{
				Resource resource2 = castle.CalcGovernorProduction(null, character2);
				float num2 = resource2.Eval(resource);
				Debug.Log(string.Format("[{0}] {1} {2}: {3}", new object[]
				{
					num2,
					character2.class_name,
					character2.Name,
					resource2
				}));
				if (num2 > num)
				{
					character = character2;
					num = num2;
				}
			}
		}
		Debug.Log(string.Format("Best: [{0}] {1} {2}", num, (character != null) ? character.class_name : null, (character != null) ? character.Name : null));
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x000A78F4 File Offset: 0x000A5AF4
	[ConsoleMethod("ai_spec", "Set AI province specialization")]
	public void AISpec(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ai_spec", true))
		{
			return;
		}
		AI.ProvinceSpecialization provinceSpecialization;
		if (!Enum.TryParse<AI.ProvinceSpecialization>(name, out provinceSpecialization))
		{
			Debug.LogError("Invalid specialization");
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		if (settlement == null)
		{
			Debug.LogError("No selected settlement");
			return;
		}
		Logic.Realm realm = settlement.GetRealm();
		realm.ai_specialization = provinceSpecialization;
		Debug.Log(string.Format("Set {0} specialization to {1}", realm, provinceSpecialization));
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x000A7964 File Offset: 0x000A5B64
	[ConsoleMethod("ai_spec_eval", "Evaluate AI province specialization")]
	public void AISpecEval(string name)
	{
		AI.ProvinceSpecialization provinceSpecialization;
		if (!Enum.TryParse<AI.ProvinceSpecialization>(name, out provinceSpecialization))
		{
			Debug.LogError("Invalid specialization");
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		if (settlement == null)
		{
			Debug.LogError("No selected settlement");
			return;
		}
		Logic.Realm realm = settlement.GetRealm();
		Castle castle = (realm != null) ? realm.castle : null;
		float num = castle.EvalAISpec(provinceSpecialization);
		string arg = this.DumpAISpecEval(castle, provinceSpecialization);
		Debug.Log(string.Format("[{0}] {1}: {2}", num, provinceSpecialization, arg));
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x000A79E4 File Offset: 0x000A5BE4
	[ConsoleMethod("ai_spec_eval", "Evaluate AI province specializations")]
	public void AISpecEval()
	{
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		if (settlement == null)
		{
			Debug.LogError("No selected settlement");
			return;
		}
		Logic.Realm realm = settlement.GetRealm();
		Castle castle = (realm != null) ? realm.castle : null;
		string text = "";
		AI.ProvinceSpecialization provinceSpecialization = AI.ProvinceSpecialization.General;
		float num = 0f;
		for (AI.ProvinceSpecialization provinceSpecialization2 = AI.ProvinceSpecialization.General; provinceSpecialization2 < AI.ProvinceSpecialization.COUNT; provinceSpecialization2++)
		{
			float num2 = castle.EvalAISpec(provinceSpecialization2);
			if (num2 > num)
			{
				provinceSpecialization = provinceSpecialization2;
				num = num2;
			}
			string arg = this.DumpAISpecEval(castle, provinceSpecialization2);
			text += string.Format("[{0}] {1}: {2}\n", num2, provinceSpecialization2, arg);
		}
		text = string.Format("{0}Best: {1}", text, provinceSpecialization);
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x000A7AA0 File Offset: 0x000A5CA0
	private string DumpAISpecEval(Castle c, AI.ProvinceSpecialization ps)
	{
		string text = "";
		Resource production_weights = c.game.ai.def.province_specialization_resource_weights[(int)ps];
		Resource resource = new Resource();
		int num = c.MaxBuildingSlots();
		int num2 = 0;
		while (num2 < Castle.build_options.Count && num2 < num)
		{
			Castle.BuildOption buildOption = Castle.build_options[num2];
			Resource resource2 = buildOption.def.CalcProduction(null, buildOption.castle, 3, false, 1f, 0f, 0f, 0);
			this.FilterProduction(resource2, production_weights);
			resource.Add(resource2, 1f, Array.Empty<ResourceType>());
			text += string.Format("\n    {0}: {1}", buildOption, resource2);
			num2++;
		}
		return string.Format("{0}{1}", resource, text);
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x000A7B73 File Offset: 0x000A5D73
	[ConsoleMethod("aib", "Test AI build")]
	public void AIBuild()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "aib", true))
		{
			return;
		}
		this.AIBuild(null, false, false);
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x000A7B8D File Offset: 0x000A5D8D
	[ConsoleMethod("aibk", "Test AI build")]
	public void AIBuildInKingdom()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "aibk", true))
		{
			return;
		}
		this.AIBuild(null, false, true);
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x000A7BA8 File Offset: 0x000A5DA8
	[ConsoleMethod("aib", "Test AI build")]
	public void AIBuild(string spec_or_weights)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "aib", true))
		{
			return;
		}
		Resource resource = DevCheats.ParseProductionWeights(spec_or_weights);
		if (resource == null)
		{
			Debug.LogError("Invalid spec or weights");
			return;
		}
		this.AIBuild(resource, false, false);
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x000A7BE8 File Offset: 0x000A5DE8
	[ConsoleMethod("ai_eb", "Eval AI build")]
	public void AIEvalBuild()
	{
		this.AIBuild(null, true, false);
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x000A7BF3 File Offset: 0x000A5DF3
	[ConsoleMethod("ai_ebk", "Eval AI build")]
	public void AIEvalBuildInKingdom()
	{
		this.AIBuild(null, true, true);
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x000A7C00 File Offset: 0x000A5E00
	[ConsoleMethod("ai_eb", "Eval AI build")]
	public void AIEvalBuild(string spec_or_weights)
	{
		Resource resource = DevCheats.ParseProductionWeights(spec_or_weights);
		if (resource == null)
		{
			Debug.LogError("Invalid spec or weights");
			return;
		}
		this.AIBuild(resource, true, false);
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x000A7C34 File Offset: 0x000A5E34
	private static Resource ParseProductionWeights(string spec_or_weights)
	{
		AI.ProvinceSpecialization provinceSpecialization;
		Resource result;
		if (Enum.TryParse<AI.ProvinceSpecialization>(spec_or_weights, out provinceSpecialization))
		{
			result = GameLogic.Get(true).ai.def.province_specialization_resource_weights[(int)provinceSpecialization];
		}
		else
		{
			result = Resource.Parse(spec_or_weights, false);
		}
		return result;
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x000A7C70 File Offset: 0x000A5E70
	private void AIBuild(Resource production_weights, bool eval_only, bool force_in_kingdom)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "fbc", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		Castle castle = @object as Castle;
		Logic.Kingdom kingdom = (castle != null) ? castle.GetKingdom() : null;
		if (castle == null)
		{
			kingdom = (@object as Logic.Kingdom);
		}
		if (kingdom == null)
		{
			kingdom = BaseUI.LogicKingdom();
		}
		if (force_in_kingdom)
		{
			castle = null;
		}
		Castle.ClearBuildOptions();
		if (castle != null)
		{
			castle.AddBuildOptions(false, production_weights ?? castle.AISpecProductionWeights(null, true));
			if (eval_only)
			{
				Castle.Build structure_build = castle.structure_build;
				if (((structure_build != null) ? structure_build.current_building_def : null) != null)
				{
					castle.AddBuildOption(castle.structure_build.current_building_def, production_weights ?? castle.AISpecProductionWeights(null, true), 0f);
				}
			}
		}
		else
		{
			for (int i = 0; i < kingdom.realms.Count; i++)
			{
				Logic.Realm realm = kingdom.realms[i];
				if (((realm != null) ? realm.castle : null) != null)
				{
					realm.castle.AddBuildOptions(false, production_weights ?? realm.castle.AISpecProductionWeights(null, true));
				}
			}
		}
		Castle.SortBuildOptions();
		Castle.BuildOption buildOption = Castle.ChooseBuildOption(kingdom.game.game, Castle.build_options, Castle.build_options_sum);
		int num = -1;
		string text = string.Format("Options: {0}", Castle.build_options.Count);
		int num2 = 0;
		while (num2 < Castle.build_options.Count && num2 < 30)
		{
			Castle.BuildOption buildOption2 = Castle.build_options[num2];
			Resource resource = buildOption2.def.CalcProduction(null, buildOption2.castle, 1, false, 1f, 0f, 0f, 2);
			this.FilterProduction(resource, production_weights ?? buildOption2.castle.AISpecProductionWeights(null, true));
			Resource resource2 = buildOption2.def.CalcProduction(null, buildOption2.castle, 3, false, 1f, 1f, 0f, 2);
			this.FilterProduction(resource2, production_weights ?? buildOption2.castle.AISpecProductionWeights(null, true));
			Resource resource3 = buildOption2.def.CalcProduction(null, buildOption2.castle, 3, false, 1f, 1f, 1f, 2);
			this.FilterProduction(resource3, production_weights ?? buildOption2.castle.AISpecProductionWeights(null, true));
			text += string.Format("\n{0}: {1}: {2} -> {3} -> {4}", new object[]
			{
				num2,
				buildOption2,
				resource,
				resource2,
				resource3
			});
			if (buildOption2.castle == buildOption.castle && buildOption2.def == buildOption.def)
			{
				text += "    <--------";
				num = num2;
			}
			num2++;
		}
		text += string.Format("\nChosen: {0}: {1}", num, buildOption);
		Debug.Log(text);
		if (eval_only || buildOption.castle == null)
		{
			return;
		}
		if (!buildOption.def.IsUpgrade() && buildOption.castle.NeedsExpandCity())
		{
			if (!buildOption.castle.CanExpandCity())
			{
				return;
			}
			buildOption.castle.SetTier(buildOption.castle.GetTier() + 1, true);
		}
		buildOption.castle.BuildBuilding(buildOption.def, -1, true);
		if (force_in_kingdom)
		{
			BaseUI baseUI = BaseUI.Get();
			if (baseUI == null)
			{
				return;
			}
			baseUI.SelectObjFromLogic(buildOption.castle, false, true);
		}
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x000A7FC0 File Offset: 0x000A61C0
	private void FilterProduction(Resource p, Resource production_weights)
	{
		if (production_weights == null)
		{
			return;
		}
		for (ResourceType resourceType = ResourceType.Gold; resourceType < ResourceType.COUNT; resourceType++)
		{
			if (production_weights[resourceType] <= 0f)
			{
				p.Set(resourceType, 0f, null);
			}
		}
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x000A8000 File Offset: 0x000A6200
	[ConsoleMethod("aihu", "Test AI Hire Units")]
	public void TestAIHireUnits(int deterministic = 0)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "aihu", true))
		{
			return;
		}
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		string text = "Evaluating units to hire for ";
		List<Logic.Unit> units;
		int max_units;
		if (castle.army != null)
		{
			units = castle.army.units;
			max_units = castle.army.MaxUnits() + 1;
			text += castle.army.ToString();
		}
		else
		{
			units = castle.garrison.units;
			max_units = castle.garrison.SlotCount();
			text += "garrison";
		}
		List<Logic.Unit.Def> list = new List<Logic.Unit.Def>();
		List<Logic.Unit> list2 = new List<Logic.Unit>();
		float num = castle.EvalHireUnits(units, max_units, list, list2, castle.army, castle.army == null, true, true, float.NegativeInfinity, deterministic != 0);
		text += string.Format(": {0}", num);
		if (list.Count > 0)
		{
			text += "\nhire ";
			for (int i = 0; i < list.Count; i++)
			{
				Logic.Unit.Def def = list[i];
				if (i > 0)
				{
					text += ", ";
				}
				text += def.id;
			}
		}
		if (list2.Count > 0)
		{
			text += "\ndisband ";
			for (int j = 0; j < list2.Count; j++)
			{
				Logic.Unit unit = list2[j];
				if (j > 0)
				{
					text += ", ";
				}
				text += unit.def.id;
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x000A819A File Offset: 0x000A639A
	[ConsoleMethod("kbw", "Show kingdom-wide build window")]
	public void ShowKingdomBuildWindow()
	{
		UICastleBuildWindow.Create(BaseUI.SelKingdom() ?? BaseUI.LogicKingdom(), null, -1, null);
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x000A81B4 File Offset: 0x000A63B4
	[ConsoleMethod("abt", "Use alternative building tooltips")]
	public void AlternativeBuildingTooltips(int i)
	{
		Building.alt_tooltips = (i != 0);
		global::Defs defs = global::Defs.Get(false);
		if (defs != null)
		{
			defs.num_changes++;
		}
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x000A81E8 File Offset: 0x000A63E8
	[ConsoleMethod("fbc", "Force building count between min/max in ALL realms")]
	public void ForceBuildingCount(int min, int max)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "fbc", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (game.realms == null)
		{
			return;
		}
		if (!game.IsAuthority())
		{
			Debug.Log("Not Authority");
			return;
		}
		for (int i = 0; i < game.realms.Count; i++)
		{
			Logic.Realm realm = game.realms[i];
			if (realm != null && realm.castle != null)
			{
				Castle castle = realm.castle;
				castle.burned_buildings.Clear();
				Castle.Build structure_build = castle.structure_build;
				if (structure_build != null)
				{
					structure_build.CancelBuild(true, true);
				}
				for (int j = 0; j < castle.buildings.Count; j++)
				{
					castle.RemoveBuilding(castle.buildings[j], Castle.BuildingRemovalMode.Regular, true);
				}
				int num = Random.Range(min, max);
				for (int k = 0; k < num; k++)
				{
					castle.BuildRandomBuilding(false);
				}
			}
		}
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x000A82D4 File Offset: 0x000A64D4
	[ConsoleMethod("sack", "Sack selected Castle")]
	public void SackCastle()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sack", true))
		{
			return;
		}
		if (GameLogic.Get(true) == null)
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		if (realm.castle == null)
		{
			return;
		}
		realm.castle.Sack(null, false, true);
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x000A8338 File Offset: 0x000A6538
	[ConsoleMethod("hab", "Hide Army Banners")]
	public void HideArmyBanders(int shown)
	{
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (GameLogic.instance == null)
		{
			return;
		}
		if (game.type != "battle_view")
		{
			if (game.kingdoms == null && game.kingdoms.Count == 0)
			{
				return;
			}
			for (int i = 0; i < game.kingdoms.Count; i++)
			{
				Logic.Kingdom kingdom = game.kingdoms[i];
				if (kingdom.armies != null && kingdom.armies.Count != 0)
				{
					for (int j = 0; j < kingdom.armies.Count; j++)
					{
						Logic.Army army = kingdom.armies[j];
						if (army.battle != null && army.battle.simulation != null)
						{
							for (int k = 0; k < 2; k++)
							{
								List<BattleSimulation.Squad> squads = army.battle.simulation.GetSquads(k);
								if (squads != null || squads.Count != 0)
								{
									for (int l = 0; l < squads.Count; l++)
									{
										if (squads[l] != null && squads[l].squad != null && squads[l].squad.visuals != null)
										{
											global::Squad squad = squads[l].squad.visuals as global::Squad;
											if (!(squad == null))
											{
												squad.ShowBanners(shown == 0);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x000A84C4 File Offset: 0x000A66C4
	[ConsoleMethod("tmtc", "Try move trade center, according to their logic.")]
	public void TryMoveTradeCentre()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "tmtc", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
		if (realm == null)
		{
			Debug.Log("No selected realm");
			return;
		}
		game.economy.SpawnDespawnTC(false, null, realm);
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x000A8520 File Offset: 0x000A6720
	[ConsoleMethod("sdtc", "Spawn or Despawn a Trade Centre")]
	public void SpawnDespawnTradeCentre()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sdtc", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		Logic.Settlement settlement = BaseUI.SelLO() as Logic.Settlement;
		Logic.Realm forcedRealm = (settlement != null) ? settlement.GetRealm() : null;
		game.economy.SpawnDespawnTC(true, forcedRealm, null);
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x000A856C File Offset: 0x000A676C
	[ConsoleMethod("setac", "Set Achievement")]
	public void SetAchievement(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "setac", true))
		{
			return;
		}
		Game.Stats stats = GameLogic.Get(true).stats;
		if (stats == null)
		{
			return;
		}
		stats.SetAchievement(name);
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x000A8593 File Offset: 0x000A6793
	[ConsoleMethod("clearac", "Clear Achievement")]
	public void ClearAchievement(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "clearac", true))
		{
			return;
		}
		Game.Stats stats = GameLogic.Get(true).stats;
		if (stats == null)
		{
			return;
		}
		stats.ClearAchievement(name);
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x000A85BC File Offset: 0x000A67BC
	[ConsoleMethod("getac", "Get Achievement")]
	public void GetAchievement(string name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "getac", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null || game.stats == null)
		{
			Debug.LogWarning("getac " + name + ": Couldn't find stats!");
			return;
		}
		if (!THQNORequest.connected)
		{
			Debug.LogWarning("getac " + name + ": Not connected!");
		}
		if (!(name == "all"))
		{
			bool achievement = game.stats.GetAchievement(name);
			Debug.Log(string.Format("Achievement {0} = {1}", name, achievement));
			return;
		}
		DT.Field field = game.dt.files.Find((DT.Field f) => f.key.ToLowerInvariant() == "achievementrules.def");
		List<DT.Field> list = (field != null) ? field.Children() : null;
		if (list == null)
		{
			Debug.LogWarning("getac " + name + ": Failed to find achievement rules!");
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field2 = list[i].FindChild("achievement", null, true, true, true, '.');
			if (field2 != null)
			{
				string text = field2.String(null, "");
				if (text != null && text != "" && text != "unknown")
				{
					game.stats.CheckAchievement(field2.String(null, ""));
				}
			}
		}
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x000A8718 File Offset: 0x000A6918
	[ConsoleMethod("setstat", "Set Stat")]
	public void SetStat(string name, int val)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "setstat", true))
		{
			return;
		}
		Game.Stats stats = GameLogic.Get(true).stats;
		if (stats == null)
		{
			return;
		}
		stats.SetIntStat(name, val);
	}

	// Token: 0x06000F91 RID: 3985 RVA: 0x000A8740 File Offset: 0x000A6940
	[ConsoleMethod("getstat", "Get the value of an achievement stat")]
	private void GetStat(string stat_name)
	{
		Game game = GameLogic.Get(false);
		int? num;
		if (game == null)
		{
			num = null;
		}
		else
		{
			Game.Stats stats = game.stats;
			num = ((stats != null) ? new int?(stats.GetIntStat(stat_name)) : null);
		}
		Debug.Log(num);
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x000A878B File Offset: 0x000A698B
	[ConsoleMethod("incstat", "Increments the value of an achievement stat")]
	private void IncStat(string stat_name)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Game.Stats stats = game.stats;
		if (stats == null)
		{
			return;
		}
		stats.IncIntStat(stat_name, 1);
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x000A87AC File Offset: 0x000A69AC
	[ConsoleMethod("incstatcycle", "Increments the value of an achievement stat 10 times")]
	private void IncStatCycle(string stat_name, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Game game = GameLogic.Get(false);
			if (game != null)
			{
				Game.Stats stats = game.stats;
				if (stats != null)
				{
					stats.IncIntStat(stat_name, 1);
				}
			}
		}
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x000A87E3 File Offset: 0x000A69E3
	[ConsoleMethod("getstats", "Get Stats")]
	public void GetStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "getstats", true))
		{
			return;
		}
		Logic.Coroutine.Start("RequestCurrentStats", THQNORequest.RequestCurrentStatsCoro(), null);
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x000A8805 File Offset: 0x000A6A05
	[ConsoleMethod("storestats", "Get Stats")]
	public void StoreStats()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "storestats", true))
		{
			return;
		}
		Logic.Coroutine.Start("StoreStatsCoro", THQNORequest.StoreStatsCoro(), null);
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x000A8827 File Offset: 0x000A6A27
	[ConsoleMethod("bgm_track", "Select background music track")]
	public void SelectBackgroundMusicTrack(int trackIndex)
	{
		if (trackIndex == 0)
		{
			BackgroundMusic.Toggle(false);
			return;
		}
		BackgroundMusic.PlayTrack(trackIndex);
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x000A8839 File Offset: 0x000A6A39
	[ConsoleMethod("bgm_group", "Select background music group")]
	public void SelectBackgroundMusicGroup(string groupKey)
	{
		if (BackgroundMusic.eventEmitter == null)
		{
			Debug.Log("There is no background music script in the current scene or it is not attached to _AudioListener");
			return;
		}
		BackgroundMusic.SetMusicGroup(groupKey.ToLowerInvariant());
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x000A8860 File Offset: 0x000A6A60
	[ConsoleMethod("kill", "kill current character")]
	private void KillCharacter()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "kill", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		character.Die(null, "");
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x000A889C File Offset: 0x000A6A9C
	[ConsoleMethod("kill", "kill current character with reason")]
	private void KillCharacter(string reason)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "kill", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			Debug.Log("No character selected");
			return;
		}
		character.Die(new DeadStatus(reason, character), "");
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x000A88E0 File Offset: 0x000A6AE0
	[ConsoleMethod("inf", "Influence over selected kingdom")]
	private void GetInfluence()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "inf", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			Debug.Log("No kingdom selected");
			return;
		}
		if (kingdom2 == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		Debug.Log(string.Concat(new object[]
		{
			"Influence in ",
			kingdom.Name,
			": ",
			kingdom2.GetInfluenceIn(kingdom)
		}));
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x000A895C File Offset: 0x000A6B5C
	[ConsoleMethod("ka", "Enable Knight Ageing")]
	private void EnableKnightsAging(int enbale)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "ka", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (game.rules == null)
		{
			return;
		}
		bool flag = enbale != 0;
		game.rules.knight_aging = true;
		Debug.Log("Knight's aging: " + (flag ? "enabled" : "disabled"));
		int num = 0;
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (kingdom != null && kingdom.type == Logic.Kingdom.Type.Regular && kingdom.court != null)
			{
				for (int j = 0; j < kingdom.court.Count; j++)
				{
					Logic.Character character = kingdom.court[j];
					if (character != null)
					{
						character.EnableAging(flag);
						num++;
					}
				}
			}
		}
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x000A8A34 File Offset: 0x000A6C34
	[ConsoleMethod("kad", "Set Ageing def")]
	private void SetAgingDef(string def_id)
	{
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (game.rules == null)
		{
			return;
		}
		game.rules.SetAgingDef(def_id);
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x000A8A61 File Offset: 0x000A6C61
	[ConsoleMethod("pglts", "Print Generate Labels Thread States")]
	private void PrintGenerateLabelsThreadStates()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pglts", true))
		{
			return;
		}
		LabelUpdater.PrintThreadsStates();
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x000A8A77 File Offset: 0x000A6C77
	[ConsoleMethod("egls", "Enable GL script")]
	private void EnalbeGenerateLabelsScript()
	{
		LabelUpdater.Get(true).enabled = true;
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x000A8A85 File Offset: 0x000A6C85
	[ConsoleMethod("gl", "Generates Labels with numthreads")]
	private void GenerateLabelsThreaded(int numThreads)
	{
		this.GenerateLabelsThreaded(numThreads, 1);
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x000A8A8F File Offset: 0x000A6C8F
	[ConsoleMethod("gl", "Generates Labels with numthreads")]
	private void GenerateLabelsThreaded(int numThreads, int blocking)
	{
		LabelUpdater.Get(true).SetThreads(numThreads);
		while (LabelUpdater.IsProcessing())
		{
		}
		LabelUpdater.Get(true).GenerateLabels(blocking != 0);
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x000A8AB4 File Offset: 0x000A6CB4
	[ConsoleMethod("pakn", "Print all kingdom names")]
	private void PrintAllKingdomNames()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pakn", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		using (FileStream fileStream = File.OpenWrite(Path.Combine(Game.GetSavesRootDir(Game.SavesRoot.Root), "wrongKingdomNobilities.txt")))
		{
			for (int i = 0; i < game.kingdoms.Count; i++)
			{
				string text = global::Defs.Localize("@{obj.KingdomType} of {obj.NameKey}", game.kingdoms[i], null, true, true);
				if (text.Contains("{"))
				{
					Debug.Log("Kingdom[" + (i + 1).ToString() + "] = " + text);
					text += "\n";
					byte[] bytes = new UTF8Encoding(true).GetBytes(text);
					fileStream.Write(bytes, 0, bytes.Length);
				}
			}
		}
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x000A8B94 File Offset: 0x000A6D94
	[ConsoleMethod("set_winner", "Set battle winner")]
	private void SetBattleWinner(int side)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "set_winner", true))
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle == null)
		{
			return;
		}
		if (side == -1)
		{
			battle.winner = -1;
			battle.SetStage(Logic.Battle.Stage.Ongoing, true, 0f);
			return;
		}
		battle.Victory(side == 0, Logic.Battle.VictoryReason.Combat, true);
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x000A8BDF File Offset: 0x000A6DDF
	[ConsoleMethod("ctr", "Close Trade Route")]
	private void CloseTradeRoute()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ctr", true))
		{
			return;
		}
		this.CloseTradeRoute(0, 0);
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x000A8BF8 File Offset: 0x000A6DF8
	[ConsoleMethod("ctr", "Close Trade Route")]
	private void CloseTradeRoute(int side, int auto)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ctr", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 == null)
		{
			return;
		}
		if (kingdom == null)
		{
			Debug.Log("No selected Kingdom");
			return;
		}
		if (side == 0)
		{
			if (!kingdom2.tradeRouteWith.Contains(kingdom))
			{
				Debug.Log("Player kingdom(" + kingdom2.Name + ") doesnt have a trade route with " + kingdom.Name);
				return;
			}
			CloseTradeRouteAction closeTradeRouteAction = kingdom2.actions.Find("CloseTradeRouteAction") as CloseTradeRouteAction;
			closeTradeRouteAction.type = ((auto == 1) ? CloseTradeRouteAction.Type.Auto : CloseTradeRouteAction.Type.Manual);
			closeTradeRouteAction.Execute(kingdom);
			return;
		}
		else
		{
			if (!kingdom.tradeRouteWith.Contains(kingdom2))
			{
				Debug.Log(kingdom.Name + " doesnt have a trade route with player kingdom(" + kingdom2.Name + ")");
				return;
			}
			CloseTradeRouteAction closeTradeRouteAction2 = kingdom.actions.Find("CloseTradeRouteAction") as CloseTradeRouteAction;
			closeTradeRouteAction2.type = ((auto == 1) ? CloseTradeRouteAction.Type.Auto : CloseTradeRouteAction.Type.Manual);
			closeTradeRouteAction2.Execute(kingdom2);
			return;
		}
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x000A8CE8 File Offset: 0x000A6EE8
	[ConsoleMethod("hssum", "Show hypothetical diplomatic gold amounts (S-sums) for the selected kingdom for given income")]
	private void DumpHypotheticalDiplomaticGoldAmounts(float income)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "hssum", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		this.DumpHypotheticalDiplomaticGoldAmounts((kingdom != null) ? kingdom.Name : null, income);
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x000A8D14 File Offset: 0x000A6F14
	[ConsoleMethod("hssum", "Show hypothetical diplomatic gold amounts (S-sums) for the provided kingdom for given income")]
	private void DumpHypotheticalDiplomaticGoldAmounts(string kingdom_name, float income)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "hssum", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			Debug.Log("No game found!");
			return;
		}
		Logic.Kingdom kingdom = game.GetKingdom(kingdom_name);
		if (kingdom == null)
		{
			Debug.Log("Unrecognized kingdom: " + kingdom_name);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Hypothetical diplomatic gold amounts of {0} for income={1}:\n", kingdom.Name, income);
		for (int i = 1; i <= 5; i++)
		{
			stringBuilder.AppendFormat("  S{0} = {1}\n", i, kingdom.GetHypotheticalDiplomaticGoldAmount(i, income, -1f));
		}
		Debug.Log(stringBuilder);
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x000A8DB5 File Offset: 0x000A6FB5
	[ConsoleMethod("ssum", "Show diplomatic gold amounts (S-sums) for the selected kingdom")]
	private void DumpDiplomaticGoldAmounts()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ssum", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		this.DumpDiplomaticGoldAmounts((kingdom != null) ? kingdom.Name : null);
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x000A8DE0 File Offset: 0x000A6FE0
	[ConsoleMethod("ssum", "Show diplomatic gold amounts (S-sums) for the provided kingdom")]
	private void DumpDiplomaticGoldAmounts(string kingdom_name)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ssum", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			Debug.Log("No game found!");
			return;
		}
		Logic.Kingdom kingdom = game.GetKingdom(kingdom_name);
		if (kingdom == null)
		{
			Debug.Log("Unrecognized kingdom: " + kingdom_name);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Diplomatic gold amounts of {0}:\n", kingdom.Name);
		for (int i = 1; i <= 5; i++)
		{
			stringBuilder.AppendFormat("  S{0} = {1}\n", i, kingdom.GetDiplomaticGoldAmount(i, -1f));
		}
		Debug.Log(stringBuilder);
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x000A8E7A File Offset: 0x000A707A
	[ConsoleMethod("show_kingdom_ai", "Show Kingdom AI Window")]
	public void ShowKingdomAI(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "show_kingdom_ai", true))
		{
			return;
		}
		UIKingdomAIInspector.Show(enable > 0);
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x000A8E94 File Offset: 0x000A7094
	[ConsoleMethod("check_name_dublication", "Check for name dublication (per kingdom)")]
	public void CheckCharacterNameDublications()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "check_name_dublication", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		if (game.kingdoms == null || game.kingdoms.Count == 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Character's name dublication check result:");
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Dictionary<string, HashSet<Logic.Character>> dictionary = new Dictionary<string, HashSet<Logic.Character>>();
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (kingdom.court != null)
			{
				for (int j = 0; j < kingdom.court.Count; j++)
				{
					DevCheats.<CheckCharacterNameDublications>g__AddCharacter|482_0(kingdom.court[j], dictionary);
				}
			}
			DevCheats.<CheckCharacterNameDublications>g__AddCharacter|482_0(kingdom.GetKing(), dictionary);
			DevCheats.<CheckCharacterNameDublications>g__AddCharacter|482_0(kingdom.GetQueen(), dictionary);
			if (kingdom.royalFamily != null && kingdom.royalFamily.Children != null && kingdom.royalFamily.Children.Count > 0)
			{
				for (int k = 0; k < kingdom.royalFamily.Children.Count; k++)
				{
					DevCheats.<CheckCharacterNameDublications>g__AddCharacter|482_0(kingdom.royalFamily.Children[k], dictionary);
				}
			}
			string text = "-";
			int num = 0;
			foreach (KeyValuePair<string, HashSet<Logic.Character>> keyValuePair in dictionary)
			{
				if (keyValuePair.Value.Count > 1)
				{
					text += string.Format("{0} ({1})", keyValuePair.Key, keyValuePair.Value.Count);
					num++;
				}
			}
			if (num > 0)
			{
				stringBuilder.AppendLine("Kingdom " + kingdom.Name + ":");
				stringBuilder.AppendLine(text);
			}
		}
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x000A9088 File Offset: 0x000A7288
	[ConsoleMethod("ssev", "view espionage severity  vs players")]
	public void SetEspionageSeverity()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "ssev", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Debug.Log(string.Format("Espionage severity vs players: {0}", game.rules.espionage_vs_players_max_severity));
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x000A90CC File Offset: 0x000A72CC
	[ConsoleMethod("ssev", "change espionage severity vs players")]
	public void SetEspionageSeverity(int severity)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "ssev", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		game.rules.espionage_vs_players_max_severity = severity;
		Debug.Log(string.Format("Espionage severity vs players: {0}", game.rules.espionage_vs_players_max_severity));
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x000A911C File Offset: 0x000A731C
	[ConsoleMethod("aissev", "view espionage severity vs AI")]
	public void SetAIEspionageSeverity()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "aissev", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Debug.Log(string.Format("Espionage severity vs AI: {0}", game.rules.espionage_vs_AI_max_severity));
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x000A9160 File Offset: 0x000A7360
	[ConsoleMethod("aissev", "change espionage severity vs AI")]
	public void SetAIEspionageSeverity(int severity)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "aissev", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		game.rules.espionage_vs_AI_max_severity = severity;
		Debug.Log(string.Format("Espionage severity vs AI: {0}", game.rules.espionage_vs_AI_max_severity));
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x000A91B0 File Offset: 0x000A73B0
	[ConsoleMethod("apk", "Add King puppet to your spy")]
	public void AddPuppetKing()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "apk", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Character character = null;
		for (int i = 0; i < kingdom.court.Count; i++)
		{
			Logic.Character character2 = kingdom.court[i];
			if (character2 != null && character2.IsSpy())
			{
				character = character2;
				break;
			}
		}
		if (character == null)
		{
			Debug.Log("No spy in court");
			return;
		}
		Logic.Kingdom kingdom2 = character.mission_kingdom;
		if (kingdom2 == null)
		{
			Logic.Object @object = BaseUI.SelLO();
			if (@object == null)
			{
				Debug.Log("Nothing selected and no spy is on a mission");
				return;
			}
			kingdom2 = ((@object != null) ? @object.GetKingdom() : null);
			if (kingdom2 == null)
			{
				Debug.Log("Selected object is not connected to a kingdom");
				return;
			}
		}
		Logic.Character sovereign = kingdom2.royalFamily.Sovereign;
		character.AddPuppet(sovereign, true);
		Debug.Log(sovereign + " is now a puppet of " + character);
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x000A9280 File Offset: 0x000A7480
	[ConsoleMethod("app", "Add pope puppet to your spy")]
	public void AddPuppetPope()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "app", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Character character = null;
		for (int i = 0; i < kingdom.court.Count; i++)
		{
			Logic.Character character2 = kingdom.court[i];
			if (character2 != null && character2.IsSpy())
			{
				character = character2;
				break;
			}
		}
		if (character == null)
		{
			Debug.Log("No spy in court");
			return;
		}
		Logic.Character head = character.game.religions.catholic.head;
		character.AddPuppet(head, true);
		Debug.Log(head + " is now a puppet of " + character);
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x000A9317 File Offset: 0x000A7517
	[ConsoleMethod("kill_pope", "Kill the pope")]
	public void KillPope()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "app", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		Logic.Character character = (game != null) ? game.religions.catholic.head : null;
		if (character == null)
		{
			return;
		}
		character.Die(null, "");
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x000A9354 File Offset: 0x000A7554
	[ConsoleMethod("give_pope", "Give the selected kingdom a pope")]
	public void GivePope()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "app", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		Logic.Object @object = BaseUI.SelLO();
		Logic.Kingdom kingdom = (@object != null) ? @object.GetKingdom() : null;
		if (kingdom == null)
		{
			Debug.LogWarning("Couldn't infer selected kingdom!");
			return;
		}
		Logic.Character character = CharacterFactory.CreatePope(game, kingdom, false);
		game.religions.catholic.SetHead(character, false);
		kingdom.AddToSpecialCourt(character, -1, true);
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x000A93BC File Offset: 0x000A75BC
	[ConsoleMethod("apc", "Add crusader puppet to your spy")]
	public void AddPuppetCrusader()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "apc", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Character character = null;
		for (int i = 0; i < kingdom.court.Count; i++)
		{
			Logic.Character character2 = kingdom.court[i];
			if (character2 != null && character2.IsSpy())
			{
				character = character2;
				break;
			}
		}
		if (character == null)
		{
			Debug.Log("No spy in court");
			return;
		}
		Catholic catholic = character.game.religions.catholic;
		Logic.Character character3;
		if (catholic == null)
		{
			character3 = null;
		}
		else
		{
			Crusade crusade = catholic.crusade;
			character3 = ((crusade != null) ? crusade.leader : null);
		}
		Logic.Character character4 = character3;
		if (character4 == null)
		{
			Debug.Log("No active crusade");
			return;
		}
		character.AddPuppet(character4, true);
		Debug.Log(character4 + " is now a puppet of " + character);
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x000A9474 File Offset: 0x000A7674
	[ConsoleMethod("apr", "Add rebel puppet to your spy")]
	public void AddPuppetRebel()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "apr", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Character character = null;
		for (int i = 0; i < kingdom.court.Count; i++)
		{
			Logic.Character character2 = kingdom.court[i];
			if (character2 != null && character2.IsSpy())
			{
				character = character2;
				break;
			}
		}
		if (character == null)
		{
			Debug.Log("No spy in court");
			return;
		}
		Logic.Rebel rebel = BaseUI.SelLO() as Logic.Rebel;
		if (rebel == null)
		{
			Debug.Log("No rebel selected");
			return;
		}
		character.AddPuppet(rebel.character, true);
		Debug.Log(rebel.character + " is now a puppet of " + character);
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x000A9518 File Offset: 0x000A7718
	[ConsoleMethod("rdi", "Rebels declare independence")]
	public void RebelsDeclareIndependence()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "rdi", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		Logic.Rebel rebel = @object as Logic.Rebel;
		if (rebel == null)
		{
			Logic.Realm realm = @object as Logic.Realm;
			Logic.Rebel rebel2;
			if (realm == null)
			{
				rebel2 = null;
			}
			else
			{
				Castle castle = realm.castle;
				if (castle == null)
				{
					rebel2 = null;
				}
				else
				{
					Logic.Army army = castle.army;
					rebel2 = ((army != null) ? army.rebel : null);
				}
			}
			rebel = rebel2;
		}
		if (rebel == null)
		{
			Logic.Settlement settlement = @object as Logic.Settlement;
			Rebellion rebellion = ((settlement != null) ? settlement.GetRealm().controller : null) as Rebellion;
			rebel = ((rebellion != null) ? rebellion.leader : null);
		}
		if (rebel == null)
		{
			Logic.Battle battle = @object as Logic.Battle;
			Logic.Army army2 = (battle != null) ? battle.attacker : null;
			rebel = ((army2 != null) ? army2.rebel : null);
		}
		if (rebel == null)
		{
			Logic.Battle battle2 = @object as Logic.Battle;
			Logic.Army army3 = ((battle2 != null) ? battle2.defender : null) as Logic.Army;
			rebel = ((army3 != null) ? army3.rebel : null);
		}
		if (rebel == null)
		{
			Debug.Log("No selected rebel");
			return;
		}
		if (rebel.rebellion.occupiedRealms.Count == 0)
		{
			Debug.Log("The rebellion has no occupied realms");
			return;
		}
		rebel.rebellion.GetComponent<RebellionIndependence>().DeclareIndependence();
		Debug.Log("Independence declared");
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x000A962C File Offset: 0x000A782C
	[ConsoleMethod("marriage", "Marriage: 0 our prince their princess, 1 their prince our princess")]
	public void Marriage(int switch_sides)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "marriage", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelLO().GetKingdom();
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (!kingdom2.IsAuthority())
		{
			Debug.Log("marriage command is unavailable for multiplayer clients");
			return;
		}
		for (int i = 0; i < kingdom2.royalFamily.Children.Count; i++)
		{
			kingdom2.royalFamily.Children[i].Die(null, "");
		}
		for (int j = 0; j < kingdom.royalFamily.Children.Count; j++)
		{
			kingdom.royalFamily.Children[j].Die(null, "");
		}
		Logic.Character character;
		Logic.Character character2;
		if (switch_sides == 0)
		{
			character = CharacterFactory.CreatePrince(kingdom2);
			kingdom2.royalFamily.AddChild(character, true, true);
			character2 = CharacterFactory.CreatePrincess(kingdom);
			kingdom.royalFamily.AddChild(character2, true, true);
		}
		else
		{
			character = CharacterFactory.CreatePrince(kingdom);
			kingdom.royalFamily.AddChild(character, true, true);
			character2 = CharacterFactory.CreatePrincess(kingdom2);
			kingdom2.royalFamily.AddChild(character2, true, true);
		}
		character.age = Logic.Character.Age.Young;
		character2.age = Logic.Character.Age.Young;
		new Marriage(character, character2);
		kingdom2.SetStance(kingdom, RelationUtils.Stance.Marriage, null, true);
		Debug.Log("Marriage");
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x000A9770 File Offset: 0x000A7970
	[ConsoleMethod("teleport")]
	public static void Teleport(int x, int y)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "teleport", true))
		{
			return;
		}
		Logic.Army army = BaseUI.SelLO() as Logic.Army;
		if (army == null)
		{
			Debug.Log("No army selected");
			return;
		}
		army.SetPosition(new PPos((float)x, (float)y, 0));
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x000A97B5 File Offset: 0x000A79B5
	[ConsoleMethod("muib", "Enable moving units in battle")]
	public static void MoveUnitInBattle(int enabled)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "muib", true))
		{
			return;
		}
		global::Army.move_units_in_battle = (enabled != 0);
		Debug.Log("Unit movement in battle: " + (global::Army.move_units_in_battle ? "ON" : "OFF"));
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x000A97F4 File Offset: 0x000A79F4
	[ConsoleMethod("dos", "Dump Object State")]
	public static void DumpObjectState()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dos", true))
		{
			return;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			Debug.Log("No selected object");
			return;
		}
		StateDump stateDump = new StateDump();
		@object.DumpState(stateDump, 0);
		string text = stateDump.ToString();
		Debug.Log(text);
		Game.CopyToClipboard(text);
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x000A9844 File Offset: 0x000A7A44
	[ConsoleMethod("dos", "Dump Object State")]
	public static void DumpObjectState(string expr_str)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dos", true))
		{
			return;
		}
		Expression expression = Expression.Parse(expr_str, true);
		if (expression == null || expression.type == Expression.Type.Invalid)
		{
			Debug.Log("invalid expression");
			return;
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		DefsContext context = GameLogic.Get(true).dt.context;
		Logic.Object @object = BaseUI.SelLO();
		if (@object != null)
		{
			context.vars = new Vars(@object);
		}
		Value value = expression.Calc(context, false);
		context.vars = null;
		Vars.PopReflectionMode(old_mode);
		BaseObject baseObject = value.Get<BaseObject>();
		if (baseObject == null)
		{
			Debug.LogWarning(string.Format("Not an object: {0}", value));
			return;
		}
		StateDump stateDump = new StateDump();
		baseObject.DumpState(stateDump, 0);
		string text = stateDump.ToString();
		Debug.Log(text);
		Game.CopyToClipboard(text);
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x000A990C File Offset: 0x000A7B0C
	[ConsoleMethod("dgs", "Dump Game State (server only)")]
	public static void DumpGameState()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dgs", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null || game.multiplayer == null)
		{
			return;
		}
		if (!game.multiplayer.IsConnected())
		{
			game.Error("Cannot dump game state while multiplayer is not connected. Try \"dump_game_state_local\" to dump it to a file locally.");
			return;
		}
		if (game.multiplayer.type != Logic.Multiplayer.Type.Server)
		{
			game.Error("Only the server can dump game state over the network. Try \"dump_game_state_local\" to dump it to a file locally.");
			return;
		}
		game.DumpGameStateServer();
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x000A9974 File Offset: 0x000A7B74
	[ConsoleMethod("dgsl", "Dump Game State locally")]
	public static void DumpGameStateLocally()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "dgsl", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		game.DumpGameStateLocally();
		game.Log("Created local game state dump");
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x000A99AC File Offset: 0x000A7BAC
	[ConsoleMethod("gp_refresh", "Refresh great powers")]
	public static void GreatPowersRefresh()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "gp_refresh", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		game.great_powers.TopKingdoms(true);
		game.Log("Great powers refreshed");
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x000A99EC File Offset: 0x000A7BEC
	[ConsoleMethod("eow_vote", "Emperor of the world vote for selected kingdom")]
	public static void EmperorOfTheWorldStartVote()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "eow_vote", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom != null && !kingdom.IsGreatPower())
		{
			game.Log("Selected kingdom is not a great power.");
			return;
		}
		if (game.emperorOfTheWorld == null)
		{
			game.Log("EOW Logic is missing");
			return;
		}
		game.emperorOfTheWorld.StartVote(kingdom);
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x000A9A50 File Offset: 0x000A7C50
	[ConsoleMethod("eow_enabled", "Emperor of the world vote for selected kingdom")]
	public static void EmperorOfTheWorldToggle(int v)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "eow_enabled", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (game.emperorOfTheWorld == null)
		{
			game.Log("EOW Logic is missing");
			return;
		}
		EmperorOfTheWorld.EOWEnabled = (v == 1);
		Debug.Log("EOW enabled: " + EmperorOfTheWorld.EOWEnabled.ToString());
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x000A9AB0 File Offset: 0x000A7CB0
	[ConsoleMethod("add_war_score", "Add war score to the war between you and the selected kingdom")]
	public static void AddWarScore(int score)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_war_score", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		War war = kingdom2.FindWarWith(kingdom);
		if (war == null)
		{
			Debug.Log("No war between " + kingdom2.Name + " and " + kingdom.Name);
			return;
		}
		war.AddActivity("Score", kingdom2, kingdom, null, (float)score);
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x000A9B30 File Offset: 0x000A7D30
	[ConsoleMethod("add_war_score_them", "Add war score to the war between you and the selected kingdom")]
	public static void AddWarScoreThem(int score)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "add_war_score_them", true))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.Log("No selected kingdom");
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom2 == null)
		{
			Debug.Log("No player kingdom");
			return;
		}
		War war = kingdom2.FindWarWith(kingdom);
		if (war == null)
		{
			Debug.Log("No war between " + kingdom2.Name + " and " + kingdom.Name);
			return;
		}
		war.AddActivity("Score", kingdom, kingdom2, null, (float)score);
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x000A9BB0 File Offset: 0x000A7DB0
	[ConsoleMethod("cpc", "War peace points view change PC")]
	public static void WPPVC(string newPC)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "cpc", true))
		{
			return;
		}
		if (!(ViewMode.current is WarPeacePointsView))
		{
			Debug.Log("You need to be in WarPeacePointsView to use this command.");
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			Debug.Log("No game object!");
			return;
		}
		if (game.defs.Find<ProsAndCons.Def>(newPC) == null)
		{
			Debug.Log("No such Pro-Con: " + newPC);
			return;
		}
		WarPeacePointsView.pc_name = newPC;
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		worldMap.ReloadView();
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x000A9C2C File Offset: 0x000A7E2C
	[ConsoleMethod("offer_me_counter", "Receive an offer from the selected kingdom")]
	public void OfferMeCounter(string offer_type, string counter_offer1, string counter_offer2)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "offer_me_counter", true))
		{
			return;
		}
		string text = Offers.TestSendCounterOffer(offer_type, BaseUI.SelKingdom(), BaseUI.LogicKingdom(), new string[]
		{
			counter_offer1,
			counter_offer2
		});
		if (!string.IsNullOrEmpty(text))
		{
			Debug.Log(text);
		}
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x000A9C75 File Offset: 0x000A7E75
	[ConsoleMethod("offer_me_counter", "Receive an offer from the selected kingdom")]
	public void OfferMeCounter(string offer_type, string counter_offer)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "offer_me_counter", true))
		{
			return;
		}
		this.OfferMeCounter(offer_type, counter_offer);
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x000A9C90 File Offset: 0x000A7E90
	[ConsoleMethod("rkf", "Recalc kingdom fame")]
	public void RecalcKingdomFame()
	{
		Game game = GameLogic.Get(false);
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (!kingdom.IsDefeated() && kingdom.type == Logic.Kingdom.Type.Regular)
			{
				kingdom.fameObj.CalcFame();
			}
		}
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x000A9CE4 File Offset: 0x000A7EE4
	[ConsoleMethod("rkr", "Recalc kingdom ranking")]
	public void RecalcKingdomRanking()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			Debug.Log("No game");
			return;
		}
		game.rankings.FullRecalc();
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x000A9D14 File Offset: 0x000A7F14
	[ConsoleMethod("show_building", "Show building window")]
	public void ShowBuildWindow(string building_def)
	{
		Building.Def def = this.FindDef<Building.Def>(building_def);
		if (def == null)
		{
			Debug.Log("Invalid building def: " + building_def);
		}
		Logic.Object @object = BaseUI.SelLO();
		Logic.Kingdom kingdom = @object as Logic.Kingdom;
		Castle castle = @object as Castle;
		if (kingdom == null && castle == null)
		{
			Debug.Log("No selected kingdom ot castle");
			return;
		}
		bool flag;
		UIBuildingWindow.Create(castle, kingdom, def, castle, out flag);
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x000A9D6C File Offset: 0x000A7F6C
	[ConsoleMethod("pdp", "Print distant ports")]
	public void PrintDistantPorts()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "pdp", true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		for (int i = 0; i < game.realms.Count; i++)
		{
			if (game.realms[i].HasTag("DistantPort", 1))
			{
				Debug.Log(game.realms[i].name);
			}
		}
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x000A9DD4 File Offset: 0x000A7FD4
	[ConsoleMethod("fast_bv_render", "Draw battleview meshes instanced")]
	public void FastBVRender()
	{
		GeometryBatching geometryBatching = UnityEngine.Object.FindObjectOfType<GeometryBatching>();
		if (geometryBatching != null)
		{
			geometryBatching.enabled = true;
		}
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x000A9DF7 File Offset: 0x000A7FF7
	[ConsoleMethod("drf", "Disable relations fading")]
	public void DisableRelationsFade(int value)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "drf", true))
		{
			return;
		}
		KingdomAndKingdomRelation.debugDisableRelationsFade = (value != 0);
		if (value != 0)
		{
			Debug.Log("Relations fading enabled");
			return;
		}
		Debug.Log("Relations fading disabled");
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x000A9E2C File Offset: 0x000A802C
	[ConsoleMethod("test_pf", "Test Path Finding from selected army to mouse position")]
	public void TestPathFinding(int low_level_only)
	{
		Logic.Army army = BaseUI.SelLO() as Logic.Army;
		if (army == null)
		{
			return;
		}
		Point pt_from = army.position;
		Point pt_to = WorldUI.Get().picked_terrain_point;
		this.TestPathFinding(pt_from, pt_to, low_level_only);
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x000A9E70 File Offset: 0x000A8070
	[ConsoleMethod("test_pf", "Test Path Finding given start and target coordinates")]
	public void TestPathFinding(int x1, int y1, int x2, int y2, int low_level_only)
	{
		Point pt_from = new Point((float)x1, (float)y1);
		Point pt_to = new Point((float)x2, (float)y2);
		this.TestPathFinding(pt_from, pt_to, low_level_only);
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x000A9EA0 File Offset: 0x000A80A0
	private void TestPathFinding(Point pt_from, Point pt_to, int low_level_only)
	{
		Game game = GameLogic.Get(true);
		Logic.PathFinding pathFinding = (game != null) ? game.path_finding : null;
		if (pathFinding == null)
		{
			return;
		}
		pathFinding.FlushPending(false);
		bool multithreaded = pathFinding.settings.multithreaded;
		int max_steps = pathFinding.settings.max_steps;
		pathFinding.settings.multithreaded = false;
		pathFinding.settings.max_steps = -1;
		Logic.PathFinding.ClearProfileStats();
		Path path = new Path(pathFinding.game, pt_from, PathData.PassableArea.Type.All, false);
		path.Find(pt_to, 1f, false);
		using (Game.ProfileScope profileScope = Game.Profile("TestPathfinding", false, 0f, null))
		{
			while (path.state == Path.State.Pending)
			{
				pathFinding.Process(true, low_level_only == 1);
			}
			float millis = profileScope.Millis;
			string text = string.Format("{0}, {1} high segments / {2} low segments, {3} ms", new object[]
			{
				path.state,
				Logic.PathFinding.highPath_dbg.Count,
				path.segments.Count,
				millis
			});
			Debug.Log(text);
			Game.CopyToClipboard(string.Format("test_pf {0:F0} {1:F0} {2:F0} {3:F0} {4}\n{5}\n{6}", new object[]
			{
				pt_from.x,
				pt_from.y,
				pt_to.x,
				pt_to.y,
				low_level_only,
				text,
				Logic.PathFinding.StatsText("\n", "  ")
			}));
		}
		pathFinding.settings.multithreaded = multithreaded;
		pathFinding.settings.max_steps = max_steps;
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x000AA054 File Offset: 0x000A8254
	[ConsoleMethod("begin_pf", "Test debugging path finding from selected army to mouse position")]
	public void BeginPathFinding(int low_level_only)
	{
		MapObject mapObject = BaseUI.SelLO() as MapObject;
		if (mapObject == null)
		{
			return;
		}
		Point v = mapObject.position;
		BaseUI baseUI = BaseUI.Get();
		PPos pt_to = baseUI.picked_terrain_point;
		if (baseUI.picked_passable_area != 0)
		{
			pt_to = new PPos(baseUI.picked_passable_area_pos.x, baseUI.picked_passable_area_pos.z, baseUI.picked_passable_area);
		}
		this.BeginPathFinding(v, pt_to, low_level_only, mapObject, 1f, false);
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x000AA0D0 File Offset: 0x000A82D0
	[ConsoleMethod("begin_pf", "Test debugging path finding from selected army to mouse position")]
	public void BeginPathFinding()
	{
		MapObject mapObject = BaseUI.SelLO() as MapObject;
		if (mapObject == null)
		{
			return;
		}
		Movement component = mapObject.GetComponent<Movement>();
		if (((component != null) ? component.path : null) == null)
		{
			Debug.LogWarning("No movement/path on object");
			return;
		}
		this.BeginPathFinding(component.path.src_pt, component.path.dst_pt, 0, mapObject, component.path.range, component.path.flee);
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x000AA140 File Offset: 0x000A8340
	[ConsoleMethod("begin_pf", "Begin debugging path finding given start and target coordinates")]
	private void BeginPathFinding(float x1, float y1, float x2, float y2, int low_level_only)
	{
		this.BeginPathFinding(x1, y1, 0, x2, y2, 0, low_level_only);
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x000AA154 File Offset: 0x000A8354
	[ConsoleMethod("begin_pf", "Begin debugging path finding given start and target coordinates")]
	private void BeginPathFinding(float x1, float y1, int paid1, float x2, float y2, int paid2, int low_level_only)
	{
		PPos pt_from = new PPos(x1, y1, paid1);
		PPos pt_to = new PPos(x2, y2, paid2);
		this.BeginPathFinding(pt_from, pt_to, low_level_only, null, 1f, false);
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x000AA18C File Offset: 0x000A838C
	private void BeginPathFinding(PPos pt_from, PPos pt_to, int low_level_only, MapObject src_obj = null, float range = 1f, bool flee = false)
	{
		Debug.Log(string.Format("begin_pf {0:F0} {1:F0} {2:F0} {3:F0} {4} {5} {6}", new object[]
		{
			pt_from.x,
			pt_from.y,
			pt_to.x,
			pt_to.y,
			low_level_only,
			range,
			flee
		}));
		global::PathFinding.BeginDebugging(pt_from, pt_to, low_level_only != 0, src_obj, range, flee);
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x000AA218 File Offset: 0x000A8418
	[ConsoleMethod("process_pf", "Process debugging pathfinding")]
	private void ProcessPathFinding(int count)
	{
		global::PathFinding.DebugProcess(count);
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x000AA220 File Offset: 0x000A8420
	[ConsoleMethod("end_pf", "Stop debugging pathfinding")]
	private void EndPathFinding()
	{
		global::PathFinding.StopDebugging();
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x000AA228 File Offset: 0x000A8428
	[ConsoleMethod("pfem", "Set Path Finding Estimation Multiplier")]
	private void SetPathFindingEstimationMultiplier(int mul)
	{
		if (mul < 1 || mul > 255)
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Logic.PathFinding pathFinding = (game != null) ? game.path_finding : null;
		if (pathFinding == null)
		{
			return;
		}
		pathFinding.settings.estimate_weight = (byte)mul;
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000AA268 File Offset: 0x000A8468
	[ConsoleMethod("pfds", "Dump Path Finding Data Stats")]
	private void DumpPathFindingDataStats()
	{
		Game game = GameLogic.Get(true);
		Logic.PathFinding pathFinding = (game != null) ? game.path_finding : null;
		StringBuilder stringBuilder = new StringBuilder(1024);
		bool flag;
		if (pathFinding == null)
		{
			flag = (null != null);
		}
		else
		{
			PathData data = pathFinding.data;
			flag = (((data != null) ? data.nodes : null) != null);
		}
		if (flag)
		{
			int num = pathFinding.data.nodes.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = int.MaxValue;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			for (int i = 0; i < pathFinding.data.nodes.Length; i++)
			{
				int num10;
				int num11;
				pathFinding.data.GetNodeCoords(i, out num10, out num11);
				PathData.Node node = pathFinding.data.nodes[i];
				if (!node.ocean)
				{
					num3++;
				}
				byte weight = node.weight;
				if (weight > 0)
				{
					num2++;
					if (node.road)
					{
						num4++;
					}
					if ((int)weight < num5)
					{
						num5 = (int)weight;
					}
					if ((int)weight > num6)
					{
						num6 = (int)weight;
					}
					if (weight < 2)
					{
						num7++;
					}
					if (weight < 3)
					{
						num8++;
					}
					if (weight < 4)
					{
						num9++;
					}
				}
			}
			stringBuilder.AppendLine(string.Format("Low nodes: {0}", num));
			stringBuilder.AppendLine(string.Format("  Land: {0}", num3));
			stringBuilder.AppendLine(string.Format("  Passable: {0}", num2));
			stringBuilder.AppendLine(string.Format("  Road: {0}", num4));
			stringBuilder.AppendLine(string.Format("  Weights: {0} - {1}", num5, num6));
			stringBuilder.AppendLine(string.Format("    <2: {0}", num7));
			stringBuilder.AppendLine(string.Format("    <3: {0}", num8));
			stringBuilder.AppendLine(string.Format("    <4: {0}", num9));
		}
		bool flag2;
		if (pathFinding == null)
		{
			flag2 = (null != null);
		}
		else
		{
			PathData data2 = pathFinding.data;
			flag2 = (((data2 != null) ? data2.highPFGrid : null) != null);
		}
		if (flag2)
		{
			int length = pathFinding.data.highPFGrid.GetLength(0);
			int length2 = pathFinding.data.highPFGrid.GetLength(1);
			int num12 = 0;
			float num13 = float.MaxValue;
			float num14 = float.MinValue;
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int num19 = 0;
			for (int j = 0; j < length2; j++)
			{
				for (int k = 0; k < length; k++)
				{
					pathFinding.data.GetHighGridNode(k, j);
					float num20 = float.MaxValue;
					float num21 = float.MinValue;
					for (int l = 0; l < 8; l++)
					{
						ushort highNodeWeight = pathFinding.data.GetHighNodeWeight(k, j, l);
						if (highNodeWeight >= 0)
						{
							if ((float)highNodeWeight < num20)
							{
								num20 = (float)highNodeWeight;
							}
							if ((float)highNodeWeight > num21)
							{
								num21 = (float)highNodeWeight;
							}
						}
					}
					if (num21 >= 0f)
					{
						num12++;
						if (num20 < num13)
						{
							num13 = num20;
						}
						if (num21 > num14)
						{
							num14 = num21;
						}
						if (num21 > 255f)
						{
							num15++;
						}
						if (num21 > 512f)
						{
							num16++;
						}
						if (num21 > 1024f)
						{
							num17++;
						}
						if (num21 > 2048f)
						{
							num18++;
						}
						if (num21 > 32000f)
						{
							num19++;
						}
					}
				}
			}
			stringBuilder.AppendLine(string.Format("High nodes: {0} ({1} x {2})", length * length2, length, length2));
			stringBuilder.AppendLine(string.Format("  Passable: {0}", num12));
			stringBuilder.AppendLine(string.Format("  Weights: {0} - {1}", num13, num14));
			stringBuilder.AppendLine(string.Format("    >255: {0}", num15));
			stringBuilder.AppendLine(string.Format("    >512: {0}", num16));
			stringBuilder.AppendLine(string.Format("    >1024: {0}", num17));
			stringBuilder.AppendLine(string.Format("    >2048: {0}", num18));
			stringBuilder.AppendLine(string.Format("    >32000: {0}", num19));
		}
		string text = stringBuilder.ToString();
		Game.CopyToClipboard(text);
		Debug.Log(text);
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x000AA690 File Offset: 0x000A8890
	[ConsoleMethod("sound_log", "Toggle logging of sounds")]
	public static void ToggleSoundLog(int enabled)
	{
		BaseUI.log_sound_queue = (enabled == 1);
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x000AA69B File Offset: 0x000A889B
	[ConsoleMethod("voice_cd", "Toggle voice cooldowns")]
	public static void ToggleVoiceCooldowns(int enabled)
	{
		FMODVoiceProvider.use_voice_cooldowns = (enabled > 0);
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x000AA6A6 File Offset: 0x000A88A6
	[ConsoleMethod("voice_queue", "Print the voice queue's contents")]
	public static void PrintVoiceQueue()
	{
		FMODVoiceProvider.PrintQueue();
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x000AA6AD File Offset: 0x000A88AD
	[ConsoleMethod("last_voices", "Print the last voices played")]
	public static void ToggleVoiceCooldowns()
	{
		FMODVoiceProvider.PrintLastVoices();
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x000AA6B4 File Offset: 0x000A88B4
	[ConsoleMethod("fmod_log", "Toggle logging of voice overs")]
	public static void ToggleFMODLog(int enabled)
	{
		EventInstance.LogEvents = (enabled == 1);
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x000AA6BF File Offset: 0x000A88BF
	[ConsoleMethod("voice_log", "Toggle logging of voices")]
	public static void ToggleVoiceLog(int enabled)
	{
		AudioLog.printInMainLog = (enabled > 0);
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x000AA6BF File Offset: 0x000A88BF
	[ConsoleMethod("audio_log", "Toggle logging of audio")]
	public static void ToggleAudioLog(int enabled)
	{
		AudioLog.printInMainLog = (enabled > 0);
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x000AA6CA File Offset: 0x000A88CA
	[ConsoleMethod("music_log", "Toggle logging of background music actions")]
	public static void ToggleMusicLog(int enabled)
	{
		BackgroundMusic.logging_enabled = (enabled == 1);
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x000AA6D8 File Offset: 0x000A88D8
	[ConsoleMethod("fmod_dump_strings", "Toggle logging of voice overs")]
	public static void FMODDumpEvents(string prefix, string filename)
	{
		bool flag = prefix == "no_prefix";
		Bank bank;
		RuntimeManager.StudioSystem.getBank("bank:/Master.strings", out bank);
		int num;
		bank.getStringCount(out num);
		if (num <= 0)
		{
			Debug.LogWarning("No strings found!");
			return;
		}
		using (StreamWriter streamWriter = File.CreateText(filename))
		{
			for (int i = 0; i < num; i++)
			{
				Guid guid;
				string text;
				bank.getStringInfo(i, out guid, out text);
				if (flag || text.StartsWith(prefix, StringComparison.Ordinal))
				{
					streamWriter.WriteLine(text);
				}
			}
			Debug.Log("Dumped fmod strings into " + Directory.GetCurrentDirectory() + "\\" + filename);
		}
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x000AA794 File Offset: 0x000A8994
	[ConsoleMethod("destroy_fort", "Destroy target fortification")]
	public static void DestroyFortification()
	{
		Logic.Battle battle = BattleMap.battle;
		if (battle == null)
		{
			return;
		}
		Logic.Fortification fortification = null;
		float num = float.MaxValue;
		for (int i = 0; i < battle.fortifications.Count; i++)
		{
			Logic.Fortification fortification2 = battle.fortifications[i];
			global::Fortification fortification3 = fortification2.visuals as global::Fortification;
			if (!(fortification3 == null))
			{
				float num2 = Vector3.Distance(fortification3.transform.position, BattleViewUI.Get().picked_terrain_point);
				if (num2 < num)
				{
					num = num2;
					fortification = fortification2;
				}
			}
		}
		if (fortification != null)
		{
			(fortification.visuals as global::Fortification).OnHit(fortification.health);
		}
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x000AA830 File Offset: 0x000A8A30
	[ConsoleMethod("dump_go", "Save how many unique game objects we have to a text file")]
	public void DumpGameObjects()
	{
		List<GameObject> list = new List<GameObject>();
		List<GameObject> list2 = new List<GameObject>();
		int sceneCount = SceneManager.sceneCount;
		for (int i = 0; i < sceneCount; i++)
		{
			list2.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
		}
		if (GameLogic.instance != null)
		{
			list2.AddRange(GameLogic.instance.gameObject.scene.GetRootGameObjects());
		}
		for (int j = 0; j < list2.Count; j++)
		{
			Transform[] componentsInChildren = list2[j].GetComponentsInChildren<Transform>(true);
			list.Add(list2[j]);
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				list.Add(componentsInChildren[k].gameObject);
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		Dictionary<string, DevCheats.DumpGOInfo> dictionary = new Dictionary<string, DevCheats.DumpGOInfo>();
		for (int l = 0; l < list.Count; l++)
		{
			string name = list[l].name;
			DevCheats.DumpGOInfo dumpGOInfo;
			if (!dictionary.TryGetValue(name, out dumpGOInfo))
			{
				dumpGOInfo = new DevCheats.DumpGOInfo();
				dumpGOInfo.key = name;
				dictionary[name] = dumpGOInfo;
			}
			string text = "";
			Transform transform = list[l].transform;
			while (transform != null)
			{
				text = transform.name + "/" + text;
				transform = transform.parent;
			}
			int num = -1;
			for (int m = 0; m < dumpGOInfo.paths.Count; m++)
			{
				if (dumpGOInfo.paths[m].Item1 == text)
				{
					num = m;
					break;
				}
			}
			if (num == -1)
			{
				dumpGOInfo.paths.Add(new ValueTuple<string, int>(text, 1));
			}
			else
			{
				ValueTuple<string, int> valueTuple = dumpGOInfo.paths[num];
				dumpGOInfo.paths[num] = new ValueTuple<string, int>(valueTuple.Item1, valueTuple.Item2 + 1);
			}
		}
		List<DevCheats.DumpGOInfo> list3 = new List<DevCheats.DumpGOInfo>();
		foreach (KeyValuePair<string, DevCheats.DumpGOInfo> keyValuePair in dictionary)
		{
			keyValuePair.Value.paths.Sort((ValueTuple<string, int> a, ValueTuple<string, int> b) => b.Item2.CompareTo(a.Item2));
			list3.Add(keyValuePair.Value);
		}
		list3.Sort((DevCheats.DumpGOInfo a, DevCheats.DumpGOInfo b) => b.Count().CompareTo(a.Count()));
		stringBuilder.AppendLine("Total: " + list.Count);
		foreach (DevCheats.DumpGOInfo dumpGOInfo2 in list3)
		{
			int num2 = dumpGOInfo2.Count();
			stringBuilder.AppendLine(string.Format("{0} - {1}", dumpGOInfo2.key, num2));
			foreach (ValueTuple<string, int> valueTuple2 in dumpGOInfo2.paths)
			{
				stringBuilder.AppendLine(string.Format("     {0} - {1}", valueTuple2.Item1, valueTuple2.Item2));
			}
		}
		File.WriteAllText("dbg/dump_go.txt", stringBuilder.ToString());
		stringBuilder.Clear();
		dictionary.Clear();
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x000AABD0 File Offset: 0x000A8DD0
	[ConsoleMethod("avg_rels")]
	public void AverageRelations()
	{
		this.AverageRelations(-1f);
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x000AABE0 File Offset: 0x000A8DE0
	[ConsoleMethod("avg_rels")]
	public void AverageRelations(float threshold = -1f)
	{
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (threshold != -1f)
		{
			threshold = Mathf.Abs(threshold);
		}
		if (kingdom == null)
		{
			this.AverageRelationsAll(threshold);
			return;
		}
		int num2;
		int num3;
		float num = this.AverageRelations(kingdom, out num2, out num3, threshold);
		if (threshold != -1f)
		{
			Debug.Log(string.Format("{0}: {1}, {2} friends, {3} enemies", new object[]
			{
				kingdom,
				num,
				num2,
				num3
			}));
			return;
		}
		Debug.Log(string.Format("{0}: {1}", kingdom, num));
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x000AAC70 File Offset: 0x000A8E70
	public void AverageRelationsAll(float threshold = -1f)
	{
		float num = 0f;
		Game game = GameLogic.Get(true).game;
		int num2 = 0;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (!kingdom.IsDefeated())
			{
				num2++;
				int num4;
				int num5;
				float num3 = this.AverageRelations(kingdom, out num4, out num5, threshold);
				num += num3;
				if (threshold != -1f)
				{
					stringBuilder.AppendLine(string.Format("{0}: {1}, {2} friends, {3} enemies", new object[]
					{
						kingdom,
						num3,
						num4,
						num5
					}));
				}
				else
				{
					stringBuilder.AppendLine(string.Format("{0}: {1}", kingdom, num3));
				}
			}
		}
		if (num2 == 0)
		{
			Debug.Log("No valid kingdoms");
			return;
		}
		num /= (float)num2;
		stringBuilder.AppendLine(string.Format("Average Relations for all kingdoms: {0}", num));
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x000AAD7C File Offset: 0x000A8F7C
	private float AverageRelations(Logic.Kingdom k, out int friends, out int enemies, float threshold = -1f)
	{
		float num = 0f;
		Game game = GameLogic.Get(true).game;
		int num2 = 0;
		friends = 0;
		enemies = 0;
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (kingdom != k && !kingdom.IsDefeated())
			{
				float relationship = k.GetRelationship(kingdom);
				if (threshold != -1f)
				{
					if (relationship > threshold)
					{
						friends++;
					}
					else
					{
						if (relationship >= -threshold)
						{
							goto IL_71;
						}
						enemies++;
					}
				}
				num2++;
				num += relationship;
			}
			IL_71:;
		}
		if (num2 == 0)
		{
			return 0f;
		}
		return num / (float)num2;
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x000AAE1C File Offset: 0x000A901C
	[ConsoleMethod("vrf")]
	public static void ValidateRoyalFamily()
	{
		Game game = GameLogic.Get(false);
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (kingdom.royalFamily.GetKingdom() != kingdom)
			{
				Debug.Log(string.Format("Kigdom {0} has invalid royal family assigned ({1}", kingdom, kingdom.royalFamily.kingdom_id));
			}
			if (kingdom.GetKing() != null && kingdom.GetQueen() != null && Timer.Find(kingdom, "check_newborn") == null)
			{
				Debug.Log(string.Format("Kigdom {0} is missing a \"check_newborn\" timer", kingdom));
			}
		}
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x000AAEB0 File Offset: 0x000A90B0
	[ConsoleMethod("flee")]
	public static void TestSquadFlee()
	{
		Logic.Squad squad = BaseUI.SelLO() as Logic.Squad;
		if (squad == null)
		{
			return;
		}
		squad.simulation.SetState(BattleSimulation.Squad.State.Fled, null, -1f);
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x000AAEE0 File Offset: 0x000A90E0
	[ConsoleMethod("retreat")]
	public static void TestSquadRetreat()
	{
		Logic.Squad squad = BaseUI.SelLO() as Logic.Squad;
		if (squad == null)
		{
			return;
		}
		squad.simulation.SetState(BattleSimulation.Squad.State.Retreating, null, -1f);
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x000AAF0E File Offset: 0x000A910E
	[ConsoleMethod("reveal_squads")]
	public static void ToggleSquadReveal(int val)
	{
		TreesBatching.hide_camera_enabled = (val == 1);
		Debug.Log(string.Format("Squad reveal {0}", TreesBatching.hide_camera_enabled));
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x000AAF34 File Offset: 0x000A9134
	[ConsoleMethod("old_squad_path_arrows")]
	public static void ToggleSquadPathArrows(int val)
	{
		global::Squad.UseNormalPathArrows = (val == 1);
		Debug.Log(string.Format("Squad old path arrows {0}", global::Squad.UseNormalPathArrows));
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI == null)
		{
			return;
		}
		List<Logic.Squad> selectedSquads = battleViewUI.GetSelectedSquads();
		for (int i = 0; i < selectedSquads.Count; i++)
		{
			Logic.Squad squad = selectedSquads[i];
			global::Squad squad2 = ((squad != null) ? squad.visuals : null) as global::Squad;
			if (!(squad2 == null))
			{
				squad2.Selected = false;
				squad2.Selected = true;
			}
		}
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x000AAFBC File Offset: 0x000A91BC
	[ConsoleMethod("benchmark", "Perform bechmark to print average frame time in game logs.")]
	public void PerformBenchmark(string benchmarkType)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "benchmark", true))
		{
			return;
		}
		if (benchmarkType == "europe")
		{
			EuropeBenchmark.Instance.Benchmark(delegate(List<float> results)
			{
				Debug.Log(string.Format("Europe benchmark average: {0}", results.Average()));
				Debug.Log(string.Format("CPU: {0}, {1}MHz, {2} proc. GPU: {3}", new object[]
				{
					SystemInfo.processorType,
					SystemInfo.processorFrequency,
					SystemInfo.processorCount,
					SystemInfo.graphicsDeviceName
				}));
			});
			return;
		}
		Debug.Log("Not known benchmark type " + benchmarkType + ". Available options: europe");
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x000AB024 File Offset: 0x000A9224
	[ConsoleMethod("realm_cost_sel", "Dump the gold value of the selected realm, for the selected kingdom")]
	public void RealmCostSel()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "realm_cost", true))
		{
			return;
		}
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		Debug.Log(Logic.RealmCost.Dump(castle.GetRealm(), castle.GetRealm().GetKingdom()));
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x000AB074 File Offset: 0x000A9274
	[ConsoleMethod("realm_cost", "Dump the gold value of the selected realm, for our kingdom")]
	public void RealmCost()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "realm_cost", true))
		{
			return;
		}
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		Debug.Log(Logic.RealmCost.Dump(castle.GetRealm(), BaseUI.LogicKingdom()));
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x000AB0C0 File Offset: 0x000A92C0
	[ConsoleMethod("realm_cost", "Dump the gold value of the selected realm, for the input kingdom")]
	public void RealmCost(string kingdomName)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "realm_cost", true))
		{
			return;
		}
		Logic.Kingdom kingdom = this.FindKingdom(kingdomName, false);
		if (kingdom == null)
		{
			Debug.Log("Kingdom " + kingdom.Name + " not found!");
			return;
		}
		Castle castle = BaseUI.SelLO() as Castle;
		if (castle == null)
		{
			Debug.LogError("No selected castle");
			return;
		}
		Debug.Log(Logic.RealmCost.Dump(castle.GetRealm(), kingdom));
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000FEF RID: 4079 RVA: 0x000AB12D File Offset: 0x000A932D
	private BSGTerrain bsgTerrain
	{
		get
		{
			return UnityEngine.Object.FindObjectOfType<BSGTerrain>();
		}
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x000AB134 File Offset: 0x000A9334
	[ConsoleMethod("bsgterrain", "Methods to test terrain optimizations.")]
	public void BSGTerrainCommands(string method)
	{
		this.BSGTerrainCommands(method, "");
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x000AB144 File Offset: 0x000A9344
	[ConsoleMethod("bsgterrain", "Methods to test terrain optimizations.")]
	public void BSGTerrainBuildCommand(string build_word, int alphamap_cell_resolution, int texture_resolution, int use_compression)
	{
		if (build_word != "build")
		{
			Debug.Log("Command does not exist");
			return;
		}
		alphamap_cell_resolution = Mathf.ClosestPowerOfTwo(alphamap_cell_resolution);
		texture_resolution = Mathf.ClosestPowerOfTwo(texture_resolution);
		bool compress_textures = use_compression != 0;
		BSGTerrain.ConvertUnityTerrain(UnityEngine.Object.FindObjectOfType<Terrain>(), alphamap_cell_resolution, texture_resolution, compress_textures);
		Debug.Log("BSGTerrain renderer is enabled");
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x000AB198 File Offset: 0x000A9398
	[ConsoleMethod("bsgterrain", "Methods to test terrain optimizations.")]
	public void BSGTerrainCommands(string method, string arg)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "bsgterrain", true))
		{
			return;
		}
		if (this.bsgTerrain == null)
		{
			Debug.Log("BSGTerrain was not created yet. Use 'bsgterrain build <cell_resolution> <terrain_texture_resolution> <compress_textures>' command.");
			Debug.Log("Example usage: 'bsgterrain build 16 1024 0'");
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(method);
		if (num <= 2945169614U)
		{
			if (num <= 958132120U)
			{
				if (num != 247908339U)
				{
					if (num == 958132120U)
					{
						if (method == "mesh-quality")
						{
							float num2;
							if (float.TryParse(arg, out num2))
							{
								num2 = Mathf.Clamp01(num2);
								this.bsgTerrain.SetMeshQuality(num2);
								return;
							}
							Debug.Log("Argument should be float float from range 0.0 - 1.0. Can't parse " + arg + ".");
							return;
						}
					}
				}
				else if (method == "normals")
				{
					if (arg == "enable")
					{
						BSGTerrain.EnableNormalTextures();
						return;
					}
					if (!(arg == "disable"))
					{
						Debug.Log("Not known bsgterrain normal command argument " + arg + ". Available options: enable, disable");
						return;
					}
					BSGTerrain.DisableNormalTextures();
					return;
				}
			}
			else if (num != 1193435050U)
			{
				if (num == 2945169614U)
				{
					if (method == "enable")
					{
						this.bsgTerrain.enabled = true;
						return;
					}
				}
			}
			else if (method == "texture-count")
			{
				if (arg == "3")
				{
					BSGTerrain.SetNumberOfTextures(BSGTerrain.NumberOfTextures._3);
					return;
				}
				if (!(arg == "4"))
				{
					Debug.Log("Not known bsgterrain texture-count command argument " + arg + ". Available options: 3, 4");
					return;
				}
				BSGTerrain.SetNumberOfTextures(BSGTerrain.NumberOfTextures._4);
				return;
			}
		}
		else if (num <= 3454897251U)
		{
			if (num != 3294324549U)
			{
				if (num == 3454897251U)
				{
					if (method == "disable")
					{
						this.bsgTerrain.enabled = false;
						return;
					}
				}
			}
			else if (method == "destroy")
			{
				if (this.bsgTerrain != null)
				{
					UnityEngine.Object.Destroy(this.bsgTerrain);
					return;
				}
				return;
			}
		}
		else if (num != 3516040836U)
		{
			if (num != 3770694068U)
			{
				if (num == 4111477022U)
				{
					if (method == "preset")
					{
						if (arg == "low")
						{
							BSGTerrain.SetNumberOfTextures(BSGTerrain.NumberOfTextures._3);
							BSGTerrain.DisableNormalTextures();
							this.bsgTerrain.SetMeshQuality(0.6f);
							this.bsgTerrain.SetLODOffset(0.25f);
							return;
						}
						if (arg == "mid")
						{
							BSGTerrain.SetNumberOfTextures(BSGTerrain.NumberOfTextures._4);
							BSGTerrain.EnableNormalTextures();
							this.bsgTerrain.SetMeshQuality(0.9f);
							this.bsgTerrain.SetLODOffset(0f);
							return;
						}
						if (!(arg == "high"))
						{
							Debug.Log("Available options: low, mid, high");
							return;
						}
						BSGTerrain.SetNumberOfTextures(BSGTerrain.NumberOfTextures._4);
						BSGTerrain.EnableNormalTextures();
						this.bsgTerrain.SetMeshQuality(0.9f);
						this.bsgTerrain.SetLODOffset(0.25f);
						return;
					}
				}
			}
			else if (method == "lod-offset")
			{
				float lodoffset;
				if (float.TryParse(arg, out lodoffset))
				{
					this.bsgTerrain.SetLODOffset(lodoffset);
					return;
				}
				Debug.Log("Argument should be float float from range -3.0-3.0. Can't parse " + arg + ".");
				return;
			}
		}
		else if (method == "chunk-borders")
		{
			if (arg == "enable")
			{
				BSGTerrain.EnableChunkBorders();
				return;
			}
			if (!(arg == "disable"))
			{
				Debug.Log("Available options: enable, disable");
				return;
			}
			BSGTerrain.DisableChunkBorders();
			return;
		}
		Debug.Log("Not known bsgterrain command " + method + ". Available options: enable, disable, normals, texture-count, mesh-quality, chunk-borders, sharp-normals, lod-offset");
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x000AB535 File Offset: 0x000A9735
	[ConsoleMethod("us_ad", "User settings auto detect")]
	public void UserSettingsAutoDetect()
	{
		UserSettings.CheckAutoDetect(true);
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x000AB53D File Offset: 0x000A973D
	[ConsoleMethod("voice_properties_csv", "Generate voice properties CSV file")]
	public void GenerateVoicePropertiesCsv()
	{
		Voices.FillPropertiesCSV();
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x000AB544 File Offset: 0x000A9744
	[ConsoleMethod("debug_squad_anim")]
	public unsafe void DebugSquadAnim()
	{
		global::Squad.debugging_troop_anims = false;
		global::Squad.debug_squad_id = -1;
		global::Squad.debug_troop_id = -1;
		global::Squad[] picked_squads = BattleViewUI.Get().picked_squads;
		global::Squad squad = (picked_squads[0] == null) ? picked_squads[1] : picked_squads[0];
		if (squad == null)
		{
			return;
		}
		Ray ray = CameraController.MainCamera.ScreenPointToRay(Input.mousePosition);
		Logic.Squad logic = squad.logic;
		Troops.SquadData* data = squad.data;
		Transform transform = squad.transform;
		if (logic != null && logic.IsDefeated())
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		Vector3 a = transform.position;
		float num = Vector3.Dot(a - ray.origin, ray.direction);
		if (num < 0f)
		{
			return;
		}
		Vector3 b = ray.origin + ray.direction * num;
		Vector3 squad_center = squad.squad_center;
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI.picked_passable_area != 0)
		{
			b = battleViewUI.picked_passable_area_pos;
		}
		float sqrMagnitude = (squad_center - b).sqrMagnitude;
		float num2 = (squad.data_formation == null || data->logic_alive <= 1) ? squad.def.selection_radius : Mathf.Max(new float[]
		{
			squad.data_formation.spacing.x,
			squad.data_formation.spacing.y,
			squad.def.selection_radius
		});
		num2 *= 1.75f;
		float num3 = num2 * num2;
		float num4 = Mathf.Max(num3, Mathf.Pow(data->BoundingBoxSize.x / 2f, 2f) + Mathf.Pow(data->BoundingBoxSize.y / 2f, 2f));
		if (sqrMagnitude > num4)
		{
			return;
		}
		Troops.Troop troop = default(Troops.Troop);
		float num5 = num3 + 1f;
		Troops.Troop troop2 = data->FirstTroop;
		while (troop2 <= data->LastTroop)
		{
			if (!troop2.HasFlags(Troops.Troop.Flags.Dead))
			{
				a = troop2.pos3d;
				a.y += num2;
				num = Vector3.Dot(a - ray.origin, ray.direction);
				if (num >= 0f)
				{
					b = ray.origin + ray.direction * num;
					sqrMagnitude = (a - b).sqrMagnitude;
					if (sqrMagnitude <= num3 && sqrMagnitude <= num5)
					{
						num5 = sqrMagnitude;
						troop = troop2;
					}
				}
			}
			troop2 = ++troop2;
		}
		if (num5 > num3)
		{
			return;
		}
		global::Squad.debugging_troop_anims = true;
		global::Squad.debug_squad_id = troop.squad->id;
		global::Squad.debug_troop_id = troop.id;
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x000AB7F8 File Offset: 0x000A99F8
	[ConsoleMethod("set_troops")]
	public unsafe void SetRemainingTroops(int n)
	{
		global::Squad[] picked_squads = BattleViewUI.Get().picked_squads;
		global::Squad squad = (picked_squads[0] == null) ? picked_squads[1] : picked_squads[0];
		if (squad == null)
		{
			return;
		}
		Troops.SquadData* data = squad.data;
		int num = data->logic_alive - n;
		if (num > 0)
		{
			Troops.Troop troop = data->FirstTroop;
			while (troop <= data->LastTroop)
			{
				if (!troop.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
				{
					if (num <= 0)
					{
						return;
					}
					troop.Kill();
					num--;
				}
				troop = ++troop;
			}
		}
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x000AB882 File Offset: 0x000A9A82
	[ConsoleMethod("burst_fp", "Enable or disable burst function pointer compilation")]
	public void EnableBurst(bool enable)
	{
		if (enable)
		{
			PathFindingBurst.Compile();
			FormationBurst.Compile();
			return;
		}
		PathFindingBurst.Decompile();
		FormationBurst.Decompile();
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x000AB89C File Offset: 0x000A9A9C
	[ConsoleMethod("grass", "enable or disable grass")]
	public void SwitchGrass(int enable)
	{
		if (enable == 0)
		{
			BSGGrass.DisableGrass();
			return;
		}
		BSGGrass.EnableGrass();
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x000AB8AC File Offset: 0x000A9AAC
	[ConsoleMethod("mockup_grass")]
	public void MockupGrass()
	{
		BSGGrass bsggrass = UnityEngine.Object.FindObjectOfType<BSGGrass>();
		if (bsggrass == null)
		{
			return;
		}
		bsggrass.MockupFirstLayer();
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x000AB8D0 File Offset: 0x000A9AD0
	[ConsoleMethod("test_zero")]
	public void TestZero()
	{
		int num = 0;
		int num2 = 100 / num;
		float positiveInfinity = float.PositiveInfinity;
		Debug.Log(num2);
		Debug.Log(positiveInfinity);
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x000AB8FE File Offset: 0x000A9AFE
	[ConsoleMethod("test_stackoverflow")]
	public void TestStackOverFlow()
	{
		this.TestStackOverFlow();
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x000AB906 File Offset: 0x000A9B06
	[ConsoleMethod("test_indirect_stackoverflow")]
	public void TestIndirectOverflowA()
	{
		this.TestIndirectOverflowB();
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x000AB90E File Offset: 0x000A9B0E
	public void TestIndirectOverflowB()
	{
		this.TestIndirectOverflowA();
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x000AB918 File Offset: 0x000A9B18
	[ConsoleMethod("pf", "enable / disable preserve formation for side")]
	private void EnablePreserveFormation(int side, int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "pf", true))
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle == null)
		{
			return;
		}
		List<Logic.Squad> list = battle.squads.Get(side);
		if (list == null && list.Count == 0)
		{
			return;
		}
		enable = Mathf.Clamp(enable, 0, 1);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				list[i].SetStance(Logic.Squad.Stance.Aggressive - enable);
			}
		}
		string arg = (enable == 0) ? "disabled" : "enabled";
		Debug.Log(string.Format("Preserve Formation for side {0}: {1}", side, arg));
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x000AB9B0 File Offset: 0x000A9BB0
	[ConsoleMethod("idle_leave_battle")]
	public void LeaveBattle()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "idle_leave_battle", true))
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle == null || battle.initiative == null)
		{
			return;
		}
		battle.DoAction("idle_leave_battle", battle.initiative_side, "");
		Debug.Log(string.Format("Idle leave battle for {0} side", battle.initiative_side));
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x000ABA10 File Offset: 0x000A9C10
	[ConsoleMethod("alb", "Auto leave battle")]
	public void AutoLeaveBattle(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "alb", true))
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle == null || battle.initiative == null)
		{
			return;
		}
		string str = (enable == 0) ? "disabled" : "enabled";
		battle.initiative_auto_leave_battle = (enable == 1);
		Debug.Log("Auto leave battle " + str);
	}

	// Token: 0x06001001 RID: 4097 RVA: 0x000ABA68 File Offset: 0x000A9C68
	[ConsoleMethod("allocation_manager")]
	public void AllocationManagerRaport(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "allocation_manager", true))
		{
			return;
		}
		AllocationManager.enable_tracking = (enable == 1);
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x000ABA82 File Offset: 0x000A9C82
	[ConsoleMethod("allocation_manager_raport")]
	public void AllocationManagerRaport()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "allocation_manager_raport", true))
		{
			return;
		}
		AllocationManager.SaveAllocationDataIntoFile(Path.Combine(Application.dataPath, "../", "AllocationData.txt"));
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x000ABAAC File Offset: 0x000A9CAC
	[ConsoleMethod("troop_highlight")]
	public void SetTroopHighlight(int enable)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "troop_highlight", true))
		{
			return;
		}
		TextureBaker.disable_troops_highlight = (enable == 0);
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x000ABAC8 File Offset: 0x000A9CC8
	[ConsoleMethod("reload_defs")]
	public void ReloadDef()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "reload_def", true))
		{
			return;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs != null)
		{
			defs.Reload();
		}
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x000ABAFC File Offset: 0x000A9CFC
	[ConsoleMethod("show_mods")]
	public static void ShowModsList()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "show_mods", true))
		{
			return;
		}
		ModManager modManager = ModManager.Get(false);
		if (modManager != null)
		{
			List<Mod> allMods = modManager.GetAllMods();
			string text = "";
			text += "\n-1: No mods";
			Mod activeMod = modManager.GetActiveMod();
			if (activeMod == null)
			{
				text += " - (Active)";
			}
			for (int i = 0; i < allMods.Count; i++)
			{
				Mod mod = allMods[i];
				text = string.Concat(new object[]
				{
					text,
					"\n",
					i,
					": Mod: ",
					mod.name,
					(!string.IsNullOrEmpty(mod.version)) ? (" V: " + mod.version) : ""
				});
				if (!mod.Exists)
				{
					text += " [Empty]";
				}
				if (activeMod != null && mod.mod_id.Equals(activeMod.mod_id))
				{
					text += " - (Active)";
				}
			}
			Debug.Log(text);
		}
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x000ABC14 File Offset: 0x000A9E14
	[ConsoleMethod("select_mod")]
	public static void SelectMod(int index)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "select_mod", true))
		{
			return;
		}
		ModManager modManager = ModManager.Get(false);
		if (modManager != null && index >= -1)
		{
			if (index == -1)
			{
				Mod activeMod = null;
				modManager.SetActiveMod(activeMod);
				Debug.Log("Mods disabled.");
				return;
			}
			List<Mod> allMods = modManager.GetAllMods();
			if (allMods.Count > 0 && index < allMods.Count)
			{
				Mod mod = allMods[index];
				modManager.SetActiveMod(mod);
				Debug.Log("Mod: " + mod.name + ((!string.IsNullOrEmpty(mod.version)) ? (" : " + mod.version) : "") + " set as active");
				return;
			}
			Debug.LogError("Mod not found.");
		}
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x000ABCD0 File Offset: 0x000A9ED0
	[ConsoleMethod("save_mod")]
	public static void SaveMod()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "save_mod", true))
		{
			return;
		}
		UserSettings.SettingData setting = UserSettings.GetSetting("active_mod");
		if (setting == null)
		{
			return;
		}
		ModManager modManager = ModManager.Get(false);
		if (modManager == null)
		{
			return;
		}
		Mod activeMod = modManager.GetActiveMod();
		if (activeMod == null)
		{
			return;
		}
		setting.SetValue(activeMod.mod_id);
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x000ABD24 File Offset: 0x000A9F24
	[ConsoleMethod("cdr")]
	public static void CheckDeadRoyalty()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "cdr", true))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (((game != null) ? game.kingdoms : null) == null)
		{
			return;
		}
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (!kingdom.IsDefeated() && kingdom.royalFamily != null)
			{
				if (kingdom.royalFamily.Sovereign == null)
				{
					Debug.Log(string.Format("Missing king for {0}", kingdom));
				}
				else if (kingdom.royalFamily.Sovereign.IsDead())
				{
					Debug.Log(string.Format("Dead king for {0}", kingdom));
				}
				if (kingdom.royalFamily.Spouse != null && kingdom.royalFamily.Spouse.IsDead())
				{
					Debug.Log(string.Format("Dead queen for {0}", kingdom));
				}
			}
		}
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x000ABDFF File Offset: 0x000A9FFF
	[ConsoleMethod("memory_snapshot", "Takes memory snapshot of the game data")]
	public static void SaveMemorySnapshotIntoFile(string filename, int min_object_weight = 1024)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "memory_snapshot", true))
		{
			return;
		}
		MemoryAnalyser.MemorySnapshotToCSVFile(min_object_weight, filename);
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x000ABE17 File Offset: 0x000AA017
	[ConsoleMethod("memory_snapshot", "Takes memory snapshot of the game data")]
	public static void SaveMemorySnapshotIntoFile(string filename)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "memory_snapshot", true))
		{
			return;
		}
		MemoryAnalyser.MemorySnapshotToCSVFile(1024, filename);
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x000ABE33 File Offset: 0x000AA033
	[ConsoleMethod("duck_audio")]
	public static void AltTabVoiceChange(int enable)
	{
		GameLogic.duckAudioOnFocusLost = (enable != 0);
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x000ABE40 File Offset: 0x000AA040
	[ConsoleMethod("win")]
	public static void ForceVictory()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "win", true))
		{
			return;
		}
		Logic.Battle battle = WorldUI.Get().selected_logic_obj as Logic.Battle;
		if (battle == null)
		{
			battle = BattleMap.battle;
		}
		if (battle == null || !battle.IsValid())
		{
			return;
		}
		if (global::Battle.PlayerIsDefender(battle, true))
		{
			battle.Victory(false, Logic.Battle.VictoryReason.Combat, false);
			return;
		}
		battle.Victory(true, Logic.Battle.VictoryReason.Combat, false);
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x000ABEA0 File Offset: 0x000AA0A0
	[ConsoleMethod("lose")]
	public static void ForceLoss()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "lose", true))
		{
			return;
		}
		Logic.Battle battle = WorldUI.Get().selected_logic_obj as Logic.Battle;
		if (battle == null)
		{
			battle = BattleMap.battle;
		}
		if (battle == null || !battle.IsValid())
		{
			return;
		}
		if (global::Battle.PlayerIsDefender(battle, true))
		{
			battle.Victory(true, Logic.Battle.VictoryReason.Combat, false);
			return;
		}
		battle.Victory(false, Logic.Battle.VictoryReason.Combat, false);
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x000ABF09 File Offset: 0x000AA109
	[CompilerGenerated]
	internal static void <SetCheatLevel>g__SetCL|4_0(Game.CheatLevel c, ref DevCheats.<>c__DisplayClass4_0 A_1)
	{
		if (A_1.game == null)
		{
			Game.cheat_level = c;
			return;
		}
		A_1.game.SetCheatLevel(c);
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x000ABF26 File Offset: 0x000AA126
	[CompilerGenerated]
	internal static string <DumpKingdomAI>g__b2s|36_0(bool b)
	{
		if (!b)
		{
			return "off";
		}
		return "ON";
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x000ABF36 File Offset: 0x000AA136
	[CompilerGenerated]
	internal static string <DumpKingdomAI>g__f2s|36_1(KingdomAI.EnableFlags flag, ref DevCheats.<>c__DisplayClass36_0 A_1)
	{
		return DevCheats.<DumpKingdomAI>g__b2s|36_0((A_1.kingdom.ai.enabled & flag) > KingdomAI.EnableFlags.Disabled);
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x000ABF54 File Offset: 0x000AA154
	[CompilerGenerated]
	internal static string <ProfileIncome>g__pstr1|89_0(float income, float upkeep)
	{
		string text = DT.FloatToStr(income - upkeep, 3);
		if (upkeep != 0f)
		{
			text = string.Concat(new string[]
			{
				text,
				" (",
				DT.FloatToStr(income, 3),
				" - ",
				DT.FloatToStr(upkeep, 3),
				")"
			});
		}
		return text;
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x000ABFB0 File Offset: 0x000AA1B0
	[CompilerGenerated]
	internal static string <ProfileIncome>g__pstr|89_1(ResourceType rt, Resource income, Resource upkeep)
	{
		return DevCheats.<ProfileIncome>g__pstr1|89_0(income[rt], upkeep[rt]);
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x000ABFC8 File Offset: 0x000AA1C8
	[CompilerGenerated]
	private bool <Filter>g__filter|120_0(object o, bool forced, ref DevCheats.<>c__DisplayClass120_0 A_3)
	{
		object obj = o;
		if (o is DictionaryEntry)
		{
			DictionaryEntry dictionaryEntry = (DictionaryEntry)o;
			DevCheats.tmp_ctx_vars.Set<object>("Key", dictionaryEntry.Key);
			obj = dictionaryEntry.Value;
		}
		else
		{
			DevCheats.tmp_ctx_vars.Del("Key");
		}
		Value value = this.CalcExpr(A_3.filter_expr, obj, true, Vars.ReflectionMode.Enabled);
		if (!forced && !value.Bool())
		{
			return false;
		}
		A_3.txt.AppendLine(string.Format("{0} -> {1}", Logic.Object.ToString(o), value));
		return true;
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x000AC058 File Offset: 0x000AA258
	[CompilerGenerated]
	internal static string <ListResourceDependencies>g__cntstr|242_0(int num, int alt, ref DevCheats.<>c__DisplayClass242_0 A_2)
	{
		if (num == alt || A_2.format == 1)
		{
			return num.ToString();
		}
		return string.Format("{0}-{1}", num, alt);
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x000AC085 File Offset: 0x000AA285
	[CompilerGenerated]
	internal static void <CheckCharacterNameDublications>g__AddCharacter|482_0(Logic.Character c, Dictionary<string, HashSet<Logic.Character>> nameList)
	{
		if (c == null)
		{
			return;
		}
		if (nameList == null)
		{
			return;
		}
		if (!nameList.ContainsKey(c.Name))
		{
			nameList.Add(c.Name, new HashSet<Logic.Character>());
		}
		nameList[c.Name].Add(c);
	}

	// Token: 0x04000A92 RID: 2706
	private static DevCheats instance;

	// Token: 0x04000A93 RID: 2707
	private static Vars tmp_ctx_vars = new Vars();

	// Token: 0x0200063B RID: 1595
	public class DbgStatModifier : Stat.Modifier
	{
		// Token: 0x0600473D RID: 18237 RVA: 0x0002C53B File Offset: 0x0002A73B
		public override bool IsConst()
		{
			return true;
		}
	}

	// Token: 0x0200063C RID: 1596
	private class TimerLogger : IListener
	{
		// Token: 0x0600473F RID: 18239 RVA: 0x00213132 File Offset: 0x00211332
		public static DevCheats.TimerLogger Get()
		{
			if (DevCheats.TimerLogger.instance == null)
			{
				DevCheats.TimerLogger.instance = new DevCheats.TimerLogger();
				GameLogic.Get(true).AddListener(DevCheats.TimerLogger.instance);
			}
			return DevCheats.TimerLogger.instance;
		}

		// Token: 0x06004740 RID: 18240 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnMessage(object obj, string message, object param)
		{
		}

		// Token: 0x040034AF RID: 13487
		public static DevCheats.TimerLogger instance;
	}

	// Token: 0x0200063D RID: 1597
	private class EthnicityInfo
	{
		// Token: 0x040034B0 RID: 13488
		public string name;

		// Token: 0x040034B1 RID: 13489
		public List<Logic.Kingdom> kingdoms = new List<Logic.Kingdom>();

		// Token: 0x040034B2 RID: 13490
		public int provinces;
	}

	// Token: 0x0200063E RID: 1598
	private class DiplomacyStats
	{
		// Token: 0x06004744 RID: 18244 RVA: 0x0021316D File Offset: 0x0021136D
		public static string avg(int total, int cnt)
		{
			if (cnt == 0)
			{
				return "-";
			}
			return string.Format("{0:F1}", (float)total / (float)cnt);
		}

		// Token: 0x06004745 RID: 18245 RVA: 0x0021318C File Offset: 0x0021138C
		public string Dump()
		{
			this.txt.AppendLine(string.Format("Kingdoms: {0} / {1}", this.valid_kingdoms, this.total_kingdoms));
			StringBuilder stringBuilder = this.txt;
			string format = "Kingdoms in war: {0}, avg enemies: {1}, max enemies: {2} ({3})";
			object[] array = new object[4];
			array[0] = this.num_kingdoms_in_war;
			array[1] = DevCheats.DiplomacyStats.avg(this.num_enemies, this.num_kingdoms_in_war);
			array[2] = this.max_enemies;
			int num = 3;
			Logic.Kingdom kingdom = this.max_enemies_kingdom;
			array[num] = ((kingdom != null) ? kingdom.Name : null);
			stringBuilder.AppendLine(string.Format(format, array));
			this.txt.AppendLine(string.Format("Wars: {0}, with supporters: {1}", this.num_wars, this.num_wars_with_supporters));
			this.txt.AppendLine("  Average attacker supporters: " + DevCheats.DiplomacyStats.avg(this.num_attacker_supporters, this.num_wars_with_supporters));
			this.txt.AppendLine("  Average defender supporters: " + DevCheats.DiplomacyStats.avg(this.num_defender_supporters, this.num_wars_with_supporters));
			StringBuilder stringBuilder2 = this.txt;
			string format2 = "0:0 wars: {0}, max_score: {1} ({2})";
			object arg = this.no_score_wars;
			object arg2 = this.max_score;
			Logic.Kingdom kingdom2 = this.max_score_kingdom;
			stringBuilder2.AppendLine(string.Format(format2, arg, arg2, (kingdom2 != null) ? kingdom2.Name : null));
			this.txt.AppendLine("  Avg attacker score: " + DevCheats.DiplomacyStats.avg(this.total_attackers_score, this.num_wars - this.no_score_wars));
			this.txt.AppendLine("  Avg defender score: " + DevCheats.DiplomacyStats.avg(this.total_defenders_score, this.num_wars - this.no_score_wars));
			this.txt.AppendLine(string.Format("Defensive Pacts: {0}", this.dpacts));
			this.txt.AppendLine(string.Format("Offensive Pacts: {0}", this.opacts));
			return this.txt.ToString();
		}

		// Token: 0x06004746 RID: 18246 RVA: 0x00213387 File Offset: 0x00211587
		public void AddKingdom(Logic.Kingdom k)
		{
			this.total_kingdoms++;
			if (k.IsDefeated())
			{
				return;
			}
			this.valid_kingdoms++;
			this.AddWars(k);
			this.AddPacts(k);
		}

		// Token: 0x06004747 RID: 18247 RVA: 0x002133BC File Offset: 0x002115BC
		private void AddWars(Logic.Kingdom k)
		{
			if (k.wars.Count > 0)
			{
				this.num_kingdoms_in_war++;
			}
			int num = 0;
			for (int i = 0; i < k.wars.Count; i++)
			{
				War war = k.wars[i];
				List<Logic.Kingdom> enemies = war.GetEnemies(k);
				this.num_enemies += enemies.Count;
				num += enemies.Count;
				if (war.attacker == k)
				{
					this.AddWar(war);
				}
			}
			if (num > this.max_enemies)
			{
				this.max_enemies = num;
				this.max_enemies_kingdom = k;
			}
		}

		// Token: 0x06004748 RID: 18248 RVA: 0x00213458 File Offset: 0x00211658
		private void AddWar(War war)
		{
			this.num_wars++;
			if (war.attackers.Count > 1 || war.defenders.Count > 1)
			{
				this.num_wars_with_supporters++;
			}
			this.num_attacker_supporters += war.attackers.Count - 1;
			this.num_defender_supporters += war.defenders.Count - 1;
			int num = (int)war.GetSideScore(0);
			int num2 = (int)war.GetSideScore(1);
			if (num == 0 && num2 == 0)
			{
				this.no_score_wars++;
			}
			this.total_attackers_score += num;
			this.total_defenders_score += num2;
			if (num > this.max_score)
			{
				this.max_score = num;
				this.max_score_kingdom = war.attacker;
			}
			if (num2 > this.max_score)
			{
				this.max_score = num2;
				this.max_score_kingdom = war.defender;
			}
		}

		// Token: 0x06004749 RID: 18249 RVA: 0x0021354C File Offset: 0x0021174C
		private void AddPacts(Logic.Kingdom k)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < k.pacts.Count; i++)
			{
				Pact pact = k.pacts[i];
				if (pact.type == Pact.Type.Defensive)
				{
					flag = true;
					if (pact.leader == k)
					{
						this.dpacts.Add(pact);
					}
				}
				if (pact.type == Pact.Type.Offensive)
				{
					flag2 = true;
					if (pact.leader == k)
					{
						this.opacts.Add(pact);
					}
				}
			}
			if (flag)
			{
				this.dpacts.num_kingdoms = this.dpacts.num_kingdoms + 1;
			}
			if (flag2)
			{
				this.opacts.num_kingdoms = this.opacts.num_kingdoms + 1;
			}
		}

		// Token: 0x040034B3 RID: 13491
		public int total_kingdoms;

		// Token: 0x040034B4 RID: 13492
		public int valid_kingdoms;

		// Token: 0x040034B5 RID: 13493
		public int num_wars;

		// Token: 0x040034B6 RID: 13494
		public int num_kingdoms_in_war;

		// Token: 0x040034B7 RID: 13495
		public int num_wars_with_supporters;

		// Token: 0x040034B8 RID: 13496
		public int num_attacker_supporters;

		// Token: 0x040034B9 RID: 13497
		public int num_defender_supporters;

		// Token: 0x040034BA RID: 13498
		public int num_enemies;

		// Token: 0x040034BB RID: 13499
		public int max_enemies;

		// Token: 0x040034BC RID: 13500
		public Logic.Kingdom max_enemies_kingdom;

		// Token: 0x040034BD RID: 13501
		public int no_score_wars;

		// Token: 0x040034BE RID: 13502
		public int total_attackers_score;

		// Token: 0x040034BF RID: 13503
		public int total_defenders_score;

		// Token: 0x040034C0 RID: 13504
		public int max_score;

		// Token: 0x040034C1 RID: 13505
		public Logic.Kingdom max_score_kingdom;

		// Token: 0x040034C2 RID: 13506
		public DevCheats.DiplomacyStats.PactStats dpacts;

		// Token: 0x040034C3 RID: 13507
		public DevCheats.DiplomacyStats.PactStats opacts;

		// Token: 0x040034C4 RID: 13508
		private StringBuilder txt = new StringBuilder();

		// Token: 0x020009F7 RID: 2551
		public struct PactStats
		{
			// Token: 0x06005513 RID: 21779 RVA: 0x002484B4 File Offset: 0x002466B4
			public void Add(Pact pact)
			{
				this.count++;
				if (pact.members.Count > 1)
				{
					this.with_supporters++;
				}
				this.num_supporters += pact.members.Count - 1;
			}

			// Token: 0x06005514 RID: 21780 RVA: 0x00248508 File Offset: 0x00246708
			public override string ToString()
			{
				return string.Format("{0}, with supporters: {1}, kingdoms: {2}, avg. supporters: {3}", new object[]
				{
					this.count,
					this.with_supporters,
					this.num_kingdoms,
					DevCheats.DiplomacyStats.avg(this.num_supporters, this.with_supporters)
				});
			}

			// Token: 0x04004603 RID: 17923
			public int count;

			// Token: 0x04004604 RID: 17924
			public int with_supporters;

			// Token: 0x04004605 RID: 17925
			public int num_supporters;

			// Token: 0x04004606 RID: 17926
			public int num_kingdoms;
		}
	}

	// Token: 0x0200063F RID: 1599
	private class DumpGOInfo
	{
		// Token: 0x0600474B RID: 18251 RVA: 0x002135F8 File Offset: 0x002117F8
		public int Count()
		{
			int num = 0;
			foreach (ValueTuple<string, int> valueTuple in this.paths)
			{
				num += valueTuple.Item2;
			}
			return num;
		}

		// Token: 0x040034C5 RID: 13509
		public string key;

		// Token: 0x040034C6 RID: 13510
		public List<ValueTuple<string, int>> paths = new List<ValueTuple<string, int>>();
	}
}
