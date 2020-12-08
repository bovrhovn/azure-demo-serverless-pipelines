using System.Threading.Tasks;
using Demo.Functions.Models;

namespace Demo.Functions.Interfaces
{
    public interface IDOOperations
    {
        Task<DOBuild> GetLatestBuildAsync();
    }
}