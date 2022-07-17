using Microsoft.EntityFrameworkCore;
using Portal.Models;

namespace Portal.DB
{
    public class DB
    {
        private readonly DataContext _ctx;
        private readonly ScaleContext _sctx;

        public DB(DataContext ctx, ScaleContext sctx)
        {
            this._ctx = ctx;
            this._sctx = sctx;
        }

        /**************************************** Role ************************************************/

        public List<Role> GetRoles()
        {
            try
            {
                var roles = _ctx.Roles.ToList();

                return roles;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SaveNewRole(Role role)
        {
            try
            {
                _ctx.Roles.Add(role);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Role GetRoleForEdit(int? id)
        {
            try
            {
                Role role = _ctx.Roles.Find(id);

                return role;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SaveEditRole(Role role)
        {
            try
            {
                _ctx.Entry(role).State = EntityState.Modified;
                _ctx.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Role GetRoleForDelete(int? id)
        {
            try
            {
                Role role = _ctx.Roles.Find(id);

                return role;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteRole(int? id)
        {
            try
            {
                Role role = _ctx.Roles.Find(id);
                _ctx.Roles.Remove(role);
                _ctx.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
