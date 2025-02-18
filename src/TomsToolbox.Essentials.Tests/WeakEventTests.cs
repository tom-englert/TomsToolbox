namespace TomsToolbox.Essentials.Tests;

using Xunit;

public class WeakEventTests
{
    [Fact]
    public void WeakEventTests_ReEntranceTest()
    {
        var source = new WeakEventSource<EventArgs>();
        var eventCount = 0;

        source.Subscribe(OnEvent1);
        source.Subscribe(OnEvent1);
        source.Subscribe(OnEvent1);
        source.Subscribe(OnEvent2);
        source.Subscribe(OnEvent2);
        source.Subscribe(OnEvent3);

        void OnEvent1(object? sender, EventArgs e)
        {
            source.Unsubscribe(OnEvent1);
            eventCount++;
        }
        void OnEvent2(object? sender, EventArgs e)
        {
            source.Unsubscribe(OnEvent2);
            eventCount++;
        }
        void OnEvent3(object? sender, EventArgs e)
        {
            source.Unsubscribe(OnEvent3);
            eventCount++;
        }

        source.Raise(this, EventArgs.Empty);
        Assert.Equal(6, eventCount);
    }
}
