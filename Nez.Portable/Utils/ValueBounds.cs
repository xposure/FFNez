﻿namespace Nez
{
    public class ValueBounds<T> where T : struct
    {
        public T min { get; set; }
        public T max { get; set; }

        public ValueBounds(T val) : this(val, val)
        {
        }

        public ValueBounds(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public static implicit operator ValueBounds<T>(T val)
        {
            return new ValueBounds<T>(val);
        }
    }
}