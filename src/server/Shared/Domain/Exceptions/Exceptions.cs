namespace Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an age restriction already exists.
/// </summary>
public class AlreadyExistsException(string message) : Exception(message) { }


/// <summary>
/// Represents an exception that is thrown when a bad request is encountered.
/// </summary>
public class BadRequestException(string message) : Exception(message) { }


/// <summary>
/// Represents an exception that is thrown when an age restriction is not found.
/// </summary>
public class NotFoundException(string message) : Exception(message) { }


/// <summary>
/// Represents an exception that occurs when there is a validation problem with the data.
/// </summary>
public class ValidationProblemException(string message) : Exception(message) { }


/// <summary>
/// Represents an exception that occurs when there is a invalid token.
/// </summary>
public class InvalidTokenException(string message) : Exception(message) { }


/// <summary>
/// Represents an exception that occurs when there is a semantic error.
/// </summary>
public class UnprocessableContentException(string message) : Exception(message) { }