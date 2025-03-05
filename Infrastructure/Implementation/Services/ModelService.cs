using Application.DTOs.Dropdown;
using Application.Interfaces.Services;
using ClosedXML.Excel;
using Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementation.Services;

public class ModelService : IModelService
{
    private readonly ApplicationDbContext _dbContext;

    public ModelService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<SelectItemModel> GetAllDatabaseModels()
    {
        var models = _dbContext.Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Distinct()
            .ToList();

        var result = models.Select(x => new SelectItemModel()
        {
            Id = x ?? "",
            Value = x ?? ""
        }).ToList();
        
        return result;
    }

    public async Task<(bool, byte[])> ExportDatabaseModel(string tableName)
    {
        if (tableName == "AggregatedCounter")
        {
            var query = _dbContext.Set<AggregatedCounter>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "Counter")
        {
            var query = _dbContext.Set<Counter>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "Hash")
        {
            var query = _dbContext.Set<Hash>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "Job")
        {
            var query = _dbContext.Set<Job>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "JobParameter")
        {
            var query = _dbContext.Set<JobParameter>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "JobQueue")
        {
            var query = _dbContext.Set<JobQueue>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "List")
        {
            var query = _dbContext.Set<List>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "Schema")
        {
            var query = _dbContext.Set<Schema>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "Server")
        {
            var query = _dbContext.Set<Server>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "Set")
        {
            var query = _dbContext.Set<Set>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "State")
        {
            var query = _dbContext.Set<State>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        
        if (tableName == "tblChatBotMessages")
        {
            var query = _dbContext.Set<tblChatBotMessage>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < headers.Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperty(headers[j])?.GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblCommons")
        {
            var query = _dbContext.Set<tblCommon>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblCommonsArchive")
        {
            var query = _dbContext.Set<tblCommonsArchive>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblContents")
        {
            var query = _dbContext.Set<tblContent>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblContentArchive")
        {
            var query = _dbContext.Set<tblContentArchive>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblEbooks")
        {
            var query = _dbContext.Set<tblEbook>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblEbookArchive")
        {
            var query = _dbContext.Set<tblEbookArchive>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblEnrollmentStatus")
        {
            var query = _dbContext.Set<tblEnrollmentStatus>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblExceptionLogs")
        {
            var query = _dbContext.Set<tblExceptionLog>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblFAQs")
        {
            var query = _dbContext.Set<tblFAQ>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblNewsAndAlerts")
        {
            var query = _dbContext.Set<tblNewsAndAlert>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblNotifications")
        {
            var query = _dbContext.Set<tblNotification>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblPCPDates")
        {
            var query = _dbContext.Set<tblPCPDate>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblQuestions")
        {
            var query = _dbContext.Set<tblQuestion>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblQuestionPaperSheets")
        {
            var query = _dbContext.Set<tblQuestionPaperSheet>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblQuestionsArchive")
        {
            var query = _dbContext.Set<tblQuestionsArchive>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblStudentLoginDetails")
        {
            var query = _dbContext.Set<tblStudentLoginDetail>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblStudentLoginHistory")
        {
            var query = _dbContext.Set<tblStudentLoginHistory>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblStudentResponses")
        {
            var query = _dbContext.Set<tblStudentResponse>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblStudentScore")
        {
            var query = _dbContext.Set<tblStudentScore>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblStudentVideoTracking")
        {
            var query = _dbContext.Set<tblStudentVideoTracking>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblSubjects")
        {
            var query = _dbContext.Set<tblSubject>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        if (tableName == "tblUsers")
        {
            var query = _dbContext.Set<tblUser>();

            var data = await query.ToListAsync();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(tableName);

            var headers = data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToArray();
            
            for (var i = 0; i < headers!.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range("A1:" + (char)('A' + headers.Length - 1) + "1");
            
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                
                for (var j = 0; j < item.GetType().GetProperties().Length; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = item.GetType().GetProperties()[j].GetValue(item)?.ToString();
                }
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
            
            return (true, stream.ToArray());
        }
        
        return (false, []);
    }
}