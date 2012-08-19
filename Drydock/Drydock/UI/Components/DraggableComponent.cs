﻿#region

using System;
using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.UI.Components{
    internal delegate void DraggableObjectClamp(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY);

    internal delegate void OnComponentDrag(object caller, int dx, int dy);


    /// <summary>
    ///   allows a UI element to be dragged. Required element to be IUIInteractiveComponent
    /// </summary>
    internal class DraggableComponent : IUIComponent{
        private bool _isEnabled;
        private bool _isMoving;
        private Vector2 _mouseOffset;
        private IUIInteractiveElement _owner;

        #region properties

        public IUIElement Owner{ //this function acts as kind of a pseudo-constructor
            set{
                if (!(value is IUIInteractiveElement)){
                    throw new Exception("Invalid element componenet: Unable to set a drag component for a non-interactive element.");
                }
                _owner = (IUIInteractiveElement) value;
                ComponentCtor();
            }
        }

        #endregion

        #region ctor

        public DraggableComponent(){
            _mouseOffset = new Vector2();
        }

        private void ComponentCtor(){
            _isEnabled = true;
            _isMoving = false;
            _owner.OnLeftButtonPress.Add(OnLeftButtonDown);
            _owner.OnLeftButtonRelease.Add(OnLeftButtonUp);
            _owner.OnMouseMovement.Add(OnMouseMovement);
        }

        #endregion

        #region IUIComponent Members

        public bool IsEnabled{
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public void Update(){
        }

        #endregion

        #region event handlers

        private InterruptState OnLeftButtonDown(MouseState state, MouseState? prevState = null){
            if (!_isMoving){
                if (_owner.BoundingBox.Contains(state.X, state.Y)){
                    _isMoving = true;
                    _owner.Owner.DisableEntryHandlers = true;
                    _mouseOffset.X = _owner.X - state.X;
                    _mouseOffset.Y = _owner.Y - state.Y;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        private InterruptState OnLeftButtonUp(MouseState state, MouseState? prevState = null){
            if (_isMoving){
                _isMoving = false;
                _owner.Owner.DisableEntryHandlers = false;
            }
            return InterruptState.AllowOtherEvents;
        }

        private InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null){
            if (_isMoving){
                var oldX = (int) _owner.X;
                var oldY = (int) _owner.Y;
                var x = (int) (state.X + _mouseOffset.X);
                var y = (int) (state.Y + _mouseOffset.Y);
                if (DragMovementClamp != null){
                    DragMovementClamp(_owner, ref x, ref y, oldX, oldY);
                }

                //this block checks if a drag clamp is preventing the owner from moving, if thats the case then kill the drag
                var tempRect = new Rectangle(x, y,(int) _owner.BoundingBox.Width, (int)_owner.BoundingBox.Height);
                if (!tempRect.Contains(state.X, state.Y)) {
                    _isMoving = false;
                    _owner.Owner.DisableEntryHandlers = false;
                    return InterruptState.AllowOtherEvents;
                }

                _owner.X = x;
                _owner.Y = y;

                if (DragMovementDispatcher != null){
                    DragMovementDispatcher(_owner, x - oldX, y - oldY);
                }
                return InterruptState.InterruptEventDispatch;
            }
            return InterruptState.AllowOtherEvents;
        }

        #endregion

        public event OnComponentDrag DragMovementDispatcher;
        public event DraggableObjectClamp DragMovementClamp;
    }
}