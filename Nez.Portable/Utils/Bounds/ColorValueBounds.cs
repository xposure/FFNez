using Microsoft.Xna.Framework;

namespace Nez
{
    public class ColorValueBounds : ValueBounds<Color>
    {
        public ColorValueBounds(Color val) : base(val)
        {
        }

        public ColorValueBounds(Color min, Color max) : base(min, max)
        {
        }

        public Color nextValue()
        {
            var nR = min.R + (max.R - min.R) * Random.nextFloat();
            var nG = min.G + (max.G - min.G) * Random.nextFloat();
            var nB = min.B + (max.B - min.B) * Random.nextFloat();
            var nA = min.A + (max.A - min.A) * Random.nextFloat();

            return new Color(nR, nG, nB, nA);
        }
    }
}