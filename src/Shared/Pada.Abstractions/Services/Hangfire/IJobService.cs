using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pada.Abstractions.Services.Hangfire
{
    public interface IJobService
    {
        string Enqueue(Expression<Func<Task>> methodCall);
    }
}