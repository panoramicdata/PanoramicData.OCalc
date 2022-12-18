using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace PanoramicData.OCalc.Test;

public abstract class BaseTest
{
	protected ILogger Logger { get; }

	protected BaseTest(ITestOutputHelper helper)
	{
		Logger = LoggerFactory.Create(l => l.AddXunit(helper)).CreateLogger(GetType().Name);
	}
}