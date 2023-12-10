using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Grafika.Services
{
    public interface IChartService
    {
        public WriteableBitmap? StretchHistogram(BitmapSource bitmapSource);

        public WriteableBitmap? EqualizeHistogram(BitmapSource bitmapSource);
    }
    class ChartService : IChartService
    {
        public WriteableBitmap? EqualizeHistogram(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                WriteableBitmap inputImage = new WriteableBitmap(bitmapSource);
                int width = inputImage.PixelWidth;
                int height = inputImage.PixelHeight;
                int[] histogram = CalculateHistogram(inputImage);
                int[] cumulativeHistogram = CalculateCumulativeHistogram(histogram);
                int minIntensity = Array.FindIndex(cumulativeHistogram, val => val > 0);
                int maxIntensity = Array.FindLastIndex(cumulativeHistogram, val => val > 0);
                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, inputImage.Format, inputImage.Palette);
                byte[] pixelData = new byte[4];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        inputImage.CopyPixels(new System.Windows.Int32Rect(x, y, 1, 1), pixelData, 4, 0);
                        byte originalIntensity = pixelData[0];
                        byte newIntensity = CalculateNewIntensity(originalIntensity, minIntensity, maxIntensity);
                        pixelData[0] = newIntensity;
                        pixelData[1] = newIntensity;
                        pixelData[2] = newIntensity;

                        outputImage.WritePixels(new System.Windows.Int32Rect(x, y, 1, 1), pixelData, 4, 0);
                    }
                }

                return outputImage;
            }
            return null;
        }

        public WriteableBitmap? StretchHistogram(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                WriteableBitmap inputImage = new WriteableBitmap(bitmapSource);
                int width = inputImage.PixelWidth;
                int height = inputImage.PixelHeight;
                int[] histogram = CalculateHistogram(inputImage);
                int minIntensity = Array.FindIndex(histogram, val => val > 0);
                int maxIntensity = Array.FindLastIndex(histogram, val => val > 0);
                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, inputImage.Format, inputImage.Palette);
                byte[] pixelData = new byte[4];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        inputImage.CopyPixels(new System.Windows.Int32Rect(x, y, 1, 1), pixelData, 4, 0);
                        byte originalIntensity = pixelData[0];
                        byte newIntensity = CalculateNewIntensity(originalIntensity, minIntensity, maxIntensity);
                        pixelData[0] = newIntensity;
                        pixelData[1] = newIntensity;
                        pixelData[2] = newIntensity;

                        outputImage.WritePixels(new System.Windows.Int32Rect(x, y, 1, 1), pixelData, 4, 0);
                    }
                }

                return outputImage;
            }
            return null;
        }

        private int[] CalculateHistogram(WriteableBitmap image)
        {
            byte[] pixelData = new byte[4];
            int[] histogram = new int[256];

            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    image.CopyPixels(new System.Windows.Int32Rect(x, y, 1, 1), pixelData, 4, 0);
                    byte intensity = pixelData[0];
                    histogram[intensity]++;
                }
            }

            return histogram;
        }

        private int[] CalculateCumulativeHistogram(int[] histogram)
        {
            int[] cumulativeHistogram = new int[256];
            cumulativeHistogram[0] = histogram[0];

            for (int i = 1; i < histogram.Length; i++)
            {
                cumulativeHistogram[i] = cumulativeHistogram[i - 1] + histogram[i];
            }

            return cumulativeHistogram;
        }

        private byte CalculateNewIntensity(byte originalIntensity, int minIntensity, int maxIntensity)
        {
            return (byte)(((originalIntensity - minIntensity) * 255) / (maxIntensity - minIntensity));
        }
    }
}
