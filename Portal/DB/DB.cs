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

        public string GetUserMarketID(string user)
        {
            string userMID = String.Empty;

            string mID = GetMarketForUser(user);

            if (!string.IsNullOrEmpty(mID))
                userMID = mID;
            else
            {
                Admin adm = GetAdmin(user);

                if (adm != null)
                    userMID = "OFCE";
            }

            return userMID;
        }

        public string GetMarketForUser(string user)
        {
            try
            {
                var userMarketID = _ctx.Users.Where(w => w.Login == user).FirstOrDefault();

                if (userMarketID != null)
                    return userMarketID.MarketID;
                else
                    return null;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return null;
            }
        }

        public Admin GetAdmin(string user)
        {
            try
            {
                var admin = _ctx.Admins.Where(w => w.Login == user).FirstOrDefault();

                return admin;

            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return null;
            }
        }

        public Role GetUserRole(string user)
        {
            try
            {
                var adm = GetAdmin(user);

                if (adm == null)
                {
                    var roleID = _ctx.Users.Where(w => w.Login == user).ToList()[0].RoleID;
                    Role role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();

                    return role;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return null;
            }
        }

        public List<MarketsName> GetMarkets()
        {
            try
            {
                List<MarketsName> markets = _ctx.Markets.ToList();

                return markets;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return new List<MarketsName>();
            }
        }

        public MarketsName GetMarkets(string id)
        {
            try
            {
                MarketsName markets = _ctx.Markets.Where(w => w.MarketID == id).FirstOrDefault();

                return markets;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion

                return new MarketsName();
            }
        }

        public List<MarketsName> GetMarketsForPrivileges(string user)
        {
            try
            {
                List<MarketsName> markets = new List<MarketsName>();

                var admin = _ctx.Admins.Where(w => w.Login == user).FirstOrDefault();
                var users = _ctx.Users.Where(w => w.Login == user).FirstOrDefault();

                if (admin != null)
                {
                    markets = _ctx.Markets.ToList();
                }
                else if (admin == null && users != null)
                {
                    var roleID = users.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = users.MarketID;

                    if (role.AllMarkets)
                    {
                        markets = _ctx.Markets.ToList();
                    }
                    else
                    {
                        markets = _ctx.Markets.Where(w => w.MarketID == market).ToList();
                    }
                }

                return markets;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return null;
            }
        }

        /**************************************** Role ************************************************/

        #region Roles

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
                _ctx.Roles.Update(role);
                _ctx.SaveChanges();

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
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region Users

        public List<User> GetUsers(string user)
        {
            try
            {
                var admin = GetAdmin(user);

                List<User> usersLst = new List<User>();

                if (admin != null)
                {
                    usersLst = _ctx.Users.OrderByDescending(d => d.ID).ToList();

                    for (int i = 0; i < usersLst.Count; i++)
                    {
                        int roleID = usersLst[i].RoleID;
                        var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();

                        usersLst[i].Role = role;
                    }
                }
                else if (admin == null)
                {
                    var _userRole = GetUserRole(user);

                    if (_userRole.AdminForScale)
                    {
                        var rSA = _ctx.Roles.Where(w => w.AdminForScale || w.Scales || w.POSs).ToList();

                        if (rSA != null)
                        {
                            for (int i = 0; i < rSA.Count; i++)
                            {
                                var rID = rSA[i].ID;
                                var scaleUsers = _ctx.Users.Where(w => w.RoleID == rID).ToList();
                                usersLst.AddRange(scaleUsers);
                            }

                            usersLst = usersLst.GroupBy(g => g.ID).Select(s => s.First()).ToList();
                        }
                    }
                }

                return usersLst;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return null;
            }
        }

        public bool SaveNewUser(User user)
        {
            try
            {
                _ctx.Users.Add(user);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return false;
            }
        }

        public User GetUser(int? id)
        {
            try
            {
                User user = _ctx.Users.Find(id);

                return user;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return null;
            }
        }

        public bool SaveEditUser(User user)
        {
            try
            {
                _ctx.Users.Update(user);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return false;
            }
        }

        public bool DeleteUser(int? id)
        {
            try
            {
                User user = _ctx.Users.Find(id);
                _ctx.Users.Remove(user);
                _ctx.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return false;
            }
        }

        #endregion

        #region Cashiers

        public CashierView GetCashiers(string user, string marketID)
        {
            try
            {
                var admin = _ctx.Admins.Where(w => w.Login == user).FirstOrDefault();
                var users = _ctx.Users.Where(w => w.Login == user).FirstOrDefault();

                CashierView cashierView = new CashierView();

                if (admin != null)
                {
                    if (string.IsNullOrEmpty(marketID))
                        marketID = _ctx.Markets.ToList()[0].MarketID;

                    cashierView.Cashiers = _ctx.Cashiers.ToList();//Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                    cashierView.IsAdmin = true;
                    cashierView.UserRole = null;
                    cashierView.Markets = _ctx.Markets.ToList();
                }
                else if (admin == null && users != null)
                {
                    var roleID = users.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = users.MarketID;

                    if (role.AllMarkets)
                    {
                        cashierView.Cashiers = _ctx.Cashiers.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                        cashierView.IsAdmin = false;
                        cashierView.UserRole = role;
                        cashierView.Markets = _ctx.Markets.ToList();
                    }
                    else
                    {
                        cashierView.Cashiers = _ctx.Cashiers.Where(w => w.MarketID == market).OrderByDescending(o => o.ID).ToList();
                        cashierView.IsAdmin = false;
                        cashierView.UserRole = role;
                        cashierView.Markets = _ctx.Markets.Where(w => w.MarketID == market).ToList();
                    }
                }

                return cashierView;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return null;
            }
        }

        public Cashier GetCashier(string id, string market)
        {
            try
            {
                var checkCashierID = _ctx.Cashiers.Where(w => w.ID == id && w.MarketID == market).FirstOrDefault();

                return checkCashierID;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return null;
            }
        }

        public bool SaveNewCashier(string market, Cashier cashier)
        {
            try
            {
                cashier.MarketID = market;
                _ctx.Cashiers.Add(cashier);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return false;
            }
        }

        public bool EditCashier(Cashier _cashier)
        {
            try
            {
                _cashier.DateBegin = DateTime.Now;
                _cashier.DateEnd = DateTime.Now;
                _cashier.IsSavedToMarket = "0";
                _cashier.Password = "";
                _cashier.TabelNumber = "";

                _ctx.Update(_cashier);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                //#region Log
                //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
                //#endregion
                return false;
            }
        }

        #endregion

    }
}
