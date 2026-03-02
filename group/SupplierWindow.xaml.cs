using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace group
{
    public partial class NhaCungCap : Window
    {
        string connStr =
            "server=localhost;database=phieuxuat;uid=root;pwd=123456;SslMode=none";
        bool dangTim = false;
        public NhaCungCap()
        {
            InitializeComponent();
            LoadData();
        }

        // ================= LOAD =================
        void LoadData()
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn =
                   new MySql.Data.MySqlClient.MySqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT * FROM nhacungcap";

                MySql.Data.MySqlClient.MySqlDataAdapter da =
                    new MySql.Data.MySqlClient.MySqlDataAdapter(sql, conn);

                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);

                dgNCC.ItemsSource = dt.DefaultView;
            }
        }

        // ================= CHECK TRÙNG =================
        bool CheckTrung(string ma, string sdt, string email)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"SELECT COUNT(*) FROM nhacungcap
                               WHERE MaNCC=@ma 
                               OR DienThoai=@sdt 
                               OR Email=@email";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.Parameters.AddWithValue("@sdt", sdt);
                cmd.Parameters.AddWithValue("@email", email);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }

        // ================= THÊM =================
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            if (txtMaNCC.Text == "" ||
                txtTenNCC.Text == "" ||
                txtSDT.Text == "" ||
                txtEmail.Text == "" ||
                txtDiaChi.Text == "")
            {
                MessageBox.Show("Nhập đầy đủ thông tin");
                return;
            }

            if (CheckTrung(txtMaNCC.Text, txtSDT.Text, txtEmail.Text))
            {
                MessageBox.Show("Mã / SDT / Email bị trùng");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"INSERT INTO nhacungcap
                                   VALUES(@ma,@ten,@sdt,@email,@dc)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@ma", txtMaNCC.Text);
                    cmd.Parameters.AddWithValue("@ten", txtTenNCC.Text);
                    cmd.Parameters.AddWithValue("@sdt", txtSDT.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@dc", txtDiaChi.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Thêm thành công");
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm lỗi: " + ex.Message);
            }
            ClearForm();

        }

        // ================= CHỌN GRID =================
        string maDangXoa = "";

        private void dgNCC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNCC.SelectedItem == null) return;

            var row = dgNCC.SelectedItem as System.Data.DataRowView;
            if (row == null) return;

            maDangXoa = row["MaNCC"].ToString();
        }

        // ================= SỬA =================
        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgNCC.SelectedItem == null)
            {
                MessageBox.Show("Chọn dòng để sửa");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"UPDATE nhacungcap
                                   SET TenNCC=@ten,
                                       DienThoai=@sdt,
                                       Email=@email,
                                       DiaChi=@dc
                                   WHERE MaNCC=@ma";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@ma", txtMaNCC.Text);
                    cmd.Parameters.AddWithValue("@ten", txtTenNCC.Text);
                    cmd.Parameters.AddWithValue("@sdt", txtSDT.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@dc", txtDiaChi.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Sửa thành công");
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa: " + ex.Message);
            }
            ClearForm();

        }

        // ================= TÌM =================
        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            // Nếu đang ở chế độ tìm → bấm lần nữa sẽ huỷ
            if (dangTim)
            {
                LoadData();

                dangTim = false;
                btn_timKiem.Content = "Tìm kiếm";
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"SELECT * FROM nhacungcap
                       WHERE MaNCC=@ma
                       OR DienThoai=@sdt
                       OR Email=@email";

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);

                da.SelectCommand.Parameters.AddWithValue("@ma", txtMaNCC.Text);
                da.SelectCommand.Parameters.AddWithValue("@sdt", txtSDT.Text);
                da.SelectCommand.Parameters.AddWithValue("@email", txtEmail.Text);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy");
                    return;
                }

                dgNCC.ItemsSource = dt.DefaultView;

                // bật chế độ tìm
                dangTim = true;
                btn_timKiem.Content = "Huỷ";
                ClearForm();

            }
        }

        // ================= XOÁ =================
        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (maDangXoa == "")
            {
                MessageBox.Show("Hãy chọn dòng cần xoá");
                return;
            }

            // mở form xác minh
            XacNhanWindow f = new XacNhanWindow();

            if (f.ShowDialog() == true)
            {
                XoaNhaCungCap(maDangXoa);
                LoadData();
                ClearForm();
                maDangXoa = "";
            }
        }
        //============ XOÁ NHÀ CUNG CẤP ============
        void XoaNhaCungCap(string ma)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn =
                   new MySql.Data.MySqlClient.MySqlConnection(connStr))
            {
                conn.Open();

                string sql = "DELETE FROM nhacungcap WHERE MaNCC = @ma";

                MySql.Data.MySqlClient.MySqlCommand cmd =
                    new MySql.Data.MySqlClient.MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ma", ma);

                int kq = cmd.ExecuteNonQuery();

                if (kq > 0)
                    MessageBox.Show("Xoá thành công");
                else
                    MessageBox.Show("Không tìm thấy dữ liệu để xoá");
            }
        }
        // ================= THOÁT =================
        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // ================ XOÁ TẤT CẢ CÁC TRƯỜNG ==================
        void ClearForm()
        {
            txtMaNCC.Clear();
            txtTenNCC.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();

            txtMaNCC.Focus();
        }
    }
}