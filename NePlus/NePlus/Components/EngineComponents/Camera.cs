﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NePlus;

public class Camera2D : Component
{
    #region Properties and Fields
    #region Position

    protected Vector2 position = Vector2.Zero;
    public Vector2 Position
    {
        get { return position; }
        set
        {
            position = value;

            visibleArea = new Rectangle((int)position.X + (int)offset.X - visibleArea.Width / 2,
                              (int)position.Y + (int)offset.Y - visibleArea.Height / 2, visibleArea.Width, visibleArea.Height);
        }
    }

    protected Vector2 offset = Vector2.Zero;
    public Vector2 Offset
    {
        get { return offset; }
        set
        {
            offset = value;
            visibleArea = new Rectangle((int)position.X + (int)offset.X - visibleArea.Width / 2,
                          (int)position.Y + (int)offset.Y - visibleArea.Height / 2, visibleArea.Width, visibleArea.Height);
        }
    }
    #endregion Position

    #region Culling
    protected Rectangle visibleArea;
    public Rectangle VisibleArea
    {
        get { return visibleArea; }
    }

    public int ViewingWidth
    {
        get { return visibleArea.Width; }
        set { visibleArea.Width = value; }
    }

    public int ViewingHeight
    {
        get { return visibleArea.Height; }
        set { visibleArea.Height = value; }
    }
    #endregion Culling

    #region Transformations
    protected float rotation = 0.0f;
    public float Rotation
    {
        get { return rotation; }
        set { rotation = value; }
    }

    // <0 - <1 = Zoom Out
    // >1 = Zoom In
    // <0 = Funky (flips axis)
    protected float zoom = 1.0f;
    public float Zoom
    {
        get { return zoom; }
        set { zoom = value; }
    }
    #endregion Transformations

    public Vector2 ScreenPosition
    {
        get { return new Vector2(Engine.Video.GraphicsDevice.Viewport.Width / 2, Engine.Video.GraphicsDevice.Viewport.Height / 2); }
    }

    // Returns a transformation matrix based on the camera's position, rotation, and zoom.
    // Best used as a parameter for the SpriteBatch.Begin() call.
    public Matrix CameraMatrix
    {
        get
        {
            Vector3 matrixRotOrigin = new Vector3(Position + Offset, 0);
            Vector3 matrixScreenPos = new Vector3(ScreenPosition, 0.0f);

            // Translate back to the origin based on the camera's offset position, since we're rotating around the camera
            // Then, we scale and rotate around the origin
            // Finally, we translate to SCREEN coordinates, so translation is based on the ScreenCenter
            return Matrix.CreateTranslation(-matrixRotOrigin) *
                Matrix.CreateScale(zoom, zoom, 1.0f) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(matrixScreenPos);
        }
    }

    public Matrix ProjectionMatrix
    {
        get
        {
            return Matrix.CreateOrthographic(zoom * Engine.Video.GraphicsDevice.Viewport.AspectRatio, zoom, 0, 1);
        }
    }

    public Matrix TransformMatrix
    {
        get
        {
            return ViewMatrix
                 * Matrix.CreateScale(Engine.Video.GraphicsDevice.Viewport.Height / zoom)
                 * Matrix.CreateScale(1, -1, 1)
                 * Matrix.CreateTranslation(Engine.Video.GraphicsDevice.Viewport.Width * 0.5f, Engine.Video.GraphicsDevice.Viewport.Height * 0.5f, 0f);
        }
    }

    public Matrix ViewMatrix
    {
        get
        {
            return Matrix.CreateTranslation(-position.X, -position.Y, 0)
                 * Matrix.CreateRotationZ(rotation);
        }
    }
    #endregion Properties and Fields

    #region Constructors
    public Camera2D(Engine engine)
        : base(engine)
    {
        visibleArea = new Rectangle(0, 0, Engine.Video.GraphicsDevice.Viewport.Width, Engine.Video.GraphicsDevice.Viewport.Height);
        position = ScreenPosition;
    }
    public Camera2D(Engine engine, int width, int height)
        : base(engine)
    {
        visibleArea = new Rectangle(0, 0, width, height);
        position = ScreenPosition;
    }
    public Camera2D(Engine engine, int x, int y, int width, int height)
        : base(engine)
    {
        visibleArea = new Rectangle(x - (width / 2), y - (height / 2), width, height);
        position.X = x;
        position.Y = y;
    }
    #endregion Constructors

    #region Destructors
    public override void Dispose(bool disposing)
    {
        visibleArea = Rectangle.Empty;
    }
    #endregion
}