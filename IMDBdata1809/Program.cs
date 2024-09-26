
using IMDBdata1809;
using IMDBdata1809.Models;
using Microsoft.Data.SqlClient;

IInserter inserter;
Console.WriteLine("Tast 1 for normal\r\nTast 2 for prepared\r\nTast 3 for Bulk");
string input = Console.ReadLine();

switch (input)
{
    case "1":
        inserter = new NormalInserter();
        break;
    case "2":
        inserter = new PreparedInserter();
        break;
    case "3":
        inserter = new BulkInserter();
        break;
    default:
        throw new Exception("Invalid input");
}

inserter = new PreparedInserter();

int lineCount = 0;
List<Title> titles = new List<Title>();
string filePath = "C:/temp/title.basics.tsv";

foreach (string line in File.ReadLines(filePath).Skip(1))
{
    if (lineCount == 50000)
    {
        break;
    }
    string[] splitLine = line.Split('\t');
    if (splitLine.Length != 9)
    {
        throw new Exception("Ikke rigtigt antal tabs! " + line);
    }
    string tconst = splitLine[0];
    string primaryTitle = splitLine[2];
    string originalTitle = splitLine[3];
    // kan den måske være andet end 1 og 0?
    bool isAdult = splitLine[4] == "1";
    int? startYear = ParseInt(splitLine[5]);
    int? endYear = ParseInt(splitLine[6]);
    int? runtimeMinutes = ParseInt(splitLine[7]);

    Title newTitle = new Title() { TConst = tconst, PrimaryTitle = primaryTitle, OriginalTitle = originalTitle, IsAdult = isAdult, StartYear = startYear, EndYear = endYear, RuntimeMinutes = runtimeMinutes };

    titles.Add(newTitle);

    lineCount++;
}

Console.WriteLine("List of titles length " + titles.Count);

SqlConnection sqlConn = new SqlConnection("Server=localhost;database=IMDB;" + 
    "user id=sa;password=2024forlife;TrustServerCertificate=True");

sqlConn.Open();
SqlTransaction transAction = sqlConn.BeginTransaction();

DateTime before = DateTime.Now;

try
{
    //NormalInserter inserter = new NormalInserter();
    inserter.Insert(titles, sqlConn, transAction);
    //transAction.Commit();
    transAction.Rollback();
}
catch (SqlException ex)
{
    Console.WriteLine(ex.Message);
    transAction.Rollback();
}

DateTime after = DateTime.Now;

sqlConn.Close();

Console.WriteLine("milliseconds passed: " + (after - before).TotalMilliseconds);

//SqlConnection sqlConn1 = new SqlConnection("Server=localhost;database=IMDB;" + "integrated security=True;TrustServerCertificate=True");
int? ParseInt(string value)
{
    if (value.ToLower() == "\\n" ) // check if it is \n
    {
        return null;
    }
    return int.Parse(value);
}