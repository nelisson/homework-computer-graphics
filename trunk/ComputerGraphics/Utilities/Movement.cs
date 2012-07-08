namespace Utilities
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Extension;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class Movement
    {
        #region Privates

        private static readonly Vector3 InitialEye = new Vector3(10.0f, 10.0f, 10.0f);
        private static readonly Vector3 InitialLook = new Vector3(-1.0f, -1.0f, -1.0f);
        private static readonly Vector3 InitialRight = new Vector3(1.0f, 0.0f, 0.0f);
        private static readonly Vector3 InitialUp = new Vector3(0.0f, 1.0f, 0.0f);

        private readonly GLControl _glControl;
        private readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>();
        private Point _gPtCurrentMousePosit;
        private Point _gPtLastMousePosit;
        private float _gfMoveSpeed = 0.10f;
        private Vector3 _gvEye = new Vector3(InitialEye);
        private Vector3 _gvLook = new Vector3(InitialLook);
        private Vector3 _gvRight = new Vector3(InitialRight);
        private Vector3 _gvUp = new Vector3(InitialUp);
        private bool _mousing;
        private int _xDiff;
        private int _yDiff;
        private float _zfar = 100.0f;
        private float _znear = 0.1f;

        public float ZNear
        {
            get { return _znear; }
            set { _znear = value; }
        }

        public float ZFar
        {
            get { return _zfar; }
            set { _zfar = value; }
        }

        #endregion

        public Movement(GLControl gl)
        {
            _glControl = gl;
        }

        public void ProcessUserInput()
        {
            var tmpLook = new Vector3(_gvLook);
            var tmpRight = new Vector3(_gvRight);

            if (_mousing)
            {
                Matrix4 matRotation = Matrix4.Identity;

                if (_yDiff != 0)
                {
                    MatrixEx.Rotate(ref matRotation, -(float) _yDiff/3.0f, _gvRight);
                    _gvLook = Vector3.Transform(_gvLook, matRotation);
                    _gvUp = Vector3.Transform(_gvUp, matRotation);
                }

                if (_xDiff != 0)
                {
                    var tmpVector = new Vector3(0.0f, 1.0f, 0.0f);

                    MatrixEx.Rotate(ref matRotation, -(float) _xDiff/3.0f, tmpVector);
                    _gvLook = Vector3.Transform(_gvLook, matRotation);
                    _gvUp = Vector3.Transform(_gvUp, matRotation);
                }
            }

            bool validKey = false;

            // Left Arrow Key - View side-steps or strafes to the left
            if (_pressedKeys.Contains(Keys.A))
            {
                _gvEye -= (tmpRight*_gfMoveSpeed);
                validKey = true;
            }

            // Right Arrow Key - View side-steps or strafes to the right
            if (_pressedKeys.Contains(Keys.D))
            {
                _gvEye += (tmpRight*_gfMoveSpeed);
                validKey = true;
            }

            // UP ARROW - View moves forward
            if (_pressedKeys.Contains(Keys.W))
            {
                _gvEye -= (tmpLook*(-_gfMoveSpeed));
                validKey = true;
            }

            // DOWN ARROW - View moves backward
            if (_pressedKeys.Contains(Keys.S))
            {
                _gvEye += (tmpLook*(-_gfMoveSpeed));
                validKey = true;
            }

            // HOME KEY - View moves up
            if (_pressedKeys.Contains(Keys.F))
            {
                _gvEye.Y += _gfMoveSpeed;
                validKey = true;
            }

            // END KEY - View moves down
            if (_pressedKeys.Contains(Keys.V))
            {
                _gvEye.Y -= _gfMoveSpeed;
                validKey = true;
            }

            if (validKey)
                _glControl.Invalidate();
        }

        public void InitialPosition()
        {
            _mousing = false;
            _xDiff = 0;
            _yDiff = 0;
            _znear = 0.1f;
            _zfar = 100.0f;
            _gfMoveSpeed = 0.10f;

            _gvEye = new Vector3(InitialEye);
            _gvLook = new Vector3(InitialLook);
            _gvRight = new Vector3(InitialRight);
            _gvUp = new Vector3(InitialUp);

            _gPtCurrentMousePosit = new Point();
            _gPtLastMousePosit = new Point();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            UpdateViewMatrix();
            _glControl.Invalidate();
        }

        public void Resize()
        {
            int w = _glControl.Width;
            int h = _glControl.Height;

            // set the viewport to the size of the window
            GL.Viewport(0, 0, w, h);
            // set the projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                       (float) w/h,
                                                                       _znear, _zfar);
            GL.LoadMatrix(ref perspective);
            _glControl.Invalidate();
        }

        public void KeyUp(KeyEventArgs e)
        {
            if (_pressedKeys.Contains(e.KeyCode))
                _pressedKeys.Remove(e.KeyCode);
            _glControl.Invalidate();
            _glControl.Focus();
        }

        public void KeyDown(KeyEventArgs e)
        {
            _pressedKeys.Add(e.KeyCode);
            _glControl.Invalidate();
        }

        public void UpdateViewMatrix()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix4 view = Matrix4.Identity;

            // normalize the lookat vector. We will call this vector "n" 
            _gvLook.Normalize();
            // compute and normalize the "X-direction" of the camera coordinate system.
            // We will call this vector "u"
            _gvRight = Vector3.Cross(_gvLook, _gvUp);
            _gvRight.Normalize();
            // compute and normalize the "Y-direction" of the camera coordinate system.
            // We will call this vector "v"
            _gvUp = Vector3.Cross(_gvRight, _gvLook);
            _gvLook.Normalize();

            // set the elements of the view matrix as a ModelView
            view.M11 = _gvRight.X;
            view.M12 = _gvUp.X;
            view.M13 = -_gvLook.X;
            view.M14 = 0.0f;

            view.M21 = _gvRight.Y;
            view.M22 = _gvUp.Y;
            view.M23 = -_gvLook.Y;
            view.M24 = 0.0f;

            view.M31 = _gvRight.Z;
            view.M32 = _gvUp.Z;
            view.M33 = -_gvLook.Z;
            view.M34 = 0.0f;

            view.M41 = -Vector3.Dot(_gvRight, _gvEye);
            view.M42 = -Vector3.Dot(_gvUp, _gvEye);
            view.M43 = Vector3.Dot(_gvLook, _gvEye);
            view.M44 = 1.0f;

            // multiply the current ModelView matrix (current an Identity Matrix) 
            // by the content of the matrix view
            GL.MultMatrix(ref view);
        }

        public void MouseMove(MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            // update the current mouse position
            _gPtCurrentMousePosit.X = x;
            _gPtCurrentMousePosit.Y = y;

            _xDiff = (x - _gPtLastMousePosit.X);
            _yDiff = (y - _gPtLastMousePosit.Y);

            _mousing = e.Button == MouseButtons.Left;

            if (_mousing)
                _glControl.Invalidate();

            // update the last position as the old current position
            _gPtLastMousePosit.X = _gPtCurrentMousePosit.X;
            _gPtLastMousePosit.Y = _gPtCurrentMousePosit.Y;
        }
    }
}