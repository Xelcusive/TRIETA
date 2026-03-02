#nullable disable
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Trita.Views.NhatKyThiCong
{
    public partial class LyTrinhPickerWindow : Window
    {
        public string LyTrinhDaChon { get; private set; }

        public LyTrinhPickerWindow(string hangMuc, string congViec, List<string> danhSachLyTrinh)
        {
            InitializeComponent();

            TxtCongViecInfo.Text = $"Hạng mục: {hangMuc}\nCông việc: {congViec}";

            foreach (var lt in danhSachLyTrinh)
                DanhSachPanel.Children.Add(TaoDongLyTrinh(lt));
        }

        private Border TaoDongLyTrinh(string lyTrinh)
        {
            Border row = new Border
            {
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 10, 14, 10),
                Margin = new Thickness(0, 3, 0, 3),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 235, 220)),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand
            };

            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = System.Windows.GridLength.Auto });

            TextBlock tb = new TextBlock
            {
                Text = lyTrinh,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(tb, 0);
            g.Children.Add(tb);

            TextBlock icon = new TextBlock
            {
                Text = "›",
                FontSize = 18,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 160, 100)),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 0, 0)
            };
            Grid.SetColumn(icon, 1);
            g.Children.Add(icon);

            row.Child = g;

            // Hover effect
            row.MouseEnter += (s, e) =>
            {
                row.Background = new SolidColorBrush(Color.FromRgb(238, 247, 238));
                row.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 180, 100));
            };
            row.MouseLeave += (s, e) =>
            {
                row.Background = Brushes.White;
                row.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 235, 220));
            };

            row.MouseLeftButtonDown += (s, e) =>
            {
                LyTrinhDaChon = lyTrinh;
                DialogResult = true;
            };

            return row;
        }

        private void Huy_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}