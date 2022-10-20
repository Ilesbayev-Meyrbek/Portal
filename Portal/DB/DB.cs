using NLog;
using UZ.STS.POS2K.DataAccess;
using UZ.STS.POS2K.DataAccess.Models;
using Portal.DTO;

namespace Portal.DB
{
    public class DB
    {
        Logger logger = LogManager.GetCurrentClassLogger();

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

        public string GetMarketForUser(string user, Users currentUser)
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
                new Logs.Logs(currentUser, "DB->GetMarketForUser", "", ex.ToString()).WriteInfoLogs();

                return null;
            }
        }

        public List<MarketsName> GetMarkets(Users currentUser)
        {
            try
            {
                List<MarketsName> markets = _ctx.MarketsName.ToList();

                return markets;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetMarkets", "", ex.ToString()).WriteInfoLogs();
                return new List<MarketsName>();
            }
        }

        public MarketsName GetMarkets(string id, Users currentUser)
        {
            try
            {
                MarketsName markets = _ctx.MarketsName.Where(w => w.MarketID == id).FirstOrDefault();

                return markets;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetMarkets", "", ex.ToString()).WriteInfoLogs();

                return new MarketsName();
            }
        }

        public List<MarketsName> GetMarketsForPrivileges(Users user)
        {
            try
            {
                List<MarketsName> markets = new List<MarketsName>();

                if (user.IsAdmin)
                {
                    markets = _ctx.MarketsName.ToList();
                }
                else
                {
                    var roleID = user.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = user.MarketID;

                    if (role.AllMarkets)
                    {
                        markets = _ctx.MarketsName.ToList();
                    }
                    else
                    {
                        markets = _ctx.MarketsName.Where(w => w.MarketID == market).ToList();
                    }
                }

                return markets;
            }
            catch (Exception ex)
            {
                new Logs.Logs(user, "DB->GetMarketsForPrivileges", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }


        #region Roles

        public List<Roles> GetRoles(Users currentUser)
        {
            try
            {
                var roles = _ctx.Roles.ToList();

                return roles;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetRoles", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public bool SaveNewRole(Roles role, Users currentUser)
        {
            try
            {
                _ctx.Roles.Add(role);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->SaveNewRole", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        public Roles GetRoleForEdit(int? id, Users currentUser)
        {
            try
            {
                Roles role = _ctx.Roles.Find(id);

                return role;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetRoleForEdit", "", ex.ToString()).WriteInfoLogs();

                return null;
            }
        }

        public bool SaveEditRole(Roles role, Users currentUser)
        {
            try
            {
                _ctx.Roles.Update(role);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->SaveEditRole", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        public Roles GetRoleForDelete(int? id, Users currentUser)
        {
            try
            {
                Roles role = _ctx.Roles.Find(id);

                return role;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetRoleForDelete", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public bool DeleteRole(int? id, Users currentUser)
        {
            try
            {
                Roles role = _ctx.Roles.Find(id);
                _ctx.Roles.Remove(role);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->DeleteRole", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        #endregion

        #region Users

        public Users GetUser(int? id, Users currentUser)
        {
            try
            {
                Users user = _ctx.Users.Find(id);

                return user;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->DeleteRole", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public bool DeleteUser(int? id, Users currentUser)
        {
            try
            {
                Users user = _ctx.Users.Find(id);
                _ctx.Users.Remove(user);
                _ctx.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->DeleteRole", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        #endregion

        #region Logo

        public LogoView GetLogos(Users currentUser, string marketID)
        {
            try
            {
                LogoView logosView = new LogoView();

                if (currentUser.IsAdmin)
                {
                    if (string.IsNullOrEmpty(marketID))
                        marketID = _ctx.MarketsName.ToList()[0].MarketID;

                    logosView.Logos = _ctx.Logo.Where(w => w.MarketID == marketID).OrderByDescending(o => o.Id).ToList();
                    logosView.IsAdmin = true;
                    logosView.UserRole = null;
                    logosView.Markets = _ctx.MarketsName.ToList();
                }
                else
                {
                    var roleID = currentUser.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = currentUser.MarketID;

                    if (role.AllMarkets)
                    {
                        logosView.Logos = _ctx.Logo.Where(w => w.MarketID == marketID).OrderByDescending(o => o.Id).ToList();
                        logosView.IsAdmin = false;
                        logosView.UserRole = role;
                        logosView.Markets = _ctx.MarketsName.ToList();
                    }
                    else
                    {
                        logosView.Logos = _ctx.Logo.Where(w => w.MarketID == market).OrderByDescending(o => o.Id).ToList();
                        logosView.IsAdmin = false;
                        logosView.UserRole = role;
                        logosView.Markets = _ctx.MarketsName.Where(w => w.MarketID == market).ToList();
                    }
                }

                return logosView;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetLogos", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public bool DeleteOldLogos(string MarketID, LogoViewModel lvm, Users currentUser)
        {
            try
            {
                var datenow = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                var yearNow = DateTime.Now.AddYears(-1);

                var deleteOldLogo = _ctx.Logo.Where(w => w.MarketID == MarketID && w.DateE < yearNow).ToList();
                for (int i = 0; i < deleteOldLogo.Count; i++)
                {
                    _ctx.Logo.Remove(deleteOldLogo[i]);
                    _ctx.SaveChangesAsync();
                }

                var _logo = _ctx.Logo.Where(w => w.MarketID == MarketID && w.DateBegin < datenow && w.DateEnd > datenow).ToList();
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
                new Logs.Logs(currentUser, "DB->DeleteOldLogos", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        public bool SaveNewLogo(string market, LogoViewModel lvm, string ds, string de, byte[] imageData, Users currentUser)
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

                _ctx.Logo.Add(logo);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->SaveNewLogo", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        public bool EditOldLogos(Logo logo, Users currentUser)
        {
            try
            {
                var datenow = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

                var _logo = _ctx.Logo.Where(w => w.MarketID == logo.MarketID && w.DateBegin < datenow && w.DateEnd > datenow).ToList();
                for (int i = 0; i < _logo.Count; i++)
                {
                    if (_logo[i] != null)
                    {
                        _logo[i].DateE = logo.DateS.AddDays(-1);
                        _logo[i].DateEnd = Convert.ToInt32(_logo[i].DateE.ToString("yyyyMMdd"));

                        _ctx.Update(_logo[i]);
                        _ctx.SaveChanges();
                    }
                }

                _ctx.Logo.Add(logo);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->EditOldLogos", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        public Logo GetLogo(int? id, Users currentUser)
        {
            try
            {
                Logo user = _ctx.Logo.Find(id);

                return user;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetLogo", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public bool SaveEditLogo(Logo logo, Users currentUser)
        {
            try
            {
                _ctx.Logo.Update(logo);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->SaveEditLogo", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        #endregion

        #region Cashiers

        public CashierView GetCashiers(Users currentUser, string marketID)
        {
            try
            {
                CashierView cashierView = new CashierView();

                if (currentUser.IsAdmin)
                {
                    if (string.IsNullOrEmpty(marketID))
                        marketID = _ctx.MarketsName.ToList()[0].MarketID;

                    cashierView.Cashiers = _ctx.Cashiers.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                    cashierView.IsAdmin = true;
                    cashierView.UserRole = null;
                    cashierView.Markets = _ctx.MarketsName.ToList();
                }
                else
                {
                    var roleID = currentUser.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = currentUser.MarketID;

                    if (role.AllMarkets)
                    {
                        cashierView.Cashiers = _ctx.Cashiers.Where(w => w.MarketID == marketID).OrderByDescending(o => o.ID).ToList();
                        cashierView.IsAdmin = false;
                        cashierView.UserRole = role;
                        cashierView.Markets = _ctx.MarketsName.ToList();
                    }
                    else
                    {
                        cashierView.Cashiers = _ctx.Cashiers.Where(w => w.MarketID == market).OrderByDescending(o => o.ID).ToList();
                        cashierView.IsAdmin = false;
                        cashierView.UserRole = role;
                        cashierView.Markets = _ctx.MarketsName.Where(w => w.MarketID == market).ToList();
                    }
                }

                return cashierView;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetCashiers", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public Cashiers GetCashier(string id, string market, Users currentUser)
        {
            try
            {
                var checkCashierID = _ctx.Cashiers.Where(w => w.ID == id && w.MarketID == market).FirstOrDefault();

                return checkCashierID;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetCashier", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public bool SaveNewCashier(string market, Cashiers cashier, Users currentUser)
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
                new Logs.Logs(currentUser, "DB->SaveNewCashier", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        public bool EditCashier(Cashiers _cashier, Users currentUser)
        {
            try
            {
                _cashier.DateBegin = DateTime.Now;
                _cashier.DateEnd = DateTime.Now;
                _cashier.IsSavedToMarket = "0";
                _cashier.Password = "";
                _cashier.TabelNumber = "";

                _ctx.Cashiers.Update(_cashier);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->EditCashier", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        #endregion

        #region Keyboards

        public KeyboardView GetKeyboards(Users currentUser, string marketID)
        {
            try
            {
                KeyboardView keyboardView = new KeyboardView();

                if (string.IsNullOrEmpty(marketID))
                    marketID = _ctx.MarketsName.ToList()[0].MarketID;

                var roleID = currentUser.RoleID;
                var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();

                if (currentUser.IsAdmin)
                {
                    keyboardView.Keyboards = _ctx.POS_SettingsKeyboard.Where(w => w.MarketID == marketID).OrderByDescending(o => o.Id).ToList();
                    keyboardView.IsAdmin = true;
                    keyboardView.UserRole = role;
                    keyboardView.Markets = _ctx.MarketsName.ToList();
                }
                else
                {
                    if (role.AllMarkets)
                    {
                        keyboardView.Keyboards = _ctx.POS_SettingsKeyboard.Where(w => w.MarketID == marketID).OrderByDescending(o => o.Id).ToList();
                        keyboardView.IsAdmin = false;
                        keyboardView.UserRole = role;
                        keyboardView.Markets = _ctx.MarketsName.ToList();
                    }
                    else
                    {
                        var market = currentUser.MarketID;
                        keyboardView.Keyboards = _ctx.POS_SettingsKeyboard.Where(w => w.MarketID == market).OrderByDescending(o => o.Id).ToList();
                        keyboardView.IsAdmin = false;
                        keyboardView.UserRole = role;
                        keyboardView.Markets = _ctx.MarketsName.Where(w => w.MarketID == market).ToList();
                    }
                }

                return keyboardView;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetKeyboards", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public List<SettingsKeys> GetKeys(Users currentUser)
        {
            try
            {
                var keys = _ctx.SettingsKeys.ToList();

                return keys;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetKeys", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public List<SettingsKeys> GetKeyCode(string _value, Users currentUser)
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

        public bool SaveNewKeyboard(POSSettingsKeyboard keyboard, Users currentUser)
        {
            try
            {
                _ctx.POS_SettingsKeyboard.Add(keyboard);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->SaveNewKeyboard", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        public POSSettingsKeyboard GetKeyboard(int? id, Users currentUser)
        {
            try
            {
                var checkKeyboard = _ctx.POS_SettingsKeyboard.Where(w => w.Id == id).FirstOrDefault();

                return checkKeyboard;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetKeyboard", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        public bool EditKeyboard(POSSettingsKeyboard keyboard, Users currentUser)
        {
            try
            {
                _ctx.POS_SettingsKeyboard.Update(keyboard);
                _ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->EditKeyboard", "", ex.ToString()).WriteInfoLogs();
                return false;
            }
        }

        #endregion

        #region POS

        public POSView GetUserForPOS(Users currentUser, string marketID)
        {
            try
            {
                if (string.IsNullOrEmpty(marketID))
                    marketID = _ctx.MarketsName.ToList()[0].MarketID;

                POSView posView = new POSView();

                if (currentUser.IsAdmin)
                {
                    posView.IsAdmin = true;
                    posView.UserRole = null;
                    posView.Market = marketID;
                    posView.Markets = _ctx.MarketsName.ToList();
                }
                else
                {
                    var roleID = currentUser.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = currentUser.MarketID;

                    if (role.AllMarkets)
                    {
                        posView.IsAdmin = false;
                        posView.UserRole = role;
                        posView.Market = marketID;
                        posView.Markets = _ctx.MarketsName.ToList();
                    }
                    else
                    {
                        posView.IsAdmin = false;
                        posView.UserRole = role;
                        posView.Market = market;
                        posView.Markets = _ctx.MarketsName.Where(w => w.MarketID == market).ToList();
                    }
                }

                return posView;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public POSView GetPOSes(POSView posView, Users currentUser)
        {
            try
            {
                posView.Items = new List<GoodStatus>();

                var today = DateTime.Now.Date;

                if (posView.IsAdmin || posView.UserRole.AllMarkets)
                {
                    var markets = GetMarkets(currentUser);

                    //var goodsForToday = _ctx.Database.SqlQuery<GoodDT>("SELECT * FROM [KORZINKA].[dbo].[Goods] g, [KORZINKA].[dbo].[GoodsDetails] gd where g.ID = gd.GoodsID and gd.ServerDateTime >= @today", new SqlParameter("@today", today)).ToList();

                    var goodsForToday =
                        (from g in _ctx.Goods
                         join gd in _ctx.GoodsDetails on g.ID equals gd.GoodsID
                         where gd.ServerDateTime.Date == today.Date
                         select new GoodDT { IsSaved = g.IsSaved, GoodsID = gd.GoodsID, MarketID = g.MarketID, ServerDateTime = gd.ServerDateTime }).ToList();

                    if (goodsForToday.Count > 0)
                    {
                        for (int i = 0; i < markets.Count; i++)
                        {
                            GoodStatus goodStatus = new GoodStatus();
                            var market = markets[i].MarketID;

                            var goodError = goodsForToday.Where(w => w.MarketID == market && !w.IsSaved).ToList();

                            if (goodError.Count > 0)
                            {
                                var errTime = goodError.OrderBy(o => o.ServerDateTime).FirstOrDefault();

                                if (errTime != null)
                                {
                                    goodStatus.MarketID = market;
                                    goodStatus.Status = "Не загружено";
                                    goodStatus.Note = "Товары не обновились за " + errTime.ServerDateTime.ToString("dd.MM.yyyy HH:mm");

                                    posView.Items.Add(goodStatus);
                                }
                            }
                            else
                            {
                                var goodSuccess = goodsForToday.Where(w => w.MarketID == market && w.IsSaved).ToList();
                                var _goodSuccess = goodSuccess.OrderByDescending(o => o.ServerDateTime).FirstOrDefault();

                                if (goodSuccess.Count > 0 && _goodSuccess != null)
                                {
                                    goodStatus.MarketID = market;
                                    goodStatus.Status = "Загружено";
                                    goodStatus.Note = "Товары обновились за " + _goodSuccess.ServerDateTime.ToString("dd.MM.yyyy HH:mm");

                                    posView.Items.Add(goodStatus);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var market = posView.Market;
                    //var goodsForToday = _ctx.Database.SqlQuery<GoodDT>("SELECT * FROM [KORZINKA].[dbo].[Goods] g, [KORZINKA].[dbo].[GoodsDetails] gd where g.ID = gd.GoodsID and gd.ServerDateTime >= @today", new SqlParameter("@today", today)).ToList();
                    var goodsForToday = (from g in _ctx.Goods
                                         join gd in _ctx.GoodsDetails on g.ID equals gd.GoodsID
                                         where g.ID == gd.GoodsID && g.MarketID == market && gd.ServerDateTime >= today
                                         select new GoodDT { IsSaved = g.IsSaved, GoodsID = gd.GoodsID, MarketID = g.MarketID, ServerDateTime = gd.ServerDateTime }).ToList();

                    if (goodsForToday.Count > 0)
                    {
                        GoodStatus goodStatus = new GoodStatus();

                        var goodError = goodsForToday.Where(w => w.MarketID == market && !w.IsSaved).ToList();

                        if (goodError.Count > 0)
                        {
                            var errTime = goodError.OrderBy(o => o.ServerDateTime).FirstOrDefault();

                            if (errTime != null)
                            {
                                goodStatus.MarketID = market;
                                goodStatus.Status = "Не загружено";
                                goodStatus.Note = "Товары не обновились за " + errTime.ServerDateTime.ToString("dd.MM.yyyy HH:mm");

                                posView.Items.Add(goodStatus);
                            }
                        }
                        else
                        {
                            var goodSuccess = goodsForToday.Where(w => w.MarketID == market && w.IsSaved).ToList();
                            var _goodSuccess = goodSuccess.OrderByDescending(o => o.ServerDateTime).FirstOrDefault();

                            if (goodSuccess.Count > 0 && _goodSuccess != null)
                            {
                                goodStatus.MarketID = market;
                                goodStatus.Status = "Загружено";
                                goodStatus.Note = "Товары обновились за " + _goodSuccess.ServerDateTime.ToString("dd.MM.yyyy HH:mm");

                                posView.Items.Add(goodStatus);
                            }
                        }
                    }
                }

                return posView;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetPOSes", "", ex.ToString()).WriteInfoLogs();
                return null;
            }
        }

        #endregion

        #region Scales

        public ScaleView GetUserForScales(Users currentUser, string marketID)
        {
            try
            {
                if (string.IsNullOrEmpty(marketID))
                    marketID = _ctx.MarketsName.ToList()[0].MarketID;

                ScaleView scalesView = new ScaleView();

                if (currentUser.IsAdmin)
                {
                    scalesView.IsAdmin = true;
                    scalesView.UserRole = null;
                    scalesView.Market = marketID;
                    scalesView.Markets = _ctx.MarketsName.ToList();
                }
                else
                {
                    var roleID = currentUser.RoleID;
                    var role = _ctx.Roles.Where(w => w.ID == roleID).FirstOrDefault();
                    var market = currentUser.MarketID;

                    if (role.AllMarkets)
                    {
                        scalesView.IsAdmin = false;
                        scalesView.UserRole = role;
                        scalesView.Market = marketID;
                        scalesView.Markets = _ctx.MarketsName.ToList();
                    }
                    else
                    {
                        scalesView.IsAdmin = false;
                        scalesView.UserRole = role;
                        scalesView.Market = market;
                        scalesView.Markets = _ctx.MarketsName.Where(w => w.MarketID == market).ToList();
                    }
                }

                return scalesView;
            }
            catch (Exception ex)
            {
                new Logs.Logs(currentUser, "DB->GetUserForScales", "", ex.ToString()).WriteInfoLogs();
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
