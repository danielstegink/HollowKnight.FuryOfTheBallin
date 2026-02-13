using DanielSteginkUtils.Helpers;
using DanielSteginkUtils.Helpers.Charms.Templates;
using GlobalEnums;
using SFCore;
using System;
using System.Diagnostics;
using UnityEngine;

namespace FuryOfTheBallin.Helpers
{
    public class CustomCharm : TemplateCharm
    {
        public CustomCharm() : base(FuryOfTheBallin.Instance.Name, false) { }

        #region Properties
        protected override string GetName()
        {
            return @"Fury of the Ballin'";
        }

        protected override string GetDescription()
        {
            return "Embodies the playfulness of the Protector of the Queen's Gardens.\n\n" +
                    "When close to death, summons an echo of the fallen warrior.";
        }

        protected override int GetCharmCost()
        {
            return 1;
        }

        public override ItemChanger.AbstractLocation ItemChangerLocation()
        {
            throw new NotImplementedException();
        }

        protected override Sprite GetSpriteInternal()
        {
            return SpriteHelper.GetLocalSprite($"FuryOfTheBallin.Resources.Icon.png", "FuryOfTheBallin");
        }
        #endregion

        #region Settings
        public override void OnLoadLocal()
        {
            EasyCharmState charmSettings = new EasyCharmState()
            {
                IsEquipped = SharedData.localSaveData.charmEquipped,
                GotCharm = PlayerData.instance.killedGhostMarmu,
                IsNew = false,
            };

            RestoreCharmState(charmSettings);
        }

        public override void OnSaveLocal()
        {
            EasyCharmState charmSettings = GetCharmState();
            SharedData.localSaveData.charmEquipped = IsEquipped;
        }
        #endregion

        #region Activation
        /// <summary>
        /// Stores custom Marmu
        /// </summary>
        internal GameObject prefab = null;

        /// <summary>
        /// Tracks active copy of prefab
        /// </summary>
        private GameObject clone = null;

        /// <summary>
        /// Tracks how long clone has been active
        /// </summary>
        private Stopwatch timer = new Stopwatch();

        /// <summary>
        /// Activates the charm effects
        /// </summary>
        public override void Equip()
        {
            if (prefab == null)
            {
                prefab = UnityEngine.GameObject.Instantiate(SharedData.marmu);
                prefab.layer = (int)PhysLayers.HERO_ATTACK;
                Satchel.GameObjectUtils.RemoveComponent<DamageHero>(prefab);
                Satchel.GameObjectUtils.RemoveComponent<HealthManager>(prefab);
                Satchel.GameObjectUtils.RemoveComponent<ExtraDamageable>(prefab);
                prefab.AddComponent<BallinDamage>();
                GameObject hitIdle = Satchel.GameObjectUtils.Find(prefab, "Hit Idle");
                Satchel.GameObjectUtils.RemoveComponent<DamageHero>(hitIdle);
                GameObject hitRoll = Satchel.GameObjectUtils.Find(prefab, "Hit Roll");
                Satchel.GameObjectUtils.RemoveComponent<DamageHero>(hitRoll);
                prefab.name = "FuryOfTheBallin.Marmu";
                prefab.SetActive(false);
                UnityEngine.GameObject.DontDestroyOnLoad(prefab);
            }

            On.HeroController.Update += SpawnMarmu;
            On.HealthManager.Hit += MarmuBlock;
        }

        /// <summary>
        /// Deactivates the charm effects
        /// </summary>
        public override void Unequip()
        {
            On.HeroController.Update -= SpawnMarmu;
            On.HealthManager.Hit -= MarmuBlock;
            if (clone != null)
            {
                UnityEngine.Object.Destroy(clone);
            }
        }

        /// <summary>
        /// Checks if the player is at low health, and if so spawns a clone of Marmu
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void SpawnMarmu(On.HeroController.orig_Update orig, HeroController self)
        {
            orig(self);

            // Due to Marmu's bouncy nature and ability to get stuck out-of-bounds, we want to periodically reset him
            if (timer.ElapsedMilliseconds > 5000 &&
                clone != null)
            {
                UnityEngine.Object.Destroy(clone);
            }

            // If the player has 1 health and no blue health, and
            // there is no Marmu at the moment, spawn a Marmu clone
            if (PlayerData.instance.health == 1 &&
                PlayerData.instance.healthBlue == 0 &&
                clone == null)
            {
                timer.Restart();
                clone = UnityEngine.GameObject.Instantiate(prefab, HeroController.instance.transform.position, Quaternion.identity);
                clone.SetActive(true);
            }
        }

        /// <summary>
        /// Some enemies don't trigger Marmu's block, so she can one-shot certain bosses.
        /// 
        /// Now, if an enemy takes damage from Marmu, they will get a temporary component that renders them
        /// immune to her damage.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MarmuBlock(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.Source.name.StartsWith("FuryOfTheBallin.Marmu"))
            {
                GameObject enemy = self.gameObject;
                MarmuShield marmuShield = enemy.GetComponent<MarmuShield>();
                if (marmuShield == default)
                {
                    marmuShield = enemy.AddComponent<MarmuShield>();
                }
                else
                {
                    if (marmuShield.timer.ElapsedMilliseconds < 1000)
                    {
                        hitInstance.DamageDealt = 0;
                    }
                    else
                    {
                        marmuShield.timer.Restart();
                    }
                }
            }

            orig(self, hitInstance);
        }
        #endregion
    }
}