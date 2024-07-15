using System.Net;

namespace CashFlow.Exception.ExceptionsBase;

public class NotFoundException : CashFlowException
{

    public override int StatusCode => (int)HttpStatusCode.NotFound;
    public NotFoundException(string message) : base(message)
    {
        
    }

    public override List<string> GetErrors()
    {
        return [Message]; //return new List<string>() {Message};
    }
}