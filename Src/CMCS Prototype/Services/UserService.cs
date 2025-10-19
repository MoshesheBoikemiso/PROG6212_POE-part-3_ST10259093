using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Services
{
    public class UserService
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        FirstName = "John",
                        LastName = "Lecturer",
                        Email = "lecturer@email.com",
                        UserRole = "Lecturer"
                    },
                    new User
                    {
                        FirstName = "Sarah",
                        LastName = "Coordinator",
                        Email = "coordinator@email.com",
                        UserRole = "Coordinator"
                    },
                    new User
                    {
                        FirstName = "Mike",
                        LastName = "Manager",
                        Email = "manager@email.com",
                        UserRole = "Manager"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}