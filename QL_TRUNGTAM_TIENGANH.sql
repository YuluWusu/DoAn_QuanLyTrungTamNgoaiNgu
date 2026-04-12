CREATE DATABASE QL_TRUNGTAM_TIENGANH
GO
USE QL_TRUNGTAM_TIENGANH
GO
-- 1. BẢNG LOẠI KHÓA HỌC
CREATE TABLE LOAI_KHOAHOC
(
	MALOAI_KH NCHAR(5) NOT NULL,
	TENLOAI NVARCHAR(50) NOT NULL,
	CONSTRAINT PK_LOAI_KHOAHOC PRIMARY KEY (MALOAI_KH)
)
GO
-- 2. BẢNG KHÓA HỌC
CREATE TABLE KHOA_HOC
(
	MAKH NCHAR(5) NOT NULL,
	TENKH NVARCHAR(50) NOT NULL,
	HOCPHI FLOAT CHECK(HOCPHI>=0),
	SOBUOI INT CHECK(SOBUOI > 0),
	MALOAI_KH NCHAR(5),
	CONSTRAINT PK_KHOA_HOC PRIMARY KEY (MAKH),
	CONSTRAINT FK_KHOA_HOC_LOAI_KHOA_HOC FOREIGN KEY (MALOAI_KH) REFERENCES LOAI_KHOAHOC(MALOAI_KH)
)
GO
-- 3. BẢNG GIÁO VIÊN
CREATE TABLE GIAOVIEN
(
    MaGV char(6)not null,
    TenGV nvarchar(50)not null,
    SDT nvarchar(50)UNIQUE,
    Email char(50)UNIQUE,
    DiaChi nvarchar(100),
    TrinhDo nvarchar(100) Check(TrinhDo In (N'Cử nhân',N'Thạc sĩ',N'Tiến Sĩ')),
    NgayVaoLam date Check(NgayVaoLam<=GetDate()),
    CONSTRAINT PK_GiaoVien PRIMARY KEY (MaGV)
);
GO
-- 4. BẢNG HỌC VIÊN
CREATE TABLE HOCVIEN (
    MaHV CHAR(6) not null,
    HoTen nvarchar(50)not null,
    NgaySinh DATE Check(NgaySinh<GetDate()),
    GioiTinh NVARCHAR(10) Check(GioiTinh In (N'Nam',N'Nữ')),
    DiaChi NVARCHAR(255),
    SDT char(15)UNIQUE,
    Email char(50)UNIQUE,
    TrangThai nvarchar(50) Check(TrangThai In(N'Đang học',N'Bảo lưu',N'Nghỉ',N'Hoàn Thành')),
    CONSTRAINT PK_HocVien PRIMARY KEY (MaHV),
);
GO
-- 5. BẢNG PHÒNG HỌC
CREATE TABLE PHONGHOC
(
	MAPHONG CHAR(5) PRIMARY KEY,
	TENPHONG NVARCHAR(50),
	SOGHENGOI INT,
)
GO
-- 6. BẢNG CA HỌC
CREATE TABLE CAHOC
(
	MACA CHAR(5) PRIMARY KEY,
	TENCA NVARCHAR(50) NOT NULL,
	GIOBATDAU TIME NOT NULL,
	GIOKETTHUC TIME NOT NULL,
	CHECK (GIOBATDAU < GIOKETTHUC)
);
GO
-- 7. BẢNG LỚP HỌC
CREATE TABLE LOPHOC
(
	MALOP CHAR(5) PRIMARY KEY,
	TENLOP NVARCHAR(50) NOT NULL,
	MAKH NCHAR(5),
	MAGV CHAR(6),
	MAPHONG CHAR(5),
	MACA CHAR(5),
	NGAYBATDAU DATE NOT NULL,
	NGAYKETTHUC DATE NOT NULL,
	FOREIGN KEY (MAKH) REFERENCES KHOA_HOC(MAKH),
	FOREIGN KEY (MAGV) REFERENCES GIAOVIEN(MaGV),
	FOREIGN KEY (MAPHONG) REFERENCES PHONGHOC(MAPHONG),
	FOREIGN KEY (MACA) REFERENCES CAHOC(MACA),
	CHECK (NGAYBATDAU <= NGAYKETTHUC)
)
GO
-- 8. BẢNG PHÂN CÔNG PHÒNG (LỊCH HỌC CHI TIẾT)
CREATE TABLE PHANCONGPHONG
(
	MALOP CHAR(5) NOT NULL,
	MAPHONG CHAR(5) NOT NULL,
	MACA CHAR(5) NOT NULL,
	NGAYHOC DATE NOT NULL,
	PRIMARY KEY(MAPHONG, MACA, NGAYHOC),
	FOREIGN KEY (MALOP) REFERENCES LOPHOC(MALOP),
	FOREIGN KEY (MAPHONG) REFERENCES PHONGHOC(MAPHONG),
	FOREIGN KEY (MACA) REFERENCES CAHOC(MACA)
)
GO
-- 9. BẢNG ĐĂNG KÝ CHỨNG CHỈ
CREATE TABLE DANGKYCHUNGCHI
(
    MAHV CHAR(6) NOT NULL,
    MALOP CHAR(5) NOT NULL,
    NGAYDK DATETIME DEFAULT GETDATE(),
    HOCPHI MONEY CHECK (HOCPHI >= 0), 
    TRANGTHAI NVARCHAR(30) DEFAULT N'Chờ đóng tiền' CHECK (TRANGTHAI IN (N'Chờ đóng tiền', N'Đang học', N'Bảo lưu', N'Hoàn thành', N'Hủy')),
    
    CONSTRAINT PK_DKCC PRIMARY KEY (MAHV, MALOP),
    CONSTRAINT FK_DKCC_HV FOREIGN KEY (MAHV) REFERENCES HOCVIEN(MAHV),
    CONSTRAINT FK_DKCC_ML FOREIGN KEY (MALOP) REFERENCES LOPHOC(MALOP)  
);
GO
-- 10. BẢNG PHIẾU THU
CREATE TABLE PHIEUTHU
(
    MAPHIEU CHAR(10) NOT NULL, 
    MAHV CHAR(6) NOT NULL,
    MALOP CHAR(5) NOT NULL,
    NGAYTHU DATETIME DEFAULT GETDATE(),
    SOTIEN MONEY CHECK (SoTien > 0), 
    HINHTHUTHU NVARCHAR(50) DEFAULT N'Tiền mặt',
    GHICHU NVARCHAR(200),

    CONSTRAINT PK_PT PRIMARY KEY (MAPHIEU),
    CONSTRAINT FK_PT_DK FOREIGN KEY (MAHV, MALOP) REFERENCES DANGKYCHUNGCHI(MAHV, MALOP)
);
GO
-- 11. BẢNG ĐIỂM DANH
CREATE TABLE DIEMDANH
(
    MAHV CHAR(6) NOT NULL,
    MALOP CHAR(5) NOT NULL,
    NGAYDD DATE NOT NULL,
    TRANGTHAI NVARCHAR(20) DEFAULT N'Có mặt' CHECK (TRANGTHAI IN (N'Có mặt', N'Vắng CP', N'Vắng KP', N'Trễ')),
    GHICHU NVARCHAR(100), 
    CONSTRAINT PK_DD PRIMARY KEY (MAHV, MALOP, NGAYDD), 
    CONSTRAINT FK_DD_DK FOREIGN KEY (MAHV, MALOP) REFERENCES DANGKYCHUNGCHI(MAHV, MALOP)
);
GO
-- 12. BẢNG TÀI KHOẢN
CREATE TABLE TAIKHOAN (
    MATK CHAR(5) PRIMARY KEY,
    TENDANGNHAP VARCHAR(50) UNIQUE NOT NULL,
    MATKHAU VARCHAR(255) NOT NULL,
    HOTEN NVARCHAR(100),
    EMAIL VARCHAR(100),
    TRANGTHAI BIT DEFAULT 1,
    NGAYTAO DATETIME DEFAULT GETDATE()
);
GO
-- 13. BẢNG QUYỀN
CREATE TABLE QUYEN (
    MAQUYEN CHAR(5) PRIMARY KEY,
    TENQUYEN VARCHAR(100) UNIQUE NOT NULL,
    MOTA NVARCHAR(255)
);
GO
-- 14. BẢNG TÀI KHOẢN_QUYỀN
CREATE TABLE TAIKHOAN_QUYEN (
    MATK CHAR(5),
    MAQUYEN CHAR(5),
    PRIMARY KEY (MATK, MAQUYEN),
    FOREIGN KEY (MATK) REFERENCES TAIKHOAN(MATK),
    FOREIGN KEY (MAQUYEN) REFERENCES QUYEN(MAQUYEN)
);
GO
-- 1. INSERT LOAI_KHOAHOC
INSERT INTO LOAI_KHOAHOC (MALOAI_KH, TENLOAI) VALUES
('L01', N'Tiếng Anh giao tiếp'),
('L02', N'Tiếng Anh trẻ em'),
('L03', N'Luyện thi IELTS'),
('L04', N'Luyện thi TOEIC'),
('L05', N'Luyện thi TOEFL'),
('L06', N'Luyện thi VSTEP')
GO

