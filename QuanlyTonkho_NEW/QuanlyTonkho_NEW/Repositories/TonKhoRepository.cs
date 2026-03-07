using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QuanlyTonkho_NEW.Helpers;
using QuanlyTonkho_NEW.Models;

namespace QuanlyTonkho_NEW.Repositories
{
    public class TonKhoRepository
    {
        //--------------------------------
        // LẤY TOÀN BỘ TỒN KHO
        //--------------------------------
        public List<TonKhoModel> GetAll()
        {
            List<TonKhoModel> list = new List<TonKhoModel>();

            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "SELECT MaSP, SoLuongTon FROM tonkho";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    list.Add(new TonKhoModel
                    {
                        MaSP = rd["MaSP"].ToString(),
                        TongNhap = Convert.ToInt32(rd["SoLuongTon"]),
                        TongXuat = 0
                    });
                }
            }

            return list;
        }

        //--------------------------------
        // LẤY SỐ LƯỢNG TỒN
        //--------------------------------
        public int GetSoLuongTon(string maSP)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "select SoLuongTon from tonkho where MaSP=@ma";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maSP);

                object result = cmd.ExecuteScalar();

                return result == null ? 0 : Convert.ToInt32(result);
            }
        }

        //--------------------------------
        // CỘNG TỒN
        //--------------------------------
        public void CongTon(string maSP, int soLuong,
                            MySqlConnection conn, MySqlTransaction tran)
        {
            string sql = @"
                insert into tonkho(MaSP, SoLuongTon)
                values(@ma,@sl)
                on duplicate key update
                SoLuongTon = SoLuongTon + @sl";

            MySqlCommand cmd = new MySqlCommand(sql, conn, tran);

            cmd.Parameters.AddWithValue("@ma", maSP);
            cmd.Parameters.AddWithValue("@sl", soLuong);

            cmd.ExecuteNonQuery();
        }

        //--------------------------------
        // TRỪ TỒN
        //--------------------------------
        public void TruTon(string maSP, int soLuong,
                           MySqlConnection conn, MySqlTransaction tran)
        {
            string sql = @"update tonkho
                           set SoLuongTon = SoLuongTon - @sl
                           where MaSP=@ma";

            MySqlCommand cmd = new MySqlCommand(sql, conn, tran);

            cmd.Parameters.AddWithValue("@ma", maSP);
            cmd.Parameters.AddWithValue("@sl", soLuong);

            cmd.ExecuteNonQuery();
        }
    }
}