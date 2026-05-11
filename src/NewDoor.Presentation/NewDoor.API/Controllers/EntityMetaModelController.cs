using System.Linq;
using NewDoor.API.Features.MetaModel.Command;
using NewDoor.API.Features.MetaModel.Query;
using DoWhatta.Platform.Builder;
using DoWhatta.Platform.Builder.Output;
using DoWhatta.Platform.DTO.Features.MetaModel;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.ProductFeatures.Models;
using DoWhatta.Platform.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NewDoor.API.Controllers
{
    [Route("api/platform/[controller]")]
    [ApiController]
    [Authorize]
    public class EntityMetaModelController(
        IMediator mediator,
        DoWhattaCodeGenerator dowhattaCodegen,
        CodeOutputOrchestrator outputOrchestrator,
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
        public async Task<IActionResult> GenerateCode(
            int id,
            [FromBody] CodeGenRequest request,
            [FromQuery] CodeGenerationTarget target = CodeGenerationTarget.Platform,
            [FromQuery] CodeOutputMode outputMode = CodeOutputMode.LocalOnly,
            [FromQuery] bool compile = false,
            [FromQuery] DatabaseProvider provider = DatabaseProvider.SqlServer)
        {
           EntityMetaModelResponse entity =
                await mediator.Send(new FindEntityMetaModelByIdQuery(id));

            string apiRootPath = env.ContentRootPath;
            string solutionRootPath = Path.GetDirectoryName(apiRootPath)!;


            await mediator.Send(new GeneratePageFieldsCommand(entity));

            var generation = await dowhattaCodegen.CodeBuilderAsync(
                request,
                entity,
                target,
                string.Empty,
                HttpContext.RequestAborted);

            await outputOrchestrator.WriteAsync(outputMode, solutionRootPath, generation, HttpContext.RequestAborted);

            if (compile)
            {
                await mediator.Send(new CompileModelCommand(CompileMode.Schema,provider,id));
            }

            return Ok(new
            {
                Message = compile
                    ? "Code + compilation triggered"
                    : "Code generated successfully",
                EntityId = id,
                EntityName = request.EntityName,
                Target = target.ToString(),
                OutputMode = outputMode.ToString(),
                Compiled = compile,
                Provider = provider,
                Files = generation.RelativePaths
            });
        }


    }
}