-- 2. INSERT KHOA_HOC

INSERT INTO KHOA_HOC (MAKH, TENKH, HOCPHI, SOBUOI, MALOAI_KH) VALUES
('K001', N'Giao tiếp cơ bản 1', 2500000, 12, 'L01'),
('K002', N'Giao tiếp cơ bản 2', 2700000, 12, 'L01'),
('K003', N'Giao tiếp trung cấp', 3200000, 15, 'L01'),
('K004', N'Giao tiếp nâng cao', 3800000, 15, 'L01'),
('K005', N'Phát âm chuẩn Anh-Mỹ', 2000000, 10, 'L01'),
('K006', N'Luyện phản xạ giao tiếp', 3500000, 14, 'L01');


INSERT INTO KHOA_HOC (MAKH, TENKH, HOCPHI, SOBUOI, MALOAI_KH) VALUES
('K007', N'Super Kids 1 (4-6 tuổi)', 3000000, 16, 'L02'),
('K008', N'Super Kids 2 (4-6 tuổi)', 3200000, 16, 'L02'),
('K009', N'Young Learners 1 (7-9 tuổi)', 3500000, 18, 'L02'),
('K010', N'Young Learners 2 (7-9 tuổi)', 3700000, 18, 'L02'),
('K011', N'Tiếng Anh qua bài hát', 2200000, 10, 'L02'),
('K012', N'Phonics - Đánh vần tiếng Anh', 2800000, 14, 'L02');


