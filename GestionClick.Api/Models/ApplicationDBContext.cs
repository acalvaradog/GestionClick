using Autogestion.Shared.DTO.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace GestionClick.Api.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<Empleadoes> Empleadoes { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasOne(d => d.ToEmpleado)
                    .WithMany(p => p.ReceivedMessages)
                    .HasForeignKey(d => d.ToEmpleadoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.FromEmpleado)
                    .WithMany(p => p.SentMessages)
                    .HasForeignKey(d => d.FromEmpleadoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

        }
    }
}
