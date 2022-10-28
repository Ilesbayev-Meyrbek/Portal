using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class GroupRepository : BaseScaleRepository<Group>, IGroupRepository
    {
        private readonly ScaleContext _context;

        public GroupRepository(ScaleContext context) : base(context)
        {
            _context = context;
        }
    }
}