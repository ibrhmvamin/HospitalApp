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
    //internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    //{
    //    public void Configure(EntityTypeBuilder<Message> builder)
    //    {
    //        builder.Property(m => m.Content).HasMaxLength(1024);
    //        builder.HasOne(m => m.Sender).WithMany(u => u.SentMessages).HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.NoAction);
    //        builder.HasOne(m => m.Receiver).WithMany(u => u.ReceivedMessages).HasForeignKey(m => m.ReceiverId).OnDelete(DeleteBehavior.NoAction);
    //    }
    //}
}
