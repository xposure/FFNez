#if ATMA_PHYSICS

namespace Atma
{
    /// <summary>
    ///		Simple struct to allow returning a complex intersection result.
    /// </summary>
    public struct IntersectResult
    {
#region Fields

        /// <summary>
        ///		Did the intersection test result in a hit?
        /// </summary>
        public bool Hit;

        /// <summary>
        ///		If Hit was true, this will hold a query specific distance value.
        ///		i.e. for a Ray-Box test, the distance will be the distance from the start point
        ///		of the ray to the point of intersection.
        /// </summary>
        public float Distance;

#endregion Fields

        /// <summary>
        ///		Constructor.
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="distance"></param>
        public IntersectResult(bool hit, float distance)
        {
            this.Hit = hit;
            this.Distance = distance;
        }
    }
}
#endif