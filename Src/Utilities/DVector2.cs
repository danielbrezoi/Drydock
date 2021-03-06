﻿#region

using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Utilities{
    internal struct DVector2{
        public double X;
        public double Y;

        public DVector2(double x, double y){
            X = x;
            Y = y;
        }

        public static explicit operator Vector2?(DVector2 v){
            return new Vector2((float) v.X, (float) v.Y);
        }

        public static explicit operator DVector2?(Vector2 v){
            return new DVector2(v.X, v.Y);
        }
    }
}