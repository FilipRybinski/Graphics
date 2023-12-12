
using System.Windows;

namespace Grafika.Shared
{
    public class Translation : Transformation
    {
        public override void MouseMoveTo(Point point)
        {
            if (!isDragged) return;
            Vector startToPoint = point - mouseStart;
            var win = BezierB.Instance;
            var pol = win.SelectedPolygon;
            win.Cover();
            pol.RestoreVerticesFrom(startVertices);
            pol.Translate(startToPoint.X, startToPoint.Y);
            win.Draw();
        }

        public override void LeftMouseDown(Point point)
        {
            var win = BezierB.Instance;
            if (win.SelectedPolygonIndex == -1) return;
            if (isDragged) return;
            mouseStart = point;
            startVertices = win.SelectedPolygon.CloneVertices();
            isDragged = true;
        }
    }
}

