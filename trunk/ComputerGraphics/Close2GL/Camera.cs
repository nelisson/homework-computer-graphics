namespace Close2GL
{
    using Enumerations;
    using OpenTK;

    public class Camera
    {
        public float FovX, FovY;
        public float ZFar, ZNear;

        public Vector3 U, V, N;
        public Vector3 Up, Center, Eye;


        public Camera()
        {
            ZNear = 1;
            ZFar = 3000;
            FovX = 60;
            FovY = 60;
            View = Matrix4.Identity;
        }

        public Matrix4 View { get; set; }

        public void LookAt(Vector3 eye, Vector3 center, Vector3 up)
        {
            LookAt(eye.X, eye.Y, eye.Z, center.X, center.Y, center.Z, up.X, up.Y, up.Z);
        }

        public void TranslateCenter(Vector3 t)
        {
            TranslateCenter(t.X, t.Y, t.Z);
        }

        public void Translate(Vector3 t)
        {
            Translate(t.X, t.Y, t.Z);
        }

        private void BuildAxis(float eyeX, float eyeY, float eyeZ,
                               float centerX, float centerY, float centerZ,
                               float upX, float upY, float upZ)
        {
            N = new Vector3(eyeX - centerX, eyeY - centerY, eyeZ - centerZ);
            N.Normalize();

            U = Vector3.Cross(new Vector3(upX, upY, upZ), N);
            V = Vector3.Cross(N, U);

            Eye = new Vector3(eyeX, eyeY, eyeZ);
            Up = new Vector3(upX, upY, upZ);
            Center = new Vector3(centerX, centerY, centerZ);
        }

        private void LookAt(float eyeX, float eyeY, float eyeZ,
                            float centerX, float centerY, float centerZ,
                            float upX, float upY, float upZ)
        {
            BuildAxis(eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ);
            BuildMatrix();
        }

        private void TranslateCenter(float x, float y, float z)
        {
            LookAt(Eye.X + x, Eye.Y + y, Eye.Z + z,
                   Center.X, Center.Y, Center.Z,
                   Up.X, Up.Y, Up.Z);
        }

        private void Translate(float x, float y, float z)
        {
            LookAt(Eye.X + x, Eye.Y + y, Eye.Z + z,
                   Center.X + x, Center.Y + y, Center.Z + z,
                   Up.X, Up.Y, Up.Z);
        }


        public void Rotate(float theta, CameraAxes transformAxis)
        {
            Matrix4 m;

            switch (transformAxis)
            {
                case CameraAxes.N:
                    m = Matrix4.CreateFromAxisAngle(N, theta);
                    U = Vector3.Transform(U, m);
                    V = Vector3.Transform(V, m);
                    break;
                case CameraAxes.U:
                    m = Matrix4.CreateFromAxisAngle(U, theta);
                    U = Vector3.Transform(N, m);
                    V = Vector3.Transform(V, m);
                    break;
                case CameraAxes.V:
                    m = Matrix4.CreateFromAxisAngle(V, theta);
                    U = Vector3.Transform(U, m);
                    V = Vector3.Transform(N, m);
                    break;
            }

            LookAt(Eye, Center - N, V);
        }

        public void BuildMatrix()
        {
            View = Matrix4.Identity;

            View = new Matrix4(new Vector4(U, Vector3.Dot(-Eye, U)),
                               new Vector4(V, Vector3.Dot(-Eye, V)),
                               new Vector4(N, Vector3.Dot(-Eye, N)),
                               new Vector4(Vector3.Zero, 1));
        }
    }
}