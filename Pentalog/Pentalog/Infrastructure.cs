using System.Linq;
using System.Threading.Tasks;
using Pentalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Pentalog.Infrastructure
{
    public class CarEntity
    {
        public long? Id { get; set; }
        public string Color { get; set; }
        public Model Type { get; set; }
        public long KilometersAmount { get; set; }

        public bool IsDeleted { get; set; }

        public EngineEntity Engine { get; set; }

        public CarEntity(Car car)
        {
            // ... 
        }
    }

    public class EngineEntity
    {
        public long? Id { get; set; }
        public int OilLevel { get; set; }

        public long CarId { get; set; }
        public CarEntity CarEntity { get; set; }

        public EngineEntity(Car car)
        {
            // ... 
        }
    }

    public class CarContext : DbContext
    {
        public DbSet<CarEntity> Cars { get; set; }
        public DbSet<EngineEntity> Engines { get; set; }
    }

    public class CarRepository : ICarRepository
    {

        CarContext carContext;

        public CarRepository(CarContext carContext)
        {
            this.carContext = carContext;
        }

        public void Add(Car car)
        {
            this.carContext.Cars.Add(new CarEntity(car));
        }

        public async Task Remove(Car car)
        {
            var carEntity = await this.carContext.Cars.FindAsync(car.Id.GetValueOrDefault());
            carEntity.IsDeleted = true;
            this.carContext.Cars.Update(carEntity);
        }

        public void Update(Car car)
        {
            this.carContext.Cars.Update(new CarEntity(car));
        }

        public async Task<Car> GetByIdAsync(long id)
        {
            return await this.carContext.Cars
                .Where(v => v.Id == id)
                .Select(c => new Car(c.Color, c.Type, c.KilometersAmount, c.Id))
                .FirstOrDefaultAsync();
        }

        public IQueryable<Car> GetAll()
        {
            return this.carContext.Cars
                .Where(v => v.IsDeleted == false)
                .Select(v => new Car(v.Color, v.Type, v.KilometersAmount, v.Id));
        }

        public async Task SaveAsync(Car entity)
        {
            await this.carContext.SaveChangesAsync();
        }
    }
}