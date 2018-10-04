using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.DbModels
{
    public class PersinalIntentData
    {
        public long Id { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Data { get; set; }
        public async Task<IEnumerable<KeyValuePair<string, string>>> ReadData()
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            try
            {
                if (File.Exists($"{Id}.txt"))
                {
                    var lines = await File.ReadAllLinesAsync($"{Id}.txt");
                    foreach (var line in lines)
                    {
                        var parts = line.Split('#');
                        data.Add(new KeyValuePair<string, string>(parts[0], parts[1]));
                    }
                }
                else
                    File.Create($"{Id}.txt");
            }
            catch { }
            return data;
        }
        public async Task WriteData(IEnumerable<KeyValuePair<string, string>> data)
        {
            List<string> lines = new List<string>();
            foreach (var item in data)
                lines.Add($"{item.Key}#{item.Value}");
            await File.AppendAllLinesAsync($"{Id}.txt", lines);
        }
        public async Task WriteData(KeyValuePair<string, string> data)
        {
            List<string> lines = new List<string>
            {
                $"{data.Key}#{data.Value}"
            };
            await File.AppendAllLinesAsync($"{Id}.txt", lines);
        }
    }
}
