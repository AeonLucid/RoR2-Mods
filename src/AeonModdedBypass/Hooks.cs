using System;
using System.Reflection;
using Facepunch.Steamworks;
using RoR2;

namespace AeonModdedBypass
{
    internal static class Hooks
    {
        public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                             BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private static string _steamBuildId;
        private static string _realBuildId;
        
        private static string GetSteamBuildId()
        {
            if (_steamBuildId == null)
            {
                var field = typeof(RoR2Application).GetField("steamBuildId", AllFlags);
                if (field == null)
                {
                    throw new ArgumentNullException(nameof(field));
                }

                _steamBuildId = (string) field.GetValue(null);
            }

            return _steamBuildId;
        }

        private static string GetRealBuildId()
        {
            if (_realBuildId == null)
            {
                var method = typeof(RoR2Application).GetMethod("GetBuildId", AllFlags);
                if (method == null)
                {
                    throw new ArgumentNullException(nameof(method));
                }

                _realBuildId = (string) method.Invoke(null, null);
            }

            return _realBuildId;
        }
        
        public static bool OnLobbySetData(Func<Lobby.LobbyData, string, string, bool> orig, Lobby.LobbyData self, string key, string value)
        {
            if (key == "build_id")
            {
#if DEBUG
                AeonMod.LoggerStatic.LogWarning($"Overwrote SetData('build_id', " + GetSteamBuildId() + ")");
#endif
                value = GetSteamBuildId();
            }
            
            return orig(self, key, value);
        }
        
        public static string OnLobbyGetData(Func<Lobby.LobbyData, string, string> orig, Lobby.LobbyData self, string key)
        {
            if (key == "build_id")
            {
#if DEBUG
                AeonMod.LoggerStatic.LogWarning($"Overwrote GetData('build_id') to return " + GetRealBuildId());
#endif
                
                return GetRealBuildId();
            }
            
            return orig(self, key);
        }
    }
}