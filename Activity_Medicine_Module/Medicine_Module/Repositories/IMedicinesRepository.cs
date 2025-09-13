using System.Data;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// 药品查询仓储接口
    /// </summary>
    public interface IMedicinesRepository
    {
        /// <summary>
        /// 按关键词分页搜索药品；kw 为空则返回全部。关键词会在名称/规格/剂型上做 LIKE。
        /// </summary>
        Task<IReadOnlyList<MedicineDto>> SearchAsync(string? kw, int page, int pageSize, IDbTransaction? tx = null);

        /// <summary>
        /// 按主键获取单个药品。
        /// </summary>
        Task<MedicineDto?> GetByIdAsync(int medicine_id, IDbTransaction? tx = null);
    }
}
