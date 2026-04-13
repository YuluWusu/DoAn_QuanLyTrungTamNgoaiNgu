CREATE DATABASE QL_TRUNGTAM_TIENGANH
GO
USE QL_TRUNGTAM_TIENGANH
GO

-- ============================================================
-- PHAN 1: TAO BANG (TABLES)
-- ============================================================

CREATE TABLE LOAI_KHOAHOC
(
    MALOAI_KH CHAR(5)      PRIMARY KEY,
    TENLOAI   NVARCHAR(50) NOT NULL UNIQUE
)
GO

CREATE TABLE KHOA_HOC
(
    MAKH      CHAR(5)      PRIMARY KEY,
    TENKH     NVARCHAR(50) NOT NULL,
    SOBUOI    INT          NOT NULL CHECK (SOBUOI > 0),
    HOCPHI_GD MONEY        NOT NULL CHECK (HOCPHI_GD >= 0),
    MALOAI_KH CHAR(5)      NOT NULL,
    FOREIGN KEY (MALOAI_KH) REFERENCES LOAI_KHOAHOC(MALOAI_KH)
)
GO

CREATE TABLE GIAOVIEN
(
    MaGV       CHAR(6)      PRIMARY KEY,
    TenGV      NVARCHAR(50) NOT NULL,
    SDT        VARCHAR(15)  UNIQUE,
    Email      VARCHAR(100) UNIQUE,
    DiaChi     NVARCHAR(100),
    TrinhDo    NVARCHAR(20) CHECK (TrinhDo IN (N'Cu nhan', N'Thac si', N'Tien si')),
    NgayVaoLam DATE         CHECK (NgayVaoLam <= GETDATE())
)
GO

CREATE TABLE HOCVIEN
(
    MaHV      CHAR(6)      PRIMARY KEY,
    HoTen     NVARCHAR(50) NOT NULL,
    NgaySinh  DATE         CHECK (NgaySinh < GETDATE()),
    GioiTinh  NVARCHAR(5)  CHECK (GioiTinh IN (N'Nam', N'Nu')),
    DiaChi    NVARCHAR(255),
    SDT       VARCHAR(15)  UNIQUE,
    Email     VARCHAR(100) UNIQUE,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Dang hoc'
              CHECK (TrangThai IN (N'Dang hoc', N'Bao luu', N'Nghi', N'Hoan thanh'))
)
GO

CREATE TABLE PHONGHOC
(
    MAPHONG   CHAR(5)      PRIMARY KEY,
    TENPHONG  NVARCHAR(50) NOT NULL UNIQUE,
    SOGHENGOI INT          NOT NULL CHECK (SOGHENGOI > 0)
)
GO

CREATE TABLE CAHOC
(
    MACA       CHAR(5)      PRIMARY KEY,
    TENCA      NVARCHAR(50) NOT NULL,
    GIOBATDAU  TIME         NOT NULL,
    GIOKETTHUC TIME         NOT NULL,
    CHECK (GIOBATDAU < GIOKETTHUC)
)
GO

CREATE TABLE LOPHOC
(
    MALOP       CHAR(5)      PRIMARY KEY,
    TENLOP      NVARCHAR(50) NOT NULL,
    MAKH        CHAR(5)      NOT NULL,
    MaGV        CHAR(6)      NOT NULL,
    NGAYBATDAU  DATE         NOT NULL,
    NGAYKETTHUC DATE         NOT NULL,
    SISO_TOIDA  INT          NOT NULL DEFAULT 20 CHECK (SISO_TOIDA > 0),
    TRANGTHAI   NVARCHAR(20) NOT NULL DEFAULT N'Dang mo'
                CHECK (TRANGTHAI IN (N'Dang mo', N'Da ket thuc', N'Huy')),
    CHECK (NGAYBATDAU <= NGAYKETTHUC),
    FOREIGN KEY (MAKH) REFERENCES KHOA_HOC(MAKH),
    FOREIGN KEY (MaGV) REFERENCES GIAOVIEN(MaGV)
)
GO

CREATE TABLE LICHOC
(
    MALICH  INT     IDENTITY PRIMARY KEY,
    MALOP   CHAR(5) NOT NULL,
    NGAYHOC DATE    NOT NULL,
    MAPHONG CHAR(5) NOT NULL,
    MACA    CHAR(5) NOT NULL,
    CONSTRAINT UQ_LICH UNIQUE (MAPHONG, MACA, NGAYHOC),
    FOREIGN KEY (MALOP)   REFERENCES LOPHOC(MALOP),
    FOREIGN KEY (MAPHONG) REFERENCES PHONGHOC(MAPHONG),
    FOREIGN KEY (MACA)    REFERENCES CAHOC(MACA)
)
GO

CREATE TABLE DANGKYLOP
(
    MaHV      CHAR(6)      NOT NULL,
    MALOP     CHAR(5)      NOT NULL,
    NGAYDK    DATETIME     NOT NULL DEFAULT GETDATE(),
    HOCPHI    MONEY        NOT NULL CHECK (HOCPHI >= 0),
    TRANGTHAI NVARCHAR(20) NOT NULL DEFAULT N'Cho dong tien'
              CHECK (TRANGTHAI IN (N'Cho dong tien', N'Dang hoc', N'Bao luu', N'Hoan thanh', N'Huy')),
    PRIMARY KEY (MaHV, MALOP),
    FOREIGN KEY (MaHV)  REFERENCES HOCVIEN(MaHV),
    FOREIGN KEY (MALOP) REFERENCES LOPHOC(MALOP)
)
GO

CREATE TABLE PHIEUTHU
(
    MAPHIEU     CHAR(10)     PRIMARY KEY,
    MaHV        CHAR(6)      NOT NULL,
    MALOP       CHAR(5)      NOT NULL,
    NGAYTHU     DATETIME     NOT NULL DEFAULT GETDATE(),
    SOTIEN      MONEY        NOT NULL CHECK (SOTIEN > 0),
    HINHTHUCTHU NVARCHAR(20) NOT NULL DEFAULT N'Tien mat'
                CHECK (HINHTHUCTHU IN (N'Tien mat', N'Chuyen khoan', N'The')),
    GHICHU      NVARCHAR(200),
    FOREIGN KEY (MaHV, MALOP) REFERENCES DANGKYLOP(MaHV, MALOP)
)
GO

CREATE TABLE DIEMDANH
(
    MaHV      CHAR(6)      NOT NULL,
    MALOP     CHAR(5)      NOT NULL,
    NGAYDD    DATE         NOT NULL,
    TRANGTHAI NVARCHAR(15) NOT NULL DEFAULT N'Co mat'
              CHECK (TRANGTHAI IN (N'Co mat', N'Vang CP', N'Vang KP', N'Tre')),
    GHICHU    NVARCHAR(100),
    PRIMARY KEY (MaHV, MALOP, NGAYDD),
    FOREIGN KEY (MaHV, MALOP) REFERENCES DANGKYLOP(MaHV, MALOP)
)
GO

