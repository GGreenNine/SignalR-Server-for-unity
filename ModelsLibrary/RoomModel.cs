using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModelsLibrary
{
    public class RoomModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonIgnore] // Ignoring circular reference
        public ICollection<SyncObjectModel> Models { get; set; }
        [JsonIgnore]// Ignoring circular reference
        public ICollection<UserModel> Users { get; set; }

    }
}
