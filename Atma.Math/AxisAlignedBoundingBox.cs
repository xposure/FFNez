#if ATMA_PHYSICS

namespace Atma
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public struct AxisAlignedBox 
    {
#region Fields

        internal vec2 maxVector;
        internal vec2 minVector;
        private bool isInfinite;
        private bool isNull;

#endregion Fields

#region Constructors

        public AxisAlignedBox(float x0, float y0, float x1, float y1)
        {
            //corners = new vec2[4];
            minVector.x = x0;
            minVector.y = y0;
            maxVector.x = x1;
            maxVector.y = y1;
            isNull = false;
            isInfinite = false;
        }

        public AxisAlignedBox(vec2 min, vec2 max)
        {
            //corners = new vec2[4];
            minVector = min;
            maxVector = max;
            isNull = false;
            isInfinite = false;
        }

        public AxisAlignedBox(AxisAlignedBox box)
        {
            //corners = new vec2[4];
            minVector = box.minVector;
            maxVector = box.maxVector;
            isNull = box.IsNull;
            isInfinite = box.IsInfinite;
        }

#endregion Constructors

#region Public methods

        public float Height
        {
            get { return maxVector.y - minVector.y; }
        }

        public float Width
        {
            get { return maxVector.x - minVector.x; }
        }

        public float X0 { get { return minVector.x; } }

        public float X1 { get { return maxVector.x; } }

        public float Y0 { get { return minVector.y; } }

        public float Y1 { get { return maxVector.y; } }


        /// <summary>
        ///     Return new bounding box from the supplied dimensions.
        /// </summary>
        /// <param name="center">Center of the new box</param>
        /// <param name="size">Entire size of the new box</param>
        /// <returns>New bounding box</returns>
        public static AxisAlignedBox FromDimensions(vec2 center, vec2 size)
        {
            vec2 halfSize = .5f * size;

            return new AxisAlignedBox(center - halfSize, center + halfSize);
        }

        public static AxisAlignedBox FromRect(float x, float y, float w, float h)
        {
            var min = new vec2(x, y);
            var max = new vec2(w, h) + min;
            return new AxisAlignedBox(min, max);
        }

        public static AxisAlignedBox FromRect(vec2 min, float w, float h)
        {
            var max = new vec2(w, h) + min;
            return new AxisAlignedBox(min, max);
        }

        public static AxisAlignedBox FromRect(vec2 min, vec2 size)
        {
            return new AxisAlignedBox(min, min + size);
        }

        public void Inflate(float x, float y)
        {
            var hx = x / 2f;
            var hy = y / 2f;
            minVector.x -= hx;
            minVector.y -= hy;
            maxVector.x += hx;
            maxVector.y += hy;
        }

        public void Inflate(vec2 size)
        {
            Inflate(size.x, size.y);
        }

        /// <summary>
        ///		Allows for merging two boxes together (combining).
        /// </summary>
        /// <param name="box">Source box.</param>
        public void Merge(AxisAlignedBox box)
        {
            if (box.IsNull)
            {
                // nothing to merge with in this case, just return
                return;
            }
            else if (box.IsInfinite)
            {
                this.IsInfinite = true;
            }
            else if (this.IsNull)
            {
                SetExtents(box.Minimum, box.Maximum);
            }
            else if (!this.IsInfinite)
            {
                if (box.minVector.x < minVector.x)
                    minVector.x = box.minVector.x;
                if (box.maxVector.x > maxVector.x)
                    maxVector.x = box.maxVector.x;

                if (box.minVector.y < minVector.y)
                    minVector.y = box.minVector.y;
                if (box.maxVector.y > maxVector.y)
                    maxVector.y = box.maxVector.y;

            }
        }

        /// <summary>
        ///		Extends the box to encompass the specified point (if needed).
        /// </summary>
        /// <param name="point"></param>
        public void Merge(vec2 point)
        {
            if (isNull || isInfinite)
            {
                // if null, use this point
                SetExtents(point, point);
            }
            else
            {
                if (point.x > maxVector.x)
                    maxVector.x = point.x;
                else if (point.x < minVector.x)
                    minVector.x = point.x;

                if (point.y > maxVector.y)
                    maxVector.y = point.y;
                else if (point.y < minVector.y)
                    minVector.y = point.y;

            }
        }

        /// <summary>
        ///    Scales the size of the box by the supplied factor.
        /// </summary>
        /// <param name="factor">Factor of scaling to apply to the box.</param>
        public void Scale(vec2 factor)
        {
            SetExtents(minVector * factor, maxVector * factor);
        }

        /// <summary>
        ///		Sets both Minimum and Maximum at once, so that UpdateCorners only
        ///		needs to be called once as well.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetExtents(vec2 min, vec2 max)
        {
            isNull = false;
            isInfinite = false;

            minVector = min;
            maxVector = max;

        }

#endregion Public methods

#region Contain methods

        /// <summary>
        /// Tests whether the given point contained by this box.
        /// </summary>
        /// <param name="v"></param>
        /// <returns>True if the vector is contained inside the box.</returns>
        public bool Contains(vec2 v)
        {
            if (IsNull)
                return false;
            if (IsInfinite)
                return true;

            return Minimum.x <= v.x && v.x <= Maximum.x &&
                   Minimum.y <= v.y && v.y <= Maximum.y;
        }

        public bool Contains(AxisAlignedBox box)
        {
            return Contains(box.minVector) && Contains(box.maxVector);
        }

#endregion Contain methods

#region Intersection Methods

        public MinimumTranslationVector collide(AxisAlignedBox box2)
        {
            var overlap = Intersection(box2);
            if (!overlap.IsNull)
            {
                var axis = new Axis();
                var minOverlap = 0.0;
                var adjust = overlap.Size;

                if (adjust.x < adjust.y)
                {
                    minOverlap = adjust.x;
                    if (overlap.Center.x < box2.Center.x)
                        axis.edge = new vec2(overlap.X0, overlap.Y0) - new vec2(overlap.X0, overlap.Y1);
                    else
                        axis.edge = new vec2(overlap.X1, overlap.Y1) - new vec2(overlap.X1, overlap.Y0);
                }
                else
                {
                    minOverlap = adjust.y;
                    if (overlap.Center.y > box2.Center.y)
                        axis.edge = new vec2(overlap.X0, overlap.Y0) - new vec2(overlap.X1, overlap.Y0);
                    else
                        axis.edge = new vec2(overlap.X1, overlap.Y1) - new vec2(overlap.X0, overlap.Y1);
                }

                axis.unit = axis.edge.Perpendicular;
                axis.normal = axis.unit.Normalized;
                return new MinimumTranslationVector(axis, minOverlap);
            }
            return MinimumTranslationVector.Zero;
        }

        public MinimumTranslationVector collideX(AxisAlignedBox box2)
        {
            var overlap = Intersection(box2);
            if (!overlap.IsNull)
            {
                var axis = new Axis();
                var minOverlap = 0.0;
                var adjust = overlap.Size;

                minOverlap = adjust.x;
                if (overlap.Center.x < box2.Center.x)
                    axis.edge = new vec2(overlap.X0, overlap.Y0) - new vec2(overlap.X0, overlap.Y1);
                else
                    axis.edge = new vec2(overlap.X1, overlap.Y1) - new vec2(overlap.X1, overlap.Y0);

                axis.unit = axis.edge.Perpendicular;
                axis.normal = axis.unit.Normalized;
                return new MinimumTranslationVector(axis, minOverlap);
            }
            return MinimumTranslationVector.Zero;
        }

        public MinimumTranslationVector collideY(AxisAlignedBox box2)
        {
            var overlap = Intersection(box2);
            if (!overlap.IsNull)
            {
                var axis = new Axis();
                var minOverlap = 0.0;
                var adjust = overlap.Size;

                minOverlap = adjust.y;
                if (overlap.Center.y > box2.Center.y)
                    axis.edge = new vec2(overlap.X0, overlap.Y0) - new vec2(overlap.X1, overlap.Y0);
                else
                    axis.edge = new vec2(overlap.X1, overlap.Y1) - new vec2(overlap.X0, overlap.Y1);

                axis.unit = axis.edge.Perpendicular;
                axis.normal = axis.unit.Normalized;
                return new MinimumTranslationVector(axis, minOverlap);
            }
            return MinimumTranslationVector.Zero;
        }

        /// <summary>
        ///		Calculate the area of intersection of this box and another
        /// </summary>
        public AxisAlignedBox Intersection(AxisAlignedBox b2)
        {
            if (!Intersects(b2))
                return AxisAlignedBox.Null;

            vec2 intMin = vec2.Zero;
            vec2 intMax = vec2.Zero;

            vec2 b2max = b2.maxVector;
            vec2 b2min = b2.minVector;

            if (b2max.x > maxVector.x && maxVector.x > b2min.x)
                intMax.x = maxVector.x;
            else
                intMax.x = b2max.x;
            if (b2max.y > maxVector.y && maxVector.y > b2min.y)
                intMax.y = maxVector.y;
            else
                intMax.y = b2max.y;

            if (b2min.x < minVector.x && minVector.x < b2max.x)
                intMin.x = minVector.x;
            else
                intMin.x = b2min.x;
            if (b2min.y < minVector.y && minVector.y < b2max.y)
                intMin.y = minVector.y;
            else
                intMin.y = b2min.y;

            return new AxisAlignedBox(intMin, intMax);
        }

        /// <summary>
        ///		Returns whether or not this box intersects another.
        /// </summary>
        /// <param name="box2"></param>
        /// <returns>True if the 2 boxes intersect, false otherwise.</returns>
        public bool Intersects(AxisAlignedBox box2)
        {
            // Early-fail for nulls
            if (this.IsNull || box2.IsNull)
                return false;

            if (this.IsInfinite || box2.IsInfinite)
                return true;

            // Use up to 6 separating planes
            if (this.maxVector.x <= box2.minVector.x)
                return false;
            if (this.maxVector.y <= box2.minVector.y)
                return false;

            if (this.minVector.x >= box2.maxVector.x)
                return false;
            if (this.minVector.y >= box2.maxVector.y)
                return false;

            // otherwise, must be intersecting
            return true;
        }

        public bool Intersects(Circle circle)
        {
            if (Intersects(circle.Center))
                return true;

            foreach (var it in Corners)
                if (circle.Intersects(it))
                    return true;

            return false;
        }

        public bool Intersects(Ray ray)
        {
            return glm.Intersects(ray, this).Hit;
        }

        /// <summary>
        ///		Tests whether this box intersects a sphere.
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns>True if the sphere intersects, false otherwise.</returns>
        //public bool Intersects(Sphere sphere)
        //{
        //    return Utility.Intersects(sphere, this);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="plane"></param>
        /// <returns>True if the plane intersects, false otherwise.</returns>
        //public bool Intersects(Plane plane)
        //{
        //    return Utility.Intersects(plane, this);
        //}

        /// <summary>
        ///		Tests whether the vector point is within this box.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>True if the vector is within this box, false otherwise.</returns>
        public bool Intersects(vec2 vector)
        {
            return (vector.x >= minVector.x && vector.x <= maxVector.x &&
                vector.y >= minVector.y && vector.y <= maxVector.y);
        }

#endregion Intersection Methods

#region Properties

        /// <summary>
        ///		Returns a null box
        /// </summary>
        public static AxisAlignedBox Null
        {
            get
            {
                AxisAlignedBox nullBox = new AxisAlignedBox(new vec2(-0.5f, -0.5f), new vec2(0.5f, 0.5f));
                nullBox.IsNull = true;
                nullBox.isInfinite = false;
                return nullBox;
            }
        }

        /// <summary>
        ///    Get/set the center point of this bounding box.
        /// </summary>
        public vec2 Center
        {
            get
            {
                return (minVector + maxVector) * 0.5f;
            }
            set
            {
                vec2 halfSize = .5f * Size;
                minVector = value - halfSize;
                maxVector = value + halfSize;
            }
        }

        public IEnumerable<vec2> Corners
        {
            get
            {
                yield return minVector;
                yield return new vec2(maxVector.x, minVector.y);
                yield return maxVector;
                yield return new vec2(minVector.x, maxVector.y);
            }
        }

        public vec2 HalfSize
        {
            get
            {
                if (isNull)
                    return vec2.Zero;

                if (isInfinite)
                    return new vec2(float.PositiveInfinity, float.PositiveInfinity);

                return (Maximum - Minimum) * 0.5f;
            }
        }

        /// <summary>
        /// Returns true if the box is infinite.
        /// </summary>
        public bool IsInfinite
        {
            get
            {
                return isInfinite;
            }
            set
            {
                isInfinite = value;
                if (value)
                    isNull = false;
            }
        }

        /// <summary>
        ///		Get/set the value of whether this box is null (i.e. not dimensions, etc).
        /// </summary>
        public bool IsNull
        {
            get
            {
                return isNull;
            }
            set
            {
                isNull = value;
                if (value)
                    isInfinite = false;
            }
        }

        /// <summary>
        ///		Get/set the maximum corner of the box.
        /// </summary>
        public vec2 Maximum
        {
            get
            {
                return maxVector;
            }
            set
            {
                isNull = false;
                maxVector = value;
            }
        }

        /// <summary>
        ///		Get/set the minimum corner of the box.
        /// </summary>
        public vec2 Minimum
        {
            get
            {
                return minVector;
            }
            set
            {
                isNull = false;
                minVector = value;
            }
        }

        /// <summary>
        ///     Get/set the size of this bounding box.
        /// </summary>
        public vec2 Size
        {
            get
            {
                return maxVector - minVector;
            }
            set
            {
                vec2 center = Center;
                vec2 halfSize = .5f * value;
                minVector = center - halfSize;
                maxVector = center + halfSize;
            }
        }

        /// <summary>
        ///     Calculate the volume of this box
        /// </summary>
        public float Volume
        {
            get
            {
                if (isNull)
                    return 0.0f;

                if (isInfinite)
                    return float.PositiveInfinity;

                vec2 diff = Maximum - Minimum;
                return diff.x * diff.y;
            }
        }

#endregion Properties

#region Operator Overloads

        public static bool operator !=(AxisAlignedBox left, AxisAlignedBox right)
        {
            //if ((object.ReferenceEquals(left, null) || left.isNull) &&
            //    (object.ReferenceEquals(right, null) || right.isNull))
            //    return false;

            //else if ((object.ReferenceEquals(left, null) || left.isNull) ||
            //         (object.ReferenceEquals(right, null) || right.isNull))
            //    return true;

            return left.minVector != right.minVector || left.maxVector != right.maxVector;
            //return
            //    (left.corners[0] != right.corners[0] || left.corners[1] != right.corners[1] || left.corners[2] != right.corners[2] ||
            //    left.corners[3] != right.corners[3] || left.corners[4] != right.corners[4] || left.corners[5] != right.corners[5] ||
            //    left.corners[6] != right.corners[6] || left.corners[7] != right.corners[7]);
        }

        public static bool operator ==(AxisAlignedBox left, AxisAlignedBox right)
        {
            //if ((object.ReferenceEquals(left, null) || left.isNull) &&
            //    (object.ReferenceEquals(right, null) || right.isNull))
            //    return true;

            //else if ((object.ReferenceEquals(left, null) || left.isNull) ||
            //         (object.ReferenceEquals(right, null) || right.isNull))
            //    return false;

            return left.minVector == right.minVector && left.maxVector == right.maxVector;
            //(left.corners[0] == right.corners[0] && left.corners[1] == right.corners[1] && left.corners[2] == right.corners[2] &&
            //left.corners[3] == right.corners[3] && left.corners[4] == right.corners[4] && left.corners[5] == right.corners[5] &&
            //left.corners[6] == right.corners[6] && left.corners[7] == right.corners[7]);
        }

        public override bool Equals(object obj)
        {
            return obj is AxisAlignedBox && this == (AxisAlignedBox)obj;
        }

        public override int GetHashCode()
        {
            if (isNull)
                return 0;

            unchecked
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ minVector.GetHashCode();
                hash = (hash * 16777619) ^ maxVector.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", this.minVector, this.maxVector);
        }

#endregion Operator Overloads

     

        //public AxisAlignedBox[] fromRectOffset(RectOffset offset)
        //{
        //    var inner = new AxisAlignedBox(minVector + offset.min, maxVector - offset.max);

        //    var rects = new AxisAlignedBox[9];

        //    rects[0] = new AxisAlignedBox(corners[0], inner.corners[0]);
        //    rects[2] = new AxisAlignedBox(glm.Min(corners[1], inner.corners[1]), glm.Max(corners[1], inner.corners[1]));
        //    rects[6] = new AxisAlignedBox(glm.Min(corners[3], inner.corners[3]), glm.Max(corners[3], inner.corners[3]));
        //    rects[8] = new AxisAlignedBox(inner.corners[2], corners[2]);

        //    rects[1] = new AxisAlignedBox(rects[0].corners[1], inner.corners[1]);
        //    rects[3] = new AxisAlignedBox(rects[0].corners[3], inner.corners[3]);
        //    rects[4] = inner;
        //    rects[5] = new AxisAlignedBox(rects[2].corners[3], rects[8].corners[1]);
        //    rects[7] = new AxisAlignedBox(rects[6].corners[1], rects[8].corners[3]);
        //    return rects;
        //}



        public void RotateAndContain(vec2 pivot, float r)
        {
            if (!isNull && !isInfinite && r != 0)
            {
                var rotatedCorners = this.ToOBB(pivot, r);
                var center = (rotatedCorners[0] + rotatedCorners[1] + rotatedCorners[2] + rotatedCorners[3]) / 4f;
                this.minVector = center;
                this.maxVector = center;

                this.Merge(rotatedCorners[0]);
                this.Merge(rotatedCorners[1]);
                this.Merge(rotatedCorners[2]);
                this.Merge(rotatedCorners[3]);
            }
        }

        public vec2[] ToOBB(vec2 pivot, float r)
        {
            var corners = Corners.ToArray();

            if (!isNull && !isInfinite && r != 0)
            {
                var rotatedCorners = new vec2[4];
                for (var i = 0; i < corners.Length; i++)
                {
                    rotatedCorners[i] = glm.RotateAround(corners[i], pivot, r);//corners[i].Rotate(pivot, r);
                }
                return rotatedCorners;
            }
            return corners;
        }

        public Rectangle ToRect()
        {
            if (IsNull)
                throw new NullReferenceException();

            return new Rectangle((int)minVector.x, (int)minVector.y, (int)(maxVector.x - minVector.x), (int)(maxVector.y - minVector.y));
        }
    }
}
#endif