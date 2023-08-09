namespace Sample.Models;

public record ProcessParams(
    string? Title,
    string? FirstName,
    string? LastName,
    int? Type,
    string? Comment
);