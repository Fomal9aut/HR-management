using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await employeeRepository.GetAllAsync();
    }

    public async Task<Employee> GetEmployeeByIdAsync(int id)
    {
        return await employeeRepository.GetByIdAsync(id);
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        await employeeRepository.AddAsync(employee);
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        await employeeRepository.UpdateAsync(employee);
    }

    public async Task DeleteEmployeeAsync(int employeeId)
    {
        await employeeRepository.DeleteAsync(employeeId);
    }
}