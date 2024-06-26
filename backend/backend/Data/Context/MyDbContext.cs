﻿using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Metadata;

namespace backend.Data.Context
{
    public class MyDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public MyDbContext(DbContextOptions<MyDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        public DbSet<Review> ReviewTable { get; set; }

        public DbSet<ProductOrder> ProductOrderTable { get; set; }

        public DbSet<Order> OrderTable { get; set; }
        public DbSet<Product> ProductTable { get; set; }

        public DbSet<Discount> DiscountTable { get; set; }

        public DbSet<User> UserTable { get; set; }

        public DbSet<Address> AddressTable { get; set; }

        public DbSet<Admin> AdminTable { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("BasketballDBConnection"));

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<ProductOrder>()
        .HasKey(po => new { po.ProductId, po.OrderId });
            modelBuilder.Entity<Review>()
        .ToTable(tb => tb.HasTrigger("RatingChange"));
        }

    }


}



