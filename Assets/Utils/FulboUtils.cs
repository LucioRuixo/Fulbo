using UnityEngine;

namespace Fulbo
{
    public static class FulboUtils
    {
        public static int Random1Min1() => Random.Range(0, 1) == 0 ? -1 : 1;
    }
}