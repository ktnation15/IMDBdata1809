using IMDBdata1809.Models;
using Microsoft.Data.SqlClient;

namespace IMDBdata1809
{
    public interface IInserter
    {
        void Insert(List<Title> titles, SqlConnection sqlConn, SqlTransaction transAction);
    }
}