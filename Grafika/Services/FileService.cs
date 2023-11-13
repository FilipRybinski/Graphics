using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Grafika.Services
{
    public interface IFileSerivce
    {
        public void SaveFile();
    }
    public class FileService : IFileSerivce
    {
        Canvas paintSufrace;
        public FileService(Canvas canvas)
        {
            paintSufrace= canvas;
        }
        public void SaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif|All Files|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                SaveSurfaceAsImage(paintSufrace, fileName);
            }
        }
        private void SaveSurfaceAsImage(Canvas canvas, string fileName)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream fs = File.Create(fileName))
            {
                encoder.Save(fs);
            }
        }

    }
}
