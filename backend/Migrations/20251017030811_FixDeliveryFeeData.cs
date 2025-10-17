using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TentRentalSaaS.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixDeliveryFeeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix corrupted delivery fees that were stored as raw distances instead of monetary amounts
            // Any DeliveryFee > 1000 is likely raw distance in meters that was incorrectly stored
            
            migrationBuilder.Sql(@"
                -- Step 1: Add a temporary column to store the original corrupted values for audit
                ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""CorruptedDeliveryFee"" DECIMAL(18,6) NULL;
            ");
            
            migrationBuilder.Sql(@"
                -- Step 2: Back up the corrupted values
                UPDATE ""Bookings"" 
                SET ""CorruptedDeliveryFee"" = ""DeliveryFee""
                WHERE ""DeliveryFee"" > 1000;
            ");
            
            migrationBuilder.Sql(@"
                -- Step 3: Fix the corrupted delivery fees
                -- Assuming corrupted values are meters, convert to miles and apply $2/mile rate
                -- Formula: (meters / 1609.344) * 2.00 with minimum fee of $5.00
                UPDATE ""Bookings"" 
                SET ""DeliveryFee"" = GREATEST(
                    ROUND((""DeliveryFee"" / 1609.344) * 2.00, 2),
                    5.00
                )
                WHERE ""DeliveryFee"" > 1000;
            ");
            
            migrationBuilder.Sql(@"
                -- Step 4: Recalculate TotalPrice for affected bookings
                UPDATE ""Bookings"" 
                SET ""TotalPrice"" = ""RentalFee"" + ""SecurityDeposit"" + ""DeliveryFee""
                WHERE ""CorruptedDeliveryFee"" IS NOT NULL;
            ");
            
            migrationBuilder.Sql(@"
                -- Step 5: Add a check constraint to prevent future corruption
                ALTER TABLE ""Bookings"" 
                ADD CONSTRAINT ""CK_Bookings_DeliveryFee_Reasonable"" 
                CHECK (""DeliveryFee"" >= 0 AND ""DeliveryFee"" <= 1000);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback the migration by restoring corrupted values (for testing purposes only)
            migrationBuilder.Sql(@"
                -- Remove the check constraint
                ALTER TABLE ""Bookings"" DROP CONSTRAINT IF EXISTS ""CK_Bookings_DeliveryFee_Reasonable"";
            ");
            
            migrationBuilder.Sql(@"
                -- Restore the original corrupted values (WARNING: This will restore the bug!)
                UPDATE ""Bookings"" 
                SET ""DeliveryFee"" = ""CorruptedDeliveryFee""
                WHERE ""CorruptedDeliveryFee"" IS NOT NULL;
            ");
            
            migrationBuilder.Sql(@"
                -- Recalculate TotalPrice with the corrupted values
                UPDATE ""Bookings"" 
                SET ""TotalPrice"" = ""RentalFee"" + ""SecurityDeposit"" + ""DeliveryFee""
                WHERE ""CorruptedDeliveryFee"" IS NOT NULL;
            ");
            
            migrationBuilder.Sql(@"
                -- Drop the audit column
                ALTER TABLE ""Bookings"" DROP COLUMN IF EXISTS ""CorruptedDeliveryFee"";
            ");
        }
    }
}
