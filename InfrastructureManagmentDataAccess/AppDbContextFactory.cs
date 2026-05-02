using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InfrastructureManagmentDataAccess.EntityFramework
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // ممكن تقرأ من appsettings أو Environment Variable
            //"Data Source=DESKTOP-RQQ7T1F;Initial Catalog=Infra5;User Id=InfraUser2;Password=$cww@123;TrustServerCertificate=True;"
            //"Data Source=sql5105.site4now.net;Initial Catalog=db_ac8482_tareqbakr1;User Id=db_ac8482_tareqbakr1_admin;Password=136912151821az!@;TrustServerCertificate=True;"
            optionsBuilder.UseSqlServer("Data Source=sql5105.site4now.net;Initial Catalog=db_ac8482_charity;User Id=db_ac8482_charity_admin;Password=136912151821az!@;TrustServerCertificate=True;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
