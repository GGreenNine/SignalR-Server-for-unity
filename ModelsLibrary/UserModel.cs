using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls.WebParts;
using Newtonsoft.Json;

namespace ModelsLibrary
{
    public class UserModel : IEquatable<UserModel>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }
        public string connectionId { get; set; }
        [Key]
        public string UserName { get; set; }
        public string Password { get; set; }

        //Room RoomModelId witch user belongs
        public int? RoomModelId { get; set; }
        [JsonIgnore]
        public RoomModel Rooms { get; set; }

        //Collection of user objects
        [JsonIgnore]
        public ICollection<SyncObjectModel> SyncObjectModel { get; set; }

        public UserModel()
        {
            this.SyncObjectModel = new HashSet<SyncObjectModel>();
        }

        public bool Equals(UserModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UserName, other.UserName) && string.Equals(Password, other.Password);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((UserName != null ? UserName.GetHashCode() : 0) * 397) ^ (Password != null ? Password.GetHashCode() : 0);
            }
        }
    }
}
