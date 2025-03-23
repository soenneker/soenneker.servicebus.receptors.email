using Soenneker.ServiceBus.Receptors.Email.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.ServiceBus.Receptors.Email.Tests;

[Collection("Collection")]
public class EmailsReceptorTests : FixturedUnitTest
{
    private readonly IEmailsReceptor _util;

    public EmailsReceptorTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IEmailsReceptor>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
