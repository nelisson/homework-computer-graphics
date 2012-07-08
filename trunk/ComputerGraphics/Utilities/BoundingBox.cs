namespace Utilities
{
    using OpenTK;

    public class BoundingBox
    {
        public float MaxX { get; set; }
        public float MinX { get; set; }

        public float MaxY { get; set; }
        public float MinY { get; set; }

        public float MaxZ { get; set; }
        public float MinZ { get; set; }

        public Vector3 Center
        {
            get { return new Vector3((MaxX + MinX)/2, (MaxY + MinY)/2, (MaxZ + MinZ)/2); }
        }

        public float SizeX
        {
            get { return MaxX - MinX; }
        }

        public float SizeY
        {
            get { return MaxY - MinY; }
        }

        public float SizeZ
        {
            get { return MaxZ - MinZ; }
        }
    }
}