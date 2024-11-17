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
    internal class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasOne(a => a.Doctor).WithMany(u => u.AppointmentAsDoctor).HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.Patient).WithMany(u => u.AppointmentAsPatient).HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
