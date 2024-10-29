using System;
using System.Data;
using System.Data.SQLite;
using SQLitePCL;

namespace Lab6
{
    class Program
    {
        private const string ConnectionString = "Data Source=shop.db";

        static void Main(string[] args)
        {
            ClearDatabase();
            EnsureDatabaseCreated();
            SeedDatabase();

            Console.WriteLine("Displaying all products:");
            DisplayProducts();
            CalculateTotalValue();
            CountExpiredProducts();
        }

        private static void EnsureDatabaseCreated()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Quantity INTEGER NOT NULL,
                        Price REAL NOT NULL,
                        ExpirationDays INTEGER NOT NULL
                    );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string checkTableQuery = "SELECT COUNT(*) FROM Products;";
                using (var command = new SQLiteCommand(checkTableQuery, connection))
                {
                    var count = Convert.ToInt32(command.ExecuteScalar());
                    if (count == 0)
                    {
                        string insertQuery = @"
                            INSERT INTO Products (Name, Quantity, Price, ExpirationDays)
                            VALUES
                                ('Apples', 50, 0.5, 0),
                                ('Bread', 30, 1.2, 3),
                                ('Milk', 20, 1.5, 0),
                                ('Eggs', 100, 0.2, 0),
                                ('Butter', 10, 2.5, 10);";
                        using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                        {
                            insertCommand.ExecuteNonQuery();
                        }
                        Console.WriteLine("Database seeded with sample products.");
                    }
                }
            }
        }

        public static void DisplayProducts()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Name, Quantity, Price, ExpirationDays FROM Products;";
                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        int quantity = reader.GetInt32(1);
                        decimal price = reader.GetDecimal(2);
                        int expirationDays = reader.GetInt32(3);

                        Console.WriteLine($"Name: {name}, Quantity: {quantity}, Price: {price}, ExpirationDays: {expirationDays}");
                    }
                }
            }
        }

        private static void ClearDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Products;";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("All rows in the Products table have been deleted.");
                }
            }
        }

        public static void CalculateTotalValue()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sumQuery = "SELECT SUM(Quantity * Price) FROM Products;";
                using (var command = new SQLiteCommand(sumQuery, connection))
                {
                    var totalValue = Convert.ToDouble(command.ExecuteScalar());
                    Console.WriteLine($"Total value of all products in the store: {totalValue}");
                }
            }
        }

        public static void CountExpiredProducts()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string expiredCountQuery = "SELECT SUM(Quantity) FROM Products WHERE ExpirationDays > 0;";
                using (var command = new SQLiteCommand(expiredCountQuery, connection))
                {
                    var expiredProductsCount = command.ExecuteScalar();
                    int count = expiredProductsCount == DBNull.Value ? 0 : Convert.ToInt32(expiredProductsCount);
                    Console.WriteLine($"Total expired products: {count}");
                }
            }
        }

    }
}

