using BookService.Controllers;
using BookStoreLib.Data;
using BookStoreLib.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Interfaces;

public interface IBooksController
{
    Task<IActionResult> CreateBook([FromForm] CreateBookDTO dto);

    Task<IActionResult> UpdateBook([FromForm] UpdateBookDTO dto);

    Task<IActionResult> DeleteBook([FromRoute] int id);

    Task<IActionResult> DownloadBookFile([FromRoute] int id);

    Task<IActionResult> SearchBook([FromRoute] int id);
}