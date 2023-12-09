using Grafika;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Grafika.Services;

namespace Test
{
    public class Tests
    {
        [Fact]
        public void SmoothingFilterTest()
        {
            int[,] pixels = {
            { 10, 20, 30, 40, 50 },
            { 60, 70, 80, 90, 100 },
            { 110, 120, 130, 140, 150 },
            { 160, 170, 180, 190, 200 },
            { 210, 220, 230, 240, 250 }
            };
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            WriteableBitmap testImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);

            for (int y = 0; y < testImage.PixelHeight; y++)
            {
                for (int x = 0; x < testImage.PixelWidth; x++)
                {
                    int pixelValue = pixels[y, x];
                    byte[] colorData = { (byte)pixelValue, (byte)pixelValue, (byte)pixelValue, 255 };

                    Int32Rect rect = new Int32Rect(x, y, 1, 1);
                    testImage.WritePixels(rect, colorData, 4, 0);
                }
            }
            FilterService imageProcessing = new FilterService();
            WriteableBitmap smoothedImage = imageProcessing.SmoothingFilter(testImage);
            Assert.NotNull(smoothedImage);
            Assert.Equal(width, smoothedImage.PixelWidth);
            Assert.Equal(height, smoothedImage.PixelHeight);
            byte[] pixelData = new byte[4];
            smoothedImage.CopyPixels(new Int32Rect(2, 2, 1, 1), pixelData, 4, 0);
            int expectedSmoothedValue = (pixels[1, 1] + pixels[1, 2] + pixels[1, 3] +
                                          pixels[2, 1] + pixels[2, 2] + pixels[2, 3] +
                                          pixels[3, 1] + pixels[3, 2] + pixels[3, 3]) / 9;
            int smoothedPixelValue = pixelData[0];
            Assert.Equal(expectedSmoothedValue, smoothedPixelValue);
        }
        [Fact]
        public void HighPassFilterTest()
        {
            int[,] pixels = {
                { 10, 20, 30 },
                { 40, 50, 60 },
                { 70, 80, 90 }
            };
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            WriteableBitmap testImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);

            for (int y = 0; y < testImage.PixelHeight; y++)
            {
                for (int x = 0; x < testImage.PixelWidth; x++)
                {
                    int pixelValue = pixels[y, x];
                    byte[] colorData = { (byte)pixelValue, (byte)pixelValue, (byte)pixelValue, 255 };
                    Int32Rect rect = new Int32Rect(x, y, 1, 1);
                    testImage.WritePixels(rect, colorData, 4, 0);
                }
            }
            FilterService imageProcessing = new FilterService();
            WriteableBitmap highPassImage = imageProcessing.HighPassFilter(testImage);
            Assert.NotNull(highPassImage);
            byte[] pixelData = new byte[4];
            highPassImage.CopyPixels(new Int32Rect(1, 1, 1, 1), pixelData, 4, 0);
            int expectedHighPassValue = 50 * 4 - 20 - 40 - 80 - 60;
            expectedHighPassValue = Math.Max(0, Math.Min(255, expectedHighPassValue));
            int highPassPixelValue = pixelData[0];
            Assert.Equal(expectedHighPassValue, highPassPixelValue);
        }
    }
}