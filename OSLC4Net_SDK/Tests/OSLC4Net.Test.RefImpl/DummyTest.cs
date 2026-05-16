public class DummyTest
{
    [Test]
    public async Task DummyPass()
    {
        var success = true;

        await Assert.That(success).IsTrue();
    }

}
