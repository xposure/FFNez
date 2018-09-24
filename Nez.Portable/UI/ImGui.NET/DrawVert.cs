using System.Runtime.InteropServices;

#if MONOGAME

using Nez;
#else
using System.Numerics;
#endif

namespace ImGuiNET
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DrawVert
    {
        public vec2 pos;
        public vec2 uv;
        public uint col;

        public const int PosOffset = 0;
        public const int UVOffset = 8;
        public const int ColOffset = 16;
    };
}
