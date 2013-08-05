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
using System.Windows.Shapes;
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
            // User cancelled, keep current stuff by not progressing
            if (e.TaskResult == TaskResult.Cancel)
            {
                return;
            }

            bool error = false;
            try
            {
                // First try jpg
                currentImage = PictureDecoder.DecodeJpeg(e.ChosenPhoto);
            }
            catch (Exception exc)
            {
                // Then try png
                try
                {
                }
                catch (Exception exc2)
                {
                    error = true;
                }
            }

            if (e.TaskResult == TaskResult.OK && !error)
            {
                if (capturingLeft)
                {
                    resizeRectangle(leftRect, ContentPanel, currentImage);
                    leftImage = currentImage;
                    leftRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
                else
                {
                    resizeRectangle(rightRect, ContentPanel, currentImage);
                    rightImage = currentImage;
                    rightRect.Fill = new ImageBrush { ImageSource = currentImage };
                }
            }
            else
            {
                MessageBox.Show("I'm sorry, but the file " + e.OriginalFileName + " does not look like an image to me.", "Something went wrong :(", MessageBoxButton.OK);
                currentImage = null;
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

        void resizeRectangle(Rectangle rectangle, Grid grid, WriteableBitmap bitmap)
        {
            Size bitmapSize = new Size(bitmap.PixelWidth, bitmap.PixelWidth);
            Size gridSize = new Size(grid.ActualWidth, grid.ActualHeight);
            Size rectOldSize = new Size(gridSize.Width / 2 - rectangle.Margin.Left - rectangle.Margin.Right, gridSize.Height);
            Size rectNewSize = new Size();

            double ratio = bitmapSize.Width / bitmapSize.Height;
            double sizeRatio = bitmapSize.Height / grid.ActualHeight;
            bool isHighPhoto = bitmapSize.Height > bitmapSize.Width;
            bool isOverlyHigh = gridSize.Height * ratio > gridSize.Height;

            if (isHighPhoto)
            {
                rectNewSize.Height = gridSize.Height;
                rectNewSize.Width = rectNewSize.Height * ratio;
            }
            else
            {
                rectNewSize.Width = gridSize.Width / 2 - rectangle.Margin.Left - rectangle.Margin.Right;
                rectNewSize.Height = rectNewSize.Width * ratio;
            }

            rectangle.MaxWidth = rectNewSize.Width;
            rectangle.MaxHeight = rectNewSize.Height;
            rectangle.MinWidth = rectNewSize.Width;
            rectangle.MinHeight = rectNewSize.Height;

            dudu.Text = string.Format("r-w: {0}\tr-h: {1}\nb-w: {2}\tb-h: {3}", rectNewSize.Width, rectNewSize.Height, bitmapSize.Width, bitmapSize.Height);
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