using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace restful_blog.Data
{
    [Table("Blog")]
    public class Blog
    {   
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(50000, ErrorMessage = "Post cannot be longer than 50000 characters")]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }



        public string getCaption()
        {
            int lineLimit = 5;
            int characterLimit = 100;

            int nthLineBreakIndex = getNthIndex(Content, '\n', lineLimit);

            if (nthLineBreakIndex != Content.Length)
            {
                return Content.Substring(0, nthLineBreakIndex) + "...";
            }

            return Content.Length > characterLimit ? Content.Substring(0, characterLimit) + "..." : Content;
        }

        private int getNthIndex(string s, char t, int n)
        {
            return s.TakeWhile(c => (n -= (c == t ? 1 : 0)) > 0).Count();
        }
    }
}
