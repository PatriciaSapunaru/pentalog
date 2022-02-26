using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pentalog.Domain
{
    public interface ICarRepository
    {
        void Add(Car car);
        Task Remove(Car car);
        void Update(Car car);
        Task<Car> GetByIdAsync(long id);
        IQueryable<Car> GetAll();
        Task SaveAsync(Car car);
    }

    public class Engine
    {
        public int OilLevel { get; private set; }

        public Engine()
        {
            OilLevel = 100;
        }

        public void PrimeSprocket() { }
        public bool EngageSprocket() { return true; }

        public bool HasEnoughOilLevel
        {
            get
            {
                return OilLevel >= 20;
            }
        }
    }

    public class PoppetValve
    {
    }

    public class Car
    {
        public long? Id { get; private set; }
        public string Color { get; private set; }
        public Model Type { get; private set; }
        public long KmAmount { get; private set; }

        public Engine Engine { get; private set; }
        PoppetValve poppetValve;

        public Car(string color, Model type, long kmAmount, long? id = null)
        {
            Color = color;
            Type = type;
            KmAmount = kmAmount;
            Id = id;

            this.Engine = new Engine();
            this.poppetValve = new PoppetValve();
        }

        public float NumberOfSeats
        {
            get
            {
                if (Type == Model.QashqaiAcenta || Type == Model.QashqaiNConnecta)
                {
                    return 4;
                }

                if (Type == Model.QashqaiTekna)
                {
                    return 5;
                }

                throw new ArgumentOutOfRangeException(nameof(Type),
                    $"Number of seats unimplemented for model { Type }");
            }
        }

        public string GetModelDescription
        {
            get
            {
                return $"This car is a {Enum.GetName(typeof(Model), Type)}";
            }
        }

        public bool HasSunroof
        {
            get
            {
                switch (this.Type)
                {
                    case Model.Peugeot208Phase1:
                    case Model.QashqaiTekna:
                    case Model.QashqaiVisia:
                    case Model.QashqaiAcenta:
                    case Model.Peugeot208Phase2:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool HasEnoughEngineOilLevel() { return Engine.HasEnoughOilLevel; }
        public void PrimeEngine() { Engine.PrimeSprocket(); }
        public bool EngageEngine() { return Engine.EngageSprocket(); }
        public void IncrementKms() { KmAmount++; }

    }

    public enum Model
    {
        QashqaiTekna,
        QashqaiVisia,
        QashqaiNConnecta,
        QashqaiAcenta,
        ScenicPhase1,
        ScenicPhase2,
        Peugeot208Phase1,
        Peugeot208Phase2
    }

    public class CarService
    {
        ICarRepository carRepository;

        public CarService(ICarRepository carRepository)
        {
            this.carRepository = carRepository;
        }

        public async Task<bool> StartCar(long carId)
        {
            var car = await carRepository.GetByIdAsync(carId);

            if (car == null)
            {
                throw new Exception($"Car with id { carId} not found.");
            }

            if (!car.HasEnoughEngineOilLevel())
            {
                throw new Exception("Impossible to start the car, oil level too low");
            }

            car.PrimeEngine();
            return car.EngageEngine();
        }

        public async Task<Car> IncrementKms(long carId)
        {
            var car = await carRepository.GetByIdAsync(carId);

            if (car == null)
            {
                throw new Exception($"Car with id { carId} not found.");
            }

            car.IncrementKms();
            carRepository.Update(car);
            await carRepository.SaveAsync(car);
            return car;
        }
    }
}