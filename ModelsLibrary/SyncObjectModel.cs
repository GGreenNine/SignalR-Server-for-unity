using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
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
        public string ModelId { get; set; }
        //Prefab name in Unity Resources folder.
        public string PrefabName { get; set; }

        [Required]
        public string UserModelId { get; set; }
        public UserModel User { get; set; }

        //Model current position
        public string ModelPosition;
        //Model current rotation
        public string ModelRotation;
    }
}
