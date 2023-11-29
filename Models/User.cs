using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApp.Models
{
    public class User : IdentityUser
    {
            [NotMapped]
            public IList<string> RoleNames { get; set; } = null!;
    }
}
