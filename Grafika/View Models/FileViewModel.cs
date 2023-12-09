using Grafika.Commands;
using Grafika.Enums;
using Grafika.Handler;
using Grafika.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Grafika.View_Models
{
    public interface IFileViewModel : INotifyPropertyChanged
    {
        public ICommand LoadImage { get; set; }
        public ICommand SaveImage { get; set; }
        public ICommand AddPixels { get; set; }
        public ICommand SubtractPixels { get; set; }
        public ICommand MultiplyPixels { get; set; }
        public ICommand DividePixels { get; set; }
        public ICommand BrightnessPixels { get; set; }
        public ICommand GrayScale { get; set; }
        public ICommand SmoothingFilter { get; set; }
        public ICommand MedianFilter { get; set; }
        public ICommand HighPassFilter { get; set; }
        public ICommand GaussianFilter { get; set; }
    }
    public class FileViewModel: IFileViewModel
    {
        private readonly IFileSerivce _fileService;
        private readonly IFilterService _filterService;
        private readonly IOperationService _operationService;
        private FileWindow fileWindow;
        public FileViewModel(IFileSerivce fileServic,FileWindow _fileWindow,IFilterService filterService,IOperationService operationService)
        {
            fileWindow= _fileWindow;
            _fileService = fileServic;
            _filterService = filterService;
            _operationService = operationService;
            LoadImage = new RelayCommand(Load);
            SaveImage = new RelayCommand(Save);
            AddPixels = new RelayCommand(Add);
            SubtractPixels = new RelayCommand(Substract);
            MultiplyPixels=new RelayCommand(Multiply);
            DividePixels = new RelayCommand(Divide);
            BrightnessPixels = new RelayCommand(Brightness);
            GrayScale = new RelayCommand(Gray);
            SmoothingFilter = new RelayCommand(Smothing);
            MedianFilter = new RelayCommand(Median);
            HighPassFilter=new RelayCommand(HighPass);
            GaussianFilter=new RelayCommand(Gaussian);
        }
        public ICommand LoadImage { get; set; }
        public ICommand SaveImage { get; set; }
        public ICommand AddPixels { get; set; }
        public ICommand SubtractPixels { get; set; }
        public ICommand MultiplyPixels { get; set; }
        public ICommand DividePixels { get; set; }
        public ICommand BrightnessPixels { get; set; }
        public ICommand GrayScale { get; set; }
        public ICommand SmoothingFilter { get; set; }
        public ICommand MedianFilter { get; set; }
        public ICommand HighPassFilter { get; set; }
        public ICommand GaussianFilter { get; set; }
        private void Save(object sender)
        {
            _fileService.SaveImage(fileWindow);
        }
        private void Load(object sender)
        {
            _fileService.LoadImage(fileWindow);
        }
        private void Add(object sender)
        {
            _operationService.Operation(fileWindow, "add");
        }
        private void Substract(object sender)
        {
            _operationService.Operation(fileWindow, "substract");
        }
        private void Multiply(object sender)
        {
            _operationService.Operation(fileWindow, "multiply");
        }
        private void Divide(object sender)
        {
            _operationService.Operation(fileWindow, "divide");
        }
        private void Brightness(object sender)
        {
            _operationService.Operation(fileWindow, "brightness");
        }
        private void Gray(object sender)
        {
            _operationService.Operation(fileWindow, "gray-average");
        }
        private void Smothing(object sender)
        {
            fileWindow.ImageView.Source=_filterService.SmoothingFilter((BitmapSource)fileWindow.ImageView.Source);
        }
        private void Median(object sender)
        {
            fileWindow.ImageView.Source = _filterService.MedianFilter((BitmapSource)fileWindow.ImageView.Source);
        }
        private void HighPass(object sender)
        {
            fileWindow.ImageView.Source =_filterService.HighPassFilter((BitmapSource)fileWindow.ImageView.Source);
        }
        private void Gaussian(object sender)
        {
            fileWindow.ImageView.Source = _filterService.GaussianFilter((BitmapSource)fileWindow.ImageView.Source);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
