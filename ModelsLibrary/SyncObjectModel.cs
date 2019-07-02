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
using Microsoft.Owin.Security;
using Unity;

namespace ModelsLibrary
{
    public class SyncObjectModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ModelId { get; set; }
        /// <summary>
        /// Prefab name in Unity Resources folder.
        /// </summary>
        public string PrefabName { get; set; }

        /// <summary>
        /// Room Id witch user belongs
        /// </summary>
        public string UserName { get; set; }
        public UserModel UserModel { get; set; }

        /// <summary>
        /// Room id the syncObjectModel is related for
        /// </summary>
        public int? RoomModelId { get; set; }
        [JsonIgnore]
        public RoomModel Rooms { get; set; }

        /// <summary>
        /// Model current rotation
        /// Doesn't updating by database
        /// </summary>
        public string ModelPosition;

        /// <summary>
        /// Model current rotation
        /// Doesn't updating by database
        /// </summary>
        public string ModelRotation;

        /// <summary>
        /// Distance in units the object is moving by period of time
        /// Doesn't storing in database
        /// </summary>
        public float Distance;

        /// <summary>
        /// Rotation in eulers angles the object is turning by period of time
        /// Doesn't storing in database
        /// </summary>
        public float Rotation;

    }
}
