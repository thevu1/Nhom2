/*
 Navicat Premium Data Transfer

 Source Server         : 123
 Source Server Type    : MySQL
 Source Server Version : 80408 (8.4.8)
 Source Host           : localhost:3306
 Source Schema         : phieuxuat

 Target Server Type    : MySQL
 Target Server Version : 80408 (8.4.8)
 File Encoding         : 65001

 Date: 03/03/2026 19:56:40
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for ct_phieunhap
-- ----------------------------
DROP TABLE IF EXISTS `ct_phieunhap`;
CREATE TABLE `ct_phieunhap`  (
  `MaPhieuNhap` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MaSP` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SoLuong` int NOT NULL,
  `DonGia` decimal(18, 2) NULL DEFAULT NULL,
  PRIMARY KEY (`MaPhieuNhap`, `MaSP`) USING BTREE,
  INDEX `MaSP`(`MaSP` ASC) USING BTREE,
  CONSTRAINT `ct_phieunhap_ibfk_1` FOREIGN KEY (`MaPhieuNhap`) REFERENCES `phieunhap` (`MaPhieuNhap`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `ct_phieunhap_ibfk_2` FOREIGN KEY (`MaSP`) REFERENCES `sanpham` (`MaSP`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ct_phieunhap
-- ----------------------------

-- ----------------------------
-- Table structure for ct_phieuxuat
-- ----------------------------
DROP TABLE IF EXISTS `ct_phieuxuat`;
CREATE TABLE `ct_phieuxuat`  (
  `MaPhieuXuat` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MaSP` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SoLuong` int NOT NULL,
  `DonGia` decimal(18, 2) NULL DEFAULT NULL,
  PRIMARY KEY (`MaPhieuXuat`, `MaSP`) USING BTREE,
  INDEX `MaSP`(`MaSP` ASC) USING BTREE,
  CONSTRAINT `ct_phieuxuat_ibfk_1` FOREIGN KEY (`MaPhieuXuat`) REFERENCES `phieuxuat` (`MaPhieuXuat`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `ct_phieuxuat_ibfk_2` FOREIGN KEY (`MaSP`) REFERENCES `sanpham` (`MaSP`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ct_phieuxuat
-- ----------------------------

-- ----------------------------
-- Table structure for danhmuc
-- ----------------------------
DROP TABLE IF EXISTS `danhmuc`;
CREATE TABLE `danhmuc`  (
  `MaDanhMuc` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TenDanhMuc` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `HinhAnh` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`MaDanhMuc`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of danhmuc
-- ----------------------------
INSERT INTO `danhmuc` VALUES ('DM01', 'Laptop', NULL);
INSERT INTO `danhmuc` VALUES ('DM02', 'Màn hình', NULL);
INSERT INTO `danhmuc` VALUES ('DM03', 'Tai nghe', NULL);
INSERT INTO `danhmuc` VALUES ('DM04', 'Bàn phím', NULL);
INSERT INTO `danhmuc` VALUES ('DM05', 'Chuột', NULL);

-- ----------------------------
-- Table structure for khachhang
-- ----------------------------
DROP TABLE IF EXISTS `khachhang`;
CREATE TABLE `khachhang`  (
  `MaKH` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TenKH` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DienThoai` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `DiaChi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`MaKH`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of khachhang
-- ----------------------------
INSERT INTO `khachhang` VALUES ('19022005', 'The Vũ', NULL, NULL, NULL);
INSERT INTO `khachhang` VALUES ('321', 'TheVu', NULL, NULL, NULL);
INSERT INTO `khachhang` VALUES ('KH001', 'Nguyễn Văn An', '0901111111', 'an@gmail.com', 'Đà Nẵng');
INSERT INTO `khachhang` VALUES ('KH002', 'Trần Minh Khoa', '0902222222', 'khoa@gmail.com', 'Huế');
INSERT INTO `khachhang` VALUES ('KH003', 'Lê Quốc Bảo', '0903333333', 'bao@gmail.com', 'Quảng Nam');
INSERT INTO `khachhang` VALUES ('KH004', 'Phạm Thu Hà', '0904444444', 'ha@gmail.com', 'Hà Nội');
INSERT INTO `khachhang` VALUES ('KH005', 'Đặng Hoàng Nam', '0905555555', 'nam@gmail.com', 'TP HCM');

-- ----------------------------
-- Table structure for nhacungcap
-- ----------------------------
DROP TABLE IF EXISTS `nhacungcap`;
CREATE TABLE `nhacungcap`  (
  `MaNCC` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TenNCC` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DienThoai` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `DiaChi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`MaNCC`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of nhacungcap
-- ----------------------------

-- ----------------------------
-- Table structure for nhanvien
-- ----------------------------
DROP TABLE IF EXISTS `nhanvien`;
CREATE TABLE `nhanvien`  (
  `MaNV` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TenNV` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NgaySinh` date NULL DEFAULT NULL,
  `GioiTinh` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `DienThoai` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `DiaChi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ChucVu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`MaNV`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of nhanvien
-- ----------------------------
INSERT INTO `nhanvien` VALUES ('NV001', 'Nguyễn Văn An', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `nhanvien` VALUES ('NV002', 'Trần Thị Bình', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `nhanvien` VALUES ('NV003', 'Lê Văn Cường', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `nhanvien` VALUES ('NV004', 'Phạm Thị Dung', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `nhanvien` VALUES ('NV005', 'Hoàng Văn Em', NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for phieunhap
-- ----------------------------
DROP TABLE IF EXISTS `phieunhap`;
CREATE TABLE `phieunhap`  (
  `MaPhieuNhap` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NgayNhap` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `MaNV` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `MaNCC` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`MaPhieuNhap`) USING BTREE,
  INDEX `MaNV`(`MaNV` ASC) USING BTREE,
  INDEX `MaNCC`(`MaNCC` ASC) USING BTREE,
  CONSTRAINT `phieunhap_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `phieunhap_ibfk_2` FOREIGN KEY (`MaNCC`) REFERENCES `nhacungcap` (`MaNCC`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of phieunhap
-- ----------------------------

-- ----------------------------
-- Table structure for phieuxuat
-- ----------------------------
DROP TABLE IF EXISTS `phieuxuat`;
CREATE TABLE `phieuxuat`  (
  `MaPhieuXuat` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NgayXuat` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `MaNV` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `MaKH` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`MaPhieuXuat`) USING BTREE,
  INDEX `MaNV`(`MaNV` ASC) USING BTREE,
  INDEX `MaKH`(`MaKH` ASC) USING BTREE,
  CONSTRAINT `phieuxuat_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `phieuxuat_ibfk_2` FOREIGN KEY (`MaKH`) REFERENCES `khachhang` (`MaKH`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of phieuxuat
-- ----------------------------

-- ----------------------------
-- Table structure for sanpham
-- ----------------------------
DROP TABLE IF EXISTS `sanpham`;
CREATE TABLE `sanpham`  (
  `MaSP` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TenSP` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DonViTinh` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `GiaNhap` decimal(18, 2) NULL DEFAULT NULL,
  `GiaBan` decimal(18, 2) NULL DEFAULT NULL,
  `MaDanhMuc` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `HinhAnh` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`MaSP`) USING BTREE,
  INDEX `fk_sp_danhmuc`(`MaDanhMuc` ASC) USING BTREE,
  CONSTRAINT `fk_sanpham_danhmuc` FOREIGN KEY (`MaDanhMuc`) REFERENCES `danhmuc` (`MaDanhMuc`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sanpham
-- ----------------------------
INSERT INTO `sanpham` VALUES ('SP01', 'Laptop ASUS TUF F15', 'Cái', 15000000.00, 17000000.00, 'DM01', NULL);
INSERT INTO `sanpham` VALUES ('SP02', 'Màn hình Dell 24 inch', 'Cái', 3000000.00, 3500000.00, 'DM02', NULL);
INSERT INTO `sanpham` VALUES ('SP03', 'Tai nghe HyperX Cloud II', 'Cái', 1500000.00, 1800000.00, 'DM03', NULL);
INSERT INTO `sanpham` VALUES ('SP04', 'Bàn phím AKKO 3087', 'Cái', 1200000.00, 1500000.00, 'DM04', NULL);
INSERT INTO `sanpham` VALUES ('SP05', 'Chuột Logitech G102', 'Cái', 300000.00, 400000.00, 'DM05', NULL);

-- ----------------------------
-- Table structure for taikhoan
-- ----------------------------
DROP TABLE IF EXISTS `taikhoan`;
CREATE TABLE `taikhoan`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MaNV` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Role` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT 'user',
  `CreatedAt` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `Username`(`Username` ASC) USING BTREE,
  INDEX `MaNV`(`MaNV` ASC) USING BTREE,
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of taikhoan
-- ----------------------------
INSERT INTO `taikhoan` VALUES (1, 'admin', 'admin', NULL, 'admin', '2026-03-03 02:23:57');

-- ----------------------------
-- Table structure for tonkho
-- ----------------------------
DROP TABLE IF EXISTS `tonkho`;
CREATE TABLE `tonkho`  (
  `MaSP` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SoLuongTon` int NULL DEFAULT 0,
  PRIMARY KEY (`MaSP`) USING BTREE,
  CONSTRAINT `tonkho_ibfk_1` FOREIGN KEY (`MaSP`) REFERENCES `sanpham` (`MaSP`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tonkho
-- ----------------------------

-- ----------------------------
-- Table structure for xacnhan
-- ----------------------------
DROP TABLE IF EXISTS `xacnhan`;
CREATE TABLE `xacnhan`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MaXacNhan` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of xacnhan
-- ----------------------------
INSERT INTO `xacnhan` VALUES (1, '123456');

SET FOREIGN_KEY_CHECKS = 1;