INSERT INTO KHOA_HOC (MAKH, TENKH, HOCPHI, SOBUOI, MALOAI_KH) VALUES
('K013', N'IELTS Foundation 4.0-5.0', 6000000, 24, 'L03'),
('K014', N'IELTS Intermediate 5.0-6.0', 7000000, 24, 'L03'),
('K015', N'IELTS Advanced 6.0-7.0', 8500000, 24, 'L03'),
('K016', N'IELTS Intensive 7.0+', 10000000, 28, 'L03'),
('K017', N'Luyện IELTS Listening', 3500000, 12, 'L03'),
('K018', N'Luyện IELTS Reading', 3500000, 12, 'L03'),
('K019', N'Luyện IELTS Writing', 4500000, 14, 'L03'),
('K020', N'Luyện IELTS Speaking', 4000000, 12, 'L03'),
('K021', N'IELTS Cấp tốc', 9000000, 20, 'L03');


INSERT INTO KHOA_HOC (MAKH, TENKH, HOCPHI, SOBUOI, MALOAI_KH) VALUES
('K022', N'TOEIC Foundation 300-450', 5000000, 20, 'L04'),
('K023', N'TOEIC Intermediate 450-650', 6000000, 22, 'L04'),
('K024', N'TOEIC Advanced 650-850', 7500000, 24, 'L04'),
('K025', N'TOEIC Listening trọng tâm', 3800000, 14, 'L04'),
('K026', N'TOEIC Reading trọng tâm', 3800000, 14, 'L04'),
('K027', N'TOEIC 4 kỹ năng', 5500000, 20, 'L04'),
('K028', N'TOEIC Cấp tốc', 6800000, 16, 'L04');


INSERT INTO KHOA_HOC (MAKH, TENKH, HOCPHI, SOBUOI, MALOAI_KH) VALUES
('K029', N'TOEFL iBT Foundation', 8000000, 24, 'L05'),
('K030', N'TOEFL iBT Advanced', 10000000, 26, 'L05'),
('K031', N'TOEFL Intensive', 12000000, 30, 'L05'),
('K032', N'TOEFL Reading & Writing', 6000000, 18, 'L05'),
('K033', N'TOEFL Listening & Speaking', 6000000, 18, 'L05');


