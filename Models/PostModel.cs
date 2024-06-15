using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SocialNetwork.Models
{

    [Table("posts")]
    public class PostDBModel
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
    
        [Column("text")]
        public string Text { get; set; }


        [Column("userId")]
        public string userID { get; set; }
        [ForeignKey("userID")]
        public UserDBModel? User { get; set; }  // навигационное свойство

    }

    public class PostModel
    {
        public int PostId { get; set; }

        public string PostText { get; set; }
        
        public UserModel User  { get; set; }
    }




}
