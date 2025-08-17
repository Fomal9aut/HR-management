using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class DepartmentHistoryService(
    IDepartmentHistoryRepository departmentHistoryRepository,
    IDepartmentRepository departmentRepository,
    IEmployeeHistoryService employeesHistoryService)
    : IDepartmentHistoryService
{
    public async Task<IEnumerable<DepartmentHistory>> GetAllDepartmentHistoriesAsync()
    {
        return await departmentHistoryRepository.GetAllAsync();
    }

    public async Task<DepartmentHistory> GetDepartmentHistoryByIdAsync(int departmentHistoryId)
    {
        return await departmentHistoryRepository.GetByIdAsync(departmentHistoryId);
    }

    public async Task<DepartmentHistory> GetOpenedDepartmentHistoryByDepartmentId(int departmentId)
    {
        return (await departmentHistoryRepository.GetWhereAsync(dh =>
            dh.DepartmentId == departmentId && dh.CloseDate == null)).FirstOrDefault() ?? throw new NotFoundException("History of department is not found");
    }

    public async Task AddDepartmentHistoryAsync(DepartmentHistory departmentHistory)
    {
        await departmentHistoryRepository.AddAsync(departmentHistory);
    }

    public async Task UpdateDepartmentHistoryAsync(DepartmentHistory departmentHistory)
    {
        await departmentHistoryRepository.UpdateAsync(departmentHistory);
    }

    public async Task DeleteDepartmentHistoryAsync(int departmentHistoryId)
    {
        await departmentHistoryRepository.DeleteAsync(departmentHistoryId);
    }

    public async Task CloseDepartmentAndSubDepartmentsAsync(int departmentId, DateTime closeDate)
    {
        var departmentHistory =
            (await departmentHistoryRepository.GetWhereAsync(dh =>
                dh.DepartmentId == departmentId && dh.CloseDate == null)).FirstOrDefault();

        if (departmentHistory != null)
        {
            departmentHistory.CloseDate = closeDate;
            await departmentHistoryRepository.UpdateAsync(departmentHistory);
            await employeesHistoryService.FireEmployeesInDepartment(departmentHistory);
        }

        var subDepartments = (await departmentRepository
            .GetWhereAsync(d => d.ParentDepartmentId == departmentId)).ToList();

        foreach (var subDept in subDepartments)
        {
            await CloseDepartmentAndSubDepartmentsAsync(subDept.Id, closeDate);
        }
    }
    
    public async Task<IEnumerable<Department>> GetAllOpenedDepartmentsAsync(DateTime date)
    {
        return (await departmentHistoryRepository.GetWhereWithIncludeAsync(
            dh => dh.OpenDate <= date && (dh.CloseDate == null || dh.CloseDate >= date),
            dh => dh.Department.ParentDepartment)).Select(dh => dh.Department);
    }

    public async Task<IEnumerable<Department>> GetAllOpenedRootDepartmentsAsync(DateTime date)
    {
        var departmentHistories = await departmentHistoryRepository.GetWhereWithIncludeAsync(
            dh => dh.OpenDate <= date && (dh.CloseDate == null || dh.CloseDate >= date),
            dh => dh.Department.SubDepartments);

        return departmentHistories
            .Where(dh => dh.Department.ParentDepartmentId == null)
            .Select(dh => dh.Department);
    }
}