using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class EmployeeHistoryService(
    IEmployeeHistoryRepository employeeHistoryRepository,
    IEmployeeRepository employeeRepository)
    : IEmployeeHistoryService
{
    public async Task<IEnumerable<EmployeeHistory>> GetAllEmployeesHistories()
    {
        return await employeeHistoryRepository.GetAllAsync();
    }

    public async Task<EmployeeHistory> GetEmployeesHistoryById(int employeeHistoryId)
    {
        return await employeeHistoryRepository.GetByIdAsync(employeeHistoryId);
    }
    
    public async Task AddEmployeeHistoryAsync(EmployeeHistory employeeHistory)
    {
        await employeeHistoryRepository.AddAsync(employeeHistory);
    }

    public async Task UpdateEmployeeHistoryAsync(EmployeeHistory employeeHistory)
    {
        await employeeHistoryRepository.UpdateAsync(employeeHistory);
    }

    public async Task DeleteEmployeeHistoryAsync(int employeeHistoryId)
    {
        await employeeHistoryRepository.DeleteAsync(employeeHistoryId);
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesNotInDepartmentAsync(int departmentId)
    {
        var employeeHistories = await employeeHistoryRepository
            .GetWhereAsync(eh => eh.DepartmentId == departmentId &&
                                 eh.FireDate == null);
        var employeeIdsInDepartment = employeeHistories.Select(eh => eh.EmployeeId).ToList();
        var employees = await employeeRepository.GetWhereAsync(e => !employeeIdsInDepartment.Contains(e.Id));
        return employees;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesInDepartmentAsync(int departmentId)
    {
        return (await employeeHistoryRepository.GetWhereWithIncludeAsync(
            eh => eh.DepartmentId == departmentId &&
                  eh.FireDate == null, eh => eh.Employee))
            .Select(eh => eh.Employee);
    }

    public async Task FireEmployeesInDepartment(DepartmentHistory departmentHistory)
    {
        var firedEmployeesHistories = await employeeHistoryRepository
            .GetWhereAsync(eh => eh.DepartmentId == departmentHistory.DepartmentId &&
                                 eh.FireDate == null);
        foreach (var firedEmployeesHistory in firedEmployeesHistories)
        {
            firedEmployeesHistory.FireDate = departmentHistory.CloseDate;
            await employeeHistoryRepository.UpdateAsync(firedEmployeesHistory);
        }
    }

    public async Task FireEmployee(int employeeId, int departmentId, DateTime fireDate)
    {
        var employeesHistory = (await employeeHistoryRepository
            .GetWhereAsync(eh => eh.EmployeeId == employeeId &&
                                 eh.DepartmentId == departmentId &&
                                 eh.FireDate == null)).FirstOrDefault();
        employeesHistory!.FireDate = fireDate;
        await employeeHistoryRepository.UpdateAsync(employeesHistory);
    }

    public async Task<IEnumerable<EmployeeHistory>> GetAllEmployeesInDepartmentByPeriod(int departmentId, DateTime startDate,
        DateTime endDate)
    {
        return await employeeHistoryRepository.GetWhereWithIncludeAsync(
            eh => eh.DepartmentId == departmentId && eh.HireDate <= endDate &&
                  (eh.FireDate == null || eh.FireDate >= startDate), eh => eh.Employee);
    }
    
}