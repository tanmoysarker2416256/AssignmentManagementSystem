using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities; 

public class ApplicationUser : IdentityUser

{
    public string FullName { get; set; } = default!;

 
    public int? ClassId { get; set; }
    public Class? Class { get; set; }

}
