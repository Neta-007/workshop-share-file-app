
namespace FileSharingApp.Exceptions;

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

    public override string ToString()
    {
        return $"error info: Title: {Title}, Message: {base.Message}";
    }
}
