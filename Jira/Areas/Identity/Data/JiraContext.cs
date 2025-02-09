using System.Net.Mail;
using System.Reflection.Emit;
using Jira.Areas.Identity.Data;
using Jira.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jira.Data;

public class JiraContext : IdentityDbContext<JiraUser>
{
    public JiraContext(DbContextOptions<JiraContext> options)
        : base(options)
    {
    }
    public DbSet<Models.Attachment> Attachments { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<Sprint> Sprints { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
   
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // many to many
        builder.Entity<JiraUser>()
            .HasMany(s => s.Projects)
            .WithMany(c => c.Users)
            .UsingEntity(j => j.ToTable("UserProjects"));


        //one to many
        builder.Entity<Models.Attachment>()
            .HasOne(s => s.Ticket)
            .WithMany(c => c.Attachments)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Project>()
        .HasOne(p => p.Creator)       // Projekt ma jednego kreatora
        .WithMany(u => u.CreatedProjects)  // Użytkownik może stworzyć wiele projektów
        .HasForeignKey(p => p.CreatorId)
        .OnDelete(DeleteBehavior.Restrict);



        builder.Entity<Ticket>()
            .HasOne(s => s.Project)
            .WithMany(c => c.Tickets)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ticket>()
         .HasOne(t => t.Sprint)
         .WithMany(s => s.Tickets)
         .HasForeignKey(t => t.SprintId)
         .OnDelete(DeleteBehavior.NoAction);


    }
}
