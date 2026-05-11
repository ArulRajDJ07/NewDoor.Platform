    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class Rule : BaseEntity
        {
            [Key]
            public int Id { get; set; }

    
        }
    }