using Microsoft.AspNetCore.Identity;

namespace FinacialBackend.Model
{  
    public class AppUser : IdentityUser
    {
        public int MyProperty { get; set; }
    }
}
