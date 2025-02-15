﻿using Microsoft.EntityFrameworkCore;
using BookStoreLib.Data;

namespace UserService.Services;
public class BookManager
{
    private readonly UserDbContext _dbContext;
    private readonly AuthServiceClient _authServiceClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BookManager(
        UserDbContext dbContext,
        AuthServiceClient authServiceClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _authServiceClient = authServiceClient;
        _httpContextAccessor = httpContextAccessor;
    }

    // Process book based on its ID
    public async Task ProcessBookAsync(int bookId)
    {
        var book = await _dbContext.Books
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
            return;

        // Extract token from request headers
        var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(accessToken))
        {
            Console.WriteLine("Access token not found");
            return;
        }

        // Fetch user information using access token
        var user = await _authServiceClient.GetCurrentUserAsync(accessToken);
        Console.WriteLine(user != null
            ? $"Book '{book.Title}' belongs to {user.UserName}"
            : "User not found");
    }
}