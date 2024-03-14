using KYC360_InternshipAssessment.Models;
using KYC360_InternshipAssessment.Repository;

namespace KYC360_InternshipAssessment.Service
{

    public interface IEntityService
    {
        IEnumerable<Entity> GetAll();
        IEnumerable<Entity> GetAllBySearchQuery(string search);
        IEnumerable<Entity> GetAllEntitiesByFilter(Gender? gender, DateTime startDate, DateTime endDate, List<string> countries);
        IEntity GetById(int id);
        int Create(CreateRequest entity);
        bool Update(int id, UpdateRequest entity);
        bool DeleteById(int id);


    }

    public class EntitiesService : IEntityService
    {
        private IEntityRepository _entityRepository;

        public EntitiesService(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }


        public IEnumerable<Entity> GetAll()
        {
            return _entityRepository.GetAll();
        }

        public IEntity GetById(int id)
        {
            return _entityRepository.GetById(id);
        }

        public bool DeleteById(int id)
        {
            return _entityRepository.DeleteById(id);
        }

        public int Create(CreateRequest entity)
        {
            return _entityRepository.Create(entity);
        }

        public bool Update(int id, UpdateRequest entity)
        {
            return _entityRepository.Update(id, entity);
        }

        public IEnumerable<Entity> GetAllBySearchQuery(string search)
        {
            return _entityRepository.GetAllBySearchQuery(search);
        }

        public IEnumerable<Entity> GetAllEntitiesByFilter(Gender? gender, DateTime startDate, DateTime endDate, List<string> countries)
        {
            return _entityRepository.GetAllEntitiesByFilter(gender, startDate, endDate, countries);
        }
    }
}
