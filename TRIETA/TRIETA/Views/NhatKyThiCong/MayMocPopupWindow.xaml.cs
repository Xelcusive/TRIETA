#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Trita.Views.NhatKyThiCong
{
    public partial class MayMocPopupWindow : Window
    {
        // Nguồn máy từ tab Máy thi công - truyền vào dạng danh sách tên máy + đơn vị
        private List<string> _nguonMay;
        public List<MayMocSuDung> KetQua { get; private set; } = new List<MayMocSuDung>();
        private List<MayMocSuDung> _danhSach = new List<MayMocSuDung>();

        public MayMocPopupWindow(List<string> nguonMay, List<MayMocSuDung> duLieuCu = null)
        {
            InitializeComponent();
            _nguonMay = nguonMay ?? new List<string>();

            if (duLieuCu != null && duLieuCu.Count > 0)
            {
                _danhSach = duLieuCu.Select(d => new MayMocSuDung
                {
                    TenMay = d.TenMay,
                    SoLuong = d.SoLuong,
                    CaMay = d.CaMay
                }).ToList();
            }

            RenderTable();
        }

        private void RenderTable()
        {
            DanhSachMayMocPanel.Children.Clear();
            for (int i = 0; i < _danhSach.Count; i++)
                DanhSachMayMocPanel.Children.Add(TaoDong(i, _danhSach[i]));
            CapNhatTong();
        }

        private Border TaoDong(int idx, MayMocSuDung item)
        {
            Border row = new Border
            {
                Background = idx % 2 == 0
                    ? new SolidColorBrush(Color.FromRgb(255, 255, 255))
                    : new SolidColorBrush(Color.FromRgb(253, 245, 238)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(235, 210, 190)),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(0, 4, 0, 4)
            };

            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });

            TextBlock stt = new TextBlock { Text = (idx + 1).ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 11 };
            Grid.SetColumn(stt, 0); g.Children.Add(stt);

            // Tên máy - ComboBox từ nguồn
            ComboBox cbTen = new ComboBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 11,
                Padding = new Thickness(4, 0, 4, 0),
                VerticalContentAlignment = VerticalAlignment.Center
            };
            cbTen.Items.Add("-- Chọn máy --");
            foreach (var m in _nguonMay) cbTen.Items.Add(m);

            if (!string.IsNullOrEmpty(item.TenMay))
            {
                int found = cbTen.Items.IndexOf(item.TenMay);
                cbTen.SelectedIndex = found >= 0 ? found : 0;
            }
            else cbTen.SelectedIndex = 0;

            cbTen.SelectionChanged += (s, e) =>
            {
                string sel = cbTen.SelectedItem?.ToString() ?? "";
                item.TenMay = sel == "-- Chọn máy --" ? "" : sel;
            };
            Grid.SetColumn(cbTen, 1); g.Children.Add(cbTen);

            // Số lượng
            TextBox txtSL = new TextBox
            {
                Text = item.SoLuong > 0 ? item.SoLuong.ToString() : "1",
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 170, 140)),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 11,
                Padding = new Thickness(4, 0, 4, 0)
            };
            txtSL.TextChanged += (s, e) =>
            {
                item.SoLuong = int.TryParse(txtSL.Text, out int sl) ? sl : 1;
                CapNhatTong();
            };
            Grid.SetColumn(txtSL, 2); g.Children.Add(txtSL);

            // Ca máy
            TextBox txtCa = new TextBox
            {
                Text = item.CaMay > 0 ? item.CaMay.ToString() : "1",
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 170, 140)),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 11,
                Padding = new Thickness(4, 0, 4, 0)
            };
            txtCa.TextChanged += (s, e) =>
            {
                item.CaMay = double.TryParse(txtCa.Text, out double ca) ? ca : 1;
            };
            Grid.SetColumn(txtCa, 3); g.Children.Add(txtCa);

            Button btnXoa = new Button { Content = "×", Width = 22, Height = 22, Background = Brushes.Transparent, BorderThickness = new Thickness(0), Foreground = Brushes.Red, FontWeight = FontWeights.Bold, FontSize = 14, Cursor = System.Windows.Input.Cursors.Hand, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            btnXoa.Click += (s, e) => { _danhSach.Remove(item); RenderTable(); };
            Grid.SetColumn(btnXoa, 4); g.Children.Add(btnXoa);

            row.Child = g;
            return row;
        }

        private void CapNhatTong()
        {
            int tong = _danhSach.Sum(d => d.SoLuong);
            TongSoLuongTextBlock.Text = tong.ToString();
        }

        private void ThemDong_Click(object sender, RoutedEventArgs e)
        {
            _danhSach.Add(new MayMocSuDung { SoLuong = 1, CaMay = 1 });
            RenderTable();
        }

        private void XacNhan_Click(object sender, RoutedEventArgs e)
        {
            KetQua = _danhSach.Where(d => !string.IsNullOrEmpty(d.TenMay)).ToList();
            DialogResult = true;
        }

        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}