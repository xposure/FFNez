

namespace Nez
{
    public class Vector2ValueBounds : ValueBounds<vec2>
    {
        public Vector2ValueBounds(vec2 val) : base(val)
        {
        }

        public Vector2ValueBounds(vec2 min, vec2 max) : base(min, max)
        {
        }

        public vec2 nextValue()
        {
            var nX = min.x + (max.x - min.x) * Random.nextFloat();
            var nY = min.y + (max.y - min.y) * Random.nextFloat();
            return new vec2(nX, nY);
        }
    }
}