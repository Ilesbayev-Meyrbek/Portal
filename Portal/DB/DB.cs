using Portal.Models;
using System.Data.SqlClient;

namespace Portal.DB
{
    public class DB
    {
        private readonly DataContext _ctx;
        private readonly ScaleContext _sctx;

        public DB()
        {
            
        }

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

        #region Logo

        public LogoView GetLogos(string user, string marketID)
        {
            try
            {
                var admin = _ctx.Admins.Where(w => w.Login == user).FirstOrDefault();
                var users = _ctx.Users.Where(w => w.Login == user).FirstOrDefault();

                if (string.IsNullOrEmpty(marketID))
                    marketID = _ctx.Markets.ToList()[0].MarketID;

                LogoView logosView = new LogoView();

                if (admin != null)
                {
                    logosView.Logos = _ctx.Logos.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                    logosView.IsAdmin = true;
                    logosView.UserRole = null;
                    logosView.Markets = _ctx.Markets.ToList();
                }
                else if (admin == null && users != null)
                {
                    var roleID = users.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = users.MarketID;

                    if (role.AllMarkets)
                    {
                        logosView.Logos = _ctx.Logos.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                        logosView.IsAdmin = false;
                        logosView.UserRole = role;
                        logosView.Markets = _ctx.Markets.ToList();
                    }
                    else
                    {
                        logosView.Logos = _ctx.Logos.Where(w => w.MarketID == market).OrderByDescending(o => o.ID).ToList();
                        logosView.IsAdmin = false;
                        logosView.UserRole = role;
                        logosView.Markets = _ctx.Markets.Where(w => w.MarketID == market).ToList();
                    }
                }

                //Log.WriteLogoLog(logosView.Logos);

                return logosView;
            }
            catch (Exception ex)
            {
                //Log.WriteErrorLog(ex.ToString());
                return null;
            }
        }

