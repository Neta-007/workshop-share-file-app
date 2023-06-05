
namespace FileShareConnectivity.Exceptions;

/*
 * TODO: Change this
 */
public class MyException : Exception
{
    public string Title { get; set; }

    public MyException(string title, string message) : base(message)
    {
        Title = title;
    }
}
