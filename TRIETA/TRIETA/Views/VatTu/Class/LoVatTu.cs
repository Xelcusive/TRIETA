#nullable disable
using System;
using System.Collections.Generic;
using TRIETA.Views.VatTu.Class;

namespace Trita.Views.VatTu
{
    public class LoVatTu
    {
        /// <summary>
        /// Tổng khối lượng - tự động tính từ DanhSachVatTuChiTiet, không cho người dùng sửa trực tiếp
        /// </summary>
        public double KhoiLuong { get; set; }

        public DateTime NgayNhap { get; set; }
        public string DonViCungCap { get; set; }
        public DateTime? BienBanGiaoNhan { get; set; }
        public DateTime? CO { get; set; }
        public DateTime? CQ { get; set; }

        /// <summary>"Có" hoặc "Không"</summary>
        public string ThiNghiem { get; set; }

        public DateTime? NgayLayMau { get; set; }
        public DateTime? NgayTraKetQua { get; set; }
        public string NoiThiNghiem { get; set; }

        /// <summary>
        /// Danh sách chi tiết vật tư trong lô (bảng nhập liệu).
        /// Tổng KhoiLuong của các dòng này = KhoiLuong của lô.
        /// </summary>
        public List<VatTuChiTietDong> DanhSachVatTuChiTiet { get; set; }

        public LoVatTu()
        {
            DanhSachVatTuChiTiet = new List<VatTuChiTietDong>();
        }
    }
}