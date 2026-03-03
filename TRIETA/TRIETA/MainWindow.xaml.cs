#nullable disable
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Trita.Views.NhatKyThiCong;

namespace Trita
{
    public partial class MainWindow : Window
    {
        private bool isChangingCheckbox = false;
        private string _tabHienTai = "VatTu";

        // Giữ instance để không mất dữ liệu khi đổi tab
        private NhatKyThiCongView _nhatKyView;

        public MainWindow()
        {
            InitializeComponent();
            DauVaoVatTuCheckBox.Checked += DauVaoVatTuCheckBox_Changed;
            DauVaoVatTuCheckBox.Unchecked += DauVaoVatTuCheckBox_Changed;
            BangPhanBoVatTuCheckBox.Checked += BangPhanBoVatTuCheckBox_Changed;
            BangPhanBoVatTuCheckBox.Unchecked += BangPhanBoVatTuCheckBox_Changed;
        }

        // ================= WINDOW LOADED =================

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DauVaoVatTuCheckBox.IsChecked = true;
            ChuyenTab("VatTu");
        }

        // ================= TAB NAVIGATION =================

        private void Tab_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string tag)
                ChuyenTab(tag);
        }

        private void ChuyenTab(string tab)
        {
            _tabHienTai = tab;

            // Reset tất cả tab về inactive
            SetTabStyle(TabThongTin, TabThongTinText, false);
            SetTabStyle(TabVatTu, TabVatTuText, false);
            SetTabStyle(TabNhanLuc, TabNhanLucText, false);
            SetTabStyle(TabMayMoc, TabMayMocText, false);
            SetTabStyle(TabNhatKy, TabNhatKyText, false);
            SetTabStyle(TabBienBan, TabBienBanText, false);

            // Ẩn tất cả ribbon
            RibbonVatTu.Visibility = Visibility.Collapsed;
            RibbonNhatKy.Visibility = Visibility.Collapsed;

            switch (tab)
            {
                case "VatTu":
                    SetTabStyle(TabVatTu, TabVatTuText, true);
                    RibbonVatTu.Visibility = Visibility.Visible;
                    MainContent.Content = VatTuViewControl;
                    break;

                case "NhatKy":
                    SetTabStyle(TabNhatKy, TabNhatKyText, true);
                    RibbonNhatKy.Visibility = Visibility.Visible;
                    MoCuaNhatKy();
                    break;

                case "ThongTin":
                    SetTabStyle(TabThongTin, TabThongTinText, true);
                    MainContent.Content = TaoPlaceholder("Thông tin dự án", "#3A7BD5");
                    break;

                case "NhanLuc":
                    SetTabStyle(TabNhanLuc, TabNhanLucText, true);
                    MainContent.Content = TaoPlaceholder("Nhân lực", "#5B8C3A");
                    break;

                case "MayMoc":
                    SetTabStyle(TabMayMoc, TabMayMocText, true);
                    MainContent.Content = TaoPlaceholder("Máy móc / Thiết bị", "#C0692A");
                    break;

                case "BienBan":
                    SetTabStyle(TabBienBan, TabBienBanText, true);
                    MainContent.Content = TaoPlaceholder("Biên bản", "#7B3A7B");
                    break;
            }
        }

        private void MoCuaNhatKy()
        {
            // Khởi tạo một lần, giữ dữ liệu khi quay lại tab
            if (_nhatKyView == null)
            {
                _nhatKyView = new NhatKyThiCongView();
            }

            // Luôn cập nhật nguồn dữ liệu mới nhất từ tab Vật tư
            _nhatKyView.NguonVatTu = VatTuViewControl?.GetDanhSachVatTu()
                                     ?? new System.Collections.Generic.List<Trita.Views.VatTu.VatTuItem>();

            // TODO: khi tab Máy móc được implement, truyền thêm:
            // _nhatKyView.NguonMayMoc = MayMocViewControl?.GetDanhSachTenMay() ?? new List<string>();

            MainContent.Content = _nhatKyView;
        }

        private void SetTabStyle(Border border, TextBlock text, bool active)
        {
            if (active)
            {
                border.Background = Brushes.White;
                border.CornerRadius = new CornerRadius(8, 8, 0, 0);
                text.FontWeight = FontWeights.Bold;
                text.FontSize = 13;
                text.Foreground = new SolidColorBrush(Color.FromRgb(184, 50, 40));
            }
            else
            {
                border.Background = new SolidColorBrush(Color.FromRgb(208, 208, 208));
                border.CornerRadius = new CornerRadius(8, 8, 0, 0);
                text.FontWeight = FontWeights.Normal;
                text.FontSize = 12;
                text.Foreground = new SolidColorBrush(Color.FromRgb(68, 68, 68));
            }
        }

        private UIElement TaoPlaceholder(string title, string hexColor)
        {
            var color = (Color)ColorConverter.ConvertFromString(hexColor);
            return new Border
            {
                Background = Brushes.White,
                Child = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new Border
                        {
                            Background    = new SolidColorBrush(color),
                            CornerRadius  = new CornerRadius(12),
                            Padding       = new Thickness(40, 16, 40,16),
                            Margin        = new Thickness(0, 0, 0, 16),
                            Child = new TextBlock
                            {
                                Text            = title,
                                FontSize        = 22,
                                FontWeight      = FontWeights.Bold,
                                Foreground      = Brushes.White,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        },
                        new TextBlock
                        {
                            Text            = "Tính năng đang được phát triển...",
                            FontSize        = 14,
                            FontStyle       = FontStyles.Italic,
                            Foreground      = Brushes.Gray,
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                    }
                }
            };
        }

        // ================= CHECKBOX (chỉ dùng trong tab VatTu) =================

        private void DauVaoVatTuCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (isChangingCheckbox) return;
            isChangingCheckbox = true;
            if (DauVaoVatTuCheckBox.IsChecked == true)
            {
                MainContent.Content = VatTuViewControl;
                BangPhanBoVatTuCheckBox.IsChecked = false;
            }
            else if (BangPhanBoVatTuCheckBox.IsChecked != true)
            {
                DauVaoVatTuCheckBox.IsChecked = true;
            }
            isChangingCheckbox = false;
        }

        private void BangPhanBoVatTuCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (isChangingCheckbox) return;
            isChangingCheckbox = true;
            if (BangPhanBoVatTuCheckBox.IsChecked == true)
            {
                var bangPhanBoView = new Views.BangPhanBo.BangPhanBoVatTuView();
                if (VatTuViewControl != null)
                    bangPhanBoView.LoadDataFromVatTuView(
                        VatTuViewControl.GetDanhSachVatTu(),
                        _nhatKyView?.GetDanhSachCongTac());
                MainContent.Content = bangPhanBoView;
                DauVaoVatTuCheckBox.IsChecked = false;
            }
            else if (DauVaoVatTuCheckBox.IsChecked != true)
            {
                DauVaoVatTuCheckBox.IsChecked = true;
            }
            isChangingCheckbox = false;
        }

        // ================= WINDOW CONTROLS =================

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => this.WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
            => this.WindowState = this.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => this.Close();

        // ================= RIBBON TOOLS =================

        private void XoaVatTu_Click(object sender, RoutedEventArgs e)
        {
            if (DauVaoVatTuCheckBox.IsChecked == true && VatTuViewControl != null)
                VatTuViewControl.XoaVatTuDangChon();
            else
                MessageBox.Show("Vui lòng chuyển sang view 'Đầu vào vật tư' để xóa vật tư!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ThemCongTacNhatKy_Click(object sender, RoutedEventArgs e)
        {
            _nhatKyView?.ThemCongTacMoi();
        }
    }
}