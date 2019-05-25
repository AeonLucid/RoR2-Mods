using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace AeonMeteorBuff
{
    [BepInPlugin("com.aeonlucid.aeonmeteorbuff", "AeonMeteorBuff", "1.0.1")]
    public class AeonMod : BaseUnityPlugin
    {
        private readonly ConfigWrapper<bool> _disableEffects;
        private FieldInfo _fieldImpactPosition;
        
        public AeonMod()
        {
            _disableEffects = Config.Wrap("Settings", "DisableEffects", null, false);
            
            On.RoR2.MeteorStormController.DetonateMeteor += MeteorStormControllerOnDetonateMeteor;

            if (_disableEffects.Value)
            {
                IL.RoR2.MeteorStormController.DoMeteorEffect += RemoveSpawnEffects;
                IL.RoR2.MeteorStormController.FixedUpdate += RemoveSpawnEffects;
            }
        }

#if DEBUG
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var item = new PickupIndex(EquipmentIndex.Meteor);
                var transf = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                PickupDropletController.CreatePickupDroplet(item, transf.position, transf.forward * 20f);
            }
        }
#endif

        private void RemoveSpawnEffects(ILContext il)
        {
            var effectManagerGetInstance = typeof(EffectManager).GetMethod("get_instance", Constants.AllFlags);
            var effectManagerSpawnEffect = typeof(EffectManager).GetMethod("SpawnEffect", Constants.AllFlags);

            var cursor = new ILCursor(il);
            
            cursor.Goto(0);

            while (cursor.TryGotoNext(x => x.MatchCall(effectManagerGetInstance)))
            {
                // Find 
                var start = cursor.Index;
                cursor.GotoNext(MoveType.After, x => x.MatchCallvirt(effectManagerSpawnEffect));
                var end = cursor.Index;
                cursor.Goto(start);
                cursor.RemoveRange(end - start); 
                
                // Reset.
                cursor.Goto(0);
            }
        }

        private void MeteorStormControllerOnDetonateMeteor(On.RoR2.MeteorStormController.orig_DetonateMeteor orig, MeteorStormController self, object meteor)
        {
            if (_fieldImpactPosition == null)
            {
                _fieldImpactPosition = meteor.GetType().GetField("impactPosition", Constants.AllFlags);
                if (_fieldImpactPosition == null)
                {
                    throw new ArgumentNullException(nameof(_fieldImpactPosition));
                }
            }

            var impactPosition = (Vector3) _fieldImpactPosition.GetValue(meteor);
            
            // Show effect.
            if (!_disableEffects.Value)
            {
                EffectManager.instance.SpawnEffect(self.impactEffectPrefab, new EffectData
                {
                    origin = impactPosition
                }, true);
            }
            
            // Do damage.
            new BlastAttack
            {
                inflictor = self.gameObject,
                baseDamage = (self.blastDamageCoefficient * self.ownerDamage),
                baseForce = self.blastForce,
                canHurtAttacker = false, // Modified
                crit = self.isCrit,
                falloffModel = BlastAttack.FalloffModel.Linear,
                attacker = self.owner,
                bonusForce = Vector3.zero,
                damageColorIndex = DamageColorIndex.Item,
                position = impactPosition,
                procChainMask = new ProcChainMask(),
                procCoefficient = 1f,
                teamIndex = TeamComponent.GetObjectTeam(self.owner),
                radius = self.blastRadius
            }.Fire();
        }
    }
}