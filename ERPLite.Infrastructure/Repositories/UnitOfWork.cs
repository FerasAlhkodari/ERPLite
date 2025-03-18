// ERPLite.Infrastructure/Repositories/UnitOfWork.cs
using ERPLite.Core.Domain;
using ERPLite.Core.Interfaces;
using ERPLite.Infrastructure.Data;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ERPLite.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        private IRepository<User> _userRepository;
        private IRepository<Role> _roleRepository;
        private IRepository<Permission> _permissionRepository;
        private IRepository<Employee> _employeeRepository;
        private IRepository<Department> _departmentRepository;
        private IRepository<Position> _positionRepository;
        private IRepository<Attendance> _attendanceRepository;
        private IRepository<Leave> _leaveRepository;
        private IRepository<Product> _productRepository;
        private IRepository<Inventory> _inventoryRepository;
        private IRepository<InventoryTransaction> _inventoryTransactionRepository;
        private IRepository<Supplier> _supplierRepository;
        private IRepository<PurchaseOrder> _purchaseOrderRepository;
        private IRepository<PurchaseOrderItem> _purchaseOrderItemRepository;
        private IRepository<Budget> _budgetRepository;
        private IRepository<Expense> _expenseRepository;
        private IRepository<ExpenseApproval> _expenseApprovalRepository;
        private IRepository<Report> _reportRepository;
        private IRepository<Dashboard> _dashboardRepository;
        private IRepository<DashboardWidget> _dashboardWidgetRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IRepository<User> UserRepository => _userRepository ??= new Repository<User>(_dbContext);
        public IRepository<Role> RoleRepository => _roleRepository ??= new Repository<Role>(_dbContext);
        public IRepository<Permission> PermissionRepository => _permissionRepository ??= new Repository<Permission>(_dbContext);
        public IRepository<Employee> EmployeeRepository => _employeeRepository ??= new Repository<Employee>(_dbContext);
        public IRepository<Department> DepartmentRepository => _departmentRepository ??= new Repository<Department>(_dbContext);
        public IRepository<Position> PositionRepository => _positionRepository ??= new Repository<Position>(_dbContext);
        public IRepository<Attendance> AttendanceRepository => _attendanceRepository ??= new Repository<Attendance>(_dbContext);
        public IRepository<Leave> LeaveRepository => _leaveRepository ??= new Repository<Leave>(_dbContext);
        public IRepository<Product> ProductRepository => _productRepository ??= new Repository<Product>(_dbContext);
        public IRepository<Inventory> InventoryRepository => _inventoryRepository ??= new Repository<Inventory>(_dbContext);
        public IRepository<InventoryTransaction> InventoryTransactionRepository => _inventoryTransactionRepository ??= new Repository<InventoryTransaction>(_dbContext);
        public IRepository<Supplier> SupplierRepository => _supplierRepository ??= new Repository<Supplier>(_dbContext);
        public IRepository<PurchaseOrder> PurchaseOrderRepository => _purchaseOrderRepository ??= new Repository<PurchaseOrder>(_dbContext);
        public IRepository<PurchaseOrderItem> PurchaseOrderItemRepository =>
            _purchaseOrderItemRepository ??= new Repository<PurchaseOrderItem>(_dbContext);
        public IRepository<Budget> BudgetRepository => _budgetRepository ??= new Repository<Budget>(_dbContext);
        public IRepository<Expense> ExpenseRepository => _expenseRepository ??= new Repository<Expense>(_dbContext);
        public IRepository<ExpenseApproval> ExpenseApprovalRepository => _expenseApprovalRepository ??= new Repository<ExpenseApproval>(_dbContext);

        // New repositories for the Reporting Module
        public IRepository<Report> ReportRepository => _reportRepository ??= new Repository<Report>(_dbContext);
        public IRepository<Dashboard> DashboardRepository => _dashboardRepository ??= new Repository<Dashboard>(_dbContext);
        public IRepository<DashboardWidget> DashboardWidgetRepository => _dashboardWidgetRepository ??= new Repository<DashboardWidget>(_dbContext);

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}