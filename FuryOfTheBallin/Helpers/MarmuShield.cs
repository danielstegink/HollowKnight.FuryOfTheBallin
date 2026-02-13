using System.Diagnostics;
using UnityEngine;

namespace FuryOfTheBallin.Helpers
{
    public class MarmuShield : MonoBehaviour
    {
        internal Stopwatch timer = new Stopwatch();

        public MarmuShield()
        {
            timer.Start();
        }
    }
}