CREATE TABLE NHANVIEN
(
    MaNV       CHAR(6)      PRIMARY KEY,
    HoTen      NVARCHAR(50) NOT NULL,
    ChucVu     NVARCHAR(20) NOT NULL CHECK (ChucVu IN (N'Quan ly', N'Le tan', N'Ke toan')),
    SDT        VARCHAR(15)  UNIQUE,
    Email      VARCHAR(100) UNIQUE,
    NgayVaoLam DATE         CHECK (NgayVaoLam <= GETDATE())
)
GO
CREATE TABLE TAIKHOAN
(
    MATK        CHAR(6)      PRIMARY KEY,
    TENDANGNHAP VARCHAR(50)  NOT NULL UNIQUE,
    MATKHAU     VARCHAR(255) NOT NULL,
    MaNV        CHAR(6)      NULL REFERENCES NHANVIEN(MaNV),
    MaGV        CHAR(6)      NULL REFERENCES GIAOVIEN(MaGV),
    TRANGTHAI   BIT          NOT NULL DEFAULT 1,
    NGAYTAO     DATETIME     NOT NULL DEFAULT GETDATE(),
    CHECK (
        (MaNV IS NOT NULL AND MaGV IS NULL) OR
        (MaNV IS NULL AND MaGV IS NOT NULL)
    )
)
GO

CREATE TABLE QUYEN
(
    MAQUYEN  CHAR(5)       PRIMARY KEY,
    TENQUYEN VARCHAR(100)  NOT NULL UNIQUE,
    MOTA     NVARCHAR(255)
)
GO

CREATE TABLE TAIKHOAN_QUYEN
(
    MATK    CHAR(6) NOT NULL,
    MAQUYEN CHAR(5) NOT NULL,
    PRIMARY KEY (MATK, MAQUYEN),
    FOREIGN KEY (MATK)    REFERENCES TAIKHOAN(MATK),
    FOREIGN KEY (MAQUYEN) REFERENCES QUYEN(MAQUYEN)
)
GO
-- ============================================================
-- PHAN 2: Nhap Lieu (INSERT INTO)
-- ============================================================
-- ============================================================
-- PHAN 2: NHAP LIEU MAU (INSERT INTO)
-- ============================================================

-- 1. LOAI_KHOAHOC
INSERT INTO LOAI_KHOAHOC (MALOAI_KH, TENLOAI) VALUES
('L0001', N'Tiếng Anh trẻ em'),
('L0002', N'Tiếng Anh giao tiếp'),
('L0003', N'Tiếng Anh học thuật'),
('L0004', N'Luyện thi chứng chỉ'),
('L0005', N'Tiếng Anh doanh nghiệp');

-- 2. KHOA_HOC
INSERT INTO KHOA_HOC (MAKH, TENKH, SOBUOI, HOCPHI_GD, MALOAI_KH) VALUES
('KH001', N'Kids Starter', 20, 2500000, 'L0001'),
('KH002', N'Kids Mover', 24, 3000000, 'L0001'),
('KH003', N'Kids Flyer', 24, 3200000, 'L0001'),
('KH004', N'Giao tiếp cơ bản', 30, 4500000, 'L0002'),
('KH005', N'Giao tiếp nâng cao', 30, 5500000, 'L0002'),
('KH006', N'IELTS Foundation', 40, 8000000, 'L0004'),
('KH007', N'IELTS Target 6.5+', 48, 12000000, 'L0004'),
('KH008', N'TOEIC 4 kỹ năng', 36, 7500000, 'L0004'),
('KH009', N'Academic Writing', 20, 5000000, 'L0003'),
('KH010', N'Business English', 24, 6000000, 'L0005');

-- 3. GIAOVIEN
INSERT INTO GIAOVIEN (MaGV, TenGV, SDT, Email, DiaChi, TrinhDo, NgayVaoLam) VALUES
('GV0001', N'Nguyễn Thị Lan Anh', '0901234001', 'lananh.nguyen@center.edu', N'123 Nguyễn Trãi, Q1, TP.HCM', N'Thac si', '2020-01-15'),
('GV0002', N'Trần Văn Bình', '0901234002', 'binh.tran@center.edu', N'456 Lê Lợi, Q3, TP.HCM', N'Cu nhan', '2020-03-20'),
('GV0003', N'Lê Thị Cẩm Tú', '0901234003', 'camtu.le@center.edu', N'789 Phạm Ngũ Lão, Q1, TP.HCM', N'Thac si', '2019-06-10'),
('GV0004', N'Phạm Quốc Hùng', '0901234004', 'quochung.pham@center.edu', N'321 Điện Biên Phủ, Bình Thạnh', N'Tien si', '2018-09-01'),
('GV0005', N'Hoàng Thị Mai', '0901234005', 'mai.hoang@center.edu', N'555 Cách Mạng Tháng 8, Q10', N'Cu nhan', '2021-02-14'),
('GV0006', N'Đỗ Minh Tuấn', '0901234006', 'minhtuan.do@center.edu', N'888 Võ Văn Kiệt, Q5', N'Thac si', '2020-07-20'),
('GV0007', N'Vũ Thị Hương', '0901234007', 'huong.vu@center.edu', N'222 Lê Văn Sỹ, Q3', N'Cu nhan', '2021-11-01'),
('GV0008', N'Ngô Văn Thành', '0901234008', 'thanh.ngo@center.edu', N'777 Trường Chinh, Tân Bình', N'Tien si', '2019-03-15');
-- ============================================================
-- PHAN 2: NHAP LIEU MAU (INSERT INTO)
-- ============================================================
GO
-- 1. LOAI_KHOAHOC
INSERT INTO LOAI_KHOAHOC (MALOAI_KH, TENLOAI) VALUES
('L0001', N'Tiếng Anh trẻ em'),
('L0002', N'Tiếng Anh giao tiếp'),
('L0003', N'Tiếng Anh học thuật'),
('L0004', N'Luyện thi chứng chỉ'),
('L0005', N'Tiếng Anh doanh nghiệp');

-- 2. KHOA_HOC
INSERT INTO KHOA_HOC (MAKH, TENKH, SOBUOI, HOCPHI_GD, MALOAI_KH) VALUES
('KH001', N'Kids Starter', 20, 2500000, 'L0001'),
('KH002', N'Kids Mover', 24, 3000000, 'L0001'),
('KH003', N'Kids Flyer', 24, 3200000, 'L0001'),
('KH004', N'Giao tiếp cơ bản', 30, 4500000, 'L0002'),
('KH005', N'Giao tiếp nâng cao', 30, 5500000, 'L0002'),
('KH006', N'IELTS Foundation', 40, 8000000, 'L0004'),
('KH007', N'IELTS Target 6.5+', 48, 12000000, 'L0004'),
('KH008', N'TOEIC 4 kỹ năng', 36, 7500000, 'L0004'),
('KH009', N'Academic Writing', 20, 5000000, 'L0003'),
('KH010', N'Business English', 24, 6000000, 'L0005');

