#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Trita.Views.NhatKyThiCong;
using Trita.Views.VatTu;

namespace Trita.Views.BangPhanBo
{
    public partial class BangPhanBoVatTuView : UserControl
    {
        // ── Model nội bộ: mỗi dòng bảng phân bổ ──────────────────────────
        private class DongPhanBo
        {
            public string TenCongTac { get; set; }
            public string LyTrinh { get; set; }
            public DateTime Ngay { get; set; }
            public double KhoiLuong { get; set; }  // khối lượng vật liệu dùng trong ngày đó
        }

        private class DonutSegment
        {
            public string Label { get; set; }
            public double Value { get; set; }
            public double Percent { get; set; }
            public Color Color { get; set; }
        }

        private List<VatTuItem> _danhSachVatTu = new();
        private VatTuItem _vatTuDangChon;
        private List<CongTacItem> _tatCaCongTac = new();   // toàn bộ lịch sử

        public BangPhanBoVatTuView()
        {
            InitializeComponent();
            KhoiTaoDuLieuMau();
        }

        // ── Nhận data từ MainWindow ───────────────────────────────────────
        public void LoadDataFromVatTuView(List<VatTuItem> danhSachVatTu,
                                          List<CongTacItem> tatCaCongTac = null)
        {
            _danhSachVatTu = danhSachVatTu ?? new List<VatTuItem>();
            _tatCaCongTac = tatCaCongTac ?? new List<CongTacItem>();
            HienThiDanhSachVatTu();
        }

        private void KhoiTaoDuLieuMau()
        {
            _danhSachVatTu = new List<VatTuItem>
            {
                new VatTuItem { Ten = "Vật tư 1", DonVi = "Tấn",
                    DanhSachLo = new List<LoVatTu> { new LoVatTu { KhoiLuong = 100 } } }
            };
            _tatCaCongTac = new List<CongTacItem>();
            HienThiDanhSachVatTu();
        }

        // ── Danh sách vật tư (cột trái) ──────────────────────────────────
        private void HienThiDanhSachVatTu()
        {
            DanhSachVatTuPanel.Children.Clear();
            if (_danhSachVatTu == null || _danhSachVatTu.Count == 0)
            {
                DanhSachVatTuPanel.Children.Add(new TextBlock
                {
                    Text = "Chưa có vật tư nào",
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10)
                });
                return;
            }

            for (int i = 0; i < _danhSachVatTu.Count; i++)
            {
                var vt = _danhSachVatTu[i];
                var btn = new Button
                {
                    Content = $"{i + 1}.   [{vt.Ten}]",
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Padding = new Thickness(5, 8, 5, 8),
                    FontWeight = FontWeights.Bold
                };
                btn.Click += (s, e) => { _vatTuDangChon = vt; HienThiBangPhanBo(); };
                DanhSachVatTuPanel.Children.Add(btn);

                if (i < _danhSachVatTu.Count - 1)
                    DanhSachVatTuPanel.Children.Add(new TextBlock
                    {
                        Text = ".......................",
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(0, 2, 0, 2)
                    });
            }
        }

        // ── Bảng phân bổ chính ───────────────────────────────────────────
        private void HienThiBangPhanBo()
        {
            if (_vatTuDangChon == null) return;

            TitleTextBlock.Text = _vatTuDangChon.Ten.ToUpper();
            DonViTextBlock.Text = $"Đơn vị: [{_vatTuDangChon.DonVi}]";

            double tongKL = _vatTuDangChon.DanhSachLo?.Sum(lo => lo.KhoiLuong) ?? 0;

            // Tổng hợp tất cả dòng phân bổ: 1 dòng = (công tác, lý trình, ngày)
            var danhSachDong = LayDanhSachDongPhanBo();

            double tongDaSuDung = danhSachDong.Sum(d => d.KhoiLuong);
            TongKhoiLuongTextBlock.Text = $"{Math.Max(tongKL - tongDaSuDung, 0):N0}";

            HienThiBangCongTac(tongKL, danhSachDong);
            HienThiThongKeLo(tongDaSuDung);
            VeBieuDoDonut(tongKL, tongDaSuDung, danhSachDong);
        }

