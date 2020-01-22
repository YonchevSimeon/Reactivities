namespace Persistence.Configurations
{
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
    {
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder
                .HasKey(ua => new { ua.AppUserId, ua.ActivityId });

            builder
                .HasOne(ua => ua.AppUser)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(ua => ua.AppUserId);

            builder
                .HasOne(ua => ua.Activity)
                .WithMany(a => a.UserActivities)
                .HasForeignKey(ua => ua.ActivityId);
        }
    }
}