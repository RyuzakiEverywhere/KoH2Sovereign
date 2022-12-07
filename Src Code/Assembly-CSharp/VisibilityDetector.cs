using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000317 RID: 791
public class VisibilityDetector : MonoBehaviour
{
	// Token: 0x06003177 RID: 12663 RVA: 0x0018FA65 File Offset: 0x0018DC65
	public static Camera GetCullingCamera(string groupId)
	{
		if (VisibilityDetector.visibliltyGroups.ContainsKey(groupId))
		{
			return VisibilityDetector.visibliltyGroups[groupId].culling_group.targetCamera;
		}
		return null;
	}

	// Token: 0x06003178 RID: 12664 RVA: 0x0018FA8C File Offset: 0x0018DC8C
	public static void SetCamera(Camera camera, string groupId = "WorldView")
	{
		if (!Application.isPlaying)
		{
			return;
		}
		VisibilityDetector.CullingGroupInfo group = VisibilityDetector.GetGroup(groupId, true);
		if (group.culling_group == null)
		{
			Debug.Log("group " + groupId + " is null");
			return;
		}
		group.culling_group.targetCamera = camera;
		group.culling_group.SetDistanceReferencePoint(camera.transform);
		VisibilityDetector.SetLayers(camera.layerCullDistances, group, camera.farClipPlane);
	}

