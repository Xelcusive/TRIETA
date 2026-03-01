#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Trita.Views.VatTu;

namespace Trita.Views.NhatKyThiCong
{
    public partial class NhatKyThiCongView : UserControl
    {
        private NhatKyThiCongData _data = new NhatKyThiCongData();
        private CongTacItem _congTacDangChon;

        // ===== NGUỒN DỮ LIỆU TỪ CÁC TAB KHÁC =====
        // Được set từ bên ngoài (MainWindow) sau khi khởi tạo
        public List<VatTuItem> NguonVatTu { get; set; } = new List<VatTuItem>();
        public List<string> NguonMayMoc { get; set; } = new List<string>();

        private static readonly List<Color> _palette = new List<Color>
        {
            Color.FromRgb(74,  144, 217),
            Color.FromRgb(240, 173, 78),
            Color.FromRgb(92,  184, 92),
            Color.FromRgb(217, 83,  79),
            Color.FromRgb(153, 102, 204),
            Color.FromRgb(23,  162, 184),
            Color.FromRgb(255, 127, 80),
            Color.FromRgb(60,  179, 113)
        };

        public NhatKyThiCongView()
        {
            InitializeComponent();
            NgayPicker.SelectedDate = DateTime.Today;
            _data.DanhSachCongTac.Add(new CongTacItem { HangMuc = "Hạng mục 1", CongViec = "Công việc 1", LyTrinh = "Từ A đến B", DonVi = "Đơn vị 1", KhoiLuong = 0, TienDo = 0 });
            RenderBangCongTac();
            VeBieuDo();
        }

        // ===== RENDER BẢNG =====

        private void RenderBangCongTac()
        {
            BangCongTacPanel.Children.Clear();

            for (int i = 0; i < _data.DanhSachCongTac.Count; i++)
            {
                CongTacItem ct = _data.DanhSachCongTac[i];
                BangCongTacPanel.Children.Add(TaoDongBang(i, ct));
            }
        }

        private Border TaoDongBang(int idx, CongTacItem ct)
        {
            bool isEven = idx % 2 == 0;
            bool isSelected = ct == _congTacDangChon;

            Border row = new Border
            {
                Background = isSelected
                    ? new SolidColorBrush(Color.FromRgb(210, 230, 255))
                    : (isEven
                        ? new SolidColorBrush(Color.FromRgb(255, 255, 255))
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
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(65) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(65) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(55) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(65) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(55) });

            // STT
            AddCell(g, 0, (idx + 1).ToString(), HorizontalAlignment.Center, true);

            // Hạng mục - editable
            TextBox tbHM = MakeEditTextBox(ct.HangMuc, v => ct.HangMuc = v);
            Grid.SetColumn(tbHM, 1); g.Children.Add(tbHM);

            // Công việc
            TextBox tbCV = MakeEditTextBox(ct.CongViec, v => ct.CongViec = v);
            Grid.SetColumn(tbCV, 2); g.Children.Add(tbCV);

            // Lý trình
            TextBox tbLT = MakeEditTextBox(ct.LyTrinh, v => ct.LyTrinh = v);
            Grid.SetColumn(tbLT, 3); g.Children.Add(tbLT);

            // Đơn vị
            TextBox tbDV = MakeEditTextBox(ct.DonVi, v => ct.DonVi = v);
            Grid.SetColumn(tbDV, 4); g.Children.Add(tbDV);

            // Khối lượng
            TextBox tbKL = MakeEditTextBox(ct.KhoiLuong > 0 ? ct.KhoiLuong.ToString() : "", v =>
            {
                if (double.TryParse(v, out double kl)) ct.KhoiLuong = kl;
            });
            tbKL.HorizontalContentAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(tbKL, 5); g.Children.Add(tbKL);

            // Tiến độ
            TextBox tbTD = MakeEditTextBox(ct.TienDo > 0 ? ct.TienDo.ToString("F0") + "%" : "", v =>
            {
                string clean = v.Replace("%", "").Trim();
                if (double.TryParse(clean, out double td)) ct.TienDo = td;
            });
            tbTD.HorizontalContentAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(tbTD, 6); g.Children.Add(tbTD);

            // Số máy
            AddCell(g, 7, ct.TongMay > 0 ? ct.TongMay.ToString() + " máy" : "-", HorizontalAlignment.Center, false);

            // Số loại vật liệu
            AddCell(g, 8, ct.TongLoaiVatLieu > 0 ? ct.TongLoaiVatLieu.ToString() + " loại" : "-", HorizontalAlignment.Center, false);

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
            Grid.SetColumn(tb, col);
            g.Children.Add(tb);
        }

