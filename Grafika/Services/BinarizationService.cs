using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Grafika.Services
{
    public interface IBinarizationService
    {
        public WriteableBitmap? Binarization(BitmapSource bitmapSource, byte threshold);
        public WriteableBitmap? PercentBlackSelection(BitmapSource bitmapSource, int blackPercentage);
        public WriteableBitmap? MeanIterativeSelection(BitmapSource bitmapSource);
        public WriteableBitmap? Otsu(BitmapSource bitmapSource);
        public WriteableBitmap? Niblack(BitmapSource bitmapSource, int windowSize = 15, double k = 0.2);
        public WriteableBitmap? Sauvola(BitmapSource bitmapSource, int windowSize = 15, double k = 0.2, double R=128);
    }
    class BinarizationService : IBinarizationService
    {
        public WriteableBitmap? Binarization(BitmapSource bitmapSource, byte threshold)
        {
            if (bitmapSource != null)
            {
                WriteableBitmap inputImage = new WriteableBitmap(bitmapSource);
                int width = inputImage.PixelWidth;
                int height = inputImage.PixelHeight;
                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, inputImage.Format, inputImage.Palette);
                byte[] pixelData = new byte[4];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        inputImage.CopyPixels(new System.Windows.Int32Rect(x, y, 1, 1), pixelData, 4, 0);
                        byte originalIntensity = pixelData[0];
                        byte newIntensity = (originalIntensity > threshold) ? (byte)255 : (byte)0;
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
        public WriteableBitmap? PercentBlackSelection(BitmapSource bitmapSource, int blackPercentage)
        {
            if (bitmapSource != null)
            {
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;
                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, bitmapSource.Format, bitmapSource.Palette);
                byte[] pixelData = new byte[4];
                int totalPixels = width * height;
                int blackPixelCount = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bitmapSource.CopyPixels(new Int32Rect(x, y, 1, 1), pixelData, 4, 0);
                        byte originalIntensity = pixelData[0];
                        if (originalIntensity == 0)
                        {
                            blackPixelCount++;
                        }
                    }
                }

                double percentageBlack = (double)blackPixelCount / totalPixels * 100.0;
                byte threshold = (byte)((blackPercentage / 100.0) * 255);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bitmapSource.CopyPixels(new Int32Rect(x, y, 1, 1), pixelData, 4, 0);
                        byte originalIntensity = pixelData[0];
                        byte newIntensity = (originalIntensity > threshold) ? (byte)255 : (byte)0;
                        outputImage.WritePixels(new Int32Rect(x, y, 1, 1), new byte[] { newIntensity, newIntensity, newIntensity, pixelData[3] }, 4, 0);
                    }
                }

                return outputImage;
            }
            return null;
        }
        public WriteableBitmap? MeanIterativeSelection(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;
                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, bitmapSource.Format, bitmapSource.Palette);
                byte[] pixelData = new byte[width * height * 4];
                bitmapSource.CopyPixels(pixelData, width * 4, 0);

                int[] histogram = new int[256];
                foreach (byte pixelValue in pixelData)
                {
                    histogram[pixelValue]++;
                }

                int totalPixels = width * height;
                double sum = 0;
                for (int i = 0; i < 256; i++)
                {
                    sum += i * histogram[i];
                }

                double sumB = 0;
                int wB = 0;
                int wF = 0;
                double maxVariance = 0;
                double threshold = 0;

                for (int i = 0; i < 256; i++)
                {
                    wB += histogram[i];
                    if (wB == 0)
                        continue;

                    wF = totalPixels - wB;
                    if (wF == 0)
                        break;

                    sumB += i * histogram[i];

                    double meanBackground = sumB / wB;
                    double meanForeground = (sum - sumB) / wF;

                    double betweenClassVariance = wB * wF * (meanBackground - meanForeground) * (meanBackground - meanForeground);

                    if (betweenClassVariance > maxVariance)
                    {
                        maxVariance = betweenClassVariance;
                        threshold = i;
                    }
                }

                for (int i = 0; i < pixelData.Length; i += 4)
                {
                    byte intensity = pixelData[i];
                    byte newIntensity = (intensity > threshold) ? (byte)255 : (byte)0;

                    pixelData[i] = newIntensity;
                    pixelData[i + 1] = newIntensity;
                    pixelData[i + 2] = newIntensity;
                }

                outputImage.WritePixels(new Int32Rect(0, 0, width, height), pixelData, width * 4, 0);
                return outputImage;
            }
            return null;
        }
        public WriteableBitmap? Otsu(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;

                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, bitmapSource.Format, bitmapSource.Palette);
                byte[] pixelData = new byte[width * height * 4];

                bitmapSource.CopyPixels(pixelData, width * 4, 0);

                int[] histogram = CalculateHistogram(pixelData);

                double totalPixels = width * height;
                double sum = 0;
                for (int i = 0; i < 256; ++i)
                {
                    sum += i * histogram[i];
                }

                double sumB = 0;
                int wB = 0;
                int wF = 0;

                double varMax = 0;
                int threshold = 0;

                for (int i = 0; i < 256; ++i)
                {
                    wB += histogram[i];
                    if (wB == 0)
                        continue;

                    wF = (int)(totalPixels - wB);
                    if (wF == 0)
                        break;

                    sumB += i * histogram[i];

                    double meanBackground = sumB / wB;
                    double meanForeground = (sum - sumB) / wF;

                    double varBetween = wB * wF * (meanBackground - meanForeground) * (meanBackground - meanForeground);

                    if (varBetween > varMax)
                    {
                        varMax = varBetween;
                        threshold = i;
                    }
                }

                for (int i = 0; i < pixelData.Length; i += 4)
                {
                    byte intensity = pixelData[i];
                    byte newIntensity = (intensity > threshold) ? (byte)255 : (byte)0;

                    pixelData[i] = newIntensity;
                    pixelData[i + 1] = newIntensity;
                    pixelData[i + 2] = newIntensity;
                }

                outputImage.WritePixels(new Int32Rect(0, 0, width, height), pixelData, width * 4, 0);
                return outputImage;
            }
            return null;
        }

        private int[] CalculateHistogram(byte[] pixelData)
        {
            int[] histogram = new int[256];

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte intensity = pixelData[i];
                histogram[intensity]++;
            }

            return histogram;
        }
        public WriteableBitmap? Niblack(BitmapSource bitmapSource, int windowSize=15, double k=0.2)
        {
            if (bitmapSource != null)
            {
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;

                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, bitmapSource.Format, bitmapSource.Palette);
                byte[] pixelData = new byte[width * height * 4];

                bitmapSource.CopyPixels(pixelData, width * 4, 0);

                byte[] outputPixelData = new byte[width * height * 4];

                int stride = width * 4;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int startX = Math.Max(0, x - windowSize / 2);
                        int startY = Math.Max(0, y - windowSize / 2);
                        int endX = Math.Min(width - 1, x + windowSize / 2);
                        int endY = Math.Min(height - 1, y + windowSize / 2);

                        double mean = 0;
                        double variance = 0;

                        int pixelCount = 0;

                        for (int j = startY; j <= endY; j++)
                        {
                            for (int i = startX; i <= endX; i++)
                            {
                                int index = (j * stride) + (i * 4);
                                byte intensity = pixelData[index];

                                mean += intensity;
                                pixelCount++;
                            }
                        }

                        mean /= pixelCount;

                        for (int j = startY; j <= endY; j++)
                        {
                            for (int i = startX; i <= endX; i++)
                            {
                                int index = (j * stride) + (i * 4);
                                byte intensity = pixelData[index];

                                variance += Math.Pow(intensity - mean, 2);
                            }
                        }

                        variance = Math.Sqrt(variance / pixelCount);

                        int indexCurrent = (y * stride) + (x * 4);
                        byte intensityCurrent = pixelData[indexCurrent];

                        byte newIntensity = (byte)(mean + k * variance);

                        outputPixelData[indexCurrent] = (intensityCurrent > newIntensity) ? (byte)0 : (byte)255;
                        outputPixelData[indexCurrent + 1] = outputPixelData[indexCurrent];
                        outputPixelData[indexCurrent + 2] = outputPixelData[indexCurrent];
                        outputPixelData[indexCurrent + 3] = 255;
                    }
                }

                outputImage.WritePixels(new Int32Rect(0, 0, width, height), outputPixelData, stride, 0);
                return outputImage;
            }
            return null;
        }
        public WriteableBitmap? Sauvola(BitmapSource bitmapSource, int windowSize = 15, double k = 0.2, double R = 128)
        {
            if (bitmapSource != null)
            {
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;

                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, bitmapSource.Format, bitmapSource.Palette);
                byte[] pixelData = new byte[width * height * 4];

                bitmapSource.CopyPixels(pixelData, width * 4, 0);

                byte[] outputPixelData = new byte[width * height * 4];

                int stride = width * 4;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int startX = Math.Max(0, x - windowSize / 2);
                        int startY = Math.Max(0, y - windowSize / 2);
                        int endX = Math.Min(width - 1, x + windowSize / 2);
                        int endY = Math.Min(height - 1, y + windowSize / 2);

                        double mean = 0;
                        double variance = 0;

                        int pixelCount = 0;

                        for (int j = startY; j <= endY; j++)
                        {
                            for (int i = startX; i <= endX; i++)
                            {
                                int index = (j * stride) + (i * 4);
                                byte intensity = pixelData[index];

                                mean += intensity;
                                pixelCount++;
                            }
                        }

                        mean /= pixelCount;

                        for (int j = startY; j <= endY; j++)
                        {
                            for (int i = startX; i <= endX; i++)
                            {
                                int index = (j * stride) + (i * 4);
                                byte intensity = pixelData[index];

                                variance += Math.Pow(intensity - mean, 2);
                            }
                        }

                        variance = Math.Sqrt(variance / pixelCount);

                        int indexCurrent = (y * stride) + (x * 4);
                        byte intensityCurrent = pixelData[indexCurrent];

                        double threshold = mean * (1 + k * ((variance / R) - 1));

                        byte newIntensity = (intensityCurrent > threshold) ? (byte)255 : (byte)0;

                        outputPixelData[indexCurrent] = newIntensity;
                        outputPixelData[indexCurrent + 1] = outputPixelData[indexCurrent];
                        outputPixelData[indexCurrent + 2] = outputPixelData[indexCurrent];
                        outputPixelData[indexCurrent + 3] = 255;
                    }
                }

                outputImage.WritePixels(new Int32Rect(0, 0, width, height), outputPixelData, stride, 0);
                return outputImage;
            }
            return null;
        }

    }
}
