﻿using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FaceDetection
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [System.Runtime.InteropServices.DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop
                  .Imaging.CreateBitmapSourceFromHBitmap(
                  ptr,
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        private CascadeClassifier _faceclassifier = new CascadeClassifier(@"haarcascade_frontalface_default.xml");

        private void ProcessFrame()
        {
            Image<Bgr, Byte> ImageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>();  //line 1
            ImageFrame = DetectFaces(ImageFrame);
            image.Source = ToBitmapSource(ImageFrame);  //line 2
        }

        private Image<Bgr, Byte> DetectFaces(Image<Bgr, Byte> myImage)
        {
            var grayFrame = myImage.Convert<Gray, byte>();
            var myFaces = _faceclassifier.DetectMultiScale(grayFrame);
            var noFaces = myFaces.Count();
            label.Content = noFaces;
            foreach (var face in myFaces)
                myImage.Draw(face, new Bgr(100, 100, 100), 3);
            return myImage;
        }

        private Capture _capture = new Capture();
        DispatcherTimer timer;

        void timer_Tick(object sender, EventArgs e)
        {
            ProcessFrame();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
        }
    }
}
