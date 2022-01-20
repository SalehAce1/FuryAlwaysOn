using System.Collections;
using UnityEngine;
using Logger = Modding.Logger;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using Vasi;

namespace FuryAlwaysOn
{
	internal class EffectFixer : MonoBehaviour
	{
		PlayMakerFSM _fsm;

		private void Start()
		{
			StartCoroutine(WaitForPlayer());
		}

		IEnumerator WaitForPlayer()
		{
			yield return new WaitWhile(() => HeroController.instance == null); // Waits for Hero to be loaded
			yield return null;
			_fsm = GameObject.Find("Charm Effects").LocateMyFSM("Fury");
			_fsm.ChangeTransition("Check HP", "CANCEL", "Get Ref");         // Ensures the FSM does not cancel when HP is not at 1
			_fsm.GetState("Activate").RemoveTransition("HERO DAMAGED");     // Ensures HP is not checked when hit
			_fsm.GetState("Activate").RemoveTransition("HERO HEALED");      // Ensures HP is not checked when healed
			_fsm.GetState("Activate").RemoveTransition("HERO HEALED FULL"); // Ensures HP is not checked when at bench
			_fsm.GetState("Activate").RemoveTransition("ADD BLUE HEALTH");  // Ensures HP is not checked when Lifeblood is received
			_fsm.GetState("Activate").RemoveAction(21);                     // Disables Fury vignette
			_fsm.GetState("Activate").RemoveAction(20);                     // Disables Fury burst effect
			_fsm.GetState("Activate").RemoveAction(2);                      // Disables Fury particle effects
			_fsm.GetState("Activate").RemoveAction(1);                      // Required(?) to disable the rest of the effects
			_fsm.GetState("Activate").RemoveAction(0);                      // Disables Fury audio effects

			// Gives Grubberfly beams their Fury color
			HeroController.instance.grubberFlyBeamPrefabL = HeroController.instance.grubberFlyBeamPrefabL_fury;
			HeroController.instance.grubberFlyBeamPrefabR = HeroController.instance.grubberFlyBeamPrefabR_fury;
			HeroController.instance.grubberFlyBeamPrefabU = HeroController.instance.grubberFlyBeamPrefabU_fury;
			HeroController.instance.grubberFlyBeamPrefabD = HeroController.instance.grubberFlyBeamPrefabD_fury;

			while (true)
			{
				if (PlayerData.instance.equippedCharm_6 && _fsm.ActiveStateName == "Idle") // Turns on Fury when equipped
				{
					_fsm.SendEvent("HERO DAMAGED"); // Starts
				}
				if (!PlayerData.instance.equippedCharm_6 && _fsm.ActiveStateName == "Activate") // Turns off Fury when not equipped
				{
					_fsm.SetState("Deactivate");
				}
				yield return null;
			}
		}

		public static void Log(object o)
		{
			Logger.Log("[EffectFixer] " + o);
		}
	}
}