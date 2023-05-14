using AlgebraicFractals;
using Microsoft.Win32;
using SuperFractal.Commands;
using SuperFractal.Stores;
using SuperFractal.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Media.Media3D;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace SuperFractal.ViewModels
{
    public class SceneViewModel : ViewModelBase
    {
        public FractalStore FractalStore { get; init; }
        public SceneViewModel(FractalStore fractalStore) 
        {
            FractalStore = fractalStore;
            DrawingMethods = fractalStore.MultiFractalsCreators;
            Timer.Start(); 
        }


        #region Scene
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
        private int SceneWidth { get; set; } = 0;
        private int SceneHeight { get; set; } = 0;
        private int[] ImageBuffer { get; set; } = new int[0];

        private Coord<int> ImageTopLeft = new Coord<int>(0, 0);
        private Coord<int> ImageBottomRight = new Coord<int>(0, 0);
        public void ChangeSceneSize(int width, int height)
        {
            SceneHeight = height;
            SceneWidth = width;
            ImageBuffer = new int[SceneWidth * SceneHeight];
            ImageBottomRight = new Coord<int>(SceneWidth,SceneHeight);
            Scene = BitmapFactory.New(width,height);
            UpdateScene();
        }
        #endregion

        #region PanAndZoom
        public Coord<double> worldScale { get; set; }

        public Coord<double> worldOffset { get; set; } = new Coord<double>(0, 0);

        public Coord<double> worldStartPan { get; set; } = new Coord<double>(0, 0);

        public Coord<int> WorldToScene(Coord<double> world)
        {
            int x, y = 0;
            x = (int)((world.X - worldOffset.X) * worldScale.X);
            y = (int)((world.Y - worldOffset.Y) * worldScale.Y);
            return new Coord<int>(x, y);
        }

        public Coord<double> SceneToWorld(Coord<int> screen)
        {
            double x, y = 0;
            x = (double)(screen.X) / worldScale.X + worldOffset.X;
            y = (double)(screen.Y) / worldScale.Y + worldOffset.Y;
            return new Coord<double>(x, y);
        }
        private Coord<double> mouseInWorldPos;
        public Coord<double> MousePosInWorld
        {
            get
            {
                return mouseInWorldPos;
            }
            set
            {
                mouseInWorldPos = value;
                OnPropertyChanged(nameof(MousePosInWorld));
            }
        }
        #endregion

        #region TimeClock
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
				_frameDrawingTime = value + "c";
				OnPropertyChanged(nameof(FrameDrawingTime));
			}
		}
        #endregion

        #region Drawing

        private int maxIteration;
        public int MaxIter
        {
            get
            {
                return FractalStore.MaxIterations;
            }
            set
            {
                if (value <= 0) return;
                FractalStore.MaxIterations = value;
                OnPropertyChanged(nameof(MaxIter));
            }
        }


        private Dictionary<string,CreateMultiFractal> drawingMethods;
        public Dictionary<string, CreateMultiFractal> DrawingMethods
        {
            get
            {
                return drawingMethods;
            }
            set
            {
                drawingMethods = value;
                OnPropertyChanged(nameof(DrawingMethods));
            }
        }
        private void UpdateScene()
        {
            PreviousTick = Timer.Elapsed;
            DrawFrame();
            FrameDrawingTime = ElapsedSecondsSinceLastTick.ToString();
        }
        private void DrawFrame()
        {
            Scene.Lock();
            foreach(var fractal in FractalStore.Fractals)
            {
                if (fractal == null) continue;
                fractal.TopLeft = SceneToWorld(ImageTopLeft);
                fractal.BottomRight = SceneToWorld(ImageBottomRight);
            }
            if (FractalStore.Fractals.Count > 0)
            {
                FractalStore.MultiFractalCreator(FractalStore.Fractals,
                                               ImageBuffer,
                                               SceneWidth,
                                               ImageTopLeft,
                                               ImageBottomRight,
                                               FractalStore.MaxIterations);
                Scene.WritePixels(new Int32Rect(0, 0, SceneWidth, SceneHeight), ImageBuffer, Scene.BackBufferStride, 0);
            }
            else
            {
                Scene.Clear(Colors.Black);
            }
            Scene.Unlock();
        }
        #endregion

        #region Commands
        private void Loaded(object parametr)
        {
            var obj = parametr as RoutedEventArgs;
            if (obj == null) return;
            var container = obj.Source as Grid;
            if (container == null) return;
            var width = (int)container.ActualWidth;
            var height = (int)container.ActualHeight;
            worldScale = new Coord<double>(1280/2.0, 720);
            ChangeSceneSize(width, height);
        }



		private void UpdateSceneSize(object parametr)
		{
            var obj = parametr as SizeChangedEventArgs;
            if (obj is null) return;
            ChangeSceneSize((int)obj.NewSize.Width, (int)obj.NewSize.Height);
        }
        private double scale = 100;
        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }

        private void MouseWheel(object parametr)
        {
            var e = parametr as MouseWheelEventArgs; 
            if (e == null) return;
            var mousePos = e.GetPosition((IInputElement)e.Source);
            var mouseBeforeZoom = SceneToWorld(new Coord<int>((int)mousePos.X, (int)mousePos.Y));
            if (e.Delta > 0)
            {
                worldScale = worldScale * 1.1;
                
            }
            else
            {
                worldScale = worldScale * 0.9;  
            }
            Scale = ((worldScale.Y / 720.0) * 100);
            AutoIteration?.Invoke();
           
            var mouseAfterZoom = SceneToWorld(new Coord<int>((int)mousePos.X, (int)mousePos.Y));
            worldOffset = worldOffset + (mouseBeforeZoom - mouseAfterZoom);
            MousePosInWorld = SceneToWorld(new Coord<int>((int)mousePos.X, (int)mousePos.Y));
            UpdateScene();
        }

        private void PreviewMouseDown(object parametr)
        {
            var e = parametr as MouseButtonEventArgs;
            if (e == null) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition((IInputElement)e.Source);
                worldStartPan = new Coord<double>(mousePos.X, mousePos.Y);
            }
            if(e.RightButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition((IInputElement)e.Source);
                MousePosInWorld = SceneToWorld(new Coord<int>((int)mousePos.X, (int)mousePos.Y));
            }
        }
        private void MouseDown(object parametr)
        {
            var e = parametr as MouseButtonEventArgs;
            if (e == null) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition((IInputElement)e.Source);
                var mouse = new Coord<double>(mousePos.X, mousePos.Y);
                worldOffset -= (mouse - worldStartPan) / worldScale;
                worldStartPan = mouse;
                MousePosInWorld = SceneToWorld(new Coord<int>((int)mouse.X, (int)mouse.Y));
            }
            
        }
        private void MouseMove(object parametr)
        {
            var e = parametr as MouseEventArgs;
            if (e == null) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition((IInputElement)e.Source);
                var mouse = new Coord<double>(mousePos.X, mousePos.Y);
                double x = worldOffset.X
                       - ((mouse.X - worldStartPan.X) / worldScale.X);
                double y = worldOffset.Y
                    - ((mouse.Y - worldStartPan.Y) / worldScale.Y);
                worldOffset = new Coord<double>(x, y);
                worldStartPan = mouse;
                UpdateScene();
            }
        }

        private void SaveSceneToFile(object e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);      
            saveImageDialog.Title = "Save an image file";
            saveImageDialog.CheckFileExists = false;
            saveImageDialog.CheckPathExists = true;
            saveImageDialog.DefaultExt = "jpg";
            saveImageDialog.Filter = "JPeg Image|*.jpg|Png Image|*.png";
            saveImageDialog.FilterIndex = 2;
            saveImageDialog.RestoreDirectory = true;
            saveImageDialog.OverwritePrompt = true;
            saveImageDialog.CreatePrompt = false;
            if (saveImageDialog.ShowDialog()?? false)
            {
                if (saveImageDialog.FileName == "") return;
                using (var fs = (System.IO.FileStream)saveImageDialog.OpenFile())
                {
                    switch (saveImageDialog.FilterIndex)
                    {
                        case 1:
                            JpegBitmapEncoder encoder1 = new JpegBitmapEncoder();
                            encoder1.Frames.Add(BitmapFrame.Create(Scene));
                            encoder1.Save(fs);
                            break;

                        case 2:
                            PngBitmapEncoder encoder2 = new PngBitmapEncoder();
                            encoder2.Frames.Add(BitmapFrame.Create(Scene));
                            encoder2.Save(fs);
                            break;
                    }

                }
            }
        }

        private void LoadNewWindowToEditScene(object e)
        {
            NavigationStore navigation = new NavigationStore();
            navigation.CurrentViewModel = new FractalCollectionViewModel(MousePosInWorld,FractalStore, navigation);
            EditSceneWindow editScene = new EditSceneWindow()
            {
                DataContext = new EditSceneViewModel(navigation)
            };
            editScene.ShowDialog();
            UpdateScene();
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

        private RelayCommand sceneLoaded;
        public RelayCommand SceneLoaded
        {
            get
            {
                return sceneLoaded ??
                    (sceneLoaded = new RelayCommand(Loaded));
            }
        }

        private RelayCommand comboBoxLoaded;
        public RelayCommand ComboBoxLoaded
        {
            get
            {
                return comboBoxLoaded ??
                    (comboBoxLoaded = new RelayCommand(
                         e =>
                         {
                             var obj = e as RoutedEventArgs;
                             if (obj == null) return;
                             var comboBox = obj.Source as ComboBox;
                             comboBox.SelectedIndex = 3;
                         }));
            }
        }

        private RelayCommand drawingMethodSelectionChanged;
        public RelayCommand DrawingMethodSelectionChanged
        {
            get
            {
                return drawingMethodSelectionChanged ??
                    (drawingMethodSelectionChanged = new RelayCommand(
                         e =>
                         {
                             var obj = e as SelectionChangedEventArgs;
                             if (obj == null) return;
                             FractalStore.MultiFractalCreator = FractalStore.MultiFractalsCreators[obj.AddedItems[0] as string];
                         }));
            }
        }

        private RelayCommand saveScene;
        public RelayCommand SaveScene
        {
            get
            {
                return saveScene ??
                    (saveScene = new RelayCommand(SaveSceneToFile));
            }
        }

        private RelayCommand editScene;
        public RelayCommand EditScene
        {
            get
            {
                return editScene ??
                    (editScene = new RelayCommand(LoadNewWindowToEditScene));
            }
        }

        private event Action AutoIteration;

        private void AutoIncrementIter()
        {
            MaxIter = (int)(32 * Math.Pow(Math.Log10(Scale), 1.2));
        }

        private RelayCommand autoIterationChecked;
        public RelayCommand AutoIterationChecked
        {
            get
            {
                return autoIterationChecked ??
                    (autoIterationChecked
                    = new RelayCommand(
                        e=>
                        {
                            AutoIteration += AutoIncrementIter;
                        }
                        ));
            }
        }

        private RelayCommand autoIterationUnchecked;
        public RelayCommand AutoIterationUnchecked
        {
            get
            {
                return autoIterationUnchecked ??
                    (autoIterationUnchecked
                    = new RelayCommand(
                        e =>
                        {
                            AutoIteration-= AutoIncrementIter;
                        }
                        ));
            }
        }
        #endregion
    }
}
