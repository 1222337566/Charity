using InfrastrfuctureManagmentCore.Domains.Charity.Workspace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Workspace
{
    public class UserWorkspaceLayoutConfiguration : IEntityTypeConfiguration<UserWorkspaceLayout>
    {
        public void Configure(EntityTypeBuilder<UserWorkspaceLayout> b)
        {
            b.ToTable("CharityUserWorkspaceLayouts");
            b.HasKey(x => x.Id);
            b.Property(x => x.UserId).HasMaxLength(450).IsRequired();
            b.Property(x => x.LayoutJson).HasColumnType("nvarchar(max)");
            b.HasIndex(x => x.UserId).IsUnique();
        }
    }
}
