using MySql.Data.MySqlClient;
using QuanlyTonkho_NEW.Helpers;
using QuanlyTonkho_NEW.Models;
using System;
using System.Collections.Generic;

namespace QuanlyTonkho_NEW.Repositories
{
    public class PhieuXuatRepository
    {
        public void InsertPhieuXuat(PhieuXuat px,
                                    List<ChiTietXuat> ds)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                TonKhoRepository tonRepo =
                    new TonKhoRepository();

                try
                {
                    foreach (var item in ds)
                    {
                        int ton = tonRepo.GetSoLuongTon(item.MaSP);

                        if (item.SoLuong > ton)
                        {
                            throw new Exception(
                                $"Sản phẩm {item.MaSP} chỉ còn {ton}");
                        }
                    }

                    string sqlPX = @"insert into phieuxuat
                    (MaPhieuXuat, NgayXuat, MaNV, MaKH, GhiChu)
                    values (@Ma,@Ngay,@MaNV,@MaKH,@GhiChu)";

                    MySqlCommand cmdPX =
                        new MySqlCommand(sqlPX, conn, tran);

                    cmdPX.Parameters.AddWithValue("@Ma", px.MaPhieuXuat);
                    cmdPX.Parameters.AddWithValue("@Ngay", px.NgayXuat);
                    cmdPX.Parameters.AddWithValue("@MaNV", px.MaNV);
                    cmdPX.Parameters.AddWithValue("@MaKH", px.MaKH);
                    cmdPX.Parameters.AddWithValue("@GhiChu", px.GhiChu);

                    cmdPX.ExecuteNonQuery();

                    foreach (var item in ds)
                    {
                        string sqlCT = @"insert into ct_phieuxuat
                        (MaPhieuXuat, MaSP, SoLuong, DonGia)
                        values (@Ma,@MaSP,@SL,@DG)";

                        MySqlCommand cmdCT =
                            new MySqlCommand(sqlCT, conn, tran);

                        cmdCT.Parameters.AddWithValue("@Ma", px.MaPhieuXuat);
                        cmdCT.Parameters.AddWithValue("@MaSP", item.MaSP);
                        cmdCT.Parameters.AddWithValue("@SL", item.SoLuong);
                        cmdCT.Parameters.AddWithValue("@DG", item.DonGia);

                        cmdCT.ExecuteNonQuery();

                        tonRepo.TruTon(item.MaSP,
                                       item.SoLuong,
                                       conn, tran);
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}