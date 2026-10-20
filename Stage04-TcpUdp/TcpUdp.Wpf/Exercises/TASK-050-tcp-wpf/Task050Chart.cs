using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TcpUdp.Wpf.Exercises.Task050
{
    // 教练复用既有折线思路提供的显示辅助，不作为新的绘图算法练习。
    public static class Task050Chart
    {
        public static void Draw(Canvas canvas, IList<Task050Reading> history, double threshold)
        {
            canvas.Children.Clear();
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            if (width < 100 || height < 80)
            {
                return;
            }
            if (history.Count == 0)
            {
                TextBlock empty = new TextBlock();
                empty.Text = "收到有效温度后显示曲线。";
                empty.Foreground = Brushes.SlateGray;
                Canvas.SetLeft(empty, 14);
                Canvas.SetTop(empty, 16);
                canvas.Children.Add(empty);
                return;
            }
            int start = Math.Max(0, history.Count - 30);
            double minimum = threshold;
            double maximum = threshold;
            for (int i = start; i < history.Count; i++)
            {
                minimum = Math.Min(minimum, history[i].Temperature);
                maximum = Math.Max(maximum, history[i].Temperature);
            }
            minimum -= 1;
            maximum += 1;
            double left = 52;
            double top = 18;
            double plotWidth = width - left - 18;
            double plotHeight = height - top - 28;
            for (int i = 0; i < 3; i++)
            {
                double value = minimum + (maximum - minimum) * i / 2;
                double y = top + plotHeight - plotHeight * i / 2;
                AddLine(canvas, left, y, left + plotWidth, y, Brushes.LightGray);
                TextBlock label = new TextBlock();
                label.Text = value.ToString("F1", CultureInfo.InvariantCulture);
                label.FontSize = 11;
                Canvas.SetLeft(label, 2);
                Canvas.SetTop(label, y - 8);
                canvas.Children.Add(label);
            }
            double alarmY = top + plotHeight - (threshold - minimum) * plotHeight / (maximum - minimum);
            AddLine(canvas, left, alarmY, left + plotWidth, alarmY, Brushes.IndianRed);
            Polyline line = new Polyline();
            line.Stroke = Brushes.SteelBlue;
            line.StrokeThickness = 2;
            for (int i = start; i < history.Count; i++)
            {
                double x = left + (i - start) * plotWidth / Math.Max(1, history.Count - start - 1);
                double y = top + plotHeight - (history[i].Temperature - minimum) * plotHeight / (maximum - minimum);
                line.Points.Add(new Point(x, y));
            }
            canvas.Children.Add(line);
            if (line.Points.Count == 1)
            {
                Ellipse point = new Ellipse();
                point.Width = 8;
                point.Height = 8;
                point.Fill = Brushes.SteelBlue;
                Canvas.SetLeft(point, line.Points[0].X - 4);
                Canvas.SetTop(point, line.Points[0].Y - 4);
                canvas.Children.Add(point);
            }
        }

        private static void AddLine(Canvas canvas, double x1, double y1, double x2, double y2, Brush brush)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.Stroke = brush;
            canvas.Children.Add(line);
        }
    }
}