        /// <summary>
        /// Duyệt TOÀN BỘ lịch sử công tác → mỗi (CongTac × Ngày) có dùng vật liệu
        /// đang chọn → thành 1 DongPhanBo riêng biệt.
        /// </summary>
        private List<DongPhanBo> LayDanhSachDongPhanBo()
        {
            if (_vatTuDangChon == null) return new List<DongPhanBo>();

            string tenVT = _vatTuDangChon.Ten?.Trim();
            var result = new List<DongPhanBo>();

            foreach (var ct in _tatCaCongTac)
            {
                if (ct.DanhSachVatLieu == null) continue;

                // Nhóm theo ngày sử dụng trong cùng 1 công tác
                var theoNgay = ct.DanhSachVatLieu
                    .Where(vl => string.Equals(
                        vl.TenVatLieu?.Trim(), tenVT,
                        StringComparison.CurrentCultureIgnoreCase))
                    .GroupBy(vl => vl.Ngay.Date)
                    .OrderBy(g => g.Key);

                foreach (var nhom in theoNgay)
                {
                    double kl = nhom.Sum(vl => vl.KhoiLuong);
                    if (kl <= 0) continue;

                    result.Add(new DongPhanBo
                    {
                        TenCongTac = string.IsNullOrWhiteSpace(ct.CongViec)
                                     ? "(Chưa đặt tên)" : ct.CongViec,
                        LyTrinh = ct.LyTrinh ?? "",
                        Ngay = nhom.Key,
                        KhoiLuong = kl
                    });
                }
            }

            // Sắp xếp theo ngày tăng dần
            return result.OrderBy(d => d.Ngay).ToList();
        }

