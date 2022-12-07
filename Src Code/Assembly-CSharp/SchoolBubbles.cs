using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class SchoolBubbles : MonoBehaviour
{
	// Token: 0x060000C8 RID: 200 RVA: 0x000076C5 File Offset: 0x000058C5
	public void Start()
	{
		if (this._bubbleParticles == null)
		{
			base.transform.GetComponent<ParticleSystem>();
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x000076E4 File Offset: 0x000058E4
	public void EmitBubbles(Vector3 pos, float amount)
	{
		if (amount * this._speedEmitMultiplier < 1f)
		{
			return;
		}
		this._bubbleParticles.transform.position = pos;
		this._bubbleParticles.Emit(Mathf.Clamp((int)(amount * this._speedEmitMultiplier), this._minBubbles, this._maxBubbles));
	}

	// Token: 0x0400013F RID: 319
	public ParticleSystem _bubbleParticles;

	// Token: 0x04000140 RID: 320
	public float _emitEverySecond = 0.01f;

	// Token: 0x04000141 RID: 321
	public float _speedEmitMultiplier = 0.25f;

	// Token: 0x04000142 RID: 322
	public int _minBubbles;

	// Token: 0x04000143 RID: 323
	public int _maxBubbles = 5;
}
