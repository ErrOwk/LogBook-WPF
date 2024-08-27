using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace LogBook_WPF.Models
{
    [Table("QSO")]
    public class QSO
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }

        public DateTime UTCTime { get; set; }
        public string selfCallsign { get; set; }
        public string toCallsign { get; set; }
        public string mode {  get; set; }
        public double freq { get; set; }
        public double? freq_rx { get; set; }
        public string? band { get; set; }
        public string? band_rx { get; set; }
        public string? prop_mode { get; set; }
        public string? satName { get; set; }

        public int selfRST { get; set; }
        public string? selfGrid { get; set; }
        public string? selfWX { get; set; }
        public string? selfRIG { get; set; }
        public string? selfANT { get; set; }
        public double? selfWatt { get; set; }
        public string? selfQTH { get; set; }

        public int toRST { get; set; }
        public string? toGrid { get; set; }
        public string? toWX { get; set; }
        public string? toRIG { get; set; }
        public string? toANT { get; set; }
        public double? toWatt { get; set; }
        public string? toQTH { get; set; }

        public string? RMKS { get; set; }
        public bool? isRcvdQSL { get; set; }
        public bool? isSentQSL { get; set; }
    }
}
