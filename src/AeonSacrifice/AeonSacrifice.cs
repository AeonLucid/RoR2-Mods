using System.Collections.Generic;
using BepInEx;
using R2API.Utils;
using RoR2;
using RoR2.Artifacts;
using UnityEngine;

namespace AeonSacrifice
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.aeonlucid.aeonsacrifice", "AeonSacrifice", "1.0.0")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class AeonSacrifice : BaseUnityPlugin
    {
        private readonly HashSet<int> _trackedPickupInfos;

        public AeonSacrifice()
        {
            _trackedPickupInfos = new HashSet<int>();
            
            On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += OnServerCharacterDeath;
            On.RoR2.PickupDisplay.SetPickupIndex += OnSetPickupIndex;
        }

        private void OnServerCharacterDeath(On.RoR2.Artifacts.SacrificeArtifactManager.orig_OnServerCharacterDeath orig, DamageReport damageReport)
        {
            Logger.LogInfo("Called OnServerCharacterDeath");
            
            var fieldDropTable = typeof(SacrificeArtifactManager).GetFieldValue<PickupDropTable>("dropTable");
            var fieldTreasureRng = typeof(SacrificeArtifactManager).GetFieldValue<Xoroshiro128Plus>("treasureRng");
            
            if (!(bool) (UnityEngine.Object) damageReport.victimMaster || damageReport.attackerTeamIndex == damageReport.victimTeamIndex && (bool) (UnityEngine.Object) damageReport.victimMaster.minionOwnership.ownerMaster)
            {
                return;
            }

            Logger.LogInfo("Called ..");
            // var dropChancePercent = Util.GetExpAdjustedDropChancePercent(5f, damageReport.victim.gameObject);
            // if (!Util.CheckRoll(dropChancePercent))
            // {
            //     return;
            // }
            
            var drop = fieldDropTable.GenerateDrop(fieldTreasureRng);
            if (!(drop != PickupIndex.none))
            {
                return;
            }
            
            Logger.LogInfo("Tracking " + drop.value);
            
            _trackedPickupInfos.Add(drop.value);

            PickupDropletController.CreatePickupDroplet(drop, damageReport.victimBody.corePosition, Vector3.up * 20f);
        }

        private void OnSetPickupIndex(On.RoR2.PickupDisplay.orig_SetPickupIndex orig, PickupDisplay self, PickupIndex newpickupindex, bool newhidden)
        {
            if (_trackedPickupInfos.Contains(newpickupindex.value))
            {
                Logger.LogInfo("Removing " + newpickupindex.value);
                orig(self, newpickupindex, true);
                // TODO: Clear on new stage or something
                // _trackedPickupInfos.Remove(newpickupindex.value);
                return;
            }
            
            orig(self, newpickupindex, newhidden);
        }
    }
}
