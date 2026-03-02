#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Trita.Views.NhatKyThiCong
{
    public partial class NghiemThuPopupWindow : Window
    {
        private List<CongTacItem> _nguon;
        private string _lyTrinhDaChon = "";
        public NghiemThuItem KetQua { get; private set; }

        public NghiemThuPopupWindow(List<CongTacItem> nguonCongTac, NghiemThuItem duLieuCu = null)
        {
            InitializeComponent();
            _nguon = nguonCongTac ?? new List<CongTacItem>();

            // Load hạng mục
            var hangMucs = _nguon
                .Select(c => c.HangMuc)
                .Where(h => !string.IsNullOrEmpty(h))
                .Distinct().OrderBy(x => x).ToList();

            CbHangMuc.Items.Add("-- Chọn hạng mục --");
            foreach (var hm in hangMucs) CbHangMuc.Items.Add(hm);
            CbHangMuc.SelectedIndex = 0;

            // Restore dữ liệu cũ
            if (duLieuCu != null)
            {
                Loaded += (s, e) =>
                {
                    int hmIdx = CbHangMuc.Items.IndexOf(duLieuCu.HangMuc);
                    if (hmIdx >= 0)
                    {
                        CbHangMuc.SelectedIndex = hmIdx;
                        int cvIdx = CbCongViec.Items.IndexOf(duLieuCu.TenCongViec);
                        if (cvIdx >= 0) CbCongViec.SelectedIndex = cvIdx;
                    }
                    SetLyTrinh(duLieuCu.LyTrinh);
                    TxtGio.Text = duLieuCu.GioPhut.Hours.ToString("D2");
                    TxtPhut.Text = duLieuCu.GioPhut.Minutes.ToString("D2");
                };
            }
        }

        private void CbHangMuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CbCongViec.Items.Clear();
            SetLyTrinh("");

            string hm = CbHangMuc.SelectedItem?.ToString() ?? "";
            if (hm == "-- Chọn hạng mục --")
            {
                CbCongViec.Items.Add("-- Chọn công việc --");
                CbCongViec.SelectedIndex = 0;
                return;
            }

            var congViecs = _nguon
                .Where(c => c.HangMuc == hm)
                .Select(c => c.CongViec)
                .Where(cv => !string.IsNullOrEmpty(cv))
                .Distinct().OrderBy(x => x).ToList();

            CbCongViec.Items.Add("-- Chọn công việc --");
            foreach (var cv in congViecs) CbCongViec.Items.Add(cv);
            CbCongViec.SelectedIndex = 0;
        }

        private void CbCongViec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetLyTrinh("");

            string hm = CbHangMuc.SelectedItem?.ToString() ?? "";
            string cv = CbCongViec.SelectedItem?.ToString() ?? "";

            bool coLyTrinh = cv != "-- Chọn công việc --" && !string.IsNullOrEmpty(cv);
            BtnChonLyTrinh.IsEnabled = coLyTrinh;
            BtnChonLyTrinh.Opacity = coLyTrinh ? 1.0 : 0.4;

            // Nếu chỉ có đúng 1 lý trình → tự điền luôn không cần picker
            if (coLyTrinh)
            {
                var lyTrinhs = LayDanhSachLyTrinh(hm, cv);
                if (lyTrinhs.Count == 1) SetLyTrinh(lyTrinhs[0]);
            }
        }

        private void BtnChonLyTrinh_Click(object sender, RoutedEventArgs e)
        {
            string hm = CbHangMuc.SelectedItem?.ToString() ?? "";
            string cv = CbCongViec.SelectedItem?.ToString() ?? "";

            var lyTrinhs = LayDanhSachLyTrinh(hm, cv);

            if (lyTrinhs.Count == 0)
            {
                MessageBox.Show("Không có lý trình nào cho công việc này!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var picker = new LyTrinhPickerWindow(hm, cv, lyTrinhs) { Owner = this };
            if (picker.ShowDialog() == true)
                SetLyTrinh(picker.LyTrinhDaChon);
        }

        private List<string> LayDanhSachLyTrinh(string hm, string cv)
            => _nguon
                .Where(c => c.HangMuc == hm && c.CongViec == cv)
                .Select(c => c.LyTrinh)
                .Where(lt => !string.IsNullOrEmpty(lt))
                .Distinct().OrderBy(x => x).ToList();

        private void SetLyTrinh(string lyTrinh)
        {
            _lyTrinhDaChon = lyTrinh;
            if (string.IsNullOrEmpty(lyTrinh))
            {
                TxtLyTrinhChon.Text = "-- Chọn lý trình --";
                TxtLyTrinhChon.Foreground = new SolidColorBrush(Colors.Gray);
                TxtLyTrinhChon.FontStyle = System.Windows.FontStyles.Italic;
            }
            else
            {
                TxtLyTrinhChon.Text = lyTrinh;
                TxtLyTrinhChon.Foreground = new SolidColorBrush(Colors.Black);
                TxtLyTrinhChon.FontStyle = System.Windows.FontStyles.Normal;
            }
        }

        private void XacNhan_Click(object sender, RoutedEventArgs e)
        {
            string hm = CbHangMuc.SelectedItem?.ToString() ?? "";
            string cv = CbCongViec.SelectedItem?.ToString() ?? "";

            if (hm == "-- Chọn hạng mục --" || string.IsNullOrEmpty(hm))
            { MessageBox.Show("Vui lòng chọn hạng mục!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (cv == "-- Chọn công việc --" || string.IsNullOrEmpty(cv))
            { MessageBox.Show("Vui lòng chọn công việc!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (string.IsNullOrEmpty(_lyTrinhDaChon))
            { MessageBox.Show("Vui lòng chọn lý trình!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            int gio = int.TryParse(TxtGio.Text, out int g) ? Math.Clamp(g, 0, 23) : 8;
            int phut = int.TryParse(TxtPhut.Text, out int p) ? Math.Clamp(p, 0, 59) : 0;

            KetQua = new NghiemThuItem
            {
                HangMuc = hm,
                TenCongViec = cv,
                LyTrinh = _lyTrinhDaChon,
                GioPhut = new TimeSpan(gio, phut, 0)
            };
            DialogResult = true;
        }

        private void Huy_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}