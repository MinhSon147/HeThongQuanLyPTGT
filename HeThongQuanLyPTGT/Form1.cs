using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeThongQuanLyPTGT
{
    public class Form1 : Form
    {
        TextBox txtMaSanPham;
        TextBox txtTenSanPham;
        TextBox txtDonGia;
        TextBox txtSoLuong;
        TextBox txtTimKiem;
        ComboBox cboDanhMuc;
        PictureBox picAnhDaiDien;
        DataGridView dgvSanPham;
        ErrorProvider baoLoi;
        StatusStrip thanhTrangThai;
        ToolStripStatusLabel nhanTrangThai;
        BindingList<SanPham> danhSachSanPham = new BindingList<SanPham>();
        BindingSource nguonDuLieu = new BindingSource();
        SanPham sanPhamDangChon;

        public Form1()
        {
            KhoiTaoGiaoDien();
            TaoDuLieuMau();
            HienThiDanhSach(danhSachSanPham);
        }

        void KhoiTaoGiaoDien()
        {
            Text = "Hệ thống quản lý phương tiện giao thông";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1100;
            Height = 650;
            MinimumSize = new Size(850, 500);

            baoLoi = new ErrorProvider();

            MenuStrip menu = new MenuStrip();
            ToolStripMenuItem tep = new ToolStripMenuItem("Tệp");
            ToolStripMenuItem xuat = new ToolStripMenuItem("Xuất CSV");
            xuat.ShortcutKeys = Keys.Control | Keys.E;
            xuat.Click += XuatCSV_Click;
            ToolStripMenuItem thoat = new ToolStripMenuItem("Thoát");
            thoat.ShortcutKeys = Keys.Control | Keys.X;
            thoat.Click += delegate { Close(); };
            tep.DropDownItems.Add(xuat);
            tep.DropDownItems.Add(thoat);
            menu.Items.Add(tep);
            Controls.Add(menu);
            MainMenuStrip = menu;

            TableLayoutPanel main = new TableLayoutPanel();
            main.Dock = DockStyle.Fill;
            main.ColumnCount = 2;
            main.RowCount = 1;
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            main.Padding = new Padding(8);
            Controls.Add(main);
            main.BringToFront();

            TableLayoutPanel left = new TableLayoutPanel();
            left.Dock = DockStyle.Fill;
            left.ColumnCount = 2;
            left.RowCount = 9;
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            for (int i = 0; i < 9; i++)
                left.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            AddLabel(left, "Mã sản phẩm:", 0);
            txtMaSanPham = new TextBox { Dock = DockStyle.Fill };
            left.Controls.Add(txtMaSanPham, 1, 0);

            AddLabel(left, "Tên sản phẩm:", 1);
            txtTenSanPham = new TextBox { Dock = DockStyle.Fill };
            left.Controls.Add(txtTenSanPham, 1, 1);

            AddLabel(left, "Đơn giá:", 2);
            txtDonGia = new TextBox { Dock = DockStyle.Fill };
            left.Controls.Add(txtDonGia, 1, 2);

            AddLabel(left, "Số lượng:", 3);
            txtSoLuong = new TextBox { Dock = DockStyle.Fill };
            left.Controls.Add(txtSoLuong, 1, 3);

            AddLabel(left, "Danh mục:", 4);
            cboDanhMuc = new ComboBox();
            cboDanhMuc.Dock = DockStyle.Fill;
            cboDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDanhMuc.DataSource = new[]
            {
                new DanhMucSanPham(1, "Điện thoại"),
                new DanhMucSanPham(2, "Laptop"),
                new DanhMucSanPham(3, "Phụ kiện")
            };
            cboDanhMuc.DisplayMember = "TenDanhMuc";
            cboDanhMuc.ValueMember = "MaDanhMuc";
            left.Controls.Add(cboDanhMuc, 1, 4);

            picAnhDaiDien = new PictureBox();
            picAnhDaiDien.Dock = DockStyle.Fill;
            picAnhDaiDien.Height = 150;
            picAnhDaiDien.SizeMode = PictureBoxSizeMode.Zoom;
            left.Controls.Add(picAnhDaiDien, 1, 5);

            Button nutChonAnh = new Button();
            nutChonAnh.Text = "Chọn ảnh";
            nutChonAnh.Dock = DockStyle.Fill;
            nutChonAnh.Click += ChonAnh_Click;
            left.Controls.Add(nutChonAnh, 1, 6);

            FlowLayoutPanel cumNut = new FlowLayoutPanel();
            cumNut.Dock = DockStyle.Fill;
            Button nutThem = new Button { Text = "Thêm mới", Width = 85 };
            Button nutCapNhat = new Button { Text = "Cập nhật", Width = 85 };
            Button nutXoa = new Button { Text = "Xóa", Width = 85 };
            Button nutLamMoi = new Button { Text = "Làm mới", Width = 85 };
            nutThem.Click += ThemMoi_Click;
            nutCapNhat.Click += CapNhat_Click;
            nutXoa.Click += Xoa_Click;
            nutLamMoi.Click += delegate { XoaTrang(); };
            cumNut.Controls.Add(nutThem);
            cumNut.Controls.Add(nutCapNhat);
            cumNut.Controls.Add(nutXoa);
            cumNut.Controls.Add(nutLamMoi);
            left.Controls.Add(cumNut, 1, 7);

            AddLabel(left, "Tìm kiếm:", 8);
            txtTimKiem = new TextBox { Dock = DockStyle.Fill };
            txtTimKiem.TextChanged += TxtTimKiem_TextChanged;
            left.Controls.Add(txtTimKiem, 1, 8);

            main.Controls.Add(left, 0, 0);

            TableLayoutPanel right = new TableLayoutPanel();
            right.Dock = DockStyle.Fill;
            right.RowCount = 2;
            right.ColumnCount = 1;
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Label tieuDe = new Label();
            tieuDe.Text = "DANH SÁCH SẢN PHẨM";
            tieuDe.Font = new Font("Arial", 12, FontStyle.Bold);
            tieuDe.Dock = DockStyle.Fill;
            tieuDe.Padding = new Padding(5);
            right.Controls.Add(tieuDe, 0, 0);

            dgvSanPham = new DataGridView();
            dgvSanPham.Dock = DockStyle.Fill;
            dgvSanPham.AutoGenerateColumns = false;
            dgvSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSanPham.MultiSelect = false;
            dgvSanPham.AllowUserToAddRows = false;
            dgvSanPham.ReadOnly = true;
            dgvSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSanPham.SelectionChanged += DgvSanPham_SelectionChanged;

            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Mã sản phẩm",
                DataPropertyName = "MaSanPham"
            });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "TenSanPham"
            });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Danh mục",
                DataPropertyName = "DanhMuc"
            });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Đơn giá",
                DataPropertyName = "DonGia",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong"
            });

            right.Controls.Add(dgvSanPham, 0, 1);
            main.Controls.Add(right, 1, 0);

            thanhTrangThai = new StatusStrip();
            nhanTrangThai = new ToolStripStatusLabel();
            thanhTrangThai.Items.Add(nhanTrangThai);
            Controls.Add(thanhTrangThai);
        }

        void AddLabel(TableLayoutPanel table, string text, int row)
        {
            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            table.Controls.Add(label, 0, row);
        }

        void TaoDuLieuMau()
        {
            danhSachSanPham.Add(new SanPham("SP01", "Laptop Dell", "Laptop", 25000000, 5));
            danhSachSanPham.Add(new SanPham("SP02", "iPhone 13", "Điện thoại", 15000000, 10));
            danhSachSanPham.Add(new SanPham("SP03", "Chuột Logitech", "Phụ kiện", 500000, 20));
        }

        void HienThiDanhSach(BindingList<SanPham> list)
        {
            nguonDuLieu.DataSource = list;
            dgvSanPham.DataSource = nguonDuLieu;
            nhanTrangThai.Text = "Tổng số sản phẩm: " + list.Count;
        }

        bool KiemTraDuLieu()
        {
            baoLoi.Clear();
            bool ok = true;
            decimal gia;
            int soLuong;

            if (string.IsNullOrWhiteSpace(txtTenSanPham.Text))
            {
                baoLoi.SetError(txtTenSanPham, "Tên sản phẩm không được để trống");
                ok = false;
            }

            if (!decimal.TryParse(txtDonGia.Text, out gia) || gia <= 0)
            {
                baoLoi.SetError(txtDonGia, "Đơn giá phải lớn hơn 0");
                ok = false;
            }

            if (!int.TryParse(txtSoLuong.Text, out soLuong) || soLuong < 0)
            {
                baoLoi.SetError(txtSoLuong, "Số lượng phải >= 0");
                ok = false;
            }

            return ok;
        }

        SanPham LaySanPhamTuForm()
        {
            decimal gia;
            int soLuong;
            decimal.TryParse(txtDonGia.Text, out gia);
            int.TryParse(txtSoLuong.Text, out soLuong);

            return new SanPham(
                txtMaSanPham.Text.Trim(),
                txtTenSanPham.Text.Trim(),
                cboDanhMuc.Text,
                gia,
                soLuong,
                picAnhDaiDien.Tag as string
            );
        }

        void ThemMoi_Click(object sender, EventArgs e)
        {
            if (!KiemTraDuLieu())
                return;

            danhSachSanPham.Add(LaySanPhamTuForm());
            HienThiDanhSach(danhSachSanPham);
            XoaTrang();
            MessageBox.Show("Đã thêm sản phẩm.");
        }

        void CapNhat_Click(object sender, EventArgs e)
        {
            if (sanPhamDangChon == null)
            {
                MessageBox.Show("Hãy chọn sản phẩm cần cập nhật.");
                return;
            }

            if (!KiemTraDuLieu())
                return;

            SanPham sp = LaySanPhamTuForm();
            sanPhamDangChon.MaSanPham = sp.MaSanPham;
            sanPhamDangChon.TenSanPham = sp.TenSanPham;
            sanPhamDangChon.DanhMuc = sp.DanhMuc;
            sanPhamDangChon.DonGia = sp.DonGia;
            sanPhamDangChon.SoLuong = sp.SoLuong;
            sanPhamDangChon.DuongDanAnh = sp.DuongDanAnh;

            nguonDuLieu.ResetBindings(false);
            nhanTrangThai.Text = "Tổng số sản phẩm: " + danhSachSanPham.Count;
            MessageBox.Show("Đã cập nhật sản phẩm.");
        }

        void Xoa_Click(object sender, EventArgs e)
        {
            if (sanPhamDangChon == null)
            {
                MessageBox.Show("Hãy chọn sản phẩm cần xóa.");
                return;
            }

            DialogResult kq = MessageBox.Show(
                "Bạn có chắc muốn xóa sản phẩm này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (kq == DialogResult.Yes)
            {
                danhSachSanPham.Remove(sanPhamDangChon);
                HienThiDanhSach(danhSachSanPham);
                XoaTrang();
            }
        }

        void DgvSanPham_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSanPham.CurrentRow == null)
                return;

            sanPhamDangChon = dgvSanPham.CurrentRow.DataBoundItem as SanPham;
            if (sanPhamDangChon == null)
                return;

            txtMaSanPham.Text = sanPhamDangChon.MaSanPham;
            txtTenSanPham.Text = sanPhamDangChon.TenSanPham;
            txtDonGia.Text = sanPhamDangChon.DonGia.ToString();
            txtSoLuong.Text = sanPhamDangChon.SoLuong.ToString();

            for (int i = 0; i < cboDanhMuc.Items.Count; i++)
            {
                DanhMucSanPham item = cboDanhMuc.Items[i] as DanhMucSanPham;
                if (item != null && item.TenDanhMuc == sanPhamDangChon.DanhMuc)
                {
                    cboDanhMuc.SelectedIndex = i;
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(sanPhamDangChon.DuongDanAnh) &&
                File.Exists(sanPhamDangChon.DuongDanAnh))
            {
                picAnhDaiDien.Image = Image.FromFile(sanPhamDangChon.DuongDanAnh);
                picAnhDaiDien.Tag = sanPhamDangChon.DuongDanAnh;
            }
            else
            {
                picAnhDaiDien.Image = null;
                picAnhDaiDien.Tag = null;
            }
        }

        void TxtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();

            if (tuKhoa == "")
            {
                HienThiDanhSach(danhSachSanPham);
                return;
            }

            BindingList<SanPham> ketQua = new BindingList<SanPham>(
                danhSachSanPham.Where(x =>
                    x.TenSanPham.IndexOf(tuKhoa,
                    StringComparison.OrdinalIgnoreCase) >= 0).ToList());

            HienThiDanhSach(ketQua);
        }

        void ChonAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Tệp ảnh|*.jpg;*.jpeg;*.png;*.bmp";

            if (open.ShowDialog() == DialogResult.OK)
            {
                if (picAnhDaiDien.Image != null)
                    picAnhDaiDien.Image.Dispose();

                picAnhDaiDien.Image = Image.FromFile(open.FileName);
                picAnhDaiDien.Tag = open.FileName;
            }
        }

        void XuatCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Tệp CSV|*.csv";
            save.FileName = "DanhSachSanPham.csv";

            if (save.ShowDialog() != DialogResult.OK)
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MaSanPham,TenSanPham,DanhMuc,DonGia,SoLuong");

            foreach (SanPham sp in danhSachSanPham)
            {
                sb.AppendLine(
                    "\"" + sp.MaSanPham.Replace("\"", "\"\"") + "\"," +
                    "\"" + sp.TenSanPham.Replace("\"", "\"\"") + "\"," +
                    "\"" + sp.DanhMuc.Replace("\"", "\"\"") + "\"," +
                    sp.DonGia.ToString() + "," +
                    sp.SoLuong);
            }

            File.WriteAllText(save.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Đã xuất file CSV.");
        }

        void XoaTrang()
        {
            txtMaSanPham.Clear();
            txtTenSanPham.Clear();
            txtDonGia.Clear();
            txtSoLuong.Clear();
            baoLoi.Clear();
            picAnhDaiDien.Image = null;
            picAnhDaiDien.Tag = null;
            sanPhamDangChon = null;
            dgvSanPham.ClearSelection();
        }
    }
}
