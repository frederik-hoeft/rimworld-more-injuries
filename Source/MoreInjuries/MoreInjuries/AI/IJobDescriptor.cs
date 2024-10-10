using Verse.AI;

namespace MoreInjuries.AI;

public interface IJobDescriptor
{
    Job CreateJob();

    void StartJob();
}
