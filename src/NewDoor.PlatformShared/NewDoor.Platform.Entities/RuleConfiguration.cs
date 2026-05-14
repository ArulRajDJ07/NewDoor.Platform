    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class RuleConfiguration : BaseEntity
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int RuleId { get; set; }

            [Required]
            [MaxLength(50)]
            public string ConfigKey { get; set; } = string.Empty;

            [Required]
            [MaxLength(200)]
            public string ConfigValue { get; set; } = string.Empty;

            [MaxLength(50)]
            public string Unit { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;

            // Navigation property
            public virtual Rule Rule { get; set; } = null!;
        }
    }