namespace BankAccountService.Dtos;

public struct OperationResult<T>
{
    public bool Success { get; private init; }
    public T Data { get; private init; }
    public string Message { get; private init; }

    public static OperationResult<T> Ok(T data, string message = "")
    {
        return new OperationResult<T> { Success = true, Data = data, Message = message };
    }

    public static OperationResult<T> Fail(string message)
    {
        return new OperationResult<T> { Success = false, Message = message };
    }
}