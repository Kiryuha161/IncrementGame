using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data.Factories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProjectContext>
    {
        public ProjectContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(
                    Path.Combine(Directory.GetCurrentDirectory(),
                    "..", "IncrementGame.Server", "appsettings.json"),
                    optional: false,
                    reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProjectContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new ProjectContext(optionsBuilder.Options);
        }
    }
}
