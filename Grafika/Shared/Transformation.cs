
using System.Windows;

namespace Grafika.Shared
{
    public abstract class Transformation : Tool
    {
        protected Point[] startVertices;
        protected Point mouseStart;

        public override void LeftMouseDown(Point point)
        {
            var win = BezierB.Instance;
            if (win.SelectedPolygonIndex == -1) return;
            mouseStart = point;
            startVertices = win.SelectedPolygon.CloneVertices();
            isDragged = true;
        }
    }
}
