using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
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

            VatTuItem vt3 = new VatTuItem { Ten = "Vật tư 3", DonVi = "" };
            VatTuItem vt4 = new VatTuItem { Ten = "Vật tư 4", DonVi = "" };
            VatTuItem vt5 = new VatTuItem { Ten = "Vật tư 5", DonVi = "" };

            danhSachVatTu.AddRange(new[] { vt1, vt2, vt3, vt4, vt5 });
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
                int vtIndex = i;

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

                txtTenVatTu.PreviewMouseDown += (s, e) =>
                {
                    if (!txtTenVatTu.IsFocused && e.ClickCount == 1)
                    {
                        vatTuDangChon = vt;
                        loDangChon = null;
                        HienThiFormTongQuanVatTu();
                        e.Handled = true;
                    }
                };

                txtTenVatTu.MouseDoubleClick += (s, e) =>
                {
                    txtTenVatTu.IsReadOnly = false;
                    txtTenVatTu.Focus();
                    txtTenVatTu.SelectAll();
                };

                txtTenVatTu.LostFocus += (s, e) =>
                {
                    txtTenVatTu.IsReadOnly = true;
                    string newText = txtTenVatTu.Text.Trim('[', ']');
                    if (!string.IsNullOrWhiteSpace(newText))
                    {
                        vt.Ten = newText;
                        txtTenVatTu.Text = $"[{newText}]";

                        // Cập nhật title nếu đang xem vật tư này
                        if (vatTuDangChon == vt)
                        {
                            if (loDangChon == null)
                            {
                                TitleTextBlock.Text = vt.Ten.ToUpper();
                            }
                            else
                            {
                                int loThu = vatTuDangChon.DanhSachLo.IndexOf(loDangChon) + 1;
                                TitleTextBlock.Text = $"{vt.Ten.ToUpper()}-LÔ {loThu}";
                            }
                        }
                    }
                    else
                    {
                        txtTenVatTu.Text = $"[{vt.Ten}]";
                    }
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
                btnThemLo.Click += (s, e) => ThemLoVaoVatTu(vt);
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
                        int loIndex = j;

                        StackPanel loPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 2, 0, 0)
                        };

                        Button btnLo = new Button
                        {
                            Content = $"     •   Lô thứ {j + 1}",
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                            Cursor = System.Windows.Input.Cursors.Hand,
                            Padding = new Thickness(0, 2, 0, 2),
                            HorizontalAlignment = HorizontalAlignment.Left
                        };

                        btnLo.Click += (s, e) =>
                        {
                            vatTuDangChon = vt;
                            loDangChon = lo;
                            HienThiFormChiTietLo();
                        };

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
                        btnXoaLo.Click += (s, e) => XoaLo(vt, lo);

                        loPanel.Children.Add(btnXoaLo);
                        vtContainer.Children.Add(loPanel);
                    }
                }

                if (i < danhSachVatTu.Count - 1)
                {
                    vtContainer.Children.Add(new TextBlock
                    {
                        Text = "....  ................",
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(0, 4, 0, 0)
                    });
                }

                DanhSachVatTuPanel.Children.Add(vtContainer);
            }
        }

        private void HienThiFormTongQuanVatTu()
        {
            if (vatTuDangChon == null) return;

            TitleTextBlock.Text = vatTuDangChon.Ten.ToUpper();
            FormTongQuanVatTu.Visibility = Visibility.Visible;
            FormChiTietLo.Visibility = Visibility.Collapsed;

            DonViComboBox.Items.Clear();
            foreach (var dv in danhSachDonVi)
            {
                DonViComboBox.Items.Add(dv);
            }

            if (!string.IsNullOrEmpty(vatTuDangChon.DonVi))
            {
                DonViComboBox.SelectedItem = vatTuDangChon.DonVi;
            }
            else
            {
                DonViComboBox.SelectedIndex = -1;
            }

            TongSoLoTextBox.Text = vatTuDangChon.DanhSachLo.Count.ToString();

            double tongKhoiLuong = vatTuDangChon.DanhSachLo.Sum(lo => lo.KhoiLuong);
            TongKhoiLuongTextBox.Text = tongKhoiLuong.ToString();

            HienThiDanhSachDonViCungCap();
            HienThiCheckListVatTu();
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

                Border labelBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(15),
                    MinHeight = 30
                };
                TextBlock labelText = new TextBlock
                {
                    Text = $"Lô thứ {i + 1}",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontWeight = FontWeights.Bold
                };
                labelBorder.Child = labelText;
                Grid.SetColumn(labelBorder, 0);

                Border valueBorder = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(15),
                    Margin = new Thickness(10, 0, 0, 0),
                    Padding = new Thickness(12, 5, 12, 5),
                    MinHeight = 30
                };
                TextBox valueTextBox = new TextBox
                {
                    Text = lo.DonViCungCap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    TextWrapping = TextWrapping.Wrap,
                    AcceptsReturn = true
                };
                valueTextBox.TextChanged += (s, e) =>
                {
                    lo.DonViCungCap = valueTextBox.Text;
                    // Auto resize border height
                    valueBorder.Height = double.NaN;
                    labelBorder.Height = double.NaN;
                };
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
                    TextBlock loHeader = new TextBlock
                    {
                        Text = $"▶ Lô {i + 1}:",
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.Red,
                        Margin = new Thickness(0, 8, 0, 4)
                    };
                    CheckListPanel.Children.Add(loHeader);

                    foreach (var item in thieu)
                    {
                        TextBlock itemText = new TextBlock
                        {
                            Text = $"   • Thiếu {item}",
                            Foreground = Brushes.DarkRed,
                            Margin = new Thickness(0, 2, 0, 2),
                            FontSize = 11
                        };
                        CheckListPanel.Children.Add(itemText);
                    }
                }
                else
                {
                    TextBlock loComplete = new TextBlock
                    {
                        Text = $"▶ Lô {i + 1}: Đầy đủ hồ sơ ✓",
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.Green,
                        Margin = new Thickness(0, 8, 0, 4)
                    };
                    CheckListPanel.Children.Add(loComplete);
                }
            }

            if (CheckListPanel.Children.Count == 0)
            {
                TextBlock empty = new TextBlock
                {
                    Text = "Chưa có lô hàng nào",
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray
                };
                CheckListPanel.Children.Add(empty);
            }
        }

        private void HienThiFormChiTietLo()
        {
            if (vatTuDangChon == null || loDangChon == null) return;

            int loThu = vatTuDangChon.DanhSachLo.IndexOf(loDangChon) + 1;
            TitleTextBlock.Text = $"{vatTuDangChon.Ten.ToUpper()}-LÔ {loThu}";

            FormTongQuanVatTu.Visibility = Visibility.Collapsed;
            FormChiTietLo.Visibility = Visibility.Visible;

            DonViLoTextBox.Text = vatTuDangChon.DonVi;
            KhoiLuongTextBox.Text = loDangChon.KhoiLuong.ToString();
            NgayNhapHangPicker.SelectedDate = loDangChon.NgayNhap;
            DonViCungCapTextBox.Text = loDangChon.DonViCungCap;
            BienBanGiaoHangPicker.SelectedDate = loDangChon.BienBanGiaoNhan;
            COPicker.SelectedDate = loDangChon.CO;
            CQPicker.SelectedDate = loDangChon.CQ;

            if (loDangChon.ThiNghiem == "Có")
                ThiNghiemComboBox.SelectedIndex = 1;
            else
                ThiNghiemComboBox.SelectedIndex = 0;

            ThiNghiemDetailsPanel.Visibility = loDangChon.ThiNghiem == "Có" ? Visibility.Visible : Visibility.Collapsed;

            if (loDangChon.ThiNghiem == "Có")
            {
                NgayLayMauPicker.SelectedDate = loDangChon.NgayLayMau;
                NgayTraKetQuaPicker.SelectedDate = loDangChon.NgayTraKetQua;
                NoiThiNghiemTextBox.Text = loDangChon.NoiThiNghiem;
            }

            KhoiLuongTextBox.TextChanged += KhoiLuongTextBox_TextChanged;
            DonViCungCapTextBox.TextChanged += DonViCungCapTextBox_TextChanged;
            NgayNhapHangPicker.SelectedDateChanged += NgayNhapHangPicker_SelectedDateChanged;
            BienBanGiaoHangPicker.SelectedDateChanged += BienBanGiaoHangPicker_SelectedDateChanged;
            COPicker.SelectedDateChanged += COPicker_SelectedDateChanged;
            CQPicker.SelectedDateChanged += CQPicker_SelectedDateChanged;
            NgayLayMauPicker.SelectedDateChanged += NgayLayMauPicker_SelectedDateChanged;
            NgayTraKetQuaPicker.SelectedDateChanged += NgayTraKetQuaPicker_SelectedDateChanged;
            NoiThiNghiemTextBox.TextChanged += NoiThiNghiemTextBox_TextChanged;

            HienThiCheckListLo();
        }

        private void HienThiCheckListLo()
        {
            CheckListPanel.Children.Clear();
            if (loDangChon == null) return;

            List<string> thieu = KiemTraHoSoThieu(loDangChon);

            if (thieu.Count > 0)
            {
                TextBlock header = new TextBlock
                {
                    Text = "Hồ sơ còn thiếu:",
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Red,
                    Margin = new Thickness(0, 0, 0, 8)
                };
                CheckListPanel.Children.Add(header);

                foreach (var item in thieu)
                {
                    StackPanel itemPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 3, 0, 3)
                    };

                    TextBlock checkbox = new TextBlock
                    {
                        Text = "☐",
                        Foreground = Brushes.Red,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 8, 0)
                    };

                    TextBlock itemText = new TextBlock
                    {
                        Text = item,
                        Foreground = Brushes.DarkRed
                    };

                    itemPanel.Children.Add(checkbox);
                    itemPanel.Children.Add(itemText);
                    CheckListPanel.Children.Add(itemPanel);
                }
            }
            else
            {
                TextBlock complete = new TextBlock
                {
                    Text = "✓ Đã đủ hồ sơ",
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Green,
                    FontSize = 14
                };
                CheckListPanel.Children.Add(complete);
            }
        }

        private List<string> KiemTraHoSoThieu(LoVatTu lo)
        {
            List<string> thieu = new List<string>();

            if (!lo.BienBanGiaoNhan.HasValue)
                thieu.Add("Biên bản giao nhận");

            if (!lo.CO.HasValue)
                thieu.Add("Chứng chỉ CO");

            if (!lo.CQ.HasValue)
                thieu.Add("Chứng chỉ CQ");

            if (lo.ThiNghiem == "Có")
            {
                if (!lo.NgayLayMau.HasValue)
                    thieu.Add("Biên bản ngày lấy mẫu");

                if (!lo.NgayTraKetQua.HasValue)
                    thieu.Add("Phiếu kết quả thí nghiệm");
            }

            return thieu;
        }

        private void DonViComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vatTuDangChon != null && DonViComboBox.SelectedItem != null)
            {
                vatTuDangChon.DonVi = DonViComboBox.SelectedItem.ToString();
            }
        }

        private void KhoiLuongTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loDangChon != null && double.TryParse(KhoiLuongTextBox.Text, out double kl))
                loDangChon.KhoiLuong = kl;
        }

        private void DonViCungCapTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loDangChon != null)
                loDangChon.DonViCungCap = DonViCungCapTextBox.Text;
        }

        private void NgayNhapHangPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loDangChon != null)
                loDangChon.NgayNhap = NgayNhapHangPicker.SelectedDate ?? DateTime.Now;
        }

        private void BienBanGiaoHangPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loDangChon != null)
            {
                loDangChon.BienBanGiaoNhan = BienBanGiaoHangPicker.SelectedDate;
                HienThiCheckListLo();
            }
        }

        private void COPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loDangChon != null)
            {
                loDangChon.CO = COPicker.SelectedDate;
                HienThiCheckListLo();
            }
        }

        private void CQPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loDangChon != null)
            {
                loDangChon.CQ = CQPicker.SelectedDate;
                HienThiCheckListLo();
            }
        }

        private void NgayLayMauPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loDangChon != null)
            {
                loDangChon.NgayLayMau = NgayLayMauPicker.SelectedDate;
                HienThiCheckListLo();
            }
        }

        private void NgayTraKetQuaPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loDangChon != null)
            {
                loDangChon.NgayTraKetQua = NgayTraKetQuaPicker.SelectedDate;
                HienThiCheckListLo();
            }
        }

        private void NoiThiNghiemTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loDangChon != null)
                loDangChon.NoiThiNghiem = NoiThiNghiemTextBox.Text;
        }

        private void ThiNghiemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loDangChon != null && ThiNghiemComboBox.SelectedIndex >= 0)
            {
                loDangChon.ThiNghiem = ThiNghiemComboBox.SelectedIndex == 1 ? "Có" : "Không";
                ThiNghiemDetailsPanel.Visibility = loDangChon.ThiNghiem == "Có" ? Visibility.Visible : Visibility.Collapsed;
                HienThiCheckListLo();
            }
        }

        private void ThemLoVaoVatTu(VatTuItem vt)
        {
            LoVatTu loMoi = new LoVatTu
            {
                KhoiLuong = 0,
                NgayNhap = DateTime.Now,
                DonViCungCap = "",
                ThiNghiem = "Không"
            };
            vt.DanhSachLo.Add(loMoi);
            HienThiDanhSachVatTu();
        }

        private void ThemVatTu_Click(object sender, RoutedEventArgs e)
        {
            VatTuItem vtMoi = new VatTuItem
            {
                Ten = $"Vật tư {danhSachVatTu.Count + 1}",
                DonVi = ""
            };
            danhSachVatTu.Add(vtMoi);
            HienThiDanhSachVatTu();
        }

        private void ThemDonViMoi_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomUnitWindow("Định nghĩa đơn vị", "Nhập đơn vị mới:");
            if (dialog.ShowDialog() == true)
            {
                string donViMoi = dialog.UnitName;
                if (!danhSachDonVi.Contains(donViMoi))
                {
                    danhSachDonVi.Add(donViMoi);
                    DonViComboBox.Items.Add(donViMoi);
                }
                DonViComboBox.SelectedItem = donViMoi;
            }
        }

        private void XoaLo(VatTuItem vt, LoVatTu lo)
        {
            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa lô này khỏi {vt.Ten}?",
                "Xác nhận xóa lô",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                vt.DanhSachLo.Remove(lo);
                HienThiDanhSachVatTu();

                if (loDangChon == lo)
                {
                    FormChiTietLo.Visibility = Visibility.Collapsed;
                    loDangChon = null;
                }
            }
        }

        public void XoaVatTuDangChon()
        {
            if (vatTuDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn vật tư cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {vatTuDangChon.Ten}?\nTất cả các lô của vật tư này sẽ bị xóa!",
                "Xác nhận xóa vật tư",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                danhSachVatTu.Remove(vatTuDangChon);
                vatTuDangChon = null;
                loDangChon = null;

                FormTongQuanVatTu.Visibility = Visibility.Collapsed;
                FormChiTietLo.Visibility = Visibility.Collapsed;
                TitleTextBlock.Text = "VẬT TƯ";

                HienThiDanhSachVatTu();
                MessageBox.Show("Đã xóa vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ThemBienBanGiaoNhan_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MoCuaSoScan(DocumentType.BienBanGiaoNhan);
        }

        private void ThemCO_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MoCuaSoScan(DocumentType.CO);
        }

        private void ThemCQ_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MoCuaSoScan(DocumentType.CQ);
        }

        private void ThemKetQuaThiNghiem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MoCuaSoScan(DocumentType.KetQuaThiNghiem);
        }

        private void ThemHopDong_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MoCuaSoScan(DocumentType.HopDong);
        }
        private void MoCuaSoScan(DocumentType type)
        {
            ScanPdfWindow window = new ScanPdfWindow();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
        // Method để BangPhanBoView lấy dữ liệu
        public List<VatTuItem> GetDanhSachVatTu()
        {
            return danhSachVatTu;
        }

    }
}