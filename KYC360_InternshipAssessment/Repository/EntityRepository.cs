using Bogus;
using KYC360_InternshipAssessment.Models;

namespace KYC360_InternshipAssessment.Repository
{

    public interface IEntityRepository
    {
        IEnumerable<Entity> GetAll();
        IEnumerable<Entity> GetAllBySearchQuery(string search);
        IEnumerable<Entity> GetAllEntitiesByFilter(Gender? gender, DateTime startDate, DateTime endDate, List<string> countries);
        IEntity GetById(int id);
        bool Update(int id, UpdateRequest entity);
        int Create(CreateRequest entity);
        bool DeleteById(int id);

    }


    public class EntityRepository : IEntityRepository
    {
        int entityIds = 1;

        private List<Entity> _entities;

        public EntityRepository()
        {
            _entities = InitializeEntityData();
        }

        // This method initializes 500 in-memory fake entity data

        private List<Entity> InitializeEntityData()
        {

            Faker<Entity> EntityGenerator = new Faker<Entity>()
                .RuleFor(entity => entity.Gender, bogus => bogus.PickRandom<Gender>().ToString())
                .RuleFor(entity => entity.Id, bogus => entityIds++.ToString())
                .RuleFor(entity => entity.Names, (b, entity) =>
                {
                    Bogus.DataSets.Name.Gender gender = entity.Gender == "Male" ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female;

                    Faker<Name> NameGenerator = new Faker<Name>()
                     .RuleFor(n => n.FirstName, b => b.Name.FirstName(gender))
                     .RuleFor(n => n.Surname, b => b.Name.LastName(gender));

                    return NameGenerator.Generate(1);

                })
                .RuleFor(entity => entity.Addresses, (bogus, entity) =>
                {
                    Faker<Address> AddressGenerator = new Faker<Address>()
                    .RuleFor(a => a.AddressLine, b => b.Address.StreetAddress())
                    .RuleFor(a => a.City, b => b.Address.City())
                    .RuleFor(a => a.Country, b => b.Address.Country());

                    return AddressGenerator.Generate(1);
                })
                .RuleFor(entity => entity.Dates, (bogus, entity) =>
                {
                    Faker<Dates> DateGenerator = new Faker<Dates>()
                    .RuleFor(d => d.Date, b => b.Date.Between(new DateTime(2000, 1, 1), new DateTime(2000, 12, 30)));

                    return DateGenerator.Generate(1);
                })
                .RuleFor(entity => entity.Deceased, bogus => bogus.Random.Bool());


            return EntityGenerator.Generate(500);

        }

        public IEnumerable<Entity> GetAll()
        {

            return _entities;
        }

        public IEntity GetById(int id)
        {
            return _entities.Find(e => e.Id == id.ToString());
        }

        public bool DeleteById(int id)
        {
            var result = _entities.RemoveAll(entity => entity.Id == id.ToString());

            return result == 1;
        }

        public int Create(CreateRequest request)
        {
            var entity = new Entity();

            entity.Id = entityIds++.ToString();
            entity.Addresses = request.Addresses;
            entity.Gender = request.Gender;
            entity.Names = request.Names;
            entity.Deceased = request.Deceased;
            entity.Dates = request.Dates;

            _entities.Add(entity);

            return Convert.ToInt32(entity.Id);


        }

        public bool Update(int id, UpdateRequest request)
        {
            var entitiy = _entities.FirstOrDefault(entity => entity.Id == id.ToString());

            if (entitiy != null)
            {
                entitiy.Addresses = request.Addresses;
                entitiy.Names = request.Names;
                entitiy.Gender = request.Gender;
                entitiy.Deceased = request.Deceased;

                return true;
            }

            return false;

        }

        public IEnumerable<Entity> GetAllBySearchQuery(string search)
        {


            var results = _entities.Where(entity =>
            entity.Addresses != null && entity.Addresses.Any(a => a.Country != null && a.Country.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            a.AddressLine != null && a.AddressLine.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
            entity.Names != null && entity.Names.Any(n => n.FirstName != null && n.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            n.Surname != null && n.Surname.Contains(search, StringComparison.OrdinalIgnoreCase)));

            return results;

        }

        public IEnumerable<Entity> GetAllEntitiesByFilter(Gender? gender, DateTime startDate, DateTime endDate, List<string> countries)
        {

            var results = _entities.Where(e => gender == null || e.Gender == gender.ToString())
                       .Where(e => e.Dates.Any(d => (startDate == DateTime.MinValue || d.Date >= startDate) && (endDate == DateTime.MinValue || d.Date <= endDate)))
                       .Where(e => e.Addresses == null || !countries.Any() || countries.Any(c => e.Addresses.Any(a => a.Country.Equals(c, StringComparison.OrdinalIgnoreCase))));

            return results;
        }
    }
}
