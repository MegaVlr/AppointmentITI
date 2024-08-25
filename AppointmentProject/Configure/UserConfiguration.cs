using AppointmentProject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {   //Properits 

        //Name
        builder.Property(n => n.Name).HasMaxLength(40);

        // Email 
        builder.HasIndex(u => u.Email).IsUnique();  
        builder.Property(e => e.Email).HasMaxLength(90);

        //Password
        builder.Property(n => n.PasswordHash).HasMaxLength(64);

        

        //Relationships
        // User-Appointment Relationship (One-to-Many)
        builder
             .HasMany(u => u.Appointments)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);
        // User-PasswordReset Relationship (One-to-Many)
        builder
             .HasMany(u => u.PasswordResets)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);
    }
}
