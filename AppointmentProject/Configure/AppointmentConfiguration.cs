using AppointmentProject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AppointmentProject.Configure
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            //Properits 
            builder.Property(d=>d.AppointmentDate).IsRequired();
            builder.HasIndex(n => n.AppointmentDate);
            //Title
            builder.Property(n => n.Title).HasMaxLength(60).IsRequired(); 
            builder.HasIndex(n => n.Title);

            //Relationships
            
        }
    }
}