INSERT INTO KHOA_HOC (MAKH, TENKH, HOCPHI, SOBUOI, MALOAI_KH) VALUES
('K034', N'VSTEP B1', 3500000, 20, 'L06'),
('K035', N'VSTEP B2', 4000000, 22, 'L06'),
('K036', N'VSTEP C1', 5000000, 24, 'L06');
GO
-- 3. INSERT GIAOVIEN
INSERT INTO GIAOVIEN (MaGV, TenGV, SDT, Email, DiaChi, TrinhDo, NgayVaoLam)
VALUES
('GV001', N'Nguyễn Hoàng Anh', '0901000001', 'hoanganh01@gmail.com', N'Q1 TP.HCM', N'Thạc sĩ', '2020-01-10'),
('GV002', N'Nguyễn Thị Mỹ Linh', '0901000002', 'mylinh02@gmail.com', N'Q3 TP.HCM', N'Thạc sĩ', '2019-03-12'),
('GV003', N'Trần Văn Minh', '0901000003', 'vanminh03@gmail.com', N'Q5 TP.HCM', N'Cử nhân', '2019-06-20'),
('GV004', N'Lê Thị Thu Hằng', '0901000004', 'thuhang04@gmail.com', N'Q7 TP.HCM', N'Thạc sĩ', '2019-08-15'),
('GV005', N'Phạm Quốc Huy', '0901000005', 'quochuy05@gmail.com', N'Thủ Đức', N'Cử nhân', '2021-02-01'),
('GV006', N'Võ Thanh Tâm', '0901000006', 'thanhtam06@gmail.com', N'Bình Thạnh', N'Thạc sĩ', '2017-11-11'),
('GV007', N'Nguyễn Minh Khoa', '0901000007', 'minhkhoa07@gmail.com', N'Gò Vấp', N'Cử nhân', '2018-01-05'),
('GV008', N'Đặng Thị Ngọc Ánh', '0901000008', 'ngocanh08@gmail.com', N'Q10 TP.HCM', N'Thạc sĩ', '2020-09-09'),
('GV009', N'Huỳnh Quốc Bảo', '0901000009', 'quocbao09@gmail.com', N'Q8 TP.HCM', N'Cử nhân', '2018-06-08'),
('GV010', N'Phan Thị Kim Oanh', '0901000010', 'kimoanh10@gmail.com', N'Q6 TP.HCM', N'Thạc sĩ', '2016-12-25'),
('GV011', N'Nguyễn Văn Long', '0901000011', 'vanlong11@gmail.com', N'Q12 TP.HCM', N'Cử nhân', '2017-12-04'),
('GV012', N'Lý Thị Bích Ngọc', '0901000012', 'bichngoc12@gmail.com', N'Tân Bình', N'Thạc sĩ', '2022-11-09'),
('GV013', N'Bùi Anh Tuấn', '0901000013', 'anhtuan13@gmail.com', N'Phú Nhuận', N'Cử nhân', '2020-10-10'),
('GV014', N'Trương Thị Thanh Tuyền', '0901000014', 'thanhtuyen14@gmail.com', N'Q11 TP.HCM', N'Thạc sĩ', '2021-09-10'),
('GV015', N'Nguyễn Nhật Nam', '0901000015', 'nhatnam15@gmail.com', N'Q4 TP.HCM', N'Cử nhân', '2016-06-03');
GO
-- 4. INSERT HOCVIEN
INSERT INTO HOCVIEN(MaHV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, Email, TrangThai)
VALUES
('HV001', N'Nguyễn Minh An', '2005-01-12', N'Nam', N'Q1 TP.HCM', '0912000001', 'minhan01@gmail.com', N'Đang học'),
('HV002', N'Nguyễn Thị Mai', '2005-02-20', N'Nữ', N'Q3 TP.HCM', '0912000002', 'thimai02@gmail.com', N'Đang học'),
('HV003', N'Nguyễn Quốc Đạt', '2004-07-22', N'Nam', N'Q5 TP.HCM', '0912000003', 'quocdat03@gmail.com', N'Đang học'),
('HV004', N'Nguyễn Gia Huy', '2004-11-02', N'Nam', N'Q7 TP.HCM', '0912000004', 'giahuy04@gmail.com', N'Đang học'),
('HV005', N'Nguyễn Thị Ngọc', '2005-05-09', N'Nữ', N'Q10 TP.HCM', '0912000005', 'thingoc05@gmail.com', N'Đang học'),
('HV006', N'Nguyễn Hoàng Long', '2004-02-14', N'Nam', N'Thủ Đức', '0912000006', 'hoanglong06@gmail.com', N'Đang học'),
('HV007', N'Nguyễn Minh Khang', '2005-06-25', N'Nam', N'Bình Thạnh', '0912000007', 'minhkhang07@gmail.com', N'Đang học'),
('HV008', N'Nguyễn Thu Trang', '2005-08-30', N'Nữ', N'Q8 TP.HCM', '0912000008', 'thutrang08@gmail.com', N'Đang học'),
('HV009', N'Nguyễn Anh Tuấn', '2004-10-10', N'Nam', N'Q12 TP.HCM', '0912000009', 'anhtuan09@gmail.com', N'Đang học'),
('HV010', N'Nguyễn Thảo Vy', '2005-12-05', N'Nữ', N'Gò Vấp', '0912000010', 'thaovy10@gmail.com', N'Đang học'),
('HV011', N'Nguyễn Quốc Bảo', '2004-04-19', N'Nam', N'Q11 TP.HCM', '0912000011', 'quocbao11@gmail.com', N'Đang học'),
('HV012', N'Nguyễn Minh Trí', '2005-09-21', N'Nam', N'Q6 TP.HCM', '0912000012', 'minhtri12@gmail.com', N'Đang học'),
('HV013', N'Nguyễn Nhật Nam', '2004-01-08', N'Nam', N'Q9 TP.HCM', '0912000013', 'nhatnam13@gmail.com', N'Đang học'),
('HV014', N'Nguyễn Kim Chi', '2005-07-17', N'Nữ', N'Q4 TP.HCM', '0912000014', 'kimchi14@gmail.com', N'Đang học'),
('HV015', N'Nguyễn Anh Quân', '2004-03-03', N'Nam', N'Q2 TP.HCM', '0912000015', 'anhquan15@gmail.com', N'Đang học');
GO
-- 5. INSERT PHONGHOC
INSERT INTO PHONGHOC VALUES
('P001', N'Phòng A101', 30),
('P002', N'Phòng A102', 30),
('P003', N'Phòng A103', 30),
('P004', N'Phòng A201', 35),
('P005', N'Phòng A202', 35),
('P006', N'Phòng B101', 40),
('P007', N'Phòng B102', 40),
('P008', N'Phòng B201', 45),
('P009', N'Phòng C101', 25),
('P010', N'Phòng C102', 25);
GO
-- 6. INSERT CAHOC
INSERT INTO CAHOC VALUES
('C001', N'Sáng 1', '07:00', '09:00'),
('C002', N'Sáng 2', '09:30', '11:30'),
('C003', N'Chiều 1', '13:30', '15:30'),
('C004', N'Chiều 2', '15:45', '17:45'),
('C005', N'Tối 1', '18:00', '20:00'),
('C006', N'Tối 2', '20:15', '22:15');
GO
-- 7. INSERT LOPHOC
INSERT INTO LOPHOC VALUES
-- Giao tiếp
('L001', N'Giao tiếp cơ bản 1', 'K001', 'GV001', 'P001', 'C001', '2026-02-01', '2026-04-30'),
('L002', N'Giao tiếp cơ bản 2', 'K002', 'GV002', 'P002', 'C002', '2026-02-01', '2026-04-30'),
('L003', N'Giao tiếp nâng cao', 'K004', 'GV003', 'P003', 'C003', '2026-02-05', '2026-05-05'),

