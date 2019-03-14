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

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint;
        SolidColorBrush brush;
        int lineThickness;
        Polyline polyline;
        bool onCanvas;


        public MainWindow()
        {
            InitializeComponent();
            currentPoint = new Point();
            brush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            lineThickness = 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Color> colors = new List<Color>
            {
                new Color { R = 255, G = 0, B = 0, A = 255 },
                new Color { R = 0, G = 255, B = 0, A = 255 },
                new Color { R = 0, G = 0, B = 255, A = 255 },
                new Color { R = 0, G = 0, B = 0, A = 255}
            };

            

            cmbColors.DataContext = colors;

            List<int> thickness = new List<int> { 1, 2, 4, 8, 10, 12, 16, 20, 25, 30, 35 };

            cmbThickness.DataContext = thickness;
        }

        private void CnvPaintSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && onCanvas)
            {
                currentPoint = e.GetPosition(this);

                polyline = new Polyline();
                cnvPaintSurface.Children.Add(polyline);


            }
        }

        private void CnvPaintSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && onCanvas)
            {
                polyline.Stroke = brush;
                polyline.StrokeThickness = lineThickness;

                Point point1 = new Point(currentPoint.X - 30, currentPoint.Y - 50);
                Point point2 = e.GetPosition(this);
                point2.X -= 30;
                point2.Y -= 50;

                polyline.Points.Add(point1);
                polyline.Points.Add(point2);

                currentPoint = e.GetPosition(this);
                //cnvPaintSurface.Children.Add(polyline);


                //////////////////////////////////////////////////
                //////////////////////////////////////////////////
                //////////////////////////////////////////////////


                //PathFigure path = new PathFigure
                //{
                //    StartPoint = new Point(currentPoint.X - 30, currentPoint.Y - 50)
                //};

                //Point point = e.GetPosition(this);
                //point.X -= 30;
                //point.Y -= 50;

                //LineSegment segment = new LineSegment(point, true);
                //path.Segments.Add(segment);


                //PathGeometry geometry = new PathGeometry();


                //currentPoint = e.GetPosition(this);
            }
        }

        private void CmbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            brush = new SolidColorBrush
            {
                Color = (Color)cmbColors.SelectedValue
            };

            brdBorder.Cursor = Cursors.Pen;
        }

        private void CmbThickness_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lineThickness = (int)cmbThickness.SelectedItem;
        }

        private void BtnEraser_Click(object sender, RoutedEventArgs e)
        {
            brush = (SolidColorBrush)cnvPaintSurface.Background.Clone();
            brdBorder.Cursor = Cursors.Cross;
        }

        private void BtnZoomUp_Click(object sender, RoutedEventArgs e)
        {
            foreach (Polyline polyline in cnvPaintSurface.Children)
            {
                for (int i = 0; i < polyline.Points.Count; i++)
                {
                    Point p = polyline.Points[i];
                    p.X *= 1.5;
                    p.Y *= 1.5;
                    polyline.Points[i] = p;
                }
            }
        }

        private void BtnZoomDown_Click(object sender, RoutedEventArgs e)
        {
            foreach (Polyline polyline in cnvPaintSurface.Children)
            {
                for (int i = 0; i < polyline.Points.Count; i++)
                {
                    Point p = polyline.Points[i];
                    p.X /= 1.5;
                    p.Y /= 1.5;
                    polyline.Points[i] = p;
                }
            }
        }

        private void CnvPaintSurface_MouseEnter(object sender, MouseEventArgs e)
        {
            onCanvas = true;
        }

        private void CnvPaintSurface_MouseLeave(object sender, MouseEventArgs e)
        {
            onCanvas = false;
        }
    }
}
