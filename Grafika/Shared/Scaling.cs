
using System.Windows;

namespace Grafika.Shared
{
    public class Scaling : Transformation
    {
        public override void MouseMoveTo(Point point)
        {
            if (!isDragged) return;
            var win = BezierB.Instance;
            var center = win.TransformationPoint;
            Vector centerToStart = center.Subtract(mouseStart);
            Vector centerToPoint = center.Subtract(point);
            var pol = win.SelectedPolygon;
            win.Cover();
            pol.RestoreVerticesFrom(startVertices);
            pol.Scale(center.X, center.Y,
                centerToPoint.X / centerToStart.X, centerToPoint.Y / centerToStart.Y);
            win.Draw();
        }
    }
}
