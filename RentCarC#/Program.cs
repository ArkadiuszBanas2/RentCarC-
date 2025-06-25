class Program
{
    static void Main()
    {
        var fleet = new Fleet();
        var user = new User("Arek Banas", "790737819");

        fleet.AddVehicle(new Car(1, "Toyota", "C-HR", 100, "Benzyna", 5));
        fleet.AddVehicle(new Car(2, "BMW", "F01")); // Uproszczony konstruktor
        fleet.AddVehicle(new HybridCar(6, "Toyota", "RAV4", 120, "Hybrid", 4, 400)); // Hybryda
        fleet.AddVehicle(new Bike(2, "Kross", "Level", 30, true));
        fleet.AddVehicle(new Scooter(3, "Romet", "Ogar200", 50, 250));

        while (true)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Wyświetl wszystkie pojazdy");
            Console.WriteLine("2. Wyświetl dostępne pojazdy");
            Console.WriteLine("3. Wypożycz pojazd");
            Console.WriteLine("4. Zwróć pojazd");
            Console.WriteLine("5. Historia wypożyczeń użytkownika");
            Console.WriteLine("6. Lista aktywnych wypożyczeń");
            Console.WriteLine("7. Raport wypożyczeń wg typu pojazdu");
            Console.WriteLine("0. Zakończ");

            Console.Write("Wybierz opcję: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowAllVehicles(fleet);
                    break;
                case "2":
                    ShowAvailableVehicles(fleet);
                    break;
                case "3":
                    RentVehicle(fleet, user);
                    break;
                case "4":
                    ReturnVehicle(fleet);
                    break;
                case "5":
                    ShowUserHistory(user);
                    break;
                case "6":
                    ShowActiveRentals();
                    break;
                case "7":
                    ShowRentalSummary();
                    break;
                case "0":
                    Console.WriteLine("Do zobaczenia!");
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór.");
                    break;
            }
        }
    }
    static void ShowAllVehicles(Fleet fleet) //Pokazuje wszystkie pojazdy
    {
        Console.WriteLine("\n--- Wszystkie pojazdy ---");
        var hybrid = fleet.GetVehicleById(6) as ElectricVehicle;
        hybrid?.Charge(); // Wypisze komunikat ładowania
        foreach (var car in fleet.GetAllVehicles())
        {
            Console.WriteLine($"{car.Id}: {car.Brand} {car.Model} ({car.VehicleType()}) - {(car.Is_Available ? "Dostępny" : "Wypożyczony")}");
        }
    }
    static void ShowAvailableVehicles(Fleet fleet) // Pokazuje dostepne pojazdy
    {
        Console.WriteLine("\n--- Dostępne pojazdy ---");
        foreach (var car in fleet.GetAvailableVehicles())
        {
            Console.WriteLine($"{car.Id}: {car.Brand} {car.Model} ({car.VehicleType()})");
        }
    }
    static void RentVehicle(Fleet fleet, User user) //Wypozycz pojazd na podstawie ID
    {
        Console.Write("\nPodaj ID pojazdu do wypożyczenia: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var vehicle = fleet.GetVehicleById(id);
            if (vehicle == null)
            {
                Console.WriteLine("Nie znaleziono pojazdu o podanym ID.");
                return;
            }
            if (!vehicle.Is_Available)
            {
                Console.WriteLine("Pojazd jest już wypożyczony.");
                return;
            }
            try
            {
                RentalManager.RentVehicle(user, vehicle);
                Console.WriteLine($"Wypożyczono: {vehicle.Brand} {vehicle.Model}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Nieprawidłowy format ID.");
        }
    }
    static void ReturnVehicle(Fleet fleet) //Zwraca pojazd na podstawie ID
    {
        // Wyświetla liste wyporzyczonych pojazdów
        var rentedVehicles = RentalManager.ActiveRentals
            .Where(r => !r.EndDate.HasValue)
            .Select(r => r.Vehicle)
            .ToList();
        if (!rentedVehicles.Any())
        {
            Console.WriteLine("\nBrak wypożyczonych pojazdów do zwrotu.");
            return;
        }
        Console.WriteLine("\n--- Wypożyczone pojazdy ---");
        foreach (var car in rentedVehicles)
        {
            Console.WriteLine($"{car.Id}: {car.Brand} {car.Model} ({car.VehicleType()})");
        }
        Console.Write("\nPodaj ID pojazdu do zwrotu: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var vehicle = fleet.GetVehicleById(id);
            if (vehicle == null)
            {
                Console.WriteLine("Nie znaleziono pojazdu o podanym ID.");
                return;
            }
            try
            {
                RentalManager.ReturnVehicle(vehicle);
                Console.WriteLine($"Zwrócono: {vehicle.Brand} {vehicle.Model}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Nieprawidłowy format ID.");
        }
    }
    static void ShowUserHistory(User user) //Pokazuje historie wypożyczeń użytkownika
    {
        Console.WriteLine("\n--- Historia wypożyczeń ---");
        if (user.RentalHistory.Count == 0)
        {
            Console.WriteLine("Brak historii wypożyczeń.");
            return;
        }
        foreach (var rental in user.RentalHistory)
        {
            Console.WriteLine(rental);
        }
    }
    static void ShowActiveRentals() //Aktywne wypożyczenia
    {
        Console.WriteLine("\n--- Aktywne wypożyczenia ---");

        if (RentalManager.ActiveRentals.Count == 0)
        {
            Console.WriteLine("Brak aktywnych wypożyczeń.");
            return;
        }

        foreach (var rental in RentalManager.ActiveRentals)
        {
            Console.WriteLine(rental);
        }
    }
    static void ShowRentalSummary()
    {
        Console.WriteLine("\n--- Raport wypożyczonych pojazdów ---");

        var rentals = RentalManager.ActiveRentals;

        int cars = rentals.Count(r => r.Vehicle is Car);
        int bikes = rentals.Count(r => r.Vehicle is Bike);
        int scooters = rentals.Count(r => r.Vehicle is Scooter);

        Console.WriteLine($"Samochody: {cars}");
        Console.WriteLine($"Rowery: {bikes}");
        Console.WriteLine($"Skutery: {scooters}");
    }
}
public abstract class Vehicle // metoda abstrakcyjna - nie mozemy stworzyc bezposrednio obiektu
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public decimal Rental_Rate { get; set; }
    public bool Is_Available { get; set; }

    public Vehicle(int id, string brand, string model, decimal rental_rate) 
    {
        Id = id;
        Brand = brand;
        Model = model;
        Rental_Rate = rental_rate >= 0 ? rental_rate : throw new ArgumentException("Kwota wynajmu nie moze byc ujemna!");
        Is_Available = true;
    }

    public void Rent()
    {
        if (!Is_Available)
        {
            throw new InvalidOperationException("Pojazd jest aktualnie wyporzyczony");
        }
        else
        {
            Is_Available = false;
        }
    }
    public void Rent(DateTime fromDate) //przeciazenie metody
    {
        if (!Is_Available)
            throw new InvalidOperationException("Pojazd jest aktualnie wypożyczony");
        Console.WriteLine($"Pojazd wypożyczony z datą: {fromDate.ToShortDateString()}");
        Is_Available = false;
    }
    public void Return()
    {
        Is_Available = true;
    }
    public abstract string VehicleType(); // polimorfizm
}

