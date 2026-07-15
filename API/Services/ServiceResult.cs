public record ServiceResult<T>(
    bool Success,
    T? Data,
    string? Log
);