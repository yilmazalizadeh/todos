namespace TravelApp.Exceptions;

public abstract class TravelAppException(string message) : Exception(message);

public class NotFoundException(string message) : TravelAppException(message);

public class ValidationException(string message) : TravelAppException(message);