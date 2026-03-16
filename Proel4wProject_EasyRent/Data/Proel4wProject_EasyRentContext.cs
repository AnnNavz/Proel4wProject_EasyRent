using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proel4wProject_EasyRent.Models;

namespace Proel4wProject_EasyRent.Data
{
    public class Proel4wProject_EasyRentContext : DbContext
    {
        public Proel4wProject_EasyRentContext (DbContextOptions<Proel4wProject_EasyRentContext> options)
            : base(options)
        {
        }

        public DbSet<Proel4wProject_EasyRent.Models.Users> Users { get; set; } = default!;
    }
}