-- 3. GIAOVIEN
INSERT INTO GIAOVIEN (MaGV, TenGV, SDT, Email, DiaChi, TrinhDo, NgayVaoLam) VALUES
('GV0001', N'Nguyễn Thị Lan Anh', '0901234001', 'lananh.nguyen@center.edu', N'123 Nguyễn Trãi, Q1, TP.HCM', N'Thac si', '2020-01-15'),
('GV0002', N'Trần Văn Bình', '0901234002', 'binh.tran@center.edu', N'456 Lê Lợi, Q3, TP.HCM', N'Cu nhan', '2020-03-20'),
('GV0003', N'Lê Thị Cẩm Tú', '0901234003', 'camtu.le@center.edu', N'789 Phạm Ngũ Lão, Q1, TP.HCM', N'Thac si', '2019-06-10'),
('GV0004', N'Phạm Quốc Hùng', '0901234004', 'quochung.pham@center.edu', N'321 Điện Biên Phủ, Bình Thạnh', N'Tien si', '2018-09-01'),
('GV0005', N'Hoàng Thị Mai', '0901234005', 'mai.hoang@center.edu', N'555 Cách Mạng Tháng 8, Q10', N'Cu nhan', '2021-02-14'),
('GV0006', N'Đỗ Minh Tuấn', '0901234006', 'minhtuan.do@center.edu', N'888 Võ Văn Kiệt, Q5', N'Thac si', '2020-07-20'),
('GV0007', N'Vũ Thị Hương', '0901234007', 'huong.vu@center.edu', N'222 Lê Văn Sỹ, Q3', N'Cu nhan', '2021-11-01'),
('GV0008', N'Ngô Văn Thành', '0901234008', 'thanh.ngo@center.edu', N'777 Trường Chinh, Tân Bình', N'Tien si', '2019-03-15');

-- 4. HOCVIEN (SỬA: bỏ TrangThai vì có DEFAULT)
INSERT INTO HOCVIEN (MaHV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, Email) VALUES
('HV0001', N'Trần Minh Anh', '2015-05-10', N'Nu', N'12 Nguyễn Du, Q1, TP.HCM', '0909001001', 'minhanh.tran@email.com'),
('HV0002', N'Lê Quang Huy', '2014-08-22', N'Nam', N'34 Hai Bà Trưng, Q3', '0909001002', 'quanghuy.le@email.com'),
('HV0003', N'Phạm Thảo Vy', '2016-02-15', N'Nu', N'56 Lý Tự Trọng, Q1', '0909001003', 'thaovy.pham@email.com'),
('HV0004', N'Nguyễn Hoàng Nam', '2013-11-30', N'Nam', N'78 Nguyễn Thị Minh Khai, Q3', '0909001004', 'hoangnam.nguyen@email.com'),
('HV0005', N'Vũ Thị Ngọc', '1995-03-18', N'Nu', N'90 Lê Lai, Q1', '0909001005', 'ngoc.vu@email.com'),
('HV0006', N'Trương Văn Đức', '1998-07-25', N'Nam', N'123 Đồng Khởi, Q1', '0909001006', 'duc.truong@email.com'),
('HV0007', N'Hoàng Thị Thanh', '1996-12-02', N'Nu', N'456 Nguyễn Đình Chiểu, Q3', '0909001007', 'thanh.hoang@email.com'),
('HV0008', N'Đặng Quốc Bảo', '1994-09-14', N'Nam', N'789 Xô Viết Nghệ Tĩnh, Bình Thạnh', '0909001008', 'quocbao.dang@email.com'),
('HV0009', N'Bùi Thị Mai Lan', '1997-04-20', N'Nu', N'321 Hồng Bàng, Q5', '0909001009', 'mailan.bui@email.com'),
('HV0010', N'Lý Hoàng Long', '1999-01-07', N'Nam', N'555 An Dương Vương, Q5', '0909001010', 'hoanglong.ly@email.com'),
('HV0011', N'Ngô Thị Kim', '2015-07-19', N'Nu', N'12 Cộng Hòa, Tân Bình', '0909001011', 'kim.ngo@email.com'),
('HV0012', N'Phan Văn Tài', '2016-10-05', N'Nam', N'34 Hoàng Văn Thụ, Tân Bình', '0909001012', 'tai.phan@email.com'),
('HV0013', N'Trịnh Thị Hoa', '1995-06-28', N'Nu', N'56 Nguyễn Văn Cừ, Q1', '0909001013', 'hoa.trinh@email.com'),
('HV0014', N'Đỗ Văn Hùng', '1993-11-11', N'Nam', N'78 Nguyễn Hữu Cảnh, Bình Thạnh', '0909001014', 'hung.do@email.com'),
('HV0015', N'Võ Thị Thúy', '1998-02-28', N'Nu', N'90 Trần Hưng Đạo, Q1', '0909001015', 'thuy.vo@email.com');

-- 5. PHONGHOC
INSERT INTO PHONGHOC (MAPHONG, TENPHONG, SOGHENGOI) VALUES
('P001', N'Phòng A101', 25),
('P002', N'Phòng A102', 30),
('P003', N'Phòng B201', 20),
('P004', N'Phòng B202', 25),
('P005', N'Phòng C301', 35),
('P006', N'Phòng C302', 40),
('P007', N'Phòng Lab 1', 20),
('P008', N'Phòng Lab 2', 20);

-- 6. CAHOC
INSERT INTO CAHOC (MACA, TENCA, GIOBATDAU, GIOKETTHUC) VALUES
('C001', N'Sáng 1', '07:30:00', '09:30:00'),
('C002', N'Sáng 2', '09:45:00', '11:45:00'),
('C003', N'Chiều 1', '13:30:00', '15:30:00'),
('C004', N'Chiều 2', '15:45:00', '17:45:00'),
('C005', N'Tối 1', '18:00:00', '20:00:00'),
('C006', N'Tối 2', '20:15:00', '22:15:00');

-- 7. LOPHOC
INSERT INTO LOPHOC (MALOP, TENLOP, MAKH, MaGV, NGAYBATDAU, NGAYKETTHUC, SISO_TOIDA, TRANGTHAI) VALUES
('L0001', N'Kids Starter - Sáng T2T4', 'KH001', 'GV0001', '2024-01-08', '2024-03-28', 20, N'Da ket thuc'),
('L0002', N'Kids Mover - Chiều T3T5', 'KH002', 'GV0002', '2024-01-09', '2024-04-04', 20, N'Da ket thuc'),
('L0003', N'Giao tiếp cơ bản - Tối T2T4', 'KH004', 'GV0003', '2024-02-01', '2024-05-15', 25, N'Dang mo'),
('L0004', N'IELTS Foundation - Sáng T7CN', 'KH006', 'GV0004', '2024-02-10', '2024-06-20', 30, N'Dang mo'),
('L0005', N'TOEIC 4 kỹ năng - Tối T3T5', 'KH008', 'GV0005', '2024-03-01', '2024-06-30', 25, N'Dang mo'),
('L0006', N'Kids Flyer - Sáng T2T4', 'KH003', 'GV0001', '2024-04-01', '2024-07-15', 20, N'Dang mo'),
('L0007', N'IELTS Target 6.5+ - T7CN', 'KH007', 'GV0006', '2024-04-15', '2024-08-30', 25, N'Dang mo'),
('L0008', N'Business English - Tối T4T6', 'KH010', 'GV0007', '2024-05-01', '2024-08-10', 20, N'Dang mo'),
('L0009', N'Giao tiếp nâng cao - Sáng T3T5', 'KH005', 'GV0008', '2024-05-10', '2024-09-15', 25, N'Dang mo'),
('L0010', N'Kids Starter - Chiều T7CN', 'KH001', 'GV0002', '2024-06-01', '2024-09-30', 20, N'Dang mo');