	// Token: 0x06003179 RID: 12665 RVA: 0x0018FAF8 File Offset: 0x0018DCF8
	private static void SetLayers(float[] distances, VisibilityDetector.CullingGroupInfo group, float farClipPlane)
	{
		List<float> list = new List<float>();
		for (int i = 0; i < distances.Length; i++)
		{
			if (distances[i] != 0f && !list.Contains(distances[i]))
			{
				list.Add(distances[i]);
			}
		}
		list.Add(farClipPlane);
		list.Sort();
		group.culling_group.SetBoundingDistances(list.ToArray());
		if (VisibilityDetector.boundIndexes == null)
		{
			VisibilityDetector.boundIndexes = new int[distances.Length];
			for (int j = 0; j < distances.Length; j++)
			{
				if (distances[j] == 0f)
				{
					VisibilityDetector.boundIndexes[j] = list.Count - 1;
				}
				else
				{
					for (int k = 0; k < list.Count; k++)
					{
						if (distances[j] == list[k])
						{
							VisibilityDetector.boundIndexes[j] = k;
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600317A RID: 12666 RVA: 0x0018FBB9 File Offset: 0x0018DDB9
	public static int Add(Vector3 pos, float radius, GameObject obj, VisibilityDetector.IVisibilityChanged changed = null, int layer = -1)
	{
		if (!Application.isPlaying)
		{
			return -1;
		}
		return VisibilityDetector.Add("WorldView", pos, radius, obj, changed, layer);
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x0018FBD4 File Offset: 0x0018DDD4
	public static int Add(string groupId, Vector3 pos, float radius, GameObject obj, VisibilityDetector.IVisibilityChanged changed = null, int layer = -1)
	{
		if (!Application.isPlaying)
		{
			return -1;
		}
		return VisibilityDetector.GetGroup(groupId, true).Add(pos, radius, obj, changed, layer);
	}

	// Token: 0x0600317C RID: 12668 RVA: 0x0018FBF2 File Offset: 0x0018DDF2
	public static void Move(int idx, Vector3 pos, float radius = -1f)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		VisibilityDetector.Move("WorldView", idx, pos, radius);
	}

	// Token: 0x0600317D RID: 12669 RVA: 0x0018FC09 File Offset: 0x0018DE09
	public static void Move(string groupId, int idx, Vector3 pos, float radius = -1f)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		VisibilityDetector.GetGroup(groupId, true).Move(idx, pos, radius);
	}

	// Token: 0x0600317E RID: 12670 RVA: 0x0018FC22 File Offset: 0x0018DE22
	public static void Del(int idx)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		VisibilityDetector.Del("WorldView", idx);
	}

	// Token: 0x0600317F RID: 12671 RVA: 0x0018FC38 File Offset: 0x0018DE38
	public static void Del(string groupId, int idx)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		VisibilityDetector.CullingGroupInfo group = VisibilityDetector.GetGroup(groupId, false);
		if (group != null)
		{
			group.Del(idx);
		}
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x0018FC60 File Offset: 0x0018DE60
	public static void ResyncVisibility()
	{
		foreach (KeyValuePair<string, VisibilityDetector.CullingGroupInfo> keyValuePair in VisibilityDetector.visibliltyGroups)
		{
			keyValuePair.Value.ResyncVisibility();
		}
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x0018FCB8 File Offset: 0x0018DEB8
	public static void EnableGroup(bool enablde, string group = "")
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (group == "")
		{
			using (Dictionary<string, VisibilityDetector.CullingGroupInfo>.Enumerator enumerator = VisibilityDetector.visibliltyGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, VisibilityDetector.CullingGroupInfo> keyValuePair = enumerator.Current;
					VisibilityDetector.CullingGroupInfo value = keyValuePair.Value;
					VisibilityDetector.CullingGroupInfo value2 = keyValuePair.Value;
					if (value2 != null)
					{
						value2.Enable(enablde);
					}
				}
				return;
			}
		}
		VisibilityDetector.CullingGroupInfo group2 = VisibilityDetector.GetGroup(group, false);
		if (group2 == null)
		{
			return;
		}
		group2.Enable(enablde);
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x0018FD48 File Offset: 0x0018DF48
	public static void ClearAll(string group = "")
	{
		if (!Application.isPlaying)
		{
			return;
		}
		foreach (KeyValuePair<string, VisibilityDetector.CullingGroupInfo> keyValuePair in VisibilityDetector.visibliltyGroups)
		{
			if (!(group != "") || !(keyValuePair.Key != group))
			{
				keyValuePair.Value.Dispose();
			}
		}
		if (group == "")
		{
			VisibilityDetector.visibliltyGroups.Clear();
			return;
		}
		VisibilityDetector.visibliltyGroups.Remove(group);
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x0018FDE8 File Offset: 0x0018DFE8
	private static VisibilityDetector.CullingGroupInfo GetGroup(string groupId, bool create = true)
	{
		if (!Application.isPlaying)
		{
			return null;
		}
		VisibilityDetector.CullingGroupInfo cullingGroupInfo;
		if (!VisibilityDetector.visibliltyGroups.TryGetValue(groupId, out cullingGroupInfo))
		{
			if (!create)
			{
				return null;
			}
			cullingGroupInfo = VisibilityDetector.CullingGroupInfo.Create();
			VisibilityDetector.visibliltyGroups.Add(groupId, cullingGroupInfo);
		}
		return cullingGroupInfo;
	}

	// Token: 0x06003184 RID: 12676 RVA: 0x0018FE25 File Offset: 0x0018E025
	private void Awake()
	{
		VisibilityDetector.ref_count++;
	}

	// Token: 0x06003185 RID: 12677 RVA: 0x0018FE34 File Offset: 0x0018E034
	private void Start()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			VisibilityDetector.Add(this.groupId, child.transform.position, this.radius, child.gameObject, null, -1);
		}
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x0018FE8C File Offset: 0x0018E08C
	private void OnDestroy()
	{
		if (VisibilityDetector.visibliltyGroups.ContainsKey(this.groupId))
		{
			VisibilityDetector.CullingGroupInfo cullingGroupInfo = VisibilityDetector.visibliltyGroups[this.groupId];
			for (int i = 0; i < base.transform.childCount; i++)
			{
				cullingGroupInfo.Del(base.transform.GetChild(i).gameObject);
			}
		}
		if (--VisibilityDetector.ref_count > 0)
		{
			return;
		}
		foreach (KeyValuePair<string, VisibilityDetector.CullingGroupInfo> keyValuePair in VisibilityDetector.visibliltyGroups)
		{
			keyValuePair.Value.Dispose();
		}
		VisibilityDetector.visibliltyGroups.Clear();
	}

	// Token: 0x04002116 RID: 8470
	public const string DEFAUL_GROUP = "WorldView";

	// Token: 0x04002117 RID: 8471
	public float radius = 10f;

	// Token: 0x04002118 RID: 8472
	public string groupId = "WorldView";

	// Token: 0x04002119 RID: 8473
	private static int ref_count = 0;

	// Token: 0x0400211A RID: 8474
	private static Dictionary<string, VisibilityDetector.CullingGroupInfo> visibliltyGroups = new Dictionary<string, VisibilityDetector.CullingGroupInfo>();

	// Token: 0x0400211B RID: 8475
	public static int[] boundIndexes = null;

	// Token: 0x0200087E RID: 2174
	public interface IVisibilityChanged
	{
		// Token: 0x06005152 RID: 20818
		void VisibilityChanged(bool visible);
	}

	// Token: 0x0200087F RID: 2175
	private class CullingGroupInfo
	{
		// Token: 0x06005153 RID: 20819 RVA: 0x0023C8B2 File Offset: 0x0023AAB2
		public bool IsEnabled()
		{
			return this.cg_enabled;
		}

		// Token: 0x06005154 RID: 20820 RVA: 0x0023C8BC File Offset: 0x0023AABC
		public static VisibilityDetector.CullingGroupInfo Create()
		{
			VisibilityDetector.CullingGroupInfo cullingGroupInfo = new VisibilityDetector.CullingGroupInfo();
			cullingGroupInfo.spheres = new BoundingSphere[2048];
			cullingGroupInfo.infos = new VisibilityDetector.CullingGroupInfo.Info[cullingGroupInfo.spheres.Length];
			cullingGroupInfo.culling_group = new CullingGroup();
			cullingGroupInfo.culling_group.targetCamera = null;
			cullingGroupInfo.culling_group.SetBoundingSpheres(cullingGroupInfo.spheres);
			cullingGroupInfo.culling_group.onStateChanged = new CullingGroup.StateChanged(cullingGroupInfo.OnCullingEvent);
			return cullingGroupInfo;
		}

		// Token: 0x06005155 RID: 20821 RVA: 0x0023C934 File Offset: 0x0023AB34
		public int Add(Vector3 pos, float radius, GameObject obj, VisibilityDetector.IVisibilityChanged changed = null, int layer = -1)
		{
			if (VisibilityDetector.boundIndexes == null)
			{
				return -1;
			}
			int num = this.first_free;
			if (num < 0)
			{
				if (this.count >= this.spheres.Length)
				{
					int num2 = 2 * this.spheres.Length;
					BoundingSphere[] array = new BoundingSphere[num2];
					VisibilityDetector.CullingGroupInfo.Info[] array2 = new VisibilityDetector.CullingGroupInfo.Info[num2];
					for (int i = 0; i < this.spheres.Length; i++)
					{
						array[i] = this.spheres[i];
						array2[i] = this.infos[i];
					}
					this.spheres = array;
					this.infos = array2;
					this.culling_group.SetBoundingSpheres(this.spheres);
				}
				num = this.count;
				this.count++;
				this.culling_group.SetBoundingSphereCount(this.count);
			}
			else
			{
				this.first_free = this.infos[num].next;
			}
			this.spheres[num] = new BoundingSphere(pos, radius);
			this.infos[num].obj = obj;
			this.infos[num].changed = changed;
			this.infos[num].visible = false;
			this.infos[num].next = -1;
			if (layer == -1)
			{
				if (obj != null)
				{
					this.infos[num].distanceBoundIndex = VisibilityDetector.boundIndexes[obj.layer];
				}
				else
				{
					this.infos[num].distanceBoundIndex = VisibilityDetector.boundIndexes.Length - 1;
				}
			}
			else
			{
				this.infos[num].distanceBoundIndex = VisibilityDetector.boundIndexes[layer];
			}
			this.OnInvisible(num);
			return num;
		}

		// Token: 0x06005156 RID: 20822 RVA: 0x0023CADC File Offset: 0x0023ACDC
		public void Del(int idx)
		{
			if (this.infos == null)
			{
				return;
			}
			this.infos[idx].next = this.first_free;
			this.first_free = idx;
			this.spheres[idx].position = new Vector3(-10000f, 10000f, -10000f);
			this.spheres[idx].radius = 0f;
			this.infos[idx].obj = null;
			this.infos[idx].changed = null;
		}

		// Token: 0x06005157 RID: 20823 RVA: 0x0023CB70 File Offset: 0x0023AD70
		public void Del(GameObject obj)
		{
			if (this.infos == null || this.infos.Length == 0)
			{
				return;
			}
			for (int i = 0; i < this.infos.Length; i++)
			{
				if (this.infos[i].obj == obj)
				{
					this.Del(i);
					return;
				}
			}
		}

		// Token: 0x06005158 RID: 20824 RVA: 0x0023CBC4 File Offset: 0x0023ADC4
		public void Move(int idx, Vector3 pos, float radius = -1f)
		{
			if (idx < 0)
			{
				return;
			}
			if (this.spheres == null)
			{
				return;
			}
			if (this.spheres.Length - 1 < idx)
			{
				return;
			}
			this.spheres[idx].position = pos;
			if (radius > 0f)
			{
				this.spheres[idx].radius = radius;
			}
		}

		// Token: 0x06005159 RID: 20825 RVA: 0x0023CC1C File Offset: 0x0023AE1C
		public void Enable(bool enable)
		{
			if (this.cg_enabled == enable)
			{
				return;
			}
			this.cg_enabled = enable;
			for (int i = 0; i < this.count; i++)
			{
				if (this.cg_enabled)
				{
					if (!this.infos[i].visible)
					{
						this.OnVisible(i);
					}
				}
				else if (this.infos[i].visible)
				{
					this.OnInvisible(i);
				}
			}
		}

		// Token: 0x0600515A RID: 20826 RVA: 0x0023CC8C File Offset: 0x0023AE8C
		private void OnCullingEvent(CullingGroupEvent sphere)
		{
			int index = sphere.index;
			VisibilityDetector.CullingGroupInfo.Info info = this.infos[index];
			bool flag = sphere.isVisible && sphere.currentDistance <= info.distanceBoundIndex;
			this.infos[index].visible = flag;
			if (this.cg_enabled && flag != info.visible)
			{
				if (flag)
				{
					this.OnVisible(index);
					return;
				}
				this.OnInvisible(index);
			}
		}

		// Token: 0x0600515B RID: 20827 RVA: 0x0023CD04 File Offset: 0x0023AF04
		private void OnVisible(int idx)
		{
			GameObject obj = this.infos[idx].obj;
			if (obj != null)
			{
				obj.SetActive(true);
			}
			VisibilityDetector.IVisibilityChanged changed = this.infos[idx].changed;
			if (changed != null)
			{
				changed.VisibilityChanged(true);
			}
		}

		// Token: 0x0600515C RID: 20828 RVA: 0x0023CD50 File Offset: 0x0023AF50
		private void OnInvisible(int idx)
		{
			VisibilityDetector.IVisibilityChanged changed = this.infos[idx].changed;
			if (changed != null)
			{
				changed.VisibilityChanged(false);
			}
			GameObject obj = this.infos[idx].obj;
			if (obj != null)
			{
				obj.SetActive(false);
			}
		}

		// Token: 0x0600515D RID: 20829 RVA: 0x0023CD9C File Offset: 0x0023AF9C
		public void ResyncVisibility()
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.infos[i].visible)
				{
					this.OnVisible(i);
				}
				else
				{
					this.OnInvisible(i);
				}
			}
		}

		// Token: 0x0600515E RID: 20830 RVA: 0x0023CDDD File Offset: 0x0023AFDD
		public void Dispose()
		{
			if (this.culling_group != null)
			{
				this.culling_group.Dispose();
				this.culling_group = null;
			}
			this.spheres = null;
			this.infos = null;
			this.count = 0;
			this.first_free = -1;
		}

		// Token: 0x04003FAE RID: 16302
		public bool cg_enabled = true;

		// Token: 0x04003FAF RID: 16303
		public CullingGroup culling_group;

		// Token: 0x04003FB0 RID: 16304
		public BoundingSphere[] spheres;

		// Token: 0x04003FB1 RID: 16305
		public VisibilityDetector.CullingGroupInfo.Info[] infos;

		// Token: 0x04003FB2 RID: 16306
		public int count;

		// Token: 0x04003FB3 RID: 16307
		public int first_free = -1;

		// Token: 0x02000A40 RID: 2624
		public struct Info
		{
			// Token: 0x04004713 RID: 18195
			public GameObject obj;

			// Token: 0x04004714 RID: 18196
			public VisibilityDetector.IVisibilityChanged changed;

			// Token: 0x04004715 RID: 18197
			public bool visible;

			// Token: 0x04004716 RID: 18198
			public int next;

			// Token: 0x04004717 RID: 18199
			public int distanceBoundIndex;
		}
	}
}
