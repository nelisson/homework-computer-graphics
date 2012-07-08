namespace Utilities
{
    using OpenTK.Graphics;

    public class Material
    {
        public Color4 Ambient { get; set; }
        public Color4 Diffuse { get; set; }
        public Color4 Specular { get; set; }
        public float Shininess { get; set; }
    }
}