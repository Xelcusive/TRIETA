#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TRIETA.Views.VatTu.Class;
using Trita.Enums;

namespace Trita.Views.VatTu
{
    public partial class VatTuView : UserControl
    {
        private List<VatTuItem> danhSachVatTu = new List<VatTuItem>();
        private VatTuItem vatTuDangChon;
        private LoVatTu loDangChon;
        private List<string> danhSachDonVi = new List<string> { "Bao", "kg", "Tấn", "m³", "m²", "m", "Cái", "Bộ" };
        private bool loHienThi = true;

        private static readonly List<Color> MauBieuDo = new List<Color>
        {
            Color.FromRgb(74, 144, 217),
            Color.FromRgb(92, 184, 92),
            Color.FromRgb(240, 173, 78),
            Color.FromRgb(217, 83, 79),
            Color.FromRgb(153, 102, 204),
            Color.FromRgb(23, 162, 184),
            Color.FromRgb(255, 127, 80),
            Color.FromRgb(60, 179, 113)
        };

        public VatTuView()
        {
            InitializeComponent();
            KhoiTaoDuLieuMau();
        }

        private void KhoiTaoDuLieuMau()
        {
            VatTuItem vt1 = new VatTuItem { Ten = "Vật tư 1", DonVi = "" };
            vt1.DanhSachLo.Add(new LoVatTu { KhoiLuong = 0, NgayNhap = DateTime.Now, DonViCungCap = "", ThiNghiem = "Không" });
            vt1.DanhSachLo.Add(new LoVatTu { KhoiLuong = 0, NgayNhap = DateTime.Now, DonViCungCap = "", ThiNghiem = "Không" });

            VatTuItem vt2 = new VatTuItem { Ten = "Vật tư 2", DonVi = "" };
            vt2.DanhSachLo.Add(new LoVatTu { KhoiLuong = 0, NgayNhap = DateTime.Now, DonViCungCap = "", ThiNghiem = "Không" });

            danhSachVatTu.AddRange(new[] { vt1, vt2,
                new VatTuItem { Ten = "Vật tư 3", DonVi = "" },
                new VatTuItem { Ten = "Vật tư 4", DonVi = "" },
                new VatTuItem { Ten = "Vật tư 5", DonVi = "" }
            });
            HienThiDanhSachVatTu();
        }

        private void AnHienLo_Click(object sender, RoutedEventArgs e)
        {
            loHienThi = !loHienThi;
            BtnAnHienLo.Content = loHienThi ? "[Ẩn lô]" : "[Hiện lô]";
            HienThiDanhSachVatTu();
        }