-- 8. LICHOC (giữ nguyên như cũ nhưng cắt bớt cho ngắn)
INSERT INTO LICHOC (MALOP, NGAYHOC, MAPHONG, MACA) VALUES
('L0001', '2024-01-08', 'P001', 'C001'),
('L0001', '2024-01-10', 'P001', 'C001'),
('L0001', '2024-01-15', 'P001', 'C001'),
('L0001', '2024-01-17', 'P001', 'C001'),
('L0003', '2024-02-01', 'P003', 'C005'),
('L0003', '2024-02-06', 'P003', 'C005'),
('L0003', '2024-02-08', 'P003', 'C005'),
('L0004', '2024-02-10', 'P005', 'C001'),
('L0004', '2024-02-11', 'P005', 'C001'),
('L0004', '2024-02-17', 'P005', 'C001');

-- 9. DANGKYLOP (SỬA: dùng đúng giá trị TRANGTHAI)
INSERT INTO DANGKYLOP (MaHV, MALOP, NGAYDK, HOCPHI, TRANGTHAI) VALUES
('HV0001', 'L0001', '2024-01-05 10:30:00', 2500000, N'Hoan thanh'),
('HV0002', 'L0001', '2024-01-05 14:15:00', 2500000, N'Hoan thanh'),
('HV0003', 'L0001', '2024-01-06 09:00:00', 2500000, N'Hoan thanh'),
('HV0005', 'L0003', '2024-01-25 08:30:00', 4500000, N'Dang hoc'),
('HV0006', 'L0003', '2024-01-26 11:00:00', 4500000, N'Dang hoc'),
('HV0007', 'L0003', '2024-01-27 14:00:00', 4500000, N'Bao luu'),
('HV0008', 'L0003', '2024-01-28 09:30:00', 4500000, N'Dang hoc'),
('HV0005', 'L0004', '2024-02-01 09:00:00', 8000000, N'Dang hoc'),
('HV0006', 'L0004', '2024-02-02 10:30:00', 8000000, N'Dang hoc'),
('HV0008', 'L0004', '2024-02-03 14:00:00', 8000000, N'Dang hoc'),
('HV0005', 'L0005', '2024-02-25 10:00:00', 7500000, N'Dang hoc'),
('HV0006', 'L0005', '2024-02-26 14:30:00', 7500000, N'Dang hoc'),
('HV0001', 'L0006', '2024-03-25 10:00:00', 3200000, N'Dang hoc'),
('HV0002', 'L0006', '2024-03-26 14:00:00', 3200000, N'Dang hoc');

-- 10. PHIEUTHU
INSERT INTO PHIEUTHU (MAPHIEU, MaHV, MALOP, NGAYTHU, SOTIEN, HINHTHUCTHU, GHICHU) VALUES
('PT00000001', 'HV0001', 'L0001', '2024-01-05 10:30:00', 2500000, N'Tien mat', N'Dong full hoc phi'),
('PT00000002', 'HV0002', 'L0001', '2024-01-05 14:15:00', 2500000, N'Chuyen khoan', N'Chuyen khoan Vietcombank'),
('PT00000003', 'HV0003', 'L0001', '2024-01-06 09:00:00', 2500000, N'Tien mat', NULL),
('PT00000004', 'HV0005', 'L0003', '2024-01-25 08:30:00', 2250000, N'Tien mat', N'Dong 50%'),
('PT00000005', 'HV0005', 'L0003', '2024-02-25 09:00:00', 2250000, N'Chuyen khoan', N'Dong not 50%'),
('PT00000006', 'HV0006', 'L0003', '2024-01-26 11:00:00', 4500000, N'Tien mat', N'Dong full'),
('PT00000007', 'HV0008', 'L0003', '2024-01-28 09:30:00', 4500000, N'Chuyen khoan', NULL),
('PT00000008', 'HV0005', 'L0004', '2024-02-01 09:00:00', 4000000, N'Tien mat', N'Dong 50%'),
('PT00000009', 'HV0005', 'L0004', '2024-03-01 10:00:00', 4000000, N'Chuyen khoan', N'Dong not 50%'),
('PT00000010', 'HV0006', 'L0004', '2024-02-02 10:30:00', 8000000, N'Tien mat', NULL);

-- 11. DIEMDANH
INSERT INTO DIEMDANH (MaHV, MALOP, NGAYDD, TRANGTHAI, GHICHU) VALUES
('HV0001', 'L0001', '2024-01-08', N'Co mat', NULL),
('HV0001', 'L0001', '2024-01-10', N'Co mat', NULL),
('HV0001', 'L0001', '2024-01-15', N'Co mat', NULL),
('HV0001', 'L0001', '2024-01-17', N'Co mat', NULL),
('HV0001', 'L0001', '2024-01-24', N'Vang CP', N'Có phép'),
('HV0002', 'L0001', '2024-01-08', N'Co mat', NULL),
('HV0002', 'L0001', '2024-01-10', N'Co mat', NULL),
('HV0002', 'L0001', '2024-01-17', N'Vang KP', N'Không phép'),
('HV0002', 'L0001', '2024-02-07', N'Tre', N'Đến trễ 15p'),
('HV0005', 'L0003', '2024-02-01', N'Co mat', NULL),
('HV0005', 'L0003', '2024-02-06', N'Co mat', NULL),
('HV0005', 'L0003', '2024-02-15', N'Vang CP', N'Bận việc gia đình'),
('HV0005', 'L0003', '2024-03-21', N'Vang KP', N'Không báo trước');

-- 12. NHANVIEN
INSERT INTO NHANVIEN (MaNV, HoTen, ChucVu, SDT, Email, NgayVaoLam) VALUES
('NV0001', N'Trần Thị Minh Tâm', N'Quan ly', '0902001001', 'tam.tran@center.edu', '2020-01-10'),
('NV0002', N'Lê Văn Phúc', N'Le tan', '0902001002', 'phuc.le@center.edu', '2021-03-15'),
('NV0003', N'Phạm Thị Ngọc', N'Le tan', '0902001003', 'ngoc.pham@center.edu', '2021-06-20'),
('NV0004', N'Nguyễn Hoàng Anh', N'Ke toan', '0902001004', 'hoanganh.nguyen@center.edu', '2020-09-01'),
('NV0005', N'Vũ Thị Hồng', N'Ke toan', '0902001005', 'hong.vu@center.edu', '2022-02-14');

