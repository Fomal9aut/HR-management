using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class DepartmentService(IDepartmentRepository departmentRepository) : IDepartmentService
{
    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await departmentRepository.GetAllAsync();
    }
    
    public async Task<IEnumerable<Department>> GetAllDepartmentsExceptGivenAsync(int departmentId)
    {
        return await departmentRepository.GetWhereAsync(d => d.Id != departmentId);
    }

    public async Task<Department> GetDepartmentByIdAsync(int id)
    {
        return await departmentRepository.GetByIdAsync(id);
    }

    public async Task<Department> GetDepartmentByIdIncludeParentAsync(int departmentId)
    {
        return (await departmentRepository.GetWhereWithIncludeAsync(d => d.Id == departmentId, d => d.ParentDepartment)).First();
    }

    public async Task AddDepartmentAsync(Department department)
    {
        await departmentRepository.AddAsync(department);
    }

    public async Task UpdateDepartmentAsync(Department department)
    {
        await departmentRepository.UpdateAsync(department);
    }

    public async Task DeleteDepartmentAsync(int departmentId)
    {
        await departmentRepository.DeleteAsync(departmentId);
    }
}