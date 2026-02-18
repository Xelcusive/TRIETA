using System;

namespace Trita.Views.VatTu
{
    public class LoVatTu
    {
        public double KhoiLuong { get; set; }
        public DateTime NgayNhap { get; set; }
        public string DonViCungCap { get; set; }
        public DateTime? BienBanGiaoNhan { get; set; }
        public DateTime? CO { get; set; }
        public DateTime? CQ { get; set; }
        public string ThiNghiem { get; set; } // "Có" hoặc "Không"

        // Thêm các thuộc tính mới cho thí nghiệm
        public DateTime? NgayLayMau { get; set; }
        public DateTime? NgayTraKetQua { get; set; }
        public string NoiThiNghiem { get; set; }
    }
}