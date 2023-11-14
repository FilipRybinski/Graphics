
using System;
using System.Windows;
using System.Windows.Media;

namespace Grafika.Services
{
    public interface IColorService
    {
        public void updateRGB(MainWindow window);
        public void updateCMYK(MainWindow window);
        public void updateHSV(MainWindow window);
    }
    public class ColorService : IColorService
    {
        private readonly IDrawService _drawService;
        private readonly IConversionService _conversionService;
        public ColorService(IDrawService drawService,IConversionService conversionService) {
            _drawService = drawService;
            _conversionService = conversionService;
        }
        public void updateRGB(MainWindow window)
        {
            byte red = (byte)window.redSlider.Value;
            byte green = (byte)window.greenSlider.Value;
            byte blue = (byte)window.blueSlider.Value;
            _drawService.color=Color.FromRgb(red, green, blue);
            var RGBtoHSV= _conversionService.RGBtoHSV(window, red, green, blue);
            window.hueSlider.Value = RGBtoHSV[0];
            window.saturationSlider.Value = RGBtoHSV[1];
            window.valueSlider.Value = RGBtoHSV[2];
            var RGBtoCMYK = _conversionService.RGBtoCMYK(window, red, green, blue);
            window.cyanSlider.Value = RGBtoCMYK[0];
            window.magentaSlider.Value = RGBtoCMYK[1];
            window.yellowSlider.Value = RGBtoCMYK[2];
            window.blackSlider.Value = RGBtoCMYK[3];
            updatePreviewColor(window);
        }
        public void updateCMYK(MainWindow window)
        {
            byte cyan = (byte)window.cyanSlider.Value;
            byte magenta = (byte)window.magentaSlider.Value;
            byte yellow = (byte)window.yellowSlider.Value;
            byte black = (byte)window.blackSlider.Value;
            _drawService.color = Color.FromRgb(cyan, magenta, yellow);
            var CMYKtoRGB= _conversionService.CMYKtoRGB(window, cyan, magenta, yellow, black);
            window.redSlider.Value = CMYKtoRGB[0];
            window.greenSlider.Value = CMYKtoRGB[1];
            window.blueSlider.Value = CMYKtoRGB[2];
            var RGBtoHSV= _conversionService.RGBtoCMYK(window, (byte)CMYKtoRGB[0], (byte)CMYKtoRGB[1], (byte)CMYKtoRGB[2]);
            window.hueSlider.Value = RGBtoHSV[0];
            window.saturationSlider.Value = RGBtoHSV[1];
            window.valueSlider.Value = RGBtoHSV[2];
            updatePreviewColor(window);
        }
        public void updateHSV(MainWindow window)
        {
            byte hue = (byte)window.hueSlider.Value;
            byte saturation = (byte)window.saturationSlider.Value;
            byte value = (byte)window.valueSlider.Value;
            var HSVtoRGB=_conversionService.HSVtoRGB(window, hue, saturation, value);
            window.redSlider.Value = HSVtoRGB[0];
            window.greenSlider.Value = HSVtoRGB[1];
            window.blueSlider.Value = HSVtoRGB[2];
            var RGBtoCMYK = _conversionService.RGBtoCMYK(window, (byte)HSVtoRGB[0], (byte)HSVtoRGB[1], (byte)HSVtoRGB[2]);
            window.cyanSlider.Value = RGBtoCMYK[0];
            window.magentaSlider.Value = RGBtoCMYK[1];
            window.yellowSlider.Value = RGBtoCMYK[2];
            window.blackSlider.Value = RGBtoCMYK[3];
            updatePreviewColor(window);
        }
        private void updatePreviewColor(MainWindow window)
        {
            byte red = (byte)window.redSlider.Value;
            byte green = (byte)window.greenSlider.Value;
            byte blue = (byte)window.blueSlider.Value;
            window.colorPreview.Fill = new SolidColorBrush(Color.FromRgb(red, green, blue));
        }
    }
}
