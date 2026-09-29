namespace OrderManagement.Application.Auth;

public sealed record LoginResponse(string Token, string TokenType, int ExpiresIn);