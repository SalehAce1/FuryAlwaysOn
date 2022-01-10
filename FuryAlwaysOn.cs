using Modding;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UObject = UnityEngine.Object;

namespace FuryAlwaysOn
{
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
            Log("Initializing.");

            Unload(); //Ensures two instances of this mod are not working at the same time
            ModHooks.AfterSavegameLoadHook += AfterSaveGameLoad; //Runs when a save file is chosen
            ModHooks.NewGameHook += AddComponent;
        }

        private void AfterSaveGameLoad(SaveGameData data) => AddComponent();

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<EffectFixer>(); //Begins class EffectFixer when a new save file loads
        }

        public void Unload()
        {
            ModHooks.AfterSavegameLoadHook -= AfterSaveGameLoad;
            ModHooks.NewGameHook -= AddComponent;

            var x = GameManager.instance?.gameObject.GetComponent<EffectFixer>();
            if (x == null) return;
            UObject.Destroy(x);
        }
    }
}