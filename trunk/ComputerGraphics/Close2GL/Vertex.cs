namespace Close2GL
{
    using OpenTK;

    public class Vertex : Utilities.Vertex
    {
        private Vector4 _normal;
        private Vector4 _position;

        public new Vector4 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public new Vector4 Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }
    }
}