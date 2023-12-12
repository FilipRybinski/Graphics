using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Grafika.Shared;
using Grafika.Shared.Geometric;
using System.Collections.Generic;

namespace Grafika
{
    public partial class BezierB : Window, INotifyPropertyChanged
    {
        public static BezierB Instance { get; private set; }
        private ObservableCollection<Polygon> _polygons;
        public ObservableCollection<Polygon> Polygons
        {
            get => _polygons;
            private set { _polygons = value; OnPropertyChanged(); }
        }
        private int _selectedPolygonIndex = -1;
        public int SelectedPolygonIndex
        {
            get => _selectedPolygonIndex;
            set
            {
                Cover();
                _selectedPolygonIndex = value;
                Draw();
                OnPropertyChanged();
            }
        }
        public Polygon SelectedPolygon => Polygons[SelectedPolygonIndex];
        private readonly Tool[] _tools;
        private Tool _selectedTool;
        public Vertex TransformationPoint { get; }
        public string TransformationPointXString
        {
            get
            {
                if (TransformationPoint == null) return string.Empty;
                return TransformationPoint.X.ToString();
            }
            set
            {
                if (!double.TryParse(value, out double val))
                {
                    MessageBox.Show("Enter X.");
                    return;
                }
                TransformationPoint.X = val;
                OnPropertyChanged();
            }
        }
        public string TransformationPointYString
        {
            get
            {
                if (TransformationPoint == null) return string.Empty;
                return TransformationPoint.Y.ToString();
            }
            set
            {
                if (!double.TryParse(value, out double val))
                {
                    MessageBox.Show("Enter Y.");
                    return;
                }
                TransformationPoint.Y = val;
                OnPropertyChanged();
            }
        }

        public BezierB()
        {
            InitializeComponent();
            Instance = this;
            Image.Source = new WriteableBitmap(670, 400, 96, 96, PixelFormats.Bgra32, null);
            Polygons = new ObservableCollection<Polygon>();
            _tools = new Tool[]
            {
                new Shared.Cursor(),
                new Translation(),
                new Shared.Rotation(),
                new Scaling()
            };
            _selectedTool = _tools[0];
            CursorRadioButton.IsChecked = true;
            PointStackPanel.Visibility = Visibility.Collapsed;
            TranslationStackPanel.Visibility = Visibility.Collapsed;
            RotationStackPanel.Visibility = Visibility.Collapsed;
            ScalingStackPanel.Visibility = Visibility.Collapsed;
            TransformationPoint = new Vertex(-2 * Vertex.POINT_WIDTH, -2 * Vertex.POINT_HEIGHT);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void AddPolygon_Click(object sender, RoutedEventArgs e)
        {
            Polygons.Add(new Polygon());
        }

        private void DeleteSelectedPolygon_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPolygonIndex == -1) return;
            Cover();
            Polygons.RemoveAt(SelectedPolygonIndex);
            Draw();
        }