-- 13. QUYEN
INSERT INTO QUYEN (MAQUYEN, TENQUYEN, MOTA) VALUES
('Q0001', 'VIEW_STUDENT', N'Xem danh sách học viên'),
('Q0002', 'EDIT_STUDENT', N'Thêm/sửa/xóa học viên'),
('Q0003', 'VIEW_COURSE', N'Xem danh sách khóa học'),
('Q0004', 'EDIT_COURSE', N'Thêm/sửa/xóa khóa học'),
('Q0005', 'VIEW_CLASS', N'Xem danh sách lớp học'),
('Q0006', 'EDIT_CLASS', N'Thêm/sửa/xóa lớp học'),
('Q0007', 'VIEW_TEACHER', N'Xem danh sách giáo viên'),
('Q0008', 'EDIT_TEACHER', N'Thêm/sửa/xóa giáo viên'),
('Q0009', 'VIEW_FINANCE', N'Xem báo cáo tài chính'),
('Q0010', 'EDIT_FINANCE', N'Thực hiện thu chi'),
('Q0011', 'VIEW_REPORT', N'Xem báo cáo thống kê'),
('Q0012', 'MANAGE_USER', N'Quản lý tài khoản người dùng');

-- 14. TAIKHOAN (SỬA: đảm bảo không có NULL trùng)
INSERT INTO TAIKHOAN (MATK, TENDANGNHAP, MATKHAU, MaNV, MaGV, TRANGTHAI, NGAYTAO) VALUES
('TK0001', 'admin', 'admin123', 'NV0001', NULL, 1, '2024-01-01 00:00:00'),
('TK0002', 'tam.tran', 'password123', 'NV0001', NULL, 1, '2024-01-01 00:00:00'),
('TK0003', 'phuc.le', 'pass123', 'NV0002', NULL, 1, '2024-01-01 00:00:00'),
('TK0004', 'ngoc.pham', 'pass123', 'NV0003', NULL, 1, '2024-01-01 00:00:00'),
('TK0005', 'hoanganh.nguyen', 'pass123', 'NV0004', NULL, 1, '2024-01-01 00:00:00'),
('TK0006', 'hong.vu', 'pass123', 'NV0005', NULL, 0, '2024-01-01 00:00:00'),
('TK0007', 'lananh.nguyen', 'gv123', NULL, 'GV0001', 1, '2024-01-01 00:00:00'),
('TK0008', 'binh.tran', 'gv123', NULL, 'GV0002', 1, '2024-01-01 00:00:00'),
('TK0009', 'camtu.le', 'gv123', NULL, 'GV0003', 1, '2024-01-01 00:00:00'),
('TK0010', 'quochung.pham', 'gv123', NULL, 'GV0004', 1, '2024-01-01 00:00:00');

-- 15. TAIKHOAN_QUYEN
INSERT INTO TAIKHOAN_QUYEN (MATK, MAQUYEN) VALUES
('TK0001', 'Q0001'), ('TK0001', 'Q0002'), ('TK0001', 'Q0003'), ('TK0001', 'Q0004'),
('TK0001', 'Q0005'), ('TK0001', 'Q0006'), ('TK0001', 'Q0007'), ('TK0001', 'Q0008'),
('TK0001', 'Q0009'), ('TK0001', 'Q0010'), ('TK0001', 'Q0011'), ('TK0001', 'Q0012'),
('TK0002', 'Q0001'), ('TK0002', 'Q0002'), ('TK0002', 'Q0003'), ('TK0002', 'Q0004'),
('TK0002', 'Q0005'), ('TK0002', 'Q0006'), ('TK0002', 'Q0007'), ('TK0002', 'Q0008'),
('TK0002', 'Q0009'), ('TK0002', 'Q0010'), ('TK0002', 'Q0011'),
('TK0003', 'Q0001'), ('TK0003', 'Q0003'), ('TK0003', 'Q0005'), ('TK0003', 'Q0010'),
('TK0004', 'Q0001'), ('TK0004', 'Q0003'), ('TK0004', 'Q0005'), ('TK0004', 'Q0010'),
('TK0005', 'Q0009'), ('TK0005', 'Q0010'), ('TK0005', 'Q0011'),
('TK0007', 'Q0001'), ('TK0007', 'Q0005'), ('TK0007', 'Q0007'),
('TK0008', 'Q0001'), ('TK0008', 'Q0005'), ('TK0008', 'Q0007'),
('TK0009', 'Q0001'), ('TK0009', 'Q0005'), ('TK0009', 'Q0007'),
('TK0010', 'Q0001'), ('TK0010', 'Q0005'), ('TK0010', 'Q0007');
GO
-- ============================================================
-- PHAN 3: PROCEDURE, FUNCTION, VIEW, TRIGGER, CURSOR
-- ============================================================

-- ============================================================
-- Thanh Tấn
-- Procedure: Tạo tài khoản mới và gán quyền
-- Procedure: Đổi mật khẩu/ Khoá tài khoản
-- Function: Kiểm tra tài khoản có quyền x hay không
-- View: Danh sách tài khoản kèm danh sách quyền
-- ============================================================

CREATE OR ALTER PROCEDURE SP_TaoTaiKhoan
    @MATK        CHAR(6),
    @TENDANGNHAP VARCHAR(50),
    @MATKHAU     VARCHAR(255),
    @MaNV        CHAR(6)       = NULL,
    @MaGV        CHAR(6)       = NULL,
    @ChuoiQuyen  NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TAIKHOAN (MATK, TENDANGNHAP, MATKHAU, MaNV, MaGV, TRANGTHAI, NGAYTAO)
        VALUES (@MATK, @TENDANGNHAP, @MATKHAU, @MaNV, @MaGV, 1, GETDATE());

        IF @ChuoiQuyen IS NOT NULL AND LTRIM(RTRIM(@ChuoiQuyen)) <> ''
        BEGIN
            INSERT INTO TAIKHOAN_QUYEN (MATK, MAQUYEN)
            SELECT @MATK, LTRIM(RTRIM(value))
            FROM STRING_SPLIT(@ChuoiQuyen, ',');
        END

        COMMIT TRANSACTION;
        PRINT N'Tao tai khoan thanh cong: ' + @TENDANGNHAP;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT N'Loi khi tao tai khoan: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE SP_DoiMatKhau
    @TENDANGNHAP VARCHAR(50),
    @MATKHAU_CU  VARCHAR(255),
    @MATKHAU_MOI VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM TAIKHOAN
        WHERE TENDANGNHAP = @TENDANGNHAP AND MATKHAU = @MATKHAU_CU
    )
    BEGIN
        UPDATE TAIKHOAN
        SET MATKHAU = @MATKHAU_MOI
        WHERE TENDANGNHAP = @TENDANGNHAP;
        PRINT N'Doi mat khau thanh cong: ' + @TENDANGNHAP;
    END
    ELSE
        PRINT N'Ten dang nhap hoac mat khau cu khong chinh xac.';
