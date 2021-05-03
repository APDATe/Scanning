using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scanning
{
    public partial class Form1 : Form
    {
        //Количество листов считаются первая сторона * 2 
        int CountList = 0;
        //Признак сканированя второй (обратной стороны)
        //bool Reverse_side = false;

        public Form1()
        {
            InitializeComponent();
        }
    
        private void начатьСканированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //SaveFiles();
             MessageBox.Show("Это старая функция востользуйтесь огромной кнопкой");
        }

        private void SaveFiles()
        {
            int numlis = 0;
            if (numericUpDown2.Value == 0)
            {
                numlis = 0;
            } else
            {
                numlis = Convert.ToInt32(numericUpDown2.Value);
                if (numericUpDown1.Value == 2)
                {
                    numlis = numlis * 2;
                }

            }
            for (int i = 0; i < 2000; i++)
            {
                if (i % 2 != 0)
                {
                    if (numericUpDown1.Value == 1)
                    {
                        if (i != 0)
                        {
                            Scanner abc = new Scanner();
                            var ms = abc.MemScan();
                            if (ms != null)
                            {
                                SaveFileOnDisk(ms, "cash\\" + i);
                                numlis++;
                                numericUpDown2.Value = numlis;
                            }
                            else
                            {
                                i = 2200;
                                                            
                                MessageBox.Show("Загрузи документ в лоток");
                            }
                        }
                    }
                } else
                {
                    if (numericUpDown1.Value == 2)
                    {
                        if (i != 0)
                        {
                            Scanner abc = new Scanner();
                            var ms = abc.MemScan();
                            if (ms != null)
                            {
                                SaveFileOnDisk(ms, "cash\\"+numlis);
                                numlis = numlis - 2;
                            }
                            else
                            {
                                i = 2200;
                                MessageBox.Show("Загрузи документ в лоток");
                            }
                        }
                    }
                }
            }
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
                        }
                        else
                        {
                            i = 2200;
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
