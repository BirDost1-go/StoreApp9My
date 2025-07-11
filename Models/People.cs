using System.ComponentModel.DataAnnotations;

namespace StoreApp9My.Models
{
    public class People
    {
        public int Id { get; set; } // PRIMARY KEY

        //[Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        private string _phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone
        {
            get => _phone;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("please enter your phone number");
                if (value.Length != 10)
                    throw new ArgumentException("phone number must be 10 (ten) digits");
                if (!value.All(char.IsDigit))
                    throw new ArgumentException("phone number must only contain digits");
                _phone = value;
            }
        }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
