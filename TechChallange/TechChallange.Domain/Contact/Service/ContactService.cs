using System.Runtime.CompilerServices;
using TechChallange.Domain.Cache;
using TechChallange.Domain.Contact.Entity;
using TechChallange.Domain.Contact.Exception;
using TechChallange.Domain.Contact.Repository;
using TechChallange.Domain.Region.Exception;
using TechChallange.Domain.Region.Service;

namespace TechChallange.Domain.Contact.Service
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly IRegionService _regionService;

        public ContactService(IContactRepository contactRepository, ICacheRepository cacheRepository, IRegionService regionService)
        {
            _contactRepository = contactRepository;
            _cacheRepository = cacheRepository;
            _regionService = regionService;
        }

        public async Task CreateAsync(ContactEntity contactEntity)
        {
            var regionDb = await _regionService.GetByIdAsync(contactEntity.RegionId);

            if (regionDb == null)
                throw new RegionNotFoundException();

            await _contactRepository.Create(contactEntity).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ContactEntity>> GetAllPagedAsync(int pageSize, int page)
        {
            return await _contactRepository.GetAllPagedAsync(c => !c.IsDeleted, pageSize, page, c => c.Name).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ContactEntity>> GetByDddAsync(string ddd)
        {
            return await _cacheRepository.GetAsync(ddd, async () => await _contactRepository.GetByDddAsync(ddd).ConfigureAwait(false));
        }

        public async Task<ContactEntity> GetByIdAsync(Guid id)
        {
            var contactDb = await _cacheRepository.GetAsync(id.ToString(), async () => await _contactRepository.GetByIdAsync(id).ConfigureAwait(false));

            if (contactDb == null)
                throw new ContactNotFoundException();

            return contactDb;
        }

        public async Task<int> GetCountAsync()
        {
            return await _contactRepository.GetCountAsync(c => !c.IsDeleted);
        }

        public async Task RemoveByIdAsync(Guid id)
        {
            var contactDb = await _contactRepository.GetByIdAsync(id).ConfigureAwait(false);

            if (contactDb == null)
                throw new ContactNotFoundException();

            contactDb.MarkAsDeleted();

            await _contactRepository.UpdateAsync(contactDb).ConfigureAwait(false);
        }

        public async Task UpdateAsync(ContactEntity contact)
        {
            var contactDb = await _contactRepository.GetByIdAsync(contact.Id).ConfigureAwait(false);

            if (contactDb == null)
                throw new ContactNotFoundException();

            var regionDb = await _regionService.GetByIdAsync(contact.RegionId);

            if (regionDb == null)
                throw new RegionNotFoundException();

            contactDb.Name = contact.Name;
            contactDb.Phone = contact.Phone;
            contactDb.Email = contact.Email;
            contactDb.RegionId = contact.RegionId;

            await _contactRepository.UpdateAsync(contact).ConfigureAwait(false);
        }


    }
}
