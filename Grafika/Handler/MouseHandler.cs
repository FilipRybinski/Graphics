using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Grafika.Handler
{
    public interface IMouseHandler
    {
        public void AttachCanvas(Canvas canvas);
        public MouseHandler getInstance();
    }
    public class MouseHandler : IMouseHandler
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

        public MouseHandler getInstance()
        {
            return this;
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
