namespace MedicalEquipmentProject.Models
{
    public class MedicalEquipmentImage
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }

        public string ImagePath { get; set; }

        public MedicalEquipment Equipment { get; set; }
    }
}
