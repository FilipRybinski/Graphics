using Grafika.Commands;
using Grafika.Enums;
using Grafika.Handler;
using Grafika.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Action = Grafika.Enums.Action;

namespace Grafika
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDrawService _drawService;
        private readonly IFileSerivce _fileSerivce;
        private Canvas paintSufrace;
        private UIElement optionSurface;
        private MainWindow window;
        private MouseHandler mouseHandler;
        private Action action = Action.Default;
        public MainViewModel(Canvas canvas,MainWindow window, UIElement _optionSurface) {
            _drawService = new DrawService();
            _fileSerivce=new FileService(canvas);
            optionSurface = _optionSurface;
            ShiftingCommand = new RelayCommand(Shifting);
            RotateCommand= new RelayCommand(Rotate);
            FreeHandCommand=new RelayCommand(FreeHand);
            LineCommand=new RelayCommand(Line);
            CircleCommand=new RelayCommand(Circle);
            SquareCommand=new RelayCommand(Square);
            TriangleCommand=new RelayCommand(Triangle);
            TextCommand=new RelayCommand(Text);
            ClearCommand=new RelayCommand(Clear);
            SaveCommand = new RelayCommand(Save);

            mouseHandler = new MouseHandler();
            mouseHandler.AttachCanvas(canvas);
            mouseHandler.MouseDownEvent += MouseDownHandler;
            mouseHandler.MouseMoveEvent += MouseMoveHandler;
            mouseHandler.MouseUpEvent += MouseUpHandler;

            paintSufrace = window.paintSurface;
        }
        public ICommand ShiftingCommand { get; set; }
        public ICommand RotateCommand { get; set; }
        public ICommand FreeHandCommand { get; set; }
        public ICommand LineCommand { get; set; }
        public ICommand CircleCommand { get; set; }
        public ICommand TriangleCommand { get; set; }
        public ICommand SquareCommand { get; set; }
        public ICommand TextCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        private void Shifting(object sender)
        {
            action = Action.Shifting;
        }
        private void Rotate(object sender)
        {
            action=Action.Rotate;
        }
        private void FreeHand(object sender)
        {
            action = Action.Freehand;
        }
        private void Line(object sender)
        {
            action = Action.Line;
        }
        private void Circle(object sender)
        {
            action = Action.Circle;
        }
        private void Square(object sender)
        {
            action = Action.Square;
        }
        private void Triangle(object sender)
        {
            action = Action.Triangle;
        }
        private void Text(object sender)
        {
            action = Action.Text;
        }
        private void Save(object sender)
        {
            _fileSerivce.SaveFile();
        }
        private void Clear(object sender)
        {
            List<UIElement> elementsToRemove = new List<UIElement>();
            foreach (UIElement child in paintSufrace.Children)
            {
                if (child != optionSurface)
                {
                    elementsToRemove.Add(child);
                }
            }
            foreach (UIElement childToRemove in elementsToRemove)
            {
                paintSufrace.Children.Remove(childToRemove);
            }
        }
        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            _drawService.BindSelected(VisualTreeHelper.HitTest(paintSufrace, e.GetPosition(paintSufrace)), e.GetPosition(paintSufrace));
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (action)
                {
                    case Action.Default:
                        break;
                    case Action.Shifting:
                        _drawService.BindSelected(VisualTreeHelper.HitTest(paintSufrace, e.GetPosition(paintSufrace)), e.GetPosition(paintSufrace));
                        break;
                    case Action.Freehand:
                        _drawService.isDrawing = true;
                        _drawService.Point = e.GetPosition(window);
                        break;
                    case Action.Line:
                        _drawService.currentLine = _drawService.Line(
                            e.GetPosition(window).X,
                            e.GetPosition(window).Y,
                            e.GetPosition(window).X,
                            e.GetPosition(window).Y);
                        paintSufrace.Children.Add(_drawService.currentLine);
                        break;
                    case Action.Triangle:
                        _drawService.Point = e.GetPosition(window);
                        _drawService.currentTriangle = _drawService.Triangle();
                        paintSufrace.Children.Add(_drawService.currentTriangle);
                        break;
                    case Action.Square:
                        _drawService.currentSquare = _drawService.Square(
                                e.GetPosition(paintSufrace).X,
                                e.GetPosition(paintSufrace).Y);
                        paintSufrace.Children.Add(_drawService.currentSquare);
                        _drawService.Point= e.GetPosition(paintSufrace);
                        break;
                    case Action.Circle:
                        _drawService.currentCircle = _drawService.Circle(
                                e.GetPosition(paintSufrace).X,
                                e.GetPosition(paintSufrace).Y);
                         paintSufrace.Children.Add(_drawService.currentCircle);
                        _drawService.Point= e.GetPosition(paintSufrace);
                        break;
                    case Action.Text:
                        var text = _drawService.Text(
                            e.GetPosition(paintSufrace).X,
                            e.GetPosition(paintSufrace).Y);
                        paintSufrace.Children.Add(text);
                        text.Focus();
                        break;
                    default:
                        break;

                }
            }
        }
        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (action)
                {
                    case Action.Default:
                        break;
                    case Action.Shifting:
                        if (e.LeftButton == MouseButtonState.Pressed && _drawService.selected != null)
                        {
                            if(_drawService.selected is Polygon triangle)
                            {
                                for(int i=0; i< triangle.Points.Count; i++)
                                {
                                    triangle.Points[i] = new Point(triangle.Points[i].X + (e.GetPosition(paintSufrace).X - _drawService.Point.X), triangle.Points[i].Y + (e.GetPosition(paintSufrace).Y - _drawService.Point.Y));
                                }
                                _drawService.Point = e.GetPosition(paintSufrace);
                            }
                            else
                            {
                                Canvas.SetLeft(_drawService.selected, Canvas.GetLeft(_drawService.selected) + (e.GetPosition(paintSufrace).X - _drawService.Point.X));
                                Canvas.SetTop(_drawService.selected, Canvas.GetTop(_drawService.selected) + (e.GetPosition(paintSufrace).Y - _drawService.Point.Y));
                            }
                            _drawService.Point= e.GetPosition(paintSufrace);
                        }
                        break;
                    case Action.Rotate:
                        if (e.LeftButton == MouseButtonState.Pressed && _drawService.selected != null)
                        {
                            _drawService.Rotate(e.GetPosition(paintSufrace));
                        }
                        break;
                    case Action.Freehand:
                        if (_drawService.isDrawing)
                        {
                            var line=_drawService.Line(
                            _drawService.Point.X,
                            _drawService.Point.Y,
                            e.GetPosition(window).X,
                            e.GetPosition(window).Y
                            );
                            _drawService.Point = e.GetPosition(window);
                            paintSufrace.Children.Add(line);
                        }
                        break;
                    case Action.Line:
                        if (_drawService.currentLine == null)
                        {
                            _drawService.currentLine= _drawService.Line(
                            e.GetPosition(window).X,
                            e.GetPosition(window).Y,
                            e.GetPosition(window).X,
                            e.GetPosition(window).Y);
                            paintSufrace.Children.Add(_drawService.currentLine);
                        }
                        else
                        {
                            _drawService.currentLine.X2 = e.GetPosition(window).X;
                            _drawService.currentLine.Y2 = e.GetPosition(window).Y;
                        }
                        break;
                    case Action.Triangle:
                        if (_drawService.currentTriangle != null)
                        {
                            var position = e.GetPosition(paintSufrace);
                            _drawService.currentTriangle.Points[1]= new Point((position.X + _drawService.Point.X) / 2, position.Y);
                            _drawService.currentTriangle.Points[2] = position;
                        }
                        break;
                    case Action.Square:
                        if(_drawService.currentSquare != null)
                        {
                            _drawService.currentSquare.SetValue(Canvas.LeftProperty, Math.Min(_drawService.Point.X, e.GetPosition(paintSufrace).X));
                            _drawService.currentSquare.SetValue(Canvas.TopProperty, Math.Min(_drawService.Point.Y, e.GetPosition(paintSufrace).Y));
                            _drawService.currentSquare.Width = Math.Abs(e.GetPosition(paintSufrace).X - _drawService.Point.X);
                            _drawService.currentSquare.Height = Math.Abs(e.GetPosition(paintSufrace).Y - _drawService.Point.Y);
                        }
                        break;
                    case Action.Circle:
                        if(_drawService.currentCircle != null)
                        {
                            _drawService.currentCircle.SetValue(Canvas.LeftProperty, Math.Min(_drawService.Point.X, e.GetPosition(paintSufrace).X));
                            _drawService.currentCircle.SetValue(Canvas.TopProperty, Math.Min(_drawService.Point.Y, e.GetPosition(paintSufrace).Y));
                            _drawService.currentCircle.Width = Math.Abs(e.GetPosition(paintSufrace).X - _drawService.Point.X);
                            _drawService.currentCircle.Height = Math.Abs(e.GetPosition(paintSufrace).Y - _drawService.Point.Y);
                        }
                        break;
                    case Action.Text:
                        if(_drawService.currentTextBlock != null)
                        {
                            _drawService.currentTextBlock.SetValue(Canvas.LeftProperty, e.GetPosition(paintSufrace).X);
                            _drawService.currentTextBlock.SetValue(Canvas.TopProperty, e.GetPosition(paintSufrace).Y);
                        }
                        break;
                    default:
                        break;

                }
            }
        }
        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            _drawService.Reset();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
