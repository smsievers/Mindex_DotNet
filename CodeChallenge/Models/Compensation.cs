using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeChallenge.Models
{
    public class Compensation
        
    {
        public int Id { get; set; }
        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Employee Employee { get; set; }

    }
}
