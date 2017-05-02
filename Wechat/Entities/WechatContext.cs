using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wechat.Entities
{
    public class WechatContext : DbContext
    {
        public WechatContext(DbContextOptions<WechatContext> options)
            : base(options)
        {
        }

        public DbSet<CleanTask> Task { get; set; }

        public DbSet<Account> Account { get; set; }

        public DbSet<CleanConfig> Config { get; set; }
    }
}