END
GO

CREATE OR ALTER PROCEDURE SP_KhoaMoTaiKhoan
    @TENDANGNHAP VARCHAR(50),
    @TRANGTHAI   BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TENDANGNHAP = @TENDANGNHAP)
    BEGIN
        PRINT N'Tai khoan khong ton tai.';
        RETURN;
    END

    UPDATE TAIKHOAN SET TRANGTHAI = @TRANGTHAI
    WHERE TENDANGNHAP = @TENDANGNHAP;

    IF @TRANGTHAI = 0
        PRINT N'Da khoa tai khoan: ' + @TENDANGNHAP;
    ELSE
        PRINT N'Da mo khoa tai khoan: ' + @TENDANGNHAP;
END
GO

CREATE OR ALTER FUNCTION FN_KiemTraQuyen
(
    @TENDANGNHAP VARCHAR(50),
    @MAQUYEN     CHAR(5)
)
RETURNS BIT
AS
BEGIN
    DECLARE @KetQua BIT = 0;
    IF EXISTS (
        SELECT 1
        FROM TAIKHOAN TK
        JOIN TAIKHOAN_QUYEN TKQ ON TK.MATK = TKQ.MATK
        WHERE TK.TENDANGNHAP = @TENDANGNHAP
          AND TKQ.MAQUYEN    = @MAQUYEN
    )
        SET @KetQua = 1;
    RETURN @KetQua;
END
GO

CREATE OR ALTER VIEW VW_TaiKhoan_Quyen
AS
SELECT
    TK.MATK,
    TK.TENDANGNHAP,
    TK.TRANGTHAI                           AS TrangThaiHoatDong,
    TK.MaNV,
    TK.MaGV,
    COALESCE(NV.HoTen, GV.TenGV)          AS HoTenNguoiDung,
    STUFF((
        SELECT ', ' + Q.TENQUYEN
        FROM TAIKHOAN_QUYEN TKQ
        JOIN QUYEN Q ON TKQ.MAQUYEN = Q.MAQUYEN
        WHERE TKQ.MATK = TK.MATK
        FOR XML PATH(''), TYPE
    ).value('.','NVARCHAR(MAX)'), 1, 2, '') AS DanhSachQuyen
FROM TAIKHOAN TK
LEFT JOIN NHANVIEN NV ON TK.MaNV = NV.MaNV
LEFT JOIN GIAOVIEN GV ON TK.MaGV = GV.MaGV;
GO


-- ============================================================
-- Phi Long
-- Procedure: Thêm/cập nhật khóa học
-- Procedure: Thống kê số lớp đang mở theo từng khóa học
-- Function: Lấy học phí gốc của một khóa học
-- View: Danh sách khóa học kèm số lớp + tổng học viên đang học
-- ============================================================

CREATE OR ALTER PROCEDURE SP_ThemKhoaHoc
    @MAKH      CHAR(5),
    @TENKH     NVARCHAR(50),
    @SOBUOI    INT,
    @HOCPHI_GD MONEY,
    @MALOAI_KH CHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM LOAI_KHOAHOC WHERE MALOAI_KH = @MALOAI_KH)
    BEGIN
        PRINT N'Loai khoa hoc khong ton tai.';
        RETURN;
    END

    IF @SOBUOI <= 0 OR @HOCPHI_GD < 0
    BEGIN
        PRINT N'So buoi phai > 0 va hoc phi phai >= 0.';
        RETURN;
    END

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM KHOA_HOC WHERE MAKH = @MAKH)
        BEGIN
            UPDATE KHOA_HOC
            SET TENKH = @TENKH, SOBUOI = @SOBUOI,
                HOCPHI_GD = @HOCPHI_GD, MALOAI_KH = @MALOAI_KH
            WHERE MAKH = @MAKH;
            PRINT N'Cap nhat khoa hoc thanh cong: ' + @MAKH;
        END
        ELSE
        BEGIN
            INSERT INTO KHOA_HOC (MAKH, TENKH, SOBUOI, HOCPHI_GD, MALOAI_KH)
            VALUES (@MAKH, @TENKH, @SOBUOI, @HOCPHI_GD, @MALOAI_KH);
            PRINT N'Them khoa hoc thanh cong: ' + @MAKH;
        END
    END TRY
    BEGIN CATCH
        PRINT N'Loi: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE SP_ThongKeLopTheoKhoaHoc
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        KH.MAKH,
        KH.TENKH,
        COUNT(LH.MALOP)                            AS TongSoLop,
        SUM(CASE WHEN LH.TRANGTHAI = N'Dang mo'
                 THEN 1 ELSE 0 END)                AS SoLopDangMo,
        SUM(CASE WHEN LH.TRANGTHAI = N'Da ket thuc'
                 THEN 1 ELSE 0 END)                AS SoLopDaKetThuc
    FROM KHOA_HOC KH
    LEFT JOIN LOPHOC LH ON KH.MAKH = LH.MAKH
    GROUP BY KH.MAKH, KH.TENKH
    ORDER BY KH.MAKH;
END
GO

CREATE OR ALTER FUNCTION FN_LayHocPhiGoc
(
    @MAKH CHAR(5)
)
RETURNS MONEY
AS
BEGIN
    DECLARE @HocPhi MONEY;
    SELECT @HocPhi = HOCPHI_GD FROM KHOA_HOC WHERE MAKH = @MAKH;
    RETURN @HocPhi;
END
GO

CREATE OR ALTER VIEW VW_KhoaHoc_ThongKe
AS
SELECT
    KH.MAKH,
    KH.TENKH,
    LK.TENLOAI,
    KH.SOBUOI,
    KH.HOCPHI_GD,
    COUNT(DISTINCT LH.MALOP)                              AS TongSoLop,
    SUM(CASE WHEN LH.TRANGTHAI = N'Dang mo' THEN 1 ELSE 0 END) AS SoLopDangMo,
    COUNT(DISTINCT DK.MaHV)                               AS TongHocVienDangHoc
FROM KHOA_HOC KH
LEFT JOIN LOAI_KHOAHOC LK ON KH.MALOAI_KH = LK.MALOAI_KH
LEFT JOIN LOPHOC LH        ON KH.MAKH     = LH.MAKH
LEFT JOIN DANGKYLOP DK     ON LH.MALOP    = DK.MALOP
                          AND DK.TRANGTHAI = N'Dang hoc'
GROUP BY KH.MAKH, KH.TENKH, LK.TENLOAI, KH.SOBUOI, KH.HOCPHI_GD;
GO


-- ============================================================
-- Hương Duyên
-- Procedure: Thêm học viên mới + kiểm tra trùng SDT/Email
-- Procedure: Cập nhật trạng thái học viên
-- Function: Tính tuổi học viên từ NgaySinh
-- View: Danh sách giáo viên kèm số lớp đang dạy
-- ============================================================

