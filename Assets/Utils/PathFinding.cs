using System.Collections.Generic;
using UnityEngine;

namespace Fulbo
{
    public class PathFinding
    {
        private static void NormalizePath(ref Vector2 a, ref Vector2 b, out bool swapAxes, out Vector2Int sign)
        {
            Vector2 delta = b - a;

            swapAxes = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
            sign = new Vector2Int((int)Mathf.Sign(delta.x), (int)Mathf.Sign(delta.y));

            a = new Vector2(a.x * sign.x, a.y * sign.y);
            if (swapAxes) a = new Vector2(a.y, a.x);

            b = new Vector2(b.x * sign.x, b.y * sign.y);
            if (swapAxes) b = new Vector2(b.y, b.x);
        }

        private static int FindY(Vector2 a, Vector2 delta, float x) => Mathf.RoundToInt((delta.y / delta.x) * (x - a.x) + a.y);

        private static Vector2Int RevertCell(Vector2Int cell, bool swapAxes, Vector2Int sign)
        {
            cell = new Vector2Int(swapAxes ? cell.y : cell.x, swapAxes ? cell.x : cell.y);
            cell *= sign;
            return cell;
        }

        public static Vector2Int NextCell(Vector2Int a, Vector2Int b)
        {
            Vector2 fA = new Vector2(a.x, a.y);
            Vector2 fB = new Vector2(b.x, b.y);
            NormalizePath(ref fA, ref fB, out bool swapAxes, out Vector2Int sign);

            Vector2 delta = b - a;
            int x = a.x + 1;
            int y = FindY(a, delta, x);
            Vector2Int cell = new Vector2Int(x, y);

            return RevertCell(cell, swapAxes, sign);
        }

        public static Vector2Int[] Path(Vector2Int a, Vector2Int b)
        {
            List<Vector2Int> path = new List<Vector2Int>() { a };

            Vector2 fA = new Vector2(a.x, a.y);
            Vector2 fB = new Vector2(b.x, b.y);
            NormalizePath(ref fA, ref fB, out bool swapAxes, out Vector2Int sign);

            Vector2 delta = fB - fA;
            for (int i = 1; i < delta.x; i++)
            {
                int x = (int)fA.x + i;
                int y = FindY(fA, delta, x);
                Vector2Int cell = new Vector2Int(x, y);

                path.Add(RevertCell(cell, swapAxes, sign));
            }

            path.Add(b);
            return path.ToArray();
        }
    }
}