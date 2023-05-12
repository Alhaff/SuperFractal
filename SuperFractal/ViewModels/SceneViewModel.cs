using SuperFractal.Commands;
using SuperFractal.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SuperFractal.ViewModels
{
    public class SceneViewModel : ViewModelBase
    {
		private WriteableBitmap _scene = BitmapFactory.New(0,0);
		public WriteableBitmap Scene
		{
			get
			{
				return _scene;
			}
			set
			{
				_scene = value;
				OnPropertyChanged(nameof(Scene));
			}
		}
        public Stopwatch Timer { get; set; } = new Stopwatch();
        private TimeSpan PreviousTick { get; set; }
        public float ElapsedSecondsSinceLastTick
        {
			get
			{
				return (float)(Timer.Elapsed - PreviousTick).TotalSeconds;
			}
        }

		private string _frameDrawingTime;
		public string FrameDrawingTime
		{
			get
			{
				return _frameDrawingTime;
			}
			private set
			{
				_frameDrawingTime = value;
				OnPropertyChanged(nameof(FrameDrawingTime));
			}
		}


        private int SceneWidth { get; set; } = 0;

        private int SceneHeight { get; set; } = 0;

		private int[] SceneBuffer { get; set; } = new int[0];

		private void UpdateScene()
		{
            PreviousTick = Timer.Elapsed;
			DrawFrame();
            FrameDrawingTime = ElapsedSecondsSinceLastTick.ToString();
        }

		private void UpdateSceneSize(object parametr)
		{
            var obj = parametr as SizeChangedEventArgs;
            SceneWidth = (int)obj.NewSize.Width;
            SceneHeight = (int)obj.NewSize.Height;
            Scene = BitmapFactory.New(SceneWidth, SceneHeight);
            SceneBuffer = new int[SceneWidth * SceneHeight];
            UpdateScene();
        }

        private void MouseWheel(object parametr)
        {

        }

        private void PreviewMouseDown(object parametr)
        {

        }
        private void MouseDown(object parametr)
        {

        }
        private void MouseMove(object parametr)
        {

        }



        private void DrawFrame()
		{
            Scene.Lock();
            //Scene.WritePixels(new Int32Rect(0, 0, SceneWidth, SceneHeight), SceneBuffer, Scene.BackBufferStride, 0);
            Scene.Clear(Colors.White);
            Scene.Unlock();
            //OnPropertyChanged(nameof(Scene));
        }
        private RelayCommand sceneSizeChanged;
        public RelayCommand SceneSizeChanged
        {
            get
            {
                return sceneSizeChanged ??
                    (sceneSizeChanged = new RelayCommand(UpdateSceneSize));
            }
        }

        private RelayCommand sceneMouseWheel;
        public RelayCommand SceneMouseWheel
        {
            get
            {
                return sceneMouseWheel ??
                    (sceneMouseWheel = new RelayCommand(MouseWheel));
            }
        }

        private RelayCommand scenePreviewMouseDown;
        public RelayCommand ScenePreviewMouseDown
        {
            get
            {
                return scenePreviewMouseDown ??
                    (scenePreviewMouseDown = new RelayCommand(PreviewMouseDown));
            }
        }

        private RelayCommand sceneMouseDown;
        public RelayCommand SceneMouseDown
        {
            get
            {
                return sceneMouseDown ??
                    (sceneMouseDown = new RelayCommand(MouseDown));
            }
        }

        private RelayCommand sceneMouseMove;
        public RelayCommand SceneMouseMove
        {
            get
            {
                return sceneMouseMove ??
                    (sceneMouseMove = new RelayCommand(MouseMove));
            }
        }
    }
}
