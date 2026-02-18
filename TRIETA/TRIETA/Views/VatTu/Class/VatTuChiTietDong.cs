#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRIETA.Views.VatTu.Class
{
    public class VatTuChiTietDong
    {
        // STT sẽ được tính tự động theo vị trí trong danh sách
        public string TenVatTu { get; set; } = "";
        // Đơn vị lấy từ VatTuItem cha (không lưu riêng)
        public double KhoiLuong { get; set; } = 0;
        public string GhiChu { get; set; } = "";
    }
}