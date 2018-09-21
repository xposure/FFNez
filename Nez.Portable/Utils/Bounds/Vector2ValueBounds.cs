#if FEATURE_UTILS
using Microsoft.Xna.Framework;

namespace Atma
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
            var nX = min.X + (max.X - min.X) * Random.nextFloat();
            var nY = min.Y + (max.Y - min.Y) * Random.nextFloat();
            return new vec2(nX, nY);
        }
    }
}
#endif
