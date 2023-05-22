using AlgebraicFractals;
using Microsoft.Win32;
using SuperFractal.Commands;
using SuperFractal.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SuperFractal.ViewModels
{
    public class SaveImageViewModel : ViewModelBase
    {
        protected NavigationStore _navigation;
        protected FractalStore _fractalStore;
        protected SceneViewModel PrevViewModel;
        WriteableBitmap Scene { get; init; }
        public SaveImageViewModel(SceneViewModel scene, FractalStore fractalStore, NavigationStore navigation)
        {
            Scene = scene.Scene;
            _fractalStore = fractalStore;
            _navigation = navigation;
            ImageWidth = (int)Scene.Width;
            ImageHeight = (int)Scene.Height;
        }

        private int imageWidth;
        public int ImageWidth
        {
            get
            {
                return imageWidth;
            }
            set
            {
                if (value <= 0 || value > 10000) return;
                imageWidth = value;
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        private int imageHeight;
        public int ImageHeight
        {
            get
            {
                return imageHeight;
            }
            set
            {
                if (value <= 0 || value > 10000) return;
                imageHeight = value;
                OnPropertyChanged(nameof(ImageHeight));
            }
        }


        private void StartFileDialog(WriteableBitmap scene)
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
            if (saveImageDialog.ShowDialog() ?? false)
            {
                if (saveImageDialog.FileName == "") return;
                using (var fs = (System.IO.FileStream)saveImageDialog.OpenFile())
                {
                    switch (saveImageDialog.FilterIndex)
                    {
                        case 1:
                            JpegBitmapEncoder encoder1 = new JpegBitmapEncoder();
                            encoder1.Frames.Add(BitmapFrame.Create(scene));
                            encoder1.Save(fs);
                            break;

                        case 2:
                            PngBitmapEncoder encoder2 = new PngBitmapEncoder();
                            encoder2.Frames.Add(BitmapFrame.Create(scene));
                            encoder2.Save(fs);
                            break;
                    }

                }
            }
        }
        private void SaveImage(object e)
        {
            var progresBar = e as ProgressBar;
            WriteableBitmap scene = Scene;
            if (ImageWidth != (int)Scene.Width && ImageHeight != scene.Height)
            {
              
                scene = BitmapFactory.New(ImageWidth,ImageHeight);
                var imageTL = new Coord<int>(0, 0);
                var imageBR = new Coord<int>(ImageWidth, ImageHeight);
                var buffer = new int[ImageWidth * ImageHeight];
                AlgebraicFractal.CreateMultiFractalIntrinsicsInThreadPool(_fractalStore.Fractals, buffer,
                    ImageWidth, imageTL, imageBR, _fractalStore.MaxIterations);
                scene.WritePixels(new Int32Rect(0, 0, ImageWidth, ImageHeight), buffer, scene.BackBufferStride, 0);
            }
            //progresBar.Visibility = Visibility.Hidden;
            StartFileDialog(scene);
        }

        private RelayCommand saveImage;
        public RelayCommand SaveImageCommand
        {
            get
            {
                return saveImage ??
                    (saveImage = new RelayCommand(SaveImage));
            }
        }
       

    }
}
