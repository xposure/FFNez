﻿namespace Atma
{
    public struct MinimumTranslationVector
    {
        public double overlap;
        public Axis smallest;

        public bool intersects { get { return overlap != 0; } }

        public readonly static MinimumTranslationVector Zero = new MinimumTranslationVector(Axis.Zero, 0);

        public MinimumTranslationVector(Axis smallest, double overlap)
        {
            this.smallest = smallest;
            this.overlap = overlap;
        }

        public override string ToString()
        {
            return string.Format("O: {0}, A:{{{1}}}", overlap, smallest);
        }
    }
}
