namespace Additional3
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Forms;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using Utilities;
    using Utilities.Extension;

    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly Stopwatch _watch = new Stopwatch();
        private float _angle;
        private bool _enabled;
        private GLControl _glControl;
        private Movement _mov;

        private float _time;
        private Vector3 _vector3;

        public MainWindow()
        {
            _vector3 = new Vector3();
            _angle = 0;
            _time = 0;
            Enabled = true;

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            InitializeComponent();
            Loaded += OnLoaded;
        }


        private Vector3 Axis
        {
            get { return _vector3.Normalized()*5; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        public float X
        {
            get { return _vector3.X; }
            set
            {
                _vector3.X = value;
                OnPropertyChanged("X");
            }
        }

        public float Y
        {
            get { return _vector3.Y; }
            set
            {
                _vector3.Y = value;
                OnPropertyChanged("Y");
            }
        }

        public float Z
        {
            get { return _vector3.Z; }
            set
            {
                _vector3.Z = value;
                OnPropertyChanged("Z");
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                OnPropertyChanged("Angle");
            }
        }

        public float Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            DataContext = this;

            GL.Enable(EnableCap.DepthTest);
        }

        private void GlControlLoad(object sender, EventArgs e)
        {
            _glControl = (GLControl) sender;
            _mov = new Movement(_glControl);
            InitialPosition(this, null);

            GL.ClearColor(Color.Black);
        }

        private void GlControlResize(object sender, EventArgs e)
        {
            _mov.Resize();
        }

        private void GlControlPaint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (Enabled)
                _mov.ProcessUserInput();

            _mov.UpdateViewMatrix();

            Elements.CoordinateSystem();
            DrawRotateAxis();

            if (_watch.IsRunning)
            {
                var elapsedTime = (float) _watch.Elapsed.TotalSeconds;


                QuaternionEx.CustomRotate(_vector3, - Math.Abs(elapsedTime*Angle/(Time/2) - Angle) + Angle);
                _glControl.Invalidate();


                if (elapsedTime > Time)
                {
                    _watch.Stop();
                    Enabled = true;
                }
            }

            Elements.SolidCube();

            _glControl.SwapBuffers();
        }

        private void DrawRotateAxis()
        {
            GL.LineWidth(5);
            Axis.DrawFromOrigin(Color.White);
            GL.LineWidth(1);
        }

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged == null) return;

            _glControl.Invalidate();
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void RotateButtonClick(object sender, RoutedEventArgs e)
        {
            Enabled = false;
            _watch.Restart();
            _glControl.Invalidate();
        }

        private void GLControlOnMouseMove(object sender, MouseEventArgs e)
        {
            _mov.MouseMove(e);
        }

        private void GLControlOnKeyUp(object sender, KeyEventArgs e)
        {
            _mov.KeyUp(e);
        }

        private void GLControlKeyDown(object sender, KeyEventArgs e)
        {
            _mov.KeyDown(e);
        }

        private void InitialPosition(object sender, RoutedEventArgs e)
        {
            _mov.InitialPosition();
            _glControl.Invalidate();
            _glControl.Focus();
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}