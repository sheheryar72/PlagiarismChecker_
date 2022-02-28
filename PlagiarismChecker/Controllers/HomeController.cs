using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlagiarismChecker.Models;
using System.Diagnostics;

namespace PlagiarismChecker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        string[] AllowedExtensions = new string[] { "txt", "doc" };
        string currentPath = "";
        Dictionary<string, string> FileMapping;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            currentPath = environment.WebRootPath;
            FileMapping = new Dictionary<string, string>();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public object? GetPlagiarism()
        {
            int i = 0;
            var file = Request.Form.Files;

            string uploadFilePath = Path.Combine(currentPath, "Processes");

            foreach (var eachFile in file)
            {
                var filename = eachFile.FileName.Split('.');
                if (eachFile.Length > 0 && AllowedExtensions.Contains(filename[^1]))
                {
                    var genFileName = i.ToString() + "." + filename[^1];
                    FileMapping[genFileName] = eachFile.FileName;
                    SaveFileToDisk(uploadFilePath, eachFile, genFileName);
                    i++;
                }
            }
            var returnedString = FindPlagiarism();

            var returnedObject = JsonConvert.DeserializeObject<List<StudentResult>>(returnedString);
            DeleteFileFromDisk(uploadFilePath, FileMapping.Keys);

            returnedObject = returnedObject?.Select(x => new StudentResult(FileMapping[x.firstDoc], FileMapping[x.secondDoc], x.Score)).ToList();
            return returnedObject;
        }

        private static void DeleteFileFromDisk(string Directory, ICollection<string> files)
        {
            foreach(var eachFile in files)
            {
                var path = Path.Combine(Directory, eachFile);
                if ((System.IO.File.Exists(path)))
                {
                    System.IO.File.Delete(path);
                }
            }
        }

        private static void SaveFileToDisk(string uploadFilePath, IFormFile eachFile, string? fileUpdatedName)
        {
            string filePath = Path.Combine(uploadFilePath, fileUpdatedName ?? eachFile.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                eachFile.CopyTo(fileStream);
            }
        }

        private string FindPlagiarism()
        {
            var PythonFilePath = "\"" + Path.Combine(currentPath, @"Processes\app.py") + "\"";
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = @"C:\Python310\python.exe",
                Arguments = string.Format("{0}", PythonFilePath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                RedirectStandardError = true,
            };
            string result = "";
            string error = "";
            using (var process = Process.Start(start))
            {
                using (var reader = process?.StandardOutput)
                {
                    result = reader?.ReadToEnd() ?? string.Empty;
                }
                using (var reader = process?.StandardError)
                {
                    error = reader?.ReadToEnd() ?? string.Empty;
                }
            }
            return result ?? error;
        }
    }
}