('L004', N'Super Kids 1', 'K007', 'GV004', 'P009', 'C002', '2026-02-02', '2026-05-02'),
('L005', N'Young Learners', 'K009', 'GV005', 'P010', 'C003', '2026-02-03', '2026-05-03'),

('L006', N'IELTS Foundation', 'K013', 'GV006', 'P004', 'C001', '2026-02-01', '2026-05-31'),
('L007', N'IELTS Intermediate', 'K014', 'GV007', 'P005', 'C002', '2026-02-01', '2026-05-31'),
('L008', N'IELTS Advanced', 'K015', 'GV008', 'P006', 'C003', '2026-02-05', '2026-06-05'),
('L009', N'IELTS Intensive', 'K016', 'GV009', 'P007', 'C004', '2026-02-10', '2026-06-10'),
('L010', N'IELTS Cấp tốc', 'K021', 'GV010', 'P008', 'C005', '2026-03-01', '2026-06-01'),

('L011', N'TOEIC Foundation', 'K022', 'GV011', 'P004', 'C003', '2026-02-05', '2026-05-05'),
('L012', N'TOEIC Intermediate', 'K023', 'GV012', 'P005', 'C004', '2026-02-05', '2026-05-05'),
('L013', N'TOEIC Advanced', 'K024', 'GV013', 'P006', 'C005', '2026-02-10', '2026-06-10'),

