using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using tesseract;

namespace LPR_Laptrinhvb
{
	public partial class FrmMain : Form
	{

		public FrmMain()
		{
			InitializeComponent();
		}

		#region định nghĩa
		List<Image<Bgr, Byte>> PlateImagesList = new List<Image<Bgr, byte>>();
		List<string> PlateTextList = new List<string>();
		List<Rectangle> listRect = new List<Rectangle>();
		PictureBox[] box = new PictureBox[12];

		public TesseractProcessor full_tesseract = null;
		public TesseractProcessor ch_tesseract = null;
		public TesseractProcessor num_tesseract = null;
		private string m_path;
		private List<string> lstimages = new List<string>();
		private const string m_lang = "eng";
		private string _Caption = "http://www.laptrinhvb.net";
		#endregion

		private void FrmMain_Load(object sender, EventArgs e)
		{
			m_path = GetAbsolutePath(typeof(Program).Assembly, "../../data/");
			full_tesseract = new TesseractProcessor();
			bool succeed = full_tesseract.Init(m_path, m_lang, 3);
			if (!succeed)
			{
				MessageBox.Show("Lỗi thư viện Tesseract. Chương trình cần kết thúc.", _Caption);
				Application.Exit();
			}
			full_tesseract.SetVariable("tessedit_char_whitelist", "ACDFHKLMNPRSTVXY1234567890").ToString();

			ch_tesseract = new TesseractProcessor();
			succeed = ch_tesseract.Init(m_path, m_lang, 3);
			if (!succeed)
			{
				MessageBox.Show("Lỗi thư viện Tesseract. Chương trình cần kết thúc.", _Caption);
				Application.Exit();
			}
			ch_tesseract.SetVariable("tessedit_char_whitelist", "ACDEFHKLMNPRSTUVXY").ToString();

			num_tesseract = new TesseractProcessor();
			succeed = num_tesseract.Init(m_path, m_lang, 3);
			if (!succeed)
			{
				MessageBox.Show("Lỗi thư viện Tesseract. Chương trình cần kết thúc.", _Caption);
				Application.Exit();
			}
			num_tesseract.SetVariable("tessedit_char_whitelist", "1234567890").ToString();

			for (int i = 0; i < box.Length; i++)
			{
				box[i] = new PictureBox();
			}
			string folder = GetAbsolutePath(typeof(Program).Assembly, "../../ImageTest");
			foreach (string fileName in Directory.GetFiles(folder, "*.bmp", SearchOption.TopDirectoryOnly))
			{
				lstimages.Add(Path.GetFullPath(fileName));
			}
			foreach (string fileName in Directory.GetFiles(folder, "*.jpg", SearchOption.TopDirectoryOnly))
			{
				lstimages.Add(Path.GetFullPath(fileName));
			}
		}

