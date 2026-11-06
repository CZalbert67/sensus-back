using Microsoft.Extensions.Logging;
using Sensus.Application.Common.Interfaces;
using Sensus.Domain.Entities;
using Sensus.Domain.Repositories;

namespace Sensus.Infrastructure.Migration;

/// <summary>
/// Servicio de migración que traslada los datos definidos en scripts SQL hacia MongoDB Atlas,
/// aplicando hashing seguro BCrypt y el modelo de documentos unificado.
/// </summary>
public sealed class SqlToMongoMigrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<SqlToMongoMigrationService> _logger;

    public SqlToMongoMigrationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<SqlToMongoMigrationService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta la migración inicial del usuario semilla provisto en Base_Datos_Movi;.sql.
    /// </summary>
    public async Task MigrateInitialDataAsync()
    {
        const string initialEmail = "oscarsa48700@gmail.com";

        var exists = await _userRepository.ExistsByEmailAsync(initialEmail);
        if (exists)
        {
            _logger.LogInformation("Los datos iniciales de SQL ya se encuentran migrados en MongoDB.");
            return;
        }

        _logger.LogInformation("Iniciando migración de datos desde SQL (Base_Datos_Movi;.sql) hacia MongoDB Atlas...");

        // En el esquema SQL original: registro inicial con rol Consumidor y estado activo.
        var seedPassword = Environment.GetEnvironmentVariable("INITIAL_SEED_PASSWORD") ?? Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        var securePasswordHash = _passwordHasher.HashPassword(seedPassword);

        var migratedUser = User.Create(
            "Oscar Jose Salinas",
            initialEmail,
            securePasswordHash,
            true,
            DateTime.UtcNow).Value;

        await _userRepository.AddAsync(migratedUser);

        _logger.LogInformation(
            "Usuario migrado exitosamente hacia MongoDB Atlas: {Email} con contraseña segura autogenerada y rol Consumidor.",
            initialEmail);
    }
}
