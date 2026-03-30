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
        public override string ToString() { return $"[{Id}] {Name} (Status: {Status})"; }
    }
    //Klasa dla Laptopa
    public class Laptop : Equipment {
        public Laptop(string name, string cpu, int ramGb) : base(name) { Cpu = cpu; RamGb = ramGb; }
        
        private string Cpu { get; }
        private int RamGb { get; }
        public override string ToString() { return base.ToString() + $" | Laptop: CPU={Cpu}, RAM={RamGb}GB"; }
    }
    //Klasa dla Projektora
    public class Projector : Equipment {
        public Projector(string name, int lumens, string resolution) : base(name) { Lumens = lumens; Resolution = resolution; }
        
        private int Lumens { get; }
        private string Resolution { get; }
        public override string ToString() { return base.ToString() + $" | Projector: {Lumens} lm, {Resolution}"; }
    }
    //Klasa dla Kamery
    public class Camera : Equipment {
        public Camera(string name, string sensorType, bool hasStabilization) : base(name) { SensorType = sensorType; HasStabilization = hasStabilization; }
        
        private string SensorType { get; }
        private bool HasStabilization { get; }
        public override string ToString() { return base.ToString() + $" | Camera: Sensor={SensorType}, Stabilization={HasStabilization}"; }
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
        public override string ToString() { return $"[{Id}] {FirstName} {LastName} ({UserType}, max rentals: {MaxActiveRentals})"; }
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
    public class Rental
    {
        private static int _idCounter = 1;

        public Rental(User user, Equipment equipment, DateTime startDate, DateTime dueDate)
        {
            Id = _idCounter++;
            User = user;
            Equipment = equipment;
            StartDate = startDate;
            DueDate = dueDate;
            IsReturned = false;
            LateFee = 0m;
        }

        public int Id { get; }
        public User User { get; }
        public Equipment Equipment { get; }
        private DateTime StartDate { get; }
        public DateTime DueDate { get; }
        public DateTime? ReturnDate { get; private set; }
        public bool IsReturned { get; private set; }
        public decimal LateFee { get; private set; }
        public bool IsOverdue(DateTime now) => !IsReturned && now > DueDate;

        public void Return(DateTime returnDate, Func<Rental, decimal> lateFeeCalculator)
        {
            if (IsReturned)
            {
                Console.WriteLine("Rental already returned.");
            }

            ReturnDate = returnDate;
            IsReturned = true;
            LateFee = lateFeeCalculator(this);
        }

        public override string ToString()
        {
            var status = IsReturned ? $"Returned at {ReturnDate}, Late fee: {LateFee:C}" : "Active";
            return $"Rental [{Id}] User={User.Id}, Equipment={Equipment.Id}, " +
                   $"Start={StartDate}, Due={DueDate}, Status={status}";
        }
    }
    
    //Wyjątek
     public class MaxRentalException : Exception { public MaxRentalException(string message) : base(message) {} }

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
        IEnumerable<Rental> GetAllRentals();
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
                throw new MaxRentalException("Equipment is not available for rental.");
            }
            var activeRentalsForUser = _rentals.Count(r => r.User.Id == user.Id && !r.IsReturned);
            
            //System powinien blokować wypożyczenie jeżeli użytkownik przekroczy limit
            if (activeRentalsForUser >= user.MaxActiveRentals)
            {
                throw new MaxRentalException($"User {user.Id} exceeded max active rentals limit ({user.MaxActiveRentals}).");
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
        
        public IEnumerable<Rental> GetAllRentals() => _rentals;
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
        
    //Klasa zarządzania raportem
    public class ReportService {
        private readonly IEquipmentService _equipmentService;
        private readonly IRentalService _rentalService;
        private readonly IUserService _userService;
        public ReportService(IEquipmentService equipmentService, IRentalService rentalService, IUserService userService) { _equipmentService = equipmentService; _rentalService = rentalService; _userService = userService; }
            
        //10. Wgenerowanie krótkiego raportu
        public void PrintSummaryReport(DateTime now) {
            Console.WriteLine("\n=== START OF REPORT ===\n");
    
            var allEquipment = _equipmentService.GetAll().ToList();
            var allRentals = _rentalService.GetAllRentals().ToList();
            var overdue = _rentalService.GetOverdueRentals(now).ToList();
            
            Console.WriteLine($"Total equipment: {allEquipment.Count}");
            Console.WriteLine($"Available: {allEquipment.Count(e => e.Status == EquipmentStatus.Available)}");
            Console.WriteLine($"Rented: {allEquipment.Count(e => e.Status == EquipmentStatus.Rented)}");
            Console.WriteLine($"Unavailable: {allEquipment.Count(e => e.Status == EquipmentStatus.Unavailable)}");
    
            Console.WriteLine($"\nTotal rentals: {allRentals.Count}");
            Console.WriteLine($"Active rentals: {allRentals.Count(r => !r.IsReturned)}");
            Console.WriteLine($"Overdue rentals: {overdue.Count}");
    
            var totalLateFees = allRentals.Sum(r => r.LateFee);
            Console.WriteLine($"\nTotal late fees collected: {totalLateFees:C}");
    
            Console.WriteLine("\nOverdue rentals details:");
            foreach (var r in overdue) { Console.WriteLine(r); }
    
            Console.WriteLine("\n=== END OF REPORT ===\n");
            }
        }
    
     //Główny program
     public class Program { 
         public static void Main(string[] args) {
            IEquipmentService equipmentService = new EquipmentService();
            IUserService userService = new UserService();
            ILateFeePolicy lateFeePolicy = new LateFeePolicy();
            IRentalService rentalService = new RentalService(lateFeePolicy);
            var reportService = new ReportService(equipmentService, rentalService, userService);
    
            //11. Dodanie kilku egzemplarzy sprzętu.
            var laptop1 = new Laptop("Dell XD13", "i7", 16);
            var laptop2 = new Laptop("Lenovo ThinkPad", "i5", 8);
            var projector1 = new Projector("Epson J21", 3000, "1920x1080");
            var camera1 = new Camera("Sony P37", "Full Frame", true);
    
            equipmentService.AddEquipment(laptop1);
            equipmentService.AddEquipment(laptop2);
            equipmentService.AddEquipment(projector1);
            equipmentService.AddEquipment(camera1);
    
             //12. Dodanie kilku użytkowników.
             var student1 = new Student("Jan", "Kowalski");
             var student2 = new Student("Anna", "Nowak");
             var employee1 = new Employee("Jacek", "Placek");
    
             userService.AddUser(student1);
             userService.AddUser(student2);
             userService.AddUser(employee1);
                
             Console.WriteLine("=== All equipment ===");
             foreach (var eq in equipmentService.GetAll()) { Console.WriteLine(eq); }
    
             Console.WriteLine("\n=== All users ===");
             foreach (var u in userService.GetAll()) { Console.WriteLine(u); }
                
             //13. Poprawne wypożyczenie sprzętu
             Console.WriteLine("\n[Rental]:");
             var start1 = new DateTime(2026, 1, 1, 10, 0, 0);
             var due1 = new DateTime(2026, 1, 5, 10, 0, 0);
             var rental1 = rentalService.CreateRental(student1, laptop1, start1, due1);
             Console.WriteLine(rental1);
                
             //14. Próba wykonania wypożyczenia sprzętu niedostępnego albo przekroczenia limitu.
             Console.WriteLine("\n[Incorrect operation]:");
             equipmentService.SetUnavailable(camera1.Id);
             try
             {
                 rentalService.CreateRental(student1, camera1, start1, due1);
             }
             catch (Exception e)
             {
                 Console.WriteLine(e.Message);
             }
               
             //15. Zwrot sprzętu w terminie
             Console.WriteLine("\n[On-time return]:");
             var onTimeReturnDate = new DateTime(2026, 1, 5, 9, 0, 0);
             rentalService.ReturnEquipment(rental1.Id, onTimeReturnDate);
             Console.WriteLine(rental1);
             
             //16. Zwrot sprzętu po terminie
             Console.WriteLine("\n[Late return with fee]:");
             var start2 = new DateTime(2026, 1, 1, 10, 0, 0);
             var due2 = new DateTime(2026, 1, 3, 10, 0, 0);
             var rental2 = rentalService.CreateRental(student2, laptop2, start2, due2);
             Console.WriteLine(rental2);
             
             var lateReturnDate = new DateTime(2026, 1, 6, 10, 0, 0);
             rentalService.ReturnEquipment(rental2.Id, lateReturnDate);
             Console.WriteLine(rental2);
             
             //17. Wyświetlenie raportu końcowego o stanie systemu
             var now = new DateTime(2026, 1, 7, 10, 0, 0);
             reportService.PrintSummaryReport(now);
             
             Console.WriteLine("Press any key to exit...");
             Console.ReadKey();
         }
     }
}