using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiresUtc { get; set; }
        public bool Revoked { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
