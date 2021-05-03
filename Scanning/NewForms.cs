using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Scanning
{
    public partial class NewForms : Form
    {
        //Количество листов считаются первая сторона * 2 
        int CountList = 0;
        //Признак сканированя второй (обратной стороны)
        //bool Reverse_side = false;

        public NewForms()
        {
            InitializeComponent();
        }

        private void buttonSkaning_Click(object sender, EventArgs e)
        {
            //Начинаем сканирование первой стороны
            First_Side();
            //Попросим перевернуть пачку обратной стороной
            var aa = MessageBox.Show("Переверните бумагу в лотке приемника и нажмите OK.", "ВНИМАНИЕ", MessageBoxButtons.OKCancel);
            if (DialogResult.OK == aa)
            {
                //Сканируем обратную сторону
                ReverseSide();
                //Создать пдф 
                GeneratePdf();
                //Открыть каталог, с возможностью отправки email
            }
            else
            {
                //создать пдф
                GeneratePdf();
                //Открыть каталог, с возможностью отправки email 
            }

        }

        private void First_Side()
        {
            bool ThereIsPaper = false;
            for (int i = 0; i < 2000; i++)
            {
                if (i % 2 != 0)
                {
                    if (i != 0)
                    {
                        Scanner abc = new Scanner();
                        var ms = abc.MemScan();
                        if (ms != null)
                        {
                            SaveFileOnDisk(ms, "cash\\" + i);
                            CountList++;
                            ThereIsPaper = true;
                        }
                        else
                        {
                            i = 2200;
                            if (ThereIsPaper == false)
                            {
                                MessageBox.Show("Положите бумагу в лоток для сканирования");
                                //MessageBox.Show("Обратите внимание на то что данной проверки не будет в случае если Вы собираетесь сканировать две сторо");
                            }
                        }
                    }
                }
            }
        }

        private void ReverseSide()
        {
            CountList = CountList * 2;
            for (int i = 0; i < 2000; i++)
            {
                if (i % 2 == 0)
                {
                    if (i != 0)
                    {
                        Scanner abc = new Scanner();
                        var ms = abc.MemScan();
                        if (ms != null)
                        {
                            SaveFileOnDisk(ms, "cash\\" + CountList);
                            CountList = CountList - 2;
                        }
                        else
                        {
                            i = 2200;
                            MessageBox.Show("Готово");
                        }
                    }
                }
            }
        }

        private void GeneratePdf()
        {
            using (var document = new PdfDocument())
            {
                for (int i = 0; i < 2000; i++)
                {
                    if (File.Exists("cash\\" + i + ".png"))
                    {
                        PdfPage page = document.AddPage();
                        using (XImage img = XImage.FromFile("cash\\" + i + ".png"))
                        {
                            var height = (int)(((double)3508 / (double)img.PixelWidth) * img.PixelHeight);
                            page.Width = 3508;
                            page.Height = height;
                            XGraphics gfx = XGraphics.FromPdfPage(page);
                            gfx.DrawImage(img, 0, 0, 3508, height);
                        }
                    }
                    else
                    {
                        if (i > 0)
                        {
                            i = 5000;
                        }
                    }
                }
                document.Save("cash\\" + "new" + ".pdf");
            }
        }

        public void SaveFileOnDisk(MemoryStream ms, string FileName)
        {
            try
            {
                using (var original = Image.FromStream(ms))
                using (var resized = ResizeWithSameRatio(original, 3508, 2480))
                {
                    resized.Save(FileName + ".png");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что то пошло не так описание ошибки - " + ex.Message);
            }
        }

        public Image ResizeWithSameRatio(Image image, float width, float height)
        {
            // the colour for letter boxing, can be a parameter
            var brush = new SolidBrush(Color.Black);

            // target scaling factor
            float scale = Math.Min(width / image.Width, height / image.Height);

            // target image
            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            // fill the background and then draw the image in the 'centre'
            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(image, new Rectangle(((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight));

            return bmp;
        }

    }
}
