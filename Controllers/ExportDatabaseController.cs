using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using ContainerManagement.Data;

namespace ContainerManagement.Controllers
{
    public partial class ExportDatabaseController : ExportController
    {
        private readonly DatabaseContext context;
        private readonly DatabaseService service;

        public ExportDatabaseController(DatabaseContext context, DatabaseService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/Database/nodes/csv")]
        [HttpGet("/export/Database/nodes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportNodesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetNodes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/Database/nodes/excel")]
        [HttpGet("/export/Database/nodes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportNodesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetNodes(), Request.Query, false), fileName);
        }
    }
}
