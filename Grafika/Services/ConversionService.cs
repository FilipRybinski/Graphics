using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafika.Services
{
    public interface IConversionService
    {
        public int[] RGBtoHSV(MainWindow window, byte red, byte green, byte blue);
        public int[] RGBtoCMYK(MainWindow window, byte red, byte green, byte blue);
        public int[] CMYKtoRGB(MainWindow window, byte cyan, byte magenta, byte yellow, byte black);
        public int[] HSVtoRGB(MainWindow window, byte hue, byte saturation, byte value);
    }
    public class ConversionService : IConversionService
    {
        public int[] RGBtoHSV(MainWindow window, byte red, byte green, byte blue)
        {
            float r = red / 255.0f;
            float g = green / 255.0f;
            float b = blue / 255.0f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            float hue = 0;
            if (delta != 0)
            {
                if (max == r)
                    hue = ((g - b) / delta) % 6;
                else if (max == g)
                    hue = ((b - r) / delta) + 2;
                else
                    hue = ((r - g) / delta) + 4;

                hue *= 60;
                if (hue < 0)
                    hue += 360;
            }

            float saturation = (max != 0) ? delta / max : 0;
            float value = max;

            float scaledHue = (hue / 360) * 100;
            float scaledSaturation = saturation * 100;
            float scaledValue = value * 100;

            return new int[] { (int)Math.Round(scaledHue), (int)Math.Round(scaledSaturation), (int)Math.Round(scaledValue) };

        }
        public int[] RGBtoCMYK(MainWindow window, byte red, byte green, byte blue)
        {
            float r = red / 255.0f;
            float g = green / 255.0f;
            float b = blue / 255.0f;

            float k = 1 - Math.Max(Math.Max(r, g), b);

            float c = (1 - r - k) / (1 - k + float.Epsilon);
            float m = (1 - g - k) / (1 - k + float.Epsilon);
            float y = (1 - b - k) / (1 - k + float.Epsilon);

            if (float.IsNaN(c)) c = 0;
            if (float.IsNaN(m)) m = 0;
            if (float.IsNaN(y)) y = 0;
            float scaledCyan = c * 100;
            float scaledMagenta = m * 100;
            float scaledYellow = y * 100;
            float scaledKey = k * 100;

            return new int[] { (int)scaledCyan, (int)scaledMagenta, (int)scaledYellow, (int)scaledKey };
        }

        public int[] CMYKtoRGB(MainWindow window, byte cyan, byte magenta, byte yellow, byte black)
        {
            double c = cyan / 100.0;
            double m = magenta / 100.0;
            double y = yellow / 100.0;
            double b = black / 100.0;

            double red = 255 * (1 - c) * (1 - b);
            double green = 255 * (1 - m) * (1 - b);
            double blue = 255 * (1 - y) * (1 - b);

            return new int[] { (int)Math.Min(255, Math.Max(0, red)), (int)Math.Min(255, Math.Max(0, green)), (int)Math.Min(255, Math.Max(0, blue)) };
        }
        
        public int[] HSVtoRGB(MainWindow window, byte hue, byte saturation, byte value)
        {
            int convertedHue = (hue % 360 + 360) % 360;
            float convertedSaturation = saturation / 100.0f;
            float convertedValue = value / 100.0f;
            float hueSegment = convertedHue / 60.0f;
            int hueSegmentIndex = (int)Math.Floor(hueSegment);
            float fractionalHue = hueSegment - hueSegmentIndex;
            float p = convertedValue * (1 - convertedSaturation);
            float q = convertedValue * (1 - convertedSaturation * fractionalHue);
            float t = convertedValue * (1 - convertedSaturation * (1 - fractionalHue));
            float red, green, blue;
            switch (hueSegmentIndex)
            {
                case 0:
                    { red = convertedValue; green = t; blue = p; }
                    break;
                case 1:
                    { red = q; green = convertedValue; blue = p; }
                    break;
                case 2:
                    { red = p; green = convertedValue; blue = t; }
                    break;
                case 3:
                    { red = p; green = q; blue = convertedValue; }
                    break;
                case 4:
                    { red = t; green = p; blue = convertedValue; }
                    break;
                case 5:
                    { red = convertedValue; green = p; blue = q; }
                    break;
                default: throw new Exception();
            }
            return new int[] { (int)(red * 255), (int)(green * 255), (int)(blue * 255) };
        }
    }
}

