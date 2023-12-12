﻿
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Grafika.Shared
{
    public class HighlightablePoint : INotifyPropertyChanged
    {
        private double x;
        public double X
        {
            get => x;
            set
            {
                var bmp = (WriteableBitmap)owner.Image.Source;
                if (!(value >= 0 && value + BezierA.POINT_WIDTH < bmp.PixelWidth))
                {
                    MessageBox.Show($"Podaj współrzędną X w przedziale <{0}," +
                        $"{bmp.PixelWidth - BezierA.POINT_WIDTH}).");
                    return;
                }
                owner.Cover();
                x = value;
                owner.Draw();
                OnPropertyChanged(nameof(X));
            }
        }
        private double y;
        public double Y
        {
            get => y;
            set
            {
                var bmp = (WriteableBitmap)owner.Image.Source;
                if (!(value >= 0 && value + BezierA.POINT_HEIGHT < bmp.PixelHeight))
                {
                    MessageBox.Show($"Podaj współrzędną Y w przedziale <{0}," +
                        $"{bmp.PixelHeight - BezierA.POINT_HEIGHT}).");
                    return;
                }
                owner.Cover();
                y = value;
                owner.Draw();
                OnPropertyChanged(nameof(Y));
            }
        }
        private bool isHighlighted;
        public bool IsHighlighted
        {
            get => isHighlighted;
            set { isHighlighted = value; OnPropertyChanged(nameof(IsHighlighted)); }
        }
        private BezierA owner;

        public HighlightablePoint(double x, double y, BezierA owner)
        {
            this.x = x;
            this.y = y;
            IsHighlighted = false;
            this.owner = owner;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
