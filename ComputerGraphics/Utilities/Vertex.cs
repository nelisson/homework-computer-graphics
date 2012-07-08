namespace Utilities
{
    using OpenTK;

    public class Vertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public int ColorIndex { get; set; }

        public new string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}", Position.X, Position.Y, Position.Z, Normal.X, Normal.Y,
                                 Normal.Z, ColorIndex);
        }
    }
}