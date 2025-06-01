using MyApi.Dtos;

namespace MyApi.Repositories
{
    public interface ISupplierInfoRepository
    {
        Task<IEnumerable<SupplierInfoDto>> GetSupplierInfoDtoAsync(string supplierName);
    }
}
