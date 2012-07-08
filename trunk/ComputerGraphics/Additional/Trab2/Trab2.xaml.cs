namespace Additional2.Trab2
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Forms;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using Utilities;
    using Utilities.Extension;

    /// <summary>
    ///   Interaction logic for Trab2.xaml
    /// </summary>
    public partial class Trab2 : INotifyPropertyChanged
    {
        private bool _drawIntersection;

        private bool _drawR1;
        private bool _drawR1Plane;
        private bool _drawR2;
        private bool _drawR2Plane;
        private bool _drawWPlane;
        private bool _enableDepthTest;
        private GLControl _glControl;
        private Vector3 _intersection;

        private Movement _mov;

        private Vector3 _p1;
        private Vector3 _p2;
        private Vector3 _p3;
        private Vector3 _p4;

        private Vector3 _r1;
        private Vector3 _r2;


        public Trab2()
        {
            _p1 = new Vector3();
            _p2 = new Vector3();
            _p3 = new Vector3();
            _p4 = new Vector3();

            _r1 = new Vector3();
            _r2 = new Vector3();

            _intersection = new Vector3();

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            InitializeComponent();
            Loaded += OnLoaded;
        }

        #region Intersection

        public string XI
        {
            get
            {
                return float.IsNaN(_intersection.X) || float.IsInfinity(_intersection.X)
                           ? "-"
                           : _intersection.X.ToString();
            }
            set
            {
                _intersection.X = float.Parse(value);
                OnPropertyChanged("XI");
            }
        }

        public string YI
        {
            get
            {
                return float.IsNaN(_intersection.Y) || float.IsInfinity(_intersection.Y)
                           ? "-"
                           : _intersection.Y.ToString();
            }
            set
            {
                _intersection.Y = float.Parse(value);
                OnPropertyChanged("YI");
            }
        }

        #endregion

        #region Options

        public bool EnableDepthTest
        {
            get { return _enableDepthTest; }
            set
            {
                _enableDepthTest = value;
                OnPropertyChanged("EnableDepthTest");
            }
        }

        public bool DrawWPlane
        {
            get { return _drawWPlane; }
            set
            {
                _drawWPlane = value;
                OnPropertyChanged("DrawWPlane");
            }
        }

        public bool DrawR1Plane
        {
            get { return _drawR1Plane; }
            set
            {
                _drawR1Plane = value;
                OnPropertyChanged("DrawR1Plane");
            }
        }

        public bool DrawR2Plane
        {
            get { return _drawR2Plane; }
            set
            {
                _drawR2Plane = value;
                OnPropertyChanged("DrawR2Plane");
            }
        }

        public bool DrawR1
        {
            get { return _drawR1; }
            set
            {
                _drawR1 = value;
                OnPropertyChanged("DrawR1");
            }
        }

        public bool DrawR2
        {
            get { return _drawR2; }
            set
            {
                _drawR2 = value;
                OnPropertyChanged("DrawR2");
            }
        }

        public bool DrawIntersection
        {
            get { return _drawIntersection; }
            set
            {
                _drawIntersection = value;
                OnPropertyChanged("DrawIntersection");
            }
        }

        #endregion

        #region P1

        public float X1
        {
            get { return _p1.X; }
            set
            {
                _p1.X = value;
                OnPropertyChanged("X1");
                CalculateR1();
            }
        }

        public float Y1
        {
            get { return _p1.Y; }
            set
            {
                _p1.Y = value;
                OnPropertyChanged("Y1");
                CalculateR1();
            }
        }

        #endregion

        #region P2

        public float X2
        {
            get { return _p2.X; }
            set
            {
                _p2.X = value;
                OnPropertyChanged("X2");
                CalculateR1();
            }
        }

        public float Y2
        {
            get { return _p2.Y; }
            set
            {
                _p2.Y = value;
                OnPropertyChanged("Y2");
                CalculateR1();
            }
        }

        #endregion

        #region P3

        public float X3
        {
            get { return _p3.X; }
            set
            {
                _p3.X = value;
                OnPropertyChanged("X3");
                CalculateR2();
            }
        }

        public float Y3
        {
            get { return _p3.Y; }
            set
            {
                _p3.Y = value;
                OnPropertyChanged("Y3");
                CalculateR2();
            }
        }

        #endregion

        #region P4

        public float X4
        {
            get { return _p4.X; }
            set
            {
                _p4.X = value;
                OnPropertyChanged("X4");
                CalculateR2();
            }
        }

        public float Y4
        {
            get { return _p4.Y; }
            set
            {
                _p4.Y = value;
                OnPropertyChanged("Y4");
                CalculateR2();
            }
        }

        #endregion

        #region W

        public float W
        {
            get { return _p1.Z; }
            set
            {
                _p1.Z = _p2.Z = _p3.Z = _p4.Z = value;
                OnPropertyChanged("W");
                CalculateR1();
                CalculateR2();
            }
        }

        #endregion

        #region R1

        public string A1
        {
            get { return float.IsNaN(_r1.X) || float.IsInfinity(_r1.X) ? "-" : _r1.X.ToString(); }
            set
            {
                _r1.X = float.Parse(value);
                OnPropertyChanged("A1");
            }
        }

        public string B1
        {
            get { return float.IsNaN(_r1.Y) || float.IsInfinity(_r1.Y) ? "-" : _r1.Y.ToString(); }
            set
            {
                _r1.Y = float.Parse(value);
                OnPropertyChanged("B1");
            }
        }

        public string C1
        {
            get { return float.IsNaN(_r1.Z) || float.IsInfinity(_r1.Z) ? "-" : _r1.Z.ToString(); }
            set
            {
                _r1.Z = float.Parse(value);
                OnPropertyChanged("C1");
            }
        }

        private void CalculateR1()
        {
            _r1 = Vector3.Cross(_p1, _p2);

            if (_r1.Z != 0)
            {
                _r1 = Vector3.Divide(_r1, _r1.Z);
                _r1.Z = W;
            }


            CalculateIntersection();

            OnPropertyChanged("A1");
            OnPropertyChanged("B1");
            OnPropertyChanged("C1");
            OnPropertyChanged("XI");
            OnPropertyChanged("YI");
        }

        #endregion

        #region R2

        public string A2
        {
            get { return float.IsNaN(_r2.X) || float.IsInfinity(_r2.X) ? "-" : _r2.X.ToString(); }
            set
            {
                _r2.X = float.Parse(value);
                OnPropertyChanged("A2");
            }
        }

        public string B2
        {
            get { return float.IsNaN(_r2.Y) || float.IsInfinity(_r2.Y) ? "-" : _r2.Y.ToString(); }
            set
            {
                _r2.Y = float.Parse(value);
                OnPropertyChanged("B2");
            }
        }

        public string C2
        {
            get { return float.IsNaN(_r2.Z) || float.IsInfinity(_r2.Z) ? "-" : _r2.Z.ToString(); }
            set
            {
                _r2.Z = float.Parse(value);
                OnPropertyChanged("C2");
            }
        }

        private void CalculateR2()
        {
            _r2 = Vector3.Cross(_p3, _p4);
            if (_r2.Z != 0)
            {
                _r2 = Vector3.Divide(_r2, _r2.Z);
                _r2.Z = W;
            }

            CalculateIntersection();

            OnPropertyChanged("A2");
            OnPropertyChanged("B2");
            OnPropertyChanged("C2");
            OnPropertyChanged("XI");
            OnPropertyChanged("YI");
        }

        private void CalculateIntersection()
        {
            _intersection = Vector3.Cross(_r1, _r2);
            if (_intersection.Z != 0)
            {
                _intersection = Vector3.Divide(_intersection, _intersection.Z);
                _intersection.Z = W;
            }
        }

        #endregion

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            DataContext = this;
            W = 1;
        }

        private void GlControlGlMouseMove(object sender, MouseEventArgs e)
        {
            _mov.MouseMove(e);
        }

        private void GlControlGlKeyDown(object sender, KeyEventArgs e)
        {
            _mov.KeyDown(e);
        }

        private void GlControlGlKeyUp(object sender, KeyEventArgs e)
        {
            _mov.KeyUp(e);
        }

        private void GlControlGlInitialized(object sender, EventArgs e)
        {
            _glControl = (GLControl) sender;
            _mov = new Movement(_glControl);
            _mov.InitialPosition();
            _glControl.Focus();

            GL.ClearColor(Color.Black);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
        }

        private void InitialPositionButtonClick(object sender, RoutedEventArgs e)
        {
            _mov.InitialPosition();
            _glControl.Focus();
        }

        private void GlControlResize(object sender, EventArgs e)
        {
            _mov.Resize();
        }

        private void GlControlPaint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _mov.ProcessUserInput();
            _mov.UpdateViewMatrix();
            Elements.CoordinateSystem();

            if (EnableDepthTest)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);

            Elements.InstructionsTrab2();

            const int diff = 20;

            if (DrawWPlane)
            {
                GL.Color4(0.35f, 0.53f, 0.7f, 0.6f);

                GL.Begin(BeginMode.Quads);
                GL.Vertex3(-diff, -diff, W);
                GL.Vertex3(-diff, diff, W);
                GL.Vertex3(diff, diff, W);
                GL.Vertex3(diff, -diff, W);
                GL.End();
            }

            if (DrawR1Plane)
            {
                var pb1 = new Vector3(_p1);
                var pb2 = new Vector3(_p2);
                pb1.Normalize();
                pb2.Normalize();

                Vector3 pp1 = pb1*(diff + W);
                Vector3 pp2 = pb2*(diff + W);
                Vector3 pp3 = pb1*(-diff);
                Vector3 pp4 = pb2*(-diff);

                GL.Color4(1.0f, 0.7f, 0.7f, 0.6f);

                GL.Begin(BeginMode.Quads);
                GL.Vertex3(pp1);
                GL.Vertex3(pp2);
                GL.Vertex3(pp3);
                GL.Vertex3(pp4);
                GL.End();

                GL.LineWidth(3);
                _r1.Normalized().DrawFromOrigin(Color.HotPink);
                GL.LineWidth(1);
            }

            if (DrawR2Plane)
            {
                var pb1 = new Vector3(_p3);
                var pb2 = new Vector3(_p4);
                pb1.Normalize();
                pb2.Normalize();

                Vector3 pp1 = pb1*(diff + W);
                Vector3 pp2 = pb2*(diff + W);
                Vector3 pp3 = pb1*(-diff);
                Vector3 pp4 = pb2*(-diff);

                GL.Color4(0.7f, 1.0f, 0.7f, 0.6f);

                GL.Begin(BeginMode.Quads);
                GL.Vertex3(pp1);
                GL.Vertex3(pp2);
                GL.Vertex3(pp3);
                GL.Vertex3(pp4);
                GL.End();

                GL.LineWidth(3);
                _r2.Normalized().DrawFromOrigin(Color.Olive);
                GL.LineWidth(1);
            }

            if (DrawR1)
            {
                _p1.DrawPoint(Color.White);
                _p2.DrawPoint(Color.Yellow);

                _p1.DrawFromOrigin(Color.White);
                _p2.DrawFromOrigin(Color.Yellow);

                Vector3Ex.DrawBetween(_p1, _p2, Color.White);
            }

            if (DrawR2)
            {
                _p3.DrawPoint(Color.Violet);
                _p4.DrawPoint(Color.LightGreen);

                _p3.DrawFromOrigin(Color.Violet);
                _p4.DrawFromOrigin(Color.LightGreen);

                Vector3Ex.DrawBetween(_p3, _p4, Color.White);
            }

            if (DrawIntersection)
            {
                Vector3Ex.DrawBetween(_p1, _intersection, Color.White);
                Vector3Ex.DrawBetween(_p2, _intersection, Color.White);
                Vector3Ex.DrawBetween(_p3, _intersection, Color.White);
                Vector3Ex.DrawBetween(_p4, _intersection, Color.White);

                _intersection.DrawPoint(Color.LightYellow);
            }

            _glControl.SwapBuffers();
        }

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                _glControl.Invalidate();
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            DrawR1 = DrawR2 = DrawIntersection = DrawWPlane = DrawR1Plane = DrawR2Plane = EnableDepthTest = false;
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}