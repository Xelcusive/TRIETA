#nullable disable
using System.Collections.Generic;
using System.Linq;

namespace Trita.Views.VatTu
{
    public class VatTuItem
    {
        public string Ten { get; set; }
        public string DonVi { get; set; }

        public double KhoiLuongTong => DanhSachLo != null
            ? DanhSachLo.Sum(lo => lo.KhoiLuong)
            : 0;

        public List<LoVatTu> DanhSachLo { get; set; }

        public VatTuItem()
        {
            DanhSachLo = new List<LoVatTu>();
        }
    }
}