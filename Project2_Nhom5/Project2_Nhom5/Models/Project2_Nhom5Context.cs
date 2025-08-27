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
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__BDBE9EF93159E2C8");
            entity.HasIndex(e => e.Code, "UQ__Discount__357D4CF91E555E70").IsUnique();
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.Code).HasMaxLength(50).IsUnicode(false).HasColumnName("code");
            entity.Property(e => e.Description).HasColumnType("text").HasColumnName("description");
            entity.Property(e => e.DiscountType).HasMaxLength(50).IsUnicode(false).HasColumnName("discount_type");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.Value).HasColumnType("decimal(10, 2)").HasColumnName("value");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__83CDF749F58746EA");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.Description).HasColumnType("text").HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Genre).HasMaxLength(255).IsUnicode(true).HasColumnName("genre");
            entity.Property(e => e.PosterUrl).HasMaxLength(255).IsUnicode(false).HasColumnName("poster_url");
            entity.Property(e => e.Status).HasMaxLength(50).IsUnicode(false).HasDefaultValue("sap_chieu").HasColumnName("status");
            entity.Property(e => e.Title).HasMaxLength(255).IsUnicode(false).HasColumnName("title");
            entity.Property(e => e.TrailerUrl).HasMaxLength(255).IsUnicode(false).HasColumnName("trailer_url");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EA8AE026E9");
            entity.HasIndex(e => e.TicketId, "UQ__Payments__D596F96A9EC87473").IsUnique();
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)").HasColumnName("amount");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50).IsUnicode(false).HasColumnName("payment_method");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.HasOne(d => d.Ticket).WithOne(p => p.Payment).HasForeignKey<Payment>(d => d.TicketId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK__Payments__ticket__6477ECF3");
        });

        modelBuilder.Entity<Revenue>(entity =>
        {
            entity.HasKey(e => e.RevenueId).HasName("PK__Revenue__3DF902E9C88C044C");
            entity.ToTable("Revenue");
            entity.Property(e => e.RevenueId).HasColumnName("revenue_id");
            entity.Property(e => e.AgencyCommission).HasDefaultValue(0m).HasColumnType("decimal(5, 2)").HasColumnName("agency_commission");
            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.TotalAmount).HasDefaultValue(0m).HasColumnType("decimal(15, 2)").HasColumnName("total_amount");
            entity.HasOne(d => d.Showtime).WithMany(p => p.Revenues).HasForeignKey(d => d.ShowtimeId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK__Revenue__showtim__693CA210");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seats__906DED9CEAA73FD8");
            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.SeatCode).HasMaxLength(10).IsUnicode(false).HasColumnName("seat_code");
            entity.Property(e => e.SeatType).HasMaxLength(50).IsUnicode(false).HasDefaultValue("thuong").HasColumnName("seat_type");
            entity.Property(e => e.TheaterId).HasColumnName("theater_id");
            entity.HasOne(d => d.Theater).WithMany(p => p.Seats).HasForeignKey(d => d.TheaterId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK__Seats__theater_i__5535A963");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.ShowtimeId).HasName("PK__Showtime__A406B5183BE13042");
            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.ShowDate).HasColumnName("show_date");
            entity.Property(e => e.ShowTime).HasColumnName("show_time");
            entity.Property(e => e.TheaterId).HasColumnName("theater_id");
            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes).HasForeignKey(d => d.MovieId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK__Showtimes__movie__4F7CD00D");
            entity.HasOne(d => d.Theater).WithMany(p => p.Showtimes).HasForeignKey(d => d.TheaterId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK__Showtimes__theat__5070F446");
        });

        modelBuilder.Entity<Theater>(entity =>
        {
            entity.HasKey(e => e.TheaterId).HasName("PK__Theaters__B53C958FDE398921");
            entity.Property(e => e.TheaterId).HasColumnName("theater_id");
            entity.Property(e => e.Location).HasMaxLength(255).IsUnicode(false).HasColumnName("location");
            entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(false).HasColumnName("name");
            entity.Property(e => e.RoomNumber).HasColumnName("room_number");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__D596F96BB866231B");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.Status).HasMaxLength(50).IsUnicode(false).HasDefaultValue("pending").HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.Seat).WithMany(p => p.Tickets).HasForeignKey(d => d.SeatId).HasConstraintName("FK__Tickets__seat_id__5BE2A6F2");
            entity.HasOne(d => d.Showtime).WithMany(p => p.Tickets).HasForeignKey(d => d.ShowtimeId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK__Tickets__showtim__5AEE82B9");
            entity.HasOne(d => d.User).WithMany(p => p.Tickets).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK__Tickets__user_id__59FA5E80");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FBB07CAC3");
            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164AAEF0817").IsUnique();
            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC5722EDB2277").IsUnique();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false).HasColumnName("email");
            entity.Property(e => e.Password).HasMaxLength(255).IsUnicode(false).HasColumnName("password");
            entity.Property(e => e.Phone).HasMaxLength(20).IsUnicode(false).HasColumnName("phone");
            entity.Property(e => e.Role).HasMaxLength(50).IsUnicode(false).HasDefaultValue("User").HasColumnName("role");
            entity.Property(e => e.Status).HasMaxLength(50).IsUnicode(false).HasDefaultValue("active").HasColumnName("status");
            entity.Property(e => e.Username).HasMaxLength(50).IsUnicode(false).HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}