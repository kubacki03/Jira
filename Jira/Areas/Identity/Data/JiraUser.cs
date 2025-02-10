using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jira.Models;
using Microsoft.AspNetCore.Identity;

namespace Jira.Areas.Identity.Data;

// Add profile data for application users by adding properties to the JiraUser class
public class JiraUser : IdentityUser
{
    // Projekty, które użytkownik stworzył
    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();

    // Projekty, w których użytkownik uczestniczy (wiele do wielu)
    public ICollection<Project> Projects { get; set; } = new List<Project>();

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
}


