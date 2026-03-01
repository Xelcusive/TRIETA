#nullable disable
using System;
using System.Collections.Generic;

namespace Trita.Views.NhatKyThiCong
{
    public class CongTacItem
    {
        public string HangMuc { get; set; } = "";
        public string CongViec { get; set; } = "";
        public string LyTrinh { get; set; } = "";
        public string DonVi { get; set; } = "";
        public double KhoiLuong { get; set; } = 0;
        public double TienDo { get; set; } = 0; // %

        public List<VatLieuSuDung> DanhSachVatLieu { get; set; }
        public List<MayMocSuDung> DanhSachMayMoc { get; set; }
        public NhanLucInfo NhanLuc { get; set; }

        public CongTacItem()
        {
            DanhSachVatLieu = new List<VatLieuSuDung>();
            DanhSachMayMoc = new List<MayMocSuDung>();
            NhanLuc = new NhanLucInfo();
        }

        public int TongNguoi => NhanLuc?.SoLuongNhanCong ?? 0;
        public int TongMay => DanhSachMayMoc?.Count ?? 0;
        public int TongLoaiVatLieu => DanhSachVatLieu?.Count ?? 0;
    }

    public class VatLieuSuDung
    {
        /// <summary>Tên vật liệu - lấy từ danh sách VatTu đã khai báo</summary>
        public string TenVatLieu { get; set; } = "";
        public string DonVi { get; set; } = "";
        public double KhoiLuong { get; set; } = 0;
    }

    public class MayMocSuDung
    {
        /// <summary>Tên máy - lấy từ danh sách Máy thi công đã khai báo</summary>
        public string TenMay { get; set; } = "";
        public int SoLuong { get; set; } = 1;
        public double CaMay { get; set; } = 1;
    }

    public class NhanLucInfo
    {
        public int SoLuongNhanCong { get; set; } = 0;
        public string KyThuat { get; set; } = "";
        public bool AnToanLaoDong { get; set; } = false;
        public bool AnToanVeSinhMoiTruong { get; set; } = false;
    }

    public class NhatKyThiCongData
    {
        public string DuAn { get; set; } = "";
        public string DiaDiem { get; set; } = "";
        public DateTime Ngay { get; set; } = DateTime.Today;
        public string ThoiTietSang { get; set; } = "";
        public string ThoiTietChieu { get; set; } = "";
        public List<CongTacItem> DanhSachCongTac { get; set; }

        public NhatKyThiCongData()
        {
            DanhSachCongTac = new List<CongTacItem>();
        }
    }
}