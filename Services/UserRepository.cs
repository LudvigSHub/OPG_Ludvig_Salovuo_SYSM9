using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookMaster.Models;
using CookMaster.Infrastructure;
using Microsoft.Data.Sqlite;

namespace CookMaster.Services
{
    public class UserRepository
    {
        // Ansvar:
        // - Läsa/skriva Users i SQLite (INSERT/SELECT/UPDATE/DELETE)
        // - Översätta mellan DB-rader <-> User-objekt
        // Inte ansvar:
        // - Inloggningsregler, validering, "CurrentUser" (det är UserManager)


        // Tar emot ett User-objekt
        // och sparar det i databasen
        // Lösenordet är i klartext (ska hashas innan sparande i verklig app)
        public void InsertUser(User user, string plainPassword)
        {
            var passwordHash = plainPassword;
            var passwordSalt = string.Empty;

            // Anropar DbHelper för att "öppna dörren" till databasen"
            // using var ser till att anslutningen stängs automatiskt när vi är klara
            using var connection = DbHelper.GetConnection();

            // Skapar ett kommando för att köra SQL mot databasen
            // CreateCommand skapar ett tomt kommando kopplat till vår öppna anslutning
            // därav connection.CreateCommand() 
            // using var ser till att kommandot stängs automatiskt när vi är klara
            using var command = connection.CreateCommand();

            // SQL-fråga för att infoga en ny användare
            // Använder parametrar ($paramName) för att undvika SQL-injektion
            // och för att enkelt kunna sätta in värden
            // VALUES-delen anger vilka värden som ska sättas in i respektive kolumn
            // Exempelvis sätts värdet av $username in i kolumnen Username
            command.CommandText = @"
        INSERT INTO Users
            (Username, PasswordHash, PasswordSalt, Country, SecurityQuestion, SecurityAnswer, IsAdmin)
        VALUES
            ($username, $hash, $salt, $country, $question, $answer, $isAdmin);
    ";

            command.Parameters.AddWithValue("$username", user.Username);
            command.Parameters.AddWithValue("$hash", passwordHash);
            command.Parameters.AddWithValue("$salt", passwordSalt);
            command.Parameters.AddWithValue("$country", user.Country);

            command.Parameters.AddWithValue("$question", user.SecurityQuestion ?? string.Empty);
            command.Parameters.AddWithValue("$answer", user.SecurityAnswer ?? string.Empty);

            // SQLite saknar boolean-typ, så vi använder 1 för true och 0 för false
            command.Parameters.AddWithValue("$isAdmin", user.IsAdmin ? 1 : 0);

            command.ExecuteNonQuery();

        }

        // Hämtar en användare från databasen baserat på användarnamn
        public User? GetByUsername(string username)
        {
            using var connection = DbHelper.GetConnection();

            using var command = connection.CreateCommand();


            // Här används SELECT istället för INSERT
            // för att hämta data från databasen
            // filtrerar på username
            command.CommandText = @"
               SELECT
                   Username,
                   PasswordHash, 
                   PasswordSalt, 
                   Country, 
                   SecurityQuestion, 
                   SecurityAnswer, 
                   IsAdmin
                FROM Users
                WHERE Username = $username;
              ";

            command.Parameters.AddWithValue("$username", username);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            var isAdmin = reader.GetInt32(6) == 1;
            User user = isAdmin ? new Admin() : new User();

            user.Username = reader.GetString(0);
            user.Password = reader.GetString(1); // I verklig app: hantera hash/salt
            user.Country = reader.GetString(3);

            user.SecurityQuestion = reader.IsDBNull(4) ? null : reader.GetString(4);
            user.SecurityAnswer = reader.IsDBNull(5) ? null : reader.GetString(5);

            return user;
        }

        public List<User> GetAll()
        {
            var result = new List<User>();

            using var connection = DbHelper.GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
               SELECT
                   Username,
                   PasswordHash, 
                   PasswordSalt, 
                   Country, 
                   SecurityQuestion, 
                   SecurityAnswer, 
                   IsAdmin
                FROM Users
                ORDER BY Username;
              ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var isAdmin = reader.GetInt32(6) == 1;
                User user = isAdmin ? new Admin() : new User();

                user.Username = reader.GetString(0);

                user.Password = reader.GetString(1); // I verklig app: hantera hash/salt

                user.Country = reader.GetString(3);

                user.SecurityQuestion = reader.IsDBNull(4) ? null : reader.GetString(4);
                user.SecurityAnswer = reader.IsDBNull(5) ? null : reader.GetString(5);

                result.Add(user);
            }
            return result;
        }

    }
}
