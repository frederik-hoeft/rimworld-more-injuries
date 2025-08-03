using Verse;
using Verse.AI;

namespace MoreInjuries.AI.Jobs;

public class JobQueueBuilder(Pawn pawn)
{
    private bool _requiresScheduling;

    public void StartOrSchedule(Job job)
    {
        if (pawn.jobs.TryTakeOrderedJob(job, requestQueueing: _requiresScheduling))
        {
            _requiresScheduling = true;
        }
    }
}