CREATE OR ALTER PROCEDURE SP_ThemHocVien
    @MaHV     CHAR(6),
    @HoTen    NVARCHAR(50),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(5),
    @DiaChi   NVARCHAR(255),
    @SDT      VARCHAR(15),
    @Email    VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM HOCVIEN WHERE SDT = @SDT)
    BEGIN
        PRINT N'SDT da ton tai!';
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM HOCVIEN WHERE Email = @Email)
    BEGIN
        PRINT N'Email da ton tai!';
        RETURN;
    END

    INSERT INTO HOCVIEN (MaHV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, Email)
    VALUES (@MaHV, @HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SDT, @Email);
    PRINT N'Them hoc vien thanh cong!';
END
GO

CREATE OR ALTER PROCEDURE SP_CapNhatTrangThaiHocVien
    @MaHV     CHAR(6),
    @TrangThai NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM HOCVIEN WHERE MaHV = @MaHV)
    BEGIN
        PRINT N'Hoc vien khong ton tai!';
        RETURN;
    END
    UPDATE HOCVIEN SET TrangThai = @TrangThai WHERE MaHV = @MaHV;
    PRINT N'Cap nhat trang thai thanh cong!';
END
GO

CREATE OR ALTER FUNCTION FN_TinhTuoi
(
    @NgaySinh DATE
)
RETURNS INT
AS
BEGIN
    DECLARE @Tuoi INT = DATEDIFF(YEAR, @NgaySinh, GETDATE());
    IF (MONTH(@NgaySinh) > MONTH(GETDATE()))
       OR (MONTH(@NgaySinh) = MONTH(GETDATE()) AND DAY(@NgaySinh) > DAY(GETDATE()))
        SET @Tuoi = @Tuoi - 1;
    RETURN @Tuoi;
END
GO

CREATE OR ALTER VIEW VW_GiaoVien_SoLop
AS
SELECT
    GV.MaGV,
    GV.TenGV,
    GV.TrinhDo,
    COUNT(LH.MALOP) AS SoLopDangDay
FROM GIAOVIEN GV
LEFT JOIN LOPHOC LH ON GV.MaGV = LH.MaGV
                   AND LH.TRANGTHAI = N'Dang mo'
GROUP BY GV.MaGV, GV.TenGV, GV.TrinhDo;
GO


-- ============================================================
-- Minh Quân
-- Procedure: Mở lớp mới + tự động sinh lịch học theo SOBUOI của khóa
-- Procedure: Hủy lớp (cập nhật TRANGTHAI, xóa lịch tương lai)
-- Function: Kiểm tra phòng còn trống theo ca + ngày
-- Trigger: Khi NGAYKETTHUC đến → tự cập nhật TRANGTHAI = N'Đã kết thúc'
-- ============================================================

CREATE OR ALTER FUNCTION FN_KiemTraPhongTrong
(
    @MaPhong CHAR(5),
    @MaCa    CHAR(5),
    @NgayHoc DATE
)
RETURNS BIT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM LICHOC
        WHERE MAPHONG = @MaPhong AND MACA = @MaCa AND NGAYHOC = @NgayHoc
    )
        RETURN 0;
    RETURN 1;
END
GO

CREATE OR ALTER PROCEDURE SP_MoLopMoi
    @MaLop     CHAR(5),
    @TenLop    NVARCHAR(50),
    @MaKH      CHAR(5),
    @MaGV      CHAR(6),
    @NgayBatDau DATE,
    @MaPhong   CHAR(5),
    @MaCa      CHAR(5),
    @T2 BIT, @T3 BIT, @T4 BIT,
    @T5 BIT, @T6 BIT, @T7 BIT, @CN BIT
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFIRST 1;

    DECLARE @SoBuoi INT;
    SELECT @SoBuoi = SOBUOI FROM KHOA_HOC WHERE MAKH = @MaKH;
    IF @SoBuoi IS NULL
    BEGIN
        PRINT N'Khoa hoc khong ton tai.';
        RETURN;
    END

    INSERT INTO LOPHOC (MALOP, TENLOP, MAKH, MaGV, NGAYBATDAU, NGAYKETTHUC, TRANGTHAI)
    VALUES (@MaLop, @TenLop, @MaKH, @MaGV, @NgayBatDau, @NgayBatDau, N'Dang mo');

    DECLARE @Dem     INT  = 0;
    DECLARE @NgayCoi DATE = @NgayBatDau;
    DECLARE @NgayCuoi DATE = NULL;
    DECLARE @MaxLoop INT  = 730;

    WHILE @Dem < @SoBuoi AND @MaxLoop > 0
    BEGIN
        DECLARE @W INT = DATEPART(WEEKDAY, @NgayCoi);
        IF (@W=2 AND @T2=1) OR (@W=3 AND @T3=1) OR (@W=4 AND @T4=1)
        OR (@W=5 AND @T5=1) OR (@W=6 AND @T6=1) OR (@W=7 AND @T7=1)
        OR (@W=1 AND @CN=1)
        BEGIN
            IF dbo.FN_KiemTraPhongTrong(@MaPhong, @MaCa, @NgayCoi) = 1
            BEGIN
                INSERT INTO LICHOC (MALOP, NGAYHOC, MAPHONG, MACA)
                VALUES (@MaLop, @NgayCoi, @MaPhong, @MaCa);
                SET @Dem    = @Dem + 1;
                SET @NgayCuoi = @NgayCoi;
            END
            ELSE
                PRINT N'Phong bi trung ngay ' + CONVERT(NVARCHAR(10), @NgayCoi, 103);
        END
        SET @NgayCoi  = DATEADD(DAY, 1, @NgayCoi);
        SET @MaxLoop  = @MaxLoop - 1;
    END

    UPDATE LOPHOC
    SET NGAYKETTHUC = ISNULL(@NgayCuoi, @NgayBatDau)
    WHERE MALOP = @MaLop;

    PRINT N'Da tao lop ' + @MaLop + N' voi ' + CAST(@Dem AS VARCHAR) + N' buoi.';
END
GO

CREATE OR ALTER PROCEDURE SP_HuyLop
    @MaLop CHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM LOPHOC WHERE MALOP = @MaLop)
    BEGIN
        PRINT N'Lop khong ton tai.';
        RETURN;
    END

    UPDATE LOPHOC SET TRANGTHAI = N'Huy' WHERE MALOP = @MaLop;

    DELETE FROM LICHOC
    WHERE MALOP = @MaLop AND NGAYHOC >= CAST(GETDATE() AS DATE);

    PRINT N'Da huy lop va xoa lich hoc tuong lai.';
END
GO

CREATE OR ALTER TRIGGER TRG_TuDongKetThucLop
ON LOPHOC
AFTER UPDATE, INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE L
    SET TRANGTHAI = N'Da ket thuc'
    FROM LOPHOC L
    JOIN INSERTED i ON L.MALOP = i.MALOP
    WHERE L.NGAYKETTHUC < CAST(GETDATE() AS DATE)
      AND L.TRANGTHAI   = N'Dang mo';
END
GO


