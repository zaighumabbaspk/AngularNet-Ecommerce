using System.ComponentModel.DataAnnotations;

namespace eCommerce.Application.DTOs.Checkouts
{
    public class AddressDto
    {
        [Required]
        [StringLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string AddressLine2 { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{AddressLine1}, {AddressLine2}, {City}, {State} {ZipCode}, {Country}"
                .Replace(", ,", ",").Trim(',', ' ');
        }
    }
}