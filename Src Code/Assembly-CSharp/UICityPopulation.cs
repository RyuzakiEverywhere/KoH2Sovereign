using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AF RID: 687
public class UICityPopulation : MonoBehaviour, IListener
{
	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06002B2C RID: 11052 RVA: 0x0016DCBD File Offset: 0x0016BEBD
	// (set) Token: 0x06002B2D RID: 11053 RVA: 0x0016DCC5 File Offset: 0x0016BEC5
	public Castle Data { get; private set; }

	// Token: 0x06002B2E RID: 11054 RVA: 0x0016DCD0 File Offset: 0x0016BED0
	public void SetData(Castle castle)
	{
		UICommon.FindComponents(this, false);
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.Data = castle;
		if (this.Data != null)
		{
			this.Data.AddListener(this);
		}
		this.vars.obj = this.Data;
		this.Init();
		this.Refresh();
	}

	// Token: 0x06002B2F RID: 11055 RVA: 0x0016DD35 File Offset: 0x0016BF35
	private void OnDestroy()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.CleanIcons();
	}

	// Token: 0x06002B30 RID: 11056 RVA: 0x0016DD54 File Offset: 0x0016BF54
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		this.iconMaps.Clear();
		this.iconMaps.Add(0, UICityPopulation.IconData.Build(global::Common.FindChildByName(base.gameObject, "id_RebelPrototype", true, true)));
		this.iconMaps.Add(1, UICityPopulation.IconData.Build(global::Common.FindChildByName(base.gameObject, "id_WorkerPrototype", true, true)));
		this.iconMaps.Add(2, UICityPopulation.IconData.Build(global::Common.FindChildByName(base.gameObject, "id_LevyPrototype", true, true)));
		this.iconMaps.Add(3, UICityPopulation.IconData.Build(global::Common.FindChildByName(base.gameObject, "id_WorkerDisorderPrototype", true, true)));
		this.iconMaps.Add(4, UICityPopulation.IconData.Build(global::Common.FindChildByName(base.gameObject, "id_RebelsDisorderPrototype", true, true)));
		this.workerContainer = global::Common.FindChildByName(base.gameObject, "id_WorkerIconsContainer", true, true);
		if (this.workerContainer != null)
		{
			this.workerContainerWidth = (int)(this.workerContainer.transform as RectTransform).rect.width;
		}
		if (this.m_PopMajority != null)
		{
			this.m_PopMajority.onClick = new BSGButton.OnClick(this.HandleOnPopMajorityClick);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x0016DE9A File Offset: 0x0016C09A
	private void CleanIcons()
	{
		if (this.workerContainer != null)
		{
			UICityPopulation.DeleteActiveChildren(this.workerContainer.transform);
		}
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x0016DEBA File Offset: 0x0016C0BA
	private void Update()
	{
		this.Init();
		this.UpdatePopulionProgress();
		this.invalidate |= this.CheckSlotsChange();
		if (this.invalidate)
		{
			this.Refresh();
			this.invalidate = false;
		}
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x0016DEF0 File Offset: 0x0016C0F0
	private bool CheckSlotsChange()
	{
		if (this.Data == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i <= 2; i++)
		{
			int num = this.Data.population.Slots((Population.Type)i, false);
			if (this.slots[i] != num)
			{
				result = true;
				this.slots[i] = num;
			}
		}
		return result;
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x0016DF3F File Offset: 0x0016C13F
	private void OnValidate()
	{
		this.invalidate = true;
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x0016DF48 File Offset: 0x0016C148
	private void RebuildLayout()
	{
		this.Init();
		this.CleanIcons();
		this.PopulatreWorkers();
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x0016DF5C File Offset: 0x0016C15C
	private void UpdatePopMajority()
	{
		if (this.m_PopMajorityCrest != null)
		{
			Logic.Realm realm = this.Data.GetRealm();
			if (realm == null)
			{
				return;
			}
			if (realm.pop_majority.kingdom != realm.GetKingdom())
			{
				this.m_PopMajorityCrest.gameObject.SetActive(true);
				this.m_PopMajorityCrest.SetObject(realm.pop_majority.kingdom, null);
				return;
			}
			this.m_PopMajorityCrest.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x0016DFD4 File Offset: 0x0016C1D4
	private void PopulatreWorkers()
	{
		UICityPopulation.IconData iconData = this.iconMaps[0];
		UICityPopulation.IconData iconData2 = this.iconMaps[1];
		UICityPopulation.IconData iconData3 = this.iconMaps[3];
		UICityPopulation.IconData iconData4 = this.iconMaps[4];
		Canvas componentInParent = base.GetComponentInParent<Canvas>();
		if (this.GetSlots(Population.Type.Worker) + this.GetSlots(Population.Type.Rebel) > 0 && this.workerContainer != null)
		{
			float num = iconData.iconWidth / 3f;
			int num2 = this.GetSlots(Population.Type.TOTAL);
			int count = this.GetCount(Population.Type.Rebel);
			float num3 = (float)count * iconData.iconWidth;
			int count2 = this.GetCount(Population.Type.Worker);
			float num4 = (float)count2 * iconData2.iconWidth;
			float num5 = num3 + num + num4 + iconData2.iconWidth * (float)(num2 - (count2 + count));
			float num6 = (num5 <= (float)this.workerContainerWidth) ? 1f : (((float)this.workerContainerWidth - num) / (num5 - num));
			num6 *= componentInParent.scaleFactor;
			float num7 = 0f;
			bool flag = this.Data.GetRealm().IsDisorder();
			if (flag)
			{
				if (iconData4.prototype)
				{
					num7 = iconData4.iconWidth / 2f;
					for (int i = 0; i < count; i++)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(iconData4.prototype, this.workerContainer.transform);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.Translate(num7 + iconData4.iconWidth * (float)i * num6, 0f, 0f, Space.Self);
						gameObject.gameObject.SetActive(true);
					}
				}
			}
			else if (iconData.prototype)
			{
				num7 = iconData.iconWidth / 2f;
				for (int j = 0; j < count; j++)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(iconData.prototype, this.workerContainer.transform);
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.Translate(num7 + iconData.iconWidth * (float)j * num6, 0f, 0f, Space.Self);
					gameObject2.gameObject.SetActive(true);
				}
			}
			num7 += ((num + (float)count2 > 0f) ? (num3 * num6) : 0f);
			if (flag)
			{
				if (iconData3.prototype)
				{
					for (int k = 0; k < count2; k++)
					{
						GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(iconData3.prototype, this.workerContainer.transform);
						gameObject3.transform.localPosition = Vector3.zero;
						gameObject3.transform.Translate(num7 + iconData3.iconWidth * (float)k * num6, 0f, 0f, Space.Self);
						gameObject3.gameObject.SetActive(true);
					}
					return;
				}
			}
			else if (iconData2.prototype)
			{
				for (int l = 0; l < count2; l++)
				{
					GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(iconData2.prototype, this.workerContainer.transform);
					gameObject4.transform.localPosition = Vector3.zero;
					gameObject4.transform.Translate(num7 + iconData2.iconWidth * (float)l * num6, 0f, 0f, Space.Self);
					gameObject4.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x0016E30D File Offset: 0x0016C50D
	public void Refresh()
	{
		if (this.Data != null)
		{
			this.Data.population.Recalc(false);
		}
		this.RebuildLayout();
		this.BuildVars();
		this.BuildTooltip();
		this.PopulateLabels();
		this.UpdatePopulionProgress();
		this.UpdatePopMajority();
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x0016E350 File Offset: 0x0016C550
	private void BuildVars()
	{
		if (this.Data == null)
		{
			return;
		}
		this.vars.Set<int>("workers", this.Data.population.Count(Population.Type.Worker, true));
		int val = this.Data.population.Slots(Population.Type.Worker, true) + this.Data.population.Slots(Population.Type.Rebel, true);
		this.vars.Set<int>("worker_slots", val);
		this.vars.Set<int>("rebellious_population", this.Data.population.Count(Population.Type.Rebel, true));
		this.vars.Set<bool>("has_rebellious_population", this.Data.population.Count(Population.Type.Rebel, true) > 0);
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x0016E408 File Offset: 0x0016C608
	private void BuildTooltip()
	{
		if (this.Data == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_IconTotalWorkers", true, true);
		if (gameObject != null)
		{
			Tooltip.Get(gameObject, true).SetDef("WorkersTotalTooltip", this.vars);
		}
		GameObject gameObject2 = global::Common.FindChildByName(base.gameObject, "id_TotalWorkers", true, true);
		if (gameObject2 != null)
		{
			Tooltip.Get(gameObject2, true).SetDef("WorkersTotalTooltip", this.vars);
		}
		GameObject gameObject3 = global::Common.FindChildByName(base.gameObject, "id_PopMajority", true, true);
		if (gameObject3 != null)
		{
			Tooltip.Get(gameObject3, true).SetDef("MajorityAndCultureTooltip", this.vars);
		}
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x0016E4B8 File Offset: 0x0016C6B8
	private void UpdatePopulionProgress()
	{
		if (this.Data == null)
		{
			return;
		}
		Castle data = this.Data;
		bool? flag;
		if (data == null)
		{
			flag = null;
		}
		else
		{
			Logic.Realm realm = data.GetRealm();
			flag = ((realm != null) ? new bool?(realm.IsDisorder()) : null);
		}
		bool flag2 = flag ?? true;
		this.m_NextWorkerProgress.gameObject.SetActive(!flag2);
		if (this.m_NextWorkerProgress != null)
		{
			this.m_NextWorkerProgress.fillAmount = this.Data.population.GetNextVilagerProgress(Population.Type.Worker);
		}
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x0016E554 File Offset: 0x0016C754
	private void PopulateLabels()
	{
		if (this.Data == null || this.Data.population == null)
		{
			return;
		}
		UIText.SetTextKey(base.gameObject, "id_WorkerValue", "CastleWindow.population.workers", this.vars, null);
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x0016E588 File Offset: 0x0016C788
	private void HandleOnPopMajorityClick(BSGButton b)
	{
		Logic.Realm realm = this.Data.GetRealm();
		if (realm == null)
		{
			return;
		}
		if (realm.pop_majority.kingdom != realm.GetKingdom())
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI != null)
			{
				int id = realm.pop_majority.kingdom.id;
				worldUI.DoubleClickCheck();
				if (worldUI.selected_kingdom == null || worldUI.selected_kingdom.id != id)
				{
					worldUI.SelectKingdom(id, true);
					return;
				}
				if (worldUI.dblclk && worldUI.selected_kingdom != null && worldUI.selected_kingdom.logic != null)
				{
					Logic.Realm capital = worldUI.selected_kingdom.logic.GetCapital();
					if (capital != null)
					{
						worldUI.LookAt(capital.castle.position, false);
					}
				}
			}
		}
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x0016E648 File Offset: 0x0016C848
	private int GetCount(Population.Type type)
	{
		if (Application.isPlaying)
		{
			return this.Data.population.Count(type, true);
		}
		switch (type)
		{
		case Population.Type.Rebel:
			return Mathf.Max(0, this.rebel_cnt);
		case Population.Type.Worker:
			return Mathf.Max(0, this.workers_cnt);
		case Population.Type.TOTAL:
			return Mathf.Max(0, this.total_slots);
		default:
			return 0;
		}
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x0016E6AB File Offset: 0x0016C8AB
	private int GetSlots(Population.Type type)
	{
		if (Application.isPlaying)
		{
			return this.Data.population.Slots(type, true);
		}
		if (type == Population.Type.Rebel)
		{
			return Mathf.Max(0, this.worker_slots);
		}
		if (type != Population.Type.Worker)
		{
			return 0;
		}
		return Mathf.Max(0, this.worker_slots);
	}

	// Token: 0x06002B41 RID: 11073 RVA: 0x0016E6EC File Offset: 0x0016C8EC
	private static void DeleteActiveChildren(Transform t)
	{
		if (t == null)
		{
			return;
		}
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			if (t.GetChild(i).gameObject.activeSelf)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(t.GetChild(i).gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(t.GetChild(i).gameObject);
				}
			}
		}
	}

	// Token: 0x04001D6F RID: 7535
	[UIFieldTarget("id_PopMajorityKingdom")]
	public UIKingdomIcon m_PopMajorityCrest;

	// Token: 0x04001D70 RID: 7536
	[UIFieldTarget("id_PopMajority")]
	public BSGButton m_PopMajority;

	// Token: 0x04001D71 RID: 7537
	[UIFieldTarget("id_Porgress")]
	public Image m_NextWorkerProgress;

	// Token: 0x04001D72 RID: 7538
	[Range(0f, 100f)]
	public int total_slots = 12;

	// Token: 0x04001D73 RID: 7539
	[Range(0f, 100f)]
	public int worker_slots = 5;

	// Token: 0x04001D74 RID: 7540
	[Range(0f, 100f)]
	public int workers_cnt = 2;

	// Token: 0x04001D75 RID: 7541
	[Range(0f, 100f)]
	public int rebel_cnt = 1;

	// Token: 0x04001D76 RID: 7542
	public int workerContainerWidth = 165;

	// Token: 0x04001D78 RID: 7544
	private bool invalidate;

	// Token: 0x04001D79 RID: 7545
	private GameObject workerContainer;

	// Token: 0x04001D7A RID: 7546
	private Dictionary<int, UICityPopulation.IconData> iconMaps = new Dictionary<int, UICityPopulation.IconData>();

	// Token: 0x04001D7B RID: 7547
	private Vars vars = new Vars();

	// Token: 0x04001D7C RID: 7548
	private bool m_Initialized;

	// Token: 0x04001D7D RID: 7549
	private int[] slots = new int[3];

	// Token: 0x0200080F RID: 2063
	private class IconData
	{
		// Token: 0x06004F87 RID: 20359 RVA: 0x00235C88 File Offset: 0x00233E88
		public static UICityPopulation.IconData Build(GameObject prototype)
		{
			UICityPopulation.IconData iconData = new UICityPopulation.IconData();
			if (prototype != null)
			{
				prototype.SetActive(false);
				iconData.prototype = prototype;
				iconData.iconWidth = (prototype.transform as RectTransform).rect.width;
			}
			return iconData;
		}

		// Token: 0x04003D99 RID: 15769
		public GameObject prototype;

		// Token: 0x04003D9A RID: 15770
		public float iconWidth;
	}
}
