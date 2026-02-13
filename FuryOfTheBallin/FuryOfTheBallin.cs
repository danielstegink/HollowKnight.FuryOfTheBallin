using FuryOfTheBallin.Helpers;
using GlobalEnums;
using Modding;
using Satchel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FuryOfTheBallin
{
    public class FuryOfTheBallin : Mod, ILocalSettings<LocalSaveData>
    {
        internal static FuryOfTheBallin Instance;

        public override string GetVersion() => "1.0.1.0";

        #region Save Data
        public void OnLoadLocal(LocalSaveData s)
        {
            SharedData.localSaveData = s;

            if (SharedData.customCharm != null)
            {
                SharedData.customCharm.OnLoadLocal();
            }
        }

        public LocalSaveData OnSaveLocal()
        {
            if (SharedData.customCharm != null)
            {
                SharedData.customCharm.OnSaveLocal();
            }

            return SharedData.localSaveData;
        }
        #endregion

        public override List<ValueTuple<string, string>> GetPreloadNames()
        {
            return new List<ValueTuple<string, string>>
            {
                new ValueTuple<string, string>("Fungus3_40_boss", "Warrior/Ghost Warrior Marmu")
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            SharedData.customCharm = new CustomCharm();

            SharedData.marmu = preloadedObjects["Fungus3_40_boss"]["Warrior/Ghost Warrior Marmu"];
            SharedData.marmu.SetActive(false);

            On.HeroController.Update += OnUpdate;
            Log("Initialized");
        }

        /// <summary>
        /// Easiest way to update the charm status is to check here if Marmu has been killed
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void OnUpdate(On.HeroController.orig_Update orig, HeroController self)
        {
            // If the player has defeated Marmu, make sure to note it
            if (PlayerData.instance.killedGhostMarmu)
            {
                SharedData.customCharm.GiveCharm();
            }

            orig(self);
        }
    }
}