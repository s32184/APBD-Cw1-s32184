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
            Console.WriteLine("Test");
        }    
    }
    
    //Możliwe typy użytkowników
    public enum UserType { Student, Employee }
    
    //Klasa dla Użytkownika
    public abstract class User {
        private static int _idCounter = 1;
        protected User(string firstName, string lastName, UserType userType) { Id = _idCounter++; FirstName = firstName; LastName = lastName; UserType = userType; }
        
        public int Id { get; }
        private string FirstName { get; }
        private string LastName { get; }
        private UserType UserType { get; }
        public abstract int MaxActiveRentals { get; }
    }
    
    //Klasa dla Studenta
    public class Student : User {
        public Student(string firstName, string lastName) : base(firstName, lastName, UserType.Student) {}
        
        //Maksymalnie 2 wypożyczenia jednocześnie
        public override int MaxActiveRentals => 2;
    }
    
    //Klasa dla Pracownnika
    public class Employee : User {
        public Employee(string firstName, string lastName) : base(firstName, lastName, UserType.Employee) {}
        
        //Maksymalnie 5 wypożyczeń jednocześnie
        public override int MaxActiveRentals => 5;
    }
}