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
        public string TenVatLieu { get; set; } = "";
        public string DonVi { get; set; } = "";
        public double KhoiLuong { get; set; } = 0;
    }

    public class MayMocSuDung
    {
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

    /// <summary>Công việc được nghiệm thu trong ngày</summary>
    public class NghiemThuItem
    {
        public string HangMuc { get; set; } = "";
        public string TenCongViec { get; set; } = "";
        public string LyTrinh { get; set; } = "";
        /// <summary>Giờ phút nghiệm thu (chỉ dùng phần giờ/phút)</summary>
        public TimeSpan GioPhut { get; set; } = DateTime.Now.TimeOfDay;
    }

    public class NhatKyThiCongData
    {
        public string DuAn { get; set; } = "";
        public string DiaDiem { get; set; } = "";
        public DateTime Ngay { get; set; } = DateTime.Today;
        public string ThoiTietSang { get; set; } = "";
        public string ThoiTietChieu { get; set; } = "";
        public List<CongTacItem> DanhSachCongTac { get; set; }
        public List<NghiemThuItem> DanhSachNghiemThu { get; set; }

        public NhatKyThiCongData()
        {
            DanhSachCongTac = new List<CongTacItem>();
            DanhSachNghiemThu = new List<NghiemThuItem>();
        }
    }

    /// <summary>
    /// Lưu lịch sử tất cả các ngày đã khai báo trong phiên làm việc.
    /// Dùng để lọc dữ liệu cho popup nghiệm thu.
    /// </summary>
    public static class LichSuNhatKy
    {
        public static List<NhatKyThiCongData> TatCaNgay { get; } = new List<NhatKyThiCongData>();

        /// <summary>Lấy tất cả CongTacItem đã khai báo từ trước đến ngày hiện tại</summary>
        public static List<CongTacItem> LayTatCaCongTac(DateTime? truocNgay = null)
        {
            var result = new List<CongTacItem>();
            foreach (var ngay in TatCaNgay)
            {
                if (truocNgay == null || ngay.Ngay < truocNgay.Value)
                    result.AddRange(ngay.DanhSachCongTac);
            }
            return result;
        }
    }
}