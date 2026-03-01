#nullable disable
using System.Windows;

namespace Trita.Views.NhatKyThiCong
{
    public partial class NhanLucPopupWindow : Window
    {
        public NhanLucInfo KetQua { get; private set; } = new NhanLucInfo();

        public NhanLucPopupWindow(NhanLucInfo duLieuCu = null)
        {
            InitializeComponent();

            if (duLieuCu != null)
            {
                SoLuongNhanCongTextBox.Text = duLieuCu.SoLuongNhanCong > 0 ? duLieuCu.SoLuongNhanCong.ToString() : "";
                KyThuatTextBox.Text = duLieuCu.KyThuat;
                AnToanLaoDongCo.IsChecked = duLieuCu.AnToanLaoDong;
                AnToanLaoDongKhong.IsChecked = !duLieuCu.AnToanLaoDong;
                AnToanVSMTCo.IsChecked = duLieuCu.AnToanVeSinhMoiTruong;
                AnToanVSMTKhong.IsChecked = !duLieuCu.AnToanVeSinhMoiTruong;
            }
        }

        private void XacNhan_Click(object sender, RoutedEventArgs e)
        {
            KetQua = new NhanLucInfo
            {
                SoLuongNhanCong = int.TryParse(SoLuongNhanCongTextBox.Text, out int sl) ? sl : 0,
                KyThuat = KyThuatTextBox.Text,
                AnToanLaoDong = AnToanLaoDongCo.IsChecked == true,
                AnToanVeSinhMoiTruong = AnToanVSMTCo.IsChecked == true
            };
            DialogResult = true;
        }

        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}