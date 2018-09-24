#if ATMA_PHYSICS
namespace Atma
{
    public struct Axis
    {
        public vec2 normal;
        public vec2 unit;
        public vec2 edge;

        public readonly static Axis Zero = new Axis(vec2.Zero, vec2.Zero, vec2.Zero);

        public Axis(vec2 normal, vec2 unit, vec2 edge)
        {
            this.normal = normal;
            this.unit = unit;
            this.edge = edge;
        }

        public override string ToString()
        {
            return string.Format("Axis {{ Normal: {0}, Unit: {1}, Edge: {2} }}", normal, unit, edge);
        }
    }
}
#endif