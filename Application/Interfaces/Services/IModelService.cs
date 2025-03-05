using Application.DTOs.Dropdown;

namespace Application.Interfaces.Services;

public interface IModelService
{
    List<SelectItemModel> GetAllDatabaseModels();

    Task<(bool, byte[])> ExportDatabaseModel(string tableName);
}