﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

public class BookModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public int Year { get; set; }

    [Required]
    public string Publisher { get; set; } = string.Empty;

    [Required]
    public string ISBN { get; set; } = string.Empty;

    [Required]
    public int Pages { get; set; }

    [Required]
    public string Language { get; set; } = string.Empty;

    public byte[] CoverImageBase64 { get; set; }
}