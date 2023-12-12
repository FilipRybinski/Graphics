using Grafika.Services;
using Grafika.View_Models;
using System.Windows;


namespace Grafika
{
    public partial class FileWindow : Window
    {
        public FileWindow(IFileSerivce fileSerivce,IFilterService filterService, IOperationService operationService,IChartService chartService,IBinarizationService binarizationService,IAnalyzeService analysisService)
        {
            InitializeComponent();
            DataContext = new FileViewModel(fileSerivce, this,filterService,operationService,chartService,binarizationService,analysisService);
        }
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { }
    }
}
