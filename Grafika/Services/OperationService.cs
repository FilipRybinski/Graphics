using Grafika.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Grafika.Services
{
    public interface IOperationService
    {
        public void Operation(FileWindow fileWindow, OperationType operation);
    }
    class OperationService:IOperationService
    {
        public void Operation(FileWindow fileWindow, OperationType operation)
        {
            var value =convertValue(fileWindow);
            if (fileWindow.ImageView.Source is BitmapSource bitmapSource)
            {
                WriteableBitmap writableBitmap = new WriteableBitmap(bitmapSource);

                int width = writableBitmap.PixelWidth;
                int height = writableBitmap.PixelHeight;
                int bytesPerPixel = (writableBitmap.Format.BitsPerPixel + 7) / 8;
                int stride = width * bytesPerPixel;

                byte[] pixelData = new byte[height * stride];
                writableBitmap.CopyPixels(pixelData, stride, 0);

                for (int i = 0; i < pixelData.Length; i += bytesPerPixel)
                {
                    byte blue = pixelData[i];
                    byte green = pixelData[i + 1];
                    byte red = pixelData[i + 2];
                    switch (operation)
                    {
                        case OperationType.Add:
                            red = (byte)Math.Min(255, red + value);
                            green = (byte)Math.Min(255, green + value);
                            blue = (byte)Math.Min(255, blue + value);
                            break;
                        case OperationType.Substract:
                            red = (byte)Math.Max(0, red - value);
                            green = (byte)Math.Max(0, green - value);
                            blue = (byte)Math.Max(0, blue - value);

                            break;
                        case OperationType.Multiply:
                            red = (byte)Math.Min(255, red * value);
                            green = (byte)Math.Min(255, green * value);
                            blue = (byte)Math.Min(255, blue * value);
                            break;
                        case OperationType.Divide:
                            red = (byte)Math.Max(0, red / value);
                            green = (byte)Math.Max(0, green / value);
                            blue = (byte)Math.Max(0, blue / value);
                            break;
                        case OperationType.Brightness:
                            red = (byte)Math.Max(0, Math.Min(255, red + value));
                            green = (byte)Math.Max(0, Math.Min(255, green + value));
                            blue = (byte)Math.Max(0, Math.Min(255, blue + value));
                            break;
                        case OperationType.GrayAverage:
                            red = (byte)((blue + green + red) / 3);
                            green = (byte)((blue + green + red) / 3);
                            blue = (byte)((blue + green + red) / 3);
                            break;
                        case OperationType.GrayMax:
                            red = Math.Max(blue, Math.Max(green, red));
                            green = Math.Max(blue, Math.Max(green, red));
                            blue = Math.Max(blue, Math.Max(green, red));
                            break;


                    }
                    pixelData[i] = blue;
                    pixelData[i + 1] = green;
                    pixelData[i + 2] = red;
                }

                writableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelData, stride, 0);
                fileWindow.ImageView.Source = writableBitmap;
            }
        }
        private int convertValue(FileWindow fileWindow)
        {
            if (fileWindow.valueText.Text.Equals(""))
            {
                return 0;
            }
            return Int32.Parse(fileWindow.valueText.Text);
        }
    }
}
