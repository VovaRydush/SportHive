using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DB.SportHive.Domain
{
    public class User
    {
        public int Id { get; set; }  // Первинний ключ (автоінкремент)
        public string Name { get; set; }
        public string Email { get; set; }
    }

}