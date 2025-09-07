using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users
{
    public static class RolePriority
    {
        // Lower number means higher priority
        private static readonly Dictionary<string, int> Map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Admin"] = 1,
            ["Member"] = 2
        };

        // Default priority for unknown roles is 99 (lowest)
        public static int GetPriority(string role) => Map.TryGetValue(role, out var p) ? p : 99;
        
    }
}
