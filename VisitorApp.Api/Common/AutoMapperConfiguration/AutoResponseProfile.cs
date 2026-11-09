using Pluralize.NET;
using System.Reflection;

namespace VisitorApp.API.Common.AutoMapperConfiguration;

public class AutoResponseProfile : Profile
{
    public AutoResponseProfile()
    {
        var appAssembly = Assembly.GetAssembly(typeof(VisitorApp.Application.AssemblyReference));
        var domainAssembly = Assembly.GetAssembly(typeof(VisitorApp.Domain.AssemblyReference));

        var responses = appAssembly!.GetTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Response"));

        foreach (var resp in responses)
        {
            Type? entityType = null;

            if (resp.Name.EndsWith("CommandResponse"))
            {
                var coreName = resp.Name.Replace("CommandResponse", "");

                var actionPart = resp.Namespace?.Split('.').Last() ?? string.Empty;

                var entityName = coreName.StartsWith(actionPart)
                    ? coreName.Substring(actionPart.Length)
                    : coreName;

                var pluralizer = new Pluralizer();
                string singular = pluralizer.Singularize(entityName);

                entityType = domainAssembly!.GetTypes()
                    .FirstOrDefault(t => t.IsClass && t.Name == singular);
            }
            else if (resp.Name.EndsWith("QueryResponse"))
            {
                var coreName = resp.Name.Replace("QueryResponse", "");

                var actionPart = resp.Namespace?.Split('.').Last() ?? string.Empty;

                var entityName = coreName.StartsWith(actionPart)
                    ? coreName.Substring(actionPart.Length)
                    : coreName;

                var pluralizer = new Pluralizer();
                string singular = pluralizer.Singularize(entityName);

                entityType = domainAssembly!.GetTypes()
                    .FirstOrDefault(t => t.IsClass && t.Name == singular);
            }
         
            if (entityType != null)
            {
                CreateMap(entityType, resp);
            }
        }
    }
}