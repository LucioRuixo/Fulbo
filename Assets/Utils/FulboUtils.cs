using UnityEngine;

namespace Fulbo
{
    public static class FulboUtils
    {
        public static int Random1Min1() => Random.Range(0, 1) == 0 ? -1 : 1;
    }

    public static class Die
    {
        public static int Roll(int d) => Random.Range(1, d + 1);
    }
}