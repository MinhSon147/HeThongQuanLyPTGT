using System;
using System.Collections.Generic;
using System.Linq;

namespace HeThongQuanLyPTGT
{
    public abstract class PhuongTien
    {
        private string _maPT;
        private string _tenHang;
        private int _namSanXuat;
        private decimal _giaGoc;

        public string MaPT
        {
            get { return _maPT; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _maPT = "PT000";
                else
                    _maPT = value;
            }
        }

        public string TenHang
        {
            get { return _tenHang; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên hãng không được để trống!");
                _tenHang = value;
            }
        }

        public int NamSanXuat
        {
            get { return _namSanXuat; }
            set
            {
                if (value < 1900 || value > DateTime.Now.Year)
                    throw new ArgumentException("Năm sản xuất không hợp lệ!");
                _namSanXuat = value;
            }
        }

        public decimal GiaGoc
        {
            get { return _giaGoc; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Giá gốc phải lớn hơn 0!");
                _giaGoc = value;
            }
        }

        protected PhuongTien(string maPT, string tenHang, int namSanXuat, decimal giaGoc)
        {
            MaPT = maPT;
            TenHang = tenHang;
            NamSanXuat = namSanXuat;
            GiaGoc = giaGoc;
        }

        public abstract decimal TinhGiaLanBanh();

        public virtual string LayThongTin()
        {
            return MaPT + " - " + TenHang + " - " + NamSanXuat +
                   " - " + GiaGoc.ToString("N0");
        }
    }

    public class OTo : PhuongTien
    {
        private int _soChoNgoi;
        private double _dungTichDongCo;

        public int SoChoNgoi
        {
            get { return _soChoNgoi; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Số chỗ ngồi phải lớn hơn 0!");
                _soChoNgoi = value;
            }
        }

        public double DungTichDongCo
        {
            get { return _dungTichDongCo; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Dung tích động cơ phải lớn hơn 0!");
                _dungTichDongCo = value;
            }
        }

        public OTo(string maPT, string tenHang, int namSanXuat,
                   decimal giaGoc, int soChoNgoi, double dungTichDongCo)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            SoChoNgoi = soChoNgoi;
            DungTichDongCo = dungTichDongCo;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (SoChoNgoi <= 9)
                return GiaGoc + GiaGoc * 0.12m + GiaGoc * 0.30m;

            return GiaGoc + GiaGoc * 0.10m;
        }

        public override string LayThongTin()
        {
            return base.LayThongTin() + " - " + SoChoNgoi +
                   " chỗ - " + DungTichDongCo + "L";
        }
    }

    public class XeMay : PhuongTien
    {
        private int _dungTichXylanh;

        public int DungTichXylanh
        {
            get { return _dungTichXylanh; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Dung tích xylanh phải lớn hơn 0!");
                _dungTichXylanh = value;
            }
        }

        public XeMay(string maPT, string tenHang, int namSanXuat,
                     decimal giaGoc, int dungTichXylanh)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            DungTichXylanh = dungTichXylanh;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (DungTichXylanh < 175)
                return GiaGoc + GiaGoc * 0.02m;

            return GiaGoc + GiaGoc * 0.05m;
        }

        public override string LayThongTin()
        {
            return base.LayThongTin() + " - " + DungTichXylanh + "cc";
        }
    }

    public class QuanLyPhuongTien
    {
        private List<PhuongTien> danhSachSanPham = new List<PhuongTien>();

        public void ThemPhuongTien(PhuongTien pt)
        {
            danhSachSanPham.Add(pt);
        }

        public List<PhuongTien> GetDanhSach()
        {
            return danhSachSanPham;
        }

        public void HienThiTatCa()
        {
            foreach (PhuongTien pt in danhSachSanPham)
                Console.WriteLine(pt.LayThongTin() +
                    " - Giá lăn bánh: " + pt.TinhGiaLanBanh().ToString("N0"));
        }

        public PhuongTien TimGiaLanBanhLonNhat()
        {
            if (danhSachSanPham.Count == 0)
                return null;

            return danhSachSanPham.OrderByDescending(x => x.TinhGiaLanBanh()).First();
        }

        public List<PhuongTien> TimTheoTenHang(string keyword)
        {
            return danhSachSanPham.Where(x =>
                x.TenHang.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();
        }
    }
}
