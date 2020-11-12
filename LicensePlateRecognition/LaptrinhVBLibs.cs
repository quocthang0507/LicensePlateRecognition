using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using tesseract;

namespace LPR_Laptrinhvb
{
    /// <summary>
    /// Laptrinhvb.net - Mr.Cùi bắp (Freelancer.Bsoft@gmail.com)
    /// <para>Các lớp xử lý và nhận dạng chữ, số</para>
    /// </summary>
    public class clsBSoft
    {
        private static int count = 0;
        private static string _caption = "http://www.laptrinhvb.net";

        /// <summary>
        /// Method used to process the image and set the output result images.
        /// </summary>
        /// <param name="colorImage">Source color image.</param>
        /// <param name="thresholdValue">Value used for thresholding.</param>
        /// <param name="processedGray">Resulting gray image.</param>
        /// <param name="processedColor">Resulting color image.</param>
        public static int IdentifyContours(
          Bitmap colorImage,
          int thresholdValue,
          bool invert,
          out Bitmap processedGray,
          out Bitmap processedColor,
          out List<Rectangle> list)
        {
            List<Rectangle> rectangleList = new List<Rectangle>();
            Image<Gray, byte> src = new Image<Gray, byte>(colorImage);
            Image<Gray, byte> image1 = new Image<Gray, byte>(src.Width, src.Height);
            Image<Bgr, byte> image2 = new Image<Bgr, byte>(colorImage);
            double num1 = clsBSoft.cout_avg_new(src);
            if (num1 == 0.0)
                num1 = src.GetAverage().Intensity;
            double num2 = 0.1;
            double num3 = 10.0;
            Rectangle[] array = new Rectangle[9];
            Image<Bgr, byte> image3 = new Image<Bgr, byte>(colorImage);
            Image<Gray, byte> image4 = src;
            Image<Gray, byte> image5 = image1;
            int num4 = 0;
            for (int index1 = 2; index1 < 10; ++index1)
            {
                for (double num5 = num2; num5 <= num3; num5 += 0.1)
                {
                    Image<Bgr, byte> image6 = new Image<Bgr, byte>(colorImage);
                    Image<Gray, byte> image7 = image1;
                    rectangleList.Clear();
                    int num6 = 0;
                    double intensity = num1 / num5;
                    Image<Gray, byte> image8 = src.ThresholdBinary(new Gray(intensity), new Gray((double)byte.MaxValue));
                    using (MemStorage stor = new MemStorage())
                    {
                        for (Contour<Point> contour = image8.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_LIST, stor); contour != null; contour = contour.HNext)
                        {
                            Rectangle boundingRectangle = contour.BoundingRectangle;
                            CvInvoke.cvDrawContours((IntPtr)(UnmanagedObject)image6, (IntPtr)(Seq<Point>)contour, new MCvScalar((double)byte.MaxValue, (double)byte.MaxValue, 0.0), new MCvScalar(0.0), -1, 1, LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                            double num7 = (double)boundingRectangle.Width / (double)boundingRectangle.Height;
                            if (boundingRectangle.Width > 20 && boundingRectangle.Width < 150 && (boundingRectangle.Height > 80 && boundingRectangle.Height < 150) && num7 > 0.2 && num7 < 1.1)
                            {
                                ++num6;
                                CvInvoke.cvDrawContours((IntPtr)(UnmanagedObject)image6, (IntPtr)(Seq<Point>)contour, new MCvScalar(0.0, (double)byte.MaxValue, (double)byte.MaxValue), new MCvScalar((double)byte.MaxValue), -1, 3, LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                                image6.Draw(contour.BoundingRectangle, new Bgr(0.0, (double)byte.MaxValue, 0.0), 2);
                                image7.Draw((Seq<Point>)contour, new Gray((double)byte.MaxValue), -1);
                                rectangleList.Add(contour.BoundingRectangle);
                            }
                        }
                    }
                    double num8 = 0.0;
                    double num9 = 0.0;
                    Rectangle rectangle;
                    for (int index2 = 0; index2 < num6; ++index2)
                    {
                        double num7 = num8;
                        rectangle = rectangleList[index2];
                        double height1 = (double)rectangle.Height;
                        num8 = num7 + height1;
                        for (int index3 = index2 + 1; index3 < num6; ++index3)
                        {
                            rectangle = rectangleList[index3];
                            int x1 = rectangle.X;
                            rectangle = rectangleList[index2];
                            int x2 = rectangle.X;
                            rectangle = rectangleList[index2];
                            int width1 = rectangle.Width;
                            int num10 = x2 + width1;
                            int num11;
                            if (x1 < num10)
                            {
                                rectangle = rectangleList[index3];
                                int x3 = rectangle.X;
                                rectangle = rectangleList[index2];
                                int x4 = rectangle.X;
                                if (x3 > x4)
                                {
                                    rectangle = rectangleList[index3];
                                    int y1 = rectangle.Y;
                                    rectangle = rectangleList[index2];
                                    int y2 = rectangle.Y;
                                    rectangle = rectangleList[index2];
                                    int width2 = rectangle.Width;
                                    int num12 = y2 + width2;
                                    if (y1 < num12)
                                    {
                                        rectangle = rectangleList[index3];
                                        int y3 = rectangle.Y;
                                        rectangle = rectangleList[index2];
                                        int y4 = rectangle.Y;
                                        num11 = y3 <= y4 ? 1 : 0;
                                        goto label_21;
                                    }
                                    else
                                    {
                                        num11 = 1;
                                        goto label_21;
                                    }
                                }
                            }
                            num11 = 1;
                        label_21:
                            if (num11 == 0)
                            {
                                rectangleList.RemoveAt(index3);
                                --num6;
                                --index3;
                            }
                            else
                            {
                                rectangle = rectangleList[index2];
                                int x3 = rectangle.X;
                                rectangle = rectangleList[index3];
                                int x4 = rectangle.X;
                                rectangle = rectangleList[index3];
                                int width2 = rectangle.Width;
                                int num12 = x4 + width2;
                                int num13;
                                if (x3 < num12)
                                {
                                    rectangle = rectangleList[index2];
                                    int x5 = rectangle.X;
                                    rectangle = rectangleList[index3];
                                    int x6 = rectangle.X;
                                    if (x5 > x6)
                                    {
                                        rectangle = rectangleList[index2];
                                        int y1 = rectangle.Y;
                                        rectangle = rectangleList[index3];
                                        int y2 = rectangle.Y;
                                        rectangle = rectangleList[index3];
                                        int width3 = rectangle.Width;
                                        int num14 = y2 + width3;
                                        if (y1 < num14)
                                        {
                                            rectangle = rectangleList[index2];
                                            int y3 = rectangle.Y;
                                            rectangle = rectangleList[index3];
                                            int y4 = rectangle.Y;
                                            num13 = y3 <= y4 ? 1 : 0;
                                            goto label_29;
                                        }
                                        else
                                        {
                                            num13 = 1;
                                            goto label_29;
                                        }
                                    }
                                }
                                num13 = 1;
                            label_29:
                                if (num13 == 0)
                                {
                                    double num14 = num8;
                                    rectangle = rectangleList[index2];
                                    double height2 = (double)rectangle.Height;
                                    num8 = num14 - height2;
                                    rectangleList.RemoveAt(index2);
                                    --num6;
                                    --index2;
                                    break;
                                }
                            }
                        }
                    }
                    double num15 = num8 / (double)num6;
                    for (int index2 = 0; index2 < num6; ++index2)
                    {
                        double num7 = num9;
                        double num10 = num15;
                        rectangle = rectangleList[index2];
                        double height = (double)rectangle.Height;
                        double num11 = Math.Abs(num10 - height);
                        num9 = num7 + num11;
                    }
                    if (num6 <= 8 && num6 > 1 && num6 > num4 && num9 <= (double)(num6 * index1))
                    {
                        rectangleList.CopyTo(array);
                        num4 = num6;
                        image3 = image6;
                        image5 = image7;
                        image4 = image8;
                    }
                }
                if (num4 == 8)
                    break;
            }
            clsBSoft.count = num4;
            Image<Gray, byte> image9 = image4;
            Image<Bgr, byte> image10 = image3;
            rectangleList.Clear();
            for (int index = 0; index < array.Length; ++index)
            {
                if (array[index].Height != 0)
                    rectangleList.Add(array[index]);
            }
            processedColor = image10.ToBitmap();
            processedGray = image9.ToBitmap();
            list = rectangleList;
            return clsBSoft.count;
        }

        private static double cout_avg(Image<Gray, byte> src)
        {
            double num = 0.0;
            List<Rectangle> rectangleList = new List<Rectangle>();
            Image<Gray, byte> image1 = new Image<Gray, byte>(src.Width, src.Height);
            CvInvoke.cvAdaptiveThreshold((IntPtr)(UnmanagedObject)src, (IntPtr)(UnmanagedObject)image1, (double)byte.MaxValue, ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C, THRESH.CV_THRESH_BINARY, 21, 2.0);
            Image<Gray, byte> image2 = image1.Dilate(3).Erode(3);
            using (MemStorage stor = new MemStorage())
            {
                for (Contour<Point> contour = image2.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_LIST, stor); contour != null; contour = contour.HNext)
                {
                    Rectangle boundingRectangle = contour.BoundingRectangle;
                    if (boundingRectangle.Width > 50 && boundingRectangle.Width < 150 && boundingRectangle.Height > 80 && boundingRectangle.Height < 150)
                        rectangleList.Add(boundingRectangle);
                }
            }
            for (int index = 0; index < rectangleList.Count; ++index)
            {
                Bitmap bitmap = src.ToBitmap();
                Image<Gray, byte> image3 = new Image<Gray, byte>(bitmap.Clone(rectangleList[index], bitmap.PixelFormat));
                num += image3.GetAverage().Intensity / (double)rectangleList.Count;
            }
            return num;
        }

