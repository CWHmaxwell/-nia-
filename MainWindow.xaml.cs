using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace gamework
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CanvasHelper cv;
        private Game game;
        Thread loop;
        private delegate void UpdateDelegate();
        // 每帧允许绘制的时间
        private int mpf = 1000 / 40;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            canvas.Focus();
            cv = new CanvasHelper(canvas);
            game = new Game( cv);
            game.initialze();
            loop = new Thread(new ThreadStart( Loop));
            loop.Start();
        }

        public void Loop()
        {
            while (true)
            {
                Dispatcher.Invoke(new UpdateDelegate(game.OnUpdate));
                Thread.Sleep(mpf - DateTime.Now.Millisecond % mpf);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loop.Abort();
        }
    }
}
