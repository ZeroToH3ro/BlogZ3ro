using System.Data;
using Blog.Models.Account;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Blog.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly IConfiguration _configuration;

    public AccountRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dataTable = new DataTable();
        dataTable.Columns.Add(new DataColumn("UserName", typeof(string)));
        dataTable.Columns.Add("NormalizedUsername", typeof(string));
        dataTable.Columns.Add("Email", typeof(string));
        dataTable.Columns.Add("NormalizedEmail", typeof(string));
        dataTable.Columns.Add("Fullname", typeof(string));
        dataTable.Columns.Add("PasswordHash", typeof(string));
        dataTable.Rows.Add(
            user.Username,
            user.NormalizedUsername,
            user.Email,
            user.NormalizedEmail,
            user.FullName,
            user.PasswordHash
        );
        
        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync(cancellationToken);
            
            await connection.ExecuteAsync("Account_Insert", new
            {
                Account = dataTable.AsTableValuedParameter("dbo.AccountType")
            }, commandType: CommandType.StoredProcedure);
        }
        
        return IdentityResult.Success;
    }

    public async Task<ApplicationUserIdentity?> GetByUsernameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        ApplicationUserIdentity? userIdentity;
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync(cancellationToken);

            userIdentity = await connection.QuerySingleOrDefaultAsync<ApplicationUserIdentity>(
                "Account_GetByUsername",
                new
                {
                    NormalizedUsername = normalizedUserName,
                },
                commandType: CommandType.StoredProcedure
            );
            return userIdentity;
        }
    }
}