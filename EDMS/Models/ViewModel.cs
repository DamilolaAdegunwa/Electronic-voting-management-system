using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Text;

namespace EDMS.Models
{
    public class login
    {
        [Required]
        [Display(Name = "User Name")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Your username is invalid!!")]
        public string username { get; set; }
        [Required]
        [Display(Name = "Password")]
        [StringLength(50,MinimumLength = 6,ErrorMessage = "Your password is invalid!!")]
        public string password { get; set; }
    }
    public static class Crypto
    {
        public static string Hash(string value)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        public static bool Validated(string hash, string password)
        {
            var hashCompare = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));

            return hash == hashCompare;
        }
    }
}
namespace EDMS
{
    ///////////////////////////////////////(1)
    [MetadataType(typeof(adminMetadata))]
    public partial class admin
    {
        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(dataType: DataType.Password)]
        [Compare("password", ErrorMessage = "The input must match with the password exactly!!")]
        public string confirmpassword { get; set; }
    }
    public class adminMetadata
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string username { get; set; }
        [Required]
        [DataType(dataType: DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
        public string hash { get; set; }
        public string personId { get; set; }
    }
    //////////////////////////////////////////(2)
    [MetadataType(typeof(activityMetadata))]
    public partial class activity
    {

    }
    public partial class activityMetadata
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }
    }
    //////////////////////////////////////////(3)
    [MetadataType(typeof(auditMetadata))]
    public partial class audit
    {

    }
    public class auditMetadata
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Person")]
        public int personId { get; set; }
        [Required]
        [Display(Name = "Date & Time")]
        [DataType(dataType: DataType.DateTime)]
        public System.DateTime datetime { get; set; }
        [Required]
        [Display(Name = "Activity")]
        public int activityId { get; set; }
        [Required]
        [Display(Name = "Object")]
        public string auditObject { get; set; }
    }
    //////////////////////////////////////////(4)
    [MetadataType(typeof(departmentMetadata))]
    public partial class department
    {
        
    }
    public class departmentMetadata
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Department")]
        public string deptName { get; set; }
    }
    //////////////////////////////////////////(5)
    [MetadataType(typeof(fileMetadata))]
    public partial class file
    {
        [DisplayName("File Image")]
        public string FileImage { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "You have not selected any file!!")]
        public HttpPostedFileBase ImageUpload { get; set; }
        public file()
        {
            FileImage = "~/Files/Images/default.jpg";
        }
    }
    public class fileMetadata
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "File Path")]
        public string filepath { get; set; }
        //[Required]
        [Display(Name = "File Type")]
        public int filetypeId { get; set; }
        [Required]
        [Display(Name = "Person")]
        public int personId { get; set; }
    }
    //////////////////////////////////////////(6)
    [MetadataType(typeof(filetypeMetadata))]
    public partial class filetype
    {

    }
    public class filetypeMetadata
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Person")]
        public string name { get; set; }
    }
    //////////////////////////////////////////(7)
    [MetadataType(typeof(personMetadata))]
    public partial class person
    {
    }
    public class personMetadata
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(50,MinimumLength = 5, ErrorMessage = "Your full name should be between 5 - 50 characters")]
        public string fullname { get; set; }
        [Required]
        [Display(Name = "E-mail")]
        [DataType(dataType: DataType.EmailAddress, ErrorMessage = "This is not a valid Email address!!")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not properly formatted!!")]
        public string email { get; set; }
        [Required]
        [Display(Name = "Role")]
        public int roleId { get; set; }
        [Required]
        [Display(Name = "Department")]
        public int deptId { get; set; }
        public string password { get; set; }
        public string hash { get; set; }
        public System.Guid identifier { get; set; }
    }
    ///////////////////////////////////////////////(8)
    [MetadataType(typeof(roleMetadata))]
    public partial class role
    {

    }
    public partial class roleMetadata
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string name { get; set; }
    }
}