        private void HienThiBangCongTac(double tongKL, List<DongPhanBo> danhSachDong)
        {
            BangPhanBoPanel.Children.Clear();

            // ── Dòng "Vật tư đầu kỳ" — span toàn bộ tên+lý trình+ngày, số ở cột ĐÃ DÙNG ──
            var hdrGrid = new Grid { MinHeight = 44, Background = new SolidColorBrush(Color.FromRgb(214, 232, 255)) };
            hdrGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(46) });
            hdrGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) });
            hdrGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) });
            hdrGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            hdrGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            hdrGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Label "Vật tư đầu kỳ" chiếm col 0-3
            var lblDauKy = new TextBlock
            {
                Text = "Vật tư đầu kỳ",
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 60, 140)),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetColumn(lblDauKy, 0); Grid.SetColumnSpan(lblDauKy, 4);
            hdrGrid.Children.Add(lblDauKy);

            // Đường dọc + tổng KL ở col 4
            var sepHdr4 = new Border { BorderBrush = new SolidColorBrush(Color.FromRgb(154, 188, 216)), BorderThickness = new Thickness(1, 0, 0, 0) };
            Grid.SetColumn(sepHdr4, 4); hdrGrid.Children.Add(sepHdr4);

            var lblTong = new TextBlock
            {
                Text = tongKL.ToString("N0"),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 70, 160)),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(4, 0, 8, 0)
            };
            Grid.SetColumn(lblTong, 4); hdrGrid.Children.Add(lblTong);

            // Đường dọc col 5 (cột tồn dư — để trống)
            var sepHdr5 = new Border { BorderBrush = new SolidColorBrush(Color.FromRgb(154, 188, 216)), BorderThickness = new Thickness(1, 0, 0, 0) };
            Grid.SetColumn(sepHdr5, 5); hdrGrid.Children.Add(sepHdr5);

            // Đường kẻ dưới dòng đầu kỳ
            var hdrBottom = new Border { BorderBrush = new SolidColorBrush(Color.FromRgb(90, 144, 192)), BorderThickness = new Thickness(0, 0, 0, 2), VerticalAlignment = VerticalAlignment.Bottom };
            Grid.SetColumnSpan(hdrBottom, 6); hdrGrid.Children.Add(hdrBottom);

            BangPhanBoPanel.Children.Add(hdrGrid);

            // ── Các dòng dữ liệu ──
            double tonDu = tongKL;
            for (int i = 0; i < danhSachDong.Count; i++)
            {
                var d = danhSachDong[i];
                tonDu = Math.Max(tonDu - d.KhoiLuong, 0);

                var row = TaoRow(
                    (i + 1).ToString(),
                    d.TenCongTac,
                    d.LyTrinh,
                    d.Ngay.ToString("dd/MM/yyyy"),
                    d.KhoiLuong.ToString("N0"),
                    tonDu.ToString("N0"),
                    false
                );
                row.Background = i % 2 == 0 ? Brushes.White
                    : new SolidColorBrush(Color.FromRgb(245, 248, 253));
                BangPhanBoPanel.Children.Add(row);
            }
        }

        // Header dòng: STT | Tên | Lý trình | Ngày | Đã dùng | Tồn dư
        private Grid TaoRow(string stt, string tenCT, string lyTrinh,
                            string ngay, string daSuDung, bool isHeader)
            => TaoRow(stt, tenCT, lyTrinh, ngay, daSuDung, "", isHeader);

        private Grid TaoRow(string stt, string tenCT, string lyTrinh,
                            string ngay, string daSuDung, string tonDu, bool isHeader)
        {
            var row = new Grid { MinHeight = 36 };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(46) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Căn lề: STT=Center, Tên/Lý trình=Left, Ngày=Center, Số=Right
            var aligns = new[] {
                HorizontalAlignment.Center,
                HorizontalAlignment.Left,
                HorizontalAlignment.Left,
                HorizontalAlignment.Center,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right
            };

            var cells = new[] { stt, tenCT, lyTrinh, ngay, daSuDung, tonDu };

            for (int c = 0; c < 6; c++)
            {
                var tb = new TextBlock
                {
                    Text = cells[c],
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = aligns[c],
                    FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                    Foreground = isHeader
                        ? new SolidColorBrush(Color.FromRgb(0, 0, 128))
                        : Brushes.Black,
                    TextWrapping = TextWrapping.Wrap,
                    Padding = new Thickness(c <= 1 ? 8 : 4, 2, c >= 4 ? 8 : 4, 2)
                };
                Grid.SetColumn(tb, c);
                row.Children.Add(tb);

                if (c > 0) // đường dọc phân cách
                {
                    var sep = new Border
                    {
                        BorderBrush = new SolidColorBrush(Color.FromRgb(210, 215, 220)),
                        BorderThickness = new Thickness(1, 0, 0, 0)
                    };
                    Grid.SetColumn(sep, c);
                    row.Children.Add(sep);
                }
            }

            // Đường ngang dưới
            var bottom = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(210, 215, 220)),
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
            Grid.SetColumnSpan(bottom, 6);
            row.Children.Add(bottom);

            return row;
        }

        private void HienThiThongKeLo(double tongDaSuDung)
        {
            ThongKeLoPanel.Children.Clear();
            if (_vatTuDangChon == null) return;

            var kl = _vatTuDangChon.DanhSachLo.Select(lo => lo.KhoiLuong).ToList();
            double canTru = tongDaSuDung;
            for (int i = 0; i < kl.Count && canTru > 0; i++)
            {
                double tru = Math.Min(kl[i], canTru);
                kl[i] -= tru; canTru -= tru;
            }

            for (int i = 0; i < _vatTuDangChon.DanhSachLo.Count; i++)
            {
                var g = new Grid { Margin = new Thickness(0, 4, 0, 4) };
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                g.Children.Add(new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(15),
                    Height = 28,
                    Child = new TextBlock
                    {
                        Text = $"Lô {i + 1}",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontWeight = FontWeights.Bold,
                        FontSize = 11
                    }
                });

                var valBorder = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(15),
                    Height = 28,
                    Margin = new Thickness(8, 0, 0, 0),
                    Child = new TextBlock
                    {
                        Text = $"[{Math.Max(kl[i], 0):F2}]",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 11
                    }
                };
                Grid.SetColumn(valBorder, 1);
                g.Children.Add(valBorder);
                ThongKeLoPanel.Children.Add(g);
            }
        }

        private void VeBieuDoDonut(double tongKL, double tongDaSuDung, List<DongPhanBo> danhSachDong)
        {
            DonutCanvas.Children.Clear();
            if (_vatTuDangChon == null || tongKL == 0) return;

            double cx = 120, cy = 150, rO = 110, rI = 65;
            double conLai = Math.Max(tongKL - tongDaSuDung, 0);

            // Gộp theo công tác (tên) để hiển thị donut
            var nhomCT = danhSachDong
                .GroupBy(d => d.TenCongTac)
                .Select(g => (Label: g.Key, Value: g.Sum(d => d.KhoiLuong)))
                .ToList();

            var segs = new List<DonutSegment>
            {
                new DonutSegment
                {
                    Label   = "Tồn kho",
                    Value   = conLai,
                    Percent = tongKL > 0 ? conLai / tongKL * 100 : 0,
                    Color   = Color.FromRgb(70, 130, 180)
                }
            };

            string[] palette = { "#FF6B35", "#FFA500", "#FFD700", "#5B8C3A", "#9B59B6", "#E74C3C" };
            for (int i = 0; i < nhomCT.Count; i++)
            {
                segs.Add(new DonutSegment
                {
                    Label = nhomCT[i].Label,
                    Value = nhomCT[i].Value,
                    Percent = tongKL > 0 ? nhomCT[i].Value / tongKL * 100 : 0,
                    Color = (Color)ColorConverter.ConvertFromString(palette[i % palette.Length])
                });
            }

            double startAngle = -90;
            foreach (var seg in segs)
            {
                double sweep = seg.Percent / 100 * 360;
                var path = CreateDonutSegment(cx, cy, rO, rI, startAngle, sweep);
                path.Fill = new SolidColorBrush(seg.Color);
                path.Stroke = Brushes.White; path.StrokeThickness = 2;
                DonutCanvas.Children.Add(path);

                if (seg.Percent > 5)
                {
                    double mid = startAngle + sweep / 2;
                    double midR = (rO + rI) / 2;
                    double rad = mid * Math.PI / 180;
                    var pct = new TextBlock
                    {
                        Text = $"{seg.Percent:F0}%",
                        FontSize = 13,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White
                    };
                    pct.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Canvas.SetLeft(pct, cx + midR * Math.Cos(rad) - pct.DesiredSize.Width / 2);
                    Canvas.SetTop(pct, cy + midR * Math.Sin(rad) - pct.DesiredSize.Height / 2);
                    DonutCanvas.Children.Add(pct);
                }
                startAngle += sweep;
            }

            // Chú thích
            double oy = cy + rO + 15;
            foreach (var seg in segs)
            {
                var sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 3, 5, 3) };
                sp.Children.Add(new Rectangle
                {
                    Width = 14,
                    Height = 14,
                    Fill = new SolidColorBrush(seg.Color),
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                });
                sp.Children.Add(new TextBlock
                {
                    Text = seg.Label,
                    FontSize = 11,
                    VerticalAlignment = VerticalAlignment.Center
                });
                Canvas.SetLeft(sp, 10); Canvas.SetTop(sp, oy);
                DonutCanvas.Children.Add(sp);
                oy += 24;
            }
        }

        private Path CreateDonutSegment(double cx, double cy, double rO, double rI,
                                        double start, double sweep)
        {
            if (sweep >= 360) sweep = 359.999;
            double sR = start * Math.PI / 180, eR = (start + sweep) * Math.PI / 180;
            bool large = sweep > 180;

            var fig = new PathFigure
            {
                StartPoint = new Point(cx + rO * Math.Cos(sR), cy + rO * Math.Sin(sR)),
                IsClosed = true
            };
            fig.Segments.Add(new ArcSegment(
                new Point(cx + rO * Math.Cos(eR), cy + rO * Math.Sin(eR)),
                new Size(rO, rO), 0, large, SweepDirection.Clockwise, true));
            fig.Segments.Add(new LineSegment(
                new Point(cx + rI * Math.Cos(eR), cy + rI * Math.Sin(eR)), true));
            fig.Segments.Add(new ArcSegment(
                new Point(cx + rI * Math.Cos(sR), cy + rI * Math.Sin(sR)),
                new Size(rI, rI), 0, large, SweepDirection.Counterclockwise, true));

            var geo = new PathGeometry(); geo.Figures.Add(fig);
            return new Path { Data = geo };
        }
    }
}