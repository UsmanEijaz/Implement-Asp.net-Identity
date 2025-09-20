using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace User_Management.Model
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //SeedRole(builder);
        }

        //private void SeedRole(ModelBuilder modelBuilder) 
        //{
        //    modelBuilder.Entity<IdentityRole>().HasData(
        //        new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = "Admin", ConcurrencyStamp="1" , NormalizedName ="Admin"},
        //        new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = "User", ConcurrencyStamp="2", NormalizedName = "User"},
        //        new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = "HR", ConcurrencyStamp = "3", NormalizedName = "HR" }
        //        );
        //}
    }
}