        private static double cout_avg_new(Image<Gray, byte> src)
        {
            double num1 = 0.0;
            List<Rectangle> rectangleList = new List<Rectangle>();
            Image<Gray, byte> image1 = new Image<Gray, byte>(src.Width, src.Height);
            CvInvoke.cvAdaptiveThreshold((IntPtr)(UnmanagedObject)src, (IntPtr)(UnmanagedObject)image1, (double)byte.MaxValue, ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C, THRESH.CV_THRESH_BINARY, 21, 2.0);
            Image<Gray, byte> image2 = image1.Dilate(3).Erode(3);
            using (MemStorage stor = new MemStorage())
            {
                for (Contour<Point> contour = image2.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_LIST, stor); contour != null; contour = contour.HNext)
                {
                    Rectangle boundingRectangle = contour.BoundingRectangle;
                    if (boundingRectangle.Width > 50 && boundingRectangle.Width < 150 && boundingRectangle.Height > 80 && boundingRectangle.Height < 150)
                        rectangleList.Add(boundingRectangle);
                }
            }
            try
            {
                for (int index1 = 0; index1 < rectangleList.Count; ++index1)
                {
                    Bitmap bitmap = src.ToBitmap();
                    Image<Gray, byte> image3 = new Image<Gray, byte>(bitmap.Clone(rectangleList[index1], bitmap.PixelFormat));
                    int num2 = 128;
                    int num3;
                    do
                    {
                        num3 = num2;
                        int num4 = 0;
                        int num5 = 0;
                        int num6 = 0;
                        int num7 = 0;
                        for (int index2 = 0; index2 < image3.Rows; ++index2)
                        {
                            for (int index3 = 0; index3 < image3.Cols; ++index3)
                            {
                                int num8 = (int)image3.Data[index2, index3, 0];
                                if (num8 <= num3)
                                {
                                    ++num4;
                                    num6 += num8;
                                }
                                else
                                {
                                    ++num5;
                                    num7 += num8;
                                }
                            }
                        }
                        num2 = (num6 / num4 + num7 / num5) / 2;
                    }
                    while (num3 - num2 > 1 || num2 - num3 > 1);
                    num1 += (double)num2 / (double)rectangleList.Count;
                }
            }
            catch (Exception ex)
            {
                int num2 = (int)MessageBox.Show("Lỗi: " + ex.Message, clsBSoft._caption);
            }
            return num1;
        }

