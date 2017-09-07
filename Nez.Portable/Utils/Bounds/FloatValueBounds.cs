namespace Nez
{
    public class FloatValueBounds : ValueBounds<float>
    {
        public FloatValueBounds(float val) : base(val)
        {
        }

        public FloatValueBounds(float min, float max) : base(min, max)
        {
        }

        public float NextValue()
        {
            return min + (max - min) * Random.nextFloat();
        }
    }
}