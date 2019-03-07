#region

using System;
using CollisionFloatTestNewMono.Engine.Math2;
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


        /// <summary>
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="bufferSize"></param>
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


        /// <summary>
        /// </summary>
        /// <param name="projection"></param>
        public void SetProjection(ref Matrix projection)
        {
            this.basicEffect.Projection = projection;
        }


        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
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
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool IsReady()
        {
            return this.hasBegun;
        }


        /// <summary>
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="color"></param>
        /// <param name="primitiveType"></param>
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
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="color"></param>
        public void DrawPolygon(Vector2[] vertices, Vector2 position, float angle, Color color)
        {
            var roation = new Rotation(angle);
            
            var vertexCount = vertices.Length;
            var origin = Vector2.Zero;
            for (var i = 0; i < vertexCount; ++i)
                origin += vertices[i];

            origin *= 1.0f / vertices.Length;

            var tempVertices = new Vector2[vertexCount];
            for (var i = 0; i < vertexCount; ++i)
                tempVertices[i] = position + MathUtils.Rotate(vertices[i], origin, roation);

            this.DrawPolygon(tempVertices, color);
        }


        /// <summary>
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="color"></param>
        /// <param name="closed"></param>
        public void DrawPolygon(Vector2[] vertices, Color color, bool closed = true)
        {
            if (!this.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            var count = vertices.Length;
            for (var i = 0; i < count - 1; i++)
            {
                this.AddVertex(vertices[i], color, PrimitiveType.LineList);
                this.AddVertex(vertices[i + 1], color, PrimitiveType.LineList);
            }

            if (closed)
            {
                this.AddVertex(vertices[count - 1], color, PrimitiveType.LineList);
                this.AddVertex(vertices[0], color, PrimitiveType.LineList);
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public void DrawCircle(Vector2 center, float radius, Color color)
        {
            if (!this.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            const double increment = Math.PI * 2.0 / 32;
            var theta = 0.0;

            for (var i = 0; i < 32; i++)
            {
                var v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                var v2 = center + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                this.AddVertex(v1, color, PrimitiveType.LineList);
                this.AddVertex(v2, color, PrimitiveType.LineList);

                theta += increment;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        public void DrawSegment(Vector2 start, Vector2 end, Color color)
        {
            if (!this.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            this.AddVertex(start, color, PrimitiveType.LineList);
            this.AddVertex(end, color, PrimitiveType.LineList);
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


        /// <summary>
        /// </summary>
        private void FlushTriangles()
        {
            if (!this.hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            }

            if (this.triangleVertsCount >= 3)
            {
                var primitiveCount = this.triangleVertsCount / 3;

                // submit the draw call to the graphics card
                this.device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                this.device.DrawUserPrimitives(PrimitiveType.TriangleList, this.triangleVertices, 0, primitiveCount);
                this.triangleVertsCount -= primitiveCount * 3;
            }
        }


        /// <summary>
        /// </summary>
        private void FlushLines()
        {
            if (!this.hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            }

            if (this.lineVertsCount >= 2)
            {
                var primitiveCount = this.lineVertsCount / 2;

                // submit the draw call to the graphics card
                this.device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                this.device.DrawUserPrimitives(PrimitiveType.LineList, this.lineVertices, 0, primitiveCount);
                this.lineVertsCount -= primitiveCount * 2;
            }
        }
    }
}