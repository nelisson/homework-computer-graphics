namespace Utilities
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using Exceptions;
    using OpenTK.Graphics.OpenGL;

    public class Mesh
    {
        public string Name { get; set; }
        public int TrianglesNumber { get; set; }
        public int MaterialCount { get; set; }
        public List<Material> Materials { get; set; }
        public List<Triangle> Triangles { get; set; }
        public List<Color> Colors { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public int MaterialIndex { get; set; }

        public void DrawMesh()
        {
            GL.PushMatrix();

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, Materials[MaterialIndex].Ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Materials[MaterialIndex].Diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, Materials[MaterialIndex].Specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, Materials[MaterialIndex].Shininess);

            GL.Begin(BeginMode.Triangles);

            foreach (Triangle triangle in Triangles)
            {
                GL.Color3(Colors[triangle.V0.ColorIndex]);
                GL.Normal3(triangle.V0.Normal);
                GL.Vertex3(triangle.V0.Position);

                GL.Color3(Colors[triangle.V1.ColorIndex]);
                GL.Normal3(triangle.V1.Normal);
                GL.Vertex3(triangle.V1.Position);

                GL.Color3(Colors[triangle.V2.ColorIndex]);
                GL.Normal3(triangle.V2.Normal);
                GL.Vertex3(triangle.V2.Position);
            }

            GL.End();

            GL.PopMatrix();
        }

        public void Load(StreamReader fileStream)
        {
            string file = fileStream.ReadToEnd();

            try
            {
                Name = MeshParser.ParseName(file);
                TrianglesNumber = MeshParser.ParseTrianglesNumber(file);
                MaterialCount = MeshParser.ParseMaterialCount(file);
                Materials = MeshParser.ParseMaterials(file);
                Triangles = MeshParser.ParseTriangules(file);
                Colors = new List<Color> {Color.White};
                BoundingBox = new BoundingBox
                                  {
                                      MaxX = Triangles.Max(t => t.MaxX),
                                      MinX = Triangles.Min(t => t.MinX),
                                      MaxY = Triangles.Max(t => t.MaxY),
                                      MinY = Triangles.Min(t => t.MinY),
                                      MaxZ = Triangles.Max(t => t.MaxZ),
                                      MinZ = Triangles.Min(t => t.MinZ),
                                  };
                MaterialIndex = 0;
            }
            catch (RegexException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}