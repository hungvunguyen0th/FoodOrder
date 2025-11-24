using ASM_C6.API.Models;
using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        public int Status { get; set; }
    }
}