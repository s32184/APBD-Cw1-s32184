namespace APBDCw1s32184 {
    //Możliwe statusy sprzętu
    public enum EquipmentStatus { Available, Rented, Unavailable }
    
    //Klasa dla sprzętu
    public abstract class Equipment {
        private static int _idCounter = 1;
        protected Equipment(string name) { Id = _idCounter++; Name = name; Status = EquipmentStatus.Available; }
        
        public int Id { get; }
        private string Name { get; }
        public EquipmentStatus Status { get; set; }
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
    
    //Klasa dla wypożyczenia
        public class Rental {
            private static int _idCounter = 1;
            public Rental(User user, Equipment equipment, DateTime startDate, DateTime dueDate)
            { Id = _idCounter++; User = user; Equipment = equipment; StartDate = startDate; DueDate = dueDate; IsReturned = false; LateFee = 0m; }
            
            public int Id { get; }
            public User User { get; }
            public Equipment Equipment { get; }
            private DateTime StartDate { get; }
            public DateTime DueDate { get; }
            public DateTime? ReturnDate { get; private set; }
            public bool IsReturned { get; private set; }
            public decimal LateFee { get; private set; }
            public bool IsOverdue(DateTime now) => !IsReturned && now > DueDate;
            public void Return(DateTime returnDate, Func<Rental, decimal> lateFeeCalculator) {
                if (IsReturned)
                {
                    Console.WriteLine("Rental already returned.");
                }
                ReturnDate = returnDate;
                IsReturned = true;
                LateFee = lateFeeCalculator(this);
            }
        }
    
    //Iterfejsy
    public interface IEquipmentService {
        Equipment AddEquipment(Equipment equipment);
        IEnumerable<Equipment> GetAll();
        IEnumerable<Equipment> GetAvailable();
        Equipment GetById(int id);
        void SetUnavailable(int id);
        void SetAvailable(int id);
    }
    
    public interface IUserService {
        User AddUser(User user);
        User GetById(int id);
        IEnumerable<User> GetAll();
    }
    
    public interface IRentalService {
        Rental CreateRental(User user, Equipment equipment, DateTime start, DateTime due);
        void ReturnEquipment(int rentalId, DateTime returnDate);
        IEnumerable<Rental> GetActiveRentalsForUser(int userId);
        IEnumerable<Rental> GetOverdueRentals(DateTime now);
    }
    
    public interface ILateFeePolicy { decimal CalculateLateFee(Rental rental); }
    
    //Klasa zarządzająca użytkownikami
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

    //Klasa zarządzająca sprzętem
    public class EquipmentService : IEquipmentService {
        private readonly List<Equipment> _equipment = new();

        //2. Dodawanie nowego sprzętu
        public Equipment AddEquipment(Equipment equipment) { _equipment.Add(equipment); return equipment; }

        //3. Wyświetlanie listy całego sprzętu
        public IEnumerable<Equipment> GetAll() => _equipment;
            
        //4. Wyświetlanie dostępnego sprzętu
        public IEnumerable<Equipment> GetAvailable() => _equipment.Where(e => e.Status == EquipmentStatus.Available);

        public Equipment GetById(int id) {
            var eq = _equipment.SingleOrDefault(e => e.Id == id);
            if (eq == null)
            {
                Console.WriteLine("Equipment with id " + id + " not found.");
            }
            return eq;
        }
        
        //7. Oznaczanie sprzętu jak niedostępny
        public void SetUnavailable(int id) {
            var eq = GetById(id);
            eq.Status = EquipmentStatus.Unavailable;
        }

        public void SetAvailable(int id) {
            var eq = GetById(id);
            if (eq.Status == EquipmentStatus.Unavailable)
            {
                eq.Status = EquipmentStatus.Available;
            }
        }
    }

    //Klasa zarządzająca wypożyczaniem
    public class RentalService : IRentalService {
        private readonly List<Rental> _rentals = new();
        private readonly ILateFeePolicy _lateFeePolicy;
    
        public RentalService(ILateFeePolicy lateFeePolicy) { _lateFeePolicy = lateFeePolicy; }
        //5. Wypożyczenie sprzętu użytkownikowi
        public Rental CreateRental(User user, Equipment equipment, DateTime start, DateTime due) {
            //Jeżeli sprzęt jest niedostępny, nie mona go wypożyczyć
            if (equipment.Status != EquipmentStatus.Available)
            {
                Console.WriteLine("Equipment is not available for rental.");
            }
            var activeRentalsForUser = _rentals.Count(r => r.User.Id == user.Id && !r.IsReturned);
                
            //System powinien blokować wypożyczenie jeżeli użytkownik przekroczy limit
            if (activeRentalsForUser >= user.MaxActiveRentals)
            {
                Console.WriteLine("User " + user.Id + " exceeded max active rentals limit " + user.MaxActiveRentals + " .");
            }
            var rental = new Rental(user, equipment, start, due);
            _rentals.Add(rental);
            equipment.Status = EquipmentStatus.Rented;
            return rental;
        }
        //6. Zwrot srzętu
        public void ReturnEquipment(int rentalId, DateTime returnDate) {
            var rental = _rentals.SingleOrDefault(r => r.Id == rentalId);
            if (rental == null)
            {
                Console.WriteLine("Rental with id " + rentalId + " not found.");
            }
            rental.Return(returnDate, _lateFeePolicy.CalculateLateFee);
            rental.Equipment.Status = EquipmentStatus.Available;
        }
    
        //8. Wyświetlenie aktywnych wypożyczeń
        public IEnumerable<Rental> GetActiveRentalsForUser(int userId) { return _rentals.Where(r => r.User.Id == userId && !r.IsReturned); }
        
        //9. Wyświetlenie listy przeterminowanych
        public IEnumerable<Rental> GetOverdueRentals(DateTime now) { return _rentals.Where(r => r.IsOverdue(now)); } 
    }

    //Klasa zarządzająca naliczaniem kary
    //Opóźniony zwrot skutkuje naliczeniem kary
    public class LateFeePolicy : ILateFeePolicy {
        //Opłata za 1 dzień spóźnienia 
        private const decimal FeePerDay = 5m;
        public decimal CalculateLateFee(Rental rental) {
            if (!rental.ReturnDate.HasValue)
            {
                return 0m;
            }
    
            if (rental.ReturnDate.Value <= rental.DueDate)
            {
                return 0m; 
            }
    
            var daysLate = (rental.ReturnDate.Value.Date - rental.DueDate.Date).Days;
            return daysLate * FeePerDay;
         }
    }
        
    public class Program {
            public static void Main(string[] args) {
                Console.WriteLine("Test");
            }
        }
}