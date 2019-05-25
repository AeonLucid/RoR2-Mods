using System.Collections.Concurrent;
using BepInEx.Logging;
using R2API.Utils;
using RoR2;
using RoR2Application = On.RoR2.RoR2Application;

namespace AeonHud
{
    public class PlayerManager
    {
        public PlayerManager(ManualLogSource logger)
        {
            Logger = logger;
            Players = new ConcurrentDictionary<NetworkUserId, PlayerData>();
            
            Run.OnServerGameOver += OnServerGameOver;
            On.RoR2.CharacterBody.RecalculateStats += OnRecalculateStats;
        }

        private ManualLogSource Logger { get; }
        public ConcurrentDictionary<NetworkUserId, PlayerData> Players { get; }

        private void OnServerGameOver(Run arg1, GameResultType arg2)
        {
            Logger.LogError($"====================================== CLEAR");
            
            Players.Clear();
        }
        
        private void OnRecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody body)
        {
            orig(body);
            
            if (body.master == null) return;
            
            var component = body.master.GetComponent<PlayerCharacterMasterController>();
            if (component == null) return;
         
            var player = Players.GetOrAdd(component.networkUser.id, id => new PlayerData(id, component.networkUser.userName));

            // body.master.luck
            
            Logger.LogInfo($"{Players.Count} - {player.UserName}");
            
            // body.SetPropertyValue("crit", (float) 100);
            
            // body.master.GiveMoney(1000);
            // body.master.GiveExperience(1000);
        }
    }
}