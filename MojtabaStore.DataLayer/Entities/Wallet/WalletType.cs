using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojtabaStore.DataLayer.Entities.Wallet
{
    public class WalletType
    {
        public WalletType()
        {

        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(150)]
        public string TypeTitle { get; set; }
        public virtual List<Wallet> Wallets { get; set; }
    }
}
