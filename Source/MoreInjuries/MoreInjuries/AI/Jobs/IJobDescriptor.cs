using Verse.AI;

namespace MoreInjuries.AI.Jobs;

public interface IJobDescriptor
{
    Job CreateJob();

    void StartJob();
}
