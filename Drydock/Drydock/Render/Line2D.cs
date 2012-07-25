﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render{
    internal class Line2D{
        #region methods and fields

        private readonly int _id;
        private bool _isDisposed;
        private Vector2 _point1;
        private Vector2 _point2;

        public Line2D(int x0, int y0, int x1, int y1){
            int i = 0;
            while (!_isFrameSlotAvail[i]){ //i cant wait for this to crash
                i++;
            }
            _point1 = new Vector2(x0, y0);
            _point2 = new Vector2(x1, y1);

            _isFrameSlotAvail[i] = false;
            _id = i;
            CalculateLineInfo();
            _isDisposed = false;
        }

        #region properties
        public Vector2 OriginPoint{
            get { return _point1; }
            set{
                _point1 = value;
                CalculateLineInfo();
            }
        }

        public Vector2 DestPoint{
            get { return _point2; }
            set{
                _point2 = value;
                CalculateLineInfo();
            }
        }

        public void TransposeOrigin(int dx, int dy){
            _point1.X += dx;
            _point1.Y += dy;
            CalculateLineInfo();
        }

        public void TransposeDestination(int dx, int dy){
            _point2.X += dx;
            _point2.Y += dy;
            CalculateLineInfo();
        }
        #endregion

        private void CalculateLineInfo(){
            _lineBlitLocations[_id] = new Vector2(_point1.X, _point1.Y);
            _lineAngles[_id] = (float) Math.Atan2(_point2.Y - _point1.Y, _point2.X - _point1.X);
            _lineLengths[_id] = Vector2.Distance(_point1, _point2);
        }

        ~Line2D(){
            Dispose();
        }


        public void Dispose(){
            if (!_isDisposed){
                _isFrameSlotAvail[_id] = true;
                _isDisposed = true;
            }
        }

        #endregion

        #region static methods and fields

        private const int _maxLines = 100;
        private static bool[] _isFrameSlotAvail;
        private static Texture2D _lineTexture;
        private static Vector2[] _lineBlitLocations;
        private static float[] _lineAngles;
        private static float[] _lineLengths;
        // private static float[] _frameLayerLevels;
        private static SpriteBatch _spriteBatch;

        public static void Init(GraphicsDevice device){
            _isFrameSlotAvail = new bool[_maxLines];
            _lineBlitLocations = new Vector2[_maxLines];
            _lineAngles = new float[_maxLines];
            _lineLengths = new float[_maxLines];

            _spriteBatch = new SpriteBatch(device);
            _lineTexture = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            _lineTexture.SetData(new[] {Color.Black});

            for (int i = 0; i < _maxLines; i++){
                _isFrameSlotAvail[i] = true;
            }
        }

        public static void Draw() {
            _spriteBatch.Begin();
            for (int i = 0; i < _maxLines; i++){
                if (_isFrameSlotAvail[i] == false){
                    _spriteBatch.Draw( //welp
                        _lineTexture,
                        _lineBlitLocations[i],
                        null,
                        Color.Black,
                        _lineAngles[i],
                        Vector2.Zero,
                        new Vector2(_lineLengths[i], 1),
                        SpriteEffects.None,
                        0
                        );
                }
            }
            _spriteBatch.End();
        }

        #endregion
    }
}