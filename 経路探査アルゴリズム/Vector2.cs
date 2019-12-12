using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 経路探査アルゴリズム
{
    class Vector2
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2(int _x_, int _y_)
        {
            this.x = _x_;
            this.y = _y_;
        }

        public static Vector2 operator +(Vector2 n1, Vector2 n2)
        {
            return new Vector2(n1.x + n2.x, n1.y + n2.y);
        }

        public static Vector2 operator -(Vector2 n1, Vector2 n2)
        {
            return new Vector2(n1.x - n2.x, n1.y - n2.y);
        }

        public static Vector2 operator *(Vector2 n1, Vector2 n2)
        {
            return new Vector2(n1.x * n2.x, n1.y * n2.y);
        }

        public static int Distance(Vector2 v1, Vector2 v2)
        {
            return (int)Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2));
        }

        public static double DistanceEx(Vector2 v1, Vector2 v2)
        {
            return Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2));
        }
    }
}
