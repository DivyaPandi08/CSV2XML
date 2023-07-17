
using System;
using System.Data;
using System.IO;
using System.Threading;

public static class CsvToXmlConverter
{
    public static void ConvertCsvToXml(string csvFilePath, string xsdFilePath, string outputXmlFilePath)
    {
        // Load the CSV file into a DataTable
        DataTable csvData = new DataTable();
        csvData.ReadXmlSchema(xsdFilePath);
        csvData.ReadCsv(csvFilePath);

        // Save the DataTable as XML
        csvData.WriteXml(outputXmlFilePath);
    }

    private static void ReadCsv(this DataTable dataTable, string csvFilePath)
    {
        using (var reader = new StreamReader(csvFilePath))
        {
            string[] headers = reader.ReadLine()?.Split(',');
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header.Trim());
                }
            }

            while (!reader.EndOfStream)
            {
                string[] rows = reader.ReadLine()?.Split(',');
                if (rows != null)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dataRow[i] = rows[i].Trim();
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
        }
    }
}

public static class FolderWatcher
{
    public static void StartWatching(string folderPath, string xsdFilePath, string outputFolderPath)
    {
        FileSystemWatcher watcher = new FileSystemWatcher(folderPath);
        watcher.Filter = "*.csv";
        watcher.Created += (sender, e) =>
        {
            Thread.Sleep(1000); // Delay to ensure the file is fully written

            string csvFilePath = e.FullPath;
            string csvFileName = Path.GetFileNameWithoutExtension(csvFilePath);
            string outputXmlFilePath = Path.Combine(outputFolderPath, csvFileName + ".xml");

            CsvToXmlConverter.ConvertCsvToXml(csvFilePath, xsdFilePath, outputXmlFilePath);
        };
        watcher.EnableRaisingEvents = true;
    }
}

static void Main(string[] args)
{
    string folderPath = "path/to/csv/folder";
    string xsdFilePath = "path/to/xsd/schema.xsd";
    string outputFolderPath = "path/to/output/folder";

    FolderWatcher.StartWatching(folderPath, xsdFilePath, outputFolderPath);

    Console.WriteLine("CSV to XML conversion started. Watching folder: " + folderPath);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