('L014', N'TOEFL Foundation', 'K029', 'GV014', 'P007', 'C002', '2026-02-15', '2026-06-15'),
('L015', N'TOEFL Advanced', 'K030', 'GV015', 'P008', 'C003', '2026-02-15', '2026-06-15');
GO
-- 8. INSERT PHANCONGPHONG
INSERT INTO PHANCONGPHONG VALUES

('L001', 'P001', 'C001', '2026-02-01'),
('L001', 'P001', 'C001', '2026-02-03'),
('L001', 'P001', 'C001', '2026-02-05'),
('L001', 'P001', 'C001', '2026-02-08'),
('L001', 'P001', 'C001', '2026-02-10'),

('L002', 'P002', 'C002', '2026-02-02'),
('L002', 'P002', 'C002', '2026-02-04'),
('L002', 'P002', 'C002', '2026-02-06'),
('L002', 'P002', 'C002', '2026-02-09'),
('L002', 'P002', 'C002', '2026-02-11'),

('L006', 'P004', 'C001', '2026-02-01'),
('L006', 'P004', 'C001', '2026-02-03'),
('L006', 'P004', 'C001', '2026-02-05'),
('L006', 'P004', 'C001', '2026-02-08'),
('L006', 'P004', 'C001', '2026-02-10'),

('L007', 'P005', 'C002', '2026-02-02'),
('L007', 'P005', 'C002', '2026-02-04'),
('L007', 'P005', 'C002', '2026-02-06'),
('L007', 'P005', 'C002', '2026-02-09'),
('L007', 'P005', 'C002', '2026-02-11'),

('L011', 'P004', 'C003', '2026-02-05'),
('L011', 'P004', 'C003', '2026-02-07'),
('L011', 'P004', 'C003', '2026-02-12'),
('L011', 'P004', 'C003', '2026-02-14'),
('L011', 'P004', 'C003', '2026-02-19');
GO
-- 9. INSERT DANGKYCHUNGCHI
INSERT INTO DANGKYCHUNGCHI (MAHV, MALOP, NGAYDK, HOCPHI, TRANGTHAI) VALUES
('HV001', 'L001', '2026-01-15', 2500000, N'Đang học'),
('HV002', 'L001', '2026-01-16', 2500000, N'Đang học'),
('HV003', 'L002', '2026-01-15', 2700000, N'Đang học'),
('HV004', 'L002', '2026-01-17', 2700000, N'Đang học'),
('HV005', 'L003', '2026-01-20', 3800000, N'Đang học'),
('HV006', 'L006', '2026-01-10', 6000000, N'Đang học'),
('HV007', 'L006', '2026-01-12', 6000000, N'Đang học'),
('HV008', 'L007', '2026-01-10', 7000000, N'Đang học'),
('HV009', 'L007', '2026-01-11', 7000000, N'Bảo lưu'),
('HV010', 'L008', '2026-01-15', 8500000, N'Đang học'),
('HV011', 'L009', '2026-01-20', 10000000, N'Chờ đóng tiền'),
('HV012', 'L011', '2026-01-18', 5000000, N'Đang học'),
('HV013', 'L011', '2026-01-19', 5000000, N'Đang học'),
('HV014', 'L012', '2026-01-20', 6000000, N'Đang học'),
('HV015', 'L012', '2026-01-21', 6000000, N'Đang học');
-- 10. INSERT PHIEUTHU
INSERT INTO PHIEUTHU (MAPHIEU, MAHV, MALOP, NGAYTHU, SOTIEN, HINHTHUTHU, GHICHU) VALUES
('PT0001', 'HV001', 'L001', '2026-01-15', 2500000, N'Tiền mặt', N'Đóng đủ'),
('PT0002', 'HV002', 'L001', '2026-01-16', 2500000, N'Chuyển khoản', N'Đóng đủ'),
('PT0003', 'HV003', 'L002', '2026-01-15', 2700000, N'Tiền mặt', N'Đóng đủ'),
('PT0004', 'HV004', 'L002', '2026-01-17', 2700000, N'Tiền mặt', N'Đóng đủ'),
('PT0005', 'HV005', 'L003', '2026-01-20', 3800000, N'Chuyển khoản', N'Đóng đủ'),
('PT0006', 'HV006', 'L006', '2026-01-10', 6000000, N'Tiền mặt', N'Đóng đủ'),
('PT0007', 'HV007', 'L006', '2026-01-12', 6000000, N'Chuyển khoản', N'Đóng đủ'),
('PT0008', 'HV008', 'L007', '2026-01-10', 7000000, N'Tiền mặt', N'Đóng đủ'),
('PT0009', 'HV009', 'L007', '2026-01-11', 7000000, N'Chuyển khoản', N'Đóng đủ'),
('PT0010', 'HV010', 'L008', '2026-01-15', 8500000, N'Tiền mặt', N'Đóng đủ'),
('PT0011', 'HV011', 'L009', '2026-01-20', 5000000, N'Chuyển khoản', N'Đóng đợt 1'),
('PT0012', 'HV012', 'L011', '2026-01-18', 5000000, N'Tiền mặt', N'Đóng đủ'),
('PT0013', 'HV013', 'L011', '2026-01-19', 5000000, N'Chuyển khoản', N'Đóng đủ'),
('PT0014', 'HV014', 'L012', '2026-01-20', 6000000, N'Tiền mặt', N'Đóng đủ'),
('PT0015', 'HV015', 'L012', '2026-01-21', 6000000, N'Chuyển khoản', N'Đóng đủ');
GO
-- 11. INSERT DIEMDANH
INSERT INTO DIEMDANH (MAHV, MALOP, NGAYDD, TRANGTHAI, GHICHU) VALUES

