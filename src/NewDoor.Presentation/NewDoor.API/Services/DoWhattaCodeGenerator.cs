using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoWhatta.Platform.Builder.Output;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;

namespace DoWhatta.Platform.Builder
{
    public sealed class EntityGenerator
    {
        public async Task<List<string>> CodeBuilderAsync(CodeGenRequest request, EntityMetaModelResponse entityResponse)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Code generation request cannot be null.");

            if (string.IsNullOrWhiteSpace(request.EntityName))
                throw new ArgumentException("Entity name must be provided in the request.", nameof(request.EntityName));


            var entity = entityResponse?.EntityName ?? request.EntityName;
            var entityVar = char.ToLowerInvariant(entity[0]) + entity[1..];

            var baseNs = request.BaseNamespace;
            var basePlatfromNs = request.BasePlatformNamespace;
            var featurePath = request.FeaturePath;
            var appPath = request.ApplicationPath;
            var DtoPath = request.DTOPath;
            var EntityPath = request.EntityPath;


            var files = new Dictionary<string, string>
            {
                [PathFor(featurePath, "Command", $"Add{entity}Command.cs")] = BuildAddCommand(entity, entityVar, baseNs),
                [PathFor(featurePath, "Command", $"Delete{entity}Command.cs")] = BuildDeleteCommand(entity, entityVar, baseNs),
                [PathFor(featurePath, "Command", $"Bulk{entity}Command.cs")] = BuildBulkAddCommand(entity, entityVar, baseNs),
                [PathFor(featurePath, "Handler", $"Add{entity}Handler.cs")] = BuildHandler(entity, entityVar, baseNs),
                [PathFor(featurePath, "Handler", $"FindAll{entity}Handler.cs")] = BuildFindAllHandler(entity, entityVar, baseNs),
                [PathFor(featurePath, "Query", $"FindAll{entity}Query.cs")] = BuildQuery(entity, baseNs),
                [PathFor(featurePath, "Mapper", $"{entity}Mapper.cs")] = BuildMapper(entity, baseNs),
                [Path.Combine(appPath, "Controllers", $"{entity}Controller.cs")] = BuildController(entity, baseNs),
                [Path.Combine(appPath, "Data", "Repositories", "Interface", $"I{entity}Repository.cs")] = BuildRepositoryInterface(entity, entityResponse, baseNs),
                [Path.Combine(appPath, "Data", "Repositories", $"{entity}Repository.cs")] = BuildRepositoryImpl(entity, entityResponse, baseNs),
                [Path.Combine(appPath, "Data", "Configuration", $"{entity}Configuration.cs")] = BuildConfiguration(entityResponse, baseNs),
                [Path.Combine(EntityPath, $"{request.EntityName}.cs")] = BuildEntity(entityResponse, basePlatfromNs),
                [Path.Combine(DtoPath, $"{request.EntityName}s", $"Add{request.EntityName}Request.cs")] = BuildAddDto(entityResponse, basePlatfromNs),
                [Path.Combine(DtoPath, $"{request.EntityName}s", $"BulkAdd{request.EntityName}Request.cs")] = BuildBulkAddDto(entityResponse, basePlatfromNs),
                [Path.Combine(DtoPath, $"{request.EntityName}s", $"{request.EntityName}Response.cs")] = BuildResponseDto(entityResponse, basePlatfromNs),
            };

            var writtenFiles = new List<string>();
            foreach (var (filePath, content) in files)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                await File.WriteAllTextAsync(filePath, content);
                writtenFiles.Add(filePath);
            }

