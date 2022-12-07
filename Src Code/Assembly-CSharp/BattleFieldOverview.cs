using System;
using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class BattleFieldOverview : MonoBehaviour
{
	// Token: 0x060007EA RID: 2026 RVA: 0x000539A2 File Offset: 0x00051BA2
	public static BattleFieldOverview Get()
	{
		return BattleFieldOverview.sm_Instance;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x000539AC File Offset: 0x00051BAC
	public static bool InProgress()
	{
		BattleFieldOverview battleFieldOverview = BattleFieldOverview.Get();
		return !(battleFieldOverview == null) && battleFieldOverview.m_InPorgress;
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x000539D0 File Offset: 0x00051BD0
	private void Awake()
	{
		BattleFieldOverview.sm_Instance = this;
		this.Hide();
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x000539DE File Offset: 0x00051BDE
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(0.2f);
		this.m_Ready = true;
		this.LocalizeStatic();
		yield break;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x000539F0 File Offset: 0x00051BF0
	private void LocalizeStatic()
	{
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_SkipLabel");
		if (textMeshProUGUI != null)
		{
			UIText.SetTextKey(textMeshProUGUI, "Battle.skip", null, null);
		}
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x00053A24 File Offset: 0x00051C24
	public void Prepair()
	{
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI != null)
		{
			battleViewUI.Show(false, false);
		}
		else
		{
			Debug.LogWarning("Battleveiw UI is missing!");
		}
		CameraController.GameCamera.LockUserInput(true);
		CameraController.GameCamera.SetScheme("Scenic");
		this.Show();
		GameLogic.Get(true).SetSpeed(1f, -1);
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x00053A88 File Offset: 0x00051C88
	public void InitCameraLocation()
	{
		KeyValuePair<Vector3, Vector3> startEndPoints = this.GetStartEndPoints();
		CameraController.GameCamera.LookAt(startEndPoints.Key, false);
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x00053AAE File Offset: 0x00051CAE
	public void BeginBattleOverview()
	{
		this.snapshot = new FMODWrapper.Snapshot("battle_introductions_snapshot");
		if (!this.m_Ready)
		{
			this.m_BeginWhenReady = true;
			return;
		}
		this.BuildCameraPath();
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00053AD8 File Offset: 0x00051CD8
	private void Complete()
	{
		Debug.Log("Finishing BattleFieldOverview");
		this.m_InPorgress = false;
		FMODWrapper.Snapshot snapshot = this.snapshot;
		if (snapshot != null)
		{
			snapshot.EndSnapshot();
		}
		GameSpeed.SupressSpeedChangesByPlayer = false;
		FlybyCamera component = base.GetComponent<FlybyCamera>();
		component.OnComplete = (Action)Delegate.Remove(component.OnComplete, new Action(this.HandleOnFlybyCameraPathComplete));
		bool flag = true;
		if (this.UIRoot != null)
		{
			GameObject gameObject = global::Common.FindChildByName(this.UIRoot, "id_BlackBars", true, true);
			gameObject.SetActive(true);
			TweenCanvasGroupAplha component2 = gameObject.GetComponent<TweenCanvasGroupAplha>();
			component2.ignoreTimeScale = true;
			component2.delay = 0.2f;
			component2.duration = 0.4f;
			component2.ResetToBeginning();
			component2.PlayForward();
			flag = false;
			component2.onFinished.RemoveAllListeners();
			component2.onFinished.AddListener(delegate()
			{
				BattleViewUI battleViewUI2 = BattleViewUI.Get();
				if (battleViewUI2 != null)
				{
					battleViewUI2.Show(true, true);
				}
			});
			TweenRectTransformPivot[] componentsInChildren = gameObject.GetComponentsInChildren<TweenRectTransformPivot>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ignoreTimeScale = true;
				componentsInChildren[i].ResetToBeginning();
				componentsInChildren[i].PlayForward();
			}
		}
		if (flag)
		{
			BattleViewUI battleViewUI = BattleViewUI.Get();
			if (battleViewUI != null)
			{
				battleViewUI.Show(true, true);
			}
		}
		CameraController.GameCamera.LockUserInput(false);
		CameraController.GameCamera.SetScheme("GameMode");
		Debug.Log("Starting battle");
		BattleMap.battle.SetStage(Logic.Battle.Stage.Ongoing, false, 0f);
		BackgroundMusic.OnTrigger("BattleIntroAfterNarratorTrigger", null);
		if (this.UIRoot != null)
		{
			GameObject gameObject2 = global::Common.FindChildByName(this.UIRoot, "id_Skip", true, true);
			if (gameObject2 != null)
			{
				gameObject2.gameObject.SetActive(false);
			}
		}
		this.fly_by_points.Clear();
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x00053C90 File Offset: 0x00051E90
	private void Hide()
	{
		this.UIRoot.SetActive(false);
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x00053C9E File Offset: 0x00051E9E
	private void Show()
	{
		this.UIRoot.SetActive(true);
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x00053CAC File Offset: 0x00051EAC
	private void Update()
	{
		if (this.m_BeginWhenReady && !this.m_InPorgress)
		{
			this.m_BeginWhenReady = false;
			this.BuildCameraPath();
		}
		if (this.m_InPorgress && UICommon.GetKeyDown(KeyCode.Escape, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			this.Skip();
		}
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x00053CE4 File Offset: 0x00051EE4
	private DT.Field GetPointsField(int pk_side, out float default_delay)
	{
		default_delay = 0f;
		DT.Field field = global::Defs.Get(false).dt.Find("BattleviewOverview", null);
		if (field == null)
		{
			return null;
		}
		default_delay = field.GetFloat("delay", null, 0f, true, true, true, '.');
		string text = null;
		Logic.Battle battle = BattleMap.battle;
		Logic.Army army = battle.GetArmy(pk_side);
		Logic.Army army2 = battle.GetArmy(1 - pk_side);
		Logic.Battle.Type type = battle.type;
		if ((type == Logic.Battle.Type.BreakSiege || type == Logic.Battle.Type.Assault) && battle.settlement.type == "Keep")
		{
			type = Logic.Battle.Type.OpenField;
		}
		switch (type)
		{
		case Logic.Battle.Type.OpenField:
			if (army2.rebel != null)
			{
				if (army2.rebel.IsLoyalist())
				{
					text = "field_battle_vs_loyalist_rebels";
				}
				else if (army2.rebel.IsLeader() && army2.rebel.rebellion.IsFamous())
				{
					text = "field_battle_vs_famous_rebels";
				}
				else
				{
					text = "field_battle_vs_rebels";
				}
			}
			else if (((army2 != null) ? army2.FindStatus<ArmyDisorginizedStatus>() : null) != null)
			{
				text = "we_caught_enemy_disorganized";
			}
			else if (((army != null) ? army.FindStatus<ArmyDisorginizedStatus>() : null) != null)
			{
				text = "they_caught_us_disorganized";
			}
			else
			{
				text = "field_battle";
			}
			break;
		case Logic.Battle.Type.Assault:
			if (pk_side == 0)
			{
				text = "we_assault_enemy";
			}
			else
			{
				text = "enemy_assaults_us";
			}
			break;
		case Logic.Battle.Type.BreakSiege:
		{
			List<Logic.Army> armies = battle.GetArmies(pk_side);
			List<Logic.Army> armies2 = battle.GetArmies(1 - pk_side);
			bool flag = false;
			if (pk_side == 0)
			{
				for (int i = 0; i < armies2.Count; i++)
				{
					if (armies2[i].castle == null)
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < armies.Count; j++)
				{
					if (armies[j].castle == null)
					{
						flag = true;
						break;
					}
				}
			}
			if (pk_side == 0)
			{
				if (flag)
				{
					text = "enemy_breaks_siege_from_outside";
				}
				else
				{
					text = "enemy_breaks_siege_from_inside";
				}
			}
			else if (flag)
			{
				if (army2.rebel != null)
				{
					text = "we_break_siege_from_outside_rebels";
				}
				else
				{
					text = "we_break_siege_from_outside";
				}
			}
			else if (army2.rebel != null)
			{
				text = "we_break_siege_from_inside_rebels";
			}
			else
			{
				text = "we_break_siege_from_inside";
			}
			break;
		}
		case Logic.Battle.Type.PlunderInterrupt:
			if (pk_side == 0)
			{
				if (army2.rebel != null)
				{
					text = "we_pillage_enemy_rebels";
				}
				else
				{
					text = "we_pillage_enemy";
				}
			}
			else if (army2.rebel != null)
			{
				if (army2.rebel.IsLoyalist())
				{
					text = "enemy_pillages_us_loyalists";
				}
				else if (army2.rebel.IsLeader() && army2.rebel.rebellion.IsFamous())
				{
					text = "enemy_pillages_us_famous_rebels";
				}
				else
				{
					text = "enemy_pillages_us_rebels";
				}
			}
			else
			{
				text = "enemy_pillages_us";
			}
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return field.FindChild(text, null, true, true, true, '.');
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x00053FA8 File Offset: 0x000521A8
	private void GatherFlyByPoints()
	{
		this.fly_by_points = new List<BattleFieldOverview.FlyByPoint>();
		int num = -1;
		if (global::Battle.PlayerIsAttacker(BattleMap.battle, true))
		{
			num = 0;
		}
		else if (global::Battle.PlayerIsDefender(BattleMap.battle, true))
		{
			num = 1;
		}
		else
		{
			GameLogic.Get(true).Error("Player isn't in this battle");
		}
		float def_val;
		DT.Field field = this.GetPointsField(num, out def_val);
		List<DT.Field> list = new List<DT.Field>();
		while (field != null && field.type == "points")
		{
			for (int i = 0; i < field.children.Count; i++)
			{
				DT.Field field2 = field.children[i];
				bool flag = false;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].key == field2.key)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(field2);
				}
			}
			field = field.based_on;
		}
		for (int k = 0; k < list.Count; k++)
		{
			DT.Field field3 = list[k];
			if (field3 != null)
			{
				string key = field3.key;
				Transform transform = null;
				bool val = false;
				bool flag2 = true;
				uint num2 = <PrivateImplementationDetails>.ComputeStringHash(key);
				if (num2 <= 1620322081U)
				{
					if (num2 <= 582611469U)
					{
						if (num2 != 375450386U)
						{
							if (num2 != 582611469U)
							{
								goto IL_67E;
							}
							if (!(key == "enemy_army"))
							{
								goto IL_67E;
							}
						}
						else
						{
							if (!(key == "gate"))
							{
								goto IL_67E;
							}
							Logic.PassableGate passableGate = null;
							Transform transform2 = this.FindArmyTransform(num);
							if (!(transform2 == null))
							{
								float num3 = float.MaxValue;
								for (int l = 0; l < BattleMap.battle.gates.Count; l++)
								{
									Logic.PassableGate passableGate2 = BattleMap.battle.gates[l];
									if (passableGate2.IsDefeated())
									{
										PPos position = passableGate2.position;
										float num4 = position.SqrDist(transform2.position);
										if (num4 < num3)
										{
											num3 = num4;
											passableGate = passableGate2;
											val = true;
										}
									}
								}
								if (passableGate == null)
								{
									num3 = float.MaxValue;
									for (int m = 0; m < BattleMap.battle.gates.Count; m++)
									{
										Logic.PassableGate passableGate3 = BattleMap.battle.gates[m];
										if (!passableGate3.IsDefeated())
										{
											PPos position2 = passableGate3.position;
											float num5 = position2.SqrDist(transform2.position);
											if (num5 < num3)
											{
												num3 = num5;
												passableGate = passableGate3;
											}
										}
									}
								}
								global::Fortification fortification = passableGate.fortifications[0].visuals as global::Fortification;
								transform = ((fortification != null) ? fortification.transform : null);
								goto IL_67E;
							}
							goto IL_67E;
						}
					}
					else if (num2 != 1422345314U)
					{
						if (num2 != 1620322081U)
						{
							goto IL_67E;
						}
						if (!(key == "their_army"))
						{
							goto IL_67E;
						}
					}
					else
					{
						if (!(key == "tower"))
						{
							goto IL_67E;
						}
						Logic.Fortification fortification2 = null;
						Transform transform3 = this.FindArmyTransform(num);
						if (!(transform3 == null))
						{
							float num6 = float.MaxValue;
							for (int n = 0; n < BattleMap.battle.towers.Count; n++)
							{
								Logic.Fortification fortification3 = BattleMap.battle.towers[n];
								if (fortification3.gate == null && fortification3.IsDefeated())
								{
									PPos position3 = fortification3.position;
									float num7 = position3.SqrDist(transform3.position);
									if (num7 < num6)
									{
										num6 = num7;
										fortification2 = fortification3;
										val = true;
									}
								}
							}
							if (fortification2 == null)
							{
								for (int num8 = 0; num8 < BattleMap.battle.towers.Count; num8++)
								{
									Logic.Fortification fortification4 = BattleMap.battle.towers[num8];
									if (fortification4.gate == null && !fortification4.IsDefeated())
									{
										PPos position4 = fortification4.position;
										float num9 = position4.SqrDist(transform3.position);
										if (num9 < num6)
										{
											num6 = num9;
											fortification2 = fortification4;
										}
									}
								}
							}
							global::Fortification fortification5 = ((fortification2 != null) ? fortification2.visuals : null) as global::Fortification;
							transform = ((fortification5 != null) ? fortification5.transform : null);
							goto IL_67E;
						}
						goto IL_67E;
					}
					transform = this.FindArmyTransform(1 - num);
				}
				else if (num2 <= 2036944879U)
				{
					if (num2 != 1963634499U)
					{
						if (num2 == 2036944879U)
						{
							if (key == "initiative")
							{
								transform = null;
								flag2 = false;
							}
						}
					}
					else if (key == "citadel")
					{
						BattleMap battleMap = BattleMap.Get();
						Transform transform4;
						if (battleMap == null)
						{
							transform4 = null;
						}
						else
						{
							SettlementBV sbv = battleMap.sbv;
							if (sbv == null)
							{
								transform4 = null;
							}
							else
							{
								PrefabGrid citadel = sbv.citadel;
								transform4 = ((citadel != null) ? citadel.transform : null);
							}
						}
						transform = transform4;
					}
				}
				else if (num2 != 2108364519U)
				{
					if (num2 != 2650762412U)
					{
						if (num2 == 2804296981U)
						{
							if (key == "wall")
							{
								Logic.Fortification fortification6 = null;
								Transform transform5 = this.FindArmyTransform(num);
								if (!(transform5 == null))
								{
									float num10 = float.MaxValue;
									for (int num11 = 0; num11 < BattleMap.battle.fortifications.Count; num11++)
									{
										Logic.Fortification fortification7 = BattleMap.battle.fortifications[num11];
										if (fortification7.def.type == Logic.Fortification.Type.Wall && fortification7.IsDefeated())
										{
											PPos position5 = fortification7.position;
											float num12 = position5.SqrDist(transform5.position);
											if (num12 < num10)
											{
												num10 = num12;
												fortification6 = fortification7;
												val = true;
											}
										}
									}
									global::Fortification fortification8 = ((fortification6 != null) ? fortification6.visuals : null) as global::Fortification;
									transform = ((fortification8 != null) ? fortification8.transform : null);
								}
							}
						}
					}
					else if (key == "capture_point")
					{
						float num13 = float.MaxValue;
						Logic.CapturePoint capturePoint = null;
						Transform transform6 = this.FindArmyTransform(num);
						if (!(transform6 == null))
						{
							for (int num14 = 0; num14 < BattleMap.battle.capture_points.Count; num14++)
							{
								Logic.CapturePoint capturePoint2 = BattleMap.battle.capture_points[num14];
								if (capturePoint2.original_battle_side != num && capturePoint2.def.count_victory)
								{
									PPos position6 = capturePoint2.position;
									float num15 = position6.SqrDist(transform6.position);
									if (num15 < num13)
									{
										num13 = num15;
										capturePoint = capturePoint2;
									}
								}
							}
							global::CapturePoint capturePoint3 = ((capturePoint != null) ? capturePoint.visuals : null) as global::CapturePoint;
							transform = ((capturePoint3 != null) ? capturePoint3.transform : null);
						}
					}
				}
				else if (key == "our_army")
				{
					transform = this.FindArmyTransform(num);
				}
				IL_67E:
				if (!(transform == null) || !flag2)
				{
					Vars vars = new Vars();
					vars.Set<bool>("destroyed", val);
					string @string = field3.GetString("voice_line", vars, "", true, true, true, '.');
					if (@string != null)
					{
						Vector3 b = new Vector3(field3.GetFloat("offset_x", null, 0f, true, true, true, '.'), 0f, field3.GetFloat("offset_y", null, 0f, true, true, true, '.'));
						int order = field3.Int(null, 0);
						float @float = field3.GetFloat("transition_speed_mod", null, 1f, true, true, true, '.');
						this.fly_by_points.Add(new BattleFieldOverview.FlyByPoint
						{
							key = key,
							voice_line = @string,
							pt = (flag2 ? global::Common.SnapToTerrain(transform.position + b, 0f, null, -1f, false) : Vector3.zero),
							delay = field3.GetFloat("delay", null, def_val, true, true, true, '.'),
							order = order,
							speed_mod = @float
						});
					}
				}
			}
		}
		this.fly_by_points.Sort((BattleFieldOverview.FlyByPoint x, BattleFieldOverview.FlyByPoint y) => x.order.CompareTo(y.order));
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x00054790 File Offset: 0x00052990
	private Transform FindArmyTransform(int side)
	{
		List<Logic.Squad> list = BattleMap.battle.squads.Get(side);
		Logic.Kingdom sideKingdom = BattleMap.battle.GetSideKingdom(side);
		Logic.Squad squad = null;
		Logic.Squad squad2 = null;
		for (int i = 0; i < list.Count; i++)
		{
			Logic.Squad squad3 = list[i];
			if (!squad3.IsDefeated() && squad3.kingdom_id == sideKingdom.id)
			{
				if (squad3.def.type == Logic.Unit.Type.Noble)
				{
					squad = squad3;
					break;
				}
				if (squad2 == null)
				{
					squad2 = squad3;
				}
			}
		}
		if (((squad != null) ? squad.visuals : null) != null)
		{
			return (squad.visuals as global::Squad).transform;
		}
		if (((squad2 != null) ? squad2.visuals : null) != null)
		{
			return (squad2.visuals as global::Squad).transform;
		}
		return null;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x00054850 File Offset: 0x00052A50
	private void BuildCameraPath()
	{
		Debug.Log("Starting BattleFieldOverview");
		FMODWrapper.Snapshot snapshot = this.snapshot;
		if (snapshot != null)
		{
			snapshot.StartSnapshot();
		}
		GameCamera gameCamera = CameraController.GameCamera;
		if (gameCamera != null)
		{
			gameCamera.Camera.enabled = true;
		}
		GameSpeed.SupressSpeedChangesByPlayer = true;
		Logic.Battle battle = BattleMap.battle;
		Game game = (battle != null) ? battle.game : null;
		if (game != null)
		{
			Pause pause = game.pause;
			if (pause != null)
			{
				pause.Reset(true);
			}
		}
		if (game != null)
		{
			game.SetSpeed(1f, -1);
		}
		if (this.spline == null)
		{
			this.spline = new GameObject("CameraPathSpline").AddComponent<BezierSpline>();
		}
		DT.Field field = global::Defs.Get(false).dt.Find("BattleviewOverview", null);
		if (field == null)
		{
			return;
		}
		if (BattleMap.battle.has_restarted && field.GetBool("skip_on_restart", null, false, true, true, true, '.'))
		{
			int side = -1;
			if (global::Battle.PlayerIsAttacker(BattleMap.battle, true))
			{
				side = 0;
			}
			else if (global::Battle.PlayerIsDefender(BattleMap.battle, true))
			{
				side = 1;
			}
			Transform transform = this.FindArmyTransform(side);
			if (transform != null)
			{
				CameraController.LookAt(transform.position);
				this.Complete();
				return;
			}
		}
		else
		{
			this.GatherFlyByPoints();
		}
		Debug.Log(string.Format("Fly by points: {0}", this.fly_by_points.Count));
		if (this.fly_by_points.Count == 0)
		{
			this.Complete();
			return;
		}
		BattleMap.battle.SetStage(Logic.Battle.Stage.Preparing, false, 0f);
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI != null)
		{
			battleViewUI.Show(false, false);
		}
		BackgroundMusic.OnTrigger("BattleIntroTrigger", null);
		this.m_InPorgress = true;
		FlybyCamera component = base.GetComponent<FlybyCamera>();
		component.OnComplete = (Action)Delegate.Combine(component.OnComplete, new Action(this.HandleOnFlybyCameraPathComplete));
		KeyValuePair<Vector3, Quaternion> keyValuePair = CameraController.GameCamera.CalcPositionAndRotation(this.fly_by_points[0].pt, 1f);
		CameraController.GameCamera.Set(keyValuePair.Key, keyValuePair.Value, false);
		CameraController.GameCamera.LookAt(this.fly_by_points[0].pt, false);
		base.StartCoroutine(this.FlyByStep());
		if (this.UIRoot != null)
		{
			GameObject gameObject = global::Common.FindChildByName(this.UIRoot, "id_Skip", true, true);
			if (gameObject != null)
			{
				gameObject.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x00054AB3 File Offset: 0x00052CB3
	private IEnumerator FlyByStep()
	{
		if (this.fly_by_points.Count > 0)
		{
			BattleFieldOverview.FlyByPoint cur = this.fly_by_points[0];
			BaseUI.PlayVoiceEvent(cur.voice_line, null);
			yield return new WaitForSecondsRealtime(cur.delay);
			if (this.fly_by_points.Count > 0)
			{
				if (this.fly_by_points.Count >= 2)
				{
					Vector3[] array = new Vector3[2];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this.fly_by_points[i].pt;
						if (array[i] == Vector3.zero)
						{
							array[i] = CameraController.GameCamera.GetLookAtPoint();
						}
					}
					this.spline.Initialize(array.Length);
					for (int j = 0; j < array.Length; j++)
					{
						this.spline[j].position = array[j];
					}
					this.spline.AutoConstructSpline2();
					AnimationCurve zoomCurve = this.BuildZoomCurve(this.spline);
					FlybyCamera component = base.GetComponent<FlybyCamera>();
					CameraController.GameCamera.LookAt(array[0], false);
					component.Begin(this.spline, zoomCurve, null, null);
					component.speed_mod = cur.speed_mod;
				}
				this.fly_by_points.RemoveAt(0);
				if (this.fly_by_points.Count == 0)
				{
					this.Complete();
				}
			}
			cur = null;
		}
		yield break;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x00054AC4 File Offset: 0x00052CC4
	public void Skip()
	{
		if (this.fly_by_points.Count == 0)
		{
			return;
		}
		if (this.UIRoot != null)
		{
			GameObject gameObject = global::Common.FindChildByName(this.UIRoot, "id_Skip", true, true);
			if (gameObject != null)
			{
				gameObject.gameObject.SetActive(false);
			}
		}
		Vector3 pt = new Vector3(256f, 20f, 60f);
		FlybyCamera component = base.GetComponent<FlybyCamera>();
		if (component != null)
		{
			component.Stop();
			int count = 0;
			for (int i = this.fly_by_points.Count - 1; i >= 0; i--)
			{
				if (this.fly_by_points[i].pt != Vector3.zero)
				{
					count = i;
					break;
				}
			}
			if (this.fly_by_points.Count > 1)
			{
				this.fly_by_points.RemoveRange(0, count);
				this.fly_by_points.RemoveRange(1, this.fly_by_points.Count - 1);
			}
			pt = this.fly_by_points[0].pt;
		}
		CameraController.GameCamera.LockUserInput(false);
		CameraController.GameCamera.SetScheme("GameMode");
		base.StartCoroutine(this.SkipMoveCamera(0.3f, pt));
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x00054BF8 File Offset: 0x00052DF8
	private IEnumerator SkipMoveCamera(float duration, Vector3 endPoint)
	{
		Camera cam = CameraController.GameCamera.Camera;
		CameraController.GameCamera.Lock(true);
		KeyValuePair<Vector3, Quaternion> keyValuePair = CameraController.GameCamera.CalcPositionAndRotation(endPoint, 1f);
		Vector3 aimPoint = keyValuePair.Key;
		Quaternion aimRotation = keyValuePair.Value;
		Quaternion startingCameraRotation = cam.transform.rotation;
		float time = 0f;
		while (time <= duration)
		{
			cam.transform.position = Vector3.Lerp(cam.transform.position, aimPoint, time / duration);
			cam.transform.rotation = Quaternion.Lerp(startingCameraRotation, aimRotation, time / duration);
			time += UnityEngine.Time.unscaledDeltaTime;
			yield return null;
		}
		CameraController.GameCamera.Lock(false);
		CameraController.GameCamera.LookAt(endPoint, false);
		CameraController.GameCamera.Zoom(1f, false);
		this.Complete();
		yield break;
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x00054C15 File Offset: 0x00052E15
	private void HandleOnFlybyCameraPathComplete()
	{
		if (this.fly_by_points.Count > 0)
		{
			base.StartCoroutine(this.FlyByStep());
			return;
		}
		this.Complete();
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x00054C3C File Offset: 0x00052E3C
	private AnimationCurve BuildZoomCurve(BezierSpline spline)
	{
		Keyframe[] array = new Keyframe[spline.Count * 2 - 1];
		float[] array2 = new float[spline.Count - 1];
		float num = 0f;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = Vector3.Distance(spline[i].position, spline[i + 1].position);
			num += array2[i];
		}
		array[0].time = 0f;
		array[0].value = 0.5f;
		int num2 = 1;
		float num3 = 0f;
		for (int j = 0; j < spline.Count - 1; j++)
		{
			array[num2].time = (num3 + array2[j] / 2f) / num;
			array[num2].value = 0.5f + 0.5f * (array2[j] / num);
			num2++;
			array[num2].time = (num3 + array2[j]) / num;
			array[num2].value = 1f;
			num2++;
			num3 += array2[j];
		}
		return new AnimationCurve(array);
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x00054D66 File Offset: 0x00052F66
	private AnimationCurve BuildSpeedCurve(BezierSpline spline)
	{
		return new AnimationCurve();
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x00054D70 File Offset: 0x00052F70
	private Vector3[] ExtractPoints()
	{
		if (BattleMap.battle != null && BattleMap.battle.simulation != null)
		{
			int num = 2;
			Vector3[] array = new Vector3[num];
			KeyValuePair<Vector3, Vector3> startEndPoints = this.GetStartEndPoints();
			array[0] = startEndPoints.Key;
			array[num - 1] = startEndPoints.Value;
			return array;
		}
		GameObject gameObject = GameObject.Find("DimmyPoints");
		Vector3 position = new Vector3(35f, 10f, 35f);
		Vector3 position2 = new Vector3(430f, 10f, 430f);
		List<Vector3> list = new List<Vector3>();
		if (gameObject != null)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				Transform child = gameObject.transform.GetChild(i);
				if (child.name.Equals("StartPoint_Attacker"))
				{
					position = child.position;
				}
				if (child.name.Equals("StartPoint_Defender"))
				{
					position2 = child.position;
				}
				if (child.name.Contains("POI_"))
				{
					list.Add(child.position);
				}
			}
		}
		int num2 = 2 + list.Count;
		Vector3[] array2 = new Vector3[num2];
		array2[0] = position;
		array2[num2 - 1] = position2;
		for (int j = 0; j < list.Count; j++)
		{
			array2[1 + j] = list[j];
		}
		return array2;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x00054EE0 File Offset: 0x000530E0
	private KeyValuePair<Vector3, Vector3> GetStartEndPoints()
	{
		Vector3 vector = BaseUI.Get().GetTerrainSize() / 2f;
		Vector3? vector2 = null;
		Vector3? vector3 = null;
		if (BattleMap.battle != null && BattleMap.battle.simulation != null)
		{
			BattleSimulation simulation = BattleMap.battle.simulation;
			Logic.Squad squad = null;
			for (int i = 0; i < simulation.attacker_squads.Count; i++)
			{
				if (simulation.attacker_squads[i].def.type == Logic.Unit.Type.Noble && !simulation.attacker_squads[i].IsDefeated())
				{
					squad = simulation.attacker_squads[i].FindSquad();
					break;
				}
			}
			Logic.Squad squad2 = null;
			for (int j = 0; j < simulation.defender_squads.Count; j++)
			{
				if (simulation.defender_squads[j].def.type == Logic.Unit.Type.Noble && !simulation.defender_squads[j].IsDefeated())
				{
					squad2 = simulation.defender_squads[j].FindSquad();
					break;
				}
			}
			if (squad == null && simulation.attacker_squads.Count > 0)
			{
				for (int k = 0; k < simulation.attacker_squads.Count; k++)
				{
					if (!simulation.attacker_squads[k].IsDefeated())
					{
						squad = simulation.attacker_squads[k].FindSquad();
						break;
					}
				}
			}
			if (squad2 == null && simulation.defender_squads.Count > 0)
			{
				for (int l = 0; l < simulation.defender_squads.Count; l++)
				{
					if (!simulation.defender_squads[l].IsDefeated())
					{
						squad2 = simulation.defender_squads[l].FindSquad();
						break;
					}
				}
			}
			if (squad == null || squad2 == null)
			{
				BattleMap.battle.game.Error("No valid units for battlefield overview, please report this bug");
			}
			if (squad != null)
			{
				if (squad.kingdom_id == BattleMap.KingdomId)
				{
					global::Squad squad3 = squad.visuals as global::Squad;
					if (((squad3 != null) ? squad3.logic : null) != null)
					{
						vector3 = new Vector3?(global::Common.SnapToTerrain(squad3.logic.position, 0f, null, -1f, false));
					}
					else
					{
						vector3 = new Vector3?(vector);
					}
				}
				else
				{
					global::Squad squad4 = squad.visuals as global::Squad;
					if (((squad4 != null) ? squad4.logic : null) != null)
					{
						vector2 = new Vector3?(global::Common.SnapToTerrain(squad4.logic.position, 0f, null, -1f, false));
					}
					else
					{
						vector2 = new Vector3?(vector);
					}
				}
			}
			if (squad2 != null)
			{
				if (squad2.kingdom_id == BattleMap.KingdomId)
				{
					global::Squad squad5 = squad2.visuals as global::Squad;
					if (((squad5 != null) ? squad5.logic : null) != null)
					{
						vector3 = new Vector3?(global::Common.SnapToTerrain(squad5.logic.position, 0f, null, -1f, false));
					}
					else
					{
						vector3 = new Vector3?(vector);
					}
				}
				else
				{
					global::Squad squad6 = squad2.visuals as global::Squad;
					if (((squad6 != null) ? squad6.logic : null) != null)
					{
						vector2 = new Vector3?(global::Common.SnapToTerrain(squad6.logic.position, 0f, null, -1f, false));
					}
					else
					{
						vector2 = new Vector3?(vector);
					}
				}
			}
		}
		else
		{
			vector2 = new Vector3?(vector);
			vector3 = new Vector3?(vector);
		}
		vector2 = new Vector3?(vector2 ?? vector);
		vector3 = new Vector3?(vector3 ?? vector);
		return new KeyValuePair<Vector3, Vector3>(vector2.Value, vector3.Value);
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x00055294 File Offset: 0x00053494
	private List<Vector3> GetRandomPOIs(int cnt, Vector3 startPoint)
	{
		List<Vector3> list = new List<Vector3>(cnt);
		if (cnt == 0)
		{
			return list;
		}
		for (int i = 0; i < cnt; i++)
		{
			list.Add(BattleFieldOverview.RandomPointInBox(new Vector3(256f, 10f, 256f), new Vector3(420f, 1f, 250f)));
			if (this.POIPrefab != null)
			{
				Vector3 vector = list[i];
				vector.y = global::Common.GetTerrainHeight(vector, null, false);
				UnityEngine.Object.Instantiate<GameObject>(this.POIPrefab, vector, Quaternion.identity);
			}
		}
		if (cnt > 1)
		{
			List<Vector3> list2 = new List<Vector3>(cnt);
			Vector3 vector2 = startPoint;
			while (list.Count > 0)
			{
				if (list.Count == 1)
				{
					list2.Add(list[0]);
					break;
				}
				Vector3 vector3 = list[0];
				float num = Mathf.Abs(vector2.z - vector3.z);
				for (int j = 0; j < list.Count; j++)
				{
					float num2 = Mathf.Abs(vector2.z - list[j].z);
					if (num2 < num)
					{
						num = num2;
						vector3 = list[j];
					}
				}
				list.Remove(vector3);
				list2.Add(vector3);
				vector2 = vector3;
			}
			return list2;
		}
		return list;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x000553DC File Offset: 0x000535DC
	private static Vector3 RandomPointInBox(Vector3 center, Vector3 size)
	{
		return center + new Vector3((Random.value - 0.5f) * size.x, (Random.value - 0.5f) * size.y, (Random.value - 0.5f) * size.z);
	}

	// Token: 0x04000652 RID: 1618
	[Tooltip("if True, will activate BattleField Overview when in editor and not loaded through WorldView")]
	public bool playIfStandAlone;

	// Token: 0x04000653 RID: 1619
	[SerializeField]
	private GameObject UIRoot;

	// Token: 0x04000654 RID: 1620
	[SerializeField]
	private GameObject POIPrefab;

	// Token: 0x04000655 RID: 1621
	private bool m_BeginWhenReady;

	// Token: 0x04000656 RID: 1622
	private bool m_Ready;

	// Token: 0x04000657 RID: 1623
	private bool m_InPorgress;

	// Token: 0x04000658 RID: 1624
	private static BattleFieldOverview sm_Instance;

	// Token: 0x04000659 RID: 1625
	private FMODWrapper.Snapshot snapshot;

	// Token: 0x0400065A RID: 1626
	private List<BattleFieldOverview.FlyByPoint> fly_by_points;

	// Token: 0x0400065B RID: 1627
	private BezierSpline spline;

	// Token: 0x0200059C RID: 1436
	public class FlyByPoint
	{
		// Token: 0x040030FD RID: 12541
		public string key;

		// Token: 0x040030FE RID: 12542
		public string voice_line;

		// Token: 0x040030FF RID: 12543
		public float delay;

		// Token: 0x04003100 RID: 12544
		public Vector3 pt;

		// Token: 0x04003101 RID: 12545
		public int order;

		// Token: 0x04003102 RID: 12546
		public float speed_mod = 1f;
	}
}
