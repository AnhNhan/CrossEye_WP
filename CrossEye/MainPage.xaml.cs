using Microsoft.Devices;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Storage;
using CrossEye.Resources;

namespace CrossEye
{
    public partial class MainPage : PhoneApplicationPage
    {
        private WriteableBitmap currentImage;
        private PhotoCamera camera;
        private bool capturingLeft;

        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            InitializeCamera();
        }

        #region camera

        void InitializeCamera()
        {
            camera = new PhotoCamera(CameraType.Primary);
            camera.CaptureImageAvailable += camera_CaptureImageAvailable;
            camera.CaptureCompleted += camera_CaptureCompleted;

            CameraButtons.ShutterKeyPressed += cameraButtons_ShutterKeyPressed;
        }

        void CleanUpCamera()
        {
            CameraButtons.ShutterKeyPressed -= cameraButtons_ShutterKeyPressed;
            camera.CaptureImageAvailable -= camera_CaptureImageAvailable;
            camera.CaptureCompleted -= camera_CaptureCompleted;
            camera.Dispose();
            camera = null;
        }

        private void cameraButtons_ShutterKeyPressed(object sender, EventArgs e)
        {
            camera.CaptureImage();
        }

        void camera_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                currentImage = PictureDecoder.DecodeJpeg(e.ImageStream);
                if (capturingLeft)
                {
                    leftRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
                else
                {
                    rightRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
            });
        }

        void camera_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            if (!e.Succeeded)
            {
                if (capturingLeft)
                {
                    leftRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
                else
                {
                    rightRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
            }
            CleanUpCamera();
        }

        #endregion

        void button1_Click(object sender, EventArgs e)
        {
            capturingLeft = true;
            var task = new CameraCaptureTask();
            task.Completed += chooserTask_Completed;
            task.Show();
        }

        void button2_Click(object sender, EventArgs e)
        {
            capturingLeft = false;
            var task = new CameraCaptureTask();
            task.Completed += chooserTask_Completed;
            task.Show();
        }

        void chooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                currentImage = PictureDecoder.DecodeJpeg(e.ChosenPhoto);

                if (capturingLeft)
                {
                    leftRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
                else
                {
                    rightRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
            }
            else
            {
                if (capturingLeft)
                {
                    leftRect.Fill = new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    rightRect.Fill = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        void Save_Click(object sender, EventArgs e)
        {
            // Do something here
        }
    }
}