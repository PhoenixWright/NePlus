using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

namespace NePlus.EngineComponents
{
    public class Camera
    {
        Vector2 _position = Vector2.Zero;
        Vector2 _origPosition = Vector2.Zero;
        Vector2 _targetPosition = Vector2.Zero;
        float _moveRate = 1;
        float _rotation = 0;
        float _origRotation = 0;
        float _targetRotation = 0;
        float _zoom = 1;
        float _origZoom = 1;
        float _targetZoom = 1;
        float _zoomRate = 0.01f;
        float _maxZoom = 4;
        float _minZoom = 0.25f;
        float _rotationRate = 0.01f;
        float _transition;
        bool _transitioning = false;
        const float _transitionSpeed = 0.01f;
        const float _smoothingSpeed = 0.15f;
        bool positionUnset = true;
        bool _zoomUnset = true;
        bool _rotationUnset = true;
        Vector2 _size;
        Vector2 _minPosition = Vector2.Zero;
        Vector2 _maxPosition = Vector2.Zero;
        Body _trackingBody;
        Func<Input, bool> _zoomIn = (Input input) =>
        {
            return input.IsCurPress(Buttons.DPadUp);
        };
        Func<Input, bool> _zoomOut = (Input input) =>
        {
            return input.IsCurPress(Buttons.DPadDown);
        };

        Func<Camera, bool> _clampingEnabled = (Camera camera) =>
        {
            return (camera._minPosition != camera._maxPosition);
        };

        Func<Input, Camera, float> _horizontalCameraMovement =
            (Input input, Camera camera) =>
            {
                return (input.RightStickPosition.X * camera._moveRate) * camera._zoom;
            };

        Func<Input, Camera, float> _verticalCameraMovement =
            (Input input, Camera camera) =>
            {
                return (input.RightStickPosition.Y * camera._moveRate) * camera.Zoom;
            };

        Func<Input, bool> _rotateLeft = (Input input) =>
        {
            return false;
        };

        Func<Input, bool> _rotateRight = (Input input) =>
        {
            return false;
        };

        Func<Input, bool> _resetCamera = (Input input) =>
        {
            return input.IsCurPress(Buttons.RightStick);
        };


