using System;

namespace ERPLite.Core.DTOs
{
    // Employee DTOs
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int PositionId { get; set; }
        public string PositionTitle { get; set; }
        public decimal Salary { get; set; }
        public string ContactNumber { get; set; }
        public string EmergencyContact { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
    }

    public class CreateEmployeeDto
    {
        public string EmployeeId { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public decimal Salary { get; set; }
        public string ContactNumber { get; set; }
        public string EmergencyContact { get; set; }
        public int? UserId { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public decimal Salary { get; set; }
        public string ContactNumber { get; set; }
        public string EmergencyContact { get; set; }
    }

    // Department DTOs
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ManagerId { get; set; }
        public string ManagerName { get; set; }
    }

    public class CreateDepartmentDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ManagerId { get; set; }
    }

    public class UpdateDepartmentDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ManagerId { get; set; }
    }
}