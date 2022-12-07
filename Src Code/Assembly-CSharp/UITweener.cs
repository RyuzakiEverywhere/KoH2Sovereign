using System;
using AnimationOrTween;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002F6 RID: 758
public abstract class UITweener : MonoBehaviour
{
	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06002F98 RID: 12184 RVA: 0x00184B4C File Offset: 0x00182D4C
	public float amountPerDelta
	{
		get
		{
			if (this.mDuration != this.duration)
			{
				this.mDuration = this.duration;
				this.mAmountPerDelta = Mathf.Abs((this.duration > 0f) ? (1f / this.duration) : 1000f);
			}
			return this.mAmountPerDelta;
		}
	}

	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06002F99 RID: 12185 RVA: 0x00184BA4 File Offset: 0x00182DA4
	// (set) Token: 0x06002F9A RID: 12186 RVA: 0x00184BAC File Offset: 0x00182DAC
	public float tweenFactor
	{
		get
		{
			return this.mFactor;
		}
		set
		{
			this.mFactor = Mathf.Clamp01(value);
		}
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x06002F9B RID: 12187 RVA: 0x00184BBA File Offset: 0x00182DBA
	public Direction direction
	{
		get
		{
			if (this.mAmountPerDelta >= 0f)
			{
				return Direction.Forward;
			}
			return Direction.Reverse;
		}
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x00184BCC File Offset: 0x00182DCC
	private void Reset()
	{
		if (!this.mStarted)
		{
			this.SetStartToCurrentValue();
			this.SetEndToCurrentValue();
		}
	}

	// Token: 0x06002F9D RID: 12189 RVA: 0x00184BE2 File Offset: 0x00182DE2
	protected virtual void Start()
	{
		this.Update();
	}

	// Token: 0x06002F9E RID: 12190 RVA: 0x00184BEC File Offset: 0x00182DEC
	private void Update()
	{
		float num = this.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
		float num2 = this.ignoreTimeScale ? Time.unscaledTime : Time.time;
		if (!this.mStarted)
		{
			this.mStarted = true;
			this.mStartTime = num2 + this.delay;
			if (this.onStart != null)
			{
				this.onStart.Invoke();
			}
		}
		if (num2 < this.mStartTime)
		{
			return;
		}
		this.mFactor += this.amountPerDelta * num;
		if (this.style == UITweener.Style.Loop)
		{
			if (this.mFactor > 1f)
			{
				this.mFactor -= Mathf.Floor(this.mFactor);
			}
		}
		else if (this.style == UITweener.Style.PingPong)
		{
			if (this.mFactor > 1f)
			{
				this.mFactor = 1f - (this.mFactor - Mathf.Floor(this.mFactor));
				this.mAmountPerDelta = -this.mAmountPerDelta;
			}
			else if (this.mFactor < 0f)
			{
				this.mFactor = -this.mFactor;
				this.mFactor -= Mathf.Floor(this.mFactor);
				this.mAmountPerDelta = -this.mAmountPerDelta;
			}
		}
		if (this.style == UITweener.Style.Once && (this.duration == 0f || this.mFactor > 1f || this.mFactor < 0f))
		{
			this.mFactor = Mathf.Clamp01(this.mFactor);
			this.Sample(this.mFactor, true);
			if (this.duration == 0f || (this.mFactor == 1f && this.mAmountPerDelta > 0f) || (this.mFactor == 0f && this.mAmountPerDelta < 0f))
			{
				base.enabled = false;
			}
			if (UITweener.current == null)
			{
				UITweener.current = this;
				if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
				{
					this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
				}
				UITweener.current = null;
			}
			if (this.onFinished != null)
			{
				this.onFinished.Invoke();
				return;
			}
		}
		else
		{
			this.Sample(this.mFactor, false);
		}
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x00184E2D File Offset: 0x0018302D
	private void OnDisable()
	{
		this.mStarted = false;
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x00184E38 File Offset: 0x00183038
	public void Sample(float factor, bool isFinished)
	{
		float num = Mathf.Clamp01(factor);
		if (this.method == UITweener.Method.EaseIn)
		{
			num = 1f - Mathf.Sin(1.5707964f * (1f - num));
			if (this.steeperCurves)
			{
				num *= num;
			}
		}
		else if (this.method == UITweener.Method.EaseOut)
		{
			num = Mathf.Sin(1.5707964f * num);
			if (this.steeperCurves)
			{
				num = 1f - num;
				num = 1f - num * num;
			}
		}
		else if (this.method == UITweener.Method.EaseInOut)
		{
			num -= Mathf.Sin(num * 6.2831855f) / 6.2831855f;
			if (this.steeperCurves)
			{
				num = num * 2f - 1f;
				float num2 = Mathf.Sign(num);
				num = 1f - Mathf.Abs(num);
				num = 1f - num * num;
				num = num2 * num * 0.5f + 0.5f;
			}
		}
		else if (this.method == UITweener.Method.BounceIn)
		{
			num = this.BounceLogic(num);
		}
		else if (this.method == UITweener.Method.BounceOut)
		{
			num = 1f - this.BounceLogic(1f - num);
		}
		this.OnUpdate((this.animationCurve != null) ? this.animationCurve.Evaluate(num) : num, isFinished);
	}

	// Token: 0x06002FA1 RID: 12193 RVA: 0x00184F6C File Offset: 0x0018316C
	private float BounceLogic(float val)
	{
		if (val < 0.363636f)
		{
			val = 7.5685f * val * val;
		}
		else if (val < 0.727272f)
		{
			val = 7.5625f * (val -= 0.545454f) * val + 0.75f;
		}
		else if (val < 0.90909f)
		{
			val = 7.5625f * (val -= 0.818181f) * val + 0.9375f;
		}
		else
		{
			val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f;
		}
		return val;
	}

	// Token: 0x06002FA2 RID: 12194 RVA: 0x00184FF1 File Offset: 0x001831F1
	[Obsolete("Use PlayForward() instead")]
	public void Play()
	{
		this.Play(true);
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x00184FF1 File Offset: 0x001831F1
	public void PlayForward()
	{
		this.Play(true);
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x00184FFA File Offset: 0x001831FA
	public void PlayReverse()
	{
		this.Play(false);
	}

	// Token: 0x06002FA5 RID: 12197 RVA: 0x00185003 File Offset: 0x00183203
	public void Play(bool forward)
	{
		this.mAmountPerDelta = Mathf.Abs(this.amountPerDelta);
		if (!forward)
		{
			this.mAmountPerDelta = -this.mAmountPerDelta;
		}
		base.enabled = true;
		this.Update();
	}

	// Token: 0x06002FA6 RID: 12198 RVA: 0x00185033 File Offset: 0x00183233
	public void ResetToBeginning()
	{
		this.mStarted = false;
		this.mFactor = ((this.mAmountPerDelta < 0f) ? 1f : 0f);
		this.Sample(this.mFactor, false);
	}

	// Token: 0x06002FA7 RID: 12199 RVA: 0x00185068 File Offset: 0x00183268
	public void ResetToEnd()
	{
		this.mStarted = false;
		this.Sample(1f, false);
	}

	// Token: 0x06002FA8 RID: 12200 RVA: 0x0018507D File Offset: 0x0018327D
	public void Toggle()
	{
		if (this.mFactor > 0f)
		{
			this.mAmountPerDelta = -this.amountPerDelta;
		}
		else
		{
			this.mAmountPerDelta = Mathf.Abs(this.amountPerDelta);
		}
		base.enabled = true;
	}

	// Token: 0x06002FA9 RID: 12201
	protected abstract void OnUpdate(float factor, bool isFinished);

	// Token: 0x06002FAA RID: 12202 RVA: 0x001850B4 File Offset: 0x001832B4
	public static T Begin<T>(GameObject go, float duration) where T : UITweener
	{
		T t = go.GetComponent<T>();
		if (t != null && t.tweenGroup != 0)
		{
			t = default(T);
			T[] components = go.GetComponents<T>();
			int i = 0;
			int num = components.Length;
			while (i < num)
			{
				t = components[i];
				if (t != null && t.tweenGroup == 0)
				{
					break;
				}
				t = default(T);
				i++;
			}
		}
		if (t == null)
		{
			t = go.AddComponent<T>();
		}
		t.mStarted = false;
		t.duration = duration;
		t.mFactor = 0f;
		t.mAmountPerDelta = Mathf.Abs(t.mAmountPerDelta);
		t.style = UITweener.Style.Once;
		t.animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f, 0f, 1f),
			new Keyframe(1f, 1f, 1f, 0f)
		});
		t.eventReceiver = null;
		t.callWhenFinished = null;
		t.enabled = true;
		if (duration <= 0f)
		{
			t.Sample(1f, true);
			t.enabled = false;
		}
		return t;
	}

	// Token: 0x06002FAB RID: 12203 RVA: 0x00185231 File Offset: 0x00183431
	public void Stop()
	{
		this.Sample(0f, true);
		base.enabled = false;
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void SetStartToCurrentValue()
	{
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void SetEndToCurrentValue()
	{
	}

	// Token: 0x04001FFD RID: 8189
	public static UITweener current;

	// Token: 0x04001FFE RID: 8190
	public UITweener.Method method;

	// Token: 0x04001FFF RID: 8191
	public UITweener.Style style;

	// Token: 0x04002000 RID: 8192
	public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 1f),
		new Keyframe(1f, 1f, 1f, 0f)
	});

	// Token: 0x04002001 RID: 8193
	public bool ignoreTimeScale = true;

	// Token: 0x04002002 RID: 8194
	public float delay;

	// Token: 0x04002003 RID: 8195
	public float duration = 1f;

	// Token: 0x04002004 RID: 8196
	public bool steeperCurves;

	// Token: 0x04002005 RID: 8197
	public int tweenGroup;

	// Token: 0x04002006 RID: 8198
	public UnityEvent onFinished = new UnityEvent();

	// Token: 0x04002007 RID: 8199
	public UnityEvent onStart = new UnityEvent();

	// Token: 0x04002008 RID: 8200
	[HideInInspector]
	public GameObject eventReceiver;

	// Token: 0x04002009 RID: 8201
	[HideInInspector]
	public string callWhenFinished;

	// Token: 0x0400200A RID: 8202
	private bool mStarted;

	// Token: 0x0400200B RID: 8203
	private float mStartTime;

	// Token: 0x0400200C RID: 8204
	private float mDuration;

	// Token: 0x0400200D RID: 8205
	private float mAmountPerDelta = 1000f;

	// Token: 0x0400200E RID: 8206
	private float mFactor;

	// Token: 0x02000864 RID: 2148
	public enum Method
	{
		// Token: 0x04003F09 RID: 16137
		Linear,
		// Token: 0x04003F0A RID: 16138
		EaseIn,
		// Token: 0x04003F0B RID: 16139
		EaseOut,
		// Token: 0x04003F0C RID: 16140
		EaseInOut,
		// Token: 0x04003F0D RID: 16141
		BounceIn,
		// Token: 0x04003F0E RID: 16142
		BounceOut
	}

	// Token: 0x02000865 RID: 2149
	public enum Style
	{
		// Token: 0x04003F10 RID: 16144
		Once,
		// Token: 0x04003F11 RID: 16145
		Loop,
		// Token: 0x04003F12 RID: 16146
		PingPong
	}

	// Token: 0x02000866 RID: 2150
	public enum Mode
	{
		// Token: 0x04003F14 RID: 16148
		Relative,
		// Token: 0x04003F15 RID: 16149
		Absolute
	}
}
