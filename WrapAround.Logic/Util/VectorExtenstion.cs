using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WrapAround.Logic.Util
{
    public static class VectorExtenstion
    {
        public static void Deconstruct(this Vector2 vec, out float X, out float Y)
        { 
            (X, Y) = vec;
        }


    }
}
