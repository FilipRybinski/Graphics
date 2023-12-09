using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Grafika.Services
{
    public interface IFilterService
    {
        public WriteableBitmap? SmoothingFilter(BitmapSource bitmapSource);
        public WriteableBitmap? MedianFilter(BitmapSource bitmapSource);
        public WriteableBitmap? HighPassFilter(BitmapSource bitmapSource);
        public WriteableBitmap? GaussianFilter(BitmapSource bitmapSource);
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

    }
}
