using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacoBell.Helpers
{
    public static class Config
    {
        public static string ConnectionString =>
            "Data Source=localhost;Initial Catalog=TacoBellDb;Integrated Security=True;";
    }
}