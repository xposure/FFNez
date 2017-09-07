namespace Nez
{
    public class StartEndValueBounds<T> where T : struct
    {
        public ValueBounds<T> start = new ValueBounds<T>();
        public ValueBounds<T> end = new ValueBounds<T>();

        public StartEndValueBounds(T val) : this(val, val)
        {
        }

        public StartEndValueBounds(T startMin, T startMax) : this(startMin, startMax, startMin, startMax)
        {
        }

        public StartEndValueBounds(T startMin, T startMax, T endMin, T endMax)
        {
            this.start.min = startMin;
            this.start.max = startMax;
            this.end.min = endMin;
            this.end.max = endMax;
        }
    }
}