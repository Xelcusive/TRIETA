using System.Collections.Generic;

namespace Trita.Views.VatTu
{
    public class VatTuItem
    {
        public string Ten { get; set; }
        public string DonVi { get; set; }
        public double KhoiLuongTong { get; set; }

        // ❗ KHÔNG DÙNG new()
        public List<LoVatTu> DanhSachLo { get; set; }

        public VatTuItem()
        {
            DanhSachLo = new List<LoVatTu>();
        }
    }
}
