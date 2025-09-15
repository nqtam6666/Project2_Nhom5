CREATE DATABASE BanVeXemPhim;
GO
USE [BanVeXemPhim]
GO
/****** Object:  Table [dbo].[DoanhThu]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DoanhThu](
	[MaDoanhThu] [int] IDENTITY(1,1) NOT NULL,
	[MaSuatChieu] [int] NULL,
	[TongTien] [decimal](15, 2) NULL,
	[HoaHongDaiLy] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaDoanhThu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ghe]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ghe](
	[MaGhe] [int] IDENTITY(1,1) NOT NULL,
	[MaRap] [int] NULL,
	[MaSoGhe] [nvarchar](10) NOT NULL,
	[LoaiGhe] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaGhe] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GiamGia]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GiamGia](
	[MaGiamGia] [int] IDENTITY(1,1) NOT NULL,
	[MaCode] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](50) NULL,
	[LoaiGiamGia] [nvarchar](50) NOT NULL,
	[GiaTri] [decimal](10, 2) NOT NULL,
	[NgayHetHan] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaGiamGia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NguoiDung]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NguoiDung](
	[MaNguoiDung] [int] IDENTITY(1,1) NOT NULL,
	[TenDangNhap] [nvarchar](50) NOT NULL,
	[MatKhau] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[SoDienThoai] [nvarchar](20) NULL,
	[VaiTro] [nvarchar](50) NULL,
	[TrangThai] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNguoiDung] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Phim]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Phim](
	[MaPhim] [int] IDENTITY(1,1) NOT NULL,
	[TieuDe] [nvarchar](255) NOT NULL,
	[TheLoai] [nvarchar](255) NULL,
	[ThoiLuong] [int] NULL,
	[MoTa] [nvarchar](255) NULL,
	[AnhBia] [nvarchar](255) NULL,
	[Trailer] [nvarchar](255) NULL,
	[TrangThai] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaPhim] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RapPhim]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RapPhim](
	[MaRap] [int] IDENTITY(1,1) NOT NULL,
	[TenRap] [nvarchar](100) NOT NULL,
	[DiaDiem] [nvarchar](255) NULL,
	[SoPhong] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaRap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SuatChieu]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuatChieu](
	[MaSuatChieu] [int] IDENTITY(1,1) NOT NULL,
	[MaPhim] [int] NULL,
	[MaRap] [int] NULL,
	[NgayChieu] [date] NOT NULL,
	[GioChieu] [time](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaSuatChieu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThanhToan]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThanhToan](
	[MaThanhToan] [int] IDENTITY(1,1) NOT NULL,
	[MaVe] [int] NULL,
	[SoTien] [decimal](10, 2) NOT NULL,
	[PhuongThucThanhToan] [nvarchar](50) NULL,
	[NgayThanhToan] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaThanhToan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ve]    Script Date: 9/15/2025 7:40:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ve](
	[MaVe] [int] IDENTITY(1,1) NOT NULL,
	[MaNguoiDung] [int] NULL,
	[MaSuatChieu] [int] NULL,
	[MaGhe] [int] NULL,
	[GiaVe] [decimal](10, 2) NULL,
	[TrangThai] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaVe] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[DoanhThu] ON 
GO
INSERT [dbo].[DoanhThu] ([MaDoanhThu], [MaSuatChieu], [TongTien], [HoaHongDaiLy]) VALUES (1, 1, CAST(120000.00 AS Decimal(15, 2)), CAST(5.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[DoanhThu] ([MaDoanhThu], [MaSuatChieu], [TongTien], [HoaHongDaiLy]) VALUES (2, 2, CAST(150000.00 AS Decimal(15, 2)), CAST(5.00 AS Decimal(5, 2)))
GO
SET IDENTITY_INSERT [dbo].[DoanhThu] OFF
GO
SET IDENTITY_INSERT [dbo].[Ghe] ON 
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (31, 1, N'A1', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (32, 1, N'A2', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (33, 1, N'A3', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (34, 1, N'A4', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (35, 1, N'A5', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (36, 1, N'A6', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (37, 1, N'A7', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (38, 1, N'A8', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (39, 1, N'A9', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (40, 1, N'A10', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (41, 1, N'B1', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (42, 1, N'B2', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (43, 1, N'B3', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (44, 1, N'B4', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (45, 1, N'B5', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (46, 1, N'B6', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (47, 1, N'B7', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (48, 1, N'B8', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (49, 1, N'B9', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (50, 1, N'B10', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (51, 1, N'C1', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (52, 1, N'C2', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (53, 1, N'C3', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (54, 1, N'C4', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (55, 1, N'C5', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (56, 1, N'C6', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (57, 1, N'C7', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (58, 1, N'C8', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (59, 1, N'C9', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (60, 1, N'C10', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (61, 1, N'D1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (62, 1, N'D2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (63, 1, N'D3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (64, 1, N'D4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (65, 1, N'D5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (66, 1, N'D6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (67, 1, N'D7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (68, 1, N'D8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (69, 1, N'D9', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (70, 1, N'D10', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (71, 1, N'E1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (72, 1, N'E2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (73, 1, N'E3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (74, 1, N'E4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (75, 1, N'E5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (76, 1, N'E6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (77, 1, N'E7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (78, 1, N'E8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (79, 1, N'E9', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (80, 1, N'E10', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (81, 1, N'F1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (82, 1, N'F2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (83, 1, N'F3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (84, 1, N'F4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (85, 1, N'F5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (86, 1, N'F6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (87, 1, N'F7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (88, 1, N'F8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (89, 1, N'F9', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (90, 1, N'F10', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (91, 1, N'G1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (92, 1, N'G2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (93, 1, N'G3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (94, 1, N'G4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (95, 1, N'G5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (96, 1, N'G6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (97, 1, N'G7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (98, 1, N'G8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (99, 1, N'G9', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (100, 1, N'G10', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (101, 1, N'H1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (102, 1, N'H2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (103, 1, N'H3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (104, 1, N'H4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (105, 1, N'H5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (106, 1, N'H6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (107, 1, N'H7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (108, 1, N'H8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (109, 1, N'H9', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (110, 1, N'H10', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (111, 2, N'A1', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (112, 2, N'A2', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (113, 2, N'A3', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (114, 2, N'A4', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (115, 2, N'A5', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (116, 2, N'A6', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (117, 2, N'A7', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (118, 2, N'A8', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (119, 2, N'B1', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (120, 2, N'B2', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (121, 2, N'B3', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (122, 2, N'B4', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (123, 2, N'B5', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (124, 2, N'B6', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (125, 2, N'B7', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (126, 2, N'B8', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (127, 2, N'C1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (128, 2, N'C2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (129, 2, N'C3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (130, 2, N'C4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (131, 2, N'C5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (132, 2, N'C6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (133, 2, N'C7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (134, 2, N'C8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (135, 2, N'D1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (136, 2, N'D2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (137, 2, N'D3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (138, 2, N'D4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (139, 2, N'D5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (140, 2, N'D6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (141, 2, N'D7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (142, 2, N'D8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (143, 2, N'E1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (144, 2, N'E2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (145, 2, N'E3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (146, 2, N'E4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (147, 2, N'E5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (148, 2, N'E6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (149, 2, N'E7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (150, 2, N'E8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (151, 2, N'F1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (152, 2, N'F2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (153, 2, N'F3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (154, 2, N'F4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (155, 2, N'F5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (156, 2, N'F6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (157, 2, N'F7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (158, 2, N'F8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (159, 3, N'A1', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (160, 3, N'A2', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (161, 3, N'A3', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (162, 3, N'A4', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (163, 3, N'A5', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (164, 3, N'A6', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (165, 3, N'A7', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (166, 3, N'A8', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (167, 3, N'B1', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (168, 3, N'B2', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (169, 3, N'B3', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (170, 3, N'B4', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (171, 3, N'B5', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (172, 3, N'B6', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (173, 3, N'B7', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (174, 3, N'B8', N'VIP')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (175, 3, N'C1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (176, 3, N'C2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (177, 3, N'C3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (178, 3, N'C4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (179, 3, N'C5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (180, 3, N'C6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (181, 3, N'C7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (182, 3, N'C8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (183, 3, N'D1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (184, 3, N'D2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (185, 3, N'D3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (186, 3, N'D4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (187, 3, N'D5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (188, 3, N'D6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (189, 3, N'D7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (190, 3, N'D8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (191, 3, N'E1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (192, 3, N'E2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (193, 3, N'E3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (194, 3, N'E4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (195, 3, N'E5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (196, 3, N'E6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (197, 3, N'E7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (198, 3, N'E8', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (199, 3, N'F1', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (200, 3, N'F2', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (201, 3, N'F3', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (202, 3, N'F4', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (203, 3, N'F5', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (204, 3, N'F6', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (205, 3, N'F7', N'thuong')
GO
INSERT [dbo].[Ghe] ([MaGhe], [MaRap], [MaSoGhe], [LoaiGhe]) VALUES (206, 3, N'F8', N'thuong')
GO
SET IDENTITY_INSERT [dbo].[Ghe] OFF
GO
SET IDENTITY_INSERT [dbo].[GiamGia] ON 
GO
INSERT [dbo].[GiamGia] ([MaGiamGia], [MaCode], [MoTa], [LoaiGiamGia], [GiaTri], [NgayHetHan]) VALUES (1, N'DISCOUNT10', N'Giảm 10%', N'phantram', CAST(10.00 AS Decimal(10, 2)), CAST(N'2025-09-27' AS Date))
GO
INSERT [dbo].[GiamGia] ([MaGiamGia], [MaCode], [MoTa], [LoaiGiamGia], [GiaTri], [NgayHetHan]) VALUES (2, N'GIAM50K', N'Giảm 50.000 VND
', N'codinh', CAST(50000.00 AS Decimal(10, 2)), CAST(N'2025-10-05' AS Date))
GO
SET IDENTITY_INSERT [dbo].[GiamGia] OFF
GO
SET IDENTITY_INSERT [dbo].[NguoiDung] ON 
GO
INSERT [dbo].[NguoiDung] ([MaNguoiDung], [TenDangNhap], [MatKhau], [Email], [SoDienThoai], [VaiTro], [TrangThai]) VALUES (1, N'doanquan', N'$2a$12$9zb1KReOgMBalO.Zn23RY.psk2pxOeARNvoVf4C0Aer4c3jUCV6lu', N'doanquan6805@gmail.com', N'0359271424', N'Admin', N'hoatdong')
GO
INSERT [dbo].[NguoiDung] ([MaNguoiDung], [TenDangNhap], [MatKhau], [Email], [SoDienThoai], [VaiTro], [TrangThai]) VALUES (2, N'vanthinh', N'$2a$12$9zb1KReOgMBalO.Zn23RY.psk2pxOeARNvoVf4C0Aer4c3jUCV6lu', N'thinhvannguyen113@gmail.com', N'0343841426', N'Admin', N'hoatdong')
GO
INSERT [dbo].[NguoiDung] ([MaNguoiDung], [TenDangNhap], [MatKhau], [Email], [SoDienThoai], [VaiTro], [TrangThai]) VALUES (3, N'quangtam', N'$2a$12$o2Ge3iJr6HpEKpEX3kHgIezpIKICsMLVJcFO053xlAdSy/P5xlOLi', N'nguyenquangtam179@gmail.com', N'0961138440', N'Admin', N'hoatdong')
GO
SET IDENTITY_INSERT [dbo].[NguoiDung] OFF
GO
SET IDENTITY_INSERT [dbo].[Phim] ON 
GO
INSERT [dbo].[Phim] ([MaPhim], [TieuDe], [TheLoai], [ThoiLuong], [MoTa], [AnhBia], [Trailer], [TrangThai]) VALUES (1, N'Avengers: Endgame', N'Hanh_dong, Khoa_hoc_vien_tuong', 181, N'Mot dinh cao cua 22 bo phim lien ket.', N'img/poster_20250913_080428_36910cf2.jpg', N'https://example.com/avengers_trailer', N'dangchieu')
GO
INSERT [dbo].[Phim] ([MaPhim], [TieuDe], [TheLoai], [ThoiLuong], [MoTa], [AnhBia], [Trailer], [TrangThai]) VALUES (2, N'The Lion King', N'Hoat_hinh, Chinh_kich', 118, N'Ban lam lai cua bo phim hoat hinh kinh dien.', N'img/poster_20250913_080422_6bba869f.jpg', N'https://example.com/lionking_trailer', N'sapchieu')
GO
INSERT [dbo].[Phim] ([MaPhim], [TieuDe], [TheLoai], [ThoiLuong], [MoTa], [AnhBia], [Trailer], [TrangThai]) VALUES (3, N'Inception', N'Khoa_hoc_vien_tuong, Gay_can', 148, N'Mot bo phimFAC phim cuop giat tam tri.', N'img/poster_20250913_080416_f0f0c97e.jpg', N'https://example.com/inception_trailer', N'ngungchieu')
GO
SET IDENTITY_INSERT [dbo].[Phim] OFF
GO
SET IDENTITY_INSERT [dbo].[RapPhim] ON 
GO
INSERT [dbo].[RapPhim] ([MaRap], [TenRap], [DiaDiem], [SoPhong]) VALUES (1, N'CGV Vincom', N'Ha Noi, Viet Nam', 1)
GO
INSERT [dbo].[RapPhim] ([MaRap], [TenRap], [DiaDiem], [SoPhong]) VALUES (2, N'BHD Star Cineplex', N'TP Ho Chi Minh, Viet Nam', 2)
GO
INSERT [dbo].[RapPhim] ([MaRap], [TenRap], [DiaDiem], [SoPhong]) VALUES (3, N'Lotte Cinema', N'Da Nang, Viet Nam', 3)
GO
SET IDENTITY_INSERT [dbo].[RapPhim] OFF
GO
SET IDENTITY_INSERT [dbo].[SuatChieu] ON 
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (1, 1, 1, CAST(N'2025-08-26' AS Date), CAST(N'14:30:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (2, 2, 2, CAST(N'2025-08-27' AS Date), CAST(N'18:00:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (3, 3, 3, CAST(N'2025-08-28' AS Date), CAST(N'20:15:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (4, 1, 1, CAST(N'2025-09-12' AS Date), CAST(N'20:02:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (5, 1, 1, CAST(N'2025-09-13' AS Date), CAST(N'20:02:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (6, 1, 1, CAST(N'2025-09-14' AS Date), CAST(N'20:02:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (7, 1, 1, CAST(N'2025-09-15' AS Date), CAST(N'20:02:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (8, 1, 1, CAST(N'2025-09-16' AS Date), CAST(N'20:02:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (9, 1, 1, CAST(N'2025-09-12' AS Date), CAST(N'09:09:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (10, 1, 1, CAST(N'2025-09-13' AS Date), CAST(N'09:09:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (11, 1, 1, CAST(N'2025-09-14' AS Date), CAST(N'09:09:00' AS Time))
GO
INSERT [dbo].[SuatChieu] ([MaSuatChieu], [MaPhim], [MaRap], [NgayChieu], [GioChieu]) VALUES (12, 1, 1, CAST(N'2025-09-17' AS Date), CAST(N'09:09:00' AS Time))
GO
SET IDENTITY_INSERT [dbo].[SuatChieu] OFF
GO
SET IDENTITY_INSERT [dbo].[ThanhToan] ON 
GO
INSERT [dbo].[ThanhToan] ([MaThanhToan], [MaVe], [SoTien], [PhuongThucThanhToan], [NgayThanhToan]) VALUES (1, 1, CAST(45000.00 AS Decimal(10, 2)), N'momo', CAST(N'2025-09-13T07:14:44.683' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[ThanhToan] OFF
GO
SET IDENTITY_INSERT [dbo].[Ve] ON 
GO
INSERT [dbo].[Ve] ([MaVe], [MaNguoiDung], [MaSuatChieu], [MaGhe], [GiaVe], [TrangThai]) VALUES (1, 3, 5, 35, CAST(95000.00 AS Decimal(10, 2)), N'dathanhtoan')
GO
SET IDENTITY_INSERT [dbo].[Ve] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__GiamGia__152C7C5CD476ACE3]    Script Date: 9/15/2025 7:40:50 AM ******/
ALTER TABLE [dbo].[GiamGia] ADD UNIQUE NONCLUSTERED 
(
	[MaCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__NguoiDun__55F68FC0462EB447]    Script Date: 9/15/2025 7:40:50 AM ******/
ALTER TABLE [dbo].[NguoiDung] ADD UNIQUE NONCLUSTERED 
(
	[TenDangNhap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__NguoiDun__A9D10534C08662B4]    Script Date: 9/15/2025 7:40:50 AM ******/
ALTER TABLE [dbo].[NguoiDung] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__ThanhToa__2725100ECC14EF19]    Script Date: 9/15/2025 7:40:50 AM ******/
ALTER TABLE [dbo].[ThanhToan] ADD UNIQUE NONCLUSTERED 
(
	[MaVe] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DoanhThu] ADD  DEFAULT ((0)) FOR [TongTien]
GO
ALTER TABLE [dbo].[DoanhThu] ADD  DEFAULT ((0)) FOR [HoaHongDaiLy]
GO
ALTER TABLE [dbo].[Ghe] ADD  DEFAULT ('thuong') FOR [LoaiGhe]
GO
ALTER TABLE [dbo].[NguoiDung] ADD  DEFAULT ('NguoiDung') FOR [VaiTro]
GO
ALTER TABLE [dbo].[NguoiDung] ADD  DEFAULT ('hoatdong') FOR [TrangThai]
GO
ALTER TABLE [dbo].[Phim] ADD  DEFAULT ('sapchieu') FOR [TrangThai]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT (getdate()) FOR [NgayThanhToan]
GO
ALTER TABLE [dbo].[Ve] ADD  DEFAULT ('choxuly') FOR [TrangThai]
GO
ALTER TABLE [dbo].[DoanhThu]  WITH CHECK ADD FOREIGN KEY([MaSuatChieu])
REFERENCES [dbo].[SuatChieu] ([MaSuatChieu])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ghe]  WITH CHECK ADD FOREIGN KEY([MaRap])
REFERENCES [dbo].[RapPhim] ([MaRap])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SuatChieu]  WITH CHECK ADD FOREIGN KEY([MaPhim])
REFERENCES [dbo].[Phim] ([MaPhim])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SuatChieu]  WITH CHECK ADD FOREIGN KEY([MaRap])
REFERENCES [dbo].[RapPhim] ([MaRap])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ThanhToan]  WITH CHECK ADD FOREIGN KEY([MaVe])
REFERENCES [dbo].[Ve] ([MaVe])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ve]  WITH CHECK ADD FOREIGN KEY([MaGhe])
REFERENCES [dbo].[Ghe] ([MaGhe])
GO
ALTER TABLE [dbo].[Ve]  WITH CHECK ADD FOREIGN KEY([MaNguoiDung])
REFERENCES [dbo].[NguoiDung] ([MaNguoiDung])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ve]  WITH CHECK ADD FOREIGN KEY([MaSuatChieu])
REFERENCES [dbo].[SuatChieu] ([MaSuatChieu])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ghe]  WITH CHECK ADD CHECK  (([LoaiGhe]='VIP' OR [LoaiGhe]='thuong'))
GO
ALTER TABLE [dbo].[GiamGia]  WITH CHECK ADD CHECK  (([LoaiGiamGia]='codinh' OR [LoaiGiamGia]='phantram'))
GO
ALTER TABLE [dbo].[NguoiDung]  WITH CHECK ADD CHECK  (([TrangThai]='khonghoatdong' OR [TrangThai]='hoatdong'))
GO
ALTER TABLE [dbo].[NguoiDung]  WITH CHECK ADD CHECK  (([VaiTro]='DaiLy' OR [VaiTro]='Admin' OR [VaiTro]='NguoiDung'))
GO
ALTER TABLE [dbo].[Phim]  WITH CHECK ADD CHECK  (([TrangThai]='ngungchieu' OR [TrangThai]='sapchieu' OR [TrangThai]='dangchieu'))
GO
ALTER TABLE [dbo].[Ve]  WITH CHECK ADD CHECK  (([TrangThai]='dahuy' OR [TrangThai]='dathanhtoan' OR [TrangThai]='choxuly'))
GO
