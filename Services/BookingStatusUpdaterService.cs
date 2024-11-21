using Npgsql;

namespace BookingService.Services
{
    public class BookingStatusUpdaterService : BackgroundService
    {
        private readonly string _connectionString;

        public BookingStatusUpdaterService(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        await connection.OpenAsync(stoppingToken);
                        using (var command = new NpgsqlCommand("SELECT update_all_expired_bookings();", connection))
                        {
                            await command.ExecuteNonQueryAsync(stoppingToken);
                            Console.WriteLine($"[{DateTime.UtcNow}] - Completed updating expired bookings.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.UtcNow}] - Error updating bookings: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