		private void metroButton1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void metroButton2_Click(object sender, EventArgs e)
		{

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Image (*.bmp; *.jpg; *.jpeg; *.png) |*.bmp; *.jpg; *.jpeg; *.png|All files (*.*)|*.*||";
			dlg.InitialDirectory = GetAbsolutePath(typeof(Program).Assembly, "../../ImageTest");
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
			{
				return;
			}
			string startupPath = dlg.FileName;

			ProcessImage(startupPath);
			if (PlateImagesList.Count != 0)
			{
				Image<Bgr, byte> src = new Image<Bgr, byte>(PlateImagesList[0].ToBitmap());
				Bitmap grayframe;
				Bitmap color;
				int c = clsBSoft.IdentifyContours(src.ToBitmap(), 50, false, out grayframe, out color, out listRect);
				pictureBox1.Image = color;
				pictureBox2.Image = grayframe;
				textBox2.Text = c.ToString();
				Image<Gray, byte> dst = new Image<Gray, byte>(grayframe);
				grayframe = dst.ToBitmap();
				string zz = "";

				// lọc và sắp xếp số
				List<Bitmap> bmp = new List<Bitmap>();
				List<int> erode = new List<int>();
				List<Rectangle> up = new List<Rectangle>();
				List<Rectangle> dow = new List<Rectangle>();
				int up_y = 0, dow_y = 0;
				bool flag_up = false;

				int di = 0;

				if (listRect == null) return;

				for (int i = 0; i < listRect.Count; i++)
				{
					Bitmap ch = grayframe.Clone(listRect[i], grayframe.PixelFormat);
					int cou = 0;
					full_tesseract.Clear();
					full_tesseract.ClearAdaptiveClassifier();
					string temp = full_tesseract.Apply(ch);
					while (temp.Length > 3)
					{
						Image<Gray, byte> temp2 = new Image<Gray, byte>(ch);
						temp2 = temp2.Erode(2);
						ch = temp2.ToBitmap();
						full_tesseract.Clear();
						full_tesseract.ClearAdaptiveClassifier();
						temp = full_tesseract.Apply(ch);
						cou++;
						if (cou > 10)
						{
							listRect.RemoveAt(i);
							i--;
							di = 0;
							break;
						}
						di = cou;
					}
				}

				for (int i = 0; i < listRect.Count; i++)
				{
					for (int j = i; j < listRect.Count; j++)
					{
						if (listRect[i].Y > listRect[j].Y + 100)
						{
							flag_up = true;
							up_y = listRect[j].Y;
							dow_y = listRect[i].Y;
							break;
						}
						else if (listRect[j].Y > listRect[i].Y + 100)
						{
							flag_up = true;
							up_y = listRect[i].Y;
							dow_y = listRect[j].Y;
							break;
						}
						if (flag_up == true) break;
					}
				}

				for (int i = 0; i < listRect.Count; i++)
				{
					if (listRect[i].Y < up_y + 50 && listRect[i].Y > up_y - 50)
					{
						up.Add(listRect[i]);
					}
					else if (listRect[i].Y < dow_y + 50 && listRect[i].Y > dow_y - 50)
					{
						dow.Add(listRect[i]);
					}
				}

				if (flag_up == false) dow = listRect;

				for (int i = 0; i < up.Count; i++)
				{
					for (int j = i; j < up.Count; j++)
					{
						if (up[i].X > up[j].X)
						{
							Rectangle w = up[i];
							up[i] = up[j];
							up[j] = w;
						}
					}
				}
				for (int i = 0; i < dow.Count; i++)
				{
					for (int j = i; j < dow.Count; j++)
					{
						if (dow[i].X > dow[j].X)
						{
							Rectangle w = dow[i];
							dow[i] = dow[j];
							dow[j] = w;
						}
					}
				}

				int x = 0;
				int c_x = 0;

				for (int i = 0; i < up.Count; i++)
				{
					Bitmap ch = grayframe.Clone(up[i], grayframe.PixelFormat);
					Bitmap o = ch;
					string temp;
					if (i < 2)
					{
						temp = clsBSoft.Ocr(ch, false, full_tesseract, num_tesseract, ch_tesseract, true); // nhan dien so
					}
					else
					{
						temp = clsBSoft.Ocr(ch, false, full_tesseract, num_tesseract, ch_tesseract, false); // nhan dien chu
					}

					zz += temp;
					box[i].Location = new Point(x + i * 50, 0);
					box[i].Size = new Size(50, 100);
					box[i].SizeMode = PictureBoxSizeMode.StretchImage;
					box[i].Image = ch;
					panel1.Controls.Add(box[i]);
					c_x++;
				}
				zz += "\r\n";
				for (int i = 0; i < dow.Count; i++)
				{
					Bitmap ch = grayframe.Clone(dow[i], grayframe.PixelFormat);
					string temp = clsBSoft.Ocr(ch, false, full_tesseract, num_tesseract, ch_tesseract, true); // nhan dien so

					zz += temp;
					box[i + c_x].Location = new Point(x + i * 50, 100);
					box[i + c_x].Size = new Size(50, 100);
					box[i + c_x].SizeMode = PictureBoxSizeMode.StretchImage;
					box[i + c_x].Image = ch;
					panel1.Controls.Add(box[i + c_x]);
				}
				textBox1.Text = zz;

			}

		}

		public void ProcessImage(string urlImage)
		{
			PlateImagesList.Clear();
			PlateTextList.Clear();
			panel1.Controls.Clear();
			Bitmap img = new Bitmap(urlImage);
			pictureBox2.Image = null;
			pictureBox1.Image = img;
			pictureBox1.Update();
			clsBSoft.FindLicensePlate(img, pictureBox1, imageBox1, PlateImagesList, panel1);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(linkLabel1.Text);
		}

		private string GetAbsolutePath(Assembly assembly, string relativePath)
		{
			var assemblyFolderPath = new FileInfo(assembly.Location).Directory.FullName;
			return Path.GetFullPath(Path.Combine(assemblyFolderPath, relativePath));
		}
	}

}