        private Image<Gray, byte> search(
          double thr,
          Image<Gray, byte> grayImage,
          double min,
          double max,
          out List<Rectangle> list_out,
          out int count,
          Image<Bgr, byte> color,
          out Image<Bgr, byte> color_out,
          Image<Gray, byte> bi,
          out Image<Gray, byte> bi_out)
        {
            List<Rectangle> rectangleList1 = (List<Rectangle>)null;
            List<Rectangle> rectangleList2 = (List<Rectangle>)null;
            Image<Bgr, byte> image1 = color;
            Image<Gray, byte> image2 = grayImage;
            Image<Gray, byte> image3 = bi;
            int num1 = 0;
            int num2 = 0;
            for (double num3 = min; num3 <= max; num3 += 0.1)
            {
                double intensity = thr / num3;
                image2 = grayImage.ThresholdBinary(new Gray(intensity), new Gray((double)byte.MaxValue));
                using (MemStorage stor = new MemStorage())
                {
                    for (Contour<Point> contour = image2.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_LIST, stor); contour != null; contour = contour.HNext)
                    {
                        Rectangle boundingRectangle = contour.BoundingRectangle;
                        CvInvoke.cvDrawContours((IntPtr)(UnmanagedObject)image1, (IntPtr)(Seq<Point>)contour, new MCvScalar((double)byte.MaxValue, (double)byte.MaxValue, 0.0), new MCvScalar(0.0), -1, 1, LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                        if (boundingRectangle.Width > 20 && boundingRectangle.Width < 150 && boundingRectangle.Height > 80 && boundingRectangle.Height < 150)
                        {
                            ++num1;
                            CvInvoke.cvDrawContours((IntPtr)(UnmanagedObject)image1, (IntPtr)(Seq<Point>)contour, new MCvScalar(0.0, (double)byte.MaxValue, (double)byte.MaxValue), new MCvScalar((double)byte.MaxValue), -1, 3, LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                            image1.Draw(contour.BoundingRectangle, new Bgr(0.0, (double)byte.MaxValue, 0.0), 2);
                            image3.Draw((Seq<Point>)contour, new Gray((double)byte.MaxValue), -1);
                            rectangleList1.Add(contour.BoundingRectangle);
                        }
                    }
                    for (int index1 = 0; index1 < num1; ++index1)
                    {
                        for (int index2 = index1 + 1; index2 < num1; ++index2)
                        {
                            int x1 = rectangleList1[index2].X;
                            int x2 = rectangleList1[index1].X;
                            Rectangle rectangle = rectangleList1[index1];
                            int width1 = rectangle.Width;
                            int num4 = x2 + width1;
                            int num5;
                            if (x1 < num4)
                            {
                                rectangle = rectangleList1[index2];
                                int x3 = rectangle.X;
                                rectangle = rectangleList1[index1];
                                int x4 = rectangle.X;
                                if (x3 > x4)
                                {
                                    rectangle = rectangleList1[index2];
                                    int y1 = rectangle.Y;
                                    rectangle = rectangleList1[index1];
                                    int y2 = rectangle.Y;
                                    rectangle = rectangleList1[index1];
                                    int width2 = rectangle.Width;
                                    int num6 = y2 + width2;
                                    if (y1 < num6)
                                    {
                                        rectangle = rectangleList1[index2];
                                        int y3 = rectangle.Y;
                                        rectangle = rectangleList1[index1];
                                        int y4 = rectangle.Y;
                                        num5 = y3 <= y4 ? 1 : 0;
                                        goto label_15;
                                    }
                                    else
                                    {
                                        num5 = 1;
                                        goto label_15;
                                    }
                                }
                            }
                            num5 = 1;
                        label_15:
                            if (num5 == 0)
                            {
                                rectangleList1.RemoveAt(index2);
                                --num1;
                                --index2;
                            }
                            else
                            {
                                rectangle = rectangleList1[index1];
                                int x3 = rectangle.X;
                                rectangle = rectangleList1[index2];
                                int x4 = rectangle.X;
                                rectangle = rectangleList1[index2];
                                int width2 = rectangle.Width;
                                int num6 = x4 + width2;
                                int num7;
                                if (x3 < num6)
                                {
                                    rectangle = rectangleList1[index1];
                                    int x5 = rectangle.X;
                                    rectangle = rectangleList1[index2];
                                    int x6 = rectangle.X;
                                    if (x5 > x6)
                                    {
                                        rectangle = rectangleList1[index1];
                                        int y1 = rectangle.Y;
                                        rectangle = rectangleList1[index2];
                                        int y2 = rectangle.Y;
                                        rectangle = rectangleList1[index2];
                                        int width3 = rectangle.Width;
                                        int num8 = y2 + width3;
                                        if (y1 < num8)
                                        {
                                            rectangle = rectangleList1[index1];
                                            int y3 = rectangle.Y;
                                            rectangle = rectangleList1[index2];
                                            int y4 = rectangle.Y;
                                            num7 = y3 <= y4 ? 1 : 0;
                                            goto label_23;
                                        }
                                        else
                                        {
                                            num7 = 1;
                                            goto label_23;
                                        }
                                    }
                                }
                                num7 = 1;
                            label_23:
                                if (num7 == 0)
                                {
                                    rectangleList1.RemoveAt(index1);
                                    --num1;
                                    --index1;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (num1 <= 8 && num1 > num2)
                {
                    rectangleList2 = rectangleList1;
                    num2 = num1;
                    if (num1 == 8)
                    {
                        color_out = image1;
                        bi_out = image3;
                        list_out = rectangleList2;
                        count = num2;
                        return image2;
                    }
                }
            }
            color_out = image1;
            bi_out = image3;
            list_out = rectangleList2;
            count = num2;
            return image2;
        }

        public static string Ocr(
          Bitmap image_s,
          bool isFull,
          TesseractProcessor full_tesseract,
          TesseractProcessor num_tesseract,
          TesseractProcessor ch_tesseract,
          bool isNum = false)
        {
            Image<Gray, byte> image = new Image<Gray, byte>(image_s);
            while (true)
            {
                if ((double)CvInvoke.cvCountNonZero((IntPtr)(UnmanagedObject)image) / (double)(image.Width * image.Height) <= 0.5)
                    image = image.Dilate(2);
                else
                    break;
            }
            Bitmap bitmap = image.ToBitmap();
            TesseractProcessor tesseractProcessor = !isFull ? (!isNum ? ch_tesseract : num_tesseract) : full_tesseract;
            int num = 0;
            tesseractProcessor.Clear();
            tesseractProcessor.ClearAdaptiveClassifier();
            string str = tesseractProcessor.Apply((Image)bitmap);
            while (str.Length > 3)
            {
                bitmap = new Image<Gray, byte>(bitmap).Erode(2).ToBitmap();
                tesseractProcessor.Clear();
                tesseractProcessor.ClearAdaptiveClassifier();
                str = tesseractProcessor.Apply((Image)bitmap);
                ++num;
                if (num > 10)
                {
                    str = "";
                    break;
                }
            }
            return str;
        }

        public static void FindLicensePlate(
          Bitmap image,
          PictureBox pictureBox1,
          ImageBox imageBox1,
          List<Image<Bgr, byte>> PlateImagesList,
          Panel panel1)
        {
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(image);
            using (Image<Gray, byte> image2 = new Image<Gray, byte>(image))
            {
                MCvAvgComp[] mcvAvgCompArray = image2.DetectHaarCascade(new HaarCascade(Application.StartupPath + "\\output-hv-33-x25.xml"), 1.1, 8, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(0, 0))[0];
                if (mcvAvgCompArray.Length == 0)
                {
                    int num = (int)MessageBox.Show("Không thể nhận dạng được biển số xe này!", clsBSoft._caption);
                }
                foreach (MCvAvgComp mcvAvgComp in mcvAvgCompArray)
                {
                    Image<Bgr, byte> image3 = image1.Copy();
                    image3.ROI = mcvAvgComp.rect;
                    image1.Draw(mcvAvgComp.rect, new Bgr(Color.Blue), 2);
                    PlateImagesList.Add(image3.Resize(500, 500, INTER.CV_INTER_CUBIC, true));
                    pictureBox1.Image = (Image)image3.ToBitmap();
                    pictureBox1.Update();
                }
                Image<Bgr, byte> image4 = new Image<Bgr, byte>(image.Size);
                Image<Bgr, byte> image5 = image1.Resize(imageBox1.Width, imageBox1.Height, INTER.CV_INTER_NN);
                imageBox1.Image = (IImage)image5;
                Label label1 = new Label();
                label1.Location = new Point(0, panel1.Height - 15);
                label1.ForeColor = Color.Red;
                Label label2 = label1;
                label2.Text = clsBSoft._caption;
                label2.Size = new Size(350, 24);
                panel1.Controls.Add((Control)label2);
            }
        }
    }
}
