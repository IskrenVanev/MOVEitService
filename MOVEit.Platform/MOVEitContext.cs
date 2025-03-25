using MOVEit.Platform.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace MOVEit.Platform
{
    public class MOVEitContext : DbContext
    {
        public MOVEitContext()
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
