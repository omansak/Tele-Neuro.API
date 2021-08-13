using Microsoft.EntityFrameworkCore;
using TeleNeuro.Entities;

namespace TeleNeuro.Entity.Context
{
    public class TeleNeuroDatabaseContext :DbContext
    {
        public TeleNeuroDatabaseContext(DbContextOptions<TeleNeuroDatabaseContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>();
        }
    }
}