            return writtenFiles;
        }

        private static string PathFor(string root, string folder, string file) =>
            Path.Combine(root, folder, file);

        #region Build Command,Handler ,Query

        private static string BuildAddCommand(string entity, string entityVar, string baseNs) => $$"""
            using DoWhatta.Platform.Data.Mediator.BaseCommands;
            using NewDoor.Platform.DTO.Features.{{entity}}s.Models;

            namespace {{baseNs}}.Features.{{entity}}s.Command
            {
                public record Add{{entity}}Command(Add{{entity}}Request {{entityVar}}Request)
                    : BaseAddCommand<Add{{entity}}Request, {{entity}}Response>({{entityVar}}Request);
            }
        """;


        private static string BuildDeleteCommand(string entity, string entityVar, string baseNs) => $$"""
            using DoWhatta.Platform.Data.Mediator.BaseCommands;
            using NewDoor.Platform.DTO.Features.{{entity}}s.Models;

            namespace {{baseNs}}.Features.{{entity}}s.Command
            {
                public record Delete{{entity}}Command(long Id) : BaseDeleteCommand<long>(Id);
            }
        """;

        private static string BuildBulkAddCommand(string entity, string entityVar, string baseNs) => $$"""
            using DoWhatta.Platform.Data.Mediator.BaseCommands;
            using NewDoor.Platform.DTO.Features.{{entity}}s.Models;

            namespace {{baseNs}}.Features.{{entity}}s.Command
            {
                public record BulkAdd{{entity}}Command(BulkAdd{{entity}}Request {{entityVar}}Request)
                    : BaseAddCommand<BulkAdd{{entity}}Request, int>({{entityVar}}Request);
            }
        """;

        private static string BuildHandler(string entity, string entityVar, string baseNs) => $$"""
            using AutoMapper;
            using DoWhatta.Platform.Data.Mediator.Handlers;
            using NewDoor.Platform.DTO.Features.{{entity}}s.Models;
            using NewDoor.Platform.Entities;
            using NewDoor.API.Repositories.Interface;
            using NewDoor.API.Features.{{entity}}s.Command;

            namespace {{baseNs}}.Features.{{entity}}s.Handler
            {
                public class Add{{entity}}Handler(IMapper mapper, I{{entity}}Repository {{entityVar}}Repository)
                    : BaseAddHandler<Add{{entity}}Command, Add{{entity}}Request, {{entity}}, I{{entity}}Repository, {{entity}}Response>(mapper, {{entityVar}}Repository);
            }
        """;

        private static string BuildFindAllHandler(string entity, string entityVar, string baseNs) => $$"""
            using AutoMapper;
            using DoWhatta.Platform.Data.Mediator.Handlers;
            using NewDoor.Platform.DTO.Features.{{entity}}s.Models;
            using NewDoor.Platform.Entities;
            using NewDoor.API.Repositories.Interface;
            using NewDoor.API.Features.{{entity}}s.Query;

            namespace {{baseNs}}.Features.{{entity}}s.Handler
            {
                public class FindAll{{entity}}Handler(IMapper mapper, I{{entity}}Repository {{entityVar}}Repository)
                    : FindAllHandler<FindAll{{entity}}Query, {{entity}}Response, {{entity}}, I{{entity}}Repository>(mapper, {{entityVar}}Repository);
            }
        """;

        private static string BuildQuery(string entity, string baseNs) => $$"""
            using DoWhatta.Platform.Data.Mediator.Queries;
            using NewDoor.Platform.DTO.Features.{{entity}}s.Models;

            namespace {{baseNs}}.Features.{{entity}}s.Query
            {
                public record FindAll{{entity}}Query : BaseFindAllQuery<{{entity}}Response>;
            }
        """;

        private static string BuildMapper(string entity, string baseNs) => $$"""
            using AutoMapper;
            using NewDoor.Platform.DTO.Features.{{entity}}s.Models;
            using NewDoor.Platform.Entities;

            namespace {{baseNs}}.Features.{{entity}}s.Mapper
            {
                public class {{entity}}Mapper : Profile
                {
                    public {{entity}}Mapper()
                    {
                        CreateMap<Add{{entity}}Request, {{entity}}>();
                        CreateMap<{{entity}}, {{entity}}Response>();
                    }
                }
            }
        """;

        #endregion

        #region Build Controller

        private static string BuildController(string entity, string baseNs) => $$"""
            using MediatR;
            using Microsoft.AspNetCore.Mvc;
            using NewDoor.Platform.DTO.Features.{{entity}}s.Models;
            using NewDoor.API.Features.{{entity}}s.Command;
            using NewDoor.API.Features.{{entity}}s.Query;

            [Route("api/[controller]")]
            [ApiController]
            public class {{entity}}Controller(IMediator mediator) : ControllerBase
            {
                [HttpGet("GetAll")]
                public async Task<List<{{entity}}Response>> GetAll() =>
                    await mediator.Send(new FindAll{{entity}}Query());

                [HttpPost]
                public async Task<{{entity}}Response> Create([FromBody] Add{{entity}}Request request) =>
                    await mediator.Send(new Add{{entity}}Command(request));

                [HttpDelete("{id}")]
                public async Task<long> Delete(long id) =>
                    await mediator.Send(new Delete{{entity}}Command(id));

                [HttpPost("{{entity}}/bulk")]
                public async Task<ActionResult<BulkAdd{{entity}}Request >> Create{{entity}}s([FromBody] BulkAdd{{entity}}Request {{entity}}requests)
                {
                    var result = await mediator.Send(new BulkAdd{{entity}}Command({{entity}}requests));
                    return Ok(result);
                }
            }
        """;
        #endregion

        #region Build Repository & Configuration
        private static string BuildRepositoryInterface(string entity, EntityMetaModelResponse entityResponse, string baseNs) => $$"""
            using DoWhatta.Platform.Data.Base;
            using DoWhatta.Platform.Core.DependencyInjection;
            using NewDoor.Platform.Entities;

            namespace {{baseNs}}.Repositories.Interface;

            public interface I{{entity}}Repository : IBaseRepository<{{entity}}>, IscopedService
            {
                // Add custom methods here if needed
            }
        """;

        private static string BuildRepositoryImpl(string entity, EntityMetaModelResponse entityResponse, string baseNs) => $$"""
            using DoWhatta.Platform.Data.Base;
            using NewDoor.Platform.Entities;
            using {{baseNs}}.Repositories.Interface;

            namespace {{baseNs}}.Data.Repositories;

            public class {{entity}}Repository({{entityResponse.DatabaseMarker}}Context context)
                : BaseRepository<{{entity}}>(context), I{{entity}}Repository
            {
            }
        """;

        private static string BuildConfiguration(EntityMetaModelResponse model, string baseNs)
        {
            var entity = model.EntityName;
            var tableName = model.TableName ?? entity;
            var props = new List<string>();

            props.Add($"builder.ToTable(\"{tableName}\");");
            props.Add($"builder.HasKey(x => x.{model.PrimaryKey});");

            foreach (var prop in model.Properties)
            {
                // Skip collection navigation properties
                if (prop.PropertyType.Contains('`')) continue;

                var line = $"builder.Property(x => x.{prop.PropertyName})";

                if (prop.PropertyType == "String" && prop.MaxLength > 0)
                {
                    line += $".HasMaxLength({prop.MaxLength})";
                }

                line += ";";
                props.Add(line);
            }

            // Handle one-to-many relationships
            foreach (var prop in model.Properties.Where(p => p.PropertyType.Contains('`')))
            {
                if (!string.IsNullOrEmpty(prop.ForeignKeyEntity) && !string.IsNullOrEmpty(prop.NavigationProperty))
                {
                    var navProp = prop.NavigationProperty;
                    var fkName = $"{entity}Id";
                    props.Add($"""
                builder.HasMany(x => x.{navProp})
                    .WithOne(x => x.{entity})
                    .HasForeignKey(x => x.{fkName})
                    .OnDelete(DeleteBehavior.Cascade);
            """);
                }
            }

            return $$"""
                using NewDoor.Platform.Entities;
                using DoWhatta.Platform.Data.Marker
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Metadata.Builders;

                namespace {{baseNs}}.Data.Configuration
                {
                    public class {{entity}}Config : IEntityTypeConfiguration<{{entity}}> , I{{model.DatabaseMarker}}ContextMarker
                    {
                        public void Configure(EntityTypeBuilder<{{entity}}> builder)
                        {
                {{string.Join("\n\n        ", props)}}
                        }
                    }
                }
             """;
        }


        #endregion

        #region Build Entity & DTO
        private static string BuildEntity(EntityMetaModelResponse model, string baseNs)
        {
            var props = model.Properties.Select(p =>
            {
                var type = p.PropertyType;
                if (type.Contains('`')) type = $"ICollection<{p.ForeignKeyEntity}>";
                var annotations = new List<string>();

                if (p.IsRequired) annotations.Add("[Required]");
                if (p.MaxLength > 0) annotations.Add($"[MaxLength({p.MaxLength})]");
                if (!string.IsNullOrEmpty(p.ForeignKeyEntity)) annotations.Add($"public virtual {p.ForeignKeyEntity} {p.NavigationProperty} {{ get; set; }}");

                return $"{string.Join("\n        ", annotations)}\n        public {type} {p.PropertyName} {{ get; set; }}";
            });

            return $$"""
                using System.Collections.Generic;
                using System.ComponentModel.DataAnnotations;
                using {{baseNs}}.Entities;

                namespace {{baseNs}}.Entities
                {
                    public class {{model.EntityName}} : BaseEntity
                    {
                        [Key]
                        public int {{model.PrimaryKey}} { get; set; }

                {{string.Join("\n\n        ", props)}}
                    }
                }
            """;
        }

        private static string BuildAddDto(EntityMetaModelResponse model, string baseNs)
        {
            var props = model.Properties
                .Where(p => p.PropertyType != "ICollection`1")
                .Select(p =>
                {
                    var annotations = new List<string>();
                    if (p.IsRequired) annotations.Add("[Required]");
                    if (p.MaxLength > 0) annotations.Add($"[MaxLength({p.MaxLength})]");
                    return $"{string.Join("\n        ", annotations)}\n        public {p.PropertyType} {p.PropertyName} {{ get; set; }}";
                });

            return $$"""
                    using System.ComponentModel.DataAnnotations;
                    namespace {{baseNs}}.DTO.Features.{{model.EntityName}}s.Models
                    {
                        public class Add{{model.EntityName}}Request  
                        {
                    {{string.Join("\n\n        ", props)}}
                        }
                    }
                """;
        }

        private static string BuildBulkAddDto(EntityMetaModelResponse model, string baseNs)
        {
            var props = model.Properties
                .Where(p => p.PropertyType != "ICollection`1")
                .Select(p =>
                {
                    var annotations = new List<string>();
                    if (p.IsRequired) annotations.Add("[Required]");
                    if (p.MaxLength > 0) annotations.Add($"[MaxLength({p.MaxLength})]");
                    return $"{string.Join("\n        ", annotations)}\n        public {p.PropertyType} {p.PropertyName} {{ get; set; }}";
                });

            return $$"""
                    using System.ComponentModel.DataAnnotations;
                    namespace {{baseNs}}.DTO.Features.{{model.EntityName}}s.Models
                    {
                        public class BulkAdd{{model.EntityName}}Request  
                        {
                           public ICollection<Add{{model.EntityName}}Request> {{char.ToLowerInvariant(model.EntityName[0]) + model.EntityName[1..]}}List { get; set; }
                        }
                    }
                """;
        }

        private static string BuildResponseDto(EntityMetaModelResponse model, string baseNs)
        {
            var props = model.Properties.Select(p =>
            {
                var type = p.PropertyType.Contains('`') ? $"ICollection<{p.ForeignKeyEntity}>" : p.PropertyType;
                return $"public {type} {p.PropertyName} {{ get; set; }}";
            });

            return $$"""
                using {{baseNs}}.DTO.Model;
                namespace DoWhatta.Platform.DTO.Features.{{model.EntityName}}s.Models
                {
                    public class {{model.EntityName}}Response : BaseModel
                    {
                        public int {{model.PrimaryKey}} { get; set; }

                {{string.Join("\n\n        ", props)}}
                    }
                }
            """;
        }
        #endregion
    }
}
