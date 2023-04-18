using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArmValidation.Models
{
    public class SingleFileModel : ReponseModel
    {
        [Required(ErrorMessage = "Please enter file name")]
        public string FileName { get; set; }
        [Required(ErrorMessage = "Please select file")]
        public IFormFile File{ get; set; }
        
    }

    public class ReponseModel
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsResponse { get; set; }
    }


    public class MultipleFilesModel : ReponseModel
    {
        
        [Required(ErrorMessage = "Please select files")]
        public List<IFormFile> Files { get; set; }
        public List<string> SummaryLines { get; set; }
        public List<string> DetailLines { get; set; }
    }
}
