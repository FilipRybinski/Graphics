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
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Reflection.PortableExecutable;
using Grafika.Handler;
using Grafika.Commands;
using System.Windows.Input;

namespace Grafika.Services
{
    public interface IFileSerivce
    {
        public void SaveFile(Canvas paintSurface);
        public  void LoadImage(FileWindow fileWindow);
        public void SaveImage(FileWindow fileWindow);
    }
    public class FileService : IFileSerivce
    {
        private CommandQueueHandler _commandQueueHandler = new CommandQueueHandler();

        public void SaveFile(Canvas paintSurface)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif|All Files|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                SaveSurfaceAsImage(paintSurface, fileName);
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

        public void SaveImage(FileWindow fileWindow)
        {
            if (fileWindow.ImageView.Source == null)
            {
                MessageBox.Show("Musisz na poczatku cos wczytac zeby zapisac");
                return;
            }
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PBM Files (*.pbm)|*.pbm|PGM Files (*.pgm)|*.pgm|PPM Files (*.ppm)|*.ppm |JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png",
                Title = "Save Image"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                switch(saveFileDialog.FileName.Substring(saveFileDialog.FileName.Length- 3).ToLower())
                {
                    case "ppm":
                        SaveAsPpm(fileWindow.ImageView.Source, saveFileDialog.FileName);
                        break;
                    case "pbm":
                        SaveAsPbm(fileWindow.ImageView.Source, saveFileDialog.FileName);
                        break;
                    case "pgm":
                        SaveAsPgm(fileWindow.ImageView.Source, saveFileDialog.FileName);
                        break;
                    case "jpg":
                        SaveAsJpg(fileWindow.ImageView.Source, saveFileDialog.FileName);
                        break;
                    case "png":
                        SaveAsPng(fileWindow.ImageView.Source, saveFileDialog.FileName);
                        break;
                    default:
                        throw new FormatException();
                }
            }

        }

