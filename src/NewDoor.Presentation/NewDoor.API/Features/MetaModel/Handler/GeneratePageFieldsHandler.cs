
using System.Text.RegularExpressions;
using MediatR;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.PageField.Models;

namespace NewDoor.API.Features.MetaModel.Handler
{
    public class GeneratePageFieldsHandler
        : IRequestHandler<GeneratePageFieldsCommand>
    {
        private readonly IMediator _mediator;

        public GeneratePageFieldsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(
            GeneratePageFieldsCommand request,
            CancellationToken cancellationToken)
        {
            var model = request.Entity;
            if (model.SectionId <= 0)
                return;

            var bulkRequest = new BulkAddPageFieldRequest
            {
                PageFieldList = BuildFromProperties(model)
            };

           
        }

        private static List<AddPageFieldRequest> BuildFromProperties(
            EntityMetaModelResponse model)
        {
            var list = new List<AddPageFieldRequest>();
            int order = 0;

            list.Add(new AddPageFieldRequest
            {
                PageSectionId = model.SectionId,
                FieldName = $"{model.EntityName} Id",
                EntityFieldName = model.PrimaryKey,
                FieldType = (int)PageFieldType.TextBox,
                IsReadOnly = false,
                IsRequired = true,
                IsVisible = true,
                DisplayOrder = order++,
                IsActive = true
            });

            foreach (var prop in model.Properties)
            {
                if (prop.PropertyName == model.PrimaryKey)
                    continue;

                if (prop.PropertyType.Contains('`'))
                    continue;

                list.Add(new AddPageFieldRequest
                {
                    PageSectionId = model.SectionId,
                    FieldName = Regex.Replace(prop.PropertyName, @"(\B[A-Z])", " $1"),
                    EntityFieldName = prop.PropertyName,
                    FieldType = (int)Resolve(prop),
                    Placeholder = $"Enter {prop.PropertyName}",
                    Tooltip = $"Enter {prop.PropertyName}",
                    IsRequired = prop.IsRequired,
                    IsVisible = true,
                    IsReadOnly = false,
                    DisplayOrder = order++,
                    IsActive = true
                });
            }

            return list;
        }

        private static PageFieldType Resolve(EntityPropertyMetaModelResponse prop)
        {
            if (!string.IsNullOrEmpty(prop.ForeignKeyEntity))
                return PageFieldType.DropDown;

            var name = prop.PropertyName.ToLowerInvariant();
            var type = prop.PropertyType.ToLowerInvariant();

            if (name.Contains("email")) return PageFieldType.EmailAddress;
            if (name.Contains("phone")) return PageFieldType.PhoneNumber;
            if (name.Contains("price") || name.Contains("amount")) return PageFieldType.Currency;
            if (name.Contains("date")) return PageFieldType.Date;
            if (name.Contains("time")) return PageFieldType.DateTime;

            return type switch
            {
                "string" => PageFieldType.TextBox,
                "int" => PageFieldType.Number,
                "long" => PageFieldType.Number,
                "decimal" => PageFieldType.Currency,
                "bool" => PageFieldType.Boolean,
                "datetime" => PageFieldType.DateTime,
                _ => PageFieldType.TextBox
            };
        }
    }
}
