using Microsoft.EntityFrameworkCore;
using TelephoneDirectory.Models.contact;
namespace TelephoneDirectory.Data.AppDbContext
{
    public class AppDbContext:DbContext
    {
        public DbSet<Contact> contats {get; set;}

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().HasKey(contact=> contact.Id);
            modelBuilder.Entity<Contact>().Property(contat=>contat.FullName).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Contact>().HasIndex(contat=>contat.Mobile).IsUnique(true);
            modelBuilder.Entity<Contact>().Property(contact=>contact.Mobile).IsRequired(true).HasMaxLength(11);
            modelBuilder.Entity<Contact>().Property(contact=>contact.BirthDate).HasColumnType("date").IsRequired(false);
            modelBuilder.Entity<Contact>().Property(contact=>contact.Photo).IsRequired(false);



        }

    }
    
}