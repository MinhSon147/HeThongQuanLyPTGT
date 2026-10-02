namespace HeThongQuanLyPTGT
{
    public class DanhMucSanPham
    {
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }

        public DanhMucSanPham(int maDanhMuc, string tenDanhMuc)
        {
            MaDanhMuc = maDanhMuc;
            TenDanhMuc = tenDanhMuc;
        }
    }

    public class SanPham
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string DanhMuc { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string DuongDanAnh { get; set; }

        public SanPham(string maSanPham, string tenSanPham, string danhMuc,
                       decimal donGia, int soLuong, string duongDanAnh = "")
        {
            MaSanPham = maSanPham;
            TenSanPham = tenSanPham;
            DanhMuc = danhMuc;
            DonGia = donGia;
            SoLuong = soLuong;
            DuongDanAnh = duongDanAnh;
        }
    }
}
