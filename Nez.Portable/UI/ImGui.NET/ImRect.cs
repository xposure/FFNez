#if MONOGAME

using Nez;
#else
using System.Numerics;
#endif

namespace ImGuiNET
{
    public struct ImRect
    {
        public vec2 Min;
        public vec2 Max;

        public ImRect(vec2 min, vec2 max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(vec2 p)
        {
            return p.x >= Min.x && p.y >= Min.y && p.x < Max.x && p.y < Max.y;
        }

        public vec2 GetSize()
        {
            return Max - Min;
        }
    }
}
