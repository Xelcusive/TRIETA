using System;
using System.Windows;
using System.Windows.Input;

namespace Trita
{
    public partial class MainWindow : Window
    {
        private bool isChangingCheckbox = false;

        public MainWindow()
        {
            InitializeComponent();

            // Subscribe events cho checkbox
            DauVaoVatTuCheckBox.Checked += DauVaoVatTuCheckBox_Changed;
            DauVaoVatTuCheckBox.Unchecked += DauVaoVatTuCheckBox_Changed;
            BangPhanBoVatTuCheckBox.Checked += BangPhanBoVatTuCheckBox_Changed;
            BangPhanBoVatTuCheckBox.Unchecked += BangPhanBoVatTuCheckBox_Changed;
        }

        // ================= WINDOW EVENTS =================
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DauVaoVatTuCheckBox.IsChecked = true;
        }

        // ================= CHECKBOX EVENTS =================
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
                {
                    bangPhanBoView.LoadDataFromVatTuView(VatTuViewControl.GetDanhSachVatTu());
                }

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
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ================= RIBBON TOOLS =================
        private void XoaVatTu_Click(object sender, RoutedEventArgs e)
        {
            if (DauVaoVatTuCheckBox.IsChecked == true && VatTuViewControl != null)
            {
                VatTuViewControl.XoaVatTuDangChon();
            }
            else
            {
                MessageBox.Show(
                    "Vui lòng chuyển sang view 'Đầu vào vật tư' để xóa vật tư!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}