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



        public void LoadImage(FileWindow fileWindow)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "PBM Files (*.pbm)|*.pbm|PGM Files (*.pgm)|*.pgm|PPM Files (*.ppm)|*.ppm",
                Title = "Load Image"
            };
            if (openFileDialog.ShowDialog() == true)
            {
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
                        if (!string.IsNullOrEmpty(line))
                        {
                            line = line.Split('#')[0].Trim();
                            IEnumerable<int> numbers = GetNumbersFromString(line);
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
                                throw new Exception();
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
                        formatType = ReadOneLine(binaryReader)?.Split('#')[0]?.Trim();
                        bool endStrings = false;
                        int width = 0;
                        int height = 0;
                        int colorValue = 255;
                        string line;
                        List<int> pixels = new List<int>();
                        while ((line=ReadOneLine(binaryReader)) != null && endStrings==false)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                line = line.Split('#')[0].Trim();
                                IEnumerable<int> numbers = GetNumbersFromString(line);
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
                                break;
                            case "P6":

                                fileWindow.ImageView.Source = CreatePpmImage(width, height, pixels);
                                return;
                            default:
                                throw new Exception();
                        }
                    });
                    }
                }
                throw new Exception();
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
                pixelData[index] = (byte)pixels[i + 2]; // Blue
                pixelData[index + 1] = (byte)pixels[i + 1]; // Green
                pixelData[index + 2] = (byte)pixels[i]; // Red
                pixelData[index + 3] = 255;   // Alpha
            }
            return CreateBitmap(width, height, pixelData);
        }
        private BitmapSource CreatePgmImage(int width, int height, List<int> pixels)
        {
            byte[] pixelData = new byte[width * height * 4];
            for (int i = 0; i < pixels.Count; i++)
            {
                byte color = (byte)pixels[i];
                int index = i * 4;
                pixelData[index] = color;
                pixelData[index + 1] = color;
                pixelData[index + 2] = color;
                pixelData[index + 3] = 255;
            }
            return CreateBitmap(width, height, pixelData);
        }
        private BitmapSource CreatePbmImage(int width,int height,List<int> pixels)
        {
            byte[] pixelData = new byte[width * height * 4];
            for (int i = 0; i < pixels.Count; i++)
            {
                byte color = pixels[i] == 1 ? (byte)0 : (byte)255;
                int index = i * 4;
                pixelData[index] = color;
                pixelData[index + 1] = color;
                pixelData[index + 2] = color;
                pixelData[index + 3] = 255;
            }
            return CreateBitmap(width, height, pixelData);

        }
        private BitmapSource CreateBitmap(int width,int height, byte[] data)
        {
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, data, width * 4);
        }
        private static IEnumerable<int> GetNumbersFromString(string line)
        {
            int startIndex = 0;
            int length = 0;
            bool isNumber = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (char.IsDigit(c))
                {
                    if (!isNumber)
                    {
                        startIndex = i;
                        length = 1;
                        isNumber = true;
                    }
                    else
                    {
                        length++;
                    }
                }
                else if (isNumber)
                {
                    yield return int.Parse(line.Substring(startIndex, length));
                    isNumber = false;
                }
            }

            if (isNumber)
            {
                yield return int.Parse(line.Substring(startIndex, length));
            }
        }

    }
}
