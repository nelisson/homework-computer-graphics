namespace Close2GL
{
    using System;
    using System.IO;
    using OpenTK;

    public static class Matrix
    {
        static Matrix()
        {
            Clear();
        }

        public static Matrix4 Model, View, Projection, Viewport;

        private static void Clear()
        {
            Model = Matrix4.Identity;
            View = Matrix4.Identity;
            Projection = Matrix4.Identity;
            Viewport = Matrix4.Identity;
        }

        public static void PerspectiveTransform(float fovy, float aspect, float zNear, float zFar)
        {
            var t = (float) Math.Tan(MathHelper.DegreesToRadians(fovy/2));
            var b = -t;
            var r = t*aspect;
            var l = -r;

            Projection = new Matrix4(new Vector4(2*zNear/(r - l), 0, (r + l)/(r - l), 0),
                                     new Vector4(0, 2*zNear/(t - b), (t + b)/(t - b), 0),
                                     new Vector4(0, 0, -(zFar + zNear)/(zFar - zNear), -(2*zNear*zFar)/(zFar - zNear)),
                                     new Vector4(0, 0, -1, 1)
                );
        }

        public static void ViewportTransform(int x, int y, int width, int height)
        {
            Viewport = new Matrix4(new Vector4(width/(float) 2, 0, 0, x + width/(float) 2),
                                   new Vector4(0, height/(float) 2, 0, y + height/(float) 2),
                                   new Vector4(0, 0, 1, 0),
                                   new Vector4(0, 0, 0, 1));
        }

        public static void Print()
        {
            using (var stream = new StreamWriter(@"D:\matrix.txt"))
            {
                stream.WriteLine("Model");

                stream.WriteLine(Model.Row0);
                stream.WriteLine(Model.Row1);
                stream.WriteLine(Model.Row2);
                stream.WriteLine(Model.Row3);

                stream.WriteLine("View");

                stream.WriteLine(View.Row0);
                stream.WriteLine(View.Row1);
                stream.WriteLine(View.Row2);
                stream.WriteLine(View.Row3);

                stream.WriteLine("Projection");

                stream.WriteLine(Projection.Row0);
                stream.WriteLine(Projection.Row1);
                stream.WriteLine(Projection.Row2);
                stream.WriteLine(Projection.Row3);

                stream.WriteLine("ViewPort");

                stream.WriteLine(Viewport.Row0);
                stream.WriteLine(Viewport.Row1);
                stream.WriteLine(Viewport.Row2);
                stream.WriteLine(Viewport.Row3);
            }
        }
    }
}