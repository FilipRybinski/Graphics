﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Grafika.Shared.Geometric
{
    public class Vertex : ObservableObject
    {
        public const int POINT_WIDTH = 10, POINT_HEIGHT = 10;

        private double x;
        public double X
        {
            get => x;
            set
            {
                var win = BezierB.Instance;
                win.Cover();
                x = value;
                win.Draw();
                OnPropertyChanged();
            }
        }
        private double y;
        public double Y
        {
            get => y;
            set
            {
                var win = BezierB.Instance;
                win.Cover();
                y = value;
                win.Draw();
                OnPropertyChanged();
            }
        }
        private bool isHighlighted;
        public bool IsHighlighted
        {
            get => isHighlighted;
            set { isHighlighted = value; OnPropertyChanged(); }
        }

        public Vertex(double x, double y)
        {
            this.x = x;
            this.y = y;
            IsHighlighted = false;
        }

        public void DrawRectangle(WriteableBitmap bmp, int color)
        {
            int xTopLeft = (int)X, yTopLeft = (int)Y;
            unsafe
            {
                int* p = (int*)bmp.BackBuffer;
                int bmpWid = bmp.PixelWidth, bmpHei = bmp.PixelHeight;
                int yLimit = yTopLeft + POINT_HEIGHT, xLimit = xTopLeft + POINT_WIDTH;
                for (int y = yTopLeft; y < yLimit; ++y)
                    for (int x = xTopLeft; x < xLimit; ++x)
                        if (x >= 0 && x < bmpWid && y >= 0 && y < bmpHei)
                        {
                            *(p + x + bmpWid * y) = color;
                            bmp.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                        }
            }
        }

        public bool Intersects(Point point)
        {
            double pX = point.X, pY = point.Y;
            return pX >= X && pX < X + POINT_WIDTH &&
                pY >= Y && pY < Y + POINT_HEIGHT;
        }

        public void SetXY(double x, double y)
        {
            var win = BezierB.Instance;
            win.Cover();
            SetXYWithoutRedraw(x, y);
            win.Draw();
        }

        public void SetXYWithoutRedraw(double x, double y)
        {
            this.x = x;
            this.y = y;
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public Vector Subtract(Point p)
        {
            return new Vector(X - p.X, Y - p.Y);
        }
    }
}
