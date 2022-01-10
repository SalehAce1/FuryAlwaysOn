using System.Collections;
using UnityEngine;
using Logger = Modding.Logger;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

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
            yield return new WaitWhile(() => HeroController.instance == null); //Waits for Hero to be loaded
            yield return null;
            _fsm = GameObject.Find("Charm Effects").LocateMyFSM("Fury");
            _fsm.ChangeTransition("Check HP", "CANCEL", "Get Ref"); //Ensures the fsm does not cancel when HP is not at 1
            _fsm.RemoveTransition("Activate", "HERO DAMAGED"); //Ensures hp is not checked when hit
            _fsm.RemoveTransition("Activate", "HERO HEALED"); //Ensures hp is not checked when healed
            _fsm.RemoveTransition("Activate", "HERO HEALED FULL"); //Ensures hp is not checked when at bench
            _fsm.RemoveTransition("Activate", "ADD BLUE HEALTH"); //Ensures hp is not checked when lifeblood is received
            _fsm.RemoveAction("Activate",21);
            _fsm.RemoveAction("Activate",20);
            _fsm.RemoveAction("Activate", 2);
            _fsm.RemoveAction("Activate", 1);
            _fsm.RemoveAction("Activate", 0);
            HeroController.instance.grubberFlyBeamPrefabL = HeroController.instance.grubberFlyBeamPrefabL_fury;
            HeroController.instance.grubberFlyBeamPrefabR = HeroController.instance.grubberFlyBeamPrefabR_fury;
            HeroController.instance.grubberFlyBeamPrefabU = HeroController.instance.grubberFlyBeamPrefabU_fury;
            HeroController.instance.grubberFlyBeamPrefabD = HeroController.instance.grubberFlyBeamPrefabD_fury;

            while (true)
            {
                if (PlayerData.instance.equippedCharm_6 && _fsm.ActiveStateName == "Idle") //Turns on fury when equipped
                {
                    _fsm.SendEvent("HERO DAMAGED"); //Starts
                }
                if (!PlayerData.instance.equippedCharm_6 && _fsm.ActiveStateName == "Activate") //Turns of fury when not equipped
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
