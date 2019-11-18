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

using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Windows.Interop;

using HolaOpenCV;

namespace FiltroDeCaras
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool running = false;

        Image<Bgr, byte> current;

        VideoCapture webcam = null;

		int iB2C = 1;
		Image<Bgr, Byte> ImageHSVwheel = new Image<Bgr, Byte>("D:/images/hsv.jpg");
		public MainWindow()
        {
            InitializeComponent();
			imgHSV.Source = BitmapConverter.ToBitmapSource(ImageHSVwheel);
		}

        private void btn_Capturar_Click(object sender, RoutedEventArgs e)
        {
            if (!running)
            {
                if (webcam == null)
                {
                    webcam = new VideoCapture(0);
                    ComponentDispatcher.ThreadIdle += new System.EventHandler (ComponentDispatcher_ThreadIdle);
                }
                running = true;
            }
        }

		private void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
		{
			//tomar un frame de la webcam
			current = webcam.QueryFrame().ToImage<Bgr, byte>();
			//Hacer flip horizontal a la imagen
			current = current.Flip(FlipType.Horizontal);

			//convertir a escala de grises
			Image<Gray, byte> gris = current.Convert<Gray, byte>(); 

            imgOriginal.Source = BitmapConverter.ToBitmapSource(current);
			ImageProcessing();
		}

		private void ImageProcessing()
		{
			Image<Gray, Byte> ImageFrameDetection = cvAndHsvImage(
				current,
			   Convert.ToInt32(sld_HL.Value), Convert.ToInt32(sld_HH.Value),
			   Convert.ToInt32(sld_SL.Value), Convert.ToInt32(sld_SH.Value),
			   Convert.ToInt32(sld_VL.Value), Convert.ToInt32(sld_VH.Value));



			if (iB2C == 0) imgResult.Source = BitmapConverter.ToBitmapSource(ImageFrameDetection);

			if (iB2C == 1)
			{
				Image<Bgr, Byte> imgF = new Image<Bgr, Byte>(current.Width, current.Height);
				Image<Bgr, Byte> imgD = ImageFrameDetection.Convert<Bgr, Byte>();
				CvInvoke.BitwiseAnd(current, imgD, imgF, null);
				imgResult.Source = BitmapConverter.ToBitmapSource(imgF);
			}

			if (iB2C == 2)
			{
				Image<Bgr, Byte> imgF = new Image<Bgr, Byte>(current.Width, current.Height);
				Image<Bgr, Byte> imgD = ImageFrameDetection.Convert<Bgr, Byte>();
				CvInvoke.BitwiseAnd(current, imgD, imgF);
				for (int x = 0; x < imgF.Width; x++)
					for (int y = 0; y < imgF.Height; y++)
					{
						{
							Bgr c = imgF[y, x];
							if (c.Red == 0 && c.Blue == 0 && c.Green == 0)
							{
								imgF[y, x] = new Bgr(255, 255, 255);
							}
						}
					}

				imgResult.Source = BitmapConverter.ToBitmapSource(imgF);

			}

			Image<Gray, Byte> ImageHSVwheelResult = cvAndHsvImage(
			   ImageHSVwheel,
			   Convert.ToInt32(sld_HL.Value), Convert.ToInt32(sld_HH.Value),
			   Convert.ToInt32(sld_SL.Value), Convert.ToInt32(sld_SH.Value),
			   Convert.ToInt32(sld_VL.Value), Convert.ToInt32(sld_VH.Value));

			Image<Bgr, Byte> imgF2 = new Image<Bgr, Byte>(ImageHSVwheel.Width, ImageHSVwheel.Height);
			Image<Bgr, Byte> imgD2 = ImageHSVwheelResult.Convert<Bgr, Byte>();
			CvInvoke.BitwiseAnd(ImageHSVwheel, imgD2, imgF2);
			for (int x = 0; x < imgF2.Width; x++)
				for (int y = 0; y < imgF2.Height; y++)
				{
					{
						Bgr c = imgF2[y, x];
						if (c.Red == 0 && c.Blue == 0 && c.Green == 0)
						{
							imgF2[y, x] = new Bgr(255, 255, 255);
						}
					}
				}

			imgHSV.Source = BitmapConverter.ToBitmapSource(imgF2);
		}

		private Image<Gray, Byte> cvAndHsvImage(Image<Bgr, Byte> imgFame, int L1, int H1, int L2, int H2, int L3, int H3)
		{
			Image<Hsv, Byte> hsvImage = imgFame.Convert<Hsv, Byte>();

			Image<Gray, Byte> ResultImage = new Image<Gray, Byte>(hsvImage.Width, hsvImage.Height);
			Image<Gray, Byte> ResultImageH = new Image<Gray, Byte>(hsvImage.Width, hsvImage.Height);
			Image<Gray, Byte> ResultImageS = new Image<Gray, Byte>(hsvImage.Width, hsvImage.Height);
			Image<Gray, Byte> ResultImageV = new Image<Gray, Byte>(hsvImage.Width, hsvImage.Height);

			CvInvoke.InRange(hsvImage, new ScalarArray(new MCvScalar(L1, L2, L3)),
						   new ScalarArray(new MCvScalar(H1, H2, H3)), ResultImage);

			return ResultImage;
		}


		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (sld_VH != null)
			{
				lbl_HL.Content = (int)sld_HL.Value;
				lbl_SL.Content = (int)sld_SL.Value;
				lbl_VL.Content = (int)sld_VL.Value;
				lbl_HH.Content = (int)sld_HH.Value;
				lbl_SH.Content = (int)sld_SH.Value;
				lbl_VH.Content = (int)sld_VH.Value;
			}
		}
		
		private void btn_cambiar_Click(object sender, RoutedEventArgs e)
		{
			iB2C++;
			if (iB2C > 2) iB2C = 0;
		}
	}
	

}
