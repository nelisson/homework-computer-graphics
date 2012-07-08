namespace Utilities.Extension
{
    using OpenTK;

    public static class Vector4Ex
    {
        public static Vector4 Transform(this Vector4 v, Matrix4 m)
        {
            return new Vector4(Vector4.Dot(v, m.Row0), Vector4.Dot(v, m.Row1), Vector4.Dot(v, m.Row2),
                               Vector4.Dot(v, m.Row3));
        }
    }
}