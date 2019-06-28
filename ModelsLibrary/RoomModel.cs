using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary
{
    public class RoomModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public ICollection<UserModel> Users { get; set; }
        public ICollection<SyncObjectModel> Models { get; set; }

        public RoomModel()
        {
            this.Users = new HashSet<UserModel>();
            this.Models = new HashSet<SyncObjectModel>();
        }
    }
}
