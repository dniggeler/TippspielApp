﻿using System;
using System.Data.Entity.ModelConfiguration;

namespace Data.SqlClient
{
    public class MatchInfoItemMap:EntityTypeConfiguration<MatchInfoItem>
    {
        public MatchInfoItemMap()
        {
            // Primary Key
            HasKey(t => new { t.Id });

            ToTable("MatchHistory", "dbo");

            Property(t => t.LeagueIdentifier).HasColumnName("LeagueIdentifier");
            Property(t => t.SeasonIdentifier).HasColumnName("SeasonIdentifier");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.MatchId).HasColumnName("MatchId");
            Property(t => t.GroupOrderId).HasColumnName("GroupOrderId");
            Property(t => t.MatchNr).HasColumnName("MatchNr");
            Property(t => t.KickoffTime).HasColumnName("KickoffTime");
            Property(t => t.KickoffTimeUtc).HasColumnName("KickoffTimeUtc");
            Property(t => t.HomeTeamId).HasColumnName("HomeTeamId");
            Property(t => t.AwayTeamId).HasColumnName("AwayTeamId");
            Property(t => t.HomeTeam).HasColumnName("HomeTeam");
            Property(t => t.AwayTeam).HasColumnName("AwayTeam");
            Property(t => t.HomeTeamScore).HasColumnName("HomeTeamScore");
            Property(t => t.AwayTeamScore).HasColumnName("AwayTeamScore");
            Property(t => t.HomeTeamIcon).HasColumnName("HomeTeamIcon");
            Property(t => t.AwayTeamIcon).HasColumnName("AwayTeamIcon");
            Property(t => t.IsInProlongation).HasColumnName("IsInProlongation");
            Property(t => t.IsFinished).HasColumnName("IsFinished");
        }
    }
}