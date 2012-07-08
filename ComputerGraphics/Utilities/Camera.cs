namespace Utilities
{
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class Camera
    {
        private GLControl opengl;
        private const int Escala = 40;
        private float aspect;
        private Vector3 eye;
        private float fovy;
        private Vector3 look;
        private Vector3 n;
        private Vector3 u;
        private Vector3 up;
        private Vector3 v;
        private float zFar;
        private float zNear;

        public Camera(GLControl control)
        {
            opengl = control;
            u.X = 1;
            u.Y = 0;
            u.Z = 0;
            v.X = 0;
            v.Y = 1;
            v.Z = 0;
            n.X = 0;
            n.Y = 0;
            n.Z = 1;
            up.X = 0;
            up.Y = 1;
            up.Z = 0;
            eye = Vector3.Zero;
        }

        public Vector3 Eye
        {
            get { return eye; }
        }

        private void SetModelViewMatrix()
        {
            var m = new float[16];

            var eVec = new Vector3(eye);
            m[0] = u.X;
            m[4] = u.Y;
            m[8] = u.Z;
            m[12] = -Vector3.Dot(eVec, u);
            m[1] = v.X;
            m[5] = v.Y;
            m[9] = v.Z;
            m[13] = -Vector3.Dot(eVec, v);
            m[2] = n.X;
            m[6] = n.Y;
            m[10] = n.Z;
            m[14] = -Vector3.Dot(eVec, n);
            m[3] = 0;
            m[7] = 0;
            m[11] = 0;
            m[15] = 1;
            opengl.MakeCurrent();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(m);
        }

        public void Set(Vector3 _eye, Vector3 _look, Vector3 _up)
        {
            eye = _eye;
            look = _look;
            up = _up;
            n = eye - look;
            u = Vector3.Cross(up, n);
            n.Normalize();
            u.Normalize();
            v = Vector3.Cross(n, u);
            up = v;
            SetModelViewMatrix();
        }

        public void Set(Vector3 _look)
        {
            look = _look;
            n = eye - look;
            u = Vector3.Cross(up, n);
            n.Normalize();
            u.Normalize();
            v = Vector3.Cross(n, u);
            up = v;
            SetModelViewMatrix();
        }

        public void Slide(float dU, float dV, float dN)
        {
            var xVector = new Vector3(u.X, v.X, n.X);
            var yVector = new Vector3(u.Y, v.Y, n.Y);
            var zVector = new Vector3(u.Z, v.Z, n.Z);
            var dVector = new Vector3(dU, dV, dN);

            eye.X += Vector3.Dot(dVector, xVector);
            eye.Y += Vector3.Dot(dVector, yVector);
            eye.Z += Vector3.Dot(dVector, zVector);
            look.X += Vector3.Dot(dVector, xVector);
            look.Y += Vector3.Dot(dVector, yVector);
            look.Z += Vector3.Dot(dVector, zVector);
            SetModelViewMatrix();
        }

        public void SlideFixed(float dU, float dV, float dN)
        {
            var xVector = new Vector3(u.X, v.X, n.X);
            var yVector = new Vector3(u.Y, v.Y, n.Y);
            var zVector = new Vector3(u.Z, v.Z, n.Z);
            var dVector = new Vector3(dU, dV, dN);

            eye.X += Vector3.Dot(dVector, xVector);
            eye.Y += Vector3.Dot(dVector, yVector);
            eye.Z += Vector3.Dot(dVector, zVector);
            Set(eye, look, up);
            SetModelViewMatrix();
        }

        /**
         * roll é a rotação com o eixo n(z) fixo
         */

        public void Roll(float angle)
        {
            var mat = Matrix4.CreateFromAxisAngle(n, angle);

            v = Vector3.Transform(v, mat);
            u = Vector3.Transform(u, mat);
            SetModelViewMatrix();
        }

        /**
         * pitch é a rotação com o eixo u(x) fixo
         */

        public void Pitch(float angle)
        {
            var mat = Matrix4.CreateFromAxisAngle(u, angle);

            n = Vector3.Transform(n, mat);
            v = Vector3.Transform(v, mat);
            SetModelViewMatrix();
        }

        /**
         * yaw é a rotação com o eixo v(y) fixo
         */

        public void Yaw(float angle)
        {
            var mat = Matrix4.CreateFromAxisAngle(v, angle);

            n = Vector3.Transform(n, mat);
            u = Vector3.Transform(u, mat);
            SetModelViewMatrix();
        }

        public void SetShape(float vAng, float asp, float nearPlane, float farPlane)
        {
            fovy = vAng;
            aspect = asp;
            zNear = nearPlane;
            zFar = farPlane;
            opengl.MakeCurrent();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            var mat = Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar);

            GL.LoadMatrix(ref mat);
            SetModelViewMatrix();
        }

        /**
         * retorna verdadeiro se a translação de z não ultrapassará look e false se vai ultrapassar
         */

        public bool TranslateN()
        {
            var maior = eye.Z > look.Z;
            var tempEyez = eye.Z + (Escala*n.Z);
            var maior2 = tempEyez > look.Z;
            return (!maior && !maior2) || (maior && maior2);
        }
    }
}