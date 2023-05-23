
namespace FileShareConnectivity.Exceptions;

public class MyException : Exception
{
    public string Title { get; set; }

    public MyException(string title, string message) : base(message)
    {
        Title = title;
    }
}
