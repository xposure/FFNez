#if ATMA_PHYSICS

namespace Atma
{
    public struct LineSegment
    {
        public vec2 p0;
        public vec2 p1;

        public vec2 Direction { get { return p0 - p1; } }

        public LineSegment(vec2 p0, vec2 p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        public vec2 Closest(vec2 p)
        {
            var l2 = (p0 - p1).LengthSqr;
            if (l2 == 0)
                return p0;

            var t = vec2.Dot(p - p0, p1 - p0) / l2;
            if (t < 0)
                return p0;
            else if (t > 1)
                return p1;

            return p0 + t * (p1 - p0);
        }

        public float Distance(vec2 p)
        {
            var l2 = (p0 - p1).LengthSqr;
            if (l2 == 0)
                return (p0 - p).Length;

            var t = vec2.Dot(p - p0, p1 - p0) / l2;
            if (t < 0)
                return (p0 - p).Length;
            else if (t > 1)
                return (p1 - p).Length;

            var projection = p0 + t * (p1 - p0);
            return (p - projection).Length;
        }

        public float DistanceSqr(vec2 p)
        {
            var l2 = (p0 - p1).LengthSqr;
            if (l2 == 0)
                return (p0 - p).LengthSqr;

            var t = vec2.Dot(p - p0, p1 - p0) / l2;
            if (t < 0)
                return (p0 - p).LengthSqr;
            else if (t > 1)
                return (p1 - p).LengthSqr;

            var projection = p0 + t * (p1 - p0);
            return (p - projection).LengthSqr;
        }

        //public IntersectResult Intersects(Ray ray)
        //{
        //    float t;
        //    vec2 hitPoint;
        //    vec2 min = p0;
        //    vec2 max = p1;

        //    if (ray.origin.x <= min.x && ray.direction.x > 0)
        //    {
        //        t = (min.x - ray.origin.x) / ray.direction.x;

        //        if (t >= 0)
        //        {
        //            // substitue t back into ray and check bounds and distance
        //            hitPoint = ray.origin + ray.direction * t;

        //            if (hitPoint.y >= min.y && hitPoint.y <= max.y)
        //                return new IntersectResult(true, t);
        //        }
        //    }

        //    return new IntersectResult(false, 0f);
        //}

        public bool Intersects(vec2 vertex1, vec2 vertex2, vec2 vertex3, vec2 vertex4, out float r, out float s)
        {
            //float d = bd.x * ad.y - bd.y * ad.x;
            r = 0;
            s = 0;
            var d = 0f;
            //Make sure the lines aren't parallel
            vec2 vertex1to2 = vertex2 - vertex1;
            vec2 vertex3to4 = vertex4 - vertex3;
            //if (vertex1to2.x * -vertex3to4.y + vertex1to2.y * vertex3to4.x != 0)
            //{
            //if (vertex1to2.Y / vertex1to2.X != vertex3to4.Y / vertex3to4.X)
            {
                d = vertex1to2.x * vertex3to4.y - vertex1to2.y * vertex3to4.x;
                if (d != 0)
                {
                    vec2 vertex3to1 = vertex1 - vertex3;
                    r = (vertex3to1.y * vertex3to4.x - vertex3to1.x * vertex3to4.y) / d;
                    s = (vertex3to1.y * vertex1to2.x - vertex3to1.x * vertex1to2.y) / d;
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(LineSegment ls, out float r, out float s)
        {
            var vertex1 = ls.p0;
            var vertex2 = ls.p1;
            var vertex3 = p0;
            var vertex4 = p1;

            return Intersects(p0, p1, ls.p0, ls.p1, out r, out s);

        }

        public IntersectResult Intersects(LineSegment other)
        {
            float r, s;
            if (Intersects(other, out r, out s))
            {
                if (r >= 0)
                    return new IntersectResult(true, r);
            }

            return new IntersectResult();
        }

        public IntersectResult Intersects(Ray ls)
        {
            IntersectResult result = new IntersectResult();
            float r, s;

            var vertex1 = vec2.Zero;
            var vertex2 = ls.direction;
            var vertex3 = p0 - ls.origin;
            var vertex4 = p1 - ls.origin;

            if (Intersects(vertex1, vertex2, vertex3, vertex4, out r, out s))
            {
                if (r >= 0)
                {
                    //if (s > -0.000000001)
                    //    s = 0;

                    if (s >= 0 && s <= 1)
                    {
                        return new IntersectResult(true, r);
                    }
                }
            }


            return result;
        }
    }
}
#endif