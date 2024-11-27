using Microsoft.EntityFrameworkCore;

namespace ToDoApplication.Models
{
    public class ToDoContext : DbContext
    {
        public ToDoContext() { }

        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<ToDo> ToDos { get; set; } = null!;

        public DbSet<Status> Statuses { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        //seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = "work" , CategoryName = "Work"},
                new Category { CategoryId = "home" , CategoryName = "Home"},
                new Category { CategoryId = "ex" , CategoryName = "Exercise"},
                new Category { CategoryId = "shopping" , CategoryName = "Shopping"},
                new Category { CategoryId = "call" , CategoryName = "Contact"}
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = "open", StatusName = "Pending" },
                new Status { StatusId = "closed", StatusName = "Completed" }
            );
        }
    }
}
