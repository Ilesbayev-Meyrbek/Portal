using Portal.DTO;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Services.Interfaces
{
    public interface IChequeService
    {
        public Task<Result<List<string>>> GetAllTerminalIDAsync();
        
        public Task<Result<List<AccountantReport>>> GetAllAsync(int dateBegin, int dateEnd, string marketID, int posNumber, string terminalID);

        public Result<MemoryStream> GetCSVReport(List<AccountantReport> report);
    }
}