using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SocialNetwork.Models
{
    [Table("friendship")]
    public class FriendshipDBModel
    {



        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("userId")]
        public string userID { get; set; }
        public UserDBModel User { get; set; }


        [Column("friendId")]
        public string friendID { get; set; }
        public UserDBModel Friend { get; set; }
               

    }



}
