using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExamEquityCalculatorApp.Models;

namespace ExamEquityCalculatorApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ExamEquityCalculatorApp.Models.EquityCalculator> EquityCalculator { get; set; } = default!;
    }
}