('HV001', 'L001', '2026-02-01', N'Có mặt', NULL),
('HV001', 'L001', '2026-02-03', N'Có mặt', NULL),
('HV002', 'L001', '2026-02-01', N'Có mặt', NULL),
('HV002', 'L001', '2026-02-03', N'Có mặt', NULL),
('HV002', 'L001', '2026-02-05', N'Trễ', N'Kẹt xe'),

('HV003', 'L002', '2026-02-02', N'Có mặt', NULL),
('HV003', 'L002', '2026-02-04', N'Có mặt', NULL),
('HV004', 'L002', '2026-02-02', N'Có mặt', NULL),
('HV004', 'L002', '2026-02-04', N'Vắng CP', N'Bệnh'),

('HV006', 'L006', '2026-02-01', N'Có mặt', NULL),
('HV006', 'L006', '2026-02-03', N'Có mặt', NULL),
('HV007', 'L006', '2026-02-01', N'Có mặt', NULL),
('HV007', 'L006', '2026-02-03', N'Có mặt', NULL),

('HV008', 'L007', '2026-02-02', N'Có mặt', NULL),
('HV008', 'L007', '2026-02-04', N'Có mặt', NULL),
('HV009', 'L007', '2026-02-02', N'Có mặt', NULL),
('HV009', 'L007', '2026-02-04', N'Vắng KP', N'Không lý do');
GO
-- 12. INSERT TAIKHOAN
INSERT INTO TAIKHOAN (MATK, TENDANGNHAP, MATKHAU, HOTEN, EMAIL)
VALUES
('TK001', 'admin', 'Admin@123', N'Nguyễn Thanh Tấn', 'tan@ttav.com'),
('TK002', 'nvlan', 'Lan@123', N'Nguyễn Thị Lan', 'lan@ttav.com'),
('TK003', 'nvhuy', 'Huy@123', N'Nguyễn Văn Huy', 'huy@ttav.com'),
('TK004', 'gvhoanganh', 'Anh@123', N'Nguyễn Hoàng Anh', 'anh@ttav.com'),
('TK005', 'gvmylinh', 'Linh@123', N'Nguyễn Thị Mỹ Linh', 'linh@ttav.com'),
('TK006', 'hvminhan', 'An@123', N'Nguyễn Minh An', 'minhan@mail.com'),
('TK007', 'hvthimai', 'Mai@123', N'Nguyễn Thị Mai', 'thimai@mail.com'),
('TK008', 'hvquocdat', 'Dat@123', N'Nguyễn Quốc Đạt', 'quocdat@mail.com'),
('TK009', 'hvgiahuy', 'Huy@456', N'Nguyễn Gia Huy', 'giahuy@mail.com'),
('TK010', 'hvthingoc', 'Ngoc@123', N'Nguyễn Thị Ngọc', 'thingoc@mail.com'),
('TK011', 'hvhoanglong', 'Long@123', N'Nguyễn Hoàng Long', 'hoanglong@mail.com'),
('TK012', 'hvminhkhang', 'Khang@123', N'Nguyễn Minh Khang', 'minhkhang@mail.com'),
('TK013', 'hvthutrang', 'Trang@123', N'Nguyễn Thu Trang', 'thutrang@mail.com'),
('TK014', 'hvanhtuan', 'Tuan@123', N'Nguyễn Anh Tuấn', 'anhtuan@mail.com'),
('TK015', 'hvthaovy', 'Vy@123', N'Nguyễn Thảo Vy', 'thaovy@mail.com');
GO
-- 13. INSERT QUYEN
INSERT INTO QUYEN (MAQUYEN, TENQUYEN, MOTA)
VALUES
('Q001', 'QUAN_TRI_HE_THONG', N'Quản trị toàn bộ hệ thống'),
('Q002', 'QUAN_LY_TAI_KHOAN', N'Quản lý tài khoản'),
('Q003', 'QUAN_LY_KHOA_HOC', N'Quản lý khóa học'),
('Q004', 'QUAN_LY_HOC_VIEN', N'Quản lý học viên'),
('Q005', 'GIANG_DAY', N'Giảng dạy'),
('Q006', 'XEM_THONG_TIN', N'Xem thông tin'),
('Q007', 'DANG_KY_KHOA_HOC', N'Đăng ký khóa học');
GO

