namespace OrderManagement.Application.Auth;

public sealed record LoginRequest(string Email, string Password);