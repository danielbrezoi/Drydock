﻿using System;
using Drydock.Render;
using Drydock.UI;
using Drydock.UI.Button;
using Microsoft.Xna.Framework;

namespace Drydock.Logic.InterfaceObj{
    internal class CurveController{
        //private readonly CurveHandle _centerHandle;
       // private readonly CurveHandle _handle1;
       // private readonly CurveHandle _handle2;
        private readonly Line2D _line1;
        private readonly Line2D _line2;

        private readonly Button _centerHandle;
        private readonly Button _handle1;
        private readonly Button _handle2;


        #region properties

        public Vector2 CenterHandlePos{
            get { return _centerHandle.CentPosition; }
        }

        public Vector2 PrevHandlePos{
            get { return _handle1.CentPosition; }
        }

        public Vector2 NextHandlePos{
            get { return _handle2.CentPosition; }
        }

        #endregion

        // private bool _isSelected;

        public CurveController(int initX, int initY, float length1, float length2, float angle1){
            Vector2 component1 = Common.GetComponentFromAngle(angle1, length1);
            Vector2 component2 = Common.GetComponentFromAngle((float) (angle1 - Math.PI), length2); // minus math.pi to reverse direction

            _handle1 = UIContext.Add<Button>(
                new Button(
                    identifier: 0,
                    width: 9,
                    height: 9,
                    x: (int)component1.X + initX,
                    y: (int)component1.Y + initY,
                    layerDepth: 1.0f,
                    textureName: "box",
                    components: new IUIElementComponent[]{
                        new DraggableComponent(),
                        new FadeComponent(FadeComponent.FadeState.Faded)
                    }
                    )
                    );

            _handle2 = UIContext.Add<Button>(
                new Button(
                    identifier: 1,
                    width: 9,
                    height: 9,
                    x: (int)component2.X + initX,
                    y: (int)component2.Y + initY,
                    layerDepth: 1.0f,
                    textureName: "box",
                    components: new IUIElementComponent[]{
                        new DraggableComponent(),
                        new FadeComponent(FadeComponent.FadeState.Faded)
                    }
                    )
                    );

            _centerHandle = UIContext.Add<Button>(
                new Button(
                    identifier: 2,
                    width: 9,
                    height: 9,
                    x: initX,
                    y: initY,
                    layerDepth: 1.0f,
                    textureName: "box",
                    components: new IUIElementComponent[]{
                        new DraggableComponent(),
                        new FadeComponent(FadeComponent.FadeState.Faded)
                    }
                    )
                    );
            _line1 = new Line2D(_centerHandle.CentPosition, _handle1.CentPosition, 0.5f);
            _line2 = new Line2D(_centerHandle.CentPosition, _handle2.CentPosition, 0.5f);
        }


        /// <summary>
        /// this function balances handle movement so that they stay in a straight line and their movements translate to other handles
        /// </summary>
        public void ReactToDragMovement(Button owner, int dx, int dy){
            switch (owner.Identifier){
                case 0:
                    _line1.TranslateOrigin(dx, dy);
                    _line1.TranslateDestination(dx, dy);
                    _line2.TranslateOrigin(dx, dy);
                    _line2.TranslateDestination(dx, dy);

                    //_handle1.ManualTranslation(dx, dy);
                    //_handle2.ManualTranslation(dx, dy);
                    break;
                case 1:
                    _line1.TranslateDestination(dx, dy);
                    _line2.Angle = (float) (_line1.Angle + Math.PI);
                    _handle2.X = (int) _line2.DestPoint.X - _handle2.BoundingBox.Width/2;
                    _handle2.Y = (int) _line2.DestPoint.Y - _handle2.BoundingBox.Height/2;

                    break;
                case 2:
                    _line2.TranslateDestination(dx, dy);
                    _line1.Angle = (float) (_line2.Angle + Math.PI);
                    _handle1.X = (int) _line1.DestPoint.X - _handle1.BoundingBox.Width/2;
                    _handle1.Y = (int) _line1.DestPoint.Y - _handle1.BoundingBox.Height/2;
                    break;
            }
        }
    }
}