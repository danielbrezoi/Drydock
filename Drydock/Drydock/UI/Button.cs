﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal class Button : IUIInteractiveElement{
        #region properties and fields

        private const int _timeTillHoverProc = 1000;
        private readonly Stopwatch _clickTimer;
        private readonly Stopwatch _hoverTimer; //nonimp, put in superclass
        private readonly int _identifier; //non-function based identifier that can be used to differentiate buttons
        private readonly Sprite2D _sprite; //the button's sprite
        private Rectangle _boundingBox; //bounding box that represents the bounds of the button
        private Vector2 _centPosition; //represents the approximate center of the button

        public Vector2 CentPosition{
            get { return _centPosition; }
        }
        public int Identifier{
            get { return _identifier; }
        }
        public IAdvancedPrimitive Sprite{
            get { return _sprite; }
        }
        public int X{
            get { return _boundingBox.X; }
            set{
                _boundingBox.X = value;
                _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            }
        }
        public int Y{
            get { return _boundingBox.Y; }
            set{
                _boundingBox.Y = value;
                _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;
            }
        }
        public int Width{
            get { return _boundingBox.Width; }
            set { _boundingBox.Width = value; }
        }
        public int Height{
            get { return _boundingBox.Height; }
            set { _boundingBox.Height = value;  }
        }

        public Rectangle BoundingBox{
            get { return _boundingBox; }
        }

        public float Opacity { get; set; }
        public float Depth { get; set; }
        public IUIElementComponent[] Components { get; set; }
        public List<OnMouseAction> OnLeftButtonClick { get; set; }
        public List<OnMouseAction> OnLeftButtonDown { get; set; }
        public List<OnMouseAction> OnLeftButtonUp { get; set; }
        public List<OnMouseAction> OnMouseEntry { get; set; }
        public List<OnMouseAction> OnMouseExit { get; set; }
        public List<OnMouseAction> OnMouseHover { get; set; }
        public List<OnMouseAction> OnMouseMovement { get; set; }
        public List<OnKeyboardAction> OnKeyboardAction { get; set; }

        #endregion

        #region ctor

        public Button(int x, int y, int width, int height, float layerDepth, string textureName, IUIElementComponent[] components, int identifier = 0){
            _identifier = identifier;
            _centPosition = new Vector2();
            _boundingBox = new Rectangle(x, y, width, height);
            _sprite = new Sprite2D(textureName, this);
            _clickTimer = new Stopwatch();
            _hoverTimer = new Stopwatch();
            OnLeftButtonDown = new List<OnMouseAction>();
            OnLeftButtonUp = new List<OnMouseAction>();
            OnLeftButtonClick = new List<OnMouseAction>();
            OnMouseMovement = new List<OnMouseAction>();
            OnMouseHover = new List<OnMouseAction>();
            OnMouseEntry = new List<OnMouseAction>();
            OnMouseExit = new List<OnMouseAction>();
            OnKeyboardAction = new List<OnKeyboardAction>();

            _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;

            Depth = layerDepth;
            Components = components;

            foreach (IUIElementComponent component in Components){
                component.Owner = this;
            }
        }

        #endregion

        #region event handlers

        public bool MouseMovementHandler(MouseState state){
            foreach (OnMouseAction t in OnMouseMovement){
                t(state);
            }
            if (_hoverTimer.IsRunning){
                _hoverTimer.Restart();
            }
            return false;
        }

        public bool MouseClickHandler(MouseState state){
            //for this event, we can assume the mouse is within the button's boundingbox
            bool denyOtherElementsFromClick = false;

            if (state.LeftButton == ButtonState.Pressed){
                _clickTimer.Start();
                foreach (OnMouseAction t in OnLeftButtonDown){
                    if (t(state)){
                       denyOtherElementsFromClick = true;
                    }
                }
            }
            if (state.LeftButton == ButtonState.Released){
                foreach (OnMouseAction t in OnLeftButtonUp){
                    if (t(state)){
                       denyOtherElementsFromClick = true;
                    }
                }

                _clickTimer.Stop();
                //this is something that should be handled by the uicontext ffs
                if (_clickTimer.ElapsedMilliseconds < 200){
                    //okay, click registered. now dispatch events.
                    foreach (OnMouseAction t in OnLeftButtonClick){
                        System.Console.WriteLine("dispatching click");
                        if (t(state)){
                           denyOtherElementsFromClick = true;
                            
                        }
                    }
                }
                _clickTimer.Reset();
            }
            return denyOtherElementsFromClick;
        }

        public bool MouseEntryHandler(MouseState state){
            foreach (OnMouseAction action in OnMouseEntry){
                action(state);
            }
            _hoverTimer.Start();

            return false;
        }

        public bool MouseExitHandler(MouseState state){
            foreach (OnMouseAction action in OnMouseExit){
                action(state);
            }
            _hoverTimer.Reset();
            return false;
        }

        public bool KeyboardActionHandler(KeyboardState state){

            return false;
        }

        #endregion

        #region other IUIElement derived methods

        public TComponent GetComponent<TComponent>(){
            foreach (IUIElementComponent component in Components){
                if (component.GetType() == typeof (TComponent)){
                    return (TComponent) component;
                }
            }
            throw new Exception("Request made to a Button object for a component that did not exist.");
        }

        public void Update(){
            foreach (IUIElementComponent component in Components){
                component.Update();
            }
            if (_hoverTimer.IsRunning){
                if (_hoverTimer.ElapsedMilliseconds > _timeTillHoverProc){
                    _hoverTimer.Reset();
                    foreach (OnMouseAction action in OnMouseHover){
                        action(Mouse.GetState());
                    }
                }
            }
        }

        #endregion
    }
}