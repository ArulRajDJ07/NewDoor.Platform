using DoWhatta.Platform.Builder;
using DoWhatta.Platform.Builder.Output;
using DoWhatta.Platform.DTO.Features.MetaModel;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.ProductFeatures.Models;
using DoWhatta.Platform.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using NewDoor.API.Features.MetaModel.Command;
using NewDoor.API.Features.MetaModel.Query;
using System.Linq;

namespace NewDoor.API.Controllers
{
    [Route("api/platform/[controller]")]
    [ApiController]
    [Authorize]
    public class EntityMetaModelController(
        IMediator mediator,
        EntityGenerator dowhattaCodegen,
        IWebHostEnvironment env) : ControllerBase
    {
        [HttpGet("GetAllEntityMetaModel")]
        public async Task<List<EntityMetaModelResponse>> GetAllEntityMetaModel()
        {
            var result = await mediator.Send(new FindAllEntityMetaModelQuery());
            return result;
        }


        [HttpPost("EntityMetaModel")]
        public async Task<EntityMetaModelResponse> CreateEntityMetaModel([FromBody] AddEntityMetaModelRequest model)
        {
            return await mediator.Send(new AddEntityMetaModelCommand(model));
        }

        [HttpGet("{id}")]
        public async Task<EntityMetaModelResponse> GetEntityMetaModel(int id)
        {
            return await mediator.Send(new FindEntityMetaModelByIdQuery(id));
        }

        [HttpPost("PropertyMetaModel")]
        public async Task<EntityPropertyMetaModelResponse> CreateEntityPropertyMetaModel([FromBody] AddEntityPropertyMetaModelRequest model)
        {
            return await mediator.Send(new AddEntityPropertyMetaModelCommand(model));
        }

        [HttpPost("PropertyMetaModel/bulk")]
        public async Task<ActionResult<List<EntityPropertyMetaModelResponse>>> CreateBulk([FromBody] BulkAddEntityPropertyMetaModelRequest request)
        {
            var result = await mediator.Send(new BulkAddEntityPropertyMetaModelCommand(request));
            return Ok(result);
        }

        [HttpPost("{EntityMetaModelId}/PublishFieldMeta")]
        public async Task<IActionResult> PublishFieldMeta(int EntityMetaModelId,[FromQuery] bool compile = false,[FromQuery] DatabaseProvider provider = DatabaseProvider.SqlServer)
        {
          
            EntityMetaModelResponse entity =await mediator.Send(new FindEntityMetaModelByIdQuery(EntityMetaModelId));

            await mediator.Send(new GeneratePageFieldsCommand(entity));

            if (compile)
            {
                await mediator.Send(new CompileModelCommand(CompileMode.Schema,provider,EntityMetaModelId));
            }

            // 4. Return response
            return Ok(new
            {
                Message = compile
                    ? "Page MetaModel generated + compilation triggered"
                    : "Page MetaModel generated",
                Status = true,
                Compiled = compile,
                Provider = provider.ToString()
            });
        }



        [HttpPost("{id}/GenerateCode")]
        public async Task<IActionResult> GenerateCode(int id, [FromBody] CodeGenRequest request, [FromQuery] bool compile = false, [FromQuery] DatabaseProvider provider = DatabaseProvider.SqlServer)
        {
            EntityMetaModelResponse entity =
                 await mediator.Send(new FindEntityMetaModelByIdQuery(id));

            string apiRootPath = env.ContentRootPath;
            // Navigate up to src root folder (2 levels from API project)
            string srcRootPath = Path.GetDirectoryName(Path.GetDirectoryName(apiRootPath))!;

            string featurePath = Path.Combine(
                apiRootPath,
                "Features",
                request.EntityName + "s"
            );

            request.ApplicationPath = apiRootPath;
            request.FeaturePath = featurePath;
            // Point to PlatformShared location (code generator adds "Features" folder)
            request.DTOPath = Path.Combine(
                srcRootPath,
                "NewDoor.PlatformShared",
                "NewDoor.Platform.DTO",
                "Features"
            );
            request.EntityPath = Path.Combine(
                srcRootPath,
                "NewDoor.PlatformShared",
                "NewDoor.Platform.Entities"
            );

            await mediator.Send(new GeneratePageFieldsCommand(entity));
            var generatedFiles =
                await dowhattaCodegen.CodeBuilderAsync(request, entity);

            if (compile)
            {
                await mediator.Send(new CompileModelCommand(CompileMode.Schema, provider, id));
            }

            return Ok(new
            {
                Message = compile
                    ? "Code + compilation triggered"
                    : "Code generated successfully",
                EntityId = id,
                EntityName = request.EntityName,
                Compiled = compile,
                Provider = provider,
                Files = generatedFiles
            });
        }


    }
}
