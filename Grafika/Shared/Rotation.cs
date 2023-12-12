using System;
using System.Windows;

namespace Grafika.Shared
{
    public class Rotation : Transformation
    {
        public override void MouseMoveTo(Point point)
        {
            if (!isDragged) return;
            var win = BezierB.Instance;
            var center = win.TransformationPoint;
            Vector centerToStart = center.Subtract(mouseStart);
            Vector centerToPoint = center.Subtract(point);
            double angCenPt = Math.Atan2(centerToPoint.Y, centerToPoint.X);
            double angCenSt = Math.Atan2(centerToStart.Y, centerToStart.X);
            var pol = win.SelectedPolygon;
            win.Cover();
            pol.RestoreVerticesFrom(startVertices);
            pol.Rotate(center.X, center.Y, angCenPt - angCenSt);
            win.Draw();
        }
    }
}
