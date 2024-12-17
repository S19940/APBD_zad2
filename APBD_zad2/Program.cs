using System;
using System.Collections.Generic;

public interface IHazardNotifier
{
    void SendHazardNotification();
}

public class OverfillException : Exception
{
    public OverfillException(string message) : base(message) { }
}

public class Container
{
    private static int serialNumberCounter = 1;
    public string SerialNumber { get; private set; }
    public double LoadWeight { get; private set; } // kg
    public double Height { get; set; } // cm
    public double Depth { get; set; } // cm
    public double OwnWeight { get; set; } // kg
    public double MaxLoad { get; set; } // kg

    public Container(string type)
    {
        SerialNumber = $"KON-{type}-{serialNumberCounter++}";
    }

    public void Load(double weight)
    {
        if (weight + LoadWeight > MaxLoad)
            throw new OverfillException("Overload! The load exceeds the container capacity.");
        LoadWeight += weight;
    }

    public void Unload()
    {
        LoadWeight = 0;
    }
}

public class LiquidContainer : Container, IHazardNotifier
{
    public string ProductType { get; set; }
    public double Pressure { get; set; }

    public LiquidContainer() : base("L") { }

    public void SendHazardNotification()
    {
        Console.WriteLine($"Hazard: Liquid container {SerialNumber} has a dangerous situation.");
    }

    public void Load(double weight)
    {
        double allowedPercentage = ProductType == "fuel" ? 0.5 : 0.9; 
        if (weight + LoadWeight > MaxLoad * allowedPercentage)
            throw new Exception("Unsafe loading attempt.");
        base.Load(weight);
    }
}

public class GasContainer : Container, IHazardNotifier
{
    public double Pressure { get; set; }

    public GasContainer() : base("G") { }

    public void SendHazardNotification()
    {
        Console.WriteLine($"Hazard: Gas container {SerialNumber} has a dangerous situation.");
    }

    public new void Unload()
    {
        LoadWeight = LoadWeight * 0.05; 
    }
}

public class RefrigeratedContainer : Container
{
    public string ProductType { get; set; }
    public double Temperature { get; set; } 

    public RefrigeratedContainer() : base("C") { }

    public new void Load(double weight)
    {
        if (Temperature < GetRequiredTemperature(ProductType))
            throw new Exception($"Temperature too low for {ProductType}!");
        base.Load(weight);
    }

    private double GetRequiredTemperature(string productType)
    {
        switch (productType)
        {
            case "bananas": return 10.0;
            case "milk": return 4.0;
            default: return 0.0; 
        }
    }
}

public class Ship
{
    public string Name { get; set; }
    public double MaxSpeed { get; set; } 
    public int MaxContainers { get; set; }
    public double MaxWeight { get; set; } 
    public List<Container> Containers { get; set; } = new List<Container>();

    public Ship(string name, double maxSpeed, int maxContainers, double maxWeight)
    {
        Name = name;
        MaxSpeed = maxSpeed;
        MaxContainers = maxContainers;
        MaxWeight = maxWeight;
    }

    public void LoadContainer(Container container)
    {
        if (Containers.Count >= MaxContainers || Containers.Sum(c => c.LoadWeight) + container.LoadWeight > MaxWeight)
            throw new Exception("Cannot load container. Ship capacity exceeded.");
        Containers.Add(container);
    }

    public void UnloadContainer(Container container)
    {
        Containers.Remove(container);
    }

    public void PrintShipInfo()
    {
        Console.WriteLine($"Ship {Name} (Speed: {MaxSpeed} knots, Max Containers: {MaxContainers}, Max Weight: {MaxWeight} tons)");
        foreach (var container in Containers)
        {
            Console.WriteLine($"- Container {container.SerialNumber} ({container.LoadWeight} kg loaded)");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        
        Ship ship = new Ship("Ship 1", 25, 100, 40000); 

        
        Container container1 = new LiquidContainer { MaxLoad = 1000, ProductType = "milk" };
        Container container2 = new GasContainer { MaxLoad = 2000 };
        Container container3 = new RefrigeratedContainer { MaxLoad = 1500, ProductType = "bananas", Temperature = 5 };

        try
        {
            
            container1.Load(500); 
            ship.LoadContainer(container1);
            container2.Load(1500); 
            ship.LoadContainer(container2);
            container3.Load(1000); 
            ship.LoadContainer(container3);
            ship.PrintShipInfo();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ship.UnloadContainer(container1);
        Console.WriteLine("\nAfter unloading container 1:");
        ship.PrintShipInfo();
    }
}