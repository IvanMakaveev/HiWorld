namespace HiWorld.Data.Configurations
{

    using HiWorld.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProfileFriendConfiguration : IEntityTypeConfiguration<ProfileFriend>
    {
        public void Configure(EntityTypeBuilder<ProfileFriend> builder)
        {
            builder
                .HasOne(x => x.Profile)
                .WithMany(x => x.FriendsSent)
                .HasForeignKey(x => x.ProfileId);
            builder
                .HasOne(x => x.Friend)
                .WithMany(x => x.FriendsRecieved)
                .HasForeignKey(x => x.FriendId);
        }
    }
}
