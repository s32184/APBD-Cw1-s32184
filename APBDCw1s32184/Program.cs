namespace APBDCw1s32184 {
    //Możliwe statusy sprzętu
    public enum EquipmentStatus { Available, Rented, Unavailable }
    
    //Klasa dla sprzętu
    public abstract class Equipment {
        private static int _idCounter = 1;
        protected Equipment(string name) { Id = _idCounter++; Name = name; Status = EquipmentStatus.Available; }
        
        public int Id { get; }
        private string Name { get; }
        public EquipmentStatus Status { get; }
    }
    //Klasa dla Laptopa
    public class Laptop : Equipment {
        public Laptop(string name, string cpu, int ramGb) : base(name) { Cpu = cpu; RamGb = ramGb; }
        
        private string Cpu { get; }
        private int RamGb { get; }
    }
    //Klasa dla Projektora
    public class Projector : Equipment {
        public Projector(string name, int lumens, string resolution) : base(name) { Lumens = lumens; Resolution = resolution; }
        
        private int Lumens { get; }
        private string Resolution { get; }
    }
    //Klasa dla Kamery
    public class Camera : Equipment {
        public Camera(string name, string sensorType, bool hasStabilization) : base(name) { SensorType = sensorType; HasStabilization = hasStabilization; }
        
        private string SensorType { get; }
        private bool HasStabilization { get; }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Test2");
        }    
    }
}