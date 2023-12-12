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
        public ICommand StretchChart { get; set; }
        public ICommand EqualizeChart { get; set; }
        public ICommand BinarizatioThreshold { get; set; }
        public ICommand BinarizationPercentBlackSelection { get; set; }
        public ICommand BinarizationMeanIterativeSelection { get; set; }
        public ICommand BinarizationOtsu { get; set; }
        public ICommand BinarizationNiblack { get; set; }
        public ICommand BinarizationSauvola { get; set; }
        public ICommand OpenFilter { get; set; }
        public ICommand CloseFilter { get; set;}
        public ICommand ErodeFilter { get; set; }
        public ICommand DilateFilter { get; set; }
        public ICommand HitOrMissFilter { get; set; }
        public ICommand GreenAnalysis { get; set; }
    }
    public class FileViewModel: IFileViewModel
    {
        private readonly IFileSerivce _fileService;
        private readonly IFilterService _filterService;
        private readonly IOperationService _operationService;
        private readonly IChartService _chartService;
        private readonly IBinarizationService _binarizationService;
        private readonly IAnalyzeService _AnalysisService;
        private FileWindow fileWindow;
        public FileViewModel(IFileSerivce fileServic,FileWindow _fileWindow, IFilterService filterService, IOperationService operationService, IChartService chartService, IBinarizationService binarizationService,IAnalyzeService analysisService)
        {
            fileWindow = _fileWindow;
            _fileService = fileServic;
            _filterService = filterService;
            _operationService = operationService;
            _chartService = chartService;
            _binarizationService = binarizationService;
            _AnalysisService = analysisService;
            LoadImage = new RelayCommand(Load);
            SaveImage = new RelayCommand(Save);
            AddPixels = new RelayCommand(Add);
            SubtractPixels = new RelayCommand(Substract);
            MultiplyPixels = new RelayCommand(Multiply);
            DividePixels = new RelayCommand(Divide);
            BrightnessPixels = new RelayCommand(Brightness);
            GrayScale = new RelayCommand(Gray);
            SmoothingFilter = new RelayCommand(Smothing);
            MedianFilter = new RelayCommand(Median);
            HighPassFilter = new RelayCommand(HighPass);
            GaussianFilter = new RelayCommand(Gaussian);
            StretchChart = new RelayCommand(Stretch);
            EqualizeChart = new RelayCommand(Equalize);
            BinarizatioThreshold = new RelayCommand(Binarization);
            BinarizationPercentBlackSelection = new RelayCommand(PercentBlackSelection);
            BinarizationMeanIterativeSelection=new RelayCommand(MeanIterativeSelection);
            BinarizationOtsu=new RelayCommand(Otus);
            BinarizationNiblack=new RelayCommand(Niblack);
            BinarizationSauvola = new RelayCommand(Sauvola);
            OpenFilter = new RelayCommand(Open);
            CloseFilter = new RelayCommand(Close);
            ErodeFilter = new RelayCommand(Erode);
            DilateFilter = new RelayCommand(Dilate);
            HitOrMissFilter=new RelayCommand(HitOrMiss);
            GreenAnalysis = new RelayCommand(Analysis);
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
        public ICommand StretchChart { get; set; }
        public ICommand EqualizeChart { get; set; }
        public ICommand BinarizatioThreshold { get; set; }
        public ICommand BinarizationPercentBlackSelection { get; set; }
        public ICommand BinarizationMeanIterativeSelection { get; set; }
        public ICommand BinarizationNiblack { get; set; }
        public ICommand BinarizationOtsu { get; set; }
        public ICommand BinarizationSauvola { get; set; }
        public ICommand OpenFilter { get; set; }
        public ICommand CloseFilter { get; set; }
        public ICommand ErodeFilter { get; set; }
        public ICommand DilateFilter { get; set; }
        public ICommand HitOrMissFilter { get; set; }
        public ICommand GreenAnalysis { get; set; }
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
            _operationService.Operation(fileWindow, OperationType.Add);
        }
        private void Substract(object sender)
        {
            _operationService.Operation(fileWindow, OperationType.Substract);
        }
        private void Multiply(object sender)
        {
            _operationService.Operation(fileWindow, OperationType.Multiply);
        }
        private void Divide(object sender)
        {
            _operationService.Operation(fileWindow, OperationType.Divide);
        }
        private void Brightness(object sender)
        {
            _operationService.Operation(fileWindow, OperationType.Brightness);
        }
        private void Gray(object sender)
        {
            _operationService.Operation(fileWindow, OperationType.GrayAverage);
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
        private void Stretch(object sender)
        {
            fileWindow.ImageView.Source = _chartService.StretchHistogram((BitmapSource)fileWindow.ImageView.Source);
        }
        private void Equalize(object sender)
        {
            fileWindow.ImageView.Source = _chartService.EqualizeHistogram((BitmapSource)fileWindow.ImageView.Source);
        }
        private void Binarization(object sender)
        {
            fileWindow.ImageView.Source = _binarizationService.Binarization((BitmapSource)fileWindow.ImageView.Source, fileWindow.valueBinText.Text.Equals("") ?  (byte)0 :byte.Parse(fileWindow.valueBinText.Text) );
        }
        private void PercentBlackSelection(object sender)
        {
            fileWindow.ImageView.Source = _binarizationService.PercentBlackSelection((BitmapSource)fileWindow.ImageView.Source, fileWindow.valueBinPerText.Text.Equals("") ? (int)0 : int.Parse(fileWindow.valueBinPerText.Text) );
        }
        private void MeanIterativeSelection(object sender)
        {
            fileWindow.ImageView.Source = _binarizationService.MeanIterativeSelection((BitmapSource)fileWindow.ImageView.Source);
        }
        private void Otus(object sender)
        {
            fileWindow.ImageView.Source = _binarizationService.Otsu((BitmapSource)fileWindow.ImageView.Source);
        }
        private void Niblack(object sender)
        {
            fileWindow.ImageView.Source = _binarizationService.Niblack((BitmapSource)fileWindow.ImageView.Source);
        }
        private void Sauvola(object sender)
        {
            fileWindow.ImageView.Source = _binarizationService.Sauvola((BitmapSource)fileWindow.ImageView.Source);
        }
        private void Close(object sender)
        {
            fileWindow.ImageView.Source = _filterService.Close((BitmapSource)fileWindow.ImageView.Source, fileWindow.valueKerText.Text.Equals("") ? (int)0 : int.Parse(fileWindow.valueKerText.Text));
        }
        private void Open(object sender)
        {
            fileWindow.ImageView.Source = _filterService.Open((BitmapSource)fileWindow.ImageView.Source, fileWindow.valueKerText.Text.Equals("") ? (int)0 : int.Parse(fileWindow.valueKerText.Text));
        }
        private void Erode(object sender)
        {
            fileWindow.ImageView.Source = _filterService.Erode((BitmapSource)fileWindow.ImageView.Source, fileWindow.valueKerText.Text.Equals("") ? (int)0 : int.Parse(fileWindow.valueKerText.Text));
        }
        private void Dilate(object sender)
        {
            fileWindow.ImageView.Source = _filterService.Dilate((BitmapSource)fileWindow.ImageView.Source, fileWindow.valueKerText.Text.Equals("") ? (int)0 : int.Parse(fileWindow.valueKerText.Text));
        }
        private void HitOrMiss(object sender)
        {
            byte[,] mask = new byte[,]
{
                { 0, 255, 0 },
                { 0, 255, 255 },
                { 0, 0, 0 }
            };
            fileWindow.ImageView.Source = _filterService.HitOrMiss((BitmapSource)fileWindow.ImageView.Source,mask);
        }
        private void Analysis(object sender)
        {
            _AnalysisService.AnalyzeImage((BitmapImage)fileWindow.ImageView.Source, fileWindow.valueAnalText.Text.Equals("") ? (int)0 : int.Parse(fileWindow.valueAnalText.Text), fileWindow);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