        public void LoadImage(FileWindow fileWindow)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "PBM Files (*.pbm)|*.pbm|PGM Files (*.pgm)|*.pgm|PPM Files (*.ppm)|*.ppm|JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png",
                Title = "Load Image"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileExtenstion = openFileDialog.FileName.Substring(openFileDialog.FileName.Length - 3).ToLower();
                if (fileExtenstion.Equals("jpg") || fileExtenstion.Equals("png")){
                    fileWindow.ImageView.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                }
                ICommand loadImageCommand = new RelayCommand(param => {
                    ReadFile(openFileDialog.FileName,fileWindow);
                });
                _commandQueueHandler.EnqueueCommand(loadImageCommand);
            }
        }
        private void ReadFile(string filePath,FileWindow fileWindow)
        {
            string formatType;
            using (var streamReader=new StreamReader(filePath))
            {
                formatType = streamReader.ReadLine()?.Split('#')[0]?.Trim();
                if(new List<string> { "P1", "P2", "P3" }.Contains(formatType))
                {
                    int width=0;
                    int height=0;
                    int colorValue = 1;
                    string line;
                    List<int> pixels = new List<int>();
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        line = line.Split('#')[0];
                        if (!string.IsNullOrEmpty(line))
                        {
                            int[] numbers = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(e => Convert.ToInt32(e)).ToArray();
                            Console.WriteLine(numbers);
                            foreach (int match in numbers)
                            {
                                if (width == 0 )
                                {
                                    width = match;
                                    continue;
                                }
                                else if (width != 0 && height == 0)
                                {
                                    height = match;
                                    continue;
                                }else if (formatType != "P1" && colorValue==1)
                                {
                                    colorValue = match;
                                    continue;
                                }
                                else
                                {
                                    pixels.Add(255 / colorValue * match);
                                }

                            }
                               
                        }
                    }
                    fileWindow.Dispatcher.Invoke(() =>
                    {
                        switch (formatType)
                        {
                            case "P1":
                                fileWindow.ImageView.Source = CreatePbmImage(width, height, pixels);
                                return;
                            case "P2":
                                fileWindow.ImageView.Source = CreatePgmImage(width, height, pixels);
                                return;
                            case "P3":
                                fileWindow.ImageView.Source = CreatePpmImage(width, height, pixels);
                                return;
                            default:
                                throw new FormatException();
                        }
                    });
                }
            }
            if (new List<string> { "P4", "P5", "P6" }.Contains(formatType))
                {
                    using ( var fileStream=new FileStream(filePath,FileMode.Open, FileAccess.Read))
                    using (var bufferedStream = new BufferedStream(fileStream))
                    using ( var binaryReader=new BinaryReader(bufferedStream))
                    {
                        ReadOneLine(binaryReader)?.Split('#')[0]?.Trim(); // skip line
                        bool endStrings = false;
                        int width = 0;
                        int height = 0;
                        int colorValue = 255;
                        string line;
                        List<int> pixels = new List<int>();
                        while ((line=ReadOneLine(binaryReader)) != null && endStrings==false)
                        {
                            line = line.Split('#')[0];
                            if (!string.IsNullOrEmpty(line))
                            {
                                int[] numbers = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(e => Convert.ToInt32(e)).ToArray();
                                foreach (int match in numbers)
                                    {
                                    if (width == 0)
                                    {
                                        width = match;
                                        continue;
                                    }
                                    else if (width != 0 && height == 0)
                                    {
                                        height = match;
                                        if (formatType == "P4")
                                        {
                                            endStrings = true;
                                            break;
                                        }
                                    
                                    }
                                    else if (formatType != "P4" && colorValue == 255)
                                    {
                                        colorValue =match;
                                        endStrings = true;
                                        break;
                                    }
                                }
                            if (endStrings) break;
                            }
                        }
                        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                        {
                            pixels.Add(255 / colorValue * binaryReader.ReadByte());
                        }
                        if (formatType == "P4")
                        {
                            pixels.RemoveAt(0);
                        }
                        fileWindow.Dispatcher.Invoke(() =>
                        {
                            switch (formatType)
                            {
                                case "P4":
                                    fileWindow.ImageView.Source = CreatePbmImage(width, height, pixels);
                                    return;
                                case "P5":
                                    fileWindow.ImageView.Source = CreatePgmImage(width, height, pixels);
                                    break;
                                case "P6":
                                    fileWindow.ImageView.Source = CreatePpmImage(width, height, pixels);
                                    return;
                                default:
                                    throw new FormatException();
                            }
                        });
                    }
            }
        }
        private string ReadOneLine(BinaryReader reader)
        {
            StringBuilder line = new StringBuilder();
            char c;
            while ((c = (char)reader.ReadByte()) != '\n')
            {
                line.Append(c);
            }
            return line.ToString();

        }
        private BitmapSource CreatePpmImage(int width, int height, List<int> pixels)
        {
            byte[] pixelData = new byte[width * height * 4];
            for (int i = 0; i < pixels.Count; i += 3)
            {
                int index = i / 3 * 4;
                pixelData[index] = (byte)pixels[i + 2];
                pixelData[index + 1] = (byte)pixels[i + 1];
                pixelData[index + 2] = (byte)pixels[i];
                pixelData[index + 3] = 255;
            }
            return CreateBitmap(width, height, pixelData);
        }
        private BitmapSource CreatePgmImage(int width, int height, List<int> pixels)
        {
            byte[] pixelData = new byte[width * height * 4];

            for (int i = 0; i < pixels.Count; i++)
            {
                byte grayValue = (byte)pixels[i];
                pixelData[i * 4] = grayValue;
                pixelData[i * 4 + 1] = grayValue;
                pixelData[i * 4 + 2] = grayValue; 
                pixelData[i * 4 + 3] = 255; 
            }

            return CreateBitmap(width, height, pixelData);
        }
        private BitmapSource CreatePbmImage(int width,int height,List<int> pixels)
        {
            byte[] pixelData = new byte[width * height * 4];

            for (int i = 0; i < pixels.Count; i++)
            {
                byte pixelValue = (byte)(pixels[i] == 0 ? 0 : 255); 
                pixelData[i * 4] = pixelValue; 
                pixelData[i * 4 + 1] = pixelValue;
                pixelData[i * 4 + 2] = pixelValue; 
                pixelData[i * 4 + 3] = 255;
            }


            return CreateBitmap(width, height, pixelData);

        }
        private BitmapSource CreateBitmap(int width,int height, byte[] data)
        {
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, data, width * 4);
        }

        private void SaveAsPpm(ImageSource imageSource, string filePath)
        {
            BitmapSource bitmapSource = (BitmapSource)imageSource;
            byte[] pixelData = ImageSourceToByteArray(bitmapSource);

            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("P3");
                writer.WriteLine($"{width} {height}");
                writer.WriteLine("255");

                for (int i = 0; i < pixelData.Length; i += 4)
                {
                    writer.Write($"{pixelData[i + 2]} ");
                    writer.Write($"{pixelData[i + 1]} ");
                    writer.Write($"{pixelData[i]} ");
                }
            }
        }

        private void SaveAsPbm(ImageSource imageSource, string filePath)
        {
            BitmapSource bitmapSource = (BitmapSource)imageSource;
            byte[] pixelData = ImageSourceToByteArray(bitmapSource);

            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("P1");
                writer.WriteLine($"{width} {height}");

                for (int i = 0; i < pixelData.Length; i += 4)
                {
                    int grayscale = (int)(0.299 * pixelData[i + 2] + 0.587 * pixelData[i + 1] + 0.114 * pixelData[i]);
                    int pbmValue = (grayscale < 128) ? 1 : 0;
                    writer.Write($"{pbmValue} ");
                }
            }
        }

        private void SaveAsPgm(ImageSource imageSource, string filePath)
        {
            BitmapSource bitmapSource = (BitmapSource)imageSource;
            byte[] pixelData = ImageSourceToByteArray(bitmapSource);

            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("P2");
                writer.WriteLine($"{width} {height}");
                writer.WriteLine("255");

                for (int i = 0; i < pixelData.Length; i += 4)
                {
                    int grayscale = (int)(0.299 * pixelData[i + 2] + 0.587 * pixelData[i + 1] + 0.114 * pixelData[i]);
                    writer.Write($"{grayscale} ");
                }
            }
        }
        private void SaveAsPng(ImageSource imageSource, string filePath)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }
        private void SaveAsJpg(ImageSource imageSource, string filePath)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }
        private byte[] ImageSourceToByteArray(BitmapSource bitmapSource)
        {
            byte[] pixelData = new byte[bitmapSource.PixelWidth * bitmapSource.PixelHeight * 4];
            bitmapSource.CopyPixels(pixelData, bitmapSource.PixelWidth * 4, 0);
            return pixelData;
        }
    }
}
