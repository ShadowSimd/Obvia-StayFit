using Kinect_WPF_Natif.Model.Play;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Kinect_WPF_Natif.Model.Data
{
    public static class JsonHelper
    {

        public static SongSelectItem ChargerFichier(int songId)
        {
            if (!File.Exists($"{Constants.SongDataJsonPath}/{songId}.json"))
                return null;

            StreamReader sr = new StreamReader($"{Constants.SongDataJsonPath}/{songId}.json");
            string jsonContent = sr.ReadToEnd();
            SongSelectItem song = JsonSerializer.Deserialize<SongSelectItem>(jsonContent);

            return song;
        }

        public static void SaveSongJson(SongSelectItem song)
        {
            if (!Directory.Exists(Constants.SongDataJsonPath))
            {
                Directory.CreateDirectory(Constants.SongDataJsonPath);
            }

            string json = JsonSerializer.Serialize(song, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText($"{Constants.SongDataJsonPath}/{song.SongId}.json", json);
        }
    }
}
