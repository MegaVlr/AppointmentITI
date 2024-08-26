using AppointmentProject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {   //Properits 

        //Name
        builder.HasIndex(u => u.Name);
        builder.Property(n => n.Name).HasMaxLength(40).IsRequired().HasColumnOrder(0).HasComment("Name User");
        // Email 
        builder.HasIndex(u => u.Email).IsUnique();  
        builder.Property(e => e.Email).HasMaxLength(90).IsRequired().HasColumnOrder(1).HasComment("Email User");
        //Password
        builder.Property(n => n.PasswordHash).HasMaxLength(64).IsRequired().HasColumnOrder(2).HasComment("Password User");
        //phone 
        builder.Property(p => p.phoneNumber).HasMaxLength(18).IsRequired().HasColumnOrder(3).HasComment("Phone User");
        builder.HasIndex(u => u.phoneNumber).IsUnique();
        //CreatedDate 
        builder.Property(d=>d.CreatedDate).IsRequired().HasColumnOrder(4).HasComment("This CreatedDate For User");

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
