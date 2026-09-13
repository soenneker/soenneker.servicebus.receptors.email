using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.ServiceBus.Receptors.Email;
using Soenneker.Email.Senders.Abstract;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Audit;

public class EmailJobTests
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    [Test]
    public async Task EmailJobPreservesHangfireContract()
    {
        var client = new RecordingJobs();
        var receptor = new EmailsReceptor(null!, null!, NullLogger<EmailsReceptor>.Instance, Fixture.Config(), client);
        await receptor.OnMessageReceived("body", "type");
        Check(client.Job!.Type == typeof(IEmailSender) && client.Job.Method.Name == "Send", "Wrong Hangfire target");
        Check((string)client.Job.Args[0] == "body" && (string)client.Job.Args[1] == "type", "Wrong Hangfire args");
        Check((CancellationToken)client.Job.Args[2] == CancellationToken.None && client.State is EnqueuedState, "Wrong cancellation/state");
    }
    private sealed class RecordingJobs : IBackgroundJobClient
    {
        public Job? Job; public IState? State;
        public string Create(Job job, IState state) { Job = job; State = state; return "1"; }
        public bool ChangeState(string jobId, IState state, string expectedState) => true;
    }
}

internal static class Fixture
{
    public static Microsoft.Extensions.Configuration.IConfiguration Config(bool logging = false, bool counts = true) =>
        new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Azure:ServiceBus:Enable"] = "true", ["Azure:ServiceBus:TransmitterLogging"] = logging.ToString(),
            ["Background:QueueLength"] = "32", ["Background:LockCounts"] = counts.ToString()
        }).Build();
}
