using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Grafika.Handler
{
    public interface IColorHandler
    {
        public void AttachRGB(MainWindow window);
        public void AttachCMYK(MainWindow window);
        public void AttachHSV(MainWindow window);
        public void AttachSelectedType(MainWindow window);
        public ColorHandler getInstance();
    }
    public class ColorHandler : IColorHandler
    {
        public event RoutedPropertyChangedEventHandler<double> REvent;
        public event RoutedPropertyChangedEventHandler<double> GEvent;
        public event RoutedPropertyChangedEventHandler<double> BEvent;

        public event RoutedPropertyChangedEventHandler<double> CEvent;
        public event RoutedPropertyChangedEventHandler<double> MEvent;
        public event RoutedPropertyChangedEventHandler<double> YEvent;
        public event RoutedPropertyChangedEventHandler<double> KEvent;

        public event RoutedPropertyChangedEventHandler<double> HEvent;
        public event RoutedPropertyChangedEventHandler<double> SEvent;
        public event RoutedPropertyChangedEventHandler<double> VEvent;

        public event RoutedEventHandler selected;
        public void AttachRGB(MainWindow window)
        {
            window.redSlider.ValueChanged += RChange;
            window.greenSlider.ValueChanged += GChange;
            window.blueSlider.ValueChanged += BChange;
        }
        public void AttachCMYK(MainWindow window)
        {
            window.cyanSlider.ValueChanged += CChange;
            window.magentaSlider.ValueChanged += MChange;
            window.yellowSlider.ValueChanged += YChange;
            window.blackSlider.ValueChanged += KChange;
        }
        public void AttachHSV(MainWindow window)
        {
            window.hueSlider.ValueChanged += HChange;
            window.saturationSlider.ValueChanged += SChange;
            window.valueSlider.ValueChanged += VChange;
        }
        public void AttachSelectedType(MainWindow window)
        {
            window.RGB.Checked += Checked;
            window.CMYK.Checked += Checked;
            window.HSV.Checked += Checked;
        }
        public ColorHandler getInstance()
        {
            return this;
        }
        private void RChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            REvent?.Invoke(sender, e);
        }
        private void GChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GEvent?.Invoke(sender, e);
        }
        private void BChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BEvent?.Invoke(sender, e);
        }



        private void CChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CEvent?.Invoke(sender, e);
        }
        private void MChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MEvent?.Invoke(sender, e);
        }
        private void YChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            YEvent?.Invoke(sender, e);
        }
        private void KChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            KEvent?.Invoke(sender, e);
        }



        private void HChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HEvent?.Invoke(sender, e);
        }
        private void SChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SEvent?.Invoke(sender, e);
        }
        private void VChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VEvent?.Invoke(sender, e);
        }


        private void Checked(object sender, RoutedEventArgs e)
        {
            selected.Invoke(sender, e);
        }
    }
}
