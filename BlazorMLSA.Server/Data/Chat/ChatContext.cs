using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Data.Chat
{
    public class ChatContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ChatContext(DbContextOptions<ChatContext> dbContextOptions) : base(dbContextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(user => user.SentMessages)
                .WithOne(message => message.Sender)
                .HasForeignKey(message => message.SenderId);

            modelBuilder.Entity<User>()
                .HasMany(user => user.ReceivedMessages)
                .WithOne(message => message.Receiver)
                .HasForeignKey(message => message.ReceiverId);
        }
    }
}
