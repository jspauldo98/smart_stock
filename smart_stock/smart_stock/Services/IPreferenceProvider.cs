using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface IPreferenceProvider
    {
        //Interface to be used when creating a new tradeStrategy through the registration process
        Task<List<string>> InsertPreference(Preference preference);

        Task<IEnumerable<RiskLevel>> GetRiskLevels();
    }
}