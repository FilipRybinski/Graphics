using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Grafika.Services
{
    public interface IFilterService
    {
        public WriteableBitmap? SmoothingFilter(BitmapSource bitmapSource);
        public WriteableBitmap? MedianFilter(BitmapSource bitmapSource);
        public WriteableBitmap? HighPassFilter(BitmapSource bitmapSource);
        public WriteableBitmap? GaussianFilter(BitmapSource bitmapSource);
        public WriteableBitmap? Close(BitmapSource bitmapSource, int kernelSize);
        public WriteableBitmap? Open(BitmapSource bitmapSource, int kernelSize);
        public WriteableBitmap? Erode(BitmapSource bitmapSource, int kernelSize);
        public WriteableBitmap? Dilate(BitmapSource bitmapSource, int kernelSize);
        public WriteableBitmap? HitOrMiss(BitmapSource bitmapSource, byte[,] mask);
    }
    public class FilterService:IFilterService
    {
        public WriteableBitmap? SmoothingFilter(BitmapSource bitmapSource)
        {
            if (bitmapSource!=null)
            {
                WriteableBitmap writableBitmap = new WriteableBitmap(bitmapSource);

                int width = writableBitmap.PixelWidth;
                int height = writableBitmap.PixelHeight;
                int bytesPerPixel = (writableBitmap.Format.BitsPerPixel + 7) / 8;
                int stride = width * bytesPerPixel;

                byte[] originalPixelData = new byte[height * stride];
                writableBitmap.CopyPixels(originalPixelData, stride, 0);

                byte[] smoothedPixelData = new byte[height * stride];
                writableBitmap.CopyPixels(smoothedPixelData, stride, 0);

                int matrixSize = 3;

                int matrixLength = matrixSize * matrixSize;
                int matrixOffset = matrixSize / 2;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int sumRed = 0, sumGreen = 0, sumBlue = 0;

                        for (int i = -matrixOffset; i <= matrixOffset; i++)
                        {
                            for (int j = -matrixOffset; j <= matrixOffset; j++)
                            {
                                int currentX = x + j;
                                int currentY = y + i;

                                if (currentX >= 0 && currentX < width && currentY >= 0 && currentY < height)
                                {
                                    int currentIndex = (currentY * stride) + (currentX * bytesPerPixel);
                                    sumBlue += originalPixelData[currentIndex];
                                    sumGreen += originalPixelData[currentIndex + 1];
                                    sumRed += originalPixelData[currentIndex + 2];
                                }
                            }
                        }

                        int currentIndexPixel = (y * stride) + (x * bytesPerPixel);
                        smoothedPixelData[currentIndexPixel] = (byte)(sumBlue / matrixLength);
                        smoothedPixelData[currentIndexPixel + 1] = (byte)(sumGreen / matrixLength);
                        smoothedPixelData[currentIndexPixel + 2] = (byte)(sumRed / matrixLength);
                        smoothedPixelData[currentIndexPixel + 3] = originalPixelData[currentIndexPixel + 3];
                    }
                }

                writableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), smoothedPixelData, stride, 0);
                return writableBitmap;
            }
            return null;
        }
        public WriteableBitmap? MedianFilter(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                WriteableBitmap writableBitmap = new WriteableBitmap(bitmapSource);

                int width = writableBitmap.PixelWidth;
                int height = writableBitmap.PixelHeight;
                int bytesPerPixel = (writableBitmap.Format.BitsPerPixel + 7) / 8;
                int stride = width * bytesPerPixel;

                byte[] originalPixelData = new byte[height * stride];
                writableBitmap.CopyPixels(originalPixelData, stride, 0);

                byte[] filteredPixelData = new byte[height * stride];
                writableBitmap.CopyPixels(filteredPixelData, stride, 0);

                int filterSize = 3;

                int filterOffset = filterSize / 2;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        List<byte> redValues = new List<byte>();
                        List<byte> greenValues = new List<byte>();
                        List<byte> blueValues = new List<byte>();

                        for (int i = -filterOffset; i <= filterOffset; i++)
                        {
                            for (int j = -filterOffset; j <= filterOffset; j++)
                            {
                                int currentX = x + j;
                                int currentY = y + i;

                                if (currentX >= 0 && currentX < width && currentY >= 0 && currentY < height)
                                {
                                    int currentIndex = (currentY * stride) + (currentX * bytesPerPixel);
                                    blueValues.Add(originalPixelData[currentIndex]);
                                    greenValues.Add(originalPixelData[currentIndex + 1]);
                                    redValues.Add(originalPixelData[currentIndex + 2]);
                                }
                            }
                        }

                        int currentIndexPixel = (y * stride) + (x * bytesPerPixel);
                        filteredPixelData[currentIndexPixel] = GetMedian(blueValues);
                        filteredPixelData[currentIndexPixel + 1] = GetMedian(greenValues);
                        filteredPixelData[currentIndexPixel + 2] = GetMedian(redValues);
                        filteredPixelData[currentIndexPixel + 3] = originalPixelData[currentIndexPixel + 3];
                    }
                }

                writableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), filteredPixelData, stride, 0);
                return writableBitmap;
            }
            return null;
        }
        public WriteableBitmap? HighPassFilter(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                WriteableBitmap writableBitmap = new WriteableBitmap(bitmapSource);

                int width = writableBitmap.PixelWidth;
                int height = writableBitmap.PixelHeight;
                int bytesPerPixel = (writableBitmap.Format.BitsPerPixel + 7) / 8;
                int stride = width * bytesPerPixel;

                byte[] originalPixelData = new byte[height * stride];
                writableBitmap.CopyPixels(originalPixelData, stride, 0);

                byte[] filteredPixelData = new byte[height * stride];
                writableBitmap.CopyPixels(filteredPixelData, stride, 0);

                int[,] laplacianMatrix = new int[,] {
                { 0, -1, 0 },
                { -1, 4, -1 },
                { 0, -1, 0 }
            };

                int filterOffset = 1;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int sumRed = 0, sumGreen = 0, sumBlue = 0;

                        for (int i = -filterOffset; i <= filterOffset; i++)
                        {
                            for (int j = -filterOffset; j <= filterOffset; j++)
                            {
                                int currentX = x + j;
                                int currentY = y + i;

                                if (currentX >= 0 && currentX < width && currentY >= 0 && currentY < height)
                                {
                                    int currentIndex = (currentY * stride) + (currentX * bytesPerPixel);
                                    int laplacianValue = laplacianMatrix[i + filterOffset, j + filterOffset];

                                    sumBlue += originalPixelData[currentIndex] * laplacianValue;
                                    sumGreen += originalPixelData[currentIndex + 1] * laplacianValue;
                                    sumRed += originalPixelData[currentIndex + 2] * laplacianValue;
                                }
                            }
                        }

                        int currentIndexPixel = (y * stride) + (x * bytesPerPixel);
                        filteredPixelData[currentIndexPixel] = (byte)Math.Max(0, Math.Min(255, sumBlue));
                        filteredPixelData[currentIndexPixel + 1] = (byte)Math.Max(0, Math.Min(255, sumGreen));
                        filteredPixelData[currentIndexPixel + 2] = (byte)Math.Max(0, Math.Min(255, sumRed));
                        filteredPixelData[currentIndexPixel + 3] = originalPixelData[currentIndexPixel + 3];
                    }
                }

                writableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), filteredPixelData, stride, 0);
                return writableBitmap;
            }
            return null;
        }
        public WriteableBitmap? GaussianFilter(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                WriteableBitmap writableBitmap = new WriteableBitmap(bitmapSource);

                int width = writableBitmap.PixelWidth;
                int height = writableBitmap.PixelHeight;
                int bytesPerPixel = (writableBitmap.Format.BitsPerPixel + 7) / 8;
                int stride = width * bytesPerPixel;

                byte[] originalPixelData = new byte[height * stride];
                writableBitmap.CopyPixels(originalPixelData, stride, 0);

                byte[] blurredPixelData = new byte[height * stride];
                writableBitmap.CopyPixels(blurredPixelData, stride, 0);

                double[,] gaussianMatrix = new double[,] {
                { 1, 2, 1 },
                { 2, 4, 2 },
                { 1, 2, 1 }
            };

                int filterOffset = 1;
                double factor = 1.0 / 16.0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double sumRed = 0, sumGreen = 0, sumBlue = 0;

                        for (int i = -filterOffset; i <= filterOffset; i++)
                        {
                            for (int j = -filterOffset; j <= filterOffset; j++)
                            {
                                int currentX = x + j;
                                int currentY = y + i;

                                if (currentX >= 0 && currentX < width && currentY >= 0 && currentY < height)
                                {
                                    int currentIndex = (currentY * stride) + (currentX * bytesPerPixel);
                                    double gaussianValue = gaussianMatrix[i + filterOffset, j + filterOffset];

                                    sumBlue += originalPixelData[currentIndex] * gaussianValue;
                                    sumGreen += originalPixelData[currentIndex + 1] * gaussianValue;
                                    sumRed += originalPixelData[currentIndex + 2] * gaussianValue;
                                }
                            }
                        }

                        int currentIndexPixel = (y * stride) + (x * bytesPerPixel);
                        blurredPixelData[currentIndexPixel] = (byte)Math.Max(0, Math.Min(255, sumBlue * factor));
                        blurredPixelData[currentIndexPixel + 1] = (byte)Math.Max(0, Math.Min(255, sumGreen * factor));
                        blurredPixelData[currentIndexPixel + 2] = (byte)Math.Max(0, Math.Min(255, sumRed * factor));
                        blurredPixelData[currentIndexPixel + 3] = originalPixelData[currentIndexPixel + 3];
                    }
                }

                writableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), blurredPixelData, stride, 0);
                return writableBitmap;
            }
            return null;
        }

        private byte GetMedian(List<byte> values)
        {
            values.Sort();
            return values[values.Count / 2];
        }
        public WriteableBitmap? Dilate(BitmapSource bitmapSource, int kernelSize)
        {
            if (bitmapSource != null)
            {
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;

                int bytesPerPixel = (bitmapSource.Format.BitsPerPixel + 7) / 8;
                int stride = width * bytesPerPixel;

                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                byte[] pixelData = new byte[stride * height];
                byte[] outputPixelData = new byte[stride * height];

                bitmapSource.CopyPixels(pixelData, stride, 0);

                int borderOffset = (kernelSize - 1) / 2;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * stride + x * bytesPerPixel;
                        byte maxIntensity = 0;

                        for (int j = -borderOffset; j <= borderOffset; j++)
                        {
                            for (int i = -borderOffset; i <= borderOffset; i++)
                            {
                                int neighborX = x + i;
                                int neighborY = y + j;

                                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                                {
                                    int neighborIndex = neighborY * stride + neighborX * bytesPerPixel;
                                    byte intensity = pixelData[neighborIndex];
                                    maxIntensity = Math.Max(maxIntensity, intensity);
                                }
                            }
                        }

                        outputPixelData[index] = maxIntensity;
                    }
                }

                outputImage.WritePixels(new Int32Rect(0, 0, width, height), outputPixelData, stride, 0);
                return outputImage;
            }
            return null;
        }

        public WriteableBitmap? Erode(BitmapSource bitmapSource, int kernelSize)
        {
            if (bitmapSource != null)
            {
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;

                int bytesPerPixel = (bitmapSource.Format.BitsPerPixel + 7) / 8;
                int stride = width * bytesPerPixel;

                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                byte[] pixelData = new byte[stride * height];
                byte[] outputPixelData = new byte[stride * height];

                bitmapSource.CopyPixels(pixelData, stride, 0);

                int borderOffset = (kernelSize - 1) / 2;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * stride + x * bytesPerPixel;
                        byte minIntensity = 255;

                        for (int j = -borderOffset; j <= borderOffset; j++)
                        {
                            for (int i = -borderOffset; i <= borderOffset; i++)
                            {
                                int neighborX = x + i;
                                int neighborY = y + j;

                                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                                {
                                    int neighborIndex = neighborY * stride + neighborX * bytesPerPixel;
                                    byte intensity = pixelData[neighborIndex];
                                    minIntensity = Math.Min(minIntensity, intensity);
                                }
                            }
                        }

                        outputPixelData[index] = minIntensity;
                    }
                }

                outputImage.WritePixels(new Int32Rect(0, 0, width, height), outputPixelData, stride, 0);
                return outputImage;
            }
            return null;
        }

        public WriteableBitmap? Open(BitmapSource bitmapSource, int kernelSize)
        {
            WriteableBitmap? tempErode = Erode(bitmapSource, kernelSize);
            if (tempErode != null)
            {
                return Dilate(tempErode, kernelSize);
            }
            return null;
        }

        public WriteableBitmap? Close(BitmapSource bitmapSource, int kernelSize)
        {
            WriteableBitmap? tempDilate = Dilate(bitmapSource, kernelSize);
            if (tempDilate != null)
            {
                return Erode(tempDilate, kernelSize);
            }
            return null;
        }
        public WriteableBitmap? HitOrMiss(BitmapSource bitmapSource, byte[,] mask)
        {
            if (bitmapSource != null && mask != null)
            {
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;

                int bytesPerPixel = (bitmapSource.Format.BitsPerPixel + 7) / 8;
                int stride = width * bytesPerPixel;

                WriteableBitmap outputImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                byte[] pixelData = new byte[stride * height];
                byte[] outputPixelData = new byte[stride * height];

                bitmapSource.CopyPixels(pixelData, stride, 0);

                int maskWidth = mask.GetLength(1);
                int maskHeight = mask.GetLength(0);
                int maskCenterX = maskWidth / 2;
                int maskCenterY = maskHeight / 2;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bool hit = true;

                        for (int j = 0; j < maskHeight; j++)
                        {
                            for (int i = 0; i < maskWidth; i++)
                            {
                                int neighborX = x + i - maskCenterX;
                                int neighborY = y + j - maskCenterY;

                                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                                {
                                    int neighborIndex = neighborY * stride + neighborX * bytesPerPixel;
                                    byte intensity = pixelData[neighborIndex];

                                    if (mask[j, i] != 255 && intensity != mask[j, i])
                                    {
                                        hit = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    hit = false;
                                    break;
                                }
                            }

                            if (!hit)
                                break;
                        }

                        outputPixelData[y * stride + x * bytesPerPixel] = hit ? (byte)255 : (byte)0;
                    }
                }
                outputImage.WritePixels(new Int32Rect(0, 0, width, height), outputPixelData, stride, 0);
                return outputImage;
            }
            return null;
        }

    }
}
