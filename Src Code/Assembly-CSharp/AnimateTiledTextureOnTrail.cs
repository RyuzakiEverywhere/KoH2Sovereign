using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class AnimateTiledTextureOnTrail : MonoBehaviour
{
	// Token: 0x0600005C RID: 92 RVA: 0x00004358 File Offset: 0x00002558
	public void RegisterCallback(AnimateTiledTextureOnTrail.VoidEvent cbFunction)
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList.Add(cbFunction);
			return;
		}
		Debug.LogWarning("AnimateTiledTextureOnTrail: You are attempting to register a callback but the events of this object are not enabled!");
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00004379 File Offset: 0x00002579
	public void UnRegisterCallback(AnimateTiledTextureOnTrail.VoidEvent cbFunction)
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList.Remove(cbFunction);
			return;
		}
		Debug.LogWarning("AnimateTiledTextureOnTrail: You are attempting to un-register a callback but the events of this object are not enabled!");
	}

	// Token: 0x0600005E RID: 94 RVA: 0x0000439C File Offset: 0x0000259C
	public void Play()
	{
		if (this._isPlaying)
		{
			base.StopCoroutine("updateTiling");
			this._isPlaying = false;
		}
		base.GetComponent<TrailRenderer>().enabled = true;
		this._index = this._columns;
		base.StartCoroutine(this.updateTiling());
	}

	// Token: 0x0600005F RID: 95 RVA: 0x000043E8 File Offset: 0x000025E8
	public void ChangeMaterial(Material newMaterial, bool newInstance = false)
	{
		if (newInstance)
		{
			if (this._hasMaterialInstance)
			{
				Object.Destroy(base.GetComponent<TrailRenderer>().sharedMaterial);
			}
			this._materialInstance = new Material(newMaterial);
			base.GetComponent<TrailRenderer>().sharedMaterial = this._materialInstance;
			this._hasMaterialInstance = true;
		}
		else
		{
			base.GetComponent<TrailRenderer>().sharedMaterial = newMaterial;
		}
		this.CalcTextureSize();
		base.GetComponent<TrailRenderer>().sharedMaterial.SetTextureScale("_MainTex", this._textureSize);
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00004463 File Offset: 0x00002663
	private void Awake()
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList = new List<AnimateTiledTextureOnTrail.VoidEvent>();
		}
		this.ChangeMaterial(base.GetComponent<TrailRenderer>().sharedMaterial, this._newMaterialInstance);
	}

	// Token: 0x06000061 RID: 97 RVA: 0x0000448F File Offset: 0x0000268F
	private void OnDestroy()
	{
		if (this._hasMaterialInstance)
		{
			Object.Destroy(base.GetComponent<TrailRenderer>().sharedMaterial);
			this._hasMaterialInstance = false;
		}
	}

	// Token: 0x06000062 RID: 98 RVA: 0x000044B0 File Offset: 0x000026B0
	private void HandleCallbacks(List<AnimateTiledTextureOnTrail.VoidEvent> cbList)
	{
		for (int i = 0; i < cbList.Count; i++)
		{
			cbList[i]();
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x000044DA File Offset: 0x000026DA
	private void OnEnable()
	{
		this.CalcTextureSize();
		if (this._playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x06000064 RID: 100 RVA: 0x000044F0 File Offset: 0x000026F0
	private void CalcTextureSize()
	{
		this._textureSize = new Vector2(1f / (float)this._columns, 1f / (float)this._rows);
		this._textureSize.x = this._textureSize.x / this._scale.x;
		this._textureSize.y = this._textureSize.y / this._scale.y;
		this._textureSize -= this._buffer;
	}

	// Token: 0x06000065 RID: 101 RVA: 0x0000457D File Offset: 0x0000277D
	private IEnumerator updateTiling()
	{
		this._isPlaying = true;
		int checkAgainst = this._rows * this._columns;
		for (;;)
		{
			if (this._index >= checkAgainst)
			{
				this._index = 0;
				if (this._playOnce)
				{
					if (checkAgainst == this._columns)
					{
						break;
					}
					checkAgainst = this._columns;
				}
			}
			this.ApplyOffset();
			this._index++;
			yield return new WaitForSeconds(1f / this._framesPerSecond);
		}
		if (this._enableEvents)
		{
			this.HandleCallbacks(this._voidEventCallbackList);
		}
		if (this._disableUponCompletion)
		{
			base.gameObject.GetComponent<TrailRenderer>().enabled = false;
		}
		this._isPlaying = false;
		yield break;
		yield break;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x0000458C File Offset: 0x0000278C
	private void ApplyOffset()
	{
		Vector2 vector = new Vector2((float)this._index / (float)this._columns - (float)(this._index / this._columns), 1f - (float)(this._index / this._columns) / (float)this._rows);
		if (vector.y == 1f)
		{
			vector.y = 0f;
		}
		vector.x += (1f / (float)this._columns - this._textureSize.x) / 2f;
		vector.y += (1f / (float)this._rows - this._textureSize.y) / 2f;
		vector.x += this._offset.x;
		vector.y += this._offset.y;
		base.GetComponent<TrailRenderer>().sharedMaterial.SetTextureOffset("_MainTex", vector);
	}

	// Token: 0x04000082 RID: 130
	public int _columns = 2;

	// Token: 0x04000083 RID: 131
	public int _rows = 2;

	// Token: 0x04000084 RID: 132
	public Vector2 _scale = new Vector3(1f, 1f);

	// Token: 0x04000085 RID: 133
	public Vector2 _offset = Vector2.zero;

	// Token: 0x04000086 RID: 134
	public Vector2 _buffer = Vector2.zero;

	// Token: 0x04000087 RID: 135
	public float _framesPerSecond = 10f;

	// Token: 0x04000088 RID: 136
	public bool _playOnce;

	// Token: 0x04000089 RID: 137
	public bool _disableUponCompletion;

	// Token: 0x0400008A RID: 138
	public bool _enableEvents;

	// Token: 0x0400008B RID: 139
	public bool _playOnEnable = true;

	// Token: 0x0400008C RID: 140
	public bool _newMaterialInstance;

	// Token: 0x0400008D RID: 141
	private int _index;

	// Token: 0x0400008E RID: 142
	private Vector2 _textureSize = Vector2.zero;

	// Token: 0x0400008F RID: 143
	private Material _materialInstance;

	// Token: 0x04000090 RID: 144
	private bool _hasMaterialInstance;

	// Token: 0x04000091 RID: 145
	private bool _isPlaying;

	// Token: 0x04000092 RID: 146
	private List<AnimateTiledTextureOnTrail.VoidEvent> _voidEventCallbackList;

	// Token: 0x020004ED RID: 1261
	// (Invoke) Token: 0x06004237 RID: 16951
	public delegate void VoidEvent();
}
