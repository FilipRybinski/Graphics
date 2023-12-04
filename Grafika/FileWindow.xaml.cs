using Grafika.Services;
using Grafika.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Grafika
{
    public partial class FileWindow : Window
    {
        public FileWindow(IFileSerivce fileSerivce)
        {
            InitializeComponent();
            DataContext = new FileViewModel(fileSerivce, this);
        }
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { }
    }
}
