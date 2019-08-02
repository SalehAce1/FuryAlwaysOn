using System;
using System.Diagnostics;
using System.Reflection;
using Modding;
using JetBrains.Annotations;
using ModCommon;
using MonoMod.RuntimeDetour;
using UnityEngine.SceneManagement;
using UnityEngine;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UObject = UnityEngine.Object;
using System.Collections.Generic;
using System.IO;

namespace FuryAlwaysOn
{
    [UsedImplicitly]
    public class FuryAlwaysOn : Mod, ITogglableMod
    {
        public static FuryAlwaysOn Instance;

        public override string GetVersion()
        {
            return "0.0.0.0";
        }

        public override void Initialize()
        {
            Instance = this;
            Log("Initalizing.");

            Unload(); //Ensures two instances of this mod are not working at the same time
            ModHooks.Instance.AfterSavegameLoadHook += AfterSaveGameLoad; //Runs when a save file is chosen
            ModHooks.Instance.NewGameHook += AddComponent; 
        }

        private void AfterSaveGameLoad(SaveGameData data) => AddComponent();

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<EffectFixer>(); //Begins class EffectFixer when a new save file loads
        }

        public void Unload()
        {
            ModHooks.Instance.AfterSavegameLoadHook -= AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook -= AddComponent;

            var x = GameManager.instance?.gameObject.GetComponent<EffectFixer>();
            if (x == null) return;
            UObject.Destroy(x);
        }
    }
}