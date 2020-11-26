using HiWorld.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Data.Configurations
{
    public class ProfileFollowerConfiguration : IEntityTypeConfiguration<ProfileFollower>
    {
        public void Configure(EntityTypeBuilder<ProfileFollower> builder)
        {
            builder
                .HasOne(x => x.Profile)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.ProfileId);
            builder
                .HasOne(x => x.Follower)
                .WithMany(x => x.Following)
                .HasForeignKey(x => x.FollowerId);
        }
    }
}
