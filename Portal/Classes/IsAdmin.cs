using Portal.DB;
using Portal.Models;

namespace Portal.Classes
{
    public class IsAdmin
    {
        private readonly ILogger<IsAdmin> _logger;

        private readonly DataContext _ctx;
        private readonly ScaleContext _sctx;

        public IsAdmin()//ILogger<IsAdmin> logger, DataContext ctx, ScaleContext sctx)
        {
            //_logger = logger;
            //_ctx = ctx;
            //_sctx = sctx;
        }

        public Admin GetAdmin(string user)
        {
            var admin = new Portal.DB.DB(_ctx, _sctx).GetAdmin(user);
            return admin;
        }
    }
}
