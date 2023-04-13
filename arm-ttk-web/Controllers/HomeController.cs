using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArmValidation.Models;
using System.Diagnostics;

namespace ArmValidation.Controllers
{
    public class HomeController : Controller
    {
        
        [HttpGet]
        public IActionResult Index()
        {
            MultipleFilesModel model = new MultipleFilesModel();
            return View(model);
        }


        [HttpPost]
        public IActionResult Index(MultipleFilesModel model)
        {
            if (ModelState.IsValid)
            {
                bool showSummary = true;
                model.IsResponse = true;
                if (model.Files.Count > 0)
                {
                    foreach (var file in model.Files)
                    {

                        string path = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

                        //create folder if not exist
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);


                        string fileNameWithPath = Path.Combine(path, file.FileName);

                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                    }
                    model.IsSuccess = true;
                    model.Message = "Files upload successfully";
                    var Lines = ValiateFile();
                    model.DetailLines = new List<string>();
                    model.SummaryLines = new List<string>();
                    string prevLine = "";
                    foreach (var line in Lines)
                    {
                        if (prevLine.Contains("[?]"))
                        {
                            model.SummaryLines.Add("[?] " + line);
                            model.DetailLines.Add("[?] " + line);
                        }
                        else
                        {
                            if (prevLine.Contains("[-]"))
                            {
                                model.SummaryLines.Add("[-] " + line);
                                model.DetailLines.Add("[-] " + line);
                            }
                            else
                            {
                                if ((!showSummary) 
                                    || (line.Contains("[-]")) 
                                    || (line.Contains("[?]")) 
                                    || (line.StartsWith("Pass "))
                                    || (line.StartsWith("Fail "))
                                    || (line.StartsWith("Total "))

                                    )
                                {
                                    model.SummaryLines.Add(line);
                                }
                                model.DetailLines.Add(line);


                            }
                        }
                        prevLine = line;

                    }

                }
                else
                {
                    model.IsSuccess = false;
                    model.Message = "Please select files";
                }
            }

            return View(model);
        }



        private List<string> ValiateFile()
        {
            
            List<string> result = new List<string>();
            var escapedArgs = "/app/arm-ttk/arm-ttk/Test-AzTemplate.sh /app/UploadedFiles";
            string? output = "";
            var psi = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"{escapedArgs}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var proc = new Process
            {
                StartInfo = psi
            };

            proc.Start();



            Task.WaitAll(Task.Run(() =>
            {
                while (!proc.StandardOutput.EndOfStream)
                {
                    result.Add(proc.StandardOutput.ReadLine().Replace("[0m", "").Replace("[35m", "").Replace("\u001b", "").Replace("[32m", "").Replace("[1;33m", "").Replace("[1;31m", ""));
                }
            }), Task.Run(() =>
            {
                while (!proc.StandardError.EndOfStream)
                {
                    result.Add(proc.StandardOutput.ReadLine().Replace("[0m", "").Replace("[35m", "").Replace("\u001b", "").Replace("[32m", "").Replace("[1;33m", "").Replace("[1;31m", ""));
                }
            }));


            proc.WaitForExit();
            return result;

        }

    }
}
