using Soenneker.ServiceBus.Receptors.Email.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.ServiceBus.Receptors.Email.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class EmailsReceptorTests : HostedUnitTest
{
    private readonly IEmailsReceptor _util;

    public EmailsReceptorTests(Host host) : base(host)
    {
        _util = Resolve<IEmailsReceptor>(true);
    }

    [Test]
    public void Default()
    {

    }
}