public class Car : Vehicle
{
    public string Fuel_Type { get; set; }
    public int Number_Of_Doors { get; set; }
    // Konstruktor glowny
    public Car(int id, string brand, string model, decimal rental_rate, string fuel_type, int number_of_doors) : base(id, brand, model, rental_rate)
    {
        Fuel_Type = fuel_type;
        Number_Of_Doors = number_of_doors;
    }
    // Konstruktor uproszczony
    public Car(int id, string brand, string model)
        : base(id, brand, model, 100) // Domyślna cena
    {
        Fuel_Type = "Nieznane";
        Number_Of_Doors = 5;
    }
    public override string VehicleType()
    {
        return "Samochód";
    }
}
//Dziedziczy po Car, implementuje ElectricVehicle (polaczenie klasy Car jako pojazd spalinowy oraz ElectricVehicle jak pojazd elektryczny)
public class HybridCar : Car, ElectricVehicle
{
    public int BatteryCapacity { get; set; }
    public HybridCar(int id, string brand, string model, decimal rentalRate, string fuelType, int numberOfDoors, int batteryCapacity)
: base(id, brand, model, rentalRate, fuelType, numberOfDoors)
    {
        BatteryCapacity = batteryCapacity;
    }
    public void Charge()
    {
        Console.WriteLine($"Samochód jest w trakcie ładowania: {Brand} {Model} ({BatteryCapacity} Wh)");
    }
    public override string ToString()
    {
        return "Samochód Hybrydowy";
    }
}
public class Bike : Vehicle
{
    public bool Has_Gears { get; set; }
    // Konstruktor glowny
    public Bike(int id, string brand, string model, decimal rental_rate, bool has_gears) : base(id, brand, model, rental_rate)
    {
        Has_Gears = has_gears;
    }
    // Konstruktor uproszczony
    public Bike(int id, string brand, string model) : base(id, brand, model, 50)
    {
        Has_Gears = true;
    }
    public override string VehicleType()
    {
        return "Rower";
    }
}
public class Scooter : Vehicle, ElectricVehicle
{
    public int Battery_Capacity { get; set; }
    // Konstruktor glowny
    public Scooter(int id, string brand, string model, decimal rental_rate, int battery_capacity) : base(id, brand, model, rental_rate)
    {
        Battery_Capacity = battery_capacity;
    }
    // Konstruktor uproszczony
    public Scooter(int id, string brand, string model) : base(id, brand, model, 70)
    {
        Battery_Capacity = 150;
    }
    public override string VehicleType() 
    {
        return "Skuter";
    }
    public void Charge()
    {
        Console.WriteLine("Skuter jest w trakcie ladowania.");
    }
}
public interface ElectricVehicle
{
    void Charge();
}
public class User
{
    private string name;
    private string phoneNumber;
    private List<Rental> rental_History = new List<Rental>();
    public User(string name, string phoneNumber)
    {
        this.Name = name;
        this.phoneNumber = phoneNumber;
    }
    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Imie nie moze byc puste");
            }
            name = value;
        }
    }
    public string PhoneNumber
    {
        get => phoneNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 7 || !value.All(char.IsDigit))
            {
                throw new ArgumentException("Podano niepoprany numer telefonu");
            }
            phoneNumber = value;
        }
    }
    public IReadOnlyList<Rental> RentalHistory => rental_History.AsReadOnly();
    public void AddRental(Rental rental)
    {
        rental_History.Add(rental);
    }
}
public class Rental
{
    public User User { get; set; }
    public Vehicle Vehicle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; private set; }

    public Rental(User user, Vehicle vehicle, DateTime startDate)
    {
        User = user;
        Vehicle = vehicle;
        StartDate = startDate;
        EndDate = null;
    }

    public void EndRental(DateTime endDate)
    {
        if (endDate < StartDate)
        {
            throw new ArgumentException("Data zakonczenia wyporzyczenia pojazdu nie moge byc wczesniejsza jak data rozpoczenia");
        }
        EndDate = endDate;
    }
    public override string ToString()
    {
        return $"{Vehicle.VehicleType()} {Vehicle.Brand} {Vehicle.Model} wypożyczony przez: {User.Name} od {StartDate.ToShortDateString()} do {(EndDate.HasValue ? EndDate.Value.ToShortDateString() : "teraz")}";
    }
}
public static class RentalManager
{
    private static List<Rental> activeRentals = new List<Rental>();
    public static IReadOnlyList<Rental> ActiveRentals => activeRentals.AsReadOnly();
    public static void RentVehicle(User user, Vehicle vehicle)
    {
        RentVehicle(user, vehicle, DateTime.Now); // wywołuje drugą wersję
    }
    public static void RentVehicle(User user, Vehicle vehicle, DateTime? startDate = null)
    {
        if (!vehicle.Is_Available)
            throw new InvalidOperationException("Vehicle is not available for rent.");

        vehicle.Rent();
        Rental rental = new Rental(user, vehicle, startDate ?? DateTime.Now);
        activeRentals.Add(rental);
        user.AddRental(rental);
    }
    public static void ReturnVehicle(Vehicle vehicle, DateTime? returnDate = null)
    {
        Rental rental = activeRentals.FirstOrDefault(r => r.Vehicle == vehicle && !r.EndDate.HasValue);

        if (rental == null)
            throw new InvalidOperationException("Ten pojazd nie jest aktualnie wyporzyczony.");

        rental.EndRental(returnDate ?? DateTime.Now);
        vehicle.Return();
        activeRentals.Remove(rental);
    }
}
public class Fleet
{
    private List<Vehicle> vehicles = new List<Vehicle>();

    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        vehicles.Add(vehicle);
    }
    public bool RemoveVehicleById(int id)
    {
        var vehicle = vehicles.FirstOrDefault(v => v.Id == id);
        if (vehicle != null)
        {
            vehicles.Remove(vehicle);
            return true;
        }
        return false;
    }
    public Vehicle? GetVehicleById(int id)
    {
        return vehicles.FirstOrDefault(v => v.Id == id);
    }
    public List<Vehicle> GetAvailableVehicles()
    {
        return vehicles.Where(v => v.Is_Available).ToList();
    }
    public List<Vehicle> GetAllVehicles()
    {
        return new List<Vehicle>(vehicles);
    }
}