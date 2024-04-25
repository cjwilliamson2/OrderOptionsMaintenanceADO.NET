using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace OrderOptionsMaintenance.Models
{
    public class MMABooksDataAccess
    {

        private string ConnectionString => ConfigurationManager.ConnectionStrings["MMABooks"].ConnectionString;

        public OrderOption GetOrderOptions()
        {
            using SqlConnection connection = new(ConnectionString);

            string select = "SELECT OptionID, SalesTaxRate, FirstBookShipCharge, AdditionalBookShipCharge " +
                            "FROM OrderOptions";

            using SqlCommand command = new SqlCommand(select, connection);
            connection.Open();

            OrderOption options = null!;

            using SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow & CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                options = new OrderOption
                {
                    OptionId = (int)reader["OptionID"],
                    SalesTaxRate = (decimal)reader["SalesTaxRate"],
                    FirstBookShipCharge = (decimal)reader["FirstBookShipCharge"],
                    AdditionalBookShipCharge = (decimal)reader["AdditionalBookShipCharge"]
                };
            }

            return options;
        }

        public void SaveOrderOptions(OrderOption options)
        {
            try
            {
                using SqlConnection connection = new(ConnectionString);

                string update = "UPDATE OrderOptions SET " +
                                "SalesTaxRate = @SalesTaxRate, " +
                                "FirstBookShipCharge = @FirstBookShipCharge, " +
                                "AdditionalBookShipCharge = @AdditionalBookShipCharge";

                using SqlCommand command = new(update, connection);

                command.Parameters.AddWithValue("@SalesTaxRate", options.SalesTaxRate);
                command.Parameters.AddWithValue("@FirstBookShipCharge", options.FirstBookShipCharge);
                command.Parameters.AddWithValue("@AdditionalBookShipCharge", options.AdditionalBookShipCharge);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(ex.Message, "Update Error");
            }
        }
    }

    public class DataAccessException : Exception
    {
        public DataAccessException(string msg, string errorType) : base(msg) =>
            ErrorType = errorType;

        public string ErrorType { get; init; }
    }
}
