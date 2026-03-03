#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Trita.Views.VatTu;

namespace Trita.Views.NhatKyThiCong
{
    public partial class VatLieuPopupWindow : Window
    {
        /// <summary>Ngày được set từ ngoài (NgayPicker của NhatKyThiCongView)</summary>
        public DateTime NgayNhatKy { get; set; } = DateTime.Today;

        private List<VatTuItem> _nguonVatTu;
        public List<VatLieuSuDung> KetQua { get; private set; } = new List<VatLieuSuDung>();

        // Dữ liệu làm việc nội bộ
        private List<VatLieuSuDung> _danhSach = new List<VatLieuSuDung>();

        public VatLieuPopupWindow(List<VatTuItem> nguonVatTu, List<VatLieuSuDung> duLieuCu = null)
        {
            InitializeComponent();
            _nguonVatTu = nguonVatTu ?? new List<VatTuItem>();

            if (duLieuCu != null && duLieuCu.Count > 0)
            {
                _danhSach = duLieuCu.Select(d => new VatLieuSuDung
                {
                    TenVatLieu = d.TenVatLieu,
                    DonVi = d.DonVi,
                    KhoiLuong = d.KhoiLuong
                }).ToList();
            }

            RenderTable();
        }

        private void RenderTable()
        {
            DanhSachVatLieuPanel.Children.Clear();

            for (int i = 0; i < _danhSach.Count; i++)
            {
                DanhSachVatLieuPanel.Children.Add(TaoDong(i, _danhSach[i]));
            }

            CapNhatTong();
        }

        private Border TaoDong(int idx, VatLieuSuDung item)
        {
            Border row = new Border
            {
                Background = idx % 2 == 0
                    ? new SolidColorBrush(Color.FromRgb(255, 255, 255))
                    : new SolidColorBrush(Color.FromRgb(245, 250, 245)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 235, 220)),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(0, 4, 0, 4)
            };

            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });

            // STT
            TextBlock stt = new TextBlock
            {
                Text = (idx + 1).ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 11
            };
            Grid.SetColumn(stt, 0);
            g.Children.Add(stt);

            // Tên vật liệu - ComboBox từ nguồn VatTu
            ComboBox cbTen = new ComboBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 11,
                Padding = new Thickness(4, 0, 4, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                IsEditable = false
            };

            // Thêm placeholder
            cbTen.Items.Add("-- Chọn vật liệu --");
            foreach (var vt in _nguonVatTu)
                cbTen.Items.Add(vt.Ten);

            // Chọn item hiện tại
            if (!string.IsNullOrEmpty(item.TenVatLieu))
            {
                int found = cbTen.Items.IndexOf(item.TenVatLieu);
                cbTen.SelectedIndex = found >= 0 ? found : 0;
            }
            else
            {
                cbTen.SelectedIndex = 0;
            }

            cbTen.SelectionChanged += (s, e) =>
            {
                string selected = cbTen.SelectedItem?.ToString() ?? "";
                if (selected == "-- Chọn vật liệu --") { item.TenVatLieu = ""; item.DonVi = ""; }
                else
                {
                    item.TenVatLieu = selected;
                    var vt = _nguonVatTu.FirstOrDefault(v => v.Ten == selected);
                    item.DonVi = vt?.DonVi ?? "";
                }
                // Cập nhật hiển thị đơn vị
                RenderTable();
            };
            Grid.SetColumn(cbTen, 1);
            g.Children.Add(cbTen);

            // Đơn vị (auto)
            TextBlock txtDV = new TextBlock
            {
                Text = item.DonVi,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.DimGray,
                FontSize = 11
            };
            Grid.SetColumn(txtDV, 2);
            g.Children.Add(txtDV);

            // Khối lượng
            TextBox txtKL = new TextBox
            {
                Text = item.KhoiLuong > 0 ? item.KhoiLuong.ToString() : "",
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(180, 210, 180)),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 11,
                Padding = new Thickness(4, 0, 4, 0)
            };
            txtKL.TextChanged += (s, e) =>
            {
                item.KhoiLuong = double.TryParse(txtKL.Text, out double kl) ? kl : 0;
                CapNhatTong();
            };
            Grid.SetColumn(txtKL, 3);
            g.Children.Add(txtKL);

            // Xóa dòng
            Button btnXoa = new Button
            {
                Content = "×",
                Width = 22,
                Height = 22,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Cursor = System.Windows.Input.Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            btnXoa.Click += (s, e) => { _danhSach.Remove(item); RenderTable(); };
            Grid.SetColumn(btnXoa, 4);
            g.Children.Add(btnXoa);

            row.Child = g;
            return row;
        }

        private void CapNhatTong()
        {
            double tong = _danhSach.Sum(d => d.KhoiLuong);
            TongKhoiLuongTextBlock.Text = tong.ToString();
        }

        private void ThemDong_Click(object sender, RoutedEventArgs e)
        {
            _danhSach.Add(new VatLieuSuDung());
            RenderTable();
        }

        private void XacNhan_Click(object sender, RoutedEventArgs e)
        {
            KetQua = _danhSach.Where(d => !string.IsNullOrEmpty(d.TenVatLieu)).ToList();
            foreach (var item in KetQua) item.Ngay = NgayNhatKy;
            DialogResult = true;
        }

        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}