        private void AddVertex_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPolygonIndex == -1) return;
            if (!double.TryParse(XTextBox.Text, out double x))
            {
                MessageBox.Show("Enter X.");
                return;
            }
            if (!double.TryParse(YTextBox.Text, out double y))
            {
                MessageBox.Show("Enter Y.");
                return;
            }

            Cover();
            var p = Polygons[SelectedPolygonIndex];
            p.Vertices.Add(new Vertex(x, y));
            Draw();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectedTool.LeftMouseDown(e.GetPosition(Image));
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            _selectedTool.MouseMoveTo(e.GetPosition(Image));
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _selectedTool.LeftMouseUp(e.GetPosition(Image));
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectedTool.RightMouseDown(e.GetPosition(Image));
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle) return;
            if (CanDrawTransformationPoint())
            {
                var mp = e.GetPosition(Image);
                TransformationPoint.SetXY(mp.X, mp.Y);
                UpdateTransformationPointTextBoxes();
            }
        }

        public void UpdateTransformationPointTextBoxes()
        {
            OnPropertyChanged(nameof(TransformationPointXString));
            OnPropertyChanged(nameof(TransformationPointYString));
        }

        public void Cover()
        {
            if (Image == null) return;
            var bmp = (WriteableBitmap)Image.Source;
            bmp.Lock();
            const int white = (255 << 24) | (255 << 16) | (255 << 8) | 255;
            foreach (var p in Polygons)
            {
                p.DrawOn(bmp, white);
            }
            if (TransformationPoint != null)
            {
                TransformationPoint.DrawRectangle(bmp, white);
            }
        }

        public void Draw()
        {
            if (Image == null) return;
            var bmp = (WriteableBitmap)Image.Source;
            for (int i = 0; i < Polygons.Count; ++i)
            {
                var pol = Polygons[i];
                if (i == SelectedPolygonIndex)
                {
                    pol.DrawOn(bmp, (255 << 24) | (0 << 16) | (255 << 8) | 0);
                }
                else
                {
                    pol.DrawOn(bmp, (255 << 24) | (0 << 16) | (0 << 8) | 0);
                }

            }
            if (CanDrawTransformationPoint())
            {
                TransformationPoint.DrawRectangle(bmp, (255 << 24) | (255 << 16) | (0 << 8) | 0);
            }
            bmp.Unlock();
        }

        private bool CanDrawTransformationPoint()
        {
            return TransformationPoint != null && (_selectedTool == _tools[2] || _selectedTool == _tools[3]);
        }

        private class JsonPolygon
        {
            public Point[] Vertices;

            public JsonPolygon(Point[] vertices)
            {
                Vertices = vertices;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Save";
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.Filter = "JSON (*.json)|*.json";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() != true) return;
            var jsonStruct = new JsonPolygon[Polygons.Count];
            for (int p = 0; p < jsonStruct.Length; ++p)
            {
                var verts = Polygons[p].Vertices;
                var points = new Point[verts.Count];
                for (int v = 0; v < verts.Count; ++v)
                    points[v] = new Point(verts[v].X, verts[v].Y);
                jsonStruct[p] = new JsonPolygon(points);
            }
            var json = JsonConvert.SerializeObject(jsonStruct);
            File.WriteAllText(dialog.FileName, json, Encoding.UTF8);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Load";
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.Filter = "JSON (*.json)|*.json";
            if (dialog.ShowDialog() != true) return;
            var name = dialog.FileName;
            if (!File.Exists(name)) MessageBox.Show($"File {name} does not exist.");
            var json = File.ReadAllText(name, Encoding.UTF8);
            var list = JsonConvert.DeserializeObject<LinkedList<JsonPolygon>>(json);

            Cover();
            Polygons.Clear();
            foreach (var readPol in list)
            {
                var pol = new Polygon();
                var polVerts = pol.Vertices;
                var readVerts = readPol.Vertices;
                foreach (var rv in readVerts)
                {
                    polVerts.Add(new Vertex(rv.X, rv.Y));
                }
                Polygons.Add(pol);
            }
            Draw();
        }

        private void Tools_Checked(object sender, RoutedEventArgs e)
        {
            var rbs = new RadioButton[] { CursorRadioButton, TranslationRadioButton,
                RotationRadioButton, ScalingRadioButton };
            var sps = new StackPanel[] { PointStackPanel, TranslationStackPanel,
                RotationStackPanel, ScalingStackPanel };
            for (int i = 0; i < sps.Length; ++i)
                sps[i].Visibility = Visibility.Collapsed;
            PerformButton.Visibility = Visibility.Collapsed;
            Cover();
            if (sender == rbs[0])
            {
                _selectedTool = _tools[0];
                goto cleanup;
            }
            PerformButton.Visibility = Visibility.Visible;
            if (sender == rbs[1])
            {
                _selectedTool = _tools[1];
                TranslationStackPanel.Visibility = Visibility.Visible;
                goto cleanup;
            }
            PointStackPanel.Visibility = Visibility.Visible;
            for (int i = 2; i < rbs.Length; ++i)
                if (sender == rbs[i])
                {
                    _selectedTool = _tools[i];
                    sps[i].Visibility = Visibility.Visible;
                    goto cleanup;
                }
            cleanup:
            Draw();
        }

        private void Perform_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPolygonIndex == -1)
            {
                return;
            }
            if (_selectedTool == _tools[1])
            {
                PerformTranslation();
            }
            else if (_selectedTool == _tools[2])
            {
                PerformRotation();
            }
            else if (_selectedTool == _tools[3])
            {
                PerformScaling();
            }
        }

        private void PerformTranslation()
        {
            if (!double.TryParse(TranslationVectorXTextBox.Text, out double x))
            {
                MessageBox.Show("Enter correct X coordinate of the vector.");
                return;
            }
            if (!double.TryParse(TranslationVectorYTextBox.Text, out double y))
            {
                MessageBox.Show("Enter correct Y coordinate of the vector.");
                return;
            }
            Cover();
            Polygons[SelectedPolygonIndex].Translate(x, y);
            Draw();
        }

        private void PerformRotation()
        {
            if (!double.TryParse(RotationAngleTextBox.Text, out double ang))
            {
                MessageBox.Show("Enter correct angle.");
                return;
            }
            Cover();
            Polygons[SelectedPolygonIndex].Rotate(TransformationPoint.X, TransformationPoint.Y, (ang / 180.0) * Math.PI);
            Draw();
        }

        private void PerformScaling()
        {
            if (!double.TryParse(ScalingCoefficientXTextBox.Text, out double x))
            {
                MessageBox.Show("Enter correct X coefficient.");
                return;
            }
            if (!double.TryParse(ScalingCoefficientYTextBox.Text, out double y))
            {
                MessageBox.Show("Enter correct Y coefficient.");
                return;
            }
            Cover();
            Polygons[SelectedPolygonIndex].Scale(TransformationPoint.X, TransformationPoint.Y, x, y);
            Draw();
        }
    }
}
