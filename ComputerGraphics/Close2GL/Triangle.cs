namespace Close2GL
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class Triangle : Utilities.Triangle
    {
        public new Vertex V0 { get; set; }
        public new Vertex V1 { get; set; }
        public new Vertex V2 { get; set; }
        public new Vector4 FaceNormal { get; set; }

        public IEnumerable<Vertex> Vertices
        {
            get { return new List<Vertex> {V0, V1, V2}; }
        }

        public bool IsClipped()
        {
            return
                Vertices.Any(
                    vertex => vertex.Position.X > vertex.Position.W || vertex.Position.X < -vertex.Position.W ||
                              vertex.Position.Y > vertex.Position.W || vertex.Position.Y < -vertex.Position.W ||
                              vertex.Position.Z > vertex.Position.W || vertex.Position.Z < -vertex.Position.W);
        }

        public bool IsCulled(FrontFaceDirection ffd)
        {
            var v31 = V2.Position - V0.Position;
            var v32 = V2.Position - V1.Position;

            var prod = Vector3.Cross(v31.Xyz, v32.Xyz);
            var isCulled = false;

            switch (ffd)
            {
                case FrontFaceDirection.Cw:
                    isCulled = Vector3.Dot(prod, V0.Position.Xyz) >= 0;
                    break;
                case FrontFaceDirection.Ccw:
                    isCulled = Vector3.Dot(prod, V0.Position.Xyz) < 0;
                    break;
            }

            return isCulled;
        }
    }
}