        private TextBox MakeEditTextBox(string text, Action<string> onChange)
        {
            TextBox tb = new TextBox
            {
                Text = text,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 11,
                Padding = new Thickness(4, 0, 4, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            tb.TextChanged += (s, e) => onChange(tb.Text);
            return tb;
        }

        private void ThemCongTac_Click(object sender, RoutedEventArgs e)
            => ThemCongTacMoi();

        /// <summary>Gọi từ MainWindow ribbon button</summary>
        public void ThemCongTacMoi()
        {
            _data.DanhSachCongTac.Add(new CongTacItem
            {
                HangMuc = $"Hạng mục {_data.DanhSachCongTac.Count + 1}",
                CongViec = "Công việc mới",
                LyTrinh = "",
                DonVi = "",
                KhoiLuong = 0,
                TienDo = 0
            });
            RenderBangCongTac();
        }

        // ===== RENDER CHI TIẾT CÔNG TÁC =====

        private void RenderChiTiet()
        {
            ChiTietCongTacPanel.Children.Clear();
            if (_congTacDangChon == null) return;

            CongTacItem ct = _congTacDangChon;

            // Hạng mục
            StackPanel spHeader = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };
            spHeader.Children.Add(MakeBulletTitle($"Hạng mục : {ct.HangMuc}"));
            spHeader.Children.Add(MakeInfoLine($"Công việc: {ct.CongViec}"));
            spHeader.Children.Add(MakeInfoLine($"Lý trình: {ct.LyTrinh}"));
            spHeader.Children.Add(MakeInfoLine($"Khối lượng thực hiện: {ct.KhoiLuong} {ct.DonVi}"));
            ChiTietCongTacPanel.Children.Add(spHeader);

            // Progress bar tiến độ
            Border progressBg = new Border { Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)), Height = 12, CornerRadius = new CornerRadius(6), Margin = new Thickness(0, 4, 0, 12) };
            double pct = Math.Min(ct.TienDo / 100.0, 1.0);
            Border progressFill = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(74, 144, 217)),
                HorizontalAlignment = HorizontalAlignment.Left,
                CornerRadius = new CornerRadius(6),
                Height = 12
            };
            progressBg.Loaded += (s, e) =>
            {
                progressFill.Width = progressBg.ActualWidth * pct;
            };
            progressBg.SizeChanged += (s, e) =>
            {
                progressFill.Width = progressBg.ActualWidth * pct;
            };
            Grid pgGrid = new Grid(); pgGrid.Children.Add(progressBg); pgGrid.Children.Add(progressFill);
            ChiTietCongTacPanel.Children.Add(pgGrid);

            // Separator
            ChiTietCongTacPanel.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)), Margin = new Thickness(0, 8, 0, 8) });

            // ===== VẬT LIỆU =====
            ChiTietCongTacPanel.Children.Add(MakeSectionHeader("Vật liệu", () => MoCuaSoVatLieu(ct)));

            if (ct.DanhSachVatLieu.Count > 0)
            {
                foreach (var vl in ct.DanhSachVatLieu)
                {
                    Grid gVL = new Grid { Margin = new Thickness(8, 2, 0, 2) };
                    gVL.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    gVL.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                    gVL.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

                    TextBlock tbTen = new TextBlock { Text = vl.TenVatLieu, FontSize = 11 };
                    Grid.SetColumn(tbTen, 0); gVL.Children.Add(tbTen);

                    TextBlock tbKL = new TextBlock { Text = vl.KhoiLuong.ToString(), FontSize = 11, TextDecorations = TextDecorations.Underline, HorizontalAlignment = HorizontalAlignment.Center };
                    Grid.SetColumn(tbKL, 1); gVL.Children.Add(tbKL);

                    TextBlock tbDV = new TextBlock { Text = vl.DonVi, FontSize = 11, TextDecorations = TextDecorations.Underline, HorizontalAlignment = HorizontalAlignment.Center };
                    Grid.SetColumn(tbDV, 2); gVL.Children.Add(tbDV);

                    ChiTietCongTacPanel.Children.Add(gVL);
                }
            }

            ChiTietCongTacPanel.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(235, 235, 235)), Margin = new Thickness(0, 8, 0, 8) });

            // ===== MÁY MÓC =====
            ChiTietCongTacPanel.Children.Add(MakeSectionHeader("Máy móc/Thiết bị", () => MoCuaSoMayMoc(ct)));

            if (ct.DanhSachMayMoc.Count > 0)
            {
                foreach (var mm in ct.DanhSachMayMoc)
                {
                    Grid gMM = new Grid { Margin = new Thickness(8, 2, 0, 2) };
                    gMM.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    gMM.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                    gMM.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

                    TextBlock tbTen = new TextBlock { Text = mm.TenMay, FontSize = 11 };
                    Grid.SetColumn(tbTen, 0); gMM.Children.Add(tbTen);

                    TextBlock tbSL = new TextBlock { Text = $"{mm.SoLuong} chiếc", FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center };
                    Grid.SetColumn(tbSL, 1); gMM.Children.Add(tbSL);

                    TextBlock tbCa = new TextBlock { Text = $"{mm.CaMay} ca", FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center };
                    Grid.SetColumn(tbCa, 2); gMM.Children.Add(tbCa);

                    ChiTietCongTacPanel.Children.Add(gMM);
                }
            }

            ChiTietCongTacPanel.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(235, 235, 235)), Margin = new Thickness(0, 8, 0, 8) });

            // ===== NHÂN LỰC =====
            ChiTietCongTacPanel.Children.Add(MakeSectionHeader("Nhân lực", () => MoCuaSoNhanLuc(ct)));

            if (ct.NhanLuc.SoLuongNhanCong > 0)
            {
                ChiTietCongTacPanel.Children.Add(MakeInfoLine($"   Số lượng: {ct.NhanLuc.SoLuongNhanCong} người", 11));
                if (!string.IsNullOrEmpty(ct.NhanLuc.KyThuat))
                    ChiTietCongTacPanel.Children.Add(MakeInfoLine($"   Kỹ thuật: {ct.NhanLuc.KyThuat}", 11));
                ChiTietCongTacPanel.Children.Add(MakeInfoLine($"   An toàn LĐ: {(ct.NhanLuc.AnToanLaoDong ? "✓ Có" : "✗ Không")}", 11));
                ChiTietCongTacPanel.Children.Add(MakeInfoLine($"   An toàn VSMT: {(ct.NhanLuc.AnToanVeSinhMoiTruong ? "✓ Có" : "✗ Không")}", 11));
            }
        }

        // ===== POPUP HANDLERS =====

        private void MoCuaSoVatLieu(CongTacItem ct)
        {
            var popup = new VatLieuPopupWindow(NguonVatTu, ct.DanhSachVatLieu)
            {
                Owner = Window.GetWindow(this)
            };
            if (popup.ShowDialog() == true)
            {
                ct.DanhSachVatLieu = popup.KetQua;
                RenderBangCongTac();
                RenderChiTiet();
                VeBieuDo();
            }
        }

        private void MoCuaSoMayMoc(CongTacItem ct)
        {
            var popup = new MayMocPopupWindow(NguonMayMoc, ct.DanhSachMayMoc)
            {
                Owner = Window.GetWindow(this)
            };
            if (popup.ShowDialog() == true)
            {
                ct.DanhSachMayMoc = popup.KetQua;
                RenderBangCongTac();
                RenderChiTiet();
                VeBieuDo();
            }
        }

        private void MoCuaSoNhanLuc(CongTacItem ct)
        {
            var popup = new NhanLucPopupWindow(ct.NhanLuc)
            {
                Owner = Window.GetWindow(this)
            };
            if (popup.ShowDialog() == true)
            {
                ct.NhanLuc = popup.KetQua;
                RenderBangCongTac();
                RenderChiTiet();
                VeBieuDo();
            }
        }

        // ===== HELPER UI =====

        private StackPanel MakeSectionHeader(string title, Action onPlusClick)
        {
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 4) };

            TextBlock dot = new TextBlock { Text = "•  ", FontSize = 13, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center };
            sp.Children.Add(dot);

            TextBlock tb = new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                TextDecorations = TextDecorations.Underline,
                VerticalAlignment = VerticalAlignment.Center
            };
            sp.Children.Add(tb);

            Button btn = new Button
            {
                Content = " [+]",
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = new SolidColorBrush(Color.FromRgb(74, 144, 217)),
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0)
            };
            btn.Click += (s, e) => onPlusClick();
            sp.Children.Add(btn);

            return sp;
        }

        private TextBlock MakeBulletTitle(string text)
        {
            return new TextBlock
            {
                Text = $"•  {text}",
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 2)
            };
        }

        private TextBlock MakeInfoLine(string text, double fontSize = 11)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 1, 0, 1)
            };
        }

        // ===== BIỂU ĐỒ TRÒN =====

        private void VeBieuDo()
        {
            if (_congTacDangChon == null)
            {
                VeBieuDoTrong(BieuDoVatLieu, ChuThichVatLieu);
                VeBieuDoTrong(BieuDoMayMoc, ChuThichMayMoc);
                VeBieuDoTrong(BieuDoNhanLuc, ChuThichNhanLuc);
                return;
            }

            // Biểu đồ vật liệu: theo tên vật liệu + khối lượng
            var dataVL = _congTacDangChon.DanhSachVatLieu
                .GroupBy(v => v.TenVatLieu)
                .Select(g => (Label: g.Key, Value: (double)g.Sum(x => x.KhoiLuong)))
                .ToList();
            VeBieuDoPie(BieuDoVatLieu, ChuThichVatLieu, dataVL);

            // Biểu đồ máy móc: theo tên máy + số lượng
            var dataMM = _congTacDangChon.DanhSachMayMoc
                .GroupBy(m => m.TenMay)
                .Select(g => (Label: g.Key, Value: (double)g.Sum(x => x.SoLuong)))
                .ToList();
            VeBieuDoPie(BieuDoMayMoc, ChuThichMayMoc, dataMM);

            // Biểu đồ nhân lực: Kỹ thuật vs Nhân công bình thường
            var nl = _congTacDangChon.NhanLuc;
            var dataNL = new List<(string Label, double Value)>();
            if (!string.IsNullOrEmpty(nl.KyThuat) && nl.SoLuongNhanCong > 0)
            {
                dataNL.Add(("Kỹ thuật", 1));
                dataNL.Add(("Nhân công", nl.SoLuongNhanCong));
            }
            else if (nl.SoLuongNhanCong > 0)
                dataNL.Add(("Nhân công", nl.SoLuongNhanCong));
            VeBieuDoPie(BieuDoNhanLuc, ChuThichNhanLuc, dataNL);
        }

        private void VeBieuDoTrong(Canvas canvas, StackPanel legend)
        {
            canvas.Children.Clear();
            legend.Children.Clear();
            Ellipse circle = new Ellipse { Width = 100, Height = 100, Fill = new SolidColorBrush(Color.FromRgb(235, 235, 235)) };
            Canvas.SetLeft(circle, 10); Canvas.SetTop(circle, 10);
            canvas.Children.Add(circle);
        }

        private void VeBieuDoPie(Canvas canvas, StackPanel legend,
            List<(string Label, double Value)> data)
        {
            canvas.Children.Clear();
            legend.Children.Clear();

            if (data == null || data.Count == 0 || data.Sum(d => d.Value) == 0)
            {
                VeBieuDoTrong(canvas, legend);
                return;
            }

            double cx = 60, cy = 60, r = 55;
            double total = data.Sum(d => d.Value);
            double angle = -Math.PI / 2; // Bắt đầu từ 12 giờ

            for (int i = 0; i < data.Count; i++)
            {
                double sweep = data[i].Value / total * 2 * Math.PI;
                Color mau = _palette[i % _palette.Count];

                if (data.Count == 1)
                {
                    // Vẽ hình tròn đầy
                    Ellipse el = new Ellipse { Width = r * 2, Height = r * 2, Fill = new SolidColorBrush(mau) };
                    Canvas.SetLeft(el, cx - r); Canvas.SetTop(el, cy - r);
                    canvas.Children.Add(el);
                }
                else
                {
                    double x1 = cx + r * Math.Cos(angle);
                    double y1 = cy + r * Math.Sin(angle);
                    double x2 = cx + r * Math.Cos(angle + sweep);
                    double y2 = cy + r * Math.Sin(angle + sweep);

                    bool largeArc = sweep > Math.PI;

                    PathFigure fig = new PathFigure { StartPoint = new Point(cx, cy), IsClosed = true };
                    fig.Segments.Add(new LineSegment(new Point(x1, y1), true));
                    fig.Segments.Add(new ArcSegment(
                        new Point(x2, y2),
                        new Size(r, r), 0,
                        largeArc,
                        SweepDirection.Clockwise, true));

                    PathGeometry pg = new PathGeometry(new[] { fig });
                    Path path = new Path { Fill = new SolidColorBrush(mau), Data = pg };
                    canvas.Children.Add(path);
                }

                // Chú thích
                StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 1, 0, 1) };
                sp.Children.Add(new Rectangle { Width = 10, Height = 10, Fill = new SolidColorBrush(mau), Margin = new Thickness(0, 0, 4, 0), VerticalAlignment = VerticalAlignment.Center });
                sp.Children.Add(new TextBlock { Text = data[i].Label, FontSize = 10, VerticalAlignment = VerticalAlignment.Center });
                legend.Children.Add(sp);

                angle += sweep;
            }
        }
    }
}