#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Trita.Views.NhatKyThiCong;
using Trita.Views.NhatKyThiCong;
using Trita.Views.VatTu;

namespace Trita.Views.NhatKyThiCong
{
    public partial class NhatKyThiCongView : UserControl
    {
        private NhatKyThiCongData _data = new NhatKyThiCongData();
        private CongTacItem _congTacDangChon;

        public List<VatTuItem> NguonVatTu { get; set; } = new List<VatTuItem>();
        public List<string> NguonMayMoc { get; set; } = new List<string>();

        private static readonly List<Color> _palette = new List<Color>
        {
            Color.FromRgb(74,  144, 217),
            Color.FromRgb(240, 173,  78),
            Color.FromRgb(92,  184,  92),
            Color.FromRgb(217,  83,  79),
            Color.FromRgb(153, 102, 204),
            Color.FromRgb(23,  162, 184),
            Color.FromRgb(255, 127,  80),
            Color.FromRgb(60,  179, 113)
        };

        public NhatKyThiCongView()
        {
            InitializeComponent();
            NgayPicker.SelectedDate = DateTime.Today;

            // Dữ liệu mẫu
            _data.DanhSachCongTac.Add(new CongTacItem
            {
                HangMuc = "Hạng mục 1",
                CongViec = "Công việc 1",
                LyTrinh = "Từ A đến B",
                DonVi = "Đơn vị 1",
                KhoiLuong = 0,
                TienDo = 70
            });

            // Đẩy vào lịch sử
            LichSuNhatKy.TatCaNgay.Add(_data);

            RenderBangCongTac();
            RenderBangNghiemThu();
            VeBieuDo();
        }

        // ===================================================
        // BẢNG CÔNG TÁC CHI TIẾT
        // ===================================================

        private void RenderBangCongTac()
        {
            BangCongTacPanel.Children.Clear();
            for (int i = 0; i < _data.DanhSachCongTac.Count; i++)
                BangCongTacPanel.Children.Add(TaoDongCongTac(i, _data.DanhSachCongTac[i]));
        }

        private Border TaoDongCongTac(int idx, CongTacItem ct)
        {
            bool isSelected = ct == _congTacDangChon;
            Border row = new Border
            {
                Background = isSelected
                    ? new SolidColorBrush(Color.FromRgb(210, 230, 255))
                    : (idx % 2 == 0
                        ? Brushes.White
                        : new SolidColorBrush(Color.FromRgb(245, 248, 245))),
                BorderBrush = new SolidColorBrush(Color.FromRgb(210, 225, 210)),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            row.MouseLeftButtonDown += (s, e) =>
            {
                _congTacDangChon = ct;
                RenderBangCongTac();
                RenderChiTiet();
                VeBieuDo();
            };

            Grid g = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(56) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(58) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });

            AddCell(g, 0, (idx + 1).ToString(), HorizontalAlignment.Center, true);
            AddEditCell(g, 1, ct.HangMuc, v => ct.HangMuc = v);
            AddEditCell(g, 2, ct.CongViec, v => ct.CongViec = v);
            AddEditCell(g, 3, ct.LyTrinh, v => ct.LyTrinh = v);
            AddEditCell(g, 4, ct.DonVi, v => ct.DonVi = v);

            var tbKL = MakeTB(ct.KhoiLuong > 0 ? ct.KhoiLuong.ToString() : "");
            tbKL.HorizontalContentAlignment = HorizontalAlignment.Center;
            tbKL.TextChanged += (s, e) =>
            {
                if (double.TryParse(tbKL.Text, out double kl)) ct.KhoiLuong = kl;
            };
            Grid.SetColumn(tbKL, 5); g.Children.Add(tbKL);

            var tbTD = MakeTB(ct.TienDo > 0 ? ct.TienDo.ToString("F0") + "%" : "");
            tbTD.HorizontalContentAlignment = HorizontalAlignment.Center;
            tbTD.TextChanged += (s, e) =>
            {
                string clean = tbTD.Text.Replace("%", "").Trim();
                if (double.TryParse(clean, out double td)) ct.TienDo = td;
                VeBieuDo();
            };
            Grid.SetColumn(tbTD, 6); g.Children.Add(tbTD);

