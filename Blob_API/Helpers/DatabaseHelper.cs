using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Blob_API.Helpers
{
    [Obsolete]
    static public class DatabaseHelper
    {
        /// <summary>
        /// Revert the entities values to the values in the database which states are not unchanged.
        /// </summary>
        /// <param name="entitiesToRevert">List of entities which will be checked and reverted if not unchanged</param>
        /// <param name="_context">The database context</param>
        /// <returns>Task</returns>
        [Obsolete]
        public static async Task RevertValues(IEnumerable entitiesToRevert, DbContext _context)
        {
            foreach (var entityToRevert in entitiesToRevert)
            {
                // Revert changes, reload data from db
                if (_context.Entry(entityToRevert).State != EntityState.Unchanged)
                    await _context.Entry(entityToRevert).ReloadAsync();
            }
        }
    }
}
