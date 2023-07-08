using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WebApi;

namespace WebApi.Services;

public static class VideoConverter
{

    public static void Main(string folderPath, string fileName)
    {


        var probeResult = FFMpegCore.FFProbe.Analyse(Path.Combine(folderPath, fileName));

        int originalHeight = probeResult.PrimaryVideoStream.Height;

        Console.WriteLine("original height:" + originalHeight.ToString());
        string[] resolutions = GetResolutions(originalHeight);

        ConvertVideo(resolutions, folderPath, fileName);
        GenerateMaster(resolutions, folderPath);

    }

    static void CreateDirectory(string path)
    {
        try
        {
            System.IO.Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create directory: {path}");
            Console.WriteLine(ex.Message);
        }
    }


    static void ConvertVideo(string[] resolutions, string folderPath, string fileName)
    {

        foreach (string resolution in resolutions)
        {
            CreateDirectory(Path.Combine(folderPath, resolution));

            try
            {
                Console.WriteLine(fileName);
                string command = $"ffmpeg -i {fileName} -c:a aac -strict experimental -c:v libx264 -vf \"scale=-2:{resolution},setsar=1\" -f hls -hls_list_size 1000000 -hls_time 3 {resolution}/{resolution}_out.m3u8";

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
                {
                    WorkingDirectory = folderPath + "\\"

                };
                Console.WriteLine(folderPath);
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                process.StartInfo = startInfo;
                process.Start();

                // Read the output
                while (!process.StandardOutput.EndOfStream)
                {
                    string output = process.StandardOutput.ReadLine();
                    Console.WriteLine(output);
                }

                process.WaitForExit();
                Console.WriteLine($"Conversion for {resolution} complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to convert video for {resolution}");
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static string[] GetResolutions(int height)
    {

        if (height >= 1080)
        {
            return new string[] { "1080", "720", "480", "360", "240" };
        }
        else if (height >= 720)
        {
            return new string[] { height.ToString(), "480", "360", "240" };
        }
        else if (height >= 480)
        {
            return new string[] { height.ToString(), "360", "240" };
        }
        else if (height >= 360)
        {
            return new string[] { height.ToString(), "240" };
        }
        else if (height >= 240)
        {
            return new string[] { height.ToString() };
        }
        return new string[0];
    }

    private static void GenerateMaster(string[] resolutions, string folderPath)
    {
        Array.Reverse(resolutions);
        var bandWidth = new string[] { "700000", "1000000", "2000000", "3500000", "7000000" };
        using (StreamWriter writer = new StreamWriter(folderPath + "\\" + "master.m3u8"))
        {
            writer.Write("#EXTM3U\n");
            for (int i = 0; i < resolutions.Length; i++)
            {
                writer.Write($"#EXT-X-STREAM-INF:PROGRAM-ID=1, BANDWIDTH={bandWidth[i]}\n{resolutions[i]}/{resolutions[i]}_out.m3u8\n");
            }
        }
    }
}