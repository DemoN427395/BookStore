using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStoreLib.Models;

[Table("AspNetUsers", Schema = "user")] // Указываем схему "auth"
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    // Другие поля, если нужны (но только для чтения!)
}