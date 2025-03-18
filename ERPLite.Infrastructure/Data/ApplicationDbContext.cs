using ERPLite.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ERPLite.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSets for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> POItems { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseApproval> ExpenseApprovals { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<DashboardWidget> DashboardWidgets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Apply all configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            // Configure specific entity mappings here
            ConfigureUserEntity(modelBuilder);
            ConfigureEmployeeEntity(modelBuilder);
            ConfigureInventoryEntity(modelBuilder);
            ConfigureExpenseEntity(modelBuilder);
            ConfigureReportingEntity(modelBuilder);
        }

        private void ConfigureReportingEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Format).HasMaxLength(20);
            });

            modelBuilder.Entity<Dashboard>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<DashboardWidget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.WidgetType).HasMaxLength(20);
                entity.Property(e => e.ChartType).HasMaxLength(20);

                entity.HasOne(e => e.Dashboard)
                    .WithMany(d => d.Widgets)
                    .HasForeignKey(e => e.DashboardId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureEmployeeEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.EmergencyContact).HasMaxLength(20);
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.EmployeeId).IsUnique();

                // Employee - User relationship (one-to-one)
                entity.HasOne(e => e.User)
                    .WithOne(u => u.Employee)
                    .HasForeignKey<Employee>(e => e.UserId);

                // Employee - Department relationship (many-to-one)
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Employee - Position relationship (many-to-one)
                entity.HasOne(e => e.Position)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(e => e.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.Code).IsUnique();

                // Department - Manager relationship (one-to-one)
                entity.HasOne(d => d.Manager)
                    .WithMany()
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("Positions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);

                // Position - Department relationship (many-to-one)
                entity.HasOne(p => p.Department)
                    .WithMany()
                    .HasForeignKey(p => p.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("Attendances");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);

                // Attendance - Employee relationship (many-to-one)
                entity.HasOne(a => a.Employee)
                    .WithMany(e => e.Attendances)
                    .HasForeignKey(a => a.EmployeeId);
            });

            modelBuilder.Entity<Leave>(entity =>
            {
                entity.ToTable("Leaves");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LeaveType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Reason).HasMaxLength(500);

                // Leave - Employee relationship (many-to-one)
                entity.HasOne(l => l.Employee)
                    .WithMany(e => e.Leaves)
                    .HasForeignKey(l => l.EmployeeId);

                // Leave - Approver relationship (many-to-one)
                entity.HasOne(l => l.Approver)
                    .WithMany()
                    .HasForeignKey(l => l.ApproverId)
                    .IsRequired(false);
            });
        }

        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.PreferredLanguage).HasMaxLength(10);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique();
            });

            // Many-to-Many: User <-> Role
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            // Many-to-Many: Role <-> Permission
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(e => new { e.RoleId, e.PermissionId });

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId);
            });
        }

        private void ConfigureInventoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Sku).IsRequired().HasMaxLength(50); // Changed from SKU to Sku
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);
                entity.Property(e => e.Barcode).HasMaxLength(50);
                entity.HasIndex(e => e.Sku).IsUnique(); // Changed from SKU to Sku
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("Inventories");
                entity.HasKey(e => e.Id);

                // Inventory - Product relationship (one-to-one)
                entity.HasOne(i => i.Product)
                    .WithOne(p => p.Inventory)
                    .HasForeignKey<Inventory>(i => i.ProductId);
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.ToTable("InventoryTransactions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ReferenceType).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(500);

                // Transaction - Product relationship (many-to-one)
                entity.HasOne(t => t.Product)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(t => t.ProductId);

                // Transaction - User relationship (many-to-one)
                entity.HasOne(t => t.User)
                    .WithMany()
                    .HasForeignKey(t => t.UserId);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.ToTable("PurchaseOrders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PoNumber).IsRequired().HasMaxLength(20); // Changed from PONumber to PoNumber
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.PoNumber).IsUnique(); // Changed from PONumber to PoNumber

                // PO - Supplier relationship (many-to-one)
                entity.HasOne(po => po.Supplier)
                    .WithMany(s => s.PurchaseOrders)
                    .HasForeignKey(po => po.SupplierId);

                // PO - Creator relationship (many-to-one)
                entity.HasOne(po => po.CreatedBy)
                    .WithMany()
                    .HasForeignKey(po => po.CreatedById);

                // PO - Approver relationship (many-to-one)
                entity.HasOne(po => po.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(po => po.ApprovedById)
                    .IsRequired(false);
            });

            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.ToTable("PurchaseOrderItem");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.LineTotal).HasColumnType("decimal(18,2)");

                // POItem - PO relationship (many-to-one)
                entity.HasOne(poi => poi.PurchaseOrder)
                    .WithMany(po => po.Items)
                    .HasForeignKey(poi => poi.PurchaseOrderId);

                // POItem - Product relationship (many-to-one)
                entity.HasOne(poi => poi.Product)
                    .WithMany()
                    .HasForeignKey(poi => poi.ProductId);
            });
        }

        private void ConfigureExpenseEntity(ModelBuilder modelBuilder)
        {
            // Budget configuration stays the same
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.ToTable("Budgets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);

                // Budget - Department relationship (many-to-one)
                entity.HasOne(b => b.Department)
                    .WithMany(d => d.Budgets)
                    .HasForeignKey(b => b.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict); // Change to Restrict
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.ToTable("Expenses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Category).HasMaxLength(50);

                // Expense - Department relationship (many-to-one)
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Expenses)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict); // Change to Restrict

                // Expense - Requester relationship (many-to-one)
                entity.HasOne(e => e.Requester)
                    .WithMany()
                    .HasForeignKey(e => e.RequesterId)
                    .OnDelete(DeleteBehavior.Restrict); // Change to Restrict

                // Expense - Budget relationship (many-to-one)
                entity.HasOne(e => e.Budget)
                    .WithMany()
                    .HasForeignKey(e => e.BudgetId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull); // Change to SetNull for nullable FKs
            });

            modelBuilder.Entity<ExpenseApproval>(entity =>
            {
                entity.ToTable("ExpenseApprovals");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Comments).HasMaxLength(500);

                // Approval - Expense relationship (many-to-one)
                entity.HasOne(ea => ea.Expense)
                    .WithMany(e => e.Approvals)
                    .HasForeignKey(ea => ea.ExpenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull); // Use ClientSetNull

                // Approval - Approver relationship (many-to-one)
                entity.HasOne(ea => ea.Approver)
                    .WithMany()
                    .HasForeignKey(ea => ea.ApproverId)
                    .OnDelete(DeleteBehavior.Restrict); // Change to Restrict
            });
        }
    }
}