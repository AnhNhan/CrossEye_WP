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
        private WriteableBitmap leftImage;
        private WriteableBitmap rightImage;
        private bool capturingLeft;

        // Konstruktor
        public MainPage()
        {
            InitializeComponent();
        }

        void button1_Click(object sender, EventArgs e)
        {
            imageClick(true, new CameraCaptureTask());
        }

        void button2_Click(object sender, EventArgs e)
        {
            imageClick(false, new CameraCaptureTask());
        }

        void choose1_Click(object sender, EventArgs e)
        {
            imageClick(true, new PhotoChooserTask());
        }

        void choose2_Click(object sender, EventArgs e)
        {
            imageClick(false, new PhotoChooserTask());
        }

        private void imageClick(bool left, ChooserBase<PhotoResult> task)
        {
            capturingLeft = left;
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
                    leftImage = currentImage;
                    leftRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
                else
                {
                    rightImage = currentImage;
                    rightRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
            }
            else
            {
                if (capturingLeft)
                {
                    leftImage = null;
                    leftRect.Fill = new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    rightImage = null;
                    rightRect.Fill = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        void Save_Click(object sender, EventArgs e)
        {
            WriteableBitmap finalImg = RenderFinalImage();
            if (finalImg != null)
            {
                MessageBox.Show("Saving not supported", "Sorry, we can't save yet, I'm too lazy to program it :(", MessageBoxButton.OK);
                return;

                // TODO: Ask about filename
                string fileName = "customphoto.jpg";
                using (var stream = new MemoryStream())
                {
                    finalImg.SaveJpeg(stream, currentImage.PixelWidth, currentImage.PixelHeight, 0, 100);
                    stream.Seek(0, 0);
                    var library = new MediaLibrary();
                    Picture p = library.SavePicture(fileName, stream);

                    // TODO: Toast notification about saved picture
                }
            }
            else
            {
                MessageBox.Show("You require both images!", "Saving failed", MessageBoxButton.OK);
            }
        }

        void Swap_Click(object sender, EventArgs e)
        {
            Brush tmpBrush = leftRect.Fill;
            leftRect.Fill = rightRect.Fill;
            rightRect.Fill = tmpBrush;
        }

        WriteableBitmap RenderFinalImage()
        {
            if (leftImage == null || rightImage == null)
                return null;

            int width = leftImage.PixelWidth + rightImage.PixelWidth + 10;
            int height = leftImage.PixelHeight;
            WriteableBitmap finalImg = new WriteableBitmap(width, height);

            // TODO: Actually create the image

            return finalImg;
        }
    }
}