using System.Reflection;

namespace VisitorApp.API.Common.AutoMapperConfiguration;

public class AutoEntityProfile : Profile
{
    public AutoEntityProfile()
    {
        var appAssembly = Assembly.GetAssembly(typeof(VisitorApp.Application.AssemblyReference));
        var domainAssembly = Assembly.GetAssembly(typeof(VisitorApp.Domain.AssemblyReference));

        var commandRequests = appAssembly.GetTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Request"))
            .ToList();

        foreach (var cmdReq in commandRequests)
        {
            Type? entityType = null;
            if (cmdReq.Name.EndsWith("CommandRequest"))
            {
                // حذف "CommandRequest" از انتها
                var nameWithoutSuffix = cmdReq.Name.Replace("CommandRequest", "");

                // حذف کلمه آخر Namespace که همان عملیات است (Create/Update/Delete)
                // namespace مثال: VisitorApp.Application.Features.Catalog.Products.Create
                var nsParts = cmdReq.Namespace?.Split('.') ?? Array.Empty<string>();
                var actionPart = nsParts.Last(); // آخرین قسمت namespace (Create, Update, Delete)

                // حذف پیشوند action از نام کلاس
                var entityName = nameWithoutSuffix.Replace(actionPart, "");

                // پیدا کردن Entity در Domain
                entityType = domainAssembly.GetTypes()
                    .FirstOrDefault(t => t.IsClass && t.Name == entityName);
            }
            else if (cmdReq.Name.EndsWith("QueryRequest"))
            {
                // حذف "CommandRequest" از انتها
                var nameWithoutSuffix = cmdReq.Name.Replace("QueryRequest", "");

                // حذف کلمه آخر Namespace که همان عملیات است (Create/Update/Delete)
                // namespace مثال: VisitorApp.Application.Features.Catalog.Products.Create
                var nsParts = cmdReq.Namespace?.Split('.') ?? Array.Empty<string>();
                var actionPart = nsParts.Last(); // آخرین قسمت namespace (Create, Update, Delete)

                // حذف پیشوند action از نام کلاس
                var entityName = nameWithoutSuffix.Replace(actionPart, "");

                // پیدا کردن Entity در Domain
                entityType = domainAssembly.GetTypes()
                    .FirstOrDefault(t => t.IsClass && t.Name == entityName);
            }

            if (entityType != null)
            {
                CreateMap(cmdReq, entityType);
            }
        }
    }
}
