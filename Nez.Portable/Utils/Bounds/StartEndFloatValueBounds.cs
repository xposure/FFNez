namespace Nez
{
    public class StartEndFloatValueBounds : StartEndValueBounds<float>
    {
        public StartEndFloatValueBounds(float val) : base(val)
        {
        }

        public StartEndFloatValueBounds(float startMin, float startMax) : base(startMin, startMax)
        {
        }

        public StartEndFloatValueBounds(float startMin, float startMax, float endMin, float endMax) : base(startMin,
            startMax, endMin, endMax)
        {
        }

        public (float, float) NextValues()
        {
            return (new FloatValueBounds(start.min, start.max).NextValue(), new FloatValueBounds(end.min, end.max).NextValue());
        }
    }
}