        private void HienThiDanhSachVatTu()
        {
            DanhSachVatTuPanel.Children.Clear();

            for (int i = 0; i < danhSachVatTu.Count; i++)
            {
                VatTuItem vt = danhSachVatTu[i];
                StackPanel vtContainer = new StackPanel { Margin = new Thickness(0, 4, 0, 8) };

                Grid mainGrid = new Grid();
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(35) });
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                TextBlock soThuTu = new TextBlock
                {
                    Text = $"{i + 1}.",
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                Grid.SetColumn(soThuTu, 0);

                TextBox txtTenVatTu = new TextBox
                {
                    Text = $"[{vt.Ten}]",
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    FontWeight = FontWeights.Bold,
                    Padding = new Thickness(0),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    IsReadOnly = true,
                    Tag = vt
                };

                txtTenVatTu.PreviewMouseDown += (s, ev) =>
                {
                    if (!txtTenVatTu.IsFocused && ev.ClickCount == 1)
                    {
                        vatTuDangChon = vt;
                        loDangChon = null;
                        HienThiFormTongQuanVatTu();
                        ev.Handled = true;
                    }
                };
                txtTenVatTu.MouseDoubleClick += (s, ev) =>
                {
                    txtTenVatTu.IsReadOnly = false;
                    txtTenVatTu.Focus();
                    txtTenVatTu.SelectAll();
                };
                txtTenVatTu.LostFocus += (s, ev) =>
                {
                    txtTenVatTu.IsReadOnly = true;
                    string newText = txtTenVatTu.Text.Trim('[', ']');
                    if (!string.IsNullOrWhiteSpace(newText))
                    {
                        vt.Ten = newText;
                        txtTenVatTu.Text = $"[{newText}]";
                        if (vatTuDangChon == vt)
                        {
                            if (loDangChon == null)
                                TitleTextBlock.Text = vt.Ten.ToUpper();
                            else
                            {
                                int loThu = vatTuDangChon.DanhSachLo.IndexOf(loDangChon) + 1;
                                TitleTextBlock.Text = $"{vt.Ten.ToUpper()}-LÔ {loThu}";
                            }
                        }
                    }
                    else txtTenVatTu.Text = $"[{vt.Ten}]";
                };
                Grid.SetColumn(txtTenVatTu, 1);

                Button btnThemLo = new Button
                {
                    Content = "[+]",
                    Width = 25,
                    Height = 20,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.Blue,
                    FontWeight = FontWeights.Bold,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                btnThemLo.Click += (s, ev) => ThemLoVaoVatTu(vt);
                Grid.SetColumn(btnThemLo, 2);

                mainGrid.Children.Add(soThuTu);
                mainGrid.Children.Add(txtTenVatTu);
                mainGrid.Children.Add(btnThemLo);
                vtContainer.Children.Add(mainGrid);

                if (loHienThi)
                {
                    for (int j = 0; j < vt.DanhSachLo.Count; j++)
                    {
                        LoVatTu lo = vt.DanhSachLo[j];
                        StackPanel loPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 0) };

                        Button btnLo = new Button
                        {
                            Content = $"     •   Lô thứ {j + 1}",
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                            Cursor = System.Windows.Input.Cursors.Hand,
                            Padding = new Thickness(0, 2, 0, 2),
                            HorizontalAlignment = HorizontalAlignment.Left
                        };
                        btnLo.Click += (s, ev) => { vatTuDangChon = vt; loDangChon = lo; HienThiFormChiTietLo(); };
                        loPanel.Children.Add(btnLo);

                        Button btnXoaLo = new Button
                        {
                            Content = "[-]",
                            Width = 25,
                            Height = 20,
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                            Foreground = Brushes.Red,
                            FontWeight = FontWeights.Bold,
                            Cursor = System.Windows.Input.Cursors.Hand,
                            Margin = new Thickness(5, 0, 0, 0)
                        };
                        btnXoaLo.Click += (s, ev) => XoaLo(vt, lo);
                        loPanel.Children.Add(btnXoaLo);
                        vtContainer.Children.Add(loPanel);
                    }
                }

                if (i < danhSachVatTu.Count - 1)
                    vtContainer.Children.Add(new TextBlock { Text = "....  ................", Foreground = Brushes.Gray, Margin = new Thickness(0, 4, 0, 0) });

