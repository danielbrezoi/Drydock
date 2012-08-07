﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal class Button : IUIInteractiveElement{
        #region properties and fields

        private readonly int _identifier; //non-function based identifier that can be used to differentiate buttons
        private readonly Sprite2D _sprite; //the button's sprite
        private readonly FloatingRectangle _boundingBox; //bounding box that represents the bounds of the button
        private Vector2 _centPosition; //represents the approximate center of the button

        public Vector2 CentPosition{
            get { return _centPosition; }
            set {  
                _centPosition = value;
                _boundingBox.X = _centPosition.X - _boundingBox.Width / 2;
                _boundingBox.Y = _centPosition.Y - _boundingBox.Height / 2;
            }
        }

        public int Identifier{
            get { return _identifier; }
        }
        public IAdvancedPrimitive Sprite{
            get { return _sprite; }
        }

        public float X{
            get { return _boundingBox.X; }
            set{
                _boundingBox.X = value;
                _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            }
        }
        public float Y{
            get { return _boundingBox.Y; }
            set{
                _boundingBox.Y = value;
                _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;
            }
        }
        public float Width{
            get { return _boundingBox.Width; }
            set { _boundingBox.Width = value; }
        }
        public float Height{
            get { return _boundingBox.Height; }
            set { _boundingBox.Height = value;  }
        }
        public FloatingRectangle BoundingBox{
            get { return _boundingBox; }
        }

        public float Opacity { get; set; }
        public float Depth { get; set; }
        public UIElementCollection Owner { get; set; }
        public IUIElementComponent[] Components { get; set; }
        public List<EOnMouseEvent> OnLeftButtonClick { get; set; }
        public List<EOnMouseEvent> OnLeftButtonPress { get; set; }
        public List<EOnMouseEvent> OnLeftButtonRelease { get; set; }
        public List<EOnMouseEvent> OnMouseEntry { get; set; }
        public List<EOnMouseEvent> OnMouseExit { get; set; }
        public List<EOnMouseEvent> OnMouseMovement { get; set; }
        public List<EOnKeyboardEvent> OnKeyboardEvent { get; set; }

        #endregion

        #region ctor

        public Button(int x, int y, int width, int height, float layerDepth, string textureName, IUIElementComponent[] components, int identifier = 0){
            _identifier = identifier;
            _centPosition = new Vector2();
            _boundingBox = new FloatingRectangle(x, y, width, height);
            _sprite = new Sprite2D(textureName, this);
            OnLeftButtonClick = new List<EOnMouseEvent>();
            OnLeftButtonPress = new List<EOnMouseEvent>();
            OnLeftButtonRelease = new List<EOnMouseEvent>();
            OnMouseEntry = new List<EOnMouseEvent>();
            OnMouseExit = new List<EOnMouseEvent>();
            OnMouseMovement = new List<EOnMouseEvent>();
            OnKeyboardEvent = new List<EOnKeyboardEvent>();

            _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;

            Depth = layerDepth;
            Components = components;

            foreach (IUIElementComponent component in Components){
                component.Owner = this;
            }
        }

        #endregion


        #region other IUIElement derived methods

        public TComponent GetComponent<TComponent>(){
            foreach (IUIElementComponent component in Components){
                if (component is TComponent){
                    return (TComponent) component;
                }
            }
            throw new Exception("Request made to a Button object for a component that did not exist.");
        }

        public void Update(){
            foreach (IUIElementComponent component in Components){
                component.Update();
            }
        }

        public void Dispose(){
            Sprite.Dispose();
            Owner.DisposeElement(this);
        }

        #endregion
    }
}