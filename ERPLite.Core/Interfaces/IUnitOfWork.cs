// ERPLite.Core/Interfaces/IUnitOfWork.cs
using ERPLite.Core.Domain;
using System;
using System.Threading.Tasks;

namespace ERPLite.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<Permission> PermissionRepository { get; }
        IRepository<Employee> EmployeeRepository { get; }
        IRepository<Department> DepartmentRepository { get; }
        IRepository<Position> PositionRepository { get; }
        IRepository<Attendance> AttendanceRepository { get; }
        IRepository<Leave> LeaveRepository { get; }
        IRepository<Product> ProductRepository { get; }
        IRepository<Inventory> InventoryRepository { get; }
        IRepository<InventoryTransaction> InventoryTransactionRepository { get; }
        IRepository<Supplier> SupplierRepository { get; }
        IRepository<PurchaseOrder> PurchaseOrderRepository { get; }
        IRepository<PurchaseOrderItem> PurchaseOrderItemRepository { get; }
        IRepository<Budget> BudgetRepository { get; }
        IRepository<Expense> ExpenseRepository { get; }
        IRepository<ExpenseApproval> ExpenseApprovalRepository { get; }
        IRepository<Report> ReportRepository { get; }
        IRepository<Dashboard> DashboardRepository { get; }
        IRepository<DashboardWidget> DashboardWidgetRepository { get; }

        Task<int> SaveChangesAsync();
    }
}