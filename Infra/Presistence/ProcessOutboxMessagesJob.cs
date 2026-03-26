using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Presistence;
using Application.Contracts;

using MediatR;

using Newtonsoft.Json;

using Quartz;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob(IUnitofWork unitOfWork, IPublisher publisher) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await unitOfWork.OutboxMessagesRepo.GetUnprocessedMessagesAsync(batchSize: 20, maxErrors: 3);

        foreach (var outboxMessage in messages)
        {
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<INotification>(outboxMessage.Content,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                if (domainEvent != null)
                {
                    await publisher.Publish(domainEvent, context.CancellationToken);
                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                    outboxMessage.Error = null;      
                }
            }
            catch (Exception ex)
            {
                outboxMessage.ErrorCount++;
                outboxMessage.Error = $"Attempt {outboxMessage.ErrorCount}: {ex.Message}";
            }
        }

        await unitOfWork.CommitAsync();
    }
}