                DanhSachVatTuPanel.Children.Add(vtContainer);
            }
        }

        private void HienThiFormTongQuanVatTu()
        {
            if (vatTuDangChon == null) return;

            TitleTextBlock.Text = vatTuDangChon.Ten.ToUpper();
            FormTongQuanVatTu.Visibility = Visibility.Visible;
            FormChiTietLo.Visibility = Visibility.Collapsed;
            PanelBieuDo.Visibility = Visibility.Visible;
            PanelChungChi.Visibility = Visibility.Collapsed;

            DonViComboBox.Items.Clear();
            foreach (var dv in danhSachDonVi) DonViComboBox.Items.Add(dv);

            if (!string.IsNullOrEmpty(vatTuDangChon.DonVi))
                DonViComboBox.SelectedItem = vatTuDangChon.DonVi;
            else
                DonViComboBox.SelectedIndex = -1;

            TongSoLoTextBox.Text = vatTuDangChon.DanhSachLo.Count.ToString();
            TongKhoiLuongTextBox.Text = vatTuDangChon.DanhSachLo.Sum(lo => lo.KhoiLuong).ToString();

            HienThiDanhSachDonViCungCap();
            HienThiCheckListVatTu();
            VeBieuDoBars();
        }

        private void VeBieuDoBars()
        {
            BieuDoCanvas.Children.Clear();
            ChuThichPanel.Children.Clear();

            if (vatTuDangChon == null || vatTuDangChon.DanhSachLo.Count == 0)
            {
                ChuThichPanel.Children.Add(new TextBlock { Text = "Chưa có dữ liệu lô", FontStyle = FontStyles.Italic, Foreground = Brushes.Gray, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) });
                return;
            }

            var danhSachLo = vatTuDangChon.DanhSachLo;
            double tongKhoiLuong = danhSachLo.Sum(lo => lo.KhoiLuong);
            double canvasW = 300, canvasH = 200;
            BieuDoCanvas.Width = canvasW; BieuDoCanvas.Height = canvasH;
            double paddingLeft = 40, paddingBottom = 40, paddingTop = 10, paddingRight = 10;
            double chartW = canvasW - paddingLeft - paddingRight;
            double chartH = canvasH - paddingBottom - paddingTop;
            int n = danhSachLo.Count;
            double barW = Math.Max(20, (chartW / n) - 6);
            double maxVal = danhSachLo.Max(lo => lo.KhoiLuong);
            if (maxVal <= 0) maxVal = 1;

            for (int g = 0; g <= 4; g++)
            {
                double y = paddingTop + chartH - (chartH * g / 4);
                BieuDoCanvas.Children.Add(new Line { X1 = paddingLeft, Y1 = y, X2 = paddingLeft + chartW, Y2 = y, Stroke = g == 0 ? Brushes.Black : new SolidColorBrush(Color.FromRgb(220, 220, 220)), StrokeThickness = g == 0 ? 1.5 : 0.5 });
                if (g > 0)
                {
                    double val = maxVal * g / 4;
                    TextBlock lbl = new TextBlock { Text = val % 1 == 0 ? ((int)val).ToString() : val.ToString("F1"), FontSize = 9, Foreground = Brushes.Gray };
                    Canvas.SetLeft(lbl, 2); Canvas.SetTop(lbl, y - 8);
                    BieuDoCanvas.Children.Add(lbl);
                }
            }
            BieuDoCanvas.Children.Add(new Line { X1 = paddingLeft, Y1 = paddingTop + chartH, X2 = paddingLeft + chartW, Y2 = paddingTop + chartH, Stroke = Brushes.Black, StrokeThickness = 1.5 });

            for (int i = 0; i < n; i++)
            {
                LoVatTu lo = danhSachLo[i];
                Color mau = MauBieuDo[i % MauBieuDo.Count];
                double barHeight = chartH * (lo.KhoiLuong / maxVal);
                double x = paddingLeft + i * (chartW / n) + (chartW / n - barW) / 2;
                double y = paddingTop + chartH - barHeight;

                Rectangle bar = new Rectangle { Width = barW, Height = Math.Max(barHeight, 0), Fill = new SolidColorBrush(mau), RadiusX = 3, RadiusY = 3, ToolTip = $"Lô {i + 1}: {lo.KhoiLuong} {vatTuDangChon.DonVi}\nNhà CC: {(string.IsNullOrEmpty(lo.DonViCungCap) ? "(chưa có)" : lo.DonViCungCap)}" };
                Canvas.SetLeft(bar, x); Canvas.SetTop(bar, y);
                BieuDoCanvas.Children.Add(bar);

                if (lo.KhoiLuong > 0)
                {
                    TextBlock valLbl = new TextBlock { Text = lo.KhoiLuong % 1 == 0 ? ((int)lo.KhoiLuong).ToString() : lo.KhoiLuong.ToString("F1"), FontSize = 9, Foreground = new SolidColorBrush(mau), FontWeight = FontWeights.Bold };
                    Canvas.SetLeft(valLbl, x + barW / 2 - 8); Canvas.SetTop(valLbl, y - 14);
                    BieuDoCanvas.Children.Add(valLbl);
                }

                TextBlock loLbl = new TextBlock { Text = $"Lô {i + 1}", FontSize = 9, Foreground = Brushes.DarkGray };
                Canvas.SetLeft(loLbl, x + barW / 2 - 12); Canvas.SetTop(loLbl, paddingTop + chartH + 5);
                BieuDoCanvas.Children.Add(loLbl);

                StackPanel legend = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 3, 8, 3) };
                legend.Children.Add(new Rectangle { Width = 12, Height = 12, Fill = new SolidColorBrush(mau), Margin = new Thickness(0, 0, 5, 0), VerticalAlignment = VerticalAlignment.Center });
                string nhaCungCap = string.IsNullOrEmpty(lo.DonViCungCap) ? "(chưa có)" : lo.DonViCungCap;
                legend.Children.Add(new TextBlock { Text = $"Lô {i + 1}: {nhaCungCap} — {lo.KhoiLuong} {vatTuDangChon.DonVi}", FontSize = 10, VerticalAlignment = VerticalAlignment.Center });
                ChuThichPanel.Children.Add(legend);
            }

            if (tongKhoiLuong > 0)
                ChuThichPanel.Children.Add(new TextBlock { Text = $"Tổng: {tongKhoiLuong} {vatTuDangChon.DonVi}", FontWeight = FontWeights.Bold, FontSize = 11, Margin = new Thickness(0, 6, 0, 0) });
        }

        private void HienThiDanhSachDonViCungCap()
        {
            DanhSachDonViCungCapPanel.Children.Clear();
            if (vatTuDangChon == null) return;

            for (int i = 0; i < vatTuDangChon.DanhSachLo.Count; i++)
            {
                LoVatTu lo = vatTuDangChon.DanhSachLo[i];
                Grid grid = new Grid { Margin = new Thickness(0, 4, 0, 4) };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Border labelBorder = new Border { Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)), BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(15), MinHeight = 30, Child = new TextBlock { Text = $"Lô thứ {i + 1}", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold } };
                Grid.SetColumn(labelBorder, 0);

                Border valueBorder = new Border { Background = Brushes.White, BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(15), Margin = new Thickness(10, 0, 0, 0), Padding = new Thickness(12, 5, 12, 5), MinHeight = 30 };
                TextBox valueTextBox = new TextBox { Text = lo.DonViCungCap, VerticalAlignment = VerticalAlignment.Center, Background = Brushes.Transparent, BorderThickness = new Thickness(0), TextWrapping = TextWrapping.Wrap, AcceptsReturn = true };
                valueTextBox.TextChanged += (s, ev) => { lo.DonViCungCap = valueTextBox.Text; valueBorder.Height = double.NaN; labelBorder.Height = double.NaN; VeBieuDoBars(); };
                valueBorder.Child = valueTextBox;
                Grid.SetColumn(valueBorder, 1);

                grid.Children.Add(labelBorder);
                grid.Children.Add(valueBorder);
                DanhSachDonViCungCapPanel.Children.Add(grid);
            }
        }

        private void HienThiCheckListVatTu()
        {
            CheckListPanel.Children.Clear();
            if (vatTuDangChon == null) return;

            for (int i = 0; i < vatTuDangChon.DanhSachLo.Count; i++)
            {
                LoVatTu lo = vatTuDangChon.DanhSachLo[i];
                List<string> thieu = KiemTraHoSoThieu(lo);
                if (thieu.Count > 0)
                {
                    CheckListPanel.Children.Add(new TextBlock { Text = $"▶ Lô {i + 1}:", FontWeight = FontWeights.Bold, Foreground = Brushes.Red, Margin = new Thickness(0, 8, 0, 4) });
                    foreach (var item in thieu)
                        CheckListPanel.Children.Add(new TextBlock { Text = $"   • Thiếu {item}", Foreground = Brushes.DarkRed, Margin = new Thickness(0, 2, 0, 2), FontSize = 11 });
                }
                else
                    CheckListPanel.Children.Add(new TextBlock { Text = $"▶ Lô {i + 1}: Đầy đủ hồ sơ ✓", FontWeight = FontWeights.Bold, Foreground = Brushes.Green, Margin = new Thickness(0, 8, 0, 4) });
            }

            if (CheckListPanel.Children.Count == 0)
                CheckListPanel.Children.Add(new TextBlock { Text = "Chưa có lô hàng nào", FontStyle = FontStyles.Italic, Foreground = Brushes.Gray });
        }

        private void HienThiFormChiTietLo()
        {
            if (vatTuDangChon == null || loDangChon == null) return;

            int loThu = vatTuDangChon.DanhSachLo.IndexOf(loDangChon) + 1;
            TitleTextBlock.Text = $"{vatTuDangChon.Ten.ToUpper()}-LÔ {loThu}";
            FormTongQuanVatTu.Visibility = Visibility.Collapsed;
            FormChiTietLo.Visibility = Visibility.Visible;
            PanelBieuDo.Visibility = Visibility.Collapsed;
            PanelChungChi.Visibility = Visibility.Visible;

            DonViLoTextBox.Text = vatTuDangChon.DonVi;
            CapNhatTongKhoiLuongLo();
            NgayNhapHangPicker.SelectedDate = loDangChon.NgayNhap;
            DonViCungCapTextBox.Text = loDangChon.DonViCungCap;
            BienBanGiaoHangPicker.SelectedDate = loDangChon.BienBanGiaoNhan;
            COPicker.SelectedDate = loDangChon.CO;
            CQPicker.SelectedDate = loDangChon.CQ;
            ThiNghiemComboBox.SelectedIndex = loDangChon.ThiNghiem == "Có" ? 1 : 0;
            ThiNghiemDetailsPanel.Visibility = loDangChon.ThiNghiem == "Có" ? Visibility.Visible : Visibility.Collapsed;
            if (loDangChon.ThiNghiem == "Có")
            {
                NgayLayMauPicker.SelectedDate = loDangChon.NgayLayMau;
                NgayTraKetQuaPicker.SelectedDate = loDangChon.NgayTraKetQua;
                NoiThiNghiemTextBox.Text = loDangChon.NoiThiNghiem;
            }

            KhoiLuongTextBox.TextChanged -= KhoiLuongTextBox_TextChanged;
            DonViCungCapTextBox.TextChanged -= DonViCungCapTextBox_TextChanged;
            NgayNhapHangPicker.SelectedDateChanged -= NgayNhapHangPicker_SelectedDateChanged;
            BienBanGiaoHangPicker.SelectedDateChanged -= BienBanGiaoHangPicker_SelectedDateChanged;
            COPicker.SelectedDateChanged -= COPicker_SelectedDateChanged;
            CQPicker.SelectedDateChanged -= CQPicker_SelectedDateChanged;
            NgayLayMauPicker.SelectedDateChanged -= NgayLayMauPicker_SelectedDateChanged;
            NgayTraKetQuaPicker.SelectedDateChanged -= NgayTraKetQuaPicker_SelectedDateChanged;
            NoiThiNghiemTextBox.TextChanged -= NoiThiNghiemTextBox_TextChanged;

            DonViCungCapTextBox.TextChanged += DonViCungCapTextBox_TextChanged;
            NgayNhapHangPicker.SelectedDateChanged += NgayNhapHangPicker_SelectedDateChanged;
            BienBanGiaoHangPicker.SelectedDateChanged += BienBanGiaoHangPicker_SelectedDateChanged;
            COPicker.SelectedDateChanged += COPicker_SelectedDateChanged;
            CQPicker.SelectedDateChanged += CQPicker_SelectedDateChanged;
            NgayLayMauPicker.SelectedDateChanged += NgayLayMauPicker_SelectedDateChanged;
            NgayTraKetQuaPicker.SelectedDateChanged += NgayTraKetQuaPicker_SelectedDateChanged;
            NoiThiNghiemTextBox.TextChanged += NoiThiNghiemTextBox_TextChanged;

            HienThiBangVatTuLo();
            HienThiCheckListLo();
        }

        // ===== BẢNG VẬT TƯ TRONG LÔ =====

        private void HienThiBangVatTuLo()
        {
            BangVatTuPanel.Children.Clear();
            if (loDangChon == null) return;
            string donVi = vatTuDangChon?.DonVi ?? "";
            for (int i = 0; i < loDangChon.DanhSachVatTuChiTiet.Count; i++)
                BangVatTuPanel.Children.Add(TaoDongBang(i, loDangChon.DanhSachVatTuChiTiet[i], donVi));
            CapNhatTongKhoiLuongLo();
        }

        private Border TaoDongBang(int idx, VatTuChiTietDong dong, string donVi)
        {
            Border rowBorder = new Border
            {
                Background = idx % 2 == 0 ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(247, 250, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 228, 240)),
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
            Grid row = new Grid { Margin = new Thickness(0, 4, 0, 4) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });

            TextBlock stt = new TextBlock { Text = (idx + 1).ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 11 };
            Grid.SetColumn(stt, 0); row.Children.Add(stt);

            TextBox txtTen = new TextBox { Text = dong.TenVatTu, Background = Brushes.Transparent, BorderThickness = new Thickness(0), Padding = new Thickness(4, 0, 4, 0), VerticalContentAlignment = VerticalAlignment.Center, FontSize = 11 };
            txtTen.TextChanged += (s, ev) => dong.TenVatTu = txtTen.Text;
            Grid.SetColumn(txtTen, 1); row.Children.Add(txtTen);

            TextBlock txtDonVi = new TextBlock { Text = donVi, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontStyle = FontStyles.Italic, Foreground = Brushes.DimGray, FontSize = 11 };
            Grid.SetColumn(txtDonVi, 2); row.Children.Add(txtDonVi);

            TextBox txtKL = new TextBox { Text = dong.KhoiLuong > 0 ? dong.KhoiLuong.ToString() : "", Background = Brushes.Transparent, BorderThickness = new Thickness(0), Padding = new Thickness(4, 0, 4, 0), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 11 };
            txtKL.TextChanged += (s, ev) => { dong.KhoiLuong = double.TryParse(txtKL.Text, out double kl) ? kl : 0; CapNhatTongKhoiLuongLo(); };
            Grid.SetColumn(txtKL, 3); row.Children.Add(txtKL);

            TextBox txtGhiChu = new TextBox { Text = dong.GhiChu, Background = Brushes.Transparent, BorderThickness = new Thickness(0), Padding = new Thickness(4, 0, 4, 0), VerticalContentAlignment = VerticalAlignment.Center, FontSize = 11 };
            txtGhiChu.TextChanged += (s, ev) => dong.GhiChu = txtGhiChu.Text;
            Grid.SetColumn(txtGhiChu, 4); row.Children.Add(txtGhiChu);

            Button btnXoa = new Button { Content = "×", Width = 20, Height = 20, Background = Brushes.Transparent, BorderThickness = new Thickness(0), Foreground = Brushes.Red, FontWeight = FontWeights.Bold, Cursor = System.Windows.Input.Cursors.Hand, FontSize = 13, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            btnXoa.Click += (s, ev) => { loDangChon.DanhSachVatTuChiTiet.Remove(dong); HienThiBangVatTuLo(); };
            Grid.SetColumn(btnXoa, 5); row.Children.Add(btnXoa);

            rowBorder.Child = row;
            return rowBorder;
        }

        private void CapNhatTongKhoiLuongLo()
        {
            if (loDangChon == null) return;
            double tong = loDangChon.DanhSachVatTuChiTiet.Sum(d => d.KhoiLuong);
            loDangChon.KhoiLuong = tong;
            KhoiLuongTextBox.Text = tong.ToString();
            TongKhoiLuongLoTextBlock.Text = tong.ToString();
        }

        private void ThemDongVatTu_Click(object sender, RoutedEventArgs e)
        {
            if (loDangChon == null) return;
            loDangChon.DanhSachVatTuChiTiet.Add(new VatTuChiTietDong());
            HienThiBangVatTuLo();
        }

        // ===== CHECK LIST =====

        private void HienThiCheckListLo()
        {
            CheckListPanel.Children.Clear();
            if (loDangChon == null) return;
            List<string> thieu = KiemTraHoSoThieu(loDangChon);
            if (thieu.Count > 0)
            {
                CheckListPanel.Children.Add(new TextBlock { Text = "Hồ sơ còn thiếu:", FontWeight = FontWeights.Bold, Foreground = Brushes.Red, Margin = new Thickness(0, 0, 0, 8) });
                foreach (var item in thieu)
                {
                    StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 3, 0, 3) };
                    sp.Children.Add(new TextBlock { Text = "☐", Foreground = Brushes.Red, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 8, 0) });
                    sp.Children.Add(new TextBlock { Text = item, Foreground = Brushes.DarkRed });
                    CheckListPanel.Children.Add(sp);
                }
            }
            else
                CheckListPanel.Children.Add(new TextBlock { Text = "✓ Đã đủ hồ sơ", FontWeight = FontWeights.Bold, Foreground = Brushes.Green, FontSize = 14 });
        }

        private List<string> KiemTraHoSoThieu(LoVatTu lo)
        {
            List<string> thieu = new List<string>();
            if (!lo.BienBanGiaoNhan.HasValue) thieu.Add("Biên bản giao nhận");
            if (!lo.CO.HasValue) thieu.Add("Chứng chỉ CO");
            if (!lo.CQ.HasValue) thieu.Add("Chứng chỉ CQ");
            if (lo.ThiNghiem == "Có")
            {
                if (!lo.NgayLayMau.HasValue) thieu.Add("Biên bản ngày lấy mẫu");
                if (!lo.NgayTraKetQua.HasValue) thieu.Add("Phiếu kết quả thí nghiệm");
            }
            return thieu;
        }

        // ===== EVENT HANDLERS =====

        private void DonViComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vatTuDangChon != null && DonViComboBox.SelectedItem != null)
                vatTuDangChon.DonVi = DonViComboBox.SelectedItem.ToString();
        }

        private void KhoiLuongTextBox_TextChanged(object sender, TextChangedEventArgs e) { }

        private void DonViCungCapTextBox_TextChanged(object sender, TextChangedEventArgs e)
        { if (loDangChon != null) loDangChon.DonViCungCap = DonViCungCapTextBox.Text; }

        private void NgayNhapHangPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        { if (loDangChon != null) loDangChon.NgayNhap = NgayNhapHangPicker.SelectedDate ?? DateTime.Now; }

        private void BienBanGiaoHangPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        { if (loDangChon != null) { loDangChon.BienBanGiaoNhan = BienBanGiaoHangPicker.SelectedDate; HienThiCheckListLo(); } }

        private void COPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        { if (loDangChon != null) { loDangChon.CO = COPicker.SelectedDate; HienThiCheckListLo(); } }

        private void CQPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        { if (loDangChon != null) { loDangChon.CQ = CQPicker.SelectedDate; HienThiCheckListLo(); } }

        private void NgayLayMauPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        { if (loDangChon != null) { loDangChon.NgayLayMau = NgayLayMauPicker.SelectedDate; HienThiCheckListLo(); } }

        private void NgayTraKetQuaPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        { if (loDangChon != null) { loDangChon.NgayTraKetQua = NgayTraKetQuaPicker.SelectedDate; HienThiCheckListLo(); } }

        private void NoiThiNghiemTextBox_TextChanged(object sender, TextChangedEventArgs e)
        { if (loDangChon != null) loDangChon.NoiThiNghiem = NoiThiNghiemTextBox.Text; }

        private void ThiNghiemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loDangChon != null && ThiNghiemComboBox.SelectedIndex >= 0)
            {
                loDangChon.ThiNghiem = ThiNghiemComboBox.SelectedIndex == 1 ? "Có" : "Không";
                ThiNghiemDetailsPanel.Visibility = loDangChon.ThiNghiem == "Có" ? Visibility.Visible : Visibility.Collapsed;
                HienThiCheckListLo();
            }
        }

        // ===== THÊM / XÓA =====

        private void ThemLoVaoVatTu(VatTuItem vt)
        {
            vt.DanhSachLo.Add(new LoVatTu { KhoiLuong = 0, NgayNhap = DateTime.Now, DonViCungCap = "", ThiNghiem = "Không" });
            HienThiDanhSachVatTu();
        }

        private void ThemVatTu_Click(object sender, RoutedEventArgs e)
        {
            danhSachVatTu.Add(new VatTuItem { Ten = $"Vật tư {danhSachVatTu.Count + 1}", DonVi = "" });
            HienThiDanhSachVatTu();
        }

        private void ThemDonViMoi_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomUnitWindow("Định nghĩa đơn vị", "Nhập đơn vị mới:");
            if (dialog.ShowDialog() == true)
            {
                string donViMoi = dialog.UnitName;
                if (!danhSachDonVi.Contains(donViMoi)) { danhSachDonVi.Add(donViMoi); DonViComboBox.Items.Add(donViMoi); }
                DonViComboBox.SelectedItem = donViMoi;
            }
        }

        private void XoaLo(VatTuItem vt, LoVatTu lo)
        {
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa lô này khỏi {vt.Ten}?", "Xác nhận xóa lô", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                vt.DanhSachLo.Remove(lo);
                HienThiDanhSachVatTu();
                if (loDangChon == lo) { FormChiTietLo.Visibility = Visibility.Collapsed; loDangChon = null; }
            }
        }

        public void XoaVatTuDangChon()
        {
            if (vatTuDangChon == null) { MessageBox.Show("Vui lòng chọn vật tư cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa {vatTuDangChon.Ten}?\nTất cả các lô của vật tư này sẽ bị xóa!", "Xác nhận xóa vật tư", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                danhSachVatTu.Remove(vatTuDangChon);
                vatTuDangChon = null; loDangChon = null;
                FormTongQuanVatTu.Visibility = Visibility.Collapsed;
                FormChiTietLo.Visibility = Visibility.Collapsed;
                PanelBieuDo.Visibility = Visibility.Collapsed;
                PanelChungChi.Visibility = Visibility.Visible;
                TitleTextBlock.Text = "VẬT TƯ";
                HienThiDanhSachVatTu();
                MessageBox.Show("Đã xóa vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ===== FILE SCAN =====

        private void ThemBienBanGiaoNhan_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => MoCuaSoScan(DocumentType.BienBanGiaoNhan);
        private void ThemCO_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => MoCuaSoScan(DocumentType.CO);
        private void ThemCQ_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => MoCuaSoScan(DocumentType.CQ);
        private void ThemKetQuaThiNghiem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => MoCuaSoScan(DocumentType.KetQuaThiNghiem);
        private void ThemHopDong_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => MoCuaSoScan(DocumentType.HopDong);

        private void MoCuaSoScan(DocumentType type)
        {
            ScanPdfWindow window = new ScanPdfWindow();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }

        public List<VatTuItem> GetDanhSachVatTu() => danhSachVatTu;
    }
}