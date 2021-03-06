using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Profile.Shared.Models.Admin;

namespace Profile.Shared.Models.Admin
{

    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PaymentId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;  //Payment DateTime

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime ModifedOn { get; set; } = DateTime.Now;
        public string ProfileUserId { get; set; }
        public ProfileUser ProfileUser { get; set; }
        public string SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        public int Amount { get; set; }
    }
}
