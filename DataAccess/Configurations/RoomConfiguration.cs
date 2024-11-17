using DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasOne(r => r.Receiver).WithMany().HasForeignKey(r => r.ReceiverId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(r => r.Sender).WithMany().HasForeignKey(r => r.SenderId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