-- ============================================================
-- Hồng Vũ
-- Procedure: Đăng ký học viên vào lớp (kiểm tra sĩ số tối đa, không trùng lịch)
-- Procedure: Ghi nhận thu học phí → insert PHIEUTHU, cập nhật DANGKYLOP.TRANGTHAI
-- Function: Tính % chuyên cần của một học viên trong một lớp
-- Cursor: Duyệt danh sách lớp → tìm học viên vắng quá 3 buổi để in cảnh báo
-- ============================================================

CREATE OR ALTER PROCEDURE SP_DangKyLop
    @MaHV   CHAR(6),
    @MaLop  CHAR(5),
    @NgayDK DATETIME = NULL,
    @HocPhi MONEY    = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @NgayDK IS NULL SET @NgayDK = GETDATE();

    IF @HocPhi IS NULL
        SELECT @HocPhi = KH.HOCPHI_GD
        FROM KHOA_HOC KH
        JOIN LOPHOC LH ON KH.MAKH = LH.MAKH
        WHERE LH.MALOP = @MaLop;

    IF NOT EXISTS (SELECT 1 FROM HOCVIEN WHERE MaHV = @MaHV)
    BEGIN
        PRINT N'Hoc vien khong ton tai!';
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM LOPHOC WHERE MALOP = @MaLop AND TRANGTHAI = N'Dang mo')
    BEGIN
        PRINT N'Lop hoc khong ton tai hoac da dong!';
        RETURN;
    END

    DECLARE @SisoHT INT, @SisoMax INT;
    SELECT @SisoHT  = COUNT(*) FROM DANGKYLOP WHERE MALOP = @MaLop;
    SELECT @SisoMax = SISO_TOIDA FROM LOPHOC WHERE MALOP = @MaLop;
    IF @SisoHT >= @SisoMax
    BEGIN
        PRINT N'Lop da day si so!';
        RETURN;
    END

    IF EXISTS (
        SELECT 1 FROM LICHOC L1
        WHERE L1.MALOP = @MaLop
          AND EXISTS (
            SELECT 1 FROM DANGKYLOP DK
            JOIN LICHOC L2 ON DK.MALOP = L2.MALOP
            WHERE DK.MaHV  = @MaHV
              AND L1.NGAYHOC = L2.NGAYHOC
              AND L1.MACA    = L2.MACA
        )
    )
    BEGIN
        PRINT N'Hoc vien bi trung lich voi lop da dang ky!';
        RETURN;
    END

    BEGIN TRY
        INSERT INTO DANGKYLOP (MaHV, MALOP, NGAYDK, HOCPHI, TRANGTHAI)
        VALUES (@MaHV, @MaLop, @NgayDK, @HocPhi, N'Cho dong tien');
        PRINT N'Dang ky lop thanh cong!';
    END TRY
    BEGIN CATCH
        PRINT N'Loi he thong: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE SP_ThuHocPhi
    @MaPhieu    CHAR(10),
    @MaHV       CHAR(6),
    @MaLop      CHAR(5),
    @SoTien     MONEY,
    @NgayThu    DATETIME     = NULL,
    @HinhThuc   NVARCHAR(20) = N'Tien mat',
    @GhiChu     NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @NgayThu IS NULL SET @NgayThu = GETDATE();

    IF EXISTS (SELECT 1 FROM PHIEUTHU WHERE MAPHIEU = @MaPhieu)
    BEGIN
        PRINT N'Ma phieu thu da ton tai!';
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM DANGKYLOP WHERE MaHV = @MaHV AND MALOP = @MaLop)
    BEGIN
        PRINT N'Hoc vien chua dang ky lop nay!';
        RETURN;
    END

    DECLARE @HocPhiPhaiDong MONEY, @DaDong MONEY;
    SELECT @HocPhiPhaiDong = HOCPHI FROM DANGKYLOP WHERE MaHV = @MaHV AND MALOP = @MaLop;
    SELECT @DaDong = ISNULL(SUM(SOTIEN), 0) FROM PHIEUTHU WHERE MaHV = @MaHV AND MALOP = @MaLop;

    IF (@DaDong + @SoTien) > @HocPhiPhaiDong
    BEGIN
        PRINT N'So tien vuot qua so con no. Con no: ' + CAST((@HocPhiPhaiDong - @DaDong) AS VARCHAR(20));
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO PHIEUTHU (MAPHIEU, MaHV, MALOP, NGAYTHU, SOTIEN, HINHTHUCTHU, GHICHU)
        VALUES (@MaPhieu, @MaHV, @MaLop, @NgayThu, @SoTien, @HinhThuc, @GhiChu);

        IF (@DaDong + @SoTien) = @HocPhiPhaiDong
        BEGIN
            UPDATE DANGKYLOP SET TRANGTHAI = N'Dang hoc'
            WHERE MaHV = @MaHV AND MALOP = @MaLop;
            PRINT N'Hoan tat hoc phi. Trang thai: Dang hoc.';
        END

        COMMIT TRANSACTION;
        PRINT N'Ghi nhan phieu thu thanh cong!';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT N'Loi he thong: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER FUNCTION FN_TiLeChuyenCan
(
    @MaHV  CHAR(6),
    @MaLop CHAR(5)
)
RETURNS FLOAT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM DANGKYLOP WHERE MaHV = @MaHV AND MALOP = @MaLop)
        RETURN NULL;

    DECLARE @TongBuoi INT, @CoMat INT;

    SELECT @TongBuoi = COUNT(*) FROM DIEMDANH WHERE MaHV = @MaHV AND MALOP = @MaLop;
    IF @TongBuoi = 0 RETURN 0;

    SELECT @CoMat = COUNT(*) FROM DIEMDANH
    WHERE MaHV = @MaHV AND MALOP = @MaLop
      AND TRANGTHAI IN (N'Co mat', N'Tre');

    RETURN CAST(@CoMat AS FLOAT) / @TongBuoi * 100;
END
GO

CREATE OR ALTER PROCEDURE SP_CanhBaoVangNhieu
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaLop  CHAR(5);
    DECLARE @TenLop NVARCHAR(50);

    DECLARE cur_Lop CURSOR FOR
        SELECT MALOP, TENLOP FROM LOPHOC WHERE TRANGTHAI = N'Dang mo';

    OPEN cur_Lop;
    FETCH NEXT FROM cur_Lop INTO @MaLop, @TenLop;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT
            @TenLop                           AS TenLop,
            HV.MaHV,
            HV.HoTen,
            COUNT(*)                          AS SoBuoiVang
        FROM DIEMDANH DD
        JOIN HOCVIEN  HV ON DD.MaHV = HV.MaHV
        WHERE DD.MALOP     = @MaLop
          AND DD.TRANGTHAI IN (N'Vang CP', N'Vang KP')
        GROUP BY HV.MaHV, HV.HoTen
        HAVING COUNT(*) > 3;

        FETCH NEXT FROM cur_Lop INTO @MaLop, @TenLop;
    END

    CLOSE cur_Lop;
    DEALLOCATE cur_Lop;
END
GO
