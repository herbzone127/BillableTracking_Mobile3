namespace BillableTracking.Shared.Models.Eclipse
{
    public class RejectionReason
    {
        public string ClaimCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public RejectionReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                ClaimCode = string.Empty;
                Message = string.Empty;
                return;
            }

            // Extract claim code and message
            int colonIndex = reason.IndexOf(":");
            if (colonIndex >= 0)
            {
                // Extract the number after "Claim:"
                var claimPart = reason.Substring(colonIndex + 1).Trim();
                var spaceIndex = claimPart.IndexOf(" ");

                if (spaceIndex >= 0)
                {
                    ClaimCode = claimPart.Substring(0, spaceIndex).Trim(); // Extract number (claim code)
                    Message = claimPart.Substring(spaceIndex + 1).Trim();  // Extract the remaining message
                }
                else
                {
                    // If there's no space, assume everything after ":" is the claim code
                    ClaimCode = claimPart.Trim();
                    Message = string.Empty;
                }
            }
            else
            {
                // If no ":" is found, treat the whole string as the message
                ClaimCode = string.Empty;
                Message = reason.Trim();
            }
        }
    }
}
