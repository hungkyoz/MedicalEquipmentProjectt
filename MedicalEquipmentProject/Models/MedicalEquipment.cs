using System;
using System.Collections.Generic;

namespace MedicalEquipmentProject.Models;

public class MedicalEquipment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Model { get; set; }
    public decimal Price { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime PurchaseDate { get; set; }
    public bool IsActive { get; set; } = true;

    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
}
