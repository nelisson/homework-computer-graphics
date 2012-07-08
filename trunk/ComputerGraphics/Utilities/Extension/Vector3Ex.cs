namespace Utilities.Extension
{
    using System.Drawing;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public static class Vector3Ex
    {
        public static Vector3 Normalized(this Vector3 v)
        {
            var v2 = new Vector3(v);
            v2.Normalize();
            return v2;
        }

        public static void DrawFromOrigin(this Vector3 v, Color color)
        {
            GL.Begin(BeginMode.Lines);

            GL.Color3(color);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(v);

            GL.End();
        }

        public static void DrawBetween(Vector3 v1, Vector3 v2, Color color)
        {
            GL.Begin(BeginMode.Lines);

            GL.Color3(color);
            GL.Vertex3(v1);
            GL.Vertex3(v2);

            GL.End();
        }

        public static void DrawPoint(this Vector3 v, Color color)
        {
            GL.PointSize(5);

            GL.Begin(BeginMode.Points);

            GL.Color3(color);
            GL.Vertex3(v);

            GL.End();

            GL.PointSize(1);
        }
    }
}