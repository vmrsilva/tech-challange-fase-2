using System.Linq.Expressions;
using TechChallange.Domain.Contact.Entity;
using TechChallange.Domain.Region.Entity;

namespace TechChallange.Domain.Contact.Service
{
    public interface IContactService
    {
        Task CreateAsync(ContactEntity contactEntity);

        Task<ContactEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<ContactEntity>> GetByDddAsync(string ddd);
        Task RemoveByIdAsync(Guid id);
        Task UpdateAsync(ContactEntity contact);
        Task<IEnumerable<ContactEntity>> GetAllPagedAsync(int pageSize, int page);
        Task<int> GetCountAsync();
    }
}
