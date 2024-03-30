using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;


namespace WindowsFormsApp1
{
    abstract class Filters
    {
        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int) ((float)i / resImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resImage;
        }


        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);


        public int Clamp(int value, int minVal, int maxVal)
        {
            if (value < minVal) 
                return minVal;
            if (value > maxVal)
                return maxVal;
            return value;
        }
    }


    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);

            return resColor;
        }
    }


    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.144 * sourceColor.B);
            intensity = Clamp(intensity, 0, 255); 
            Color resColor = Color.FromArgb(intensity, intensity, intensity);
            return resColor;
        }
    }


    class SepiaFilter : Filters
    {
        private float _ratio;

        public SepiaFilter(float ratio = 25)
        {
            this._ratio = ratio;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            double intensity = 0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.144 * sourceColor.B;
            Color resColor = Color.FromArgb(Clamp((int)(intensity + 2 * _ratio), 0, 255),
                                            Clamp((int)(intensity + 0.5 * _ratio), 0, 255),
                                            Clamp((int)(intensity - 1 * _ratio), 0, 255));
            return resColor;
        }
    }


    class BrightnessFilter : Filters
    {
        private int _ratio;

        public BrightnessFilter(int ratio = 10)
        {
            this._ratio = ratio;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(Clamp(sourceColor.R + _ratio, 0, 255),
                                               Clamp(sourceColor.G + _ratio, 0, 255),
                                               Clamp(sourceColor.B + _ratio, 0, 255));
            return resultColor;
        }
    }


    class PerfectReflectorFilter : Filters
    {
        private int maxR = 0;
        private int maxG = 0;
        private int maxB = 0;

        public PerfectReflectorFilter(Bitmap image)
        {
            FindMax(image);
        }

        private void FindMax(Bitmap image)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color col = image.GetPixel(i, j);
                    if (col.R > maxR) maxR = col.R;
                    if (col.G > maxG) maxG = col.G;
                    if (col.B > maxB) maxB = col.B;
                }
            }
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color col = sourceImage.GetPixel(x, y);
            return Color.FromArgb( col.R * 255 / maxR,
                                  col.G * 255 / maxG,
                                  col.B * 255 / maxB);
        }
    }


    class LinearStretchingFilter : Filters
    {
        private int maxR = 0;
        private int maxG = 0;
        private int maxB = 0;
        private int minR = 255;
        private int minG = 255;
        private int minB = 255;

        public LinearStretchingFilter(Bitmap image)
        {
            FindMinMax(image);
        }

        private void FindMinMax(Bitmap image)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color col = image.GetPixel(i, j);
                    if (col.R > maxR) maxR = col.R;
                    if (col.G > maxG) maxG = col.G;
                    if (col.B > maxB) maxB = col.B;
                    if (col.R < minR) minR = col.R;
                    if (col.G < minG) minG = col.G;
                    if (col.B < minB) minB = col.B;
                }
            }
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color col = sourceImage.GetPixel(x, y);
            return Color.FromArgb((col.R - minR) * 255 / (maxR - minR),
                                  (col.G - minG) * 255 / (maxG - minG),
                                  (col.B - minB) * 255 / (maxB - minB));
        }
    }


    class GlassFilter : Filters
    {
        private Random rand = new Random();
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int resX = (int)(x + (rand.Next(0, 5) - 2) * 5);
            int resY = (int)(y + (rand.Next(0, 5) - 2) * 5);
            return sourceImage.GetPixel(Clamp(resX, 0, sourceImage.Width - 1), Clamp(resY, 0, sourceImage.Height - 1));
        }
    }


    class WavesFilter1 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int resX = (int)(x + 20 * Math.Sin(2 * y * Math.PI / 60));
            Color resultColor;
            resultColor = sourceImage.GetPixel(Clamp(resX, 0, sourceImage.Width - 1),
                Clamp(y, 0, sourceImage.Height - 1));
            return resultColor;

        }
    }


    class WavesFilter2 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int resX = (int)(x + 20 * Math.Sin(2 * x * Math.PI / 60));
            Color resultColor;
            resultColor = sourceImage.GetPixel(Clamp(resX, 0, sourceImage.Width - 1),
                Clamp(y, 0, sourceImage.Height - 1));
            return resultColor;
        }
    }

    
    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel) {  this.kernel = kernel; }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for (int l = -radiusY; l <= radiusY; l++) 
            {
                for (int k = -radiusX; k <= radiusX; k++) 
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idy = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idy);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }


            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }


    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }
    }


    class GaussFilter : MatrixFilter    
    {
        public void createGaussKernel(int radius, float sigma) 
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0; // коэфициент нормировки
            for (int i = -radius;  i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i +  radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }
            //нормируем ядро
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= norm;
                 }
            }
        }
        public GaussFilter() 
        {
            createGaussKernel(3, 2);
        }
    }


    internal class SobelFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            // Vertical core
            float[,] kernelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            float resultRY = 0, resultGY = 0, resultBY = 0;
            int radiusX1 = kernelY.GetLength(0) / 2;
            int radiusY1 = kernelY.GetLength(1) / 2;

            // Horisontal core
            float[,] kernelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            float resultRX = 0, resultGX = 0, resultBX = 0;
            int radiusX2 = kernelX.GetLength(0) / 2;
            int radiusY2 = kernelX.GetLength(1) / 2;


            for (int l = -radiusY1; l <= radiusY1; l++)
            {
                for (int k = -radiusX1; k <= radiusX1; k++)
                {
                    int idX1 = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY1 = Clamp(y + l, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX1, idY1);

                    resultRY += neighborColor.R * kernelY[k + radiusX1, l + radiusY1];
                    resultGY += neighborColor.G * kernelY[k + radiusX1, l + radiusY1];
                    resultBY += neighborColor.B * kernelY[k + radiusX1, l + radiusY1];
                }
            }

            for (int l = -radiusY2; l <= radiusY2; l++)
            {
                for (int k = -radiusX2; k <= radiusX2; k++)
                {
                    int idX2 = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY2 = Clamp(y + l, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX2, idY2);

                    resultRX += neighborColor.R * kernelX[k + radiusX2, l + radiusY2];
                    resultGX += neighborColor.G * kernelX[k + radiusX2, l + radiusY2];
                    resultBX += neighborColor.B * kernelX[k + radiusX2, l + radiusY2];
                }
            }

            float resultR = (float)Math.Sqrt(Math.Pow(resultRX, 2) + Math.Pow(resultRY, 2));
            float resultG = (float)Math.Sqrt(Math.Pow(resultGX, 2) + Math.Pow(resultGY, 2));
            float resultB = (float)Math.Sqrt(Math.Pow(resultGX, 2) + Math.Pow(resultGY, 2));

            return Color.FromArgb(
                Clamp((int)(resultR), 0, 255),
                Clamp((int)(resultG), 0, 255),
                Clamp((int)(resultB), 0, 255));

        }
    }


    class SharpnessFilter : MatrixFilter
    {
        public SharpnessFilter()
        { 
           kernel = new float[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
        }
    }


    class MotionBlurFilter : MatrixFilter
    {
        public MotionBlurFilter(int n = 10)
        {
            kernel = new float[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        kernel[i, j] = 1f / n;
                    else
                        kernel[i, j] = 0;
                }
            }
        }
    }


    class EmbossingFilter : Filters
    {
        private float[,] _kernel = { { 0, 1, 0 }, { 1, 0, -1 }, { 0, -1, 0 } };

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            float resultR = 0, resultG = 0, resultB = 0;
            int radiusX = _kernel.GetLength(0) / 2;
            int radiusY = _kernel.GetLength(1) / 2;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(i + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(j + l, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    Color resultColor = Color.FromArgb(
                                               (int)(0.36 * neighborColor.R) +
                                               (int)(0.53 * neighborColor.G) +
                                               (int)(0.11 * neighborColor.B),

                                               (int)(0.36 * neighborColor.R) +
                                               (int)(0.53 * neighborColor.G) +
                                               (int)(0.11 * neighborColor.B),

                                               (int)(0.36 * neighborColor.R) +
                                               (int)(0.53 * neighborColor.G) +
                                               (int)(0.11 * neighborColor.B));

                    resultR += resultColor.R * _kernel[k + radiusX, l + radiusY];
                    resultG += resultColor.G * _kernel[k + radiusX, l + radiusY];
                    resultB += resultColor.B * _kernel[k + radiusX, l + radiusY];
                }
            }

            return Color.FromArgb(
                Clamp((((int)resultR + 255) / 2), 0, 255),
                Clamp((((int)resultG + 255) / 2), 0, 255),
                Clamp((((int)resultB + 255) / 2), 0, 255));
        }
    }


    class MedianFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            int size = 3;
            int radiusX = size / 2;
            int radiusY = size / 2;
            int q = 0;
            int[] colorsR = new int[9];
            int[] colorsG = new int[9];
            int[] colorsB = new int[9];
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(i + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(j + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    colorsR[q] = neighborColor.R;
                    colorsG[q] = neighborColor.G;
                    colorsB[q] = neighborColor.B;
                    q++;
                }
            Sort(colorsR);
            Sort(colorsB);
            Sort(colorsG);
            return Color.FromArgb(colorsR[9 / 2], colorsG[9 / 2], colorsB[9 / 2]);
        }
        private void Swap(ref int x, ref int y)
        {
            int t = x;
            x = y;
            y = t;
        }
        private void Sort(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
                for (int j = 0; j < arr.Length; j++)
                {
                    if (arr[i] > arr[j])
                        Swap(ref arr[i], ref arr[j]);
                }
        }
    }


    class DilationFilter : Filters
    {
        private float[,] kernel = null;
        public DilationFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return sourceImage.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int y = kernel.GetLength(1) / 2; y < sourceImage.Height - kernel.GetLength(1) / 2; y++)
            {
                worker.ReportProgress((int)((float)y / resultImage.Height * 100));

                if (worker.CancellationPending)
                {
                    return null;
                }

                for (int x = kernel.GetLength(0) / 2; x < sourceImage.Width - kernel.GetLength(0) / 2; x++)
                {
                    int max = 0;
                    for (int j = -kernel.GetLength(1) / 2; j < kernel.GetLength(1) / 2; j++)
                    {
                        for (int i = -kernel.GetLength(0) / 2; i < kernel.GetLength(0) / 2; i++)
                        {
                            if (kernel[i + kernel.GetLength(0) / 2, j + kernel.GetLength(1) / 2] > 0)
                            {
                                max = Math.Max(max, sourceImage.GetPixel(x + i, y + j).R);
                            }
                        }
                    }

                    max = Clamp(max, 0, 255);

                    resultImage.SetPixel(x, y, Color.FromArgb(max, max, max));
                }
            }
            return resultImage;
        }
    }


    class ErosionFilter : Filters
    {
        private float[,] kernel = null;
        public ErosionFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return sourceImage.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int y = kernel.GetLength(1) / 2; y < sourceImage.Height - kernel.GetLength(1) / 2; y++)
            {
                worker.ReportProgress((int)((float)y / resultImage.Height * 100));

                if (worker.CancellationPending)
                {
                    return null;
                }

                for (int x = kernel.GetLength(0) / 2; x < sourceImage.Width - kernel.GetLength(0) / 2; x++)
                {
                    int min = 255;
                    for (int j = -kernel.GetLength(1) / 2; j < kernel.GetLength(1) / 2; j++)
                    {
                        for (int i = -kernel.GetLength(0) / 2; i < kernel.GetLength(0) / 2; i++)
                        {
                            if (kernel[i + kernel.GetLength(0) / 2, j + kernel.GetLength(1) / 2] > 0)
                            {
                                min = Math.Min(min, sourceImage.GetPixel(x + i, y + j).R);
                            }
                        }
                    }

                    min = Clamp(min, 0, 255);

                    resultImage.SetPixel(x, y, Color.FromArgb(min, min, min));
                }
            }

            return resultImage;
        }
    }


    class OpeningFilter : Filters
    {
        private float[,] kernel = null;
        public OpeningFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return sourceImage.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            var dFilter = new DilationFilter(kernel);
            var eFilter = new ErosionFilter(kernel);

            Bitmap resultImage = eFilter.processImage(sourceImage, worker);
            resultImage = dFilter.processImage(resultImage, worker);

            return resultImage;
        }
    }

    class ClosingFilter : Filters
    {
        private float[,] kernel = null;
        public ClosingFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return sourceImage.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            var dFilter = new DilationFilter(kernel);
            var eFilter = new ErosionFilter(kernel);

            Bitmap resultImage = dFilter.processImage(sourceImage, worker);
            resultImage = eFilter.processImage(resultImage, worker);

            return resultImage;
        }
    }

    class TopHatFilter : Filters
    {
        float[,] kernel = null;
        public TopHatFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return sourceImage.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            var cFilter = new ClosingFilter(kernel);
            var eFilter = new ErosionFilter(kernel);

            Bitmap resultImage = cFilter.processImage(sourceImage, worker);
            resultImage = eFilter.processImage(sourceImage, worker);

            return resultImage;
        }
    }
}
