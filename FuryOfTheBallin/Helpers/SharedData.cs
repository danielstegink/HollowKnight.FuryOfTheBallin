using UnityEngine;

namespace FuryOfTheBallin.Helpers
{
    public static class SharedData
    {
        /// <summary>
        /// The central charm of the mod
        /// </summary>
        public static CustomCharm customCharm;

        /// <summary>
        /// Data for the save file
        /// </summary>
        public static LocalSaveData localSaveData { get; set; } = new LocalSaveData();

        /// <summary>
        /// Stores Marmu for ease of reference
        /// </summary>
        public static GameObject marmu;
    }
}
