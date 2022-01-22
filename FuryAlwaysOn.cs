using Modding;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UObject = UnityEngine.Object;

namespace FuryAlwaysOn
{
	public class FuryAlwaysOn : Mod, ITogglableMod
	{
		internal static FuryAlwaysOn instance;

		public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

		public override void Initialize()
		{
			instance = this;
			Log("Initializing.");

			Unload();                                            // Ensures two instances of this mod are not working at the same time
			ModHooks.AfterSavegameLoadHook += AfterSaveGameLoad; // Runs when a save file is chosen
			ModHooks.NewGameHook           += AddComponent;
			IL.KnightHatchling.OnEnable    += ChangeGlowingWombFuryCondition;
		}

		private static void ChangeGlowingWombFuryCondition(ILContext il)
		{
			ILCursor cursor = new ILCursor(il).Goto(0);
			if (cursor.TryGotoNext(MoveType.After, i => i.MatchCallvirt<PlayerData>("GetInt")))
			{
				Logger.Log("LdcI4 was found");
				cursor.Emit(OpCodes.Pop);
				cursor.Emit(OpCodes.Ldc_I4_1);
			}
		}

		private void AfterSaveGameLoad(SaveGameData data) => AddComponent();

		private void AddComponent() => GameManager.instance.gameObject.AddComponent<EffectFixer>(); // Begins class EffectFixer when a new save file loads

		public void Unload()
		{
			ModHooks.AfterSavegameLoadHook -= AfterSaveGameLoad;
			ModHooks.NewGameHook           -= AddComponent;
			IL.KnightHatchling.OnEnable    -= ChangeGlowingWombFuryCondition;

			var x = GameManager.instance.gameObject.GetComponent<EffectFixer>();
			if (x == null)
				return;
			UObject.Destroy(x);
		}
	}
}