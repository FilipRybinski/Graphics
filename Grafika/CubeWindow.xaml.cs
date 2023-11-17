using Grafika.Statics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace Grafika
{
        public partial class CubeWindow : Window
        {
            public CubeWindow()
            {
                InitializeComponent();
                CreateCubeWalls();

            }

            private void CreateCubeWalls()
            {
                Point3DCollection positions = new Point3DCollection
                {
                    new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 0, 1), new Point3D(1, 0, 1),
                    new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(1, 1, 0),
                    new Point3D(1, 0, 0), new Point3D(1, 1, 0), new Point3D(1, 0, 1), new Point3D(1, 1, 1),
                    new Point3D(0, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1), new Point3D(0, 1, 1),
                    new Point3D(0, 0, 1), new Point3D(1, 0, 1), new Point3D(0, 1, 1), new Point3D(1, 1, 1),
                    new Point3D(0, 1, 0), new Point3D(1, 1, 0), new Point3D(0, 1, 1), new Point3D(1, 1, 1),
                };
                PointCollection textureCoordinates = new PointCollection
                {
                    new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1),
                    new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1),
                    new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1),
                    new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1),
                    new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1),
                    new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1)
                };
                Model3DGroup groupVisual = new Model3DGroup();
                for (int i = 0; i < 6; i++)
                {
                    Bitmap bitmap = Algorithm.Gradient();
                    BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                    ImageBrush imageBrush = new ImageBrush(bitmapSource);
                    MeshGeometry3D mesh = new MeshGeometry3D
                    {
                        Positions = new Point3DCollection(new Point3D[]
                        {
                        positions[i * 4], positions[i * 4 + 1], positions[i * 4 + 2], positions[i * 4 + 3]
                        }),
                        TriangleIndices = new Int32Collection(new int[]
                        {
                        0, 1, 2, 2, 1, 3
                        }),
                        TextureCoordinates = textureCoordinates,
            };
                    DiffuseMaterial material = new DiffuseMaterial(imageBrush);
                    GeometryModel3D geometryModel = new GeometryModel3D
                    {
                        Geometry = mesh,
                        BackMaterial = material,
                        Material = material
                    };
                    container.Children.Add(geometryModel);
                }
            }
        }
}
