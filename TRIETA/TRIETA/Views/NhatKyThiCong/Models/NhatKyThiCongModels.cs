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
        public double TienDo { get; set; } = 0;

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
        /// <summary>Ngày sử dụng — tự điền từ NgayPicker nhật ký thi công</summary>
        public DateTime Ngay { get; set; } = DateTime.Today;
        public string NgayHienThi => Ngay.ToString("dd/MM/yyyy");
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

    /// <summary>Công việc được nghiệm thu — có thêm cột Ngày lấy từ nhật ký</summary>
    public class NghiemThuItem
    {
        public string HangMuc { get; set; } = "";
        public string TenCongViec { get; set; } = "";
        public string LyTrinh { get; set; } = "";
        public TimeSpan GioPhut { get; set; } = DateTime.Now.TimeOfDay;

        /// <summary>Ngày nghiệm thu — tự động lấy từ NgayPicker của nhật ký thi công</summary>
        public DateTime Ngay { get; set; } = DateTime.Today;

        /// <summary>Hiển thị ngày dạng dd/MM/yyyy cho bảng</summary>
        public string NgayHienThi => Ngay.ToString("dd/MM/yyyy");
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

    public static class LichSuNhatKy
    {
        public static List<NhatKyThiCongData> TatCaNgay { get; } = new List<NhatKyThiCongData>();

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