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
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasMany(a => a.AppointmentAsDoctor).WithOne(a => a.Doctor).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.AppointmentAsPatient).WithOne(a => a.Patient).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
