using BillableTracking.Shared.Enums;

namespace BillableTracking.Shared.Models.Eclipse
{

    public class EclipseInvoiceStatusRecord
    {
        public string TransactionId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string InvoiceReference { get; set; } = string.Empty;
        public string Patient { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public decimal AmountCharged { get; set; }
        public decimal GSTExclusive { get; set; }
        public decimal AmountOutstanding { get; set; }
        public decimal AmountClaimsGap { get; set; }
        public decimal AmountClaimsBenefit { get; set; }
        public string Funder { get; set; } = string.Empty;
        public string MembershipNumber { get; set; } = string.Empty;
        public EclipseInvoiceStatusEnum Status { get; set; }
        public RejectionReason? RejectionReason { get; set; }
    }





    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "transaction_report.csv"; // Update to your CSV file path
            var records = ReadCsvFile(filePath);

            foreach (var record in records)
            {
                Console.WriteLine($"{record.TransactionId} - {record.Status}");
                if (record.RejectionReason != null)
                {
                    Console.WriteLine($"  Claim Code: {record.RejectionReason.ClaimCode}");
                    Console.WriteLine($"  Message: {record.RejectionReason.Message}");
                }
            }
        }

        static List<EclipseInvoiceStatusRecord> ReadCsvFile(string filePath)
        {
            var records = new List<EclipseInvoiceStatusRecord>();

            using (var reader = new StreamReader(filePath))
            {
                // Read and skip the header row
                var headerLine = reader.ReadLine();

                // Read each subsequent row
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        var columns = line.Split(',');

                        var record = new EclipseInvoiceStatusRecord
                        {
                            TransactionId = columns[0].Trim(),
                            Date = DateTime.Parse(columns[1].Trim()),
                            InvoiceReference = columns[2].Trim(),
                            Patient = columns[3].Trim(),
                            Provider = columns[4].Trim(),
                            AmountCharged = ParseDecimal(columns[5]),
                            GSTExclusive = ParseDecimal(columns[6]),
                            AmountOutstanding = ParseDecimal(columns[7]),
                            AmountClaimsGap = ParseDecimal(columns[8]),
                            AmountClaimsBenefit = ParseDecimal(columns[9]),
                            Funder = columns[10].Trim(),
                            MembershipNumber = columns[11].Trim(),
                            Status = ParseStatus(columns[12].Trim()),
                            RejectionReason = columns.Length > 13 ? new RejectionReason(columns[13].Trim()) : null
                        };

                        records.Add(record);
                    }
                }
            }
            return records;
        }

        static EclipseInvoiceStatusEnum ParseStatus(string status)
        {
            return status.ToLower() switch
            {
                "declined" => EclipseInvoiceStatusEnum.Declined,
                "approved" => EclipseInvoiceStatusEnum.Approved,
                "completed" => EclipseInvoiceStatusEnum.Completed,
                "cancelled" => EclipseInvoiceStatusEnum.Cancelled,
                "outstanding" => EclipseInvoiceStatusEnum.Outstanding,
                _ => EclipseInvoiceStatusEnum.NotInvoiced,
            };
        }

        static decimal ParseDecimal(string value)
        {
            // Remove currency symbols and parse as decimal
            value = value.Replace("$", "").Trim();
            return decimal.TryParse(value, out var result) ? result : 0;
        }
    }

}
