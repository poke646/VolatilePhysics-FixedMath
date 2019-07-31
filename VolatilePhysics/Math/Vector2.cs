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

using FixMath.NET;

namespace Volatile
{
    public struct Vector2
    {
        public readonly Fix64 x;
        public readonly Fix64 y;

        public static Vector2 zero => new Vector2();

        public static Fix64 Dot(Vector2 a, Vector2 b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }

        public Fix64 sqrMagnitude
        {
            get
            {
                return (this.x * this.x) + (this.y * this.y);
            }
        }

        public Fix64 magnitude
        {
            get
            {
                return Mathf.Sqrt(this.sqrMagnitude);
            }
        }

        public Vector2 normalized
        {
            get
            {
                Fix64 magnitude = this.magnitude;
                return new Vector2(this.x / magnitude, this.y / magnitude);
            }
        }

        public Vector2(Fix64 x, Fix64 y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator *(Vector2 a, Fix64 b)
        {
            return new Vector2(a.x * b, a.y * b);
        }

        public static Vector2 operator *(Fix64 a, Vector2 b)
        {
            return new Vector2(b.x * a, b.y * a);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.x, -a.y);
        }
    }
}