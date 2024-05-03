using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{


    [Table("users")]
    public class UserDBModel
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("first_name")]
        public string First_name { get; set; }

        [Column("second_name")]
        public string Second_name { get; set; }

        [Column("birthdate")]
        public string Birthdate { get; set; }

        [Column("biography")]
        public string Biography { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("passwordhash")]
        public byte[] PasswordHash { get; set; }

        [Column("paswordsalt")]
        public byte[] PaswordSalt { get; set; }
    }

    public class UserModel
    {
        public string Id { get; set; }

        public string First_name { get; set; }

        public string Second_name { get; set; }

        public string Birthdate { get; set; }

        public string Biography { get; set; }

        public string City { get; set; }        
    }



    public class UserModelRegisterRequestModel : UserModel
    {
        public string Password { get; set; }
    }
}
