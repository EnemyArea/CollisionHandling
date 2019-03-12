#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
    public sealed class Camera2D
    {
        /// <summary>
        ///     Beinhaltet die Matrix mit der gemalt wird
        /// </summary>
        private Matrix invertedViewMatrix;

        /// <summary>
        /// </summary>
        private Matrix offsetMatrix;

        /// <summary>
        /// </summary>
        private Vector2? focusPosition;

        /// <summary>
        /// </summary>
        private Vector3 positionWithOffset;

        /// <summary>
        /// </summary>
        private Vector3 positionOffset;

        /// <summary>
        /// </summary>
        private Rectangle limits;

        /// <summary>
        /// </summary>
        private Matrix scaleMatrix;

        /// <summary>
        /// </summary>
        private Matrix viewMatrix;

        /// <summary>
        /// </summary>
        private Matrix viewMatrixWithOffset;


        /// <summary>
        /// </summary>
        public Viewport Viewport { get; private set; }

        /// <summary>
        /// </summary>
        public Vector2 Origin { get; private set; }

        /// <summary>
        /// </summary>
        public float ZoomLevel { get; private set; }

        /// <summary>
        /// </summary>
        public Vector2 ViewOffset { get; private set; }

        /// <summary>
        /// </summary>
        public Vector3 CameraPosition => this.positionWithOffset - this.positionOffset;

        /// <summary>
        /// </summary>
        public Matrix ViewMatrixWithOffset => this.viewMatrixWithOffset;

        /// <summary>
        /// </summary>
        public Matrix DebugViewMatrix { get; private set; }


        /// <summary>
        /// </summary>
        public Camera2D()
        {
            this.SetZoomLevel(1);
            this.ViewOffset = Vector2.Zero;
            this.offsetMatrix = Matrix.Identity;
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        private Vector2 GetOffset()
        {
            var mapWidthInPixels = this.limits.Width;
            var mapHeightInPixels = this.limits.Height;
            var screenWidthInPixel = this.Viewport.Width;
            var screenHeightInPixel = this.Viewport.Height;

            return new Vector2(
                mapWidthInPixels < screenWidthInPixel ? this.Viewport.Width * 0.5f - mapWidthInPixels * 0.5f : 0,
                mapHeightInPixels < screenHeightInPixel ? this.Viewport.Height * 0.5f - mapHeightInPixels * 0.5f : 0);
        }


        /// <summary>
        /// </summary>
        /// <param name="viewport"></param>
        public void UpdateViewport(Viewport viewport)
        {
            this.Viewport = viewport;
            this.Origin = new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f);
            this.ViewOffset = this.GetOffset();
            this.offsetMatrix = Matrix.CreateTranslation(new Vector3(this.ViewOffset, 0));
        }


        /// <summary>
        /// </summary>
        /// <param name="zoomLevel"></param>
        public void SetZoomLevel(float zoomLevel)
        {
            // Zoomen!
            this.ZoomLevel = 1f * zoomLevel;

            // Scale Matrix
            this.scaleMatrix = Matrix.CreateScale(this.ZoomLevel, this.ZoomLevel, 1f);
        }


        /// <summary>
        /// </summary>
        /// <param name="origin"></param>
        public void SetOrigin(Vector2 origin)
        {
            this.Origin = origin;
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        public void SetFocusPosition(Vector2? position)
        {
            this.focusPosition = position;
        }


        /// <summary>
        /// </summary>
        private Vector2? GetFocusPosition()
        {
            return this.focusPosition;
        }


        /// <summary>
        /// </summary>
        /// <param name="mapWidth"></param>
        /// <param name="mapHeight"></param>
        public void ChangeMapSize(int mapWidth, int mapHeight)
        {
            // Updaten
            this.limits = new Rectangle(0, 0, mapWidth * GameHelper.TileSize, mapHeight * GameHelper.TileSize);
            this.ViewOffset = this.GetOffset();
            this.offsetMatrix = Matrix.CreateTranslation(new Vector3(this.ViewOffset, 0));
        }


        /// <summary>
        /// </summary>
        private void UpdateMatrix()
        {
            // Den Zielursprung berechnen. Dieser liegt normalerweise in der Mitte des Bildschirms
            // außer, wenn wir ein Ziel Focusieren.
            var targetOrigin = (this.GetFocusPosition() ?? this.Origin);
            var targetOriginVector = -new Vector3((int)Math.Round(targetOrigin.X), (int)Math.Round(targetOrigin.Y), 0f);

            // Matrix neuberechnen
            this.viewMatrix = Matrix.CreateTranslation(-this.positionWithOffset) *
                              Matrix.CreateTranslation(targetOriginVector) *
                              this.scaleMatrix *
                              Matrix.CreateTranslation(new Vector3(this.Origin, 0f));

            // Invertieren
            Matrix.Invert(ref this.viewMatrix, out this.invertedViewMatrix);

            // Multiplizieren
            Matrix.Multiply(ref this.viewMatrix, ref this.offsetMatrix, out this.viewMatrixWithOffset);
        }


        /// <summary>
        ///     When using limiting, makes sure the camera position is valid.
        /// </summary>
        private void UpdateCamera()
        {
            // Wir haben dann keinen Offset, weil dieser Zielabhängig berechnet wird
            this.positionWithOffset = new Vector3();

            // Matrix neuberechnen
            this.UpdateMatrix();

            // Camera Position validieren
            var cameraWorldMin = Vector3.Transform(Vector3.Zero, this.invertedViewMatrix);
            var cameraSize = new Vector3(this.Viewport.Width, this.Viewport.Height, 0) / this.ZoomLevel;
            var limitWorldMin = new Vector3(this.limits.Left, this.limits.Top, 0);
            var limitWorldMax = new Vector3(this.limits.Right, this.limits.Bottom, 0);

            // Offset speichern
            this.positionOffset = this.positionWithOffset - cameraWorldMin;

            // Neue Position bestimmen
            this.positionWithOffset = Vector3.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + this.positionOffset;

            // Matrix neuberechnen, wenn es z.B. korrekturen gab
            this.UpdateMatrix();
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // Updaten
            this.UpdateCamera();
        }
    }
}