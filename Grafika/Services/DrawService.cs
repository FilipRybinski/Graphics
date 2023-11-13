
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafika.Services
{
    public interface IDrawService
    {
        public Point Point { get; set; }
        public Line Line(double x1, double y1, double x2, double y2);
        public Polygon Triangle();
        public TextBox Text(double x, double y);
        public Ellipse Circle(double x, double y);
        public Rectangle Square(double x, double y);
        public UIElement? selected { get; set; }
        public Line? currentLine { get; set; }
        public Polygon? currentTriangle { get; set; }
        public Rectangle? currentSquare { get; set; }
        public Ellipse? currentCircle { get; set; }
        public TextBlock? currentTextBlock { get; set; }
        public bool isDrawing { get; set; }
        public void BindSelected(HitTestResult hitResult, Point point);
        public void Rotate(Point mousePosition);
        public void Reset();
    }
    public class DrawService : IDrawService
    {
        public Point Point { get; set; } = new Point();
        public UIElement? selected { get; set; } = null;
        public Line? currentLine { get; set; } = null;
        public Polygon? currentTriangle { get; set; } = null;
        public Rectangle? currentSquare { get; set; } = null;
        public Ellipse? currentCircle { get; set; } = null;
        public TextBlock? currentTextBlock { get; set; } = null;
        public bool isDrawing { get; set; } = false;
        public Line Line(double x1, double y1, double x2, double y2)
        {
            return new Line()
            {
                Stroke = SystemColors.WindowFrameBrush,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
        }
        public Polygon Triangle()
        {
            var triangle = new Polygon()
            {
                Stroke = SystemColors.WindowFrameBrush,
                Fill = Brushes.Transparent,
                StrokeThickness = 2
            };
            triangle.Points.Add(Point);
            triangle.Points.Add(Point);
            triangle.Points.Add(Point);
            return triangle;
        }
        public Rectangle Square(double x, double y)
        {
            var square = new Rectangle()
            {
                Stroke = SystemColors.WindowFrameBrush,
                Fill = Brushes.Transparent,
                StrokeThickness = 2,
                Width = 0,
                Height = 0
            };
            square.SetValue(Canvas.LeftProperty,x);
            square.SetValue(Canvas.TopProperty,y);
            return square;
        }
        public Ellipse Circle(double x, double y)
        {
            Ellipse circle = new Ellipse()
            {
                Stroke = SystemColors.WindowFrameBrush,
                Fill = Brushes.Transparent,
                StrokeThickness = 2,
                Width = 0,
                Height = 0
            };
            circle.SetValue(Canvas.LeftProperty,x);
            circle.SetValue(Canvas.TopProperty,y);
            return circle;
        }
        public TextBox Text(double x,double y)
        {
            TextBox textBox = new TextBox
            {
                Foreground = SystemColors.WindowFrameBrush,
                FontSize = 14,
                Width = 100,
            };
            textBox.SetValue(Canvas.LeftProperty, x);
            textBox.SetValue(Canvas.TopProperty, y);
            return textBox;
        }

        public void BindSelected(HitTestResult hitResult,Point point)
        {
            if (hitResult != null && hitResult.VisualHit is UIElement)
            {
                this.selected = (UIElement)hitResult.VisualHit;
                this.Point = point;
            }
        }
        public void Rotate(Point mousePosition)
        {
            if (this.selected is Shape shape)
            {
                double centerX = 0;
                double centerY = 0;

                if (this.selected is Rectangle rectangle)
                {
                    centerX = rectangle.ActualWidth / 2;
                    centerY = rectangle.ActualHeight / 2;
                }
                else if (this.selected is Ellipse ellipse)
                {
                    centerX = ellipse.ActualWidth / 2;
                    centerY = ellipse.ActualHeight / 2;
                }
                else if (this.selected is Polygon triangle)
                {
                    double sumX = 0;
                    double sumY = 0;

                    for (int i = 0; i < triangle.Points.Count; i++)
                    {
                        sumX += triangle.Points[i].X;
                        sumY += triangle.Points[i].Y;
                    }

                    centerX = sumX / triangle.Points.Count;
                    centerY = sumY / triangle.Points.Count;
                }

                double angle = Math.Atan2(mousePosition.Y - centerY, mousePosition.X - centerX);

                RotateTransform rotateTransform = new RotateTransform(angle * (180 / Math.PI));
                rotateTransform.CenterX = centerX;
                rotateTransform.CenterY = centerY;

                shape.RenderTransform = rotateTransform;
            }
        }
        public void Reset()
        {
            this.isDrawing= false;
            this.currentLine = null;
            this.currentTriangle = null;
            this.currentSquare = null;
            this.currentCircle = null;

        }
    }
}
