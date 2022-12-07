using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000138 RID: 312
[ExecuteInEditMode]
public class MassPlaceObjects : MonoBehaviour
{
	// Token: 0x0600109F RID: 4255 RVA: 0x000B104C File Offset: 0x000AF24C
	private void Spawn()
	{
		this.Despawn();
		if (this.prefab == null)
		{
			return;
		}
		int num = (int)Mathf.Sqrt((float)this.count);
		for (int i = 0; i < this.count; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.prefab, base.transform);
			gameObject.transform.localPosition = new Vector3((float)(i / num) * this.spacing.x, 0f, (float)(i % num) * this.spacing.y);
			Animation componentInChildren = gameObject.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				this.animations.Add(componentInChildren);
				componentInChildren.enabled = this.enableAnimations;
			}
			Animator componentInChildren2 = gameObject.GetComponentInChildren<Animator>();
			if (componentInChildren2 != null)
			{
				this.animators.Add(componentInChildren2);
				componentInChildren2.SetTrigger("walk");
				componentInChildren2.enabled = this.enableAnimations;
			}
		}
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x000B1134 File Offset: 0x000AF334
	private void Despawn()
	{
		this.animations.Clear();
		this.animators.Clear();
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			list.Add(base.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < list.Count; j++)
		{
			Object.DestroyImmediate(list[j]);
		}
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x000B11A8 File Offset: 0x000AF3A8
	private void UpdateAnimations()
	{
		if (this.enableAnimations)
		{
			return;
		}
		if (this.animFrameSkip < 0)
		{
			return;
		}
		int num = this.animFrameSkip + 1;
		int num2 = Time.frameCount % num;
		for (int i = 0; i < this.animations.Count; i++)
		{
			this.animations[i].enabled = (i % num == num2);
		}
		for (int j = 0; j < this.animators.Count; j++)
		{
			this.animators[j].enabled = (j % num == num2);
		}
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x000B1233 File Offset: 0x000AF433
	private void OnValidate()
	{
		this.dirty = true;
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x000B123C File Offset: 0x000AF43C
	private void Update()
	{
		if (this.dirty)
		{
			this.dirty = false;
			this.Spawn();
		}
		if (Application.isPlaying)
		{
			this.UpdateAnimations();
		}
	}

	// Token: 0x04000B06 RID: 2822
	public GameObject prefab;

	// Token: 0x04000B07 RID: 2823
	public int count;

	// Token: 0x04000B08 RID: 2824
	public Vector2 spacing = Vector2.one;

	// Token: 0x04000B09 RID: 2825
	public bool enableAnimations = true;

	// Token: 0x04000B0A RID: 2826
	public int animFrameSkip;

	// Token: 0x04000B0B RID: 2827
	private bool dirty;

	// Token: 0x04000B0C RID: 2828
	private List<Animation> animations = new List<Animation>();

	// Token: 0x04000B0D RID: 2829
	private List<Animator> animators = new List<Animator>();
}
