using Dapper;
using System.Data;
using QualaApi.Models;
using QualaBackend.Interfaces;

namespace QualaBackend.Repositories
{
    public class SucursalRepository : ISucursalRepository
    {
        private readonly IDbConnection _connection;

        public SucursalRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Sucursal>> GetAllAsync()
        {
            const string sp = "aa_sp_sucursal_obtener_todas";
            return await _connection.QueryAsync<Sucursal>(sp, commandType: CommandType.StoredProcedure);
        }

        public async Task<Sucursal?> GetByIdAsync(int id)
        {
            const string sp = "aa_sp_sucursal_obtener_por_id";
            return await _connection.QueryFirstOrDefaultAsync<Sucursal>(
                sp, new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Sucursal> CreateAsync(SucursalCreateDto sucursal)
        {
            const string sp = "aa_sp_sucursal_crear";
            return await _connection.QueryFirstAsync<Sucursal>(
                sp,
                new
                {
                    sucursal.Codigo,
                    sucursal.Descripcion,
                    sucursal.Direccion,
                    sucursal.Identificacion,
                    sucursal.FechaCreacion,
                    sucursal.MonedaId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Sucursal> UpdateAsync(SucursalUpdateDto sucursal)
        {
            const string sp = "aa_sp_sucursal_actualizar";
            return await _connection.QueryFirstAsync<Sucursal>(
                sp,
                new
                {
                    sucursal.Id,
                    sucursal.Codigo,
                    sucursal.Descripcion,
                    sucursal.Direccion,
                    sucursal.Identificacion,
                    sucursal.MonedaId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sp = "aa_sp_sucursal_eliminar";

            var result = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                sp,
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return result?.Success == 1;
        }

        public async Task<bool> ExistsCodigoAsync(int codigo, int? excludeId = null)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM aa_suc_sucursal
                WHERE Codigo = @Codigo
                AND Activo = 1
                AND (@ExcludeId IS NULL OR Id != @ExcludeId)";

            var count = await _connection.QuerySingleAsync<int>(
                sql,
                new { Codigo = codigo, ExcludeId = excludeId }
            );

            return count > 0;
        }
    }

    public class MonedaRepository : IMonedaRepository
    {
        private readonly IDbConnection _connection;

        public MonedaRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Moneda>> GetAllAsync()
        {
            const string sp = "aa_sp_moneda_obtener_todas";
            return await _connection.QueryAsync<Moneda>(sp, commandType: CommandType.StoredProcedure);
        }

        public async Task<Moneda?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, Codigo, Nombre, Simbolo, Activo 
                FROM aa_mon_moneda 
                WHERE Id = @Id AND Activo = 1";

            return await _connection.QueryFirstOrDefaultAsync<Moneda>(sql, new { Id = id });
        }
    }

    public class UsuarioRepository
    {
        private readonly IDbConnection _connection;

        public UsuarioRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            const string sp = "aa_sp_usuario_validar";
            return await _connection.QueryFirstOrDefaultAsync<Usuario>(
                sp,
                new { NombreUsuario = username },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
