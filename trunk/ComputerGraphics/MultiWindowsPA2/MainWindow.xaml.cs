using ColorsEnum = System.Drawing.Color;
using DefaultMesh = Utilities.Mesh;
using ExtendedMesh = Close2GL.Mesh;
using CameraAntiga = Utilities.Camera;

namespace MultiWindowsPA2
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Forms.Integration;
	using System.Windows.Media;
	using Close2GL;
	using Close2GL.Enumerations;
	using OpenTK;
	using OpenTK.Graphics.OpenGL;
	using Utilities.Enumerations;
	using Utilities.Extension;
	using Utilities.Windows;
	using Application = System.Windows.Forms.Application;
	using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
	using Matrix = Close2GL.Matrix;

	/// <summary>
	///   Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : INotifyPropertyChanged
	{
		#region Private Fields
        private float aspect;
        private int polaridade = 1;

		private readonly Dictionary<char, Direction> _keyMapping = new Dictionary<char, Direction>();
		private CameraAxes _axes;
		private Camera _camera;
	    private CameraAntiga camera;

		private bool _clipping;
		private GLControl _close2GL;
		private Color _color;
		private bool _culling;
		private bool _depthTest;
		private bool _enableLight;
		private FrontFaceDirection _frontFace;
		private bool _lookToObject;
		private DefaultMesh _mesh;
		private ExtendedMesh _meshEx;
		private string _modelName;
		private GLControl _opengl;
		private PolygonMode _polygonMode;
		private ShadingModel _shadingModel;
		private TransformType _transform;

		#endregion

		#region Properties

		public string ModelName
		{
			get { return _modelName; }
			set
			{
				_modelName = value;
				OnPropertyChanged("ModelName");
			}
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				if (_mesh != null)
				{
					_mesh.Colors[0] = value.FromMediaColor();
					_opengl.MakeCurrent();
					_opengl.Invalidate();
				}

				if (_meshEx != null)
				{
					_meshEx.Colors[0] = value.FromMediaColor();
					_close2GL.MakeCurrent();
					_close2GL.Invalidate();
				}
				OnPropertyChanged("Color");
			}
		}

		public double HFovy
		{
			get { return _camera.FovX; }
			set
			{
				_camera.FovX = (float) value;
				OnPropertyChanged("HFovy");
			}
		}

		public double VFovy
		{
			get { return _camera.FovY; }
			set
			{
				_camera.FovY = (float) value;
				OnPropertyChanged("VFovy");
			}
		}

		private float VFovyRadian
		{
			get { return MathHelper.DegreesToRadians((float) VFovy); }
			set { VFovy = MathHelper.RadiansToDegrees(value); }
		}

		private float HFovyRadian
		{
			get { return MathHelper.DegreesToRadians((float) HFovy); }
			set { HFovy = MathHelper.RadiansToDegrees(value); }
		}

		public bool Clipping
		{
			get { return _clipping; }
			set
			{
				_clipping = value;
				if (_meshEx != null)
					_meshEx.UseClipping = value;
				OnPropertyChanged("Clipping");
			}
		}

		public bool DepthTest
		{
			get { return _depthTest; }
			set
			{
				_depthTest = value;

				if (_opengl != null)
				{
					_opengl.MakeCurrent();
					if (DepthTest)
						GL.Enable(EnableCap.DepthTest);
					else
						GL.Disable(EnableCap.DepthTest);
				}

				if (_close2GL != null)
				{
					_close2GL.MakeCurrent();
					if (DepthTest)
						GL.Enable(EnableCap.DepthTest);
					else
						GL.Disable(EnableCap.DepthTest);
				}
				OnPropertyChanged("DepthTest");
			}
		}

		public bool Culling
		{
			get { return _culling; }
			set
			{
				_culling = value;
				if (_opengl != null)
				{
					_opengl.MakeCurrent();
					if (Culling)
						GL.Enable(EnableCap.CullFace);
					else
						GL.Disable(EnableCap.CullFace);
				}
				if (_close2GL != null)
				{
					_close2GL.MakeCurrent();
					if (Culling)
						GL.Enable(EnableCap.CullFace);
					else
						GL.Disable(EnableCap.CullFace);
				}
				if (_meshEx != null)
				{
					_meshEx.UseCulling = value;
				}

				OnPropertyChanged("Culling");
			}
		}

		public PolygonMode PolygonMode
		{
			get { return _polygonMode; }
			set
			{
				_polygonMode = value;
				if (_opengl != null)
				{
					_opengl.MakeCurrent();
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode);
				}
				if (_close2GL != null)
				{
					_close2GL.MakeCurrent();
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode);
				}
				OnPropertyChanged("PolygonMode");
			}
		}

		public FrontFaceDirection FrontFaceDirection
		{
			get { return _frontFace; }
			set
			{
				_frontFace = value;
				if (_opengl != null)
				{
					_opengl.MakeCurrent();
					GL.FrontFace(FrontFaceDirection);
				}
				if (_close2GL != null)
				{
					_close2GL.MakeCurrent();
					GL.FrontFace(FrontFaceDirection);
				}
                if (_meshEx != null)
                {
                    _meshEx.FrontFaceDirection = value;
                }
				OnPropertyChanged("FrontFaceDirection");
			}
		}

		public ShadingModel ShadingModel
		{
			get { return _shadingModel; }
			set
			{
				_shadingModel = value;
				if (_opengl != null)
				{
					_opengl.MakeCurrent();
					GL.ShadeModel(ShadingModel);
				}
				if (_close2GL != null)
				{
					_close2GL.MakeCurrent();
					GL.ShadeModel(ShadingModel);
				}
				OnPropertyChanged("ShadingModel");
			}
		}

		public bool EnableLight
		{
			get { return _enableLight; }
			set
			{
				_enableLight = value;
				if (_opengl != null)
				{
					_opengl.MakeCurrent();
					if (EnableLight)
						GL.Enable(EnableCap.Lighting);
					else
						GL.Disable(EnableCap.Lighting);
				}

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
			get { return _camera.ZNear; }
			set
			{
				_camera.ZNear = (float) value;
				OnPropertyChanged("ZNear");
			}
		}

		public double ZFar
		{
			get { return _camera.ZFar; }
			set
			{
				_camera.ZFar = (float) value;
				OnPropertyChanged("ZFar");
			}
		}

		public CameraAxes Axes
		{
			get { return _axes; }
			set
			{
				_axes = value;
				OnPropertyChanged("Axes");
			}
		}

		#endregion

		#region MainWindow

		public MainWindow()
		{
			InitializeComponent();
			Loaded += SizeUtil.WindowLoaded;
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			_camera = new Camera();

			_opengl = new GLControl {VSync = true};
			_opengl.Paint += OpenglOnPaint;
			_opengl.Load += OpenglOnLoad;
			_opengl.Resize += OpenglOnResize;
			_opengl.KeyPress += OnKeyPressed;

			_close2GL = new GLControl {VSync = true};
			_close2GL.Paint += Close2GLOnPaint;
			_close2GL.Load += Close2GLOnLoad;
			_close2GL.Resize += Close2GLOnResize;
			_close2GL.KeyPress += OnKeyPressed;

			SizeUtil.upWindow.Content = new WindowsFormsHost {Child = _opengl};
			SizeUtil.upWindow.Title = "OpenGL";

			SizeUtil.downWindow.Content = new WindowsFormsHost {Child = _close2GL};
			SizeUtil.downWindow.Title = "Close2GL";

			DataContext = this;
			_keyMapping['a'] = Direction.Negative;
			_keyMapping['A'] = Direction.Negative;
			_keyMapping['s'] = Direction.Positive;
			_keyMapping['S'] = Direction.Positive;
		}

		#endregion

		#region Opengl

		private void OpenglOnResize(object sender, EventArgs eventArgs)
		{
			_opengl.MakeCurrent();
            var w = _opengl.Width;
            var h = _opengl.Height;

            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            aspect = (float)(Math.Tan(HFovyRadian) / Math.Tan(VFovyRadian));

            var perspective = Matrix4.CreatePerspectiveFieldOfView(VFovyRadian, aspect, (float)ZNear,
                                                                   (float)ZFar);
            GL.LoadMatrix(ref perspective);
			_opengl.Invalidate();
		}

		private void OpenglOnLoad(object sender, EventArgs eventArgs)
		{
            _opengl.MakeCurrent();
            ResetValues();
            camera = new CameraAntiga(_opengl);
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
            Setup();
			GL.ClearColor(ColorsEnum.CornflowerBlue);
		    _opengl.Size = _opengl.Size;
		}

		private void OpenglOnPaint(object sender, PaintEventArgs paintEventArgs)
		{
			_opengl.MakeCurrent();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			if (_mesh != null)
			{
                Setup();
                Lighting();
                _mesh.DrawMesh();
			}
			_opengl.SwapBuffers();
		}

		#endregion

		#region Close2gl

		private void Close2GLOnResize(object sender, EventArgs eventArgs)
		{
			_close2GL.MakeCurrent();

			// set the viewport to the size of the window
			GL.Viewport(0, 0, _close2GL.Width, _close2GL.Height);
			// set the projection matrix
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0.0, 1.0, 0.0, 1.0, -1, 1);
			Matrix.ViewportTransform(0, 0, 1, 1);
			// set the modelview matrix to identity
			Matrix.PerspectiveTransform((float) VFovy, (float) (Math.Tan(HFovyRadian/2)/Math.Tan(VFovyRadian/2)),
										(float) ZNear,
										(float) ZFar);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			_close2GL.Invalidate();
		}

		private void Close2GLOnLoad(object sender, EventArgs eventArgs)
		{
			ResetValues();
			_close2GL.MakeCurrent();
			GL.ClearColor(ColorsEnum.DarkSeaGreen);
            _close2GL.Size = _close2GL.Size;
		}

		private void Close2GLOnPaint(object sender, PaintEventArgs paintEventArgs)
		{
			_close2GL.MakeCurrent();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			if (_meshEx != null)
			{
				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadIdentity();
				double w = _close2GL.Width;
				double h = _close2GL.Height;

				GL.Ortho(0, w, 0, h, -1, 1);

				//we dont want to use the other window configs
				InitGL();
				GL.Disable(EnableCap.CullFace);

				// applying the projection
				Matrix.Projection = Matrix4.Identity;
				Matrix.PerspectiveTransform((float) VFovy, (float) (Math.Tan(HFovyRadian/2)/Math.Tan(VFovyRadian/2)),
											(float) ZNear, (float) ZFar);

				Matrix.Model = Matrix4.Identity;

				// applying the camera to closetogl
				Matrix.View = Matrix4.Identity;
				_camera.BuildMatrix();
				Matrix.View = _camera.View;

				Matrix.Viewport = Matrix4.Identity;
				Matrix.ViewportTransform(0, 0, (int) w, (int) h);

				_meshEx.DrawMesh();
			}
			_close2GL.SwapBuffers();
		}

		private void InitGL()
		{
			GL.Enable(EnableCap.Normalize);
			GL.Enable(EnableCap.DepthTest);
			GL.ShadeModel(ShadingModel.Smooth);
			GL.Enable(EnableCap.ColorMaterial);
			GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
		}

		#endregion

		#region GUI

		private void ColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
		{
			Color = e.NewValue;
		}

		private void LoadClick(object sender, RoutedEventArgs e)
		{
			var openFile = new OpenFileDialog
							   {
								   InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath),
								   Multiselect = false
							   };
			if (openFile.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			if (!(openFile.CheckFileExists && openFile.CheckPathExists)) return;

			var filePath = openFile.FileName;

			using (var meshStream = new StreamReader(filePath))
			{
                _opengl.MakeCurrent();
				_mesh = new Mesh();
				_mesh.Load(meshStream);

				ModelName = _mesh.Name;
				_mesh.Colors[0] = Color.FromMediaColor();
                ResetCamera();
				_opengl.Invalidate();
			}

			using (var meshStream = new StreamReader(filePath))
			{
				_meshEx = new ExtendedMesh();
				_meshEx.Load(meshStream);

				_meshEx.Colors[0] = Color.FromMediaColor();

				LookAtModel();

				_close2GL.MakeCurrent();
				_close2GL.Invalidate();
			}
		}

		private void ResetViewClick(object sender, RoutedEventArgs e)
		{
			ResetValues();
			LookAtModel();
		    ResetCamera();
		}

        public void Translate(Direction dir)
        {
            const int Escala = 40;
            var movement = dir.Signal() * Escala;
            switch (Axes)
            {
                case CameraAxes.U:
                    if (LookToObject) camera.SlideFixed(movement, 0, 0);
                    else camera.Slide(movement, 0, 0);
                    break;
                case CameraAxes.V: //y
                    if (LookToObject) camera.SlideFixed(0, movement, 0);
                    else camera.Slide(0, movement, 0);
                    break;
                case CameraAxes.N: //z
                    if (LookToObject)
                    {
                        if (camera.TranslateN())
                        {
                            camera.SlideFixed(0, 0, movement * polaridade);
                        }
                        else
                        {
                            polaridade *= -1;
                            camera.SlideFixed(0, 0, movement * polaridade);
                        }
                    }

                    else
                        camera.Slide(0, 0, movement);
                    break;
            }
            Setup();
        }

        public void Rotate(Direction dir)
        {
            const int Angle = 5;
            var movement = dir.Signal() * MathHelper.DegreesToRadians(Angle);
            switch (Axes)
            {
                case CameraAxes.U: //x
                    if (!LookToObject) camera.Pitch(movement);
                    break;
                case CameraAxes.V: //y
                    if (!LookToObject) camera.Yaw(movement);
                    break;
                case CameraAxes.N: //z
                    camera.Roll(movement);
                    break;
            }
            Setup();
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
                maiorAng = (float)(HFovy / 2);
            }
            else
            {
                maiorTam = box.SizeY;
                maiorAng = (float)(VFovy / 2);
            }

            //calcula z da camera
            g_vEye.Z = (float)(box.MaxZ + (maiorTam / 2) / Math.Tan(MathHelper.DegreesToRadians(maiorAng)));

            //relacao de aspecto da camera
            aspect = (float)(Math.Tan(HFovyRadian) / Math.Tan(VFovyRadian));

            var g_vLookAt = center;

            var g_vUp = Vector3.UnitY;

            camera.SetShape(VFovyRadian, aspect, (float)ZNear, (float)ZFar);
            camera.Set(g_vEye, g_vLookAt, g_vUp);

            Setup();
        }


        private void Setup()
        {
            //relacao de aspecto da camera
            aspect = (float)(Math.Tan(HFovy) / Math.Tan(VFovy));
            camera.SetShape(VFovyRadian, aspect, (float)ZNear, (float)ZFar);
            //se está olhando para o objeto tem que recalcular os vetores da camera
            if (LookToObject && _mesh != null)
            {
                camera.Set(_mesh.BoundingBox.Center);
            }
        }
		#endregion

		#region Implementation of INotifyPropertyChanged

		private ChangedDel _changed;
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string prop)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(prop));

				if (_opengl != null)
				{
					_changed = null;
					_changed += _opengl.MakeCurrent;
					_changed += _opengl.Invalidate;
					_opengl.Invoke(_changed);
				}

				if (_close2GL != null)
				{
					_changed = null;
					_changed += _close2GL.MakeCurrent;
					_changed += _close2GL.Invalidate;
					_close2GL.Invoke(_changed);
				}
			}
		}

		private delegate void ChangedDel();

		#endregion

		#region Common

		private void ResetValues()
		{
			PolygonMode = PolygonMode.Fill;
			FrontFaceDirection = FrontFaceDirection.Cw;
			ShadingModel = ShadingModel.Smooth;

			Colors.SelectedColor = System.Windows.Media.Colors.White;

			Clipping = true;
			Culling = true;
			DepthTest = true;
			EnableLight = false;

			ZNear = 0.5;
			ZFar = 3000;
			VFovy = 60;
			HFovy = 60;

			LookToObject = true;
			Transform = TransformType.Translate;
			Axes = CameraAxes.U;
		}

		private void LookAtModel()
		{
			var boundingBox = _mesh.BoundingBox;
			var center = boundingBox.Center;
			var alfa = MathHelper.DegreesToRadians(_camera.FovY/2);

			var fovy = (float) (((boundingBox.MaxY - boundingBox.MinY)/2)/Math.Tan(alfa));

			var beta = (float) Math.Atan((float) _opengl.Width/_opengl.Height*Math.Tan(alfa));

			var fovx = (float) (((boundingBox.MaxX - boundingBox.MinX)/2)/Math.Tan(beta));

			var newZ = Math.Max(fovy, fovx);

			_camera.LookAt(new Vector3(center.X, center.Y, boundingBox.MaxZ + newZ),
						   center,
						   Vector3.UnitY);
		}

		private void OnKeyPressed(object sender, KeyPressEventArgs keyEventArgs)
		{
			if (!_keyMapping.ContainsKey(keyEventArgs.KeyChar)) return;
			var direction = _keyMapping[keyEventArgs.KeyChar].Signal();

            _opengl.MakeCurrent();
            switch (Transform)
            {
                case TransformType.Rotation:
                    Rotate(_keyMapping[keyEventArgs.KeyChar]);
                    break;
                case TransformType.Translate:
                    Translate(_keyMapping[keyEventArgs.KeyChar]);
                    break;
            }
            _opengl.Invalidate();

            _close2GL.MakeCurrent();
			switch (Transform)
			{
				case TransformType.Rotation:
					_camera.Rotate(MathHelper.DegreesToRadians(5*direction), Axes);
					break;
				case TransformType.Translate:
					var trans = Vector3.Zero;
					switch (Axes)
					{
						case CameraAxes.U:
							trans += _camera.U;
							break;
						case CameraAxes.V:
							trans += _camera.V;
							break;
						case CameraAxes.N:
							trans += _camera.N;
							break;
					}

					trans = 40*direction*trans;

					if (!LookToObject)
						_camera.Translate(trans);
					else
						_camera.TranslateCenter(trans);
					break;
			}
			_close2GL.Invalidate();
		}

		private void Lighting()
		{
			_opengl.MakeCurrent();
			GL.Enable(EnableCap.Light0);

			float[] globalAmbient = {0.4f, 0.4f, 0.4f, 1.0f};
			GL.LightModel(LightModelParameter.LightModelAmbient, globalAmbient);

			float[] position = {
								   _mesh.BoundingBox.Center.X, _mesh.BoundingBox.Center.Y,
								   _mesh.BoundingBox.MinZ - _mesh.BoundingBox.MaxZ, 1.0f
							   };
			float[] color = {0.8f, 0.8f, 0.8f, 1.0f};

			GL.Light(LightName.Light0, LightParameter.Position, position);
			GL.Light(LightName.Light0, LightParameter.Diffuse, color);
			GL.Light(LightName.Light0, LightParameter.Specular, color);
		}

		#endregion

	}
}