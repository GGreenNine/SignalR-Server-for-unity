using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;
using Unity;

namespace ModelsLibrary
{
    public class SyncObjectModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ModelId { get; set; }
        //Prefab name in Unity Resources folder.
        public string PrefabName { get; set; }

        //Room Id witch user belongs
        public int? PlayerId { get; set; }
        public UserModel UserModel { get; set; }

        public int? RoomModelId { get; set; }
        [JsonIgnore]
        public RoomModel Rooms { get; set; }

        //Model current position
        public string ModelPosition { get; set; }

        //Model current rotation
        public string ModelRotation { get; set; }

    }
}
