namespace Close2GL
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using Utilities.Extension;

    public class Mesh : Utilities.Mesh
    {
        public Mesh()
        {
            TransformedTriangles = new List<Triangle>();
            UseClipping = UseCulling = false;
            FrontFaceDirection = FrontFaceDirection.Cw;
        }

        public List<Triangle> TransformedTriangles { get; set; }
        public bool UseCulling { get; set; }
        public bool UseClipping { get; set; }
        public FrontFaceDirection FrontFaceDirection { get; set; }

        private void TransformTriangles()
        {
            TransformedTriangles.Clear();

            foreach (var t in Triangles.Select(triangle => new Triangle
                                                               {
                                                                   V0 = new Vertex
                                                                            {
                                                                                Normal =
                                                                                    new Vector4(triangle.V0.Normal, 1),
                                                                                Position =
                                                                                    new Vector4(triangle.V0.Position, 1)
                                                                            },
                                                                   V1 = new Vertex
                                                                            {
                                                                                Normal =
                                                                                    new Vector4(triangle.V1.Normal, 1),
                                                                                Position =
                                                                                    new Vector4(triangle.V1.Position, 1)
                                                                            },
                                                                   V2 = new Vertex
                                                                            {
                                                                                Normal =
                                                                                    new Vector4(triangle.V2.Normal, 1),
                                                                                Position =
                                                                                    new Vector4(triangle.V2.Position, 1)
                                                                            },
                                                                   FaceNormal = new Vector4(triangle.FaceNormal, 1)
                                                               }))
            {
                foreach (var vertex in t.Vertices)
                {
                    vertex.Position = vertex.Position.Transform(Matrix.Model);
                    vertex.Position = vertex.Position.Transform(Matrix.View);
                    vertex.Position = vertex.Position.Transform(Matrix.Projection);
                }


                if (UseClipping && t.IsClipped())
                {
                    continue;
                }

                if (UseCulling && t.IsCulled(FrontFaceDirection))
                {
                    continue;
                }

                foreach (var vertex in t.Vertices)
                {
                    vertex.Position = vertex.Position.Transform(Matrix.Viewport);

                    var w = vertex.Position.W;

                    vertex.Position = new Vector4(vertex.Position.X/w, vertex.Position.Y/w, vertex.Position.Z,
                                                  vertex.Position.W);
                }

                TransformedTriangles.Add(t);
            }
        }

        public new void DrawMesh()
        {
            TransformTriangles();

            GL.Begin(BeginMode.Triangles);

            foreach (var triangle in TransformedTriangles)
            {
                GL.Color3(Colors[triangle.V0.ColorIndex]);
                GL.Vertex2(triangle.V0.Position.Xy);

                GL.Color3(Colors[triangle.V1.ColorIndex]);
                GL.Vertex2(triangle.V1.Position.Xy);

                GL.Color3(Colors[triangle.V2.ColorIndex]);
                GL.Vertex2(triangle.V2.Position.Xy);
            }
            GL.End();
        }

        private void Print()
        {
            using (var text = new StreamWriter(@"D:\Triangles.txt"))
            {
                foreach (var transformedTriangle in TransformedTriangles)
                {
                    foreach (var vertex in transformedTriangle.Vertices)
                    {
                        text.WriteLine(vertex.Position);
                    }
                }
                text.WriteLine("{0}", Environment.NewLine);
            }
        }
    }
}