using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000460 RID: 1120
	public class EffectManager : MonoBehaviour, IAnimatorListener
	{
		// Token: 0x06003B11 RID: 15121 RVA: 0x001C5BD4 File Offset: 0x001C3DD4
		private void Awake()
		{
			foreach (Effect effect in this.Effects)
			{
				effect.Owner = base.transform;
				if (!effect.instantiate && effect.effect)
				{
					effect.Instance = effect.effect;
				}
			}
		}

		// Token: 0x06003B12 RID: 15122 RVA: 0x001C5C50 File Offset: 0x001C3E50
		public virtual void PlayEffect(int ID)
		{
			List<Effect> list = this.Effects.FindAll((Effect effect) => effect.ID == ID && effect.active);
			if (list != null)
			{
				foreach (Effect effect2 in list)
				{
					this.Play(effect2);
				}
			}
		}

		// Token: 0x06003B13 RID: 15123 RVA: 0x001C5CC8 File Offset: 0x001C3EC8
		private IEnumerator IPlayEffect(Effect e)
		{
			if (e.delay > 0f)
			{
				yield return new WaitForSeconds(e.delay);
			}
			yield return new WaitForEndOfFrame();
			if (e.instantiate)
			{
				e.Instance = Object.Instantiate<GameObject>(e.effect);
				e.effect.gameObject.SetActive(false);
			}
			else
			{
				e.Instance = e.effect;
			}
			if (e.Instance && e.root)
			{
				e.Instance.transform.position = e.root.position;
				e.Instance.gameObject.SetActive(true);
			}
			TrailRenderer componentInChildren = e.Instance.GetComponentInChildren<TrailRenderer>();
			if (componentInChildren)
			{
				componentInChildren.Clear();
			}
			e.Instance.transform.localScale = Vector3.Scale(e.Instance.transform.localScale, e.ScaleMultiplier);
			e.OnPlay.Invoke();
			if (e.root)
			{
				if (e.isChild)
				{
					e.Instance.transform.parent = e.root;
				}
				if (e.useRootRotation)
				{
					e.Instance.transform.rotation = e.root.rotation;
				}
			}
			e.Instance.transform.localPosition += e.PositionOffset;
			e.Instance.transform.localRotation *= Quaternion.Euler(e.RotationOffset);
			if (e.Modifier)
			{
				e.Modifier.StartEffect(e);
			}
			base.StartCoroutine(this.Life(e));
			yield return null;
			yield break;
		}

		// Token: 0x06003B14 RID: 15124 RVA: 0x001C5CDE File Offset: 0x001C3EDE
		private IEnumerator Life(Effect e)
		{
			if (e.life > 0f)
			{
				yield return new WaitForSeconds(e.life);
				if (e.Modifier)
				{
					e.Modifier.StopEffect(e);
				}
				e.OnStop.Invoke();
				if (e.instantiate)
				{
					Object.Destroy(e.Instance);
				}
			}
			yield return null;
			yield break;
		}

		// Token: 0x06003B15 RID: 15125 RVA: 0x001C5CF0 File Offset: 0x001C3EF0
		protected virtual void Play(Effect effect)
		{
			if (effect.effect == null)
			{
				return;
			}
			if (effect.Modifier)
			{
				effect.Modifier.AwakeEffect(effect);
			}
			if (!effect.toggleable)
			{
				base.StartCoroutine(this.IPlayEffect(effect));
				return;
			}
			effect.On = !effect.On;
			if (effect.On)
			{
				base.StartCoroutine(this.IPlayEffect(effect));
				return;
			}
			effect.OnStop.Invoke();
		}

		// Token: 0x06003B16 RID: 15126 RVA: 0x001AF9E6 File Offset: 0x001ADBE6
		public virtual void OnAnimatorBehaviourMessage(string message, object value)
		{
			this.InvokeWithParams(message, value);
		}

		// Token: 0x06003B17 RID: 15127 RVA: 0x001C5D70 File Offset: 0x001C3F70
		public virtual void _DisableEffect(string name)
		{
			List<Effect> list = this.Effects.FindAll((Effect effect) => effect.Name.ToUpperInvariant() == name.ToUpperInvariant());
			if (list != null)
			{
				using (List<Effect>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Effect effect2 = enumerator.Current;
						effect2.active = false;
					}
					return;
				}
			}
			Debug.LogWarning("No effect with the name: " + name + " was found");
		}

		// Token: 0x06003B18 RID: 15128 RVA: 0x001C5E00 File Offset: 0x001C4000
		public virtual void _DisableEffect(int ID)
		{
			List<Effect> list = this.Effects.FindAll((Effect effect) => effect.ID == ID);
			if (list != null)
			{
				using (List<Effect>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Effect effect2 = enumerator.Current;
						effect2.active = false;
					}
					return;
				}
			}
			Debug.LogWarning("No effect with the ID: " + ID + " was found");
		}

		// Token: 0x06003B19 RID: 15129 RVA: 0x001C5E94 File Offset: 0x001C4094
		public virtual void _EnableEffect(string name)
		{
			List<Effect> list = this.Effects.FindAll((Effect effect) => effect.Name.ToUpperInvariant() == name.ToUpperInvariant());
			if (list != null)
			{
				using (List<Effect>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Effect effect2 = enumerator.Current;
						effect2.active = true;
					}
					return;
				}
			}
			Debug.LogWarning("No effect with the name: " + name + " was found");
		}

		// Token: 0x06003B1A RID: 15130 RVA: 0x001C5F24 File Offset: 0x001C4124
		public virtual void _EnableEffect(int ID)
		{
			List<Effect> list = this.Effects.FindAll((Effect effect) => effect.ID == ID);
			if (list != null)
			{
				using (List<Effect>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Effect effect2 = enumerator.Current;
						effect2.active = true;
					}
					return;
				}
			}
			Debug.LogWarning("No effect with the ID: " + ID + " was found");
		}

		// Token: 0x06003B1B RID: 15131 RVA: 0x001C5FB8 File Offset: 0x001C41B8
		public virtual void _EnableEffectPrefab(int ID)
		{
			Effect effect = this.Effects.Find((Effect item) => item.ID == ID);
			if (effect != null)
			{
				effect.Instance.SetActive(true);
			}
		}

		// Token: 0x06003B1C RID: 15132 RVA: 0x001C5FFC File Offset: 0x001C41FC
		public virtual void _DisableEffectPrefab(int ID)
		{
			Effect effect = this.Effects.Find((Effect item) => item.ID == ID);
			if (effect != null)
			{
				effect.Instance.SetActive(false);
			}
		}

		// Token: 0x04002AF5 RID: 10997
		public List<Effect> Effects;
	}
}
