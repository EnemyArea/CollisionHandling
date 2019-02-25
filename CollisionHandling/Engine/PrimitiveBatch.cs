#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    ///     https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/DebugViews/MonoGame/PrimitiveBatch.cs
    /// </summary>
    public class PrimitiveBatch : IDisposable
    {
        private const int DefaultBufferSize = 500;

        // a basic effect, which contains the shaders that we will use to draw our
        // primitives.
        private readonly BasicEffect basicEffect;

        // the device that we will issue draw calls to.
        private readonly GraphicsDevice device;

        private readonly VertexPositionColor[] lineVertices;
        private readonly VertexPositionColor[] triangleVertices;

        // hasBegun is flipped to true once Begin is called, and is used to make
        // sure users don't call End before Begin is called.
        private bool hasBegun;

        private bool isDisposed;
        private int lineVertsCount;
        private int triangleVertsCount;

        public PrimitiveBatch(GraphicsDevice graphicsDevice, int bufferSize = DefaultBufferSize)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            this.device = graphicsDevice;

            this.triangleVertices = new VertexPositionColor[bufferSize - bufferSize % 3];
            this.lineVertices = new VertexPositionColor[bufferSize - bufferSize % 2];

            // set up a new basic effect, and enable vertex colors.
            this.basicEffect = new BasicEffect(graphicsDevice);
            this.basicEffect.VertexColorEnabled = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void SetProjection(ref Matrix projection)
        {
            this.basicEffect.Projection = projection;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.isDisposed)
            {
                if (this.basicEffect != null)
                    this.basicEffect.Dispose();

                this.isDisposed = true;
            }
        }

        /// <summary>
        ///     Begin is called to tell the PrimitiveBatch what kind of primitives will be
        ///     drawn, and to prepare the graphics card to render those primitives.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="view">The view.</param>
        public void Begin(ref Matrix projection, ref Matrix view)
        {
            if (this.hasBegun)
                throw new InvalidOperationException("End must be called before Begin can be called again.");

            //tell our basic effect to begin.
            this.basicEffect.Projection = projection;
            this.basicEffect.View = view;
            this.basicEffect.CurrentTechnique.Passes[0].Apply();

            // flip the error checking boolean. It's now ok to call AddVertex, Flush,
            // and End.
            this.hasBegun = true;
        }

        public bool IsReady()
        {
            return this.hasBegun;
        }

        public void AddVertex(Vector2 vertex, Color color, PrimitiveType primitiveType)
        {
            if (!this.hasBegun)
                throw new InvalidOperationException("Begin must be called before AddVertex can be called.");

            if (primitiveType == PrimitiveType.LineStrip || primitiveType == PrimitiveType.TriangleStrip)
                throw new NotSupportedException("The specified primitiveType is not supported by PrimitiveBatch.");

            if (primitiveType == PrimitiveType.TriangleList)
            {
                if (this.triangleVertsCount >= this.triangleVertices.Length)
                    this.FlushTriangles();

                this.triangleVertices[this.triangleVertsCount].Position = new Vector3(vertex, -0.1f);
                this.triangleVertices[this.triangleVertsCount].Color = color;
                this.triangleVertsCount++;
            }

            if (primitiveType == PrimitiveType.LineList)
            {
                if (this.lineVertsCount >= this.lineVertices.Length)
                    this.FlushLines();

                this.lineVertices[this.lineVertsCount].Position = new Vector3(vertex, 0f);
                this.lineVertices[this.lineVertsCount].Color = color;
                this.lineVertsCount++;
            }
        }

        /// <summary>
        ///     End is called once all the primitives have been drawn using AddVertex.
        ///     it will call Flush to actually submit the draw call to the graphics card, and
        ///     then tell the basic effect to end.
        /// </summary>
        public void End()
        {
            if (!this.hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before End can be called.");
            }

            // Draw whatever the user wanted us to draw
            this.FlushTriangles();
            this.FlushLines();

            this.hasBegun = false;
        }

        private void FlushTriangles()
        {
            if (!this.hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            }

            if (this.triangleVertsCount >= 3)
            {
                int primitiveCount = this.triangleVertsCount / 3;

                // submit the draw call to the graphics card
                this.device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                this.device.DrawUserPrimitives(PrimitiveType.TriangleList, this.triangleVertices, 0, primitiveCount);
                this.triangleVertsCount -= primitiveCount * 3;
            }
        }

        private void FlushLines()
        {
            if (!this.hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            }

            if (this.lineVertsCount >= 2)
            {
                int primitiveCount = this.lineVertsCount / 2;

                // submit the draw call to the graphics card
                this.device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                this.device.DrawUserPrimitives(PrimitiveType.LineList, this.lineVertices, 0, primitiveCount);
                this.lineVertsCount -= primitiveCount * 2;
            }
        }
    }
}