            AddCell(g, 7, ct.TongMay > 0 ? ct.TongMay + " máy" : "-", HorizontalAlignment.Center, false);
            AddCell(g, 8, ct.TongLoaiVatLieu > 0 ? ct.TongLoaiVatLieu + " loại" : "-", HorizontalAlignment.Center, false);

            row.Child = g;
            return row;
        }

        private void AddCell(Grid g, int col, string text, HorizontalAlignment ha, bool bold)
        {
            TextBlock tb = new TextBlock
            {
                Text = text,
                HorizontalAlignment = ha,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 11,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                Padding = new Thickness(4, 0, 4, 0),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetColumn(tb, col); g.Children.Add(tb);
        }

        private void AddEditCell(Grid g, int col, string text, Action<string> onChange)
        {
            var tb = MakeTB(text);
            tb.TextChanged += (s, e) => onChange(tb.Text);
            Grid.SetColumn(tb, col); g.Children.Add(tb);
        }

        private TextBox MakeTB(string text) => new TextBox
        {
            Text = text,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            FontSize = 11,
            Padding = new Thickness(4, 0, 4, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };

        private void ThemCongTac_Click(object sender, RoutedEventArgs e) => ThemCongTacMoi();

        public void ThemCongTacMoi()
        {
            _data.DanhSachCongTac.Add(new CongTacItem
            {
                HangMuc = $"Hạng mục {_data.DanhSachCongTac.Count + 1}",
                CongViec = "Công việc mới",
                LyTrinh = "",
                DonVi = ""
            });
            RenderBangCongTac();
        }

        // ===================================================
        // BẢNG NGHIỆM THU
        // ===================================================

        private void RenderBangNghiemThu()
        {
            BangNghiemThuPanel.Children.Clear();
            for (int i = 0; i < _data.DanhSachNghiemThu.Count; i++)
                BangNghiemThuPanel.Children.Add(TaoDongNghiemThu(i, _data.DanhSachNghiemThu[i]));
        }

        private Border TaoDongNghiemThu(int idx, NghiemThuItem item)
        {
            Border row = new Border
            {
                Background = idx % 2 == 0 ? Brushes.White
                    : new SolidColorBrush(Color.FromRgb(240, 250, 240)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(180, 220, 180)),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Cursor = System.Windows.Input.Cursors.Hand,
                Padding = new Thickness(0, 3, 0, 3)
            };

            // Click để chỉnh sửa
            row.MouseLeftButtonDown += (s, e) => MoCuaSoNghiemThu(item);

            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });

            AddCell(g, 0, (idx + 1).ToString(), HorizontalAlignment.Center, true);
            AddCell(g, 1, item.HangMuc, HorizontalAlignment.Left, false);
            AddCell(g, 2, item.TenCongViec, HorizontalAlignment.Left, false);
            AddCell(g, 3, item.LyTrinh, HorizontalAlignment.Left, false);
            AddCell(g, 4, item.GioPhut.ToString(@"hh\:mm"), HorizontalAlignment.Center, false);

            // Nút xóa
            Button btnXoa = new Button
            {
                Content = "×",
                Width = 20,
                Height = 20,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            btnXoa.Click += (s, e) =>
            {
                e.Handled = true; // Không trigger row click
                _data.DanhSachNghiemThu.Remove(item);
                RenderBangNghiemThu();
            };
            Grid.SetColumn(btnXoa, 5); g.Children.Add(btnXoa);

            row.Child = g;
            return row;
        }

        private void ThemNghiemThu_Click(object sender, RoutedEventArgs e)
        {
            MoCuaSoNghiemThu(null);
        }

        private void MoCuaSoNghiemThu(NghiemThuItem itemCu)
        {
            // Lấy công tác từ lịch sử (trước ngày hiện tại + ngày hiện tại)
            var nguon = LichSuNhatKy.LayTatCaCongTac();
            // Nếu chưa có lịch sử, dùng dữ liệu hiện tại
            if (!nguon.Any()) nguon = _data.DanhSachCongTac.ToList();

            var popup = new NghiemThuPopupWindow(nguon, itemCu)
            {
                Owner = Window.GetWindow(this)
            };

            if (popup.ShowDialog() == true)
            {
                if (itemCu != null)
                {
                    // Cập nhật item cũ
                    itemCu.HangMuc = popup.KetQua.HangMuc;
                    itemCu.TenCongViec = popup.KetQua.TenCongViec;
                    itemCu.LyTrinh = popup.KetQua.LyTrinh;
                    itemCu.GioPhut = popup.KetQua.GioPhut;
                }
                else
                {
                    _data.DanhSachNghiemThu.Add(popup.KetQua);
                }
                RenderBangNghiemThu();
            }
        }

        // ===================================================
        // CHI TIẾT CÔNG TÁC (cột phải)
        // ===================================================

        private void RenderChiTiet()
        {
            ChiTietCongTacPanel.Children.Clear();
            if (_congTacDangChon == null) return;

            CongTacItem ct = _congTacDangChon;

            // Header thông tin
            ChiTietCongTacPanel.Children.Add(MakeBulletTitle($"Hạng mục : {ct.HangMuc}"));
            ChiTietCongTacPanel.Children.Add(MakeInfo($"Công việc: {ct.CongViec}"));
            ChiTietCongTacPanel.Children.Add(MakeInfo($"Lý trình: {ct.LyTrinh}"));
            ChiTietCongTacPanel.Children.Add(MakeInfo($"Khối lượng thực hiện: {ct.KhoiLuong} {ct.DonVi}"));

            // Progress bar dùng Grid thay vì Canvas (fix lỗi không khớp khung)
            double pct = Math.Min(Math.Max(ct.TienDo / 100.0, 0), 1.0);
            Grid pgGrid = new Grid { Height = 10, Margin = new Thickness(0, 6, 0, 10) };
            pgGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(pct, GridUnitType.Star) });
            pgGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1 - pct, GridUnitType.Star) });

            Border pgBg = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                CornerRadius = new CornerRadius(5)
            };
            Grid.SetColumnSpan(pgBg, 2);
            pgGrid.Children.Add(pgBg);

            if (pct > 0)
            {
                Border pgFill = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(74, 144, 217)),
                    CornerRadius = new CornerRadius(5)
                };
                Grid.SetColumn(pgFill, 0);
                pgGrid.Children.Add(pgFill);
            }
            ChiTietCongTacPanel.Children.Add(pgGrid);

            // Separator
            ChiTietCongTacPanel.Children.Add(new Border
            {
                Height = 1,
                Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                Margin = new Thickness(0, 4, 0, 8)
            });

            // Vật liệu
            ChiTietCongTacPanel.Children.Add(MakeSectionBtn("Vật liệu", () => MoCuaSoVatLieu(ct)));
            if (ct.DanhSachVatLieu.Count > 0)
            {
                foreach (var vl in ct.DanhSachVatLieu)
                {
                    Grid gVL = new Grid { Margin = new Thickness(8, 1, 0, 1) };
                    gVL.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    gVL.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });
                    gVL.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(55) });
                    TextBlock t1 = new TextBlock { Text = vl.TenVatLieu, FontSize = 11 };
                    TextBlock t2 = new TextBlock { Text = vl.KhoiLuong.ToString(), FontSize = 11, TextDecorations = TextDecorations.Underline, HorizontalAlignment = HorizontalAlignment.Center };
                    TextBlock t3 = new TextBlock { Text = vl.DonVi, FontSize = 11, TextDecorations = TextDecorations.Underline, HorizontalAlignment = HorizontalAlignment.Center };
                    Grid.SetColumn(t1, 0); gVL.Children.Add(t1);
                    Grid.SetColumn(t2, 1); gVL.Children.Add(t2);
                    Grid.SetColumn(t3, 2); gVL.Children.Add(t3);
                    ChiTietCongTacPanel.Children.Add(gVL);
                }
            }

            ChiTietCongTacPanel.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(235, 235, 235)), Margin = new Thickness(0, 6, 0, 6) });

            // Máy móc
            ChiTietCongTacPanel.Children.Add(MakeSectionBtn("Máy móc/Thiết bị", () => MoCuaSoMayMoc(ct)));
            if (ct.DanhSachMayMoc.Count > 0)
            {
                foreach (var mm in ct.DanhSachMayMoc)
                {
                    Grid gMM = new Grid { Margin = new Thickness(8, 1, 0, 1) };
                    gMM.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    gMM.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                    gMM.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                    TextBlock t1 = new TextBlock { Text = mm.TenMay, FontSize = 11 };
                    TextBlock t2 = new TextBlock { Text = $"{mm.SoLuong} chiếc", FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center };
                    TextBlock t3 = new TextBlock { Text = $"{mm.CaMay} ca", FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center };
                    Grid.SetColumn(t1, 0); gMM.Children.Add(t1);
                    Grid.SetColumn(t2, 1); gMM.Children.Add(t2);
                    Grid.SetColumn(t3, 2); gMM.Children.Add(t3);
                    ChiTietCongTacPanel.Children.Add(gMM);
                }
            }

            ChiTietCongTacPanel.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(235, 235, 235)), Margin = new Thickness(0, 6, 0, 6) });

            // Nhân lực
            ChiTietCongTacPanel.Children.Add(MakeSectionBtn("Nhân lực", () => MoCuaSoNhanLuc(ct)));
            if (ct.NhanLuc.SoLuongNhanCong > 0)
            {
                ChiTietCongTacPanel.Children.Add(MakeInfo($"   Số lượng: {ct.NhanLuc.SoLuongNhanCong} người", 11));
                if (!string.IsNullOrEmpty(ct.NhanLuc.KyThuat))
                    ChiTietCongTacPanel.Children.Add(MakeInfo($"   Kỹ thuật: {ct.NhanLuc.KyThuat}", 11));
                ChiTietCongTacPanel.Children.Add(MakeInfo($"   An toàn LĐ: {(ct.NhanLuc.AnToanLaoDong ? "✓ Có" : "✗ Không")}", 11));
                ChiTietCongTacPanel.Children.Add(MakeInfo($"   An toàn VSMT: {(ct.NhanLuc.AnToanVeSinhMoiTruong ? "✓ Có" : "✗ Không")}", 11));
            }
        }

        // ===================================================
        // POPUP HANDLERS
        // ===================================================

        private void MoCuaSoVatLieu(CongTacItem ct)
        {
            var popup = new VatLieuPopupWindow(NguonVatTu, ct.DanhSachVatLieu) { Owner = Window.GetWindow(this) };
            if (popup.ShowDialog() == true) { ct.DanhSachVatLieu = popup.KetQua; RenderBangCongTac(); RenderChiTiet(); VeBieuDo(); }
        }

        private void MoCuaSoMayMoc(CongTacItem ct)
        {
            var popup = new MayMocPopupWindow(NguonMayMoc, ct.DanhSachMayMoc) { Owner = Window.GetWindow(this) };
            if (popup.ShowDialog() == true) { ct.DanhSachMayMoc = popup.KetQua; RenderBangCongTac(); RenderChiTiet(); VeBieuDo(); }
        }

        private void MoCuaSoNhanLuc(CongTacItem ct)
        {
            var popup = new NhanLucPopupWindow(ct.NhanLuc) { Owner = Window.GetWindow(this) };
            if (popup.ShowDialog() == true) { ct.NhanLuc = popup.KetQua; RenderBangCongTac(); RenderChiTiet(); VeBieuDo(); }
        }

        // ===================================================
        // HELPER UI
        // ===================================================

        private StackPanel MakeSectionBtn(string title, Action onPlus)
        {
            var sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 4) };
            sp.Children.Add(new TextBlock { Text = "•  ", FontSize = 13, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center });
            sp.Children.Add(new TextBlock { Text = title, FontWeight = FontWeights.Bold, FontSize = 13, TextDecorations = TextDecorations.Underline, VerticalAlignment = VerticalAlignment.Center });
            var btn = new Button { Content = " [+]", Background = Brushes.Transparent, BorderThickness = new Thickness(0), Foreground = new SolidColorBrush(Color.FromRgb(74, 144, 217)), FontWeight = FontWeights.Bold, FontSize = 13, Cursor = System.Windows.Input.Cursors.Hand, VerticalAlignment = VerticalAlignment.Center, Padding = new Thickness(0) };
            btn.Click += (s, e) => onPlus();
            sp.Children.Add(btn);
            return sp;
        }

        private TextBlock MakeBulletTitle(string text) => new TextBlock
        {
            Text = $"•  {text}",
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 2)
        };

        private TextBlock MakeInfo(string text, double fs = 11) => new TextBlock
        {
            Text = text,
            FontSize = fs,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 1, 0, 1)
        };

        // ===================================================
        // BIỂU ĐỒ
        // ===================================================

        private void VeBieuDo()
        {
            if (_congTacDangChon == null)
            {
                VeBieuDoTrong(BieuDoVatLieu, ChuThichVatLieu);
                VeBieuDoTrong(BieuDoMayMoc, ChuThichMayMoc);
                VeBieuDoTrong(BieuDoNhanLuc, ChuThichNhanLuc);
                VeDonut(BieuDoTienDo, 0);
                TienDoText.Text = "0%";
                return;
            }

            var ct = _congTacDangChon;

            // Vật liệu
            var dataVL = ct.DanhSachVatLieu
                .GroupBy(v => v.TenVatLieu)
                .Select(g => (g.Key, (double)g.Sum(x => x.KhoiLuong))).ToList();
            VeBieuDoPie(BieuDoVatLieu, ChuThichVatLieu, dataVL);

            // Máy móc
            var dataMM = ct.DanhSachMayMoc
                .GroupBy(m => m.TenMay)
                .Select(g => (g.Key, (double)g.Sum(x => x.SoLuong))).ToList();
            VeBieuDoPie(BieuDoMayMoc, ChuThichMayMoc, dataMM);

            // Nhân lực
            var dataNL = new List<(string, double)>();
            if (!string.IsNullOrEmpty(ct.NhanLuc.KyThuat) && ct.NhanLuc.SoLuongNhanCong > 0)
            { dataNL.Add(("Kỹ thuật", 1)); dataNL.Add(("Nhân công", ct.NhanLuc.SoLuongNhanCong)); }
            else if (ct.NhanLuc.SoLuongNhanCong > 0)
                dataNL.Add(("Nhân công", ct.NhanLuc.SoLuongNhanCong));
            VeBieuDoPie(BieuDoNhanLuc, ChuThichNhanLuc, dataNL);

            // Tiến độ donut
            double td = Math.Min(Math.Max(ct.TienDo, 0), 100);
            VeDonut(BieuDoTienDo, td);
            TienDoText.Text = td.ToString("F0") + "%";

            // Chú thích tiến độ
            ChuThichTienDo.Children.Clear();
            AddLegend(ChuThichTienDo, Color.FromRgb(74, 144, 217), $"Hoàn thành: {td:F0}%");
            AddLegend(ChuThichTienDo, Color.FromRgb(220, 220, 220), $"Còn lại: {100 - td:F0}%");
        }

        private void VeDonut(Canvas canvas, double pct)
        {
            canvas.Children.Clear();
            double cx = 50, cy = 50, rOuter = 45, rInner = 28;

            if (pct <= 0)
            {
                // Vòng tròn rỗng xám
                VeDonutArc(canvas, cx, cy, rOuter, rInner, 0, 360, Color.FromRgb(220, 220, 220));
                return;
            }
            if (pct >= 100)
            {
                VeDonutArc(canvas, cx, cy, rOuter, rInner, 0, 360, Color.FromRgb(74, 144, 217));
                return;
            }

            double sweepDeg = pct / 100.0 * 360.0;

            // Phần còn lại (xám) - vẽ trước
            VeDonutArc(canvas, cx, cy, rOuter, rInner, sweepDeg - 90, 360 - sweepDeg,
                Color.FromRgb(220, 220, 220));
            // Phần hoàn thành (xanh)
            VeDonutArc(canvas, cx, cy, rOuter, rInner, -90, sweepDeg,
                Color.FromRgb(74, 144, 217));
        }

        private void VeDonutArc(Canvas canvas, double cx, double cy,
            double rOuter, double rInner, double startDeg, double sweepDeg, Color color)
        {
            if (sweepDeg <= 0) return;

            double startRad = startDeg * Math.PI / 180;
            double endRad = (startDeg + sweepDeg) * Math.PI / 180;
            bool large = sweepDeg > 180;

            // 4 điểm: outer start, outer end, inner end, inner start
            Point outerStart = new Point(cx + rOuter * Math.Cos(startRad), cy + rOuter * Math.Sin(startRad));
            Point outerEnd = new Point(cx + rOuter * Math.Cos(endRad), cy + rOuter * Math.Sin(endRad));
            Point innerEnd = new Point(cx + rInner * Math.Cos(endRad), cy + rInner * Math.Sin(endRad));
            Point innerStart = new Point(cx + rInner * Math.Cos(startRad), cy + rInner * Math.Sin(startRad));

            PathFigure fig = new PathFigure { StartPoint = outerStart, IsClosed = true };
            fig.Segments.Add(new ArcSegment(outerEnd, new Size(rOuter, rOuter), 0, large, SweepDirection.Clockwise, true));
            fig.Segments.Add(new LineSegment(innerEnd, true));
            fig.Segments.Add(new ArcSegment(innerStart, new Size(rInner, rInner), 0, large, SweepDirection.Counterclockwise, true));

            canvas.Children.Add(new Path
            {
                Data = new PathGeometry(new[] { fig }),
                Fill = new SolidColorBrush(color)
            });
        }

        private void VeBieuDoPie(Canvas canvas, StackPanel legend, List<(string Label, double Value)> data)
        {
            canvas.Children.Clear();
            legend.Children.Clear();

            if (data == null || data.Count == 0 || data.Sum(d => d.Value) == 0)
            { VeBieuDoTrong(canvas, legend); return; }

            double cx = 50, cy = 50, r = 46;
            double total = data.Sum(d => d.Value);
            double angle = -Math.PI / 2;

            for (int i = 0; i < data.Count; i++)
            {
                double sweep = data[i].Value / total * 2 * Math.PI;
                Color mau = _palette[i % _palette.Count];

                if (data.Count == 1)
                {
                    canvas.Children.Add(new Ellipse { Width = r * 2, Height = r * 2, Fill = new SolidColorBrush(mau) });
                    Canvas.SetLeft(canvas.Children[0] as UIElement, cx - r);
                    Canvas.SetTop(canvas.Children[0] as UIElement, cy - r);
                }
                else
                {
                    double x1 = cx + r * Math.Cos(angle);
                    double y1 = cy + r * Math.Sin(angle);
                    double x2 = cx + r * Math.Cos(angle + sweep);
                    double y2 = cy + r * Math.Sin(angle + sweep);
                    PathFigure fig = new PathFigure { StartPoint = new Point(cx, cy), IsClosed = true };
                    fig.Segments.Add(new LineSegment(new Point(x1, y1), true));
                    fig.Segments.Add(new ArcSegment(new Point(x2, y2), new Size(r, r), 0, sweep > Math.PI, SweepDirection.Clockwise, true));
                    canvas.Children.Add(new Path { Data = new PathGeometry(new[] { fig }), Fill = new SolidColorBrush(mau) });
                }

                AddLegend(legend, mau, data[i].Label);
                angle += sweep;
            }
        }

        private void VeBieuDoTrong(Canvas canvas, StackPanel legend)
        {
            canvas.Children.Clear();
            legend.Children.Clear();
            var el = new Ellipse { Width = 92, Height = 92, Fill = new SolidColorBrush(Color.FromRgb(235, 235, 235)) };
            Canvas.SetLeft(el, 4); Canvas.SetTop(el, 4);
            canvas.Children.Add(el);
        }

        private void AddLegend(StackPanel legend, Color color, string label)
        {
            var sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 1, 0, 1) };
            sp.Children.Add(new Rectangle { Width = 9, Height = 9, Fill = new SolidColorBrush(color), Margin = new Thickness(0, 0, 4, 0), VerticalAlignment = VerticalAlignment.Center });
            sp.Children.Add(new TextBlock { Text = label, FontSize = 9, VerticalAlignment = VerticalAlignment.Center });
            legend.Children.Add(sp);
        }
    }
}