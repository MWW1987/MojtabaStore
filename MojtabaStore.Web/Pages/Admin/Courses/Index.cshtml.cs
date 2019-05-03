using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MojtabaStore.Core.DTOs.Course;
using MojtabaStore.Core.Services.Interfaces;

namespace MojtabaStore.Web.Pages.Admin.Courses
{
    public class IndexModel : PageModel
    {
        private readonly ICourseService courseService;

        public IndexModel(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        public List<ShowCourseForAdminViewModel> ListCourse { get; set; }

        public void OnGet()
        {
            ListCourse = courseService.GetcoursesForAdmin();
        }
    }
}