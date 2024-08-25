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

            //Title
            builder.Property(n => n.Title).HasMaxLength(60);

            //Relationships
            
        }
    }
}