        public bool DeleteOldLogos(string MarketID, LogoViewModel lvm)
        {
            try
            {
                var datenow = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                var yearNow = DateTime.Now.AddYears(-1);

                var deleteOldLogo = _ctx.Logos.Where(w => w.MarketID == MarketID && w.DateE < yearNow).ToList();
                for (int i = 0; i < deleteOldLogo.Count; i++)
                {
                    _ctx.Logos.Remove(deleteOldLogo[i]);
                    _ctx.SaveChangesAsync();
                }

                var _logo = _ctx.Logos.Where(w => w.MarketID == MarketID && w.DateBegin < datenow && w.DateEnd > datenow).ToList();
                for (int i = 0; i < _logo.Count; i++)
                {
                    if (_logo[i] != null)
                    {
                        _logo[i].DateE = lvm.DateS.AddDays(-1);
                        _logo[i].DateEnd = Convert.ToInt32(_logo[i].DateE.ToString("yyyyMMdd"));

                        _ctx.Update(_logo[i]);
                        _ctx.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveNewLogo(string market, LogoViewModel lvm, string ds, string de, byte[] imageData)
        {
            try
            {
                Logo logo = new Logo();
                logo.MarketID = market;
                logo.DateS = lvm.DateS;
                logo.DateE = lvm.DateE;
                logo.DateBegin = Convert.ToInt32(ds);
                logo.DateEnd = Convert.ToInt32(de);
                logo.BMP = imageData;
                logo.Note = lvm.Note;
                logo.IsSaved = false;
                logo.IsSavedToPOS = 0;
                
                _ctx.Logos.Add(logo);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
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

                    cashierView.Cashiers = _ctx.Cashiers.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
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

        #region Keyboards

        public KeyboardView GetKeyboards(string user, string marketID)
        {
            try
            {
                var admin = _ctx.Admins.Where(w => w.Login == user).FirstOrDefault();
                var users = _ctx.Users.Where(w => w.Login == user).FirstOrDefault();

                KeyboardView keyboardView = new KeyboardView();

                if (admin != null)
                {
                    if (string.IsNullOrEmpty(marketID))
                        marketID = _ctx.Markets.ToList()[0].MarketID;

                    keyboardView.Keyboards = _ctx.Keyboards.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                    keyboardView.IsAdmin = true;
                    keyboardView.UserRole = null;
                    keyboardView.Markets = _ctx.Markets.ToList();
                }
                else if (admin == null && users != null)
                {
                    var roleID = users.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();

                    if (role.AllMarkets)
                    {
                        keyboardView.Keyboards = _ctx.Keyboards.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                        keyboardView.IsAdmin = false;
                        keyboardView.UserRole = role;
                        keyboardView.Markets = _ctx.Markets.ToList();
                    }
                    else
                    {
                        var market = users.MarketID;
                        keyboardView.Keyboards = _ctx.Keyboards.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                        keyboardView.IsAdmin = false;
                        keyboardView.UserRole = role;
                        keyboardView.Markets = _ctx.Markets.Where(w => w.MarketID == market).ToList();
                    }
                }

                //Log.WriteCashierLog(keyboardView.Keyboards);

                return keyboardView;
            }
            catch (Exception ex)
            {
                //Log.WriteErrorLog(ex.ToString());
                return null;
            }
        }

        public List<SettingsKey> GetKeys()
        {
            try
            {
                var keys = _ctx.SettingsKeys.ToList();

                return keys;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<SettingsKey> GetKeyCode(string _value)
        {
            try
            {
                var keyCode = _ctx.SettingsKeys.Where(w => w.Value == _value).ToList();

                return keyCode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SaveNewKeyboard(string market, Keyboard keyboard)
        {
            try
            {
                keyboard.MarketID = market;
                _ctx.Keyboards.Add(keyboard);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Keyboard GetKeyboard(int? id)
        {
            try
            {
                var checkKeyboard = _ctx.Keyboards.Where(w => w.ID == id).FirstOrDefault();

                return checkKeyboard;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool EditKeyboard(string market, Keyboard keyboard)
        {
            try
            {
                keyboard.MarketID = market;

                _ctx.Update(keyboard);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region POS

        //public POSView GetUserForPOS(string user, string marketID)
        //{
        //    try
        //    {
        //        var admin = _ctx.Admins.Where(w => w.Login == user).FirstOrDefault();
        //        var users = _ctx.Users.Where(w => w.Login == user).FirstOrDefault();

        //        if (string.IsNullOrEmpty(marketID))
        //            marketID = _ctx.Markets.ToList()[0].MarketID;

        //        POSView posView = new POSView();

        //        if (admin != null)
        //        {
        //            posView.IsAdmin = true;
        //            posView.UserRole = null;
        //            posView.Market = marketID;
        //            posView.Markets = _ctx.Markets.ToList();
        //        }
        //        else if (admin == null && users != null)
        //        {
        //            var roleID = users.RoleID;
        //            var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
        //            var market = users.MarketID;

        //            if (role.AllMarkets)
        //            {
        //                posView.IsAdmin = false;
        //                posView.UserRole = role;
        //                posView.Market = marketID;
        //                posView.Markets = _ctx.Markets.ToList();
        //            }
        //            else
        //            {
        //                posView.IsAdmin = false;
        //                posView.UserRole = role;
        //                posView.Market = market;
        //                posView.Markets = _ctx.Markets.Where(w => w.MarketID == market).ToList();
        //            }
        //        }

        //        return posView;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public POSView GetPOSes(POSView posView)
        //{
        //    try
        //    {
        //        posView.Items = new List<GoodStatus>();

        //        var today = DateTime.Now.Date;

        //        if (posView.IsAdmin || posView.UserRole.AllMarkets)
        //        {
        //            var markets = GetMarkets();

        //            var goodsForToday = _ctx.Database.SqlQuery<GoodDT>("SELECT * FROM [KORZINKA].[dbo].[Goods] g, [KORZINKA].[dbo].[GoodsDetails] gd where g.ID = gd.GoodsID and gd.ServerDateTime >= @today", new SqlParameter("@today", today)).ToList();

        //            if (goodsForToday.Count > 0)
        //            {
        //                for (int i = 0; i < markets.Count; i++)
        //                {
        //                    GoodStatus goodStatus = new GoodStatus();
        //                    var market = markets[i].MarketID;

        //                    var goodError = goodsForToday.Where(w => w.MarketID == market && !w.IsSaved).ToList();

        //                    if (goodError.Count > 0)
        //                    {
        //                        var errTime = goodError.OrderBy(o => o.ServerDateTime).FirstOrDefault();

        //                        if (errTime != null)
        //                        {
        //                            goodStatus.MarketID = market;
        //                            goodStatus.Status = "Не загружено";
        //                            goodStatus.Note = "Товары не обновились за " + errTime.ServerDateTime.ToString("dd.MM.yyyy HH:mm");

        //                            posView.Items.Add(goodStatus);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var goodSuccess = goodsForToday.Where(w => w.MarketID == market && w.IsSaved).ToList();
        //                        var _goodSuccess = goodSuccess.OrderByDescending(o => o.ServerDateTime).FirstOrDefault();

        //                        if (goodSuccess.Count > 0 && _goodSuccess != null)
        //                        {
        //                            goodStatus.MarketID = market;
        //                            goodStatus.Status = "Загружено";
        //                            goodStatus.Note = "Товары обновились за " + _goodSuccess.ServerDateTime.ToString("dd.MM.yyyy HH:mm");

        //                            posView.Items.Add(goodStatus);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var market = posView.Market;
        //            var goodsForToday = _ctx.Database.SqlQuery<GoodDT>("SELECT * FROM [KORZINKA].[dbo].[Goods] g, [KORZINKA].[dbo].[GoodsDetails] gd where g.ID = gd.GoodsID and gd.ServerDateTime >= @today", new SqlParameter("@today", today)).ToList();

        //            if (goodsForToday.Count > 0)
        //            {
        //                GoodStatus goodStatus = new GoodStatus();

        //                var goodError = goodsForToday.Where(w => w.MarketID == market && !w.IsSaved).ToList();

        //                if (goodError.Count > 0)
        //                {
        //                    var errTime = goodError.OrderBy(o => o.ServerDateTime).FirstOrDefault();

        //                    if (errTime != null)
        //                    {
        //                        goodStatus.MarketID = market;
        //                        goodStatus.Status = "Не загружено";
        //                        goodStatus.Note = "Товары не обновились за " + errTime.ServerDateTime.ToString("dd.MM.yyyy HH:mm");

        //                        posView.Items.Add(goodStatus);
        //                    }
        //                }
        //                else
        //                {
        //                    var goodSuccess = goodsForToday.Where(w => w.MarketID == market && w.IsSaved).ToList();
        //                    var _goodSuccess = goodSuccess.OrderByDescending(o => o.ServerDateTime).FirstOrDefault();

        //                    if (goodSuccess.Count > 0 && _goodSuccess != null)
        //                    {
        //                        goodStatus.MarketID = market;
        //                        goodStatus.Status = "Загружено";
        //                        goodStatus.Note = "Товары обновились за " + _goodSuccess.ServerDateTime.ToString("dd.MM.yyyy HH:mm");

        //                        posView.Items.Add(goodStatus);
        //                    }
        //                }
        //            }
        //        }

        //        return posView;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.WriteErrorLog(ex.ToString());
        //        return null;
        //    }
        //}

        #endregion

        #region Scales

        public ScaleView GetUserForScales(string user, string marketID)
        {
            try
            {
                var admin = _ctx.Admins.Where(w => w.Login == user).FirstOrDefault();
                var users = _ctx.Users.Where(w => w.Login == user).FirstOrDefault();

                if (string.IsNullOrEmpty(marketID))
                    marketID = _ctx.Markets.ToList()[0].MarketID;

                ScaleView scalesView = new ScaleView();

                if (admin != null)
                {
                    //scalesView.Scales = ctx.Scales.Where(w => w.MarketID == marketID).ToList();
                    scalesView.IsAdmin = true;
                    scalesView.UserRole = null;
                    scalesView.Market = marketID;
                    scalesView.Markets = _ctx.Markets.ToList();
                }
                else if (admin == null && users != null)
                {
                    var roleID = users.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = users.MarketID;

                    if (role.AllMarkets)
                    {
                        //scalesView.Scales = ctx.Scales.Where(w => w.MarketID == marketID).ToList();
                        scalesView.IsAdmin = false;
                        scalesView.UserRole = role;
                        scalesView.Market = marketID;
                        scalesView.Markets = _ctx.Markets.ToList();
                    }
                    else
                    {
                        //scalesView.Scales = ctx.Scales.Where(w => w.MarketID == market).ToList();
                        scalesView.IsAdmin = false;
                        scalesView.UserRole = role;
                        scalesView.Market = market;
                        scalesView.Markets = _ctx.Markets.Where(w => w.MarketID == market).ToList();
                    }
                }

                return scalesView;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ScaleView GetScales(ScaleView scaleView)
        {
            try
            {
                List<MarketsName> markets = new List<MarketsName>();

                if (scaleView.IsAdmin)
                {
                    scaleView.Scales = _sctx.Scales.Where(w => !w.Tiger && !w.BPlus && !w.FreshBase).OrderBy(o => o.MarketID).ToList();
                }
                else
                {
                    if (scaleView.UserRole.AllMarkets)
                    {
                        scaleView.Scales = _sctx.Scales.Where(w => !w.Tiger && !w.BPlus && !w.FreshBase).OrderBy(o => o.MarketID).ToList();
                    }
                    else
                    {
                        scaleView.Scales = _sctx.Scales.Where(w => w.MarketID == scaleView.Market).ToList();
                    }
                }

                return scaleView;
            }
            catch (Exception ex)
            {
                //Log.WriteErrorLog(ex.ToString());
                return null;
            }
        }

        #endregion
    }
}
