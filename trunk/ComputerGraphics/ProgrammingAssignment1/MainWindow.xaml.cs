namespace ProgrammingAssignment1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using Utilities;
    using Utilities.Enumerations;
    using Utilities.Extension;
    using Application = System.Windows.Forms.Application;
    using MessageBox = System.Windows.MessageBox;

    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly Dictionary<Keys, Direction> KeyMapping = new Dictionary<Keys, Direction>();
        private Axes _axes;
        private Camera _camera;
        private bool _culling;
        private bool _depthTest;
        private bool _enableLight;
        private FrontFaceDirection _frontFace;
        private GLControl _glControl;
        private double _hFovy;
        private bool _lookToObject;
        private Mesh _mesh;
        private PolygonMode _polygonMode;
        private ShadingModel _shadingModel;
        private TransformType _transform;
        private double _vFovy;
        private double _zfar;
        private double _znear;
        private float aspect;
        private int polaridade = 1;

        #endregion

        #region Properties

        public double HFovy
        {
            get { return _hFovy; }
            set
            {
                _hFovy = value;
                OnPropertyChanged("HFovy");
            }
        }

        public double VFovy
        {
            get { return _vFovy; }
            set
            {
                _vFovy = value;
                OnPropertyChanged("VFovy");
            }
        }

        private float VFovyRadian
        {
            get { return MathHelper.DegreesToRadians((float) VFovy); }
        }

        private float HFovyRadian
        {
            get { return MathHelper.DegreesToRadians((float) HFovy); }
        }

        public bool DepthTest
        {
            get { return _depthTest; }
            set
            {
                _depthTest = value;

                if (DepthTest)
                    GL.Enable(EnableCap.DepthTest);
                else
                    GL.Disable(EnableCap.DepthTest);

                OnPropertyChanged("DepthTest");
            }
        }


        public bool Culling
        {
            get { return _culling; }
            set
            {
                _culling = value;

                if (Culling)
                    GL.Enable(EnableCap.CullFace);
                else
                    GL.Disable(EnableCap.CullFace);

                OnPropertyChanged("Culling");
            }
        }


        public PolygonMode PolygonMode
        {
            get { return _polygonMode; }
            set
            {
                _polygonMode = value;
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode);
                OnPropertyChanged("PolygonMode");
            }
        }


        public FrontFaceDirection FrontFaceDirection
        {
            get { return _frontFace; }
            set
            {
                _frontFace = value;
                GL.FrontFace(FrontFaceDirection);
                OnPropertyChanged("FrontFaceDirection");
            }
        }

        public ShadingModel ShadingModel
        {
            get { return _shadingModel; }
            set
            {
                _shadingModel = value;
                GL.ShadeModel(ShadingModel);
                OnPropertyChanged("ShadingModel");
            }
        }


        public bool EnableLight
        {
            get { return _enableLight; }
            set
            {
                _enableLight = value;

                if (EnableLight)
                    GL.Enable(EnableCap.Lighting);
                else
                    GL.Disable(EnableCap.Lighting);

                OnPropertyChanged("EnableLight");
            }
        }


        public bool LookToObject
        {
            get { return _lookToObject; }
            set
            {
                _lookToObject = value;
                OnPropertyChanged("LookToObject");
            }
        }


        public TransformType Transform
        {
            get { return _transform; }
            set
            {
                _transform = value;
                OnPropertyChanged("Transform");
            }
        }

        public double ZNear
        {
            get { return _znear; }
            set
            {
                _znear = value;
                OnPropertyChanged("ZNear");
            }
        }

        public double ZFar
        {
            get { return _zfar; }
            set
            {
                _zfar = value;
                OnPropertyChanged("ZFar");
            }
        }

        public Axes Axes
        {
            get { return _axes; }
            set
            {
                _axes = value;
                OnPropertyChanged("Axes");
            }
        }

        #endregion

        public MainWindow()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            DataContext = this;
            KeyMapping[Keys.A] = Direction.Negative;
            KeyMapping[Keys.S] = Direction.Positive;
        }

        private void GLControlPaint(object sender, PaintEventArgs e)
        {
            try
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (_mesh != null)
                {
                    Setup();
                    Lighting();
                    _mesh.DrawMesh();
                }

                _glControl.SwapBuffers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GLControlLoad(object sender, EventArgs e)
        {
            ResetValues();

            _camera = new Camera(_glControl);

            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.ColorMaterial);

            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);

            Setup();

            _glControl = (GLControl) sender;
            _glControl.Invalidate();
        }

        private void ResetValues()
        {
            PolygonMode = PolygonMode.Fill;
            FrontFaceDirection = FrontFaceDirection.Cw;
            ShadingModel = ShadingModel.Smooth;

            DepthTest = true;
            Culling = true;
            EnableLight = true;

            ZNear = 0.5;
            ZFar = 3000;
            VFovy = 60;
            HFovy = 60;

            Red.Value = 255;
            Green.Value = 255;
            Blue.Value = 255;

            LookToObject = true;
            Transform = TransformType.Translate;
            Axes = Axes.X;
        }

        private void Setup()
        {
            //relacao de aspecto da camera
            aspect = (float) (Math.Tan(HFovyRadian)/Math.Tan(VFovyRadian));
            _camera.SetShape(VFovyRadian, aspect, (float) ZNear, (float) ZFar);
            //se está olhando para o objeto tem que recalcular os vetores da camera
            if (LookToObject && _mesh != null)
            {
                _camera.Set(_mesh.BoundingBox.Center);
            }
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog {InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath)};
            if (openFile.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            _mesh = new Mesh();
            _mesh.Load(new StreamReader(openFile.OpenFile()));

            ModelName.Text = _mesh.Name;

            _mesh.Colors[0] = Color.FromArgb((int) Red.Value, (int) Green.Value, (int) Blue.Value);
            ResetCamera();
            _glControl.Invalidate();
        }

        private void GLControlResize(object sender, EventArgs e)
        {
            var w = _glControl.Width;
            var h = _glControl.Height;

            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            aspect = (float) (Math.Tan(HFovyRadian)/Math.Tan(VFovyRadian));

            var perspective = Matrix4.CreatePerspectiveFieldOfView(VFovyRadian, aspect, (float) ZNear,
                                                                   (float) ZFar);
            GL.LoadMatrix(ref perspective);
            _glControl.Invalidate();
        }

        private void ResetViewClick(object sender, RoutedEventArgs e)
        {
            ResetValues();
            ResetCamera();
            _glControl.Invalidate();
        }

        private void ResetCamera()
        {
            var g_vEye = Vector3.Zero; // Eye Position
            var box = _mesh.BoundingBox;
            var center = box.Center;

            g_vEye.X = center.X;
            g_vEye.Y = center.Y;

            float maiorTam;
            float maiorAng;

            if (box.SizeX > box.SizeY)
            {
                maiorTam = box.SizeX;
                maiorAng = (float) (HFovy/2);
            }
            else
            {
                maiorTam = box.SizeY;
                maiorAng = (float) (VFovy/2);
            }

            //calcula z da camera
            g_vEye.Z = (float) (box.MaxZ + (maiorTam/2)/Math.Tan(MathHelper.DegreesToRadians(maiorAng)));

            //relacao de aspecto da camera
            aspect = (float) (Math.Tan(HFovyRadian)/Math.Tan(VFovyRadian));

            var g_vLookAt = center;

            var g_vUp = Vector3.UnitY;

            _camera.SetShape(VFovyRadian, aspect, (float) ZNear, (float) ZFar);
            _camera.Set(g_vEye, g_vLookAt, g_vUp);

            Setup();
        }

        private void SliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_mesh == null)
                return;

            _mesh.Colors[0] = Color.FromArgb((int) Red.Value, (int) Green.Value, (int) Blue.Value);
            _glControl.Invalidate();
        }

        private void Lighting()
        {
            GL.Enable(EnableCap.Light0);

            float[] globalAmbient = {0.4f, 0.4f, 0.4f, 1.0f};
            GL.LightModel(LightModelParameter.LightModelAmbient, globalAmbient);

            float[] position = {
                                   _mesh.BoundingBox.Center.X, _mesh.BoundingBox.Center.Y,
                                   _mesh.BoundingBox.MinZ - _mesh.BoundingBox.SizeZ*2, 1.0f
                               };
            float[] color = {0.6f, 0.6f, 0.6f, 1.0f};

            GL.Light(LightName.Light0, LightParameter.Position, position);
            GL.Light(LightName.Light0, LightParameter.Diffuse, color);
            GL.Light(LightName.Light0, LightParameter.Specular, color);
        }

        private void GLControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyMapping.ContainsKey(e.KeyCode))
            {
                switch (Transform)
                {
                    case TransformType.Rotation:
                        Rotate(KeyMapping[e.KeyCode]);
                        break;
                    case TransformType.Translate:
                        Translate(KeyMapping[e.KeyCode]);
                        break;
                }
            }
            _glControl.Invalidate();
        }

        public void Translate(Direction dir)
        {
            const int Escala = 40;
            var movement = dir.Signal()*Escala;
            switch (Axes)
            {
                case Axes.X:
                    if (LookToObject) _camera.SlideFixed(movement, 0, 0);
                    else _camera.Slide(movement, 0, 0);
                    break;
                case Axes.Y: //y
                    if (LookToObject) _camera.SlideFixed(0, movement, 0);
                    else _camera.Slide(0, movement, 0);
                    break;
                case Axes.Z: //z
                    if (LookToObject)
                    {
                        if (_camera.TranslateN())
                        {
                            _camera.SlideFixed(0, 0, movement*polaridade);
                        }
                        else
                        {
                            polaridade *= -1;
                            _camera.SlideFixed(0, 0, movement*polaridade);
                        }
                    }

                    else
                        _camera.Slide(0, 0, movement);
                    break;
            }
            Setup();
        }

        public void Rotate(Direction dir)
        {
            const int Angle = 5;
            var movement = dir.Signal()*MathHelper.DegreesToRadians(Angle);
            switch (Axes)
            {
                case Axes.X: //x
                    if (!LookToObject) _camera.Pitch(movement);
                    break;
                case Axes.Y: //y
                    if (!LookToObject) _camera.Yaw(movement);
                    break;
                case Axes.Z: //z
                    _camera.Roll(movement);
                    break;
            }
            Setup();
        }

        #region Implementation of INotifyPropertyChanged

        private FocusDel _focus;
        private InvalidateDel _invalidate;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

                if (_glControl != null)
                {
                    _invalidate = _glControl.Invalidate;
                    _glControl.Invoke(_invalidate);

                    _focus = _glControl.Focus;
                    _glControl.Invoke(_focus);
                }
            }
        }

        #region Nested type: FocusDel

        private delegate bool FocusDel();

        #endregion

        #region Nested type: InvalidateDel

        private delegate void InvalidateDel();

        #endregion

        #endregion
    }
}