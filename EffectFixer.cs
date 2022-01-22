using System.Collections;
using UnityEngine;
using Logger = Modding.Logger;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using Vasi;

namespace FuryAlwaysOn
{
	internal class EffectFixer : MonoBehaviour
	{
		private PlayMakerFSM fsm;

		private void Start() => StartCoroutine(WaitForPlayer());

		private IEnumerator WaitForPlayer()
		{
			yield return new WaitWhile(() => HeroController.instance == null); // Waits for Hero to be loaded
			yield return null;
			fsm = GameObject.Find("Charm Effects").LocateMyFSM("Fury");
			fsm.ChangeTransition("Check HP", "CANCEL", "Get Ref");         // Ensures the FSM does not cancel when HP is not at 1
			fsm.GetState("Activate").RemoveTransition("HERO DAMAGED");     // Ensures HP is not checked when hit
			fsm.GetState("Activate").RemoveTransition("HERO HEALED");      // Ensures HP is not checked when healed
			fsm.GetState("Activate").RemoveTransition("HERO HEALED FULL"); // Ensures HP is not checked when at bench
			fsm.GetState("Activate").RemoveTransition("ADD BLUE HEALTH");  // Ensures HP is not checked when Lifeblood is received
			fsm.GetState("Activate").RemoveAction(21);                     // Disables Fury vignette
			fsm.GetState("Activate").RemoveAction(20);                     // Disables Fury burst effect
			fsm.GetState("Activate").RemoveAction(2);                      // Disables Fury particle effects
			fsm.GetState("Activate").RemoveAction(1);                      // Required(?) to disable the rest of the effects
			fsm.GetState("Activate").RemoveAction(0);                      // Disables Fury audio effects

			// Gives Grubberfly beams their Fury color
			HeroController.instance.grubberFlyBeamPrefabL = HeroController.instance.grubberFlyBeamPrefabL_fury;
			HeroController.instance.grubberFlyBeamPrefabR = HeroController.instance.grubberFlyBeamPrefabR_fury;
			HeroController.instance.grubberFlyBeamPrefabU = HeroController.instance.grubberFlyBeamPrefabU_fury;
			HeroController.instance.grubberFlyBeamPrefabD = HeroController.instance.grubberFlyBeamPrefabD_fury;

			while (true)
			{
				if (PlayerData.instance.equippedCharm_6 && fsm.ActiveStateName == "Idle") // Turns on Fury when equipped
				{
					fsm.SendEvent("HERO DAMAGED"); // Starts
				}
				if (!PlayerData.instance.equippedCharm_6 && fsm.ActiveStateName == "Activate") // Turns off Fury when not equipped
				{
					fsm.SetState("Deactivate");
				}
				yield return null;
			}
		}

		public static void Log(object message) => Logger.Log("[EffectFixer] " + message);
	}
}