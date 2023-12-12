using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Grafika.Services
{
    public interface IAnalyzeService
    {
        public void AnalyzeImage(BitmapImage bitmapImage, int threshold, FileWindow fileWindow);
    }
    public class ImageAnalyzeService : IAnalyzeService
    {

        public void AnalyzeImage(BitmapImage bitmapImage, int threshold, FileWindow fileWindow)
        {
            if (bitmapImage == null || threshold == 0) return;
            BitmapSource binaryImage = ConvertToBinaryImage(bitmapImage, threshold);
            int[,] labels = LabelPixels(binaryImage);
            int largestGreenAreaLabel = FindLargestGreenAreaLabel(labels);
            HighlightLargestGreenArea(bitmapImage, labels, largestGreenAreaLabel, fileWindow);
            double greenPixelPercentage = CalculateGreenPixelPercentage(labels, binaryImage.PixelWidth * binaryImage.PixelHeight);
            MessageBox.Show($"Procent piskeli powyzej progu koloru zielonego: {Math.Round(greenPixelPercentage , 2)}%");
        }

        private WriteableBitmap ConvertToBinaryImage(BitmapImage original, int threshold)
        {
            int width = original.PixelWidth;
            int height = original.PixelHeight;

            WriteableBitmap binaryImage = new WriteableBitmap(original);

            binaryImage.Lock();

            unsafe
            {
                byte* ptr = (byte*)binaryImage.BackBuffer;

                for (int i = 0; i < binaryImage.PixelWidth * binaryImage.PixelHeight; i++)
                {
                    int grayscaleValue = (int)(ptr[2] * 0.3 + ptr[1] * 0.59 + ptr[0] * 0.11);

                    ptr[0] = ptr[1] = ptr[2] = (byte)(grayscaleValue > threshold ? 255 : 0);

                    ptr += 4;
                }
            }

            binaryImage.Unlock();

            return binaryImage;
        }

        private int[,] LabelPixels(BitmapSource binaryImage)
        {
            int width = binaryImage.PixelWidth;
            int height = binaryImage.PixelHeight;

            int[,] labels = new int[width, height];
            int currentLabel = 1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (GetPixelValue(binaryImage, x, y) == 0)
                    {
                        labels[x, y] = 0;
                    }
                    else
                    {
                        int label = GetNeighbourLabel(labels, x, y);
                        if (label == 0)
                        {
                            label = currentLabel;
                            currentLabel++;
                        }

                        labels[x, y] = label;
                    }
                }
            }

            return labels;
        }

        private int GetNeighbourLabel(int[,] labels, int x, int y)
        {
            int width = labels.GetLength(0);
            int height = labels.GetLength(1);

            int[] neighbours = { -1, 0, 1 };

            foreach (int dx in neighbours)
            {
                foreach (int dy in neighbours)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && labels[nx, ny] != 0)
                        return labels[nx, ny];
                }
            }

            return 0;
        }

        private int FindLargestGreenAreaLabel(int[,] labels)
        {
            var labelCounts = labels.Cast<int>()
                .Where(label => label != 0)
                .GroupBy(label => label)
                .ToDictionary(group => group.Key, group => group.Count());

            int largestGreenAreaLabel = labelCounts
                .Where(kv => IsGreenLabel(kv.Key))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .FirstOrDefault();

            return largestGreenAreaLabel;
        }

        private bool IsGreenLabel(int label)
        {
            return label % 2 == 0;
        }

        private void HighlightLargestGreenArea(BitmapImage original, int[,] labels, int largestGreenAreaLabel, FileWindow fileWindow)
        {
            int width = original.PixelWidth;
            int height = original.PixelHeight;

            byte[] pixels = new byte[width * height * 4];
            original.CopyPixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * 4;

                    if (labels[x, y] == largestGreenAreaLabel)
                    {
                        pixels[pixelIndex + 1] = 255;
                    }
                }
            }

            WriteableBitmap markedImage = new WriteableBitmap(width, height, original.DpiX, original.DpiY, PixelFormats.Bgr32, null);
            markedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            fileWindow.ImageView.Source = markedImage;
        }

        private double CalculateGreenPixelPercentage(int[,] labels, int totalPixels)
        {
            int greenPixelCount = 0;

            foreach (int label in labels)
            {
                if (IsGreenLabel(label))
                {
                    greenPixelCount++;
                }
            }

            double greenPixelPercentage = (greenPixelCount / (double)totalPixels) * 100;

            return greenPixelPercentage;
        }

        private byte GetPixelValue(BitmapSource image, int x, int y)
        {
            byte[] pixels = new byte[4];
            image.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
            return pixels[0];
        }
    }
}
