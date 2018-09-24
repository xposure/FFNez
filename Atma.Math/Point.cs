using System;
using System.Runtime.Serialization;

namespace Nez
{
    public struct Point : IEquatable<Point>
    {
#if DEBUG
        public static implicit operator vec2(Point v) => new vec2(v.X, v.Y);
        public static explicit operator Point(Microsoft.Xna.Framework.Vector2 v) => new Point((int)v.X, (int)v.Y);
        public static implicit operator Microsoft.Xna.Framework.Point(Point v) => new Microsoft.Xna.Framework.Point(v.X, v.Y);
        public static implicit operator Microsoft.Xna.Framework.Vector2(Point v) => new Microsoft.Xna.Framework.Vector2(v.X, v.Y);
        public static implicit operator Point(Microsoft.Xna.Framework.Point v) => new Point(v.X, v.Y);


#endif
        #region Private Fields

        private static Point zeroPoint = new Point();

        #endregion Private Fields

        #region Public Fields

        public int X;

        public int Y;

        #endregion Public Fields

        #region Properties

        public static Point Zero
        {
            get { return zeroPoint; }
        }

        #endregion Properties

        #region Constructors

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        #endregion Constructors

        #region Operators

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator *(Point a, Point b)
        {
            return new Point(a.X * b.X, a.Y * b.Y);
        }

        public static Point operator /(Point a, Point b)
        {
            return new Point(a.X / b.X, a.Y / b.Y);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !a.Equals(b);
        }

        #endregion

        #region Public methods

        public bool Equals(Point other)
        {
            return ((X == other.X) && (Y == other.Y));
        }

        public override bool Equals(object obj)
        {
            return (obj is Point) ? Equals((Point)obj) : false;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override string ToString()
        {
            return string.Format("{{X:{0} Y:{1}}}", X, Y);
        }

        #endregion
    }
}

