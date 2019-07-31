/*
 *  VolatilePhysics - A 2D Physics Library for Networked Games
 *  Copyright (c) 2015-2016 - Alexander Shoulson - http://ashoulson.com
 *
 *  This software is provided 'as-is', without any express or implied
 *  warranty. In no event will the authors be held liable for any damages
 *  arising from the use of this software.
 *  Permission is granted to anyone to use this software for any purpose,
 *  including commercial applications, and to alter it and redistribute it
 *  freely, subject to the following restrictions:
 *  
 *  1. The origin of this software must not be misrepresented; you must not
 *     claim that you wrote the original software. If you use this software
 *     in a product, an acknowledgment in the product documentation would be
 *     appreciated but is not required.
 *  2. Altered source versions must be plainly marked as such, and must not be
 *     misrepresented as being the original software.
 *  3. This notice may not be removed or altered from any source distribution.
*/

#if !UNITY

using System;
using FixMath.NET;

namespace Volatile
{
    public static class Mathf
    {
        public static readonly Fix64 PI = Fix64.Pi;

        public static Fix64 Clamp(Fix64 value, Fix64 min, Fix64 max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        public static Fix64 Max(Fix64 a, Fix64 b)
        {
            if (a > b)
                return a;
            return b;
        }

        public static Fix64 Min(Fix64 a, Fix64 b)
        {
            if (a < b)
                return a;
            return b;
        }

        public static Fix64 Sqrt(Fix64 a)
        {
            return Fix64.Sqrt(a);
        }

        public static Fix64 Sin(Fix64 a)
        {
            return Fix64.Sin(a);
        }

        public static Fix64 Cos(Fix64 a)
        {
            return Fix64.Cos(a);
        }

        public static Fix64 Atan2(Fix64 a, Fix64 b)
        {
            return Fix64.Atan2(a, b);
        }

        public static int Round(Fix64 fix64)
        {
            return (int)Fix64.Round(fix64);
        }
    }
}
#endif