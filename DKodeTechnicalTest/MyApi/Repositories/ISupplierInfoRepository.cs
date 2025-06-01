using MyApi.Dtos;

namespace MyApi.Repositories
{
    /// <summary>
    /// Interface to define contract for a repo providing supplier info
    /// </summary>
    public interface ISupplierInfoRepository
    {
        /// <summary>
        /// Retrievs supplier info asynchronously for the specified supplier name
        /// </summary>
        /// <param name="supplierName">Supplier name for which to retrieve info</param>
        /// <returns>Async task which results contain supplier info dto object with supplier detais, product categories, stock info, pricing</returns>
        Task<IEnumerable<SupplierInfoDto>> GetSupplierInfoDtoAsync(string supplierName);
    }
}