-- 14. INSERT TAIKHOAN_QUYEN

INSERT INTO TAIKHOAN_QUYEN
SELECT 'TK001', MAQUYEN FROM QUYEN;


INSERT INTO TAIKHOAN_QUYEN VALUES
('TK002','Q002'), ('TK002','Q003'), ('TK002','Q004'),
('TK003','Q002'), ('TK003','Q003'), ('TK003','Q004');


INSERT INTO TAIKHOAN_QUYEN VALUES
('TK004','Q005'), ('TK004','Q006'),
('TK005','Q005'), ('TK005','Q006');


INSERT INTO TAIKHOAN_QUYEN
SELECT MATK, 'Q006' FROM TAIKHOAN WHERE MATK >= 'TK006';
INSERT INTO TAIKHOAN_QUYEN
SELECT MATK, 'Q007' FROM TAIKHOAN WHERE MATK >= 'TK006';
GO

SELECT 'LOAI_KHOAHOC' AS TABLE_NAME, COUNT(*) AS ROW_COUNT FROM LOAI_KHOAHOC
UNION ALL
SELECT 'KHOA_HOC', COUNT(*) FROM KHOA_HOC
UNION ALL
SELECT 'GIAOVIEN', COUNT(*) FROM GIAOVIEN
UNION ALL
SELECT 'HOCVIEN', COUNT(*) FROM HOCVIEN
UNION ALL
SELECT 'PHONGHOC', COUNT(*) FROM PHONGHOC
UNION ALL
SELECT 'CAHOC', COUNT(*) FROM CAHOC
UNION ALL
SELECT 'LOPHOC', COUNT(*) FROM LOPHOC
UNION ALL
SELECT 'PHANCONGPHONG', COUNT(*) FROM PHANCONGPHONG
UNION ALL
SELECT 'DANGKYCHUNGCHI', COUNT(*) FROM DANGKYCHUNGCHI
UNION ALL
SELECT 'PHIEUTHU', COUNT(*) FROM PHIEUTHU
UNION ALL
SELECT 'DIEMDANH', COUNT(*) FROM DIEMDANH
UNION ALL
SELECT 'TAIKHOAN', COUNT(*) FROM TAIKHOAN
UNION ALL
SELECT 'QUYEN', COUNT(*) FROM QUYEN
UNION ALL
SELECT 'TAIKHOAN_QUYEN', COUNT(*) FROM TAIKHOAN_QUYEN
ORDER BY TABLE_NAME;
GO