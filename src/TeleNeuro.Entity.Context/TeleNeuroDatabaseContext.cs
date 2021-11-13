using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeleNeuro.Entities;

namespace TeleNeuro.Entity.Context
{
    public class TeleNeuroDatabaseContext : DbContext
    {
        public TeleNeuroDatabaseContext(DbContextOptions<TeleNeuroDatabaseContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.LogTo(i =>
            {
                Console.WriteLine("------- DATABASE QUERY ----------");
                Console.WriteLine(i);
                Console.WriteLine("------- DATABASE QUERY ----------");
            }, LogLevel.Information);
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>();
            builder.Entity<Document>();
            builder.Entity<Exercise>();
            builder.Entity<Program>();
            builder.Entity<User>();
            builder.Entity<UserProfile>();
            builder.Entity<Role>();
            builder.Entity<UserRoleRelation>();
            builder.Entity<ExercisePropertyDefinition>();
            builder.Entity<ExerciseProgramRelation>();
            builder.Entity<ExerciseProgramRelationProperty>();
            builder.Entity<UserProgramRelation>();
        }
    }
}
