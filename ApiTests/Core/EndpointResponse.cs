namespace ApiTests.Core;

public sealed record EndpointResponse(string ResponseText1, string ResponseText2)
{
    public void AssertEqual()
    {
        Assert.Equal(ResponseText1, ResponseText2);
    }

    public void AssertNotEqual()
    {
        Assert.NotEqual(ResponseText1, ResponseText2);
    }
}
