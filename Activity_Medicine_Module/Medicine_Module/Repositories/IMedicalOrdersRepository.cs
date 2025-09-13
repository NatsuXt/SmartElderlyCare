using System.Data;

namespace ElderCare.Api.Modules.Medical
{
    public interface IMedicalOrdersRepository
    {
        Task<int> CreateAsync(MedicalOrder o, IDbTransaction tx);
        Task UpdateAsync(int order_id, UpdateMedicalOrderDto dto, IDbTransaction tx);
        Task<MedicalOrder?> GetAsync(int order_id, IDbTransaction? tx = null);

        /// <summary>分页查询；elderly_id 为 null 时表示查询全部</summary>
        Task<IReadOnlyList<MedicalOrder>> QueryAsync(
            int? elderly_id, DateTime? from, DateTime? to, int page, int pageSize);

        /// <summary>按主键删除</summary>
        Task<int> DeleteAsync(int order_id, IDbTransaction tx);
    }
}
