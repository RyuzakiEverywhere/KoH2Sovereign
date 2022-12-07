using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class UICapturePointsBar : MonoBehaviour, IListener
{
	// Token: 0x06001C3A RID: 7226 RVA: 0x0010AE65 File Offset: 0x00109065
	private void OnDisable()
	{
		this.Clear();
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x0010AE6D File Offset: 0x0010906D
	private void Update()
	{
		if (!this.m_isInitialized)
		{
			this.Initialize();
		}
		if (this.m_refresh)
		{
			this.Refresh();
		}
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x0010AE8B File Offset: 0x0010908B
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "capture_points_changed")
		{
			this.m_refresh = true;
			return;
		}
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x0010AEA4 File Offset: 0x001090A4
	private void Initialize()
	{
		if (this.m_isInitialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.Clear();
		BattleMap.battle.AddListener(this);
		Logic.Battle battle = BattleMap.battle;
		if (((battle != null) ? battle.capture_points : null) == null)
		{
			this.m_OwnRoot.gameObject.SetActive(false);
			this.m_EnemyRoot.gameObject.SetActive(false);
			return;
		}
		if (this.m_pointPrefab == null)
		{
			DT.Field field = global::Defs.Get(false).dt.Find("CapturePointSlot", null);
			if (field != null)
			{
				this.m_pointPrefab = field.GetRandomValue("window_prefab", null, true, true, true, '.').Get<GameObject>();
			}
		}
		float width = this.m_pointPrefab.GetComponent<RectTransform>().rect.width;
		this.m_OwnRoot.Initialize(width);
		this.m_EnemyRoot.Initialize(width);
		this.m_OwnRoot.gameObject.SetActive(true);
		this.m_EnemyRoot.gameObject.SetActive(true);
		this.CreateCapturePoints();
		this.m_isInitialized = true;
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x0010AFB0 File Offset: 0x001091B0
	private void CreateCapturePoints()
	{
		for (int i = 0; i < BattleMap.battle.capture_points.Count; i++)
		{
			Logic.CapturePoint capturePoint = BattleMap.battle.capture_points[i];
			if (capturePoint.def.count_victory)
			{
				GameObject gameObject = global::Common.Spawn(this.m_pointPrefab, false, false);
				if (capturePoint.battle_side == global::Battle.PlayerBattleSide())
				{
					gameObject.transform.SetParent(this.m_OwnRoot.transform, false);
				}
				else
				{
					gameObject.transform.SetParent(this.m_EnemyRoot.transform, false);
				}
				UICapturePoint component = gameObject.GetComponent<UICapturePoint>();
				component.SetData(capturePoint);
				component.onChange += this.CapturePoint_OnChange;
				this.m_capturePoints.Add(component);
			}
		}
	}

	// Token: 0x06001C3F RID: 7231 RVA: 0x0010B074 File Offset: 0x00109274
	private void Refresh()
	{
		if (!this.m_isInitialized)
		{
			this.Initialize();
		}
		for (int i = 0; i < this.m_capturePoints.Count; i++)
		{
			if (this.m_capturePoints[i].Logic.battle_side == global::Battle.PlayerBattleSide())
			{
				this.m_OwnRoot.AddPoint(this.m_capturePoints[i]);
			}
			else
			{
				this.m_EnemyRoot.AddPoint(this.m_capturePoints[i]);
			}
		}
		this.m_OwnRoot.Refresh();
		this.m_EnemyRoot.Refresh();
		this.m_refresh = false;
	}

	// Token: 0x06001C40 RID: 7232 RVA: 0x0010B110 File Offset: 0x00109310
	private void Clear()
	{
		foreach (UICapturePoint uicapturePoint in this.m_capturePoints)
		{
			uicapturePoint.onChange -= this.CapturePoint_OnChange;
		}
		this.m_capturePoints.Clear();
		if (this.m_OwnRoot != null)
		{
			this.m_OwnRoot.Clear();
		}
		if (this.m_EnemyRoot != null)
		{
			this.m_EnemyRoot.Clear();
		}
		this.m_isInitialized = false;
	}

	// Token: 0x06001C41 RID: 7233 RVA: 0x0010B1B0 File Offset: 0x001093B0
	private void CapturePoint_OnChange()
	{
		this.m_refresh = true;
	}

	// Token: 0x04001267 RID: 4711
	[UIFieldTarget("id_OwnRoot")]
	private UIBattleViewCapturePointsContainer m_OwnRoot;

	// Token: 0x04001268 RID: 4712
	[UIFieldTarget("id_EnemyRoot")]
	private UIBattleViewCapturePointsContainer m_EnemyRoot;

	// Token: 0x04001269 RID: 4713
	private bool m_isInitialized;

	// Token: 0x0400126A RID: 4714
	private bool m_refresh;

	// Token: 0x0400126B RID: 4715
	private GameObject m_pointPrefab;

	// Token: 0x0400126C RID: 4716
	private List<UICapturePoint> m_capturePoints = new List<UICapturePoint>();
}
