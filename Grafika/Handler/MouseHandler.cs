using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Grafika.Handler
{
    public class MouseHandler
    {
        public event MouseButtonEventHandler MouseDownEvent;
        public event MouseEventHandler MouseMoveEvent;
        public event MouseButtonEventHandler MouseUpEvent;
        public void AttachCanvas(Canvas canvas)
        {
            canvas.MouseDown += MouseDown;
            canvas.MouseMove += MouseMove;
            canvas.MouseUp += MouseUp;
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseDownEvent?.Invoke(sender, e);
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveEvent?.Invoke(sender, e);
        }
        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseUpEvent?.Invoke(sender, e);
        }
    }
}
