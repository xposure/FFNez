namespace Nez
{
    public class StartEndValueBounds<T> where T : struct
    {
        public T startMin { get; set; }
        public T startMax { get; set; }

        public T endMin { get; set; }
        public T endMax { get; set; }

        public StartEndValueBounds(T val) : this(val, val)
        {
        }

        public StartEndValueBounds(T startMin, T startMax) : this(startMin, startMax, startMin, startMax)
        {
        }

        public StartEndValueBounds(T startMin, T startMax, T endMin, T endMax)
        {
            this.startMin = startMin;
            this.startMax = startMax;
            this.endMin = endMin;
            this.endMax = endMax;
        }
    }
}