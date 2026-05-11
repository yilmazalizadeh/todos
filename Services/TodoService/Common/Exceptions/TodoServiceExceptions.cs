namespace TodoService.Common.Exceptions;

public abstract class TodoServiceException(string message) : Exception(message);

public class NotFoundException(string message) : TodoServiceException(message);

public class ValidationException(string message) : TodoServiceException(message);
