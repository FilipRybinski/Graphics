using Grafika.Enums;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(paintSurface, this,optionSurface);
        }
        private void MouseDownHandler(object sender, MouseButtonEventArgs e){}
        private void MouseMoveHandler(object sender, MouseEventArgs e){}
        private void MouseUpHandler(object sender, MouseButtonEventArgs e){}
    }
}
