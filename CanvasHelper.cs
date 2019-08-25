using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;


namespace gamework
{
    //画布的各个组件
    class CanvasHelper
    {
        public Canvas cv;
        public double  width;
        public double  height;
        public class PointEventArg : EventArgs
        {
            public Point point;
        }
        public event EventHandler<PointEventArg> MouseLeftButtonUp;
        public event KeyEventHandler KeyDown; 
        public event KeyEventHandler KeyUp;
        
        public CanvasHelper(Canvas cv_)
        {
            cv = cv_;
            if (Double.IsNaN(cv.Height) && cv.ActualHeight == 0)
            {
                throw new Exception("Error spawning canvashelper ,canvas should be rendered first!");
            }
            width = cv.ActualWidth;
            height = cv.ActualHeight;
            cv.PreviewMouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            cv.PreviewKeyDown += Canvas_KeyDown;
            cv.PreviewKeyUp += Canvas_KeyUp;           
        }
        //鼠标键盘事件处理;
        #region
       private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDown(sender, e);
        }
        private void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUp(sender, e);
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(cv);
            MouseLeftButtonUp(sender, new PointEventArg()
            {
                point = new Point(w(p.X, true), h(p.Y, true))
            });
        }
        #endregion
        public CanvasHelper SetRangle(double width_, double height_)
        {
            width = width_;
            height = height_;
            return this;
        }

        //画线;
        public CanvasHelper line(double x1,double y1,double x2,double y2 ,string color = "#fff",int thickness =1  )
        {
            Line line_ = new Line();
            line_.X1 = w(x1);
            line_.Y1 = h(y1);
            line_.X2 = w(x2);
            line_.Y2 = h(y2);
            line_.Stroke = Helper.Color(color);
            cv.Children.Add(line_);
            return this;
        }

        private CanvasHelper Shape<T>(double x1,double y1,double width_,double height_,double  thickness = 1,Brush color = null,Brush fill =null,double Opacity = -1)where T : Shape,new(){
            Shape shape = new T()
            {
                Width = w(width_),
                Height = h(height_),
                Stroke = color == null ? Helper.Color("#fff"):color,
                StrokeThickness = thickness
            };
            if (Opacity >= 0)
            {
                shape.Opacity = Opacity;
            }
            if (fill != null)
                shape.Fill = fill;
            shape.SetValue(Canvas.LeftProperty,w(x1));
            shape.SetValue(Canvas.TopProperty, h(y1));
            cv.Children.Add(shape);
            return this;
        }
        //画矩形
        public CanvasHelper Rectangle(double x1, double y1, double width_, double height_, double thickness = 1, Brush color = null, Brush fill = null)
        {
            return Shape<Rectangle>(x1, y1, width_, height_,thickness,color,fill);
        }

        public CanvasHelper Ellipse(double x1,double y1,double width_,double height_,double thickness =1,Brush color = null,Brush fill = null)
        {
            return Shape<Ellipse>(x1, y1, width_, height_, thickness, color, fill);
        }
        public CanvasHelper Image(double x1,double y1,double width_, double height_,Brush img,double op=-1)
        {
            return Shape<Rectangle>(x1, y1, width_, height_, 0, null, img,op);
        }
        //写字
        public CanvasHelper Text(double x1, double y1, double fontsize ,string t,Brush color = null,double width_=-1)
        {
            
            TextBlock text = new TextBlock();
            text.Text = t; 
            if (width_ > 0)
            {
                text.Width = w(width_);
            }
            text.FontSize = fontsize;
            text.Foreground = color != null ? color : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff"));
            text.TextAlignment = TextAlignment.Center;
            text.SetValue(Canvas.LeftProperty, w(x1));
            text.SetValue(Canvas.TopProperty, h(y1));
            cv.Children.Add(text);
            return this;
        }
        public double w(double x_,bool re = false)
        {
            if (re)
            {
                return x_ / cv.ActualWidth * width;
            }
            return x_ / width * cv.ActualWidth;
        }
        public double h(double y_, bool re = false)
        {
            if (re)
            {
                return y_ / cv.ActualHeight * height;
            }
            return y_ / height * cv.ActualHeight;
        }
    }
}
