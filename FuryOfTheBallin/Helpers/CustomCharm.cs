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
                prefab.name = "FuryOfTheBallin.Marmu";
                prefab.SetActive(false);
                UnityEngine.GameObject.DontDestroyOnLoad(prefab);
            }

            On.HeroController.Update += SpawnMarmu;
        }

        /// <summary>
        /// Deactivates the charm effects
        /// </summary>
        public override void Unequip()
        {
            On.HeroController.Update -= SpawnMarmu;
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
                clone = UnityEngine.GameObject.Instantiate(prefab, HeroController.instance.transform.position, Quaternion.identity);
                clone.SetActive(true);
                timer.Restart();
            }
        }
        #endregion
    }
}