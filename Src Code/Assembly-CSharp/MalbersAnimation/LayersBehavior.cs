using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D8 RID: 984
	public class LayersBehavior : StateMachineBehaviour
	{
		// Token: 0x06003768 RID: 14184 RVA: 0x001B6FAC File Offset: 0x001B51AC
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			foreach (LayersActivation layersActivation in this.layers)
			{
				int layerIndex2 = animator.GetLayerIndex(layersActivation.layer);
				this.transition = animator.GetAnimatorTransitionInfo(layerIndex);
				if (animator.IsInTransition(layerIndex))
				{
					if (layersActivation.activate)
					{
						if (layersActivation.transA == StateTransition.First && stateInfo.normalizedTime <= 0.5f)
						{
							animator.SetLayerWeight(layerIndex2, this.transition.normalizedTime);
						}
						if (layersActivation.transA == StateTransition.Last && stateInfo.normalizedTime >= 0.5f)
						{
							animator.SetLayerWeight(layerIndex2, this.transition.normalizedTime);
						}
					}
					if (layersActivation.deactivate)
					{
						if (layersActivation.transD == StateTransition.First && stateInfo.normalizedTime <= 0.5f)
						{
							animator.SetLayerWeight(layerIndex2, 1f - this.transition.normalizedTime);
						}
						if (layersActivation.transD == StateTransition.Last && stateInfo.normalizedTime >= 0.5f)
						{
							animator.SetLayerWeight(layerIndex2, 1f - this.transition.normalizedTime);
						}
					}
				}
				else
				{
					if (layersActivation.activate && layersActivation.transA == StateTransition.First)
					{
						animator.SetLayerWeight(layerIndex2, 1f);
					}
					if (layersActivation.deactivate && layersActivation.transD == StateTransition.First)
					{
						animator.SetLayerWeight(layerIndex2, 0f);
					}
				}
			}
		}

		// Token: 0x06003769 RID: 14185 RVA: 0x001B70FC File Offset: 0x001B52FC
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			foreach (LayersActivation layersActivation in this.layers)
			{
				int layerIndex2 = animator.GetLayerIndex(layersActivation.layer);
				if (layersActivation.activate && layersActivation.transA == StateTransition.Last)
				{
					animator.SetLayerWeight(layerIndex2, 1f);
				}
				if (layersActivation.deactivate && layersActivation.transD == StateTransition.Last)
				{
					animator.SetLayerWeight(layerIndex2, 0f);
				}
			}
		}

		// Token: 0x04002767 RID: 10087
		public LayersActivation[] layers;

		// Token: 0x04002768 RID: 10088
		private AnimatorTransitionInfo transition;
	}
}
