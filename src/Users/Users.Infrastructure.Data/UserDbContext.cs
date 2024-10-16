﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options) { }

        public DbSet<BannedUser> BannedUsers { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<SavedPost> SavedPosts { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<MapPoint> MapPoints { get; set; }

        public DbSet<MapPointWithUser> MapPointWithUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
