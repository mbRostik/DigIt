using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Infrastructure.Data.EntityTypeConfiguration
{
    public class MapPointWithUserEntityConfiguration : IEntityTypeConfiguration<MapPointWithUser>
    {
        public void Configure(EntityTypeBuilder<MapPointWithUser> builder)
        {
            builder.HasKey(cp => new { cp.UserId, cp.PointId });

            builder.HasOne(cp => cp.User)
               .WithMany(u => u.MapPointWithUsers)
               .HasForeignKey(cp => cp.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.MapPoint)
                   .WithMany(c => c.MapPointWithUsers)
                   .HasForeignKey(cp => cp.PointId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
