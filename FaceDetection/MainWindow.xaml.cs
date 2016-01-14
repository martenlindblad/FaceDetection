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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var iHeight = image.Height;
            var iWidth = image.Width;
            var myCapture = new Capture();
            var myImage = myCapture.QueryFrame().ToImage<Bgr, Byte>();
            var grayFrame = myImage.Convert<Gray, byte>();
            CascadeClassifier myClassifier = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
            var myFaces = myClassifier.DetectMultiScale(grayFrame);
            foreach (var face in myFaces)
                myImage.Draw(face, new Bgr(0, double.MaxValue, 0), 3);
            image.Source = ToBitmapSource(myImage);

        }
    }
}