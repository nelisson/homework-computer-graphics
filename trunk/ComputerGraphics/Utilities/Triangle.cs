namespace Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTK;

    public class Triangle
    {
        public Vertex V0 { get; set; }
        public Vertex V1 { get; set; }
        public Vertex V2 { get; set; }
        public Vector3 FaceNormal { get; set; }

        public float MaxX
        {
            get { return new List<float> {V0.Position.X, V1.Position.X, V2.Position.X}.Max(); }
        }

        public float MinX
        {
            get { return new List<float> {V0.Position.X, V1.Position.X, V2.Position.X}.Min(); }
        }

        public float MaxY
        {
            get { return new List<float> {V0.Position.Y, V1.Position.Y, V2.Position.Y}.Max(); }
        }

        public float MinY
        {
            get { return new List<float> {V0.Position.Y, V1.Position.Y, V2.Position.Y}.Min(); }
        }

        public float MaxZ
        {
            get { return new List<float> {V0.Position.Z, V1.Position.Z, V2.Position.Z}.Max(); }
        }

        public float MinZ
        {
            get { return new List<float> {V0.Position.Z, V1.Position.Z, V2.Position.Z}.Min(); }
        }

        public new string ToString()
        {
            return string.Format("v0 {1}{0}v1 {2}{0}v2 {3}{0}face normal {4} {5} {6}", Environment.NewLine,
                                 V0.ToString(), V1.ToString(), V2.ToString(), FaceNormal.X, FaceNormal.Y, FaceNormal.Z);
        }
    }
}