        /// <summary>
        /// The constructor for the Camera2D class.
        /// </summary>
        /// <param name="size">
        /// the size of the camera's view when at zoom = 1. This is usually 
        /// set to the viewport's size.
        /// </param>
        public Camera(Vector2 size)
        {
            _size = size;
        }
        /// <summary>
        /// The current position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (positionUnset == true)
                {
                    _origPosition = value;
                    positionUnset = false;
                }
                _position = value;
                _targetPosition = value;
            }
        }
        /// <summary>
        /// The current rotation of the camera in radians.
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotationUnset == true)
                {
                    _origRotation = value;
                    _rotationUnset = false;
                }
                _rotation = value;
                _targetRotation = value;
            }
        }
        /// <summary>
        /// The current zoom of the camera. This is a value indicating 
        /// how far zoomed in or out the camera is. To get the actual 
        /// current size of the camera view, see CurSize.
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                if (_zoomUnset == true)
                {
                    _origZoom = value;
                    _zoomUnset = false;
                }
                _zoom = value;
                _targetZoom = value;
            }
        }
        /// <summary>
        /// the furthest zoomed in the camera can be. Larger numbers 
        /// are further zoomed in.
        /// </summary>
        public float MaxZoom
        {
            get { return _maxZoom; }
            set { _maxZoom = value; }
        }
        /// <summary>
        /// The futhest zoomed out that the camera can be. Smaller numbers 
        /// are further zoomed out.
        /// </summary>
        public float MinZoom
        {
            get { return _minZoom; }
            set { _minZoom = value; }
        }
        /// <summary>
        /// The amount that the camera rotates (in radians) in one timestep. 
        /// </summary>
        public float RotationRate
        {
            get { return _rotationRate; }
            set { _rotationRate = value; }
        }
        /// <summary>
        /// The amount that the camera zooms in or out in one timestep.
        /// </summary>
        public float ZoomRate
        {
            get { return _zoomRate; }
            set { _zoomRate = value; }
        }
        /// <summary>
        /// The rate at which the camera moves in one timestep.
        /// </summary>
        public float MoveRate
        {
            get { return _moveRate; }
            set { _moveRate = value; }
        }
        /// <summary>
        /// A vector representing the size of the camera when zoom is at 1.
        /// </summary>
        public Vector2 Size
        {
            get { return _size; }
            //i think changing the camera size on the fly 
            //would screw up stuff
            //set { _size = value; }
        }
        /// <summary>
        /// a vector representing the current size of the camera view.
        /// Expressed as: Size * (1 / zoom).
        /// </summary>
        public Vector2 CurSize
        {
            get { return Vector2.Multiply(_size, 1 / _zoom); }
        }
        /// <summary>
        /// A matrix representing the camera's current position, rotation, and zoom.
        /// Feed this to SpriteBatch.Begin (or use it in your matrix calculations for 
        /// drawing 2D polygons)
        /// </summary>
        public Matrix CameraMatrix
        {
            get
            {
                return Matrix.Identity *
                      Matrix.CreateTranslation(new Vector3(-_position, 0)) *
                      Matrix.CreateScale(_zoom) *
                      Matrix.CreateRotationZ(_rotation) *
                      Matrix.CreateTranslation(new Vector3(_size / 2, 0));
            }
        }
        /// <summary>
        /// The furthest up, and the furthest left the camera can go.
        /// if this value equals maxPosition, then no clamping will be 
        /// applied (unless you override that function).
        /// </summary>
        public Vector2 MinPosition
        {
            get { return _minPosition; }
            set { _minPosition = value; }
        }
        /// <summary>
        /// the furthest down, and the furthest right the camera will go.
        /// if this value equals minPosition, then no clamping will be 
        /// applied (unless you override that function).
        /// </summary>
        public Vector2 MaxPosition
        {
            get { return _maxPosition; }
            set { _maxPosition = value; }
        }
        /// <summary>
        /// the body that this camera is currently tracking. 
        /// Null if not tracking any.
        /// </summary>
        public Body TrackingBody
        {
            get { return _trackingBody; }
            set { _trackingBody = value; }
        }
        /// <summary>
        /// a function that is called to determine if the user wants 
        /// to zoom in.
        /// </summary>
        public Func<Input, bool> ZoomIn
        {
            get { return _zoomIn; }
            set { _zoomIn = value; }
        }
        /// <summary>
        /// a function that is called to determine whether the user wants 
        /// to zoom out.
        /// </summary>
        public Func<Input, bool> ZoomOut
        {
            get { return _zoomOut; }
            set { _zoomOut = value; }
        }
        /// <summary>
        /// a function that determines whether clamping is currently enabled 
        /// for this camera.
        /// </summary>
        public Func<Camera, bool> ClampingEnabled
        {
            get { return _clampingEnabled; }
            set { _clampingEnabled = value; }
        }
        /// <summary>
        /// a function that is called to determine the amount of horizontal 
        /// movement that the user is requesting that the camera be moved 
        /// by.
        /// </summary>
        public Func<Input, Camera, float> HorizontalCameraMovement
        {
            get { return _horizontalCameraMovement; }
            set { _horizontalCameraMovement = value; }
        }
        /// <summary>
        /// a function that is called to determine the amount of vertical 
        /// movement that the user is requesting that the camera be moved 
        /// by.
        /// </summary>
        public Func<Input, Camera, float> VerticalCameraMovement
        {
            get { return _verticalCameraMovement; }
            set { _verticalCameraMovement = value; }
        }
        /// <summary>
        /// a function that is called to determine if the user wants to 
        /// rotate the camera left.
        /// </summary>
        public Func<Input, bool> RotateLeft
        {
            get { return _rotateLeft; }
            set { _rotateLeft = value; }
        }
        /// <summary>
        /// a function that is called to determine if the user wants to rotate 
        /// the camera right.
        /// </summary>
        public Func<Input, bool> RotateRight
        {
            get { return _rotateRight; }
            set { _rotateRight = value; }
        }
        /// <summary>
        /// A function that is called to determine if the user is requesting 
        /// that the camera be reset to it's original parameters.
        /// </summary>
        public Func<Input, bool> ResetCamera
        {
            get { return _resetCamera; }
            set { _resetCamera = value; }
        }


        /// <summary>
        /// Moves the camera forward one timestep.
        /// </summary>
        /// <param name="input">
        /// the an InputHelper input representing the current 
        /// input state.
        /// </param>
        public void Update(Input input)
        {
            if (!_transitioning)
            {
                if (_trackingBody == null)
                {
                    if (_clampingEnabled(this))
                        _targetPosition = Vector2.Clamp(_position + new Vector2(
                                _horizontalCameraMovement(input, this),
                                _verticalCameraMovement(input, this)),
                                _minPosition,
                                _maxPosition);
                    else
                        _targetPosition += new Vector2(
                            _horizontalCameraMovement(input, this),
                            _verticalCameraMovement(input, this));
                }
                else
                {
                    if (_clampingEnabled(this))
                        _targetPosition = Vector2.Clamp(
                            _trackingBody.Position,
                            _minPosition,
                            _maxPosition);
                    else
                        _targetPosition = _trackingBody.Position * 100.0f;
                }
                if (_zoomIn(input))
                    _targetZoom = Math.Min(_maxZoom, _zoom + _zoomRate);
                if (_zoomOut(input))
                    _targetZoom = Math.Max(_minZoom, _zoom - _zoomRate);
                //these might need to be swapped
                if (_rotateLeft(input))
                    _targetRotation = (_rotation + _rotationRate) % (float)(Math.PI * 2);
                if (_rotateRight(input))
                    _targetRotation = (_rotation - _rotationRate) % (float)(Math.PI * 2);
                if (input.IsCurPress(Buttons.RightStick))
                {
                    _transitioning = true;
                    _targetPosition = _origPosition;
                    _targetRotation = _origRotation;
                    _targetZoom = _origZoom;
                    _trackingBody = null;
                }
            }
            else if (_transition < 1)
            {
                _transition += _transitionSpeed;
            }
            if (_transition >= 1f ||
                (_position == _origPosition &&
                _rotation == _origRotation &&
                _zoom == _origZoom))
            {
                _transition = 0;
                _transitioning = false;
            }
            _position = Vector2.SmoothStep(_position, _targetPosition, _smoothingSpeed);
            _rotation = MathHelper.SmoothStep(_rotation, _targetRotation, _smoothingSpeed);
            _zoom = MathHelper.SmoothStep(_zoom, _targetZoom, _smoothingSpeed);
        }

        /// <summary>
        /// tells you if drawing the texture will actually draw onscreen.
        /// </summary>
        /// <param name="tex">
        /// the texture to check.
        /// </param>
        /// <param name="position">
        /// the position of the texture's center.
        /// </param>
        /// <param name="origin">
        /// a Vector2 equaling half of the texture's size.
        /// </param>
        /// <param name="rotation">
        /// the rotation of the texture, in radians.
        /// </param>
        /// <returns>
        /// a bool indicating whether you should draw this texture.
        /// </returns>
        public bool ShouldDraw(Texture2D tex, Vector2 position, Vector2 origin, float rotation)
        {
            Matrix textureMatrix = Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(position, 0));
            Vector2 topLeft = Vector2.Transform(-origin, textureMatrix);
            Vector2 topRight = Vector2.Transform(new Vector2(origin.X, -origin.Y), textureMatrix);
            Vector2 botLeft = Vector2.Transform(new Vector2(-origin.X, origin.Y), textureMatrix);
            Vector2 botRight = Vector2.Transform(origin, textureMatrix);
            AABB texAABB = new AABB();
            if (rotation >= 0 && rotation <= MathHelper.PiOver2)
            {
                Vector2 left = new Vector2(botLeft.X, topLeft.Y);
                Vector2 right = new Vector2(topRight.X, botRight.Y);
                texAABB = new AABB(ref left, ref right);
            }
            else if (rotation > MathHelper.PiOver2 && rotation <= MathHelper.Pi)
            {
                Vector2 bottom = new Vector2(botRight.X, botLeft.Y);
                Vector2 top = new Vector2(topLeft.X, topRight.Y);
                texAABB = new AABB(ref bottom, ref top);
            }
            else if (rotation > MathHelper.Pi && rotation <= (MathHelper.Pi + MathHelper.PiOver2))
            {
                Vector2 right = new Vector2(botRight.X, topRight.Y);
                Vector2 left = new Vector2(botLeft.X, topLeft.Y);
                texAABB = new AABB(ref right, ref left);
            }
            else if (rotation > (MathHelper.Pi + MathHelper.PiOver2) && rotation <= MathHelper.TwoPi)
            {
                Vector2 top = new Vector2(topLeft.X, topRight.Y);
                Vector2 bottom = new Vector2(botRight.X, botLeft.Y);
                texAABB = new AABB(ref top, ref bottom);
            }


            Matrix simpleCameraMatrix = Matrix.Identity *
                Matrix.CreateTranslation(new Vector3(-_position, 0)) *
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateTranslation(new Vector3(-_size / 2, 0));
            Vector2 camOrigin = (_size * (1 / _zoom));
            topLeft = Vector2.Transform(-camOrigin, simpleCameraMatrix);
            topRight = Vector2.Transform(new Vector2(camOrigin.X, -camOrigin.Y), simpleCameraMatrix);
            botLeft = Vector2.Transform(new Vector2(-camOrigin.X, camOrigin.Y), simpleCameraMatrix);
            botRight = Vector2.Transform(camOrigin, simpleCameraMatrix);
            AABB camAABB = new AABB();
            if (rotation >= 0 && rotation <= MathHelper.PiOver2)
            {
                Vector2 left = new Vector2(botLeft.X, topLeft.Y);
                Vector2 right = new Vector2(topRight.X, botRight.Y);
                camAABB = new AABB(ref left, ref right);
            }
            else if (rotation > MathHelper.PiOver2 && rotation <= MathHelper.Pi)
            {
                Vector2 bottom = new Vector2(botRight.X, botLeft.Y);
                Vector2 top = new Vector2(topLeft.X, topRight.Y);
                camAABB = new AABB(ref bottom, ref top);
            }
            else if (rotation > MathHelper.Pi && rotation <= (MathHelper.Pi + MathHelper.PiOver2))
            {
                Vector2 right = new Vector2(botRight.X, topRight.Y);
                Vector2 left = new Vector2(botLeft.X, topLeft.Y);
                camAABB = new AABB(ref right, ref left);
            }
            else if (rotation > (MathHelper.Pi + MathHelper.PiOver2) && rotation <= MathHelper.TwoPi)
            {
                Vector2 top = new Vector2(topLeft.X, topRight.Y);
                Vector2 bottom = new Vector2(botRight.X, botLeft.Y);
                camAABB = new AABB(ref top, ref bottom);
            }

            if (camAABB.Contains(ref texAABB.LowerBound) || camAABB.Contains(ref texAABB.UpperBound))
                return true;
            return false;
        }

        /// <summary>
        /// Given a position in screen coordiantes, will convert it to world coordinates.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToWorldLocation(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(CameraMatrix));
        }
    }
}