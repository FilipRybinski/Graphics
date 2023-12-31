﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Grafika.Handler;
using Grafika.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Grafika
{
    public partial class MainWindow : Window
    {
        private ServiceProvider serviceProvider;
        public MainWindow()
        {
            InitializeComponent();
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IDrawService, DrawService>();
            services.AddSingleton<IFileSerivce, FileService>();
            services.AddSingleton<IMouseHandler, MouseHandler>();
            services.AddSingleton<IColorService, ColorService>();
            services.AddSingleton<IColorHandler, ColorHandler>();
            services.AddSingleton<IConversionService, ConversionService>();
            services.AddSingleton<IOperationService, OperationService>();
            services.AddSingleton<IFilterService, FilterService>();
            services.AddSingleton<IChartService, ChartService>();
            services.AddSingleton<IBinarizationService, BinarizationService>();
            services.AddSingleton<IAnalyzeService,ImageAnalyzeService>();
            services.AddTransient<IMainViewModel>(provider =>
            {
                var drawService = provider.GetRequiredService<IDrawService>();
                var fileService = provider.GetRequiredService<IFileSerivce>();
                var mouseHandler=provider.GetRequiredService<IMouseHandler>();
                var colorService=provider.GetRequiredService<IColorService>();
                var colorHandler=provider.GetRequiredService<IColorHandler>();
                var filterService = provider.GetRequiredService<IFilterService>();
                var operationService = provider.GetRequiredService<IOperationService>();
                var chartService=provider.GetRequiredService<IChartService>();
                var binarizationService=provider.GetRequiredService<IBinarizationService>();
                var analysisService = provider.GetRequiredService<IAnalyzeService>();
                return new MainViewModel(paintSurface, this, optionSurface, fileService, drawService,mouseHandler, colorService,colorHandler,filterService,operationService,chartService,binarizationService,analysisService);
            });
            serviceProvider = services.BuildServiceProvider();
            this.DataContext = serviceProvider.GetRequiredService<IMainViewModel>();
        }
        private void OpenFileLoader(object sender, RoutedEventArgs e){}
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e){}
        private void RadioButton(object sender, RoutedEventArgs e){}
        private void MouseDownHandler(object sender, MouseButtonEventArgs e){}
        private void MouseMoveHandler(object sender, MouseEventArgs e){}
        private void MouseUpHandler(object sender, MouseButtonEventArgs e){}
    }
}
