using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Project2_Nhom5.Models;

public partial class Project2_Nhom5Context : DbContext
{
    public Project2_Nhom5Context()
    {
    }

    public Project2_Nhom5Context(DbContextOptions<Project2_Nhom5Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Discount> Discounts { get; set; }
    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Revenue> Revenues { get; set; }
    public virtual DbSet<Seat> Seats { get; set; }
    public virtual DbSet<Showtime> Showtimes { get; set; }
    public virtual DbSet<Theater> Theaters { get; set; }
    public virtual DbSet<Ticket> Tickets { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map Discount -> GiamGia
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.ToTable("GiamGia");
            entity.HasKey(e => e.DiscountId);
            entity.Property(e => e.DiscountId).HasColumnName("MaGiamGia");
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasColumnName("MaCode");
            entity.Property(e => e.Description).HasColumnName("MoTa").HasColumnType("text");
            entity.Property(e => e.DiscountType).HasColumnName("LoaiGiamGia");
            entity.Property(e => e.Value).HasColumnName("GiaTri").HasColumnType("decimal(10,2)");
            entity.Property(e => e.ExpiryDate).HasColumnName("NgayHetHan").HasColumnType("date");
        });

        // Map Movie -> Phim
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("Phim");
            entity.HasKey(e => e.MovieId);
            entity.Property(e => e.MovieId).HasColumnName("MaPhim");
            entity.Property(e => e.Title).HasColumnName("TieuDe");
            entity.Property(e => e.Genre).HasColumnName("TheLoai");
            entity.Property(e => e.Duration).HasColumnName("ThoiLuong");
            entity.Property(e => e.Description).HasColumnName("MoTa").HasColumnType("text");
            entity.Property(e => e.PosterUrl).HasColumnName("AnhBia");
            entity.Property(e => e.TrailerUrl).HasColumnName("Trailer");
            entity.Property(e => e.Status).HasColumnName("TrangThai").HasDefaultValue("sapchieu");
        });

        // Map Theater -> RapPhim
        modelBuilder.Entity<Theater>(entity =>
        {
            entity.ToTable("RapPhim");
            entity.HasKey(e => e.TheaterId);
            entity.Property(e => e.TheaterId).HasColumnName("MaRap");
            entity.Property(e => e.Name).HasColumnName("TenRap");
            entity.Property(e => e.Location).HasColumnName("DiaDiem");
            entity.Property(e => e.RoomNumber).HasColumnName("SoPhong");
        });

        // Map Showtime -> SuatChieu
        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.ToTable("SuatChieu");
            entity.HasKey(e => e.ShowtimeId);
            entity.Property(e => e.ShowtimeId).HasColumnName("MaSuatChieu");
            entity.Property(e => e.MovieId).HasColumnName("MaPhim");
            entity.Property(e => e.TheaterId).HasColumnName("MaRap");
            entity.Property(e => e.ShowDate).HasColumnName("NgayChieu").HasColumnType("date");
            entity.Property(e => e.ShowTime).HasColumnName("GioChieu").HasColumnType("time");

            entity.HasOne(d => d.Movie)
                .WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Theater)
                .WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.TheaterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Map Seat -> Ghe
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.ToTable("Ghe");
            entity.HasKey(e => e.SeatId);
            entity.Property(e => e.SeatId).HasColumnName("MaGhe");
            entity.Property(e => e.TheaterId).HasColumnName("MaRap");
            entity.Property(e => e.SeatCode).HasColumnName("MaSoGhe");
            entity.Property(e => e.SeatType).HasColumnName("LoaiGhe");

            entity.HasOne(d => d.Theater)
                .WithMany(p => p.Seats)
                .HasForeignKey(d => d.TheaterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Map Ticket -> Ve
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ve");
            entity.HasKey(e => e.TicketId);
            entity.Property(e => e.TicketId).HasColumnName("MaVe");
            entity.Property(e => e.UserId).HasColumnName("MaNguoiDung");
            entity.Property(e => e.ShowtimeId).HasColumnName("MaSuatChieu");
            entity.Property(e => e.SeatId).HasColumnName("MaGhe");
            entity.Property(e => e.Price).HasColumnName("GiaVe").HasColumnType("decimal(10,2)");
            entity.Property(e => e.Status).HasColumnName("TrangThai");

            entity.HasOne(d => d.Seat)
                .WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.Showtime)
                .WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Map Payment -> ThanhToan
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("ThanhToan");
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.PaymentId).HasColumnName("MaThanhToan");
            entity.Property(e => e.TicketId).HasColumnName("MaVe");
            entity.HasIndex(e => e.TicketId).IsUnique();
            entity.Property(e => e.Amount).HasColumnName("SoTien").HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentMethod).HasColumnName("PhuongThucThanhToan");
            entity.Property(e => e.PaymentDate).HasColumnName("NgayThanhToan").HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Ticket)
                .WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Map Revenue -> DoanhThu
        modelBuilder.Entity<Revenue>(entity =>
        {
            entity.ToTable("DoanhThu");
            entity.HasKey(e => e.RevenueId);
            entity.Property(e => e.RevenueId).HasColumnName("MaDoanhThu");
            entity.Property(e => e.ShowtimeId).HasColumnName("MaSuatChieu");
            entity.Property(e => e.TotalAmount).HasColumnName("TongTien").HasColumnType("decimal(15,2)").HasDefaultValue(0m);
            entity.Property(e => e.AgencyCommission).HasColumnName("HoaHongDaiLy").HasColumnType("decimal(5,2)").HasDefaultValue(0m);

            entity.HasOne(d => d.Showtime)
                .WithMany(p => p.Revenues)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Map User -> NguoiDung
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("NguoiDung");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("MaNguoiDung");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasColumnName("TenDangNhap");
            entity.Property(e => e.Password).HasColumnName("MatKhau");
            entity.Property(e => e.Email).HasColumnName("Email");
            entity.Property(e => e.Phone).HasColumnName("SoDienThoai");
            entity.Property(e => e.Role).HasColumnName("VaiTro");
            entity.Property(e => e.Status).HasColumnName("TrangThai");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}