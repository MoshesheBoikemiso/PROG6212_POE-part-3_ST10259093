using System.Collections.Generic;
using CMCS_Prototype.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS_Prototype.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Document> Documents { get; set; }
    }
}