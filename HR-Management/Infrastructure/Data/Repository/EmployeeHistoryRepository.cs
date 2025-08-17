using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Repository;

public class EmployeeHistoryRepository : Repository<EmployeeHistory>, IEmployeeHistoryRepository
{
    public EmployeeHistoryRepository(ApplicationDbContext context, ILogger<EmployeeHistory> logger) : base(context, logger)
    {
    }
}