namespace Compooler.Domain.Tests;

public class ResultTests
{
    private record TestError() : Error(Code: "test", Message: "test");

    [Fact]
    public void Success_NoError()
    {
        var result = Result.Success();

        Assert.False(result.IsFailed);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_HasError()
    {
        var error = new TestError();
        var result = Result.Failure(error: error);

        Assert.True(result.IsFailed);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Generic_Success_NoError()
    {
        const int value = 42;
        var result = Result<int>.Success(value);

        Assert.False(result.IsFailed);
        Assert.Null(result.Error);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Generic_Failure_HasError()
    {
        var error = new TestError();
        var result = Result<int>.Failure(error: error);

        Assert.True(result.IsFailed);
        Assert.Equal(error, result.Error);
        Assert.Equal(default, result.Value);
    }
}
