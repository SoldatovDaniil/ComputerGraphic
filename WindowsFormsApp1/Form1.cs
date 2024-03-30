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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Bitmap image;
        string kernelName = "kernel.txt";


        private float[,] parseMatrix(string m)
        {
            string nor = "";

            for (int i = 0; i < m.Length; i++)
            {
                if (m[i] != '\r')
                {
                    nor += m[i];
                }
            }

            if (nor[nor.Length - 1] == '\n')
            {
                nor = nor.Substring(0, nor.Length - 1);
            }

            int numLines = 1;
            int numCols = 1;

            string onlySpaces = "";

            for (int i = 0; i < nor.Length; i++)
            {
                if (nor[i] == '\n')
                {
                    numLines++;
                    numCols++;
                    onlySpaces += " ";
                    continue;
                }
                else if (nor[i] == ' ')
                {
                    numCols++;
                }
                onlySpaces += nor[i];
            }

            numCols /= numLines;

            var result = new float[numLines, numCols];

            string[] ssize = onlySpaces.Split();

            for (int i = 0; i < numLines; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    result[i, j] = float.Parse(ssize[numCols * i + j]);
                }
            }

            return result;
        }


        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|* .png;*.jpg;*.bmp|All files(*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void гаусToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void полутонToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void яркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BrightnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void собелToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void идеальныйОтражательToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image != null)
            {
                Filters filter = new PerfectReflectorFilter(image);
                backgroundWorker1.RunWorkerAsync(filter);
            }
        }

        private void линенйоеРастяжениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image != null) 
            {
                Filters filter = new LinearStretchingFilter(image);
                backgroundWorker1.RunWorkerAsync(filter);
            }
        }

        private void эффектСтеклаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GlassFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void волныпоГоризонталиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new WavesFilter2();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void волныпоВертикалиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new WavesFilter1();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void движениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MotionBlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void теснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new EmbossingFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void медианныйToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            var filter = new DilationFilter(parseMatrix(richTextBox1.Text));
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            var filter = new ErosionFilter(parseMatrix(richTextBox1.Text));
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void открытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            var filter = new ClosingFilter(parseMatrix(richTextBox1.Text));
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            var filter = new OpeningFilter(parseMatrix(richTextBox1.Text));
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            using (var streamWriter = new StreamWriter(kernelName, false))
            {
                streamWriter.Write(richTextBox1.Text);
                streamWriter.Close();
            }
        }

        private void topHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            var filter = new TopHatFilter(parseMatrix(richTextBox1.Text));
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
