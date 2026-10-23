namespace waypoint_server.Errors;

public static class ErrorFactory
{
    public static GraphQLException Unauthorized(
        string? message = null)
    {
        return new GraphQLException(
            ErrorBuilder.New()
                .SetCode("UNAUTHORIZED")
                .SetMessage(message ?? "Пользователь не авторизован")
                .SetExtension("status", 401)
                .Build()
        );
    }

    public static GraphQLException Forbidden(
        string? message = null)
    {
        return new GraphQLException(
            ErrorBuilder.New()
                .SetCode("FORBIDDEN")
                .SetMessage(message ?? "Доступ запрещен")
                .SetExtension("status", 403)
                .Build()
        );
    }

    public static GraphQLException NotFound(
        string? message = null)
    {
        return new GraphQLException(
            ErrorBuilder.New()
                .SetCode("NOT_FOUND")
                .SetMessage(message ?? "Данные не найдены")
                .SetExtension("status", 404)
                .Build()
        );
    }

    public static GraphQLException InternalServerError(
        string? message = null)
    {
        return new GraphQLException(
            ErrorBuilder.New()
                .SetCode("INTERNAL_SERVER_ERROR")
                .SetMessage(message ?? "Внутренняя ошибка сервера")
                .SetExtension("status", 500)
                .Build()
        );
    }

    public static GraphQLException BadRequest(
        string? message = null)
    {
        return new GraphQLException(
            ErrorBuilder.New()
                .SetCode("BAD_REQUEST")
                .SetMessage(message ?? "Некорректный запрос")
                .SetExtension("status", 400)
                .Build()
        );
    }
    public static GraphQLException Validation(
        string code,
        string message)
    {
        return new GraphQLException(
            ErrorBuilder.New()
                .SetCode(code)
                .SetMessage(message)
                .Build()
        );
    }
    public static IError ErrorParser(IError error)
    {
        if (error.Code == "AUTH_NOT_AUTHORIZED")
        {
            return ErrorBuilder.New()
            .SetCode("UNAUTHORIZED")
            .SetMessage("Пользователь не авторизован")
            .SetExtension("status", 401)
            .Build();
        }
        if (error.Code == "AUTH_NOT_AUTHENTICATED")
        {
            return ErrorBuilder.New()
            .SetCode("FORBIDDEN")
            .SetMessage("Доступ запрещен")
            .SetExtension("status", 403)
            .Build();
        }
        return error;
    }
}