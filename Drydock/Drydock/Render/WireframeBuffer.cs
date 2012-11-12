﻿#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class WireframeBuffer : BaseBufferObject<VertexPositionColor>{
        public WireframeBuffer(int numIndicies, int numVerticies, int numPrimitives) : base(numIndicies, numVerticies, numPrimitives, PrimitiveType.LineList){
            BufferRasterizer = new RasterizerState();
            BufferRasterizer.CullMode = CullMode.None;
            BufferEffect = Singleton.ContentManager.Load<Effect>("hlsl/WireframeEffect").Clone();

            BufferEffect.Parameters["Projection"].SetValue(Singleton.ProjectionMatrix);
            BufferEffect.Parameters["World"].SetValue(Matrix.Identity);
        }

        public new IndexBuffer Indexbuffer{
            get { return base.Indexbuffer; }
        }

        public new VertexBuffer Vertexbuffer{
            get { return base.Vertexbuffer; }
        }
    }

    /*internal struct Vertex3 : IVertexType{
        public Vector3 Pos;

        public VertexDeclaration VertexDeclaration{
            get { 
                return new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));
            }
        }

        public Vertex3(float x, float y, float z){
            Pos = new Vector3(x, y, z);
        }
    }*/
}