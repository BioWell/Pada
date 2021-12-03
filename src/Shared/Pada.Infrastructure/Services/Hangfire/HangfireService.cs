using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using Pada.Abstractions.Services.Hangfire;

namespace Pada.Infrastructure.Services.Hangfire
{
    public class HangfireService : IJobService
    {
        public string Enqueue(Expression<Func<Task>> methodCall)
        {
            return BackgroundJob.Enqueue(methodCall);
        }
    }
}