using BepInEx;
using BepInEx.Logging;
using Facepunch.Steamworks;
using MonoMod.RuntimeDetour;

namespace AeonModdedBypass
{
    [BepInPlugin("com.aeonlucid.aeonmoddedbypass", "AeonModdedBypass", "1.0.0")]
    public class AeonMod : BaseUnityPlugin
    {
        public static ManualLogSource LoggerStatic;
        
        public AeonMod()
        {
            LoggerStatic = Logger;

            HookLobbySetData();
            HookLobbyGetData();
        }

        private void HookLobbySetData()
        {
            new Hook(
                typeof(Lobby.LobbyData).GetMethod("SetData", Hooks.AllFlags), 
                typeof(Hooks).GetMethod(nameof(Hooks.OnLobbySetData), Hooks.AllFlags)).Apply();
        }

        private void HookLobbyGetData()
        {
            new Hook(
                typeof(Lobby.LobbyData).GetMethod("GetData", Hooks.AllFlags), 
                typeof(Hooks).GetMethod(nameof(Hooks.OnLobbyGetData), Hooks.AllFlags)).Apply();
        }
    }
}