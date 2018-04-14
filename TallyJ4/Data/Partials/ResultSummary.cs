﻿using System;
using TallyJ4.Extensions;

namespace TallyJ4.Data.DbModel
{
  public partial class ResultSummary : IIndexedForCaching
  {
    public int PercentParticipation
    {
      get
      {
        return NumEligibleToVote.AsInt() == 0
                            ? 0
                            : Math.Round(
                                (NumBallotsWithManual.AsInt() * 100D) /
                                NumEligibleToVote.AsInt(), 0).AsInt();
      }
    }

    /// <Summary>Total of all collected</Summary>
    public int? SumOfEnvelopesCollected
    {
      get
      {
        if (InPersonBallots.HasValue || DroppedOffBallots.HasValue || MailedInBallots.HasValue ||
            CalledInBallots.HasValue)
        {
          return InPersonBallots.GetValueOrDefault()
                 + DroppedOffBallots.GetValueOrDefault()
                 + MailedInBallots.GetValueOrDefault()
                 + CalledInBallots.GetValueOrDefault();
        }
        return null;
      }
    }

    public int? NumBallotsWithManual
    {
      get
      {
        if (BallotsReceived.HasValue || SpoiledBallots.HasValue)
        {
          return BallotsReceived.GetValueOrDefault() + SpoiledBallots.GetValueOrDefault();
        }

        return null;
      }
    }
  }
}