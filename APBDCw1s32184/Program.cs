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
    
    //Iterfejsy
    public interface IUserService {
        User AddUser(User user);
        User GetById(int id);
        IEnumerable<User> GetAll();
    }
    
    public interface IEquipmentService {
        Equipment AddEquipment(Equipment equipment);
        IEnumerable<Equipment> GetAll();
        Equipment GetById(int id);
    }
    
    public class UserService : IUserService {
        private readonly List<User> _users = new();
        //1. Dodawanie nowego użytkownika
        public User AddUser(User user) { _users.Add(user); return user; }

        public User GetById(int id) {
            var user = _users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                Console.WriteLine("User with id " + id + " not found.");
            }
            return user;
        }
        public IEnumerable<User> GetAll() => _users;
    }

    public class EquipmentService : IEquipmentService {
        private readonly List<Equipment> _equipment = new();

        //2. Dodawanie nowego sprzętu
        public Equipment AddEquipment(Equipment equipment) { _equipment.Add(equipment); return equipment; }

        //3. Wyświetlanie listy całego sprzętu
        public IEnumerable<Equipment> GetAll() => _equipment;

        public Equipment GetById(int id)
        {
            var eq = _equipment.SingleOrDefault(e => e.Id == id);
            if (eq == null)
            {
                Console.WriteLine("Equipment with id " + id + " not found.");
            }
            return eq;
        }
    }

    public class Program {
            public static void Main(string[] args) {
                Console.WriteLine("Test");
            }
        }
}